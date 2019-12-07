using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private GameManager() { }
    private static GameManager _instance;
    public static GameManager GetInstance()
    {
        if (_instance == null)
        {
            GameObject obj = new GameObject("GameManager");
            _instance = obj.AddComponent<GameManager>();
        }
        return _instance;
    }

    private PlayerFactory playerFactory;//简单工厂模式
    private GamePanel gamePanel;
    public int client_id;//该客户端的id
    public BasePlayer[] players;//存放这些玩家的引用
    public const float timeCount = 20;//代表出牌计时的时间
    public bool isChuPai = false;//是否为出牌，还是进行吃碰杠的判断
    public int nowTurnid = 0;
    public float timeLast = timeCount;
    public bool startTimeCount = false;
    public int skillCount;//代表当前主机玩家还剩下几次技能可以使用
    public bool canTimeAlarm = false;//代表是否剩下最后三秒，是否可以开始播放倒计时的bgm了
    public bool isZhuang = false;

    public PlayerFactory PlayerFactory
    {
        set { playerFactory = value; }
    }

    public GamePanel GamePanel
    {
        set { gamePanel = value; }
    }

    // Use this for initialization
    void Start()
    {
        gamePanel = GameObject.Find("Root").GetComponent<GamePanel>();
    }

    //结束只有两种可能，一个是ChuPai协议中出现了胡，一个是FaPai协议中没牌的情况

    /// <summary>
    /// 接收到初始数据的消息
    /// </summary>
    /// <param name="msg"></param>
    public void OnMsgInitData(MsgInitData msg)
    {
        players = new BasePlayer[4];
        client_id = msg.id;
        gamePanel.InitNumToDir(msg.id);
        InitPlayer(msg);
        gamePanel.Skill = (Major)msg.data[msg.id].skillIndex;
        skillCount = msg.data[msg.id].skillCount;
        gamePanel.RestSkillCount = skillCount;
        //gamePanel中先给Skill进行赋值，才能给RestSkillCount进行赋值
        Debug.Log("你的id是：" + client_id + " ,你的性别是:" + players[client_id].gender);
        //生成牌
        for (int i = 0; i < 4; i++)
        {
            if (msg.data[i].paiIndex.Length == 14)
            {
                gamePanel.ZhuangImage = i;
                if(i == client_id)
                {
                    isZhuang = true;
                }
            }
            for (int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j], i);
            }
            players[i].PlacePai();
        }
    }

    /// <summary>
    /// 客户端处理发牌协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgFaPai(MsgBase msgBase)
    {
        Debug.Log("can receive msgFaPai");
        MsgFaPai msg = (MsgFaPai)msgBase;
        nowTurnid = msg.id;
        if (msg.paiId == -1)
        {
            Debug.Log("没牌了，游戏结束");
            PanelManager.Open<GameoverPanel>(-1, client_id);
            PanelManager.Close("GamePanel");
            return;
        }

        gamePanel.TurnText = msg.turnNum;//设置现在是第几轮了
        gamePanel.TurnLight(msg.id);//切换灯光
        if (msg.id == client_id)//如果是玩家，就显示出牌/发动技能/胡的按钮
        {
            gamePanel.ChuPaiButton = true;
            if (msg.isHu)
            {
                gamePanel.HuButton = true;
            }
            if (msg.canSkill)
            {
                gamePanel.SkillButton = true;
            }
        }

        CreatePai(msg.paiId, msg.id);
        players[msg.id].SynHandPai();//调整牌的顺序
        players[msg.id].PlacePai();//调整牌的位置

        //string arrString = "";
        //for (int i = 0; i < paiManager.playerPai[msg.id].Count; i++)
        //{
        //    arrString += paiManager.playerPai[msg.id][i] + " ";
        //}
        //Debug.Log("player" + msg.id + ": " + arrString);//验证服务端的牌是否进行了同步

        isChuPai = true;
        //开始计时，玩家出牌
        StartTimeCount();
    }

    /// <summary>
    /// 客户端接收到聊天协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgChat(MsgBase msgBase)
    {
        MsgChat msg = (MsgChat)msgBase;

        string addText = "\n  " + "<color=red>" + players[msg.id].username + "</color>: " + msg.chatmsg;
        gamePanel.chatRoom.chatText.text = gamePanel.chatRoom.chatText.text + addText;
        gamePanel.chatRoom.chatInput.ActivateInputField();
        Canvas.ForceUpdateCanvases();       //关键代码
        gamePanel.chatRoom.scrollRect.verticalNormalizedPosition = 0f;  //关键代码
        Canvas.ForceUpdateCanvases();   //关键代码
    }

    /// <summary>
    /// 客户端收到出牌的协议，进行同步
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgChuPai(MsgBase msgBase)
    {
        MsgChuPai msg = (MsgChuPai)msgBase;
        Debug.Log("PlayerId: " + msg.id);
        if (msg.paiIndex == -1)//胡的情况
        {
            Gender gender = players[msg.id].gender;
            switch (gender)
            {
                case Gender.Female:
                    Audio.PlayCue(Audio.audioHuFemale);
                    break;
                case Gender.Male:
                    Audio.PlayCue(Audio.audioHuMale);
                    break;
            }

            Debug.Log("胡牌成功！");
            PanelManager.Open<GameoverPanel>(msg.id, client_id);
            PanelManager.Close("GamePanel");
        }
        else
        {
            players[msg.id].DiscardPai(msg.paiIndex);
        }
    }

    /// <summary>
    /// 客户端接收到吃碰杠的协议
    /// 第一次收到协议，就让玩家操作是否进行吃碰杠，并将结果传给服务端
    /// 第二次收到协议，同步结果
    /// 这两次协议都需要服务端广播
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgChiPengGang(MsgBase msgBase)
    {
        MsgChiPengGang msg = (MsgChiPengGang)msgBase;
        if (msg.result == -1)//第一次收到协议
        {
            StartTimeCount();
            nowTurnid = msg.id;

            players[nowTurnid].msgChiPengGang = msg;
            gamePanel.TurnLight(nowTurnid);
            if (nowTurnid == client_id)
            {
                bool[] isChiPengGang = msg.isChiPengGang;
                if (isChiPengGang[1])
                {
                    gamePanel.ChiButton = true;
                }
                if (isChiPengGang[2])
                {
                    gamePanel.PengButton = true;
                }
                if (isChiPengGang[3])
                {
                    gamePanel.GangButton = true;
                }
            }
        }
        else
        {
            switch (msg.result)
            {
                case 0://取消操作，不需要任何改变
                    break;
                case 1:
                    players[msg.id].OnChi(msg.paiId);
                    break;
                case 2:
                    players[msg.id].OnPeng(msg.paiId);
                    break;
                case 3:
                    players[msg.id].OnGang(msg.paiId);
                    break;
            }
        }
    }

    /// <summary>
    /// 客户端接收化学系的协议
    /// 进行相关的同步，并重新计时，继续让同一个玩家出牌
    /// </summary>
    public void OnMsgChemistry(MsgBase msgBase)
    {
        MsgChemistry msg = (MsgChemistry)msgBase;
        if (msg.id == client_id)
        {
            skillCount--;
            gamePanel.RestSkillCount = skillCount;
            gamePanel.ChuPaiButton = true;
        }
        if (msg.canSkill && msg.id == client_id)//如果可以发动技能，就显示发动技能的按钮
        {
            gamePanel.SkillButton = true;
        }

        players[msg.id].DiscardPai(msg.paiIndex);

        CreatePai(msg.paiId, msg.id);
        players[msg.id].SynHandPai();//同步牌的顺序
        players[msg.id].PlacePai();//调整牌的位置

        Debug.Log("客户端接收msgchemistry信息");
        isChuPai = true;
        
        StartTimeCount();
    }

    /// <summary>
    /// 客户端处理数学系技能的协议
    /// 根据服务端的消息生成对应的牌，并重新开始计时
    /// </summary>
    /// <param name="msgBase"></param>
    public void OnMsgMath(MsgBase msgBase)
    {
        Debug.Log("客户端接收msgMath信息");
        MsgMath msg = (MsgMath)msgBase;
        if (msg.observerPlayerId == client_id)
        {
            skillCount--;
            gamePanel.RestSkillCount = skillCount;
            gamePanel.ChuPaiButton = true;
        }

        gamePanel.DisplayOtherPai(msg.paiId);

        if (msg.canSkill && msg.observerPlayerId == client_id)//如果可以发动技能，就显示发动技能的按钮
        {
            gamePanel.SkillButton = true;
        }
        isChuPai = true;
        StartTimeCount();
    }

    public void StartTimeCount()
    {
        startTimeCount = true;
        timeLast = timeCount;
    }

    public void TimeCount()
    {
        timeLast -= Time.deltaTime;
        gamePanel.SetTimeCount(timeLast);
        if(timeLast <= 3 && !canTimeAlarm && nowTurnid == client_id)
        {
            Audio.PlayCue(Audio.timeup_alarm);
            canTimeAlarm = true;
        }
        if(timeLast <= 0)
        {
            canTimeAlarm = false;//对播放倒计时进行重置
            timeLast = 0;
            startTimeCount = false;
            if(nowTurnid == client_id)
            {
                gamePanel.HideSkillUI();
                if (isChuPai)//代表是自己出牌，到时间了
                {
                    CtrlPlayer self = (CtrlPlayer)players[client_id];
                    self.ChuPai();
                }
                else//该自己进行吃碰杠的判断，且到时间了
                {
                    CtrlPlayer self = (CtrlPlayer)players[client_id];
                    self.ChiPengGang();
                }
            }
            
            Debug.Log("时间到了");
        }
    }

    private void Update()
    {
        if (startTimeCount)
        {
            TimeCount();
            if (isChuPai && !(client_id == nowTurnid && players[client_id].skill == Major.Biology && gamePanel.isDoSkilling == true))//是自己控制的玩家在出牌，而且是数学系的而且已经按下了发动技能的按钮
            {        //发动数学系技能时不可选中自己的牌
                players[nowTurnid].DaPai();//这里人机只是打牌，不发动任何技能
            }//否则进行吃碰杠的判断
        }


        //这个应该放在gameManager中，用于发送聊天的输入框
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (gamePanel.chatRoom.chatInput.text != "")
            {//发送聊天协议
                MsgChat msg = new MsgChat();
                msg.chatmsg = gamePanel.chatRoom.chatInput.text;
                msg.id = client_id;
                gamePanel.chatRoom.chatInput.text = "";
                NetManager.Send(msg);
            }
        }
    }

    /// <summary>
    /// 客户端中实例化牌，并分配给对应的玩家
    /// </summary>
    /// <param name="paiId">需要生成的牌的id</param>
    /// <param name="playerId">玩家的id</param>
    /// <param name="index">牌放在哪里的索引</param>
    public void CreatePai(int paiId, int playerId)
    {
        string path = "";
        Vector3 scale = Vector3.one;
        Direction dir = gamePanel.numToDir[playerId];
        switch (dir)
        {
            case Direction.DOWN:
                path = Pai.pai_player1[paiId];
                break;
            case Direction.RIGHT:
                path = Pai.pai_player2;
                scale = new Vector3(0.6f,0.6f,0);
                break;
            case Direction.UP:
                path = Pai.pai_player3;

                break;
            case Direction.LEFT:
                path = Pai.pai_player4;
                scale = new Vector3(0.7f, 0.7f, 0);
                break;
        }
        GameObject go = ResManager.LoadSprite(path,1);
        players[playerId].handPai.Add(go);
        go.transform.SetParent(players[playerId].transform);
        go.AddComponent<BoxCollider>();
        go.transform.localScale = scale;
        go.tag = "Pai" + playerId;//通过标签选中对应的牌
        go.name = Pai.int2name[paiId];
        Pai pai = go.AddComponent<Pai>();
        pai.paiId = paiId;
    }

    //添加CtrlPlayer和SyncPlayer
    private void InitPlayer(MsgInitData msg)
    {
        int id = msg.id;
        for (int i = 0; i < 4; i++)
        {
            UnityEngine.Object[] obj = null;
            if(i == id)
            {
                obj = playerFactory.CreatePlayer(PlayerName.CtrlPlayer);
                Debug.Log("该客户端的id: " + id);
            }
            else
            {
                obj = playerFactory.CreatePlayer(PlayerName.SyncPlayer);
            }
            GameObject go = (GameObject)obj[0];
            go.name = "Player " + (i);
            BasePlayer bp = (BasePlayer)obj[1];
            bp.Init(gamePanel);
            players[i] = bp;
            players[i].id = i;
            players[i].username = msg.data[i].username;
            players[i].gender = (Gender)msg.data[i].gender;
            players[i].skill = (Major)msg.data[i].skillIndex;

        }
    }

}
