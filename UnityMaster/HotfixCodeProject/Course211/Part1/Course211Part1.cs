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
    public class Course211Part1
    {
        private GameObject bell;
        private GameObject bellBg;
        private GameObject imgBtn;
        private GameObject car;
        private GameObject cun;
        private GameObject cunBtn;
        private GameObject replayBtn;

        BellSprites _bgImage;

        private int talkIndex;
        private string[] spiName;
        private bool isDown;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;

            Transform curTrans = curGo.transform;

            bellBg = curTrans.Find("Bell").gameObject;
            bell = curTrans.Find("Bell/Bell").gameObject;
            imgBtn = curTrans.Find("ImgBtn").gameObject;
            cunBtn = curTrans.Find("CunBtn").gameObject;
            car = curTrans.Find("Car").gameObject;
            cun = curTrans.Find("Cun").gameObject;
            replayBtn = curTrans.Find("ReplayBtn").gameObject;
            _bgImage = curTrans.GetGameObject("bg").GetComponent<BellSprites>();
            Button[] imgBtnChild = imgBtn.transform.GetComponentsInChildren<Button>();
            for(int i = 0; i < imgBtnChild.Length; i++)
            {
                Util.AddBtnClick(imgBtnChild[i].gameObject, DoImgBtnClick);
            }
            Button[] cunBtnChild = cunBtn.transform.GetComponentsInChildren<Button>();
            for (int i = 0; i < cunBtnChild.Length; i++)
            {
                Util.AddBtnClick(cunBtnChild[i].gameObject, DoCunBtnClick);
            }
            Util.AddBtnClick(replayBtn, DoReplayBtnClick);

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            talkIndex = 1;
            isDown = true;
            string[] spiName_test = {"4","3","1","2" };
            spiName = spiName_test;
            _bgImage.gameObject.GetComponent<Image>().sprite = _bgImage.sprites[0];
            bellBg.SetActive(true);
            imgBtn.SetActive(false);
            cunBtn.SetActive(false);
            car.SetActive(true);
            cun.SetActive(false);
            replayBtn.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(car, "daiji", true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                bellBg.SetActive(false);
                imgBtn.SetActive(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                Talk_1_Event();
            }
            else if (talkIndex == 2)
            {
                Talk_2_Event();
            }
            else if (talkIndex == 3)
            {
                _bgImage.gameObject.GetComponent<Image>().sprite = _bgImage.sprites[1];
                replayBtn.SetActive(false);
                car.SetActive(false);
                cun.SetActive(true);
                bellBg.SetActive(true);
                imgBtn.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => { }, () =>
                {
                    cunBtn.SetActive(true);
                }));

            }
            else if (talkIndex == 4)
            {
 
                cunBtn.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                    cunBtn.SetActive(true);
                }));
            }
            else if (talkIndex == 5)
            {
                cunBtn.SetActive(false);
                
                //cun.SetActive(false);
                _bgImage.gameObject.GetComponent<Image>().sprite = _bgImage.sprites[0];
                cun.transform.GetComponent<SkeletonGraphic>().CrossFadeAlpha(0, 0.5f, true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, () =>
                {
                    car.SetActive(true);
                }, () =>
                {
                    imgBtn.SetActive(true);
                }, 0.5f));
            }
            talkIndex++;
        }

        void Talk_1_Event()
        {
            imgBtn.SetActive(false);
            replayBtn.SetActive(false);
            float spiTime = SpineManager.instance.DoAnimation(car, "qingsao", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () =>
            {

            }, () =>
            {
                SpineManager.instance.DoAnimation(car, "daiji", true);
                SoundManager.instance.ShowVoiceBtn(true);
                replayBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
            }, spiTime));
        }

        void Talk_2_Event()
        {
            replayBtn.SetActive(false);
            float spiTime = SpineManager.instance.DoAnimation(car, "qingsao2", true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => {
                SpineManager.instance.DoAnimation(car, "daiji", true);
            }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
                replayBtn.SetActive(true);
                SoundManager.instance.SetShield(true);
            }, 2 * spiTime));
        }

        void DoImgBtnClick(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            imgBtn.SetActive(false);
            int idx = int.Parse(obj.name);
            float spiTime = SpineManager.instance.DoAnimation(car, spiName[idx], false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, idx + 1, () => {
                SpineManager.instance.DoAnimation(car, "daiji", true);
            }, () => {
                SoundManager.instance.SetShield(true);
                imgBtn.SetActive(true);
                if (isDown)
                {
                    isDown = false;
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }, spiTime));
        }

        void DoCunBtnClick(GameObject obj)
        {
            cunBtn.SetActive(false);
            SoundManager.instance.SetShield(false);
            mono.StartCoroutine(CunBtnCoroutine());
      
        }

        IEnumerator CunBtnCoroutine()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10,false);
            SpineManager.instance.DoAnimation(cun, "2", true);
            yield return new WaitForSeconds(4.57f);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SpineManager.instance.DoAnimation(cun, "1", false);
            SoundManager.instance.ShowVoiceBtn(true);
            SoundManager.instance.SetShield(true);
        }

        void DoReplayBtnClick(GameObject obj)
        {
            SoundManager.instance.SetShield(false);
            if(talkIndex == 2)
            {
                Talk_1_Event();
            }else if(talkIndex == 3)
            {
                Talk_2_Event();
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

        void OnDisable()
        {
            mono.StopAllCoroutines();
            SoundManager.instance.Stop();
        }
    }
}
