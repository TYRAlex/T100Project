using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course212Part2
    {
        private GameObject bell;
        private GameObject bell_spine;
        private GameObject bg2;
        private GameObject bg3;
        private GameObject becu_1;
        private GameObject becu_2;
        private GameObject becu_3;
        private GameObject imgBtn_1;
        private GameObject proImg;
        private GameObject dragerImg_parent;
        private GameObject droperImg_parent;
        private GameObject ligthImg_parent;
        private GameObject title_parent;
        private ILDrager[] dragerImg;
        private GameObject[] droperImg;
        private GameObject[] ligthImg;
        private GameObject[] title;

        private int talkIndex;
        private bool[] isDown;
        private bool[] isDrager;//�Ƿ���óɹ�
        private bool[] isTrun;
        private float[] trunAngle;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("bell").gameObject;
            bell_spine = curTrans.Find("bell/bell").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            bg3 = curTrans.Find("bg3").gameObject;
            becu_1 = curTrans.Find("Spine/becu_1").gameObject;
            becu_2 = curTrans.Find("Spine/becu_2").gameObject;
            becu_3 = curTrans.Find("Spine/becu_3").gameObject;

            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;
            dragerImg_parent = curTrans.Find("ImgBtn/DragerImg").gameObject;
            droperImg_parent = curTrans.Find("ImgBtn/DroperImg").gameObject;
            ligthImg_parent = curTrans.Find("ImgBtn/LigthImg").gameObject;
            title_parent = curTrans.Find("ImgBtn/Title").gameObject;
            proImg = curTrans.Find("ImgBtn/ProImg").gameObject;

            dragerImg = new ILDrager[dragerImg_parent.transform.childCount];
            for(int i = 0; i < dragerImg_parent.transform.childCount; i++)
            {
                dragerImg[i] = dragerImg_parent.transform.GetChild(i).GetComponent<ILDrager>();
                dragerImg[i].transform.GetChild(0).gameObject.SetActive(true);
                dragerImg[i].SetDragCallback(StartDrag, Drag, EndDrag);
            }
            droperImg = new GameObject[droperImg_parent.transform.childCount];
            for (int i = 0; i < droperImg_parent.transform.childCount; i++)
            {
                droperImg[i] = droperImg_parent.transform.GetChild(i).gameObject;
            }
            ligthImg = new GameObject[ligthImg_parent.transform.childCount];
            for (int i = 0; i < ligthImg_parent.transform.childCount; i++)
            {
                ligthImg[i] = ligthImg_parent.transform.GetChild(i).gameObject;
            }
            title = new GameObject[title_parent.transform.childCount];
            for (int i = 0; i < title_parent.transform.childCount; i++)
            {
                title[i] = title_parent.transform.GetChild(i).gameObject;
            }


            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();         
        }

        void GameInit()
        {
            talkIndex = 1;
            bool[] isDown_test = {false, false};
            isDown = isDown_test;
            bool[] isDrager_test = { false, false, false };
            isDrager = isDrager_test;
            bool[] isTrun_tese = {false, false, false };
            isTrun = isTrun_tese;
            float[] trunAngle_test = { 6, 2, 4};
            trunAngle = trunAngle_test;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            becu_1.SetActive(true);
            becu_2.SetActive(true);
            becu_1.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            becu_2.GetComponent<RectTransform>().anchoredPosition = new Vector2(3000, 3000);
            SpineManager.instance.DoAnimation(becu_1, "chuanga", false);
            SpineManager.instance.DoAnimation(becu_2, "chuangb", false);
            SpineManager.instance.DoAnimation(becu_3, "wg", false);
            bg2.SetActive(false);
            bell.SetActive(true);
            bg3.SetActive(false);
            becu_3.SetActive(false);
            imgBtn_1.SetActive(false);
            dragerImg_parent.SetActive(false);
            for (int i = 0; i < dragerImg_parent.transform.childCount; i++)
            {
                dragerImg_parent.transform.GetChild(i).gameObject.Show();
            }

            droperImg_parent.SetActive(false);
            ligthImg_parent.SetActive(false);
            title_parent.SetActive(false);
            proImg.SetActive(false);

            for(int i = 0; i < dragerImg.Length; i++)
            {
                dragerImg[i].isActived = false;
            }
            for (int i = 0; i < ligthImg.Length; i++)
            {
                ligthImg[i].SetActive(false);
            }
            for (int i = 0; i < title.Length; i++)
            {
                title[i].SetActive(false);
            }
            for (int i = 0; i < imgBtn_1.transform.childCount; i++)
            {
                Util.AddBtnClick(imgBtn_1.transform.GetChild(i).gameObject, DoImgBtn_1Click);
            }

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () =>
            {
                bell.SetActive(false);
                becu_1.GetComponent<RectTransform>().anchoredPosition = new Vector2(-812f, -488f);
                becu_2.GetComponent<RectTransform>().anchoredPosition = new Vector2(-812f, -488f);
                SpineManager.instance.DoAnimation(becu_1, "chuanga", false);
                SpineManager.instance.DoAnimation(becu_2, "chuangb", false);
                imgBtn_1.SetActive(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                imgBtn_1.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    mono.StartCoroutine(Talk_1Coroutine());
                }));
            }
            else if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { }, () =>
                {
                    mono.StartCoroutine(Talk_2Coroutine());
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () => {
                    //SoundManager.instance.Stop("sound");
                    SoundManager.instance.Stop("voice");
                    //SpineManager.instance.DoAnimation(becu_2, "wg", false);
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if(talkIndex == 4)
            {
                droperImg_parent.SetActive(true);
                dragerImg_parent.SetActive(true);
                proImg.SetActive(true);
                becu_3.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () => {
                    for (int i = 0; i < dragerImg.Length; i++)
                    {
                        dragerImg[i].isActived = true;
                    }
                }));
            }
            else if (talkIndex == 5)
            {
                SoundManager.instance.Stop("sound");
                SoundManager.instance.Stop("voice");
                bg3.SetActive(true);
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => { }, () => {
                }));
            }
            talkIndex++;
        }

        IEnumerator Talk_1Coroutine()
        {
            float len = SpineManager.instance.DoAnimation(becu_1, "chuangxs", false);
            SpineManager.instance.DoAnimation(becu_2, "chuangxs2", false);
            yield return new WaitForSeconds(len - 0.2f);
            becu_1.SetActive(false);
            becu_2.SetActive(false);
            yield return new WaitForSeconds(0.1f);
            bg2.SetActive(true);
            becu_3.SetActive(true);
            SpineManager.instance.DoAnimation(becu_3, "wg", false);
            yield return new WaitForSeconds(0.1f);
            SoundManager.instance.ShowVoiceBtn(true);
        }
        IEnumerator Talk_2Coroutine()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
            float len = SpineManager.instance.DoAnimation(becu_3, "wg1", false);
            yield return new WaitForSeconds(len);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, true);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, true);
            SpineManager.instance.DoAnimation(becu_3, "wg2", true);
        }

        void DoImgBtn_1Click(GameObject obj)
        {
            int n = 1;
            if (obj.name == "a") n = 0;
            if (isDown[n]) return;
            imgBtn_1.SetActive(false);
            SoundManager.instance.SetShield(false);
            mono.StartCoroutine(DoImgBtn_1ClickCoroutine(obj));
        }

        IEnumerator DoImgBtn_1ClickCoroutine(GameObject obj)
        {
            GameObject spiObj = becu_2;
            int idx = 1;
            string str = obj.name;
            if (str == "a")
            {
                spiObj = becu_1;
                idx = 0;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            }
            isDown[idx] = true;
            float len = SpineManager.instance.DoAnimation(spiObj, "chuang" + str + "1", false);
            yield return new WaitForSeconds(len);
            if (isDown[0] && isDown[1]) SoundManager.instance.ShowVoiceBtn(true);
            imgBtn_1.SetActive(true);
            SoundManager.instance.SetShield(true);
        }

        void StartDrag(Vector3 pos, int index, int type)
        {
            dragerImg[index].transform.GetChild(0).gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            Debug.Log("��ʼ�϶�");
        }

        void Drag(Vector3 pos, int index, int type)
        {
        }

        void EndDrag(Vector3 pos, int index, int type, bool isMatch)
        {
            Debug.Log("�����϶�"+isMatch);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                //droperImg[index].transform.Find("ligth_1").gameObject.SetActive(true);
                //droperImg[index].transform.Find("ligth_2").gameObject.SetActive(true);
                ligthImg_parent.SetActive(true);
                ligthImg[index].SetActive(true);
                title_parent.SetActive(true);
                title[index].SetActive(true);
                isTrun[index] = true;
                if (isTrun[index]) mono.StartCoroutine(DoTurnCoroutine(index, trunAngle[index]));
                dragerImg[index].DoReset();
                dragerImg[index].transform.gameObject.SetActive(false);
                isDrager[index] = true;
                if(isDrager[0] && isDrager[1] && isDrager[2])
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }
            else
            {
                dragerImg[index].DoReset();
                dragerImg[index].transform.GetChild(0).gameObject.SetActive(true);
            }
        }

        IEnumerator DoTurnCoroutine(int index,float angle)
        {
            float startAngle = 0;
            ligthImg[index].transform.localRotation = new Quaternion(0, 0, 0, 0);
            while (isTrun[index])
            {
                ligthImg[index].transform.DOLocalRotate(new Vector3(0, 0, startAngle + angle), 0.1f);
                startAngle = startAngle + angle;
                yield return new WaitForSeconds(0.1f);
            }
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(bell_spine, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell_spine, "DAIJI");
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
