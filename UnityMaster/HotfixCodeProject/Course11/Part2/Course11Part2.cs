using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course11Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course11Part2");
            if(init){lua.DoFile("Course11Part2/C11Game2.lua");

            LuaFunction func = lua.GetFunction("C11Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C11Game2.Show");
            func1.Call(true, true);
        }
    }
}
