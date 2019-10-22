using System;
using System.Collections;
using System.Collections.Generic;

public static class Server{
    public static PaiManager_Server paiManager = PaiManager_Server.GetInstance();
    public static GameManager gameManager = GameManager.GetInstance();
    public static int turn;

    //服务端收到开始接收游戏数据的协议
    public static void OnMsgStartRecieveGameData(MsgBase msgBase)
    {
        MsgStartReceiveGameData msg = (MsgStartReceiveGameData)msgBase;
        //获取骰子的点数
        Random rd = new Random();
        int zhuangIdx = rd.Next() % 4;
        turn = zhuangIdx;
        //对协议名称进行初始化，这里表述不完全
        msg.data = new StartGameData[4];
        for(int i = 0; i < msg.data.Length; i++)
        {
            msg.data[i] = new StartGameData();
        }//初始化协议的牌数组
        for(int i = 0; i < 4; i++)
        {
            int num = 13;
            if(i == zhuangIdx)
            {
                num = 14;
            }
            int[] res = paiManager.FaPai(num,i);
            msg.data[i].paiIndex = res;
        }
        msg.id = 0;
        gameManager.OnMsgStartRecieveGameData(msg);
        System.Console.WriteLine("庄家：" + zhuangIdx);
        //for(int i = 0; i < 3; i++)
        //{
        //    msg.id = (zhuangIdx + i) % 4;
        //    gameManager.OnMsgStartRecieveGameData(msg);
        //}


        //摇色子，发送协议
        //发初始手牌，发送协议
        //广播所有玩家，庄家出牌
    }

    //收到发牌的协议
    public static void OnMsgFaPai(MsgBase msgBase)
    {
        MsgFaPai msg = (MsgFaPai)msgBase;
        msg.id = turn;
        int[] paiIdx = paiManager.FaPai(1,turn);
        msg.paiIndex = paiIdx[0];
        gameManager.OnMsgFaPai(msg);
    }

    //在收到玩家出的牌的协议
    public static void OnMsgShoudaoPai()
    {
        //更新牌库
        //调用PaiManager_Server判断吃碰杠胡
    }

    //出牌的协议
    //告知吃碰杠胡的协议，一个list,枚举类,id
    //结束游戏的协议
}
