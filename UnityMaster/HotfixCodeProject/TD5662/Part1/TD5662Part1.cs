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
    public class TD5662Part1
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
        int flag;
        int gameIndex;
        Vector2[] ballsPos;
        bool isMoveBackground;
        bool isFlying;

        Transform bgTra;
        Transform grassTra;
        Transform ballTra1;
        Transform ballTra2;
        Transform glowwormTra1;
        Transform glowwormTra2;
        Transform uiTra;

        BellSprites bgSprites;
        BellSprites grassSprites;
        BellSprites darkBgSprites;
        BellSprites darkGrasSprites;
        BellSprites obstacleSprites;
        BellSprites ballSprites;
        BellSprites textSprites;

        GameObject bird;
        GameObject click;
        GameObject light;
        GameObject end;

        RawImage blackMaskRaw;

        EventDispatcher birdEvent;

        Rigidbody2D birdRigidbody2D;

        RectTransform birdRect;
        RectTransform lightRect;
        RectTransform demonRect;
        RectTransform moonRect;

        Coroutine bg;
        Coroutine grass;
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
            Util.AddBtnClick(nextButton, NextGame);
        }

        void LoadGame()
        {
            bgTra = curCanvasTra.Find("Bg");
            grassTra = curCanvasTra.Find("Grass");

            bgSprites = curCanvasTra.Find("Bg/Bg1").GetComponent<BellSprites>();
            grassSprites = curCanvasTra.Find("Grass/Grass1").GetComponent<BellSprites>();
            darkBgSprites = curCanvasTra.Find("Bg/Bg1/dark").GetComponent<BellSprites>();
            darkGrasSprites = curCanvasTra.Find("Grass/Grass1/dark").GetComponent<BellSprites>();

            bird = curCanvasTra.Find("light/bird").gameObject;
            birdEvent = bird.GetComponent<EventDispatcher>();
            birdEvent.TriggerEnter2D += BirdTiggerEnter;
            birdEvent.CollisionEnter2D += BirdColliderEnter;

            birdRigidbody2D = bird.GetComponent<Rigidbody2D>();

            birdRect = bird.transform.GetRectTransform();

            demonRect = curCanvasTra.Find("demon").GetRectTransform();

            ballTra1 = curCanvasTra.Find("Bg/Bg1/ball");
            ballTra2 = curCanvasTra.Find("Bg/Bg2/ball");

            click = curCanvasTra.Find("click").gameObject;

            light = curCanvasTra.Find("light").gameObject;

            lightRect = light.transform.GetRectTransform();

            glowwormTra1 = bgTra.Find("Bg1/dark/glowworm");
            glowwormTra2 = bgTra.Find("Bg2/dark/glowworm");
            moonRect = curCanvasTra.Find("moon").GetRectTransform();

            obstacleSprites = bgTra.GetChild(0).GetChild(1).GetComponent<BellSprites>();
            ballSprites = ballTra1.GetComponent<BellSprites>();

            end = curCanvasTra.Find("end").gameObject;
            blackMaskRaw = curCanvasTra.Find("blackMask").GetRawImage();

            uiTra = curCanvasTra.Find("UI");
            textSprites = uiTra.Find("Text").GetComponent<BellSprites>();

            Util.AddBtnClick(click, BirdRise);

            //保存球的初始位置
            ballsPos = new Vector2[] {
                Vector2.zero,
                new Vector2(550, 312),
                new Vector2(1100, -80),
                new Vector2(1511, 250),
                new Vector2(2075, -30)};
        }

        void MaskInit()
        {
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

        void GameInit()
        {
            talkIndex = 1;
            isFlying = false;
            flag = 0;

            SpineManager.instance.DoAnimation(bird, "niao");

            InitBall(ballTra1);
            InitBall(ballTra2);

            #region 背景初始化
            RectTransform rect = bgTra.GetChild(0).GetRectTransform();
            rect.anchoredPosition = Vector2.right * 960;
            rect.transform.GetChild(0).GetImage().fillAmount = 0.115f;

            rect = bgTra.GetChild(1).GetRectTransform();
            rect.anchoredPosition = Vector2.right * 4800;
            rect.transform.GetChild(0).GetImage().fillAmount = 0f;

            rect = grassTra.GetChild(0).GetRectTransform();
            rect.anchoredPosition = new Vector2(960, rect.anchoredPosition.y);
            rect.transform.GetChild(0).GetImage().fillAmount = 0.115f;

            rect = grassTra.GetChild(1).GetRectTransform();
            rect.anchoredPosition = new Vector2(4800, rect.anchoredPosition.y);
            rect.transform.GetChild(0).GetImage().fillAmount = 0f;
            #endregion

            birdRigidbody2D.gravityScale = 0;
            birdRect.anchoredPosition = Vector2.right * 50;
            birdRigidbody2D.GetComponent<PolygonCollider2D>().enabled = true;

            demonRect.anchoredPosition = new Vector2(1150, -125);

            MoveObstacle(bgTra.Find("Bg1/Obstacle"), 0);
            MoveObstacle(bgTra.Find("Bg2/Obstacle"), 0);

            light.SetActive(true);
            lightRect.anchoredPosition = Vector2.left * 525;
            SpineManager.instance.DoAnimation(demonRect.gameObject, "xem1");

            //萤火虫初始化
            mono.StartCoroutine(GlowwormAnimation(glowwormTra1.GetChild(0)));
            mono.StartCoroutine(GlowwormAnimation(glowwormTra1.GetChild(1)));
            mono.StartCoroutine(GlowwormAnimation(glowwormTra2.GetChild(0)));
            mono.StartCoroutine(GlowwormAnimation(glowwormTra2.GetChild(1)));

            moonRect.gameObject.SetActive(false);
            blackMaskRaw.gameObject.SetActive(false);
            end.SetActive(false);

            uiTra.GetChild(1).localScale = Vector2.one;
            uiTra.GetChild(1).GetRawImage().color = Color.white;
        }

        void MaskStart()
        {
            gameIndex = 1;
            ChangeImages(gameIndex - 1);

            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);

            RawImage _raw = uiTra.GetChild(0).GetRawImage();
            _raw.texture = _raw.GetComponent<BellSprites>().texture[0];

            _raw = uiTra.GetChild(1).GetRawImage();
            _raw.texture = textSprites.texture[0];
            _raw.SetNativeSize();
        }

        void Update()
        {
            if (birdRect.anchoredPosition.y > 400)
            {
                birdRect.anchoredPosition = new Vector2(birdRect.anchoredPosition.x, 400);
                birdRigidbody2D.velocity = Vector2.zero;
            }
            if (birdRect.anchoredPosition.y < -540) BirdColliderEnter();
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
                    mask.SetActive(false);
                    btn03.SetActive(false);

                    isPlaying = false;

                    ddTra.GetChild(0).gameObject.SetActive(false);

                    float _time1 = 0;

                    switch (gameIndex)
                    {
                        case 1:
                            _time1 = 25f;
                            break;
                        case 2:
                            _time1 = 20f;
                            break;
                        case 3:
                            _time1 = 15f;
                            break;
                    }

                    bg = mono.StartCoroutine(MoveBackground(bgTra, _time1));
                    grass = mono.StartCoroutine(MoveBackground(grassTra, _time1 / 2));

                    birdRigidbody2D.gravityScale = 35f;

                    for (int i = 0; i < ballTra1.childCount; i++)
                    {
                        MoveBall(ballTra1.GetChild(i).GetRectTransform());
                        MoveBall(ballTra2.GetChild(i).GetRectTransform());
                    }
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

                SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false, () =>
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

                gameIndex = 1;

                ChangeImages(gameIndex - 1);

                mask.SetActive(false);

                GameReset(()=> isPlaying = false);

                isPlaying = false;
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

        //下一关
        void NextGame(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(nextButton, "next", false, () =>
            {
                SpineManager.instance.DoAnimation(nextButton, "next2", false);

                mask.SetActive(false);
                nextButton.SetActive(false);
                isPlaying = false;
            });

            ChangeImages(gameIndex - 1);

            GameReset();
        }
        #endregion

        #region 游戏方法
        //小鸟向上飞
        void BirdRise(GameObject obj)
        {
            if (isFlying || isPlaying) return;
            isFlying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
            birdRigidbody2D.velocity = Vector2.up* 400;

            mono.StartCoroutine(WaitFor(0.15f, () => isFlying = false));
        }

        //小鸟碰撞小球
        void BirdTiggerEnter(Collider2D c, int _time)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            int num = c.transform.GetSiblingIndex();
            ballTra1.GetChild(num).gameObject.SetActive(false);
            ballTra2.GetChild(num).gameObject.SetActive(false);

            ++flag;
            ChangeText();

            if (flag == 5) Success();
        }

        //小鸟撞到柱子
        void BirdColliderEnter(Collision2D c = null, int _time = 0)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            isMoveBackground = false;
            
            mono.StopCoroutine(bg);
            mono.StopCoroutine(grass);
            DOTween.Kill("Background");

            bird.GetComponent<PolygonCollider2D>().enabled = false;
            birdRigidbody2D.gravityScale = 0;
            birdRigidbody2D.velocity = Vector2.zero;

            mono.StartCoroutine(WaitFor(3f, () =>
            {
                GameReset(()=> isPlaying = false);
            }));
        }

        //游戏重置
        void GameReset(Action method  = null)
        {
            mono.StopAllCoroutines();
            DOTween.Kill("Ball");

            GameInit();

            ChangeImages(gameIndex - 1);

            method?.Invoke();

            float _time1 = 0;

            switch (gameIndex)
            {
                case 1:
                    _time1 = 25f;
                    break;
                case 2:
                    _time1 = 20f;
                    break;
                case 3:
                    _time1 = 15f;
                    break;
            }

            bg = mono.StartCoroutine(MoveBackground(bgTra, _time1));
            grass = mono.StartCoroutine(MoveBackground(grassTra, _time1 / 2));

            birdRigidbody2D.gravityScale = 35f;

            for (int i = 0; i < ballTra1.childCount; i++)
            {
                MoveBall(ballTra1.GetChild(i).GetRectTransform());
                MoveBall(ballTra2.GetChild(i).GetRectTransform());
            }
        }

        //小鸟捡完全部球之后
        void Success()
        {
            isPlaying = true;

            MoveObstacle(bgTra.Find("Bg1/Obstacle"), 1);
            MoveObstacle(bgTra.Find("Bg2/Obstacle"), 1);

            birdRigidbody2D.gravityScale = 0;
            birdRigidbody2D.velocity = Vector2.zero;
            birdRigidbody2D.GetComponent<PolygonCollider2D>().enabled = false;

            birdRect.DOAnchorPosY(0, 1f).SetEase(Ease.Linear);

            
            demonRect.anchoredPosition = new Vector2(1150, -125);
            demonRect.DOAnchorPosX(450, 2f).SetEase(Ease.Linear).OnComplete(() =>
            {
                //小鸟飞过场景
                isMoveBackground = false;
                mono.StopCoroutine(bg);
                mono.StopCoroutine(grass);
                DOTween.Kill("Background");

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                float _time = SpineManager.instance.DoAnimation(demonRect.gameObject, "xem-jx", false, () =>
                {
                    SpineManager.instance.DoAnimation(demonRect.gameObject, "xem-jx", false);
                });

                mono.StartCoroutine(WaitFor(_time, () =>
                {
                    SpineManager.instance.DoAnimation(demonRect.gameObject, "xem1");

                    ++gameIndex;
                    _time = SpineManager.instance.DoAnimation(bird, "niao" + gameIndex, false, () =>
                    {
                        SpineManager.instance.DoAnimation(bird, "niao");
                    });

                    mono.StartCoroutine(WaitFor(_time - 0.75f, ()=>
                    {
                        //小鸟攻击小恶魔
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    }));

                    mono.StartCoroutine(WaitFor(_time - 0.5f, () =>
                    {
                        SpineManager.instance.DoAnimation(demonRect.gameObject, "xem-y", false, () =>
                        {
                            SpineManager.instance.DoAnimation(demonRect.gameObject, "xem-y2");

                            demonRect.DOAnchorPosY(-850, 1.5f).SetEase(Ease.OutQuad);

                            mono.StartCoroutine(WaitFor(1f, () =>
                            {
                                mono.StartCoroutine(GameSuccess(bgTra));
                                mono.StartCoroutine(GameSuccess(grassTra));

                                light.transform.GetRectTransform().DOAnchorPosX(1395, 4f).SetEase(Ease.Linear);

                                mono.StartCoroutine(WaitFor(3f, () =>
                                {
                                    //月亮出现
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                                    moonRect.anchoredPosition = new Vector2(-500, 200);
                                    moonRect.gameObject.SetActive(true);
                                    InitSpine(moonRect.transform, "moon");
                                    moonRect.DOAnchorPosY(-200, 2f).SetEase(Ease.OutBack);
                                }));

                                mono.StartCoroutine(WaitFor(8f, () =>
                                {
                                    if (gameIndex <= 3)
                                    {
                                        mask.SetActive(true);
                                        nextButton.SetActive(true);
                                        InitSpine(nextButton.transform, "next2");
                                        isPlaying = false;
                                    }
                                    else
                                    {

                                        mono.StartCoroutine(BlackCurtainTransition(blackMaskRaw, () => end.SetActive(true)));
                                        InitSpine(end.transform, "end");
                                        mono.StartCoroutine(WaitFor(5f, () =>
                                        {
                                            GameEnd();
                                        }));
                                    }
                                }));
                            }));
                        });
                    }));

                }));
            });
        }

        //障碍物的移动和复原
        void MoveObstacle(Transform tra, int type = 0)
        {
            switch (type)
            {
                case 0:
                    tra.GetChild(0).GetRectTransform().anchoredPosition = Vector2.zero;
                    tra.GetChild(1).GetRectTransform().anchoredPosition = Vector2.zero;
                    break;
                case 1:
                    tra.GetChild(0).GetRectTransform().DOAnchorPosY(400, 1.5f).SetEase(Ease.InOutSine);
                    tra.GetChild(1).GetRectTransform().DOAnchorPosY(-400, 1.5f).SetEase(Ease.InOutSine);
                    break;
            }
        }

        //切换关卡图片
        void ChangeImages(int num)
        {
            bgTra.GetChild(0).GetRawImage().texture = bgSprites.texture[num];
            bgTra.GetChild(1).GetRawImage().texture = bgSprites.texture[num];

            bgTra.GetChild(0).GetChild(0).GetImage().sprite = darkBgSprites.sprites[num];
            bgTra.GetChild(1).GetChild(0).GetImage().sprite = darkBgSprites.sprites[num];

            RawImage[] raws = bgTra.GetChild(0).GetChild(1).GetComponentsInChildren<RawImage>();
            foreach (var raw in raws)
            {
                raw.texture = obstacleSprites.texture[num];
            }

            raws = bgTra.GetChild(1).GetChild(1).GetComponentsInChildren<RawImage>();
            foreach (var raw in raws)
            {
                raw.texture = obstacleSprites.texture[num];
            }

            grassTra.GetChild(0).GetImage().sprite = grassSprites.sprites[num];
            grassTra.GetChild(1).GetImage().sprite = grassSprites.sprites[num];

            grassTra.GetChild(0).GetChild(0).GetImage().sprite = darkGrasSprites.sprites[num];
            grassTra.GetChild(1).GetChild(0).GetImage().sprite = darkGrasSprites.sprites[num];

            for (int i = 0; i < ballTra1.childCount; ++i)
            {
                ballTra1.GetChild(i).GetRawImage().texture = ballSprites.texture[num * 5 + i];
                ballTra2.GetChild(i).GetRawImage().texture = ballSprites.texture[num * 5 + i];
            }

            RawImage _raw = uiTra.GetChild(0).GetRawImage();
            _raw.texture = _raw.GetComponent<BellSprites>().texture[num];

            _raw = uiTra.GetChild(1).GetRawImage();
            _raw.texture = textSprites.texture[num];
            _raw.SetNativeSize();
        }

        //背景移动及颜色渐变
        IEnumerator MoveBackground(Transform tra, float _time = 16f)
        {
            isMoveBackground = true;

            RectTransform rect1 = tra.GetChild(0).GetRectTransform();
            RectTransform rect2 = tra.GetChild(1).GetRectTransform();

            Image dark1 = rect1.transform.GetChild(0).GetImage();
            Image dark2 = rect2.transform.GetChild(0).GetImage();

            float curFillAmount = dark1.fillAmount;
            float _time1 = (1 - curFillAmount) * _time;
            WaitForSeconds wait1 = new WaitForSeconds(_time1);
            WaitForSeconds wait2 = new WaitForSeconds(_time - _time1);

            while (isMoveBackground)
            {
                float curPos1 = rect1.anchoredPosition.x;
                float curPos2 = rect2.anchoredPosition.x;

                yield return 0;

                rect1.DOAnchorPosX(curPos1 - 3840, _time).SetEase(Ease.Linear).SetId<Tween>("Background");
                rect2.DOAnchorPosX(curPos2 - 3840, _time).SetEase(Ease.Linear).SetId<Tween>("Background");

                yield return dark1.DOFillAmount(1, _time1).SetEase(Ease.Linear).SetId<Tween>("Background").WaitForCompletion();

                yield return dark2.DOFillAmount(curFillAmount, _time - _time1).SetEase(Ease.Linear).SetId<Tween>("Background").WaitForCompletion();

                if (!isMoveBackground) yield break;

                rect1.anchoredPosition = new Vector2(curPos2, rect1.anchoredPosition.y);
                dark1.fillAmount = 0;

                Swap<Image>(ref dark1, ref dark2);
                Swap<RectTransform>(ref rect1, ref rect2);
                tra.GetChild(0).SetAsLastSibling();
                yield return 0;
            }
        }

        //游戏成功时背景移动
        IEnumerator GameSuccess(Transform tra)
        {
            Image dark1 = tra.GetChild(0).GetChild(0).GetImage();
            Image dark2 = tra.GetChild(1).GetChild(0).GetImage();

            float curFillAmount = dark1.fillAmount;

            if (curFillAmount > 0.5f)
            {
                float _time = 4f * ((1f - curFillAmount) / 0.5f);
                dark1.DOFillAmount(1f, _time).SetEase(Ease.Linear);

                yield return new WaitForSeconds(_time);

                dark2.DOFillAmount(curFillAmount - 0.5f, 4f - _time).SetEase(Ease.Linear);
            }
            else
            {
                dark1.DOFillAmount(0.5f + curFillAmount, 4f).SetEase(Ease.Linear);
            }

        }

        //球的初始化
        void InitBall(Transform tra)
        {
            tra.GetRectTransform().anchoredPosition = Vector2.left * 960;

            for (int i = 0; i < tra.childCount; ++i)
            {
                tra.GetChild(i).gameObject.SetActive(true);

                tra.GetChild(i).GetRectTransform().anchoredPosition =
                    new Vector2(ballsPos[i].x + Random.Range(-50f, 50f),
                    ballsPos[i].y + Random.Range(-50f, 50f));
            }
        }

        //球上下移动(随机化)
        void MoveBall(RectTransform rect)
        {
            //随机距离 时间 方向
            int type = Random.Range(-1, 1);
            if(type == 0) type = 1;

            float distance = rect.anchoredPosition.y + type * Random.Range(80, 120);
            Debug.Log(distance);
            float _time = Random.Range(0.8f, 1.2f);

            rect.DOAnchorPosY(distance, _time).SetEase(Ease.InOutSine).SetId<Tween>("Ball").SetLoops(-1, LoopType.Yoyo);
        }

        //萤火虫动画
        IEnumerator GlowwormAnimation(Transform tra)
        {
            //随机数组
            int n = tra.childCount;
            int[] nums = new int[n];
            for (int i = 0; i < n; i++)
            {
                nums[i] = i;
            }
            Shuffle<int>(ref nums);

            float _time = SpineManager.instance.GetAnimationLength(tra.GetChild(0).gameObject, "yhc") / n;
            WaitForSeconds wait = new WaitForSeconds(_time);
            
            for (int i = 0; i < n; i++)
            {
                SpineManager.instance.DoAnimation(tra.GetChild(nums[i]).gameObject, "yhc", true);
                yield return wait;
            }
        }

        //改变数字
        void ChangeText()
        {
            RawImage textRaw = uiTra.GetChild(1).GetRawImage();

            TextEnlarge(uiTra.GetChild(1));
            ColorDisPlay(textRaw);

            textRaw.texture = textSprites.texture[flag * 3 + gameIndex - 1];
            textRaw.SetNativeSize();
        }
        #endregion

        #region 通用方法
        //黑幕转场
        IEnumerator BlackCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(0, 0, 0, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.black, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.2f);

            _raw.DOColor(new Color(0, 0, 0, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
        }

        //字体放大
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

        //交换
        void Swap<T>(ref T a, ref T b)
        {
            T c = a;
            a = b;
            b = c;
        }

        //Spine初始化
        void InitSpine(Transform _tra, string animation)
        {
            SkeletonGraphic _ske = _tra.GetComponent<SkeletonGraphic>();
            _ske.startingAnimation = animation;
            _ske.Initialize(true);
        }

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

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = ddTra.GetChild(0).gameObject;

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");


            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        #endregion
    }
}
