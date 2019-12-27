using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PressMePanel : BasePanel
{
    //确定按钮
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "PressMePanel";
        layer = PanelManager.Layer.Panel;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        okBtn = skin.transform.Find("OkButton").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(OnOkClick);
        okBtn.onClick.AddListener(Audio.ButtonClick);
    }

    //关闭
    public override void OnClose()
    {

    }

    //当按下确定按钮
    public void OnOkClick()
    {
        Close();
    }
}
