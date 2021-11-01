using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course10010Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course10010Part1");

            if (init)
            {
                lua.DoFile("Course10010Part1/C10010Game1.lua");
                LuaFunction func = lua.GetFunction("C10010Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C10010Game1.Show");
            func1.Call(true, true);
        }
    }
}
