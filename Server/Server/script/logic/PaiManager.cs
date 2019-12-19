using System;
using System.Collections;
using System.Collections.Generic;

public class PaiManager
{
    //剩下的牌
    public List<int> restPai;
    public List<int>[] playerPai;

    public void Init()
    {
        restPai = new List<int>();
        playerPai = new List<int>[4];
        for (int i = 0; i < 4; i++)
        {
            playerPai[i] = new List<int>();
        }
        initPaiKu();
    }

    //全部牌list
    private void initPaiKu()
    {
        //1 ~9 : 一万 ~9万
        //11 ~19 : 一条 ~9条
        //21 ~29 : 一筒 ~9筒
        //31 ~34 : 东南西北风
        //41 : 发财
        //42 : 红中
        //43 : 白板
        for (int i = 1; i <= 43; i++)
        {
            if (i == 10 || i == 20 || i == 30 || i == 35 || i == 36 || i == 37 || i == 38 || i == 39 || i == 40)
            {
                continue;
            }
            for (int j = 0; j < 4; j++)
            {
                restPai.Add(i);
            }

        }
    }

    /// <summary>
    /// 进行初始化时的发牌方法
    /// </summary>
    /// <param name="num">发牌的数量</param>
    /// <param name="id">玩家的id</param>
    /// <returns></returns>
    public int[] FaPai(int num, int id)
    {
        int[] res = new int[num];
        for (int i = 0; i < num; i++)
        {
            Random rd = new Random();
            int ranIdx = rd.Next() % restPai.Count;
            res[i] = restPai[ranIdx];
            restPai.RemoveAt(ranIdx);
        }
        //对res按照升序进行排序
        Array.Sort(res);
        for (int i = 0; i < num; i++)
        {
            playerPai[id].Add(res[i]);
        }
        return res;
    }

    /// <summary>
    /// 发送单个牌调用的方法
    /// </summary>
    /// <param name="playerId">玩家的id</param>
    /// <returns></returns>
    public int FaPai(int playerId)
    {
        if (restPai.Count == 0)
        {
            return -1;
        }
        Random rd = new Random();
        int ranIdx = rd.Next() % restPai.Count;
        int paiId = restPai[ranIdx];
        restPai.RemoveAt(ranIdx);

        int arrIndex = playerPai[playerId].Count;
        playerPai[playerId].Add(paiId);
        while (arrIndex >= 1 && playerPai[playerId][arrIndex] < playerPai[playerId][arrIndex - 1])
        {
            //arrIndex和arrIndex-1交换元素
            int tmp = playerPai[playerId][arrIndex];
            playerPai[playerId][arrIndex] = playerPai[playerId][arrIndex - 1];
            playerPai[playerId][arrIndex - 1] = tmp;
            arrIndex--;
        }
        return paiId;
    }

