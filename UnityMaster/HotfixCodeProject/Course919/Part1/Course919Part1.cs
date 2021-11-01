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
    public class Course919Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject max;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject jgSpine;
        private GameObject btnBack;

        private Transform maxStartPos;
        private Transform maxEndPos;


        bool isPlaying = false;

        int time = 0;
        string spineName = "";
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
            max = curTrans.Find("max").gameObject;
            max.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            jgSpine = curTrans.Find("jgSpine").gameObject;
            jgSpine.SetActive(true);
            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBack);
            btnBack.SetActive(false);
            for (int i = 0; i < jgSpine.transform.childCount; i++)
            {
                Util.AddBtnClick(jgSpine.transform.GetChild(i).gameObject, OnClick);
            }

            maxStartPos = curTrans.Find("maxStartPos");
            max.transform.position = maxStartPos.position;
            maxEndPos = curTrans.Find("maxEndPos");
            maxEndPos.GetChild(0).gameObject.SetActive(false);
            time = 0;
            spineName = "";
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        private void OnClickBack(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            SpineManager.instance.DoAnimation(jgSpine, spineName + "3", false, () =>
            {
                btnBack.SetActive(false); isPlaying = false;
                if (time >= 3)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            });

        }

        private void OnClick(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            spineName = obj.name;
            SoundManager.instance.ShowVoiceBtn(false);
            time++;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, obj.transform.GetSiblingIndex(), false);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, () =>
            {
                SpineManager.instance.DoAnimation(jgSpine, spineName, false, () => { SpineManager.instance.DoAnimation(jgSpine, spineName + "2", false); });
            }, () =>
            {
                btnBack.SetActive(true); isPlaying = false;

            }));
        }

        void GameStart()
        {
            isPlaying = true;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 0, true);
            SpineManager.instance.DoAnimation(jgSpine, "kong2", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
        {

            SpineManager.instance.DoAnimation(jgSpine, "animation", false,
() => { btnBack.SetActive(false); });


        }, () => { max.SetActive(false); isPlaying = false; }));
            });
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                maxEndPos.GetChild(0).gameObject.SetActive(true);
                max.transform.position = maxEndPos.position;
                max.SetActive(true);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                jgSpine.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4));
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
