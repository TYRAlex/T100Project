using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System.Timers;
using System.Security.Cryptography;
using System;
using bell.ai.course2.unity.websocket;

namespace ILFramework
{


    public class ConnectAndroid : MonoBehaviour
    {

        public static ConnectAndroid Instance;
        
        /// <summary>
        /// 本地连接的websocket
        /// </summary>
        private WebSocket _client;

        private MsgCallback _msgCallback;

        private int portIndex;

        public MsgCode CurrentStatus = MsgCode.WS_CLOSE;

        private MD5CryptoServiceProvider md5;

        private Timer _timer;

        private int _index;

        private Communicate _communicateCenter;

        private void Awake()
        {
            Instance = this.GetComponent<ConnectAndroid>();
            _communicateCenter = this.GetComponent<Communicate>();
        }

        public void Init(MsgCallback callback)
        {
            if (callback != null)
            {
                _msgCallback = callback;
            }

            md5 = new MD5CryptoServiceProvider();
            _timer = new Timer();
            // Debug.Log("Aws,init");
            // ConnectPerforms(0);
        }

        public void SendMsg(string data)
        {
            if (_client == null)
            {
                //Debug.LogError("客户端为空，请重新调用");
                if (_index < Constants.ports.Length - 1)
                {
                    ConnectPerforms(_index);
                    _index++;
                }
            }
            else
            {
                //Debug.LogError("客户端成功发送");
                _client.Send(data);
            }
        }

        /// <summary>
        /// 遍历连接服务器
        /// </summary>
        /// <param name="index">相应的接口的数组下标</param>
        private void ConnectPerforms(int index)
        {
            Debug.Log("Aws:connect" + index);
            _client = new WebSocket(Constants.ws + Constants.ports[index]);
            _client.OnOpen += ConnectHandle;
            _client.OnMessage += ConnectEchoHandle;
            _client.OnError += ConnectErrorHandle;
            _client.OnClose += ConnectCloseHandle;
            _client.Connect();

        }

        private void ConnectHandle(object sender, EventArgs e)
        {
            Debug.Log("Aws:connected" + e.ToString());
            WsRequest<string> wsRequest = new WsRequest<string>();
            wsRequest.messageId = "17059";
            wsRequest.deviceId = "ANDROID_2d93ec99-a83d-4389-a5b1-bd0eb055936f";
            wsRequest.path = "/common/get3DOFData";
            wsRequest.mode = "subscribe";

            wsRequest.data = "";
            wsRequest.signature = "6d43f7783a762448b8ee27d98d6d5b12";
            ((WebSocket) sender).Send(JsonUtility.ToJson(wsRequest));
            _timer.Interval = 15000;
            _timer.Elapsed += new ElapsedEventHandler(ElapsedHandler);
            _timer.Start();
        }
        
        public void SendConnectData()
        {
            //Debug.Log("点击建立连接");
#if UNITY_ANDROID && !UNITY_EDITOR
            WsRequest<string> wsRequest = new WsRequest<string>();
            wsRequest.messageId = "17059";
            wsRequest.deviceId = "ANDROID_2d93ec99-a83d-4389-a5b1-bd0eb055936f";
            wsRequest.path = "/common/get3DOFData";
            wsRequest.mode = "subscribe";

            wsRequest.data = "";
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicateCenter.sendMessage(JsonUtility.ToJson(wsRequest));
            //SendMsg(JsonUtility.ToJson(wsRequest));
#endif





        }
        
