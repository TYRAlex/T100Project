using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course101Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course101Part3");
            if(init){lua.DoFile("Course101Part3/C101Game3.lua");

            LuaFunction func = lua.GetFunction("C101Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C101Game3.Show");
            func1.Call(true, true);
        }
    }
}
