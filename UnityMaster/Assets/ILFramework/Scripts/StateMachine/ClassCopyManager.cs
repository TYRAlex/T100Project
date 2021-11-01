using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace ILFramework
{
    public class ClassCopyManager : Manager<ClassCopyManager>
    {
        object obj;
        public MethodInfo[] method;
        public TaskQueue taskQueue;

        public void run(List<string> names, Type t,object _obj)
        {
            method = new MethodInfo[names.Count];
            //创建t类的实例 "obj"
            obj = _obj;
            for (int i = 0; i < names.Count; i++)
            {
                //通过string类型的strMethod获得同名的方法“method”
                method[i] = t.GetMethod(names[i]);
            }
            taskQueue.AddTaskList(method);
            taskQueue.Start();
        }
        public void OnInvoke(int index)
        {
            //t类实例obj,调用方法"method"
            method[index].Invoke(obj, null);
        }
    }
}