        public void CloseSubscribe()
        {
            //Debug.LogError("点击关闭蓝牙体感连接，这个时候的连接状态为:"+CurrentStatus.ToString());
            if (_communicateCenter.GetCurrentConnectState() == MsgCode.WS_CONNECT)
            {
                //Debug.Log("点击关闭蓝牙体感连接");
                WsRequest<string> wsRequest = new WsRequest<string>();
                wsRequest.messageId = "17059";
                wsRequest.deviceId = "ANDROID_2d93ec99-a83d-4389-a5b1-bd0eb055936f";
                wsRequest.path = "/common/get3DOFData";
                wsRequest.mode = "unsubscribe";

                wsRequest.data = "";
                wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
                //CurrentStatus = MsgCode.WS_CLOSE;
                _communicateCenter.sendMessage(JsonUtility.ToJson(wsRequest));
               
                //SendMsg(JsonUtility.ToJson(wsRequest));
            }

           
        }

        private void ConnectCloseHandle(object sender, CloseEventArgs e)
        {
            Debug.Log("Aws:close" + e.Reason);
            if (_index < Constants.ports.Length - 1)
            {
                _index++;
                ConnectPerforms(_index);
            }
        }

        private void ConnectErrorHandle(object sender, ErrorEventArgs e)
        {
            Debug.Log("Aws:error");
            if (_index < Constants.ports.Length - 1)
            {
                _index++;
                ConnectPerforms(_index);
            }
        }

        private void ConnectEchoHandle(object sender, MessageEventArgs e)
        {
            Debug.Log("Aws:echo"+e.Data);
            WsResponse<StringMsgData> data = JsonUtility.FromJson<WsResponse<StringMsgData>>(e.Data);
            // if (data.mode == "subscribeData")
            // {
            //     ConnectSuccessHandle();
            // }
            if (data.messageId.Equals("17059"))
            {
                ConnectSuccessHandle();
            }
            else
            {
                _client.Close();
            }
        }

        private void ConnectSuccessHandle()
        {
            _timer.Stop();
            Debug.Log("Aws:success");
            MsgRebindPerforms();
            _msgCallback.statusCallback(MsgCode.WS_CONNECT);
            CurrentStatus = MsgCode.WS_CONNECT;
        }

        private void MsgRebindPerforms()
        {
            print("重新绑定方法");
            _client.OnOpen -= ConnectHandle;
            _client.OnMessage -= ConnectEchoHandle;
            _client.OnError -= ConnectErrorHandle;
            _client.OnClose -= ConnectCloseHandle;
            _client.OnMessage += MsgHandle;
            _client.OnError += ErrorHandle;
            _client.OnClose += CloseCallBack;
        }

        private void CloseCallBack(object sender, CloseEventArgs e)
        {
            Debug.LogFormat(" AWebSocket Close: {0} ", e.Reason);
            _msgCallback.statusCallback(MsgCode.WS_CLOSE);
            CurrentStatus = MsgCode.WS_CLOSE;
        }

        private void ErrorHandle(object sender, ErrorEventArgs e)
        {
            Debug.LogErrorFormat(" AWebSocket Error: {0} , ws isAlive: {1}", e.Message, _client.IsAlive);
            _msgCallback.statusCallback(MsgCode.WS_ERROR);
            CurrentStatus = MsgCode.WS_ERROR;
        }

        private void MsgHandle(object sender, MessageEventArgs e)
        {
            //Debug.LogFormat("AWebSocket MsgHandle data: {0}", e.Data);
            string data = e.Data;
            if (e.Data.Contains(Constants.subscribe) && !e.Data.Contains(Constants.subscribeData) && !e.Data.Contains(Constants.onSubscribe))
            {
                _msgCallback.reqCallback(data);
            }
            else if (e.Data.Contains(Constants.response) || e.Data.Contains(Constants.subscribeData) ||
                     e.Data.Contains(Constants.onSubscribe))
            {

                _msgCallback.resCallback(data);

            }
        }

        private void ElapsedHandler(object sender, ElapsedEventArgs e)
        {
            _client.Close();
            if (_index < Constants.ports.Length - 1)
            {
                _index++;
                ConnectPerforms(_index);
            }

        }

        private void OnDestroy()
        {
            if (_client != null)
            {
                _client.Close();
            }
        }
    }
}
