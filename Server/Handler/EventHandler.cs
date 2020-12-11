﻿using System;

public partial class EventHandler
{
	public static void OnDisconnect(ClientState c)
	{
		Console.WriteLine("Close");
		if (c.player != null)
		{
			PlayerManager.RemovePlayer(c.player.id);
		}
	}


	public static void OnTimer()
	{
		CheckPing();
	}

	//Ping Check
	public static void CheckPing()
	{
		long timeNow = NetManager.GetTimeStamp();
		foreach (ClientState s in NetManager.clientDic.Values)
		{
			if (timeNow - s.lastPingTime > NetManager.pingInterval * 4)
			{
				Console.WriteLine("Ping Close " + s.socket.RemoteEndPoint.ToString());
				NetManager.Close(s);
				return;
			}
		}
	}


}
