using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course16Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course16Part1");
            if(init){lua.DoFile("Course16Part1/C16Game1.lua");

            LuaFunction func = lua.GetFunction("C16Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C16Game1.Show");
            func1.Call(true, true);
        }
    }
}
