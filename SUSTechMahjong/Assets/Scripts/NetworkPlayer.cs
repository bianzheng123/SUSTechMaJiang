using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class NetworkPlayer : NetworkBehaviour {

	// Use this for initialization
	private void Start () {
        if (isLocalPlayer)
        {
            CmdSetPlayer();
            
        }
	}

    [Command]
    private void CmdSetPlayer()
    {
        NetworkData.Instance.PlayerNum++;
    }
}
