using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ClaseState
{
    public class TargetTask : TaskBase
    {
        public  string name;
        public  string curname;
        public  float time;
        public  GameObject target;
        public  bool isDraw;

        public TargetTask(string name):base()
        {
            this.name = name + "_1";
            curname = name;
            isDraw = true;
            target = curGo.transform.Find("target").gameObject;
        }
        public override void OnInit()
        {
            ClassCopyManager.instance.taskQueue.OnStart = OnStart;
        }
        IEnumerator wait()
        {
            yield return new WaitForSeconds(time);
            ClassCopyManager.instance.taskQueue.End(); ;
        }
        public void OnStart()
        {
            int num = UnityEngine.Random.Range(9, 12);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, num, false);
            SpineManager.instance.DoAnimation(curGo.transform.Find("UI3D/tiantian").gameObject, "talk2", true);
            curGo.GetComponent<MonoBehaviour>().StartCoroutine(waitSpeak(time));
        }

        IEnumerator waitSpeak(float time)
        {
            yield return new WaitForSeconds(time);
            if (!CourseHieroglyphsPart1.isNext)
            {
                SpineManager.instance.DoAnimation(curGo.transform.Find("UI3D/tiantian").gameObject, "breath", true);
            }
        }
        IEnumerator wait1()
        {
            float count = time / 0.02f;
            int index = (int)(count);
            for (int i = 0; i < index; i++)
            {
                if (CourseHieroglyphsPart1.mask.activeInHierarchy == false)
                {
                    CourseHieroglyphsPart1.mask.SetActive(true);
                }
                yield return new WaitForSecondsRealtime(0.02f);
            }
        }
        public void Draw()
        {
            CourseHieroglyphsPart1.mask.SetActive(true);
            time = SpineManager.instance.DoAnimation(target,name, false);
            
            mono.StartCoroutine(wait());
            mono.StartCoroutine(wait1());
        }
        public void LookAt()
        {
            SpineManager.instance.PlayAnimationState(target.GetComponent<SkeletonGraphic>(),name, time + "|" + time);
            time = 1.0f;
            mono.StartCoroutine(wait());
        }
        public void Over()
        {
            CourseHieroglyphsPart1.mask.SetActive(false);
            SpineManager.instance.PlayAnimationState(target.GetComponent<SkeletonGraphic>(), curname, "0|0");
            ClassCopyManager.instance.taskQueue.End();
            MesManager.instance.Dispatch("CourseHieroglyphs", (int)Target.HuanTu);
        }
    }
}
