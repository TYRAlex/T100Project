using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course181Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course181Part1");
            if(init){lua.DoFile("Course181Part1/C181Game1.lua");

            LuaFunction func = lua.GetFunction("C181Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C181Game1.Show");
            func1.Call(true, true);
        }
    }
}
