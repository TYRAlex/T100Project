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
    public class Course833Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private BellSprites bellSprite;
        private Transform panel;
        private GameObject btnBlue;
        private GameObject btnRed;
        private GameObject btnYellow;
        private Transform panel2;
        private GameObject blue;
        private GameObject red;
        private GameObject yellow;
        private GameObject showSpine;
        private GameObject bell;

        private GameObject btnBack;

        bool isPlaying = false;

        private int timeNum = 0;

        private GameObject btnTest;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }
        void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellSprite = Bg.GetComponent<BellSprites>();

            panel = curTrans.Find("panel");
            panel.gameObject.SetActive(true);
            btnBlue = panel.Find("blue").gameObject;
            btnRed = panel.Find("red").gameObject;
            btnYellow = panel.Find("yellow").gameObject;

            panel2 = curTrans.Find("panel2");
            panel2.gameObject.SetActive(false);
            blue = panel2.Find("blue").gameObject;
            red = panel2.Find("red").gameObject;
            yellow = panel2.Find("yellow").gameObject;

            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            showSpine = curTrans.Find("showSpine").gameObject;
            showSpine.SetActive(false);

            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            Util.AddBtnClick(btnBlue,onClickBtnBlue);
            Util.AddBtnClick(btnRed, onClickBtnRed);
            Util.AddBtnClick(btnYellow, onClickBtnYellow);
            Util.AddBtnClick(btnBack, onClickBtnBack);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }

        private void onClickBtnBack(GameObject obj)
        {
            BtnPlaySound();
            Bg.GetComponent<Image>().sprite = bellSprite.sprites[0];
            showSpine.SetActive(false);
            panel.gameObject.SetActive(true);
            btnBack.SetActive(false);
            timeNum++;
            if (timeNum>=3)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        private void onClickBtnBlue(GameObject obj)
        {         
            onClickCommonEvent(1);
           
        }
        private void onClickBtnRed(GameObject obj)
        {
            onClickCommonEvent(2);


        }
        private void onClickBtnYellow(GameObject obj)
        {           
            onClickCommonEvent(3);
                 
        }
        void endSpine() {
            btnBack.SetActive(true);
            isPlaying = false;
        }
        void onClickCommonEvent(int index) {
            if (isPlaying)
                return;
            isPlaying = true;
            Bg.GetComponent<Image>().sprite = bellSprite.sprites[1];
            panel.gameObject.SetActive(false);
            showSpine.SetActive(true);
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            showSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index, () => { SpineManager.instance.DoAnimation(showSpine, index+"", false); }, () => { endSpine(); }));
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<Image>().sprite = bellSprite.sprites[0];
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,0,()=> { isPlaying = true; }, () => { isPlaying = false;bell.SetActive(false); }));
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {
                panel.gameObject.SetActive(false);
                panel2.gameObject.SetActive(true);
                bell.SetActive(true);
                showSpine.SetActive(false);
                
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(blue, "1", false);
                    SpineManager.instance.DoAnimation(red, "2", false);
                    SpineManager.instance.DoAnimation(yellow, "3", false);
                }, null ));
                

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
