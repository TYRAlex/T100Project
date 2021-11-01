using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course75Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course75Part3");
            if(init){lua.DoFile("Course75Part3/C75Game3.lua");

            LuaFunction func = lua.GetFunction("C75Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C75Game3.Show");
            func1.Call(true, true);
        }
    }
}
