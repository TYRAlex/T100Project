using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course934Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private List<string> name1;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform three;
        private Transform spineGo;
        private Transform OnClick;
        private Transform _mask;
        private bool _canClick;
        private int index;
       

        bool isPlaying = false;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            three = curTrans.Find("three");
            spineGo = curTrans.Find("spine");
            OnClick = curTrans.Find("OnClick");
            _mask = curTrans.Find("_mask");
            _canClick = false;
            index = 0;
            name1 = new List<string>();
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }


        private void GameInit()
        {
            talkIndex = 1;
            _mask.gameObject.SetActive(false);
            Bg.transform.GetChild(0).gameObject.SetActive(false);
            three.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            spineGo.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(three.gameObject,"jing",false);
            ;
            //同学们，你们有主意到智能停车系统中道闸有哪些种类吗，请说一说
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,0,null,()=> 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
            Util.AddBtnClick(OnClick.GetChild(0).gameObject, OnClickEvent1);
            Util.AddBtnClick(OnClick.GetChild(1).gameObject, OnClickEvent2);
            Util.AddBtnClick(OnClick.GetChild(2).gameObject, OnClickEvent3);
            OnClick.gameObject.SetActive(false);
            three.gameObject.SetActive(false);
            spineGo.gameObject.SetActive(false);
            RemoveEvent(_mask.gameObject);

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
                Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                three.gameObject.SetActive(true);
                OnClick.gameObject.SetActive(true);
                //接下来三幅图点击
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        _canClick = true;
                        Max.SetActive(false);
                    }));
            }
            else if (talkIndex==2)
            {
                Max.SetActive(true);
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,5));
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
        private void OnClickEvent1(GameObject obj) 
        {
            if (_canClick == false) { return; }
            if (name1.Contains(obj.name) == false) 
            {
                name1.Add(obj.name);
                index++;
            }
            SoundManager.instance.ShowVoiceBtn(false);
           
            _canClick = false;
            _mask.gameObject.SetActive(true);
            spineGo.gameObject.SetActive(true);
            BtnPlaySound();
        
            SpineManager.instance.DoAnimation(three.gameObject,"d3",false,()=>
            {
                Bg.transform.GetChild(0).gameObject.SetActive(true);
                Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[2];

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2,null,()=> { OnClickMask(obj); }));

                SpineManager.instance.DoAnimation(spineGo.gameObject,"a1",false,()=>
                {
                    
                    SpineManager.instance.DoAnimation(spineGo.gameObject,"a2",false,()=> 
                    {
                     
                    });
                });
            });
        }
        private void OnClickEvent2(GameObject obj)
        {
            if (_canClick == false) { return; }
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }
            SoundManager.instance.ShowVoiceBtn(false);
       
            _canClick = false;
            _mask.gameObject.SetActive(true);
            spineGo.gameObject.SetActive(true);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(three.gameObject, "d2", false, () =>
            {
                Bg.transform.GetChild(0).gameObject.SetActive(true);
                Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[2];

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3,null,()=> { OnClickMask(obj); }));

                SpineManager.instance.DoAnimation(spineGo.gameObject, "b1", false, () =>
                {
                    
                    SpineManager.instance.DoAnimation(spineGo.gameObject, "b2", false, () =>
                    {
                      
                    });
                });
            });
        }
        private void OnClickEvent3(GameObject obj)
        {
            if (_canClick == false) { return; }

            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }
            SoundManager.instance.ShowVoiceBtn(false);
          
            _canClick = false;
            _mask.gameObject.SetActive(true); spineGo.gameObject.SetActive(true);
            BtnPlaySound(); 
            SpineManager.instance.DoAnimation(three.gameObject, "d1", false, () =>
            {
                Bg.transform.GetChild(0).gameObject.SetActive(true);
                Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[2];

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4,null,()=> { OnClickMask(obj); }));

                SpineManager.instance.DoAnimation(spineGo.gameObject, "c1", false, () =>
                {
                    
                    SpineManager.instance.DoAnimation(spineGo.gameObject, "c2", false, () =>
                    {
                        
                    });
                });
            });
        }
        void OnClickMask(GameObject obj)
        {
           
            AddEvent(_mask.gameObject, g => {
                BtnPlaySound();
               
                // PlayCommonSound(2);
                switch (obj.name)
                {
                    
                    case "1":
                        //
                        SpineManager.instance.DoAnimation(spineGo.gameObject,"a3",false,()=>
                        {
                            Bg.transform.GetChild(0).gameObject.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                            SpineManager.instance.DoAnimation(three.gameObject, "h", false,()=> 
                            {
                                _canClick = true;
                            }); 
                         
                            _mask.gameObject.SetActive(false);

                        });
                 
                        break;
                    case "2":
                        //
                        SpineManager.instance.DoAnimation(spineGo.gameObject, "b3", false,()=>
                        {
                            Bg.transform.GetChild(0).gameObject.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                            SpineManager.instance.DoAnimation(three.gameObject, "h", false,()=> 
                            {
                                _canClick = true;
                            });
                          
                            _mask.gameObject.SetActive(false);
                        });
                     
                        break;
                    case "3":
                        //
                        SpineManager.instance.DoAnimation(spineGo.gameObject, "c3", false,()=>
                        {
                            Bg.transform.GetChild(0).gameObject.SetActive(false);
                            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                            SpineManager.instance.DoAnimation(three.gameObject, "h", false,()=> 
                            {
                                _canClick = true;
                            }); 
                         
                            _mask.gameObject.SetActive(false);
                        });

                        break;
                }
                if (index== 3) 
                {
                    Wait(1.666f, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    });
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
        IEnumerator wait(float time, Action method_1 = null) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void Wait(float time, Action method_1 = null) 
        {
            mono.StartCoroutine(wait(time,method_1));
        }
    }
}
