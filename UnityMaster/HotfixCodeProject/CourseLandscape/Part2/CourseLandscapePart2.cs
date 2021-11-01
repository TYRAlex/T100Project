using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseLandscapePart2
    {
        GameObject curGo;
        GameObject dingding;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curTrans.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            dingding = curTrans.Find("dingding").gameObject;
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("ScrollRect_Parent"), Speaking);
        }
        public void Speaking()
        {
            if (ScrollRectMoveManager.instance.index == 0 || ScrollRectMoveManager.instance.index == 1 || ScrollRectMoveManager.instance.index == 2)
            {
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, ScrollRectMoveManager.instance.index);
            }
        }
    }
}
