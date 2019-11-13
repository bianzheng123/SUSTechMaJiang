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
    public int id;//该客户端的id
    public BasePlayer[] players;
    private Dictionary<int, Direction> numToDir;
    public const float timeCount = 10;//代表出牌计时的时间
    public bool isChuPai = false;//是否为出牌，还是进行吃碰杠的判断
    public int nowTurnid = 0;
    public float timeLast = timeCount;
    public int hostTurnNum = 0;
    public bool startTimeCount = false;
    public int skillCount;//代表当前主机玩家还剩下几次技能可以使用

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
        InitNumToDir();
        gamePanel = GameObject.Find("Root").GetComponent<GamePanel>();
        //发送开始接收游戏数据的协议
        MsgInitData msg = new MsgInitData();
        ServerOnInitData(msg);
    }

    //服务端收到开始接收游戏数据的协议，并对使用技能的次数进行初始化
    public void ServerOnInitData(MsgBase msgBase)
    {
        //服务端的初始化
        doSkillTime = new int[4];
        maxSkillTime = new int[4];
        for (int i = 0; i < 4; i++) {
            doSkillTime[i] = 0;
            maxSkillTime[i] = 3;
        }
        paiManager.Init();

        MsgInitData msg = (MsgInitData)msgBase;
        //获取骰子的点数
        System.Random rd = new System.Random();
        int zhuangIdx = rd.Next() % 4;//随机确定庄家
        turn = zhuangIdx;
        //对协议名称进行初始化，这里表述不完全
        msg.data = new StartGameData[4];
        for (int i = 0; i < msg.data.Length; i++)
        {
            msg.data[i] = new StartGameData();
            msg.data[i].skillIndex = (int)Skill.Chemistry;//现在先全部设置成化学系
            msg.data[i].skillCount = maxSkillTime[i];
        }//初始化协议的牌数组
        for (int i = 0; i < 4; i++)
        {
            int num = 13;
            if (i == zhuangIdx)
            {
                num = 14;
            }
            int[] res = paiManager.FaPai(num, i);
            msg.data[i].paiIndex = res;
        }
        msg.id = 0;
        ClientOnInitData(msg);//广播
        Debug.Log("庄家：" + zhuangIdx);
        //for(int i = 0; i < 3; i++)
        //{
        //    msg.id = (zhuangIdx + i) % 4;
        //    ClientOnMsgStartRecieveGameData(msg);
        //}

        //发初始手牌
        //庄家出牌
    }

    /// <summary>
    /// 客户端接收到初始数据的消息
    /// 只有房主才会发送这个协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnInitData(MsgBase msgBase)
    {
        MsgInitData msg = (MsgInitData)msgBase;
        Pai.Init();
        players = new BasePlayer[4];
        id = msg.id;
        InitPlayer(msg);
        gamePanel.skill = (Skill)msg.data[msg.id].skillIndex;
        gamePanel.SkillDispalyText = (Skill)msg.data[msg.id].skillIndex;
        skillCount = msg.data[msg.id].skillCount;
        gamePanel.RestSkillCount = skillCount;
        //生成牌
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j], i, PlacePaiLocation.HandPai);
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
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        if(msg.paiId == -1)
        {
            Debug.Log("没牌了，游戏结束");
            PanelManager.Open<GameoverPanel>(0,-1,id);
            return;
        }
        hostTurnNum = msg.turnNum;
        gamePanel.SetTurnText("第 " + hostTurnNum + " 轮");
        gamePanel.TurnLight(numToDir[Math.Abs(msg.id - id)]);
        if (msg.id == id)
        {
            gamePanel.ChuPaiButton = true;
            if (msg.isHu)
            {
                gamePanel.HuButton = true;
            }
        }
        
        nowTurnid = msg.id;
        if (msg.canSkill && msg.id == id)//如果可以发动技能，就显示发动技能的按钮
        {
            gamePanel.SkillButton = true;
        }
        CreatePai(msg.paiId,msg.id,PlacePaiLocation.HandPai);

        players[msg.id].SynHandPai();//同步牌的顺序
        players[msg.id].PlacePai();//调整牌的位置

        string arrString = "";
        for (int i = 0; i < paiManager.playerPai[msg.id].Count; i++)
        {
            arrString += paiManager.playerPai[msg.id][i] + " ";
        }
        Debug.Log("player" + msg.id + ": " + arrString);

        isChuPai = true;
        //开始计时，玩家出牌
        StartTimeCount();
        //如果不是自己控制的玩家，就收到协议显示
        //如果是自己的玩家，就打出牌，同时发送协议
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

        queueChiPengGang = paiManager.HasEvent(paiId, id);

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
    /// 客户端收到出牌的协议，进行对应的变化
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgChuPai(MsgBase msgBase)
    {
        MsgChuPai msg = (MsgChuPai)msgBase;
        Debug.Log("PlayerId: " + msg.id);
        if(msg.paiIndex == -1)
        {
            Debug.Log("胡牌成功！");
            if(msg.id == turn)
            {
                //跳转到成功界面
                PanelManager.Open<GameoverPanel>(1,msg.id,id);
            }
            else
            {
                //跳转到失败的界面
                PanelManager.Open<GameoverPanel>(2,msg.id,id);
            }
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
        if(msg.result == -1)
        {
            GetChiPengGangResult(msg);
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
        if(msg.id == id)
        {
            skillCount--;
            gamePanel.RestSkillCount = skillCount;
        }
        players[msg.id].DiscardPai(msg.paiIndex);

        CreatePai(msg.paiId, msg.id, PlacePaiLocation.HandPai);

        players[msg.id].SynHandPai();//同步牌的顺序
        players[msg.id].PlacePai();//调整牌的位置
        Debug.Log(msg.canSkill && msg.id == id);

        if (msg.canSkill && msg.id == id)//如果可以发动技能，就显示发动技能的按钮
        {
            gamePanel.SkillButton = true;
        }
        isChuPai = true;
        Debug.Log("客户端接收msgchemistry信息");
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
        if(timeLast <= 0)
        {
            timeLast = 0;
            startTimeCount = false;
            if(nowTurnid == id)
            {
                if (isChuPai)//代表是自己出牌，到时间了
                {
                    CtrlPlayer self = (CtrlPlayer)players[id];
                    self.DaPaiCompolsory();
                }
                else//该自己进行吃碰杠的判断，且到时间了
                {
                    CtrlPlayer self = (CtrlPlayer)players[id];
                    self.ChiPengGang();
                }
            }
            
            Debug.Log("时间到了");
        }
    }

    private void GetChiPengGangResult(MsgChiPengGang msg)
    {
        nowTurnid = msg.id;
        gamePanel.TurnLight(numToDir[Math.Abs(nowTurnid - id)]);
        StartTimeCount();
        players[nowTurnid].msgChiPengGang = msg;
        if (nowTurnid != id) { return; }

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

    private void Update()
    {
        if (startTimeCount)
        {
            Debug.Log("timecount");
            TimeCount();
            if (isChuPai)
            {
                players[nowTurnid].DaPai();
            }//否则进行吃碰杠的判断
            else if(nowTurnid != id)
            {//用于人机的操作
                players[nowTurnid].ChiPengGang();
            }
        }
        
    }

    /// <summary>
    /// 客户端中实例化牌，并分配给对应的玩家
    /// </summary>
    /// <param name="paiId">需要生成的牌的id</param>
    /// <param name="playerId">玩家的id</param>
    /// <param name="index">牌放在哪里的索引</param>
    public void CreatePai(int paiId, int playerId, PlacePaiLocation location)
    {
        string path = Pai.name2path[paiId];
        GameObject go = ResManager.LoadSprite(path,1);
        switch (location)
        {
            case PlacePaiLocation.HandPai:
                players[playerId].handPai.Add(go);
                break;
            case PlacePaiLocation.Chi:
                players[playerId].chi.Add(go);
                break;
            case PlacePaiLocation.Peng:
                players[playerId].peng.Add(go);
                break;
            case PlacePaiLocation.Gang:
                players[playerId].gang.Add(go);
                break;
        }
        go.transform.SetParent(players[playerId].transform);
        go.AddComponent<BoxCollider>();
        go.tag = "Pai" + playerId;
        Pai pai = go.AddComponent<Pai>();
        pai.paiId = paiId;
    }

    private void InitNumToDir()
    {
        numToDir = new Dictionary<int, Direction>();
        numToDir[0] = Direction.DOWN;
        numToDir[1] = Direction.RIGHT;
        numToDir[2] = Direction.UP;
        numToDir[3] = Direction.LEFT;
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
                obj = playerFactory.createPlayer(PlayerName.CtrlPlayer);
                Debug.Log("id: " + id);
            }
            else
            {
                obj = playerFactory.createPlayer(PlayerName.SyncPlayer);
            }
            GameObject go = (GameObject)obj[0];
            go.name = "Player " + (i + 1);
            BasePlayer bp = (BasePlayer)obj[1];
            bp.Init(gamePanel);
            players[i] = bp;
            players[i].id = i;
            players[i].skill = (Skill)msg.data[i].skillIndex;

        }
    }

}

public enum PlacePaiLocation
{
    HandPai,
    Chi,
    Peng,
    Gang
}
