using ClaseState;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum PlayTaskCmd
    {
        open,
        close
    }
    public class playTask
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            InitStateManager();
        }
        public void InitStateManager()
        {
            curGo.AddComponent<MesManager>();
            MesManager.instance.Register("playTask",(int)PlayTaskCmd.open, open);
            MesManager.instance.Register("playTask1", (int)PlayTaskCmd.close, close);

            curGo.AddComponent<ClassCopyManager>();

            RunTask runTask = new RunTask();
            runTask.AddTask_Run();
        }
        public void close(params object[] param)
        {
            Debug.Log("-----------------¹Ø±Õ-------------");
            for (int i = 0; i < param.Length; i++)
            {
                Debug.Log(param[i]);
            }
        }
        public void open(params object[] param)
        {
            Debug.Log("----------------¿ªÆô-------------");
            for (int i = 0;i<param.Length;i++)
            {
                Debug.Log(param[i]);
            }
        }
       
    }
}
