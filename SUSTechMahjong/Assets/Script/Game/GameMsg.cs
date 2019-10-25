using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//得到初始化游戏的信息，即骰子结果，该客户端的id
public class MsgStartReceiveGameData : MsgBase
{
    public MsgStartReceiveGameData() { protoName = "MsgStartReceiveGameData"; }
    //服务端回
    public int id = 0;//该客户端的id
    public StartGameData[] data = null;
}

//不同玩家的初始手牌信息
[System.Serializable]
public class StartGameData
{
    public int[] paiIndex = null;
}

//用于发牌
public class MsgFaPai : MsgBase
{
    public MsgFaPai() { protoName = "MsgFaPai"; }
    //服务端回
    public int paiId;//牌的类型
    public int id;//收到牌的玩家id
}

public class MsgChuPai : MsgBase
{
    public MsgChuPai() { protoName = "MsgChuPai"; }
    //客户端发,服务端广播
    public int paiIndex;//牌在这个玩家的索引
    public int id;//出牌的玩家id
}

