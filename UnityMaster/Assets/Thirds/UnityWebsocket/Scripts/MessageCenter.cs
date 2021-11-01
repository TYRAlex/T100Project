using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System;
using System.Linq;
using ILFramework;
using ILFramework.Scripts.Utility;
using ILRuntime.Runtime;

namespace bell.ai.course2.unity.websocket
{
    public class MessageCenter : MonoBehaviour
    {

        protected Communicate communicate;

        private Dictionary<string, MsgPackage> serviceMessagePool;
        private Dictionary<string, MsgPackage> clientMessagePool;




        // Start is called before the first frame update
        void Start()
        {
            serviceMessagePool = new Dictionary<string, MsgPackage>();
            clientMessagePool = new Dictionary<string, MsgPackage>();
            communicate = gameObject.GetComponent<Communicate>();
        }

        public void reqMessageHandler(string request)
        {
            WsRequest<object> data = JsonConvert.DeserializeObject<WsRequest<object>>(request);
            if (!data.signature.Equals(MD5Helper.CalcMD5(data.messageId + Constants.MD5SEED)))
            {
                Debug.Log("------------- req calc failed");
                return;
            }

            Debug.LogFormat(" reqMessageHandler request : {0} ", request);
            if (request.Contains(Constants.HeartRep))
                EchoHandler(request);
            else if (request.Contains(Constants.PlayU3DRep))
                PlayHandler(request);
            else if (request.Contains(Constants.StopU3DRep))
            {
                StopUnityEvent();
                StopHandler(request);
            }
            else if (request.Contains(Constants.PreLoadRep))
                PreLoadHandler(request);
            else if (request.Contains(Constants.ExitCourseRep))
                ExitCourseHandler(request);
            else if (request.Contains(Constants.subscribe))
            {
                EchHandle3Dof(request);

            }
        }


        public void resMessageHandler(string response)
        {
            Debug.LogFormat(" -=-=-=-=-=-=-=- ResMessageHandler -=-=-=-=-=-=-=-=-=-= {0} ", response);
            //WsRequest<object> data = JsonConvert.DeserializeObject<WsRequest<object>>(response);
            //if (!data.signature.Equals(MD5Helper.CalcMD5(data.messageId + Constants.MD5SEED)))
            //{
            //    Debug.Log("------------- res calc failed");
            //    return;
            //}

            MabotResHandler(response);
        }


        public virtual void EchoHandler(string request) { }

        public virtual void PlayHandler(string request) { }

        public virtual void StopHandler(string request) { }

        public virtual void PreLoadHandler(string request) { }

        public virtual void ExitCourseHandler(string request){ }

        public virtual void EchHandle3Dof(string request) {  }

        private void StopUnityEvent()
        {
            NetworkManager.instance.WSRequest();
            SoundManager.instance.ResetAudio();
        }

        public void MabotReqHandler(string request) { }

        public void MabotResHandler(string response)
        {
            Debug.LogFormat(" MabotResHandler response: {0} ", response);
            if (response.Contains(Constants.GetBlueDvicesReq))
            {
                WsResponse<DeviceInfoReq> data = JsonConvert.DeserializeObject<WsResponse<DeviceInfoReq>>(response);
                DevicesUtil.SetDevicesInfo(data.data.data.devices);
            }

            NetworkManager.instance.WSResponse(response);
        }

        public void TabletReqHandler(string request)
        {
            WsRequest<object> data = JsonConvert.DeserializeObject<WsRequest<object>>(request);
            if (!data.signature.Equals(MD5Helper.CalcMD5(data.messageId + Constants.MD5SEED)))
            {
                Debug.Log("-------------calc failed");
                return;
            }

            Debug.Log("PB Device req:" + request);
        }

        void JudgeAndRemoveUnuseDevice(ref List<Response_BleDevice> devices)
        {
            for (int i = 0; i < devices.Count; i++)
            {
                //Debug.Log("设备名称"+devices[i].name+"连接状态:"+devices[i].connected);
                if (!devices[i].name.Equals("CodingBoard") || devices[i].connected == false)
                {
                    //Debug.Log("发现未满足条件设备:"+devices[i].name+" 连接状态："+devices[i].connected+"予以删除");
                    devices.RemoveAt(i);
                    JudgeAndRemoveUnuseDevice(ref devices);
                }
            }
        }

