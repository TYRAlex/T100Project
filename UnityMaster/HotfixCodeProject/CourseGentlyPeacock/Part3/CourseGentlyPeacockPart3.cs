using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseGentlyPeacockPart3
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            PowerpointManager.instance.CreatePower(curGo, new string[]{ "temp", "temp (1)", "temp (2)", "temp (3)"});
        }
    }
}
