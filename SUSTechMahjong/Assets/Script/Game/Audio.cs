﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Audio {
    public static Dictionary<int, string> audioPathMale;
    public static string audioGangMale;
    public static string audioHuMale;
    public static string audioPengMale;
    public static Dictionary<int, string> audioPathFemale;
    public static string audioGangFemale;
    public static string audioHuFemale;
    public static string audioPengFemale;

    public static string bgGamePanel;
    public static string bgLoginPanel;
    public static string bgRoomListPanel;
    public static string bgRoomPanel;
    public static string lose;
    public static string timeup_alarm;
    public static string win;

    public static void Init()
    {
        bgGamePanel = "Audios/bgGamePanel";
        bgLoginPanel = "Audios/bgLoginPanel";
        bgRoomListPanel = "Audios/bgRoomListPanel";
        bgRoomPanel = "Audios/bgRoomPanel";
        lose = "Audios/lose";
        timeup_alarm = "Audios/timeup_alarm";
        win = "Audios/win";

        audioGangMale = "Audios/Mahjong/male/gang";
        audioHuMale = "Audios/Mahjong/male/hu";
        audioPengMale = "Audios/Mahjong/male/peng";
        audioPathMale = new Dictionary<int, string>();
        for(int i = 0; i < 3; i++)
        {
            for(int j = 1; j <= 9; j++)
            {
                audioPathMale.Add(i * 10 + j, "Audios/Mahjong/male/mjt" + i.ToString() + "_" + j.ToString());
            }
        }

        audioPathMale.Add(31, "Audios/Mahjong/male/mjt3_1");
        audioPathMale.Add(32, "Audios/Mahjong/male/mjt3_2");
        audioPathMale.Add(33, "Audios/Mahjong/male/mjt3_3");
        audioPathMale.Add(34, "Audios/Mahjong/male/mjt3_4");

        audioPathMale.Add(41, "Audios/Mahjong/male/mjt4_1");
        audioPathMale.Add(42, "Audios/Mahjong/male/mjt4_2");
        audioPathMale.Add(43, "Audios/Mahjong/male/mjt4_3");

        audioGangFemale = "Audios/Mahjong/female/gang";
        audioHuFemale = "Audios/Mahjong/female/hu";
        audioPengFemale = "Audios/Mahjong/female/peng";
        audioPathFemale = new Dictionary<int, string>();

        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                audioPathFemale.Add(i * 10 + j, "Audios/Mahjong/female/mjt" + i.ToString() + "_" + j.ToString());
            }
        }

        audioPathFemale.Add(31, "Audios/Mahjong/female/mjt3_1");
        audioPathFemale.Add(32, "Audios/Mahjong/female/mjt3_2");
        audioPathFemale.Add(33, "Audios/Mahjong/female/mjt3_3");
        audioPathFemale.Add(34, "Audios/Mahjong/female/mjt3_4");

        audioPathFemale.Add(41, "Audios/Mahjong/female/mjt4_1");
        audioPathFemale.Add(42, "Audios/Mahjong/female/mjt4_2");
        audioPathFemale.Add(43, "Audios/Mahjong/female/mjt4_3");
    }
}
