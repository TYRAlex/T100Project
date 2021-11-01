using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course939Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;
        private Transform panel;
        private GameObject spineShow;
        private Transform btnLast;
        private Transform btnNext;
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

            panel = curTrans.Find("panel");
            spineShow = panel.Find("spineShow").gameObject;
            spineShow.SetActive(true);
            btnNext = panel.Find("b");
            Util.AddBtnClick(btnNext.GetChild(0).gameObject, OnClickBtnNext);
            btnNext.gameObject.SetActive(false);
            btnLast = panel.Find("a");
            Util.AddBtnClick(btnLast.GetChild(0).gameObject, OnClickBtnLast);
            btnLast.gameObject.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtnLast(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(btnLast.gameObject, obj.name, false,
                () =>
                {
                    if (talkIndex == 3)
                    {
                        SpineManager.instance.DoAnimation(spineShow, "jing3", false, () => { isPlaying = false; SoundManager.instance.ShowVoiceBtn(true); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(spineShow, "hj3", false, () => { isPlaying = false; });
                    }
                    btnNext.gameObject.SetActive(true);
                    btnLast.gameObject.SetActive(false);
                });
        }

        private void OnClickBtnNext(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(btnNext.gameObject, obj.name, false,
                () =>
                {
                    if (talkIndex == 3)
                    {
                        SpineManager.instance.DoAnimation(spineShow, "jing4", false, () => { isPlaying = false; SoundManager.instance.ShowVoiceBtn(true); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(spineShow, "hj4", false, () => { isPlaying = false; });
                    }
                    btnNext.gameObject.SetActive(false);
                    btnLast.gameObject.SetActive(true);
                });

        }

        private void GameInit()
        {
            talkIndex = 1;
            spineShow.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(spineShow, "jing1", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            bell.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null,
                () =>
                {
                    bell.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, () => { SpineManager.instance.DoAnimation(spineShow, "animation2", false); }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        SpineManager.instance.DoAnimation(spineShow, "tuxian1", false, () => { SpineManager.instance.DoAnimation(spineShow, "tuxian2", false); });
                    }, () => { bell.SetActive(false); SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                SpineManager.instance.DoAnimation(spineShow, "jing3", false, () => { btnNext.gameObject.SetActive(true); isPlaying = false; });

            }

            if (talkIndex == 3)
            {
                btnLast.gameObject.SetActive(false);
                btnNext.gameObject.SetActive(false);
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(spineShow, "jing1", false);
                    },
                    () =>
                    {
                        bell.SetActive(false);
                        mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4,
                     () =>
                     {
                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                         SpineManager.instance.DoAnimation(spineShow, "hj1", false);
                     }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    }));
            }
            if (talkIndex == 4)
            {
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(spineShow, "hj2", false);
                    }, () => { bell.SetActive(false); SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 5)
            {
                SpineManager.instance.DoAnimation(spineShow, "hj3", false, () => { btnNext.gameObject.SetActive(true); });
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
