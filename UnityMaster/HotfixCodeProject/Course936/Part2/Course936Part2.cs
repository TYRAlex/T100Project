using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course936Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject OnClickMask;

        private GameObject Max;
        private GameObject spineUI;
        private Transform onClick;
        private Transform Level1Bg;
        private Transform Level2Bg;
        private Transform Level3Bg;
        private Transform Level4Bg;
        private Transform Level5Bg;
        private List<string> nameList;
        private int nameindex;

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
            spineUI = curTrans.Find("black/spineui").gameObject;
            onClick = curTrans.Find("OnClick");
            Level1Bg = curTrans.Find("Level1bg");
            Level2Bg = curTrans.Find("Level2bg");
            Level3Bg = curTrans.Find("Level3bg");
            Level4Bg = curTrans.Find("Level4bg");
            Level5Bg = curTrans.Find("Level5bg");
            OnClickMask = curTrans.Find("mask").gameObject;
            nameindex = 0;
            nameList = new List<string>();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            AddOnClickEvent();
        }







        private void GameInit()
        {
            talkIndex = 1;
            RemoveEvent(OnClickMask);
            //我们知道指南针可以辨别方向
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,0,null,()=> { SoundManager.instance.ShowVoiceBtn(true); }));
            Wait(3.5f,()=> 
            {
                SpineManager.instance.DoAnimation(spineUI,"animation",false);
            });

            for (int i = 0; i < onClick.childCount; i++) 
            {
                onClick.GetChild(i).gameObject.SetActive(true);
            }
            spineUI.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level1Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level2Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level3Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Level5Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject,"kong",false);
            SpineManager.instance.DoAnimation(Level2Bg.Find("spine").gameObject, "kong", false);
            SpineManager.instance.DoAnimation(Level3Bg.Find("spine").gameObject, "kong", false);
            SpineManager.instance.DoAnimation(spineUI,"animation4",false);
            // SpineManager.instance.DoAnimation(Level4Bg.Find("spine").gameObject, "kong", false);
            SpineManager.instance.DoAnimation(Level5Bg.Find("spine").gameObject, "kong", false);
            onClick.gameObject.SetActive(false);
            Level1Bg.gameObject.SetActive(false);
            Level2Bg.gameObject.SetActive(false);
            Level3Bg.gameObject.SetActive(false);
            Level4Bg.gameObject.SetActive(false);
            Level5Bg.gameObject.SetActive(false);

        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
           // mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; }));

        }
        IEnumerator wait(float time, Action method_1 = null)
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void Wait(float time, Action method_1 = null)
        {
            mono.StartCoroutine(wait(time, method_1));
        }
        private void AddOnClickEvent() 
        {
            Util.AddBtnClick(onClick.Find("1").gameObject,OnClickEvent1);
            Util.AddBtnClick(onClick.Find("2").gameObject, OnClickEvent2);
            Util.AddBtnClick(onClick.Find("3").gameObject, OnClickEvent3);
            Util.AddBtnClick(onClick.Find("4").gameObject, OnClickEvent4);
            Util.AddBtnClick(onClick.Find("5").gameObject, OnClickEvent5);
        }
        private void OnClickEvent1(GameObject obj) 
        {
            SoundManager.instance.ShowVoiceBtn(false);
            Level1Bg.gameObject.SetActive(true);
            Max.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,2,null,()=> 
            {
                //OnClickMask;
                OnClickMaskEvent(obj);
            }));
            SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject,"1",false,()=> 
            {
                SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject, "2", false,()=> 
                {
                    SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject, "3", false,()=> 
                    {
                        SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject, "4", false,()=> 
                        {
                            SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject, "5", true);
                        });
                    });

                });
            });
        }

        private void OnClickEvent2(GameObject obj)
        {
            Max.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);
            Level2Bg.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
            {
                //OnClickMask;
                OnClickMaskEvent(obj);
            }));
            SpineManager.instance.DoAnimation(Level2Bg.Find("spine").gameObject,"animation",true) ;
        }
        private void OnClickEvent3(GameObject obj)
        {
            Max.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);
            Level3Bg.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Level3Bg.Find("spine").gameObject,"animation3",false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
            {
                //OnClickMask;
                OnClickMaskEvent(obj);
            }));
            SpineManager.instance.DoAnimation(Level3Bg.Find("ui").gameObject,"zi",false);
            Wait(13f,()=> 
            {
                SpineManager.instance.DoAnimation(Level3Bg.Find("ui").gameObject, "zi3", false);
                SpineManager.instance.DoAnimation(Level3Bg.Find("spine").gameObject,"animation",false);
            });
            Wait(16f, () =>
            {
                SpineManager.instance.DoAnimation(Level3Bg.Find("ui").gameObject, "zi2", false);
                SpineManager.instance.DoAnimation(Level3Bg.Find("spine").gameObject, "animation2", false);
            });
        }
        private void OnClickEvent4(GameObject obj)
        {
            Max.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);
            Level4Bg.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, () =>
            {
                //OnClickMask;
                OnClickMaskEvent(obj);
            }));
            SpineManager.instance.DoAnimation(Level4Bg.Find("spine").gameObject,"animation",false);
        }
        private void OnClickEvent5(GameObject obj)
        {
            Max.SetActive(true);
            SoundManager.instance.ShowVoiceBtn(false);
            Level5Bg.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE,6, null, () =>
            {
                //OnClickMask;
                OnClickMaskEvent(obj);
            }));
            SpineManager.instance.DoAnimation(Level5Bg.Find("spine").gameObject,"animation2",true);

        }
        private void OnClickMaskEvent(GameObject obj)
        {
            Max.SetActive(false);
            OnClickMask.SetActive(true);
            if (!nameList.Contains(obj.name)) 
            {
                nameList.Add(obj.name);
                nameindex++;

            }
            AddEvent(OnClickMask,g=> 
            {
                switch (obj.name) 
                {
                    case "1":
                        BtnPlaySound();
                        Level1Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(Level1Bg.Find("spine").gameObject,"kong",false);
                        Level1Bg.gameObject.SetActive(false);
                        OnClickMask.SetActive(false);
                        if (nameindex == 5) { SoundManager.instance.ShowVoiceBtn(true); }
                        break;
                    case "2":
                        BtnPlaySound();
                        Level2Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(Level2Bg.Find("spine").gameObject,"kong",false);
                        Level2Bg.gameObject.SetActive(false);
                        OnClickMask.SetActive(false);
                        if (nameindex == 5) { SoundManager.instance.ShowVoiceBtn(true); }
                        break;
                    case "3":
                        BtnPlaySound();
                        Level3Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(Level3Bg.Find("spine").gameObject,"kong",false);
                        Level3Bg.gameObject.SetActive(false);
                        OnClickMask.SetActive(false);
                        if (nameindex == 5) { SoundManager.instance.ShowVoiceBtn(true); }
                        break;
                    case "4":
                        BtnPlaySound();
                        Level4Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(Level4Bg.Find("spine").gameObject, "kong", false);
                        Level4Bg.gameObject.SetActive(false);
                        OnClickMask.SetActive(false);
                        if (nameindex == 5) { SoundManager.instance.ShowVoiceBtn(true); }
                        break;
                    case "5":
                        BtnPlaySound();
                        Level5Bg.Find("spine").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(Level5Bg.Find("spine").gameObject, "kong", false);
                        Level5Bg.gameObject.SetActive(false);
                        OnClickMask.SetActive(false);
                        if (nameindex == 5) { SoundManager.instance.ShowVoiceBtn(true); }
                        break;
                }
                RemoveEvent(OnClickMask);
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

        private void AddEvent(GameObject obj, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(obj).onClick += g => { callBack?.Invoke(g); };
        }
        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                OnClickMask.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,()=> { Max.SetActive(false); onClick.GetChild(0).gameObject.SetActive(true); OnClickMask.SetActive(false); }));

                SpineManager.instance.DoAnimation(spineUI,"animation2",false);
                onClick.gameObject.SetActive(true);
             
            }
            else if(talkIndex==2) 
            {
                
                Max.SetActive(true);
                onClick.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, null));
                SpineManager.instance.DoAnimation(spineUI,"animation3",false,null);
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
