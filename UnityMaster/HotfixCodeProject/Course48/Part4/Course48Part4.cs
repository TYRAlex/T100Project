using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course48Part4
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course48Part4");

            if (init)
            {
                lua.DoFile("Course48Part4/C48Game4.lua");
                LuaFunction func = lua.GetFunction("C48Game4.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C48Game4.Show");
            func1.Call(true, true);
        }
    }
}
