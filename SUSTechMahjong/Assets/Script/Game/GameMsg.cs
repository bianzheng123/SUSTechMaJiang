using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//每一个协议都要一个Roomid
//得到初始化游戏的信息，即骰子结果，该客户端的id
public class MsgInitData : MsgBase
{
    public MsgInitData() { protoName = "MsgInitData"; }
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
    public int id;//收到牌的玩家id,如果是-1代表剩余牌没了，游戏结束
}

public class MsgChuPai : MsgBase
{
    public MsgChuPai() { protoName = "MsgChuPai"; }
    //客户端发,服务端广播
    public int paiIndex;//牌在这个玩家的索引
    public int id;//出牌的玩家id
}

public class MsgChiPengGang : MsgBase
{
    public MsgChiPengGang() { protoName = "MsgChiPengGang"; }
    //服务端发送
    public int paiId;//打出的牌的id
    public int id;//执行吃碰杠胡玩家的id
    public bool[] isChiPengGang;//分别代表能否进行吃碰杠
    //客户端回
    public int result = -1;//0代表什么都不做，其余分别代表吃碰杠

    public override string ToString()
    {
        return "执行玩家id: " + id + ",打出的牌id: " + paiId + ",是否吃: " + isChiPengGang[1] + ",是否碰: " + isChiPengGang[2] + ",是否杠: " + isChiPengGang[3];
    }
}

