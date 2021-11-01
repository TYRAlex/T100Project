using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course65Part3
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course65Part3");

            if (init)
            {
                lua.DoFile("Course65Part3/C65Game3.lua");
                LuaFunction func = lua.GetFunction("C65Game3.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C65Game3.Show");
            func1.Call(true, true);
        }
    }
}
