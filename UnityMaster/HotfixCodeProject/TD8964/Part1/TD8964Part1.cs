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
    public class TD8964Part1
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject mask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform ddTra;
        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        bool isPlaying = false;
        #endregion

        #region 游戏变量
        int gameIndex;
        int clickNumber;
        int musicNumber;
        int roundMusic;
        bool isPlayed;

        SkeletonGraphic[] lightSkes;
        CustomImage[] Clicks;

        Transform lampTra;
        Text text;

        SkeletonGraphic startSke;
        SkeletonGraphic haloSke;

        BellSprites bellSprites;

        Dictionary<int, int[]> _answerDic;
        #endregion

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;

            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            if (!ddTra)
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

            ddTra = maskTra.Find("DD");

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
            //unDragableMask.SetActive(false);
            mask.SetActive(true);

            ddTra.GetChild(0).gameObject.SetActive(false);
            ddTra.GetChild(1).gameObject.SetActive(false);

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
            lightSkes = curCanvasTra.Find("lights").GetComponentsInChildren<SkeletonGraphic>(true);
            Clicks = curCanvasTra.Find("People").GetComponentsInChildren<CustomImage>(true);

            lampTra = curCanvasTra.Find("lamp");
            text = curCanvasTra.Find("UI/Text").GetText();

            haloSke = curCanvasTra.Find("halo").GetComponent<SkeletonGraphic>();
            startSke = curCanvasTra.Find("start").GetComponent<SkeletonGraphic>();

            bellSprites = curCanvasTra.Find("Bg").GetComponent<BellSprites>();

            //答案
            _answerDic = new Dictionary<int, int[]>
            {
                { 0, new int[2] { 1 , 2 } },
                { 1, new int[2] { 0 , 0 } },
                { 2, new int[2] { 2 , 0 } },
                { 3, new int[3] { 1, 1 , 2 } },
                { 4, new int[3] { 2, 1 , 0 } },
                { 5, new int[4] { 2, 2 , 0 , 1} }
            };
        }

        void GameInit()
        {
            talkIndex = 1;
            clickNumber = 0;
            gameIndex = 0;
            roundMusic = musicNumber = 0;
            isPlayed = false;

            startSke.Initialize(true);
            haloSke.Initialize(true);

            text.text = "" + musicNumber;

            for (int i = 0; i < lightSkes.Length; ++i)
            {
                InitSpine(lightSkes[i].gameObject, "", false);
            }

            for (int i = 0; i < Clicks.Length; ++i)
            {
                SkeletonGraphic ske = Clicks[i].transform.parent.GetComponent<SkeletonGraphic>();
                ske.startingAnimation = ske.name + "1";
                ske.Initialize(true);
                Util.AddBtnClick(Clicks[i].gameObject, ClickCharacter);
            }

            InitSpine(curCanvasTra.Find("Bg/Bg").gameObject, "bj");
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        #region 游戏通用环节
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                ddTra.gameObject.SetActive(true);
                ddTra.GetChild(0).gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.VOICE, 2, null, () =>
                    {
                        mask.SetActive(false);
                        btn03.SetActive(false);
                        ddTra.GetChild(0).gameObject.SetActive(false);

                        mono.StartCoroutine(WaitChangeLight(() =>
                        {
                            mono.StartCoroutine(Round(0));
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

                mask.SetActive(false);

                mono.StartCoroutine(WaitChangeLight(() =>
                {
                    mono.StartCoroutine(Round(0));
                }));
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.VOICE, 1));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }
        #endregion

        #region 游戏方法
        //点击人物
        void ClickCharacter(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            Transform tra = obj.transform.parent;
            string animationName = tra.name;

            int num = tra.GetSiblingIndex();

            //是否正确
            if(num == _answerDic[gameIndex][clickNumber])
            {
                //人物撞击唱片(正确)
                float _time = SpineManager.instance.DoAnimation(tra.gameObject, animationName + 2, false, ()=>
                {
                    SpineManager.instance.DoAnimation(tra.gameObject, animationName + 1);
                });

                //唱片动画
                mono.StartCoroutine(WaitFor(_time / 2, ()=>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                    SpineManager.instance.DoAnimation(lampTra.GetChild(num).gameObject, "d" + animationName, false, () =>
                    {
                        SpineManager.instance.DoAnimation(lampTra.GetChild(num).gameObject, "d" + animationName + "2", false);
                    });

                    //左上方数字变化动画
                    text.text = ++musicNumber + "";
                    TextEnlarge(text.transform);
                    TextColorDisPlay(text);
                }));

                //数据处理
                mono.StartCoroutine(WaitFor(_time, () =>
                {
                    //是否是这轮的最后一个
                    if (++clickNumber == _answerDic[gameIndex].Length)
                    {
                        //游戏是否结束
                        if (++gameIndex == _answerDic.Count)
                        {
                            GameEnd();
                        }
                        else
                        {
                            isPlayed = false;
                            mono.StartCoroutine(Round(gameIndex));
                        }
                    }
                    else
                    {
                        isPlaying = false;
                    }
                }));
            }
            else
            {
                //人物撞击唱片(错误)
                float _time = SpineManager.instance.DoAnimation(tra.gameObject, animationName + 3, false, ()=>
                {
                    SpineManager.instance.DoAnimation(tra.gameObject, animationName + 1);
                });

                //晕眩动画
                mono.StartCoroutine(WaitFor(_time - 1.4f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

                    haloSke.transform.position = new Vector2(tra.position.x, haloSke.transform.position.y);

                    SpineManager.instance.DoAnimation(haloSke.gameObject, "xem-h", false, ()=>
                    {
                        haloSke.Initialize(true);
                    });
                }));

                //重玩这轮
                mono.StartCoroutine(WaitFor(_time, ()=>
                {
                    //左上方数字变化动画
                    if(musicNumber != roundMusic)
                    {
                        musicNumber = roundMusic;
                        text.text = musicNumber + "";

                        TextEnlarge(text.transform);
                        TextColorDisPlay(text, method: () =>
                        {
                            mono.StartCoroutine(Round(gameIndex));
                        });
                    }
                    else
                    {
                        mono.StartCoroutine(Round(gameIndex));
                    }
                }));
            }
        }

        //一回合开始
        IEnumerator Round(int num)
        {
            float _time = 0f;
            clickNumber = 0;//重置

            int temp = 4;

            //如果是错误重玩的话跳过Round动画
            if(!isPlayed)
            {
                if (num == 0) temp = 1;
                else if (num == 3) temp = 2;
                else if (num == 5) temp = 3;
            }

            if (temp < 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _time = SpineManager.instance.DoAnimation(startSke.gameObject, "r" + temp, false) + 0.5f;
            }
            yield return new WaitForSeconds(_time);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            _time = SpineManager.instance.DoAnimation(startSke.gameObject, "r0", false);
            yield return new WaitForSeconds(_time);

            startSke.Initialize(true);

            for (int i = 0; i < _answerDic[num].Length; i++)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                GameObject obj = lightSkes[_answerDic[num][i]].gameObject;
                _time = SpineManager.instance.DoAnimation(obj, "d" + obj.name, false);

                yield return new WaitForSeconds(_time + 0.3f);
            }

            isPlayed = true;
            isPlaying = false;

            //暂时存储
            roundMusic = musicNumber;
        }

        //间断随机开关灯
        IEnumerator WaitChangeLight(Action method = null)
        {
            WaitForSeconds wait = new WaitForSeconds(0.6f);

            for (int i = 0; i < 2; ++i)
            {
                List<SkeletonGraphic> skes = new List<SkeletonGraphic>();

                for (int j = 0; j < lightSkes.Length; j++)
                {
                    skes.Add(lightSkes[j]);
                }

                for (int m = 0; m < lightSkes.Length; ++m)
                {
                    int num = Random.Range(0, skes.Count);

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    GameObject obj = skes[num].gameObject;
                    SpineManager.instance.DoAnimation(obj, "d" + obj.name, false);

                    skes.RemoveAt(num);
                    yield return wait;
                }

                yield return wait;
            }

            yield return new WaitForSeconds(1f);

            method?.Invoke();
        }

        #endregion

        #region 通用方法
        //字体放大出现
        void TextEnlarge(Transform tra, bool isEnlarge = true, float time = 0.5f, Action method = null)
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
        void TextColorDisPlay(Text text, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                text.color = new Color(255, 255, 255, 0);
                text.gameObject.SetActive(true);
                text.DOColor(Color.white, _time).SetEase(Ease.OutExpo).OnComplete(() => method?.Invoke());
            }
            else
            {
                text.color = Color.white;
                text.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    text.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = ddTra.GetChild(0).gameObject;

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

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }
        #endregion
    }
}
