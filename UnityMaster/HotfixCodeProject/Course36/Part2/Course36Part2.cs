using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course36Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course36Part2");

            if (init)
            {
                lua.DoFile("Course36Part2/C36Game2.lua");
                LuaFunction func = lua.GetFunction("C36Game2.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C36Game2.Show");
            func1.Call(true, true);
        }
    }
}
