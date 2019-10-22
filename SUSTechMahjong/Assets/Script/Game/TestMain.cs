using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PanelManager.Init();
        PanelManager.Open<GamePanel>();
        PaiManager_Server.GetInstance().Init();
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
