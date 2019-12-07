using System;


public partial class MsgHandler {
	
	//进入RoomListPanel时查询自己的战绩
	public static void MsgGetAchieve(ClientState c, MsgBase msgBase){
		MsgGetAchieve msg = (MsgGetAchieve)msgBase;
		Player player = c.player;
		if(player == null) return;

		msg.win = player.data.win;
		msg.lost = player.data.lost;
        msg.major = player.data.major;
        msg.coin = player.data.coin;

		player.Send(msg);
	}


	//请求房间列表
	public static void MsgGetRoomList(ClientState c, MsgBase msgBase){
		MsgGetRoomList msg = (MsgGetRoomList)msgBase;
		Player player = c.player;
		if(player == null) return;

		player.Send(RoomManager.ToMsg());
	}

	//创建房间
	public static void MsgCreateRoom(ClientState c, MsgBase msgBase){
		MsgCreateRoom msg = (MsgCreateRoom)msgBase;
		Player player = c.player;
		if(player == null) return;
		//已经在房间里
		if(player.roomId >=0 ){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//创建
		Room room = RoomManager.AddRoom();
		room.AddPlayer(player.id);

		msg.result = 0;
		player.Send(msg);
	}

	//进入房间
	public static void MsgEnterRoom(ClientState c, MsgBase msgBase){
		MsgEnterRoom msg = (MsgEnterRoom)msgBase;
		Player player = c.player;
		if(player == null) return;
		//已经在房间里
		if(player.roomId >=0 ){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//获取房间
		Room room = RoomManager.GetRoom(msg.id);
		if(room == null){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//进入
		if(!room.AddPlayer(player.id)){
			msg.result = 1;
			player.Send(msg);
			return;
		}
		//返回协议	
		msg.result = 0;
		player.Send(msg);
	}

    //获取房间信息
    public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase){
		MsgGetRoomInfo msg = (MsgGetRoomInfo)msgBase;
		Player player = c.player;
		if(player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			player.Send(msg);
			return;
		}

		player.Send(room.ToMsg());
	}

	//离开房间
	public static void MsgLeaveRoom(ClientState c, MsgBase msgBase){
		MsgLeaveRoom msg = (MsgLeaveRoom)msgBase;
		Player player = c.player;
		if(player == null) return;

		Room room = RoomManager.GetRoom(player.roomId);
		if(room == null){
			msg.result = 1;
			player.Send(msg);
			return;
		}

		room.RemovePlayer(player.id);
		//返回协议
		msg.result = 0;
		player.Send(msg);
	}

    //用于开战的协议，代表是否可以开战
    public static void MsgStartBattle(ClientState c, MsgBase msgBase)
    {
        MsgStartBattle msg = (MsgStartBattle)msgBase;
        //检查是否是四个人，用于开战
        Player player = c.player;
        if (player == null) return;
        Room room = RoomManager.GetRoom(player.roomId);
        if (room == null)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        int playerNum = room.playerIds.Count;
        if (playerNum != 4)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }//只需要点击开战的收到开战协议就行了，其余的只需要收到了初始化游戏数据的协议即可
        room.status = Room.Status.FIGHT;

        room.gameManager = new GameManager(room);//对gameManager进行初始化

        MsgInitData msgInit = room.gameManager.GetMsgInitData();
        for (int i = 0; i < 4; i++)
        {
            msgInit.id = i;
            room.gameManager.players[i].Send(msgInit);
        }
        Console.WriteLine("Send MsgInitData success");
    }

}


