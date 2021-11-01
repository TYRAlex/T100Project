using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9312Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;
        private GameObject _shou;
        private Transform _shouPos;

        private GameObject _ani;
        private Empty4Raycast[] _click;
        private GameObject _back;

        private Transform _bg2;
        private Transform _bigAni;
        private GameObject _ani3;
        private GameObject _ani4;
        private GameObject _ani5;
        private GameObject _ani6;
        private GameObject _ani7;
        private GameObject _TVMask;
        private GameObject _ani4_2;

        private bool _canClick;
        private int _flag;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            _shou = curTrans.Find("shou").gameObject;
            _shouPos = curTrans.Find("shouPos");

            _ani = curTrans.GetGameObject("ani");
            _click = curTrans.Find("Click").GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _click.Length; i++)
            {
                Util.AddBtnClick(_click[i].gameObject, ClickEvent);
            }
            _back = curTrans.GetGameObject("back");
            Util.AddBtnClick(_back, BackEvent);

            _bg2 = curTrans.Find("Bg2");
            _bigAni = curTrans.Find("bigAni");
            _ani3 = _bigAni.GetGameObject("3");
            _ani4 = _bigAni.GetGameObject("4");
            _ani5 = _bigAni.GetGameObject("5");
            _ani6 = _bigAni.GetGameObject("6");
            _ani7 = _bigAni.GetGameObject("7");
            _TVMask = _bigAni.GetGameObject("TVmask");
            _ani4_2 = _bigAni.GetGameObject("4_copy");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            _flag = 0;
            _canClick = false;

            _bg2.gameObject.Hide();
            _bigAni.gameObject.Hide();
            for (int i = 0; i < _bigAni.childCount; i++)
            {
                _bigAni.GetChild(i).gameObject.Hide();
            }

            _back.Hide();
            _shou.Hide();
            _ani.Show();
            InitAni(_ani);
            SpineManager.instance.DoAnimation(_ani, "animation", false);
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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

        void Wait(Action method_1 = null, float len = 0)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _shou.transform.position = _shouPos.GetChild(0).position;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                mono.StartCoroutine(WaitCoroutine(() => { _shou.Show(); ShouTween(1); }, 2.5f));
                SetTimeScale(_ani, 0.55f);
                SpineManager.instance.DoAnimation(_ani, "1", false, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_ani, "2", false); 
                });
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE,1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 2)
            {
                _shou.Hide();
                InitAni(_ani);
                SpineManager.instance.DoAnimation(_ani, "z", false);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () => { _canClick = true; bell.Hide(); }));
            }
            if (talkIndex == 3)
            {
                bell.Show();
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 9, null, null));
            }
            talkIndex++;
        }

        private void SetTimeScale(GameObject ani, float speed)
        {
            SpineManager.instance.SetTimeScale(ani, speed);
        }

        private void InitAni(GameObject ani)
        {
            ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.SetTimeScale(ani, 1.0f);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        void ShouTween(int i)
        {
            if(i == 6)
                _shou.transform.DOMove(_shouPos.GetChild(6).position, 0.85f).OnComplete(
                () => 
                { 
                    _shou.transform.DOMove(_shouPos.GetChild(0).position, 0.85f).OnComplete(
                    () =>
                    {
                        _shou.Hide();
                    });
                });
            else
                _shou.transform.DOMove(_shouPos.GetChild(i).position, 0.85f).OnComplete(() => { ShouTween(i + 1); });
        }

        private void ChangeTexture(int num)
        {
            _bg2.GetComponent<RawImage>().texture = _bg2.GetComponent<BellSprites>().texture[num];
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);

                if ((_flag & 1 << (int.Parse(obj.name) - 1)) == 0)
                    _flag += 1 << (int.Parse(obj.name) - 1);

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                SpineManager.instance.DoAnimation(_ani, "d" + obj.name, false,
                () =>
                {
                    _ani.Hide();
                    ChooseAction(obj.name);
                });
            }
        }

        private void BackEvent(GameObject obj)
        {
            obj.Hide();

            _bg2.gameObject.Hide();
            _bigAni.gameObject.Hide();
            for (int i = 0; i < _bigAni.childCount; i++)
            {
                _bigAni.GetChild(i).gameObject.Hide();
            }

            _ani.Show();
            SpineManager.instance.DoAnimation(_ani, "z", false);

            mono.StartCoroutine(WaitCoroutine(
            () =>
            {
                _canClick = true;
                if (_flag == Mathf.Pow(2, curTrans.Find("Click").childCount) - 1)
                    SoundManager.instance.ShowVoiceBtn(true);
            }, 0.3f));
        }

        private void ChooseAction(string name)
        {
            switch (name)
            {
                case "1":
                    BackHome();
                    break;
                case "2":
                    Movie();
                    break;
                case "3":
                    Book();
                    break;
                case "4":
                    Sleep();
                    break;
                case "5":
                    GetUp();
                    break;
                case "6":
                    LeaveHome();
                    break;
                default:
                    break;
            };
        }

        //回家场景
        private void BackHome()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, ()=> { _back.Show(); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

            _bigAni.gameObject.Show();
            _ani3.Show();
            InitAni(_ani3);

            _bg2.gameObject.Show();
            ChangeTexture(0);
            SpineManager.instance.DoAnimation(_ani3, "5", false, 
            () => 
            {
                Wait(
                () => 
                {
                    SpineManager.instance.DoAnimation(_ani3, "4", false,
                    () =>
                    {
                        Wait(
                        () => 
                        {
                            ChangeTexture(1);
                            SpineManager.instance.DoAnimation(_ani3, "2", false,
                            () =>
                            {
                                ChangeTexture(2);
                                SpineManager.instance.DoAnimation(_ani3, "1", false,
                                () =>
                                {
                                    ChangeTexture(3);
                                    SpineManager.instance.DoAnimation(_ani3, "3", false);
                                });
                            });
                        }, 0.7f);
                    });
                }, 2.0f);
            });
        }

        //看电影场景
        private void Movie()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, null, () => { _back.Show(); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

            _bg2.gameObject.Show();
            ChangeTexture(5);

            _bigAni.gameObject.Show();
            _ani4.Show();
            _TVMask.Show();
            InitAni(_ani4);
            SpineManager.instance.DoAnimation(_ani4, "1", true);

            Wait(
            () =>
            {
                _ani4_2.Show();
                InitAni(_ani4_2);
                SpineManager.instance.DoAnimation(_ani4_2, "2", false,
                () =>
                {
                    _ani4_2.Hide();
                    _TVMask.Hide();
                    Wait(
                    () =>
                    {
                        _ani4.Hide();
                        _ani3.Show();
                        InitAni(_ani3);
                        ChangeTexture(2);
                        SpineManager.instance.DoAnimation(_ani3, "6", false,
                        () =>
                        {
                            _ani4.Show();
                            _ani3.Hide();
                            ChangeTexture(11);
                        });
                    }, 1.0f);
                });
            }, 2.5f);
        }

        //看书场景
        private void Book()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5, null, () => { _back.Show(); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

            _bigAni.gameObject.Show();
            _ani5.Show();
            InitAni(_ani5);

            _bg2.gameObject.Show();
            ChangeTexture(6);
            SpineManager.instance.DoAnimation(_ani5, "animation", false,
            () =>
            {
                Wait(
                () =>
                {
                    _ani5.Hide();
                    _ani3.Show();
                    InitAni(_ani3);
                    ChangeTexture(2);
                    SpineManager.instance.DoAnimation(_ani3, "6", false,
                    () =>
                    {
                        _ani5.Show();
                        _ani3.Hide();
                        ChangeTexture(6);
                    });
                }, 3.5f);
            });
        }

        //睡觉场景
        private void Sleep()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6, null, null));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

            _bigAni.gameObject.Show();
            _ani6.Show();
            InitAni(_ani6);

            _bg2.gameObject.Show();
            ChangeTexture(7);
            SpineManager.instance.DoAnimation(_ani6, "1", false,
            () =>
            {
                Wait(
                () =>
                {
                    InitAni(_ani6);
                    ChangeTexture(8);
                    SpineManager.instance.DoAnimation(_ani6, "2", false,
                    () =>
                    {
                        _ani6.Hide();
                        _ani3.Show();
                        InitAni(_ani3);
                        ChangeTexture(2);
                        SpineManager.instance.DoAnimation(_ani3, "6", false,
                        () =>
                        {
                            ChangeTexture(1);
                            SpineManager.instance.DoAnimation(_ani3, "7", false,
                            () =>
                            {
                                _ani3.Hide();
                                _ani6.Show();
                                InitAni(_ani6);
                                ChangeTexture(8);
                                SpineManager.instance.DoAnimation(_ani6, "4", false,
                                () =>
                                {
                                    _back.Show();
                                });
                            });
                        });
                    });
                }, 1.5f);
            });
        }

        //起床场景
        private void GetUp()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 7, null, null));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);

            _bigAni.gameObject.Show();
            _ani6.Show();
            InitAni(_ani6);

            _bg2.gameObject.Show();
            ChangeTexture(8);

            SetTimeScale(_ani6, 0.5f);
            SpineManager.instance.DoAnimation(_ani6, "2", false,
            () =>
            {
                Wait(
                () => 
                {
                    _ani6.Hide();
                    _ani3.Show();
                    InitAni(_ani3);
                    ChangeTexture(2);
                    SpineManager.instance.DoAnimation(_ani3, "1", false,
                    () =>
                    {
                        _ani3.Hide();
                        _ani6.Show();
                        InitAni(_ani6);
                        ChangeTexture(8);
                        SpineManager.instance.DoAnimation(_ani6, "3", false,
                        () =>
                        {
                            _back.Show();
                        });
                    });
                }, 4.5f);
            });
        }

        //离家场景
        private void LeaveHome()
        {
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 8, null, null));
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);

            _bigAni.gameObject.Show();
            _ani7.Show();
            InitAni(_ani7);

            _bg2.gameObject.Show();
            ChangeTexture(9);
            SpineManager.instance.DoAnimation(_ani7, "1", false,
            () =>
            {
                Wait(
                () =>
                {
                    ChangeTexture(10);
                    SpineManager.instance.DoAnimation(_ani7, "2", false,
                    () =>
                    {
                        _ani7.Hide();
                        _ani3.Show();
                        InitAni(_ani3);
                        ChangeTexture(1);
                        SpineManager.instance.DoAnimation(_ani3, "7", false,
                        () =>
                        {
                            ChangeTexture(2);
                            SpineManager.instance.DoAnimation(_ani3, "6", false,
                            () =>
                            {
                                InitAni(_ani3);
                                ChangeTexture(0);
                                SpineManager.instance.DoAnimation(_ani3, "8", false, () =>
                                {
                                    _back.Show();
                                });
                            });
                        });
                    });
                }, 2.0f);
            });
        }
    }
}
