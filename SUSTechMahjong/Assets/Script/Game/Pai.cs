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
    public static Dictionary<int, string> name2path_handPai;
    
    public Pai(int paiId)
    {
        this.paiId = paiId;
    }

    public static void Init()
    {
        name2path_handPai = new Dictionary<int, string>();
        name2path_handPai.Add(1, "Mahjong/Middle/handmah_1");
        name2path_handPai.Add(2, "Mahjong/Middle/handmah_2");
        name2path_handPai.Add(3, "Mahjong/Middle/handmah_3");
        name2path_handPai.Add(4, "Mahjong/Middle/handmah_4");
        name2path_handPai.Add(5, "Mahjong/Middle/handmah_5");
        name2path_handPai.Add(6, "Mahjong/Middle/handmah_6");
        name2path_handPai.Add(7, "Mahjong/Middle/handmah_7");
        name2path_handPai.Add(8, "Mahjong/Middle/handmah_8");
        name2path_handPai.Add(9, "Mahjong/Middle/handmah_9");

        name2path_handPai.Add(11, "Mahjong/Middle/handmah_11");
        name2path_handPai.Add(12, "Mahjong/Middle/handmah_12");
        name2path_handPai.Add(13, "Mahjong/Middle/handmah_13");
        name2path_handPai.Add(14, "Mahjong/Middle/handmah_14");
        name2path_handPai.Add(15, "Mahjong/Middle/handmah_15");
        name2path_handPai.Add(16, "Mahjong/Middle/handmah_16");
        name2path_handPai.Add(17, "Mahjong/Middle/handmah_17");
        name2path_handPai.Add(18, "Mahjong/Middle/handmah_18");
        name2path_handPai.Add(19, "Mahjong/Middle/handmah_19");

        name2path_handPai.Add(21, "Mahjong/Middle/handmah_21");
        name2path_handPai.Add(22, "Mahjong/Middle/handmah_22");
        name2path_handPai.Add(23, "Mahjong/Middle/handmah_23");
        name2path_handPai.Add(24, "Mahjong/Middle/handmah_24");
        name2path_handPai.Add(25, "Mahjong/Middle/handmah_25");
        name2path_handPai.Add(26, "Mahjong/Middle/handmah_26");
        name2path_handPai.Add(27, "Mahjong/Middle/handmah_27");
        name2path_handPai.Add(28, "Mahjong/Middle/handmah_28");
        name2path_handPai.Add(29, "Mahjong/Middle/handmah_29");

        name2path_handPai.Add(31, "Mahjong/Middle/handmah_31");
        name2path_handPai.Add(32, "Mahjong/Middle/handmah_32");
        name2path_handPai.Add(33, "Mahjong/Middle/handmah_33");
        name2path_handPai.Add(34, "Mahjong/Middle/handmah_34");

        name2path_handPai.Add(41, "Mahjong/Middle/handmah_41");
        name2path_handPai.Add(42, "Mahjong/Middle/handmah_42");
        name2path_handPai.Add(43, "Mahjong/Middle/handmah_43");
    }
}
