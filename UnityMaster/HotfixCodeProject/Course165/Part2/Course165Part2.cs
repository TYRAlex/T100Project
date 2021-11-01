using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class Course165Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course165Part2");
            if(init){lua.DoFile("Course165Part2/C165Game2.lua");

            LuaFunction func = lua.GetFunction("C165Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C165Game2.Show");
            func1.Call(true, true);
        }
    }
}
