using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework
{
    public class ContactPartManager : Manager<ContactPartManager>
    {
        GameObject curGo, npc;
        int voiceCount, voiceNum;

        public void PlayVoice(GameObject go, float voiceVolume)
        {
            this.curGo = go;
            Transform curTrans = curGo.transform;

            npc = curTrans.Find("npc").gameObject;
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = voiceVolume;
            SoundManager.instance.Speaking(npc, "animation2", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                npc.SetActive(true);
                SpineManager.instance.DoAnimation(npc, "animation", true);
            });
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
                    //if (voiceNum == 1)
                    //{
                    //    SoundManager.instance.ShowVoiceBtn(true);
                    //    SoundManager.instance.SetVoiceBtnEvent(SkipBtnEvent);
                    //}
                    if (voiceNum > 1 && voiceCount < voiceNum - 1)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                        SoundManager.instance.SetVoiceBtnEvent(SkipBtnEvent);
                    }

                    voiceCount++;
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

        private void OnDisable()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(null);
            SoundManager.instance.skipBtn.SetActive(false);
            SoundManager.instance.sheildGo.SetActive(false);
        }
    }
}

