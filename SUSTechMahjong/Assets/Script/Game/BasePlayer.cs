using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour {
    //玩家手中的牌
    public List<GameObject> pai;

    //吃，碰杠list
    public List<GameObject> chi;
    public List<GameObject> peng;
    public List<GameObject> gang;

    //id
    public int id;
    //代表是否轮到自己出牌
    public bool isTurn;


    public virtual void Init() { 
        pai = new List<GameObject>();
        chi = new List<GameObject>();
        peng = new List<GameObject>();
        gang = new List<GameObject>();
    }

    //出牌
    public virtual void ChuPai()
    {

    }

    //系统给玩家发牌
    public virtual void GetPai()
    {

    }
}
