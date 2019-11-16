﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MathTipPanel : BasePanel
{
    //提示文本
    private Transform content;
    //确定按钮
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "MathTipPanel";
        layer = PanelManager.Layer.Tip;
    }
    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        content = skin.transform.Find("Scroll View/Viewport/Content");
        okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(OnOkClick);
        //提示语
        if(args.Length == 0)
        {
            Debug.Log("传递的参数个数不能为零，出现bug");
        }
        else
        {
            Debug.Log(args[0]);
            string tmp = (string)args[0];
            string[] paths = tmp.Split(',');
            
            for(int i = 0; i < paths.Length; i++)
            {
                if (paths[i] == "") continue;
                string[] split = paths[i].Split('/');
                GameObject go = new GameObject(split[split.Length-1]);
                go.transform.localScale = new Vector3(0.5f,0.5f,0.5f);
                Image image = go.AddComponent<Image>();
                image.sprite = ResManager.LoadUISprite(paths[i]);
                go.transform.SetParent(content);
            }
        }

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
