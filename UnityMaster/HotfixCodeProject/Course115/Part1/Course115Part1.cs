using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course115Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course115Part1");
            if(init){lua.DoFile("Course115Part1/C115Game1.lua");

            LuaFunction func = lua.GetFunction("C115Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C115Game1.Show");
            func1.Call(true, true);
        }
    }
}
