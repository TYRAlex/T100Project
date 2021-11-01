using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using bell.ai.course2.unity.websocket;
using UnityEngine;
using Newtonsoft.Json;

namespace ILFramework
{
    public class NetworkManager : Manager<NetworkManager>
    {
        public Communicate cm;
        public event EventHandler<string> gameRspEvent;
        public Action gameStopReqEvent;

        public void WSResponse(string resp)
        {
            gameRspEvent?.Invoke(this, resp);
        }
        
        public void WSRequest()
        {
            gameStopReqEvent?.Invoke();
        }

    }
}
