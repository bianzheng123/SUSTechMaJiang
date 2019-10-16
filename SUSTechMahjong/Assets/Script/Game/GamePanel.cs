using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePanel : BasePanel {

    //退出按钮
    private Button exitButton;
    //设置UI按钮
    private Button setButton;
    //灯的亮灭
    private GameObject[] light;

    //初始化
    public override void OnInit()
    {
        skinPath = "GamePanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        exitButton = skin.transform.Find("ExitButton").GetComponent<Button>();
        setButton = skin.transform.Find("SetButton").GetComponent<Button>();
        light = new GameObject[4];
        light[0] = skin.transform.Find("TimeImage/Up").gameObject;
        light[1] = skin.transform.Find("TimeImage/Down").gameObject;
        light[2] = skin.transform.Find("TimeImage/Left").gameObject;
        light[3] = skin.transform.Find("TimeImage/Right").gameObject;
        //监听
        exitButton.onClick.AddListener(OnExitClick);
        setButton.onClick.AddListener(OnSetClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        //组件的初始化
        for(int i = 0; i < light.Length; i++)
        {
            light[i].SetActive(false);
        }
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

    public void OnExitClick()
    {
        Debug.Log("Exit");
    }

    public void OnSetClick()
    {
        Debug.Log("Set");
    }

}
public enum Direction
{
    UP = 0,
    DOWN = 1,
    LEFT = 2,
    RIGHT = 3
}


