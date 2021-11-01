using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class CourseTDKP2L13Part1
    {
        GameObject curGo;
        Transform Buttom;
        Transform TimeCircle, Temperature;
        Image TimeLine;
        Transform EndTimePoint;

        Transform Numbers;

        //秒
        Transform Sec;
        Image t1;
        Image t2;
        float maxTime = 15f;
        float curTime_1 = 0f;
        float curTime_2 = 0f;
        float curtime_3 = 0f;

        GameObject Cloud, FireMon, ClickArea, CoverMask, StarAnim;
        int RoundsCount = 0;
        int LastRoundCount = 0;
        int[] ClickCountOfRound;
        int clickCount = 0;
        int curStepOfRound = 0;
        bool OnClick = false;
        bool OnChange = false;
        List<GameObject> allCubes;
        bool GameEnd = false;
        bool MonFire = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");

            Temperature = Buttom.Find("TimeBar/Temperature");

            TimeCircle = Buttom.Find("TimeBar/TimeCircle");
            TimeLine = TimeCircle.Find("TimeLine").GetComponent<Image>();
            EndTimePoint = TimeCircle.Find("End");
            Sec = Buttom.Find("TimeBar/Time/Sec");
            t1 = Sec.Find("t1").GetComponent<Image>();
            t2 = Sec.Find("t2").GetComponent<Image>();

            Numbers = Buttom.Find("TimeBar/Numbers");

            Cloud = Buttom.Find("Cloud").gameObject;
            FireMon = Buttom.Find("FireMon").gameObject;
            ClickArea = Buttom.Find("ClickArea").gameObject;
            CoverMask = curTrans.Find("Content/CoverMask").gameObject;
            StarAnim = Buttom.Find("StarAnim").gameObject;
            InitGame();
        }

        void InitGame()
        {
            //每回的点击次数
            ClickCountOfRound = new int[] { 6, 9, 12, 15 };
            allCubes = new List<GameObject>();
            RoundsCount = 0;
            LastRoundCount = -1;
            clickCount = 0;
            curStepOfRound = 0;
            InitTemperature();

            curTime_1 = 0f;
            curTime_2 = 0f;
            maxTime = 30f;
            TimeLine.fillAmount = 0f;
            CoverMask.SetActive(false);
            StarAnim.SetActive(false);


            t1.sprite = GetNumberSprite(3);
            t2.sprite = GetNumberSprite(0);

            SpineManager.instance.DoAnimation(Cloud, "yun");
            SpineManager.instance.DoAnimation(FireMon, "1_idle");
            ClickArea.GetComponent<ILObject3DAction>().OnPointDownLua = ClickFireMon;
            ClickArea.GetComponent<ILObject3DAction>().OnPointUpLua = UpFireMon;
            OnClick = false;
            OnChange = false;
            GameEnd = false;
            MonFire = false;
            LogicManager.instance.ShowReplayBtn(false);
        }

        void Update()
        {
            if (curTime_1 < maxTime && !GameEnd)
            {
                curTime_2 += Time.deltaTime;

                if (curTime_2 >= 1f)
                {
                    curTime_2 = 0f;
                    curTime_1 += 1f;

                    //更新时间显示
                    ShowTime();
                }

                //2s refersh temperature
                if (!OnClick)
                {
                    curtime_3 += Time.deltaTime;
                    if (curtime_3 >= 2f)
                    {
                        curtime_3 = 0f;
                        TemperatureDrop();
                    }
                }
                else
                {
                    curtime_3 = 0f;
                }
            }
        }

        void InitTemperature()
        {
            for (int i = 0; i < Temperature.childCount; i++)
            {
                var temp = Temperature.GetChild(i);
                for (int j = 0; j < temp.childCount; j++)
                {
                    var colorTemp = temp.GetChild(j).gameObject;
                    allCubes.Add(colorTemp);
                    colorTemp.SetActive(false);
                }
            }
        }

        void ShowTime()
        {
            float percentage = curTime_1 / maxTime;
            TimeLine.fillAmount = percentage;
            EndTimePoint.eulerAngles = new Vector3(0, 0, -360f * percentage);

            // Debug.Log(Convert.ToInt32(maxTime - curTime_1));
            t1.sprite = GetNumberSprite(Convert.ToInt32(maxTime - curTime_1) / 10);
            t2.sprite = GetNumberSprite(Convert.ToInt32((maxTime - curTime_1) % 10));

            if (curTime_1 >= maxTime)
            {
                CoverMask.SetActive(true);
                GameEnd = true;
                SetStarAnim();
            }
        }

        Sprite GetNumberSprite(int sec)
        {
            return Numbers.GetChild(sec).GetComponent<Image>().sprite;
        }

        private void ClickFireMon(int index)
        {
            OnClick = true;
            clickCount++;
            SetTemperature(clickCount);
        }

        private void UpFireMon(int index)
        {
            // OnClicked = false;
            OnClick = false;
        }


        void SetTemperature(int count)
        {
            if (RoundsCount <= 3)
            {
                int maxCount = ClickCountOfRound[RoundsCount];
                //3个阶段，每个阶段点击次数
                int stepCount = maxCount / 3;
                if (count % stepCount == 0)
                {
                    //点够次数，加一格显示
                    var temp = Temperature.GetChild(RoundsCount).GetChild(curStepOfRound).gameObject;
                    temp.SetActive(true);
                    curStepOfRound++;

                    CoverMask.SetActive(true);
                    string animName = "animation" + (RoundsCount + 1).ToString() + "_" + curStepOfRound;
                    Debug.Log("Anim----" + animName);
                    SpineManager.instance.DoAnimation(FireMon, animName, false, () =>
                      {
                          if (curStepOfRound >= 3)
                          {
                              RoundsCount++;
                              LastRoundCount = RoundsCount - 1;
                              OnChange = true;
                              OnClick = false;

                              if (RoundsCount + 1 <= 4)
                              {
                                  OnChange = false;
                                  CoverMask.SetActive(false);
                                  SpineManager.instance.DoAnimation(FireMon, (RoundsCount + 1) + "_idle");
                                  curStepOfRound = 0;
                                  clickCount = 0;
                              }
                              else
                              {
                                  Debug.Log("进入喷发");
                                  var finalTemp = Temperature.GetChild(3).GetChild(3).gameObject;
                                  finalTemp.SetActive(true);

                                  if (GameEnd)
                                      return;
                                  SpineManager.instance.DoAnimation(FireMon, RoundsCount + "_" + (RoundsCount + 1), false,
                                     () =>
                                     {
                                         //  OnChange = false;
                                         CoverMask.SetActive(false);
                                         SpineManager.instance.DoAnimation(FireMon, "2_idle");

                                         //游戏结束标志
                                         GameEnd = true;
                                         MonFire = true;
                                         SetStarAnim();
                                     });
                              }
                          }
                          else
                          {
                              SpineManager.instance.DoAnimation(FireMon, (RoundsCount + 1) + "_idle");
                              CoverMask.SetActive(false);
                          }
                      });
                }
            }
            else
            {
                var temp = Temperature.GetChild(3).GetChild(3).gameObject;
                temp.SetActive(true);
            }
        }

        void TemperatureDrop()
        {
            if (OnChange)
            {
                return;
            }

            //Debug.LogError(OnClick);

            //当前颜色内的回落
            if (curStepOfRound != 0 && RoundsCount != LastRoundCount)
            {
                if (curStepOfRound > 0)
                {
                    curStepOfRound--;
                }
            }
            else if (curStepOfRound == 0 && RoundsCount != LastRoundCount)
            {
                //不同颜色内的回落
                if (RoundsCount > 0)
                {
                    RoundsCount--;
                    LastRoundCount = RoundsCount - 1;
                    curStepOfRound = 2;
                }
                else
                {
                    //Debug.LogError("没有一个被激活");
                    LastRoundCount = -1;
                }
            }

            // //获取最后一个被激活的cube
            //Debug.LogError("RoundsCount:" + RoundsCount + ",curStepOfRound:" + curStepOfRound);
            GameObject colorTemp = allCubes[RoundsCount * 3 + curStepOfRound];
            colorTemp.SetActive(false);

            //回复不同状态后切换对应的待机动画
            SpineManager.instance.DoAnimation(FireMon, (RoundsCount + 1) + "_idle");
        }

        void SetStarAnim()
        {
            if (RoundsCount + 1 == 5 && MonFire && GameEnd)
            {
                PlayStarAnim(5);
            }
            else if (RoundsCount + 1 == 4 && GameEnd)
            {
                PlayStarAnim(4);
            }
            else if (RoundsCount + 1 == 3 && GameEnd)
            {
                PlayStarAnim(3);
            }
            else if (RoundsCount + 1 == 2 && GameEnd)
            {
                PlayStarAnim(2);
            }
            else if (RoundsCount + 1 == 1 && GameEnd)
            {
                PlayStarAnim(1);
            }
        }

        void PlayStarAnim(int count)
        {
            StarAnim.SetActive(true);
            var skt = StarAnim.GetComponent<SkeletonGraphic>();
            skt.AnimationState.SetEmptyAnimations(0);
            SpineManager.instance.DoAnimation(StarAnim, count.ToString(), false, () =>
            {
                Debug.Log("游戏结束");
                LogicManager.instance.ShowReplayBtn(true);
                LogicManager.instance.SetReplayEvent(ReplayGame);
            });
        }

        void ReplayGame()
        {
            InitGame();
        }

        



    }
}