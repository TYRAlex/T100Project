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
    public class Course212Part1
    {
        private GameObject bell;
        private GameObject bell_spine;
        private GameObject problems;
        private GameObject spine_1;
        private GameObject spine_2;
        private GameObject imgBtn_1;
        private GameObject imgBtn_2;
        private GameObject backBtn;
        private GameObject bg_3;
        private GameObject[] problemsArray;

        private int talkIndex;
        private int downNumber;
        private string str;
        private Vector3[] problemsPos;


        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("bell").gameObject;
            bell_spine = curTrans.Find("bell/bell").gameObject;
            problems = curTrans.Find("Problems").gameObject;
            spine_1 = curTrans.Find("Spine/Spine_1").gameObject;
            spine_2 = curTrans.Find("Spine/Spine_2").gameObject;
            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;
            imgBtn_2 = curTrans.Find("ImgBtn/ImgBtn_2").gameObject;
            backBtn = curTrans.Find("ImgBtn/BackBtn").gameObject;
            bg_3 = curTrans.Find("bg_3").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            downNumber = 1;
            str = "";
            Vector3[] problemsPos_test = {new Vector3(108,0,0), new Vector3(108,143,0) };
            problemsPos = problemsPos_test;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            problemsArray = new GameObject[problems.transform.childCount];
            for(int i=0;i< problems.transform.childCount; i++)
            {
                problemsArray[i] = problems.transform.GetChild(i).gameObject;
                problemsArray[i].SetActive(true);
            }
            problemsArray[1].SetActive(false);
            problemsArray[3].SetActive(false);
            problemsArray[5].SetActive(false);
            problemsArray[6].SetActive(false);
            for (int i = 0; i < imgBtn_1.transform.childCount; i++)
            {
                Util.AddBtnClick(imgBtn_1.transform.GetChild(i).gameObject, DoImgBtn_1Click);
            }
            for (int i = 0; i < imgBtn_2.transform.childCount; i++)
            {
                imgBtn_2.transform.GetChild(i).GetComponent<Image>().alphaHitTestMinimumThreshold = 0.1f;
                Util.AddBtnClick(imgBtn_2.transform.GetChild(i).gameObject, DoImgBtn_2Click);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            spine_1.SetActive(false);
            spine_2.SetActive(false);
            imgBtn_1.SetActive(false);
            imgBtn_2.SetActive(false);
            backBtn.SetActive(false);
            bg_3.SetActive(false);

            problemsArray[0].transform.localPosition = problemsPos[0];
            problemsArray[2].transform.localScale = new Vector3(0, 1, 1);
            problemsArray[4].transform.localScale = new Vector3(0, 1, 1);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                problemsArray[0].transform.DOLocalMove(problemsPos[1], 1);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => {
                    mono.StartCoroutine(TalkClick_1());
                }, () =>
                {
                    imgBtn_1.SetActive(true);
                },1));
            }
            else if (talkIndex == 2)
            {
                backBtn.SetActive(false);
                imgBtn_1.SetActive(false);
                imgBtn_2.SetActive(false);
                bg_3.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () =>{ }));
            }
            talkIndex++;
        }
        IEnumerator TalkClick_1()
        {
            yield return new WaitForSeconds(3f);
            problemsArray[1].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            problemsArray[2].transform.DOScale(new Vector3(1, 1, 1), 0.8f);
            yield return new WaitForSeconds(1f);
            problemsArray[3].SetActive(true);
            yield return new WaitForSeconds(0.3f);
            problemsArray[4].transform.DOScale(new Vector3(1, 1, 1), 0.8f);
            yield return new WaitForSeconds(0.8f);
            spine_1.SetActive(true);
            SpineManager.instance.DoAnimation(spine_1, "a",false);
            spine_2.SetActive(true);
            SpineManager.instance.DoAnimation(spine_2, "b", false);
            yield return new WaitForSeconds(0.1f);
            for(int i = 1; i <= 4; i++)
            {
                problemsArray[i].SetActive(false);
            }

        }

        void DoImgBtn_1Click(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            imgBtn_1.SetActive(false);
            mono.StartCoroutine(DoImgBtn_1ClickCoroutine(obj));
        }

        IEnumerator DoImgBtn_1ClickCoroutine(GameObject obj)
        {
            GameObject spi;
            int idx = 2;
            if(obj.name == "0")
            {
                spi = spine_1;
                str = "a2";
                idx = 2;
            }
            else
            {
                downNumber = 1;
                spi = spine_2;
                str = "b2";
                idx = 3;
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            float aniTime = SpineManager.instance.DoAnimation(spi, str, false);
            yield return new WaitForSeconds(aniTime);
            problemsArray[0].SetActive(false);
            if (str == "a2")
            {
                spine_2.SetActive(false);
                problemsArray[5].SetActive(true);
            }
            else
            {
                spine_1.SetActive(false);
                problemsArray[6].SetActive(true);
            }
            aniTime = SpineManager.instance.DoAnimation(spi, "chilun", false);
            yield return new WaitForSeconds(aniTime);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx, () => { }, () => {
                imgBtn_2.SetActive(true);
                backBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
            }));
        }

        void DoImgBtn_2Click(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            imgBtn_2.SetActive(false);
            backBtn.SetActive(false);
            mono.StartCoroutine(DoImgBtn_2ClickCoroutine(obj));
        }
        IEnumerator DoImgBtn_2ClickCoroutine(GameObject obj)
        {
            if(str == "a2")
            {
                string spiName = "";
                if(obj.name == "0")
                {
                    spiName = "chilunwl1";
                }
                else
                {   
                    spiName = "chilunwg0";
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                float len = SpineManager.instance.DoAnimation(spine_1, spiName, false);
                yield return new WaitForSeconds(len);
            }else if(str == "b2")
            {
                if(obj.name != "0")
                {
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                    float len = SpineManager.instance.DoAnimation(spine_2, "chilunwg"+downNumber, false);
                    yield return new WaitForSeconds(len);
                    SoundManager.instance.Stop("voice");
                    downNumber++;
                    if (downNumber == 5) downNumber = 1;
                }
            }
            SoundManager.instance.ShowVoiceBtn(true);
            imgBtn_2.SetActive(true);
            backBtn.SetActive(true);
            SoundManager.instance.SetShield(true);
        }

        void DoBackBtnClick(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            imgBtn_2.SetActive(false);
            backBtn.SetActive(false);
            mono.StartCoroutine(DoBackBtnClickCoroutine(obj));
        }

        IEnumerator DoBackBtnClickCoroutine(GameObject obj)
        {
            GameObject spiObj = spine_2;
            string spiName = "b";
            if(str == "a2")
            {
                spiName = "a";
                spiObj = spine_1;
            }
            float len = SpineManager.instance.DoAnimation(spiObj, "chilun2", false);
            yield return new WaitForSeconds(len);
            spine_1.SetActive(true);
            spine_2.SetActive(true);
            SpineManager.instance.DoAnimation(spiObj, spiName, false);
            problemsArray[0].SetActive(true);
            problemsArray[5].SetActive(false);
            problemsArray[6].SetActive(false);
            imgBtn_1.SetActive(true);
            SoundManager.instance.SetShield(true);
        }

            IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(bell_spine, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            SoundManager.instance.SetShield(false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell_spine, "DAIJI");
            SoundManager.instance.SetShield(true);
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
