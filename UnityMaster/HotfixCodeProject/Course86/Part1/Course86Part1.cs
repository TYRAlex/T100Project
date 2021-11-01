using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course86Part1
    {
        GameObject curGo;
        LuaState lua; bool init; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = init = LuaManager.instance.AddLuaCourseBundle("Course86Part1");
            if(init){if(init){lua.DoFile("Course86Part1/C86Game1.lua");

            LuaFunction func = lua.GetFunction("C86Game1.InitPanel");
            func.Call(curGo);}}

            LuaFunction func1 = lua.GetFunction("C86Game1.Show");
            func1.Call(true, true);
        }
    }
}
