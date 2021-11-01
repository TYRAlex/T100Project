using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course81Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course81Part3");
            if(init){lua.DoFile("Course81Part3/C81Game3.lua");

            LuaFunction func = lua.GetFunction("C81Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C81Game3.Show");
            func1.Call(true, true);
        }
    }
}
