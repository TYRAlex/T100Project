using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseRobotPart2
    {
        GameObject curGo;
        GameObject tiantian;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            tiantian = curGo.transform.Find("tamp_Parent/tiantian").gameObject;
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_one (1)", "tamp_one (2)", "tamp_one (3)" }, Speaking);
            SoundManager.instance.BgSoundPart1();
        }
        public void Speaking()
        {
            if (PowerpointManager.instance.curIndex >= 3)
            {
                return;
            }


            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, PowerpointManager.instance.curIndex,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = false,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = true
            );
        }
    }
}
