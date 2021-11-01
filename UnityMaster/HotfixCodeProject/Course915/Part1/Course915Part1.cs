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
    public class Course915Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;

        private GameObject _mainTarget;
        


        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
           

            ReStart();
        }

        void ReStart()
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;

            ResetGameProperty();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        
        }

        void ResetGameProperty()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            _mainTarget = curTrans.GetGameObject("MainTarget");
            SpineManager.instance.DoAnimation(_mainTarget, "Part", false);
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_mainTarget, "Part-ANIME1", false);
                },
                () =>
                {
                    SpineManager.instance.DoAnimation(_mainTarget, "Part", false);
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
        }

        #region BellFunc

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            SpineManager.instance.DoAnimation(_mainTarget, "Part-ANIME2", false);
                        },
                        () => SoundManager.instance.ShowVoiceBtn(true)));
                    break;
                case 2:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            SpineManager.instance.DoAnimation(_mainTarget, "Part-ANIME3", false);
                        }));
                    break;
                case 3:
                    break;
            }
            talkIndex++;
        }
        
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
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
        

        #endregion
       
    }
}
