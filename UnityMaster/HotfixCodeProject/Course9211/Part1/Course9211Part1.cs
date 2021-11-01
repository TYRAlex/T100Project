using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9211Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject Bg2;
        float a;
        private Transform BlackGround;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private Transform light;
        private Transform cz;
        private Transform plus;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            Bg2 = curTrans.Find("Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);


            light = curTrans.Find("light");
            cz = curTrans.Find("cz");
            plus = curTrans.Find("plus");
            BlackGround = curTrans.Find("BlackGround");
            a = 0;
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            BlackGround.gameObject.SetActive(false);
            plus.gameObject.SetActive(false);
            cz.gameObject.SetActive(false);
            Bg2.gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1,0);
            BlackGround.GetChild(1).gameObject.SetActive(false);
            BlackGround.GetChild(2).gameObject.SetActive(false);
            BlackGround.GetChild(3).gameObject.SetActive(false);
            light.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(light.gameObject,"animation",false);

            StartMaxTalk();
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
          //  mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; }));

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

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, () =>
                   {
                       WaitTime(5.5f, () =>
                     {
                         Max.SetActive(true);

                         SpineManager.instance.DoAnimation(Max, "DAIJIshuohua", true);

                         WaitTime(0.5f, () => { SpineManager.instance.DoAnimation(light.gameObject, "animation", true); });
                     });
                   }
                , ()=> { SoundManager.instance.ShowVoiceBtn(true); })); 
            }
            if (talkIndex==2) 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,2,()=> 
                {
                    cz.gameObject.SetActive(true);
                    WaitTime(6f, () => { SpineManager.instance.DoAnimation(cz.gameObject, "sz", false); });
                },()=> { SoundManager.instance.ShowVoiceBtn(true); }));
                
            }
            if (talkIndex == 3) 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, () =>
                {
                    WaitTime(6f, () =>
                    {
                        Max.SetActive(false);
                        plus.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(plus.gameObject, "zi", true);
                        SpineManager.instance.DoAnimation(light.gameObject, "animation2", true);
                    });
                }, () => 
                { 
                    SoundManager.instance.ShowVoiceBtn(true);
                })
                    );
            }
            if (talkIndex == 4) 
            {
                Mathf.Clamp(a,0, 255);
                Max.gameObject.SetActive(true);
                cz.gameObject.SetActive(false);
                plus.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max.gameObject,SoundManager.SoundType.VOICE,4,()=>
                {
                    WaitTime(5f, () => { BlackGround.GetChild(1).gameObject.SetActive(true); });
                    WaitTime(9f, () => { BlackGround.GetChild(2).gameObject.SetActive(true); });
                    WaitTime(12.5f, () => { BlackGround.GetChild(3).gameObject.SetActive(true); });
                },()=> 
                {

                })) ;


                myupdate(0.1f,()=> 
                {
                    a += 50;
                    Bg2.gameObject.GetComponent<RawImage>().color = new Color(1,1,1, a/255);
                    if (a >= 255) 
                    {
                        Debug.LogError("1");
                        BlackGround.gameObject.SetActive(true);
                        return;
                    }
                });
              
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void StartMaxTalk()
        {
            Max.SetActive(true);
            //这里有台紫外线灯 仔细观察把
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                Max.SetActive(false);
                SpineManager.instance.DoAnimation(light.gameObject,"animation2",true);
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }


        IEnumerator _WaitTime(float time,Action method_1=null) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        IEnumerator MyUpdate(float time,Action method_1=null) 
        {

            while (true) 
            {
                yield return new WaitForSeconds(time);
                method_1?.Invoke();
                if (a >= 255)
                {
                    yield break;
                }
            }
        }
        private void WaitTime(float time, Action method_1 = null) 
        {
            mono.StartCoroutine(_WaitTime(time, method_1));
        }
        private void myupdate(float time, Action method_1 = null) 
        {
            mono.StartCoroutine(MyUpdate(time, method_1));
           
        }
        
    }
}
