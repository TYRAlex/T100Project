using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using bell.ai.course2.unity.websocket;
using Newtonsoft.Json;

namespace ILFramework.HotClass
{
    public class CourseMabotWater
    {
        struct Flower
        {
            public GameObject flowerObj;
            public bool isWater;
            public string witheredSp;
            public string flowerSp;
            public string overSp;
        }

        GameObject curGo;
        GameObject bellGo;
        GameObject BgGo;
        Vector3 part1Pos;  // 背景移动位置
        Vector3 part2Pos;
        //Vector3 moveDistance; // 背景移动距离
        float bgMoveTime;    // 背景移动时间

        Vector3 waterToolPos; // 花洒初始位置
        float waterToolMovex; // 花洒移动距离
        float waterMoveTime;  // 花洒移动时间

        Flower[] flowers;
        int CurWaterFlowerID;
        int maxflowerNum;
        GameObject waterTool;
        int lastAnglex, curAnglex;

        float waterAngle;
        bool waterLock;
        int changeFlowerId;

        SpineManager spineManager;
        NetworkManager networkManager;
        SoundManager soundManager;

        string bellSpeak, bellBuild, bellIdle;
        string waterIdle, waterDown, watering, waterUp;
        bool waterIng;
        bool waterIdleing;
        // Test //
        GameObject waterBtn;
        bool testwater;

        DeviceInfoReq.SingleDevice mabot;
        //float[] initTurns;  // 驱动轮旋转圈数
        bool isMove;      // 自动装置环节花洒是否在移动
        bool isAuto;      // 当前环节是否为自动环节
        //int vailCount;  // 驱动轮数据只取前5个...
        float[] standerColor;  // 标准颜色数据 3个分别为 [ Red: 5, Green:3, Blue:2 ] 
        float lastColor, curColor;
        bool ban;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            spineManager = SpineManager.instance;
            networkManager = NetworkManager.instance;
            soundManager = SoundManager.instance;

            BgGo = curTrans.Find("bg").gameObject;
            part1Pos = BgGo.transform.localPosition;
            part2Pos = new Vector3(-part1Pos.x, part1Pos.y, part1Pos.z);

            Transform flowerGp = BgGo.transform.Find("flowerGroup");
            flowers = new Flower[flowerGp.childCount];
            string chidlName;
            GameObject child;
            for (int i = 0; i < flowerGp.childCount; i++)
            {
                child = flowerGp.GetChild(i).gameObject;
                flowers[i].flowerObj = child;
                flowers[i].isWater = false;
                chidlName = child.name.Split('_')[1];
                flowers[i].witheredSp = chidlName + 1;
                flowers[i].flowerSp = chidlName + 2;
                flowers[i].overSp = chidlName + 3;
            }
            
            waterTool = curTrans.Find("waterTool").gameObject;
            waterToolPos = waterTool.transform.localPosition;

            bellGo = curTrans.Find("bell").gameObject;
            bellSpeak = "DAIJIshuohua";
            bellIdle = "DAIJI";
            bellBuild = "zuo";
            waterIdle = "1";
            waterDown = "2";
            watering = "3";
            waterUp = "4";

            bgMoveTime = 1;
            waterMoveTime = 0.5f;
            waterToolMovex = 527;
            waterAngle = 3;//5;
            maxflowerNum = 6;
            changeFlowerId = 2;
            //vailCount = 5;
            standerColor = new float[] { 5, 3, 2 };
            ban = false;

            // Test ///
            waterBtn = curTrans.Find("waterBtn").gameObject;
            testwater = false;
            Util.AddBtnClick(waterBtn, TestWater);
            //////////
            ///
            GameInit();
        }

