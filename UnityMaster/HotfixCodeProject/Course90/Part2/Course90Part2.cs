using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course90Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course90Part2");
            if(init){lua.DoFile("Course90Part2/C90Game2.lua");

            LuaFunction func = lua.GetFunction("C90Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C90Game2.Show");
            func1.Call(true, true);
        }
    }
}
