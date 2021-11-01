using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course210Part1
    {
        private GameObject bell;
        private GameObject imgBtn_1;
        private GameObject imgBtn_2;
        private GameObject carlu_1;
        private GameObject chilunName1;
        private GameObject chilunName2;

        private bool isEnd;
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("Bell/BellSpine").gameObject;
            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;
            imgBtn_2 = curTrans.Find("ImgBtn/ImgBtn_2").gameObject;
            carlu_1 = curTrans.Find("CarLus/Chilun_1").gameObject;
            chilunName1 = curTrans.Find("CarLus/ChilunName1").gameObject;
            chilunName2 = curTrans.Find("CarLus/ChilunName2").gameObject;
            Button[] btn_1 = imgBtn_1.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < btn_1.Length; i++)
            {
                Util.AddBtnClick(btn_1[i].gameObject, DoImgBtn_1Click);
            }
            Button[] btn_2 = imgBtn_2.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < btn_2.Length; i++)
            {
                Util.AddBtnClick(btn_2[i].gameObject, DoImgBtn_2Click);
            }
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            GameInit();
        }

        void GameInit()
        {
            isEnd = false;
            bell.SetActive(true);
            imgBtn_1.SetActive(false);
            imgBtn_2.SetActive(false);
            carlu_1.SetActive(true);
            chilunName1.SetActive(false);
            chilunName2.SetActive(false);

            SpineManager.instance.DoAnimation(carlu_1, "z1", false);

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(carlu_1, "z4", false);
                }, () => { imgBtn_1.SetActive(true); }));
            }
            else if (talkIndex == 2)
            {
                imgBtn_1.SetActive(false);
                SpineManager.instance.DoAnimation(carlu_1, "1", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null, () =>
                {
                    imgBtn_2.SetActive(true);
                }));
            }
            else if (talkIndex == 4)
            {
                imgBtn_2.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null, () =>
                {
                    imgBtn_2.SetActive(true);
                }));
            }
            talkIndex++;
        }

        //ImgBtn_1的按钮点击事件
        void DoImgBtn_1Click(GameObject obj)
        {
            imgBtn_1.SetActive(false);
            int idx = int.Parse(obj.name);
            string spiName = "z3";
            if (idx == 1)
            {
                spiName = "z2";
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SoundManager.instance.ShowVoiceBtn(true);
            }

         
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 1, () => { SpineManager.instance.DoAnimation(carlu_1, spiName, false); }, () =>
            {

                imgBtn_1.SetActive(true);
            }));
        }

        //ImgBtn_2的按钮点击事件
        void DoImgBtn_2Click(GameObject obj)
        {
            imgBtn_2.SetActive(false);
            int idx = int.Parse(obj.name);
            int clipIndex = 0;
            int waitTime = 0;
            string spiName = "";
            string spiChiLunName = "";
            GameObject tem = null;
            switch (obj.name)
            {
                case "1":
                    {
                        clipIndex = 6;
                        spiName = "b2";

                        spiChiLunName = "and1";
                        tem = chilunName1;                    
                    }
                    break;
                case "2":
                    {
                        clipIndex = 8;
                        spiName = "d1";
                    }

                    break;
                case "3":
                    {
                        clipIndex = 7;
                        spiName = "b1";

                        spiChiLunName = "and2";
                        tem = chilunName2;
                    }
                    break;
                case "4":
                    {
                        clipIndex = 8;
                        spiName = "d2";
                    }
                    break;
                default:
                    break;
            }

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, clipIndex, () =>
            {
                SpineManager.instance.DoAnimation(carlu_1, spiName, false);
                if (tem)
                {
                    tem.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(tem, spiChiLunName, false);
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    
                }

            }, () =>
            {
                imgBtn_2.SetActive(true);
                if (!isEnd)
                {
                    isEnd = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }, waitTime));
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
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

    }
}

