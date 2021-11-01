using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseColorfulWorldPart
{
    class TargetTrigger
    {
        List<ILObject3DAction> triggerList;
        Transform parent;
        bool isStart;
        public TargetTrigger()
        {
            parent = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/zhuanpan01/SkeletonUtility-Root/root/bone/zp_0");
            triggerList = new List<ILObject3DAction>();
            for(int i = 0;i < parent.childCount;i++)
            {
                triggerList.Add(parent.GetChild(i).GetComponent<ILObject3DAction>());
                triggerList[i].OnTriggerEnter2DLua = OnTriggerEnter2D;
            }
            isStart = false;
        }

        private void OnTriggerEnter2D(Collider2D collider, int index)
        {
            if(index == 20 && isStart)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            }
            isStart = true;
        }
    }
}
