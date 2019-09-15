using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour {
    [SerializeField]
    private PlayerType playerType;
    [SerializeField]
    private List<Card> cards = new List<Card>();
    private int index;//代表玩家的编号
    private Text playerIdeneity;

    public List<Card> Cards
    {
        get { return cards; }
        set { cards = value; }
    }

    public PlayerType PlayType
    {
        get { return playerType; }
        set { playerType = value; }
    }
    public int Index
    {
        get { return index; }
        set { index = value; }
    }

	// Use this for initialization
	private void Start () {
        if (isLocalPlayer)
        {
            CmdSetPlayerOnCreate();
            playerIdeneity = GameObject.Find("Text").GetComponent<Text>();
        }
        
    }

    [Command]
    private void CmdSetPlayerOnCreate()
    {
        if(GameManager.Instance.PlayerNum < 4)
        {
            GameManager.Instance.PlayerNum++;
            Debug.Log("play num: " + GameManager.Instance.PlayerNum);
            playerType = PlayerType.READY_PLAYER;
        }
        else
        {
            playerType = PlayerType.WATCH;
        }
    }

    /// <summary>
    /// 将卡片展示出来
    /// </summary>
    public void DisplayCard()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        List<Card> play_self = GameManager.Instance.Players[index].GetComponent<NetworkPlayer>().cards;
        int len = play_self.Count;
        for (int i = 0; i < len; i++)
        {
            play_self[i].transform.position = new Vector3(-5.0f + 0.7f * i, -4.0f, 0);
        }

        List<Card> play_left = GameManager.Instance.Players[(index - 1 + 4) % 4].GetComponent<NetworkPlayer>().cards;
        len = play_left.Count;
        for (int i = 0; i < len; i++)
        {
            play_left[i].transform.position = new Vector3(-5.5f, -3.5f + 0.9f * i, 0);
        }

        List<Card> play_right = GameManager.Instance.Players[(index + 1) % 4].GetComponent<NetworkPlayer>().cards;
        len = play_right.Count;
        for (int i = 0; i < len; i++)
        {
            play_right[i].transform.position = new Vector3(5.5f, -3.5f + 0.9f * i, 0);
        }

        List<Card> play_mid = GameManager.Instance.Players[(index + 2) % 4].GetComponent<NetworkPlayer>().cards;
        len = play_mid.Count;
        for (int i = 0; i < len; i++)
        {
            play_mid[i].transform.position = new Vector3(-5.0f + 0.7f * i, 4.0f, 0);
        }
        playerIdeneity.text = "你是" + this.playerType + "," + index;
    }

    public override void OnNetworkDestroy()
    {
        CmdSetPlayerOnDestroy();
    }

    [Command]
    private void CmdSetPlayerOnDestroy()
    {
        if (playerType != PlayerType.WATCH)
        {
            GameManager.Instance.PlayerNum--;
        }
    }
}
