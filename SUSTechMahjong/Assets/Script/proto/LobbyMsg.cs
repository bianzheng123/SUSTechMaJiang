public class LobbyMsg  {

	
}

//注册
public class MsgRegister : MsgBase
{
    public MsgRegister() { protoName = "MsgRegister"; }
    //客户端发
    public string id = "";
    public string pw = "";
    //服务端回（0-成功，1-失败）
    public int result = 0;
}

//刷新房间
public class MsgRefreshRoom : MsgBase
{
    public MsgRefreshRoom() { protoName = "MsgRefreshRoom"; }
    //客户端发

    //服务端回
    public 
}