    /// <summary>
    /// 用于出牌，返回的是这张牌的id
    /// </summary>
    /// <param name="paiIndex"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    public int ChuPai(int paiIndex, int id)
    {
        if(!(0 <= id && id <= 3))
        {
            Console.WriteLine("发牌时id越界，出现bug！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！！");
        }
        if(!(0<= paiIndex && paiIndex < playerPai[id].Count))
        {
            Console.WriteLine("发牌时牌id越界，出现bug!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
        }
        int paiId = playerPai[id][paiIndex];
        playerPai[id].RemoveAt(paiIndex);
        
        return paiId;
    }

    /// <summary>
    /// 判断是否触发碰杠胡的条件
    /// </summary>
    /// <param name="paiId">这个牌的索引</param>
    /// <param name="id">打出这张牌的玩家的id</param>
    /// <returns>null代表没有触发条件，</returns>
    public Queue<MsgChiPengGang> HasEvent(int paiId, int id)
    {
        Queue<MsgChiPengGang> queue = new Queue<MsgChiPengGang>();
        for (int i = id + 1; i < id + 4; i++)
        {
            int panId = i % 4;
            bool[] res = new bool[4];
            res[0] = true;//代表永远可以取消吃碰杠操作
            res[1] = HasChi(paiId, panId);//检测吃
            res[2] = HasPeng(paiId, panId);//检测碰
            res[3] = HasGang(paiId, panId);//检测杠
            if (res[1] | res[2] | res[3])//至少有一个为true，就可以发送消息
            {
                MsgChiPengGang msg = new MsgChiPengGang();
                msg.isChiPengGang = res;
                msg.id = panId;
                msg.paiId = paiId;
                msg.result = -1;
                queue.Enqueue(msg);
            }
        }
        return queue;
    }

    /// <summary>
    /// 判断是否出现吃的情况
    /// </summary>
    /// <param name="paiId">打出的牌的id</param>
    /// <param name="playerId">要搜索的玩家id</param>
    /// <returns>是否有吃</returns>
    private bool HasChi(int paiId, int playerId)
    {
        bool pan1 = playerPai[playerId].Contains(paiId + 1) && playerPai[playerId].Contains(paiId + 2);
        bool pan2 = playerPai[playerId].Contains(paiId - 1) && playerPai[playerId].Contains(paiId + 1);
        bool pan3 = playerPai[playerId].Contains(paiId - 2) && playerPai[playerId].Contains(paiId - 1);
        return pan1 | pan2 | pan3;
    }

    private bool HasPeng(int paiId, int playerId)
    {
        int count = 0;
        for (int i = 0; i < playerPai[playerId].Count; i++)
        {
            if (playerPai[playerId][i] == paiId)
            {
                count++;
            }
        }
        return count >= 2;
    }

    private bool HasGang(int paiId, int playerId)
    {
        int count = 0;
        for (int i = 0; i < playerPai[playerId].Count; i++)
        {
            if (playerPai[playerId][i] == paiId)
            {
                count++;
            }
        }
        return count >= 3;
    }

    public bool HasHu(int playerId)
    {
        List<int> cards = playerPai[playerId];
        int len = playerPai[playerId].Count;
        if (len <= 9)//用于测试
        {
            return true;
        }
        return false;                      
    }

    /// <summary>
    /// 更新牌库
    /// 返回值是为了测试
    /// </summary>
    /// <param name="id"></param>
    /// <param name="paiId"></param>
    public bool OnChi(int id, int paiId)
    {
        //分为三种可能
        if (NumOfPai(id, paiId - 1) >= 1 && NumOfPai(id, paiId - 2) >= 1)
        {
            int index1 = playerPai[id].IndexOf(paiId - 1);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId - 2);
            playerPai[id].RemoveAt(index2);
        }
        else if (NumOfPai(id, paiId - 1) >= 1 && NumOfPai(id, paiId + 1) >= 1)
        {
            int index1 = playerPai[id].IndexOf(paiId - 1);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId + 1);
            playerPai[id].RemoveAt(index2);
        }
        else if (NumOfPai(id, paiId + 1) >= 1 && NumOfPai(id, paiId + 2) >= 1)
        {
            int index1 = playerPai[id].IndexOf(paiId + 1);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId + 2);
            playerPai[id].RemoveAt(index2);
        }
        else
        {
            //程序出现bug
            return false;
        }
        return true;
    }

    public bool OnPeng(int id, int paiId)
    {
        if (NumOfPai(id, paiId) >= 2)
        {
            int index1 = playerPai[id].IndexOf(paiId);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId);
            playerPai[id].RemoveAt(index2);
            return true;
        }
        return false;
    }

    public bool OnGang(int id, int paiId)
    {
        if (NumOfPai(id, paiId) >= 3)
        {
            int index1 = playerPai[id].IndexOf(paiId);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId);
            playerPai[id].RemoveAt(index2);

            int index3 = playerPai[id].IndexOf(paiId);
            playerPai[id].RemoveAt(index3);
            return true;
        }
        return false;
    }

    /// <summary>
    /// 用于计算某张牌在某个玩家的出现次数
    /// </summary>
    /// <param name="id"></param>
    /// <param name="paiId"></param>
    /// <returns></returns>
    private int NumOfPai(int id, int paiId)
    {
        int count = 0;
        for (int i = 0; i < playerPai[id].Count; i++)
        {
            if (paiId == playerPai[id][i])
            {
                count++;
            }
        }
        return count;
    }
    //四个玩家四个list,存在List中
    //List<>
    //判断吃，碰，杠，胡（发送协议）

    /// <summary>
    /// 用于发动数学系技能，查看某个玩家的随机几张牌
    /// </summary>
    /// <param name="id">被偷看的玩家id</param>
    /// <param name="turn">现在是第几轮</param>
    /// <returns>返回牌的id，如果是null代表没有牌可以看</returns>
    public int[] GetPaiIdRandom(int id, int turn)
    {
        int paiLeng = playerPai[id].Count - turn / 3;
        if (paiLeng <= 0)
        {
            return null;
        }
        if (paiLeng == playerPai[id].Count)
        {
            return playerPai[id].ToArray();
        }

        int[] res = new int[paiLeng];
        List<int> handPai = playerPai[id];
        List<int> index = new List<int>();//用来存放已经加入牌库的东西
        while (index.Count < paiLeng)
        {
            Random rd = new Random();
            int ranIdx = rd.Next() % handPai.Count;
            if (!index.Contains(ranIdx))
            {
                res[index.Count] = handPai[ranIdx];
                index.Add(ranIdx);
            }
        }
        return res;
    }
}
