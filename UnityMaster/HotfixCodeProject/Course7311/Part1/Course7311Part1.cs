using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course7311Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject show;
        private GameObject show2;

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
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            show = curTrans.Find("show").gameObject;
            show2 = curTrans.Find("show2").gameObject;
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            show.SetActive(true);
            show2.SetActive(false);
            show.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            show2.GetComponent<SkeletonGraphic>().Initialize(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
            Max.SetActive(true);
            isPlaying = true;
            SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject,"la",false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {  isPlaying = false;SoundManager.instance.ShowVoiceBtn(true); }));

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
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                show.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject, "qdqcx", false,
                    () =>
                    {
                        
                        MaxSpeak(1,()=> { SoundManager.instance.ShowVoiceBtn(true); });
                        SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject, "qdq", false);
                        Delay(7f,()=> { SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject, "qdq2", false); });
                    }
                    );
            }
            if(talkIndex==2)
            {
                show.SetActive(false);
                show2.SetActive(true);
                MaxSpeak(2,()=> 
                {
                    Delay(1f, () => 
                    {
                        MaxSpeak(3,()=> { SoundManager.instance.ShowVoiceBtn(true); });
                        SpineManager.instance.DoAnimation(show2, "B3", false, () => {  SpineManager.instance.DoAnimation(show2, "B4", false); });
                        mono.StartCoroutine(Wait(16f,()=> { SpineManager.instance.DoAnimation(show2, "B5", false); }));
                    });
                });
                SpineManager.instance.DoAnimation(show2,"B2",false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1);
                
            }
            if(talkIndex ==3)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                MaxSpeak(4,()=> { Delay(1f, () => { SpineManager.instance.DoAnimation(show2, "shu4", false); MaxSpeak(5,()=> { Delay(1f,()=> { MaxSpeak(6); }); }); }); });
                SpineManager.instance.DoAnimation(show2, "shu2", false);
            }
            talkIndex++;
        }

        private void MaxSpeak(int index,Action callback =null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,index,null,callback));
        }
        private void Delay(float time,Action callback =null )
        {
            mono.StartCoroutine(Wait(time,callback));
        }
        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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
