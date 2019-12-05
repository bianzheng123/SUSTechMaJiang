using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 展示棋子的面板
/// 用于发动数学系技能，查看别人丢弃的拍，吃碰杠的牌
/// </summary>
public class ObjectTipPanel : BasePanel
{
    //提示文本
    private Transform content;
    //确定按钮
    private Button okBtn;

    //初始化
    public override void OnInit()
    {
        skinPath = "ObjectTipPanel";
        layer = PanelManager.Layer.Panel;
    }

    /// <summary>
    /// 显示牌
    /// </summary>
    /// <param name="args">存储牌的路径，不能为空</param>
    public override void OnShow(params object[] args)
    {
        //寻找组件
        content = skin.transform.Find("Scroll View/Viewport/Content");
        okBtn = skin.transform.Find("OkBtn").GetComponent<Button>();
        //监听
        okBtn.onClick.AddListener(OnOkClick);
        okBtn.onClick.AddListener(Audio.ButtonClick);
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
                go.transform.localScale = new Vector3(0.7f,0.7f,0.7f);
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
