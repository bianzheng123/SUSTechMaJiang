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

    //碰按钮
    private Button pengButton;
    //杠按钮
    private Button gangButton;
    //胡按钮
    private Button huButton;
    //不进行操作
    private Button noActionButton;

    public bool ChuPaiButton
    {
        set
        {
            okButton.enabled = value;
            cancelButton.enabled = value;
        }
    }

    public bool PengButton
    {
        set
        {
            pengButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
            if (value) noActionButton.onClick.AddListener(OnPengCancelClick);
            else
                noActionButton.onClick.RemoveListener(OnPengCancelClick);
        }
    }

    public bool GangButton
    {
        set
        {
            gangButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
            if (value) noActionButton.onClick.AddListener(OnGangCancelClick);
            else
                noActionButton.onClick.RemoveListener(OnGangCancelClick);
        }
    }

    public bool HuButton
    {
        set
        {
            huButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
            if (value) noActionButton.onClick.AddListener(OnHuCancelClick);
            else
                noActionButton.onClick.RemoveListener(OnHuCancelClick);
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
        pengButton = skin.transform.Find("PengButton").GetComponent<Button>();
        gangButton = skin.transform.Find("GangButton").GetComponent<Button>();
        huButton = skin.transform.Find("HuButton").GetComponent<Button>();
        noActionButton = skin.transform.Find("NoActionButton").GetComponent<Button>();
        //监听
        exitButton.onClick.AddListener(OnExitClick);
        setButton.onClick.AddListener(OnSetClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        //生成gameManager类
        GameObject gameManager = ResManager.LoadPrefab("GameManager");
        gameManager.GetComponent<GameManager>().GamePanel = this;
        GameObject init_gameManager = Instantiate(gameManager);
        this.gameManager = init_gameManager.GetComponent<GameManager>();

        //生成背景图片
        GameObject bg = ResManager.LoadSprite("bg_game",0);
        bg.transform.localScale = new Vector3(2,2,2);

        //灯光的初始化
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
        ChuPaiButton = false;

        //特殊事件的初始化
        pengButton.onClick.AddListener(OnPengClick);
        gangButton.onClick.AddListener(OnGangClick);
        huButton.onClick.AddListener(OnHuClick);//不做任何动作不需要进行复用
        pengButton.gameObject.SetActive(false);
        gangButton.gameObject.SetActive(false);
        huButton.gameObject.SetActive(false);
        noActionButton.gameObject.SetActive(false);

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

    public void OnPengClick()
    {
        Debug.Log("Peng");
    }

    public void OnGangClick()
    {
        Debug.Log("Gang");
    }

    public void OnHuClick()
    {
        Debug.Log("Hu");
    }

    public void OnPengCancelClick()
    {
        Debug.Log("PengCancel");
    }

    public void OnGangCancelClick()
    {
        Debug.Log("GangCancel");
    }

    public void OnHuCancelClick()
    {
        Debug.Log("HuCancel");
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


