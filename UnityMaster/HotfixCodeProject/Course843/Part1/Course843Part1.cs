using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
namespace ILFramework.HotClass
{
    public class Course843Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject clickBox;
        private bool _canClick;
        private GameObject pbj;
        private bool[] allJugle;
        private GameObject pbr;
        private GameObject mask;
        private GameObject clickPeople;
        private bool cc;
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

            allJugle = new bool[7];
            pbj = curTrans.Find("pbj").gameObject;
            pbr = curTrans.Find("pbr").gameObject;
            mask = curTrans.Find("mask").gameObject;
            clickPeople = curTrans.Find("clickPeople").gameObject;
            clickBox = curTrans.Find("clickBox").gameObject;
            for (int i = 0; i < clickBox.transform.childCount; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject,ClickEvent);
            }

            Util.AddBtnClick(clickPeople,ClickPeopleEvent);
            GameInit();
            GameStart();
        }


        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                BtnPlaySound();
                _canClick = false;
                pbj.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(pbj, obj.name, false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE, Convert.ToInt32(obj.name), null,
                    () =>
                    {
                        allJugle[Convert.ToInt32(obj.name) - 1] = true;
                        JugleAll();
                        _canClick = true; }));
            }
        }

        private void JugleAll()
        {
            for (int i = 0; i < 7; i++)
            {
                if (!allJugle[i])
                {
                    return;
                }
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void ClickPeopleEvent(GameObject obj)
        {
            mask.SetActive(true);
            Run3times();
        }
        IEnumerator WaitTime(float time ,Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private void GameInit()
        {
            cc = false;
            _canClick = false;
            mask.SetActive(false);
            clickBox.SetActive(true);
            clickPeople.SetActive(false);
            pbj.SetActive(true);
            pbr.SetActive(false);
            talkIndex = 1;

        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; isPlaying = false; }));

        }

        private void Wait1Speak()
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 8,null,
                ()=>
                { SoundManager.instance.ShowVoiceBtn(true); }
                ));
        }

        private void Run3times()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,true);
            if (cc)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false); }
            pbr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(pbr,"animation4",false,
                ()=>
                { SpineManager.instance.DoAnimation(pbr, "animation4", false,
                    ()=>
                    { SpineManager.instance.DoAnimation(pbr, "animation4", false,
                        ()=>
                        { SpineManager.instance.DoAnimation(pbr, "animation2", false,
                            ()=>
                            { mask.SetActive(false);
                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                cc = true;
                            }
                            ); }
                        ); }
                    ); }
                );
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
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                pbj.SetActive(false);
                clickBox.SetActive(false);
                pbr.SetActive(true);
                pbr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(pbr,"animation",true);
                mono.StartCoroutine(WaitTime(1, Wait1Speak));        
            }
            if (talkIndex == 2)
            {
                mask.SetActive(true);
                clickPeople.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,9));
                //pbr.transform.GetComponent<SkeletonGraphic>().startingLoop = false;
                //SpineManager.instance.DoAnimation(pbr, "animation3", false,
                //        () =>
                //        { Run3times(); }
                //        );
                pbr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(pbr, "animation", false,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
                        pbr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(pbr, "animation3", false,
                          () =>
                          { Run3times(); }
                          );
                    }
                    );
            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
