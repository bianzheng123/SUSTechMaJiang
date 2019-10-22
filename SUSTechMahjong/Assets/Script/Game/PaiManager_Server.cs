using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaiManager_Server {

    private static PaiManager_Server _instance = null;  

    private PaiManager_Server() { }


    public static PaiManager_Server GetInstance() 
    {
        if (_instance == null)
        {
            _instance = new PaiManager_Server();
        }
        return _instance;
    }//单例


    //剩下的牌
    List<int> restPai;

    public void Init()
    {
        restPai = new List<int>();
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

    public int[] FaPai(int num)
    {
        int[] res = new int[num];
        for(int i = 0; i < num; i++)
        {
            
            int ranIdx = UnityEngine.Random.Range(0,restPai.Count);
            res[i] = restPai[ranIdx];
            restPai.RemoveAt(ranIdx);
        }
        return res;
    }

    
    //四个玩家四个list,存在List中
    //List<>
    //判断吃，碰，杠，胡（发送协议）

}
