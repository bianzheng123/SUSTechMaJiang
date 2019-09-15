using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class CardManager : NetworkBehaviour {
    private static CardManager _instance;
    /// <summary>
    /// 获取棋牌的对象
    /// </summary>
    [SerializeField]
    private Sprite[] handSprite;
    /// <summary>
    /// cards用于生成棋牌
    /// </summary>
    [SerializeField]
    private Card[] cards = new Card[108];
    /// <summary>
    /// 获取棋牌的实例
    /// </summary>
    [SerializeField]
    private GameObject prefab_card;
    //private SpriteRenderer render;

    public Sprite[] HandSprite
    {
        get { return handSprite; }
    }
    public static CardManager Instance
    {
        get { return _instance; }
    }
    public Card[] Cards
    {
        get { return cards; }
    }

    private void Awake()
    {
        if (Instance == null)
        {
            _instance = this;
        }
        //render = GetComponent<SpriteRenderer>();
        
        //for(int i = 0; i < 108; i++)
        //{
        //    Debug.Log(cards[i].ToString());
        //}
    }
    [Command]
    private void CmdWashCard()
    {
        if (null == cards || 0 == cards.Length)
        {
            Debug.Log("生成牌时出现错误！");
            return;
        }

        int length = cards.Length;
        /* 每次发牌的时候任意分配待交换的数据 */
        for (int index = 0; index < length; index++)
        {
            int value = Random.Range(0,108);

            Card median = cards[index];
            cards[index] = cards[value];
            cards[value] = median;

            cards[index].gameObject.transform.name = (cards[index].Index + 1) + "-" + cards[index].Type;
            cards[index].GetComponent<SpriteRenderer>().sprite = handSprite[cards[index].Index + 9 * (int)cards[index].Type];

            cards[value].gameObject.transform.name = (cards[value].Index + 1) + "-" + cards[value].Type;
            cards[value].GetComponent<SpriteRenderer>().sprite = handSprite[cards[value].Index + 9 * (int)cards[value].Type];
        }
    }

    /// <summary>
    /// 生成牌的引用
    /// </summary>
    private void CreateCards()
    {
        for(int i = 0; i < 4; i++)//相同的牌有4个
        {
            for (int j = 0; j < 9; j++)//每一个花色都有9个数字
            {
                for (int k = 0; k < 3; k++)//一共有3个花色
                {
                    cards[i * 27 + j * 3 + k].Type = (HuaSe)k;
                    cards[i * 27 + j * 3 + k].Index = j;
                    cards[i * 27 + j * 3 + k].gameObject.transform.name = (cards[i * 27 + j * 3 + k].Index + 1) + "-" + cards[i * 27 + j * 3 + k].Type;
                    cards[i * 27 + j * 3 + k].GetComponent<SpriteRenderer>().sprite = handSprite[cards[i * 27 + j * 3 + k].Index + 9 * (int)cards[i * 27 + j * 3 + k].Type];

                }
            }
        }
    }
    public void InitializeCard()
    {
        InstantiateCard();
        CreateCards();
        CmdWashCard();
    }

    private void InstantiateCard()
    {
        int len = cards.Length;
        for (int i = 0; i < len; i++)
        {
            GameObject go = Instantiate(prefab_card, Vector3.zero, Quaternion.identity);
            Debug.Log(NetworkServer.active);
            
            cards[i] = go.GetComponent<Card>();
            go.transform.SetParent(transform);
            go.transform.position = new Vector3(100000, 100000, 0);
        }

        
    }

    void Update()
    {
        //render.sprite = handSprite[frame];
        
    }
}

public enum HuaSe
{
    /// <summary>
    /// 筒
    /// </summary>
    TONG=0,

    /// <summary>
    /// 万
    /// </summary>
    WAN = 1,

    /// <summary>
    /// 条
    /// </summary>
    TIAO =2
    
}
