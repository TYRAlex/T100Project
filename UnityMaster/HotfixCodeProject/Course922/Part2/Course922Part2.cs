using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course922Part2
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
        private bool[] _jugleClick;

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
            _jugleClick = new bool[3];
            GameInit();
            GameStart();

            for (int i = 0; i < 3; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject, ClickEvent);
            }

        }

        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                //obj.transform.parent.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
                Max.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "d" + obj.name, false,
                    () =>
                    {
                        curTrans.Find(obj.name).gameObject.SetActive(true);
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[Convert.ToInt32(obj.name)];
                        if(obj.name=="3")
                        {
                            curTrans.Find("32/31").gameObject.SetActive(true);
                            curTrans.Find("32").gameObject.SetActive(true);
                        }
                        
                        SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "cx", false,
                      () =>
                      {
                          if (obj.name == "1")
                          {
                              mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, Convert.ToInt32(obj.name), null,
                                  () =>
                                  {
                                      //curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                      SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "xs", false,
                              () =>
                              {
                                  curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                  Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                  SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                  SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "t" + obj.name, false,
                            () =>
                                  {
                                      SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "jing", false);
                                      
                                      _canClick = true;
                                      Max.SetActive(true);
                                      _jugleClick[0] = true;
                                      JugleClick();
                                      
                                  }
                            );
                              }
                              );

                                  }
                                  ));
                              SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,true);
                              //curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                              SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "2", true);
                              mono.StartCoroutine(WaitTime(10f,
                                  () =>
                                  {
                                      SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "1", true);
                                  }));
                          }

                          if (obj.name == "2")
                          {
                              mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, Convert.ToInt32(obj.name), null,
                                  () =>
                                  {
                                      //curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                      SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "xs", false,
                                     () =>
                                     {
                                         curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                         Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                         SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                         SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "t" + obj.name, false,
                                        () =>
                                        {
                                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "jing", false);
                                            _canClick = true;
                                            _jugleClick[1] = true;
                                            Max.SetActive(true);
                                            JugleClick();
                                        }
                                        );
                                     }
                                     );
                                  }
                                  ));
                              SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2,true);
                              //curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                              SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "1", true);
                          }

                          if (obj.name == "3")
                          {
                              mono.StartCoroutine(WaitTime(4f,
                                  ()=>
                                  { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3,false); }
                                  ));
                              
                              SpineManager.instance.DoAnimation(curTrans.Find("32/31").gameObject, "hld", false);
                              mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, Convert.ToInt32(obj.name), null,
                                  () =>
                                  {
                                      curTrans.Find(obj.name).gameObject.SetActive(false);
                                      curTrans.Find("32/31").gameObject.SetActive(false);
                                      curTrans.Find("32").gameObject.SetActive(false);
                                      Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                      SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "t" + obj.name, false,
                                     () =>
                                     {
                                         curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                         SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "jing", false);
                                         _canClick = true;
                                         _jugleClick[2] = true;
                                         Max.SetActive(true);
                                         JugleClick();
                                     }
                                     );
                                  }
                                  ));
                              //curTrans.Find(obj.name).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                              SpineManager.instance.DoAnimation(curTrans.Find(obj.name).gameObject, "1", true);
                          }


                      }
                      );
                    }
                    );
            }
        }

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
        private void JugleClick()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_jugleClick[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }



        private void GameInit()
        {
            clickBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true); 
            SpineManager.instance.DoAnimation(clickBox, "jing", false);
            _canClick = false;
            curTrans.Find("1").gameObject.SetActive(false);
            curTrans.Find("2").gameObject.SetActive(false);
            curTrans.Find("3").gameObject.SetActive(false);
            curTrans.Find("32/31").gameObject.SetActive(false);
            curTrans.Find("32").gameObject.SetActive(false);
            talkIndex = 1;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            curTrans.Find("1").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            curTrans.Find("2").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            curTrans.Find("3").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; isPlaying = false; }));

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
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
