﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Pai : MonoBehaviour
{
    //存放牌的路径
    public int paiId;
    //牌索引及其对应字典
    public static Dictionary<int, string> pai_player1;
    public static string pai_player2;
    public static string pai_player3;
    public static string pai_player4;
    public static Dictionary<int,string> int2name;
    
    public Pai(int paiId)
    {
        this.paiId = paiId;
    }

    public static void Init()
    {
        pai_player2 = "Mahjong/hand_right";
        pai_player3 = "Mahjong/hand_top";
        pai_player4 = "Mahjong/hand_left";

        pai_player1 = new Dictionary<int, string>();
        pai_player1.Add(1, "Mahjong/Middle/handmah_1");
        pai_player1.Add(2, "Mahjong/Middle/handmah_2");
        pai_player1.Add(3, "Mahjong/Middle/handmah_3");
        pai_player1.Add(4, "Mahjong/Middle/handmah_4");
        pai_player1.Add(5, "Mahjong/Middle/handmah_5");
        pai_player1.Add(6, "Mahjong/Middle/handmah_6");
        pai_player1.Add(7, "Mahjong/Middle/handmah_7");
        pai_player1.Add(8, "Mahjong/Middle/handmah_8");
        pai_player1.Add(9, "Mahjong/Middle/handmah_9");

        pai_player1.Add(11, "Mahjong/Middle/handmah_11");
        pai_player1.Add(12, "Mahjong/Middle/handmah_12");
        pai_player1.Add(13, "Mahjong/Middle/handmah_13");
        pai_player1.Add(14, "Mahjong/Middle/handmah_14");
        pai_player1.Add(15, "Mahjong/Middle/handmah_15");
        pai_player1.Add(16, "Mahjong/Middle/handmah_16");
        pai_player1.Add(17, "Mahjong/Middle/handmah_17");
        pai_player1.Add(18, "Mahjong/Middle/handmah_18");
        pai_player1.Add(19, "Mahjong/Middle/handmah_19");

        pai_player1.Add(21, "Mahjong/Middle/handmah_21");
        pai_player1.Add(22, "Mahjong/Middle/handmah_22");
        pai_player1.Add(23, "Mahjong/Middle/handmah_23");
        pai_player1.Add(24, "Mahjong/Middle/handmah_24");
        pai_player1.Add(25, "Mahjong/Middle/handmah_25");
        pai_player1.Add(26, "Mahjong/Middle/handmah_26");
        pai_player1.Add(27, "Mahjong/Middle/handmah_27");
        pai_player1.Add(28, "Mahjong/Middle/handmah_28");
        pai_player1.Add(29, "Mahjong/Middle/handmah_29");

        pai_player1.Add(31, "Mahjong/Middle/handmah_31");
        pai_player1.Add(32, "Mahjong/Middle/handmah_32");
        pai_player1.Add(33, "Mahjong/Middle/handmah_33");
        pai_player1.Add(34, "Mahjong/Middle/handmah_34");

        pai_player1.Add(41, "Mahjong/Middle/handmah_41");
        pai_player1.Add(42, "Mahjong/Middle/handmah_42");
        pai_player1.Add(43, "Mahjong/Middle/handmah_43");

        int2name = new Dictionary<int, string>();
        int2name.Add(1, "一筒");
        int2name.Add(2, "二筒");
        int2name.Add(3, "三筒");
        int2name.Add(4, "四筒");
        int2name.Add(5, "五筒");
        int2name.Add(6, "六筒");
        int2name.Add(7, "七筒");
        int2name.Add(8, "八筒");
        int2name.Add(9, "九筒");

        int2name.Add(11, "一万");
        int2name.Add(12, "二万");
        int2name.Add(13, "三万");
        int2name.Add(14, "四万");
        int2name.Add(15, "五万");
        int2name.Add(16, "六万");
        int2name.Add(17, "七万");
        int2name.Add(18, "八万");
        int2name.Add(19, "九万");

        int2name.Add(21, "一条");
        int2name.Add(22, "二条");
        int2name.Add(23, "三条");
        int2name.Add(24, "四条");
        int2name.Add(25, "五条");
        int2name.Add(26, "六条");
        int2name.Add(27, "七条");
        int2name.Add(28, "八条");
        int2name.Add(29, "九条");

        int2name.Add(31,"东");
        int2name.Add(32,"南");
        int2name.Add(33,"西");
        int2name.Add(34, "北");
        int2name.Add(41, "发财");
        int2name.Add(42, "中");
        int2name.Add(43, "空");
    }
}
