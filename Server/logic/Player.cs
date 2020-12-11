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

    public void Send(MsgBase msg)
    {
        NetManager.Send(state, msg);
    }
}