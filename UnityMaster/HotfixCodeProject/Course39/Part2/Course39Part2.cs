using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course39Part2
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course39Part2");

            if (init)
            {
                lua.DoFile("Course39Part2/C39Game2.lua");
                LuaFunction func = lua.GetFunction("C39Game2.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C39Game2.Show");
            func1.Call(true, true);
        }
    }
}
