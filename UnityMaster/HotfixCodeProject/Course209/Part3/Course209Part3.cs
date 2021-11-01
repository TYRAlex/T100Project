using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course209Part3
    {
        private GameObject bell;
        private GameObject bell_1;
        private int talkIndex;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell_1 = curTrans.Find("bg/bell_1").gameObject;
            bell = curTrans.Find("bg/bell").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameStart();
        }

        void GameStart()
        {
            talkIndex = 1;
            bell.SetActive(true);
            bell_1.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 2)
            {
                bell_1.SetActive(true);
                bell.SetActive(false);
                float len = SpineManager.instance.DoAnimation(bell_1, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => {
                    bell.SetActive(true);
                    bell_1.SetActive(false);
                }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                },len));
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () => { }));
            }
            talkIndex++;
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
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

