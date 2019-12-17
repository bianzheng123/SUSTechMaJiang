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
    public int skillIndex = 0;
    public int skillCount = 0;
    public int gender = -1;//0代表男,1代表女,生成性别用于控制不同的声音播放
    public string username = "";//该客户端的用户名
}

//用于发牌
public class MsgFaPai : MsgBase
{
    public MsgFaPai() { protoName = "MsgFaPai"; }
    //服务端发送
    public int paiId;//牌的类型
    public int id;//收到牌的玩家id,如果是-1代表剩余牌没了，游戏结束
    public bool isHu;//代表该玩家的牌是否能胡
    public int turnNum;//代表现在到第几轮了
    public bool canSkill;//代表能否使用技能
}

/// <summary>
/// 丢一张牌，拿一张牌
/// </summary>
public class MsgChemistry : MsgBase
{
    public MsgChemistry() { protoName = "MsgChemistry"; }
    //客户端发送
    public int paiIndex;//牌的索引值
    public int id;//发动技能的玩家id
    //服务端发回
    public int paiId;//新的牌的id
    public bool canSkill;//接收到牌之后能否继续使用技能
}

/// <summary>
/// 观察对手x张牌
/// </summary>
public class MsgMath : MsgBase
{
    public MsgMath() { protoName = "MsgMath"; }
    //客户端发送
    public int observerPlayerId;//发动技能的玩家id
    public int observedPlayerId;//被观察的玩家id
    //服务端发回
    public int[] paiId;
    public bool canSkill;//代表能否继续使用技能
}

/// <summary>
/// 把所有的牌当成白板打出
/// </summary>
public class MsgComputerScience : MsgBase
{
    public MsgComputerScience() { protoName = "MsgComputerScience"; }
    //客户端发送
    public int paiIndex;//牌在这个玩家的索引
    public int id;//出牌玩家的id
    //服务端发回
    public bool canSkill;//能否继续使用技能
}

/// <summary>
/// 用来代表出牌或者胡的协议
/// </summary>
public class MsgChuPai : MsgBase
{
    public MsgChuPai() { protoName = "MsgChuPai"; }
    //客户端发,服务端广播
    public int paiIndex;//牌在这个玩家的索引,-1代表执行胡的操作
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
    public int result;//0代表什么都不做，其余分别代表吃碰杠,初始化为-1，代表什么都没做

    public override string ToString()
    {
        return "执行玩家id: " + id + ",打出的牌id: " + paiId + ",是否吃: " + isChiPengGang[1] + ",是否碰: " + isChiPengGang[2] + ",是否杠: " + isChiPengGang[3];
    }
}

public class MsgChat : MsgBase
{
    public MsgChat() { protoName = "MsgChat"; }
    //客户端发送
    public string chatmsg;//该轮玩家发送的信息
    public int id;//该客户端的玩家id
}

public class MsgQuit : MsgBase
{
    public MsgQuit() { protoName = "MsgQuit"; }
    public int id = -1;//代表这个客户端的id
    //服务端会
    public bool isQuit = false;//收到取消消息后判断该客户端是否要quit
}

