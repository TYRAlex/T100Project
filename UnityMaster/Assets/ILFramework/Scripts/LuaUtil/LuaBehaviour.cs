using UnityEngine;
using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.UI;

namespace LuaFramework {
    public class LuaBehaviour : MonoBehaviour {
        public bool autoRelease = true;//对于课程游戏prefab则设置为false，自动释放则会报错
        protected string data = null;
        protected Dictionary<string, LuaFunction> buttons = new Dictionary<string, LuaFunction>();

        protected void Awake() {
            Util.CallMethod(name, "Awake", gameObject);
        }

        //protected void Start() {
        //    Util.CallMethod(name, "Start");
        //}

        protected void OnClick() {
            Util.CallMethod(name, "OnClick");
        }

        protected void OnClickEvent(GameObject go) {
            Util.CallMethod(name, "OnClick", go);
        }

        protected void OnDisable()
        {
            Util.CallMethod(name, "OnDisable");
        }

        /// <summary>
        /// 添加单击事件
        /// </summary>
        public virtual void AddClick(GameObject go, LuaFunction luafunc) {
            if (go == null || luafunc == null) return;
            if (buttons.ContainsKey(go.name))
            {
                go.GetComponent<Button>().onClick.RemoveAllListeners();
                buttons.Remove(go.name);
            }
            //Debug.Log("AddClick:" + go.name + " luafunc:" + luafunc.ToString());
            buttons.Add(go.name, luafunc);
            go.GetComponent<Button>().onClick.AddListener(
                delegate() {
                    luafunc.Call(go);
                }
            );
        }

        /// <summary>
        /// 删除单击事件
        /// </summary>
        /// <param name="go"></param>
        public void RemoveClick(GameObject go) {
            if (go == null) return;
            LuaFunction luafunc = null;
            if (buttons.TryGetValue(go.name, out luafunc)) {
                luafunc.Dispose();
                luafunc = null;
                buttons.Remove(go.name);
            }
        }

        /// <summary>
        /// 清除单击事件
        /// </summary>
        public void ClearClick() {
            foreach (var de in buttons) {
                if (de.Value != null) {
                    de.Value.Dispose();
                }
            }
            buttons.Clear();
        }

        //-----------------------------------------------------------------
        protected void OnDestroy() {
            Util.CallMethod(name, "OnDestroy");
            //ClearClick();
//#if ASYNC_MODE
//            string abName = name.ToLower().Replace("panel", "");
//            if (autoRelease)//排除报错
//            {
//                ResManager.UnloadAssetBundle(abName + AppConst.ExtName);
//            }
//#endif
//            Util.ClearMemory();
//            Debug.Log("~" + name + " was destroy!");
        }
    }
}