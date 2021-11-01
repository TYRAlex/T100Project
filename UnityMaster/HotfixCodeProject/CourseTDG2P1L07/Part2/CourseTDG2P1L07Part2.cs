using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG2P1L07Part2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            if (curGo.transform.Find("ScrollRect_Parent").gameObject.GetComponent<ScrollRectMoveManager>() == null)
            {
                curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
                ScrollRectMoveManager.instance.CreateManager(curTrans.transform.Find("ScrollRect_Parent"));
            }
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
        }
    }
}
