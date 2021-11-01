using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDKP2L14Part2
    {
        GameObject curGo;
        GameObject Npc;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Npc = curTrans.Find("Npc").gameObject;
            if (curTrans.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() == null)
                curTrans.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("ScrollRect_Parent"),Speaking);
        }

        private void Speaking()
        {
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, ScrollRectMoveManager.instance.index);
        }
    }
}
