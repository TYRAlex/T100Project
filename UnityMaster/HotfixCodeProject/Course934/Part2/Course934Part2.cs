using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course934Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject Bg2;
        private BellSprites bellTextures;

        private GameObject Max;
        private Transform OnClick;
        private GameObject SpineGo;
        private GameObject _mask;
        private bool _canClick;
        private int index;
        private List<string> name1;

        bool isPlaying = false;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            Bg2 = curTrans.Find("GameObject/Bg2").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            OnClick = curTrans.Find("OnClick");
            _mask = curTrans.Find("_mask").gameObject;
            SpineGo = curTrans.Find("GameObject/SpineGo").gameObject;
            Max = curTrans.Find("bell").gameObject;
            _canClick = false;
            index = 0;
            name1 = new List<string>();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            AddBtnOnClick();
        }







        private void GameInit()
        {
            talkIndex = 1;

            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
            SpineGo.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(SpineGo,"jing",false);
            Bg2.SetActive(false);
            RemoveEvent(_mask);
            _mask.SetActive(false);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            //麦克斯说话
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;_canClick = true; }));

        }
        private void AddBtnOnClick() 
        {
            Util.AddBtnClick(OnClick.GetChild(0).gameObject,OnClickEvent1);
            Util.AddBtnClick(OnClick.GetChild(1).gameObject, OnClickEvent2);
        }
        private void OnClickEvent1(GameObject obj) 
        {
            if (_canClick == false) 
            {
                return;
            }
            if (name1.Contains(obj.name)==false) 
            {
                index++;
                name1.Add(obj.name);
            }
            SoundManager.instance.ShowVoiceBtn(false);
            _canClick = false;
            _mask.SetActive(true);
            SpineManager.instance.DoAnimation(SpineGo,"d1",false,()=> 
            {
                //Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                Bg2.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,()=> { OnClickMask(obj); }));

                SpineManager.instance.DoAnimation(SpineGo,"dh1",true,()=> 
                {
                    //添加点击返回事件
                  
                });
            });
        }
        private void OnClickEvent2(GameObject obj)
        {
            if (_canClick == false)
            {
                return;
            }
            if (name1.Contains(obj.name) == false)
            {
                index++;
                name1.Add(obj.name);
            }
            SoundManager.instance.ShowVoiceBtn(false);
            _canClick = false;
            _mask.SetActive(true);
            SpineManager.instance.DoAnimation(SpineGo, "d2", false, () =>
            {
               // Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                Bg2.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () => { OnClickMask(obj); }));

                SpineManager.instance.DoAnimation(SpineGo, "dh2",true, () =>
                {
                    //添加点击返回事件
                  
                });
            });
        }
        private void OnClickMask(GameObject obj) 
        {
      
           
            AddEvent(_mask,g=> 
            {
                switch (obj.name)
                {     
                    case "1":
                        BtnPlaySound();
                        // Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
                        Bg2.SetActive(false);
                        SpineManager.instance.DoAnimation(SpineGo,"h1",false,()=> 
                        {
                            _mask.SetActive(false);
                            _canClick = true;
                            SpineManager.instance.DoAnimation(SpineGo,"jing",false);
                            if (index == 2) 
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                        });
                        break;
                    case "2":
                        BtnPlaySound();
                        Bg2.SetActive(false);
                        // Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
                        SpineManager.instance.DoAnimation(SpineGo, "h2", false, () =>
                        {
                            _mask.SetActive(false);
                            _canClick = true;
                            SpineManager.instance.DoAnimation(SpineGo, "jing", false);
                            if (index == 2)
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                        });
                        break;
                }
                RemoveEvent(_mask);
            });
        }
        private void AddEvent(GameObject obj,PointerClickListener.VoidDelegate callBack) 
        {
            PointerClickListener.Get(obj).onClick += g => { callBack?.Invoke(g); };
        }
        private void RemoveEvent(GameObject go) 
        {
            PointerClickListener.Get(go).onClick = null;
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
                Max.SetActive(true);
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE,3, null, () => { }));
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
