using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course39Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course39Part1");

            if (init)
            {
                lua.DoFile("Course39Part1/C39Game1.lua");
                LuaFunction func = lua.GetFunction("C39Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C39Game1.Show");
            func1.Call(true, true);
        }
    }
}
