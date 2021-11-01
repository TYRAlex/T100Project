using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseButterflyPart
{
    class LightClass : TaskBase
    {
        GameObject target;
        string name;
        public LightClass(GameObject go,string name):base()
        {
            target = go;
            this.name = name;
        }
        public override void OnInit()
        {
            ClassCopyManager.instance.taskQueue.OnFinish = OnFinish;
        }
        public void RunLight()
        {
            Debug.Log("--------RunLight------------"+target.name);
            SpineManager.instance.DoAnimation(target, name, false,delegate()
            {
                ClassCopyManager.instance.taskQueue.End();
            });
        }
        public void OnFinish()
        {
            Debug.Log("--------OnFinish------------");
            PowerpointManager.instance.MaskShow();
        }
    }
}
