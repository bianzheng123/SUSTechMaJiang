using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMain : MonoBehaviour {
	public static string id = "";


	// Use this for initialization
	void Start () {
		//网络监听
		NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);
		NetManager.AddMsgListener("MsgKick", OnMsgKick);
		//初始化
		PanelManager.Init();
        Audio.Init();
        Pai.Init();
        Gamedata.Init();
		//打开登陆面板
		PanelManager.Open<LoginPanel>();
        Debug.Log("start");
	}


	// Update is called once per frame
	void Update () {
		NetManager.Update();
	}

	//关闭连接
	void OnConnectClose(string err){
		PanelManager.Open<TipPanel>(err);
	}

	//被踢下线
	void OnMsgKick(MsgBase msgBase){
		PanelManager.Open<TipPanel>("被踢下线");
	}
}
