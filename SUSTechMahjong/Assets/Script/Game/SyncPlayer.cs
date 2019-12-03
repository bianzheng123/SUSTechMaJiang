using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SyncPlayer : BasePlayer {

    public override void DaPai()
    {
        System.Threading.Thread.Sleep(2000);
        ChuPai_Hu(0);
    }

    IEnumerator SyncDaPai()
    {
        yield return new WaitForSeconds(2f);
        ChuPai_Hu(0);
    }

}
