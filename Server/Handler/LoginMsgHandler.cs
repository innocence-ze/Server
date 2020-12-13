using System;


public partial class MsgHandler
{
    //TODO
    // add more player property
    public static void MsgLogin(ClientState c, MsgBase msg)
    {
        LoginMsg m = msg as LoginMsg;
        if (c.player != null || PlayerManager.IsOnline(m.id))
        {
            m.result = 1;
            NetManager.Send(c, m);
            return; 
        }
        Player p = new Player(c)
        {
            id = m.id,
            data = new PlayerData(),
        };

        c.player = p;
        m.result = 0;

        PlayerManager.AddPlayer(c.player.id, c.player);
        p.Send(m);

    }
}