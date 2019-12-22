using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFactory : MonoBehaviour {

	public UnityEngine.Object[] CreatePlayer(PlayerName name)
    {
        GameObject go = new GameObject();
        BasePlayer bp = null;
        switch (name)
        {
            case PlayerName.CtrlPlayer:
                bp = go.AddComponent<CtrlPlayer>();
                break;
            case PlayerName.SyncPlayer:
                bp = go.AddComponent<SyncPlayer>();
                break;

        }
        return new Object[] { go,bp};
    }

    
}
public enum PlayerName
{
    CtrlPlayer,
    SyncPlayer
}