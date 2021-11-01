using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseOwlPart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curTrans.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("ScrollRect_Parent"));
            SoundManager.instance.BgSoundPart1();
        }
    }
}
