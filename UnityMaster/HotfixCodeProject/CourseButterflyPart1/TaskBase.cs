using ILFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace CourseButterflyPart
{
    public abstract class TaskBase
    {
        public  List<string> FuncName;
        public  MonoBehaviour mono;
        public  GameObject curGo;
        public string[] splitStr = new string[] { "OnInit", "OnStart", "OnFinish", "wait", "Wait" ,"_"};
        public TaskBase()
        {
            mono = ClassCopyManager.instance.gameObject.GetComponent<MonoBehaviour>();
            curGo = mono.gameObject;
            ClassCopyManager.instance.taskQueue = new TaskQueue();
            FuncName = new List<string>();
            OnInit();
        }
        public abstract void OnInit();
        public void AddTask_Run(object[] args,TaskBase taskBase)
        {
            Type t = taskBase.GetType();
            if(FuncName.Count == 0)
            {
                MethodInfo[] infos = t.GetMethods();
                //Debug.Log("方法名：");
                foreach (var info in infos)
                {
                    int index = 0;
                    foreach (var sp in splitStr)
                    {
                        index = info.Name.IndexOf(sp);
                        if (index == 0)
                        {
                            break;
                        }
                    }
                    if (index == -1)
                    {
                        FuncName.Add(info.Name);
                        //Debug.Log("Add.." + info.Name);
                    }
                    //Debug.Log(info.Name + ",");
                }
            }
            object obj = System.Activator.CreateInstance(t, args);
            ClassCopyManager.instance.run(FuncName, t, obj);
        }
    }
}
