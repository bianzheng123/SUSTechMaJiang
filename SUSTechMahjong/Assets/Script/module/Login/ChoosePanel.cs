using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoosePanel : BasePanel
{
    //院系输入框
    private InputField idInput;

    //确认按钮
    private Button ChooseBtn;

    private Button closeBtn;
    //初始化
    public override void OnInit()
    {
        skinPath = "ChoosePanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        idInput = skin.transform.Find("IdInput").GetComponent<InputField>();

        ChooseBtn = skin.transform.Find("ChooseBtn").GetComponent<Button>();
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //监听
        ChooseBtn.onClick.AddListener(OnChooseClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgChoose", OnMsgChoose);

    }

    //关闭
    public override void OnClose()
    {
        //网络协议监听
        NetManager.RemoveMsgListener("MsgChoose", OnMsgChoose);

    }


    //当按下确定按钮
    public void OnChooseClick()
    {
        //用户名密码为空
        if (idInput.text == "")
        {
            PanelManager.Open<TipPanel>("请正确选择院系");
            return;
        }
        //发送
        MsgChoose MsgChoose = new MsgChoose();

        MsgChoose.camp = int.Parse(idInput.text);
        MsgChoose.id = GameMain.id;
        NetManager.Send(MsgChoose);
    }

    //收到协议
    public void OnMsgChoose(MsgBase msgBase)
    {
        MsgChoose msg = (MsgChoose)msgBase;
        if (msg.result == 0)
        {
            Debug.Log("选择成功");
            //提示
            PanelManager.Open<TipPanel>("选择成功");
            //关闭界面
            Close();
        }
        else
        {
            PanelManager.Open<TipPanel>("选择失败");
        }

    }
    public void OnCloseClick()
    {
        Close();
    }
}
