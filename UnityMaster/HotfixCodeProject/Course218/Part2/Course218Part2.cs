using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course218Part2
    {
        private GameObject bellBg;
        private GameObject bell;
        private GameObject spine_B1_1;
        private GameObject spine_B1_2;
        private GameObject spine_B1_3;
        private GameObject spine_B2;
        private GameObject bg2;
        private GameObject imgBtn_1;
        private GameObject imgBtn_2;
        private GameObject imgBtn_3;
        private GameObject backBtn;

        private string[] spiName;//点击Spine动画的名字
        private int[] clipIndex;//播放Spine动画时的语音
        private bool isEnd;

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bellBg = curTrans.Find("Bell").gameObject;
            bell = curTrans.Find("Bell/Bell").gameObject;
            spine_B1_1 = curTrans.Find("SpineB1/B1_1").gameObject;   //优缺点按钮
            spine_B1_2 = curTrans.Find("Spines/B1_2").gameObject;     //优点选项
            spine_B1_3 = curTrans.Find("Spines/B1_3").gameObject;     //缺点选项
            spine_B2 = curTrans.Find("Spines/B2").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;
            imgBtn_2 = curTrans.Find("Spines/ImgBtn_2").gameObject;
            imgBtn_3 = curTrans.Find("Spines/ImgBtn_3").gameObject;
            backBtn = curTrans.Find("BackBtn").gameObject;

            Button[] imgBtn_1Child = imgBtn_1.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgBtn_1Child.Length; i++)
            {
                Util.AddBtnClick(imgBtn_1Child[i].gameObject, DoImgBtn_1Click);
            }
            Button[] imgBtn_2Child = imgBtn_2.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgBtn_2Child.Length; i++)
            {
                Util.AddBtnClick(imgBtn_2Child[i].gameObject, DoImgBtn_2Click);
            }
            Button[] imgBtn_3Child = imgBtn_3.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < imgBtn_3Child.Length; i++)
            {
                Util.AddBtnClick(imgBtn_3Child[i].gameObject, DoImgBtn_2Click);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            isEnd = false;

            spine_B1_1.SetActive(true);
            spine_B1_2.SetActive(false);
            spine_B1_3.SetActive(false);
            spine_B2.SetActive(false);
            bg2.SetActive(false);
            imgBtn_1.SetActive(false);
            imgBtn_2.SetActive(false);
            imgBtn_3.SetActive(false);
            backBtn.SetActive(false);

            string[] spiName_test = { "b1", "b2", "b3", "c1", "c2" };
            spiName = spiName_test;
            int[] clipIndex_test = { 1, 2, 3, 4, 5 };
            clipIndex = clipIndex_test;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(spine_B1_1, "animation", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                imgBtn_1.SetActive(true);
            }));

        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            backBtn.SetActive(false);
            imgBtn_1.SetActive(false);
            imgBtn_2.SetActive(false);
            imgBtn_3.SetActive(false);
            spine_B2.SetActive(false);
            bg2.SetActive(false);
            spine_B1_1.SetActive(true);
            spine_B1_2.SetActive(false);
            spine_B1_3.SetActive(false);
            SpineManager.instance.DoAnimation(spine_B1_1, "animation", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { }, 
            () => 
            {
                imgBtn_1.SetActive(true);
            }));
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //优点和缺点按钮的点击事件
        void DoImgBtn_1Click(GameObject Obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            imgBtn_1.SetActive(false);
            SoundManager.instance.SetShield(false);
            int idx = int.Parse(Obj.name);
            mono.StartCoroutine(ImgBtn_1Coroutine(idx));
        }

        IEnumerator ImgBtn_1Coroutine(int idx)
        {
            string spineName = "a1";
            if (idx == 1) spineName = "a2";
            float spiTime = SpineManager.instance.DoAnimation(spine_B1_1, spineName, false);
            yield return new WaitForSeconds(spiTime);
            bg2.SetActive(true);
            spine_B1_1.SetActive(false);
            if(idx == 0)
            {
                spine_B1_2.SetActive(true);
                SpineManager.instance.DoAnimation(spine_B1_2, "animation" + (idx + 2), false);
            }
            if (idx == 1)
            {
                spine_B1_3.SetActive(true);
                SpineManager.instance.DoAnimation(spine_B1_3, "animation" + (idx + 2), false);
            }
            spine_B2.SetActive(true);
            SoundManager.instance.SetShield(true);
            SpineManager.instance.DoAnimation(spine_B2, "animation", false);
            yield return new WaitForSeconds(0.3f);
            if (idx == 0) imgBtn_2.SetActive(true);
            else imgBtn_3.SetActive(true);
            backBtn.SetActive(true);
        }

        //点击优点和缺点后按钮的点击事件
        void DoImgBtn_2Click(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            imgBtn_2.SetActive(false);
            imgBtn_3.SetActive(false);
            backBtn.SetActive(false);
            SoundManager.instance.SetShield(false);
            int idx = int.Parse(obj.name);
            mono.StartCoroutine(ImgBtn_2Coroutine(idx));
        }

        IEnumerator ImgBtn_2Coroutine(int idx)
        {
            string spiIdx;
            bool isEnd2 = false;
            float spiTime = 0;
            if (idx <= 2 && idx >= 0)
                spiTime = SpineManager.instance.DoAnimation(spine_B1_2, spiName[idx], false);
            if (idx >= 3)
                spiTime = SpineManager.instance.DoAnimation(spine_B1_3, spiName[idx], false);
            yield return new WaitForSeconds(spiTime);
            spiIdx = "" + (idx + 1);
            if (spiIdx == "5") spiIdx = "6";
            spiTime = SpineManager.instance.DoAnimation(spine_B2, spiIdx, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 1, 
                () => 
                { 
                    if(idx == 0 || idx == 1 || idx == 2)
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    if (idx == 3)
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    if (idx == 4)
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                },
                () => {
                if (isEnd2)
                {
                    if (idx <= 2) imgBtn_2.SetActive(true);
                    else imgBtn_3.SetActive(true);
                    backBtn.SetActive(true);
                    SoundManager.instance.SetShield(true);
                    SpineManager.instance.DoAnimation(spine_B2, "animation", false);
                }
                else isEnd2 = true;
            }));
            if (idx == 3)
            {
                yield return new WaitForSeconds(2);
                spiTime = SpineManager.instance.DoAnimation(spine_B2, "5", false);
                yield return new WaitForSeconds(spiTime + 1);
            }
            else yield return new WaitForSeconds(spiTime + 1);
            if (isEnd2)
            {
                if (idx <= 2) imgBtn_2.SetActive(true);
                else imgBtn_3.SetActive(true);
                backBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
                SpineManager.instance.DoAnimation(spine_B2, "animation", false);
            }
            else isEnd2 = true;
        }

        void DoBackBtnClick(GameObject obj)
        {
            backBtn.SetActive(false);
            imgBtn_2.SetActive(false);
            imgBtn_3.SetActive(false);
            spine_B2.SetActive(false);
            bg2.SetActive(false);
            spine_B1_1.SetActive(true);
            spine_B1_2.SetActive(false);
            spine_B1_3.SetActive(false);
            SpineManager.instance.DoAnimation(spine_B1_1, "animation", false);
            imgBtn_1.SetActive(true);
            if (!isEnd)
            {
                SoundManager.instance.ShowVoiceBtn(true);
                isEnd = true;
            }
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
    }
}
