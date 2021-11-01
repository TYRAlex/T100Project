using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course723Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;

        private GameObject _tap;    //水龙头
        private GameObject _title;
        private GameObject _tapDragerItem;
        private mILDrager _tapDrager;  //水龙头把手
        private GameObject _tapPointClick;  //支点点击
        private GameObject _anotherPointClick;  //其他部分的点击
        private Transform _twoClick;
        private GameObject[] _twoClickArray;

        private Vector2 _downPos;
        private Vector2 _upPos;
        private bool _canClick;

        private bool _alreadyLeft;
        private bool _alreadyRight;
        private bool _alreadyTrue;
        private bool _click1;
        private bool _click2;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            bell = curTrans.Find("bell").gameObject;

            _tap = curTrans.GetGameObject("Tap");
            _title = curTrans.GetGameObject("Title");
            _tapDragerItem = curTrans.transform.GetGameObject("TapHandle");
            _tapDrager = _tapDragerItem.GetComponent<mILDrager>();
            _tapPointClick = curTrans.GetGameObject("TapPointClick");
            _anotherPointClick = curTrans.GetGameObject("AnotherPointClick");
            _twoClick = curTrans.Find("TwoClick");
            _twoClickArray = new GameObject[_twoClick.childCount];
            for (int i = 0; i < _twoClickArray.Length; i++)
            {
                _twoClickArray[i] = _twoClick.GetChild(i).gameObject;
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }



        private void GameInit()
        {
            talkIndex = 1;

            _tapPointClick.Hide();
            _tapDragerItem.Hide();
            _anotherPointClick.Hide();
            _twoClick.gameObject.Hide();

            AddTapDrag();
            _canClick = false;
            _alreadyLeft = false;
            _alreadyRight = false;
            _click1 = false;
            _click2 = false;
            SpineManager.instance.DoAnimation(_title, "f", false);
            SpineManager.instance.DoAnimation(_tap, "f", false, 
            ()=> 
            {
                SpineManager.instance.DoAnimation(_tap, "a1", false);
            });
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { _tapDragerItem.Show(); }));

        }

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bell;
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _tapDragerItem.Hide();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    _tapDragerItem.Hide();
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 2)
            {
                _tapDragerItem.Hide();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_tap, "a5", true);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
                }, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_tap, "a5", false, ()=> { SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND); });
                    SoundManager.instance.ShowVoiceBtn(true); 
                }));
            }
            if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    SpineManager.instance.DoAnimation(_tap, "b1", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                })); ;
            }
            if (talkIndex == 4)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, null, 
                ()=> 
                {
                    _canClick = true;
                    _alreadyTrue = false;
                    _tapPointClick.Show();
                    _anotherPointClick.Show();
                    Util.AddBtnClick(_tapPointClick, TruePointClick);
                    Util.AddBtnClick(_anotherPointClick, FalsePointClick);
                })); ;
            }
            if (talkIndex == 5)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 9, 
                ()=> 
                {
                    _canClick = false;
                    _twoClick.gameObject.Show();
                    _anotherPointClick.Hide();
                    _tapPointClick.Hide();
                    SpineManager.instance.DoAnimation(_tap, "c1", false);
                },
                () =>
                {
                    _canClick = true;
                    for (int i = 0; i < _twoClickArray.Length; i++)
                    {
                        Util.AddBtnClick(_twoClickArray[i], ClickTwoImage);
                        Util.AddBtnClick(_anotherPointClick, ReturnClick);
                    }
                })); ;
            }
            if (talkIndex == 6)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 12,
                () =>
                {
                    _canClick = false;
                    _twoClick.gameObject.Hide();
                    _anotherPointClick.Hide();
                    SpineManager.instance.DoAnimation(_tap, "zzz", false);
                }, null));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        #region 开关水龙头拖拽事件 

        void AddTapDrag()
        {
            _tapDrager.SetDragCallback(StartDrag, Draging, EndDrag, null);
        }

        private void StartDrag(Vector3 dragPos, int dragType, int dragIndex)
        {
            _downPos = dragPos;
        }

        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            _upPos = dragPos;
            //右滑顺时针
            if (_upPos.x > _downPos.x && Vector3.Distance(_upPos, _downPos) > 150)
            {
                _alreadyRight = true;
                if (SpineManager.instance.GetCurrentAnimationName(_tap) != "a2" && SpineManager.instance.GetCurrentAnimationName(_tap) != "a3")
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_tap, "a2", false, () => 
                    { 
                        SpineManager.instance.DoAnimation(_tap, "a3", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                    });
                }
            }
            //左滑逆时针
            if (_upPos.x < _downPos.x && Vector3.Distance(_upPos, _downPos) > 150)
            {
                _alreadyLeft = true;
                if (SpineManager.instance.GetCurrentAnimationName(_tap) != "a4" && SpineManager.instance.GetCurrentAnimationName(_tap) != "a1")
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(_tap, "a4", false, () => { SpineManager.instance.DoAnimation(_tap, "a1", true); });
                }
            }
        }

        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _tapDrager.DoReset();
            if(_alreadyLeft && _alreadyRight)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion

        #region 其他事件与方法
        private void TruePointClick(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5, 
                ()=> 
                { 
                    SpineManager.instance.DoAnimation(_tap, "b2", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                }, 
                () => 
                { 
                    _alreadyTrue = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
        }

        private void FalsePointClick(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                int random = Random.Range(6, 9);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, random, null, 
                () => 
                {
                    _canClick = true; 
                    if(_alreadyTrue)
                        SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
        }

        private void ClickTwoImage(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                if (obj.name == "0")
                    TwoImageAni("c2", "d1", "d3", "zia", 10, 7, 10.0f, 1.5f);
                if (obj.name == "1")
                    TwoImageAni("c3", "e1", "e3", "zib", 11, 8, 11.0f, 1.0f);
            }
        }

        private void TwoImageAni(string firstAni, string secondAni, string thirdAni, string titleAni, int voiceIndex, int soundIndex, float waitLen,float soundWait)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
            mono.StartCoroutine(WaitCoroutine(
            () => 
            { 
                SpineManager.instance.DoAnimation(_title, titleAni, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

            }, waitLen));
            SpineManager.instance.DoAnimation(_tap, firstAni, false, 
            () => 
            {
                SpineManager.instance.DoAnimation(_tap, secondAni, false, 
                ()=> 
                {
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, voiceIndex,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_tap, thirdAni, false);
                        mono.StartCoroutine(WaitCoroutine(
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, soundIndex, false);
                        }, soundWait));
                    },
                    () =>
                    {
                        _canClick = true;
                        _anotherPointClick.Show();
                    }));
                });
            });
        }

        private void ReturnClick(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
                SpineManager.instance.DoAnimation(_title, "f", false);
                if (SpineManager.instance.GetCurrentAnimationName(_tap) == "d3")
                {
                    SpineManager.instance.DoAnimation(_tap, "d5", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_tap, "c4", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_tap, "c1", false);
                            _anotherPointClick.Hide();
                            _click1 = true;
                            _canClick = true;
                            JudgeAllClick();
                        });
                    });
                }
                if (SpineManager.instance.GetCurrentAnimationName(_tap) == "e3")
                {
                    SpineManager.instance.DoAnimation(_tap, "e5", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_tap, "c5", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_tap, "c1", false);
                            _anotherPointClick.Hide();
                            _click2 = true;
                            _canClick = true;
                            JudgeAllClick();
                        });
                    });
                }
            }
        }

        void JudgeAllClick()
        {
            if(_click1 && _click2)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #endregion
    }
}
