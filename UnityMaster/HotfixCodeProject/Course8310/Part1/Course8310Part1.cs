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
    public class Course8310Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private MonoScripts _monoScripts;

        GameObject _louti;
        GameObject _dianti;
        GameObject _futi;
        GameObject _za;
        GameObject _zb;
        GameObject _zc;
        GameObject _ani;
        GameObject _ani1;
        GameObject _ani2;
        GameObject _left;
        GameObject _right;

        int _index;
        GameObject _mask;



        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            _mask = curTrans.GetGameObject("mask");
            _monoScripts = curTrans.Find("MonoScripts").GetComponent<MonoScripts>();
            _monoScripts.UpdateCallBack = null;

            _louti = curTrans.GetGameObject("louti");
            _futi = curTrans.GetGameObject("futi");
            _dianti = curTrans.GetGameObject("dianti");

            _za = curTrans.GetGameObject("louti/za");
            _zb = curTrans.GetGameObject("futi/zb");
            _zc = curTrans.GetGameObject("dianti/zc");

            _ani = curTrans.GetGameObject("changePage/ani");
            _ani1 = curTrans.GetGameObject("changePage/ani1");
            _ani2 = curTrans.GetGameObject("changePage/ani2");

            _left = curTrans.GetGameObject("changePage/left");
            _right = curTrans.GetGameObject("changePage/right");

            Util.AddBtnClick(_left, ChangePage);
            Util.AddBtnClick(_right, ChangePage);

            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.GetGameObject("btnTest");
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(true);
            ReStart(btnTest);
        }





        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            _mask.Hide();
            SoundManager.instance.Stop();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            talkIndex = 1;
            _dianti.Show();
            _futi.Show();
            _louti.Show();

            bell.Show();
            _right.Hide();
            _left.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(_za, "za1");
            SpineManager.instance.DoAnimation(_zb, "zb1");
            SpineManager.instance.DoAnimation(_zc, "zc1");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SpineManager.instance.DoAnimation(_ani1, "kong", false);
            SpineManager.instance.DoAnimation(_ani2, "kong", false);
            SpineManager.instance.DoAnimation(_ani, "kong", false);


            GameStart();
            btnTest.Hide();
        }



        void GameStart()
        {

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { _mask.Show(); }, () => { _mask.Hide(); }));
            Util.AddBtnClick(_louti, ChangeImage);
            Util.AddBtnClick(_futi, ChangeImage);
            Util.AddBtnClick(_dianti, ChangeImage);
        }



        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            if (callback != null)
            {
                callback();
            }
        }





        private void ChangeImage(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            mono.StopAllCoroutines();
            bell.Hide();
            _ani.Show();
            _left.Hide();
            _right.Hide();
            SpineManager.instance.DoAnimation(_ani1, "kong", false);
            SpineManager.instance.DoAnimation(_ani2, "kong", false);
            SpineManager.instance.DoAnimation(_ani, "kong", false);

            if (obj == _louti)
            {
                mono.StartCoroutine(Wait(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false); }));
                

                SpineManager.instance.DoAnimation(_za, "za2", false, () => { SpineManager.instance.DoAnimation(_za, "za1"); });

                SpineManager.instance.DoAnimation(_ani, "a1", false, () => { SpineManager.instance.DoAnimation(_ani, "a2"); });
            }
            //附体
            if (obj == _futi)
            {
                _index = 0;
               
                _left.Show();
                _right.Show();
                if (_index == 0) _left.Hide();
                if (_index == 4) _right.Hide();
                //连续播放
                SpineManager.instance.DoAnimation(_zb, "zb2", false, () => { SpineManager.instance.DoAnimation(_zb, "zb1"); });
                mono.StartCoroutine(Wait(1f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false); }));
                SpineManager.instance.DoAnimation(_ani, "b1", false,
                 () =>
                 {
                     SpineManager.instance.DoAnimation(_ani, "b2", false);
                 });


            }
            if (obj == _dianti)
            {

        
                Debug.LogError("电梯");
                Debug.LogError("开门");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1,false);
                SpineManager.instance.DoAnimation(_zc, "zc2", false, () => { SpineManager.instance.DoAnimation(_zc, "zc1"); });
                SpineManager.instance.DoAnimation(_ani, "f2", false, () => { SpineManager.instance.DoAnimation(_ani, "f1", false); });
                mono.StartCoroutine(Wait(2f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3); }));

                mono.StartCoroutine(Wait(3f, () => { Debug.LogError("关门"); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1); }));

                mono.StartCoroutine(Wait(5, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false); }));
                mono.StartCoroutine(Wait(12f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3); }));
                mono.StartCoroutine(Wait(8.3f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false); }));
                mono.StartCoroutine(Wait(9f, () => { Debug.LogError("开门"); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1); }));
                mono.StartCoroutine(Wait(13.5f, () => { Debug.LogError("关门"); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1); }));
            }
          


        }


        //点击换动画

        private void ChangePage(GameObject obj)
        {
            Debug.Log(_index);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0,false);
            _mask.Show();
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            if (obj == _left)
            {

                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7);
                if (_index == 1)
                {

                    SpineManager.instance.DoAnimation(_ani1, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);

                    SpineManager.instance.DoAnimation(_ani, "b1", false,
                      () =>
                      {
                          SpineManager.instance.DoAnimation(_ani, "b2", false);
                          _mask.Hide();
                      });
                }
                if (_index == 2)
                {
                    SpineManager.instance.DoAnimation(_ani, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);

                    SpineManager.instance.DoAnimation(_ani1, "c1", false,
                      () =>
                      {
                          SpineManager.instance.DoAnimation(_ani1, "c2", false);
                          _mask.Hide();
                      });
                }
                if (_index == 3)
                {
                    SpineManager.instance.DoAnimation(_ani, "kong", false);
                    SpineManager.instance.DoAnimation(_ani1, "kong", false);

                    SpineManager.instance.DoAnimation(_ani2, "d1", false,
                       () =>
                       {
                           SpineManager.instance.DoAnimation(_ani2, "d2", false);
                           _mask.Hide();
                       });
                }
                if (_index == 4)
                {
                    SpineManager.instance.DoAnimation(_ani1, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);

                    SpineManager.instance.DoAnimation(_ani, "e2", false,
                     () =>
                     {
                         SpineManager.instance.DoAnimation(_ani, "e1", false);
                         _mask.Hide();
                     });
                }
                _index--;
                Debug.LogError("左划" + _index);
                if (_index <= 0) _index = 0;
            }
            if (obj == _right)
            {

                if (_index == 0)
                {
                    SpineManager.instance.DoAnimation(_ani, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                    SpineManager.instance.DoAnimation(_ani1, "c1", false,
                     () =>
                     {
                         SpineManager.instance.DoAnimation(_ani1, "c2", false);
                         _mask.Hide();
                     });
                }
                if (_index == 1)
                {
                    SpineManager.instance.DoAnimation(_ani, "kong", false);
                    SpineManager.instance.DoAnimation(_ani1, "kong", false);

       
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                    SpineManager.instance.DoAnimation(_ani2, "d1", false,
                     () =>
                     {
                         SpineManager.instance.DoAnimation(_ani2, "d2", false);
                         _mask.Hide();
                     });
                }
                if (_index == 2)
                {
                    SpineManager.instance.DoAnimation(_ani1, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);


                    SpineManager.instance.DoAnimation(_ani, "e2", false,
                     () =>
                     {
                         _mask.Hide();
                         SpineManager.instance.DoAnimation(_ani, "e1", false);
                     });
                }
                if (_index == 3)
                {
                    SpineManager.instance.DoAnimation(_ani1, "kong", false);
                    SpineManager.instance.DoAnimation(_ani2, "kong", false);

                    SpineManager.instance.DoAnimation(_ani, "zb3", false,
                      () =>
                      {
                          SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                          SpineManager.instance.DoAnimation(_ani, "zb4", false,
                              () =>
                              {
                                  _mask.Hide();
                              });
                      });
                }
                _index++;
                Debug.Log("右滑:" + _index);
                if (_index >= 4) _index = 4;

            }

            if (_index == 0) { _left.Hide(); }
            else if (_index > 0) { _left.Show(); }
            if (_index == 4) { _right.Hide(); }
            else if (_index < 4) { _right.Show(); }

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
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");
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
    }
}
