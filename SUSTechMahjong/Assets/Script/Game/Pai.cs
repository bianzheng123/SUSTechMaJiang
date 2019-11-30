using System;
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
    /// <summary>
    /// 用于测试，显示游戏物体名字的
    /// </summary>
    public static Dictionary<int,string> int2name;
    public static Dictionary<int, string> audioPathMale;
    public static string audioGangMale;
    public static string audioHuMale;
    public static string audioPengMale;
    public static Dictionary<int, string> audioPathFemale;
    public static string audioGangFemale;
    public static string audioHuFemale;
    public static string audioPengFemale;
    
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

        audioGangMale = "GameLayer/Audios/male/gang";
        audioHuMale = "GameLayer/Audios/male/hu";
        audioPengMale = "GameLayer/Audios/male/peng";
        audioPathMale = new Dictionary<int, string>();
        audioPathMale.Add(1, "GameLayer/Audios/male/mjt0_1");
        audioPathMale.Add(2, "GameLayer/Audios/male/mjt0_2");
        audioPathMale.Add(3, "GameLayer/Audios/male/mjt0_3");
        audioPathMale.Add(4, "GameLayer/Audios/male/mjt0_4");
        audioPathMale.Add(5, "GameLayer/Audios/male/mjt0_5");
        audioPathMale.Add(6, "GameLayer/Audios/male/mjt0_6");
        audioPathMale.Add(7, "GameLayer/Audios/male/mjt0_7");
        audioPathMale.Add(8, "GameLayer/Audios/male/mjt0_8");
        audioPathMale.Add(9, "GameLayer/Audios/male/mjt0_9");

        audioPathMale.Add(11, "GameLayer/Audios/male/mjt1_1");
        audioPathMale.Add(12, "GameLayer/Audios/male/mjt1_2");
        audioPathMale.Add(13, "GameLayer/Audios/male/mjt1_3");
        audioPathMale.Add(14, "GameLayer/Audios/male/mjt1_4");
        audioPathMale.Add(15, "GameLayer/Audios/male/mjt1_5");
        audioPathMale.Add(16, "GameLayer/Audios/male/mjt1_6");
        audioPathMale.Add(17, "GameLayer/Audios/male/mjt1_7");
        audioPathMale.Add(18, "GameLayer/Audios/male/mjt1_8");
        audioPathMale.Add(19, "GameLayer/Audios/male/mjt1_9");

        audioPathMale.Add(21, "GameLayer/Audios/male/mjt2_1");
        audioPathMale.Add(22, "GameLayer/Audios/male/mjt2_2");
        audioPathMale.Add(23, "GameLayer/Audios/male/mjt2_3");
        audioPathMale.Add(24, "GameLayer/Audios/male/mjt2_4");
        audioPathMale.Add(25, "GameLayer/Audios/male/mjt2_5");
        audioPathMale.Add(26, "GameLayer/Audios/male/mjt2_6");
        audioPathMale.Add(27, "GameLayer/Audios/male/mjt2_7");
        audioPathMale.Add(28, "GameLayer/Audios/male/mjt2_8");
        audioPathMale.Add(29, "GameLayer/Audios/male/mjt2_9");

        audioPathMale.Add(31, "GameLayer/Audios/male/mjt3_1");
        audioPathMale.Add(32, "GameLayer/Audios/male/mjt3_2");
        audioPathMale.Add(33, "GameLayer/Audios/male/mjt3_3");
        audioPathMale.Add(34, "GameLayer/Audios/male/mjt3_4");

        audioPathMale.Add(41, "GameLayer/Audios/male/mjt4_1");
        audioPathMale.Add(42, "GameLayer/Audios/male/mjt4_2");
        audioPathMale.Add(43, "GameLayer/Audios/male/mjt4_3");

        audioGangFemale = "GameLayer/Audios/female/gang";
        audioHuFemale = "GameLayer/Audios/female/hu"; 
        audioPengFemale = "GameLayer/Audios/female/peng";
        audioPathFemale = new Dictionary<int, string>();

        audioPathFemale.Add(1, "GameLayer/Audios/female/mjt0_1");
        audioPathFemale.Add(2, "GameLayer/Audios/female/mjt0_2");
        audioPathFemale.Add(3, "GameLayer/Audios/female/mjt0_3");
        audioPathFemale.Add(4, "GameLayer/Audios/female/mjt0_4");
        audioPathFemale.Add(5, "GameLayer/Audios/female/mjt0_5");
        audioPathFemale.Add(6, "GameLayer/Audios/female/mjt0_6");
        audioPathFemale.Add(7, "GameLayer/Audios/female/mjt0_7");
        audioPathFemale.Add(8, "GameLayer/Audios/female/mjt0_8");
        audioPathFemale.Add(9, "GameLayer/Audios/female/mjt0_9");

        audioPathFemale.Add(11, "GameLayer/Audios/female/mjt1_1");
        audioPathFemale.Add(12, "GameLayer/Audios/female/mjt1_2");
        audioPathFemale.Add(13, "GameLayer/Audios/female/mjt1_3");
        audioPathFemale.Add(14, "GameLayer/Audios/female/mjt1_4");
        audioPathFemale.Add(15, "GameLayer/Audios/female/mjt1_5");
        audioPathFemale.Add(16, "GameLayer/Audios/female/mjt1_6");
        audioPathFemale.Add(17, "GameLayer/Audios/female/mjt1_7");
        audioPathFemale.Add(18, "GameLayer/Audios/female/mjt1_8");
        audioPathFemale.Add(19, "GameLayer/Audios/female/mjt1_9");

        audioPathFemale.Add(21, "GameLayer/Audios/female/mjt2_1");
        audioPathFemale.Add(22, "GameLayer/Audios/female/mjt2_2");
        audioPathFemale.Add(23, "GameLayer/Audios/female/mjt2_3");
        audioPathFemale.Add(24, "GameLayer/Audios/female/mjt2_4");
        audioPathFemale.Add(25, "GameLayer/Audios/female/mjt2_5");
        audioPathFemale.Add(26, "GameLayer/Audios/female/mjt2_6");
        audioPathFemale.Add(27, "GameLayer/Audios/female/mjt2_7");
        audioPathFemale.Add(28, "GameLayer/Audios/female/mjt2_8");
        audioPathFemale.Add(29, "GameLayer/Audios/female/mjt2_9");

        audioPathFemale.Add(31, "GameLayer/Audios/female/mjt3_1");
        audioPathFemale.Add(32, "GameLayer/Audios/female/mjt3_2");
        audioPathFemale.Add(33, "GameLayer/Audios/female/mjt3_3");
        audioPathFemale.Add(34, "GameLayer/Audios/female/mjt3_4");

        audioPathFemale.Add(41, "GameLayer/Audios/female/mjt4_1");
        audioPathFemale.Add(42, "GameLayer/Audios/female/mjt4_2");
        audioPathFemale.Add(43, "GameLayer/Audios/female/mjt4_3");

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
