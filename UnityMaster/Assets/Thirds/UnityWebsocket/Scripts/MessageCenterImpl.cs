#define PRELOAD
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.IO;
using ILFramework;
using OneID;

namespace bell.ai.course2.unity.websocket
{
    public class MessageCenterImpl : MessageCenter
    {

        public Action<string> playCallBack;
        public Action<string> stopCallBack;
        public Action<string,string[], Action> preLoadCallBack;
        public Action exitCallBack;

        public override void EchoHandler(string request)
        {
            WsRequest<StringMsgData> data = JsonConvert.DeserializeObject<WsRequest<StringMsgData>>(request);
            Debug.Log("-------------++");
            WsResponse<StringMsgData> response = new WsResponse<StringMsgData>();
            response.messageId = data.messageId;
            response.deviceId = data.deviceId;
            response.path = data.path;
            response.mode = "response";
            response.data = new WsResponse<StringMsgData>.ResponeData<StringMsgData>();
            response.data.code = 0;
            response.data.msg = new StringMsgData("connect");
            communicate.sendMessage(JsonConvert.SerializeObject(response));
        }

        public override void EchHandle3Dof(string request)
        {
            WsRequest<StringMsgData> data = JsonConvert.DeserializeObject<WsRequest<StringMsgData>>(request);
            Debug.Log("3Dof:-------------++");
            WsResponse<StringMsgData> response = new WsResponse<StringMsgData>();
            response.messageId = data.messageId;
            response.deviceId = data.deviceId;
            response.path = data.path;
            response.mode = Constants.subscribeData;
            response.data =new WsResponse<StringMsgData>.ResponeData<StringMsgData>();
            communicate.sendMessage(JsonConvert.SerializeObject(response));
        }


        public override void PlayHandler(string request)
        {
            Debug.Log("play:handler");
            WsRequest<StringMsgData> data = JsonConvert.DeserializeObject<WsRequest<StringMsgData>>(request);
            playCallBack?.Invoke(data.data.msg);
        }


        public override void StopHandler(string request)
        {
            Debug.Log("stop:handler");
            WsRequest<StringMsgData> data = JsonConvert.DeserializeObject<WsRequest<StringMsgData>>(request);
            stopCallBack?.Invoke(data.data.msg);
        }

        public override void PreLoadHandler(string request)
        {
            WsRequest<PreLoadMsgData> data = JsonConvert.DeserializeObject<WsRequest<PreLoadMsgData>>(request);
            Debug.Log(" Unity PreLoad ");
            // BellLoom.QueueActions((o) =>
            // {
            //     string path = data.data.path + "/" + data.data.nameList[0] + AppConst.hotfix_platform + "lua_" +
            //                   data.data.nameList[0].ToLower() + "_htp";
            //     if (File.Exists(path))
            //     {
            //         LuaManager.instance.GenerateAllLuaRes();
            //     }
            // },null);
            preLoadCallBack?.Invoke(data.data.path,data.data.nameList, () =>
            {
                Debug.Log(" PreLoad Over ");
                if (GameManager.instance.IsPlayingOneIDCourse&&request.Contains("TD5623"))
                {
                    OneIDSceneManager.Instance.ShowTargetPanel(OneID_SceneType.CameraPanel);
                    OneIDSceneManager.Instance.LoadAllVideoPlayer();
                }
                else
                {
                    WsResponse<PreLoadMsgData> wsResponse = new WsResponse<PreLoadMsgData>
                    {
                        messageId = data.messageId,
                        path = data.path,
                        data = new WsResponse<PreLoadMsgData>.ResponeData<PreLoadMsgData>() { code = 0 },
                        signature = MD5Helper.CalcMD5(data.messageId + Constants.MD5SEED),
                    };
                    communicate.sendMessage(JsonConvert.SerializeObject(wsResponse));
                }
            });
        }

        
        
        
        
        
        public override void ExitCourseHandler(string request)
        {
            Debug.Log(" Exit Course ");
            exitCallBack?.Invoke();
        }

        private void PlayResponse(WsRequest<StringMsgData> data, bool status)
        {
            WsResponse<StringMsgData> wsResponse = new WsResponse<StringMsgData>();
            wsResponse.messageId = data.messageId;
            wsResponse.path = data.path;
            wsResponse.data = new WsResponse<StringMsgData>.ResponeData<StringMsgData>();
            wsResponse.data.code = status ? 0 : 1;
            communicate.sendMessage(JsonConvert.SerializeObject(wsResponse));
        }

    }
}