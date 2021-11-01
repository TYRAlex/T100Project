using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course220Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        GameObject process0;
        GameObject process1;
        GameObject mask;
        GameObject btn;
        GameObject ppt;

        GameObject bell;

        GameObject hsqzj;
        GameObject di;
        GameObject btnShowStruct;
        GameObject structImg;
        GameObject btnHideStruct;

        SkeletonGraphic skeletonG;
        bool notFirst = false;

        bool isPlaying = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;

            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            process0 = curTrans.Find("process0").gameObject;
            process0.SetActive(true);
            process1 = curTrans.Find("process1").gameObject;
            process1.SetActive(false);

            btn = curTrans.Find("process0/btn").gameObject;
            btn.SetActive(false);
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);
            ppt = curTrans.Find("process0/ppt").gameObject;
            ppt.SetActive(true);
            SpineManager.instance.DoAnimation(ppt, "kong", false);
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            hsqzj = curTrans.Find("process1/hsqzj").gameObject;
            hsqzj.transform.GetChild(0).gameObject.SetActive(true);
            hsqzj.SetActive(true);
            skeletonG = hsqzj.GetComponent<SkeletonGraphic>();
            skeletonG.raycastTarget = false;
            btnShowStruct = curTrans.Find("process1/btnShowStruct").gameObject;
            btnShowStruct.SetActive(false);
            structImg = curTrans.Find("process1/structImg").gameObject;
            structImg.SetActive(false);

            btnHideStruct = curTrans.Find("process1/structImg/btnHideStruct").gameObject;
            btnHideStruct.SetActive(false);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }


        void GameStart()
        {
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { mono.StartCoroutine(pptPlay()); }, () => { btn.SetActive(true); isPlaying = false; }, 2));
            Util.AddBtnClick(btn, btnClick);
            Util.AddBtnClick(hsqzj, btnQZJClick);
            Util.AddBtnClick(btnShowStruct, btnClickPlayStructImg);
            Util.AddBtnClick(btnHideStruct, btnClickHideStructImg);
        }

        private void btnClickHideStructImg(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            notFirst = true;
            SpineManager.instance.DoAnimation(structImg, "animation3", false, () =>
            {
                structImg.SetActive(false);
                hsqzj.SetActive(true);
                btnHideStruct.SetActive(false);
                mask.SetActive(false);
                SpineManager.instance.DoAnimation(hsqzj, "3", false, () => { btnShowStruct.SetActive(true); isPlaying = false; });
            });
        }

        void btnQZJClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            //TODO 播放语音
            //bell.SetActive(false);
            skeletonG.raycastTarget = false;
            di.SetActive(true);
            hsqzj.transform.GetChild(0).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(hsqzj, "3", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    SpineManager.instance.DoAnimation(hsqzj, "1", false, () =>
                    {
                        isPlaying = false;
                        SoundManager.instance.ShowVoiceBtn(true);
                    });
                }, null, 2));
            }
           );
        }



        void btnClickPlayStructImg(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            btnShowStruct.SetActive(false);
            hsqzj.SetActive(false);
            structImg.SetActive(true);
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(structImg, "animation2", false, () =>
            {

                if (notFirst)
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(structImg, "animation", false);
                        },
                        () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                     () =>
                     {
                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                         SpineManager.instance.DoAnimation(structImg, "s1", false);
                     },
                     () =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7,
                     () =>
                     {
                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                         SpineManager.instance.DoAnimation(structImg, "s2", false);
                     },
                     () =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8,
                     () =>
                     {
                         SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 5,()=> { SoundManager.instance.skipBtn.SetActive(false); },()=> { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5); });
                         SpineManager.instance.DoAnimation(structImg, "s3", false, () => { SpineManager.instance.DoAnimation(structImg, "s4", false); });
                     },
                     () =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9,
                () =>
                {

                },
                () => { btnHideStruct.SetActive(true); isPlaying = false; }));
                     }));
                     }));
                     }));
                        }));
                }
                else
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                    {
                        mono.StartCoroutine(WaitExe(() =>
                       {
                           isPlaying = false;
                           SoundManager.instance.ShowVoiceBtn(true);
                       }, 2));
                    }, 2));
                }

            });

        }
        void btnClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(ppt, "syg", false, () =>
            {
                SpineManager.instance.DoAnimation(ppt, "kong", false, () =>
                {
                    mask.SetActive(false);
                    process0.SetActive(false);
                    process1.SetActive(true);
                    SpineManager.instance.DoAnimation(hsqzj, "animation", false, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                        {
                            skeletonG.raycastTarget = true;
                            isPlaying = false;
                        }, 2));
                    });
                });
            });
        }

        IEnumerator pptPlay()
        {
            float temSecond = 0;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            for (int i = 0; i < 5; i++)
            {
                temSecond = SpineManager.instance.DoAnimation(ppt, i > 0 ? "animation" + (i + 1) : "animation", false);
                yield return new WaitForSeconds(temSecond + 1.5f);
                SpineManager.instance.DoAnimation(ppt, "j" + (i + 1), false);
            }

        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                SpineManager.instance.DoAnimation(bell, "DAIJI");
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            if (method_2 != null)
            {
                method_2();
            }
        }

        IEnumerator WaitExe(Action method_1 = null, float len = 0)
        {

            yield return new WaitForSeconds(len);

            if (method_1 != null)
            {
                method_1();
            }
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () => { btnShowStruct.SetActive(true); }, 0));
                //TODO 播放语音
            }
            else if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                         () =>
                         {
                             SpineManager.instance.DoAnimation(structImg, "animation", false);
                         },
                         () =>
                         {
                             mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                          SpineManager.instance.DoAnimation(structImg, "s1", false);
                      },
                      () =>
                      {
                          mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                          SpineManager.instance.DoAnimation(structImg, "s2", false);
                      },
                      () =>
                      {
                          mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8,
                      () =>
                      {
                          SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 5, () => { SoundManager.instance.skipBtn.SetActive(false); }, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5); });
                          SpineManager.instance.DoAnimation(structImg, "s3", false,()=> { SpineManager.instance.DoAnimation(structImg, "s4", false); });
                      },
                      () =>
                      {
                          mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9,
                 () =>
                 {
                    
                 },
                 () => { btnHideStruct.SetActive(true); isPlaying = false; }));
                      }));
                      }));
                      }));
                         }));
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, null, () => { btnHideStruct.SetActive(true); }, 0));
            }
            talkIndex++;
        }

    }
}
