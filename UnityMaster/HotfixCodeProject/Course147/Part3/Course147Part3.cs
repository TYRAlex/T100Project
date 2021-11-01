using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course147Part3
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course147Part3");
            if(init){lua.DoFile("Course147Part3/C147Game3.lua");

            LuaFunction func = lua.GetFunction("C147Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C147Game3.Show");
            func1.Call(true, true);
        }
    }
}
