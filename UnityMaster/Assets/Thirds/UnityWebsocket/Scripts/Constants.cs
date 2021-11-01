using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace bell.ai.course2.unity.websocket
{
    public class Constants
    {
        //public static int[] ports = new int[] { 4008,5008,6008,7008,8008,9008,10008,11008,12008,13008,14008 };
        public static int[] ports = new int[] {8605, 8606, 8607, 8608, 8609};
        public static int[] mabotPorts = new int[] { 15003, 15002, 15001 };
        //public static int[] mabotPorts = new int[] { 16005, 16006, 16007, 16008, 16009 };
        public static string ws = "ws://127.0.0.1:";
        public static string MD5SEED = "5da5a905eaa375006dc97d87";

        public static string cloudws = "ws://192.168.10.59:8080/hub/uncertainty__bCJMJqJnBt";

        public static string U3D_Sign = "U3D_";

        public enum DEVICE_ID
        {
            GetMabot = 10000,      // 获取MaBot等设备的信息
            GetGyroscope = 10001,  // 获取陀螺仪数据
            StopSubscribe = 10002, // 停止订阅消息
            GetMotor = 10003,      // 获取驱动轮数据
            GetColorSensor = 10004,// 获取颜色传感器数据
        }

        public enum MESSAG_ID
        {
            GetMabot = 10000,
            GetGyroscope = 10001,
            StopSubscribe = 10002,
            GetMotor = 10003,
            GetColorSensor = 10004,
        }

        /// 协议模式
        public static string subscribe = "subscribe";         // 开始订阅消息
        public static string onSubscribe = "onSubscribe";     // 收到订阅消息返回
        public static string subscribeData = "subscribeData"; // 持续发送订阅消息
        public static string unsubscribe = "unsubscribe";     // 停止订阅消息
        public static string onUnsubscribe = "onUnsubscribe"; // 收到停止订阅消息返回

        public static string request = "request";             // 请求数据
        public static string response = "response";           // 返回数据
        public static string stop = "stop";

        /// 通信协议
        public static string GetBlueDvicesReq = "/common/bleDevices";         // 获取蓝牙设备
        public static string GetGyroscopeReq = "/common/getGyroscopeData";    // 获取陀螺仪数据
        public static string GetColorSensorReq = "/common/getColorData";      // 获取颜色传感器数据
        public static string GetMotorDataReq = "/common/getMotorData";        // 获取驱动轮数据

        public static string HeartRep = "/common/echo";          //心跳包
        public static string PlayU3DRep = "/u3d/play-file";      // 进入unity环节
        public static string StopU3DRep = "/u3d/stop";           // 退出unity环节
        public static string PreLoadRep = "/u3d/preload";        // 预加载
        public static string ExitCourseRep = "/u3d/quit";        // 退出课程
    }
}