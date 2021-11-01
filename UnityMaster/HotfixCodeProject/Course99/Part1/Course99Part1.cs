using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course99Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course99Part1");
            if(init){lua.DoFile("Course99Part1/C99Game1.lua");

            LuaFunction func = lua.GetFunction("C99Game1.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C99Game1.Show");
            func1.Call(true, true);
        }
    }
}
