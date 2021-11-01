using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course101Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course101Part2");
            if(init){lua.DoFile("Course101Part2/C101Game2.lua");

            LuaFunction func = lua.GetFunction("C101Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C101Game2.Show");
            func1.Call(true, true);
        }
    }
}
