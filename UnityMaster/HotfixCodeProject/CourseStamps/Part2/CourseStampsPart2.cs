using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseStampsPart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_two", "tamp_two (1)" });
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
        }
    }
}
