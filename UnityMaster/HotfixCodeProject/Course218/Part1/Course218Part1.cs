using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course218Part1
    {
        private GameObject bg1;
        private GameObject bg2;
        private GameObject bellBg;
        private GameObject bell;
        private GameObject imgBtn;
        private GameObject imgBtn_2;
        private GameObject backBtn;
        private GameObject spine_gaoxiao_1;
        private GameObject spine_gaoxiao_2;
        private GameObject spine_gaoxiao_3;
        private GameObject problem;

        private int talkIndex;
        private string[] spiName;
        private string[] spiName_2;
        private Vector3 bellStartPos;
        private Vector3 bellEndPos;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bg1 = curTrans.Find("bg").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bellBg = curTrans.Find("Bell").gameObject;
            bell = curTrans.Find("Bell/Bell").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            imgBtn_2 = curTrans.Find("ImgBtn_2").gameObject;
            backBtn = curTrans.Find("BackBtn").gameObject;
            spine_gaoxiao_1 = curTrans.Find("Spines/gaoxiao_1").gameObject;
            spine_gaoxiao_2 = curTrans.Find("Spines/gaoxiao_2").gameObject;
            spine_gaoxiao_3 = curTrans.Find("Spines/gaoxiao_3").gameObject;
            problem = curTrans.Find("bg2/Proelam").gameObject;

            Button[] imgBtnChild = imgBtn.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgBtnChild.Length; i++)
            {
                Util.AddBtnClick(imgBtnChild[i].gameObject, DoImgBtnClick);
            }
            Button[] imgBtn_1Child = imgBtn_2.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgBtn_1Child.Length; i++)
            {
                Util.AddBtnClick(imgBtn_1Child[i].gameObject, DoImgBtn_1Click);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;

            bg1.SetActive(true);
            bg2.SetActive(false);
            bellBg.SetActive(true);
            imgBtn.SetActive(false);
            imgBtn_2.SetActive(false);
            backBtn.SetActive(false);
            spine_gaoxiao_1.SetActive(false);
            spine_gaoxiao_2.SetActive(false);
            spine_gaoxiao_3.SetActive(false);
            problem.SetActive(true);

            string[] spiName_test = { "1", "2", "3", "4", "5" };
            spiName = spiName_test;
            string[] spiName_2_test = { "1", "2", "3" };
            spiName_2 = spiName_2_test;

            //bellStartPos = new Vector3(-643.7f, -411.5f, 0);
            //bellEndPos = bellStartPos;//new Vector3(20, -434, 0);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            //bellBg.transform.localPosition = bellStartPos;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }


        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bellBg.SetActive(false);
                bg1.SetActive(false);
                bg2.SetActive(true);
                spine_gaoxiao_1.SetActive(true);
                SpineManager.instance.DoAnimation(spine_gaoxiao_1, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () => {
                    imgBtn.SetActive(true);
                }));
            } else if (talkIndex == 2)
            {
                problem.SetActive(false);
                imgBtn.SetActive(false);
                spine_gaoxiao_1.SetActive(false);
                spine_gaoxiao_2.SetActive(true);
                SpineManager.instance.DoAnimation(spine_gaoxiao_2, "animation", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => { }, () => {
                    imgBtn_2.SetActive(true);
                }));
            }else if(talkIndex == 3)
            {
                bg1.SetActive(true);
                //bellBg.transform.localPosition = bellEndPos;
                spine_gaoxiao_2.SetActive(false);
                spine_gaoxiao_3.SetActive(false);
                imgBtn_2.SetActive(false);
                backBtn.SetActive(false);
                bg2.SetActive(false);
                bellBg.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () => { }, () => { }));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        void DoImgBtnClick(GameObject obj)
        {
            int idx = int.Parse(obj.name);
            imgBtn.SetActive(false);
            SoundManager.instance.SetShield(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 2, () => {
                SpineManager.instance.DoAnimation(spine_gaoxiao_1, spiName[idx], false);
            }, () => {
                imgBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void DoImgBtn_1Click(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            int idx = int.Parse(obj.name);
            imgBtn_2.SetActive(false);
            SoundManager.instance.SetShield(false);
            mono.StartCoroutine(ImgBtn_1Coroutine(idx));
        }

        IEnumerator ImgBtn_1Coroutine(int idx)
        {
            float spiTime = SpineManager.instance.DoAnimation(spine_gaoxiao_2, spiName_2[idx], false);
            yield return new WaitForSeconds(spiTime);
            spine_gaoxiao_3.SetActive(true);
            spiTime = SpineManager.instance.DoAnimation(spine_gaoxiao_3, "" + (idx + 1), false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 8, 
            () => 
            {
                if (idx == 0)
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                else if (idx == 1)
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                else
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            }, 
            () =>
            {
                backBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
            }, spiTime));
        }

        void DoBackBtnClick(GameObject obj)
        {
            backBtn.SetActive(false);
            SoundManager.instance.SetShield(false);
            mono.StartCoroutine(BackBtnCoroutine());
        }

        IEnumerator BackBtnCoroutine()
        {
            spine_gaoxiao_3.SetActive(false);
            SpineManager.instance.DoAnimation(spine_gaoxiao_2, "animation", false);
            yield return new WaitForSeconds(1);
            imgBtn_2.SetActive(true);
            SoundManager.instance.SetShield(true);
            SoundManager.instance.ShowVoiceBtn(true);
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                method_1();
            }
            yield return new WaitForSeconds(clipLength);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}
