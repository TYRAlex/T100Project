using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course62Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course62Part2");

            if (init)
            {
                lua.DoFile("Course62Part2/C62Game2.lua");
                LuaFunction func = lua.GetFunction("C62Game2.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C62Game2.Show");
            func1.Call(true, true);
        }
    }
}
