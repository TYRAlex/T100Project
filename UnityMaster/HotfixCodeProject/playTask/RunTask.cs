using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClaseState
{
    public class RunTask: TaskBase
    {
        int count = 0;
        public override void OnInit()
        {
            FuncName = new string[] { "A", "B", "C", "D" };
            curClassName = "ClaseState.RunTask";
            MesManager.instance.Dispatch("playTask", (int)PlayTaskCmd.open, "playTask", "开始工作", 3);
        }
        public void A()
        {
            Debug.Log("A--1");
            mono.StartCoroutine(wait(3, "A--2"));
        }
        IEnumerator wait(int time, string name, bool Isback = false)
        {
            for (int i = 0; i < time; i++)
            {
                yield return new WaitForSeconds(1.0f);
                Debug.Log(i + 1);
            }
            Debug.Log(name);
            ClassCopyManager.instance.taskQueue.End();
            if (Isback)
            {
                Debug.Log("返回返回返回");
                ClassCopyManager.instance.taskQueue.SkipBackId(0);
            }
        }
        public void B()
        {
            Debug.Log("B----1");
            mono.StartCoroutine(wait(1, "B---2"));

        }
        public void C()
        {
            Debug.Log("C----1");
            mono.StartCoroutine(wait(2, "C---2"));
        }
        public void D()
        {
            count++;
            Debug.Log("D----1");
            if(count < 2)
            {
                mono.StartCoroutine(wait(1, "D---2", true));
            }
            else
            {
                MesManager.instance.Dispatch("playTask1", (int)PlayTaskCmd.close, "playTask1", "结束工作",3);
            }
        }
    }
}
