using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course839Part2
    {
       
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
     
        private GameObject bell;

        private SkeletonGraphic _skeletonGraphic;
        private GameObject _spine2;
        private Transform _onClicks;
        private GameObject _mask;

        void Start(object o)
        {          
            curGo = (GameObject)o;
            Transform  curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
         

            bell = curTrans.GetGameObject("bell");
            _spine2 = curTrans.GetGameObject("Spines/2");
            _onClicks = curTrans.GetTransform("OnClicks");
            _mask = curTrans.GetGameObject("mask");

            _skeletonGraphic = _spine2.GetComponent<SkeletonGraphic>();
            DOTween.KillAll();
            GameInit();
            GameStart();
        }

        void GameInit()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SpineManager.instance.DoAnimation(_spine2, "j", false);
            bell.Show();
            _onClicks.gameObject.Hide();
            PointerClickListener.Get(_mask).onClick = null;
            AddOnClickEvent();
        }

        void GameStart()
        {
            OnPlayBgm(0);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0,
            ()=> {
                _mask.Show();
            },
            ()=>{
                _mask.Hide();
                _onClicks.gameObject.Show();
                bell.Hide();
            }));

        }
        
        
        void AddOnClickEvent()
        {
            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClick;
            }
        }

        void OnClick(GameObject go)
        {
            BtnPlaySound();
            _mask.Show();

            string name = go.name;
            switch (name)
            {
                case "1":                   
                    OnPlayVoice(0);                 
                    OnPlayAni("d1","1","12","d4",1);
                    break;
                case "2":                    
                    OnPlayVoice(1);                
                    OnPlayAni("d2", "2","22", "d5", 2);
                    break;
                case "3":
                    OnPlayVoice(2);             
                    OnPlayAni("d3", "3","32", "d6", 3);
                    break;
            }

        }

        void OnPlayBgm(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, true);
        }

        void OnPlayVoice(int index,bool isLooo=false)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLooo);
        }

        void OnPlayAni(string ani1,string ani2, string daiji, string ani3, int index)
        {
            SpineManager.instance.DoAnimation(_spine2, ani1, false, One);

            void One()
            {
                SpineManager.instance.DoAnimation(_spine2, ani2, false, Two);
            }

            void Two()
            {
                SpineManager.instance.DoAnimation(_spine2, daiji, true);
            }

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, index, null, SpeckerEnd));

            void SpeckerEnd()
            {
                PointerClickListener.Get(_mask).onClick = go => {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    PointerClickListener.Get(_mask).onClick = null;
                    SpineManager.instance.DoAnimation(_spine2, ani3, false,()=> {
                        _mask.Hide();
                    });
                };
            }


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
