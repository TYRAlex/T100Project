using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Newtonsoft.Json;
using bell.ai.course2.unity.websocket;

namespace ILFramework.HotClass
{
    public class CourseMabotCat
    {
        GameObject curGo;
        GameObject overScene, startScene, gameScene;
        GameObject successView, failView;

        GameObject startBtn, startView, bell, restartBtn;
        Transform leftBasket, rightBasket, leftTrans, rightTrans, crossBar;
        Transform leftHand, rightHand;
        Vector3 leftPos, rightPos;

        Transform pointGo, leftFishPos, rightFishPos;
        GameObject leftTestBtn, RightTestBtn;
        GameObject cat, fish, fishesGp, leftDropWater, rightDropWater;
        GameObject[] fishes;
        // 倒计时
        Text minute, second;
        int seconds;

        enum CAT_SPINE
        {
            Idle, leftTilt, rightTilt, leftDrop, rightDrop, leftDropOver, rightDropOver, leftDanger, rightDanger, leftDropIdle, rightDropIdle
        }
        string[] catSpines;

        float proportion;  // 指针矫正 天平倾斜角与指针倾斜角比例
        //int offsetHand;  // 双手角度矫正
        int redPoint; // 指针红色区域阈值 x>39 || x<-39
        int greenPoint; // 指针绿色区域阈值  -7<x<7
        int preMabotY;
        int curMabotY;

        int dangerousTime;
        bool isDangerousStart; // 是否开启危险倒计时
        bool insFish;

        Tweener pointTweener, crossBarTweener; //leftHandTweener, rightHandTweener;
        string preAni;

        float fishAngle;
        Coroutine dangerCo;
        Coroutine successCo;
        float successTime;
        MonoBehaviour mono;

        Vector3 startPos, sucPos;
        Transform catall;
        int lastDanger;  // 上次危险区域 -1 - Left / 1 - right
        bool gameOver;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform.Find("Canvas");

            // 结束环节
            overScene = curTrans.Find("overScene").gameObject;
            successView = overScene.transform.Find("success").gameObject;
            failView = overScene.transform.Find("failure").gameObject;
            restartBtn = overScene.transform.Find("restartBtn").gameObject;
            Util.AddBtnClick(restartBtn, StartGame);

            // 开始环节
            startScene = curTrans.Find("startScene").gameObject;
            startView = startScene.transform.Find("start").gameObject;
            startBtn = startView.transform.Find("startBtn").gameObject;
            bell = startScene.transform.Find("bell").gameObject;
            Util.AddBtnClick(startBtn, StartGame);

            // 游戏环节
            gameScene = curTrans.Find("gameScene").gameObject;
            catall = gameScene.transform.Find("catOverAll");
            leftBasket = catall.Find("Baskets/LeftBasket");
            rightBasket = catall.Find("Baskets/RightBasket");
            crossBar = catall.Find("hengGan");
            leftTrans = crossBar.Find("LeftPos");
            rightTrans = crossBar.Find("RightPos");
            leftFishPos = crossBar.Find("LeftFishPos");
            rightFishPos = crossBar.Find("RightFishPos");
            leftPos = leftTrans.localPosition;
            rightPos = rightTrans.localPosition;
            leftHand = crossBar.Find("LeftHand");  // catall.Find("LeftHand");
            rightHand = crossBar.Find("RightHand"); // catall.Find("RightHand");

            leftDropWater = catall.Find("dropLeftWater").gameObject;
            rightDropWater = catall.Find("dropRightWater").gameObject;

            leftTestBtn = curTrans.Find("leftTestBtn").gameObject;
            RightTestBtn = curTrans.Find("rightTestBtn").gameObject;
            Util.AddBtnClick(leftTestBtn, Test);
            Util.AddBtnClick(RightTestBtn, Test);

            // 倒计时环节
            Transform cd = gameScene.transform.Find("cutDownGp");
            minute = cd.Find("minute").GetComponent<Text>();
            second = cd.Find("seconds").GetComponent<Text>();

            // 指针
            pointGo = gameScene.transform.Find("pointGp/points");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);

            cat = catall.Find("cat").gameObject;
            //fish = gameScene.transform.Find("fishItem").gameObject;
            fishesGp = gameScene.transform.Find("fishesGroup").gameObject;
            fishes = new GameObject[fishesGp.transform.childCount];
            for (int i = 0; i < fishesGp.transform.childCount; i++)
            {
                fishes[i] = fishesGp.transform.GetChild(i).gameObject;
            }

