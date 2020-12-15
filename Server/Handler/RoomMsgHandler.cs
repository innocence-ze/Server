public partial class MsgHandler
{
	public static void MsgGetRoomList(ClientState c, MsgBase msgBase)
	{
		MsgGetRoomList msg = (MsgGetRoomList)msgBase;
		Player player = c.player;
		if (player == null) return;

		player.Send(RoomManager.ToMsg());
	}

	public static void MsgCreateRoom(ClientState c, MsgBase msgBase)
	{
		MsgCreateRoom msg = (MsgCreateRoom)msgBase;
		Player player = c.player;
		if (player == null) return;
		if (player.roomId >= 0)
		{
			msg.result = 1;
			player.Send(msg);
			return;
		}

		Room room = RoomManager.AddRoom();
		room.AddPlayer(player.id);

		msg.result = 0;
		player.Send(msg);
	}

	public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
	{
		MsgEnterRoom msg = (MsgEnterRoom)msgBase;
		Player player = c.player;
		if (player == null) return;
		if (player.roomId >= 0)
		{
			msg.result = 1;
			player.Send(msg);
			return;
		}
		Room room = RoomManager.GetRoom(msg.id);
		if (room == null)
		{
			msg.result = 1;
			player.Send(msg);
			return;
		}
		if (!room.AddPlayer(player.id))
		{
			msg.result = 1;
			player.Send(msg);
			return;
		}
		msg.result = 0;
		player.Send(msg);
	}

	public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase)
	{
		MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
		Player player = c.player;
		if (player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if (room == null)
		{
			player.Send(msg);
			return;
		}

		player.Send(room.ToMsg());
	}

	public static void MsgLeaveRoom(ClientState c, MsgBase msgBase)
	{
		MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
		Player player = c.player;
		if (player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if (room == null)
		{
			msg.result = 1;
			player.Send(msg);
			return;
		}

		room.RemovePlayer(player.id);
		player.roomId = -1;
		msg.result = 0;
		player.Send(msg);
	}

	public static void MsgStartBattle(ClientState c, MsgBase msgBase)
    {
		MsgStartBattle msg = msgBase as MsgStartBattle;
		Player player = c.player;
		if(player == null)
        {
			return;
        }

        Room room = RoomManager.GetRoom(player.roomId);
		if(room == null)
        {
			msg.result = 1;
			player.Send(msg);
			return;
        }

        if (!room.IsOwner(player))
        {
			msg.result = 1;
			player.Send(msg);
			return;
        }

        if (!room.StartBattle())
        {
			msg.result = 1;
			player.Send(msg);
			return;
        }
        else
        {
			msg.result = 0;
			player.Send(msg);
        }


    }

}