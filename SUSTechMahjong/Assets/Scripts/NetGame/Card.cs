using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Card : MonoBehaviour{
    /// <summary>
    /// 代表了这个牌在花色中的序号
    /// </summary>
   [SerializeField]
    private int index;
    /// <summary>
    /// 代表了这个牌的花色
    /// </summary>
    [SerializeField]
    private HuaSe type;
    public int Index
    {
        get { return index;}
        set { index = value; }
    }

    public HuaSe Type
    {
        get { return type; }
        set { type = value; }
    }
    public override string ToString()
    {
        return string.Format((index + 1) + "-" + type);
    }
}
