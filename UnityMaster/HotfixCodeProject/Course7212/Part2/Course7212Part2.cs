using ILRuntime.Runtime;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course7212Part2
    {
        private int _talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;


        //---------------------------------
        //大狗
        private GameObject _bigDog;
        private Vector2 _bigDogPos;
        private Vector2 _bigDogPos2;
        private Vector2 _bigDogPos3;
        private GameObject _bd;
        private GameObject _bigBack;

        //小狗
        private GameObject _spotDog;
        private Vector2 _spotDogPos;
        private Vector2 _spotDogPos2;
        private Vector2 _spotDogPos3;
        private GameObject _sd;
        private GameObject _spotBack;

        private GameObject _aniMask;

        private bool onClick;
        private GameObject _back;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            onClick = false;
            var curTran = curGo.transform;
            _bigDog = curTran.GetGameObject("bigdog");
            _bigDogPos = curTran.GetGameObject("bigdogPos").transform.position;
            _bigDogPos2 = curTran.GetGameObject("bigdogPos2").transform.position;
            _bigDogPos3 = curTran.GetGameObject("bigdogPos3").transform.position;
            _bd = curTran.GetGameObject("bd");
            _bigBack = curTran.GetGameObject("1");
            _spotBack = curTran.GetGameObject("0");
            _sd = curTran.GetGameObject("sd");
            _spotDog = curTran.GetGameObject("spotdog");
            _spotDogPos = curTran.GetGameObject("spotdogPos").transform.position;
            _spotDogPos2 = curTran.GetGameObject("spotdogPos2").transform.position;
            _spotDogPos3 = curTran.GetGameObject("spotdogPos3").transform.position;
            _aniMask = curTran.GetGameObject("animask");
            Util.AddBtnClick(_bd, ShowDog);
            Util.AddBtnClick(_sd, ShowDog);
            Util.AddBtnClick(_aniMask, Back);
            _back = curTran.GetGameObject("background");
            ReStart(curGo);
        }

        private void Back(GameObject obj)
        {
            _aniMask.Hide();
            if (temp == _sd)
            {
                SpineManager.instance.DoAnimation(_spotDog, "b4", false, () =>
                {
                    _bigDog.transform.position = _bigDogPos;
                    _spotDog.transform.position = _spotDogPos;
                    _bigDog.Show();
                    _spotDog.Show();
                    SpineManager.instance.DoAnimation(_bigDog, "jing");
                    SpineManager.instance.DoAnimation(_spotDog, "jing2");
                    _bd.Show();
                    _sd.Show();
                    _back.Show();
                    _bigBack.Hide();
                    _spotBack.Hide();
                    _aniMask.Hide();
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(_bigDog, "a4", false, () =>
                {
                    _bigDog.transform.position = _bigDogPos;
                    _spotDog.transform.position = _spotDogPos;
                    _bigDog.Show();
                    _spotDog.Show();
                    SpineManager.instance.DoAnimation(_bigDog, "jing");
                    SpineManager.instance.DoAnimation(_spotDog, "jing2");
                    _bd.Show();
                    _sd.Show();
                    _back.Show();
                    _bigBack.Hide();
                    _spotBack.Hide();
                    _aniMask.Hide();
                });
            }
        }

        private void ReStart(GameObject obj)
        {
            Transform curTrans = curGo.transform;

            bell = curTrans.Find("bell").gameObject;
            bell.Show();
            _bigDog.transform.position = _bigDogPos;
            _spotDog.transform.position = _spotDogPos;
            _bigDog.Show();
            _spotDog.Show();
            SpineManager.instance.DoAnimation(_bigDog, "kong", false, () => { SpineManager.instance.DoAnimation(_bigDog, "jing"); });
            SpineManager.instance.DoAnimation(_spotDog, "kong", false, () => { SpineManager.instance.DoAnimation(_spotDog, "jing2"); });
            SpineManager.instance.DoAnimation(_back, "kong", false, () => { SpineManager.instance.DoAnimation(_back, "jing3"); });
            SpineManager.instance.DoAnimation(_back.transform.GetGameObject("0"), "kong", false, 
            () => 
            { 
                SpineManager.instance.DoAnimation(_back.transform.GetGameObject("0"), "jing4"); 
            });

            _bd.Hide();
            _sd.Hide();
            SpineManager.instance.DoAnimation(_bigBack, "kong", false, () => { _bigBack.Hide(); });
            SpineManager.instance.DoAnimation(_spotBack, "kong", false, () => { _spotBack.Hide(); });
            _aniMask.Hide();
            _back.Show();
            SoundManager.instance.ShowVoiceBtn(false); //语音键

            //Util.AddBtnClick(_button, ReStart);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                bell.Hide();
                _bd.Show();
                _sd.Show();
                onClick = true; 
            }));
        }


        private GameObject temp;

        /// <summary>
        /// 放大图片
        /// </summary>
        /// <param name="obj"></param>
        private void ShowDog(GameObject obj)
        {
            if (onClick)
            {
                temp = obj;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                _bigDog.transform.position = _bigDogPos2;
                _spotDog.transform.position = _spotDogPos2;
                if (temp == _bd)
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () =>
                    {
                        _bigBack.Show();
                        _spotDog.Hide();
                        _spotBack.Hide();
                        _bigDog.Show();
                        _bd.Hide();
                        _sd.Hide();
                        _back.Hide();
                        _bigDog.transform.position = _bigDogPos3;
                        SpineManager.instance.DoAnimation(_bigBack, "x2", false);
                        Wait(4.8f, () =>
                        {
                            SpineManager.instance.DoAnimation(_bigBack, "x", false);
                        });

                        SpineManager.instance.DoAnimation(_bigDog, "kong", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_bigDog, "a3", false,
                            () =>
                            {
                                Wait(3,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                                    Wait(6.5f, () =>
                                    {
                                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                        var t = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,
                                            1, false);
                                        Wait(t + 2f,
                                            () =>
                                            {
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,
                                                    0, true);
                                            });
                                    });
                                    SpineManager.instance.DoAnimation(_bigDog, "a1", false,
                                    () =>
                                    {
                                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                    });
                                });
                            });
                        });
                    }, () => { _aniMask.Show(); }));
                }
                else
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                    {
                        _bigBack.Hide();
                        _bigDog.Hide();
                        _spotDog.Show();
                        _spotBack.Show();
                        _bd.Hide();
                        _sd.Hide();
                        _back.Hide();

                        SpineManager.instance.DoAnimation(_spotBack, "2", false);
                        _spotDog.transform.position = _spotDogPos3;

                        Wait(3.5f, () =>
                        {
                            SpineManager.instance.DoAnimation(_spotBack, "1", false);
                        });

                        SpineManager.instance.DoAnimation(_spotDog, "kong", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_spotDog, "b3", false,
                                () =>
                                {
                                    Wait(2,
                                    () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                                        Wait(6f, () =>
                                        {
                                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                            var t = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,
                                                3, false);
                                            Wait(t + 2f,
                                            () =>
                                            {
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                                            });
                                        });
                                        SpineManager.instance.DoAnimation(_spotDog, "b1", false,
                                        () =>
                                        {
                                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                        });
                                    });
                                });
                        });
                    }, () => { _aniMask.Show(); }));
                }
            }
            else

            {
                return;
            }
        }

        void Wait(float time, Action methon = null)
        {
            mono.StartCoroutine(WaitForDo(time, methon));
        }

        IEnumerator WaitForDo(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }


        /// <summary>
        /// 播放动画协程
        /// 中途等待两秒
        /// </summary>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <returns></returns>
        IEnumerator WaitCoroutine(Action method_1 = null, Action method_2 = null)
        {
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSecondsRealtime(2.0f);
            if (method_2 != null)
            {
                method_2();
            }
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            method_1?.Invoke();
            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {
            }

            _talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}