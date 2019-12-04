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
}