        public void TabletResHandler(string response)
        {
            Debug.Log("MessageCenter_resHandler:" + response);
            WsResponse<Response_Data_BleDevices> wsRes = JsonConvert.DeserializeObject<WsResponse<Response_Data_BleDevices>>(response);
            if (wsRes != null)
            {
                //Debug.Log("messageId: " + wsRes.messageId);
            }
            switch (wsRes.path)
            {
                case "/common/bleDevices":              //GetBleList
                    Response_Data_BleDevices devices = wsRes.data.data;
                    if (devices != null)
                    {
                       
                        //TabletWebsocketController.Instance.CurrDevices = devices.devices;
                        //C7G2Controller.Instance.gotNewList = true;                                  //第一次破例，在WS组件中调用界面组件
                        //List<Response_BleDevice> list = TabletWebsocketController.Instance.CurrDevices.ToList();
                        List<Response_BleDevice> list = devices.devices.ToList();
                        JudgeAndRemoveUnuseDevice(ref list);
                        TabletWebsocketController.Instance.CurrDevices = list.ToArray();
                        // for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
                        // {
                        //     Debug.Log("设备名称："+TabletWebsocketController.Instance.CurrDevices[i].name+" 设备是否连接"+TabletWebsocketController.Instance.CurrDevices[i].connected);
                        //     if (!TabletWebsocketController.Instance.CurrDevices[i].name.Equals("CodingBoard")||TabletWebsocketController.Instance.CurrDevices[i].connected==false)
                        //     {
                        //         Debug.Log("发现不符合的设备："+TabletWebsocketController.Instance.CurrDevices[i].name);
                        //         List<Response_BleDevice> list2 = TabletWebsocketController.Instance.CurrDevices.ToList();
                        //         list2.RemoveAt(i);
                        //         TabletWebsocketController.Instance.CurrDevices = list2.ToArray();
                        //     }
                        //         
                        // }
                        //
                        // for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
                        // {
                        //     Debug.Log("删除1之后剩下的设备 devices.devices[" + i + "].name: " + TabletWebsocketController.Instance.CurrDevices[i].name);
                        //     if (!TabletWebsocketController.Instance.CurrDevices[i].name.Equals("CodingBoard")||TabletWebsocketController.Instance.CurrDevices[i].connected==false)
                        //     {
                        //         Debug.Log("发现1不符合的设备："+TabletWebsocketController.Instance.CurrDevices[i].name);
                        //         List<Response_BleDevice> list2 = TabletWebsocketController.Instance.CurrDevices.ToList();
                        //         list2.RemoveAt(i);
                        //         TabletWebsocketController.Instance.CurrDevices = list2.ToArray();
                        //     }
                        // }

                        for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
                        {
                            Debug.Log("删除2之后剩下的设备 devices.devices[" + i + "].name: " +
                                      TabletWebsocketController.Instance.CurrDevices[i].name);
                        }
                    }
                    break;
                case "/common/bleDeviceControl":              //Subscribe
                    WsResponse<Response_SubscribeBleMsg> bleMsg = JsonConvert.DeserializeObject<WsResponse<Response_SubscribeBleMsg>>(response);
                    try
                    {
                        Response_SubscribeBleMsg msg = bleMsg.data.data;
                        if (msg != null)
                            Debug.Log("data.msg: " + msg.msg);
                    }
                    catch (Exception E)
                    {
                        Debug.LogError("myError:" + E.ToString());
                    }
                    break;
                case "03":              //Unsubscribe
                    break;
                case "/common/getOPBData":
                    WsResponse<Response_SubscribeBleMsg> wsResponse =
                        JsonConvert.DeserializeObject<WsResponse<Response_SubscribeBleMsg>>(response);
                    try
                    {
                        Response_SubscribeBleMsg msg = wsResponse.data.data;
                        
                        if (msg != null)
                        {
                            Debug.Log("receive:"+msg.msg);
                            if (msg.msgType == "MSG_CODE")
                            {
                                Debug.Log("device name: " + msg.name);
                                string[] steps = msg.msg.Split('\n');
                                Debug.Log("steps: " + steps.Length + "," + msg.msg);
                                if (TabletWebsocketController.Instance.Device2Steps.ContainsKey(msg.name))
                                    TabletWebsocketController.Instance.Device2Steps[msg.name] = new List<string>(steps);
                                else
                                    TabletWebsocketController.Instance.Device2Steps.Add(msg.name,
                                        new List<string>(steps));
                                TabletWebsocketController.Instance.SendPBMsg(msg.name, "MSG_CODE");
                            }
                            else if (msg.msgType == "MSG_RUN")
                            {
                                string[] steps = msg.msg.Split('\n');
                                Debug.Log("steps: " + steps.Length + "," + msg.msg);
                                if (TabletWebsocketController.Instance.Device2Steps.ContainsKey(msg.name))
                                    TabletWebsocketController.Instance.Device2Steps[msg.name] = new List<string>(steps);
                                else
                                    TabletWebsocketController.Instance.Device2Steps.Add(msg.name,
                                        new List<string>(steps));
                                TabletWebsocketController.Instance.SendPBMsg(msg.name, "MSG_RUN");
                                Debug.Log("received msg:" + msg.msgType);
                            }
                            else if (msg.msgType == "MSG_OK")
                            {
                                TabletWebsocketController.Instance.SendPBMsg(msg.name, "MSG_OK");
                                Debug.Log("received msg:" + msg.msgType);
                            }
                        }
                    }
                    catch (Exception E)
                    {
                        Debug.LogError("myError:" + E.ToString());
                    }
                    
                    break;
            }
        }
    }
}