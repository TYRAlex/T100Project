using System;

namespace bell.ai.course2.unity.websocket
{

    /// <summary>
    /// Ws回应包包
    /// </summary>
    /// <typeparam name="T">为msg的泛型</typeparam>

    [Serializable]
    public class WsResponse<T>
    {
        public string messageId;
        public string deviceId = "U3D_";
        public string path;
        public string mode = Constants.response;
        public ResponeData<T> data;
        public string signature;

        [Serializable]
        public class ResponeData<W>
        {
            public int code;
            public W msg;
            public W data;
        }
    }

    // 请求现有连接设备信息
    [Serializable]
    public class DeviceInfoReq
    {
        public SingleDevice[] devices;

        [Serializable]
        public class SingleDevice
        {
            public string connected = "";
            public string name = "";
            public string type = "";
            public string address = "";
        }
    }

    // 请求指定Mabot设备信息
    [Serializable]
    public class MabotInfoReq
    {
        public string type = "";
        public string name = "";
        public string[] addressArray;
    }

    // 停止消息订阅请求
    [Serializable]
    public class StopSubcribeReq
    {
        public string action = Constants.stop;
    }

    //获取陀螺仪等数据 [ 结构一样的数据通用(命名就不改了..) ]
    [Serializable]
    public class GyroscopeData
    {
        public string msg = "";
        public string msgType = "";
        public string name = "";
        public string type = "";
    }

    // 获取mabot各部件数据 需要根据不同的消息格式定义不同的类
    [Serializable]
    public class MabotCommonData1
    {
        public string[] addressArray;
        public string msg = "";
        public string msgType = "";
        public string name = "";
        public string type = "";
    }
}
