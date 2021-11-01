using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course48Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course48Part1");

            if (init)
            {
                lua.DoFile("Course48Part1/C48Game1.lua");
                LuaFunction func = lua.GetFunction("C48Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C48Game1.Show");
            func1.Call(true, true);
        }
    }
}
