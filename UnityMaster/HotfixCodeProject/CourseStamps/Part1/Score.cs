using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseStampsPart
{
    class Score
    {
        Transform parent;
        List<SkeletonGraphic> sketons;
        int count;
        public Score()
        {
            this.parent = CourseStampsPart1.curGo.transform.Find("progress");
            count = -1;
            sketons = new List<SkeletonGraphic>(4);
            for(int i = 0;i < 4;i++)
            {
                sketons.Add(parent.GetChild(i).GetComponent<SkeletonGraphic>());
            }
            parent.localScale = Vector3.zero;
            for(int i=0;i<sketons.Count;i++)
            {
                sketons[i].transform.localScale = Vector3.zero;
                if (i != 3)
                {
                    SpineManager.instance.PlayAnimationState(sketons[i], "star");
                }
                else
                {
                    SpineManager.instance.PlayAnimationState(sketons[i], "get");
                }
            }
        }
        public bool AddScore()
        {
            count++;
            
            parent.localScale = Vector3.one;
            string name = count != 3?"star":"get";
            Debug.Log("count:---------" + count+name);
            sketons[count].transform.localScale = new Vector3(0.5f, 0.5f, 1);
            SpineManager.instance.DoAnimation(sketons[count].gameObject, name, false, () =>
               {
                   SpineManager.instance.PlayAnimationState(sketons[count], name + "_1");
                   SpineManager.instance.DoAnimation(sketons[count].gameObject, name + "_1");
               });
           
            if(count == 3)
            {
                return true;
            }
            return false;
        }
        public void HideScore()
        {

        }
    }
}
