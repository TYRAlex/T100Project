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
    public class Course837Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _ani;
        private float _clipLen01;   //第一段语音(在颜色传感器内部有光敏电阻，它对环境的光线十分敏感。)长度
        private float _clipLen02;   //第二段语音(当光线强度增加时，电阻值会减小)长度
        private float _clipLen03;   //第三段语音(电阻值的变化会引起电流大小的变化)长度

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
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
            _ani = curTrans.GetGameObject("AniPar/Ani");

            _clipLen01 = 6.5f;
            _clipLen02 = 4.1f;
            _clipLen03 = 4.5f;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _ani.SetActive(true);
            SoundManager.instance.StopAudio();
            SpineManager.instance.DoAnimation(_ani, "animation");
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

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                SpineManager.instance.DoAnimation(_ani, "animation2", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            },
            () =>
            {
                SpineManager.instance.DoAnimation(_ani, "animation3");
                SoundManager.instance.ShowVoiceBtn(true);
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
        //多重动画（不需要按语音键）协程
        IEnumerator AniCoroutine(Action method_1 = null, float len1 = 0, Action method_2 = null, float len2 = 0, Action method_3 = null, float len3 = 0, Action method_4 = null, float len4 = 0, Action method_5 = null)
        {
            if (method_1 != null)
            {
                method_1();
            }
            if (len1 > 0)
            {
                yield return new WaitForSeconds(len1);
            }
            if (method_2 != null)
            {
                method_2();
            }
            if (len2 > 0)
            {
                yield return new WaitForSeconds(len2);
            }
            if (method_3 != null)
            {
                method_3();
            }
            if (len3 > 0)
            {
                yield return new WaitForSeconds(len3);
            }
            if (method_4 != null)
            {
                method_4();
            }
            if (len4 > 0)
            {
                yield return new WaitForSeconds(len4);
            }
            if (method_5 != null)
            {
                method_5();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            //播放Bell说话动画并语音：我们可以利用颜色传感器的环境光模式来检测。
            //同时播放转换动画1：吊灯渐渐消失，同时渐渐出现颜色传感器。
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani, "animation4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                },
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani, "animation5");
                    SoundManager.instance.ShowVoiceBtn(true);
                }, 0));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        //继续播放Bell说话动画并语音：在颜色传感器内部有光敏电阻，它对环境的光线十分敏感
                        //播放转换动画：animation6->animation7。
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () =>
                        {
                            mono.StartCoroutine(AniCoroutine(
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                SpineManager.instance.DoAnimation(_ani, "animation6", false, () => { SpineManager.instance.DoAnimation(_ani, "animation7", false); });
                            }, 3.0f,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SpineManager.instance.DoAnimation(_ani, "animation8", false, () => { SpineManager.instance.DoAnimation(_ani, "animation9", false); });
                            }));
                        }, null, 0));
                    }, _clipLen01,

                    () =>
                    {
                        //继续播放Bell说话动画并语音：当光线强度增加时，电阻值会减小。同时播放原理动画1：animation10->animation11。
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani, "animation10", false);
                        },
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                            SpineManager.instance.DoAnimation(_ani, "animation11", false);
                        }));
                    }, _clipLen02,

                    () =>
                    {
                        //继续播放Bell说话动画并语音：电阻值的变化会引起电流大小的变化。同时播放原理动画2：animation12。
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani, "animation12", false);
                        }, null));
                    }, _clipLen03,

                    () =>
                    {
                        //继续播放Bell说话动画并语音：经过一系列信号处理后，光线的强弱就用数值表现出来了。同时播放原理动画3：animation13->animation14->animation15
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                        () =>
                        {
                            mono.StartCoroutine(AniCoroutine(
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_ani, "animation13", false);
                            }, 1.0f,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_ani, "animation14", false);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                            }, 0, null, 0, null, 0, null));
                        },
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_ani, "animation15", false);
                        }));
                    }
            ));
            }
            if (talkIndex == 3)
            {
                //进入下一环节TODO
                
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
    }
}
