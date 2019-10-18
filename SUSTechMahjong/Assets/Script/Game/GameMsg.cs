using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgStartReceiveGameData : MsgBase
{
    public MsgStartReceiveGameData() { protoName = "MsgStartReceiveGameData"; }
    //服务端回
    public int id;
    public StartGameData[] data;
}

[System.Serializable]
public class StartGameData
{
    public int[] paiIndex = new int[4];
}