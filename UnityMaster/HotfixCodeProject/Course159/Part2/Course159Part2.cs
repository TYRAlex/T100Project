using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course159Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course159Part2");
            if(init){lua.DoFile("Course159Part2/C159Game2.lua");

            LuaFunction func = lua.GetFunction("C159Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C159Game2.Show");
            func1.Call(true, true);
        }
    }
}
