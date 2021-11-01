using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course914Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _lastPage;
        private GameObject _nextPage;
        private GameObject _shiYanTaiBg;
        private GameObject _shiYanTai;
        private int _curPage;

        private GameObject _level1;
        private bool _isPlayed1;

        private GameObject _level2;
        private GameObject _aniLevel2;
        private GameObject _xueBi;
        private GameObject _puTaoGan;
        private mILDrager[] _dragerLevel2;
        private bool _isPlayed2;

        private GameObject _level3;
        private GameObject _aniLeftLevel3;
        private GameObject _aniRightLevel3;
        private GameObject _juZi;
        private GameObject _boPiJuZi;
        private mILDrager[] _dragerLevel3;
        private bool _isPlayed3;

        private GameObject _level4;
        private GameObject _leftDrag;
        private GameObject _rightDrag;
        private mILDrager[] _dragerLevel4;
        private GameObject _leftAni;
        private GameObject _rightAni;
        private Vector3 _downPos;
        private Vector3 _upPos;
        private bool _leftSNorNS;   //true则为左S右N，false则为左N右S
        private bool _rightSNorNS;   //true则为左S右N，false则为左N右S
        private bool _canChangeNS;
        private bool _canClick;
        private bool _canDrag;
        private bool _isPlayed4;
        //相吸动画的几个位置
        private RectTransform _xiLeftEnd;
        private RectTransform _xiRightEnd;
        //相斥动画的几个位置
        private RectTransform _chiLeftEnd;
        private RectTransform _chiRightEnd;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            _lastPage = curTrans.GetGameObject("LastPage");
            _nextPage = curTrans.GetGameObject("NextPage");

            _level1 = curTrans.GetGameObject("Level1");

            _level2 = curTrans.GetGameObject("Level2");
            _aniLevel2 = _level2.transform.GetGameObject("Ani");
            _xueBi = _level2.transform.GetGameObject("XueBi");
            _puTaoGan = _level2.transform.GetGameObject("PuTaoGan");
            InitDragLevel2();

            _level3 = curTrans.GetGameObject("Level3");
            _aniLeftLevel3 = _level3.transform.GetGameObject("LeftAni");
            _aniRightLevel3 = _level3.transform.GetGameObject("RightAni");
            _juZi = _level3.transform.GetGameObject("Juzi");
            _boPiJuZi = _level3.transform.GetGameObject("BoPiJuzi");
            InitDragLevel3();

            _level4 = curTrans.GetGameObject("Level4");
            _leftDrag = _level4.transform.GetGameObject("LeftDrag");
            _rightDrag = _level4.transform.GetGameObject("RightDrag");
            _leftAni = _leftDrag.transform.GetGameObject("LeftAni");
            _rightAni = _rightDrag.transform.GetGameObject("RightAni");
            _xiLeftEnd = _level4.transform.GetGameObject("XiangXiPos/LeftEndPos").GetComponent<RectTransform>();
            _xiRightEnd = _level4.transform.GetGameObject("XiangXiPos/RightEndPos").GetComponent<RectTransform>();

            _chiLeftEnd = _level4.transform.GetGameObject("XiangChiPos/LeftEndPos").GetComponent<RectTransform>();
            _chiRightEnd = _level4.transform.GetGameObject("XiangChiPos/RightEndPos").GetComponent<RectTransform>();
            InitDragLevel4();

            Util.AddBtnClick(_lastPage, LastNextPage);
            Util.AddBtnClick(_nextPage, LastNextPage);
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("Level1/bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell.Show();
            _lastPage.Hide();
            _nextPage.Hide();
            _level1.Show();
            _isPlayed1 = false;

            _level2.Hide();
            SpineManager.instance.DoAnimation(_aniLevel2, "animation6");
            _xueBi.Show();
            _puTaoGan.Show();
            InitDragStateLevel2();
            _isPlayed2 = false;

            _level3.Hide();
            SpineManager.instance.DoAnimation(_aniLeftLevel3, "animation5");
            SpineManager.instance.DoAnimation(_aniRightLevel3, "animation5");
            _juZi.Show();
            _boPiJuZi.Show();
            InitDragStateLevel3();
            _isPlayed3 = false;

            _level4.Hide();
            SpineManager.instance.DoAnimation(_leftAni, "animation");
            SpineManager.instance.DoAnimation(_rightAni, "animation");
            _leftDrag.Show();
            _rightDrag.Show();
            _leftSNorNS = true;
            _rightSNorNS = true;
            _canChangeNS = false;
            _canClick = false;
            _canDrag = false;
            InitDragStateLevel4();
            _isPlayed4 = false;

            _curPage = 1;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
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
                bell.Hide();
                _nextPage.Show();
                _isPlayed1 = true;
            }, 0));
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
                //葡萄干会跳舞，主要是因为雪碧中的二氧化碳，也就是小气泡，它们会托起葡萄干上浮。
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null,
                () =>
                {
                    _nextPage.Show();
                    _lastPage.Show();
                    _isPlayed2 = true;
                }, 0));
            }
            if (talkIndex == 2)
            {
                //没剥皮的橘子，橘皮里有许多气泡孔，充满了空气，使得橘子整体的重量比相同体积的水轻，
                //从而漂浮在水面上；剥了皮的橘子，橘子瓣里大部分是水，还有固体物，整体重量比相同体积的水重，所以会沉到水里。
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null,
                () =>
                {
                    _nextPage.Show();
                    _lastPage.Show();
                    _isPlayed3 = true;
                }, 0));
            }
            if (talkIndex == 3)
            {
                //磁铁同极相斥、异极相吸，是因为当两块磁铁的同极相互接近时，由于磁场的互斥作用，两块磁铁就像有一股力量让它们分开；两个不同级的靠近，就会吸引
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                () =>
                {
                    _canClick = false;
                    foreach (var drag in _dragerLevel4)
                    {
                        drag.canMove = false;
                        drag.SetDragCallback(null, null, null, null);
                    }
                },
                () =>
                {
                    foreach (var drag in _dragerLevel4)
                    {
                        drag.canMove = true;
                    }
                    AddDragEventLevl4();
                    _canClick = true;
                    _lastPage.Show();
                    _isPlayed4 = true;
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
        
        //上下页事件
        void LastNextPage(GameObject obj)
        {
            if(obj.name == "LastPage")
            {
                if(_curPage == 2)
                {
                    _level2.Hide();
                    _level1.Show();
                    _nextPage.Show();
                    _lastPage.Hide();
                }
                if (_curPage == 3)
                {
                    _level3.Hide();
                    _level2.Show();
                    InitDragStateLevel2();
                    AddDragEventLevl2();
                    SpineManager.instance.DoAnimation(_aniLevel2, "animation6", true);
                    _dragerLevel2[0].canMove = true;
                    _nextPage.Show();
                    _lastPage.Show();
                }
                if (_curPage == 4)
                {
                    _level4.Hide();
                    _level3.Show();
                    SpineManager.instance.DoAnimation(_aniLeftLevel3, "animation5", true);
                    SpineManager.instance.DoAnimation(_aniRightLevel3, "animation5", true);
                    InitDragStateLevel3();
                    AddDragEventLevl3();
                    _dragerLevel3[0].canMove = true;
                    _dragerLevel3[1].canMove = true;
                    _nextPage.Show();
                    _lastPage.Show();
                }
                _curPage -= 1;
            }
            if (obj.name == "NextPage")
            {
                if (_curPage == 1)
                {
                    _level1.Hide();
                    _level2.Show();
                    InitDragStateLevel2();
                    SpineManager.instance.DoAnimation(_aniLevel2, "animation6", true);
                    _lastPage.Hide();
                    _nextPage.Hide();
                    if(_isPlayed2)
                    {
                        AddDragEventLevl2();
                        _dragerLevel2[0].canMove = true;
                        _nextPage.Show();
                        _lastPage.Show();
                    }
                    else
                    {
                        //请同学们先将雪碧倒入杯中，然后再把葡萄干放入杯中，观察会出现什么有趣的现象。
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                        () =>
                        {
                            AddDragEventLevl2();
                            _dragerLevel2[0].canMove = true;
                        }, 0));
                    }
                }
                if (_curPage == 2)
                {
                    _level2.Hide();
                    _level3.Show();
                    InitDragStateLevel3();
                    SpineManager.instance.DoAnimation(_aniLeftLevel3, "animation5", true);
                    SpineManager.instance.DoAnimation(_aniRightLevel3, "animation5", true);
                    _lastPage.Hide();
                    _nextPage.Hide();
                    if(_isPlayed3)
                    {
                        AddDragEventLevl3();
                        _dragerLevel3[0].canMove = true;
                        _dragerLevel3[1].canMove = true;
                        _lastPage.Show();
                        _nextPage.Show();
                    }
                    else
                    {
                        //请将没剥皮的橘子和剥了皮的橘子分别拖拽至两个装有水的杯中，观察看看谁会漂浮，谁会下沉？
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null,
                        () =>
                        {
                            AddDragEventLevl3();
                            _dragerLevel3[0].canMove = true;
                            _dragerLevel3[1].canMove = true;
                        }, 0));
                    }
                }
                if (_curPage == 3)
                {
                    _level3.Hide();
                    _level4.Show();
                    InitDragStateLevel4();
                    _lastPage.Hide();
                    _nextPage.Hide();
                    if (_isPlayed4)
                    {
                        _dragerLevel4[0].canMove = true;
                        _dragerLevel4[1].canMove = true;
                        _canClick = true;
                        _canDrag = true;
                        AddDragEventLevl4();
                        _lastPage.Show();
                    }
                    else
                    {
                        //点击磁铁，变换磁铁的极性，并拖拽任意磁铁使其相互靠近，然后观察它们之间会发生什么现象
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null,
                        () =>
                        {
                            AddDragEventLevl4();
                            _canClick = true;
                            _canDrag = true;
                            _dragerLevel4[0].canMove = true;
                            _dragerLevel4[1].canMove = true;
                        }, 0));
                    }
                }
                _curPage += 1;
            }
        }

        #region 第二关拖拽事件
        //初始化拖拽物体
        void InitDragLevel2()
        {
            _dragerLevel2 = new mILDrager[2];
            _dragerLevel2[0] = _xueBi.GetComponent<mILDrager>();
            _dragerLevel2[1] = _puTaoGan.GetComponent<mILDrager>();
        }

        //初始化拖拽物体状态
        void InitDragStateLevel2()
        {
            _xueBi.Show();
            _puTaoGan.Show();
            _dragerLevel2[0].DoReset();
            _dragerLevel2[1].DoReset();
            _dragerLevel2[0].canMove = false;
            _dragerLevel2[1].canMove = false;
            _dragerLevel2[0].SetDragCallback(null, null, null, null);
            _dragerLevel2[1].SetDragCallback(null, null, null, null);
        }

        //添加拖拽事件
        void AddDragEventLevl2()
        {
            _dragerLevel2[0].SetDragCallback(null, null, EndDragEventLevel2, null);
            _dragerLevel2[1].SetDragCallback(null, null, EndDragEventLevel2, null);
        }

        //拖拽结束事件
        void EndDragEventLevel2(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if(dragBool)
            {
                if(dragIndex == 0)
                {
                    _xueBi.Hide();
                    _dragerLevel2[1].canMove = true;
                    SpineManager.instance.DoAnimation(_aniLevel2, "animation", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                }
                if(dragIndex == 1)
                {
                    _puTaoGan.Hide();
                    SpineManager.instance.DoAnimation(_aniLevel2, "animation2", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    if (_isPlayed2)
                        SoundManager.instance.ShowVoiceBtn(false);
                    else
                        SoundManager.instance.ShowVoiceBtn(true);
                }
            }
            else
            {
                _dragerLevel2[dragIndex].DoReset();
            }
        }

        #endregion

        #region 第三关拖拽事件
        //初始化拖拽物体
        void InitDragLevel3()
        {
            _dragerLevel3 = new mILDrager[2];
            _dragerLevel3[0] = _juZi.GetComponent<mILDrager>();
            _dragerLevel3[1] = _boPiJuZi.GetComponent<mILDrager>();
        }

        //初始化拖拽物体状态
        void InitDragStateLevel3()
        {
            _juZi.Show();
            _boPiJuZi.Show();
            _dragerLevel3[0].DoReset();
            _dragerLevel3[1].DoReset();
            _dragerLevel3[0].canMove = false;
            _dragerLevel3[1].canMove = false;
            _dragerLevel3[0].SetDragCallback(null, null, null, null);
            _dragerLevel3[1].SetDragCallback(null, null, null, null);
        }
        
        //添加拖拽事件
        void AddDragEventLevl3()
        {
            _dragerLevel3[0].SetDragCallback(null, null, EndDragEventLevel3, null);
            _dragerLevel3[1].SetDragCallback(null, null, EndDragEventLevel3, null);
        }

        //拖拽结束事件
        void EndDragEventLevel3(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                if (dragIndex == 0)
                {
                    _juZi.Hide();
                    SpineManager.instance.DoAnimation(_aniLeftLevel3, "animation4", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    if (!_juZi.activeSelf && !_boPiJuZi.activeSelf)
                    {
                        if (_isPlayed3)
                            SoundManager.instance.ShowVoiceBtn(false);
                        else
                            SoundManager.instance.ShowVoiceBtn(true);
                    }
                }
                if (dragIndex == 1)
                {
                    _boPiJuZi.Hide();
                    SpineManager.instance.DoAnimation(_aniRightLevel3, "animation3", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    if (!_juZi.activeSelf && !_boPiJuZi.activeSelf)
                    {
                        if (_isPlayed3)
                            SoundManager.instance.ShowVoiceBtn(false);
                        else
                            SoundManager.instance.ShowVoiceBtn(true);
                    }
                }
            }
            else
            {
                _dragerLevel3[dragIndex].DoReset();
            }
        }

        #endregion

        #region 第四关拖拽与点击事件
        //初始化拖拽物体
        void InitDragLevel4()
        {
            _dragerLevel4 = new mILDrager[2];
            _dragerLevel4[0] = _leftDrag.GetComponent<mILDrager>();
            _dragerLevel4[1] = _rightDrag.GetComponent<mILDrager>();
        }

        //初始化拖拽物体状态
        void InitDragStateLevel4()
        {
            _leftDrag.Show();
            _rightDrag.Show();
            foreach (var drager in _dragerLevel4)
            {
                drager.DoReset();
                drager.canMove = false;
                drager.SetDragCallback(null, null, null, null);
            }
        }

        //添加拖拽事件
        void AddDragEventLevl4()
        {
            _dragerLevel4[0].SetDragCallback(BeginDragEventLevel4, null, EndDragEventLevel4, ClickEvent);
            _dragerLevel4[1].SetDragCallback(BeginDragEventLevel4, null, EndDragEventLevel4, ClickEvent);
        }

        private void ClickEvent(int dragIndex)
        {
            if(_canClick)
            {
                _canClick = false;
                mono.StartCoroutine(AniCoroutine(null,
                () =>
                {
                    if (_canChangeNS)
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            if (dragIndex == 0)
                            {
                                if (_leftSNorNS)
                                {
                                    mono.StartCoroutine(AniCoroutine(
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = false;
                                        }
                                        _leftSNorNS = false;
                                        SpineManager.instance.DoAnimation(_leftAni, "animation3", false);
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                    },
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = true;
                                        }
                                    }, 0.833f));
                                }
                                else
                                {
                                    mono.StartCoroutine(AniCoroutine(
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = false;
                                        }
                                        _leftSNorNS = true;
                                        SpineManager.instance.DoAnimation(_leftAni, "animation4", false);
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                    },
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = true;
                                        }
                                    }, 0.833f));
                                }
                            }
                            if (dragIndex == 1)
                            {
                                if (_rightSNorNS)
                                {
                                    mono.StartCoroutine(AniCoroutine(
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = false;
                                        }
                                        _rightSNorNS = false;
                                        SpineManager.instance.DoAnimation(_rightAni, "animation3", false);
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                    },
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = true;
                                        }
                                    }, 0.833f));
                                }
                                else
                                {
                                    mono.StartCoroutine(AniCoroutine(
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = false;
                                        }
                                        _rightSNorNS = true;
                                        SpineManager.instance.DoAnimation(_rightAni, "animation4", false);
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                    },
                                    () =>
                                    {
                                        foreach (var drag in _dragerLevel4)
                                        {
                                            drag.DoReset();
                                            drag.canMove = true;
                                        }
                                    }, 0.833f));
                                }
                            }
                        },
                        () =>
                        {
                            _dragerLevel4[0].canMove = true;
                            _dragerLevel4[1].canMove = true;
                            _canClick = true;
                        }, 1.0f));
                    }
                    else
                    {
                        _canChangeNS = true;
                        _canClick = true;
                    }
                }, 0.2f));
            }
        }

        //拖拽开始事件
        private void BeginDragEventLevel4(Vector3 dragPos, int dragType, int dragIndex)
        {
            _downPos = dragPos;
        }

        //拖拽结束事件
        void EndDragEventLevel4(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _upPos = dragPos;
            foreach (var drag in _dragerLevel4)
            {
                drag.canMove = false;
                drag.SetDragCallback(null, null, null, null);
            }
            _canClick = false;
            //拖拽距离大于5则认为是拖拽，否则认为是点击
            if (Vector3.Distance(_downPos, _upPos) >= 2)
            {
                _canChangeNS = false;
                if (dragBool)
                {
                    //异性相吸
                    if ((_leftSNorNS && _rightSNorNS) || (!_leftSNorNS && !_rightSNorNS))
                    {
                        mono.StartCoroutine(AniCoroutine(null,
                        () =>
                        {
                            mono.StartCoroutine(AniCoroutine(
                            () =>
                            {
                                if(dragIndex == 0)
                                    _leftDrag.transform.DOMove(_xiLeftEnd.position, 0.3f);
                                if(dragIndex == 1)
                                    _rightDrag.transform.DOMove(_xiRightEnd.position, 0.3f);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                            },
                            () =>
                            {
                                foreach (var drag in _dragerLevel4)
                                {
                                    drag.DoReset();
                                    drag.canMove = true;
                                }
                                AddDragEventLevl4();
                                _canClick = true;
                            }, 0.6f));
                        }, 0.2f));
                    }

                    //同性相斥
                    if ((!_leftSNorNS && _rightSNorNS) || (_leftSNorNS && !_rightSNorNS))
                    {
                        mono.StartCoroutine(AniCoroutine(null,
                        () =>
                        {
                            mono.StartCoroutine(AniCoroutine(
                            () =>
                            {
                                //相斥弹出
                                if(dragIndex == 0)
                                    _leftDrag.transform.DOMove(_chiLeftEnd.position, 0.3f);
                                if(dragIndex == 1)
                                    _rightDrag.transform.DOMove(_chiRightEnd.position, 0.3f);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                            },
                            () =>
                            {
                                foreach (var drag in _dragerLevel4)
                                {
                                    drag.DoReset();
                                    drag.canMove = true;
                                }
                                AddDragEventLevl4();
                                _canClick = true;
                            }, 0.6f));
                        }, 0.2f));
                    }

                    if (_isPlayed4)
                        SoundManager.instance.ShowVoiceBtn(false);
                    else
                        SoundManager.instance.ShowVoiceBtn(true);
                }
                else
                {
                    foreach (var drag in _dragerLevel4)
                    {
                        drag.DoReset();
                        drag.canMove = true;
                    }
                    AddDragEventLevl4();
                    _canClick = true;
                }
            }
            else
            {
                _canClick = true;
                _canChangeNS = true;
                foreach (var drag in _dragerLevel4)
                {
                    drag.DoReset();
                    drag.canMove = true;
                }
                AddDragEventLevl4();
            }
        }
        #endregion
    }
}
