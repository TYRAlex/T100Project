using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course739Part5
    {
        #region 通用变量
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        GameObject kangaroo;
        GameObject clock;
        GameObject click;
        Transform concertTra;
        Transform imagesTra;

        bool isClick1 = false;
        bool isClick2 = false;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            Bg.SetActive(true);

            clock = curTrans.Find("Clock").gameObject;

            concertTra = curTrans.Find("Concert");

            kangaroo = concertTra.Find("Kangaroo").gameObject;

            imagesTra = curTrans.Find("Images");

            click = curTrans.Find("click").gameObject;
        }

        private void GameInit()
        {
            talkIndex = 1;
            isClick1 = false;
            isClick2 = false;

            click.SetActive(false);

            clock.GetComponent<SkeletonGraphic>().Initialize(true);
            clock.SetActive(false);

            concertTra.gameObject.SetActive(false);
            curTrans.Find("ConcertMask").gameObject.SetActive(false);

            concertTra.Find("ConcertSpine").GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(concertTra.Find("ConcertSpine").gameObject, "b", true);

            concertTra.Find("Dark").gameObject.SetActive(false);

            kangaroo.GetComponent<SkeletonGraphic>().Initialize(true);
            kangaroo.SetActive(false);

            imagesTra.gameObject.SetActive(false);

            imagesTra.GetChild(0).localScale = Vector2.one;
            imagesTra.GetChild(1).localScale = Vector2.one;

            Util.AddBtnClick(Bg.transform.GetChild(0).gameObject, BgClick);
            Util.AddBtnClick(click, ConcertClick);
            Util.AddBtnClick(imagesTra.GetChild(0).gameObject, ImageClick);
            Util.AddBtnClick(imagesTra.GetChild(1).gameObject, ImageClick);
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                isPlaying = false;
            }));
        }

        void BgClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

            Bg.SetActive(false);
            concertTra.gameObject.SetActive(true);
            kangaroo.SetActive(true);
            Max.SetActive(false);

            mono.StartCoroutine(SpeckerCoroutine(concertTra.Find("Bell").gameObject, SoundManager.SoundType.VOICE, 1, null, ()=>
            {
                SpineManager.instance.DoAnimation(concertTra.Find("Bell").gameObject, "DAIJI3");

                click.SetActive(true);
                isPlaying = false;
            }));

            mono.StartCoroutine(WaitFor(2f, ()=>
            {
                SpineManager.instance.DoAnimation(kangaroo, "d", false, ()=>
                {
                    SpineManager.instance.DoAnimation(kangaroo, "e", true);
                });
            }));

            mono.StartCoroutine(WaitFor(13f, () =>
            {
                concertTra.Find("Dark").gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                SpineManager.instance.DoAnimation(concertTra.Find("Bell").gameObject, "DAIJI3");
                SpineManager.instance.DoAnimation(kangaroo, "e2");
            }));
        }

        void ConcertClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            Max.SetActive(true);
            SoundManager.instance.PlayClip(9);

            curTrans.Find("ConcertMask").gameObject.SetActive(true);

            concertTra.gameObject.SetActive(false);
            kangaroo.SetActive(false);
            clock.SetActive(true);

            mono.StartCoroutine(SpeakAndAnimation(clock, b: "c", index: 2, method: ()=>
            {
                isPlaying = false;

                Util.AddBtnClick(click, ClockClick);
            }));
        }

        void ClockClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            clock.SetActive(false);
            imagesTra.gameObject.SetActive(true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, ()=>
            {
                obj.SetActive(false);

                isPlaying = false;
            }));
        }

        void ImageClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            ClickFeedback(obj.transform);

            SoundManager.instance.ShowVoiceBtn(false);

            int index = int.Parse(obj.name);

            if (index == 1) isClick1 = true;
            if (index == 2) isClick2 = true;

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3 + index, null, ()=>
            {
                isPlaying = false;

                if(isClick1 && isClick2) SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.ShowVoiceBtn(false);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6));
        }

        //点击反馈
        void ClickFeedback(Transform tar)
        {
            SoundManager.instance.PlayClip(9);

            Vector2 curScale = tar.localScale;

            tar.DOScale(curScale * 0.9f, 1 / 6f).SetEase(Ease.InOutSine).OnComplete(()=>
            {
                tar.DOScale(curScale, 1 / 3f).SetEase(Ease.InOutSine);
            });
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //播放动画和语音
        IEnumerator SpeakAndAnimation(GameObject obj, string a = null, string b = null, string c = null, int index = -1, Action method = null)
        {
            float _time = 0f;
            if (a != null) _time = SpineManager.instance.DoAnimation(obj, a, false);
            yield return new WaitForSeconds(_time);

            float speakTime = 0f;
            if (index != -1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index));
                speakTime = SoundManager.instance.GetLength(SoundManager.SoundType.VOICE, index);
            }

            float animationTime = 0f;
            if (b != null) animationTime = SpineManager.instance.DoAnimation(obj, b, false);

            _time = speakTime > animationTime ? speakTime : animationTime;
            yield return new WaitForSeconds(_time);

            _time = 0f;
            if (c != null) _time = SpineManager.instance.DoAnimation(obj, c, false);
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }
    }
}
