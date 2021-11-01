using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ILFramework
{
    public class MesManager : Manager<MesManager>
    {
        //注册
        //MesManager.instance.Register(类名称，(int)枚举, 方法);   方法 => 方法名(params objet[] param)
        //调用
        //MesManager.instance.Dispatch(类名称，(int)枚举, 1, 2, 3, 4, 5, 6);可以跟多种数据类型
        public delegate void EventMgr(params object[] param);

        public Dictionary<string, Dictionary<int, EventMgr>> EventListerDict = new Dictionary<string, Dictionary<int, EventMgr>>();

        public void Register(string name,int messageType, EventMgr eventMgr)
        {
            if(!EventListerDict.ContainsKey(name))
            {
                EventListerDict[name] = new Dictionary<int, EventMgr>();
            }
            if (EventListerDict[name].ContainsKey(messageType))
            {
                Debug.LogError("messageType:" + messageType + "已存在！");
            }
            else
            {
                EventListerDict[name].Add(messageType, eventMgr);
            }
        }
        public void UnRegister(string name,int messageType)
        {
            if (EventListerDict[name] != null && EventListerDict[name].ContainsKey(messageType))
            {
                EventListerDict[name].Remove(messageType);
                Debug.Log("移除事件：" + messageType);
            }
            else
            {
                Debug.LogError("messageType:" + messageType + "不存在！");
            }
        }
        public void Dispatch(string name,int messageType, params object[] param)
        {
            if (EventListerDict[name].ContainsKey(messageType))
            {
                EventListerDict[name][messageType].Invoke(param);
            }
            else
            {
                Debug.LogError("事件：" + messageType + "未注册！");
            }
        }


    }
}

