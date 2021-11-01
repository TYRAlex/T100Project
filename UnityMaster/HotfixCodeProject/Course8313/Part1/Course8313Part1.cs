using Spine.Unity;
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
    public class Course8313Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;
        private GameObject _bell;
        private GameObject _mask;

        private GameObject _spien1;
        private GameObject _spine123;


        private Transform _onClicks;
        private SkeletonGraphic _skeletonGraphic1;
        private SkeletonGraphic _skeletonGraphic123;
        private Image _bg2Img;
        private float _speed;
        private bool _isPlayVoice;



        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;
           

               _bell = curTrans.GetGameObject("bell");
            _mask = curTrans.GetGameObject("mask");
         

            _spien1 = curTrans.GetGameObject("Spines/1");
            _spine123 = curTrans.GetGameObject("Spines/123");
            _skeletonGraphic1 = _spien1.GetComponent<SkeletonGraphic>();
            _skeletonGraphic123 = _spine123.GetComponent<SkeletonGraphic>();
            _onClicks = curTrans.GetTransform("OnClicks/Bg/OnClicks");
            _bg2Img = curTrans.GetImage("OnClicks/Bg/bg2");

       

            GameInit();
        }
      
        void GameInit()
        {
            _isPlayVoice = false;
            _bell.Show();
    
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            

            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _bg2Img.fillAmount = 0;
            _speed = 0;
            _skeletonGraphic1.timeScale = _speed;
            _skeletonGraphic123.timeScale = _speed;

            GameStart();
          
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(_spien1, "animation", true);
            SpineManager.instance.DoAnimation(_spine123, "animation", true);
            AddOnClickEvent();
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 5,
             () => { _mask.Show(); },
             () => { _mask.Hide(); _bell.Hide(); }));
            //_mono.StartCoroutine(Delay(1.0f,()=>{
              
            //}));
           
        }

        void AddOnClickEvent()
        {
            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = go =>
                {
                    var name = go.name;
                    _speed = float.Parse(name);
                    float temp = 0;
                    int tempVoiceIndex = -1;
                    switch (name)
                    {
                        case "0.1":
                            temp = 0.18f;
                            tempVoiceIndex = 2;
                            if (!_isPlayVoice)
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                                _isPlayVoice = true;
                            }
                            break;
                        case "1":
                            tempVoiceIndex = 2;
                            temp = 0.34f;
                            break;
                        case "1.5":
                            temp = 0.53f;
                            tempVoiceIndex = 3;
                            break;
                        case "2":
                            temp = 0.76f;
                            tempVoiceIndex = 3;
                            break;
                        case "20":
                            temp = 1f;
                            tempVoiceIndex = 4;
                            break;
                    }
                    _bg2Img.fillAmount = temp;
                    _skeletonGraphic1.timeScale = _speed;
                    _skeletonGraphic123.timeScale = _speed;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, tempVoiceIndex, true);

                };
            }
        }

        

        IEnumerator Delay(float delay,Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
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
            SpineManager.instance.DoAnimation(_bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(_bell, "daiji");
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
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 1,()=> { _bell.Show(); },()=> { _bell.Hide(); }));
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
    }
}
