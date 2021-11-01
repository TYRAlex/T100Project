using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course144Part1
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course144Part1");
            if(init){lua.DoFile("Course144Part1/C144Game1.lua");

            LuaFunction func = lua.GetFunction("C144Game3.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C144Game3.Show");
            func1.Call(true, true);
        }
    }
}
