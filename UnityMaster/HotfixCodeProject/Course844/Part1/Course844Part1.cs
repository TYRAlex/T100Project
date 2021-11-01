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
    public class Course844Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform _SpineGo;
        private Transform OnClick;
        private GameObject _mask;
        private Transform robot;
        private Transform people;

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
            robot = curTrans.Find("robot");
            people = curTrans.Find("people");
            _SpineGo = curTrans.Find("SpineGo");
            OnClick = curTrans.Find("OnClick");
            _mask = curTrans.Find("mask").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;

            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.SOUND,0,null,()=> { SoundManager.instance.ShowVoiceBtn(true); }));
            OnClick.gameObject.SetActive(false);
            _mask.SetActive(false);
            robot.gameObject.SetActive(true);
            people.gameObject.SetActive(true); 
            robot.GetComponent<SkeletonGraphic>().freeze = true;
            people.GetComponent<SkeletonGraphic>().freeze = true;

            _SpineGo.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
          
            _SpineGo.gameObject.SetActive(false);
            PlaySpine(_SpineGo.gameObject,"jing",null,true);


            Util.AddBtnClick(OnClick.GetChild(0).gameObject, OnClickEvent1);
            Util.AddBtnClick(OnClick.GetChild(1).gameObject, OnClickEvent2);
            Util.AddBtnClick(OnClick.GetChild(2).gameObject, OnClickEvent3);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
         //   mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; }));

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

            SpineManager.instance.DoAnimation(speaker, "daiji1");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji1");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                robot.GetComponent<SkeletonGraphic>().freeze = false;
                people.GetComponent<SkeletonGraphic>().freeze = false;
                Delay(7f, () => { SoundManager.instance.ShowVoiceBtn(true); });
            }
            if (talkIndex == 2) 
            {
                robot.gameObject.SetActive(false);
                people.gameObject.SetActive(false);
                _SpineGo.gameObject.SetActive(true);
                PlaySpine(Max, "daijishuohua", null, true);
                Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1),()=> 
                {
                    PlaySpine(Max, "daiji1", null, true);
                    Max.SetActive(false);
                    OnClick.gameObject.SetActive(true);
                });
                
            }
            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }
        private void OnClickEvent1(GameObject obj) 
        {
            _mask.SetActive(true);
            BtnPlaySound();
            PlaySpine(_SpineGo.gameObject,"d3",()=> 
            {
                PlaySpine(_SpineGo.gameObject, "dh3", null, true);
                Delay(2.5f, () => 
                {
                    OnClickMask(obj);
                });
            },false);
        }
        private void OnClickEvent2(GameObject obj)
        {
            _mask.SetActive(true);
            BtnPlaySound();
            PlaySpine(_SpineGo.gameObject, "d2", ()=> 
            {
                PlaySpine(_SpineGo.gameObject, "dh2", null, true);
                Delay(2.5f, () =>
                {
                    OnClickMask(obj);
                });
            }, false);
        }
        private void OnClickEvent3(GameObject obj)
        {
            _mask.SetActive(true);
            BtnPlaySound();
            PlaySpine(_SpineGo.gameObject, "d1",()=> 
            {
                PlaySpine(_SpineGo.gameObject, "dh1",null, true);
                Delay(2.5f, () =>
                {
                    OnClickMask(obj);
                });
            }, false);
        }

        void OnClickMask(GameObject obj)
        {
          
            AddEvent(_mask.gameObject, g => {
                BtnPlaySound();
                // PlayCommonSound(2);
                switch (obj.name)
                {
                    case "d1":
                        PlaySpine(_SpineGo.gameObject,"h3",()=> { _mask.SetActive(false); },false);
                        break;
                    case "d2":
                        PlaySpine(_SpineGo.gameObject, "h2", () => { _mask.SetActive(false); }, false);
                        break;
                    case "d3":
                        PlaySpine(_SpineGo.gameObject, "h1", () => { _mask.SetActive(false); }, false);
                        break;
                }
                RemoveEvent(g);
                
                
            });
        }
        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }
        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
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
