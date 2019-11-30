﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel {
    //储存GameManager的引用
    private static GameManager gameManager;
    //显示时间的图片
    private Image[] timeCount;
    //存储时间图片的路径
    private string[] timePath = { "GameLayer/timer_0", "GameLayer/timer_1", "GameLayer/timer_2", "GameLayer/timer_3",
        "GameLayer/timer_4", "GameLayer/timer_5","GameLayer/timer_6","GameLayer/timer_7","GameLayer/timer_8","GameLayer/timer_9"};
    //用于输入id输出这个客户端对应的控制
    public Dictionary<int, Direction> numToDir;
    //表示是否选中了发动技能的按钮
    public bool isDoSkilling = false;
    //表示现在id的技能类型
    public Skill skill = Skill.None;
    //掌管音乐文件的
    private static AudioSource audioSource;

    

    //表示出牌的按钮
    private Button okButton;
    //表示取消选中的按钮
    private Button cancelButton;
    //退出按钮
    private Button exitButton;
    //设置UI按钮
    private Button setButton;
    //灯的亮灭
    private GameObject[] lights;
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
    //显示庄家的图片
    private Image zhuangImage;
    //存储选定玩家的单选框信息，用于发动数学系技能
    private Toggle[] playerRadio;
    //用于显示其他玩家信息的按钮
    private PlayerPaiInfo[] playerPaiInfo;

    struct PlayerPaiInfo
    {
        public Button discardPai;
        public Button chi;
        public Button peng;
        public Button gang;
    }

    /// <summary>
    /// -1，代表单选框全部不显示
    /// 0-3，代表除了指定索引以外全部显示
    /// </summary>
    public bool PlayerRadio
    {
        set
        {
            for (int i = 0; i < 3; i++)
            {
                playerRadio[i].gameObject.SetActive(value);
            }
        }
    }

    public Skill Skill
    {
        set
        {
            skill = value;
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
            okButton.gameObject.SetActive(value);
            cancelButton.gameObject.SetActive(value);
            for(int i = 0; i < playerPaiInfo.Length; i++)
            {
                playerPaiInfo[i].discardPai.gameObject.SetActive(value);
                playerPaiInfo[i].chi.gameObject.SetActive(value);
                playerPaiInfo[i].peng.gameObject.SetActive(value);
                playerPaiInfo[i].gang.gameObject.SetActive(value);
            }
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
    public int ZhuangImage
    {
        set
        {
            Direction dir = numToDir[value];
            zhuangImage.gameObject.SetActive(true);
            switch (dir)
            {
                case Direction.DOWN:
                    zhuangImage.transform.localPosition = new Vector3(0,-65.5f,0);
                    break;
                case Direction.RIGHT:
                    zhuangImage.transform.localPosition = new Vector3(77f,0,0);
                    break;
                case Direction.UP:
                    zhuangImage.transform.localPosition = new Vector3(0,77,0);
                    break;
                case Direction.LEFT:
                    zhuangImage.transform.localPosition = new Vector3(-77,0,0);
                    break;
            }
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
        playerRadio = new Toggle[3];
        for(int i = 0; i < 3; i++)
        {
            //默认下方为player1，然后逆时针为player2,3,4...
            string str = "Toggle/Player" + (i + 2);
            playerRadio[i] = skin.transform.Find(str).GetComponent<Toggle>();
        }
        okButton = skin.transform.Find("OkButton").GetComponent<Button>();
        cancelButton = skin.transform.Find("CancelButton").GetComponent<Button>();
        timeCount = new Image[2];
        timeCount[0] = skin.transform.Find("TimeCount_GeWei").GetComponent<Image>();
        timeCount[1] = skin.transform.Find("TimeCount_ShiWei").GetComponent<Image>();
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
        zhuangImage = skin.transform.Find("ZhuangImage").GetComponent<Image>();
        playerPaiInfo = new PlayerPaiInfo[4];
        //第一个用来显示自己的弃牌，吃碰杠
        playerPaiInfo[0].discardPai = skin.transform.Find("OtherPlayerInfo/Player1/DiscardPai").GetComponent<Button>();
        playerPaiInfo[0].chi = skin.transform.Find("OtherPlayerInfo/Player1/Chi").GetComponent<Button>();
        playerPaiInfo[0].peng = skin.transform.Find("OtherPlayerInfo/Player1/Peng").GetComponent<Button>();
        playerPaiInfo[0].gang = skin.transform.Find("OtherPlayerInfo/Player1/Gang").GetComponent<Button>();

        playerPaiInfo[1].discardPai = skin.transform.Find("OtherPlayerInfo/Player2/DiscardPai").GetComponent<Button>();
        playerPaiInfo[1].chi = skin.transform.Find("OtherPlayerInfo/Player2/Chi").GetComponent<Button>();
        playerPaiInfo[1].peng = skin.transform.Find("OtherPlayerInfo/Player2/Peng").GetComponent<Button>();
        playerPaiInfo[1].gang = skin.transform.Find("OtherPlayerInfo/Player2/Gang").GetComponent<Button>();

        playerPaiInfo[2].discardPai = skin.transform.Find("OtherPlayerInfo/Player3/DiscardPai").GetComponent<Button>();
        playerPaiInfo[2].chi = skin.transform.Find("OtherPlayerInfo/Player3/Chi").GetComponent<Button>();
        playerPaiInfo[2].peng = skin.transform.Find("OtherPlayerInfo/Player3/Peng").GetComponent<Button>();
        playerPaiInfo[2].gang = skin.transform.Find("OtherPlayerInfo/Player3/Gang").GetComponent<Button>();

        playerPaiInfo[3].discardPai = skin.transform.Find("OtherPlayerInfo/Player4/DiscardPai").GetComponent<Button>();
        playerPaiInfo[3].chi = skin.transform.Find("OtherPlayerInfo/Player4/Chi").GetComponent<Button>();
        playerPaiInfo[3].peng = skin.transform.Find("OtherPlayerInfo/Player4/Peng").GetComponent<Button>();
        playerPaiInfo[3].gang = skin.transform.Find("OtherPlayerInfo/Player4/Gang").GetComponent<Button>();
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
        audioSource = gameManager.gameObject.AddComponent<AudioSource>();

        //生成背景图片
        GameObject bg = ResManager.LoadSprite("bg_game",0);
        bg.name = "background";
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

        playerPaiInfo[0].discardPai.onClick.AddListener(OnOtherInfoClick_discardPai_player1);
        playerPaiInfo[0].chi.onClick.AddListener(OnOtherInfoClick_chi_player1);
        playerPaiInfo[0].peng.onClick.AddListener(OnOtherInfoClick_peng_player1);
        playerPaiInfo[0].gang.onClick.AddListener(OnOtherInfoClick_gang_player1);

        playerPaiInfo[1].discardPai.onClick.AddListener(OnOtherInfoClick_discardPai_player2);
        playerPaiInfo[1].chi.onClick.AddListener(OnOtherInfoClick_chi_player2);
        playerPaiInfo[1].peng.onClick.AddListener(OnOtherInfoClick_peng_player2);
        playerPaiInfo[1].gang.onClick.AddListener(OnOtherInfoClick_gang_player2);

        playerPaiInfo[2].discardPai.onClick.AddListener(OnOtherInfoClick_discardPai_player3);
        playerPaiInfo[2].chi.onClick.AddListener(OnOtherInfoClick_chi_player3);
        playerPaiInfo[2].peng.onClick.AddListener(OnOtherInfoClick_peng_player3);
        playerPaiInfo[2].gang.onClick.AddListener(OnOtherInfoClick_gang_player3);

        playerPaiInfo[3].discardPai.onClick.AddListener(OnOtherInfoClick_discardPai_player4);
        playerPaiInfo[3].chi.onClick.AddListener(OnOtherInfoClick_chi_player4);
        playerPaiInfo[3].peng.onClick.AddListener(OnOtherInfoClick_peng_player4);
        playerPaiInfo[3].gang.onClick.AddListener(OnOtherInfoClick_gang_player4);

        chiButton.gameObject.SetActive(false);
        pengButton.gameObject.SetActive(false);
        gangButton.gameObject.SetActive(false);
        huButton.gameObject.SetActive(false);
        noActionButton.gameObject.SetActive(false);
        skillButton.gameObject.SetActive(false);
        zhuangImage.gameObject.SetActive(false);
        PlayerRadio = false;//隐藏用于发动数学系技能的单选框
        skillingText.SetActive(false);
    }

    /// <summary>
    /// 出牌结束或者打牌结束时用来隐藏发动技能的UI
    /// </summary>
    public void HideSkillUI()
    {
        SkillButton = false;
        skillingText.SetActive(false);
        if(gameManager.players[gameManager.client_id].skill == Skill.Math)
        {
            PlayerRadio = false;
        }
    }

    //改变灯光的顺序
    public void TurnLight(int playerid)
    {
        Direction dir = numToDir[playerid];
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
        lights[(int)dir].SetActive(true);
    }

    public void SetTimeCount(float lastTime)
    {
        int time = (int)lastTime;
        if(lastTime >= 10)
        {
            //[1]是十位，[0]是个位
            timeCount[1].gameObject.SetActive(true);
            timeCount[1].sprite = ResManager.LoadUISprite(timePath[time / 10]);
            timeCount[0].sprite = ResManager.LoadUISprite(timePath[time % 10]);
        }
        else
        {
            timeCount[1].gameObject.SetActive(false);
            timeCount[0].sprite = ResManager.LoadUISprite(timePath[time % 10]);
        }
        
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
                PlayerRadio = true;//用来显示单选框

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
                PlayerRadio = false;//隐藏发动数学系技能的单选框
                //还要添加取消选牌的操作
                break;
            case Skill.Chemistry:
                okButton.onClick.RemoveListener(OnChemistryClick);
                break;
        }
    }

    public void OnMathClick()//发动数学系技能时，点击确定按钮
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;
        if (player.skill != Skill.Math) return;
        int selectedPlayerIndex = -1;
        for(int i = 0; i < 3; i++)
        {
            if (playerRadio[i].isOn)
            {
                selectedPlayerIndex = i+1;
                break;
            }
        }
        if (selectedPlayerIndex == -1) return;
        selectedPlayerIndex = (gameManager.client_id + selectedPlayerIndex) % 4;
        if (selectedPlayerIndex == player.id)
        {
            Debug.Log("发动数学系技能时选中了自己，出现bug");
            return;
        }

        DeleteSkillClick();
        okButton.onClick.AddListener(OnChuPaiClick);

        gameManager.startTimeCount = false;//停止计时
        gameManager.isChuPai = false;
        HideSkillUI();
        isDoSkilling = false;
        OnCancelPlayerClick();
        //发送协议数学系技能的协议
        player.LaunchMath(selectedPlayerIndex);
    }

    public void OnChemistryClick()//发动化学系技能时，点击确定按钮
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
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
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;

        player.ChiPengGang(1);
    }

    public void OnPengClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;

        player.ChiPengGang(2);
    }

    public void OnGangClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;

        player.ChiPengGang(3);
    }

    public void OnHuClick()
    {
        Debug.Log("Hu");
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;

        player.Hu();
    }

    public void OnNoActionClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
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
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;
        if (player.selectedPaiIndex == -1) return;

        player.ChuPai();
    }

    //不发动技能时点击取消按钮
    public void OnCancelPaiClick()
    {
        if (gameManager.players == null) return;
        CtrlPlayer player = (CtrlPlayer)gameManager.players[gameManager.client_id];
        if (player == null) return;
        if (player.selectedPaiIndex == -1) return;

        player.handPai[player.selectedPaiIndex].transform.Translate(new Vector3(0, -0.5f, 0));
        player.selectedPaiIndex = -1;
    }

    public void OnOtherInfoClick_discardPai_player1() { GetPlayerPai(0, Enum_OtherPlayerInfo.DiscardPai); }

    public void OnOtherInfoClick_chi_player1() { GetPlayerPai(0, Enum_OtherPlayerInfo.Chi); }

    public void OnOtherInfoClick_peng_player1() { GetPlayerPai(0, Enum_OtherPlayerInfo.Peng); }

    public void OnOtherInfoClick_gang_player1() { GetPlayerPai(0, Enum_OtherPlayerInfo.Gang); }

    public void OnOtherInfoClick_discardPai_player2() { GetPlayerPai(1,Enum_OtherPlayerInfo.DiscardPai); }

    public void OnOtherInfoClick_chi_player2() { GetPlayerPai(1,Enum_OtherPlayerInfo.Chi); }

    public void OnOtherInfoClick_peng_player2() { GetPlayerPai(1,Enum_OtherPlayerInfo.Peng); }

    public void OnOtherInfoClick_gang_player2() { GetPlayerPai(1,Enum_OtherPlayerInfo.Gang); }

    public void OnOtherInfoClick_discardPai_player3() { GetPlayerPai(2,Enum_OtherPlayerInfo.DiscardPai); }

    public void OnOtherInfoClick_chi_player3() { GetPlayerPai(2,Enum_OtherPlayerInfo.Chi); }

    public void OnOtherInfoClick_peng_player3() { GetPlayerPai(2,Enum_OtherPlayerInfo.Peng); }

    public void OnOtherInfoClick_gang_player3() { GetPlayerPai(2,Enum_OtherPlayerInfo.Gang); }

    public void OnOtherInfoClick_discardPai_player4() { GetPlayerPai(3,Enum_OtherPlayerInfo.DiscardPai); }

    public void OnOtherInfoClick_chi_player4() { GetPlayerPai(3,Enum_OtherPlayerInfo.Chi); }

    public void OnOtherInfoClick_peng_player4() { GetPlayerPai(3,Enum_OtherPlayerInfo.Peng); }

    public void OnOtherInfoClick_gang_player4() { GetPlayerPai(3,Enum_OtherPlayerInfo.Gang); }

    /// <summary>
    /// 得到其他玩家的棋牌，已经进行完吃碰杠操作的牌
    /// </summary>
    /// <param name="relativeId">目前麻将桌上的playerid差,顺时针递增</param>
    /// <param name="paiType">需要获取的信息是吃碰杠还是弃牌</param>
    private void GetPlayerPai(int relativeId,Enum_OtherPlayerInfo paiType)
    {
        int realId = (gameManager.client_id + relativeId) % 4;
        int[] pai = null;
        List<int> list = null;
        switch (paiType)
        {
            case Enum_OtherPlayerInfo.DiscardPai:
                list = gameManager.players[realId].discardPai;
                break;
            case Enum_OtherPlayerInfo.Chi:
                list = gameManager.players[realId].chi;
                break;
            case Enum_OtherPlayerInfo.Peng:
                list = gameManager.players[realId].peng;
                break;
            case Enum_OtherPlayerInfo.Gang:
                list = gameManager.players[realId].gang;
                break;
        }
        if(list.Count != 0)
        {
            pai = list.ToArray();
        }
        DisplayOtherPai(pai);
    }

    /// <summary>
    /// 用于发送数学系技能时，显示别人的牌
    /// </summary>
    public void DisplayOtherPai(int[] pai)
    {
        if (pai == null)
        {
            PanelManager.Open<TipPanel>("无法观察到牌");
        }
        else
        {
            string path = "";
            for (int i = 0; i < pai.Length; i++)
            {
                path += Pai.pai_player1[pai[i]] + ",";
            }
            PanelManager.Open<ObjectTipPanel>(path);
        }
    }

    public static void PlayAudio(string path)
    {
        audioSource.clip = ResManager.LoadAudio(path);
        audioSource.Play();
    }

    /// <summary>
    /// 输入玩家的id，输出他在这个客户端的相对位置
    /// </summary>
    public void InitNumToDir(int client_id)
    {
        numToDir = new Dictionary<int, Direction>();
        //这个id指的是这个客户端的玩家id
        switch (client_id)
        {
            case 0:
                numToDir[0] = Direction.DOWN;
                numToDir[1] = Direction.RIGHT;
                numToDir[2] = Direction.UP;
                numToDir[3] = Direction.LEFT;
                break;
            case 1:
                numToDir[0] = Direction.LEFT;
                numToDir[1] = Direction.DOWN;
                numToDir[2] = Direction.RIGHT;
                numToDir[3] = Direction.UP;
                break;
            case 2:
                numToDir[0] = Direction.UP;
                numToDir[1] = Direction.LEFT;
                numToDir[2] = Direction.DOWN;
                numToDir[3] = Direction.RIGHT;
                break;
            case 3:
                numToDir[0] = Direction.RIGHT;
                numToDir[1] = Direction.UP;
                numToDir[2] = Direction.LEFT;
                numToDir[3] = Direction.DOWN;
                break;
        }

    }
}
public enum Direction
{
    DOWN = 0,
    RIGHT = 1,
    UP = 2,
    LEFT = 3,
}
/// <summary>
/// 用来显示别的玩家的信息
/// </summary>
public enum Enum_OtherPlayerInfo
{
    DiscardPai,
    Chi,
    Peng,
    Gang
}


