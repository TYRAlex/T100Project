using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course74Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course74Part1");
            if(init){lua.DoFile("Course74Part1/C74Game1.lua");

            LuaFunction func = lua.GetFunction("C74Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C74Game1.Show");
            func1.Call(true, true);
        }
    }
}
