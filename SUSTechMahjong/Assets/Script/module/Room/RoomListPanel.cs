using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RoomListPanel : BasePanel {
	//账号文本
	private Text idText;
	//战绩文本
	private Text scoreText;
    //院系文本
    private Text majorText;
    //现在拥有的金币数
    private Text coinText;
	//创建房间按钮
	private Button createButton;
	//刷新列表按钮
	private Button reflashButton;
    private Button chooseButton;
    //微信支付按钮
    private Button pressMeButton;
    //列表容器
    private Transform content;
	//房间物体
	private GameObject roomObj;
    //用于储存现在有多少金币
    private int coin;
    //用于存储当前用户的系
    private int major;

    public int Major
    {
        set
        {
            major = value;
            majorText.text = Gamedata.majors[major];
        }
    }

    public int Coin
    {
        set
        {
            coin = value;
            coinText.text = value.ToString();
        }
    }

	//初始化
	public override void OnInit() {
		skinPath = "RoomListPanel";
		layer = PanelManager.Layer.Panel;
	}

	//显示
	public override void OnShow(params object[] args) {
		//寻找组件
		idText = skin.transform.Find("InfoPanel/IdText").GetComponent<Text>();
		scoreText = skin.transform.Find("InfoPanel/ScoreText").GetComponent<Text>();
        majorText = skin.transform.Find("InfoPanel/CampText").GetComponent<Text>();
        coinText = skin.transform.Find("InfoPanel/GoldText").GetComponent<Text>();
        createButton = skin.transform.Find("CtrlPanel/CreateButton").GetComponent<Button>();
		reflashButton = skin.transform.Find("CtrlPanel/RefreshButton").GetComponent<Button>();
        chooseButton = skin.transform.Find("CtrlPanel/ChooseBtn").GetComponent<Button>();
        pressMeButton = skin.transform.Find("CtrlPanel/PressMeBtn").GetComponent<Button>();
        content = skin.transform.Find("ListPanel/Scroll View/Viewport/Content");
		roomObj = skin.transform.Find("Room").gameObject;
        //不激活房间
        roomObj.SetActive(false);
		//显示id
		idText.text = GameMain.id;
		//按钮事件(chooseButton实际上是reflashButton)
		createButton.onClick.AddListener(OnCreateClick);
		reflashButton.onClick.AddListener(OnReflashClick);
        chooseButton.onClick.AddListener(OnChooseClick);
        pressMeButton.onClick.AddListener(OnPressMeClick);

        createButton.onClick.AddListener(Audio.ButtonClick);
        reflashButton.onClick.AddListener(Audio.ButtonClick);
        chooseButton.onClick.AddListener(Audio.ButtonClick);
        pressMeButton.onClick.AddListener(Audio.ButtonClick);
        //协议监听
        NetManager.AddMsgListener("MsgGetAchieve", OnMsgGetAchieve);
		NetManager.AddMsgListener("MsgGetRoomList", OnMsgGetRoomList);
		NetManager.AddMsgListener("MsgCreateRoom", OnMsgCreateRoom);
		NetManager.AddMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.AddMsgListener("MsgVisitShop", OnMsgVisitShop);
		//发送查询
		MsgGetAchieve msgGetAchieve = new MsgGetAchieve();
		NetManager.Send(msgGetAchieve);
		MsgGetRoomList msgGetRoomList = new MsgGetRoomList();
		NetManager.Send(msgGetRoomList);

        Audio.PlayLoop(Audio.bgRoomListPanel);
	}


	//关闭
	public override void OnClose() {
		//协议监听
		NetManager.RemoveMsgListener("MsgGetAchieve", OnMsgGetAchieve);
		NetManager.RemoveMsgListener("MsgGetRoomList", OnMsgGetRoomList);
		NetManager.RemoveMsgListener("MsgCreateRoom", OnMsgCreateRoom);
		NetManager.RemoveMsgListener("MsgEnterRoom", OnMsgEnterRoom);
        NetManager.RemoveMsgListener("MsgVisitShop", OnMsgVisitShop);
        Audio.MuteLoop(Audio.bgRoomListPanel);
    }

	//收到成绩查询协议
	public void OnMsgGetAchieve (MsgBase msgBase) {
		MsgGetAchieve msg = (MsgGetAchieve)msgBase;
		scoreText.text = msg.win + "胜 " + msg.lost + "负";
        majorText.text = Gamedata.majors[msg.major];
        coinText.text = msg.coin.ToString();
        coin = msg.coin;
        major = msg.major;
    }

	//收到房间列表协议
	public void OnMsgGetRoomList (MsgBase msgBase) {
		MsgGetRoomList msg = (MsgGetRoomList)msgBase;
		//清除房间列表
		for(int i = content.childCount-1; i >= 0 ; i--){
			GameObject o = content.GetChild(i).gameObject;
			Destroy(o);
		}
		//重新生成列表
		if(msg.rooms == null){
			return;
		}
		for(int i = 0; i < msg.rooms.Length; i++){
			GenerateRoom(msg.rooms[i]);
		}
	}
   
	//创建一个房间单元
	public void GenerateRoom(RoomInfo roomInfo){
		//创建物体
		GameObject o = Instantiate(roomObj);
		o.transform.SetParent(content);
		o.SetActive(true);
		o.transform.localScale = Vector3.one; 
		//获取组件
		Transform trans = o.transform;
		Text idText = trans.Find("IdText").GetComponent<Text>();
		Text countText = trans.Find("CountText").GetComponent<Text>();
		Text statusText = trans.Find("StatusText").GetComponent<Text>();
		Button btn = trans.Find("JoinButton").GetComponent<Button>();
		//填充信息
		idText.text = roomInfo.id.ToString();
		countText.text = roomInfo.count.ToString();
		if(roomInfo.status == 0){
			statusText.text = "准备中";
		}
		else{
			statusText.text = "战斗中";
		}
		//按钮事件
		btn.name = idText.text;
		btn.onClick.AddListener(delegate(){
			OnJoinClick(btn.name);
		});
	}

    public void OnPressMeClick()
    {
        PanelManager.Open<PressMePanel>();
    }

    //点击刷新按钮
    public void OnReflashClick(){
		MsgGetRoomList msg = new MsgGetRoomList();
		NetManager.Send(msg);
	}

	//点击加入房间按钮
	public void OnJoinClick(string idString) {
		MsgEnterRoom msg = new MsgEnterRoom();
		msg.id = int.Parse(idString);
		NetManager.Send(msg);
	}

	//收到进入房间协议
	public void OnMsgEnterRoom (MsgBase msgBase) {
		MsgEnterRoom msg = (MsgEnterRoom)msgBase;
		//成功进入房间
		if(msg.result == 0){
			PanelManager.Open<RoomPanel>();
			Close();
		}
		//进入房间失败
		else{
			PanelManager.Open<TipPanel>("进入房间失败");
		}
	}

	//点击新建房间按钮
	public void OnCreateClick() {
		MsgCreateRoom msg = new MsgCreateRoom();
		NetManager.Send(msg);
	}
    //点击进入商城的按钮
    public void OnChooseClick(){

        MsgVisitShop msg = new MsgVisitShop();
        NetManager.Send(msg);
    }
    //收到新建房间协议
    public void OnMsgCreateRoom (MsgBase msgBase) {
		MsgCreateRoom msg = (MsgCreateRoom)msgBase;
		//成功创建房间
		if(msg.result == 0){
			PanelManager.Open<TipPanel>("创建成功");
			PanelManager.Open<RoomPanel>();
			Close();
		}
		//创建房间失败
		else{
			PanelManager.Open<TipPanel>("创建房间失败");
		}
	}

    //收到进入房间的请求
    public void OnMsgVisitShop(MsgBase msgBase)
    {
        MsgVisitShop msg = (MsgVisitShop)msgBase;
        Debug.Log(msg.price);
        PanelManager.Open<ChoosePanel>(msg.price,coin,major);
    }

}
