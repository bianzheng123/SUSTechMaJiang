//初始访问商店时发出的消息,用于确定价格（是否进系价格不同）
public class MsgVisitShop : MsgBase
{
    public MsgVisitShop() { protoName = "MsgVisitShop"; }
    //服务端回
    public int[] price;//价格分别按照顺序进行排列
}

//购买物品的协议
public class MsgChoose : MsgBase
{
    public MsgChoose() { protoName = "MsgChoose"; }
    //客户端发
    public int major = 0;//要求的系
    public int coin = 0;//进入这个系需要多少金币
    //服务端回（0-成功，1-缺少金币，2-其他原因）
    public int result = 0;
    public int restCoin = -1;
}