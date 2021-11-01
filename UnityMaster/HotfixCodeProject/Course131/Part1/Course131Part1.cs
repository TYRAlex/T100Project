using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course131Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course131Part1");
            if(init){lua.DoFile("Course131Part1/C131Game1.lua");

            LuaFunction func = lua.GetFunction("C131Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C131Game1.Show");
            func1.Call(true, true);
        }
    }
}
