using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoSomething
{
    //先ほど作成したクラス
    private SerialHandler serialHandler;
    

  public DoSomething()
    {
        serialHandler = new SerialHandler();

        //信号を受信したときに、そのメッセージの処理を行う
        serialHandler.OnDataReceived += OnDataReceived;
    }

    public void Write(string message)
    {
        //文字列を送信
        serialHandler.Write(message);
    }

    //受信した信号(message)に対する処理 データを受け取ったら自動で実行してくれる
    private void OnDataReceived(string message)
    {
        Debug.Log("受け取ったメッセージ：" + message);

        var data = message.Split(
                new string[] { "\t" }, System.StringSplitOptions.None);
        if (data.Length < 2) return;


        // クライアントからリザルトメッセージが送られてきたら、Arduinoに点灯のメッセージを送る
        if (message == "ClientResult")
        {
            Write("1");
        }


        try
        {

        }
        catch (System.Exception e)
        {
            Debug.LogWarning(e.Message);
        }
    }

    public void Update()
    {
        serialHandler.Update();

    }
}
