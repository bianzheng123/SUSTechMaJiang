using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择输入的阵营
/// </summary>
public class ChoosePanel : BasePanel
{
    //院系输入框
    private InputField idInput;

    //确认按钮
    private Button chooseBtn;

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

        chooseBtn = skin.transform.Find("ChooseBtn").GetComponent<Button>();
        closeBtn = skin.transform.Find("CloseBtn").GetComponent<Button>();
        //监听
        chooseBtn.onClick.AddListener(OnChooseClick);
        closeBtn.onClick.AddListener(OnCloseClick);
        chooseBtn.onClick.AddListener(Audio.ButtonClick);
        closeBtn.onClick.AddListener(Audio.ButtonClick);
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
        //输入的数字为空
        if (idInput.text == "")
        {
            PanelManager.Open<TipPanel>("请正确选择院系");
            return;
        }
        int major = 0;
        try
        {
            major = int.Parse(idInput.text);
        }
        catch
        {
            PanelManager.Open<TipPanel>("请正确输入数字");
            return;
        }
        if(!(0<=major && major <= 3))
        {
            PanelManager.Open<TipPanel>("请正确输入数字");
            return;
        }
        //发送
        MsgChoose MsgChoose = new MsgChoose();

        MsgChoose.major = major;
        MsgChoose.id = GameMain.id;
        NetManager.Send(MsgChoose);
    }

    //收到协议
    public void OnMsgChoose(MsgBase msgBase)
    {
        MsgChoose msg = (MsgChoose)msgBase;
        if (msg.result == 0)
        {
            //提示
            PanelManager.Open<TipPanel>("选择成功");
            
            if(PanelManager.panels.ContainsKey("RoomListPanel") && PanelManager.panels["RoomListPanel"] != null)
            {
                RoomListPanel panel = (RoomListPanel)PanelManager.panels["RoomListPanel"];
                panel.Camp = Gamedata.majors[msg.major];
            }
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
