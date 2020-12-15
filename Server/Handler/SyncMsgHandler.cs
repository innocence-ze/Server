using System;

public partial class MsgHandler
{
    public static void MsgSyncTank(ClientState c, MsgBase msgBase)
    {
        MsgSyncTank msg = msgBase as MsgSyncTank;
        Player player = c.player;
        if (player == null)
        {
            return;
        }

        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null || room.status != Room.Status.FIGHT)
        {
            return;
        }

        player.x = msg.x;
        player.y = msg.y;
        player.z = msg.z;
        player.ex = msg.ex;
        player.ey = msg.ey;
        player.ez = msg.ez;

        msg.id = player.id;
        room.Broadcast(msg);
    }

    public static void MsgFire(ClientState c, MsgBase msgBase)
    {
        MsgFire msg = msgBase as MsgFire;
        Player player = c.player;
        if (player == null)
        {
            return;
        }

        Room room = RoomManager.GetRoom(player.roomId);

        if (room == null || room.status != Room.Status.FIGHT)
        {
            return;
        }

        msg.id = player.id;
        room.Broadcast(msg);
    }

    public static void MsgHit(ClientState c, MsgBase msgBase)
    {
        MsgHit msg = msgBase as MsgHit;
        Player player = c.player;

        Player targetPlayer = PlayerManager.GetPlayer(msg.targetId);
        if (player == null || targetPlayer == null)
        {
            return;
        }


        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null || room.status != Room.Status.FIGHT)
        {
            return;
        }

        if(player.id != msg.id)
        {
            return;
        }

        int damage = 35;
        targetPlayer.hp -= damage;
        msg.id = player.id;
        msg.hp = targetPlayer.hp;
        msg.damage = damage;
        room.Broadcast(msg);
    }

}