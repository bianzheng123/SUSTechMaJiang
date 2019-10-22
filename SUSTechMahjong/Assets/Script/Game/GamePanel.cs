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
    private GameObject[] lights;

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
        lights = new GameObject[4];
        lights[0] = skin.transform.Find("TimeImage/Down").gameObject;
        lights[1] = skin.transform.Find("TimeImage/Right").gameObject;
        lights[2] = skin.transform.Find("TimeImage/Up").gameObject;
        lights[3] = skin.transform.Find("TimeImage/Left").gameObject;
        //监听
        exitButton.onClick.AddListener(OnExitClick);
        setButton.onClick.AddListener(OnSetClick);
        //网络协议监听
        //NetManager.AddMsgListener("MsgLogin", OnMsgLogin);
        //发送查询
        //MsgGetRoomInfo msg = new MsgGetRoomInfo();
        //NetManager.Send(msg);

        GameObject gameManager = ResManager.LoadPrefab("GameManager");
        gameManager.GetComponent<GameManager>().GamePanel = this;
        GameObject init_gameManager = Instantiate(gameManager);
        

        Sprite s = ResManager.LoadSprite("bg_game");
        GameObject go = new GameObject("bg");
        SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
        sr.sortingOrder = 0;
        sr.sprite = s;
        go.transform.localScale = new Vector3(2,2,2);

        //组件的初始化
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
    }

    public void OnMsgStartRecieveGameData(MsgBase msgBase)
    {

    }

    //改变灯光的顺序
    public void TurnLight(Direction dir)
    {
        for (int i = 0; i < lights.Length; i++)
        {
            lights[i].SetActive(false);
        }
        lights[(int)dir].SetActive(true);
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
    DOWN = 0,
    RIGHT = 1,
    UP = 2,
    LEFT = 3,
    
}