            catSpines = new string[] { "animation", "zuo", "you", "zuo2", "you2", "zuo3", "you3", "zuo4", "you4", "shuizuo", "shuiyou" };
            //NetworkManager.instance.gameRspEvent += OnDeviceRsp;
            mono = curGo.GetComponent<MonoBehaviour>();


            //offsetPoint = 15;//20;
            proportion = 1.5f;
            //offsetHand = 20;
            redPoint = 39;
            greenPoint = 7;
            fishAngle = 5;
            successTime = 2.5f;

            startPos = Vector3.zero;
            sucPos = new Vector3(0, -125, 0);
            ViewControl(true, true);
            dangerCo = null;
            //SetVoiceEvent();
            //TestSpeak();
        }

        void GameInit()
        {
            NetworkManager.instance.gameRspEvent -= OnMabotRsp;
            NetworkManager.instance.gameRspEvent += OnDeviceRsp;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            successCo = null;
            leftDropWater.SetActive(false);
            rightDropWater.SetActive(false);
            isDangerousStart = false;
            curval = 0;
            dangerousTime = 5;
            preMabotY = 500;
            curMabotY = 0;
            leftBasket.localPosition = leftPos;
            rightBasket.localPosition = rightPos;
            crossBar.localEulerAngles = Vector3.zero;
            pointGo.localEulerAngles = Vector3.zero;
            leftHand.localEulerAngles = Vector3.zero;
            rightHand.localEulerAngles = Vector3.zero;
            catall.localPosition = startPos;
            minute.text = "01";
            second.text = "30";
            seconds = 90;
            preAni = "";
            insFish = false;
            gameOver = false;
            lastDanger = 0;
            SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.Idle], true);
            CatBodyControl(true);
        }

        void StartGame(GameObject btn)
        {
            Debug.Log(" ---------- ! Game Start ! --------- [new]");
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            GameInit();
            ViewControl(true);
            mono.StartCoroutine(GameCutDown());
            ConnectMabot();

            //SoundManager.instance.ShowVoiceBtn(true);
        }

        void ViewControl(bool isGameStart, bool isFirstStart = false)
        {
            if (isFirstStart)
            {
                gameScene.SetActive(false);
                overScene.SetActive(false);
                startScene.SetActive(true);
                startView.SetActive(true);
                bell.SetActive(false);
            }
            else
            {
                bell.SetActive(false);
                startScene.SetActive(!isGameStart && isFirstStart);
                overScene.SetActive(!isGameStart);
                gameScene.SetActive(isGameStart);
            }
        }
        int curval = 0;
        void Test(GameObject go)
        {
            //int curval=0;
            bool isStart=false;
            bool isLeft = true;
            if (go.name == "leftTestBtn")
                curval += 10;
            else
            {
                isLeft = false;
                curval -= 10;
            }

            if (pointTweener != null)
                pointTweener.Kill();
            if (crossBarTweener != null)
                crossBarTweener.Kill();
            //if (leftHandTweener != null)
            //    leftHandTweener.Kill();
            //if (rightHandTweener != null)
            //    rightHandTweener.Kill();

            //bool isLeft = curval - preval > 0;
            
            Vector3 rz = new Vector3(0, 0, curval);
            crossBarTweener = ShortcutExtensions.DOLocalRotate(crossBar, rz, 1).OnUpdate(() =>
            {
                if (!insFish && !isStart)
                {
                    insFish = true;
                    if (isLeft)
                    {
                        CreateFish(true, true);
                        CreateFish(false, false);
                    }
                    else
                    {
                        CreateFish(false, true);
                        CreateFish(true, false);
                    }
                }
                PlayCatAnimation();
                leftBasket.position = leftTrans.position;
                rightBasket.position = rightTrans.position;
            });

            float z = rz.z * proportion;
            if (z > 52)
                z = 52;
            else if (z < -52)
                z = -52;
            Vector3 pz = new Vector3(0, 0, z);
            Debug.Log(" ----- z: " + z);
            pointTweener = ShortcutExtensions.DOLocalRotate(pointGo, new Vector3(0, 0, z), 1).OnComplete(() =>
            {
                if (!isStart)
                    CheckOutSuccess();
            });
            //leftHandTweener = ShortcutExtensions.DOLocalRotate(leftHand, rz + new Vector3(0, 0, isLeft ? offsetHand : -offsetHand), 1);
            //rightHandTweener = ShortcutExtensions.DOLocalRotate(rightHand, rz + new Vector3(0, 0, isLeft ? offsetHand : -offsetHand), 1);
        }

        /// <summary>
        /// 横杆角度调整
        /// </summary>
        /// <param name="mabotY"> Mabot数据值 </param>
        /// <param name="isLeft"> 是否左倾斜 </param>
        void CrossBarControl(int preval, int curval, bool isStart)
        {
            if ((curval == preval) || gameOver || dangerousTime == 0)
                return;

            //if (pointTweener != null)
            //    pointTweener.Kill();
            //if (crossBarTweener != null)
            //    crossBarTweener.Kill();
            //if (leftHandTweener != null)
            //    leftHandTweener.Kill();
            //if (rightHandTweener != null)
            //    rightHandTweener.Kill();

            bool isLeft = curval - preval > 0;
            Debug.LogFormat(" ----------- CrossBarControl ------------ {0} ", isLeft);

            Vector3 rz;
            if (Mathf.Abs(curval) > 15)
                rz = new Vector3(0, 0, curval + (isLeft ? 3 : -3));
            else
                rz = new Vector3(0, 0, curval);

            float z = rz.z * proportion;
            if (z > 52)
                z = 52;
            else if (z < -52)
                z = -52;
            Vector3 pz = new Vector3(0, 0, z);
            Debug.Log(" ----- z: " + z);

            crossBarTweener = ShortcutExtensions.DOLocalRotate(crossBar, rz, 1).OnUpdate(() =>
            {
                //Debug.LogFormat(" ------- CrossBarControl ------  insFish: {0}  ,  isStart: {1}  ,  abs: {2}  ", insFish, isStart, Mathf.Abs(preval - curval) >= fishAngle);
                if (!insFish && !isStart && Mathf.Abs(preval-curval) >= fishAngle)
                {
                    insFish = true;
                    if (isLeft)
                    {
                        //CreateFish(true, true);
                        CreateFish(false, false);
                    }
                    else
                    {
                        CreateFish(false, true);
                        //CreateFish(true, false);
                    }
                }
                PlayCatAnimation();
                leftBasket.position = leftTrans.position;
                rightBasket.position = rightTrans.position;
            });

            pointTweener = ShortcutExtensions.DOLocalRotate(pointGo, pz, 1).OnComplete(() =>
            {
                if (!isStart)
                    CheckOutSuccess();
            });

            //leftHandTweener =  ShortcutExtensions.DOLocalRotate(leftHand, rz + new Vector3(0, 0, isLeft ? offsetHand : -offsetHand), 1);
            //rightHandTweener = ShortcutExtensions.DOLocalRotate(rightHand, rz + new Vector3(0, 0, isLeft ? offsetHand : -offsetHand), 1);
        }


        IEnumerator GameCutDown()
        {
            while(seconds > 0)
            {
                //Debug.LogFormat(" ------------ CutDown ! ----------- {0} ", seconds);
                seconds -= 1;
                int sec = seconds % 60;
                int min = seconds / 60;
                second.text = ConvertCutDownText(sec);
                minute.text = ConvertCutDownText(min);
                yield return new WaitForSeconds(1);
            }
            if (pointTweener != null)
                pointTweener.Kill();
            StopSubScribe();
            GameOver(false);
        }

        string ConvertCutDownText(int cd)
        {
            string s = "";
            int k = cd / 10;
            if (k == 0)
                s = "0" + cd.ToString();
            else
                s = cd.ToString();
            return s;
        }

        void GameOver(bool isSuccess)
        {
            Debug.LogFormat(" -------- Game Over -------- {0}", isSuccess);
            gameOver = true;
            mono.StopAllCoroutines();
            StopSubScribe();
            SoundManager.instance.StopAudio();
            if (isSuccess)
            {
                ShortcutExtensions.DOLocalMove(catall, sucPos, 2.0f).OnComplete(
                    () => 
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                        ViewControl(false, false);
                        successView.SetActive(true);
                        failView.SetActive(false);
                    }
                );
            }
            else
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                ViewControl(false, false);
                successView.SetActive(false);
                failView.SetActive(true);
            }
            //NetworkManager.instance.gameRspEvent -= OnMabotRsp;
            //NetworkManager.instance.gameRspEvent += OnDeviceRsp;
        }

        void CreateFish(bool isAdd, bool isLeft)
        {
            int random = UnityEngine.Random.Range(0, 3);
            fish = fishes[random];
            //Debug.LogFormat(" ------- CreateFish ---------  {0} ", fish);
            GameObject fishClone = GameObject.Instantiate(fish, fishesGp.transform);
            Transform f = fishClone.transform.Find("fish");
            //f.localScale = new Vector3(0.8f, 0.8f, 0);
            fishClone.SetActive(true);

            Vector3 endPos;
            Vector3 rightPos = fishClone.transform.position + new Vector3(961, 0, 0);
            Vector3 endRot;
            if (isAdd)
            {
                f.localScale = new Vector3(isLeft ? -1 : 1, 1, 1);
                endPos = isLeft ? leftFishPos.position : rightFishPos.position;
                endRot = isLeft ? new Vector3(0, 0, -40) : new Vector3(0, 0, 40);
                fishClone.transform.position = isLeft ? fishClone.transform.position : rightPos;
                ShortcutExtensions.DOMove(fishClone.transform, endPos, 0.5f).OnComplete(() =>
                {
                    GameObject.Destroy(fishClone);
                    insFish = false;
                });
            }
            else
            {
                f.localScale = new Vector3(isLeft ? 1 : -1, 1, 1);
                fishClone.transform.position = isLeft ? leftFishPos.position : rightFishPos.position;
                endPos = isLeft ? fish.transform.position : rightPos;
                endRot = isLeft ? new Vector3(0, 0, 40) : new Vector3(0, 0, -40);
                GameObject wt = fishClone.transform.Find("waterSp").gameObject;
                wt.SetActive(false);
                ShortcutExtensions.DOMove(fishClone.transform, endPos, 0.5f).OnComplete(() =>
                {
                    wt.SetActive(true);
                    f.gameObject.SetActive(false);
                    GameObject.Destroy(fishClone, 0.8f);
                    insFish = false;
                });
            }
            ShortcutExtensions.DOLocalRotate(f, endRot, 1);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
            //ShortcutExtensions.DOPath(fishClone.transform, new Vector3[] {  }, 1);
        }

        void PlayCatAnimation()
        {
            if (dangerousTime <= 0 || seconds <= 0)
                return;
            float curz = pointGo.localEulerAngles.z > 180 ? pointGo.localEulerAngles.z - 360 : pointGo.localEulerAngles.z;
            // 绿色区域
            if (curz < greenPoint && curz > -greenPoint && preAni != catSpines[(int)CAT_SPINE.Idle])
            {
                preAni = catSpines[(int)CAT_SPINE.Idle];
                SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.Idle], true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, true);
            }
            // 右边黄色区域
            else if (-redPoint < curz && curz < -greenPoint && preAni != catSpines[(int)CAT_SPINE.rightTilt])
            {
                preAni = catSpines[(int)CAT_SPINE.rightTilt];
                SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.rightTilt], true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            }
            // 左边黄色区域
            else if (greenPoint < curz && curz < redPoint && preAni != catSpines[(int)CAT_SPINE.leftTilt])
            {
                preAni = catSpines[(int)CAT_SPINE.leftTilt];
                SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.leftTilt], true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            }
            // 左边红色区域
            else if(curz > redPoint&& preAni != catSpines[(int)CAT_SPINE.leftDanger])
            {
                preAni = catSpines[(int)CAT_SPINE.leftDanger];
                SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.leftDanger], true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, true);
            }
            //右边红色区域
            else if (curz < -redPoint && preAni != catSpines[(int)CAT_SPINE.rightDanger])
            {
                preAni = catSpines[(int)CAT_SPINE.rightDanger];
                SpineManager.instance.DoAnimation(cat, catSpines[(int)CAT_SPINE.rightDanger], true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, true);
            }
        }

        void CatBodyControl(bool isShow)
        {
            leftHand.gameObject.SetActive(isShow);
            rightHand.gameObject.SetActive(isShow);
            crossBar.gameObject.SetActive(isShow);
            leftBasket.gameObject.SetActive(isShow);
            rightBasket.gameObject.SetActive(isShow);
        }

        public void OnDisable()
        {
            Debug.Log(" CoursMabotCat OnDisable !! ");
            SoundManager.instance.StopAudio();
            StopSubScribe();
        }

        IEnumerator DangerousCountDown(bool isLeft)
        {
            while (dangerousTime > 0)
            {
                dangerousTime--;
                yield return new WaitForSeconds(1);
            }
            if (pointTweener != null)
                pointTweener.Kill();
            gameOver = true;
            StopSubScribe();
            CatBodyControl(false);
            string ani1 = isLeft ? catSpines[(int)CAT_SPINE.leftDrop]: catSpines[(int)CAT_SPINE.rightDrop];
            string ani2 = isLeft ? catSpines[(int)CAT_SPINE.leftDropOver] : catSpines[(int)CAT_SPINE.rightDropOver];
            string ani3 = isLeft ? catSpines[(int)CAT_SPINE.leftDropIdle] : catSpines[(int)CAT_SPINE.rightDropIdle];
            GameObject water = isLeft ? leftDropWater : rightDropWater;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            float aniLength = SpineManager.instance.DoAnimation(cat, ani1, false, ()=> {
                water.SetActive(true);
                SpineManager.instance.DoAnimation(water, "animation", false);
                SpineManager.instance.DoAnimation(cat, ani2, false, ()=> {
                    SpineManager.instance.DoAnimation(cat, ani3, true);
                });
            });
            yield return new WaitForSeconds(aniLength + 2.5f);
            GameOver(false);
        }

        void CheckOutSuccess()
        {
            float curz = pointGo.localEulerAngles.z > 180 ? pointGo.localEulerAngles.z - 360 : pointGo.localEulerAngles.z;
            //Debug.LogFormat(" -------- CheckOutSuccess -------- {0} ", curz);
            
            if (curz> redPoint || curz < -redPoint)
            {
                if (successCo != null)
                {
                    mono.StopCoroutine(successCo);
                    successCo = null;
                }
                if (!isDangerousStart)
                {
                    isDangerousStart = true;
                    if(dangerCo!=null)
                    {
                        Debug.Log(" **********  CheckOutSuccess set dangerCo null ********");
                        dangerCo = null;
                    }
                    dangerCo = mono.StartCoroutine(DangerousCountDown(curz > redPoint));
                }
                else
                {
                    if((lastDanger == -1 && curz < -redPoint) || (lastDanger == 1 && curz > redPoint))
                    {
                        mono.StopCoroutine(dangerCo);
                        dangerousTime = 5;
                        dangerCo = mono.StartCoroutine(DangerousCountDown(curz > redPoint));
                    }
                }
                lastDanger = curz > redPoint ? -1 : 1;
            }
            else
            {
                if (isDangerousStart)
                {
                    Debug.Log(" ----- !!! Lift Dangerous !!! ----- ");
                    //mono.StopCoroutine(DangerousCountDown(curz > redPoint));
                    //if (dangerousTime > 0)
                    //{
                        mono.StopCoroutine(dangerCo);
                        dangerousTime = 5;
                        isDangerousStart = false;
                    //}
                }
                else
                {
                    if (curz < greenPoint && curz > -greenPoint)
                    {
                        if (successCo != null)
                            return;
                        successCo = mono.StartCoroutine(IsSuccess());
                    }
                    else
                    {
                        if (successCo != null)
                        {
                            mono.StopCoroutine(successCo);
                            successCo = null;
                        }
                    }
                }
            }
        }


        IEnumerator IsSuccess()
        {
            yield return new WaitForSeconds(successTime);
            float curz = pointGo.localEulerAngles.z > 180 ? pointGo.localEulerAngles.z - 360 : pointGo.localEulerAngles.z;
            if (curz < greenPoint && curz > -greenPoint)
            {
                Debug.Log(" ------- !!! Congratulation !!! ------");
                GameOver(true);
            }
        }

        /////// ************* WS ************* ///////

        void ConnectMabot()
        {
            Debug.Log(" --=-=-==-= Start Connect Mabot!!! ");
            WsRequest<string> request = new WsRequest<string>();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.GetMabot;
            request.messageId = ((int)Constants.MESSAG_ID.GetMabot).ToString();
            request.path = "/common/bleDevices";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            //NetworkManager.instance.cm.sendMessage(JsonConvert.SerializeObject(request));
            NetworkManager.instance.cm.sendMessage(JsonUtility.ToJson(request));
            Debug.LogFormat(" Connect Mabot request: {0}", JsonUtility.ToJson(request));
        }

        void OnDeviceRsp(object sender, string rsp)
        {
            Debug.LogFormat(" ------ OnDeviceRsp resp: {0}", rsp);
            WsResponse<DeviceInfoReq> data = JsonConvert.DeserializeObject<WsResponse<DeviceInfoReq>>(rsp);
            var mabot = GetConnectMabot(data.data.data.devices);
            NetworkManager.instance.gameRspEvent -= OnDeviceRsp;
            NetworkManager.instance.gameRspEvent += OnMabotRsp;

            WsRequest<MabotInfoReq> request = new WsRequest<MabotInfoReq>();
            request.data = new MabotInfoReq();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.GetGyroscope;
            request.messageId = ((int)Constants.MESSAG_ID.GetGyroscope).ToString();
            request.path = "/common/getGyroscopeData";
            request.mode = "subscribe";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            request.data.name = mabot.name;
            request.data.type = mabot.type;
            NetworkManager.instance.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- OnDeviceRsp req : {0} ", JsonConvert.SerializeObject(request));
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
                //Debug.LogFormat(" ---------- OnMabotRsp rsp: {0}", rsp);
                WsResponse<GyroscopeData> data = JsonConvert.DeserializeObject<WsResponse<GyroscopeData>>(rsp);
                string posStr = data.data.data.msg;
                string[] xyz = posStr.Split(',');
                curMabotY = int.Parse(xyz[1]);
                Debug.LogFormat(" ---------- OnMabotRsp pre : {0}  -----  cur:{1}   ", preMabotY, curMabotY);
                if (preMabotY == 500)
                    CrossBarControl(0, curMabotY, true);
                else
                    CrossBarControl(preMabotY, curMabotY, false);
                preMabotY = curMabotY;
            }
            else if(rsp.Contains("onUnsubscribe") && rsp.Contains("/common/getGyroscopeData"))
            {
                WsResponse<string> msg = JsonConvert.DeserializeObject<WsResponse<string>>(rsp);
                if (msg.data.msg == "success")
                {
                    Debug.Log(" unsubscribe OnMabotRsp Success! ");
                    //NetworkManager.instance.gameRspEvent -= OnMabotRsp;
                    //NetworkManager.instance.gameRspEvent += OnDeviceRsp;
                }
                else
                    Debug.LogError("unsubscribe OnMabotRsp Fail!");
            }

        }

        void StopSubScribe()
        {            
            WsRequest<StopSubcribeReq> request = new WsRequest<StopSubcribeReq>();
            request.data = new StopSubcribeReq();
            request.deviceId = "U3D_" + (int)Constants.DEVICE_ID.StopSubscribe;
            request.messageId = ((int)Constants.MESSAG_ID.StopSubscribe).ToString();
            request.path = "/common/getGyroscopeData";
            request.mode = "unsubscribe";
            request.data.action = "stop";
            request.signature = MD5Helper.CalcMD5(request.messageId + Constants.MD5SEED);
            NetworkManager.instance.cm.sendMessage(JsonConvert.SerializeObject(request));
            Debug.LogFormat(" -------- StopSubScribe  request : {0} ---------", JsonConvert.SerializeObject(request));
        }

        // 获取到连接的Mabot设备信息 因为没做选择所以只取第一个连接的Mabot
        DeviceInfoReq.SingleDevice GetConnectMabot(DeviceInfoReq.SingleDevice[] devices)
        {
            for (int i = 0; i < devices.Length; i++)
            {
                if(devices[i].connected=="true")
                {
                    return devices[i];
                }
            }
            return null;
        }

        int talkIndex = 0;
        ///////// TEST //////////////
        void TestSpeak()
        {
            Debug.Log(" Test Speaker !!!!! ");
            SoundManager.instance.Speaking(bell, "DAIJIshuohua", SoundManager.SoundType.VOICE, 4, null, ()=> {
                SoundManager.instance.ShowVoiceBtn(true);
            });
        }

        void VoiceEvent()
        {
            if (talkIndex == 0)
            {

                Debug.LogFormat(" Set Voice Btn!  {0} ", talkIndex);
                SoundManager.instance.Speaking(bell, "DAIJIshuohua", SoundManager.SoundType.VOICE, 5, () =>
                {
                    Debug.LogFormat(" Speaking !!!!!!  {0}", talkIndex);
                }, () =>
                {
                    Debug.LogFormat(" Speak Over !!!!   {0}", talkIndex);

                });

            }
            else if (talkIndex == 1)
            {

                Debug.LogFormat(" Set Voice Btn!  {0} ", talkIndex);
                SoundManager.instance.Speaking(bell, "DAIJIshuohua", SoundManager.SoundType.VOICE, 3, () =>
                {
                    Debug.LogFormat(" Speaking !!!!!!  {0}", talkIndex);
                }, () =>
                {
                    Debug.LogFormat(" Speak Over !!!!   {0}", talkIndex);

                });

            }

            talkIndex++;
        }

        void SetVoiceEvent()
        {
            SoundManager.instance.SetVoiceBtnEvent(VoiceEvent);
        }
    }
}
