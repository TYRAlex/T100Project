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
    public class Course913Part2
    {
      
   
      
        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _bell;
     
        private List<string> _cheackFinish;
       
        private Transform _spines;
        private Transform _onClicks;


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;
           
            _mask = curTrans.GetGameObject("mask");
            _bell = curTrans.GetGameObject("bell");
       
            _spines = curTrans.GetTransform("Spines");
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

            _bell.Hide();
            _mask.Hide();
            PointerClickListener.Get(_mask).onClick = null;

         
            for (int i = 0; i < _spines.childCount; i++)
            {
                var child = _spines.GetChild(i);
                child.gameObject.Show();
                SpineManager.instance.DoAnimation(child.gameObject, "kong", false,()=> {
                    SpineManager.instance.DoAnimation(child.gameObject, child.name, false);
                });            
            }
          

            for (int i = 0; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClickCard;
            }

           
            _spines.gameObject.Show();
            _onClicks.gameObject.Show();
            _cheackFinish = new List<string>();


            GameStart();
        }

       
        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,
            ()=> { _mask.Show(); },
            ()=> { _mask.Hide(); }));
        }
   
        void OnClickCard(GameObject go)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            var name = go.name;
            BtnPlaySound();
            _mask.Show();
            HideSpinesChild(name);
            if (!_cheackFinish.Contains(name))
                _cheackFinish.Add(name);

            var aniGo = _spines.GetGameObject(name);

            switch (name)
            {
                case "K1":
                    PlayVoice(2);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 1, () => {
                        SpineManager.instance.DoAnimation(aniGo, "K1-ANI", false, () => {
                            SpineManager.instance.DoAnimation(aniGo, "K1-ANI1", false, () => {
                                SpineManager.instance.DoAnimation(aniGo, "K1-ANI1", false);
                            });
                        });                 
                    },
                     () => { SpeakEnd(aniGo, "K1-ANI3"); }));
               
                    break;
                case "K2":
                    PlayVoice(0);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,
                       () => {
                           _mono.StartCoroutine(Delay(0.3f,()=> {
                               SpineManager.instance.DoAnimation(aniGo, "K2-ANI", false, () => {
                                   _mono.StartCoroutine(Delay(1.0f, () => { SpineManager.instance.DoAnimation(aniGo, "K2-ANI2", false); }));
                               });
                           }));
                                            
                       },
                       () => {SpeakEnd(aniGo, "K2-ANI3"); }));
                    break;
                case "K3":
                    PlayVoice(1);
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,3,
                    ()=> { SpineManager.instance.DoAnimation(aniGo, "K3-ANI", false); _mono.StartCoroutine(Delay(3.3F,()=> { SpineManager.instance.DoAnimation(aniGo, "K3-ANI2", false); })); },
                    () => { SpeakEnd(aniGo, "K3-ANI3"); }));
                    break;
            }
        }


        void PlayVoice(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, false);
        }

        void SpeakEnd(GameObject go,string aniName)
        {
            PointerClickListener.Get(_mask).onClick = m => {
                PointerClickListener.Get(_mask).onClick = null;
                SpineManager.instance.DoAnimation(go, aniName, false, () => {

                    bool isFinish = _cheackFinish.Count == _onClicks.childCount;
                    if (isFinish)
                        SoundManager.instance.ShowVoiceBtn(true);
                    _mask.Hide();
                    ShowSpinesChild();
                });
            };
        }

        void HideSpinesChild(string name)
        {
            for (int i = 0; i < _spines.childCount; i++)
            {
                var child = _spines.GetChild(i).gameObject;
                if (child.name !=name)                
                    child.Hide();                
            }
        }

        void ShowSpinesChild()
        {
            for (int i = 0; i < _spines.childCount; i++)           
             _spines.GetChild(i).gameObject.Show();                                            
        }




        IEnumerator Delay(float delay, Action callBack)
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
                _spines.gameObject.Hide();
                _onClicks.gameObject.Hide();
             //   _bell.transform.GetRectTransform().anchoredPosition = new Vector2(960, 300);
                _bell.Show();
                _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 4));

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
