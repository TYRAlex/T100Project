using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course735Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform spineGo;
        private Transform spine_1;
        private Transform spine_2;
        private Transform effect;
        private Transform effect2;
        private Transform effect3;
        private Transform effect4;
        private Transform image_3;
        private Transform image_4;
        private Transform image_5;

        bool isPlaying = false;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            spineGo = curTrans.Find("spineGo");
            spine_1 = curTrans.Find("1");
            spine_2 = curTrans.Find("2");
            effect = curTrans.Find("effect");
            effect2 = curTrans.Find("effect2");
            effect3 = curTrans.Find("effect3");
            effect4 = curTrans.Find("effect4");
            image_3 = curTrans.Find("3");
            image_4 = curTrans.Find("4");
            image_5 = curTrans.Find("5");
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();

            CW();
            GameStart();
        }







        private void GameInit()
        {
            Max.SetActive(true);
            talkIndex = 1;
       //     mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            //我们要控制机器人交警做出指定的手势，那该如何编程呢？我们先来分析做“左转弯待转信号”手势时的流程吧！ 
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

        }



        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                //机器人交警起始均为立正姿势，左转弯待转信号需要依次完成：转头同时摆动手臂至45°—摆动手臂至15°—再次摆动手臂至45°—再次摆动手臂至15°—立正。
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => {  isPlaying = false; SoundManager.instance.ShowVoiceBtn(true);Max.SetActive(false); }));
                Wait(7f,()=> 
                {
                    SpineManager.instance.DoAnimation(spineGo.gameObject, "1", false, () =>
                    {
                        SpineManager.instance.DoAnimation(spineGo.gameObject, "2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(spineGo.gameObject, "3", false, () =>
                            {
                                SpineManager.instance.DoAnimation(spineGo.gameObject, "4", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(spineGo.gameObject, "5", false, () =>
                                    {
                                        SpineManager.instance.DoAnimation(spineGo.gameObject, "6", false, () =>
                                        {
                                          
                                        });
                                    });
                                });
                            });
                        });
                    });
                });
         

            }
            else if (talkIndex == 2)
            {
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                spineGo.gameObject.SetActive(false);
                spine_1.gameObject.SetActive(true); spine_2.gameObject.SetActive(true);
                effect.gameObject.SetActive(true); effect2.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(spine_1.gameObject, "8", true);
                SpineManager.instance.DoAnimation(spine_2.gameObject, "9", true);
                SpineManager.instance.DoAnimation(effect.gameObject, "jiantou2", true);
                SpineManager.instance.DoAnimation(effect2.gameObject, "jiantou", true);
                Wait(14f, () => 
                {
                    spine_1.gameObject.SetActive(false); spine_2.gameObject.SetActive(false);
                    effect.gameObject.SetActive(false); effect2.gameObject.SetActive(false);
                    image_4.gameObject.SetActive(true); image_5.gameObject.SetActive(true); 
                });
            }
            else if (talkIndex==3)
            {
                //点击运动语句积木的绿色链接图标，点击第二个选项，即可同时执行当前与下一个指令。接下来，赶紧试试为自己的机器人交警编写手势指令吧。
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null, () => {  }));
               
                Wait(1f, () => 
                {
                    spine_1.gameObject.SetActive(false); spine_2.gameObject.SetActive(false);
                    effect.gameObject.SetActive(false); effect2.gameObject.SetActive(false);
                    image_4.gameObject.SetActive(true); image_5.gameObject.SetActive(true); effect3.gameObject.SetActive(true);

                });

                Wait(3f, () =>
                { 
                    image_3.gameObject.SetActive(true);
                    image_4.gameObject.SetActive(false); image_5.gameObject.SetActive(false);
                    effect3.gameObject.SetActive(false);
                    effect4.gameObject.SetActive(true);
                });


            }

            talkIndex++;
        }
        IEnumerator wait(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
        private void CW() 
        {
            spineGo.gameObject.SetActive(true);
            spine_1.gameObject.SetActive(false);
            spine_2.gameObject.SetActive(false);
            effect.gameObject.SetActive(false);
            effect2.gameObject.SetActive(false);
            effect3.gameObject.SetActive(false);
            effect4.gameObject.SetActive(false);
            image_3.gameObject.SetActive(false);
            image_4.gameObject.SetActive(false);
            image_5.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(spine_1.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(spine_2.gameObject, "kong", false);
            SpineManager.instance.DoAnimation(effect.gameObject,"kong",false);
            SpineManager.instance.DoAnimation(effect2.gameObject, "kong", false);
            spineGo.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(spineGo.gameObject, "jing", false);
        }
        private void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(wait(time, method));
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
