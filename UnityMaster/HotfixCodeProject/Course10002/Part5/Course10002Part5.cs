using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course10002Part5
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course10002Part5");

            if (init)
            {
                lua.DoFile("Course10002Part5/C10002Game5.lua");
                LuaFunction func = lua.GetFunction("C10002Game5.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C10002Game5.Show");
            func1.Call(true, true);
        }
    }
}
