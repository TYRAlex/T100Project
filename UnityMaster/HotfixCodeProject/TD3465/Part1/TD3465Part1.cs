using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD3465Part1
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
        string[] options;
        int[] levels;
        int gameIndex;
        int flag;

        RectTransform conveyorRect;
        RectTransform uiRect;

        GameObject tiger;
        GameObject hat;
        GameObject star;
        GameObject light;
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
            uiRect = curCanvasTra.Find("UI").GetRectTransform();
            conveyorRect = curCanvasTra.Find("Conveyor").GetRectTransform();

            tiger = conveyorRect.Find("tiger").gameObject;
            hat = conveyorRect.Find("hat").gameObject;
            star = uiRect.Find("star").gameObject;
            light = conveyorRect.Find("light").gameObject;
        }

        void GameInit()
        {
            gameIndex = 0;
            flag = 0;

            conveyorRect.anchoredPosition = Vector2.zero;
            uiRect.anchoredPosition = new Vector2(60, -325);

            InitSpine(tiger, "lh");
            InitSpine(hat, "", false);
            InitSpine(star, "", false);
            InitSpine(light, "", false);

            for (int i = 0; i < uiRect.childCount - 1; i++)
            {
                GameObject obj = uiRect.GetChild(i).gameObject;
                InitSpine(obj, "", false);
                Util.AddBtnClick(obj, ClickButton);
            }

            //对应每个阶段的Spine首字母
            options = new string[]
            {
                "dfi", "ajh",
                "bgl", "eck"
            };

            //随机这局帽子出现的顺序
            levels = new int[uiRect.childCount - 1];
            for (int i = 0; i < levels.Length; i++)
            {
                levels[i] = i;
            }
            Shuffle(ref levels);

            hat.transform.localScale = Vector2.one * 1.2f;
        }

        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            mono.StartCoroutine(WaitFor(0.5f, () =>
            {
                //传送带转动和停止
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);

                conveyorRect.DOAnchorPosX(1780, 8f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    LevelStart();
                });
            }));
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        #region 游戏方法
        //关卡开始
        void LevelStart()
        {
            flag = 0;

            int num = levels[gameIndex] + 1;
            //老虎思考
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            SpineManager.instance.DoAnimation(tiger, "lhk" + num, false, () =>
            {
                SpineManager.instance.DoAnimation(tiger, "lhk" + (num + 3));
            });

            mono.StartCoroutine(WaitFor(4f, () =>
            {
                //UI框出现
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                uiRect.DOAnchorPosY(20, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    uiRect.DOAnchorPosY(0, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        SpineManager.instance.DoAnimation(tiger, "lhk" + (num + 6), false, () =>
                        {
                            SpineManager.instance.DoAnimation(tiger, "lh");
                        });

                        mono.StartCoroutine(OptionShow());
                    });
                });
            }));
        }

        //选项
        IEnumerator OptionShow()
        {
            float deltaTime = 0.4f;

            //随机选项
            int[] randomNum = new int[uiRect.childCount - 1];
            for (int i = 0; i < randomNum.Length; i++)
            {
                randomNum[i] = i;
            }
            Shuffle(ref randomNum);

            if (flag != 0)
            {
                //选项消失
                for (int i = 0; i < randomNum.Length; i++)
                {
                    Transform tra = uiRect.GetChild(i);
                    Material material = tra.GetComponent<SkeletonGraphic>().material;

                    tra.DOScale(Vector2.zero, deltaTime).SetEase(Ease.OutSine);
                    mono.StartCoroutine(ChangeSpineAlpha(material, 0, deltaTime));
                }

                yield return new WaitForSeconds(deltaTime);
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

            //选项出现
            for (int i = 0; i < randomNum.Length; i++)
            {
                string animation = options[flag][randomNum[i]] + "";
                Transform tra = uiRect.GetChild(i);
                Material material = tra.GetComponent<SkeletonGraphic>().material;

                //选项出现动画
                material.SetColor("_Color", new Color(1, 1, 1, 0));

                SpineManager.instance.DoAnimation(tra.gameObject, animation + 1, false);

                tra.localScale = Vector2.zero;
                tra.DOScale(Vector2.one, deltaTime + 0.3f).SetEase(Ease.OutSine);

                mono.StartCoroutine(ChangeSpineAlpha(material, 1, deltaTime));
            }

            yield return new WaitForSeconds(deltaTime + 0.3f);

            isPlaying = false;
        }

        //点击
        void ClickButton(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            char curOption = SpineManager.instance.GetCurrentAnimationName(obj)[0];
            char curAnswer = options[flag][levels[gameIndex]];

            Debug.Log("option:" + curOption);
            Debug.Log("answer:" + curAnswer);

            //判断答案是否正确
            if (curOption == curAnswer)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                //正确反馈
                float time = SpineManager.instance.DoAnimation(obj, curOption + "2", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, curOption + "1", false);

                });

                //星星
                star.transform.position = obj.transform.position;
                InitSpine(star, "xx", false);

                Debug.Log(flag);

                mono.StartCoroutine(WaitFor(time, () =>
                {
                    

                    //帽子动画
                    Debug.Log(ASCIIToChar(97 + levels[gameIndex]));
                    time = SpineManager.instance.DoAnimation(hat, "mj" + ASCIIToChar(97 + levels[gameIndex]) + (flag + 1), false);

                    //老虎高兴
                    SpineManager.instance.DoAnimation(tiger, "lhkx2", true);

                    //判断这关的进度
                    if (++flag == 4)
                    {
                        //光和UI下移
                        mono.StartCoroutine(WaitFor(time, () =>
                        {
                            SpineManager.instance.DoAnimation(light, "g", false);

                            uiRect.DOAnchorPosY(-325, 1f).SetEase(Ease.OutSine).OnComplete(()=>
                            {
                                for (int i = 0; i < uiRect.childCount - 1; i++)
                                {
                                    GameObject _obj = uiRect.GetChild(i).gameObject;
                                    InitSpine(_obj, "", false);
                                }
                            });
                        }));

                        mono.StartCoroutine(WaitFor(4f, () =>
                        {
                            //下一关还是结束
                            if (++gameIndex == 3)
                            {
                                GameEnd();
                            }
                            else
                            {
                                SpineManager.instance.DoAnimation(tiger, "lh");

                                //帽子消失
                                EnlargeShrink(hat.transform, false, 1.2f, () =>
                                {
                                    InitSpine(hat, "", false);
                                    hat.transform.localScale = Vector2.one*1.2f;

                                    LevelStart();
                                });
                            }
                        }));
                    }
                    else
                    {
                        //下一个阶段
                        mono.StartCoroutine(WaitFor(2f, () =>
                        {
                            SpineManager.instance.DoAnimation(tiger, "lh");

                            mono.StartCoroutine(OptionShow());
                        }));
                    }
                }));
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

                //错误反馈
                SpineManager.instance.DoAnimation(obj, curOption + "3", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, curOption + "1", false);
                });

                //老虎难过
                SpineManager.instance.DoAnimation(tiger, "lhjy1", false, () =>
                {
                    SpineManager.instance.DoAnimation(tiger, "lhjy2");
                });

                //继续
                mono.StartCoroutine(WaitFor(2.3f, () =>
                {
                    SpineManager.instance.DoAnimation(tiger, "lh");
                    isPlaying = false;
                    //mono.StartCoroutine(OptionShow());
                }));
            }
        }

        #endregion

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
                        btn03.SetActive(false);
                        ddTra.GetChild(0).gameObject.SetActive(false);

                        StartGame();
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
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.VOICE, 1));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        #endregion

        #region 通用方法
        //Char与ASCII相互转换	
        int CharToASCII(string character)
        {
            if (character == null) return -1;

            System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
            int intAsciiCode = (int)asciiEncoding.GetBytes(character)[0];
            return intAsciiCode;
        }

        string ASCIIToChar(int asciiCode)
        {
            if (asciiCode >= 0 && asciiCode <= 255)
            {
                System.Text.ASCIIEncoding asciiEncoding = new System.Text.ASCIIEncoding();
                byte[] byteArray = new byte[] { (byte)asciiCode };
                string strCharacter = asciiEncoding.GetString(byteArray);
                return strCharacter;
            }
            else
            {
                return "";
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

        //缩小和放大效果
        void EnlargeShrink(Transform _tra, bool _isEnlarge = true, float _time = 0.5f, Action method = null)
        {
            Vector2 _curScale = Vector2.one * 1.2f;//正常大小
            float _timePercentage = 0.8f;//时间比例
            float _range = 0.1f;//缓冲幅度

            if (_isEnlarge)
            {
                _tra.localScale = _curScale;

                _tra.DOScale(_curScale * (1 + _range), _timePercentage * _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _tra.DOScale(_curScale, (1 - _timePercentage) * _time).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        method?.Invoke();
                    });
                });
            }
            else
            {
                _tra.localScale = _curScale;

                _tra.DOScale(_curScale * (1 + _range), (1 - _timePercentage) * _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _tra.DOScale(Vector2.zero, _timePercentage * _time).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        method?.Invoke();
                    });
                });
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