using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course76Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course76Part2");
            if(init){lua.DoFile("Course76Part2/C76Game2.lua");

            LuaFunction func = lua.GetFunction("C76Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C76Game2.Show");
            func1.Call(true, true);
        }
    }
}
