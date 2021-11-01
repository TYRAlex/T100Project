using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;

namespace ILFramework.HotClass
{
    public class Course7212Part1
    {
        private int _talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        GameObject _liujiao;
        //GameObject _button;

        void Start(object o)
        {
            curGo = (GameObject) o;
            Restart(curGo);
        }

        void Restart(GameObject o)
        {
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.GetGameObject("bell");
            _liujiao = curTrans.GetGameObject("liujiao");
            _talkIndex = 1;
            //_button = curTrans.GetGameObject("Button");

            SoundManager.instance.ShowVoiceBtn(false); //语音键
            SoundManager.instance.StopAudio();
            mono.StopAllCoroutines();

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    //播放行走动画和bell说话动画
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                    SpineManager.instance.DoAnimation(_liujiao, "animation", true);
                    SoundManager.instance.ShowVoiceBtn(false);
                },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }

        //语音键点击事件
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (_talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                    () =>
                    {
                        Wait(1f,
                            () =>
                            {
                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                                SpineManager.instance.DoAnimation(_liujiao, "2", false, () =>
                                {
                                    
                                });
                            });
                        Wait(10f, () =>
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                            SpineManager.instance.DoAnimation(_liujiao, "animation3", true);
                        });
                    },
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                      
                        SpineManager.instance.DoAnimation(_liujiao, "animation2", false);
                    }
                ));
            }

            if (_talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                    () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                        SpineManager.instance.DoAnimation(_liujiao, "animation", true);
                    },
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SpineManager.instance.DoAnimation(_liujiao, "animation2", false);
                    }
                ));
            }

            if (_talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                    () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                        SpineManager.instance.DoAnimation(_liujiao, "animation", true);
                    },
                    () => { }
                ));
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

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForDoSomething(time, method));
        }

        IEnumerator WaitForDoSomething(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
    }
}