using System;
using System.Collections.Generic;

public class Room {
	//id
	public int id = 0;
	//最大玩家数
	public int maxPlayer = 4;
	//玩家列表
	public HashSet<string> playerIds = new HashSet<string>();
    //现在限定没有观战的人
	//房主id
	public string ownerId = "";
	//状态
	public enum Status {
		PREPARE = 0,
		FIGHT = 1 ,
	}
	public Status status = Status.PREPARE;
    //管理这个房间的牌
    public GameManager gameManager;

	//添加玩家
	public bool AddPlayer(string id){
		//获取玩家
		Player player = PlayerManager.GetPlayer(id);
		if(player == null){
			Console.WriteLine("room.AddPlayer fail, player is null");
			return false;
		}
		//房间人数
		if(playerIds.Count >= maxPlayer){
			Console.WriteLine("room.AddPlayer fail, reach maxPlayer");
			return false;
		}
		//准备状态才能加人
		if(status != Status.PREPARE){
			Console.WriteLine("room.AddPlayer fail, not PREPARE");
			return false;
		}
		//已经在房间里
		if(playerIds.Contains(id)){
			Console.WriteLine("room.AddPlayer fail, already in this room");
			return false;
		}
        //加入列表
        playerIds.Add(id);
        //设置玩家数据
         
		player.roomId = this.id;
		//设置房主
		if(ownerId == ""){
			ownerId = player.id;
		}
		//广播
		Broadcast(ToMsg());
		return true;
	}

	//是不是房主
	public bool isOwner(Player player){
		return player.id == ownerId;
	}

	//删除玩家
	public bool RemovePlayer(string id) {
		//获取玩家
		Player player = PlayerManager.GetPlayer(id);
		if(player == null){
			Console.WriteLine("room.RemovePlayer fail, player is null");
			return false;
		}
		//没有在房间里
		if(!playerIds.Contains(id)){
			Console.WriteLine("room.RemovePlayer fail, not in this room");
			return false;
		}
		//删除列表
		playerIds.Remove(id);
		//设置玩家数据
		player.roomId = -1;
		//设置房主
		if(ownerId == player.id){
			ownerId = SwitchOwner();
		}
		//房间为空
		if(playerIds.Count == 0){
			RoomManager.RemoveRoom(this.id);
		}
		//广播
		Broadcast(ToMsg());
		return true;
	}

	//选择房主
	public string SwitchOwner() {
		//选择第一个玩家
		foreach(string id in playerIds) {
			return id;
		}
		//房间没人
		return "";
	}


	//广播消息
	public void Broadcast(MsgBase msg){
		foreach(string id in playerIds) {
			Player player = PlayerManager.GetPlayer(id);
			player.Send(msg);
		}
	}

	//生成MsgGetRoomInfo协议
	public MsgBase ToMsg(){
		MsgGetRoomInfo msg = new MsgGetRoomInfo();
		int count = playerIds.Count;
		msg.players = new PlayerInfo[count];
		//players
		int i = 0;
		foreach(string id in playerIds){
			Player player = PlayerManager.GetPlayer(id);
			PlayerInfo playerInfo = new PlayerInfo();
			//赋值
			playerInfo.id = player.id;
			playerInfo.major = player.data.major;
			playerInfo.win = player.data.win;
			playerInfo.lost = player.data.lost;
			playerInfo.isOwner = 0;
            playerInfo.gender = player.data.gender;
			if(isOwner(player)){
				playerInfo.isOwner = 1;
			}

			msg.players[i] = playerInfo;
			i++;
		}
		return msg;
	}
}

