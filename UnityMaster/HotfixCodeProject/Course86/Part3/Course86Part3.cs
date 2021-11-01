using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course86Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = init = LuaManager.instance.AddLuaCourseBundle("Course86Part3");
            if(init){lua.DoFile("Course86Part3/C86Game3.lua");

            LuaFunction func = lua.GetFunction("C86Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C86Game3.Show");
            func1.Call(true, true);
        }
    }
}
