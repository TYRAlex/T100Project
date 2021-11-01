using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course912Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject max;
        private GameObject Bg;
        private BellSprites maxTextures;
        private GameObject btnTest;

        private int _pageCount;
        private bool _canClick;
        private bool _canDrag;
        private GameObject _page1;
        private GameObject _aniPage1;
        private GameObject _page2;
        private GameObject _aniPage2;
        private GameObject _page3;
        private GameObject _aniPage3;
        private GameObject _title;
        private GameObject _currentAniObj;
        private GameObject _clickEvent;
        private GameObject[] _clickBtn;

        private bool _played1;
        private bool _played2;
        private bool _played3;
        private Vector2 _lastPos;
        private Vector2 _nextPos;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);

            _page1 = curTrans.GetGameObject("Page1");
            _aniPage1 = _page1.transform.GetGameObject("Ani");
            _page2 = curTrans.GetGameObject("Page2");
            _aniPage2 = _page2.transform.GetGameObject("Ani");
            _page3 = curTrans.GetGameObject("Page3");
            _aniPage3 = _page3.transform.GetGameObject("Ani");
            _title = curTrans.GetGameObject("Title");
            _clickEvent = curTrans.GetGameObject("ClickEvent");
            _clickBtn = new GameObject[3];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _clickBtn[i] = _clickEvent.transform.GetChild(i).gameObject;
                Util.AddBtnClick(_clickBtn[i], ClickEvent);
            }
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            max = curTrans.Find("max").gameObject;
            max.SetActive(true);
            SpineManager.instance.DoAnimation(max, "daiji");
            Bg = curTrans.Find("Bg").gameObject;
            maxTextures = Bg.GetComponent<BellSprites>();

            talkIndex = 1;
            _pageCount = 1;
            _canClick = false;
            _canDrag = false;
            _played1 = false;
            _played2 = false;
            _played3 = false;

            UIEventListener.Get(Bg).onDown = null;
            UIEventListener.Get(Bg).onUp = null;
            UIEventListener.Get(Bg).onDown = mOnDown;
            UIEventListener.Get(Bg).onUp = mOnUp;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _title.Hide();
            _page1.SetActive(true);
            SpineManager.instance.DoAnimation(_aniPage1, "animation");
            _page2.SetActive(false);
            SpineManager.instance.DoAnimation(_aniPage2, "a1");
            _page3.SetActive(false);
            SpineManager.instance.DoAnimation(_aniPage3, "a1");
            _clickEvent.SetActive(false);
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
            //搅拌有什么作用呢？还有更好的搅拌器材吗？
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                max.SetActive(false);
                _canClick = true;
                Util.AddBtnClick(_page1, (o)=> { 
                    if(_canClick)
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            _canClick = false;
                            _canDrag = false;
                            SpineManager.instance.DoAnimation(_aniPage1, "animation2", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        },
                        () =>
                        {
                            _canClick = true;
                            _canDrag = true;
                            _played1 = true;
                        }, 8.5f));
                    }
                        
                });
            }, 1.0f));
        }
        //max说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
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

        void mOnDown(PointerEventData eventData)
        {
            _lastPos = eventData.position;
        }

        void mOnUp(PointerEventData eventData)
        {
            Debug.Log("拖拽：" + _canDrag);
            Debug.Log("当前页数：" + _pageCount);
            _nextPos = eventData.position;
            if (_canDrag)
            {
                //右滑，下一页
                if (_nextPos.x - _lastPos.x < -150)
                {
                    _canDrag = false;
                    if (_pageCount == 1)
                    {
                        _title.Show();
                        _clickEvent.SetActive(false);
                        _page1.SetActive(false);
                        _page2.SetActive(true);
                        if (_played2)
                        {
                            //播放过一次的语音可不播，且不需要点击即可切换页面
                            mono.StartCoroutine(AniCoroutine(null,
                            () =>
                            {
                                _canDrag = true;
                                _clickEvent.SetActive(true);
                                _pageCount += 1;
                            }, 1.0f));
                        }
                        else
                        {
                            //如何实现搅拌棒自动上下抬升呢？
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                            () =>
                            {
                                _clickEvent.SetActive(true);
                                _pageCount += 1;
                            }, 1.0f));
                        }
                    }
                    if (_pageCount == 2)
                    {
                        _title.Show();
                        _clickEvent.SetActive(false);
                        _page2.SetActive(false);
                        _page3.SetActive(true);
                        if (_played3)
                        {
                            //播放过一次的语音可不播，且不需要点击即可切换页面
                            mono.StartCoroutine(AniCoroutine(null,
                            () =>
                            {
                                _clickEvent.SetActive(true);
                                _canDrag = true;
                                _pageCount += 1;
                            }, 1.0f));
                        }
                        else
                        {
                            //可不可以选择不同的搅拌速度呢？
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null,
                            () =>
                            {
                                _clickEvent.SetActive(true);
                                _pageCount += 1;
                            }, 1.0f));
                        }
                    }
                    if (_pageCount == 3)
                    {
                        _canDrag = true;
                    }
                }
                //左滑，上一页
                if (_nextPos.x - _lastPos.x > 150)
                {
                    _canDrag = false;
                    if (_pageCount == 1)
                    {
                        _canDrag = true;
                    }
                    if (_pageCount == 2)
                    {
                        _title.Hide();
                        _clickEvent.SetActive(false);
                        _page2.SetActive(false);
                        _page1.SetActive(true);
                        _canClick = false;
                        if (_played1)
                        {
                            //播放过一次的语音可不播，且不需要点击即可切换页面
                            mono.StartCoroutine(AniCoroutine(null,
                            () =>
                            {
                                _canClick = true;
                                _canDrag = true;
                                _pageCount -= 1;
                            }, 1.0f));
                        }
                        else
                        {
                            //搅拌有什么作用呢？还有更好的搅拌器材吗？
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
                            () =>
                            {
                                _pageCount -= 1;
                            }, 1.0f));
                        }
                    }
                    if (_pageCount == 3)
                    {
                        _clickEvent.SetActive(false);
                        _page3.SetActive(false);
                        _page2.SetActive(true);
                        if (_played2)
                        {
                            //播放过一次的语音可不播，且不需要点击即可切换页面
                            mono.StartCoroutine(AniCoroutine(null,
                            () =>
                            {
                                _clickEvent.SetActive(true);
                                _canDrag = true;
                                _pageCount -= 1;
                            }, 1.0f));
                        }
                        else
                        {
                            //如何实现搅拌棒自动上下抬升呢？
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                            () =>
                            {
                                _clickEvent.SetActive(true);
                                _canDrag = true;
                                _pageCount -= 1;
                            }, 1.0f));
                        }
                    }
                }
            }
        }

        //气泡点击事件
        void ClickEvent(GameObject obj)
        {
            _clickEvent.SetActive(false);
            _canDrag = false;
            if (_pageCount == 2)
            {
                _currentAniObj = _aniPage2;
                _played2 = true;
            }
            if (_pageCount == 3)
            {
                _currentAniObj = _aniPage3;
                _played3 = true;
            }

            if (obj.name == "Click1")
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_currentAniObj, "a2", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                },
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_currentAniObj, "a3", false);
                        if (_pageCount == 2)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        }
                        if (_pageCount == 3)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
                        }
                    },
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_currentAniObj, "a4", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        },
                        () =>
                        {
                            _clickEvent.SetActive(true);
                            _canDrag = true;
                        }, 0.833f));
                    }, 5.0f));
                }, 0.833f));
            }
            else if (obj.name == "Click2")
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_currentAniObj, "b2", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                },
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_currentAniObj, "b3", false);
                        if (_pageCount == 2)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        }
                        if (_pageCount == 3)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
                        }
                    },
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_currentAniObj, "b4", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        },
                        () =>
                        {
                            _clickEvent.SetActive(true);
                            _canDrag = true;
                        }, 0.833f));
                    }, 5.0f));
                }, 0.833f));
            }
            else
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(_currentAniObj, "c2", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                },
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_currentAniObj, "c3", false);
                        if (_pageCount == 2)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                        }
                        if (_pageCount == 3)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);
                        }
                    },
                    () =>
                    {
                        mono.StartCoroutine(AniCoroutine(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_currentAniObj, "c4", false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        },
                        () =>
                        {
                            _clickEvent.SetActive(true);
                            _canDrag = true;
                        }, 0.833f));
                    }, 5.0f));
                }, 0.833f));
            }
        }
    }
}
