using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course85Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course85Part1");
            if(init){lua.DoFile("Course85Part1/C85Game1.lua");

            LuaFunction func = lua.GetFunction("C85Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C85Game1.Show");
            func1.Call(true, true);
        }
    }
}
