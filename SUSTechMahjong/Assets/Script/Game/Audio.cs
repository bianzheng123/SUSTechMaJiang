using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio {
    public static Dictionary<int, string> audioPathMale;
    public static string audioGangMale;
    public static string audioHuMale;
    public static string audioPengMale;
    public static Dictionary<int, string> audioPathFemale;
    public static string audioGangFemale;
    public static string audioHuFemale;
    public static string audioPengFemale;

    public static void Init()
    {
        audioGangMale = "GameLayer/Audios/male/gang";
        audioHuMale = "GameLayer/Audios/male/hu";
        audioPengMale = "GameLayer/Audios/male/peng";
        audioPathMale = new Dictionary<int, string>();
        for(int i = 0; i < 3; i++)
        {
            for(int j = 1; j <= 9; j++)
            {
                audioPathMale.Add(i * 10 + j, "GameLayer/Audios/male/mjt" + i.ToString() + "_" + j.ToString());
            }
        }

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

        for (int i = 0; i < 3; i++)
        {
            for (int j = 1; j <= 9; j++)
            {
                audioPathFemale.Add(i * 10 + j, "GameLayer/Audios/female/mjt" + i.ToString() + "_" + j.ToString());
            }
        }

        audioPathFemale.Add(31, "GameLayer/Audios/female/mjt3_1");
        audioPathFemale.Add(32, "GameLayer/Audios/female/mjt3_2");
        audioPathFemale.Add(33, "GameLayer/Audios/female/mjt3_3");
        audioPathFemale.Add(34, "GameLayer/Audios/female/mjt3_4");

        audioPathFemale.Add(41, "GameLayer/Audios/female/mjt4_1");
        audioPathFemale.Add(42, "GameLayer/Audios/female/mjt4_2");
        audioPathFemale.Add(43, "GameLayer/Audios/female/mjt4_3");
    }
}
