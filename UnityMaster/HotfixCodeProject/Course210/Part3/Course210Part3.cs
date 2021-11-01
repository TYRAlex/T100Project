using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course210Part3
    {
        private GameObject specker;//说话人物

        private int beforeClip;//收拾积木前语音
        private int afterClip;//收拾积木后语音
        private string audios;//语音类型
        private string specker_idle;//人物待机动画
        private string specker_speck;//人物说话动画

        private SoundManager.SoundType soundType;
        private MonoBehaviour mono;
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            specker = curTrans.Find("Bell").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        void GameInit()
        {
            beforeClip = 0;//收拾积木前语音
            afterClip = 1;//收拾积木后语音
            audios = "VOICE";//语音类型
            specker_idle = "DAIJI";//人物待机动画
            specker_speck = "DAIJIshuohua";//人物说话动画

            if (audios == "BGM") soundType = SoundManager.SoundType.BGM;
            else if (audios == "VOICE") soundType = SoundManager.SoundType.VOICE;
            else if (audios == "SOUND") soundType = SoundManager.SoundType.SOUND;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameStart();
        }

        void GameStart()
        {
            //背景音乐
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(soundType, beforeClip, () => { }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            mono.StartCoroutine(SpeckerCoroutine(soundType, afterClip, () => { }, () => { }));
        }

        //人物说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SpineManager.instance.DoAnimation(specker, specker_speck);
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(specker, specker_idle);
            if (method_2 != null)
            {
                method_2();
            }
        }
    }
}