using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course36Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course36Part1");

            if (init)
            {
                lua.DoFile("Course36Part1/C36Game1.lua");
                LuaFunction func = lua.GetFunction("C36Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C36Game1.Show");
            func1.Call(true, true);
        }
    }
}
