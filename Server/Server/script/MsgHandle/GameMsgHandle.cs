using System;


public partial class MsgHandler
{
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
        MsgChat msg = (MsgChat)msgBase;
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

        gameManager.ProcessMsgChuPai(msg);
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
        gameManager.ProcessChiPengGang(msg);
        
    }

    public static void MsgChemistry(ClientState c,MsgBase msgBase)
    {
        MsgChemistry msg = (MsgChemistry)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.ProcessChemistry(msg);
    }

    public static void MsgMath(ClientState c, MsgBase msgBase)
    {
        MsgMath msg = (MsgMath)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.ProcessMath(msg);
    }

    public static void MsgComputerScience(ClientState c, MsgBase msgBase)
    {
        MsgComputerScience msg = (MsgComputerScience)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.ProcessComputerScience(msg);
    }

    public static void MsgQuit(ClientState c,MsgBase msgBase)
    {
        MsgQuit msg = (MsgQuit)msgBase;
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null) return;
        GameManager gameManager = room.gameManager;
        if (gameManager == null) return;
        gameManager.ProcessMsgQuit(msg);
    }
}