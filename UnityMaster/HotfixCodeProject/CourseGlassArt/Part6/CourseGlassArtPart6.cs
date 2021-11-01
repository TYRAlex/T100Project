using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseGlassArtPart6
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            curGo.AddComponent<ContactPartManager>();
            ContactPartManager.instance.PlayVoice(curGo, 0.1f);
        }
    }
}
