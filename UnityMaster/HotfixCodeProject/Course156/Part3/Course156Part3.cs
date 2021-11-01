using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course156Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course156Part3");
            if(init){lua.DoFile("Course156Part3/C156Game3.lua");

            LuaFunction func = lua.GetFunction("C156Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C156Game3.Show");
            func1.Call(true, true);
        }
    }
}
