using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course197Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course197Part2");
            if(init){lua.DoFile("Course197Part2/C197Game2.lua");

            LuaFunction func = lua.GetFunction("C197Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C197Game2.Show");
            func1.Call(true, true);
        }
    }
}
