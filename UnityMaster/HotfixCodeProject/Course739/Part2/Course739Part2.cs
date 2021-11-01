using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course739Part2
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
        RawImage text1;
        RawImage text2;
        RawImage box1;
        RawImage box2;
        RawImage box3;

        RectTransform lampRec;

        BellSprites textSpr;
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

            LoadGame();
            GameInit();
            GameStart();
        }

        void LoadGame()
        {
            text1 = curTrans.Find("TextBg/Mask/text1").GetRawImage();
            text2 = curTrans.Find("TextBg/Mask/text2").GetRawImage();

            textSpr = text1.GetComponent<BellSprites>();

            box1 = curTrans.Find("box1").GetRawImage();
            box2 = curTrans.Find("box2").GetRawImage();
            box3 = curTrans.Find("box3").GetRawImage();

            lampRec = curTrans.Find("lamp").GetRectTransform();
        }

        private void GameInit()
        {
            talkIndex = 1;

            text1.texture = textSpr.texture[3];
            text1.SetNativeSize();
            text1.transform.GetRectTransform().anchoredPosition = Vector2.zero;

            text2.transform.GetRectTransform().anchoredPosition = Vector2.up * 120;

            box1.gameObject.SetActive(false);
            box2.gameObject.SetActive(false);
            box3.gameObject.SetActive(false);

            curTrans.Find("box4").gameObject.SetActive(false);
            curTrans.Find("box5").gameObject.SetActive(false);

            curTrans.Find("TextBg").gameObject.SetActive(false);

            lampRec.anchoredPosition = Vector2.zero;

            Util.AddBtnClick(lampRec.gameObject, LampClick);
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

        void LampClick(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            lampRec.DOAnchorPosX(-700, 1f).SetEase(Ease.InOutSine).OnComplete(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                ColorDisPlay(box1, true, () =>
                {
                    mono.StartCoroutine(WaitFor(4f, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                        ColorDisPlay(box2, method: () =>
                        {
                            mono.StartCoroutine(WaitFor(4f, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                                ColorDisPlay(box2, false);
                            }));
                        });
                    }));
                });

                ColorDisPlay(curTrans.Find("TextBg").GetRawImage());

                ColorDisPlay(curTrans.Find("TextBg/Mask/text1").GetRawImage());

                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    mono.StartCoroutine(WaitFor(2f, () =>
                    {
                        TextChange(text1, text2, 2);

                        ColorDisPlay(box1, false);
                        ColorDisPlay(box3, true);

                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () =>
                        {
                            mono.StartCoroutine(WaitFor(2f, () =>
                            {
                                TextChange(text2, text1, 1);

                                ColorDisPlay(curTrans.Find("box4").GetRawImage());

                                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
                                {
                                    mono.StartCoroutine(WaitFor(2f, () =>
                                    {
                                        TextChange(text1, text2, 0);

                                        ColorDisPlay(curTrans.Find("box4").GetRawImage(), false);
                                        ColorDisPlay(box3, false);

                                        mono.StartCoroutine(WaitFor(0.3f, ()=>
                                        {
                                            ColorDisPlay(curTrans.Find("box5").GetRawImage());

                                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                                            {
                                                mono.StartCoroutine(WaitFor(2f, () =>
                                                {
                                                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5));
                                                }));
                                            }));
                                        }));
                                    }));
                                }));
                            }));
                        }));
                    }));
                }));
            });
        }

        //文字切换上下
        void TextChange(RawImage _text1, RawImage _text2, int n)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

            _text2.transform.GetRectTransform().anchoredPosition = Vector2.up * 120;
            _text2.texture = textSpr.texture[n];
            _text2.SetNativeSize();

            _text1.transform.GetRectTransform().DOAnchorPosY(-120, 0.5f).SetEase(Ease.InOutSine);
            _text2.transform.GetRectTransform().DOAnchorPosY(0, 0.5f).SetEase(Ease.InOutSine);
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
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
    }
}
