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
    public class Course9111Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject jqSpine;

        private GameObject btnBack;

        bool isPlaying = false;
        bool isTalked = false;
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
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();


            jqSpine = curTrans.Find("jqSpine").gameObject;
            jqSpine.SetActive(true);
            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            for (int i = 0; i < jqSpine.transform.childCount; i++)
            {
                Util.AddBtnClick(jqSpine.transform.GetChild(i).gameObject, OnClickBtn);
            }
            Util.AddBtnClick(btnBack, OnClickBtnBack);

            talkIndex = 1;
            isTalked = false;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }
        string curSpine = "";
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SpineManager.instance.DoAnimation(jqSpine, curSpine + "2", false, () =>
            {
                btnBack.SetActive(false); isPlaying = false; 
                if (!isTalked)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }  });
        }

        private void OnClickBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            curSpine = obj.name;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex()+1, () => { SpineManager.instance.DoAnimation(jqSpine, curSpine + "1", false); }, () => { btnBack.SetActive(true); isPlaying = false; }));
          
        }
        void GameStart()
        {
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { SpineManager.instance.DoAnimation(jqSpine, "animation", false); }, () => { isPlaying = false; }));



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
            method_1?.Invoke();
            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                isTalked = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,()=> { isPlaying = true; }, () => { isPlaying = false; }));
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
