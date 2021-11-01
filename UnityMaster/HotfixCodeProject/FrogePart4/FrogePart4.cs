using System;
using System.Collections.Generic;
using System.Collections;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using bell.ai.course2.unity.websocket;
using FrogePart4;

namespace ILFramework.HotClass
{
    public class FrogePart4
    {
        MonoBehaviour mono;
        GameObject curGo;
        List<Froge> froges;
        Dictionary<string, bool> devicesDic;
        bool gameStart;
        bool startBtnSpOver;
        bool gameOverSp;
        GameObject startBtn;

        bool lockQuery;
        float cutDown;

        LeafManager leafManager;
        FrogeManager frogManager;
        SoundManager soundManager;

        GameObject success;
        GameObject mask;
        int curConnectFroge;
        int maxConnectFroge;

        /// TEST ///
        GameObject takeOffBtn;
        InputField takeColorInput;
        ////////////

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            NetworkManager.instance.gameRspEvent += MabotResponse;

            froges = new List<Froge>();
            devicesDic = new Dictionary<string, bool>();
            devicesDic.Clear();

            Transform leafs = curTrans.Find("Leafs");
            GameObject leafItem = leafs.Find("leafItem").gameObject;
            Transform leavesPool = curTrans.Find("LeavesPool");

            leafManager = new LeafManager(leafItem){ leafParent = leafs, leavesGroup = leavesPool };
            frogManager = new FrogeManager(leafManager);
            soundManager = SoundManager.instance;

            Transform frogesTran = curTrans.Find("Froges");
            for (int i = 0; i < frogesTran.childCount; i++)
            {
                GameObject g = frogesTran.GetChild(i).gameObject;
                g.transform.localScale = new Vector3(0.68f, 0.68f, 0.68f);
                froges.Add(new Froge(g) { 
                    //calPos = frogManager.GetNextFrogPos,
                    //canTake = frogManager.VertifyColor,
                    vertifyPos = frogManager.GetTakeOffPos,
                    frogIndex = i,
                    getLandPos = frogManager.GetLandPos,
                    oriParent = frogesTran,
                    gameOver = GameOver,
                });
            }

            success = curTrans.Find("GameOver/sucess").gameObject;
            mask = curTrans.Find("GameOver/mask").gameObject;
            Transform points = curTrans.Find("EndPoints");
            for (int i = 0; i < points.childCount; i++)
            {
                Transform c = points.GetChild(i);
                frogManager.endPoints.Add(c.localPosition, false);
            }

            startBtn = curTrans.Find("StartBtn").gameObject;
            Util.AddBtnClick(startBtn, StartGame);
            cutDown = 60;
            maxConnectFroge = 4;

            /// Test ///
            takeOffBtn = curTrans.Find("TakeOffBtn").gameObject;
            Util.AddBtnClick(takeOffBtn, TakeOffTest);
            takeColorInput = curTrans.Find("TakeOffColor").GetComponent<InputField>();
            ///////////
            frogeBanDic = new Dictionary<string, bool>();

            GameInit();

            mono.StartCoroutine(SearchMabot());
        }

        IEnumerator SearchMabot()
        {
            while (!gameStart)
            {
                GetMabot();
                yield return new WaitForSeconds(1);
            }
            lockQuery = false;
        }

