using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayer : MonoBehaviour {
    protected GameManager gameManager;
    protected GamePanel gamePanel;
    public MsgChiPengGang msgChiPengGang;//存放上一个吃碰杠协议的引用

    //玩家手中的牌
    public List<GameObject> handPai;
    public List<int> discardPai;

    //吃，碰杠
    public List<int> chi;
    public List<int> peng;
    public List<int> gang;

    //id
    public int id;
    public string username;
    public Gender gender;
    //代表是否轮到自己出牌
    public bool isTurn;
    //描述自己的技能
    public Major skill;

    public virtual void Init(GamePanel gamePanel) {
        handPai = new List<GameObject>();
        chi = new List<int>();
        peng = new List<int>();
        gang = new List<int>();
        discardPai = new List<int>();
        gameManager = GameManager.GetInstance();
        this.gamePanel = gamePanel;
    }

    /// <summary>
    /// 玩家发到牌，用于对牌进行排序
    /// </summary>
    public void SynHandPai()
    {
        //调整顺序
        int paiId = handPai[handPai.Count - 1].GetComponent<Pai>().paiId;
        int paiIndex = handPai.Count - 1;
        while (paiIndex >= 1 && handPai[paiIndex].GetComponent<Pai>().paiId < handPai[paiIndex - 1].GetComponent<Pai>().paiId)
        {
            GameObject tmp = handPai[paiIndex];
            handPai[paiIndex] = handPai[paiIndex - 1];
            handPai[paiIndex - 1] = tmp;
            paiIndex--;
        }
    }

    /// <summary>
    /// 调整手牌的位置
    /// </summary>
    public void PlacePai()
    {
        Direction dir = gamePanel.numToDir[id];
        switch (dir)
        {
            case Direction.DOWN:
                for (int i = 0; i < handPai.Count; i++)
                {
                    handPai[i].transform.position = new Vector3(i * 0.76f - 5.3f, -4, 0);
                }
                break;
            case Direction.RIGHT:
                for (int i = 0; i < handPai.Count; i++)
                {
                    handPai[i].transform.position = new Vector3(7, -4 + i * 0.6f, 0);
                }
                break;
            case Direction.UP:
                for (int i = 0; i < handPai.Count; i++)
                {
                    handPai[i].transform.position = new Vector3(i * 0.76f - 5, 4, 0);
                }
                break;
            case Direction.LEFT:
                for (int i = 0; i < handPai.Count; i++)
                {
                    handPai[i].transform.position = new Vector3(-7, -4 + i * 0.6f, 0);
                }
                break;
        }
        
    }

    //出牌
    public abstract void DaPai();

    /// <summary>
    /// 没有参数代表强制执行吃碰杠操作
    /// </summary>
    /// <param name="index">0代表什么都不做,1代表吃,2代表碰,3代表杠</param>
    public void ChiPengGang(int index = 0)
    {
        if (msgChiPengGang.isChiPengGang[index])//代表相关操作允许
        {
            msgChiPengGang.result = index;
        }
        else
        {//异常情况
            msgChiPengGang.result = 0;
            Debug.Log("非法操作！！！！！");
        }
        gamePanel.ChiButton = false;
        gamePanel.PengButton = false;
        gamePanel.GangButton = false;
        gameManager.startTimeCount = false;
        NetManager.Send(msgChiPengGang);
        msgChiPengGang = null;
    }

    /// <summary>
    /// 已经选择了出牌或者胡，将出牌的按钮隐藏，并发送ChuPai协议
    /// </summary>
    /// <param name="index">pai的索引</param>
    protected void ChuPai_Hu(int index = -1)
    {
        gameManager.startTimeCount = false;
        if(index == -1)
        {
            Debug.Log("playerid: " + id + "胡了");
        }
        else
        {
            Debug.Log("playerid: " + id + " 打出 " + Pai.int2name[handPai[index].GetComponent<Pai>().paiId]);
        }
        
        MsgChuPai msg = new MsgChuPai();
        msg.id = id;
        msg.paiIndex = index;
        gamePanel.ChuPaiButton = false;
        gamePanel.HuButton = false;
        gameManager.isChuPai = false;
        NetManager.Send(msg);
        
    }

    /// <summary>
    /// 在客户端收到MsgChuPai的时候调用，对客户端进行同步，删除对应的牌，同时播放声音
    /// </summary>
    /// <param name="paiIndex"></param>
    public void DiscardPai(int paiIndex)
    {
        GameObject go = handPai[paiIndex];
        int paiId = go.GetComponent<Pai>().paiId;
        Debug.Log("声音：" + paiId);
        switch (gender)
        {
            case Gender.Male:
                Audio.PlayCue(Audio.audioPathMale[paiId]);
                break;
            case Gender.Female:
                Audio.PlayCue(Audio.audioPathFemale[paiId]);
                break;
        }
        handPai.RemoveAt(paiIndex);
        discardPai.Add(paiId);
        Destroy(go);
        PlacePai();
        go = null;
    }

    /// <summary>
    /// 接收服务端广播吃的协议
    /// </summary>
    public void OnChi(int paiId)
    {
        switch (gender)
        {
            case Gender.Male:
                Audio.PlayCue(Audio.audioChiMale);
                break;
            case Gender.Female:
                Audio.PlayCue(Audio.audioChiFemale);
                break;
        }
        //无法播放对应的声音，没有音效
        //分为三种可能
        if (NumOfPai(paiId-1) >= 1 && NumOfPai(paiId - 2) >= 1)
        {
            AddChiPengGang(paiId-1,enum_ChiPengGang.Chi);
            AddChiPengGang(paiId - 2,enum_ChiPengGang.Chi);
            chi.Add(paiId);
        }
        else if(NumOfPai(paiId - 1) >= 1 && NumOfPai(paiId + 1) >= 1)
        {
            AddChiPengGang(paiId - 1,enum_ChiPengGang.Chi);
            AddChiPengGang(paiId + 1,enum_ChiPengGang.Chi);
            chi.Add(paiId);
        }
        else if(NumOfPai(paiId + 1) >= 1 && NumOfPai(paiId + 2) >= 1)
        {
            chi.Add(paiId);
            AddChiPengGang(paiId + 1, enum_ChiPengGang.Chi);
            AddChiPengGang(paiId + 2, enum_ChiPengGang.Chi);
        }
        else
        {
            Debug.Log("客户端处理吃出现bug");
        }
        chi.Sort();
        PlacePai();
    }

    /// <summary>
    /// 接收服务端广播碰的协议
    /// </summary>
    public void OnPeng(int paiId)
    {
        if (NumOfPai(paiId) >= 2)
        {
            AddChiPengGang(paiId, enum_ChiPengGang.Peng);
            AddChiPengGang(paiId, enum_ChiPengGang.Peng);
            peng.Add(paiId);
            switch (gender)
            {
                case Gender.Male:
                    Audio.PlayCue(Audio.audioPengMale);
                    break;
                case Gender.Female:
                    Audio.PlayCue(Audio.audioPengFemale);
                    break;
            }
        }
        else
        {
            Debug.Log("客户端处理碰出现bug");
        }
        peng.Sort();
        PlacePai();
    }

    /// <summary>
    /// 接收服务端广播杠的协议
    /// </summary>
    public void OnGang(int paiId)
    {
        if (NumOfPai(paiId) >= 3)
        {
            AddChiPengGang(paiId, enum_ChiPengGang.Gang);
            AddChiPengGang(paiId, enum_ChiPengGang.Gang);
            AddChiPengGang(paiId, enum_ChiPengGang.Gang);
            gang.Add(paiId);
            switch (gender)
            {
                case Gender.Male:
                    Audio.PlayCue(Audio.audioGangMale);
                    break;
                case Gender.Female:
                    Audio.PlayCue(Audio.audioGangFemale);
                    break;
            }
        }
        else
        {
            Debug.Log("客户端处理杠出现bug");
        }
        gang.Sort();
        PlacePai();
    }

    /// <summary>
    /// 处理吃碰杠事件，即从手牌中删除对应的牌，并添加到吃碰杠的列表中
    /// </summary>
    /// <param name="paiId"></param>
    /// <param name="type"></param>
    private void AddChiPengGang(int paiId,enum_ChiPengGang type)
    {
        int index1 = IndexOf(paiId);
        GameObject pai1 = handPai[index1];
        Destroy(pai1);
        handPai.RemoveAt(index1);
        int paiId1 = pai1.GetComponent<Pai>().paiId;
        pai1 = null;
        switch (type)
        {
            case enum_ChiPengGang.Chi:
                chi.Add(paiId1);
                break;
            case enum_ChiPengGang.Peng:
                peng.Add(paiId1);
                break;
            case enum_ChiPengGang.Gang:
                gang.Add(paiId1);
                break;
        }

    }

    /// <summary>
    /// 用于吃碰杠操作,判断查找的paiId有多少张
    /// </summary>
    /// <param name="paiId"></param>
    private int NumOfPai(int paiId)
    {
        int count = 0;
        for(int i = 0; i < handPai.Count; i++)
        {
            int id = handPai[i].GetComponent<Pai>().paiId;
            if(paiId == id)
            {
                count++;
            }
        }
        return count;
    }

    /// <summary>
    /// 查看对应paiId的索引值
    /// </summary>
    /// <param name="paiId"></param>
    /// <returns></returns>
    private int IndexOf(int paiId)
    {
        int index = -1;
        for (int i = 0; i < handPai.Count; i++)
        {
            int id = handPai[i].GetComponent<Pai>().paiId;
            if (paiId == id)
            {
                index = i;
            }
        }
        return index;
    }

    enum enum_ChiPengGang
    {
        Chi = 0,
        Peng = 1,
        Gang = 2
    }
}


