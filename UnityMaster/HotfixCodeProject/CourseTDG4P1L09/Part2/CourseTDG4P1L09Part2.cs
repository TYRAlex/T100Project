using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L09Part2
    {
        GameObject curGo, npc;
        Transform content;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            npc = curTrans.Find("npc").gameObject;
            content = curTrans.Find("ScrollRect_Parent/Viewport/Content");
            content.localPosition = new Vector3(1920f, 0, 0);

            if (curTrans.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() == null)
            {
                curTrans.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            }
            
            ScrollRectMoveManager.instance.index = 0;
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("ScrollRect_Parent"), Speak);           
        }

        void Speak()
        {
            int index = ScrollRectMoveManager.instance.index;
            if (index < 2)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, index);
            }
        }
    }
}
