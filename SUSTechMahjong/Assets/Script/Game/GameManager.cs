using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;

    private GameManager() { }


    public static GameManager GetInstance()
    {
        if (_instance == null)
        {
            _instance = new GameManager();
        }
        return _instance;
    }//单例

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
        Server.OnMsgStartRecieveGameData(msg);//向服务器发送协议
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

    //指客户端接收到初始数据的消息
    public void OnMsgStartRecieveGameData(MsgBase msgBase)
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
        Server.OnMsgFaPai(msgFaPai);
        Debug.Log(turnId - id);
        Debug.Log(gamePanel == null);
        gamePanel.TurnLight(numToDir[Math.Abs(turnId - id)]);
        
    }

    //处理MsgFaPai协议
    public void OnMsgFaPai(MsgBase msgBase)
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

    // Update is called once per frame
    void Update()
    {

    }

}
