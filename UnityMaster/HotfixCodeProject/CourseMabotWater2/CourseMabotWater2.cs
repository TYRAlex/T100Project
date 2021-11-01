using System;
using System.Collections;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using bell.ai.course2.unity.websocket;
using Newtonsoft.Json;

namespace ILFramework.HotClass
{
    public class CourseMabotWater2
    {
        GameObject curGo;

        GameObject flowers1;
        GameObject flowers2;

        GameObject waterTool;

        int flowerMoveTime;
        Vector3 f1StartPos;
        Vector3 f1EndPos;
        Vector3 f2StartPos;
        Vector3 f2EndPos;

        float distance;

        struct Flower
        {
            public GameObject flower;
            public bool isWater;
            public string witheredSp;
            public string flowerSp;
            public string overSp;
        }
        Flower[] fstruct1;
        Flower[] fstruct2;

        Flower[] curFlowers;
        int curFlowerID;
        int flowerNum;
        int lastAnglex, curAnglex;
        float waterAngle;
        float waterToolMovex; // 花洒移动距离
        float waterMoveTime;  // 花洒移动时间
        Vector3 waterToolPos;

        bool isFirst;
        bool waterIng;
        bool waterIdleing;
        bool lastSatus;

        float lastMotor, curMotor;
        bool isMove;
        bool waterBlock;
        bool moveBlock;

        SpineManager spineManager;
        SoundManager soundManager;
        NetworkManager networkManager;

        string waterIdle, waterDown, watering, waterUp;

        Tweener waterToolTweener;

        DeviceInfoReq.SingleDevice mabot;

        MonoBehaviour mono;
        Coroutine waterCo;
        int black;
        float waterTime;
        float moveTime;
        bool gameStart;

        /// Test ///
        GameObject moveBtn;
        GameObject waterBtn;

        void Start(object o)
        {
            spineManager = SpineManager.instance;
            soundManager = SoundManager.instance;
            networkManager = NetworkManager.instance;

            waterIdle = "1";
            waterDown = "2";
            watering = "3";
            waterUp = "4";

            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            flowers1 = curTrans.Find("bg/flowerGroup/flowers1").gameObject;
            flowers2 = curTrans.Find("bg/flowerGroup/flowers2").gameObject;

            waterTool = curTrans.Find("waterTool").gameObject;
            waterToolPos = waterTool.transform.localPosition;

            f1StartPos = flowers1.transform.localPosition;
            f2StartPos = flowers2.transform.localPosition;

            distance = 1981;
            f1EndPos = new Vector3(f1StartPos.x - distance, f1StartPos.y, f1StartPos.z);
            f2EndPos = new Vector3(f2StartPos.x - distance, f2StartPos.y, f2StartPos.z);

            flowerNum = 3;
            waterAngle = 10;
            waterMoveTime = 0.6f;
            waterToolMovex = 527;

            string childName;
            Transform child;
            fstruct1 = new Flower[flowerNum];
            for (int i = 0; i < fstruct1.Length; i++)
            {
                child = flowers1.transform.GetChild(i);
                fstruct1[i].flower = child.gameObject;
                fstruct1[i].isWater = false;
                childName = child.name.Split('_')[1];
                fstruct1[i].witheredSp = childName + 1;
                fstruct1[i].flowerSp = childName + 2;
                fstruct1[i].overSp = childName + 3;
            }

            fstruct2 = new Flower[flowerNum];
            for (int i = 0; i < fstruct2.Length; i++)
            {
                child = flowers2.transform.GetChild(i);
                fstruct2[i].flower = child.gameObject;
                fstruct2[i].isWater = false;
                childName = child.name.Split('_')[1];
                fstruct2[i].witheredSp = childName + 1;
                fstruct2[i].flowerSp = childName + 2;
                fstruct2[i].overSp = childName + 3;
            }

            curFlowers = new Flower[flowerNum];

            /// TEST ///
            moveBtn = curTrans.Find("moveBtn").gameObject;
            Util.AddBtnClick(moveBtn, MoveTest);
            waterBtn = curTrans.Find("waterBtn").gameObject;
            Util.AddBtnClick(waterBtn, WaterTest);
            ////////////

            mono = curGo.GetComponent<MonoBehaviour>();

            GameInit();
        }

