using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int turn;//存在服务器中的
    public static PaiManager paiManager = new PaiManager();
    public static Queue<MsgChiPengGang> queueChiPengGang;//每一个房间都存放用来判断是否吃碰杠的列表
    

    private GamePanel gamePanel;
    public int id;//该客户端的id
    public BasePlayer[] players;
    private Dictionary<int, Direction> numToDir;
    public const float timeCount = 10;//代表出牌计时的时间
    public bool isChuPai = false;//是否为出牌，还是进行吃碰杠的判断
    public int nowTurnid = 0;
    public bool startGame = false;//目前可能没用，将来也可能没用
    public float timeLast = timeCount;
    public bool startTimeCount = false;

    public GamePanel GamePanel
    {
        set { gamePanel = value; }
    }

    // Use this for initialization
    void Start()
    {
        //发送开始接收游戏数据的协议
        MsgInitData msg = new MsgInitData();
        InitNumToDir();
        gamePanel = GameObject.Find("Root").GetComponent<GamePanel>();
        ServerOnInitData(msg);//向服务器发送协议
    }

    private void InitNumToDir() {
        numToDir = new Dictionary<int, Direction>();
        numToDir[0] = Direction.DOWN;
        numToDir[1] = Direction.RIGHT;
        numToDir[2] = Direction.UP;
        numToDir[3] = Direction.LEFT;
    }

    //添加CtrlPlayer和SyncPlayer
    private void InitPlayer(int id)
    {
        for(int i = 0; i < 4; i++)
        {
            GameObject go = new GameObject("Player" + (i + 1));
            BasePlayer bp = null;
            if (i == id)
            {
                bp = go.AddComponent<CtrlPlayer>();
                bp.Init(this,gamePanel);
                players[i] = bp;
                players[i].id = i;
                Debug.Log("id: " + id);
            }
            else
            {
                bp = go.AddComponent<SyncPlayer>();
                bp.Init(this,gamePanel);
                players[i] = bp;
                players[i].id = i;
            }
        }
    }

    private void InitUI() {
        gamePanel.okButton.onClick.AddListener(gamePanel.OnOkClick);
        gamePanel.cancelButton.onClick.AddListener(gamePanel.OnCancelClick);
    }

    //服务端收到开始接收游戏数据的协议
    public void ServerOnInitData(MsgBase msgBase)
    {
        paiManager.Init();
        MsgInitData msg = (MsgInitData)msgBase;
        //获取骰子的点数
        System.Random rd = new System.Random();
        int zhuangIdx = rd.Next() % 4;
        //int zhuangIdx = 0;
        turn = zhuangIdx;
        //对协议名称进行初始化，这里表述不完全
        msg.data = new StartGameData[4];
        for (int i = 0; i < msg.data.Length; i++)
        {
            msg.data[i] = new StartGameData();
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
        ClientOnInitData(msg);
        Debug.Log("庄家：" + zhuangIdx);
        //for(int i = 0; i < 3; i++)
        //{
        //    msg.id = (zhuangIdx + i) % 4;
        //    ClientOnMsgStartRecieveGameData(msg);
        //}

        //发初始手牌，发送协议
        //广播所有玩家，庄家出牌
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
        InitPlayer(msg.id);
        InitUI();

        //生成牌
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j], i);
            }
            players[i].PlacePai();
        }

        startGame = true;
        MsgFaPai msgFaPai = new MsgFaPai();
        ServerOnMsgFaPai(msgFaPai);
    }

    //服务端收到发牌的协议
    public void ServerOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        msg.id = turn;
        int paiIdx = paiManager.FaPai(turn);
        msg.paiId = paiIdx;
        //发牌协议进行广播
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
            return;
        }
        gamePanel.TurnLight(numToDir[Math.Abs(msg.id - id)]);
        if (msg.id == id)
        {
            gamePanel.ChuPaiButton = true;
        }
        nowTurnid = msg.id;
        CreatePai(msg.paiId,msg.id);

        players[msg.id].PlacePai();//调整牌的位置，以及同步牌的顺序

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
        int paiId = paiManager.ChuPai(paiIndex, id);
        ClientOnMsgChuPai(msg);//广播

        queueChiPengGang = paiManager.HasEvent(paiId, id);

        if(queueChiPengGang.Count != 0)//一直发送吃碰杠协议，直到发完或者有人同意吃碰杠为止
        {
            Debug.Log("存在吃碰杠!");
            foreach (MsgChiPengGang item in queueChiPengGang)
            {
                Debug.Log("执行玩家id: " + item.id + ",打出的牌id: " + item.paiId + ",是否吃: " + item.isChiPengGang[1] + ",是否碰: " + item.isChiPengGang[2] + ",是否杠: " + item.isChiPengGang[3]);
            }
            MsgChiPengGang chiPengGang = queueChiPengGang.Dequeue();
            ClientOnMsgChiPengGang(chiPengGang);
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
        players[msg.id].DiscardPai(msg.paiIndex);
    }

    public void ClientOnMsgChiPengGang(MsgBase msgBase)
    {
        MsgChiPengGang msg = (MsgChiPengGang)msgBase;
        nowTurnid = msg.id;
        gamePanel.TurnLight(numToDir[Math.Abs(nowTurnid - id)]);
        StartTimeCount();
        players[nowTurnid].msg = msg;
        if(nowTurnid != id) { return; }

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

    public void ServerOnMsgChiPengGang(MsgBase msgBase)
    {
        Debug.Log("吃碰杠成功");
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

    private void Update()
    {
        if (startTimeCount)
        {
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
    /// <param name="paiId"></param>
    /// <param name="playerId"></param>
    /// <param name="pos"></param>
    public void CreatePai(int paiId, int playerId)
    {
        string path = Pai.name2path[paiId];
        GameObject go = ResManager.LoadSprite(path,1);
        players[playerId].handPai.Add(go);
        go.transform.SetParent(players[playerId].transform);
        go.AddComponent<BoxCollider>();
        go.tag = "Pai" + playerId;
        Pai pai = go.AddComponent<Pai>();
        pai.paiId = paiId;
    }

    //出牌的协议
    //告知吃碰杠胡的协议，一个list,枚举类,id
    //结束游戏的协议

}
