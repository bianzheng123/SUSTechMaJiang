using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : BasePlayer {

    public override void DaPai()
    {
        Debug.Log("人机打牌");
        System.Threading.Thread.Sleep(500);
        ChuPai(0);
    }
}
