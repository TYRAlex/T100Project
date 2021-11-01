using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseLeapOfSalmanPart5
    {
        GameObject curGo, npc;
        int voiceCount, voiceNum;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            PlayAnimationVoice(curGo, 0.3f);
        }

        public void PlayAnimationVoice(GameObject go, float voiceVolume, int voiceNum = 1)
        {
            Transform curTrans = go.transform;
            this.voiceNum = voiceNum;
            voiceCount = 0;
            npc = curTrans.Find("npc").gameObject;
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = voiceVolume;

            SkipBtnEvent();
        }

        void SkipBtnEvent()
        {
            if (voiceCount < voiceNum)
            {
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, voiceCount, null, () =>
                {
                    npc.SetActive(true);
                    SpineManager.instance.DoAnimation(npc, "breath", true);
                    SoundManager.instance.ShowVoiceBtn(true);
                    SoundManager.instance.SetVoiceBtnEvent(SkipBtnEvent);
                    if (voiceNum > 1)
                    {
                        voiceCount++;
                    }
                });
            }
        }

        void VoiceBtnEvent()
        {
            if (voiceCount >= voiceNum)
            {
                voiceCount--;
            }
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, voiceCount, null, () =>
            {
                npc.SetActive(true);
                SpineManager.instance.DoAnimation(npc, "breath", true);
                SoundManager.instance.ShowVoiceBtn(true);
                SoundManager.instance.SetVoiceBtnEvent(VoiceBtnEvent);
            });
        }
    }
}
