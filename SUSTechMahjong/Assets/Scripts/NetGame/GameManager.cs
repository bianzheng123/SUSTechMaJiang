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
    [SyncVar]
    private GameState nowGameState = GameState.READY;//判断游戏是否正在进行
    [SerializeField]
    private GameObject StartAnim;

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

    /// <summary>
    /// 用于准备阶段切换头像
    /// </summary>
    /// <param name="playerNumber"></param>
    private void ChangeProfile(int playerNumber)
    {
        NetworkGameUI.Instance.ChangeProfile(playerNumber);
        if(playerNumber == 4)
        {
            StartAnim.SetActive(true);
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
    READY_PLAYER,
    WATCH,
    PLAYER1,
    PLAYER2,
    PLAYER3,
    PLAYER4
};
