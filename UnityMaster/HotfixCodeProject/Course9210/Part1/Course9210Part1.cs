using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9210Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject pz;
        private GameObject hb;

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

            pz = curTrans.Find("pz").gameObject;
            hb = curTrans.Find("hb").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            curTrans.Find("zz").gameObject.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            hb.SetActive(false);
            //hb.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            curTrans.Find("bl").gameObject.SetActive(false);
            curTrans.Find("bl2").gameObject.SetActive(false);
            pz.SetActive(false);
            talkIndex = 1;
            pz.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);

        }



        void GameStart()
        { 
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(Wait(3f,()=> { curTrans.Find("bl").gameObject.SetActive(true); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));

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

        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                curTrans.Find("bl").gameObject.SetActive(false);
                curTrans.Find("bl2").gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                    ()=>
                    {
                        curTrans.Find("bl2").gameObject.SetActive(false);
                        pz.SetActive(true);
                        
                        SpineManager.instance.DoAnimation(pz,"animation",false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                            ()=>
                            {
                                SpineManager.instance.DoAnimation(pz, "animation2", false);
                                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                                    () =>
                                    {
                                        SpineManager.instance.DoAnimation(pz, "animation3", false,
                                            () => { SpineManager.instance.DoAnimation(pz,"animation4",false); }
                                            );
                                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                                            ()=>
                                            {
                                                SpineManager.instance.DoAnimation(pz, "20", false);
                                                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null,
                                                    () =>
                                                    {
                                                        mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,6,null,
                                                            ()=>
                                                            {
                                                                SoundManager.instance.ShowVoiceBtn(true);
                                                            }
                                                            ));
                                                        mono.StartCoroutine(Wait(4f,()=> { SpineManager.instance.DoAnimation(pz, "18", false); }));
                                                        mono.StartCoroutine(Wait(8f, () => { SpineManager.instance.DoAnimation(pz, "16", false); }));
                                                        mono.StartCoroutine(Wait(12f, () => { SpineManager.instance.DoAnimation(pz, "14", false); }));
                                                    }
                                                    ));

                                            }
                                            ));
                                    }
                                    ));
                            }
                            ));
                    
                    }
                    ));
            }
            if(talkIndex == 2)
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                curTrans.Find("zz").gameObject.SetActive(true);
                pz.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,7,null,
                    ()=>
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    ));
            }
            if(talkIndex == 3)
            {
                hb.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2,false);
                SpineManager.instance.DoAnimation(hb, "animation", false,
                    ()=>
                    { mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,8)); }
                    );
                
            }

            talkIndex++;
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
