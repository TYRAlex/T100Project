using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course842Part2
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
        private GameObject renwu;
        private GameObject zi;
        private GameObject _return;
        private bool _canClick;
        private bool _canReturn;
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

            clickBox = curTrans.Find("clickBox").gameObject;
            renwu = curTrans.Find("renwu").gameObject;
            zi = curTrans.Find("zi").gameObject;
            _return = curTrans.Find("btnBack").gameObject;

            Util.AddBtnClick(_return, ReturnEvent);

            for (int i = 0; i < clickBox.transform.childCount; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject, ClickEvent);
            }

            GameInit();
            GameStart();
        }



        private void ReturnEvent(GameObject obj)
        {
            if (_canReturn)
            {
                _canReturn = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2);
                //clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(renwu, "xs", false,
                    () =>
                    {                     
                        
                        SpineManager.instance.DoAnimation(clickBox, "cj3", false,
                            () =>
                            {
                                
                                
                                SpineManager.instance.DoAnimation(clickBox, "animation", false, ()
                               =>
                                {
                                    renwu.SetActive(false);
                                    _return.SetActive(false);
                                    _canClick = true;
                                    _canReturn = true;
                                });
                            });
                    }
                    );

            }
        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                renwu.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                mono.StartCoroutine(ieWaittime(0.5f,
                    ()=>
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false); }
                    ));
                if (obj.name == "gongbu")
                {
                    SpineManager.instance.DoAnimation(clickBox, "c", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(clickBox,"cj",false,
                                ()=>
                                {
                                    renwu.SetActive(true);
                                    SpineManager.instance.DoAnimation(renwu, "cx", false,
                                        () =>
                                        {
                                            SpineManager.instance.DoAnimation(renwu, "gongbu", true);
                                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                                                () =>
                                                {
                                                    _return.SetActive(true);
                                                }
                                                ));
                                        });
                                }
                                );
                            
                        }
                        );
                }
                if (obj.name == "gaotaitui")
                {
                    SpineManager.instance.DoAnimation(clickBox, "b", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(clickBox, "cj", false,
                                () =>
                                {
                                    renwu.SetActive(true);
                                    SpineManager.instance.DoAnimation(renwu, "cx", false,
                                        () =>
                                        {
                                            SpineManager.instance.DoAnimation(renwu, "gaotaitui", true);
                                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,
                                                () =>
                                                {
                                                    _return.SetActive(true);
                                                }
                                                ));
                                        });
                                }
                                );

                        }
                        );
                }
                if (obj.name == "houtitui")
                {
                    SpineManager.instance.DoAnimation(clickBox, "a", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(clickBox, "cj", false,
                                () =>
                                {
                                    renwu.SetActive(true);
                                    SpineManager.instance.DoAnimation(renwu, "cx", false,
                                        () =>
                                        {
                                            SpineManager.instance.DoAnimation(renwu, "houtitui", true);
                                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                                                () =>
                                                {
                                                    _return.SetActive(true);
                                                }
                                                ));
                                        });
                                }
                                );


                        }
                        );
                }
            }
        }

        IEnumerator ieWaittime(float time,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
        private void GameInit()
        {
            _return.SetActive(false);
            _canReturn = true;
            clickBox.SetActive(true);
            renwu.SetActive(false);
            talkIndex = 1;
            clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(clickBox,"animation",false) ;

        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; _canClick = true; }));

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

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
