using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course123Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course123Part2");
            if(init){lua.DoFile("Course123Part2/C123Game2.lua");

            LuaFunction func = lua.GetFunction("C123Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C123Game2.Show");
            func1.Call(true, true);
        }
    }
}
