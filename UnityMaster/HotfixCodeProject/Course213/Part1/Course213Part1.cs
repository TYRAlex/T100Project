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
    public class Course213Part1
    {
        private GameObject max;
        private GameObject problem;
        private GameObject proBtn;
        private GameObject imgBtn;
        private GameObject imgBtn_1;
        private GameObject a1;
        private GameObject a2;

        private int talkIndex;
        private bool isEnd;
        private int[] clipIndex;
        private string[] spiName;
        private string[] spi_1;
        private string[] spi_2;
        private string[] spi_3;

        private int[] _allClick1;
        private int[] _allClick2;
        private bool _isExit;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            max = curTrans.Find("max").gameObject;
            problem = curTrans.Find("problem").gameObject;
            proBtn = curTrans.Find("ImgBtn/proBtn").gameObject;
            a1 = curTrans.Find("a1").gameObject;
            a2 = curTrans.Find("a2").gameObject;
            imgBtn = curTrans.Find("ImgBtn/ImgBtn").gameObject;
            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;

            Util.AddBtnClick(proBtn, DoProBtnClick);
            Button[] imgChildBtn = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < imgChildBtn.Length; i++)
            {
                Util.AddBtnClick(imgChildBtn[i].gameObject, DoImgBtnClick);
            }
            Button[] imgChildBtn_1 = imgBtn_1.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgChildBtn_1.Length; i++)
            {
                Util.AddBtnClick(imgChildBtn_1[i].gameObject, DoImgBtn_1Click);
            }
            _allClick1 = new int[2] { 0, 1};
            _allClick2 = new int[3] { 0, 1, 2};
            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            isEnd = false;
            _isExit = false;

            max.SetActive(true);
            problem.SetActive(true);
            proBtn.SetActive(false);
            a1.SetActive(true);
            SpineManager.instance.DoAnimation(a1, "kong", false);
            a2.SetActive(true);
            SpineManager.instance.DoAnimation(a2, "kong", false);
            imgBtn.SetActive(false);
            imgBtn_1.SetActive(false);
            int[] clipIndex_test = { 2, 3 };
            clipIndex = clipIndex_test;
            string[] spiName_test = { "4", "5" };
            spiName = spiName_test;
            string[] spi_1_test = { "animation2", "animation3", "animation4" };
            spi_1 = spi_1_test;
            string[] spi_2_test = { "sj", "zxc", "dt" };
            spi_2 = spi_2_test;
            string[] spi_3_test = { "animation5", "animation6", "animation7" };
            spi_3 = spi_3_test;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            problem.transform.localScale = new Vector3(0, 0.5f, 0);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => {
                problem.transform.DOScale(new Vector3(0.5f, 0.5f, 0), 1);
            }, () => {
                proBtn.SetActive(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                imgBtn.SetActive(false);
                a1.SetActive(false);
                a2.SetActive(true);
                a2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(a2, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () => {
                    max.SetActive(false);
                    imgBtn_1.SetActive(true);
                }));
            }else if(talkIndex == 2)
            {
                _isExit = true;
                imgBtn_1.SetActive(false);
                max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () => { }, () => {
                    imgBtn_1.SetActive(true);
                    max.SetActive(false);
                }));
            }
            talkIndex++;
        }

        void DoProBtnClick(GameObject obj)
        {
            proBtn.SetActive(false);
            problem.SetActive(false);
            a1.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(a1, "1", false);
            }, () => {
                SpineManager.instance.DoAnimation(a1, "2", false);
                imgBtn.SetActive(true);
            }));
        }

        void DoImgBtnClick(GameObject obj)
        {
            int idx = int.Parse(obj.name);
            imgBtn.SetActive(false);
            mono.StartCoroutine(ImgBtnCoroutine(idx));
        }

        IEnumerator ImgBtnCoroutine(int idx)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(a1, spiName[idx], true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, idx + 1, idx == 0 ? false : true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex[idx], 
            () => 
            {
                imgBtn.SetActive(false);
                _allClick1[idx] = 2;
            }, null));
            yield return new WaitForSeconds(SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, clipIndex[idx]));
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SpineManager.instance.DoAnimation(a1, "2", false);
            imgBtn.SetActive(true);
            CheckAllClick1();
        }

        void DoImgBtn_1Click(GameObject obj)
        {
            int idx = int.Parse(obj.name);
            imgBtn_1.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            mono.StartCoroutine(ImgBtn_1Coroutine(idx));
        }

        IEnumerator ImgBtn_1Coroutine(int idx)
        {
            _allClick2[idx] = 3;
            SoundManager.instance.ShowVoiceBtn(false);
            a2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            float len = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, idx + 5, false);
            float len1 = SpineManager.instance.DoAnimation(a2, spi_1[idx], false);
            yield return new WaitForSeconds(len1);
            a2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            float len2 = SpineManager.instance.DoAnimation(a2, spi_2[idx], false);
            yield return new WaitForSeconds(0.3f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, idx + 3, false);
            yield return new WaitForSeconds(len2);
            if (len > len1 + len2 + 0.3f) yield return new WaitForSeconds(len - len1 - len2 - 0.3f);
            //len = SpineManager.instance.DoAnimation(a2, spi_3[idx], false);
            //yield return new WaitForSeconds(len);
            SpineManager.instance.DoAnimation(a2, "animation", false);
            imgBtn_1.SetActive(true);
            CheckAllClick2();
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

        void CheckAllClick1()
        {
            bool all = true;
            for (int i = 0; i < _allClick1.Length; i++)
            {
                if(_allClick1[i] != 2)
                {
                    all = false;
                    return;
                }
            }

            if (all)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }

        void CheckAllClick2()
        {
            bool all = true;
            for (int i = 0; i < _allClick2.Length; i++)
            {
                if (_allClick2[i] != 3)
                {
                    all = false;
                    return;
                }
            }

            if (all && !_isExit)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
    }
}
