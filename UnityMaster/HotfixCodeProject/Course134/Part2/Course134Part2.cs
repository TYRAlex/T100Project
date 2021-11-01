using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course134Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course134Part2");
            if(init){lua.DoFile("Course134Part2/C134Game2.lua");

            LuaFunction func = lua.GetFunction("C134Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C134Game2.Show");
            func1.Call(true, true);
        }
    }
}
