using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILFramework;
using ILFramework.HotClass;
using UnityEngine;

namespace ClaseState
{
    public class SpeakTask:TaskBase
    {
        public  GameObject tiantian;
        public  float time;
        public  float curtime;
        public  bool noSpeak;
        
        public SpeakTask(float time, string str):base()
        {
            this.time = time;
            curtime = time;
            tiantian = curGo.transform.Find("UI3D/tiantian").gameObject;
            FuncName = str.Split(',').ToList();
            noSpeak = true;
        }
        public override void OnInit()
        {
        }
        public void Speak()
        {
            time = curtime;
            SpineManager.instance.DoAnimation(tiantian, "talk2", true);
            CourseHieroglyphsPart1.mask.SetActive(true);
            mono.StartCoroutine(wait());
        }
        
        IEnumerator wait()
        {
            yield return new WaitForSeconds(time);
            if(!CourseHieroglyphsPart1.isOver)
            {
                CourseHieroglyphsPart1.mask.SetActive(false);
            }
            ClassCopyManager.instance.taskQueue.End();
            CourseHieroglyphsPart1.isNext = false;
        }
        public void Breath()
        {
            SpineManager.instance.DoAnimation(tiantian, "breath", true);
            ClassCopyManager.instance.taskQueue.End();
            if (CourseHieroglyphsPart1.isFirst)
            {
                MesManager.instance.Dispatch("CourseHieroglyphs", (int)Target.StartSpeak);
                CourseHieroglyphsPart1.isFirst = false;
            }
        }
        public void Jump()
        {
            time =  SpineManager.instance.DoAnimation(tiantian, "fun_jump", false);
            mono.StartCoroutine(wait());
        }
    }
}
