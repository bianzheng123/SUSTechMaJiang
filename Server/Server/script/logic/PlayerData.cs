/// <summary>
/// 用来存放数据库中的类
/// </summary>
public class PlayerData{
    /// <summary>
    /// 用于创建一个账号时使用
    /// </summary>
    /// <param name="gender"></param>
    public PlayerData(Gender gender)
    {
        this.gender = (int)gender;
    }
    /// <summary>
    /// 用于反序列化
    /// </summary>
    public PlayerData()
    {

    }
    //金币
    public int coin = 100;
	//胜利数
	public int win = 0;
	//失败数
	public int lost = 0;
    //院系
    public int major = (int)Major.None;
    //性别
    public int gender;
}