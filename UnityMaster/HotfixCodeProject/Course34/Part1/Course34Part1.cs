using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course34Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course34Part1");

            if (init)
            {
                lua.DoFile("Course34Part1/C34Game1.lua");
                LuaFunction func = lua.GetFunction("C34Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C34Game1.Show");
            func1.Call(true, true);
        }
    }
}
