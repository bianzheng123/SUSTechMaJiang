using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : BasePlayer {

    public override void DaPai()
    {
        System.Threading.Thread.Sleep(500);
        ChuPai_Hu(0);
    }
}
