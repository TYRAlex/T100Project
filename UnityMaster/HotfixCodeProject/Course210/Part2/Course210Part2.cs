using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course210Part2
    {
        private GameObject bell;
        private GameObject imgBtn_1;
        private GameObject backBtn;
        private GameObject car;
        private GameObject bg2;
        private GameObject carSpines_1;
        private GameObject carSpines_2;
        private GameObject wh;
        private Transform tiles;

        private int talkIndex;
        private int currentCar;

        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            bell = curTrans.Find("Bell/BellSpine").gameObject;
            imgBtn_1 = curTrans.Find("ImgBtn/ImgBtn_1").gameObject;
            backBtn = curTrans.Find("ImgBtn/BackBtn").gameObject;
            car = curTrans.Find("Car").gameObject;
            bg2 = curTrans.Find("bg2").gameObject;
            carSpines_1 = curTrans.Find("CarSpines/Car1").gameObject;
            carSpines_2 = curTrans.Find("CarSpines/Car2").gameObject;
            wh = curTrans.Find("CarSpines/Wh").gameObject;


            for (int i = 0; i < imgBtn_1.transform.childCount; i++) { Util.AddBtnClick(imgBtn_1.transform.GetChild(i).gameObject, DoImgBtn_1Click); }
            Util.AddBtnClick(backBtn, DoBackBtnClick);
            tiles = curTrans.Find("Tiles");
            tiles.gameObject.SetActive(true);
            for (int i = 0; i < tiles.childCount; i++)
            {
                tiles.GetChild(i).gameObject.SetActive(false);
            }

            talkIndex = 1;
            currentCar = 0;

            bell.SetActive(true);
            imgBtn_1.SetActive(false);
            backBtn.SetActive(false);
            car.SetActive(true);
            bg2.SetActive(false);
            carSpines_1.SetActive(false);
            carSpines_2.SetActive(false);
            wh.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { SpineManager.instance.DoAnimation(car, "animation", false); }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    imgBtn_1.SetActive(true);
                }));
            }
            else if (talkIndex == 2)
            {
                bell.SetActive(false);
                imgBtn_1.SetActive(false);
                backBtn.SetActive(false);
                bg2.SetActive(true);
                carSpines_1.SetActive(true);
                carSpines_2.SetActive(true);
                SpineManager.instance.DoAnimation(carSpines_1, "qld", true);
                SpineManager.instance.DoAnimation(carSpines_2, "hld", true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                {
                    wh.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    SpineManager.instance.DoAnimation(wh, "wh", false);
                }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }, 2));
            }
            else if (talkIndex == 3)
            {
                wh.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                float spiTime = SpineManager.instance.DoAnimation(carSpines_1, "qlp", false);
                SpineManager.instance.DoAnimation(carSpines_2, "hlp", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { }, () => { bell.SetActive(false); }, spiTime));
            }
            talkIndex++;
        }

        void DoImgBtn_1Click(GameObject obj)
        {
            imgBtn_1.SetActive(false);
            SoundManager.instance.SetShield(false);
            int idx = int.Parse(obj.name);
            currentCar = idx;
            mono.StartCoroutine(ImgBtn_1Coroutine(idx));
        }

        IEnumerator ImgBtn_1Coroutine(int idx)
        {
            string carIdx = "hd";
            if (idx == 1) carIdx = "qd";
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            float spiTime = SpineManager.instance.DoAnimation(car, carIdx, false);
            yield return new WaitForSeconds(spiTime);
            tiles.GetChild(idx - 1).gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(car, carIdx + 2, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 1, () => { }, () =>
            {
                backBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
            }));
        }

        void DoBackBtnClick(GameObject obj)
        {
            backBtn.SetActive(false);
            SoundManager.instance.SetShield(false);
            mono.StartCoroutine(BackBtnCoroutine());
        }

        IEnumerator BackBtnCoroutine()
        {
            string carIdx = "hd";
            if (currentCar == 1) carIdx = "qd";
            tiles.GetChild(currentCar - 1).gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            float spiTime = SpineManager.instance.DoAnimation(car, carIdx + 4, false);
            yield return new WaitForSeconds(spiTime);
            SpineManager.instance.DoAnimation(car, "animation", false);
            imgBtn_1.SetActive(true);
            SoundManager.instance.SetShield(true);
            SoundManager.instance.ShowVoiceBtn(true);
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
    }
}
