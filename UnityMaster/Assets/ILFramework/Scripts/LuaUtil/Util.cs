using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;
using LuaInterface;
using LuaFramework;
using UnityEngine.UI;
//using ICSharpCode.SharpZipLib.Zip;
using System.Threading;
//using Bell.List;
using UnityEngine.Events;
using ILFramework;
using Vectrosity;
//using Vectrosity;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace LuaFramework
{
    public class Util
    {
        private static List<string> luaPaths = new List<string>();

        public static int Int(object o)
        {
            return Convert.ToInt32(o);
        }

        public static float Float(object o)
        {
            return (float)Math.Round(Convert.ToSingle(o), 2);
        }

        public static long Long(object o)
        {
            return Convert.ToInt64(o);
        }

        public static int Random(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static float Random(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static string Uid(string uid)
        {
            int position = uid.LastIndexOf('_');
            return uid.Remove(0, position + 1);
        }

        public static long GetTime()
        {
            TimeSpan ts = new TimeSpan(DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks);
            return (long)ts.TotalMilliseconds;
        }

        //抛出异常测试
        public static void ExceptionTest(string message)
        {
            throw new SystemException(message);
        }

        /// <summary>
        /// 搜索子物体组件-GameObject版
        /// </summary>
        public static T Get<T>(GameObject go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.transform.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Transform版
        /// </summary>
        public static T Get<T>(Transform go, string subnode) where T : Component
        {
            if (go != null)
            {
                Transform sub = go.Find(subnode);
                if (sub != null) return sub.GetComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 搜索子物体组件-Component版
        /// </summary>
        public static T Get<T>(Component go, string subnode) where T : Component
        {
            return go.transform.Find(subnode).GetComponent<T>();
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(GameObject go) where T : Component
        {
            if (go != null)
            {
                T[] ts = go.GetComponents<T>();
                for (int i = 0; i < ts.Length; i++)
                {
                    if (ts[i] != null) GameObject.Destroy(ts[i]);
                }
                return go.gameObject.AddComponent<T>();
            }
            return null;
        }

        /// <summary>
        /// 添加组件
        /// </summary>
        public static T Add<T>(Transform go) where T : Component
        {
            return Add<T>(go.gameObject);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(GameObject go, string subnode)
        {
            return Child(go.transform, subnode);
        }

        /// <summary>
        /// 查找子对象
        /// </summary>
        public static GameObject Child(Transform go, string subnode)
        {
            Transform tran = go.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(GameObject go, string subnode)
        {
            return Peer(go.transform, subnode);
        }

        /// <summary>
        /// 取平级对象
        /// </summary>
        public static GameObject Peer(Transform go, string subnode)
        {
            Transform tran = go.parent.Find(subnode);
            if (tran == null) return null;
            return tran.gameObject;
        }

        /// <summary>
        /// 计算字符串的MD5值
        /// </summary>
        public static string md5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] data = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] md5Data = md5.ComputeHash(data, 0, data.Length);
            md5.Clear();

            string destString = "";
            for (int i = 0; i < md5Data.Length; i++)
            {
                destString += System.Convert.ToString(md5Data[i], 16).PadLeft(2, '0');
            }
            destString = destString.PadLeft(32, '0');
            return destString;
        }

        /// <summary>
        /// 计算文件的MD5值
        /// </summary>
        public static string md5file(string file)
        {
            try
            {
                if (!File.Exists(file)) return "";
                FileStream fs = new FileStream(file, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(fs);
                fs.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                throw new Exception("md5file() fail, error:" + ex.Message);
            }
        }

        /// <summary>
        /// 清除所有子节点
        /// </summary>
        public static void ClearChild(Transform go)
        {
            if (go == null) return;
            for (int i = go.childCount - 1; i >= 0; i--)
            {
                GameObject.Destroy(go.GetChild(i).gameObject);
            }
        }

        /// <summary>
        /// 取得行文本
        /// </summary>
        public static string GetFileText(string path)
        {
            return File.ReadAllText(path);
        }

        /// <summary>
        /// 网络可用
        /// </summary>
        public static bool NetAvailable
        {
            get
            {
                return Application.internetReachability != NetworkReachability.NotReachable;
            }
        }

        /// <summary>
        /// 是否是无线
        /// </summary>
        public static bool IsWifi
        {
            get
            {
                return Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork;
            }
        }

        

        public static void Log(string str)
        {
            Debug.Log(str);
        }

        public static void LogWarning(string str)
        {
            Debug.LogWarning(str);
        }

        public static void LogError(string str)
        {
            Debug.LogError(str);
        }


        /// <summary>
        /// SetSim3dofEnable
        /// </summary>
        /// <returns></returns>
        public static void SetSim3dofEnable(bool isOn, bool isShowTouchPoint = true)
        {
            _3dofEnable = isOn;
            _isShowTouchPoint = isShowTouchPoint;
        }

        public static bool GetSim3dofEnable()
        {
            return _3dofEnable;
        }

        public static bool GetTouchPointEnable()
        {
            return _isShowTouchPoint;
        }

        static bool _3dofEnable = true;
        static bool _isShowTouchPoint = true;

        public static void SetScreenEffectEnable(bool isOn)
        {
            _screenEffectEnable = isOn;
            if (isOn == false)
            {
                GameObject.FindObjectOfType<DoScreenClickEffect>().DoRelease();
            }
        }

        public static bool GetScreenEffectEnable()
        {
            return _screenEffectEnable;
        }
        static bool _screenEffectEnable = true;

        /// <summary>
        /// 全局查找Xdevice
        /// </summary>
        /// <returns></returns>
        public static GetXDeviceState GetXDevice()
        {
            GetXDeviceState device = null;
            try
            {
                device = GameObject.FindObjectOfType<GetXDeviceState>();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return device;
        }

        /// <summary>
        /// 获取BellPaintView
        /// </summary>
        /// <returns></returns>
        public static BellPaintView GetPaintView()
        {
            BellPaintView view = null;
            try
            {
                view = GameObject.FindObjectOfType<BellPaintView>();
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
            return view;
        }

        /// <summary>
        /// 设置按钮是否可用
        /// </summary>
        /// <param name="btn"></param>
        /// <param name="isOn"></param>
        public static void SetBtnEnable(GameObject btn, bool isOn)
        {
            if (btn == null) return;
            var b = btn.GetComponent<Button>();
            if (b) b.interactable = isOn;
            var t = btn.GetComponent<ToggleImageButton>();
            if (t) t.setDisable(isOn);
        }

        /// <summary>
        /// 改变scrollview横竖条的值
        /// </summary>
        /// <param name="sv"></param>
        /// <param name="v"></param>
        /// <param name="type">v竖 h横</param>
        public static void SetScollViewValue(GameObject sv, float v, string type = "v")
        {
            sv.GetComponent<ScrollRect>().verticalScrollbar.value = v;
        }

        /// <summary>
        /// 添加lua行为脚本
        /// </summary>
        /// <param name="go"></param>
        public static void AddLuaBehaviour(GameObject go, bool autoRelease = true)
        {
            if (go.GetComponent<LuaBehaviour>())
                return;
            LuaBehaviour l = go.AddComponent<LuaBehaviour>();
            l.autoRelease = autoRelease;
        }
        
        /// <summary>
        /// 添加拖拽脚本
        /// </summary>
        /// <param name="go"></param>
        public static void AddDrager(GameObject go, GameObject rect)
        {
            var drag = go.AddComponent<Drager>();
            drag.Init(rect);
        }

        /// <summary>
        /// 获取子物体所有的Button组件
        /// </summary>
        /// <returns></returns>
        public static Component[] GetButtonComponentsInChildren(GameObject go)
        {
            return go.GetComponentsInChildren<Button>();
        }

        /// <summary>
        /// 删除物体
        /// </summary>
        /// <param name="go"></param>
        public static void Destroy(GameObject go)
        {
            GameObject.Destroy(go);
        }

        /// <summary>
        /// 创建方便的数组
        /// </summary>
        /// <returns></returns>
        public static ArrayList CreateArray()
        {
            return new ArrayList();
        }

        /// <summary>
        /// tween完成回调
        /// </summary>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static DG.Tweening.TweenCallback OnComplete(LuaFunction callback)
        {
            DG.Tweening.TweenCallback arg0 = () =>
            {
                if (callback != null) callback.Call();
            };
            return arg0;
        }

        /// <summary>
        /// 使用标签设置父类
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="str"></param>
        public static void SetParentWithTag(GameObject obj, string str)
        {
            obj.transform.SetParent(GameObject.FindWithTag(str).transform);
        }

        /// <summary>
        /// 设置图片
        /// </summary>
        /// <param name="go"></param>
        /// <param name="sp"></param>
        public static void SetImage(GameObject go, Sprite sp, bool isEnable = true)
        {
            Image im = go.GetComponent<Image>();
            if (im == null) return;
            im.sprite = sp;
            im.enabled = isEnable;
        }

        public static void SetTexture(GameObject go, Texture2D tx, bool isEnable = true)
        {
            Image im = go.GetComponent<Image>();
            RawImage rim = go.GetComponent<RawImage>();
            if (rim != null)
            {
                rim.texture = tx;
                rim.enabled = isEnable;
            }
            else if (im != null)
            {
                Sprite sp = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));
                im.sprite = sp;
                im.enabled = isEnable;
            }
        }

        //保存texture2D到本地
        public static void StoreTextureToLocal(Texture2D tx, string dir, string name, bool isAsyn)
        {
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            Debug.Log("path:" + dir + "/" + name);
            byte[] pngData = tx.EncodeToPNG();
            if (isAsyn)
            {
                Thread td = new Thread(() =>
                {
                    File.WriteAllBytes(dir + "/" + name, pngData);
                });
                td.Start();
            }
            else
            {
                File.WriteAllBytes(dir + "/" + name, pngData);
            }
        }

        /// <summary>
        /// 通过tag寻找物体
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static GameObject FindWithTag(string tag)
        {
            return GameObject.FindGameObjectWithTag(tag);
        }

        /// <summary>
        /// 设置图片全屏
        /// </summary>
        /// <param name="go"></param>
        public static void SetImageFullScreen(GameObject go)
        {
            var image = go.GetComponent<Image>();
            if (!image) return;
            if (Screen.width > 1920 || Screen.height > 1080)
            {
                image.rectTransform.sizeDelta = new Vector2(Screen.width, Screen.height);
            }
        }

        /// <summary>
        /// string转byte[]
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        /// 获取设备唯一码
        /// </summary>
        /// <returns></returns>
        public static string GetUUID()
        {
            return SystemInfo.deviceUniqueIdentifier;
        }
        
        /// <summary>
        /// 获取当前时间
        /// </summary>
        /// <returns></returns>
        public static string GetCurrentTime()
        {
            return System.DateTime.Now.ToShortTimeString();
        }

        /// <summary>
        /// 生成字典数组
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> CreateStringDic()
        {
            return new Dictionary<string, string>();
        }

        //获取图片纹理
        public static Texture GetTexture(string path)
        {
            string picPath = path;
            if (!File.Exists(picPath)) return null;
            Texture2D tex = new Texture2D(2, 2);
            try
            {
                FileStream fs = new FileStream(picPath, FileMode.Open);
                if (fs.Length == 0) return null;
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                tex.LoadImage(buffer);
                tex.Apply();
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
            }
            return tex;
        }

        //获取本地图片
        public static Sprite GetSprite(string path)
        {
            string picPath = path;//Application.persistentDataPath + "/" +
            if (!File.Exists(picPath)) return null;
            FileStream fs = new FileStream(picPath, FileMode.Open);
            if (fs.Length == 0) return null;
            byte[] buffer = new byte[fs.Length];
            fs.Read(buffer, 0, buffer.Length);
            fs.Close();
            Texture2D tex = new Texture2D(2, 2);
            tex.LoadImage(buffer);
            tex.Apply();
            Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            return sprite;
        }

        public static bool CheckPath(string path, bool isDir = true)
        {
            bool reVal = false;
            if (isDir)
            {
                reVal = Directory.Exists(path);
            }
            else
            {              
                try
                {
                    reVal = File.Exists(path);
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(path);
                    if (fileInfo.Length == 0) reVal = false;
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    reVal = false;
                }
            }
            return reVal;
        }

        //获取文件扩展名
        public static string GetFileExt(string path)
        {
            string reVal = "";
            if (File.Exists(path))
            {
                reVal = Path.GetExtension(path);
            }
            return reVal;
        }

        //获取文件名
        public static string GetFileName(string path)
        {
            string reVal = "";
            if (File.Exists(path))
            {
                reVal = Path.GetFileName(path);
            }
            return reVal;
        }

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="path"></param>
        public static void DeleteFile(string path)
        {
            try
            {
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
            }
        }

        public static VectorLine DrawLine(string name, LuaTable points, float width, Texture texture = null)
        {
            List<Vector2> linePoints = new List<Vector2>();
            object[] o = points.ToArray();
            for (int i = 0; i < o.Length; i++)
            {
                linePoints.Add((Vector2)o[i]);
            }
            var line = new VectorLine(name, linePoints, texture, width);
            line.Draw();
            return line;
        }

        public static void RefreshLine(VectorLine line, LuaTable points)
        {
            object[] o = points.ToArray();
            for (int i = 0; i < o.Length; i++)
            {
                line.points2[i] = (Vector2)o[i];
            }
            line.Draw();
        }

        public static void AddToggleClick(GameObject game, LuaFunction func)
        {
            Toggle tg = game.GetComponent<Toggle>();
            if (tg)
            {
                tg.onValueChanged.AddListener((bool isOn) => {
                    func.Call(game, isOn);
                });
            }
        }

        public static void CallMethod(string luaFile, string method, params object[] args)
        {
            LuaFunction func = LuaManager.instance.GlobalLua.GetFunction(luaFile + "." + method);
            if(func!=null)
                func.Call(args);
        }
    }
}