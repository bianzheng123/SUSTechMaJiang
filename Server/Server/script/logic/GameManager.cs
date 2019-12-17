using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameManager
{
    //这里的username指的是player中的id
    private PaiManager paiManager;
    private Room room;
    public Player[] players;
    public int turn;//代表现在轮到谁了
    public Queue<MsgChiPengGang> queueChiPengGang;//每一个房间都存放用来判断是否吃碰杠的列表
    public int turnNum = 1;//代表现在是第几轮
    public int turnCount = 0;//每当服务端发送一次出牌协议就调用一次，用来判断是第几轮了
    public int[] doSkillTime;//代表每一个玩家已经用过技能的次数
    public int[] maxSkillTime;//代表每一个玩家在一局中最多使用技能的次数

    public GameManager(Room room)
    {
        this.room = room;
        players = new Player[4];
        paiManager = new PaiManager();
        paiManager.Init();
        doSkillTime = new int[4];
        maxSkillTime = new int[4];

        List<string> playerIds = new List<string>(room.playerIds);
        
        for(int i = 0; i < 4; i++)
        {
            players[i] = PlayerManager.GetPlayer(playerIds[i]);
            doSkillTime[i] = 0;
            Major major = (Major)players[i].data.major;
            switch (major)
            {
                case Major.None:
                    maxSkillTime[i] = (int)MaxSkillTime.None;
                    break;
                case Major.Math:
                    maxSkillTime[i] = (int)MaxSkillTime.Math;
                    break;
                case Major.Chemistry:
                    maxSkillTime[i] = (int)MaxSkillTime.Chemistry;
                    break;
                case Major.ComputerScience:
                    maxSkillTime[i] = (int)MaxSkillTime.ComputerScience;
                    break;
            }
        }
    }

    public MsgInitData GetMsgInitData()
    {
        MsgInitData msg = new MsgInitData();
        Random rd = new Random();
        int zhuangIdx = rd.Next() % 4;//随机确定庄家
        turn = zhuangIdx;
        msg.data = new StartGameData[4];
        for (int i = 0; i < msg.data.Length; i++)
        {
            msg.data[i] = new StartGameData();
            msg.data[i].skillIndex = players[i].data.major;
            msg.data[i].skillCount = maxSkillTime[i];
            msg.data[i].gender = players[i].data.gender;
            msg.data[i].username = players[i].id;
        }//初始化协议的牌数组
        for (int i = 0; i < 4; i++)
        {
            int num = 13;
            if (i == zhuangIdx)
            {
                num = 14;
            }
            int[] res = paiManager.FaPai(num, i);
            msg.data[i].paiIndex = res;
        }
        return msg;
    }

    public MsgFaPai ProcessMsgFaPai(MsgFaPai msg)
    {
        msg.id = turn;
        msg.turnNum = turnNum;
        msg.canSkill = doSkillTime[turn] < maxSkillTime[turn] ? true : false;//表示对于当前玩家能否使用技能
        turnCount++;
        if (turnCount % 4 == 0)
        {
            turnCount = 0;
            turnNum++;
        }//判断下一次出牌是在第几轮
        int paiIdx = paiManager.FaPai(turn);

        if(paiIdx == -1)
        {
            //没牌了，结束游戏
            Over(-1,false);
        }

        msg.paiId = paiIdx;
        if (paiManager.HasHu(turn))
        {
            msg.isHu = true;
        }
        else
        {
            msg.isHu = false;
        }
        //广播
        return msg;
    }

    /// <summary>
    /// 判断是否触发了吃碰杠，如果触发了，就发送吃碰杠协议
    /// 否则，发送发牌协议，并广播
    /// </summary>
    /// <param name="paiIndex"></param>
    /// <param name="id"></param>
    public void ProcessMsgChuPai(MsgChuPai msg)
    {
        int paiIndex = msg.paiIndex;//牌在这个玩家的索引
        int id = msg.id;//出牌的玩家id
        Broadcast(msg);//对客户端广播出牌协议，对出牌进行同步

        if (paiIndex == -1)
        {
            //服务端执行胡的操作，清空数据，写入数据库等
            Over(msg.id,false);
            return;
        }

        int paiId = paiManager.ChuPai(paiIndex, id);

        queueChiPengGang = paiManager.HasEvent(paiId, id);//检测是否有吃碰杠这件事

        if (queueChiPengGang.Count != 0)//一直发送吃碰杠协议，直到发完或者有人同意吃碰杠为止
        {
            Console.WriteLine("存在吃碰杠!");
            foreach (MsgChiPengGang item in queueChiPengGang)
            {
                Console.WriteLine(item.ToString());
            }
            MsgChiPengGang chiPengGang = queueChiPengGang.Dequeue();
            Broadcast(chiPengGang);//广播吃碰杠协议
        }
        else
        {
            MsgFaPai msgFaPai = new MsgFaPai();
            turn = (turn + 1) % 4;

            msgFaPai = ProcessMsgFaPai(msgFaPai);
            //广播
            Broadcast(msgFaPai);
        }
    }

    /// <summary>
    /// 服务端接收到吃碰杠的协议，将本轮吃碰杠的结果进行广播,同时将服务端的牌进行同步
    /// 如果本轮没有进行吃碰杠，就轮到下一个玩家判断
    /// 如果有人进行了吃碰杠，就轮到下一个人进行发牌
    /// </summary>
    /// <param name="msgBase"></param>
    public void ProcessChiPengGang(MsgChiPengGang msg)
    {
        Broadcast(msg);//对得到的本轮结果进行广播
        //取消操作，就发送下一个吃碰杠协议
        if (msg.result == 0 && queueChiPengGang.Count > 0)
        {
            MsgChiPengGang msgChiPengGang = queueChiPengGang.Dequeue();
            Broadcast(msgChiPengGang);
            return;
        }

        switch (msg.result)
        {
            case 0://这里的queue一定为空，不需要进行任何的改变
                break;
            case 1://更新牌库
                if (paiManager.OnChi(msg.id, msg.paiId) == false)
                {
                    Console.WriteLine("更新牌库吃出现了bug");
                }
                break;
            case 2:
                if (paiManager.OnPeng(msg.id, msg.paiId) == false)
                {
                    Console.WriteLine("更新牌库碰出现了bug");
                }
                break;
            case 3:
                if (paiManager.OnGang(msg.id, msg.paiId) == false)
                {
                    Console.WriteLine("更新牌库杠出现了bug");
                }
                break;
        }
        MsgFaPai msgFaPai = new MsgFaPai();
        turn = (turn + 1) % 4;
        msgFaPai = ProcessMsgFaPai(msgFaPai);
        //广播
        Broadcast(msgFaPai);
    }

    /// <summary>
    /// 服务端处理化学系的协议
    /// 删除指定的牌，添加一个额外的牌，并广播，使得每一个客户端重新开始计时
    /// </summary>
    /// <param name="msgBase"></param>
    public void ProcessChemistry(MsgChemistry msg)
    {
        int paiId = paiManager.ChuPai(msg.paiIndex, msg.id);//删除指定的牌
        Console.WriteLine("删除牌的id为 " + paiId);
        paiId = paiManager.FaPai(msg.id);
        Console.WriteLine("重新得到的牌id为 " + paiId);
        msg.paiId = paiId;
        doSkillTime[msg.id]++;
        msg.canSkill = doSkillTime[msg.id] < maxSkillTime[msg.id] ? true : false;
        Broadcast(msg);
    }

    /// <summary>
    /// 服务端处理数学系技能的协议
    /// 给客户端发送这些牌id的数组
    /// </summary>
    /// <param name="msgBase"></param>
    public void ProcessMath(MsgMath msg)
    {
        msg.paiId = paiManager.GetPaiIdRandom(msg.observedPlayerId, turnNum);
        doSkillTime[msg.observerPlayerId]++;
        msg.canSkill = doSkillTime[msg.observerPlayerId] < maxSkillTime[msg.observerPlayerId] ? true : false;
        Broadcast(msg);
    }

    /// <summary>
    /// 服务端接收到计算机系协议
    /// </summary>
    /// <param name="msgBase"></param>
    public void ProcessComputerScience(MsgComputerScience msg)
    {
        int paiIndex = msg.paiIndex;//牌在这个玩家的索引
        int id = msg.id;//出牌的玩家id
        doSkillTime[msg.id]++;
        msg.canSkill = doSkillTime[msg.id] < maxSkillTime[msg.id] ? true : false;
        Broadcast(msg);

        MsgChuPai msgChuPai = new MsgChuPai();
        msgChuPai.id = msg.id;
        msgChuPai.paiIndex = msg.paiIndex;
        Broadcast(msgChuPai);
        if (paiIndex == -1)
        {
            //服务端执行胡的操作，清空数据，写入数据库等
            Over(msg.id,false);
            return;
        }

        int paiId = paiManager.ChuPai(paiIndex, id);
        MsgFaPai msgFaPai = new MsgFaPai();
        turn = (turn + 1) % 4;

        msgFaPai = ProcessMsgFaPai(msgFaPai);
        //广播
        Broadcast(msgFaPai);
    }

    public void ProcessMsgQuit(MsgQuit msg)
    {
        Over(msg.id,true);
        for (int i = 0; i < 4; i++)
        {
            if(i == msg.id)
            {
                msg.isQuit = true;
            }
            else
            {
                msg.isQuit = false;
            }
            players[i].Send(msg);
        }
    }

    /// <summary>
    /// 服务端执行结束游戏操作
    /// </summary>
    /// <param name="winId">如果是-1代表其他原因结束游戏，否则就是赢家/逃跑的id</param>
    /// <param name="isQuit">代表是否强制退出，如果是强制退出就扣分,其他三个不扣分</param>
    public void Over(int winId,bool isQuit)
    {
        room.status = Room.Status.PREPARE;
        room.gameManager = null;
        if (isQuit)
        {
            if (winId == -1)
            {
                //没有人扣分，此时可以认为系统出现了bug
                Console.WriteLine("属于强制退出且检测到无人退出，出现bug");
        }
            else
            {
                //有人强制退出，扣除他的分
                players[winId].data.lost++;
                players[winId].data.coin -= 1000;
                for (int i = 0; i < players.Length; i++)
                {
                    if (i == winId) continue;
                    players[i].data.win++;
                    players[i].data.coin += 25;
                }
            }
        }
        else
        {
            if (winId == -1)
            {
                //没人胡，没有任何玩家扣分，默认全部都胜利
                for(int i = 0; i < players.Length; i++)
                {
                    players[i].data.win++;
                    players[i].data.coin += 25;
                }
            }
            else
            {
                //有人胡，执行胡操作
                for(int i = 0; i < players.Length; i++)
                {
                    if(i == winId)
                    {
                        players[i].data.win++;
                        players[i].data.coin += 60;
                    }
                    else
                    {
                        players[i].data.lost++;
                        players[i].data.coin += 10;
                    }
                }
            }
        }

        for(int i = 0; i < players.Length; i++)
        {
            if (!DbManager.UpdatePlayerData(players[i].id, players[i].data))
            {
                Console.WriteLine("无法更新成功，出现bug");
            }
        }
        
    }

    //广播消息
    public void Broadcast(MsgBase msg)
    {
        for(int i = 0; i < 4; i++)
        {
            players[i].Send(msg);
        }
    }
}