        void GameInit()
        {
            //networkManager.gameStopReqEvent -= StopAllMsg;
            //networkManager.gameStopReqEvent += StopAllMsg;

            networkManager.gameRspEvent -= OnMabotRsp;
            networkManager.gameRspEvent += OnDeviceRsp;

            //ConnectMabot();
            soundManager.PlayClip(SoundManager.SoundType.BGM, 0, true);

            CurWaterFlowerID = 0;
            lastAnglex = -10000;
            curAnglex = 0;
            lastColor = 5;  // 初始位置在红色位置
            curColor = 0;
            waterLock = false;
            waterIng = false;
            waterIdleing = false;
            waterTool.transform.localPosition = waterToolPos;
            isMove = false;
            isAuto = false;

            SetFlowerStatus();
            PlayFlowerSpine();
            bellGo.SetActive(false);
            waterTool.SetActive(false);
            soundManager.Speaking(bellGo, bellSpeak, SoundManager.SoundType.SOUND, 2, null, () =>
            {
                waterTool.SetActive(true);
                spineManager.DoAnimation(waterTool, waterIdle);
                ConnectMabot();
            });
        }

        void SetFlowerStatus(int flowerId = -1)
        {
            if (flowerId == -1)
            {
                for (int i = 0; i < flowers.Length; i++)
                {
                    flowers[i].isWater = false;
                }
            }
            else
                flowers[flowerId].isWater = true;
        }

        void PlayFlowerSpine(int flowerId = -1)
        {
            if(flowerId == -1)
            {
                for (int i = 0; i < flowers.Length; i++)
                {
                    spineManager.DoAnimation(flowers[i].flowerObj, flowers[i].witheredSp);
                }
                return;
            }
            Flower f = flowers[flowerId];
            if (!f.isWater)
            {
                if (flowerId == changeFlowerId)
                {
                    isAuto = true;
                    ban = true;
                }
                soundManager.PlayClip(SoundManager.SoundType.SOUND, 5);
                spineManager.DoAnimation(f.flowerObj, f.flowerSp, false, () =>
                {
                    spineManager.DoAnimation(f.flowerObj, f.overSp);
                    
                    //if(!isAuto)
                    //    MoveWaterTool();
                    //ChangeToPart2();
                });
            }
        }

        void PlayWaterToolSpine(float lastx, float curx)
        {
            if (lastx < waterAngle && curx < waterAngle || lastx > waterAngle && curx > waterAngle)
                return;

            bool isWater = lastx < waterAngle && curx >= waterAngle;
            lastAnglex = curAnglex;
            Debug.LogFormat(" PlayWaterToolSpine  isWater:   {0} ", isWater);
            if (isWater)
            {
                if (waterIng)
                    return;
                waterIdleing = false;
                spineManager.DoAnimation(waterTool, waterDown, false, () =>
                {
                    waterIng = true;
                    spineManager.DoAnimation(waterTool, watering);
                    soundManager.PlayClip(SoundManager.SoundType.VOICE, 1);
                    if (isWater)
                        WaterFlowers();
                });
            }
            else
            {
                if (waterIdleing)
                    return;
                //soundManager.StopAudio(SoundManager.SoundType.VOICE);
                waterIng = false;
                spineManager.DoAnimation(waterTool, waterUp, false, () =>
                {
                    //if (ban)
                    //    return;
                    soundManager.StopAudio(SoundManager.SoundType.VOICE);
                    waterIdleing = true;
                    spineManager.DoAnimation(waterTool, waterIdle);
                    ChangeToPart2();
                    if (!isAuto && !isMove && flowers[CurWaterFlowerID].isWater)
                    {
                        isMove = true;
                        MoveWaterTool();
                    }
                    else if (isAuto && CurWaterFlowerID == maxflowerNum - 1)  // 浇完花结束
                        MoveWaterTool();
                });
            }
        }

        void MoveWaterTool()
        {
            if (CurWaterFlowerID >= maxflowerNum - 1)
            {
                StopSubScribe("/common/getGyroscopeData");
                StopSubScribe("/common/getColorData");
                waterTool.SetActive(false);
                soundManager.Speaking(bellGo, bellSpeak, SoundManager.SoundType.SOUND, 0, null, ()=> { 
                    bellGo.SetActive(true);
                    spineManager.DoAnimation(bellGo, bellIdle);
                });
                return;
            }
            if (ban)
            {
                isAuto = true;
                isMove = false;
                return;
            }
            soundManager.PlayClip(SoundManager.SoundType.SOUND, 4);
            waterTool.transform.DOLocalMoveX(waterTool.transform.localPosition.x + waterToolMovex, waterMoveTime).OnComplete(
                () =>
                {
                    CurWaterFlowerID++;
                    isMove = false;
                    if (waterIng)
                        WaterFlowers();
                }
            );
        }

