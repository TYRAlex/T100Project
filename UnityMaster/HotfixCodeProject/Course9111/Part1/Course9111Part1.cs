using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course9111Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private Transform Bg;

        private GameObject btnTest;
        private GameObject ndjSpine;
        private Transform ndjBtn;

        private GameObject ndjSpineShow;

        bool isPlaying = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg");

            ndjSpine = curTrans.Find("ndjSpine").gameObject;
            ndjBtn = curTrans.Find("ndjBtn");
            ndjSpineShow = curTrans.Find("ndjSpineShow").gameObject;
            for (int i = 0; i < ndjBtn.childCount; i++)
            {
                Util.AddBtnClick(ndjBtn.GetChild(i).gameObject, OnClickBtn);
            }

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }
        string flag = "";
        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            string tem = "";

            tem = obj.transform.GetSiblingIndex() + 1 + "";
            if (flag != tem)
            {
                flag = tem;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, obj.transform.GetSiblingIndex() + 1, false);
            }
            else
            {
                tem = tem + tem;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, obj.transform.GetSiblingIndex() + 4, false);
            }
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 2, () =>
            {
                SpineManager.instance.DoAnimation(ndjBtn.gameObject, obj.name, false, () => { SpineManager.instance.DoAnimation(ndjSpineShow, "jt" + tem, false); });
            }, () =>
            {
                isPlaying = false;
            }));
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                isPlaying = true;
                SpineManager.instance.DoAnimation(ndjSpine, "jing", false);
                SpineManager.instance.DoAnimation(ndjBtn.gameObject, "kong", false);
                SpineManager.instance.DoAnimation(ndjSpineShow, "kong", false);
            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
           
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind- len);
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
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                {
                    SpineManager.instance.SetTimeScale(ndjBtn.gameObject,0.25f);
                    SpineManager.instance.DoAnimation(ndjBtn.gameObject, "animation", false);
                }, () => { isPlaying = false; SpineManager.instance.SetTimeScale(ndjBtn.gameObject, 1); },2.5f));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}
