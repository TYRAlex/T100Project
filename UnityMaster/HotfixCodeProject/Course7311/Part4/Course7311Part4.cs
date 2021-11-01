using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course7311Part4
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
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            show.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(show, "1", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            Max.SetActive(true);
            isPlaying = true;
            Delay(5f,()=> { SpineManager.instance.DoAnimation(show,"2",false); });
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                Delay(1f, () => 
                {
                    SpineManager.instance.DoAnimation(show, "5", false);
                    MaxSpeak(1,
                        () => 
                        {
                            SpineManager.instance.DoAnimation(show, "7", false);
                            MaxSpeak(2,()=> 
                            {
                                Delay(1f, () => 
                                {
                                    SpineManager.instance.DoAnimation(show, "8", false);
                                    MaxSpeak(3, () => 
                                    {
                                        SpineManager.instance.DoAnimation(show, "9", false);
                                        MaxSpeak(4);
                                        Delay(4f, () => { SpineManager.instance.DoAnimation(show, "11", false,
                                            () => 
                                            {
                                                SpineManager.instance.DoAnimation(show, "13", false,
                                                    () => 
                                                    {
                                                        Delay(1f, ()=> 
                                                        {
                                                            SpineManager.instance.DoAnimation(show, "15", false,
                                                            () =>
                                                            {
                                                                Delay(1f, () => 
                                                                {
                                                                    SpineManager.instance.DoAnimation(show, "17", false,
                                                                    () =>
                                                                    {
                                                                        Delay(2f, ()=> 
                                                                        {
                                                                            SpineManager.instance.DoAnimation(show, "19", false,
                                                                            () =>
                                                                            {
                                                                                Delay(1f,()=> { SpineManager.instance.DoAnimation(show, "21", false); });
                                                                            }
                                                                            );
                                                                        });
                                                                    }
                                                                    );
                                                                });
                                                            }
                                                            );
                                                        });
                                                    }
                                                    );
                                            }

                                              ); });
                                    });
                                });
                            });
                        }
                        );
                });
            }));

        }


        private void MaxSpeak(int index, Action callback = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, null, callback));
        }
        private void Delay(float time, Action callback = null)
        {
            mono.StartCoroutine(Wait(time, callback));
        }
        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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
