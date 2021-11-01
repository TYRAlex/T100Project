using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace bell.ai.course2.unity.websocket
{

    /// <summary>
    /// Ws请求包
    /// </summary>
    /// <typeparam name="T">为Data的泛型</typeparam>
    
    [Serializable]
    public class WsRequest<T>
    {
        public string messageId;
        public string deviceId = "U3D_";
        public string path;
        public string mode = Constants.request;
        public T data;
        public string signature;
    }
}