        void GetMabot()
        {
            if (lockQuery)
                return;
            lockQuery = true;
            DevicesUtil.QueryDevices();
            var devices = DevicesUtil.GetDevices();
            for (int i = 0; i < froges.Count; i++)
            {
                Debug.LogFormat(" 青蛙: {0} , Active: {1} ", i, froges[i].Active);
                if (!froges[i].Active)
                {
                    for (int j = 0; j < devices.Count; j++)
                    {
                        if (curConnectFroge == maxConnectFroge)
                            return;

                        froges[i].Active = true;
                        froges[i].Name = devices[j].name;
                        froges[i].mabotAddress = devices[j].address;
                        froges[i].mabotType = devices[j].type;

                        Debug.LogFormat(" 青蛙: {0} , devicesName: {1}, address: {3} , exist: {2}", i, devices[j].name, devicesDic.ContainsKey(devices[j].address), devices[j].address);
                        // 匹配当前连接设备是否已绑定青蛙
                        if (devicesDic.ContainsKey(devices[j].address))
                        {
                            Debug.LogFormat(" 青蛙: {0} , devicesName: {1} , connect: {2}", i, devices[j].name, devicesDic[devices[j].address]);
                            if (!devicesDic[devices[j].address])
                            {
                                devicesDic[devices[j].address] = true;
                                froges[i].DoConnectAni();
                                curConnectFroge++;
                                frogeBanDic.Add(devices[j].address, false);
                                //soundManager.PlayClip(SoundManager.SoundType.VOICE, 0);
                                break;
                            }
                            else
                                froges[i].Reset();
                        }
                        else
                        {
                            devicesDic.Add(devices[j].address, true);
                            froges[i].DoConnectAni();
                            curConnectFroge++;
                            frogeBanDic.Add(devices[j].address, false);
                            //soundManager.PlayClip(SoundManager.SoundType.VOICE, 0);
                            break;
                        }
                    }
                }
                else
                {
                    var device = DevicesUtil.GetDevices(froges[i].mabotAddress);
                    if (device == null || device.connected == "false")
                    {
                        Debug.LogFormat(" 青蛙: {0}, remove address: {1}, remove Name: {2} ", i, froges[i].mabotAddress, froges[i].Name);           
                        devicesDic.Remove(froges[i].mabotAddress);
                        curConnectFroge--;
                        froges[i].Reset();
                    }
                }
            }
            lockQuery = false;
        }

        void GameInit()
        {
            SpineManager.instance.DoAnimation(startBtn, ConstVariable.startBtnIdle, false);
            getColor = false;
            frogeBan = false;
            gameOverSp = true;
            startBtnSpOver = true;
            lockQuery = false;
            gameStart = false;
            mask.SetActive(false);
            startBtn.SetActive(true);
            success.SetActive(false);
            leafManager.leavesGroup.gameObject.SetActive(false);
            curConnectFroge = 0;
            ResetFroge();
        }

        void ResetFroge()
        {
            for (int i = 0; i < froges.Count; i++)
            {
                froges[i].Reset();
            }
        }

        void StartGame(GameObject btn)
        {
            if (curConnectFroge == 0)
                return;
            SpineManager.instance.DoAnimation(startBtn, ConstVariable.startBtnTips, false);
            soundManager.PlayClip(SoundManager.SoundType.BGM, 0, true);
            soundManager.PlayClip(SoundManager.SoundType.SOUND, 1, true);
            mono.StopAllCoroutines();
            btn.SetActive(false);
            if(gameStart)
            {
                DevicesUtil.StopSubcribe(Constants.GetColorSensorReq);
                return;
            }
            gameStart = true;
            //mono.StartCoroutine(StartDelay());
            lockQuery = false;
            List<string> addressList = new List<string>();
            for (int i = 0; i < froges.Count; i++)
            {
                if (froges[i].Active)
                {
                    Debug.LogFormat(" StartGame froge: {0}, address: {1} ", i, froges[i].mabotAddress);
                    addressList.Add(froges[i].mabotAddress);
                }

            }
            string[] address = addressList.ToArray();
            DevicesUtil.GetDeviceDataRequest(froges[0].mabotType, address, ((int)Constants.DEVICE_ID.GetColorSensor).ToString(), ((int)Constants.MESSAG_ID.GetColorSensor).ToString(), Constants.subscribe, Constants.GetColorSensorReq);

            leafManager.stop = false;
            //leafManager.GenerateLeaves(mono);
            leafManager.GenerateLeaves();
            //mono.StartCoroutine(GameCountDown());
        }

        void GameOver()
        {
            int num = 0;
            for (int i = 0; i < froges.Count; i++)
            {
                if (froges[i].success)
                    num++;
            }
            if (num == curConnectFroge)
            {
                gameOverSp = false;
                gameStart = false;
                DevicesUtil.StopSubcribe(Constants.GetColorSensorReq);
                mono.StopAllCoroutines();
                success.SetActive(true);
                mask.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                SpineManager.instance.DoAnimation(success, ConstVariable.sucessAni, false, () =>
                {
                    SpineManager.instance.DoAnimation(success, ConstVariable.sucessIdle);
                });
            }
        }

