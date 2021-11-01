using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course154Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course154Part2");
            if(init){lua.DoFile("Course154Part2/C154Game2.lua");

            LuaFunction func = lua.GetFunction("C154Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C154Game2.Show");
            func1.Call(true, true);
        }
    }
}
