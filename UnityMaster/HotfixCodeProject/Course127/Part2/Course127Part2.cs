using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course127Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course127Part2");
            if(init){lua.DoFile("Course127Part2/C127Game2.lua");

            LuaFunction func = lua.GetFunction("C127Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C127Game2.Show");
            func1.Call(true, true);
        }
    }
}
