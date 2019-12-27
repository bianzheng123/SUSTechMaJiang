using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitTipPanel : BasePanel
{
    //确定按钮
    private Button regretBtn;
    //退出按钮
    private Button quitBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "QuitTipPanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        regretBtn = skin.transform.Find("RegretBtn").GetComponent<Button>();
        quitBtn = skin.transform.Find("QuitBtn").GetComponent<Button>();
        //监听
        regretBtn.onClick.AddListener(OnOkClick);
        regretBtn.onClick.AddListener(Audio.ButtonClick);
        quitBtn.onClick.AddListener(OnQuitClick);
        quitBtn.onClick.AddListener(Audio.ButtonClick);
    }

    //关闭
    public override void OnClose()
    {
        
    }

    public void OnQuitClick()
    {
        MsgQuit msg = new MsgQuit();
        msg.id = GameManager.GetInstance().client_id;
        NetManager.Send(msg);
    }

    //当按下确定按钮
    public void OnOkClick()
    {
        Close();
    }
}
