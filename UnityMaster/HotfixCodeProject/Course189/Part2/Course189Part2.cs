using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course189Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course189Part2");
            if(init){lua.DoFile("Course189Part2/C189Game2.lua");

            LuaFunction func = lua.GetFunction("C189Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C189Game2.Show");
            func1.Call(true, true);
        }
    }
}
