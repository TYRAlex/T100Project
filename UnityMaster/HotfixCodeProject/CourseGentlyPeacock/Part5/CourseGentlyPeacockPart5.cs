using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseGentlyPeacockPart5
    {
        GameObject curGo;
        GameObject TT;
        GameObject DD;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            DD = curTrans.Find("DD").gameObject;
            TT = curTrans.Find("TT").gameObject;

            SpineManager.instance.DoAnimation(DD,"breath");
            SpineManager.instance.DoAnimation(TT, "breath");

            
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 0, ()=>{
                    SpineManager.instance.DoAnimation(DD,"talk");
            }, () =>
            {
                DD.SetActive(true);
                SpineManager.instance.DoAnimation(DD,"breath");
                SoundManager.instance.Speaking(TT, "talk", SoundManager.SoundType.VOICE, 1,null,()=> {
                    TT.SetActive(true);
                    SpineManager.instance.DoAnimation(TT, "breath");
                });
            });

            //curGo.AddComponent<ContactPartManager>();
            //ContactPartManager.instance.PlayAnimationVoice(curGo, 0.3f,2);
        }
    }
}
