using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD91262Part1
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject mask;
        GameObject unDragableMask;
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
        int[] answers;
        int[] cardArrary;
        int preCardIndex;//上一个卡牌
        int gameIndex;

        Transform cardTra;
        Transform hpTra;
        Transform starTra;

        GameObject demon;
        GameObject people;
        GameObject boom;
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
            //unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;

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

            //nextButton.SetActive(false);

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
            cardTra = curCanvasTra.Find("Card");
            hpTra = curCanvasTra.Find("Hp");
            starTra = curCanvasTra.Find("star");

            demon = curCanvasTra.Find("Spine/demon").gameObject;
            people = curCanvasTra.Find("Spine/people").gameObject;
            boom = hpTra.Find("boom").gameObject;
        }

        void GameInit()
        {
            isPlaying = false;
            preCardIndex = cardTra.childCount;
            gameIndex = 0;

            InitSpine(demon, "xem");
            InitSpine(people, "ws");
            InitSpine(curCanvasTra.Find("boom").gameObject, "", false);

            #region 生命值
            Transform tra1 = hpTra.GetChild(0);
            Transform tra2 = hpTra.GetChild(1);
            for (int i = 0; i < tra1.childCount; i++)
            {
                tra1.GetChild(i).gameObject.SetActive(true);
                tra2.GetChild(i).gameObject.SetActive(false);
            }

            InitSpine(boom, "", false);
            #endregion

            //洗牌
            int n = cardTra.childCount;
            cardArrary = new int[n];
            for (int i = 0; i < n; i++)
            {
                cardArrary[i] = i + 1;
            }
            Shuffle<int>(ref cardArrary);

            //卡牌
            for (int i = 0; i < cardTra.childCount; i++)
            {
                GameObject obj = cardTra.GetChild(i).gameObject;
                obj.SetActive(true);
                InitSpine(obj, "k-a" + cardArrary[i], false);

                cardTra.GetChild(i).GetComponent<SkeletonGraphic>().material.
                    SetColor("_Color", new Color(1, 1, 1, 0));

                Util.AddBtnClick(obj, ClickCard);
            }

            //设置答案
            answers = new int[] { 0, 7, 0, 0, 5, 4, 9, 1, 0, 6 };

            //星星
            InitSpine(starTra.GetChild(0).gameObject, "", false);
            InitSpine(starTra.GetChild(1).gameObject, "", false);

            demon.SetActive(true);
        }

        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            mono.StartCoroutine(WaitFor(0.85f, ()=>
            {
                //显示所有牌
                mono.StartCoroutine(WaitShowCard());

                mono.StartCoroutine(WaitFor(4f, ()=>
                {
                    //盖上所有牌
                    for (int i = 0; i < cardTra.childCount; i++)
                    {
                        GameObject obj = cardTra.GetChild(i).gameObject;
                        int num = obj.transform.GetSiblingIndex();

                        SpineManager.instance.DoAnimation(obj, "k-b" + cardArrary[num], false);
                    }

                    mono.StartCoroutine(WaitFor(1f, ()=>
                    {
                        isPlaying = false;
                    }));
                }));
            }));
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        #region 游戏方法

        void ClickCard(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            int num = obj.transform.GetSiblingIndex();

            //是否是点击翻开的
            if (num != preCardIndex)
            {
                SpineManager.instance.DoAnimation(obj, "k-a" + cardArrary[num], false, () =>
                {
                    //是否之前有翻开的卡牌
                    if (preCardIndex == cardTra.childCount)
                    {
                        preCardIndex = num;//记录
                        isPlaying = false;
                    }
                    else
                    {
                        //答案正确与否
                        if (cardArrary[num] == answers[cardArrary[preCardIndex]])
                        {
                            Transform tra = cardTra.GetChild(preCardIndex);

                            //星星
                            starTra.GetChild(0).position = obj.transform.position;
                            starTra.GetChild(1).position = tra.position;

                            SpineManager.instance.DoAnimation(starTra.GetChild(0).gameObject, "star", false);
                            SpineManager.instance.DoAnimation(starTra.GetChild(1).gameObject, "star", false);

                            //卡牌消失
                            Material material1 = obj.GetComponent<SkeletonGraphic>().material;
                            Material material2 = tra.GetComponent<SkeletonGraphic>().material;

                            mono.StartCoroutine(ChangeSpineAlpha(material1, 0, 0.5f, () => obj.SetActive(false)));
                            mono.StartCoroutine(ChangeSpineAlpha(material2, 0, 0.5f, () =>
                            {
                                cardTra.GetChild(preCardIndex).gameObject.SetActive(false);

                                mono.StartCoroutine(Success());
                            }));
                        }
                        else
                        {
                            //墨水
                            SpineManager.instance.DoAnimation(demon, "xem2", false, () =>
                            {
                                SpineManager.instance.DoAnimation(demon, "xem");

                                SpineManager.instance.DoAnimation(cardTra.GetChild(preCardIndex).gameObject, "k-b" + cardArrary[preCardIndex], false);

                                SpineManager.instance.DoAnimation(obj, "k-b" + cardArrary[num], false, () =>
                                {
                                    preCardIndex = cardTra.childCount;
                                    isPlaying = false;
                                });
                            });
                        }
                    }
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(obj, "k-b" + cardArrary[num], false, () =>
                {
                    preCardIndex = cardTra.childCount;
                    isPlaying = false;
                });
            }
        }

        IEnumerator Success()
        {
            //武生攻击
            float _time = SpineManager.instance.DoAnimation(people, "ws2", false, () =>
            SpineManager.instance.DoAnimation(people, "ws"));

            yield return new WaitForSeconds(1f);

            //小恶魔受伤
            _time = SpineManager.instance.DoAnimation(demon, "xem3");

            ChangeHp();

            yield return new WaitForSeconds(_time * 3);

            if (++gameIndex == 3)
            {
                demon.SetActive(false);
                SpineManager.instance.DoAnimation(curCanvasTra.Find("boom").gameObject, "boom", false);

                //翻开最后的牌
                for (int i = 0; i < cardTra.childCount; i++)
                {
                    GameObject obj = cardTra.GetChild(i).gameObject;
                    if (obj.activeSelf)
                    {
                        int num = obj.transform.GetSiblingIndex();

                        SpineManager.instance.DoAnimation(obj, "k-a" + cardArrary[num], false);
                    }
                }

                yield return new WaitForSeconds(2f);
                GameEnd();
            }
            else
            {
                preCardIndex = cardTra.childCount;
                SpineManager.instance.DoAnimation(demon, "xem");
                isPlaying = false;
            }
        }

        //生命值扣除
        void ChangeHp()
        {
            Transform tra = hpTra.GetChild(0).GetChild(gameIndex);
            boom.transform.position = tra.position + Vector3.up * 25;

            SpineManager.instance.DoAnimation(boom.gameObject, "boom", false,
                () => hpTra.GetChild(1).GetChild(gameIndex).gameObject.SetActive(false));

            hpTra.GetChild(1).GetChild(gameIndex).gameObject.SetActive(true);
            tra.gameObject.SetActive(false);
        }
        #endregion

        #region 游戏通用环节
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            //SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                ddTra.gameObject.SetActive(true);
                ddTra.GetChild(0).gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    btn03.SetActive(false);
                    ddTra.GetChild(0).gameObject.SetActive(false);

                    StartGame();
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
            Debug.LogError("1");
            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                MaskInit();
                GameInit();


                StartGame();
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
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.COMMONVOICE, 0));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        #endregion

        #region 通用方法
        //协程:间隔播放显示卡牌
        IEnumerator WaitShowCard(Action method = null)
        {
            WaitForSeconds wait = new WaitForSeconds(0.1f);

            int[] array = new int[] {0, 2, 4, 5, 3, 1, 6, 8, 7};

            for (int i = 0; i < array.Length; i++)
            {
                mono.StartCoroutine(ChangeSpineAlpha(cardTra.GetChild(array[i]).GetComponent<SkeletonGraphic>().material, 1, 1.2f));
                yield return wait;
            }
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(new Color(255, 255, 255, 0.4f), _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
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

        //协程:改变Spine透明度
        IEnumerator ChangeSpineAlpha(Material material, float aimAlpha, float time, Action method = null)
        {
            float i = 0;
            float curAlpha = material.GetColor("_Color").a;
            float deltaAlpha = aimAlpha - curAlpha;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                material.SetColor("_Color", new Color(1, 1, 1, curAlpha + deltaAlpha * i / time));

                yield return wait;
                i += Time.fixedDeltaTime;
            }

            method?.Invoke();
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

        //洗牌算法
        void Shuffle<T>(ref T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }
        #endregion
    }
}
