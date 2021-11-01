using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course6Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course6Part2");
            if(init){lua.DoFile("Course6Part2/C6Game2.lua");

            LuaFunction func = lua.GetFunction("C6Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C6Game2.Show");
            func1.Call(true, true);
        }
    }
}
