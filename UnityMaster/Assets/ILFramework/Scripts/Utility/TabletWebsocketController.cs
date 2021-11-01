using System;
using System.Collections;
using System.Collections.Generic;
using bell.ai.course2.unity.websocket;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;

namespace ILFramework.Scripts.Utility
{
    public class TabletWebsocketController : MonoBehaviour
    {
        public static TabletWebsocketController Instance;

        public Dictionary<string, List<string>> Device2Steps;

        public Response_BleDevice[] CurrDevices;
        [SerializeField] private Communicate _communicate;

        public delegate void MyAction(string device, string msg);

        public static MyAction ProgramBoardAction;
        
        
        public Dictionary<string, string> AnimalBind2Device = new Dictionary<string, string>();
        public Dictionary<string, bool> Animal2StartedRUN = new Dictionary<string, bool>();
        public Dictionary<string, List<Coroutine>> Animal2Coroutines = new Dictionary<string, List<Coroutine>>();

        public delegate void DinosaurMoveDelegate(string DinosaurName, List<string> steps);

        public static DinosaurMoveDelegate DinosaurMove;

        private void Awake()
        {
            Instance = this;
            Device2Steps=new Dictionary<string, List<string>>();
        }


        public void SendPBMsg(string pbAdress,string msg)
        {
            WebSocketController_ProgramBoardAction(pbAdress, msg);
            ProgramBoardAction?.Invoke(pbAdress, msg);
            //ProgramBoardAction(pbAdress, msg);
        }

        private void OnEnable()
        {
            if (_communicate == null)
            {
                _communicate = GameObject.Find("U3DPlayerConnunication").GetComponent<Communicate>();
            }
        }

        public void BindDinosaurNameToDeviceName(string dinosaurName, string deviceAdress)
        {
            if (deviceAdress != null)
            {
                if (AnimalBind2Device.ContainsKey(dinosaurName))
                {
                    AnimalBind2Device[dinosaurName] = deviceAdress;
                }
                else
                {
                    AnimalBind2Device.Add(dinosaurName, deviceAdress);
                }
            }
        }
        
        

        private void WebSocketController_ProgramBoardAction(string deviceAdress, string msg)
        {
            Debug.Log(deviceAdress+"设备当前接收到的信息为："+msg);
            switch (msg)
            {
                case "MSG_RUN":

                    foreach (var item in AnimalBind2Device)
                    {
                        if (item.Value == deviceAdress)
                        {
                            StartCoroutine(PlaySteps(item.Key));
                            // if (Animal2StartedRUN.ContainsKey(item.Key) && Animal2StartedRUN[item.Key] == false)
                            // {
                            //     Animal2StartedRUN[item.Key] = true;
                            //     Coroutine co = StartCoroutine(PlaySteps(item.Key));
                            //     Animal2Coroutines[item.Key].Clear(); //当前整个List中，仅有PlaySteps的Coroutine，所以暂时可以Clear再Add，避免重复添加（按说应该不会，有bool值在前判断）
                            //     Animal2Coroutines[item.Key].Add(co);
                            //     Debug.Log("Add once");
                            // }
                            // else
                            // {
                            //     if (!Animal2StartedRUN.ContainsKey(item.Key))
                            //     {
                            //         //Debug.Log("start run " + item.Key);
                            //         Animal2StartedRUN.Add(item.Key, true);
                            //         Coroutine co = StartCoroutine(PlaySteps(item.Key));
                            //         Animal2Coroutines[item.Key].Clear();
                            //         Animal2Coroutines[item.Key].Add(co);
                            //         Debug.Log("Add once");
                            //     }
                            // }

                            break;
                        }
                    }
                    

                    break;
                case "MSG_CODE":

                    break;
                case "MSG_OK":
                    Transform toggleItem = null;
                    for (int j = 0; j < CurrDevices.Length; j++)
                    {
                        if (CurrDevices[j].adress == deviceAdress /*&& wsController.currDevices[j].connected*/)
                        {
                            //toggleItem = toggleGroup.Find(device);
                            break;
                        }
                    }

                    if (toggleItem != null)
                    {
                        //StartCoroutine(DoToggleItemAnimation(toggleItem));
                    }

                    break;
                default:
                    break;
            }
        }

