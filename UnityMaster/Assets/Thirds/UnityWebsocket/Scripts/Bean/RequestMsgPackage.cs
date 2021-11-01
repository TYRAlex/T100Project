using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bell.ai.course2.unity.websocket
{
    public class MsgPackage
    {
   
        public string messageId;
        public Type reqDataType;
        public Type resDataType;

        public WsRequest<object> wsRequest;
        public WsResponse<object> wsResponse;
        public DateTime requestTime;
        
    }
}