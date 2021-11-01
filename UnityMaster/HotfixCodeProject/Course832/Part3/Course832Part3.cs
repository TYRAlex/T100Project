using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace ILFramework.HotClass
{
    public class Course832Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private GameObject _spine3;
        private GameObject _spineFinish;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;
          
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _spine3 = curTrans.Find("Spines/3").gameObject;
            _spineFinish = curTrans.Find("Spines/Finish").gameObject;

            GameInit();
          
            SpineManager.instance.DoAnimation(_spine3, "k", false, () =>
            {
                GameStart();
            });

        }

        void GameInit()
        {
            _spineFinish.Hide();

            talkIndex = 1;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();


        }

        void GameStart()
        {            
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

           var time=  SpineManager.instance.DoAnimation(_spine3, "animation", false);

            mono.StartCoroutine(Delay(18.5f,()=> {
                SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            }));

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,0,()=> {

                mono.StartCoroutine(Delay(2.0f,()=> {

                    Debug.LogError("Hello");
                }));

                mono.StartCoroutine(Delay(18.5f, () => {
                    _spineFinish.Show();

                    SpineManager.instance.DoAnimation(_spineFinish, "animation", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                }));
            }));
        }


        private IEnumerator Delay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
