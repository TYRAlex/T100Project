using ILFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ClaseState
{
    public abstract class TaskBase
    {
        public string[] FuncName;
        public string curClassName;
        public MonoBehaviour mono;
        public TaskBase()
        {
            mono = ClassCopyManager.instance.gameObject.GetComponent<MonoBehaviour>();
        }
        public abstract void OnInit();

        public void AddTask_Run()
        {
            this.OnInit();
            Type t = Type.GetType(this.curClassName);
            object obj = System.Activator.CreateInstance(t);
            ClassCopyManager.instance.run(this.FuncName, this.curClassName, t, obj);
        }
    }
}

