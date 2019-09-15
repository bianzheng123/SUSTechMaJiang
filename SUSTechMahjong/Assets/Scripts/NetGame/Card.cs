using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    [SerializeField]
    private Sprite sprite;
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

    private void Start()
    {
        sprite = CardManager.Instance.HandSprite[(int)type * 9 + index];
    }
}
