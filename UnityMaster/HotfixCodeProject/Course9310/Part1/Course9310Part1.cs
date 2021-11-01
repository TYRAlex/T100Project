using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9310Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private Transform panelSun;
        private GameObject sunObj;
        private Transform panelShow;
        private GameObject lyjObj;
        private Transform ykqTran;

        private Image mask;


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

            bell = curTrans.Find("bell").gameObject;
            panelSun = curTrans.Find("panelSun");
            sunObj = panelSun.Find("ty").gameObject;
            panelShow = curTrans.Find("panelShow");
            lyjObj = panelShow.Find("lyj").gameObject;
            ykqTran = panelShow.Find("ykq");
            for (int i = 0; i < ykqTran.childCount; i++)
            {
                ykqTran.GetChild(i).gameObject.SetActive(true);
                Util.AddBtnClick(ykqTran.GetChild(i).gameObject, OnClickYKQ);
            }
            mask = curTrans.Find("mask").GetImage();

            mask.gameObject.SetActive(false);
            mask.CrossFadeAlpha(0f, 0f, false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        string temAniName = "";
        private void OnClickYKQ(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            int temIndex = obj.transform.GetSiblingIndex();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            if (temIndex == 1 && temAniName == "2")
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,5);
                SpineManager.instance.DoAnimation(ykqTran.gameObject, obj.name + "c", false, () => { isPlaying = false; });
                return;
            }
            if (temIndex == 0 && temAniName == "3")
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                SpineManager.instance.DoAnimation(ykqTran.gameObject, obj.name + "c", false, () => { isPlaying = false; SoundManager.instance.ShowVoiceBtn(true); });
                return;
            }
            if (temIndex == 0)
            {
                temAniName = "3";
            }
            else
            {
                temAniName = "2";
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            SpineManager.instance.DoAnimation(ykqTran.gameObject, obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(lyjObj.gameObject, lyjObj.name + temAniName, false,
                  () =>
                  {
                      if (temIndex == 0)
                      {
                          SpineManager.instance.DoAnimation(lyjObj, lyjObj.name, true);
                      }
                      else
                      {
                          SpineManager.instance.DoAnimation(lyjObj, lyjObj.name+6, true);
                      }
                    
                      isPlaying = false;
                      if (temAniName == "3")
                      {
                          SoundManager.instance.ShowVoiceBtn(true);
                      }
                  });
            });

        }

        private void GameInit()
        {
            isPlaying = false;
            talkIndex = 1;
            SpineManager.instance.DoAnimation(sunObj, "kong", false);
            lyjObj.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(lyjObj, lyjObj.name, true);
            temAniName = "3";
            SpineManager.instance.DoAnimation(ykqTran.gameObject, "kong", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            bell.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

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
                speaker = bell;
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
            isPlaying = true;
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, () => { SpineManager.instance.DoAnimation(ykqTran.gameObject, ykqTran.name, false); }, () => { bell.SetActive(false); isPlaying = false; }));
            }
            if (talkIndex == 2)
            {
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    isPlaying = false;
                }));
            }
            if (talkIndex == 3)
            {
                SpineManager.instance.DoAnimation(ykqTran.gameObject, "kong", false);
                for (int i = 0; i < ykqTran.childCount; i++)
                {
                    ykqTran.GetChild(i).gameObject.SetActive(false);
                }
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        SpineManager.instance.DoAnimation(sunObj, sunObj.name, false,
                           () =>
                           {
                               SpineManager.instance.DoAnimation(sunObj, sunObj.name + 2, false,
                                   () =>
                                   {
                                       SpineManager.instance.DoAnimation(lyjObj.gameObject, lyjObj.name + 4, false,
                                           () =>
                                           {
                                               SpineManager.instance.DoAnimation(lyjObj.gameObject, lyjObj.name + 7, true);
                                               SoundManager.instance.ShowVoiceBtn(true);
                                               isPlaying = false;
                                           });
                                   });
                           });
                    },
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                        isPlaying = false;
                    }));

            }
            if (talkIndex == 4)
            {

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4,
                    () =>
                    {
                        mask.gameObject.SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        float temf = SpineManager.instance.DoAnimation(sunObj.gameObject, sunObj.name + 3, false, () => { SpineManager.instance.DoAnimation(lyjObj.gameObject, lyjObj.name + 5, false, () => { SpineManager.instance.DoAnimation(lyjObj, lyjObj.name, true); }); });
                        mask.CrossFadeAlpha(0.6f, temf, false);
                    }, null
                   ));



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
