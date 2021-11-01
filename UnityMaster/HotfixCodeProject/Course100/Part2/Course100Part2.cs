using System;
using UnityEngine;
using LuaInterface;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course100Part2
    {
        GameObject curGo;
        LuaState lua; bool init;
        void Start(object o)
        {
            curGo = (GameObject)o;

            //Sprite s = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("Course100Part2"), "left");
            //curGo.transform.Find("TestImage").GetComponent<Image>().sprite = s;

            lua = LuaManager.instance.GlobalLua;
            init = LuaManager.instance.AddLuaCourseBundle("Course100Part2");
            if(init){lua.DoFile("Course100Part2/C100Game2.lua");

            LuaFunction func = lua.GetFunction("C100Game2.InitPanel");
            func.Call(curGo);}

            LuaFunction func1 = lua.GetFunction("C100Game2.Show");
            func1.Call(true, true);
        }
    }
}
