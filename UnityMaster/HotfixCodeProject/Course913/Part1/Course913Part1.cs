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
    public class Course913Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _bell;
        private GameObject _tou;
        private Transform _onClicks;
        private GameObject _spine;
        private GameObject _spine1;


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            Transform curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _bell = curTrans.GetGameObject("bell");

            _onClicks = curTrans.Find("OnClicks");
            _spine = curTrans.GetGameObject("OnClicks/Spine");
            _spine1 = curTrans.GetGameObject("OnClicks/Spine1");
            _tou = curTrans.GetGameObject("OnClicks/Tou");
            GameInit();

        }

        void GameInit()
        {        
            _mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

          
            PlaySpine(_spine, "kong");
            PlaySpine(_spine1, "kong");
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            AddOnClickEvent();

            GameStart();

        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            BellSpeck(0, () => { _mask.Show(); }, () => { _mask.Hide(); });
        }

        void AddOnClickEvent()
        {
            for (int i = 4; i < _onClicks.childCount; i++)
            {
                var child = _onClicks.GetChild(i).gameObject;
                PointerClickListener.Get(child).onClick = null;
                PointerClickListener.Get(child).onClick = OnClick;
            }
        }

        private void OnClick(GameObject go)
        {
            BtnPlaySound();

            var name = go.name;
            Debug.LogError("OnClick Name："+name);
            _mask.Show();
            PlaySpine(_spine,name);
            switch (name)
            {
                case "G1-ANI":     //反光镜  
                    PlayVoice(0);
                    BellSpeck(3,
                        ()=> { PlaySpine(_spine1, "T2-ANI"); },
                        ()=> { PlaySpine(_spine1, "T2-ANI2", () => { PlayVoice(1); _mask.Hide(); }); });  
                    break;
                case "G2-ANI":    //细准焦螺旋
                   
                    BellSpeck(7,()=> { PlaySpine(_tou,"JT-ANI2"); },()=> { _mask.Hide(); });
                    break;
                case "G3-ANI":    //粗准焦螺旋
                    BellSpeck(6,()=> { PlaySpine(_tou, "JT-ANI"); },()=> { _mask.Hide(); });
                    break;
                case "G4-ANI":    //目镜
                    PlayVoice(3);
                    BellSpeck(1,
                        ()=> { Delay(3.5f, () => { PlaySpine(_spine1,"T5-ANI"); }); },
                        ()=> { PlaySpine(_spine1,"T5-ANI2",()=> { PlayVoice(1); _mask.Hide(); }); });
                    break;
                case "G5-ANI":    //载物台
                    PlayVoice(5);
                    BellSpeck(5,
                        ()=> { PlaySpine(_spine1, "T1-ANI"); },
                        ()=> { PlaySpine(_spine1, "T1-ANI2", () => { PlayVoice(1); _mask.Hide(); }); });
                    break;
                case "G6-ANI":    //转换器
                    PlayVoice(4);
                    BellSpeck(2,
                        ()=> { PlaySpine(_spine1,"T3-ANI"); },
                        ()=> { PlaySpine(_spine1,"T3-ANI2",()=> { PlayVoice(1); _mask.Hide(); }); });
                    break;
                case "G7-ANI":                 
                case "G8-ANI":    //物镜
                    PlayVoice(2);
                    BellSpeck(4,
                        ()=> { Delay(3.5f, () => { PlaySpine(_spine1, "T4-ANI"); }); },
                        ()=> { PlaySpine(_spine1, "T4-ANI2", () => { PlayVoice(1); _mask.Hide(); }); });
                    break;
            }
        }

        private void PlayVoice(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, false);
        }

        private void PlaySpine(GameObject go, string name,Action callBack=null)
        {
          
            var time= SpineManager.instance.DoAnimation(go, name, false, callBack);
            Debug.LogError("Spien："+go.name+"；SpineAniName ：" + name +"；Time："+time);
        }

        private void BellSpeck(int index,Action specking=null,Action speckend=null)
        {
            Debug.LogError("Sound："+index);
            _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,index,specking,speckend));
        }

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        IEnumerator IEDelay(float delay,Action callBack)
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
