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
    //显示时间的图片
    private Image timeCount;
    //存储时间图片的路径
    private string[] timePath = { "GameLayer/timer_0", "GameLayer/timer_1", "GameLayer/timer_2", "GameLayer/timer_3",
        "GameLayer/timer_4", "GameLayer/timer_5","GameLayer/timer_6","GameLayer/timer_7","GameLayer/timer_8","GameLayer/timer_9"};
    //存储选定玩家的单选框信息，用于发动数学系技能
    private Toggle[] playerRadio;
    //储存GameManager的引用
    private GameManager gameManager;

    //表示出牌的按钮
    private Button okButton;
    //表示取消选中的按钮
    private Button cancelButton;

    //碰按钮
    private Button pengButton;
    //杠按钮
    private Button gangButton;
    //吃按钮
    private Button chiButton;
    //胡按钮
    private Button huButton;
    //不进行操作
    private Button noActionButton;
    //发动技能的按钮
    private Button skillButton;
    //用来显示轮数的列表
    private Text turnText;
    //用来显示你是哪个系的
    private Text skillDisplayText;
    //用来显示还剩下几次技能能发动
    private Text restSkillCount;
    //用来显示是否发动了技能
    private GameObject skillingText;

    //表示是否选中了发动技能的按钮
    public bool isDoSkilling = false;
    //表示现在id的技能类型
    public Skill skill = Skill.None;

    /// <summary>
    /// -1，代表单选框全部不显示
    /// 0-3，代表除了指定索引以外全部显示
    /// </summary>
    public int PlayerRadio
    {
        set
        {
            if(value == -1)
            {
                for(int i = 0; i < 4; i++)
                {
                    playerRadio[i].gameObject.SetActive(false);
                }
            }
            else
            {
                for(int i = 0; i < 4; i++)
                {
                    if (i == value) continue;
                    playerRadio[i].gameObject.SetActive(true);
                }
            }
        }
    }

    public Skill SkillDispalyText
    {
        set
        {
            switch (value)
            {
                case Skill.None:
                    skillDisplayText.text = "你是通识通修";
                    Debug.Log("没加入系，出现了bug");
                    break;
                case Skill.Math:
                    skillDisplayText.text = "你是数学系";
                    break;
                case Skill.Chemistry:
                    skillDisplayText.text = "你是化学系";
                    break;
            }
        }
    }

    public int TurnText
    {
        set
        {
            turnText.text = "第" + value + "轮";
        }
    }

    public int RestSkillCount
    {
        set
        {
            string str = value == 0 ? "你不能使用技能了" : "你还剩下" + value + "次技能";
            restSkillCount.text = str;
        }
    }

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
        }
    }

    public bool GangButton
    {
        set
        {
            gangButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
        }
    }

    public bool HuButton
    {
        set
        {
            huButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
        }
    }

    public bool ChiButton
    {
        set
        {
            chiButton.gameObject.SetActive(value);
            noActionButton.gameObject.SetActive(value);
        }
    }

    public bool SkillButton
    {
        set
        {
            skillButton.gameObject.SetActive(value);
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
        playerRadio = new Toggle[4];
        for(int i = 0; i < 4; i++)
        {
            string str = "Toggle/Player" + (i + 1);
            playerRadio[i] = skin.transform.Find(str).GetComponent<Toggle>();
        }
        okButton = skin.transform.Find("OkButton").GetComponent<Button>();
        cancelButton = skin.transform.Find("CancelButton").GetComponent<Button>();
        timeCount = skin.transform.Find("TimeCount").GetComponent<Image>();
        pengButton = skin.transform.Find("PengButton").GetComponent<Button>();
        gangButton = skin.transform.Find("GangButton").GetComponent<Button>();
        huButton = skin.transform.Find("HuButton").GetComponent<Button>();
        chiButton = skin.transform.Find("ChiButton").GetComponent<Button>();
        noActionButton = skin.transform.Find("NoActionButton").GetComponent<Button>();
        skillButton = skin.transform.Find("SkillButton").GetComponent<Button>();
        turnText = skin.transform.Find("TurnText").GetComponent<Text>();
        skillDisplayText = skin.transform.Find("SkillDisplayText").GetComponent<Text>();
        restSkillCount = skin.transform.Find("RestSkillCount").GetComponent<Text>();
        skillingText = skin.transform.Find("SkillingText").gameObject;
        //监听
        exitButton.onClick.AddListener(OnExitClick);
        setButton.onClick.AddListener(OnSetClick);
        okButton.onClick.AddListener(OnChuPaiClick);
        cancelButton.onClick.AddListener(OnCancelPaiClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        //生成gameManager类
        gameManager = GameManager.GetInstance();
        gameManager.GamePanel = this;
        gameManager.PlayerFactory = new PlayerFactory();


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
        huButton.onClick.AddListener(OnHuClick);
        chiButton.onClick.AddListener(OnChiClick);
        noActionButton.onClick.AddListener(OnNoActionClick);
        skillButton.onClick.AddListener(OnSkillClick);

        chiButton.gameObject.SetActive(false);
        pengButton.gameObject.SetActive(false);
        gangButton.gameObject.SetActive(false);
        huButton.gameObject.SetActive(false);
        noActionButton.gameObject.SetActive(false);
        skillButton.gameObject.SetActive(false);
        PlayerRadio = -1;//隐藏用于发动数学系技能的单选框
        skillingText.SetActive(false);

    }

    /// <summary>
    /// 出牌结束或者打牌结束时用来隐藏发动技能的UI
    /// </summary>
    public void HideSkillUI()
    {
        SkillButton = false;
        skillingText.SetActive(false);
        if(gameManager.players[gameManager.id].skill == Skill.Math)
        {
            PlayerRadio = -1;
        }
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
        timeCount.sprite = ResManager.LoadUISprite(timePath[(int)lastTime]);
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

    public void AddSkillClick()
    {
        switch (skill)
        {
            case Skill.None:
                Debug.Log("技能不可能为空，出现bug");
                break;
            case Skill.Math:
                okButton.onClick.AddListener(OnMathClick);
                cancelButton.onClick.AddListener(OnCancelPlayerClick);
                PlayerRadio = gameManager.id;//用来显示单选框

                OnCancelPaiClick();//在发动技能之前已经选中了牌，就要将牌降落下来
                break;
            case Skill.Chemistry:
                okButton.onClick.AddListener(OnChemistryClick);
                break;
        }
    }

    public void DeleteSkillClick()
    {
        switch (skill)
        {
            case Skill.None:
                Debug.Log("技能不可能为空，出现bug");
                break;
            case Skill.Math:
                okButton.onClick.RemoveListener(OnMathClick);
                cancelButton.onClick.RemoveListener(OnCancelPlayerClick);
                PlayerRadio = -1;//隐藏发动数学系技能的单选框
                //还要添加取消选牌的操作
                break;
            case Skill.Chemistry:
                okButton.onClick.RemoveListener(OnChemistryClick);
                break;
        }
    }

    /// <summary>
    /// 用于发送数学系技能时，显示别人的牌
    /// </summary>
    public void DisplayOtherPai(int[] pai)
    {
        if(pai == null)
        {
            PanelManager.Open<TipPanel>("无法观察到牌");
        }
        else
        {
            string path = "";
            for(int i = 0; i < pai.Length; i++)
            {
                path += Pai.pai_player1[pai[i]] + ",";
            }
            PanelManager.Open<MathTipPanel>(path);
        }
    }

    public void OnMathClick()//发动数学系技能时，点击确定按钮
    {
        Debug.Log("发动数学系技能");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.skill != Skill.Math) return;
        int selectedPlayerIndex = -1;
        for(int i = 0; i < 4; i++)
        {
            if (playerRadio[i].isOn)
            {
                selectedPlayerIndex = i;
                break;
            }
        }
        if (selectedPlayerIndex == -1) return;
        if (selectedPlayerIndex == player.id)
        {
            Debug.Log("发动数学系技能时选中了自己，出现bug");
            return;
        }

        DeleteSkillClick();
        okButton.onClick.AddListener(OnChuPaiClick);

        gameManager.startTimeCount = false;
        gameManager.isChuPai = false;
        HideSkillUI();
        isDoSkilling = false;
        OnCancelPlayerClick();
        //发送协议数学系技能的协议
        player.LaunchMath(selectedPlayerIndex);
    }

    public void OnChemistryClick()//发动化学系技能时，点击确定按钮
    {
        Debug.Log("发动化学系技能");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.skill != Skill.Chemistry) return;
        if (player.selectedPaiIndex == -1) return;

        DeleteSkillClick();
        okButton.onClick.AddListener(OnChuPaiClick);

        gameManager.startTimeCount = false;
        gameManager.isChuPai = false;
        HideSkillUI();

        player.LaunchChemistry();
    }

    //点击发动技能按钮的变化
    public void OnSkillClick()
    {
        isDoSkilling = !isDoSkilling;
        skillingText.SetActive(isDoSkilling);
        if (isDoSkilling)//如果按下了发动技能的按钮
        {
            okButton.onClick.RemoveListener(OnChuPaiClick);
            //根据不同的技能进行扩充，这里可以添加相当于工厂方法的一种设计模式
            AddSkillClick();
        }
        else
        {
            DeleteSkillClick();
            okButton.onClick.AddListener(OnChuPaiClick);
        }
        Debug.Log("发动技能");

    }

    /// <summary>
    /// 发动数学系技能时，点击取消按钮
    /// </summary>
    public void OnCancelPlayerClick()
    {
        for(int i = 0; i < playerRadio.Length; i++)
        {
            playerRadio[i].isOn = false;
        }        
    }

    public void OnChiClick()
    {
        Debug.Log("Chi");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;

        player.ChiPengGang(1);
    }

    public void OnPengClick()
    {
        Debug.Log("Peng");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;

        player.ChiPengGang(2);
    }

    public void OnGangClick()
    {
        Debug.Log("Gang");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;

        player.ChiPengGang(3);
    }

    public void OnHuClick()
    {
        Debug.Log("Hu");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;

        player.Hu();
    }

    public void OnNoActionClick()
    {
        Debug.Log("NoAction");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;

        player.ChiPengGang(0);
    }

    public void OnExitClick()
    {
        Debug.Log("Exit");
    }

    public void OnSetClick()
    {
        Debug.Log("Set");
    }

    //不发动技能时点击确定按钮
    public void OnChuPaiClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.selectedPaiIndex == -1) return;

        player.DaPaiCompolsory();
    }

    //不发动技能时点击取消按钮
    public void OnCancelPaiClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.id];
        if (player == null) return;
        if (player.selectedPaiIndex == -1) return;

        player.handPai[player.selectedPaiIndex].transform.Translate(new Vector3(0, -0.5f, 0));
        player.selectedPaiIndex = -1;
    }

}
public enum Direction
{
    DOWN = 0,
    RIGHT = 1,
    UP = 2,
    LEFT = 3,
    
}


