using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Threading;

public static class NetManager
{
    static string udpString = "Tank";

    struct UdpState
    {
        public UdpClient udp;
        public IPEndPoint endIp;
    }


    public static Socket listenfd;
    public static Dictionary<Socket, ClientState> clientDic = new Dictionary<Socket, ClientState>();
    static List<Socket> checkRead = new List<Socket>();

    public static long pingInterval = 30;


    static UdpState udpState;

    public static Thread udpReceiveThread;
    public static void StartLoop(int listenPort)
    {
        UdpClient u = new UdpClient(7777);
        IPEndPoint e = new IPEndPoint(IPAddress.Broadcast, 6666);
        udpState = new UdpState
        {
            udp = u,
            endIp = e
        };

        udpReceiveThread = new Thread(UdpReceiveThread);
        udpReceiveThread.Start();


        listenfd = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //bind
        IPHostEntry ipEntry = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var item in ipEntry.AddressList)
        {
            if (item.AddressFamily == AddressFamily.InterNetwork)
            {
                IPAddress ipAdr = item;
                IPEndPoint ipEp = new IPEndPoint(ipAdr, listenPort);
                listenfd.Bind(ipEp);
                Console.WriteLine($"[server] Bind to {ipEp.Address}");
                break;
            }
        }

        //listen
        listenfd.Listen(0);
        Console.WriteLine("[server] start up");

        while (true)
        {
            ResetCheckRead();
            Socket.Select(checkRead, null, null, 1000);
            for (int i = checkRead.Count - 1; i >= 0; i--)
            {
                Socket s = checkRead[i];
                if (s == listenfd)
                {
                    ReadListenfd(s);
                }
                else
                {
                    ReadClientfd(s);
                }
            }
            Timer();
        }

    }

    static void UdpReceiveCallback(IAsyncResult ar)
    {
        UdpClient u = ((UdpState)(ar.AsyncState)).udp;
        IPEndPoint e = ((UdpState)(ar.AsyncState)).endIp;
        IPEndPoint tempE = new IPEndPoint(IPAddress.Any, 6666);

        udpState.udp.BeginReceive(UdpReceiveCallback, udpState);
    }

    static void UdpReceiveThread()
    {
        while (true)
        {
            IPEndPoint tempE = new IPEndPoint(IPAddress.Any, 6666);
            byte[] receiveBytes = udpState.udp.Receive(ref tempE);
            // block ===============
            string receiveString = System.Text.Encoding.ASCII.GetString(receiveBytes);
            if (receiveString == udpString)
            {
                var udpBytes = System.Text.Encoding.UTF8.GetBytes(udpString);
                udpState.udp.Send(udpBytes, udpBytes.Length, tempE);
                Console.WriteLine($"number of bytes sent:{udpBytes.Length}");
            }
        }
    }


    private static void ResetCheckRead()
    {
        checkRead.Clear();
        checkRead.Add(listenfd);
        foreach (var s in clientDic.Values)
        {
            checkRead.Add(s.socket);
        }
    }

    private static void ReadListenfd(Socket listenfd)
    {
        try
        {
            Socket clientfd = listenfd.Accept();
            Console.WriteLine("Accept " + clientfd.RemoteEndPoint.ToString());
            ClientState state = new ClientState()
            {
                socket = clientfd,
                lastPingTime = GetTimeStamp()
            };
            clientDic.Add(clientfd, state);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Accept fail " + ex.ToString());
        }
    }

    private static void ReadClientfd(Socket clientfd)
    {
        ClientState state = clientDic[clientfd];
        ByteArray readBuffer = state.readBuffer;

        //receive
        int count = 0;

        if (readBuffer.Remain <= 0)
        {
            OnReceivingData(state);
            readBuffer.Move();
        }
        if (readBuffer.Remain <= 0)
        {
            Console.WriteLine("Receive fail, maybe message's length is larger than buffer's capacity");
            Close(state);
            return;
        }

        try
        {
            count = clientfd.Receive(readBuffer.bytes, readBuffer.writeIndex, readBuffer.Remain, 0);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Receive socketException: " + ex.ToString());
            Close(state);
            return;
        }

        if (count <= 0)
        {
            Console.WriteLine("Socket close " + clientfd.RemoteEndPoint.ToString());
            Close(state);
            return;
        }
        readBuffer.writeIndex += count;
        OnReceivingData(state);
        readBuffer.CheckAndMove();

    }

    static void OnReceivingData(ClientState state)
    {
        ByteArray readBuffer = state.readBuffer;

        if (readBuffer.Length <= 2)
        {
            return;
        }
        Int16 bodyLength = (Int16)((readBuffer.bytes[readBuffer.readIndex + 1] << 8) | readBuffer.bytes[readBuffer.readIndex]);
        if (readBuffer.Length < bodyLength + 2)
        {
            return;
        }
        readBuffer.readIndex += 2;

        string msgName = MsgBase.DecodeName(readBuffer.bytes, readBuffer.readIndex, out int nameCount);
        if (msgName == "")
        {
            Console.WriteLine("On receiving data, Decode name fail");
            Close(state);
            return;
        }

        readBuffer.readIndex += nameCount;

        int bodyCount = bodyLength - nameCount;
        MsgBase msg = MsgBase.Decode(msgName, readBuffer.bytes, readBuffer.readIndex, bodyCount);
        readBuffer.readIndex += bodyCount;
        readBuffer.CheckAndMove();
        // have read all data, need to distribute message
        MethodInfo mi = typeof(MsgHandler).GetMethod(msgName);
        object[] o = { state, msg };
        Console.WriteLine("Receive " + msgName + " from: " + state.socket.RemoteEndPoint.ToString());
        if (mi != null)
        {
            mi.Invoke(null, o);
        }
        else
        {
            Console.WriteLine("OnReceivingData Invoke fail " + msgName);
        }
        if (readBuffer.Length > 2)
        {
            OnReceivingData(state);
        }
    }

    public static void Send(ClientState state, MsgBase msg)
    {
        if (state == null || !state.socket.Connected)
        {
            return;
        }

        byte[] nameBytes = MsgBase.EncodeName(msg);
        byte[] bodyBytes = MsgBase.Encode(msg);
        int length = nameBytes.Length + bodyBytes.Length;

        byte[] sendBytes = new byte[length + 2];
        sendBytes[0] = (byte)(length % 256);
        sendBytes[1] = (byte)(length / 256);

        Array.Copy(nameBytes, 0, sendBytes, 2, nameBytes.Length);
        Array.Copy(bodyBytes, 0, sendBytes, 2 + nameBytes.Length, bodyBytes.Length);

        try
        {
            Console.WriteLine("Send { " + msg.msgName + " } to " + state.socket.RemoteEndPoint);
            state.socket.BeginSend(sendBytes, 0, sendBytes.Length, 0, null, null);
        }
        catch (SocketException ex)
        {
            Console.WriteLine("Socket send message error " + ex.ToString());
        }

    }

    public static void Close(ClientState state)
    {
        MethodInfo mi = typeof(EventHandler).GetMethod("OnDisconnect");
        object[] o = { state };
        mi.Invoke(null, o);

        state.socket.Close();
        clientDic.Remove(state.socket);
    }

    public static void Timer()
    {
        MethodInfo mi = typeof(EventHandler).GetMethod("OnTimer");
        object[] o = { };
        mi.Invoke(null, o);
    }

    public static long GetTimeStamp()
    {
        TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
        return Convert.ToInt64(ts.TotalSeconds);
    }



}