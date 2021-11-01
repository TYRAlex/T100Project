using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course927Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private GameObject spineShow;

        private GameObject spineSlide;

        private GameObject leftBtn;
        private GameObject rightBtn;

        private Image titleImg;

        bool isPlaying = false;
        private int curQuestion = 0;
        public int CurQuestion
        {
            get => curQuestion;

            set
            {
                if (value < 0)
                {
                    curQuestion = 0;
                }
                else if (value >= 6)
                {
                    curQuestion = 5;
                }
                else
                {
                    curQuestion = value;
                }
            }
        }

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("max").gameObject;

            spineShow = curTrans.Find("spineShow").gameObject;
            spineSlide = curTrans.Find("spineSlide").gameObject;
            leftBtn = curTrans.Find("leftBtn").gameObject;
            rightBtn = curTrans.Find("rightBtn").gameObject;

            Util.AddBtnClick(leftBtn, OnClickLeftBtn);
            Util.AddBtnClick(rightBtn, OnClickRightBtn);

            leftBtn.SetActive(false);
            rightBtn.SetActive(false);
            CurQuestion = 0;
            titleImg = curTrans.Find("titleImg").GetImage();
            titleImg.sprite = titleImg.transform.GetComponent<BellSprites>().sprites[CurQuestion];
            titleImg.SetNativeSize();
            titleImg.gameObject.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            GameStart();
        }

        private void OnClickLeftBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            CurQuestion--;
            UpdateTitleImg(obj);
        }


        private void OnClickRightBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            CurQuestion++;
            UpdateTitleImg(obj);
        }

        void UpdateTitleImg(GameObject btnObj)
        {

            if (btnObj)
            {
                btnObj.transform.DOScale(Vector3.one * 0.8f, 0.2f).OnComplete(() => { btnObj.transform.DOScale(Vector3.one, 0.2f); });
            }
            leftBtn.gameObject.SetActive(CurQuestion > 0);
            rightBtn.gameObject.SetActive(CurQuestion < 5);

            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, CurQuestion + 4, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, CurQuestion + 4);


            titleImg.sprite = titleImg.transform.GetComponent<BellSprites>().sprites[CurQuestion];
            titleImg.SetNativeSize();
            SpineManager.instance.DoAnimation(spineSlide, (CurQuestion + 1) + "", true);
            isPlaying = false;
        }



        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;
            SpineManager.instance.DoAnimation(spineShow, "kong", false);
            SpineManager.instance.DoAnimation(spineSlide, "kong", false);

        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(PlaySpine());
        }

        IEnumerator PlaySpine()
        {
            float temTime = 0;
            for (int i = 1; i < 5; i++)
            {
                SpineManager.instance.DoAnimation(Max, "daijishuohua");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, (i - 1));
                temTime = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, (i - 1));
                /*temTime =*/
                SpineManager.instance.DoAnimation(spineShow, i + "", false);
                yield return new WaitForSeconds(temTime);
                SpineManager.instance.DoAnimation(Max, "daiji");
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                Max.SetActive(false);
                titleImg.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(spineShow, "kong", false);
                rightBtn.SetActive(true);
                leftBtn.SetActive(true);
                UpdateTitleImg(null);
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

    }
}