        /// TEST ///

        void MoveTest(GameObject btn)
        {
            StartMoveFlowers();
        }

        void WaterTest(GameObject btn)
        {
            spineManager.DoAnimation(waterTool, waterDown, false, () =>
            {
                spineManager.DoAnimation(waterTool, watering);
                WaterFlowers();
            });
        }

        ///////////


        void GameInit()
        {
            soundManager.PlayClip(SoundManager.SoundType.BGM, 0, true);

            networkManager.gameRspEvent -= OnMabotRsp;
            networkManager.gameRspEvent += OnDeviceRsp;

            spineManager.DoAnimation(waterTool, waterIdle);

            flowerMoveTime = 1;
            curFlowerID = 0;
            lastAnglex = -10000;
            curAnglex = 0;
            lastMotor = -10000;
            curMotor = 0;

            waterTool.transform.localPosition = waterToolPos;
            flowers1.transform.localPosition = f1StartPos;
            flowers2.transform.localPosition = f2StartPos;

            isFirst = true;
            waterIng = false;
            waterIdleing = false;
            isMove = false;
            lastSatus = false;  // 是否在浇水
            waterBlock = false;
            moveBlock = false;

            waterToolTweener = null;
            curFlowers = fstruct1;

            waterCo = null;

            gameStart = false;

            spineManager.SetTimeScale(waterTool, 2);

            ConnectMabot();
        }

        void CreateFlowers(GameObject f1, GameObject f2)
        {
            waterTool.transform.DOLocalMove(waterToolPos, flowerMoveTime);
            f1.transform.DOLocalMove(f1EndPos, flowerMoveTime - 0.5f).OnComplete(
                () => 
                {
                    f1.transform.localPosition = f2StartPos;
                    //SetFlowerStatus();
                }
            );
            SetFlowerStatus();

            f2.transform.DOLocalMove(f2EndPos, flowerMoveTime - 0.5f);

        }

        void StartMoveFlowers()
        {
            GameObject f1 = isFirst ? flowers1 : flowers2;
            GameObject f2 = isFirst ? flowers2 : flowers1;
            CreateFlowers(f1, f2);
            isFirst = !isFirst;
            //SetFlowerStatus();
            Flower[] f = isFirst ? fstruct1 : fstruct2;
            curFlowers = f;
        }

        void WaterFlowers()
        {
            SetFlowerStatus(curFlowerID);
            spineManager.DoAnimation(curFlowers[curFlowerID].flower, curFlowers[curFlowerID].flowerSp, false, () => {
                spineManager.DoAnimation(curFlowers[curFlowerID].flower, curFlowers[curFlowerID].overSp, false);
                curFlowerID++;
                //if (curFlowerID == flowerNum)
                //{
                //    curFlowerID = 0;
                //    StartMoveFlowers();
                //}
            });
        }

        void SetFlowerStatus(int flowerID = -1)
        {
            if(flowerID == -1)
            {
                for (int i = 0; i < curFlowers.Length; i++)
                {
                    curFlowers[i].isWater = false;
                    spineManager.DoAnimation(curFlowers[i].flower, curFlowers[i].witheredSp);
                }
                return;
            }

            curFlowers[flowerID].isWater = true;
        }

        void WaterToolMove()
        {
            soundManager.PlayClip(SoundManager.SoundType.SOUND, 4);
            waterToolTweener = waterTool.transform.DOLocalMoveX(waterTool.transform.localPosition.x + waterToolMovex, waterMoveTime);
        }


