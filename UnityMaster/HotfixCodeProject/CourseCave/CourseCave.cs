using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseCave
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
        }
    }
}
