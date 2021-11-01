using System;
using UnityEngine;

namespace Part1
{
    public class CommonObj
    {

        //可挖掘物体的名字
        public string name;
        //已经挖掘的次数
        public int curCount;
        public GameObject obj;
        public CommonObj(GameObject obj,string name, int count = 0)
        {
            this.obj = obj;
            this.name = name;
            this.curCount = count;
        }
    }
}