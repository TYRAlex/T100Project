using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course66Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course66Part2");

            if (init)
            {
                lua.DoFile("Course66Part2/C66Game2.lua");
                LuaFunction func = lua.GetFunction("C66Game2.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C66Game2.Show");
            func1.Call(true, true);
        }
    }
}
