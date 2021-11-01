using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class CourseRussiaPart1
    {
        GameObject curGo;

        GameObject Npc;
        float runTime;

        GameObject Buttom;

        //Part_1部分
        GameObject Part_1;
        GameObject RabbitOfpart1;
        Button RabbitOfpart1Btn;
        GameObject Finger;

        //Part_2部分
        GameObject Part_2;
        GameObject RabbitOfPart2_1;
        Button RabbitOfPart2_1_Btn;
        GameObject RabbitOfPart2_2;
        Button RabbitOfPart2_2_Btn;
        GameObject RabbitOfPart2_3;
        Button RabbitOfPart2_3_Btn;
        GameObject Finger_1;

        GameObject CloudParent;
        GameObject Cloud_1;
        GameObject Cloud_2;

        int ClickCount = 0;
        GameObject Bgs;
        GameObject Trees;
        GameObject Leaves;

        GameObject EndBg;

        MonoBehaviour mono;
        Tweener tweener;

        GameObject b1;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom").gameObject;
            Npc = Buttom.transform.Find("Npc").gameObject;

            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            runTime = 3f;


            //Part1
            Part_1 = Buttom.transform.Find("Part1").gameObject;
            RabbitOfpart1 = Part_1.transform.Find("RabbitofPart1").gameObject;
            RabbitOfpart1Btn = RabbitOfpart1.transform.Find("RabbitofPart1Btn").GetComponent<Button>();
            RabbitOfpart1Btn.onClick.AddListener(GoToPart_2);
            Finger = RabbitOfpart1.transform.Find("Finger").gameObject;

            //Part2
            Part_2 = Buttom.transform.Find("Part2").gameObject;
            Bgs = Part_2.transform.Find("Bgs").gameObject;
            RabbitOfPart2_1 = Part_2.transform.Find("RabbitOfPart2_1").gameObject;
            RabbitOfPart2_1_Btn = RabbitOfPart2_1.transform.Find("RabbitOfPart2_1Btn").GetComponent<Button>();
            RabbitOfPart2_1_Btn.onClick.AddListener(ClickRabbitOfPart2Actions);
            Finger_1 = RabbitOfPart2_1.transform.Find("Finger_1").gameObject;


            RabbitOfPart2_2 = Part_2.transform.Find("RabbitOfPart2_2").gameObject;
            RabbitOfPart2_2_Btn = RabbitOfPart2_2.transform.Find("RabbitOfPart2_2Btn").GetComponent<Button>();
            RabbitOfPart2_2_Btn.onClick.AddListener(ClickRabbitOfPart2Actions);

            RabbitOfPart2_3 = Part_2.transform.Find("RabbitOfPart2_3").gameObject;
            RabbitOfPart2_3_Btn = RabbitOfPart2_3.transform.Find("RabbitOfPart2_3Btn").GetComponent<Button>();
            RabbitOfPart2_3_Btn.onClick.AddListener(ClickRabbitOfPart2Actions);

            CloudParent = Part_2.transform.Find("CloundAnimation").gameObject;
            Cloud_1 = CloudParent.transform.Find("Cloud_1").gameObject;
            Cloud_2 = CloudParent.transform.Find("Cloud_2").gameObject;
            Trees = Part_2.transform.Find("Trees").gameObject;
            Leaves = Part_2.transform.Find("Leaves").gameObject;
            b1 = Part_2.transform.Find("b1").gameObject;

            EndBg = Buttom.transform.Find("EndBg").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();

            InitGame();

        }

        void InitGame()
        {
            RabbitOfpart1Btn.interactable = false;
            Npc.SetActive(false);
            Part_1.SetActive(true);
            Part_2.SetActive(false);
            Finger.SetActive(false);
            EndBg.SetActive(false);
            b1.SetActive(false);
            AnimationOfPart_1();
        }

        void AnimationOfPart_1()
        {
            RabbitOfpart1.transform.localPosition = new Vector3(-482f, -313f, 0f);
            SpineManager.instance.DoAnimation(RabbitOfpart1, "idle", false, () =>
            {

                RabbitOfpart1.transform.DOLocalMoveX(0, 1f);

                SpineManager.instance.DoAnimation(RabbitOfpart1, "run", true, () =>
                {
                    if (RabbitOfpart1.transform.localPosition.x >= 0f)
                    {

                        SpineManager.instance.DoAnimation(RabbitOfpart1, "run_guodu", false, () =>
                        {
                            SpineManager.instance.DoAnimation(RabbitOfpart1, "chicao", false, () =>
                            {
                                SpineManager.instance.DoAnimation(RabbitOfpart1, "naolian", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(RabbitOfpart1, "idle");
                                    //丁丁开始介绍游戏
                                    SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
                                    {
                                        RabbitOfpart1Btn.interactable = true;
                                        Finger.SetActive(true);
                                        SpineManager.instance.DoAnimation(Finger, "animation");
                                    });

                                });
                            });
                        });
                    }
                });

            });
        }

        void GoToPart_2()
        {
            RabbitOfpart1.transform.DOLocalMoveX(1100f, runTime).OnComplete(AnimationOfPart_2);
            SpineManager.instance.DoAnimation(RabbitOfpart1, "run", true);
            Finger.SetActive(false);
        }

        //Part2
        void ActiveBgAt(int index)
        {
            b1.SetActive(false);
            if (index > Bgs.transform.childCount)
            {
                Debug.LogError("index outside");
            }

            for (int i = 0; i < Bgs.transform.childCount; i++)
            {
                Bgs.transform.GetChild(i).gameObject.SetActive(false);
            }

            Bgs.transform.GetChild(index).gameObject.SetActive(true);
            if(index == 1)
            {
                b1.SetActive(true);
            }

        }


        void AnimationOfPart_2()
        {
            Debug.Log("开启第二部分");
            //设置第二部分初始部分
            Part_1.SetActive(false);
            Part_2.SetActive(true);
            ActiveBgAt(0);
            ClickCount = 0;
            RabbitOfPart2_1_Btn.interactable = false;
            RabbitOfPart2_1.transform.localPosition = new Vector3(-1200f, -260f, 0);
            RabbitOfPart2_2.transform.localPosition = new Vector3(-1200f, -538f, 0);
            RabbitOfPart2_3.transform.localPosition = new Vector3(1200f, -447f, 0);

            RabbitOfPart2_2.transform.localScale = new Vector3(0.5f,0.5f,0);
            RabbitOfPart2_3.transform.localScale = new Vector3(-0.5f,0.5f,0);

            CloudParent.SetActive(false);
            Cloud_2.SetActive(false);
            Trees.SetActive(false);
            Leaves.SetActive(false);


            //兔子跑到中间
            SpineManager.instance.DoAnimation(RabbitOfPart2_1, "run", true);
            if (tweener != null)
                DOTween.Kill(tweener,true);

            tweener = RabbitOfPart2_1.transform.DOLocalMoveX(-724f, runTime).OnComplete(() =>
            {
                //兔子开始刨土
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                SpineManager.instance.DoAnimation(RabbitOfPart2_1, "paotu", false, () =>
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    Raining();
                });

                // SpineManager.instance.DoAnimation(RabbitOfPart2_1, "run", false, () =>
                // {
                //     //兔子开始刨土
                //     SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                //     SpineManager.instance.DoAnimation(RabbitOfPart2_1, "paotu", false, () =>
                //     {
                //         SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                //         Raining();
                //     });
                // });

                // RabbitOfPart2_1_Btn.interactable = true;
            });


        }

        void OpenAllRabbitBtnsInteractable()
        {
            RabbitOfPart2_1_Btn.interactable = true;
            RabbitOfPart2_2_Btn.interactable = true;
            RabbitOfPart2_3_Btn.interactable = true;
        }

        void CloseAllRabbitBtnsInteractable()
        {
            RabbitOfPart2_1_Btn.interactable = false;
            RabbitOfPart2_2_Btn.interactable = false;
            RabbitOfPart2_3_Btn.interactable = false;
        }


        void ClickRabbitOfPart2Actions()
        {
            CloseAllRabbitBtnsInteractable();

            switch (ClickCount)
            {
                case 0:
                    PlayAutumnAnimation();
                    break;
            }
            ClickCount++;
        }

        //开始下雨
        void Raining()
        {
            // yield return new WaitForSeconds(0.1f);
            SpineManager.instance.DoAnimation(RabbitOfPart2_1, "jinkeng", false, () =>
            {
                CloudParent.SetActive(true);
                SpineManager.instance.DoAnimation(Cloud_1, "cloud_animation", false, () =>
                {
                    SpineManager.instance.DoAnimation(Cloud_1, "cloud_idle", false, () =>
                    {
                        SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 1, () =>
                        {
                            //开始下雨
                            Cloud_2.SetActive(true);
                            SpineManager.instance.DoAnimation(Cloud_1, "rain_idle", true);
                            SpineManager.instance.DoAnimation(Cloud_2, "cloud_idle", true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                        }, () =>
                        {
                            //下雨结束
                            CloudParent.SetActive(false);
                            ActiveBgAt(1);
                            Trees.SetActive(true);
                            SpineManager.instance.DoAnimation(RabbitOfPart2_1, "chukeng", false);
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            //播放语音
                            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 2, PlayTreesAnimation_1, PlayOtherRabbitAniamtion);
                        });

                    });
                }
                );
            });
        }

        void PlayTreesAnimation_1()
        {
            //播放数成长的动画
            PlayTreesAnimation("animation1", () =>
            {
                PlayTreesAnimation("animation2", () =>
                {
                    PlayTreesAnimation("animation3", () =>
                    {
                        SpineManager.instance.DoAnimation(RabbitOfPart2_2, "run2");
                        SpineManager.instance.DoAnimation(RabbitOfPart2_3, "run3");
                        RabbitOfPart2_2.transform.DOLocalMoveX(0f, runTime).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(RabbitOfPart2_2, "idle2");
                        });

                        RabbitOfPart2_3.transform.DOLocalMoveX(685, runTime).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(RabbitOfPart2_3, "idle3");
                        });
                    });
                });
            });


        }

        void PlayOtherRabbitAniamtion()
        {
            Debug.LogError("播放小鹿吃草动画");
            OpenAllRabbitBtnsInteractable();
            Finger_1.SetActive(true);
            SpineManager.instance.DoAnimation(Finger_1,"animation");
            //SpineManager.instance.DoAnimation(RabbitOfPart2_1, "chuken");
            SpineManager.instance.DoAnimation(RabbitOfPart2_2, "naolian2");
            SpineManager.instance.DoAnimation(RabbitOfPart2_3, "chicao3");

        }

        void PlayTreesAnimation(string aniname, Action endCallBack = null, bool isLoop = false)
        {
            SpineManager.instance.DoAnimation(Trees.transform.GetChild(0).gameObject, aniname, isLoop, endCallBack);
            for (int i = 1; i < Trees.transform.childCount; i++)
            {
                SpineManager.instance.DoAnimation(Trees.transform.GetChild(i).gameObject, aniname, isLoop);
            }
        }


        void PlayLeavesAnimation(string aniname, Action endCallBack = null, bool isLoop = false)
        {
            SpineManager.instance.DoAnimation(Leaves.transform.GetChild(0).gameObject, aniname, isLoop, endCallBack);
            for (int i = 1; i < Leaves.transform.childCount; i++)
            {
                SpineManager.instance.DoAnimation(Leaves.transform.GetChild(i).gameObject, aniname, isLoop);
            }
        }


        void PlayAutumnAnimation()
        {
            Debug.Log("播放秋风卷起白桦树动画");
            Finger_1.SetActive(false);
            Leaves.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            //两只兔子跑了
            RabbitOfPart2_2.transform.localScale = new Vector3(-0.5f,0.5f,0f);
            RabbitOfPart2_3.transform.localScale = new Vector3(0.5f,0.5f,0f);

            SpineManager.instance.DoAnimation(RabbitOfPart2_2,"run2");
            SpineManager.instance.DoAnimation(RabbitOfPart2_3,"run3");

            RabbitOfPart2_2.transform.DOLocalMoveX(-1100f,runTime).OnComplete(()=>{ SpineManager.instance.DoAnimation(RabbitOfPart2_2,"idle2");});
            RabbitOfPart2_3.transform.DOLocalMoveX(1100f,runTime).OnComplete(()=>{ SpineManager.instance.DoAnimation(RabbitOfPart2_3,"idle3");});

            PlayLeavesAnimation("leaf", () =>
            {
                //播放语音
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    Debug.Log("播放白桦树变黄动画");
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    Leaves.SetActive(false);
                    ActiveBgAt(2);
                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 4,
                    () =>
                    {
                        //begin
                        PlayTreesAnimation("animation4", () =>
                        {
                            PlayTreesAnimation("animation5");
                        });
                    },
                    () =>
                    {
                        mono.StartCoroutine(ShowEndBg());
                        //end
                        SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 5, null,
                        () =>
                        {
                            //结束语音说完
                            Debug.LogError("结束语音说完");
                            // ActiveBgAt(1);
                            EndBg.SetActive(true);
                            Npc.SetActive(true);
                        });
                    });

                });
            }, false);
        }

        IEnumerator ShowEndBg()
        {
            yield return new WaitForSeconds(16f);
            EndBg.SetActive(true);
        }

    }

}