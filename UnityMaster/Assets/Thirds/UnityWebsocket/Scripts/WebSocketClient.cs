using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Timers;
using UnityEngine;
using WebSocketSharp;

namespace bell.ai.course2.unity.websocket
{


    public class WebSocketClient : MonoBehaviour
    {
        WebSocket mabotClient;    // 另起一个websocket来与Mabot通信
        MsgCallback mabotMsgBack;
        int mabotPortIndex;
        MsgCode mabotStatus = MsgCode.WS_CLOSE;

        WebSocket wClient;
        MD5CryptoServiceProvider md5;

        private MsgCallback msgBack;
        Timer timer;

        private int current_ports;
        private int index;

        public void Init(MsgCallback callback)
        {
            if (callback != null)
                msgBack = callback;

            md5 = new MD5CryptoServiceProvider();
            timer = new Timer();
            Debug.Log("ws:init");
            connectPerforms(0);
        }



        public void sendMsg(string data)
        {
            wClient.Send(data);
        }
        


        /// <summary>
        /// 遍历连接服务器
        /// </summary>
        private void connectPerforms(int index)
        {
            Debug.Log("ws:conncet "+ index);

            wClient = new WebSocket(Constants.ws + Constants.ports[index]);

            wClient.OnOpen += connectHandle;
            wClient.OnMessage += connectEchoHandle;
            wClient.OnError += connectErrorHandler;
            wClient.OnClose += connectCloseHandler;
            wClient.Connect();
   
        }


        private void connectHandle(object sender, EventArgs e)
        {
            WsRequest<string> wsRequest = new WsRequest<string>();
            wsRequest.messageId = "01";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/echo";
            wsRequest.mode = "request";
            //wsRequest.data = JsonUtility.ToJson(new StringMsgData("connect"));
            //wsRequest.data = JsonConvert.SerializeObject(new StringMsgData("connect"));
            wsRequest.data = "";
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            ((WebSocket)sender).Send(JsonUtility.ToJson(wsRequest));
            timer.Interval = 15000;
            timer.Elapsed += new ElapsedEventHandler(elapsedHandler);
            timer.Start();
        }

        private void connectEchoHandle(object sender, MessageEventArgs e)
        {
            Debug.Log("ws:echo");
            WsResponse<StringMsgData> data = JsonUtility.FromJson<WsResponse<StringMsgData>>(e.Data);
            if (data.messageId.Equals("01"))
                connectSuccessHandler();
            else
                wClient.Close();

        }

        private void connectErrorHandler(object sender, ErrorEventArgs e)
        {
            Debug.Log("ws:err");
            if (index < Constants.ports.Length - 1)
            {
                index++;
                connectPerforms(index);
            }
        }

        private void connectCloseHandler(object sender, CloseEventArgs e)
        {
            Debug.Log("ws:close");
            if (index < Constants.ports.Length - 1)
            {
                index++;
                connectPerforms(index);
            }
        }

        private void connectSuccessHandler()
        {
            timer.Stop();
            Debug.Log("ws:succss");
            msgRebindPerforms();
            msgBack.statusCallback(MsgCode.WS_CONNECT);
        }

        private void msgRebindPerforms()
        {

            wClient.OnOpen -= connectHandle;
            wClient.OnMessage -= connectEchoHandle;
            wClient.OnError -= connectErrorHandler;
            wClient.OnClose -= connectCloseHandler;
            wClient.OnMessage += msgHandle;
            wClient.OnError += errHandle;
            wClient.OnClose += closeCallback;
        }

        private void msgHandle(object sender, MessageEventArgs e)
        {
            //Debug.LogFormat(" WebSocket MsgHandle data: {0} ", e.Data);
            string data = e.Data;
            if (e.Data.Contains(Constants.request)||(e.Data.Contains(Constants.subscribe) && !e.Data.Contains(Constants.subscribeData) && !e.Data.Contains(Constants.onSubscribe)))
            {
                msgBack.reqCallback(data);
            } else if (e.Data.Contains(Constants.response) || e.Data.Contains(Constants.subscribeData) || e.Data.Contains(Constants.onSubscribe))
            {
                msgBack.resCallback(data);
            }
            
        }

        private void errHandle(object sender, ErrorEventArgs e)
        {
            Debug.LogErrorFormat(" WebSocket Error: {0} , ws isAlive: {1}", e.Message,wClient.IsAlive);
            msgBack.statusCallback(MsgCode.WS_ERROR);
        }

        private void closeCallback(object sender, CloseEventArgs e)
        {
            Debug.LogFormat(" WebSocket Close: {0} ", e.Reason);
            msgBack.statusCallback(MsgCode.WS_CLOSE);
        }

        private void elapsedHandler(object sender, ElapsedEventArgs e)
        {
            wClient.Close();
            if (index < Constants.ports.Length - 1)
            {
                index++;
                connectPerforms(index);
            }
               
        }

        private void OnDestroy()
        {
            if(wClient!=null)
                wClient.Close();
            if (mabotClient != null)
                mabotClient.Close();
        }


        ///////////////////////////////// MabotClient ///////////////////////////////////////////
        
        public void MabotClientInit(MsgCallback callBack, int portsIndex = 0)
        {
            if (callBack != null)
                mabotMsgBack = callBack;

            mabotClient = new WebSocket(Constants.ws + Constants.mabotPorts[portsIndex]);
            mabotClient.OnMessage += OnMabotMessage;
            mabotClient.OnError += OnMabotError;
            mabotClient.OnClose += OnMabotClose;

            mabotClient.Connect();
        }

        void OnMabotMessage(object sender, MessageEventArgs msg)
        {
            Debug.LogFormat(" Mabot WebSocket Msg data: {0} ", msg.Data);
            string data = msg.Data;
            if (msg.Data.Contains(Constants.request))
            {
                mabotMsgBack.reqCallback(data);
            }
            else if (msg.Data.Contains(Constants.response) || msg.Data.Contains(Constants.subscribeData) || msg.Data.Contains(Constants.onSubscribe))
            {
                mabotMsgBack.resCallback(data);
            }
        }

        void OnMabotClose(object sender, CloseEventArgs msg)
        {
            Debug.LogFormat(" Mabot WebSocket Close: {0}, Rsason: {1} ", msg.Code, msg.Reason);
            ReConnectMabotSocket();
        }

        void OnMabotError(object sender, ErrorEventArgs msg)
        {
            Debug.LogErrorFormat(" Mabot WebSocket Error: {0} , isAlive: {1}", msg.Message, mabotClient.IsAlive);
            if (!mabotClient.IsAlive)
                ReConnectMabotSocket();
        }

        void ReConnectMabotSocket()
        {
            Debug.LogFormat(" ConnectMabotSocket port: {0} connect Fail ", Constants.mabotPorts[mabotPortIndex]);
            mabotClient.Close();
            if (mabotPortIndex < Constants.mabotPorts.Length - 1)
            {
                mabotPortIndex++;
                MabotClientInit(null, mabotPortIndex);
            }
        }

        public void SendMabotMsg(string msg)
        {
            mabotClient.Send(msg);
        }

    }
}


