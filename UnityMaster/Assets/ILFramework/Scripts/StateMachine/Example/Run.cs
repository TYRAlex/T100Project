using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework
{
    public class Run
    {
        public string[] FuncName = new string[] { "A", "B", "C", "D"};
        public string curClassName = "ILFramework.Run";
        public MonoBehaviour mono;
        public Run()
        {
            mono = ClassCopyManager.instance.gameObject.GetComponent<MonoBehaviour>();
        }
        public void A()
        {
            Debug.Log("A--1");
            mono.StartCoroutine(wait(3, "A--2"));
        }
        IEnumerator wait(int time,string name,bool Isback = false)
        {
            for(int i = 0;i < time;i++)
            {
                yield return new WaitForSeconds(1.0f);
                Debug.Log(i + 1);
            }
            Debug.Log(name);
            ClassCopyManager.instance.taskQueue.End();
            if(Isback)
            {
                Debug.Log("返回返回返回");
                ClassCopyManager.instance.taskQueue.SkipBackId(0);
            }
        }
        public void B()
        {
            Debug.Log("B----1");
            mono.StartCoroutine(wait(1,"B---2"));

        }
        public void C()
        {
            Debug.Log("C----1");
            mono.StartCoroutine(wait(2, "C---2"));
        }
        public void D()
        {
            Debug.Log("D----1");
            mono.StartCoroutine(wait(1 ,"D---2",true));
        }
    }
}