        void PlayWaterToolSpine(float lastx, float curx) //bool isWater)
        {
            if (lastx < waterAngle && curx < waterAngle || lastx > waterAngle && curx > waterAngle)
            {
                lastAnglex = curAnglex;
                return;
            }

            bool isWater = lastx < waterAngle && curx >= waterAngle;
            lastAnglex = curAnglex;

            //if (lastx == curx)
            //    return;

            //bool isWater = curx > waterAngle;

            Debug.LogFormat("  PlayWaterToolSpine  isWater: {0} , lastStaus: {1}", isWater, lastSatus);
            //if (isWater == lastSatus)
            //    return;
            //lastSatus = isWater;
            
            //Debug.LogFormat(" PlayWaterToolSpine  isWater:   {0} ", isWater);
            if (isWater)
            {
                if (waterIng)
                    return;
                waterIdleing = false;
                Debug.Log(" Start Water ! ");
                //waterBlock = true;
                spineManager.DoAnimation(waterTool, waterDown, false, () =>
                {
                    waterIng = true;
                    spineManager.DoAnimation(waterTool, watering);
                    //waterBlock = false;
                    soundManager.PlayClip(SoundManager.SoundType.VOICE, 1);
                    if (isWater)
                        WaterFlowers();
                });
            }
            else
            {
                if (waterIdleing)
                    return;
                waterIng = false;
                Debug.Log(" Water over ! ");
                //GetMotorDataRequest();
                spineManager.DoAnimation(waterTool, waterUp, false, () =>
                {
                    soundManager.StopAudio(SoundManager.SoundType.VOICE);
                    waterIdleing = true;
                    spineManager.DoAnimation(waterTool, waterIdle);
                    if (curFlowerID == flowerNum)
                    {
                        curFlowerID = 0;
                        StartMoveFlowers();
                    }
                });
            }
        }





        /////// ************* WS ************* ///////

        void ConnectMabot()
        {
            WsRequest<string> request = new WsRequest<string>();
            request.deviceId = Constants.U3D_Sign + (int)Constants.DEVICE_ID.GetMabot;
            request.messageId = ((int)Constants.MESSAG_ID.GetMabot).ToString();
            request.path = Constants.GetBlueDvicesReq;
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
            request.deviceId = Constants.U3D_Sign + (int)Constants.DEVICE_ID.GetGyroscope;
            request.messageId = ((int)Constants.MESSAG_ID.GetGyroscope).ToString();
            request.path = Constants.GetGyroscopeReq;
            request.mode = Constants.subscribe;
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- OnDeviceRsp req : {0} ", JsonConvert.SerializeObject(request));

            //GetColorDataRequest();
            GetMotorDataRequest();
        }

