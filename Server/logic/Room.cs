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

	long lastJudgeTime = 0;
	static readonly float[,,] birthConfig = new float[2, 3, 6]
	{
		//Camp 1 birth config
        {
			{-85.8f, 3.8f, -33.8f, 0, 24.9f, 0f},
			{-49.9f, 3.8f, -61.4f, 0, 21.4f, 0f},
			{-6.2f,  3.8f, -70.7f, 0, 21.9f, 0f},
        },
		//Camp 2 birth config
        {
			{150f, 0f, 178.9f, 0, -156.8f, 0f},
			{105f, 0f, 216.5f, 0, -156.8f, 0f},
			{52.0f,0f, 239.2f, 0, -156.8f, 0f},
        },
	};


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
		if(status == Status.FIGHT)
        {
			MsgLeaveBattle msg = new MsgLeaveBattle()
			{
				id = player.id,
			};
			Broadcast(msg);
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

	bool CanStartBattle()
    {
		if(status != Status.READY)
        {
			return false;
        }

		int count1 = 0;
		int count2 = 0;
		foreach(var id in playerDic.Keys)
        {
			Player player = PlayerManager.GetPlayer(id);
			if (player.camp == 1) count1++;
			else if (player.camp == 2) count2++;
        }

		if(count1 < 1 || count2 < 1)
        {
			return false;
        }
		return true;
    }

	void SetBirthPos(Player p, int index)
    {
		int camp = p.camp;

		p.x = birthConfig[camp - 1, index, 0];
		p.y = birthConfig[camp - 1, index, 1];
		p.z = birthConfig[camp - 1, index, 2];
		p.ex = birthConfig[camp - 1, index, 3];
		p.ey = birthConfig[camp - 1, index, 4];
		p.ez = birthConfig[camp - 1, index, 5];
    }

	//trans player info to tank info
	TankInfo PlayerToTankInfo(Player p)
    {
		TankInfo t = new TankInfo()
		{
			camp = p.camp,
			id = p.id,
			hp = p.hp,

			x = p.x, y = p.y, z = p.z,
			ex = p.ex, ey = p.ey, ez = p.ez,
		};
		return t;
    }

	private void ResetPlayers()
	{
		int count1 = 0;
		int count2 = 0;
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			if (player.camp == 1)
			{
				SetBirthPos(player, count1);
				count1++;
			}
			else
			{
				SetBirthPos(player, count2);
				count2++;
			}
		}

		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			player.hp = 100;
		}
	}

	public bool StartBattle()
	{
		if (!CanStartBattle())
		{
			return false;
		}

		status = Status.FIGHT;
		ResetPlayers();

        MsgEnterBattle msg = new MsgEnterBattle
        {
            mapId = 1,
            tanks = new TankInfo[playerDic.Count]
        };

        int i = 0;
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			msg.tanks[i] = PlayerToTankInfo(player);
			i++;
		}
		Broadcast(msg);
		return true;
	}


	public bool IsDie(Player player)
	{
		return player.hp <= 0;
	}


	public void Update()
	{
		if (status != Status.FIGHT)
		{
			return;
		}
		if (NetManager.GetTimeStamp() - lastJudgeTime < 10f)
		{
			return;
		}

		lastJudgeTime = NetManager.GetTimeStamp();
		int winCamp = Judgment();
		if (winCamp == 0)
		{
			return;
		}

		status = Status.READY;

        MsgBattleResult msg = new MsgBattleResult
        {
            winCamp = winCamp
        };
        Broadcast(msg);
	}


	public int Judgment()
	{

		int count1 = 0;
		int count2 = 0;
		foreach (string id in playerDic.Keys)
		{
			Player player = PlayerManager.GetPlayer(id);
			if (!IsDie(player))
			{
				if (player.camp == 1) { count1++; };
				if (player.camp == 2) { count2++; };
			}
		}

		if (count1 <= 0)
		{
			return 2;
		}
		else if (count2 <= 0)
		{
			return 1;
		}
		return 0;
	}

}

