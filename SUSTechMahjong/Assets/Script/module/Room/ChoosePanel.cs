using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 选择输入的阵营
/// </summary>
public class ChoosePanel : BasePanel
{
    //显示价格的按钮
    private Button[] priceButton;
    //显示每一个系价格的文本
    private Text[] priceText;
    //关闭按钮
    private Button closeButton;
    //显示金币的文本
    private Text coinText;
    //显示系的文本
    private Text majorText;
    //目前拥有的金币数
    private int coin;
    //每一个系需要的价格
    private int[] price;

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
        priceButton = new Button[3];
        priceButton[0] = skin.transform.Find("Chemistry/Button").GetComponent<Button>();
        priceButton[1] = skin.transform.Find("Math/Button").GetComponent<Button>();
        priceButton[2] = skin.transform.Find("ComputerScience/Button").GetComponent<Button>();

        priceText = new Text[3];
        priceText[0] = skin.transform.Find("Chemistry/Button/Text").GetComponent<Text>();
        priceText[1] = skin.transform.Find("Math/Button/Text").GetComponent<Text>();
        priceText[2] = skin.transform.Find("ComputerScience/Button/Text").GetComponent<Text>();

        closeButton = skin.transform.Find("Title/Button").GetComponent<Button>();
        coinText = skin.transform.Find("Title/CoinText").GetComponent<Text>();
        majorText = skin.transform.Find("Title/MajorText").GetComponent<Text>();
        //监听
        priceButton[0].onClick.AddListener(OnChemistryClick);
        priceButton[1].onClick.AddListener(OnMathClick);
        priceButton[2].onClick.AddListener(OnComputerScienceClick);
        priceButton[0].onClick.AddListener(Audio.ButtonClick);
        priceButton[1].onClick.AddListener(Audio.ButtonClick);
        priceButton[2].onClick.AddListener(Audio.ButtonClick);
        closeButton.onClick.AddListener(OnCloseClick);
        closeButton.onClick.AddListener(Audio.ButtonClick);
        //网络协议监听
        NetManager.AddMsgListener("MsgChoose", OnMsgChoose);

        price = (int[])args[0];
        coin = (int)args[1];
        int major = (int)args[2];

        switch ((Major)major)//如果已经在该系中，就能将该按钮隐藏
        {
            case Major.Chemistry:
                priceButton[0].gameObject.SetActive(false);
                break;
            case Major.Math:
                priceButton[1].gameObject.SetActive(false);
                break;
            case Major.ComputerScience:
                priceButton[2].gameObject.SetActive(false);
                break;
        }

        for(int i = 0; i < priceText.Length; i++)
        {
            priceText[i].text = price[i].ToString();
        }
        coinText.text = "金币：" + coin;
        majorText.text = Gamedata.majors[major];
    }

    //关闭
    public override void OnClose()
    {
        //网络协议监听
        NetManager.RemoveMsgListener("MsgChoose", OnMsgChoose);

    }

    public void OnChemistryClick()
    {
        BuyItem(Major.Chemistry);
    }

    public void OnMathClick()
    {
        BuyItem(Major.Math);
    }

    public void OnComputerScienceClick()
    {
        BuyItem(Major.ComputerScience);
    }

    /// <summary>
    /// 点击不同的购买按钮，发送choose协议
    /// </summary>
    /// <param name="major">想要购买的按钮</param>
    private void BuyItem(Major major)
    {
        //发送
        MsgChoose msg = new MsgChoose();

        msg.major = (int)major;
        switch (major)
        {
            case Major.Chemistry:
                msg.coin = price[0];
                break;
            case Major.Math:
                msg.coin = price[1];
                break;
            case Major.ComputerScience:
                msg.coin = price[2];
                break;
        }
        NetManager.Send(msg);
    }

    //收到协议
    public void OnMsgChoose(MsgBase msgBase)
    {
        MsgChoose msg = (MsgChoose)msgBase;
        if (msg.result == 0)
        {
            //提示
            PanelManager.Open<TipPanel>("购买成功");
            
            if(PanelManager.panels.ContainsKey("RoomListPanel") && PanelManager.panels["RoomListPanel"] != null)
            {
                RoomListPanel panel = (RoomListPanel)PanelManager.panels["RoomListPanel"];
                panel.Major = msg.major;
                panel.Coin = msg.restCoin;
            }
            //关闭界面
            Close();
        }
        else if(msg.result == 1)
        {
            PanelManager.Open<TipPanel>("金币不足，购买失败");
        }
        else
        {
            PanelManager.Open<TipPanel>("出现异常");
        }

    }
    public void OnCloseClick()
    {
        Close();
    }

}
