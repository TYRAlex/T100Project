using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course209Part2
    {
        private GameObject bell;
        private GameObject bell_spine;
        private GameObject toushiche;
        private GameObject youxi2a;
        private GameObject imgBtn;
        private GameObject imgBtn_3;
        private GameObject youxi2b;
        private GameObject bg2;

        private GameObject backBtn;

        private int talkIndex;
        private Vector3 bellPos_1;
        private Vector3 bellPos_2;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("bell").gameObject;
            bell_spine = curTrans.Find("bell/bell").gameObject;
            toushiche = curTrans.Find("Spine/toushiche").gameObject;
            youxi2a = curTrans.Find("Spine/youxi2a").gameObject;
            youxi2b = curTrans.Find("Spine/youxi2b").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            bg2 = curTrans.Find("Spine/bg2").gameObject;
            imgBtn_3 = curTrans.transform.Find("ImgBtn_3").gameObject;
           
            backBtn = curTrans.Find("BackBtn").gameObject;

            GameObject imgBtn_1 = imgBtn.transform.Find("ImgBtn_1").gameObject;
            for (int i = 0; i < imgBtn_1.transform.childCount; i++)
            {
                GameObject obj = imgBtn_1.transform.GetChild(i).gameObject;
                obj.transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
                Util.AddBtnClick(obj, DoImgBtn_1Click);
            }
            GameObject imgBtn_2 = imgBtn.transform.Find("ImgBtn_2").gameObject;
            for (int i = 0; i < imgBtn_2.transform.childCount; i++)
            {
                GameObject obj = imgBtn_2.transform.GetChild(i).gameObject;
                obj.transform.GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
                Util.AddBtnClick(obj, DoImgBtn_2Click);
            }
            for (int i = 0; i < imgBtn_3.transform.childCount; i++)
            {
                GameObject obj = imgBtn_3.transform.GetChild(i).gameObject;
                Util.AddBtnClick(obj, DoImgBtn_3Click);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();

            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            bellPos_1 = new Vector3(-29, -490, 0);
            bellPos_2 = new Vector3(-711, -627, 0);
            bell.SetActive(true);

            toushiche.SetActive(false);
            youxi2a.SetActive(false);
            youxi2b.SetActive(false);
            imgBtn.SetActive(false);
            bg2.SetActive(false);
            imgBtn_3.SetActive(false);
            backBtn.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bell.transform.localPosition = bellPos_1;
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bell.transform.localPosition = bellPos_2;
                toushiche.SetActive(true);
                SpineManager.instance.DoAnimation(toushiche, "jingzhi", true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () => {
                    imgBtn.SetActive(true);
                }));
            }
            else if (talkIndex == 2)
            {
                imgBtn.SetActive(false);
                youxi2a.SetActive(true);
                float aniTime = SpineManager.instance.DoAnimation(youxi2a, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => {
                    SpineManager.instance.DoAnimation(youxi2a, "animation2", true);
                }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }, aniTime));
            }
            else if (talkIndex == 3)
            {
                bg2.SetActive(true);
                youxi2b.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => {
                    SpineManager.instance.DoAnimation(youxi2b, "animation", false);
                }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 4)
            {
                bell.SetActive(false);

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => {
                    SpineManager.instance.DoAnimation(youxi2b, "sxdj", false);
                }, () => {
                    imgBtn_3.SetActive(true);
                }));
            }
            else if (talkIndex == 5)
            {
                bg2.SetActive(false);
                bell.SetActive(true);
                youxi2a.SetActive(false);
                youxi2b.SetActive(false);
                imgBtn_3.SetActive(false);
                backBtn.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () => { }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 6)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () => { }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 7)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () => { }, () => {
                }));
            }
            talkIndex++;
        }

        void DoImgBtn_1Click(GameObject obj)
        {
            imgBtn.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => {
                mono.StartCoroutine(DoImgBtn_1Coroutine());
            }, () => {
                imgBtn.SetActive(true);
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        IEnumerator DoImgBtn_1Coroutine()
        {
            float aniTime = SpineManager.instance.DoAnimation(toushiche, "tougang", false);
            yield return new WaitForSeconds(1);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            yield return new WaitForSeconds(aniTime-1);
            aniTime = SpineManager.instance.DoAnimation(toushiche, "shitou", false);
            yield return new WaitForSeconds(0.2f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            yield return new WaitForSeconds(aniTime+0.3f);
            aniTime = SpineManager.instance.DoAnimation(toushiche, "02toushi", false);
            yield return new WaitForSeconds(aniTime);
        }

        void DoImgBtn_2Click(GameObject obj)
        {
            imgBtn.SetActive(false);
            SpineManager.instance.DoAnimation(toushiche, "zijia", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            }, () => {
                imgBtn.SetActive(true);
            },1));
        }

        void DoImgBtn_3Click(GameObject obj)
        {
            int index = int.Parse(obj.name);
            imgBtn_3.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            mono.StartCoroutine(DoImgBtn_3ClickCoroutine(index));
        }

        IEnumerator DoImgBtn_3ClickCoroutine(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            float aniTime = SpineManager.instance.DoAnimation(youxi2b, "sxd" + index, false);
            yield return new WaitForSeconds(aniTime);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            aniTime = SpineManager.instance.DoAnimation(youxi2b, "sxd" + index + "2", false);
            yield return new WaitForSeconds(aniTime);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6 + index, () => {
                SpineManager.instance.DoAnimation(youxi2b, "sxd" + index + "4", false);
            }, () => {
                backBtn.SetActive(true);
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void DoBackBtnClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SpineManager.instance.DoAnimation(youxi2b, "sxdj", false);
            imgBtn_3.SetActive(true);
            backBtn.SetActive(false);
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            SpineManager.instance.DoAnimation(bell_spine, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell_spine, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void OnDisable()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.Stop();
        }
    }
}
