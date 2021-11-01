using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG1P1L08Part4
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<ContactPartManager>();
            ContactPartManager.instance.PlayAnimationVoice(curGo, 0.2f, 2);
        }
    }
}
