using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course71Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course71Part3");
            if(init){lua.DoFile("Course71Part3/C71Game3.lua");

            LuaFunction func = lua.GetFunction("C71Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C71Game3.Show");
            func1.Call(true, true);
        }
    }
}
