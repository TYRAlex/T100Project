using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Timers;
using bell.ai.course2.unity.websocket;
using UnityEngine;
using WebSocketSharp;

public class TabletConnectClient : MonoBehaviour
{
    private WebSocket _client;

    private MsgCallback _msgBack;

    MD5CryptoServiceProvider _md5;
    
    private Timer _timer;

    private int _current_ports;
    private int _index = 0;

    public void Init(MsgCallback callback)
    {
        if (callback != null)
            _msgBack = callback;
        _md5=new MD5CryptoServiceProvider();
        _timer=new Timer();
        _index = 0;
        ConnectPerforms(0);
    }

    public void SendMsg(string data)
    {
        _client.Send(data);
        Debug.Log("wClient.send"+data);
    }

    /// <summary>
    /// 遍历链接服务器
    /// </summary>
    /// <param name="index">端口下标</param>
    private void ConnectPerforms(int index)
    {
        Debug.Log("ws:conncet "+ index);
        _client=new WebSocket(Constants.ws+Constants.ports[index]);

        _client.OnOpen += ConnectHandle;
        _client.OnMessage += ConnectEchoHandle;
        _client.OnError += ConnectErrorHandle;
        _client.OnClose += ConnectCloseHandle;
        _client.Connect();
        
    }
    
    private void ConnectHandle(object sender, EventArgs e)
    {
        WsRequest<string> wsRequest=new WsRequest<string>();
        wsRequest.messageId = "28281";
        wsRequest.deviceId = "U3D_";
        wsRequest.path = "/common/bleDevices";
        wsRequest.mode = "request";
        wsRequest.data = "";
        wsRequest.signature=MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
        ((WebSocket)sender).Send(JsonUtility.ToJson(wsRequest));
        _timer.Interval = 15000;
        _timer.Elapsed += new ElapsedEventHandler(elapsedHandler);
        _timer.Start();
    }
    
    private void ConnectEchoHandle(object sender, MessageEventArgs e)
    {
        Debug.Log("echoHandle_MessageEventArgs.Data : " + e.Data);
        WsResponse<StringMsgData> data = JsonUtility.FromJson<WsResponse<StringMsgData>>(e.Data);
        if (data.messageId.Equals("28281"))
            ConnectSuccessHandler();
        else
        {
            _client.Close();
        }
    }

   

    private void ConnectErrorHandle(object sender, ErrorEventArgs e)
    {
        Debug.Log("tablet connect error");
        if (_index < Constants.ports.Length)
        {
            _index++;
            ConnectPerforms(_index);
        }
    }

    private void ConnectCloseHandle(object sender, CloseEventArgs e)
    {
        Debug.Log("tabletservice close");
        if (_index < Constants.ports.Length)
        {
            _index++;
            ConnectPerforms(_index);
        }
    }

    private void ConnectSuccessHandler()
    {
        _timer.Stop();
        Debug.Log("tablet connect success");
        MsgRebindPerforms();
        _msgBack.statusCallback(MsgCode.WS_CONNECT);
    }

    private void MsgRebindPerforms()
    {
        Debug.Log("重新绑定");
        _client.OnOpen -= ConnectHandle;
        _client.OnMessage -= ConnectEchoHandle;
        _client.OnError -= ConnectErrorHandle;
        _client.OnClose -= ConnectCloseHandle;
        _client.OnMessage += MsgHandle;
        _client.OnError += ErrHandle;
        _client.OnClose += CloseCallback;
    }

    private void CloseCallback(object sender, CloseEventArgs e)
    {
        _msgBack.statusCallback(MsgCode.WS_CLOSE);
        Debug.LogError("Tablet client close event reason:" + e.Reason);
    }

    private void ErrHandle(object sender, ErrorEventArgs e)
    {
        _msgBack.statusCallback(MsgCode.WS_ERROR);
        Debug.LogError("Tablet client error:" + e.Message);
    }

    private void MsgHandle(object sender, MessageEventArgs e)
    {
        string data = e.Data;
        Debug.Log("TabletConnect Handle: " + e.Data);
        if (e.Data.Contains("request") || e.Data.Contains("subscribe") || e.Data.Contains("unsubscribe"))
        {
            _msgBack.reqCallback(data);
        }
        if (e.Data.Contains("response") || e.Data.Contains("onSubscribe") || e.Data.Contains("subscribeData") || e.Data.Contains("onUnsubscribe"))
        {
            _msgBack.resCallback(data);
        }
    }


    private void elapsedHandler(object sender, ElapsedEventArgs e)
    {
        _client.Close();
        //if (index < Constants.ports.Length)
        //{
        //    index++;
        //    connectPerforms(index);
        //}
    }
    
    private void OnDestroy()
    {
        if(_client!=null)
            _client.Close();
    }

    
}
