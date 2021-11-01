using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course136Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course136Part3");
            if(init){lua.DoFile("Course136Part3/C136Game3.lua");

            LuaFunction func = lua.GetFunction("C136Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C136Game3.Show");
            func1.Call(true, true);
        }
    }
}