        void ChangeToPart2()
        {
            if (CurWaterFlowerID == changeFlowerId)
            {
                //waterTool.transform.localPosition = waterToolPos;
                isAuto = true;
                isMove = false;
                //waterTool.SetActive(false);
                //waterLock = true;
                //CurWaterFlowerID++;
                //BgGo.transform.DOLocalMove(part2Pos, bgMoveTime).OnComplete(
                //    () =>
                //    {
                //        soundManager.Speaking(bellGo, bellSpeak, SoundManager.SoundType.SOUND, 1, null, () =>
                //        {
                //            bellGo.SetActive(true);
                //            soundManager.PlayClip(SoundManager.SoundType.VOICE, 0);
                //            spineManager.DoAnimation(bellGo, bellBuild + "1", false, () =>
                //            {
                //                spineManager.DoAnimation(bellGo, bellBuild + "2", false, () =>
                //                {
                //                    spineManager.DoAnimation(bellGo, bellBuild + "3", false, () =>
                //                    {
                //                        soundManager.Speaking(bellGo, bellSpeak, SoundManager.SoundType.SOUND, 3, null, () =>
                //                        {
                //                            GetMotorDataRequest();
                //                            waterTool.SetActive(true);
                //                            waterLock = false;
                //                            lastAnglex = -10000;
                //                            curAnglex = 0;
                //                            spineManager.DoAnimation(waterTool, waterIdle);
                //                            ban = false;
                //                        });
                //                    });
                //                });
                //            });
                //        });

                //    });
            }
        }

        void WaterFlowers()
        {
            PlayFlowerSpine(CurWaterFlowerID);
            SetFlowerStatus(CurWaterFlowerID);
        }

        /// <summary>
        ///  Test
        /// </summary>
        void TestWater(GameObject gameObject)
        {
            testwater = !testwater;
            //PlayWaterToolSpine(0, 1);
        }

        //void StopAllMsg()
        //{
        //    StopSubScribe("/common/getGyroscopeData");
        //    StopSubScribe("/common/getColorData");
        //}

        void OnDisable()
        {
            Debug.LogFormat(" ========== OnDisable ==========  soundManager: {0}  ", SoundManager.instance);
            //SoundManager.instance.StopAudio();
            StopSubScribe("/common/getGyroscopeData");
            StopSubScribe("/common/getColorData");
        }

        /////// ************* WS ************* ///////

        void ConnectMabot()
        {
            WsRequest<string> request = new WsRequest<string>();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.GetMabot;
            request.messageId = ((int)Constants.MESSAG_ID.GetMabot).ToString();
            request.path = "/common/bleDevices";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            networkManager.cm.sendMessage(JsonUtility.ToJson(request));
            Debug.LogFormat(" Connect Mabot request: {0}", JsonUtility.ToJson(request));
        }

        void OnDeviceRsp(object sender, string rsp)
        {
            Debug.LogFormat(" ------ OnDeviceRsp resp: {0}", rsp);
            WsResponse<DeviceInfoReq> data = JsonConvert.DeserializeObject<WsResponse<DeviceInfoReq>>(rsp);
            mabot = GetConnectMabot(data.data.data.devices);
            networkManager.gameRspEvent -= OnDeviceRsp;
            networkManager.gameRspEvent += OnMabotRsp;

            WsRequest<MabotInfoReq> request = new WsRequest<MabotInfoReq>();
            request.data = new MabotInfoReq();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.GetGyroscope;
            request.messageId = ((int)Constants.MESSAG_ID.GetGyroscope).ToString();
            request.path = "/common/getGyroscopeData";
            request.mode = "subscribe";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- OnDeviceRsp req : {0} ", JsonConvert.SerializeObject(request));

            //GetMotorDataRequest();
        }

