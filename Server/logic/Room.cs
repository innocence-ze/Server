using System;
using System.Collections.Generic;

public class Room
{
	public int id = 0;
	public readonly int maxPlayer = 6;
	public Dictionary<string, bool> playerDic = new Dictionary<string, bool>(6);
	public string ownerId = "";

	public enum Status
	{
		READY = 0,
		FIGHT = 1,
	}
	public Status status = Status.READY;


	public bool AddPlayer(string id)
	{
		Player player = PlayerManager.GetPlayer(id);
		if (player == null)
		{
			Console.WriteLine("room.AddPlayer fail, player is null");
			return false;
		}
		if (playerDic.Count >= maxPlayer)
		{
			Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
			return false;
		}
		if (status != Status.READY)
		{
			Console.WriteLine("room.AddPlayer fail, not PREPARE");
			return false;
		}
		if (playerDic.ContainsKey(id))
		{
			Console.WriteLine("room.AddPlayer fail, already in this room");
			return false;
		}

		playerDic[id] = true;
		player.camp = SwitchCamp();
		player.roomId = this.id;
		if (ownerId == "")
		{
			ownerId = player.id;
		}

		Broadcast(ToMsg());
		return true;
	}

	public int SwitchCamp()
	{
		int count1 = 0;
		int count2 = 0;
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			if (player.camp == 1) { count1++; }
			if (player.camp == 2) { count2++; }
		}
		if (count1 <= count2)
		{
			return 1;
		}
		else
		{
			return 2;
		}
	}

	public bool IsOwner(Player player)
	{
		return player.id == ownerId;
	}


	public bool RemovePlayer(string id)
	{
		Player player = PlayerManager.GetPlayer(id);
		if (player == null)
		{
			Console.WriteLine("room.RemovePlayer fail, player is null");
			return false;
		}
		if (!playerDic.ContainsKey(id))
		{
			Console.WriteLine("room.RemovePlayer fail, not in this room");
			return false;
		}

		playerDic.Remove(id);
		player.camp = 0;
		player.roomId = -1;
		if (ownerId == player.id)
		{
			ownerId = SwitchOwner();
		}
		if (playerDic.Count == 0)
		{
			RoomManager.RemoveRoom(this.id);
		}

		Broadcast(ToMsg());
		return true;
	}

	//choose room owner
	public string SwitchOwner()
	{
		foreach (string id in playerDic.Keys)
		{
			return id;
		}
		return "";
	}


	//broadcast message to all players in the room
	public void Broadcast(MsgBase msg)
	{
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			player.Send(msg);
		}
	}

	//generate MsgGetRoomInfo protocol
	public MsgBase ToMsg()
	{
		MsgGetRoomInfo msg = new MsgGetRoomInfo();
		int count = playerDic.Count;
		msg.players = new PlayerInfo[count];
		//players
		int i = 0;
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
            PlayerInfo playerInfo = new PlayerInfo
            {
                id = player.id,
                camp = player.camp,
                isOwner = 0
            };

            if (IsOwner(player))
			{
				playerInfo.isOwner = 1;
			}

			msg.players[i] = playerInfo;
			i++;
		}
		return msg;
	}
}

