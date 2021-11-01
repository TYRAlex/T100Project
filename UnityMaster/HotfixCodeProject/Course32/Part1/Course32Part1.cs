using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course32Part1
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course32Part1");

            if (init)
            {
                lua.DoFile("Course32Part1/C32Game1.lua");
                LuaFunction func = lua.GetFunction("C32Game1.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C32Game1.Show");
            func1.Call(true, true);
        }
    }
}
