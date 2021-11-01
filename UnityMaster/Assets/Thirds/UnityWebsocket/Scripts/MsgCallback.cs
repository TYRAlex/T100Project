using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace bell.ai.course2.unity.websocket
{
    public enum MsgCode
    {
        WS_ERROR = 0,
        WS_CONNECT,
        WS_CLOSE
    }

    public class MsgCallback
    {
        public Action<string> reqCallback;
        public Action<string> resCallback;
        public Action<MsgCode> statusCallback;

        public MsgCallback(Action<string> reqCallback, Action<string> resCallback, Action<MsgCode> statusCallback)
        {
            this.reqCallback = reqCallback;
            this.resCallback = resCallback;
            this.statusCallback = statusCallback;
        }
    }

}