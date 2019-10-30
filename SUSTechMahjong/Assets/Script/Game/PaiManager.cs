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

    public void ChuPai(int paiIndex,int id)
    {
        playerPai[id].RemoveAt(paiIndex);
    }
    
    //四个玩家四个list,存在List中
    //List<>
    //判断吃，碰，杠，胡（发送协议）

}
