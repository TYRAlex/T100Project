using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD91261Part1
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
        int[] answerIndexs;
        int[][] answers;
        int gameIndex;
        bool isGameEnd;

        float totalTime;
        float currentTime;

        SkeletonGraphic machineSke;
        SkeletonGraphic boomSke;

        RectTransform lidRect;
        RectTransform replayRect;

        Image liquidImage;

        Transform imageMaskTra;
        Transform clickTra;
        Transform crackTra1;
        Transform crackTra2;
        Transform pointTra;
        Transform shakeTra;
        Transform hpTra;

        GameObject submitButton;
        GameObject hammer;
        GameObject demon;
        GameObject think;
        GameObject animal;
        GameObject replayMask;
        GameObject clickObj;
        GameObject peopleObj;
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
            shakeTra = curCanvasTra.Find("shake");
            hpTra = curCanvasTra.Find("Hp");
            imageMaskTra = shakeTra.Find("imageMask");
            clickTra = curCanvasTra.Find("Click");
            crackTra1 = shakeTra.Find("machine/crack1");
            crackTra2 = shakeTra.Find("machine/crack2");
            pointTra = curCanvasTra.Find("Point");

            machineSke = shakeTra.Find("machine").GetComponent<SkeletonGraphic>();
            boomSke = hpTra.Find("boom").GetComponent<SkeletonGraphic>();

            lidRect = shakeTra.Find("machine/lidMask/lid").GetRectTransform();
            replayRect = curCanvasTra.Find("Replay").GetRectTransform();
            liquidImage = shakeTra.Find("machine/LiquidTube/Liquid").GetImage();


            hammer = curCanvasTra.Find("hammer").gameObject;
            demon = curCanvasTra.Find("demonShake/demon").gameObject;
            think = curCanvasTra.Find("think").gameObject;
            animal = shakeTra.Find("machine/animal").gameObject;
            clickObj = shakeTra.Find("clickMask/click").gameObject;
            submitButton = clickObj.transform.Find("click").gameObject;
            peopleObj = curCanvasTra.Find("peopleMask/people").gameObject;
            replayMask = curCanvasTra.Find("ReplayMask").gameObject;
        }

        void GameInit()
        {
            talkIndex = 1;
            gameIndex = 0;
            isGameEnd = true;

            currentTime = totalTime = 45f;

            int n = imageMaskTra.childCount;

            answerIndexs = new int[n];

            for (int i = 0; i < n; i++)
            {
                answerIndexs[i] = Random.Range(0, 3);
            }

            answers = new int[][]
            {
                new int[]{0, 0, 2},
                new int[]{2, 1, 2},
                new int[]{1, 3, 0}
            };

            for (int i = 0; i < n; i++)
            {
                InitImage(imageMaskTra.GetChild(i), i);
            }

            //点击按钮添加方法和初始化动画
            for (int i = 0; i < clickTra.childCount; ++i)
            {
                Transform tra = clickTra.GetChild(i);

                for (int j = 0; j < tra.childCount; j++)
                {
                    Transform traChild = tra.GetChild(j);
                    traChild.GetComponent<SkeletonGraphic>().Initialize(true);
                    Util.AddBtnClick(traChild.GetChild(0).gameObject, ClickButton);
                }
            }

            hammer.SetActive(false);
            demon.SetActive(true);

            //Spine初始化
            InitSpine(peopleObj.transform, "ren0");
            InitSpine(clickObj.transform, "ren0");
            InitSpine(shakeTra.Find("machine"), "jiqi");
            InitSpine(shakeTra.Find("machine/animal"), "g3");
            InitSpine(lidRect.transform, "men");
            InitSpine(demon.transform, "xem1");
            InitSpine(think.transform, "");
            InitSpine(boomSke.transform, "");

            lidRect.anchoredPosition = Vector2.zero;

            shakeTra.Find("machine/animal").GetRectTransform().anchoredPosition = new Vector2(50, -17);
            shakeTra.localScale = Vector2.one;

            liquidImage.fillAmount = 1;

            #region 裂痕初始化
            /*for (int i = 0; i < crackTra1.childCount; i++)
            {
                crackTra1.GetChild(i).gameObject.SetActive(false);
            }

            for (int i = 0; i < crackTra2.childCount; i++)
            {
                crackTra2.GetChild(i).gameObject.SetActive(false);
            }*/
            #endregion

            crackTra1.gameObject.SetActive(false);
            crackTra2.gameObject.SetActive(false);
            replayRect.gameObject.SetActive(false);
            replayMask.SetActive(false);

            //生命值
            Transform tra1 = hpTra.GetChild(0);
            for (int i = 0; i < tra1.childCount; i++)
            {
                tra1.GetChild(i).gameObject.SetActive(true);
                hpTra.GetChild(1).GetChild(i).gameObject.SetActive(false);
            }

            Util.AddBtnClick(submitButton, Submit);
            Util.AddBtnClick(replayRect.gameObject, LostGame);

            //弹簧
            SpringJoint2D joint2D = replayRect.GetComponent<SpringJoint2D>();
            float percentage = replayRect.position.x / 960;
            joint2D.connectedAnchor = new Vector2(replayRect.position.x, 1600 * percentage);
            joint2D.distance = 1100 * percentage;
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        void FixedUpdate()
        {
            if (!isPlaying && !isGameEnd)
            {
                currentTime -= Time.fixedDeltaTime;
                liquidImage.fillAmount = currentTime / totalTime;

                //时间过半
                if (currentTime <= totalTime / 2 && !crackTra1.gameObject.activeSelf)
                {
                    mono.StartCoroutine(CrackShow(crackTra1));
                }

                //时间结束
                if (currentTime <= 0 && !crackTra2.gameObject.activeSelf)
                {
                    mono.StartCoroutine(CrackShow(crackTra2, () =>
                    {
                        ColorDisPlay(replayMask.transform.GetRawImage(), true, ()=>
                        {
                            isGameEnd = true;
                            isPlaying = true;

                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                            replayRect.anchoredPosition = Vector2.up * 800;
                            replayRect.gameObject.SetActive(true);

                            mono.StartCoroutine(WaitFor(1f, () => isPlaying = false));
                        });                        
                    }));
                }
            }

            if (isGameEnd)
            {
                //实时更新两条绳子的长度
                UpdateRope(replayRect.GetChild(0).GetRectTransform(), pointTra.GetChild(0));
                UpdateRope(replayRect.GetChild(1).GetRectTransform(), pointTra.GetChild(1));
            }
        }

        #region 游戏方法
        //点击切换按钮
        void ClickButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

            GameObject objParent = obj.transform.parent.gameObject;
            int num = int.Parse(obj.transform.parent.parent.name) - 1;
            BellSprites sprites = imageMaskTra.GetChild(num).GetComponent<BellSprites>();

            RawImage image1 = sprites.transform.GetChild(0).GetRawImage();
            RawImage image2 = sprites.transform.GetChild(1).GetRawImage();

            if (objParent.name == "up")
            {
                if (++answerIndexs[num] == sprites.texture.Length) answerIndexs[num] = 0;

                SpineManager.instance.DoAnimation(objParent, "an2", false, () =>
                {
                    objParent.GetComponent<SkeletonGraphic>().Initialize(true);
                });
            }
            else
            {
                if (--answerIndexs[num] == -1) answerIndexs[num] = sprites.texture.Length - 1;

                SpineManager.instance.DoAnimation(objParent, "an", false, () =>
                {
                    objParent.GetComponent<SkeletonGraphic>().Initialize(true);
                });
            }

            TextChange(image1, image2, sprites, answerIndexs[num], objParent.name == "up", () => isPlaying = false);

            image1.transform.SetAsLastSibling();
        }

        //提交答案
        void Submit(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

            mono.StartCoroutine(SubmitMethod(obj.transform.parent.gameObject));
        }

        IEnumerator SubmitMethod(GameObject objParent)
        {
            //拉下把手
            SpineManager.instance.DoAnimation(clickObj, "ren1", false);

            float time = SpineManager.instance.DoAnimation(peopleObj, "ren1", false, () =>
            {
                SpineManager.instance.DoAnimation(peopleObj, "ren0");
                SpineManager.instance.DoAnimation(clickObj, "ren0");
            });

            yield return new WaitForSeconds(time);

            //答案是否正确
            if (IsSameArray(answers[gameIndex], answerIndexs))
            {
                time = SpineManager.instance.DoAnimation(lidRect.gameObject, "men2", false, () =>
                {
                    SpineManager.instance.DoAnimation(lidRect.gameObject, "men", false);
                });

                yield return new WaitForSeconds(time);

                //门下移
                SpineManager.instance.DoAnimation(peopleObj, "ren2");
                lidRect.DOAnchorPosY(-550, 4f).SetEase(Ease.Linear);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);

                yield return new WaitForSeconds(4f);

                InitSpine(lidRect.transform, "men");

                //锤
                hammer.GetComponent<SkeletonGraphic>().Initialize(true);
                hammer.SetActive(true);
                time = SpineManager.instance.DoAnimation(hammer, "chui", false);

                yield return new WaitForSeconds(time - 0.5f);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                //生命值扣除
                Transform tra = hpTra.GetChild(0).GetChild(gameIndex);
                boomSke.transform.position = tra.position + Vector3.down * 75;
                SpineManager.instance.DoAnimation(boomSke.gameObject, "sc-boom", false,
                    () => hpTra.GetChild(1).GetChild(gameIndex).gameObject.SetActive(false));
                hpTra.GetChild(1).GetChild(gameIndex).gameObject.SetActive(true);
                tra.gameObject.SetActive(false);

                //小恶魔晕
                SpineManager.instance.DoAnimation(demon, "xem-y", false, () =>
                SpineManager.instance.DoAnimation(demon, "xem1"));

                yield return new WaitForSeconds(0.5f);

                hammer.SetActive(false);

                if (++gameIndex == 3)
                {
                    //小恶魔消失
                    GameObject boom = curCanvasTra.Find("boom").gameObject;
                    boom.GetComponent<SkeletonGraphic>().Initialize(true);
                    boom.SetActive(true);
                    time = SpineManager.instance.DoAnimation(boom, "sc-boom", false);
                    demon.SetActive(false);

                    yield return new WaitForSeconds(time);

                    boom.SetActive(false);
                    GameEnd();
                }
                else
                {
                    SpineManager.instance.DoAnimation(peopleObj, "ren0");
                    SpineManager.instance.DoAnimation(clickObj, "ren0");
                    //门上移
                    lidRect.DOAnchorPosY(0, 4f).SetEase(Ease.Linear);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);

                    yield return new WaitForSeconds(4f);

                    time = SpineManager.instance.DoAnimation(think, "ren-p" + gameIndex, false);
                    SpineManager.instance.DoAnimation(animal, "g" + gameIndex);
                    if (gameIndex == 1) animal.transform.GetRectTransform().anchoredPosition = new Vector2(50, -25f);
                    if (gameIndex == 2) animal.transform.GetRectTransform().anchoredPosition = new Vector2(50, -35f);

                    yield return new WaitForSeconds(time);

                    isPlaying = false;
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);

                //错误反馈
                time = SpineManager.instance.DoAnimation(lidRect.gameObject, "men3", false);

                //小恶魔嘲笑
                SpineManager.instance.DoAnimation(demon, "xem-jx", false, () =>
                SpineManager.instance.DoAnimation(demon, "xem1"));

                yield return new WaitForSeconds(time);

                SpineManager.instance.DoAnimation(peopleObj, "ren3");

                yield return new WaitForSeconds(2f);

                InitSpine(lidRect.transform, "men");
                SpineManager.instance.DoAnimation(peopleObj, "ren0");
                SpineManager.instance.DoAnimation(clickObj, "ren0");
                isPlaying = false;
            }
        }

        //裂痕显示加机器抖动
        IEnumerator CrackShow(Transform tra, Action method = null)
        {
            isPlaying = true;

            SpineManager.instance.DoAnimation(demon, "xem-c");

            #region 随机化出现裂痕
            //随机化出现裂痕
            /*int n = tra.childCount;
            int[] crackArray = new int[n];

            for (int i = 0; i < n; i++)
            {
                crackArray[i] = i;
            }

            Shuffle(ref crackArray);

            float _deltaTime = 2f;

            WaitForSeconds wait = new WaitForSeconds(_deltaTime / 2);

            for (int i = 0; i < n; i++)
            {
                yield return wait;

                tra.GetChild(crackArray[i]).gameObject.SetActive(true);
                JellyAnimation(shakeTra);

                yield return wait;
            }*/
            #endregion

            float _deltaTime = 2f;
            WaitForSeconds wait = new WaitForSeconds(_deltaTime / 2);

            yield return wait;

            //机器抖动
            tra.gameObject.SetActive(true);
            JellyAnimation(shakeTra);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

            yield return wait;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);

            //小恶魔嘲笑
            SpineManager.instance.DoAnimation(demon, "xem-jx", false, () =>
            SpineManager.instance.DoAnimation(demon, "xem1")); 

            yield return new WaitForSeconds(2f);

            SpineManager.instance.DoAnimation(demon, "xem1");
            
            if(method != null)
            {
                method.Invoke();
                yield return new WaitForSeconds(0.8f);
            }

            isPlaying = false;
        }

        //初始化图片
        void InitImage(Transform tra, int i)
        {
            BellSprites bellSprites = tra.GetComponent<BellSprites>();
            tra.GetChild(0).GetRawImage().texture = bellSprites.texture[answerIndexs[i]];
            tra.GetChild(0).GetRawImage().SetNativeSize();
            tra.GetChild(0).GetRectTransform().anchoredPosition = Vector2.zero;
            tra.GetChild(1).GetRectTransform().anchoredPosition = Vector2.up * 200;
        }

        //更新绳索
        void UpdateRope(RectTransform _ropeRect, Transform _pointTra)
        {
            _ropeRect.eulerAngles = new Vector3(0, 0, GetAngle(_ropeRect.position, _pointTra.position));
            _ropeRect.sizeDelta = new Vector2(_ropeRect.sizeDelta.x, GetDistance(_ropeRect.position, _pointTra.position));
        }

        void LostGame(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            MaskInit();
            GameInit();
            StartGame();
        }

        //果冻动画
        void JellyAnimation(Transform _tra)
        {
            _tra.DOScaleX(1.04f, 0.2f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                _tra.DOScaleX(1, 0.1f).SetEase(Ease.OutSine);
            });

            _tra.DOScaleY(0.96f, 0.2f).SetEase(Ease.OutQuint).OnComplete(() =>
            {
                _tra.DOScaleY(1, 0.1f).SetEase(Ease.OutSine);
            });
        }

        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.3f)
        {
            if (isShow)
            {
                raw.color = new Color(0, 0, 0, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(new Color(0, 0, 0, 0.8f), _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
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
        #endregion

        #region 游戏通用环节
        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            mono.StartCoroutine(WaitFor(0.5f, () =>
            {
                Debug.Log(1);

                SpineManager.instance.DoAnimation(think, "ren-p3", false, () =>
                {
                    isPlaying = false;
                    isGameEnd = false;
                });
            }));
        }

        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            Debug.Log(2);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                Debug.Log(3);
                ddTra.gameObject.SetActive(true);
                ddTra.GetChild(0).gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    Debug.Log(4);
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

        //文字切换上下
        void TextChange(RawImage _text1, RawImage _text2, BellSprites textSpr, int n, bool isUp, Action method = null)
        {
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

            int temp = isUp ? 1 : -1;

            _text2.transform.GetRectTransform().anchoredPosition = Vector2.down * 200 * temp;
            _text2.texture = textSpr.texture[n];
            _text2.SetNativeSize();

            _text1.transform.GetRectTransform().DOAnchorPosY(200 * temp, 0.5f).SetEase(Ease.InOutSine);
            _text2.transform.GetRectTransform().DOAnchorPosY(0, 0.5f).SetEase(Ease.InOutSine).OnComplete(() => method?.Invoke());
        }

        //比较数组是否相同
        bool IsSameArray(int[] _a, int[] _b)
        {
            if (_a.Length != _b.Length) return false;

            int n = _a.Length;
            for (int i = 0; i < n; ++i)
            {
                if (_a[i] != _b[i]) return false;
            }

            return true;
        }

        //Spine初始化
        void InitSpine(Transform _tra, string animation)
        {
            SkeletonGraphic _ske = _tra.GetComponent<SkeletonGraphic>();
            _ske.startingAnimation = animation;
            _ske.Initialize(true);
        }

        //洗牌算法
        void Shuffle<T>(ref T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = ((int)Random.value) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }

        //判断角度
        float GetAngle(Vector3 startPos, Vector3 endPos)
        {
            Vector3 dir = endPos - startPos;
            float angle = Vector3.Angle(Vector3.up, dir);
            Vector3 cross = Vector3.Cross(Vector3.up, dir);
            float dirF = cross.z > 0 ? 1 : -1;
            angle = angle * dirF;
            return angle;
        }

        //计算两点之间的距离并将长度自适应
        float GetDistance(Vector3 startPos, Vector3 endPos)
        {
            float distance = Vector3.Distance(endPos, startPos);
            return distance * 1 / curCanvasTra.localScale.x;
        }
        #endregion
    }
}
