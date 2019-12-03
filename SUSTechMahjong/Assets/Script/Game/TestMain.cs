﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMain : MonoBehaviour {

	// Use this for initialization
	void Start () {
        Audio.Init();
        Pai.Init();
        XMLManager.Init();
        PanelManager.Init();
        PanelManager.Open<GamePanel>();
        
    }

	
	// Update is called once per frame
	void Update () {
		
	}
}
