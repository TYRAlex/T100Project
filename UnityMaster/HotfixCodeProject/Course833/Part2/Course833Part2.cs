using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course833Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject startCar;
        private GameObject endCar;
        private GameObject endOtherCar;
        bool isPlaying = false;

        private GameObject btnTest;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }
        void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            startCar = curTrans.Find("startCar").gameObject;
            startCar.SetActive(true);
            endCar = curTrans.Find("endCar").gameObject;
            endCar.SetActive(false);
            SpineManager.instance.DoAnimation(endCar, "yd2", false, () => { SpineManager.instance.SetFreeze(endCar, true); });
         

            endOtherCar = curTrans.Find("endOtherCar").gameObject;
            endOtherCar.SetActive(false);
            talkIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,() => {

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(startCar, "zzzzzzzz", false);
                }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }));
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                startCar.SetActive(false);
                endCar.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                SpineManager.instance.SetFreeze(endCar, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(endCar, "yd1", false, () =>
                    {
                        SpineManager.instance.DoAnimation(endCar, "3", false);
                    });
                }, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                    {
                        endCar.SetActive(false);
                        endOtherCar.SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        SpineManager.instance.DoAnimation(endOtherCar, "yl1", false, () => {
                            SpineManager.instance.DoAnimation(endOtherCar, "yl2", false);
                            //endOtherCar.transform.DOMove(endOtherCar.transform.position, 1f).OnComplete(() =>
                            //{
                            //    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            //    SpineManager.instance.DoAnimation(endOtherCar, "yl2", false, () =>
                            //    {
                            //        SpineManager.instance.DoAnimation(endOtherCar, "yl3", false);
                            //    });
                            //});
                        });

                    }));
                }));
            }
            if (talkIndex == 3)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
