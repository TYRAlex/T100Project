using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course75Part4
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course75Part4");
            if(init){lua.DoFile("Course75Part4/C75Game4.lua");

            LuaFunction func = lua.GetFunction("C75Game4.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C75Game4.Show");
            func1.Call(true, true);
        }
    }
}
