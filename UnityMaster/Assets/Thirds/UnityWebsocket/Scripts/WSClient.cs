using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using System;
using bell.ai.course2.unity.websocket;

namespace ILFramework
{
    public class WSClient : Manager<WSClient>
    {
        List<WebSocket> wclients = new List<WebSocket>();
        Action<object> open, message, close, error;

        public WebSocket InitWebSocket(Action<object> messageHandler, Action<object> openHandler = null, Action<object> errorHandler = null, Action<object> closeHandler = null)
        {
            open = openHandler;
            message = messageHandler;
            close = errorHandler;
            error = closeHandler;

            WebSocket client = new WebSocket(Constants.cloudws);
            client.OnOpen += OnOpenHandler;
            client.OnMessage += OnMessageHandler;
            client.OnError += OnErrorHandler;
            client.OnClose += OnCloseHandler;
            client.Connect();

            wclients.Add(client);
            return client;
        }

        void OnOpenHandler(object o, EventArgs e)
        {
            Debug.Log(" ws OnOpen ");
            open?.Invoke(e);
        }

        void OnMessageHandler(object o, MessageEventArgs e)
        {
            Debug.LogFormat(" ws OnMessage data: {0} ", e.Data);
            message?.Invoke(e);
        }

        void OnCloseHandler(object o, CloseEventArgs e)
        {
            Debug.LogFormat(" ws OnClose : {0} ", e.Reason);
            close?.Invoke(e);
        }

        void OnErrorHandler(object o, ErrorEventArgs e)
        {
            Debug.LogFormat(" ws OnError : {0} ", e.Message);
            error?.Invoke(e);
        }

        public void Close(WebSocket client)
        {
            client.Close();
        }

        private void OnDestroy()
        {
            for (int i = 0; i < wclients.Count; i++)
            {
                if (wclients[i].IsAlive)
                    wclients[i].Close();
            }
        }
    }
}
