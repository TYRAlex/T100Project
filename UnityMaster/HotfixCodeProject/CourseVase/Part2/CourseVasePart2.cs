using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseVasePart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            SoundManager.instance.ShowVoiceBtn(false);
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_two", "tamp_some", "tamp_some1"}, SwitchPage);
            SoundManager.instance.BgSoundPart1();
        }

        void SwitchPage()
        {
            GameObject TT = curGo.transform.Find("npc").gameObject;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            if (PowerpointManager.instance.curIndex < 3)
            {
                SoundManager.instance.Speaking(TT, "talk", SoundManager.SoundType.VOICE, PowerpointManager.instance.curIndex, null, null);
            }
            else
            {
                TT.SetActive(false);
            }

        }
    }
}
