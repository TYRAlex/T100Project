using System;
using UnityEngine;
using LuaInterface;

namespace ILFramework.HotClass
{
    public class MigrateSeed
    {
        GameObject curGo;
        LuaState lua;
        bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("MigrateSeed");

            if (init)
            {
                lua.DoFile("MigrateSeed/originLua.lua");
                LuaFunction func = lua.GetFunction("originLua.InitPanel");
                func.Call(curGo);
            }

            LuaFunction func1 = lua.GetFunction("originLua.Show");
            func1.Call(true, true);
        }
    }
}
