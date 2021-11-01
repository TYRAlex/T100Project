using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course139Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course139Part2");
            if(init){lua.DoFile("Course139Part2/C139Game2.lua");

            LuaFunction func = lua.GetFunction("C139Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C139Game2.Show");
            func1.Call(true, true);
        }
    }
}
