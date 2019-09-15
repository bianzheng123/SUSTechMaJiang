using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : NetworkBehaviour {
    private static GameManager _instance;
    public static GameManager Instance
    {
        get { return _instance; }

    }//单例

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
    }

    [SerializeField]//为了能在Inspector面板中看到数值变化
    [SyncVar(hook = "ChangeProfile")]
    private int playerNum = 0;//统计玩家的数量
    [SerializeField]
    [SyncVar(hook = "ControlGameUI")]
    private GameState nowGameState = GameState.READY;//判断游戏是否正在进行
    [SerializeField]
    private GameObject StartAnim;
    private GameObject[] players;
    [SerializeField]
    private GameObject turnTable;

    public GameState NowGameState
    {
        get { return nowGameState; }
        set { nowGameState = value; }
    }
    public int PlayerNum
    {
        get { return playerNum; }
        set { playerNum = value; }
    }
    public GameObject[] Players
    {
        get { return players; }
        set { players = value; }
    }

    /// <summary>
    /// 用于准备阶段切换头像
    /// </summary>
    /// <param name="playerNumber"></param>
    private void ChangeProfile(int playerNumber)
    {
        NetworkGameUI.Instance.ChangeProfile(playerNumber);
        if(playerNumber == 4)
        {
            StartAnim.SetActive(true);//开始游戏
        }
    }

    /// <summary>
    /// 一开始设置为不可见，当游戏开始时就设置为可见
    /// </summary>
    /// <param name="state">现在的状态</param>
    private void ControlGameUI(GameState state)
    {
        if(state == GameState.DEAL_CARDS)
        {
            Instantiate(turnTable, Vector3.zero, Quaternion.identity);
        }
        
    }

    /// <summary>
    /// 确定出牌顺序，以及谁是庄家
    /// </summary>
    private void AllocatePlayer()
    {
        players = GameObject.FindGameObjectsWithTag("Player");

        NetworkPlayer tmp = players[0].GetComponent<NetworkPlayer>();
        tmp.PlayType = PlayerType.PLAYER1;
        Debug.Log(tmp.PlayType);
        tmp.Index = 0;

        tmp = players[1].GetComponent<NetworkPlayer>();
        tmp.PlayType = PlayerType.PLAYER2;
        Debug.Log(tmp.PlayType);
        tmp.Index = 1;

        tmp = players[2].GetComponent<NetworkPlayer>();
        tmp.PlayType = PlayerType.PLAYER3;
        Debug.Log(tmp.PlayType);
        tmp.Index = 2;

        tmp = players[3].GetComponent<NetworkPlayer>();
        tmp.PlayType = PlayerType.PLAYER4;
        Debug.Log(tmp.PlayType);
        tmp.Index = 3;
    }

    /// <summary>
    /// 将棋牌显示到屏幕上
    /// </summary>
    private void DisplayCard()
    {
        for(int i = 0; i < players.Length; i++)
        {
            players[i].GetComponent<NetworkPlayer>().DisplayCard();
        }
    }

    /// <summary>
    /// 发牌
    /// </summary>
    private void AllocateCard()
    {
        int index = 0;//记录牌发放到第几个
        for(int i = 0; i < players.Length; i++)
        {
            int len = 13;//记录对每一个人发放多少张牌
            if(players[i].GetComponent<NetworkPlayer>().PlayType == PlayerType.PLAYER1)
            {
                len = 14;//对庄家发14张牌
            }
            for(int j = 0; j < len; j++)
            {
                CardManager.Instance.Cards[index + j].transform.SetParent(players[i].transform);
                players[i].GetComponent<NetworkPlayer>().Cards.Add(CardManager.Instance.Cards[index + j]);
            }
            index += len;
        }

    }

    /// <summary>
    /// 在hide方法执行完成后，此时gameStart为true
    /// </summary>
    private void Update()
    {
        switch (nowGameState)//READY阶段体现在别的类中，所以这里不显示
        {
            case GameState.DEAL_CARDS://发牌
                CardManager.Instance.InitializeCard();
                AllocatePlayer();
                AllocateCard();
                DisplayCard();
                nowGameState = GameState.PLAY_CARDS;
                break;
            case GameState.PLAY_CARDS://打牌
                break;
            case GameState.END://结算
                break;
        }
    }
}

/// <summary>
/// 游戏的状态
/// </summary>
public enum GameState
{
    /// <summary>
    /// 准备阶段，用于凑齐4个人
    /// </summary>
    READY,
    /// <summary>
    /// 发牌阶段
    /// </summary>
    DEAL_CARDS,
    /// <summary>
    /// 打牌阶段
    /// </summary>
    PLAY_CARDS,
    /// <summary>
    /// 结算阶段
    /// </summary>
    END
};

public enum PlayerType
{
    WATCH = 0,
    READY_PLAYER =1,
    /// <summary>
    /// 默认Player1为庄家
    /// </summary>
    PLAYER1=2,
    PLAYER2=3,
    PLAYER3=4,
    PLAYER4=5
};
