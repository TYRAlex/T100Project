using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course7312Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spine0;
        private GameObject _bg0;
        private GameObject _spine1;
        private GameObject _spine2;
        private GameObject _kuang;

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

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, true);
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;

            _bg0 = curTrans.Find("Bg/0").gameObject;
            _bg0.Show();

            _spine0 = curTrans.Find("spineManager/0").gameObject;
            _spine0.Show();

            _spine1 = curTrans.Find("spineManager/1").gameObject;
            _spine1.Show();
            _spine1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine1, "kong", false);

            _spine2 = curTrans.Find("spineManager/2").gameObject;
            _spine2.Show();
            _spine2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine2, "kong", false);

            _kuang = curTrans.Find("spineManager/kuang").gameObject;
            _kuang.Show();
            _kuang.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_kuang, "kong", false);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine0, "daiji", true);                  //流程1
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                isPlaying = false;
                SoundManager.instance.ShowVoiceBtn(true);
            }));

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
        IEnumerator WariteCoroutine(Action method_2 = null, float len = 0)
        {
            
            yield return new WaitForSeconds(len);           
            method_2?.Invoke();
        }


        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _bg0.Hide();
                SpineManager.instance.DoAnimation(_spine0, "chache", false);                //流程2
                SpineManager.instance.DoAnimation(_kuang, "kuang", false);
                mono.StartCoroutine(WariteCoroutine(() => 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                    SpineManager.instance.DoAnimation(_spine1, "chachezi1", false,()=>
                    {
                        //SpineManager.instance.DoAnimation(_spine1, "kong", false);
                    });

                    mono.StartCoroutine(WariteCoroutine(() => 
                    {
                        _spine2.Show();
                        _spine2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);                        
                        SpineManager.instance.DoAnimation(_spine2, "chachezi2", false,()=> 
                        {
                            //SpineManager.instance.DoAnimation(_spine2, "kong", false);
                        });
                    }, 1.2f));
                }, 1));                
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {                   
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 2)
            {
                //流程3    
                mono.StartCoroutine(WariteCoroutine(() => 
                {
                    SpineManager.instance.DoAnimation(_spine1, "kong", false);
                    SpineManager.instance.DoAnimation(_spine2, "kong", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, false);
                    SpineManager.instance.DoAnimation(_spine0, "fj", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                        SpineManager.instance.DoAnimation(_spine1, "fjz1", false, () =>
                        {
                            SpineManager.instance.DoAnimation(_spine1, "fjz2", false);
                        });
                    });
                },5.0f));
                                          
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 3)
            {
                SpineManager.instance.DoAnimation(_spine2, "kong", false);
                SpineManager.instance.DoAnimation(_spine0, "kong", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, false);
                SpineManager.instance.DoAnimation(_spine1, "fjz3", false,()=> 
                {
                    SpineManager.instance.DoAnimation(_spine1, "chezheng", false, () =>
                    {
                        mono.StartCoroutine(WariteCoroutine(() =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                            SpineManager.instance.DoAnimation(_spine1, "chezheng2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(_spine1, "chezheng3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(_spine1, "chezheng4", false);
                                });
                            });
                        }, 5.0f));
                    });//流程4      
                });                                                                                                     
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, null));
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
