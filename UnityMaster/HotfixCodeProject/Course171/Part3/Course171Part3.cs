using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course171Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course171Part3");
            if(init){lua.DoFile("Course171Part3/C171Game3.lua");

            LuaFunction func = lua.GetFunction("C171Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C171Game3.Show");
            func1.Call(true, true);
        }
    }
}
