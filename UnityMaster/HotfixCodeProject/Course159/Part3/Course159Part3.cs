using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course159Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course159Part3");
            if(init){lua.DoFile("Course159Part3/C159Game3.lua");

            LuaFunction func = lua.GetFunction("C159Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C159Game3.Show");
            func1.Call(true, true);
        }
    }
}
