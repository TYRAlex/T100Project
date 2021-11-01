using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course737Part2
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
        bool isKeeping = true;

        RectTransform carRect;

        GameObject frame;
        GameObject land;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            if(!carRect) LoadGame();

            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            carRect = curTrans.Find("car").GetRectTransform();

            frame = curTrans.Find("frame").gameObject;
            land = curTrans.Find("land").gameObject;
        }

        void GameInit()
        {
            talkIndex = 1;

            carRect.gameObject.SetActive(true);
            land.SetActive(true);
            frame.SetActive(false);

            carRect.anchoredPosition = Vector2.left * 1000;
            carRect.GetComponent<SkeletonGraphic>().Initialize(true);

            frame.GetComponent<SkeletonGraphic>().Initialize(true);
            
            land.transform.GetChild(0).GetRawImage().color = Color.white;
            land.transform.GetChild(0).gameObject.SetActive(true);

            land.transform.GetChild(2).GetComponent<SkeletonGraphic>().Initialize(true);
        }

        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
                Max.SetActive(false);
                isPlaying = false;
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);
            SoundManager.instance.ShowVoiceBtn(false);

            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1));

                SpineManager.instance.DoAnimation(land.transform.GetChild(2).gameObject, "hf", false);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                SpineManager.instance.DoAnimation(carRect.gameObject, "w-run1", true);
                carRect.DOAnchorPosX(0, 8f).SetEase(Ease.Linear).OnComplete(()=>
                {
                    mono.StartCoroutine(WaitFor(0.75f, ()=>
                    {
                        ColorDisPlay(land.transform.GetChild(0).GetRawImage(), false);
                    }));

                    mono.StartCoroutine(SpeakAndAnimation(carRect.gameObject, b: "w1", index: 5, method: () =>
                    {
                        SpineManager.instance.DoAnimation(carRect.gameObject, "w-run2", true);

                        carRect.DOAnchorPosX(250, 2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(carRect.gameObject, "w2", false, ()=>
                            {
                                SpineManager.instance.DoAnimation(carRect.gameObject, "w4");

                                carRect.DOAnchorPosX(450, 1.5f).SetEase(Ease.Linear).OnComplete(()=>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);

                                    SpineManager.instance.DoAnimation(carRect.gameObject, "w3", false, ()=>
                                    {
                                        SoundManager.instance.ShowVoiceBtn(true);
                                    });
                                });
                            });
                        });
                    }));
                });
            }

            if (talkIndex == 2)
            {
                isKeeping = false;

                carRect.gameObject.SetActive(false);
                land.SetActive(false);
                frame.SetActive(true);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                mono.StartCoroutine(SpeakAndAnimation(frame, b: "lc", index: 2, method: () =>
                {
                    mono.StartCoroutine(WaitFor(2f, () =>
                    {
                        frame.GetComponent<SkeletonGraphic>().Initialize(true);

                        mono.StartCoroutine(SpeakAndAnimation(frame, b: "lc2", index: 3, method: () =>
                        {
                            mono.StartCoroutine(WaitFor(2f, () =>
                            {
                                mono.StartCoroutine(SpeakAndAnimation(frame, b: "lc3", index: 4, method: () =>
                                {
                                    mono.StartCoroutine(WaitFor(2f, () =>
                                    {
                                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7));
                                    }));
                                }));
                            }));
                        }));
                    }));
                }));
            }

            talkIndex++;
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f, Ease ease = Ease.InOutSine)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(ease).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(ease).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
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
