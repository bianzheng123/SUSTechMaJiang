using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverPanel : BasePanel
{
    //显示玩家的文本
    private Text[] players;
    //每个玩家的结果
    private Text[] results;
    //显示成功或失败的UI，不同玩家显示不同的界面
    private Image[] title;
    //返回按钮
    private Button okButton;

    //初始化
    public override void OnInit()
    {
        skinPath = "GameoverPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        players = new Text[4];
        players[0] = skin.transform.Find("Players/Player1").GetComponent<Text>();
        players[1] = skin.transform.Find("Players/Player2").GetComponent<Text>();
        players[2] = skin.transform.Find("Players/Player3").GetComponent<Text>();
        players[3] = skin.transform.Find("Players/Player4").GetComponent<Text>();
        results = new Text[4];
        results[0] = skin.transform.Find("PlayersResult/Player1").GetComponent<Text>();
        results[1] = skin.transform.Find("PlayersResult/Player2").GetComponent<Text>();
        results[2] = skin.transform.Find("PlayersResult/Player3").GetComponent<Text>();
        results[3] = skin.transform.Find("PlayersResult/Player4").GetComponent<Text>();
        title = new Image[3];
        title[0] = skin.transform.Find("Title/Result_draw").GetComponent<Image>();
        title[1] = skin.transform.Find("Title/Result_win").GetComponent<Image>();
        title[2] = skin.transform.Find("Title/Result_lose").GetComponent<Image>();
        
        okButton = skin.transform.Find("OkButton").GetComponent<Button>();
        //监听
        okButton.onClick.AddListener(OnOkClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        int result = (int)args[0];//0代表平局，或者意外发生，1代表成功，2代表失败
        int winId = (int)args[1];//代表赢的玩家的id
        int thisId = (int)args[2];//代表该客户端的玩家id
        ShowResult(result,winId,thisId);
    }

    //关闭
    public override void OnClose()
    {
        //网络协议监听
        //NetManager.RemoveMsgListener("MsgLogin", OnMsgLogin);
        //网络事件监听
        //NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, OnConnectSucc);
        //NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, OnConnectFail);
    }

    public void OnOkClick()
    {
        Debug.Log("返回");
    }

    /// <summary>
    /// 展示结果，用来根据传递的参数进行画面的显示
    /// </summary>
    /// <param name="result">0代表意外发生或者平局，1代表该角色成功，2代表失败</param>
    private void ShowResult(int result,int winId,int thisId)
    {
        players[thisId].text = "你";
        title[result].gameObject.SetActive(true);
        switch (result)
        {
            case 0:
                Debug.Log("播放平局音乐");
                break;
            case 1:
                Audio.PlayCue(Audio.win);
                break;
            case 2:
                Audio.PlayCue(Audio.lose);
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            switch (result)
            {
                case 0:
                    results[i].text = "+0";
                    break;
                case 1:
                case 2:
                    if (i == winId)
                    {
                        results[i].text = "-100";
                    }
                    else
                    {
                        results[i].text = "-100";
                    }
                    break;
            }
        }
        
    }
}



