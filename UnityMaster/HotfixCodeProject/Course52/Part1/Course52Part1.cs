using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course52Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course52Part1");

            if (init)
            {
                lua.DoFile("Course52Part1/C52Game1.lua");
                LuaFunction func = lua.GetFunction("C52Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C52Game1.Show");
            func1.Call(true, true);
        }
    }
}
