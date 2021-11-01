using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course10001Part4
    {
        GameObject curGo;
        LuaState lua;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            LuaManager.instance.AddLuaCourseBundle("Course10001Part4");
            lua.DoFile("Course10001Part4/C10001Game4.lua");

            LuaFunction func = lua.GetFunction("C10001Game4.InitPanel");
            func.Call(curGo);

            LuaFunction func1 = lua.GetFunction("C10001Game4.Show");
            func1.Call(true, true);
        }
    }
}
