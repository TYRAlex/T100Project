using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course96Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course96Part2");
            if(init){lua.DoFile("Course96Part2/C96Game2.lua");

            LuaFunction func = lua.GetFunction("C96Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C96Game2.Show");
            func1.Call(true, true);
        }
    }
}
