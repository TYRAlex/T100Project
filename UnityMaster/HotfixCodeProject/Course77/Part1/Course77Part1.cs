using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course77Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course77Part1");
            if(init){lua.DoFile("Course77Part1/C77Game1.lua");

            LuaFunction func = lua.GetFunction("C77Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C77Game1.Show");
            func1.Call(true, true);
        }
    }
}
