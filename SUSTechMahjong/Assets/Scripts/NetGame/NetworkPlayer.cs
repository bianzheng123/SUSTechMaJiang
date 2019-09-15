using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour {
    private PlayerType playerType;

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
