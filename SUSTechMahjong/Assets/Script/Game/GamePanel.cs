using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel {
    //退出按钮
    private Button exitButton;
    //设置UI按钮
    private Button setButton;
    //灯的亮灭
    private GameObject[] lights;
    //设置计时时间
    private Text timeCount;
    //显示时间的图片
    private Image time;

    //储存GameManager的引用
    private GameManager gameManager;

    //表示出牌的按钮
    public Button okButton;
    //表示取消选中的按钮
    public Button cancelButton;

    public bool ChuPaiButton
    {
        set
        {
            okButton.enabled = value;
            cancelButton.enabled = value;
        }
    }

    //初始化
    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        exitButton = skin.transform.Find("ExitButton").GetComponent<Button>();
        setButton = skin.transform.Find("SetButton").GetComponent<Button>();
        lights = new GameObject[4];
        lights[0] = skin.transform.Find("TimeImage/Down").gameObject;
        lights[1] = skin.transform.Find("TimeImage/Right").gameObject;
        lights[2] = skin.transform.Find("TimeImage/Up").gameObject;
        lights[3] = skin.transform.Find("TimeImage/Left").gameObject;
        timeCount = skin.transform.Find("TimeCount").GetComponent<Text>();
        okButton = skin.transform.Find("OkButton").GetComponent<Button>();
        cancelButton = skin.transform.Find("CancelButton").GetComponent<Button>();
        time = skin.transform.Find("Image").GetComponent<Image>();
        //监听
        exitButton.onClick.AddListener(OnExitClick);
        setButton.onClick.AddListener(OnSetClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        GameObject gameManager = ResManager.LoadPrefab("GameManager");
        gameManager.GetComponent<GameManager>().GamePanel = this;
        GameObject init_gameManager = Instantiate(gameManager);
        this.gameManager = init_gameManager.GetComponent<GameManager>();

        GameObject bg = ResManager.LoadSprite("bg_game",0);
        bg.transform.localScale = new Vector3(2,2,2);

        //组件的初始化
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
        ChuPaiButton = false;
    }

    //改变灯光的顺序
    public void TurnLight(Direction dir)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
        lights[(int)dir].SetActive(true);
    }

    public void SetTimeCount(float lastTime)
    {
        timeCount.text = "剩余时间: " + string.Format("{0:G}", lastTime) + " 秒";
        time.sprite = ResManager.LoadUISprite("GameLayer/timer_1");
    }

    //关闭
    public override void OnClose()
    {
        //网络协议监听
        //NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听
        //NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    public void OnExitClick()
    {
        Debug.Log("Exit");
    }

    public void OnSetClick()
    {
        Debug.Log("Set");
    }

    public void OnOkClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.selectedIndex == -1) return;

        player.DaPaiCompolsory();
    }

    public void OnCancelClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.selectedIndex == -1) return;

        player.handPai[player.selectedIndex].transform.Translate(new Vector3(0, -0.5f, 0));
    }

}
public enum Direction
{
    DOWN = 0,
    RIGHT = 1,
    UP = 2,
    LEFT = 3,
    
}


