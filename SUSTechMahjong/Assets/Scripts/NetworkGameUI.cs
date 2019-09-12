using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

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

  
}
