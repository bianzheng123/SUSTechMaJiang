using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : BasePlayer {

    public override void DaPai()
    {
        Debug.Log("人机打牌");
        ChuPai(0);
    }
}
