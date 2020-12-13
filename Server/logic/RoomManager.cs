
using System.Collections.Generic;

class RoomManager
{
	private static int maxId = 0;
	public static Dictionary<int, Room> roomDic = new Dictionary<int, Room>();

	
	public static Room AddRoom()
	{
		maxId++;
        Room room = new Room
        {
            id = maxId
        };
        roomDic.Add(room.id, room);
		return room;
	}

	public static bool RemoveRoom(int id)
	{
		return roomDic.Remove(id);
	}

	public static Room GetRoom(int id)
	{
		if (roomDic.ContainsKey(id))
		{
			return roomDic[id];
		}
		return null;
	}

	//generate MsgGetRoomList protocol
	public static MsgBase ToMsg()
	{
		MsgGetRoomList msg = new MsgGetRoomList();
		int count = roomDic.Count;
		msg.rooms = new RoomInfo[count];
		//rooms
		int i = 0;
		foreach (Room room in roomDic.Values)
		{
            RoomInfo roomInfo = new RoomInfo
            {
                id = room.id,
                count = room.playerDic.Count,
                status = (int)room.status
            };

            msg.rooms[i] = roomInfo;
			i++;
		}
		return msg;
	}

	public static void Update()
    {
		foreach(Room room in roomDic.Values)
        {
			room.Update();
        }
    }
}