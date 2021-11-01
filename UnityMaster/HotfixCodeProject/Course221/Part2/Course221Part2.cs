using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course221Part2
    {
        private GameObject bell;
        private GameObject problem;
        private GameObject img_1;
        private GameObject img_2;
        private GameObject img_3;
        private GameObject[] thrust;

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("Bell/BellSpine").gameObject;
            problem = curTrans.Find("Problem").gameObject;
            img_1 = curTrans.Find("Img_1").gameObject;
            img_2 = curTrans.Find("Img_2").gameObject;
            img_3 = curTrans.Find("Img_3").gameObject;

            GameObject img = img_2.transform.Find("Img_2").gameObject;
            thrust = new GameObject[img.transform.childCount];
            for(int i = 0; i < thrust.Length; i++)
            {
                thrust[i] = img.transform.GetChild(i).gameObject;
            }

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;

            bell.SetActive(false);
            img_1.SetActive(false);
            img_2.SetActive(false);
            img_3.SetActive(false);
            problem.transform.GetComponent<Image>().CrossFadeAlpha(0,0,true);
            for (int i = 0; i < thrust.Length; i++) thrust[i].GetComponent<Image>().CrossFadeAlpha(0, 0, true);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                problem.transform.GetComponent<Image>().CrossFadeAlpha(1, 1.5f, true);
            }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {
                problem.SetActive(false);
                img_1.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }else if(talkIndex == 2)
            {
                mono.StartCoroutine(Talk_2_Coroutine());
            }else if(talkIndex == 3)
            {
                img_1.SetActive(false);
                img_2.SetActive(false);
                problem.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }else if(talkIndex == 4)
            {
                problem.SetActive(false);
                img_3.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () => { }));
            }
            talkIndex++;
        }

        IEnumerator Talk_2_Coroutine()
        {
            img_2.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            yield return new WaitForSeconds(2.5f);
            thrust[0].transform.GetComponent<Image>().CrossFadeAlpha(1,1,true);
            yield return new WaitForSeconds(1.5f);
            thrust[1].transform.GetComponent<Image>().CrossFadeAlpha(1, 1, true);
            yield return new WaitForSeconds(1.5f);
            thrust[2].transform.GetComponent<Image>().CrossFadeAlpha(1, 1, true);
        }


        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}
