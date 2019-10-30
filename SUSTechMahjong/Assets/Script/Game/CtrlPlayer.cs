using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlayer : BasePlayer {
    public int selectedIndex = -1;

    public void SelectPai()
    {
        if (Input.GetMouseButtonDown(0))
        { //检测鼠标左键是否点击
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if(hit.collider.gameObject.tag == "Pai" + id)
                {
                    int nowSelectIndex = -1;
                    for (int i = 0; i < handPai.Count; i++)
                    {
                        if(hit.collider.gameObject.transform.position == handPai[i].transform.position)
                        {
                            nowSelectIndex = i;
                        }
                    }
                    if(nowSelectIndex == -1)
                    {
                        Debug.Log("选牌错误,发生了bug");
                    }
                    if (selectedIndex != -1)
                    {//让取消选牌的降下来
                        handPai[selectedIndex].transform.Translate(new Vector3(0, -0.5f, 0));
                    }
                    if(nowSelectIndex == selectedIndex)
                    {//两次点击选中了相同的牌，相当于没有选中
                        selectedIndex = -1;
                    }
                    else
                    {
                        //将新选中的牌上升
                        selectedIndex = nowSelectIndex;
                        handPai[selectedIndex].transform.Translate(new Vector3(0, 0.5f, 0));
                    }
                }

            }
        }
    }

    public override void DaPai()
    {
        SelectPai();
    }

    public void DaPaiCompolsory()
    {
        int index = 0;
        if(selectedIndex != -1)
        {//先将选中的牌降下来再打
            handPai[selectedIndex].transform.Translate(new Vector3(0, -0.5f, 0));
            index = selectedIndex;
        }
        ChuPai(index);
        //对选择的牌进行初始化
        selectedIndex = -1;
    }

    
}
