using System;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //得到Server的引用，最终版本删除
    public Server server;
    public int id;
    public BasePlayer[] players;

    // Use this for initialization
    void Start()
    {
        //发送开始接收游戏数据的协议
        MsgStartReceiveGameData msg = new MsgStartReceiveGameData();
        server.OnMsgStartRecieveGameData(msg);//向服务器发送协议
        
    }

    //添加CtrlPlayer和SyncPlayer
    private void InitPlayer(int id)
    {
        GameObject[] go_players = new GameObject[4];
        for(int i = 0; i < go_players.Length; i++)
        {
            go_players[i] = new GameObject("Player" + (i + 1));
            if(i == id)
            {
                BasePlayer bp = go_players[i].AddComponent<CtrlPlayer>();
                bp.Init();
                players[i] = go_players[i].GetComponent<CtrlPlayer>();
                Debug.Log("id: " + id);
            }
            else
            {
                BasePlayer bp = go_players[i].AddComponent<SyncPlayer>();
                bp.Init();
                players[i] = go_players[i].GetComponent<SyncPlayer>();
            }   
        }
    }

    //只客户端接收到初始数据的消息
    public void OnMsgStartRecieveGameData(MsgStartReceiveGameData msg)
    {
        Pai.Init();
        players = new BasePlayer[4];
        id = msg.id;
        InitPlayer(msg.id);

        
        for(int i = 0; i < 4; i++)
        {
            for(int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                string path = Pai.name2path[msg.data[i].paiIndex[j]];
                Sprite s = ResManager.LoadSprite(path);
                string[] name = path.Split('/');
                GameObject go = new GameObject(name[name.Length-1]);
                SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
                sr.sortingOrder = 1;
                sr.sprite = s;
                players[i].pai.Add(go);
                go.transform.SetParent(players[i].transform);
                go.transform.position = new Vector3(j-6,-1-i,0);
            }
        }

        for (int i = 0; i < 4; i++)
        {
            string s = "";
            for(int j = 0; j < msg.data[i].paiIndex.Length; j++)
            {
                s += msg.data[i].paiIndex[j] + " ";
            }
            Debug.Log("i: " + i + " ,s = " + s + ",length = " + msg.data[i].paiIndex.Length);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
