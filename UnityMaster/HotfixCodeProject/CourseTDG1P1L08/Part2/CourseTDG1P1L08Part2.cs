using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG1P1L08Part2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            if(curGo.transform.Find("ScrollRect_Parent").gameObject.GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("ScrollRect_Parent").gameObject.GetComponent<ScrollRectMoveManager>());
            }
            curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curGo.transform.Find("ScrollRect_Parent"));
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM, 0.2f);
        }
    }
}
