using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChatRoom
{
    private InputField chatInput;
    private Text chatText;
    private ScrollRect scrollRect;

    public ChatRoom(InputField chatInput,Text chatText,ScrollRect scrollRect)
    {
        this.chatInput = chatInput;
        this.chatText = chatText;
        this.scrollRect = scrollRect;
    }

    /// <summary>
    /// 发送发牌协议
    /// </summary>
    /// <param name="client_id"></param>
    public void Update(int client_id)
    {
        //这个应该放在gameManager中，用于发送聊天的输入框
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            if (chatInput.text != "")
            {//发送聊天协议
                MsgChat msg = new MsgChat();
                msg.chatmsg = chatInput.text;
                msg.id = client_id;
                chatInput.text = "";
                NetManager.Send(msg);
            }
        }
    }


    public void ProcessMsgChat(MsgChat msg, BasePlayer[] players)
    {
        string addText = "\n  " + "<color=red>" + players[msg.id].username + "</color>: " + msg.chatmsg;
        chatText.text = chatText.text + addText;
        chatInput.ActivateInputField();
        Canvas.ForceUpdateCanvases();       //关键代码
        scrollRect.verticalNormalizedPosition = 0f;  //关键代码
        Canvas.ForceUpdateCanvases();   //关键代码
    }
}
