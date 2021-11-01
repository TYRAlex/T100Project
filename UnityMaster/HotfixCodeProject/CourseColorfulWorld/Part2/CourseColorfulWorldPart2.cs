using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseColorfulWorldPart2
    {
        GameObject curGo;
        GameObject dingding;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            dingding = curGo.transform.Find("dingding").gameObject;
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_one (1)", "tamp_one (2)" }, Speak);
            SoundManager.instance.BgSoundPart1();
        }
        public void Speak()
        {
            int index = PowerpointManager.instance.curIndex;
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = false,
                () => PowerpointManager.instance.drag.GetComponent<BoxCollider>().enabled = true);
        }
    }
}
