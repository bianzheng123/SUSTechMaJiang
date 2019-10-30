using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePlayer : MonoBehaviour {
    protected GameManager gameManager;
    protected GamePanel gamePanel;
    public MsgChiPengGang msg;//存放上一个吃碰杠协议的引用

    //玩家手中的牌
    public List<GameObject> handPai;
    public List<GameObject> discardPai;

    //吃，碰杠
    public List<GameObject> chi;
    public List<GameObject> peng;
    public List<GameObject> gang;

    //id
    public int id;
    //代表是否轮到自己出牌
    public bool isTurn;


    public virtual void Init(GameManager gameManager,GamePanel gamePanel) {
        handPai = new List<GameObject>();
        chi = new List<GameObject>();
        peng = new List<GameObject>();
        gang = new List<GameObject>();
        discardPai = new List<GameObject>();
        this.gameManager = gameManager;
        this.gamePanel = gamePanel;
    }

    public void PlacePai()
    {
        //调整顺序
        int paiId = handPai[handPai.Count - 1].GetComponent<Pai>().paiId;
        int paiIndex = handPai.Count - 1;
        while(paiIndex >= 1 && handPai[paiIndex].GetComponent<Pai>().paiId < handPai[paiIndex - 1].GetComponent<Pai>().paiId)
        {
            GameObject tmp = handPai[paiIndex];
            handPai[paiIndex] = handPai[paiIndex - 1];
            handPai[paiIndex - 1] = tmp;
            paiIndex--;
        }

        //调整牌的位置
        for (int i = 0; i < handPai.Count; i++)
        {
            handPai[i].transform.position = new Vector3(i - 6,-1-id,0);
        }
        for(int i = 0; i < discardPai.Count; i++)
        {
            discardPai[i].transform.position = new Vector3(i-6,4-id,0);
        }
    }

    //出牌
    public virtual void DaPai()
    {

    }

    /// <summary>
    /// 没有参数代表强制执行吃碰杠操作
    /// </summary>
    /// <param name="index">0代表什么都不做,1代表吃,2代表碰,3代表杠</param>
    public void ChiPengGang(int index = 0)
    {
        if (msg.isChiPengGang[index])//代表相关操作允许
        {
            msg.result = index;
            Debug.Log("成功进行相关操作");
        }
        else
        {//异常情况
            msg.result = 0;
            Debug.Log("非法操作！！！！！");
        }
        gameManager.startTimeCount = false;
        gameManager.ServerOnMsgChiPengGang(msg);

    }

    /// <summary>
    /// 代表出牌，将出牌的按钮隐藏，并发送ChuPai协议
    /// </summary>
    /// <param name="index">pai的索引</param>
    protected void ChuPai(int index)
    {
        gameManager.startTimeCount = false;
        GameObject go = handPai[index];
        Debug.Log("index: " + index + ",id: " + id);
        MsgChuPai msg = new MsgChuPai();
        msg.id = id;
        msg.paiIndex = index;
        gamePanel.ChuPaiButton = false;
        Debug.Log("出牌");
        gameManager.isChuPai = false;
        gameManager.ServerOnMsgChuPai(msg);
        
    }

    /// <summary>
    /// 在客户端收到MsgChuPai的时候调用
    /// </summary>
    /// <param name="paiIndex"></param>
    public void DiscardPai(int paiIndex)
    {
        GameObject go = handPai[paiIndex];
        handPai.RemoveAt(paiIndex);
        discardPai.Add(go);
        PlacePai();
    }
}
