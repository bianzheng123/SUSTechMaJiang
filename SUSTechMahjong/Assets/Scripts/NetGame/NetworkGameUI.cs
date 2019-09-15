using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class NetworkGameUI : NetworkBehaviour {
    private static NetworkGameUI _instance;
    public static NetworkGameUI Instance
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

    /// <summary>
    /// 用于准备阶段切换头像
    /// </summary>
    /// <param name="playerNumber"></param>
    public void ChangeProfile(int playerNumber)
    {
        Transform[] childs = new Transform[4];
        for (int i = 1; i <= 4; i++)
        {
            childs[i - 1] = transform.Find(i.ToString() + "Players");
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

    /// <summary>
    /// 用于在准备阶段或者活动进行的阶段退出
    /// </summary>
    public void OnQuitBtn()
    {
        NetworkManager.singleton.matchMaker.DropConnection(NetworkManager.singleton.matchInfo.networkId, NetworkManager.singleton.matchInfo.nodeId, 0, NetworkManager.singleton.OnDropConnection);
        NetworkManager.singleton.StopHost();
    }

}
