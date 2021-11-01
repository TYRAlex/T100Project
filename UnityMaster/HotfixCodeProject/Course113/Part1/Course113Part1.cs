using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course113Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course113Part1");
            if(init){lua.DoFile("Course113Part1/C113Game1.lua");

            LuaFunction func = lua.GetFunction("C113Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C113Game1.Show");
            func1.Call(true, true);
        }
    }
}
