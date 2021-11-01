using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course153Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course153Part1");
            if(init){lua.DoFile("Course153Part1/C153Game1.lua");

            LuaFunction func = lua.GetFunction("C153Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C153Game1.Show");
            func1.Call(true, true);
        }
    }
}
