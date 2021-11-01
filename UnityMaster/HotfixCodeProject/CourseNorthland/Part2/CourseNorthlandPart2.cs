using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseNorthlandPart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            curGo.GetComponent<PowerpointManager>().CreatePower(curGo, new string[] { "tamp_one", "tamp_one (1)","tamp_one (3)","tamp_one (2)" });
        }
    }
}
