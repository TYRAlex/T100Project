using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course213Part2
    {
        private GameObject max;
        private GameObject problem;
        private GameObject proBtn;
        private GameObject b1;
        private GameObject b2;
        private GameObject gly;
        private GameObject imgBtn;
        private GameObject backBtn;
        private GameObject bg2;
        private GameObject bg3;

        private GameObject maxSpeak;
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            max = curTrans.Find("bg2/max").gameObject;
            maxSpeak = curTrans.Find("max").gameObject;
            problem = curTrans.Find("problem").gameObject;
            proBtn = curTrans.Find("ImgBtn/ProBtn").gameObject;
            b1 = curTrans.Find("Spine/b1").gameObject;
            b2 = curTrans.Find("Spine/b2").gameObject;
            gly = curTrans.Find("Spine/gly").gameObject;
            imgBtn = curTrans.Find("ImgBtn/ImgBtn").gameObject;
            backBtn = curTrans.Find("ImgBtn/BackBtn").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bg3 = curTrans.Find("bg3").gameObject;

            Button[] btn = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < btn.Length; i++)
            {
                Util.AddBtnClick(btn[i].gameObject, DoImgBtnClick);
            }
            Util.AddBtnClick(proBtn, DoProBtnClick);
            Util.AddBtnClick(backBtn, DoBackBtnClick);
            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            max.SetActive(false);
            maxSpeak.SetActive(true);
            b1.SetActive(true);
            SpineManager.instance.DoAnimation(b1, "kong", false);
            b2.SetActive(true);
            SpineManager.instance.DoAnimation(b2, "kong", false);
            gly.SetActive(false);
            problem.SetActive(true);
            proBtn.SetActive(false);
            imgBtn.SetActive(false);
            backBtn.SetActive(false);
            bg2.SetActive(false);
            bg3.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            problem.transform.localScale = new Vector3(0, 0.5f, 0);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            problem.transform.DOScale(new Vector3(0.5f, 0.5f, 0),1);
            mono.StartCoroutine(MaxSpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                proBtn.SetActive(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        void DoProBtnClick(GameObject obj)
        {
            proBtn.SetActive(false);
            problem.SetActive(false);
            b2.SetActive(true);
            mono.StartCoroutine(MaxSpeckerCoroutine(SoundManager.SoundType.VOICE, 1, 
            () => 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(b2, "1", false);
            }, 
            () => 
            {
                imgBtn.SetActive(true);
                maxSpeak.SetActive(false);
            }));
        }

        void DoImgBtnClick(GameObject obj)
        {
            int idx = int.Parse(obj.name);
            imgBtn.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            mono.StartCoroutine(ImgBtnCoroutine(idx));
        }

        IEnumerator ImgBtnCoroutine(int idx)
        {
            float len = SpineManager.instance.DoAnimation(b2, "" + (idx + 3), false);
            yield return new WaitForSeconds(len);
            if(idx != 2)
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, idx + 1, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx+2, () => { }, () => {
                backBtn.SetActive(true);
                b1.SetActive(false);
                if (idx != 0 && idx != 1) SpineManager.instance.DoAnimation(gly, "daiji", true); 
            }));
            if (idx == 0 || idx == 1)
            {
                bg2.SetActive(true);
                max.SetActive(true);
                b1.SetActive(true);
                SpineManager.instance.DoAnimation(max, "daijishuohua");
                if(idx == 0)
                {
                    yield return new WaitForSeconds(3);
                    SpineManager.instance.DoAnimation(b1, "animation3", false);
                    yield return new WaitForSeconds(4);
                    SpineManager.instance.DoAnimation(b1, "animation", false);
                }
                else
                {
                    yield return new WaitForSeconds(2.5f);
                    float len2 = SpineManager.instance.DoAnimation(b1, "animation2", false);
                    yield return new WaitForSeconds(len2-0.3f);
                    SpineManager.instance.DoAnimation(b1, "animation6", false);
                    yield return new WaitForSeconds(4);
                    SpineManager.instance.DoAnimation(b1, "animation5", false);
                }
            }
            else
            {
                bg3.SetActive(true);
                gly.SetActive(true);
                SpineManager.instance.DoAnimation(gly, "daijishuohua", true);
            }
        }

        void DoBackBtnClick(GameObject obj)
        {
            backBtn.SetActive(false);
            bg2.SetActive(false);
            gly.SetActive(false);
            bg3.SetActive(false);
            SpineManager.instance.DoAnimation(b2, "6", false, () => {
                imgBtn.SetActive(true);
            });
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(max, "daiji");
            if (method_2 != null)
            {
                method_2();
            }
        }

        IEnumerator MaxSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(maxSpeak, "daijishuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(maxSpeak, "daiji");
            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}
