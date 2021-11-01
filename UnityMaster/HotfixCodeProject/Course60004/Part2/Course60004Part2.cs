using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course60004Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;

        GameObject tiantian;

        void Start(object o)
        {
            curGo = (GameObject)o;

            // lua = LuaManager.instance.GlobalLua;
            // init = LuaManager.instance.AddLuaCourseBundle("Course60004Part2");

            // if (init)
            // {
            //     lua.DoFile("Course60004Part2/C60004Game2.lua");
            //     LuaFunction func = lua.GetFunction("C60004Game2.InitPanel");
            //     func.Call(curGo);
            // }

            // LuaFunction func1 = lua.GetFunction("C60004Game2.Show");
            // func1.Call(true, true);
            Transform curTrans = curGo.transform;
            tiantian = curTrans.Find("max/maxSpine").gameObject;
            curTrans.Find("pictures").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("pictures"), Speaking);



        }

        public void Speaking()
        {
            if (ScrollRectMoveManager.instance.index == 0 || ScrollRectMoveManager.instance.index == 1 || ScrollRectMoveManager.instance.index == 2 || ScrollRectMoveManager.instance.index == 3)
            {
                SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.VOICE, ScrollRectMoveManager.instance.index);
            }

        }
    }
}
