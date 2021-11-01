using DG.Tweening;
using Spine;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.UI;
using Animation = UnityEngine.Animation;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course8312Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        private GameObject _mask;
        private MonoScripts _monoScript;
        private GameObject _lifeCount;
        private GameObject[] _life;
        private int _deadCount;
        private bool _canMove;
        private bool _canReturn;
        private bool _canClick;
        private bool _canRestart;
        private bool _canRotate;
        private GameObject _restart;
        private GameObject _clickRe;

        private GameObject _playBtn;
        private GameObject _carMask;
        private GameObject _car;
        private GameObject _redRaw;
        private GameObject _greenRaw;
        private GameObject _diamond;
        private GameObject _angleText;
        private Image _circle;
        private GameObject _diamondF;
        private Vector3 _diamondStaPos;
        private Vector3 _diamondCurPos;
        private Image _defen;

        private bool _canDeFen;
        private bool _successOrFail;
        private int _lastAngleText;
        private int _curAngle;
        private int _random;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);

            _mask = curTrans.GetGameObject("mask");
            _monoScript = curTrans.GetGameObject("MonoScript").GetComponent<MonoScripts>();
            _lifeCount = curTrans.GetGameObject("LifeCount");
            _playBtn = curTrans.GetGameObject("PlayBtn");
            _carMask = curTrans.GetGameObject("CarMask");
            _car = _carMask.transform.GetGameObject("Car");
            _redRaw = _car.transform.GetGameObject("RedRaw");
            _greenRaw = _car.transform.GetGameObject("GreenRaw");
            _restart = curTrans.GetGameObject("RestartGame");
            _clickRe = _restart.transform.GetGameObject("Click");
            _diamondF = curTrans.transform.GetGameObject("DiamondF");
            _defen = curTrans.GetGameObject("FenShu/DeFen").GetComponent<Image>();
            _life = new GameObject[3];
            for (int i = 0; i < _lifeCount.transform.childCount; i++)
            {
                _life[i] = _lifeCount.transform.GetChild(2 - i).gameObject;
            }
            _diamond = curTrans.GetGameObject("Diamond");
            _angleText = curTrans.GetGameObject("AngleText");
            _circle = curTrans.GetGameObject("Circle/BlueCircle").GetComponent<Image>();
            _diamondStaPos = _diamondF.transform.localPosition;
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Util.AddBtnClick(_playBtn, null);
            bell.Show();
            _mask.Hide();
            _lifeCount.Show();
            _playBtn.Hide();
            _car.Show();
            _redRaw.Show();
            _greenRaw.Hide();
            _restart.Hide();
            _diamond.transform.GetGameObject("Bomb").Hide();
            _diamond.Hide();
            _angleText.Show();
            for (int i = 0; i < _life.Length; i++)
            {
                _life[i].Show();
            }
            _monoScript.FixedUpdateCallBack = mFixedUpdate;
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(_car, "c2");
            ResetPos();

            _canMove = false;
            _canReturn = false;
            _canClick = false;
            _canRestart = false;
            _canRotate = false;
            _canDeFen = false;
            _deadCount = 0;
            _lastAngleText = 0;

            _circle.fillAmount = 0;
            _defen.fillAmount = 0;
            talkIndex = 1;
            _diamondCurPos = _diamondStaPos;
            _diamondF.GetComponent<RectTransform>().anchoredPosition = _diamondStaPos;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void mFixedUpdate()
        {
            if(_canRotate)
            {
                _car.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, _car.GetComponent<RectTransform>().eulerAngles.z - 0.36f);
                _circle.fillAmount = 1 - (_car.GetComponent<RectTransform>().eulerAngles.z / 360);
            }

            if(_canDeFen)
            {
                _defen.fillAmount += 0.004f;
            }

            _lastAngleText = (int)(360 - _car.GetComponent<RectTransform>().eulerAngles.z);
            if (_lastAngleText == 360)
                _angleText.GetComponent<Text>().text = "0";
            else
                _angleText.GetComponent<Text>().text = _lastAngleText.ToString();

            //判断是否变为绿线
            if (_car.GetComponent<RectTransform>().eulerAngles.z >= (_curAngle - 5) && _car.GetComponent<RectTransform>().eulerAngles.z <= (_curAngle + 6))
            {
                _redRaw.Hide();
                _greenRaw.Show();
            }
            else
            {
                _redRaw.Show();
                _greenRaw.Hide();
            }

            //小车拿宝石与复位
            if(_canMove)
            {
                SpineManager.instance.DoAnimation(_car, "c1");
                _car.GetComponent<RectTransform>().position = Vector2.Lerp(_car.GetComponent<RectTransform>().position, _diamond.GetComponent<RectTransform>().position, 0.02f);
            }

            if (_canReturn)
            {
                _greenRaw.Hide();
                _car.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(_car.GetComponent<RectTransform>().anchoredPosition, new Vector2(0, 0), 0.05f);
                SpineManager.instance.DoAnimation(_car, "c2");
            }
        }

        void GameStart()
        {
            //if (bellTextures.texture.Length <= 0)
            //{
            //    Debug.LogError("@愚蠢！！ 哈哈哈 Bg上的BellSprites 里没有东西----------添加完删掉这个打印");
            //}
            //else
            //{
            //    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            //}
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 2, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                //利用陀螺仪能较精准地测量出物体的倾斜角度。那我们该如何利用陀螺仪辅助监测，使小车一步到位完成指定角度的精准转弯呢？
                SoundManager.instance.ShowVoiceBtn(true);
            }, 2.0f));
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        //自定义动画协程
        IEnumerator AniCoroutine(Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            
            if (method_1 != null)
            {
                method_1();
            }

            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    //这是一片充满宝石的矿产，宝石将不定时随机裸露出来，请在小车正对宝石时，按下触发键，前行取得宝石。记住，你只有三辆小车。
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        bell.Hide();
                        //宝石随机出现TODO
                        DiamondStart();
                        _playBtn.Show();
                    },
                    () =>
                    {
                        //添加按钮事件
                        Util.AddBtnClick(_playBtn, PlayGame);
                    }, 1.0f));
                }, 0));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        //按钮点击事件
        void PlayGame(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                if (_greenRaw.activeSelf)
                {
                    SuccessClick();
                }
                else
                {
                    FalseClick();
                }
            }
        }

        //成功事件
        void SuccessClick()
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                _car.GetComponent<RectTransform>().eulerAngles = new Vector3(0, 0, _random);
                _circle.fillAmount = 1 - (_car.GetComponent<RectTransform>().eulerAngles.z / 360);
                _diamondF.transform.DOLocalMove(new Vector3(_diamondCurPos.x + 75, _diamondCurPos.y, _diamondCurPos.z), 1);
                _canRotate = false;
                _canDeFen = true;
                _canMove = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                    _canDeFen = false;
                    _diamondCurPos = _diamondF.transform.localPosition;
                    _canMove = false;
                    _diamond.Hide();
                    _canReturn = true;
                },
                () =>
                {
                    _canReturn = false;
                    ResetPos();
                    //如果拿到五颗宝石，游戏胜利，否则宝石继续随机出现
                    if(_defen.fillAmount == 1)
                    {
                        _successOrFail = true;
                        mono.StartCoroutine(AniCoroutine(null, 
                        () => 
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            mono.StartCoroutine(AniCoroutine(
                            () =>
                            {
                                _restart.Show();
                                _mask.Show();
                                _canRestart = true;
                                SpineManager.instance.DoAnimation(_restart, "animation4", false);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                            },
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_restart, "animation5", true);
                                _clickRe.Show();
                                Util.AddBtnClick(_clickRe, RestartGame);
                            }, 1.0f));
                        }, 0));
                    }
                    else
                        DiamondStart();
                }, 1.0f));
            }, 1.0f));
        }

        //失败事件
        void FalseClick()
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                _canRotate = false;
                _diamond.GetComponent<RawImage>().color = new Color(0, 0, 0, 255);
            },
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_car, "bz", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    _diamond.transform.GetGameObject("Bomb").Show();
                    SpineManager.instance.DoAnimation(_diamond.transform.GetGameObject("Bomb"), "bz", false);
                    if (_deadCount == 3)
                    {
                        _successOrFail = false;
                        _restart.Show();
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            _mask.Show();
                            _canRestart = true;
                            SpineManager.instance.DoAnimation(_restart, "animation", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        },
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_restart, "animation2", true);
                            _clickRe.Show();
                            Util.AddBtnClick(_clickRe, RestartGame);
                        }, 1.0f));
                    }
                    else
                        _life[_deadCount].Hide();
                    _deadCount += 1;
                },
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        _circle.fillAmount = 0;
                        _diamond.GetComponent<RawImage>().color = new Color(255, 255, 255, 255);
                        _diamond.transform.GetGameObject("Bomb").Hide();
                        _diamond.Hide();
                        ResetPos();
                        SpineManager.instance.DoAnimation(_car, "c3", false);
                    },
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_car, "c2", false);
                            if (_deadCount != 4)
                                DiamondStart();
                        },
                        () =>
                        {
                            _canClick = true;
                        }, 1.0f));
                    }, 1.0f));
                }, 0.3f));
            }, 1.0f));
        }

        //重玩游戏
        void RestartGame(GameObject obj)
        {
            _clickRe.Hide();
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                if(_successOrFail)
                    SpineManager.instance.DoAnimation(_restart, "animation6");
                else
                    SpineManager.instance.DoAnimation(_restart, "animation3");
            },
            () =>
            {
                _lifeCount.Show();
                _playBtn.Show();
                _car.Show();
                _restart.Hide();
                _mask.Hide();
                _diamond.transform.GetGameObject("Bomb").Hide();
                _diamond.Hide();

                for (int i = 0; i < _life.Length; i++)
                {
                    _life[i].Show();
                }

                _canRestart = false;
                _canMove = false;
                _canReturn = false;
                _canClick = false;
                _deadCount = 0;
                _lastAngleText = 0;
                _circle.fillAmount = 0;
                _defen.fillAmount = 0;
                _angleText.GetComponent<Text>().text = _lastAngleText.ToString();
                _diamondF.transform.localPosition = _diamondStaPos;
                _diamondCurPos = _diamondStaPos;

                ResetPos();
                DiamondStart();
            }, 1.167f));
        }

        //重置小车位置
        void ResetPos()
        {
            _car.GetComponent<RectTransform>().localEulerAngles = new Vector3(0, 0, 0);
            _car.GetComponent<RectTransform>().localPosition = new Vector2(0, 0);
        }

        //生成宝石事件
        void DiamondStart()
        {
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                _circle.fillAmount = 0;
                _random = 0;
                while(_random >= 345 || _random <= 15)
                    _random = Random.Range(0, 361);
                _curAngle = _random;
                //使用三角函数计算宝石位置（已知宝石与车子的距离与角度即可求出，但unity顺时针旋转是从360->0，需重新计算）
                if (_random <= 360 && _random > 270)
                {
                    _diamond.Show();
                    float x = Mathf.Sin((360 - _random) * (float)(Math.PI / 180)) * 470;
                    float y = Mathf.Cos((360 - _random) * (float)(Math.PI / 180)) * 450;
                    _diamond.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                }
                else if (_random <= 270 && _random > 180)
                {
                    _diamond.Show();
                    float x = Mathf.Cos((270 - _random) * (float)(Math.PI / 180)) * 470;
                    float y = - Mathf.Sin((270 - _random) * (float)(Math.PI / 180)) * 470;
                    _diamond.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                }
                else if (_random <= 180 && _random > 90)
                {
                    _diamond.Show();
                    float x = -Mathf.Sin((180 - _random) * (float)(Math.PI / 180)) * 470;
                    float y = -Mathf.Cos((180 - _random) * (float)(Math.PI / 180)) * 470;
                    _diamond.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                }
                else
                {
                    _diamond.Show();
                    float x = -Mathf.Sin(_random * (float)(Math.PI / 180)) * 470;
                    float y = Mathf.Cos(_random * (float)(Math.PI / 180)) * 470;
                    _diamond.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);
                }
            },
            () =>
            {
                _canRotate = true;
                _canClick = true;
            }, 1.0f));
        }
    }
}
