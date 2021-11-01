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
    public class Course831Part1
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
        GameObject _diannao;
        GameObject _shexiangtou;
        GameObject _leida;
        GameObject _leida1;
        GameObject _chuanganqi;
        GameObject _luntai;
        GameObject _luntai1;


        void Start(object o)
        {
            curGo = (GameObject) o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            _car = curTrans.GetGameObject("car");
          
            _chuanganqi = curTrans.GetGameObject("chuanganqi");
            _diannao = curTrans.GetGameObject("diannao");
            _leida = curTrans.GetGameObject("leida");
            _leida1 = curTrans.GetGameObject("leida1");
            _shexiangtou = curTrans.GetGameObject("shexiangtou");
            _luntai = curTrans.GetGameObject("luntai");
            _luntai1 = curTrans.GetGameObject("luntai1");


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
            talkIndex = 1;
              
      
            InitSpine();
            SoundManager.instance.ShowVoiceBtn(true);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void InitSpine()
        {
            InitSpine(_car);
            InitSpine(_luntai1);
            InitSpine(_leida); 
            InitSpine(_leida1); 
            InitSpine(_shexiangtou); 
            InitSpine(_luntai);
            InitSpine(_chuanganqi);
            InitSpine(_diannao);
        }

        void InitSpine(GameObject go,string nameAni="J")
        {
            SpineManager.instance.DoAnimation(go, nameAni, false);
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () => { SoundManager.instance.ShowVoiceBtn(false); },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                //Debug.LogError("“既然无人驾驶汽车是机器人，那它的三大能力是如何体现的？");
                SoundManager.instance.ShowVoiceBtn(false);
                SaySomethings();
            }

            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null,
                    () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }

            if (talkIndex == 3)
            {
                //Debug.LogError("今天我们要学习一款车型机器人的制作，你们准备好了吗？");
                SoundManager.instance.ShowVoiceBtn(false);
                //Debug.LogError("“车型机器人已经设计好了，接下来请为它编写一个启动程序，让它动起来吧”");
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null,
                    () => { SoundManager.instance.ShowVoiceBtn(false); }));
            }

            talkIndex++;
        }

        //多个动画
        void SaySomethings()
        {
            //Debug.LogError("车载雷达是");
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    _leida.Show();
                    SpineManager.instance.DoAnimation(_leida, "3", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
       
                            _chuanganqi.Show();
                            SpineManager.instance.DoAnimation(_chuanganqi, "13", false,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    
                                    _shexiangtou.Show();
                                    
                                    SpineManager.instance.DoAnimation(_shexiangtou, "2", false, () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    });
                                });
                        });
                    _leida1.Show();
                    SpineManager.instance.DoAnimation(_leida1, "5", false,
                        () => {  });
                },
                () =>
                {
                    SpineManager.instance.DoAnimation(_leida, "9", false);
                    SpineManager.instance.DoAnimation(_chuanganqi, "7", false);
                    SpineManager.instance.DoAnimation(_shexiangtou, "8", false);
                    SpineManager.instance.DoAnimation(_leida1, "11", false);
                    
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () =>
                        {
                            _diannao.Show();
                            SpineManager.instance.DoAnimation(_diannao, "6", false,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                                });
                        },
                        () =>
                        {  
                            SpineManager.instance.DoAnimation(_diannao, "12", false);
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    _luntai.Show();
                                    SpineManager.instance.DoAnimation(_luntai1, "zi", false, () =>
                                    {
                                    });
                                    SpineManager.instance.DoAnimation(_luntai, "14", false, () =>
                                    {
                                    });
                                }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                        }));
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