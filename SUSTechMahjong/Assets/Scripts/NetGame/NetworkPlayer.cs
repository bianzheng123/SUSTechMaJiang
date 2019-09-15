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

	// Use this for initialization
	private void Start () {
        if (isLocalPlayer)
        {
            CmdSetPlayerOnCreate();
            
        }
	}

    [Command]
    private void CmdSetPlayerOnCreate()
    {
        if(GameManager.Instance.PlayerNum < 4)
        {
            GameManager.Instance.PlayerNum++;
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
        if (isLocalPlayer)
        {
            for(int i = 0; i < cards.Count; i++)
            {
                cards[i].transform.position = new Vector3(i,0,0);
            }
        }
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
