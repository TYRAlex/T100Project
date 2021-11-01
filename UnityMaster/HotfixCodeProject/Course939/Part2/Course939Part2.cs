using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course939Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private GameObject drivingBg;
        private BellSprites bellTextures;

        private GameObject bell;
        private Transform panel;
        private Transform hb;
        private GameObject spineShow;
        private Transform showPanel;
        private GameObject btnBack;
        bool isPlaying = false;
        bool isEnd = false;
        int flag = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            drivingBg = curTrans.Find("drivingBg").gameObject;          
            bell = curTrans.Find("bell").gameObject;
            panel = curTrans.Find("panel");
            hb = panel.Find("hb");
            for (int i = 0; i < hb.childCount; i++)
            {
                Util.AddBtnClick(hb.GetChild(i).gameObject, OnClickBtn);
            }
            spineShow = panel.Find("spineShow").gameObject;

            showPanel = curTrans.Find("showPanel");

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtnBack(GameObject obj)
        {
            drivingBg.transform.localScale = Vector3.one * 2;
            drivingBg.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            spineShow.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(hb.gameObject, hb.name, false,
                () =>
                {
                    if (flag >= Mathf.Pow(2, hb.childCount) - 1 && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    obj.SetActive(false); isPlaying = false;
                });


        }

        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            int temIndex = obj.transform.GetSiblingIndex();
            if ((flag & (1 << temIndex)) == 0)
            {
                flag += (1 << temIndex);
            }

            SpineManager.instance.DoAnimation(hb.gameObject, obj.name, false,
                () =>
                {
                    hb.GetComponent<SkeletonGraphic>().Initialize(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex, false);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, temIndex + 1,
                      () =>
                      {
                          Bg.GetComponent<RawImage>().texture = bellTextures.texture[temIndex + 1];
                          if (obj.name == "hbd1")
                          {                              
                              SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "shache", false,
                               () =>
                               {
                                   SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "shache2", false);

                               });

                          }
                          else if (obj.name == "hbd4")
                          {
                              SpineManager.instance.DoAnimation(spineShow, temIndex + 2 + "", false,
                                  () =>
                                  {
                                      Bg.GetComponent<RawImage>().texture = bellTextures.texture[temIndex + 2];
                                  });
                          }
                          else if (obj.name == "hbd2")
                          {
                              drivingBg.SetActive(true);
                              drivingBg.transform.DOScale(Vector3.one, 3f).SetEase(Ease.OutCirc).SetLoops(-1);
                              SpineManager.instance.DoAnimation(spineShow, temIndex + 2 + "", true);
                          }
                          else
                          {
                              SpineManager.instance.DoAnimation(spineShow, temIndex + 2 + "", false);
                          }

                      }, () =>
                      {
                          if (temIndex != 0)
                          {
                              btnBack.SetActive(true);
                          }
                          else
                          {
                              mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "kong", false);
                                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                                    SpineManager.instance.DoAnimation(spineShow, temIndex + 2 + "", false, () => { });
                                }, () => { btnBack.SetActive(true); }));
                          }
                      }));

                });


        }

        private void GameInit()
        {
            talkIndex = 1;
            flag = 0;
            isEnd = false;
            btnBack.SetActive(false);
            drivingBg.transform.localScale = Vector3.one * 2;
            drivingBg.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            hb.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(hb.gameObject, "kong", false);
            spineShow.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(spineShow, "kong", false);
            showPanel.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "kong", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bell.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "1", false);
                }, () => { SoundManager.instance.ShowVoiceBtn(true); }));

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
            if (talkIndex == 1)
            {
                SpineManager.instance.DoAnimation(showPanel.GetChild(0).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(hb.gameObject, hb.name, false, () => { bell.SetActive(false); isPlaying = false; });

            }
            if (talkIndex == 2)
            {
                bell.SetActive(true);
                isPlaying = true;
                isEnd = true;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
