using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bell.ai.course2.unity.websocket
{

    [Serializable]
    public class StringMsgData
    {
        public string msg;
        public StringMsgData(string data)
        {
            msg = data;
        }
    }

    [Serializable]
    public class PreLoadMsgData
    {
        public string[] nameList;

        public string path;
        public PreLoadMsgData(string[] data) { nameList = data; }
    }
}