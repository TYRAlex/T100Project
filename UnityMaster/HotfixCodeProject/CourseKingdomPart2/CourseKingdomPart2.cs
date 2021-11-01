using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseKingdomPart2
    {
        GameObject curGo;
        GameObject npc;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            npc = curTrans.Find("npc").gameObject;
            SoundManager.instance.BgSoundPart1();
            PowerpointManager.instance.CreatePower(curGo, new string[] { "first", "second", "third", "fourth", "fifth" });
        }
        public void Speaking()
        {
            int index = PowerpointManager.instance.curIndex;
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.SOUND, PowerpointManager.instance.curIndex,
              () => PowerpointManager.instance.MaskHide(),
              () => PowerpointManager.instance.MaskShow());
        }
    }
}
