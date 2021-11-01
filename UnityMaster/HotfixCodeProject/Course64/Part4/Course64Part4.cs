using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course64Part4
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course64Part4");

            if (init)
            {
                lua.DoFile("Course64Part4/C64Game4.lua");
                LuaFunction func = lua.GetFunction("C64Game4.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C64Game4.Show");
            func1.Call(true, true);
        }
    }
}
