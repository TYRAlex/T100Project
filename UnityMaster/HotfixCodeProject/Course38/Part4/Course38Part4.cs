using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course38Part4
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course38Part4");

            if (init)
            {
                lua.DoFile("Course38Part4/C38Game4.lua");
                LuaFunction func = lua.GetFunction("C38Game4.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C38Game4.Show");
            func1.Call(true, true);
        }
    }
}
