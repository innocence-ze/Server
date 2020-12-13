public class Player
{
    public string id = "";
    public ClientState state;
    public Player(ClientState state)
    {
        this.state = state;
    }

    public float x;
    public float y;
    public float z;
    public float ex;
    public float ey;
    public float ez;

    public int roomId = -1;
    public int camp = -1;

    public int hp = 100;

    public PlayerData data;

    public void Send(MsgBase msg)
    {
        NetManager.Send(state, msg);
    }
}