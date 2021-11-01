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
    public class Course221Part1
    {
        private GameObject bell;
        private GameObject bg2;
        private GameObject spine_A;
        private GameObject spine_B;
        private GameObject ligth;//圆的光圈
        private GameObject ligth2;//轴的光圈
        private GameObject axis;//轴
        private GameObject waterTapBtn;//水龙头按钮
        private GameObject shield;
        private GameObject imgBtn;
        private GameObject imgBtn2;
        private GameObject backBtn;

        private int talkIndex;
        private bool isAxis;//水龙头当前状态
        private System.Random random;
        private MonoBehaviour mono;
        private int currentIdx;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("Bell/BellSpine").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            spine_A = curTrans.Find("Spine_A").gameObject;
            spine_B = curTrans.Find("Spine_B").gameObject;
            ligth = curTrans.Find("Ligth").gameObject;
            ligth2 = curTrans.Find("Ligth/Ligth2").gameObject;
            axis = curTrans.Find("Ligth/Axis").gameObject;
            waterTapBtn = curTrans.Find("ImgBtn/waterTapBtn").gameObject;
            shield = curTrans.Find("Shield").gameObject;
            imgBtn = curTrans.Find("ImgBtn/ImgBtn").gameObject;
            imgBtn2 = curTrans.Find("ImgBtn/ImgBtn2").gameObject;
            backBtn = curTrans.Find("ImgBtn/BackBtn").gameObject;

            Util.AddBtnClick(waterTapBtn, DoWaterTapBtnClick);
            Button[] btn = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < btn.Length; i++)
            {
                Util.AddBtnClick(btn[i].gameObject, DoImgBtnClick);
            }
            Button[] btn2 = imgBtn2.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < btn2.Length; i++)
            {
                Util.AddBtnClick(btn2[i].gameObject, DoImgBtn2Click);
            }
            Util.AddBtnClick(backBtn, DoBackBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();
            GameStart();
        }

        void GameInit()
        {
            talkIndex = 1;
            isAxis = false;
            random = new System.Random();

            bg2.SetActive(true);
            spine_A.SetActive(true);
            spine_B.SetActive(true);
            ligth.SetActive(false);
            ligth2.SetActive(false);
            axis.SetActive(false);
            waterTapBtn.SetActive(false);
            shield.SetActive(false);
            imgBtn.SetActive(false);
            imgBtn2.SetActive(false);
            backBtn.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }

        void GameStart()
        {
            GameInit();
            SpineManager.instance.DoAnimation(spine_A, "a1", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                waterTapBtn.SetActive(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }else if(talkIndex == 2)
            {
                waterTapBtn.SetActive(false);
                mono.StartCoroutine(Talk_2_Coroutine());
            }else if(talkIndex == 3)
            {
                bg2.SetActive(false);
                ligth.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => {
                    SpineManager.instance.DoAnimation(spine_A, "b1", false);
                }, () => {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }else if(talkIndex == 4)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => { }, () => {
                    imgBtn.SetActive(true);
                }));
            }else if(talkIndex == 5)
            {
                imgBtn.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () => {
                    SpineManager.instance.DoAnimation(spine_A, "c1", false);
                }, () => {
                    imgBtn2.SetActive(true);
                }));
            }else if(talkIndex == 6)
            {
                imgBtn2.SetActive(false);
                backBtn.SetActive(false);
                spine_A.SetActive(true);
                spine_B.SetActive(false);
                SpineManager.instance.DoAnimation(spine_A, "f", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, () => { }, () => { }));
            }
            talkIndex++;
        }

        IEnumerator Talk_2_Coroutine()
        {
            if (isAxis)
            {
                isAxis = false;
                float spiTime = SpineManager.instance.DoAnimation(spine_A, "a4", false);
                yield return new WaitForSeconds(spiTime);
            }
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            yield return new WaitForSeconds(1);
            ligth.SetActive(true);
            yield return new WaitForSeconds(3);
            //ligth2.SetActive(true);
            axis.SetActive(true);
            //ligth2.transform.DOScale(new Vector3(1.3f, 1.3f, 0.5), 1f);
            axis.transform.DOScale(new Vector3(1.3f, 1.3f, 1), 1f);
            yield return new WaitForSeconds(1f);
            //ligth2.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            axis.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            yield return new WaitForSeconds(0.5f);
            ligth2.SetActive(true);
        }

        void DoWaterTapBtnClick(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            shield.SetActive(true);
            string spiName = "";
            if (isAxis)
            {
                isAxis = false;
                spiName = "a4";
            }
            else
            {
                isAxis = true;
                spiName = "a2";
            }
            SpineManager.instance.DoAnimation(spine_A, spiName, false, () => {
                if (isAxis) SpineManager.instance.DoAnimation(spine_A, "a3", true);
                SoundManager.instance.SetShield(true);
                SoundManager.instance.ShowVoiceBtn(true);
                shield.SetActive(false);
            });
        }

        void DoImgBtnClick(GameObject obj)
        {
            shield.SetActive(true);
            string btnName = obj.name;
            int clipIndex;
            clipIndex = 6;
            if (btnName == "FulcrumBtn") SpineManager.instance.DoAnimation(spine_A, "b2", false);
            else clipIndex = 6 + random.Next(1, 4);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex, () => { }, () => {
                if(btnName == "FulcrumBtn") SoundManager.instance.ShowVoiceBtn(true);
                shield.SetActive(false);
            }));
        }

        void DoImgBtn2Click(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            currentIdx = int.Parse(obj.name);
            imgBtn2.SetActive(false);
            mono.StartCoroutine(ImgBtn2Coroutine());
        }

        IEnumerator ImgBtn2Coroutine()
        {
            string str;
            spine_B.SetActive(true);
            float spiTime = SpineManager.instance.DoAnimation(spine_A, "c" + (currentIdx + 1), false);
            yield return new WaitForSeconds(spiTime);
            if (currentIdx == 1) str = "d";
            else str = "e";
            spiTime = SpineManager.instance.DoAnimation(spine_B, str + "1", false);
            yield return new WaitForSeconds(0.15f);
            spine_A.SetActive(false);
            spine_B.transform.SetSiblingIndex(3);
            yield return new WaitForSeconds(spiTime-0.1f);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10+currentIdx, () => {   
                SpineManager.instance.DoAnimation(spine_B, str + "3", false);
            }, () => {
                SoundManager.instance.SetShield(true);
                backBtn.SetActive(true);
            }));
        }

        void DoBackBtnClick(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            backBtn.SetActive(false);
            string str;
            if (currentIdx == 1) str = "d";
            else str = "e";
            SpineManager.instance.DoAnimation(spine_B, str + "5", false,()=> {
                spine_B.transform.SetAsFirstSibling();
                SpineManager.instance.DoAnimation(spine_B, "d2", false);
                spine_A.SetActive(true);
                SpineManager.instance.DoAnimation(spine_A, "c" + (currentIdx + 3), false,()=> {
                    imgBtn2.SetActive(true);
                    SoundManager.instance.SetShield(true);
                    SoundManager.instance.ShowVoiceBtn(true);
                });
            });
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
