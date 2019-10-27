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
    public static Dictionary<int, string> name2path;
    
    public Pai(int paiId)
    {
        this.paiId = paiId;
    }

    public static void Init()
    {
        name2path = new Dictionary<int, string>();
        name2path.Add(1, "Mahjong/Middle/handmah_11");
        name2path.Add(2, "Mahjong/Middle/handmah_12");
        name2path.Add(3, "Mahjong/Middle/handmah_13");
        name2path.Add(4, "Mahjong/Middle/handmah_14");
        name2path.Add(5, "Mahjong/Middle/handmah_15");
        name2path.Add(6, "Mahjong/Middle/handmah_16");
        name2path.Add(7, "Mahjong/Middle/handmah_17");
        name2path.Add(8, "Mahjong/Middle/handmah_18");
        name2path.Add(9, "Mahjong/Middle/handmah_19");

        name2path.Add(11, "Mahjong/Middle/handmah_21");
        name2path.Add(12, "Mahjong/Middle/handmah_22");
        name2path.Add(13, "Mahjong/Middle/handmah_23");
        name2path.Add(14, "Mahjong/Middle/handmah_24");
        name2path.Add(15, "Mahjong/Middle/handmah_25");
        name2path.Add(16, "Mahjong/Middle/handmah_26");
        name2path.Add(17, "Mahjong/Middle/handmah_27");
        name2path.Add(18, "Mahjong/Middle/handmah_28");
        name2path.Add(19, "Mahjong/Middle/handmah_29");

        name2path.Add(21, "Mahjong/Middle/handmah_31");
        name2path.Add(22, "Mahjong/Middle/handmah_32");
        name2path.Add(23, "Mahjong/Middle/handmah_33");
        name2path.Add(24, "Mahjong/Middle/handmah_34");
        name2path.Add(25, "Mahjong/Middle/handmah_35");
        name2path.Add(26, "Mahjong/Middle/handmah_36");
        name2path.Add(27, "Mahjong/Middle/handmah_37");
        name2path.Add(28, "Mahjong/Middle/handmah_38");
        name2path.Add(29, "Mahjong/Middle/handmah_39");

        name2path.Add(31, "Mahjong/Middle/handmah_41");
        name2path.Add(32, "Mahjong/Middle/handmah_42");
        name2path.Add(33, "Mahjong/Middle/handmah_43");
        name2path.Add(34, "Mahjong/Middle/handmah_44");

        name2path.Add(41, "Mahjong/Middle/handmah_46");
        name2path.Add(42, "Mahjong/Middle/handmah_45");
        name2path.Add(43, "Mahjong/Middle/handmah_47");
    }
}
