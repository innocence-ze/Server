//room information
[System.Serializable]
public class RoomInfo
{
	public int id = 0;      //room id
	public int count = 0;   //room player
	public int status = 0;  //0-ready 1-fight
}

//requested room list
public class MsgGetRoomList : MsgBase
{
	public MsgGetRoomList() { msgName = "MsgGetRoomList"; }
	//get from server
	public RoomInfo[] rooms;
}

public class MsgCreateRoom : MsgBase
{
	public MsgCreateRoom() { msgName = "MsgCreateRoom"; }
	//get from server
	public int result = 0;
}

public class MsgEnterRoom : MsgBase
{
	public MsgEnterRoom() { msgName = "MsgEnterRoom"; }
	//send from client
	public int id = 0;
	//get from server
	public int result = 0;
}


//player information
[System.Serializable]
public class PlayerInfo
{
	public string id = "lpy";   //id
	public int camp = 0;        //camp
	public int isOwner = 0;     //whether the homewoner
}


public class MsgGetRoomInfo : MsgBase
{
	public MsgGetRoomInfo() { msgName = "MsgGetRoomInfo"; }
	//get from server
	public PlayerInfo[] players;
}

public class MsgLeaveRoom : MsgBase
{
	public MsgLeaveRoom() { msgName = "MsgLeaveRoom"; }
	//get from server
	public int result = 0;
}

public class MsgStartBattle : MsgBase
{
	public MsgStartBattle() { msgName = "MsgStartBattle"; }
	//get from server
	public int result = 0;
}