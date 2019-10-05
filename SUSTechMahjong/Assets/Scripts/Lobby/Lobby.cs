using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lobby : MonoBehaviour {

	void Start () {
        NetManager.AddListener("CreateRoom", OnCreateRoom);
        SendOnCreateRoom();
    }
	
    private string SendOnCreateRoom()
    {
        string desc = "CreateRoom|" + NetManager.GetDesc();
        return desc;
    }

    private void OnCreateRoom(string msg)
    {

    }

    private void Update()
    {
        NetManager.Update();
    }
}
