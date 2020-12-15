
[System.Serializable]
public class TankInfo
{
    public string id = "";
    public int camp = 0;
    public int hp = 0;

    //position
    public float x = 0;
    public float y = 0;
    public float z = 0;
    //rotation
    public float ex = 0;
    public float ey = 0;
    public float ez = 0;
}

public class MsgEnterBattle : MsgBase
{
    public MsgEnterBattle() { msgName = "MsgEnterBattle"; }

    //get from server
    public TankInfo[] tanks;
    public int mapId = 1;//if we have more maps
}

public class MsgBattleResult : MsgBase
{
    public MsgBattleResult() { msgName = "MsgBattleResult"; }

    public int winCamp = 0;
}

public class MsgLeaveBattle : MsgBase
{
    public MsgLeaveBattle() { msgName = "MsgLeaveBattle"; }
    public string id = "";
}