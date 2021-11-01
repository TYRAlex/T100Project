using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course67Part3
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course67Part3");

            if (init)
            {
                lua.DoFile("Course67Part3/C67Game3.lua");
                LuaFunction func = lua.GetFunction("C67Game3.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C67Game3.Show");
            func1.Call(true, true);
        }
    }
}