        void OnMabotRsp(object sender, string rsp)
        {
            Debug.LogFormat(" ========== OnMabotRsp ===== {0} ", rsp);
            if (rsp.Contains("onSubscribe") && rsp.Contains("/common/getGyroscopeData"))
            {
                WsResponse<string> msg = JsonConvert.DeserializeObject<WsResponse<string>>(rsp);
                if (msg.data.msg == "success")
                    Debug.Log(" OnSubscribe OnMabotRsp Success! ");
                else
                    Debug.LogError("OnSubscribe OnMabotRsp Fail!");
            }
            else if (rsp.Contains("subscribeData") && rsp.Contains("/common/getGyroscopeData"))
            {
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string posStr = data.data.data.msg;
                string[] xyz = posStr.Split(',');
                if (waterLock)
                    return;
                if (lastAnglex == -10000)
                    lastAnglex = int.Parse(xyz[0]);
                curAnglex = int.Parse(xyz[0]);
                Debug.LogFormat(" Watering.....  last: {0},  cur:  {1} ", lastAnglex, curAnglex);
                PlayWaterToolSpine(lastAnglex, curAnglex);
                //lastAnglex = curAnglex;
            }
            else if (rsp.Contains("onUnsubscribe") && rsp.Contains("/common/getGyroscopeData"))
            {
                WsResponse<string> msg = JsonConvert.DeserializeObject<WsResponse<string>>(rsp);
                if (msg.data.msg == "success")
                {
                    Debug.Log(" unsubscribe OnMabotRsp Success! ");
                    //networkManager.gameRspEvent -= OnMabotRsp;
                    //networkManager.gameRspEvent += OnDeviceRsp;
                }
                else
                    Debug.LogError("unsubscribe OnMabotRsp Fail!");
            }
            else if(rsp.Contains("subscribeData") && rsp.Contains("/common/getColorData"))
            {
                
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string motorMsg = data.data.data.msg;
                string[] motors = motorMsg.Split(',');
                curColor = float.Parse(motors[0]); // 第一个数据为颜色感应器 第二个为光照感应器
                Debug.LogFormat(" GetMotorData msg: {0}  ,  isMotorMove: {1}  ,  isMove: {2} ", motorMsg, IsMotorMove(lastColor, curColor), isMove);
                if (!isMove && IsMotorMove(lastColor, curColor))
                {
                    isMove = true;
                    MoveWaterTool();
                }
                lastColor = curColor;
            }
        }

        void StopSubScribe(string msgPath)
        {
            WsRequest<StopSubcribeReq> request = new WsRequest<StopSubcribeReq>();
            request.data = new StopSubcribeReq();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.StopSubscribe;
            request.messageId = ((int)Constants.MESSAG_ID.StopSubscribe).ToString();
            request.path = msgPath; //"/common/getGyroscopeData";
            request.mode = "unsubscribe";
            request.data.action = "stop";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- StopSubScribe  request : {0} ---------", JsonConvert.SerializeObject(request));
        }

        // 获取到连接的Mabot设备信息 因为没做选择所以只取第一个连接的Mabot
        DeviceInfoReq.SingleDevice GetConnectMabot(DeviceInfoReq.SingleDevice[] devices)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].connected == "true")
                {
                    return devices[i];
                }
            }
            return null;
        }

        void GetMotorDataRequest()
        {
            WsRequest<MabotInfoReq> request = new WsRequest<MabotInfoReq>()
            {
                data = new MabotInfoReq(),
                deviceId = Constants.U3D_Sign + ((int)Constants.DEVICE_ID.GetMotor).ToString(),
                messageId = ((int)Constants.MESSAG_ID.GetMotor).ToString(),
                path = "/common/getColorData",
                mode = "subscribe",
            };
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- GetMotorDataRequest req : {0} ", JsonConvert.SerializeObject(request));
        }

        bool IsMotorMove(float lastColor, float curColor)
        {
            if (lastColor == curColor)
                return false;
            if (CheckColor(lastColor))
                return true;
            if (!CheckColor(lastColor) && CheckColor(curColor))
                isMove = false;
            return false;
        }

        bool CheckColor(float color)
        {
            for (int i = 0; i < standerColor.Length; i++)
            {
                if (color == standerColor[i])
                    return true;
            }
            return false;
        }
    }
}
