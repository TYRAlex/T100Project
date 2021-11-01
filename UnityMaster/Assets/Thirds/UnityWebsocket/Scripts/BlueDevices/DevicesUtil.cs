using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using bell.ai.course2.unity.websocket;
using Newtonsoft.Json;

namespace ILFramework
{
    public class DevicesUtil
    {
        static List<DeviceInfoReq.SingleDevice> connectDevices = new List<DeviceInfoReq.SingleDevice>();
        /// <summary>
        /// 查询当前所有蓝牙设备
        /// </summary>
        public static void QueryDevices()
        {
            WsRequest<string> req = new WsRequest<string>()
            {
                deviceId = Constants.U3D_Sign + (int)Constants.DEVICE_ID.GetMabot,
                messageId = ((int)Constants.MESSAG_ID.GetMabot).ToString(),
                path = Constants.GetBlueDvicesReq,
            };
            req.signature = MD5Helper.CalcMD5(req.messageId + Constants.MD5SEED);
            NetworkManager.instance.cm.SendMabotMsg(JsonUtility.ToJson(req));
        }


        public static void SetDevicesInfo(DeviceInfoReq.SingleDevice[] devices)
        {
            connectDevices.Clear();
            for (int i = 0; i < devices.Length; i++)
            {
                if (devices[i].connected == "true")
                    connectDevices.Add(devices[i]);
            }
        }

        /// <summary>
        /// 获取当前连接蓝牙设备
        /// </summary>
        /// <returns></returns>
        public static List<DeviceInfoReq.SingleDevice> GetDevices()
        {
            Debug.LogFormat("当前连接设备数量为{0}", connectDevices.Count);
            return connectDevices;
        }

        public static DeviceInfoReq.SingleDevice GetDevices(string address)
        {
            for (int i = 0; i < connectDevices.Count; i++)
            {
                if (connectDevices[i].address == address)
                    return connectDevices[i];
            }
            Debug.LogFormat(" 当前不存在地址为 {0} 的已连接设备! ", address);
            return null;
        }

        /// <summary>
        /// 获取指定设备的指定消息数据
        /// </summary>
        /// <param name="deviceName"> 设备名称 </param>
        /// <param name="deviceType"> 设备类型 </param>
        /// <param name="deviceID"> 消息设备ID </param>
        /// <param name="messageID"> 消息ID </param>
        /// <param name="mode"> 请求类型 </param>
        /// <param name="reqPath"> 协议消息 </param>
        public static void GetDeviceDataRequest(string deviceType, string[] address, string deviceID, string messageID, string mode, string reqPath)
        {
            WsRequest<MabotInfoReq> req = new WsRequest<MabotInfoReq>()
            {
                deviceId = deviceID,
                messageId = messageID,
                path = reqPath,
                mode = mode,
            };
            req.data = new MabotInfoReq();
            req.signature = MD5Helper.CalcMD5(req.messageId + Constants.MD5SEED);
            req.data.addressArray = address;
            req.data.type = deviceType;
            NetworkManager.instance.cm.SendMabotMsg(JsonConvert.SerializeObject(req));
        }

        /// <summary>
        /// 停止订阅消息
        /// </summary>
        /// <param name="reqPath"> 协议消息 </param>
        public static void StopSubcribe(string reqPath)
        {
            WsRequest<StopSubcribeReq> req = new WsRequest<StopSubcribeReq>()
            {
                deviceId = Constants.U3D_Sign + (int)Constants.DEVICE_ID.StopSubscribe,
                messageId = ((int)Constants.MESSAG_ID.StopSubscribe).ToString(),
                path = reqPath,
                mode = Constants.unsubscribe,
            };
            req.signature = MD5Helper.CalcMD5(req.messageId + Constants.MD5SEED);
            NetworkManager.instance.cm.SendMabotMsg(JsonConvert.SerializeObject(req));
        }
    }
}
