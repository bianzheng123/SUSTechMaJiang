using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CtrlPlayer : BasePlayer {
    public int selectedPaiIndex = -1;

    /// <summary>
    /// 这里就是点击牌，进行选择，当按下确定按钮时在进行打牌
    /// </summary>
    public override void DaPai()
    {
        if (Input.GetMouseButtonDown(0))
        { //检测鼠标左键是否点击
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.tag == "Pai" + id)
                {
                    int nowSelectIndex = -1;
                    for (int i = 0; i < handPai.Count; i++)
                    {
                        if (hit.collider.gameObject.transform.position == handPai[i].transform.position)
                        {
                            nowSelectIndex = i;
                        }
                    }
                    if (nowSelectIndex == -1)
                    {
                        Debug.Log("选牌错误,发生了bug");
                    }
                    if (selectedPaiIndex != -1)
                    {//让取消选牌的降下来
                        handPai[selectedPaiIndex].transform.Translate(new Vector3(0, -0.5f, 0));
                    }
                    if (nowSelectIndex == selectedPaiIndex)
                    {//两次点击选中了相同的牌，相当于没有选中
                        selectedPaiIndex = -1;
                    }
                    else
                    {
                        //将新选中的牌上升
                        selectedPaiIndex = nowSelectIndex;
                        handPai[selectedPaiIndex].transform.Translate(new Vector3(0, 0.5f, 0));
                    }
                }

            }
        }
    }

    public void ChuPai()
    {
        int index = 0;
        if(selectedPaiIndex != -1)
        {//先将选中的牌降下来再打
            handPai[selectedPaiIndex].transform.Translate(new Vector3(0, -0.5f, 0));
            index = selectedPaiIndex;
        }
        selectedPaiIndex = -1;
        gamePanel.SkillButton = false;
        ChuPai_Hu(index);
        //对选择的牌进行初始化
        
    }

    public void Hu()
    {
        ChuPai_Hu();
    }

    /// <summary>
    /// 代表发动化学系的技能，发送MsgChemistry协议
    /// </summary>
    public void LaunchChemistry()
    {
        MsgChemistry msg = new MsgChemistry();
        msg.id = id;
        msg.paiIndex = selectedPaiIndex;
        selectedPaiIndex = -1;
        NetManager.Send(msg);
    }

    /// <summary>
    /// 发动数学系的技能，发送MsgMath协议
    /// </summary>
    public void LaunchMath(int selectedPlayerIndex)
    {
        MsgMath msg = new MsgMath();
        msg.observerPlayerId = id;
        msg.observedPlayerId = selectedPlayerIndex;
        msg.paiId = null;
        Debug.Log("发动技能的玩家id为 " + id);
        Debug.Log("被观察的id为 " + selectedPlayerIndex);
        NetManager.Send(msg);
    }

    public void LaunchComputerScience()
    {
        MsgComputerScience msg = new MsgComputerScience();
        msg.id = id;
        msg.paiIndex = selectedPaiIndex;
        selectedPaiIndex = -1;
        NetManager.Send(msg);
    }
}
