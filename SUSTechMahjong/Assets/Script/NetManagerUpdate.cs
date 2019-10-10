using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NetManagerUpdate : MonoBehaviour {

    private static NetManagerUpdate _instance = null;   //静态私有成员变量，存储唯一实例

    private NetManagerUpdate()    //私有构造函数，保证唯一性
    {
    }

    public static NetManagerUpdate GetInstance()    //公有静态方法，返回一个唯一的实例
    {
        if (_instance == null)
        {
            _instance = new NetManagerUpdate();
        }
        return _instance;
    }
    
    public bool isOnline = false;

    private void Start()
    {
        DontDestroyOnLoad(this);
        if (GameObject.Find("NetManager").gameObject != this.gameObject)
            Destroy(this.gameObject);
    }

    private void Update () {
        NetManager.Update();
    }
}
