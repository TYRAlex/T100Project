using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseMountainPart2
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            GameObject TT = curGo.transform.Find("npc").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.Speaking(TT, "talk", SoundManager.SoundType.VOICE, 0, null, ()=>
            {
                ScrollRectMoveManager.instance.CreateManager(curTrans.transform.Find("ScrollRect_Parent"), SpeakAction);
            });
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM);
        }

        void SpeakAction()
        {
            GameObject TT =  curGo.transform.Find("npc").gameObject;
            SoundManager.instance.Speaking(TT, "talk", SoundManager.SoundType.VOICE, ScrollRectMoveManager.instance.index + 1, null, null);
        }
    }
}
