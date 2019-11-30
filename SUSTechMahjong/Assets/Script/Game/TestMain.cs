using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        PanelManager.Init();
        Pai.Init();
        Audio.Init();
        PanelManager.Open<GamePanel>();
	}

	
	// Update is called once per frame
	void Update () {
		
	}
}
