using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course746Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private float _speakTime = 0;
        private GameObject _mainTarget;
        private GameObject _textSpine;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            _mainTarget = curTrans.GetGameObject("MainTarget");
            _textSpine = curTrans.GetGameObject("TextSpine");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _speakTime = 0;
            _textSpine.Hide();
            _mainTarget.Show();
            PlaySpine(_mainTarget, "qiao");
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            bellSpeak(0,Max, () =>
                {
                    PlaySpine(_mainTarget, "qiao2",null,true);
                },()=>SoundManager.instance.ShowVoiceBtn(true));
            

        }

       



        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            _speakTime = ind;
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    bellSpeak(1,Max, () =>
                    {
                        Wait(_speakTime*0.5f, () =>
                        {
                        
                            _textSpine.Show();
                            PlaySpine(_textSpine, "zi", 
                                () => SpineManager.instance.DoAnimation(_textSpine, "zi2", false));
                        });
                    }, () =>
                    {
                        Wait(0.5f, () =>
                        {
                            _textSpine.Hide();
                            Max.Hide();
                            Wait(PlayVoice(2), () =>
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            });
                            PlaySpine(_mainTarget, "che2", () => PlaySpine(_mainTarget, "che3",null,true));
                        });
                    });
                    break;
                case 2:
                    Max.Show();
                    PlaySpine(_mainTarget, "qiao2");
                    _textSpine.Show();
                    bellSpeak(3);
                    break;
            }
            

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }

        void bellSpeak(int index,GameObject target=null,Action speakingEvent=null,Action speakbleEvent=null)
        {
            if (target == null)
                target = Max;
            mono.StartCoroutine(
                SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, speakingEvent, speakbleEvent));
        }

        void PlaySpine(GameObject target, String aniName, Action callback=null, bool isLoop = false)
        {
            SpineManager.instance.DoAnimation(target, aniName, isLoop, callback);
        }

        float PlayVoice(int index,bool isLoop=false)
        {
            return SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, false);
        }

        void Wait(float timer,Action callback)
        {
            mono.StartCoroutine(WaitEventIE(timer, callback));
        }

        IEnumerator WaitEventIE(float timer, Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }
        
        
    }
}
