using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using UnityEngine.UI;
using LuaInterface;
using UnityEngine.Events;
using Ximmerse.InputSystem;

namespace ILFramework
{
    public class XDeviceManager : Manager<XDeviceManager>
    {
        bool isOn = false;
        GameObject game3odf = null;

        public void LoadfromPrefab(string name,LuaFunction lcallback = null , UnityAction ucallback = null)
        {
            string assetName = name;
            //string abName = AppConst.commonDynamic;
            //ResManager.LoadPrefab(abName, assetName, delegate (UnityEngine.Object[] objs)
            //{
            //    if (objs.Length == 0) return;
            //    GameObject prefab = objs[0] as GameObject;
            //    if (prefab == null) return;

            //    GameObject go = Instantiate(prefab) as GameObject;
            //    go.name = assetName;
            //    go.layer = LayerMask.NameToLayer("Default");
            //    go.transform.SetParent(transform);
            //    go.transform.localScale = Vector3.one;
            //    go.transform.localPosition = Vector3.zero;
            //    Debug.LogWarning("Get ::>> " + name + " " + prefab);
            //    isOn = true;
            //    game3odf = go;
            //    if (lcallback != null) lcallback.Call();
            //    if (ucallback != null) ucallback.Invoke();
            //});

            GameObject prefab = ResourceManager.instance.LoadCommonResAB<GameObject>(assetName);
            GameObject go = Instantiate(prefab) as GameObject;
            go.name = assetName;
            go.layer = LayerMask.NameToLayer("Default");
            go.transform.SetParent(transform);
            go.transform.localScale = Vector3.one;
            go.transform.localPosition = Vector3.zero;
            Debug.LogWarning("Get ::>> " + name + " " + prefab);
            isOn = true;
            game3odf = go;
            if (lcallback != null) lcallback.Call();
            if (ucallback != null) ucallback.Invoke();
        }

        public void Init()
        {
            if(isOn == false) LoadfromPrefab("3dof");
        }

        public void InitInLua(LuaFunction callback = null)
        {
            if (isOn == false)
            {
                LoadfromPrefab("3dof", callback);
#if UNITY_ANDROID && !UNITY_EDITOR
                XDevicePlugin.OnPauseUnity(true);
                XDevicePlugin.OnPauseUnity(false); //TODO 模拟切屏 bug: 进入3dof场景需要切屏才能响应设备
#endif
            }
        }

        public void Exit()
        {
            try
            {
                var xd = Util.GetXDevice();
                if (xd)
                {
                    Util.SetSim3dofEnable(false);
                    xd.Exit();
                    Debug.Log("退出并销毁3dof预制体");
                    isOn = false;
                    if (game3odf != null) Destroy(game3odf);
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}