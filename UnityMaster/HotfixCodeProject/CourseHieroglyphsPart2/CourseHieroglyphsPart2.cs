using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseHieroglyphsPart2
    {
        GameObject curGo;
        int count;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<PowerpointManager>();
            count = 0;
            PowerpointManager.instance.CreatePower(curGo, new string[] { "tamp_one", "tamp_one1", "tamp_one2" },Speaking);
        }
        void Speaking()
        {
            SoundManager.instance.Speaking(curGo.transform.Find("tiantian").gameObject, "talk2", SoundManager.SoundType.SOUND,count++,
                PowerpointManager.instance.MaskHide, PowerpointManager.instance.MaskShow);
        }
    }
}
