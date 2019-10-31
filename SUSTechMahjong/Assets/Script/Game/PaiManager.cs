using System;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 这个在服务端中
/// </summary>
public class PaiManager {

    //剩下的牌
    public List<int> restPai;
    public List<int>[] playerPai;

    public void Init()
    {
        restPai = new List<int>();
        playerPai = new List<int>[4];
        for(int i = 0; i < 4; i++)
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
            if(i == 10 || i == 20 || i == 30 || i == 35 || i == 36 || i == 37 || i == 38 || i == 39 || i == 40)
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
    public int[] FaPai(int num,int id)
    {
        int[] res = new int[num];
        for(int i = 0; i < num; i++)
        {
            Random rd = new Random();
            int ranIdx = rd.Next() % restPai.Count;
            res[i] = restPai[ranIdx];
            restPai.RemoveAt(ranIdx);
        }
        //对res按照升序进行排序
        Array.Sort(res);
        for(int i = 0; i < num; i++)
        {
            playerPai[id].Add(res[i]);
        }
        return res;
    }

    /// <summary>
    /// 发送单个牌调用的方法
    /// </summary>
    /// <param name="id">玩家的id</param>
    /// <returns></returns>
    public int FaPai(int id)
    {
        if(restPai.Count == 0)
        {
            return -1;
        }
        Random rd = new Random();
        int ranIdx = rd.Next() % restPai.Count;
        int paiId = restPai[ranIdx];
        restPai.RemoveAt(ranIdx);

        int arrIndex = playerPai[id].Count;
        playerPai[id].Add(paiId);
        while (arrIndex >= 1 && playerPai[id][arrIndex] < playerPai[id][arrIndex - 1])
        {
            //arrIndex和arrIndex-1交换元素
            int tmp = playerPai[id][arrIndex];
            playerPai[id][arrIndex] = playerPai[id][arrIndex - 1];
            playerPai[id][arrIndex - 1] = tmp;
            arrIndex--;
        }

        return paiId;
    }

    public int ChuPai(int paiIndex,int id)
    {
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
    public Queue<MsgChiPengGang> HasEvent(int paiId,int id)
    {
        Queue<MsgChiPengGang> queue = new Queue<MsgChiPengGang>();
        for(int i = id+1; i < id+4; i++)
        {
            int panId = i % 4;
            bool[] res = new bool[4];
            res[0] = true;//代表永远可以取消吃碰杠操作
            res[1] = HasChi(paiId,panId);//检测吃
            res[2] = HasPeng(paiId,panId);//检测碰
            res[3] = HasGang(paiId,panId);//检测杠
            if(res[1] | res[2] | res[3])//至少有一个为true，就可以发送消息
            {
                MsgChiPengGang msg = new MsgChiPengGang();
                msg.isChiPengGang = res;
                msg.id = panId;
                msg.paiId = paiId;
                queue.Enqueue(msg);
            }
        }
        return queue;
    }

    /// <summary>
    /// 判断是否出现吃的情况
    /// </summary>
    /// <param name="paiId">打出的牌的id</param>
    /// <param name="panId">要搜索的玩家id</param>
    /// <returns>是否有吃</returns>
    private bool HasChi(int paiId,int panId)
    {
        bool pan1 = playerPai[panId].Contains(paiId + 1) && playerPai[panId].Contains(paiId + 2);
        bool pan2 = playerPai[panId].Contains(paiId - 1) && playerPai[panId].Contains(paiId + 1);
        bool pan3 = playerPai[panId].Contains(paiId - 2) && playerPai[panId].Contains(paiId - 1);
        return pan1 | pan2 | pan3;
    }

    private bool HasPeng(int paiId,int panId)
    {
        int count = 0;
        for(int i = 0; i < playerPai[panId].Count; i++)
        {
            if(playerPai[panId][i] == paiId)
            {
                count++;
            }
        }
        return count >= 2;
    }

    private bool HasGang(int paiId,int panId)
    {
        int count = 0;
        for (int i = 0; i < playerPai[panId].Count; i++)
        {
            if (playerPai[panId][i] == paiId)
            {
                count++;
            }
        }
        return count >= 3;
    }

    private bool HasHu(int paiId,int panId)
    {
        List<int> cards = playerPai[panId];
        int len = playerPai[panId].Count;
        if(len == 14)
        {
            int count = 0;
            for(int i = 0; i < len; i += 2)
            {
                if(cards[i] == cards[i + 1])
                {
                    count++;
                }
                else
                {
                    break;
                }
            }
            if(count == 7) { return true; }
        }
        int[][] handcards = new int[4][] { new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 },
                new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new int[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 } };
        for(int i = 0; i < len; i++)
        {
            int type = cards[i] / 10;
            int number = cards[i] % 10;
            switch (type)
            {
                case 0:
                    handcards[0][number]++;
                    handcards[0][0]++;
                    break;
                case 1:
                    handcards[1][number]++;
                    handcards[1][0]++;
                    break;
                case 2:
                    handcards[2][number]++;
                    handcards[2][0]++;
                    break;
                case 3:
                    handcards[3][number]++;
                    handcards[3][0]++;
                    break;
            }
        }

        bool isJiang = false;//判断是否有对子
        int jiangNumber = -1;
        for(int i = 0; i < handcards.GetLength(0); i++)
        {
            if (handcards[i][0] % 3 == 2)
            {
                if (isJiang)
                {
                    return false;
                }
                isJiang = true;
                jiangNumber = i;
            }
            //因为对应四种牌型只能有一种且仅包含一个对子
        }
        //先求没有将牌的情况判断其是不是都是由刻子或者砍组成
        for (int i = 0; i < handcards.GetLength(0); i++)
        {
            if (i != jiangNumber)
            {
                if (!(IsKanOrShun(handcards[i], i == 3)))
                {
                    return false;
                }
            }
        }
        bool success = false;
        //有将牌的情况下
        for(int i = 1; i <= 9; i++)
        {
            if(handcards[jiangNumber][i] >= 2)
            {
                handcards[jiangNumber][i] -= 2;
                handcards[jiangNumber][0] -= 2;
                if(IsKanOrShun(handcards[jiangNumber], jiangNumber == 3))
                {
                    success = true;
                    break;
                }
                else
                {
                    handcards[jiangNumber][i] += 2;
                    handcards[jiangNumber][0] += 2;
                }
            }
        }
        return success;
    }

    //判断是否满足牌组为顺子或砍组成
    private static bool IsKanOrShun(int[] arr, bool isZi)
    {
        if (arr[0] == 0)
        {
            return true;
        }

        int index = -1;
        for (int i = 1; i < arr.Length; i++)
        {
            if (arr[i] > 0)
            {
                index = i;
                break;
            }
        }
        bool result;
        //是否满足全是砍
        if (arr[index] >= 3)
        {
            arr[index] -= 3;
            arr[0] -= 3;
            result = IsKanOrShun(arr, isZi);
            arr[index] += 3;
            arr[0] += 3;
            return result;
        }
        //是否满足为顺子
        if (!isZi)
        {
            if (index < 8 && arr[index + 1] >= 1 && arr[index + 2] >= 1)
            {
                arr[index] -= 1;
                arr[index + 1] -= 1;
                arr[index + 2] -= 1;
                arr[0] -= 3;
                result = IsKanOrShun(arr, isZi);
                arr[index] += 1;
                arr[index + 1] += 1;
                arr[index + 2] += 1;
                arr[0] += 3;
                return result;
            }
        }

        return false;
    }

    /// <summary>
    /// 更新牌库
    /// 返回值是为了测试
    /// </summary>
    /// <param name="id"></param>
    /// <param name="paiId"></param>
    public bool OnChi(int id,int paiId)
    {
        //分为三种可能
        if (NumOfPai(id,paiId - 1) >= 1 && NumOfPai(id,paiId - 2) >= 1)
        {
            int index1 = playerPai[id].IndexOf(paiId - 1);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId - 2);
            playerPai[id].RemoveAt(index2);
        }
        else if (NumOfPai(id,paiId - 1) >= 1 && NumOfPai(id,paiId + 1) >= 1)
        {
            int index1 = playerPai[id].IndexOf(paiId - 1);
            playerPai[id].RemoveAt(index1);

            int index2 = playerPai[id].IndexOf(paiId + 1);
            playerPai[id].RemoveAt(index2);
        }
        else if (NumOfPai(id,paiId + 1) >= 1 && NumOfPai(id,paiId + 2) >= 1)
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
        if (NumOfPai(id,paiId) >= 2)
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

    private int NumOfPai(int id,int paiId)
    {
        int count = 0;
        for(int i = 0; i < playerPai[id].Count; i++)
        {
            if(paiId == playerPai[id][i])
            {
                count++;
            }
        }
        return count;
    }
    //四个玩家四个list,存在List中
    //List<>
    //判断吃，碰，杠，胡（发送协议）

}