        private IEnumerator PlaySteps(string targetAnimal)
        {
            yield return null;
            //Debug.Log("开始行走");
            if (!Device2Steps.ContainsKey(AnimalBind2Device[targetAnimal]))
            {
                Animal2StartedRUN[targetAnimal] = false;
                yield break;
            }

            List<string> steps = Device2Steps[AnimalBind2Device[targetAnimal]];
            //Debug.Log("发送行走指令");
            DinosaurMove?.Invoke(targetAnimal, steps);
            Animal2StartedRUN[targetAnimal] = false;
            SendMessageToDevice(AnimalBind2Device[targetAnimal], "255");
        }

        /// <summary>
        /// 重置设备
        /// </summary>
        public void ResetAllStatu()
        {
            UnsubScribeAllDeviceMsg();
            SendResetAllPBDevice();
            ClearAllDeviceInfo();
        }

        /// <summary>
        /// 清除设备信息
        /// </summary>
        public void ClearAllDeviceInfo()
        {
            CurrDevices = null;
            AnimalBind2Device.Clear();
        }


        #region 发送消息请求
        /// <summary>
        /// 获得了蓝牙设备请求
        /// </summary>
        public void GetBleDevices()
        {
            WsRequest<string> wsRequest = new WsRequest<string>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/bleDevices"; //"/common/echo";
            wsRequest.mode = "request";
            wsRequest.data = ""; //$"{{type: 'Mabot', name: '4Mabot',control: 'connect' }}"
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            if (_communicate != null)
                _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        /// <summary>
        /// 成功订阅到相应设备发送消息
        /// </summary>
        public void SubscribeToGetDeviceMsg()
        {
            WsRequest<Request_SubscribeBleMsg> wsRequest = new WsRequest<Request_SubscribeBleMsg>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/getOPBData"; //"/common/echo";
            wsRequest.mode = "subscribe";
            Request_SubscribeBleMsg subData = new Request_SubscribeBleMsg();
            subData.action = "start";
            wsRequest.data = subData;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        public void UnsubscribeDeviceMsg(string deviceName)
        {
            WsRequest<Request_SubscribeBleMsg> wsRequest = new WsRequest<Request_SubscribeBleMsg>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/getOPBData"; //"/common/echo";
            wsRequest.mode = "unsubscribe";
            Request_SubscribeBleMsg subData = new Request_SubscribeBleMsg();
            subData.action = "stop";
            wsRequest.data = subData;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        public void UnsubScribeAllDeviceMsg()
        {
            if(CurrDevices!=null&&CurrDevices.Length<=0)
                return;
            for (int i = 0; i < CurrDevices.Length; i++)
            {
                string deviceName = CurrDevices[i].name;
                WsRequest<Request_SubscribeBleMsg> wsRequest = new WsRequest<Request_SubscribeBleMsg>();
                wsRequest.messageId = "28281";
                wsRequest.deviceId = "U3D_";
                wsRequest.path = "/common/getOPBData"; //"/common/echo";
                wsRequest.mode = "unsubscribe";
                Request_SubscribeBleMsg subData = new Request_SubscribeBleMsg();
                subData.action = "stop";
                wsRequest.data = subData;
                wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
                _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
            }
        }


        public void ConnectToDevice(string deviceName)
        {
            WsRequest<Request_Data_Connect> wsRequest = new WsRequest<Request_Data_Connect>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/bleDeviceControl"; //"/common/echo";
            wsRequest.mode = "request";

            //string jsonData = $"{{'type':'Mabot','name':'{deviceName}','control':'connect'}}";
            Request_Data_Connect data = new Request_Data_Connect();
            data.type = "Mabot";
            data.name = deviceName;
            data.control = "connect";

            wsRequest.data = data;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        /// <summary>
        /// 发送消息给编程板（前进，转弯的指令）
        /// </summary>
        /// <param name="deviceName">设备名称</param>
        /// <param name="msg">相应的指令</param>
        public void SendMessageToDevice(string deviceName, string msg)
        {
            WsRequest<Request_Data_WriteBleData> wsRequest = new WsRequest<Request_Data_WriteBleData>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/writeBluData"; //"/common/echo";
            wsRequest.mode = "request";

            //string jsonData = $"{{'type':'Mabot','name':'{deviceName}','control':'connect'}}";
            Request_Data_WriteBleData data = new Request_Data_WriteBleData();
            data.type = "编程板";


            data.name = deviceName;
            data.msgType = "runStep";
            data.msg = msg;

            wsRequest.data = data;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }


        /// <summary>
        /// 发送OK给编程板
        /// </summary>
        /// <param name="deviceName"></param>
        public void SendOkToDevice(string deviceName)
        {
            WsRequest<Request_Data_WriteBleData> wsRequest = new WsRequest<Request_Data_WriteBleData>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/writeBluData";
            wsRequest.mode = "request";

            Request_Data_WriteBleData data = new Request_Data_WriteBleData();
            data.type = "编程板";
            data.name = deviceName;
            data.msgType = "ok";
            data.msg = "";

            wsRequest.data = data;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        /// <summary>
        /// 发送重置指令给编程板
        /// </summary>
        /// <param name="deviceName"></param>
        public void SendResetToDevice(string deviceName)
        {
            WsRequest<Request_Data_WriteBleData> wsRequest = new WsRequest<Request_Data_WriteBleData>();
            wsRequest.messageId = "28281";
            wsRequest.deviceId = "U3D_";
            wsRequest.path = "/common/writeBluData";
            wsRequest.mode = "request";

            Request_Data_WriteBleData data = new Request_Data_WriteBleData();
            data.type = "编程板";
            data.name = deviceName;
            data.msgType = "reset";
            data.msg = "";

            wsRequest.data = data;
            wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
            _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
        }

        public void SendResetAllPBDevice()
        {
            if(CurrDevices.Length<=0)
                return;
            for (int i = 0; i < CurrDevices.Length; i++)
            {
                string deviceName = CurrDevices[i].name;
                WsRequest<Request_Data_WriteBleData> wsRequest = new WsRequest<Request_Data_WriteBleData>();
                wsRequest.messageId = "28281";
                wsRequest.deviceId = "U3D_";
                wsRequest.path = "/common/writeBluData";
                wsRequest.mode = "request";

                Request_Data_WriteBleData data = new Request_Data_WriteBleData();
                data.type = "编程板";
                data.name = deviceName;
                data.msgType = "reset";
                data.msg = "";

                wsRequest.data = data;
                wsRequest.signature = MD5Helper.CalcMD5(wsRequest.messageId + Constants.MD5SEED);
                _communicate.sendMessage(JsonUtility.ToJson(wsRequest));
            }   
        }


        #endregion 
        
        
        // private IEnumerator AnimalMoveOnce(string targetAnimal, string action)
        // {
        //     Dictionary<string, List<Vector2Int>> obsPos = new Dictionary<string, List<Vector2Int>>();
        //     if (obstacleType == 0)
        //         obsPos = obstaclePosition;
        //     else
        //         obsPos = obstaclePosition1;
        //     if (action == "forward")
        //     {
        //         if (animalDirection[targetAnimal] == "forward")
        //         {
        //             Vector2Int bf = animalCoordinate[targetAnimal];
        //             if (!obsPos[targetAnimal].Contains(new Vector2Int(bf.x + 1, bf.y)))
        //             {
        //                 if (bf.x + 1 > 9)
        //                     yield break;
        //                 ShowFootPrint(targetAnimal,
        //                     animalCoordinate[targetAnimal]); //每次coordinate改变前，执行showFootPrint，显示上一步的脚印
        //                 ShowDirection(targetAnimal, animalDirection[targetAnimal], new Vector2Int(bf.x + 1, bf.y),
        //                     animalCoordinate[targetAnimal]);
        //                 animalCoordinate[targetAnimal] = new Vector2Int(bf.x + 1, bf.y);
        //
        //                 if (animalCoordinate[targetAnimal].x == 9)
        //                 {
        //                     //数据在动画执行前同步完毕,为了及时获取animal2FinishTime的值
        //                     //1s常规延时 + 2.5s动物动画延时（将数据同步执行提前，后续动画执行完再跳转）
        //                     AnimalFinishGame(targetAnimal, 3.5f);
        //                     if (animalCoordinate[targetAnimal].y == -1)
        //                     {
        //                         PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "pao2", true);
        //                         float posX = animal2AnimalObj[targetAnimal].localPosition.x + 130;
        //                         Tweener tweener = animal2AnimalObj[targetAnimal].DOLocalMoveX(posX, 1);
        //                         AddTweener(targetAnimal, tweener);
        //                         yield return new WaitForSeconds(1f);
        //                         if (targetAnimal != "zhu")
        //                         {
        //                             PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "gx", true);
        //                             yield return new WaitForSeconds(1.5f);
        //                         }
        //
        //                         PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "dj2", true);
        //                     }
        //                     else
        //                     {
        //                         PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "pao2", true);
        //                         float posX = animal2AnimalObj[targetAnimal].localPosition.x + 130;
        //                         float posY = animal2AnimalObj[targetAnimal].localPosition.y - 130;
        //                         Tweener tweener = animal2AnimalObj[targetAnimal]
        //                             .DOLocalMove(new Vector3(posX, posY), 1);
        //                         AddTweener(targetAnimal, tweener);
        //                         yield return new WaitForSeconds(1f);
        //                         if (targetAnimal != "zhu")
        //                         {
        //                             PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "gx", true);
        //                             yield return new WaitForSeconds(1.5f);
        //                         }
        //
        //                         PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "dj2", true);
        //                     }
        //
        //                     //AnimalFinishGame(targetAnimal, 1);
        //                 }
        //                 else
        //                 {
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "pao", true);
        //                     float posX = animal2AnimalObj[targetAnimal].localPosition.x + 130;
        //                     Tweener tweener = animal2AnimalObj[targetAnimal].DOLocalMoveX(posX, 1);
        //                     AddTweener(targetAnimal, tweener);
        //                     yield return new WaitForSeconds(1);
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "dj", true);
        //                 }
        //             }
        //             else
        //             {
        //                 string treeName = targetAnimal + "_" + (bf.x + 1) + "_" + bf.y;
        //                 Transform target = obstacleType == 0 ? trees.Find(treeName) : trees1.Find(treeName);
        //                 StartCoroutine(DoTreeAnimation(target));
        //                 StartCoroutine(DoStopAnimation(targetAnimal));
        //             }
        //         }
        //         else if (animalDirection[targetAnimal] == "up")
        //         {
        //             if (animalCoordinate[targetAnimal].y == 0)
        //                 StartCoroutine(DoStopAnimation(targetAnimal));
        //             else
        //             {
        //                 Vector2Int bf = animalCoordinate[targetAnimal];
        //                 if (!obsPos[targetAnimal].Contains(new Vector2Int(bf.x, bf.y + 1)))
        //                 {
        //                     ShowFootPrint(targetAnimal, animalCoordinate[targetAnimal]);
        //                     ShowDirection(targetAnimal, animalDirection[targetAnimal], new Vector2Int(bf.x, bf.y + 1),
        //                         animalCoordinate[targetAnimal]);
        //                     animalCoordinate[targetAnimal] = new Vector2Int(bf.x, bf.y + 1);
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "pao", true);
        //                     float posY = animal2AnimalObj[targetAnimal].localPosition.y + 130;
        //                     Tweener tweener = animal2AnimalObj[targetAnimal].DOLocalMoveY(posY, 1);
        //                     AddTweener(targetAnimal, tweener);
        //                     yield return new WaitForSeconds(1);
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "dj", true);
        //                 }
        //                 else
        //                 {
        //                     string treeName = targetAnimal + "_" + bf.x + "_" + (bf.y + 1);
        //                     Transform target = obstacleType == 0 ? trees.Find(treeName) : trees1.Find(treeName);
        //                     StartCoroutine(DoTreeAnimation(target));
        //                     StartCoroutine(DoStopAnimation(targetAnimal));
        //                 }
        //             }
        //         }
        //         else if (animalDirection[targetAnimal] == "down")
        //         {
        //             if (animalCoordinate[targetAnimal].y == 0)
        //             {
        //                 Vector2Int bf = animalCoordinate[targetAnimal];
        //                 if (!obsPos[targetAnimal].Contains(new Vector2Int(bf.x, bf.y - 1)))
        //                 {
        //                     ShowFootPrint(targetAnimal, animalCoordinate[targetAnimal]);
        //                     ShowDirection(targetAnimal, animalDirection[targetAnimal], new Vector2Int(bf.x, bf.y - 1),
        //                         animalCoordinate[targetAnimal]);
        //                     animalCoordinate[targetAnimal] = new Vector2Int(bf.x, bf.y - 1);
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "pao", true);
        //                     float posY = animal2AnimalObj[targetAnimal].localPosition.y - 130;
        //                     Tweener tweener = animal2AnimalObj[targetAnimal].DOLocalMoveY(posY, 1);
        //                     AddTweener(targetAnimal, tweener);
        //                     yield return new WaitForSeconds(1);
        //                     PlaySpineAnim(animal2AnimalObj[targetAnimal].gameObject, "dj", true);
        //                 }
        //                 else
        //                 {
        //                     string treeName = targetAnimal + "_" + bf.x + "_" + (bf.y - 1);
        //                     Transform target = obstacleType == 0 ? trees.Find(treeName) : trees1.Find(treeName);
        //                     StartCoroutine(DoTreeAnimation(target));
        //                     StartCoroutine(DoStopAnimation(targetAnimal));
        //                 }
        //             }
        //             else
        //                 StartCoroutine(DoStopAnimation(targetAnimal));
        //         }
        //     }
        //     else if (action == "turn left")
        //     {
        //         if (animalDirection[targetAnimal] == "forward")
        //         {
        //             animalDirection[targetAnimal] = "up";
        //         }
        //         else if (animalDirection[targetAnimal] == "down")
        //         {
        //             animalDirection[targetAnimal] = "forward";
        //         }
        //
        //         ShowDirection(targetAnimal, animalDirection[targetAnimal], animalCoordinate[targetAnimal]);
        //     }
        //     else if (action == "turn right")
        //     {
        //         if (animalDirection[targetAnimal] == "forward")
        //         {
        //             animalDirection[targetAnimal] = "down";
        //         }
        //         else if (animalDirection[targetAnimal] == "up")
        //         {
        //             animalDirection[targetAnimal] = "forward";
        //         }
        //
        //         ShowDirection(targetAnimal, animalDirection[targetAnimal], animalCoordinate[targetAnimal]);
        //     }
        //
        //     Debug.Log(targetAnimal + " currDirection:" + animalDirection[targetAnimal] + " currPosition:" +
        //               animalCoordinate[targetAnimal].ToString());
        // }
        
    }
    
    [Serializable]
    public class Request_Data_Connect
    {
        public string type;
        public string name;
        public string control;
    }

    [Serializable]
    public class Response_BleDevice
    {
        public bool connected;
        public string name;
        public string type;
        public string adress;
    }

    [Serializable]
    public class Response_Data_BleDevices
    {
        public Response_BleDevice[] devices;
    }

    [Serializable]
    public class Request_SubscribeBleMsg
    {
        public string action;
    }

    [Serializable]
    public class Response_SubscribeBleMsg
    {
        public string msg;
        public string msgType;
        public string name;
        public string type;
    }

    [Serializable]
    public class Response_BleMsg
    {
        public string type;
        public string name;
        public string msgType;
        public string msg;
    }

    [Serializable]
    public class Request_Data_WriteBleData {
        public string type;
        public string name;
        public string msgType;
        public string msg;
    }
}