        IEnumerator GameCountDown()
        {
            while (cutDown > 0)
            {
                cutDown--;
                yield return new WaitForSeconds(1);
            }
            Debug.Log(" ! Game Over ! ");
            DevicesUtil.StopSubcribe(Constants.GetColorSensorReq);
        }

        Froge GetFrogeByAddress(string address)
        {
            for (int i = 0; i < froges.Count; i++)
            {
                //Debug.LogFormat(" GetFrogeByAddress i: {0} , address: {1}", i, froges[i].mabotAddress);
                if (froges[i].mabotAddress == address)
                    return froges[i];
            }
            Debug.LogErrorFormat(" GetFroge Fail, 地址为 {0} 的青蛙不存在 ", address);
            return null;
        }

        void MabotResponse(object sender, string response)
        {
            //Debug.LogFormat(" MabotResponse Msg : {0} ", response);
            if (response.Contains(Constants.subscribeData) && response.Contains(Constants.GetColorSensorReq))
            {
                if (getColor)
                    //return;
                getColor = true;
                WsResponse<MabotCommonData1> data = JsonConvert.DeserializeObject<WsResponse<MabotCommonData1>>(response);
                string[] address = data.data.data.addressArray;
                string color = data.data.data.msg;
                double d = Convert.ToDouble(color.Split(',')[0]);
                int c = Convert.ToInt32(d);
                var f = GetFrogeByAddress(address[0]);
                Debug.LogFormat(" MabotResponse color:  {0} , takeOff: {1} , frogeBan: {2}", c, f.takeOff, frogeBan);
                if (f.takeOff || c == 0 || frogeBanDic[address[0]] /*frogeBan*/)
                {
                    getColor = false;
                    return;
                }
                mono.StartCoroutine(StartDelay(address[0]));
                f.TakeOff(c);
                getColor = false;
            }
        }
        bool getColor;
        bool frogeBan;
        Dictionary<string, bool> frogeBanDic;
        IEnumerator StartDelay(string address)
        {
            //frogeBan = true;
            frogeBanDic[address] = true;
            yield return new WaitForSeconds(1);
            //frogeBan = false;
            frogeBanDic[address] = false;
        }

        void OnDisable()
        {
            mono.StopAllCoroutines();
            DevicesUtil.StopSubcribe(Constants.GetColorSensorReq);
        }


        /// TEST ///
        
        void TakeOffTest(GameObject btn)
        {
            int c = Convert.ToInt32(takeColorInput.text);
            froges[0].TakeOff(c);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                froges[0].TakeOff((int)ConstVariable.LColor.Red);
            }
            else if (Input.GetKeyDown(KeyCode.B))
            {
                froges[0].TakeOff((int)ConstVariable.LColor.Blue);
            }
            else if (Input.GetKeyDown(KeyCode.G))
            {
                froges[0].TakeOff((int)ConstVariable.LColor.Green);
            }
            else if (Input.GetKeyDown(KeyCode.Y))
            {
                froges[0].TakeOff((int)ConstVariable.LColor.Yellow);
            }


            //if (!gameStart && startBtn.activeSelf && startBtnSpOver)
            //{
            //    startBtnSpOver = false;
            //    string ani = curConnectFroge > 0 ? ConstVariable.startBtnTips : ConstVariable.startBtnIdle;
            //    SpineManager.instance.DoAnimation(startBtn, ani, false, ()=> { startBtnSpOver = true; });
            //    soundManager.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            //    return;
            //}

            //if (gameStart)
            //{
            //    froges[0].TakeOff((int)ConstVariable.LColor.Red);
            //}

            //if (gameStart && gameOverSp)
            //{
            //    int num = 0;
            //    for (int i = 0; i < froges.Count; i++)
            //    {
            //        if (froges[i].success)
            //            num++;
            //    }
            //    if (num == curConnectFroge)
            //    {
            //        gameOverSp = false;
            //        gameStart = false;
            //        DevicesUtil.StopSubcribe(Constants.GetColorSensorReq);
            //        success.SetActive(true);
            //        mask.SetActive(true);
            //        SpineManager.instance.DoAnimation(success, ConstVariable.sucessAni, false, () => { 
            //            SpineManager.instance.DoAnimation(success, ConstVariable.sucessIdle);
            //        });
            //    }
            //}
        }

        /// 
    }
}
