using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course103Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course103Part2");
            if(init){lua.DoFile("Course103Part2/C103Game2.lua");

            LuaFunction func = lua.GetFunction("C103Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C103Game2.Show");
            func1.Call(true, true);
        }
    }
}
