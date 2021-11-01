using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course721Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform axx;
        private Transform bxx;
        private Transform cxx;
        private Transform a;
        private Transform b;
        private Transform c;

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
            a = curTrans.Find("a");
            b = curTrans.Find("b");
            c = curTrans.Find("c");
            axx = curTrans.Find("axx");
            bxx = curTrans.Find("bxx");
            cxx = curTrans.Find("cxx");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            SpineManager.instance.DoAnimation(axx.gameObject,"kong",true);
            SpineManager.instance.DoAnimation(bxx.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(cxx.gameObject, "kong", true);
            SpineManager.instance.DoAnimation(a.gameObject, "a", true);
            SpineManager.instance.DoAnimation(b.gameObject, "b", true);
            SpineManager.instance.DoAnimation(c.gameObject, "c", true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            bxx.GetComponent<SkeletonGraphic>().timeScale = 0.666f;
            Delay(2f,()=>
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0,()=>
                {
                    SpineManager.instance.DoAnimation(axx.gameObject, "a2", false, () => 
                    {
                        SpineManager.instance.DoAnimation(axx.gameObject,"a2",false,()=>
                        {
                            SpineManager.instance.DoAnimation(a.gameObject, "a3", true);
                          //  SpineManager.instance.DoAnimation(axx.gameObject, "a2", false,()=> { });
                        });
                    });
                    SpineManager.instance.DoAnimation(bxx.gameObject, "b2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(bxx.gameObject, "b2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(b.gameObject, "b3", true);
                            //SpineManager.instance.DoAnimation(bxx.gameObject, "b2", false, () => { });
                        });
                    });
                    SpineManager.instance.DoAnimation(cxx.gameObject, "c2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(cxx.gameObject, "c2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(c.gameObject, "c3", true); SoundManager.instance.ShowVoiceBtn(true);
                         //   SpineManager.instance.DoAnimation(cxx.gameObject, "c2", false, () => { });
                        });
                    });
                }, () => 
                {
                    // Max.SetActive(false); isPlaying = false;
                  
                }));
            });
         

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

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,null));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
    }
}
