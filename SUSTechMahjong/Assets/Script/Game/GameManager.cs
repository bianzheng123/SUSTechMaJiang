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

    //static关键字代表这些变量存在服务器中
    public static int turn;//代表现在轮到谁了
    public static PaiManager paiManager = new PaiManager();
    public static Queue<MsgChiPengGang> queueChiPengGang;//每一个房间都存放用来判断是否吃碰杠的列表
    public static int turnNum = 1;//代表现在是第几轮
    public static int turnCount = 0;//每当服务端发送一次出牌协议就调用一次，用来判断是第几轮了
    public static int[] doSkillTime;//代表每一个玩家已经用过技能的次数
    public static int[] maxSkillTime;//代表每一个玩家在一局中最多使用技能的次数

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

    public void ProcessInitData(MsgInitData msg)
    {
        players = new BasePlayer[4];
        client_id = msg.id;
        gamePanel.InitNumToDir(msg.id);
        InitPlayer(msg);
        gamePanel.Skill = (Major)msg.data[msg.id].skillIndex;
        skillCount = msg.data[msg.id].skillCount;
        gamePanel.RestSkillCount = skillCount;
        //gamePanel中先给Skill进行赋值，才能给RestSkillCount进行赋值

        //生成牌
        for (int i = 0; i < 4; i++)
        {
            if (msg.data[i].paiIndex.Length == 14)
            {
                gamePanel.ZhuangImage = i;
            }
            for (int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j], i);
            }
            players[i].PlacePai();
        }
    }

    /// <summary>
    /// 客户端接收到初始数据的消息
    /// 只有房主才会发送这个协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnInitData(MsgBase msgBase)
    {
        MsgInitData msg = (MsgInitData)msgBase;
        players = new BasePlayer[4];
        client_id = msg.id;
        gamePanel.InitNumToDir(msg.id);
        InitPlayer(msg);
        gamePanel.Skill = (Major)msg.data[msg.id].skillIndex;
        skillCount = msg.data[msg.id].skillCount;
        gamePanel.RestSkillCount = skillCount;
        //gamePanel中先给Skill进行赋值，才能给RestSkillCount进行赋值
        
        //生成牌
        for (int i = 0; i < 4; i++)
        {
            if(msg.data[i].paiIndex.Length == 14)
            {
                gamePanel.ZhuangImage = i;
            }
            for (int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j], i);
            }
            players[i].PlacePai();
        }

        //发送发牌协议
        MsgFaPai msgFaPai = new MsgFaPai();
        ServerOnMsgFaPai(msgFaPai);
        //可以不发这个协议
    }

    //服务端收到发牌的协议
    public void ServerOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        msg.id = turn;
        msg.turnNum = turnNum;
        msg.canSkill = doSkillTime[turn] < maxSkillTime[turn] ? true : false;//表示对于当前玩家能否使用技能
        turnCount++;
        if (turnCount % 4 == 0)
        {
            turnCount = 0;
            turnNum++;
        }//判断下一次出牌是在第几轮
        int paiIdx = paiManager.FaPai(turn);
        msg.paiId = paiIdx;
        if (paiManager.HasHu(turn))
        {
            msg.isHu = true;
        }
        else
        {
            msg.isHu = false;
        }
        //广播
        ClientOnMsgFaPai(msg);
    }

    /// <summary>
    /// 客户端处理MsgFaPai协议
    /// 客户端进行牌的同步与显示，如果是自己出牌，就显示对应的UI
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        nowTurnid = msg.id;
        if (msg.paiId == -1)
        {
            Debug.Log("没牌了，游戏结束");
            PanelManager.Open<GameoverPanel>(0,-1,client_id);
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
        
        CreatePai(msg.paiId,msg.id);
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
    /// 服务端收到出牌的协议，并广播给所有玩家
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgChuPai(MsgBase msgBase)
    {
        MsgChuPai msg = (MsgChuPai)msgBase;
        int paiIndex = msg.paiIndex;//牌在这个玩家的索引
        int id = msg.id;//出牌的玩家id
        ClientOnMsgChuPai(msg);//广播
        if (paiIndex == -1)
        {
            //服务端执行胡的操作，清空数据，写入数据库等
            return;
        }

        int paiId = paiManager.ChuPai(paiIndex, id);

        queueChiPengGang = paiManager.HasEvent(paiId, id);//检测是否有吃碰杠这件事

        if(queueChiPengGang.Count != 0)//一直发送吃碰杠协议，直到发完或者有人同意吃碰杠为止
        {
            Debug.Log("存在吃碰杠!");
            foreach (MsgChiPengGang item in queueChiPengGang)
            {
                Debug.Log(item.ToString());
            }
            MsgChiPengGang chiPengGang = queueChiPengGang.Dequeue();
            ClientOnMsgChiPengGang(chiPengGang);//广播
        }
        else
        {
            MsgFaPai msgFaPai = new MsgFaPai();
            turn = (turn + 1) % 4;
            ServerOnMsgFaPai(msgFaPai);
        }
    }

    /// <summary>
    /// 客户端收到出牌的协议，进行同步
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgChuPai(MsgBase msgBase)
    {
        MsgChuPai msg = (MsgChuPai)msgBase;
        Debug.Log("PlayerId: " + msg.id);
        if(msg.paiIndex == -1)//胡的情况
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
            if(msg.id == turn)
            {
                //跳转到成功界面
                PanelManager.Open<GameoverPanel>(1,msg.id,client_id);
            }
            else
            {
                //跳转到失败的界面
                PanelManager.Open<GameoverPanel>(2,msg.id,client_id);
            }
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
    public void ClientOnMsgChiPengGang(MsgBase msgBase)
    {
        MsgChiPengGang msg = (MsgChiPengGang)msgBase;
        if(msg.result == -1)//第一次收到协议
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
    /// 服务端接收到吃碰杠的协议，对服务端的牌进行同步，并将本轮吃碰杠的结果进行广播
    /// 如果本轮没有进行吃碰杠，就轮到下一个玩家判断
    /// 如果有人进行了吃碰杠，就轮到下一个人进行发牌
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgChiPengGang(MsgBase msgBase)
    {
        MsgChiPengGang msg = (MsgChiPengGang)msgBase;

        ClientOnMsgChiPengGang(msg);//对得到的本轮结果进行广播

        //取消操作，就发送下一个吃碰杠协议
        if (msg.result == 0 && queueChiPengGang.Count > 0)
        {
            MsgChiPengGang msgChiPengGang = queueChiPengGang.Dequeue();
            ClientOnMsgChiPengGang(msgChiPengGang);//广播
            return;
        }

        switch (msg.result)
        {
            case 0://这里的queue一定为空，不需要进行任何的改变
                break;
            case 1://更新牌库
                if(paiManager.OnChi(msg.id, msg.paiId) == false)
                {
                    Debug.Log("更新牌库吃出现了bug");
                }
                break;
            case 2:
                if (paiManager.OnPeng(msg.id, msg.paiId) == false)
                {
                    Debug.Log("更新牌库碰出现了bug");
                }
                break;
            case 3:
                if (paiManager.OnGang(msg.id, msg.paiId) == false)
                {
                    Debug.Log("更新牌库杠出现了bug");
                }
                break;
        }
        MsgFaPai msgFaPai = new MsgFaPai();
        turn = (turn + 1) % 4;
        ServerOnMsgFaPai(msgFaPai);
    }

    /// <summary>
    /// 服务端接收化学系的协议
    /// 删除指定的牌，添加一个额外的牌，并广播，使得每一个客户端开始
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgChemistry(MsgBase msgBase)
    {
        MsgChemistry msg = (MsgChemistry)msgBase;
        int paiId = paiManager.ChuPai(msg.paiIndex,msg.id);//删除指定的牌
        Debug.Log("删除牌的id为 " + paiId);
        paiId = paiManager.FaPai(msg.id);
        Debug.Log("重新得到的牌id为 " + paiId);
        msg.paiId = paiId;
        doSkillTime[msg.id]++;
        msg.canSkill = doSkillTime[msg.id] < maxSkillTime[msg.id] ? true : false;
        ClientOnMsgChemistry(msg);
        //广播
    }

    /// <summary>
    /// 客户端接收化学系的协议
    /// 进行相关的同步，并重新计时，继续让同一个玩家出牌
    /// </summary>
    public void ClientOnMsgChemistry(MsgBase msgBase)
    {
        MsgChemistry msg = (MsgChemistry)msgBase;
        if(msg.id == client_id)
        {
            skillCount--;
            gamePanel.RestSkillCount = skillCount;
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
    /// 服务端处理数学系技能的协议
    /// 给客户端发送这些牌id的数组
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgMath(MsgBase msgBase)
    {
        MsgMath msg = (MsgMath)msgBase;
        msg.paiId = paiManager.GetPaiIdRandom(msg.observedPlayerId,turnNum);
        doSkillTime[msg.observerPlayerId]++;
        msg.canSkill = doSkillTime[msg.observerPlayerId] < maxSkillTime[msg.observerPlayerId] ? true : false;
        ClientOnMsgMath(msg);//广播
    }

    /// <summary>
    /// 客户端处理数学系技能的协议
    /// 根据服务端的消息生成对应的牌，并重新开始计时
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgMath(MsgBase msgBase)
    {
        Debug.Log("客户端接收msgMath信息");
        MsgMath msg = (MsgMath)msgBase;
        if (msg.observerPlayerId == client_id)
        {
            skillCount--;
            gamePanel.RestSkillCount = skillCount;
        }

        gamePanel.DisplayOtherPai(msg.paiId);

        if (msg.canSkill && msg.observerPlayerId == client_id)//如果可以发动技能，就显示发动技能的按钮
        {
            gamePanel.SkillButton = true;
        }
        isChuPai = true;
        StartTimeCount();
    }

    /// <summary>
    /// 服务端接收到计算机系协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgComputerScience(MsgBase msgBase)
    {
        MsgComputerScience msg = (MsgComputerScience)msgBase;
        int paiIndex = msg.paiIndex;//牌在这个玩家的索引
        int id = msg.id;//出牌的玩家id
        MsgChuPai msgChuPai = new MsgChuPai();
        msgChuPai.id = msg.id;
        msgChuPai.paiIndex = msg.paiIndex;
        ClientOnMsgChuPai(msgChuPai);//广播
        if (paiIndex == -1)
        {
            //服务端执行胡的操作，清空数据，写入数据库等
            return;
        }

        int paiId = paiManager.ChuPai(paiIndex, id);
        queueChiPengGang = paiManager.HasEvent(paiId, id);//检测是否有吃碰杠这件事

        if (queueChiPengGang.Count != 0)//一直发送吃碰杠协议，直到发完或者有人同意吃碰杠为止
        {
            Debug.Log("存在吃碰杠,但是计算机系发动的技能，不产生吃碰杠协议");
        }
        MsgFaPai msgFaPai = new MsgFaPai();
        turn = (turn + 1) % 4;

        ServerOnMsgFaPai(msgFaPai);
    }

    /// <summary>
    /// 服务端接收到协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ServerOnMsgChat(MsgBase msgBase)
    {
        MsgChat msg = (MsgChat)msgBase;
        ClientOnMsgChat(msg);
        Debug.Log(msg.chatmsg + "  " + msg.id);
    }

    /// <summary>
    /// 客户端接收到协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgChat(MsgBase msgBase)
    {
        MsgChat msg = (MsgChat)msgBase;

        string addText = "\n  " + "<color=red>" + players[msg.id].username + "</color>: " + msg.chatmsg;
        gamePanel.chatRoom.chatText.text = gamePanel.chatRoom.chatText.text + addText;
        gamePanel.chatRoom.chatInput.ActivateInputField();
        Canvas.ForceUpdateCanvases();       //关键代码
        gamePanel.chatRoom.scrollRect.verticalNormalizedPosition = 0f;  //关键代码
        Canvas.ForceUpdateCanvases();   //关键代码
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
        if(timeLast <= 3 && !canTimeAlarm)
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
            if (isChuPai && !(client_id == nowTurnid && players[client_id].skill == Major.Math && gamePanel.isDoSkilling == true))//是自己控制的玩家在出牌，而且是数学系的而且已经按下了发动技能的按钮
            {        //发动数学系技能时不可选中自己的牌
                players[nowTurnid].DaPai();//这里人机只是打牌，不发动任何技能
            }//否则进行吃碰杠的判断
            else if(nowTurnid != client_id)
            {//用于人机的操作
                players[nowTurnid].ChiPengGang();
            }
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
                ServerOnMsgChat(msg);
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
