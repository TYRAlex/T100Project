using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course142Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course142Part2");
            if(init){lua.DoFile("Course142Part2/C142Game2.lua");

            LuaFunction func = lua.GetFunction("C142Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C142Game2.Show");
            func1.Call(true, true);
        }
    }
}
