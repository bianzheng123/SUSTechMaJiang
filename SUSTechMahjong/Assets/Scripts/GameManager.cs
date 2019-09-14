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
    public bool gameStart = false;//判断游戏是否开始
    [SerializeField]
    private GameObject StartAnim;
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



}
public enum PlayerType
{
    READY_PLAYER,
    WATCH,
    PLAYER1,
    PLAYER2,
    PLAYER3,
    PLAYER4
};
