using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseArtPart2
    {
        GameObject curGo;
        GameObject tiantian;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            tiantian = curTrans.Find("tiantian").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.BgSoundPart1();
            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_one1", "tamp_one2", "tamp_one3", "tamp_one4" }, speaking);
        }
        void speaking()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            int curIndex = PowerpointManager.instance.curIndex;
            if (curIndex < 3)
            { 
                GameObject mask = PowerpointManager.instance.drag.gameObject;
                SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, curIndex, delegate () { mask.GetComponent<BoxCollider>().enabled = false; }
               , delegate () { mask.GetComponent<BoxCollider>().enabled = true; });
            }
        }
    }
}
