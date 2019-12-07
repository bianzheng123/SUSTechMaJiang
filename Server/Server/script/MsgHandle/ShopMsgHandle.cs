using System;


public partial class MsgHandler
{
    public static void MsgVisitShop(ClientState c, MsgBase msgBase)
    {
        MsgVisitShop msg = (MsgVisitShop)msgBase;
        Player player = c.player;
        if (player == null) return;
        XMLMajor[] prices = XMLManager.majorInfo.ToArray();
        msg.price = new int[prices.Length];
        if (player.data.major == (int)Major.None)
        {
            for(int i = 0; i < prices.Length; i++)
            {
                msg.price[i] = prices[i].priceNoMajor;
            }
        }
        else
        {
            for (int i = 0; i < prices.Length; i++)
            {
                msg.price[i] = prices[i].priceHaveMajor;
            }
        }
        player.Send(msg);
    }

    //专业选择协议处理
    public static void MsgChoose(ClientState c, MsgBase msgBase)
    {
        MsgChoose msg = (MsgChoose)msgBase;
        Player player = c.player;
        if (player == null) return;
        if(player.data.coin < msg.coin)
        {
            msg.result = 1;
            player.Send(msg);
            return;
        }
        if (player.data.major == msg.major)
        {
            msg.result = 2;
            Console.WriteLine("购买物品时出现bug，购买的系与现有的系相同");
        }
        player.data.major = msg.major;
        player.data.coin -= msg.coin;
        msg.restCoin = player.data.coin;
        
        if (DbManager.UpdatePlayerData(player.id, player.data))
        {
            msg.result = 0;
        }
        else
        {
            msg.result = 2;
        }
        player.Send(msg);
    }
}