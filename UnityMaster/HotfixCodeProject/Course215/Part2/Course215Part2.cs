using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course215Part2
    {
        private GameObject bell;
        private GameObject bellBg;
        private GameObject bg2;
        private GameObject bg3;
        private GameObject bg4;
        private GameObject spine_1;
        private GameObject spine_2;
        private GameObject imgBtn;
        private GameObject backBtn;
        private GameObject shield;

        private int talkIndex;
        private int currentPage;
        private string[] spiName;
        private int[] clipIndex;
        private Vector3 bellStartPos;
        private Vector3 bellEndPos;

        MonoBehaviour mono;
        GameObject curGo;

    

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
        
            bellBg = curTrans.Find("Bell").gameObject;
            bell = curTrans.Find("Bell/Bell").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bg3 = curTrans.Find("bg3").gameObject;
            bg4 = curTrans.Find("bg4").gameObject;
            spine_1 = curTrans.Find("spine_1").gameObject;
            spine_2 = curTrans.Find("spine_2").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            backBtn = curTrans.Find("BackBtn").gameObject;
            shield = curTrans.Find("shield").gameObject;

            Button[] btn = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i= 0; i < btn.Length; i++)
            {
                Util.AddBtnClick(btn[i].transform.gameObject, DoImgBtnClick);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            SoundManager.instance.SetShield(true);
            talkIndex = 1;
            currentPage = -1;
            bg2.SetActive(true);
            bg3.SetActive(false);
            bg4.SetActive(false);
            spine_1.SetActive(true);
            spine_2.SetActive(false);
            imgBtn.SetActive(false);
            bellBg.SetActive(true);
            shield.SetActive(false);
            backBtn.SetActive(false);
            string[] spiName_test = { "a", "b", "c" };
            spiName = spiName_test;
            int[] clipIndex_test = { 1, 2, 3 };
            clipIndex = clipIndex_test;
            bellStartPos = new Vector3(-739.6f, -645, 0);
            bellEndPos = new Vector3(0, -369, 0);

            spine_2.transform.GetComponent<SkeletonGraphic>().CrossFadeAlpha(0, 0, true);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            bellBg.transform.localPosition = bellStartPos;
       
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                SpineManager.instance.DoAnimation(spine_1, "qbz", false, () =>
                {
                    SpineManager.instance.DoAnimation(spine_1, "qbz", false, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    });
                });
            }, null));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(Talk_1_Coroutine());
                //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                //    imgBtn.SetActive(true);
                //}));

            }else if(talkIndex == 2)
            {
                shield.SetActive(true);
                imgBtn.SetActive(false);
                backBtn.SetActive(false);
                bg3.SetActive(false);
                bg4.SetActive(false);
                spine_2.SetActive(false);
                bellBg.SetActive(true);
                bellBg.transform.localPosition = bellEndPos;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () => {
                    shield.SetActive(true);
                }));
            }
            talkIndex++;
        }

        IEnumerator Talk_1_Coroutine()
        {
            bg2.SetActive(false);
            bg3.SetActive(true);
            spine_1.SetActive(false);
            spine_2.SetActive(true);
            bellBg.SetActive(false);
            yield return new WaitForSeconds(0.2f);
            SpineManager.instance.DoAnimation(this.spine_2, "animation", false);
            spine_2.transform.GetComponent<SkeletonGraphic>().CrossFadeAlpha(1, 0.66f, true);
            yield return new WaitForSeconds(1);
            imgBtn.SetActive(true);
        }

        //图片按钮点击事件
        void DoImgBtnClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SoundManager.instance.SetShield(false);
            imgBtn.SetActive(false);
            int idx = int.Parse(obj.name);
            currentPage = idx;
            mono.StartCoroutine(ImgBtnCoroutine(idx));
            
        }

        IEnumerator ImgBtnCoroutine(int idx)
        {
            float len = SpineManager.instance.DoAnimation(spine_2, spiName[idx] + "1", false);
            yield return new WaitForSeconds(len);
            bg4.SetActive(false);
            float spiTime = SpineManager.instance.DoAnimation(spine_2, spiName[idx] + "2", true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex[idx], () => {
                //SpineManager.instance.DoAnimation(spine_2, spiName[idx] + "2", true);
            }, () => {
                backBtn.SetActive(true);
                SoundManager.instance.ShowVoiceBtn(true);
                SoundManager.instance.SetShield(true);
            }, spiTime));
        }

        //返回按钮点击事件
        void DoBackBtnClick(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SoundManager.instance.SetShield(false);
            backBtn.SetActive(false);
            mono.StartCoroutine(BackBtnCoroutine());
        }

        IEnumerator BackBtnCoroutine()
        {
            bg4.SetActive(false);
            float len = SpineManager.instance.DoAnimation(this.spine_2, spiName[currentPage] + "3", false);
            yield return new WaitForSeconds(len);
            len = SpineManager.instance.DoAnimation(this.spine_2, "animation", false);
            SoundManager.instance.SetShield(true);
            imgBtn.SetActive(true);
        }

        //bell说话协程
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

        void OnDisable()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.Stop();
        }
    }
}
