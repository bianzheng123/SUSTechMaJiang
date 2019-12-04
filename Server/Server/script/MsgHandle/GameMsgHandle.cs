using System;


public partial class MsgHandler
{
    //用于开战的协议，代表是否可以开战
    public static void MsgFaPai(ClientState c, MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        msg = gameManager.ProcessMsgFaPai(msg);
        //广播
        gameManager.Broadcast(msg);
    }

    /// <summary>
    /// 服务端接收到协议，直接广播至这个房间内的所有玩家即可
    /// </summary>
    /// <param name="msgBase"></param>
    public static void MsgChat(ClientState c, MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.Broadcast(msg);
    }

    public static void MsgChuPai(ClientState c,MsgBase msgBase)
    {
        MsgChuPai msg = (MsgChuPai)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        
        int paiIndex = msg.paiIndex;//牌在这个玩家的索引
        int id = msg.id;//出牌的玩家id
        gameManager.Broadcast(msg);//对客户端广播出牌协议，对出牌进行同步

        if (paiIndex == -1)
        {
            //服务端执行胡的操作，清空数据，写入数据库等
            return;
        }

        gameManager.ProcessMsgChuPai(paiIndex,id);
    }

    public static void MsgChiPengGang(ClientState c,MsgBase msgBase)
    {
        MsgChiPengGang msg = (MsgChiPengGang)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.Broadcast(msg);//对得到的本轮结果进行广播

        gameManager.ProcessChiPengGang(msg);
        
    }
}