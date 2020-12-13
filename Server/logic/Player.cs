public class Player
{
    public string id = "";
    public ClientState state;
    public Player(ClientState state)
    {
        this.state = state;
    }

    public int x;
    public int y;
    public int z;

    public int roomId = -1;
    public int camp = -1;

    public int hp = 100;

    public void Send(MsgBase msg)
    {
        NetManager.Send(state, msg);
    }
}