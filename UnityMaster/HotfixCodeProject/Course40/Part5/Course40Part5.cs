using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course40Part5
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course40Part5");

            if (init)
            {
                lua.DoFile("Course40Part5/C40Game5.lua");
                LuaFunction func = lua.GetFunction("C40Game5.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("C40Game5.Show");
            func1.Call(true, true);
        }
    }
}