        void OnMabotRsp(object sender, string rsp)
        {
            Debug.LogFormat(" ========== OnMabotRsp ===== {0} ", rsp);
            if (rsp.Contains(Constants.subscribeData) && rsp.Contains(Constants.GetGyroscopeReq))
            {
                if (waterBlock)
                    return;
                waterBlock = true;
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string posStr = data.data.data.msg;
                string[] xyz = posStr.Split(',');
                
                if (lastAnglex == -10000)
                    lastAnglex = int.Parse(xyz[0]);
                curAnglex = int.Parse(xyz[0]);
                Debug.LogFormat(" Watering.....  last: {0},  cur:  {1}  , block: {2} ", lastAnglex, curAnglex, waterBlock);
                //if(curAnglex <= waterAngle)
                //    waterBlock = false;
                //if (waterBlock)
                //{
                //    lastAnglex = curAnglex;
                //    return;
                //}
                //if(curAnglex > waterAngle)
                //{
                //    waterBlock = true;
                //    PlayWaterToolSpine(lastAnglex, curAnglex, true);
                //}
                PlayWaterToolSpine(lastAnglex, curAnglex);
                waterBlock = false;
            }
            else if (rsp.Contains(Constants.subscribeData) && rsp.Contains(Constants.GetMotorDataReq))
            {
                if (moveBlock)
                    return;
                moveBlock = true;
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string motorStr = data.data.data.msg;
                string[] motors = motorStr.Split(',');
                if (lastMotor == -10000)
                {
                    lastMotor = float.Parse(motors[0]);
                    curMotor = lastMotor;
                }

                Debug.LogFormat(" MotorData last: {0}, cur: {1} , isMove: {2}", lastMotor, curMotor, isMove);
                if (lastMotor!=curMotor && !isMove)
                {
                    isMove = true;
                    Debug.LogFormat(" MotorData waterToolTweener: {0} ***", waterToolTweener);
                    if (waterToolTweener == null)
                        WaterToolMove();
                    //StopSubScribe(Constants.GetMotorDataReq);
                }
                else if(lastMotor == curMotor)
                {
                    isMove = false;
                    waterToolTweener = null;
                    //if (waterCo != null)
                    //    mono.StopCoroutine(waterCo);
                    //waterCo = mono.StartCoroutine(SynWaterFlower());
                }
                lastMotor = curMotor;
                curMotor = float.Parse(motors[0]);
                moveBlock = false;
            }
            else if(rsp.Contains(Constants.subscribeData) && rsp.Contains(Constants.GetColorSensorReq))
            {
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string colorStr = data.data.data.msg;
                string[] colors = colorStr.Split(',');
                if (int.Parse(colors[0]) == black && !gameStart)
                {
                    gameStart = true;
                    if (waterCo != null)
                        mono.StopCoroutine(waterCo);
                    waterCo = mono.StartCoroutine(SynWaterFlower());
                }
            }
        }

        void StopSubScribe(string msgPath)
        {
            WsRequest<StopSubcribeReq> request = new WsRequest<StopSubcribeReq>();
            request.data = new StopSubcribeReq();
            request.deviceId = Constants.U3D_Sign + (int)Constants.DEVICE_ID.StopSubscribe;
            request.messageId = ((int)Constants.MESSAG_ID.StopSubscribe).ToString();
            request.path = msgPath;
            request.mode = Constants.unsubscribe;
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
                path = Constants.GetMotorDataReq,
                mode = Constants.subscribe,
            };
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- GetMotorDataRequest req : {0} ", JsonConvert.SerializeObject(request));
        }


        void OnDisable()
        {
            StopSubScribe(Constants.GetGyroscopeReq);
            StopSubScribe(Constants.GetMotorDataReq);
        }



        ///******************************  方案二  **********************************///
        #region 方案二
        IEnumerator SynWaterFlower()
        {
            yield return new WaitForSeconds(2);
            spineManager.DoAnimation(waterTool, waterDown, false, () =>
            {
                waterIng = true;
                spineManager.DoAnimation(waterTool, watering);
                soundManager.PlayClip(SoundManager.SoundType.VOICE, 1);
                WaterFlowers();
            });
            yield return new WaitForSeconds(2);
            spineManager.DoAnimation(waterTool, waterUp, false, () =>
            {
                soundManager.StopAudio(SoundManager.SoundType.VOICE);
                waterIdleing = true;
                spineManager.DoAnimation(waterTool, waterIdle);
            });
        }

        void GetColorDataRequest()
        {
            WsRequest<MabotInfoReq> request = new WsRequest<MabotInfoReq>()
            {
                data = new MabotInfoReq(),
                deviceId = Constants.U3D_Sign + ((int)Constants.DEVICE_ID.GetMotor).ToString(),
                messageId = ((int)Constants.MESSAG_ID.GetMotor).ToString(),
                path = Constants.GetColorSensorReq,
                mode = Constants.subscribe,
            };
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            networkManager.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- GetMotorDataRequest req : {0} ", JsonConvert.SerializeObject(request));
        }


        #endregion
        ///******************************    **********************************///
    }
}
