using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseInsectsPart2
    {
        GameObject curGo;
        GameObject dingding;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            dingding = curTrans.Find("dingding").gameObject;
            PowerpointManager.instance.CreatePower(curGo,new string[] { "tamp_one", "tamp_one (1)", "tamp_one (2)" },Speaking);
        }
        public void Speaking()
        {
            int index = PowerpointManager.instance.curIndex;
            if (index < 2)
            {
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, PowerpointManager.instance.curIndex,
               () => PowerpointManager.instance.MaskHide(),
               () => PowerpointManager.instance.MaskShow());
            }
            
        }
    }
}
