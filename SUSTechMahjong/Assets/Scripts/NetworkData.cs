using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetworkData : NetworkBehaviour {
    private static NetworkData _instance;
    public static NetworkData Instance
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
    public int PlayerNum
    {
        get { return playerNum; }
        set { playerNum = value; }
    }

    private void ChangeProfile(int playerNumber)
    {
        Transform[] childs = new Transform[4];
        for (int i = 1; i <= 4; i++)
        {
            childs[i - 1] = NetworkGameUI.Instance.transform.Find(i.ToString() + "Players");
            childs[i - 1].gameObject.SetActive(false);
        }
        switch (playerNumber)
        {
            case 1:
                childs[0].gameObject.SetActive(true);
                break;
            case 2:
                childs[1].gameObject.SetActive(true);
                break;
            case 3:
                childs[2].gameObject.SetActive(true);
                break;
            case 4:
                childs[3].gameObject.SetActive(true);
                break;
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
