using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course94Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course94Part2");
            if(init){lua.DoFile("Course94Part2/C94Game2.lua");

            LuaFunction func = lua.GetFunction("C94Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C94Game2.Show");
            func1.Call(true, true);
        }
    }
}
