using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetManagerUpdate : MonoBehaviour {
    private void Start()
    {
        DontDestroyOnLoad(this);

        if (GameObject.Find("NetManager").gameObject != this.gameObject)
            Destroy(this.gameObject);
    }

    void Update () {
        NetManager.Update();
        

    }
}
