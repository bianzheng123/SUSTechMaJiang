using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameoverPanel : BasePanel
{
    //显示玩家的文本
    private Text[] players;
    //每个玩家的结果
    private Text[] results;
    //显示成功或失败的UI，不同玩家显示不同的界面
    private Image[] title;
    //返回按钮
    private Button okButton;

    //初始化
    public override void OnInit()
    {
        skinPath = "GameoverPanel";
        layer = PanelManager.Layer.Panel;
    }

    //显示
    public override void OnShow(params object[] args)
    {
        //寻找组件
        players = new Text[4];
        players[0] = skin.transform.Find("Players/Player1").GetComponent<Text>();
        players[1] = skin.transform.Find("Players/Player2").GetComponent<Text>();
        players[2] = skin.transform.Find("Players/Player3").GetComponent<Text>();
        players[3] = skin.transform.Find("Players/Player4").GetComponent<Text>();
        results = new Text[4];
        results[0] = skin.transform.Find("PlayersResult/Player1").GetComponent<Text>();
        results[1] = skin.transform.Find("PlayersResult/Player2").GetComponent<Text>();
        results[2] = skin.transform.Find("PlayersResult/Player3").GetComponent<Text>();
        results[3] = skin.transform.Find("PlayersResult/Player4").GetComponent<Text>();
        title = new Image[3];
        title[0] = skin.transform.Find("Title/Result_draw").GetComponent<Image>();
        title[1] = skin.transform.Find("Title/Result_win").GetComponent<Image>();
        title[2] = skin.transform.Find("Title/Result_lose").GetComponent<Image>();
        
        okButton = skin.transform.Find("OkButton").GetComponent<Button>();
        //监听
        okButton.onClick.AddListener(OnOkClick);
        okButton.onClick.AddListener(Audio.ButtonClick);

        int winId = (int)args[0];//代表赢的玩家的id
        int thisId = (int)args[1];//代表该客户端的玩家id
        int quitId = (int)args[2];//-1代表没有人退出，否则就是退出的玩家id
        ShowResult(winId,thisId,quitId);
        EndGame();
    }

    public void EndGame()
    {
        GameManager gameManager = GameManager.GetInstance();
        BasePlayer[] players = gameManager.players;
        for(int i = 0; i < players.Length; i++)
        {
            GameObject[] pai = players[i].handPai.ToArray();
            for(int j = 0; j < pai.Length; j++)
            {
                Destroy(pai[i]);
                pai[i] = null;
            }
            Destroy(players[i].gameObject);
            players[i] = null;
        }
        Destroy(gameManager.gameObject);
    }

    public void OnOkClick()
    {
        Close();
        PanelManager.Open<RoomPanel>();
    }

    /// <summary>
    /// 展示结果，用来根据传递的参数进行画面的显示
    /// </summary>
    /// <param name="result">0代表意外发生或者平局，1代表该角色成功，2代表失败</param>
    private void ShowResult(int winId,int thisId,int quitId)
    {
        GameOver result;
        if (winId == -1)
        {
            result = GameOver.Peace;
        }
        else if (winId == thisId)
        {
            result = GameOver.Win;
        }
        else
        {
            result = GameOver.Lose;
        }

        players[thisId].text = "你";
        title[(int)result].gameObject.SetActive(true);
        switch (result)
        {
            case GameOver.Peace:
                Audio.PlayCue(Audio.pingJu);
                break;
            case GameOver.Win:
                Audio.PlayCue(Audio.win);
                break;
            case GameOver.Lose:
                Audio.PlayCue(Audio.lose);
                break;
        }
        for (int i = 0; i < 4; i++)
        {
            switch (result)
            {
                case GameOver.Peace:
                    if(i == quitId)
                    {
                        results[i].text = "-1000";
                    }
                    else
                    {
                        results[i].text = "+25";
                    }
                    break;
                case GameOver.Win:
                case GameOver.Lose:
                    if (i == winId)
                    {
                        results[i].text = "+60";
                    }
                    else
                    {
                        results[i].text = "+10";
                    }
                    break;
            }
        }
        
    }
}
public enum GameOver
{
    Peace = 0,
    Win = 1,
    Lose = 2
}



