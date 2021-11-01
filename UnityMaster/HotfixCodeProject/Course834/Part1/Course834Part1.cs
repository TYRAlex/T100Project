using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course834Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _car;
        GameObject _demon;
        GameObject _button;
        private bool _isPlay;
        private LongPressButton _pressButton;
        float _time;
        bool _play;
        GameObject _mask;
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            _car = curTrans.GetGameObject("obj/car");
            _demon = curTrans.GetGameObject("demon");
            _button = _car.transform.GetGameObject("button");
            Util.AddBtnClick(btnTest, ReStart);
            _pressButton = _button.GetComponent<LongPressButton>();
            _mask = curTrans.GetGameObject("mask");
            _isPlay = true;
            UIEventListener.Get(_button).onDown = null;
            UIEventListener.Get(_button).onUp = null;

            _play = true;

            _pressButton.OnDown =
                () =>
                {

                    if (_play)
                    {
                        SoundManager.instance.Stop("sound");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        _time = 0f;
                        _play = false;
                        SpineManager.instance.DoAnimation(_car, "6", false,
                            () => {});
                     //   SpineManager.instance.DoAnimation(_car, "7", false);
                        SpineManager.instance.DoAnimation(_demon, "animation", true);
                    }

                    _time += 0.1f;
                    Debug.Log(_time);

                    if (_time > 2f)
                    {
                        if (_isPlay)
                        {
                            SpineManager.instance.DoAnimation(_demon, "animation2", true);
                            _isPlay = false;
                        }
                    }

                    //Debug.LogError(_time);
                };
            _pressButton.OnUp = () =>
            {
                SpineManager.instance.DoAnimation(_car, "8", false);
                Debug.Log("songkai");
                SoundManager.instance.Stop("sound");    
                if (_time < 2f)
                {                   
                    SpineManager.instance.DoAnimation(_demon, "A", true, () => { SpineManager.instance.DoAnimation(_demon, "A", true, () => { SpineManager.instance.DoAnimation(_demon, "0", true); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true); }); });
                    
                }

                if (_time >= 2f)
                {
                    SpineManager.instance.DoAnimation(_demon, "animation2", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
                            SpineManager.instance.DoAnimation(_demon, "0", true);
                        });
                }

                _isPlay = true;
                _play = true;
            };


            btnTest.SetActive(false);
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
            _car.Show();
            _mask.Hide();
            _car.transform.position = new Vector3(0, 0);
            _demon.Hide();
            _button.Hide();
            _play = true;
            SpineManager.instance.DoAnimation(_car, "1", false);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            if (bellTextures.texture.Length <= 0)
            {
            }
            else
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];

            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    //Debug.LogError("今天我们需要搭建一个能感知到碰撞发生的碰碰车，请思考如何让它更全面检测到碰撞呢？");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(_car, "2", false);
                },
                () => { SoundManager.instance.ShowVoiceBtn(true); }
            ));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                Sence();
            }

            if (talkIndex == 2)
            {
                Sence2();
            }

            if (talkIndex == 3)
            {
                Sence3();
            }

            talkIndex++;
        }

        private void Sence3()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            _car.Show();
            _car.transform.position = new Vector3(30, 50);
            _demon.Hide();
            _button.Hide();
            SpineManager.instance.DoAnimation(_car, "3", false);
           
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
            {
                //Debug.LogError("请同学们设置程序，使触碰传感器在不同的状态下执行不同的指令吧");
            }, null));
        }

        private void Sence2()
        {
            _car.Show();
            SpineManager.instance.DoAnimation(_car, "3", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
            _car.GetComponent<RectTransform>().anchoredPosition = new Vector3(435, 130);
            _demon.Show();
            _button.Show();
            SpineManager.instance.DoAnimation(_demon, "0", true);
            SoundManager.instance.ShowVoiceBtn(true);
        }

        void Sence()
        {
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            _car.Show();
            _car.transform.position = new Vector3(-30, 50);
            SpineManager.instance.DoAnimation(_car, "3", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    //Debug.LogError("我们可以在小车上安装触碰传感器，专门用来检测是否有碰撞情况发生。触碰传感器前端有个按钮");
                    SpineManager.instance.DoAnimation(_car, "4", true);
                },
                () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () =>
                        {
                            //Debug.LogError("相当于一个“开关”");
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                            //播放手动画
                            SpineManager.instance.DoAnimation(_car, "5", false);
                        },
                        () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                                () =>
                                {
                                    // Debug.LogError("请探索触碰传感器有几种状态吧！");
                                },
                                () => { SoundManager.instance.ShowVoiceBtn(true); }
                            ));
                        }
                    ));
                }
            ));
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



    }
}