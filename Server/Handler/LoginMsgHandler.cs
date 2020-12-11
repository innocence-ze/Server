using System;


public partial class MsgHandler
{
    //TODO
    // add more player property
    public static void MsgLogin(ClientState c, MsgBase msg)
    {
        LoginMsg m = msg as LoginMsg;
        m.id += (" " + c.socket.RemoteEndPoint);
        Player p = new Player(c)
        {
            id = m.id
        };

        c.player = p;

        PlayerManager.AddPlayer(c.player.id, c.player);

    }
}