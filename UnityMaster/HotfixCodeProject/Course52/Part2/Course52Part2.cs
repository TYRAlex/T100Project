using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course52Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course52Part2");

            if (init)
            {
                lua.DoFile("Course52Part2/C52Game2.lua");
                LuaFunction func = lua.GetFunction("C52Game2.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C52Game2.Show");
            func1.Call(true, true);
        }
    }
}
