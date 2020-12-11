using System.Collections.Generic;

public class PlayerManager
{
    static Dictionary<string, Player> playerDic = new Dictionary<string, Player>();

    public static bool IsOnline(string id)
    {
        return playerDic.ContainsKey(id);
    }

    public static Player GetPlayer(string id)
    {
        return playerDic[id];
    }

    public static void AddPlayer(string id, Player player)
    {
        playerDic.Add(id, player);
    }

    public static void RemovePlayer(string id)
    {
        playerDic.Remove(id);
    }

}