using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPaiInfo
{
    private Button discardPai;
    private Button chi;
    private Button peng;
    private Button gang;
    private int realId;
    private GameManager gameManager;

    /// <param name="discardPai">弃牌的内容</param>
    /// <param name="relativeId">观察玩家牌的id</param>
    /// <param name="client_id">该客户端的id</param>
    public PlayerPaiInfo(Button discardPai,Button chi,Button peng,Button gang)
    {
        this.discardPai = discardPai;
        this.chi = chi;
        this.peng = peng;
        this.gang = gang;
        gameManager = GameManager.GetInstance();
    }

    public void AddButtonClick(int relativeId, int client_id)
    {
        this.realId = (client_id + relativeId) % 4;
        discardPai.onClick.AddListener(Audio.ButtonClick);
        chi.onClick.AddListener(Audio.ButtonClick);
        peng.onClick.AddListener(Audio.ButtonClick);
        gang.onClick.AddListener(Audio.ButtonClick);

        discardPai.onClick.AddListener(OnDiscardPaiClick);
        chi.onClick.AddListener(OnChiClick);
        peng.onClick.AddListener(OnPengClick);
        gang.onClick.AddListener(OnGangClick);
    }

    private void OnDiscardPaiClick()
    {
        GetPlayerPai(Enum_OtherPlayerInfo.DiscardPai);
    }

    private void OnChiClick()
    {
        GetPlayerPai(Enum_OtherPlayerInfo.Chi);
    }

    private void OnPengClick()
    {
        GetPlayerPai(Enum_OtherPlayerInfo.Peng);
    }

    private void OnGangClick()
    {
        GetPlayerPai(Enum_OtherPlayerInfo.Gang);
    }

    private void GetPlayerPai(Enum_OtherPlayerInfo paiType)
    {
        int[] pai = null;
        List<int> list = null;
        switch (paiType)
        {
            case Enum_OtherPlayerInfo.DiscardPai:
                list = gameManager.players[realId].discardPai;
                break;
            case Enum_OtherPlayerInfo.Chi:
                list = gameManager.players[realId].chi;
                break;
            case Enum_OtherPlayerInfo.Peng:
                list = gameManager.players[realId].peng;
                break;
            case Enum_OtherPlayerInfo.Gang:
                list = gameManager.players[realId].gang;
                break;
        }
        if (list.Count != 0)
        {
            pai = list.ToArray();
        }
        GamePanel.DisplayOtherPai(pai);
    }

    /// <summary>
    /// 用来显示别的玩家的信息
    /// </summary>
    enum Enum_OtherPlayerInfo
    {
        DiscardPai,
        Chi,
        Peng,
        Gang
    }
}
