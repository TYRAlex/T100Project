using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course916Part1
    {

       


        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;    
        private GameObject _bell;
        private GameObject _mask;
        private GameObject _di;
        private GameObject _spine1a;
        private GameObject _spine1b;
        private GameObject _spine1bDiZuo;

        private GameObject _bg2;

        private Transform _onClicks;



        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;

            _bell = curTrans.GetGameObject("bell");
            _mask = curTrans.GetGameObject("mask");
            _di = curTrans.GetGameObject("di");
            _spine1a = curTrans.GetGameObject("Spines/1a");
            _spine1b = curTrans.GetGameObject("Spines/1b");
            _spine1bDiZuo = curTrans.GetGameObject("Spines/1bdizuo");
            _bg2 = curTrans.GetGameObject("Bg/2");
            _onClicks = curTrans.GetTransform("OnClicks");

            GameInit();         
        }

        void GameInit()
        {         
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
                  
            SpineManager.instance.DoAnimation(_spine1a, "jingzhi2", false);

            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i);
                PointerClickListener.Get(child.gameObject).onClick = null;
                PointerClickListener.Get(child.gameObject).onClick = OnClickEvent;
            }

            _spine1bDiZuo.Hide();
            _spine1b.Hide();
            _bg2.Hide();
            _di.Show();
            _bell.Show();

            GameStart();        
        }

        
        void GameStart()
        {

            PlayBgm(0);

            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0,
                ()=> { _mask.Show(); },
                ()=> { _mask.Hide();_bg2.Show(); _bell.Hide();_di.Hide();
                    SpineManager.instance.DoAnimation(_spine1a, "jingzhi3", false,()=> { _onClicks.gameObject.Show(); });
                }));
        }

        private void OnClickEvent(GameObject go)
        {
            BtnPlaySound();

            var name = go.name;
            _mask.Show();

            switch (name)
            {
                case "changbi":
                    PlayVoice(1);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,
                        ()=> { Specking(_spine1a,"changtou"); },
                        ()=> { SpeckEnd(); }));
                    break;
                case "changtou":
                    PlayVoice(0);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 2,                        
                        () => {
                            _spine1b.Show();
                        Specking(_spine1a, "changbi",
                          () => { Specking(_spine1b, "a1",
                               () => {Specking(_spine1b, "a2", () => { Specking(_spine1b, "a3",null,true); }); });
                          });
                        },
                        () => {
                            Specking(_spine1b, "a4", () => { _spine1b.Hide(); });
                            SpeckEnd(); }));
                    break;
                case "laba":
                case "laba1":
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,
                        ()=> { Specking(_spine1a, "laba"); }, () => { SpeckEnd(); }));
                    break;
                case "dizuo1":
                case "dizuo2":
                    PlayVoice(2,true);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 5, () => {
                        _spine1bDiZuo.Show();
                        Specking(_spine1bDiZuo, "b1",()=> { Specking(_spine1bDiZuo,"b2",()=> { Specking(_spine1bDiZuo, "b3",null,true);}); });
                        Specking(_spine1a, "dizuo");
                    }, () => {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        Specking(_spine1bDiZuo, "b4", () => {  _spine1bDiZuo.Hide(); });
                       
                        SpeckEnd();

                    }));
                    break;
                case "shoubing":
                    PlayVoice(3);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,4,()=> { Specking(_spine1a,name); },()=> { SpeckEnd();}));
                    break;
            }
        }


        void Specking(GameObject go, string aniName,Action callBakc =null,bool isLoop=false)
        {
            SpineManager.instance.DoAnimation(go, aniName, isLoop, callBakc);
        }


        void SpeckEnd()
        {
            _mask.Hide();
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
            SpineManager.instance.DoAnimation(_bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "DAIJI");
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
            if (_talkIndex == 1)
            {

            }
            _talkIndex++;
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

        #region 播放Audio
        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);
            string log = string.Format("index:{0}---type:{1}---time:{2}---isLoop:{3}", index, type, time, isLoop);
            Debug.Log(log);
            return time;
        }
        #endregion
    }
}
