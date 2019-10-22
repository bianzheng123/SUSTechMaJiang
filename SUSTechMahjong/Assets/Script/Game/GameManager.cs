using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static int turn;
    public static PaiManager paiManager = new PaiManager();

    private GamePanel gamePanel;
    public int id;//该客户端的id
    public BasePlayer[] players;
    private Dictionary<int, Direction> numToDir;
    
    public const float timeInterval = 6;//代表出牌的最大时间间隔
    public float frame_timeInterval = 6;
    public bool startGame = false;
    public int turnId = 0;//代表现在到谁了

    public GamePanel GamePanel
    {
        set { gamePanel = value; }
    }

    // Use this for initialization
    void Start()
    {
        //发送开始接收游戏数据的协议
        MsgStartReceiveGameData msg = new MsgStartReceiveGameData();
        InitNumToDir();
        gamePanel = GameObject.Find("Root").GetComponent<GamePanel>();
        ServerOnMsgStartRecieveGameData(msg);//向服务器发送协议
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
                bp.Init();
                players[i] = bp;
                players[i].id = i;
                Debug.Log("id: " + id);
            }
            else
            {
                bp = go.AddComponent<SyncPlayer>();
                bp.Init();
                players[i] = bp;
                players[i].id = i;
            }
        }
    }

    /// <summary>
    /// 客户端接收到初始数据的消息
    /// 只有房主才会发送这个协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ClientOnMsgStartRecieveGameData(MsgBase msgBase)
    {
        MsgStartReceiveGameData msg = (MsgStartReceiveGameData)msgBase;
        Pai.Init();
        players = new BasePlayer[4];
        id = msg.id;
        InitPlayer(msg.id);

        //生成牌
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                CreatePai(msg.data[i].paiIndex[j],i,new Vector3(j-6,-1-i,0));
            }

            if(msg.data[i].paiIndex.Length == 14)
            {
                turnId = i;
            }
        }

        startGame = true;
        MsgFaPai msgFaPai = new MsgFaPai();
        ServerOnMsgFaPai(msgFaPai);
        Debug.Log(turnId - id);
        Debug.Log(gamePanel == null);
        gamePanel.TurnLight(numToDir[Math.Abs(turnId - id)]);
        
    }

    //服务端收到开始接收游戏数据的协议
    public void ServerOnMsgStartRecieveGameData(MsgBase msgBase)
    {
        paiManager.Init();
        MsgStartReceiveGameData msg = (MsgStartReceiveGameData)msgBase;
        //获取骰子的点数
        System.Random rd = new System.Random();
        int zhuangIdx = rd.Next() % 4;
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
        ClientOnMsgStartRecieveGameData(msg);
        Debug.Log("庄家：" + zhuangIdx);
        //for(int i = 0; i < 3; i++)
        //{
        //    msg.id = (zhuangIdx + i) % 4;
        //    gameManager.OnMsgStartRecieveGameData(msg);
        //}


        //
        //发初始手牌，发送协议
        //广播所有玩家，庄家出牌
    }

    //收到发牌的协议
    public void ServerOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        msg.id = turn;
        int[] paiIdx = paiManager.FaPai(1, turn);
        msg.paiIndex = paiIdx[0];
        ClientOnMsgFaPai(msg);
    }

    //处理MsgFaPai协议
    public void ClientOnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        Vector3 pos = new Vector3(0,0,0);
        switch (msg.id)
        {
            case 0:
                pos = new Vector3(8,-1,0);
                break;
            case 1:
                pos = new Vector3(8, -2, 0);
                break;
            case 2:
                pos = new Vector3(8, -3, 0);
                break;
            case 3:
                pos = new Vector3(8, -4, 0);
                break;
        }
        CreatePai(msg.paiIndex,msg.id,pos);
    }

    //实例化牌，并分配给对应的玩家
    public void CreatePai(int paiId, int playerId, Vector3 pos)
    {
        string path = Pai.name2path[paiId];
        Sprite s = ResManager.LoadSprite(path);
        string[] name = path.Split('/');
        GameObject go = new GameObject(name[name.Length - 1]);
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 1;
        sr.sprite = s;
        players[playerId].pai.Add(go);
        go.transform.SetParent(players[playerId].transform);
        go.transform.position = pos;
        Pai pai = go.AddComponent<Pai>();
        pai.paiId = paiId;
    }

    //在收到玩家出的牌的协议
    public static void OnMsgShoudaoPai()
    {
        //更新牌库
        //调用PaiManager_Server判断吃碰杠胡
    }

    //出牌的协议
    //告知吃碰杠胡的协议，一个list,枚举类,id
    //结束游戏的协议

}
