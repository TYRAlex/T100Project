using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD8954Part1
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject dd;
        GameObject mask;
        GameObject unDragableMask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int number = 0;
        int gameIndex = 0;

        Text numText;
        Text speakText;

        GameObject demon;
        GameObject one;
        GameObject up;
        GameObject down;
        GameObject submit;
        GameObject trueAnimation;
        GameObject colouredRibbon;

        Transform dialogTra;
        Transform twoTra;

        RectTransform screenRect;
        RectTransform uiRect;
        #endregion

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            if (!dd)
            {
                LoadMask();

                LoadGame();//加载
            }

            MaskInit();

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;

            dd = maskTra.Find("DD").gameObject;

            nextButton = maskTra.Find("NextButton").gameObject;

            successSpine = maskTra.Find("successSpine").gameObject;

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GamePlay);
            //Util.AddBtnClick(nextButton, NextGame);
        }

        void MaskInit()
        {
            mask.SetActive(true);

            dd.transform.GetRectTransform().anchoredPosition = new Vector2(270, -206);
            dd.transform.localScale = new Vector2(1.2f, 1.2f);
            dd.SetActive(false);

            nextButton.SetActive(false);

            successSpine.SetActive(false);

            caidaiSpine.SetActive(false);

            btn01.GetComponent<SkeletonGraphic>().Initialize(true);
            btn02.GetComponent<SkeletonGraphic>().Initialize(true);
            btn03.GetComponent<SkeletonGraphic>().Initialize(true);
            nextButton.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(nextButton, "next2", false);
            SpineManager.instance.DoAnimation(btn01, "next2", false);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);
        }

        void LoadGame()
        {
            demon = curCanvasTra.Find("Demon").gameObject;

            numText = curCanvasTra.Find("UI/Button/text").GetText();
            speakText = curCanvasTra.Find("Demon/dialog/text").GetText();

            dialogTra = demon.transform.Find("dialog");

            one = curCanvasTra.Find("One").gameObject;
            twoTra = curCanvasTra.Find("Two");

            screenRect = curCanvasTra.Find("Screen").GetRectTransform();
            uiRect = curCanvasTra.Find("UI").GetRectTransform();

            up = curCanvasTra.Find("UI/Button/up").gameObject;
            down = curCanvasTra.Find("UI/Button/down").gameObject;

            colouredRibbon = curCanvasTra.Find("caidaiSpine").gameObject;
            submit = curCanvasTra.Find("UI/submit").gameObject;
            trueAnimation = curCanvasTra.Find("Screen/True").gameObject;

            Util.AddBtnClick(up, ClickButton);
            Util.AddBtnClick(down, ClickButton);
            Util.AddBtnClick(submit, Submit);
        }

        void GameInit()
        {
            talkIndex = 1;
            gameIndex = 0;
            number = 0;

            dialogTra.localScale = Vector3.zero;

            numText.transform.localScale = Vector3.one;
            numText.color = Color.white;
            numText.text = "0";

            speakText.transform.localScale = Vector3.one;
            speakText.color = Color.white;
            speakText.text = "0";

            screenRect.anchoredPosition = Vector2.up * 50;
            uiRect.anchoredPosition = Vector2.down * 300;

            demon.transform.GetRectTransform().anchoredPosition = new Vector2(300, 500);

            one.SetActive(true);
            twoTra.gameObject.SetActive(false);
            demon.transform.Find("hand").gameObject.SetActive(false);

            demon.GetComponent<SkeletonGraphic>().Initialize(true);
            one.GetComponent<SkeletonGraphic>().Initialize(true);
            trueAnimation.GetComponent<SkeletonGraphic>().Initialize(true);
            colouredRibbon.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(demon, "xem1");
            SpineManager.instance.DoAnimation(one, "ren3");
            SpineManager.instance.DoAnimation(up, "up2", false);
            SpineManager.instance.DoAnimation(down, "down3", false);
            SpineManager.instance.DoAnimation(submit, "tj2", false);
            SpineManager.instance.DoAnimation(screenRect.gameObject, "bu");
        }

        void MaskStart()
        {

            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        #region mask
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                dd.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 4, null, () =>
                    {
                        mask.SetActive(false);
                        btn03.SetActive(false);

                        isPlaying = false;

                        dd.SetActive(false);

                        mono.StartCoroutine(WaitFor(1f, ()=>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                            screenRect.DOAnchorPosY(950, 2f).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                mono.StartCoroutine(CountDown(10));
                            });
                        }));
                    }));
                }));
            });

        }

        void GameEnd()
        {
            isPlaying = true;

            mono.StartCoroutine(WaitFor(2f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
                dd.SetActive(false);
                successSpine.SetActive(true);
                caidaiSpine.SetActive(true);

                SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

                SpineManager.instance.DoAnimation(successSpine, "6-12-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "6-12-z2", false, () =>
                    {
                        successSpine.SetActive(false);
                        caidaiSpine.SetActive(false);

                        SpineManager.instance.DoAnimation(btn01, "fh2", false);
                        SpineManager.instance.DoAnimation(btn02, "ok2", false);

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        isPlaying = false;
                    });
                });
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                MaskInit();

                GameInit();

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                mono.StartCoroutine(WaitFor(1f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                    screenRect.DOAnchorPosY(950, 2f).SetEase(Ease.Linear).OnComplete(() =>
                    {
                        mono.StartCoroutine(CountDown(10));
                    });
                }));

                mask.SetActive(false);
                dd.SetActive(false);

                isPlaying = false;
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            dd.transform.GetRectTransform().anchoredPosition = new Vector2(980, -239);
            dd.transform.localScale = Vector2.one * 1.5f;

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 3));

                dd.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }
        #endregion

        //加减按钮
        void ClickButton(GameObject obj)
        {
            if (isPlaying || (obj.name == "up" && number == 9) || (obj.name == "down" && number == 0)) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            numText.text = obj.name == "up" ? ++number + "" : --number + "";

            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (number == 9)
                {
                    SpineManager.instance.DoAnimation(up, "up3");
                    SpineManager.instance.DoAnimation(down, "down2");
                }
                else if (number == 0)
                {
                    SpineManager.instance.DoAnimation(down, "down3");
                    SpineManager.instance.DoAnimation(up, "up2");
                }
                else
                {
                    SpineManager.instance.DoAnimation(down, "down2");
                    SpineManager.instance.DoAnimation(up, "up2");
                }

            });

            Enlarge(numText.transform);

            ColorDisPlay(numText, method: () => isPlaying = false);
        }

        //点击提交
        void Submit(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            bool isTrue = false;

            SpineManager.instance.DoAnimation(submit, "tj", false, () =>
            {
                SpineManager.instance.DoAnimation(submit, "tj2", false);

                uiRect.DOAnchorPosY(-300, 1f).SetEase(Ease.InBack);
            });

            switch (gameIndex)
            {
                case 0:
                    isTrue = number == 4;
                    break;
                case 1:
                    isTrue = number == 6;
                    break;
                case 2:
                    isTrue = number == 9;
                    break;
            }

            if (isTrue)
            {
                //胜利
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                ++gameIndex;

                string animation = gameIndex == 2 ? "shou2" : "shou";

                GameObject handObj = demon.transform.Find("hand").gameObject;
                handObj.SetActive(true);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                SpineManager.instance.DoAnimation(handObj, animation, false, () =>
                {
                    handObj.SetActive(false);
                    SpineManager.instance.DoAnimation(handObj, "kong", false);
                    SpineManager.instance.DoAnimation(demon, "xem-y");

                    SpineManager.instance.DoAnimation(trueAnimation, "cut", false);

                    SpineManager.instance.DoAnimation(colouredRibbon, "sp", false, () =>
                    {
                        SpineManager.instance.DoAnimation(colouredRibbon, "kong", false);

                        if (gameIndex == 1) SpineManager.instance.DoAnimation(one, "ren1");
                        if (gameIndex == 2)
                        {
                            one.SetActive(false);
                            twoTra.gameObject.SetActive(true);

                            SpineManager.instance.DoAnimation(twoTra.GetChild(0).gameObject, "ren4");
                            SpineManager.instance.DoAnimation(twoTra.GetChild(1).gameObject, "ren2");
                        }
                        if (gameIndex == 3)
                        {
                            SpineManager.instance.DoAnimation(demon, "xem-y", false, () =>
                            {
                                SpineManager.instance.DoAnimation(demon, "xem-y2");

                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                                demon.transform.GetRectTransform().DOAnchorPosY(110, 1f).SetEase(Ease.OutBounce);

                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                                screenRect.DOAnchorPosY(950, 2f).SetEase(Ease.Linear).OnComplete(() =>
                                {
                                    mono.StartCoroutine(WaitFor(2f, () =>
                                    {
                                        GameEnd();
                                    }));
                                });
                            });

                            return;
                        }

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                        screenRect.DOAnchorPosY(950, 2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            mono.StartCoroutine(CountDown(gameIndex == 2 ? 15 : 10));

                            trueAnimation.GetComponent<SkeletonGraphic>().Initialize(true);
                        });

                        SpineManager.instance.DoAnimation(demon, "xem1");
                    });
                });

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                //失败

                SpineManager.instance.DoAnimation(demon, "xem-jx", false, () =>
                {
                    SpineManager.instance.DoAnimation(demon, "xem-jx", false, () =>
                    {
                        SpineManager.instance.DoAnimation(demon, "xem1");

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                        screenRect.DOAnchorPosY(950, 2f).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            mono.StartCoroutine(CountDown(gameIndex == 2 ? 15 : 10));
                        });

                        isPlaying = false;
                    });
                });
            }
        }

        //对话框动画
        void DialogBox(Transform tra, bool isShow = true)
        {
            if (isShow)
            {
                tra.DOScale(Vector2.one * 1.1f, 0.3f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    tra.DOScale(Vector2.one, 0.15f).SetEase(Ease.InOutSine);
                });
            }
            else
            {
                tra.DOScale(Vector2.one * 1.1f, 0.15f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    tra.DOScale(Vector2.zero, 0.3f).SetEase(Ease.InOutSine);
                });
            }

        }

        //字体放大
        void Enlarge(Transform tra, bool isEnlarge = true, float time = 0.5f, Action method = null)
        {
            Vector2 curScale = Vector2.one;

            tra.localScale = Vector3.zero;

            if (isEnlarge)
            {
                tra.DOScale(curScale, time).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    method?.Invoke();
                });
            }
        }

        //字体渐变
        void ColorDisPlay(Text raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutExpo).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //对话框倒计时
        IEnumerator CountDown(int num)
        {
            isPlaying = true;
            WaitForSeconds wait = new WaitForSeconds(1f);

            if (num == 15) SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            if (num == 10) SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);

            speakText.text = "" + num;
            DialogBox(dialogTra);

            yield return wait;

            while (--num > 0)
            {
                speakText.text = "" + num;

                Enlarge(speakText.transform);
                ColorDisPlay(speakText);

                yield return wait;
            }

            DialogBox(dialogTra, false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

            screenRect.DOAnchorPosY(50, 2f).SetEase(Ease.Linear).
                OnComplete(() => uiRect.DOAnchorPosY(0, 1f).SetEase(Ease.OutBack).
                OnComplete(()=> isPlaying = false));
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = dd;

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");


            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }
    }
}
