using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using Newtonsoft.Json;
using UnityEngine;

namespace bell.ai.course2.unity.websocket
{
    public class Communicate : MonoBehaviour
    {

        public Action<MessageCenter> inited;
        private WebSocketClient client;
        private MsgCode currentCode;
        private MsgCode _current3DofCode;
        private MsgCode _currentPBDeviceCode;
        private ConnectAndroid _new3dofClient;
        private TabletConnectClient _tabletConnectClient;

        [SerializeField]
        private MessageCenter messageCenter;


        public void sendMessage(string data)
        {
            Debug.LogFormat("  sendMessage status: {0} ,  data : {1}", currentCode, data);
            if (currentCode != MsgCode.WS_CLOSE)
            {
                if (client != null)
                    client.sendMsg(data);
            }
            else
                Debug.LogError(" websocket closed ! ");
        }

        // public void PBSendMessage(string data)
        // {
        //     Debug.LogFormat("  PBDevice sendMessage status: {0} ,  data : {1}", currentCode, data);
        //     if (currentCode != MsgCode.WS_CLOSE)
        //     {
        //         if (_tabletConnectClient != null)
        //             _tabletConnectClient.SendMsg(data);
        //     }
        //     else
        //         Debug.LogError(" PBDevice Service closed ! ");
        // }



        private void Start()
        {
#if !UNITY_EDITOR && UNITY_ANDROID
            messageCenter = GetComponent<MessageCenter>();
            client = GetComponent<WebSocketClient>();
            //_new3dofClient= GetComponent<ConnectAndroid>();
            //_tabletConnectClient = this.GetComponent<TabletConnectClient>();
            StartCoroutine(DoConnectWs());
            
#endif
            
           
        }


        private IEnumerator DoConnectWs()
        {
            yield return new WaitForSeconds(0.1f);
            client.Init(new MsgCallback(this.reqCallback, this.resCallback, this.statusCallback));

            // yield return new WaitForSeconds(0.1f);
            // _new3dofClient.Init(new MsgCallback(this.New3DofReqCallback, this.New3DofResCallBack, this.StatusCallBackIn3Dof));
            //
            // yield return new WaitForSeconds(0.1f);
            // client.MabotClientInit(new MsgCallback(MabotReqCallBack, MabotResCallBack, null));
            //
            // yield return new WaitForSeconds(0.1f);
            // _tabletConnectClient.Init(new MsgCallback(TabletReqCallback, TabletResCallback, PBDevicestatusCallback));
        }

        public void TabletReqCallback(string request)
        {
            messageCenter.TabletReqHandler(request);
        }

        public void TabletResCallback(string response)
        {
            messageCenter.TabletResHandler(response);
        }

        private void New3DofReqCallback(string request)
        {
            //Debug.LogError("New3DofReqCallback"+request);
            
            //messageCenter.reqMessageHandler(request);
        }

        private void New3DofResCallBack(string response)
        {
            //Debug.LogError("New3DofResCallBack"+ response);
            //messageCenter.resMessageHandler(response);
            TransferData(response);
        }

        void TransferData(string respones)
        {

            if (respones.Contains(Constants.subscribeData))
            {
                if (respones.Contains("msg"))
                {
                    FunctionOf3Dof.Instance.IsOpened = false;
                    return;
                }

                FunctionOf3Dof.Instance.IsOpened = true;
                //Debug.Log("这是转换前的数据"+respones);
                WsResponse<int[]> data = JsonConvert.DeserializeObject<WsResponse<int[]>>(respones);
                BellLoom.QueueActions((o) => FunctionOf3Dof.Instance.TransInfoToButton(data.data.data), null);
            }

            
        }

        private void reqCallback(string request)
        {
            messageCenter.reqMessageHandler(request);
            //messageCenter.TabletReqHandler(request);
        }

        private void resCallback(string response)
        {
            //messageCenter.resMessageHandler(response);
            //Debug.Log("111111:" + response);
            if (response.Contains("/common/getOPBData")||response.Contains("/common/bleDevices"))
            {
                //Debug.Log("222222:"+response);
                messageCenter.TabletResHandler(response);
            }
            else if (response.Contains(Constants.subscribeData))
            {
                //Debug.Log("22222:" + response);
                TransferData(response);
            }
        }


        void MabotReqCallBack(string req)
        {
            messageCenter.MabotReqHandler(req);
        }

        void MabotResCallBack(string res)
        {
            messageCenter.MabotResHandler(res);
        }

        public void SendMabotMsg(string msg)
        {
            Debug.LogFormat(" Mabot sendMessage : {0} ", msg);
            client.SendMabotMsg(msg);
        }

        void StatusCallBackIn3Dof(MsgCode response)
        {
            Debug.LogError("status CallBack:" + response.ToString());
            if (response == MsgCode.WS_CONNECT)
            {
                AwsConnectHandle();
            }
            else
            {
                _current3DofCode = response;
            }
            
        }

        void AwsConnectHandle()
        {
            Debug.LogError("----------Connected+AwsConnectHandler");
            _current3DofCode = MsgCode.WS_CONNECT;
        
        }
        
        private void PBDevicestatusCallback(MsgCode response)
        {
            Debug.LogError("status CallBack:"+response.ToString());
            if (response == MsgCode.WS_CONNECT)
            {
                PBDeviceConnectHandler();
            } else
            {
                _currentPBDeviceCode = response;
            }
                
        }


        private void PBDeviceConnectHandler()
        {
            Debug.LogError("Connected+wsConnectHandler");
            _currentPBDeviceCode = MsgCode.WS_CONNECT;
        }

        private void statusCallback(MsgCode response)
        {
            Debug.LogError("status CallBack:"+response.ToString());
            if (response == MsgCode.WS_CONNECT)
            {
                wsConnectHandler();
            } else
            {
                currentCode = response;
            }
                
        }


        private void wsConnectHandler()
        {
            Debug.LogError("Connected+wsConnectHandler");
            currentCode = MsgCode.WS_CONNECT;
        }

        public MsgCode GetCurrentConnectState()
        {
            return currentCode;
        }


    }
}