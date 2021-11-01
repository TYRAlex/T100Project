using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.IO;
using bell.ai.course2.unity.websocket;
using System.Runtime.InteropServices;
using ILFramework.Scripts.Utility;

namespace ILFramework
{
    public class GameManager : Manager<GameManager>
    {

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
        [DllImport("user32.dll", CharSet = CharSet.Auto, EntryPoint = "SetWindowPos")]
        static extern bool SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);
        [DllImport("user32.dll")]
        static extern IntPtr GetActiveWindow();
        [DllImport("user32.dll")]
        static extern IntPtr SetWindowLong(IntPtr hwnd, int _nIndex, int dwNewLong);
        [DllImport("user32.dll")]
        public static extern bool ShowWindow(IntPtr hwnd, int nCmdShow);
        [DllImport("user32.dll", EntryPoint = "FindWindow")]
        public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        public static IntPtr ParenthWnd = FindWindow(null, "ILFramework");
        [DllImport("user32.dll")]
        static extern bool SetLayeredWindowAttributes(IntPtr hwnd, int crKey, int bAlpha, int dwFlags);

        int LWA_ALPHA = 0x2;
        int LWA_COLORKEY = 0x1;
        uint SHOW_WINDOW = 0x0040;
        int GWL_STYLE = -16;
        int GWL_EXSTYLE = -20;
        int WS_POPUP = 0x800000;
        int WS_EX_LAYERED = 0x00080000;
#endif

        public MessageCenterImpl messageCenterImpl;

        public HotfixPackage currentPlayering;

        public GameObject sim3Dof;

        public bool IsPlayingOneIDCourse = false;
        
        //[HideInInspector]
        public string CurrentPlayCourseName=String.Empty;

        public bool IsUsingNewAudioFunc = true;

        private Consolation.LogUtils _logUtils;
        private void Start()
        {
            Screen.fullScreen = false;
#if !UNITY_EDITOR && UNITY_ANDROID
            messageCenterImpl.playCallBack += PlayUnity;
            messageCenterImpl.stopCallBack += StopUnity;
            messageCenterImpl.preLoadCallBack += PreLoad;
            messageCenterImpl.exitCallBack += ExitCourse;
#elif UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN || UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
            GetCmdPlayUnity();
#endif
            _logUtils = gameObject.AddComponent<Consolation.LogUtils>();
            //gameObject.AddComponent<BellLoom>();     
        }


        public void PlayUnity(string data)
        {
            BellLoom.QueueActions((o) =>
            {
                CurrentPlayCourseName = data;
                PlayUnity(data, false);
            }, null);          
        }

        public void PlayUnity(string data, bool isCmd = false)
        {
#if !UNITY_EDITOR
             // BellLoom.QueueActions((o) => _logUtils.InitWrite("Unity-" + data.Replace("/", "") + "-" +
             //                                                 DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
             //                                                 DateTime.Now.Day + "-" +
             //                                                 DateTime.Now.Hour +
             //                                                 "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second),
             //    null); 
#endif
            CloseSubscribe3Dof();
            //每次切换环节 唤醒语音按钮
            SoundManager.instance.ReSetVoiceBtnEnable();
            //每次切换环节 隐藏重玩按钮
            LogicManager.instance.ShowReplayBtn(false);
            string temStr = "";
            if (sim3Dof)
            {
                try
                {
                    temStr = data.Substring(6, 5);
                    int tem = int.Parse(temStr);
                    sim3Dof.SetActive(true);
                    Debug.Log("@是一个体感课程:---------------------------------：" + tem);
                }
                catch (Exception)
                {
                    sim3Dof.SetActive(false);
                    Debug.Log("@不是一个体感课程:---------------------------------：" + temStr);
                }
            }

            if (isCmd)
            {
                if (currentPlayering != null)
                    currentPlayering.DestoryHotfixpackage();
                HotfixManager.instance.totalPackage = 1;
                HotfixManager.instance.InstanceHotfixPackage(data, (p) =>
                {
                    HotfixManager.instance.curShowPackage = p;
                    currentPlayering = p;
                    HotfixManager.instance.LoadCourseDll(currentPlayering, () => { HotfixManager.instance.ShowHotfixPackage(data); });
                });
            }
            else
            {
                BellLoom.QueueActions((o) => _logUtils.CourseData = data, null);
                Debug.LogFormat(" PlayUnity: {0} ", data);
                LogicManager.instance.ShowVideo(false);
                SoundManager.instance.ResetAudio();
                currentPlayering = Util.GetHotfixPackage(data.Replace("/", ""));
                HotfixManager.instance.LoadCourseDll(currentPlayering,
                    () => { HotfixManager.instance.ShowHotfixPackage(data); });
            }



        }


        public void StopUnity(string data)
        {
            //if (currentPlayering != null)
            //{
            //    currentPlayering.DestoryHotfixpackage();
            //    currentPlayering = null;
            //}

            BellLoom.QueueActions((o) =>
            {
                LogicManager.instance.ShowVideo(false);
                CloseSubscribe3Dof();
                DG.Tweening.DOTween.KillAll();

                StopAllCoroutines();
                HotfixManager.instance.ShowHotfixPackage();

            }, null);

        }

        /// <summary>
        /// 退出课程回调
        /// </summary>
        public void ExitCourse()
        {
            Debug.Log("@-------------退出课程回调");
            //在多线程里面执行主线程才能执行的一些代码需要用到BellLoom脚本
            BellLoom.QueueActions((O) => ExitCourseInUnity(), null);
            //Loom.QueueOnMainThread(ExitCourseInUnity);

        }

        /// <summary>
        /// 退出课程
        /// </summary>
        void ExitCourseInUnity()
        {
            //Debug.Log("Excute ExistCourse~ ");

            foreach (var item in HotfixManager.hotfixDict)
            {
                item.Value.DestoryHotfixpackage();
            }
            // Debug.Log("item RemoveZip");
            HotfixManager.hotfixDict.Clear();
            // Debug.Log("Dic Remove");
            LogicManager.instance.ShowVideo(false);
            // Debug.Log("1");
            SoundManager.instance.ResetAllVoiceBtnEvent();
            //SoundManager.instance.StopAudio();
            // Debug.Log("2");
            SoundManager.instance.ResetStatus();
            // Debug.Log("Sound Reset");
            HotfixManager.instance.curShowPackage = null;
            currentPlayering = null;
            ResourceManager.instance.ReloadAssetBundle();
            ResourceManager.instance.ResetAudioAndCourseResAB();
            CloseSubscribe3Dof();
            TabletWebsocketController.Instance.ResetAllStatu();
            //GC.Collect();
            Debug.Log("End Reset!");
#if !UNITY_EDITOR       
            //_logUtils.StopRecord();
#endif
        }

        void CloseSubscribe3Dof()
        {
            ConnectAndroid.Instance.CloseSubscribe();
            FunctionOf3Dof.Instance.ResetAllDelegate();
            Util.SetSim3dofEnable(false);
        }

        void GetCmdPlayUnity()
        {
            string[] cmd = Environment.GetCommandLineArgs();

            // 命令行传参格式：参数0 - 启动unity命令, 参数1 - 资源加载路径, 参数2，3 - unity程序窗口坐标, 参数4，5 - unity窗口宽高
            int posx = Convert.ToInt32(cmd[2]);
            int posy = Convert.ToInt32(cmd[3]);
            int width = Convert.ToInt32(cmd[4]);
            int height = Convert.ToInt32(cmd[5]);

            StartCoroutine(SetUnityScreen(posx, posy, width, height, () =>
            {
                int idx = 0;
                if (cmd[1].Contains("\\"))
                    idx = cmd[1].LastIndexOf('\\');
                else if (cmd[1].Contains("/"))
                    idx = cmd[1].LastIndexOf('/');

                Debug.LogFormat(" cmd unity path : {0}  idx: {1}", cmd[1], idx);
                int len = cmd[1].Length;
                AppConst.hotfix_dir = cmd[1].Substring(0, idx);
                AppConst.luaCoursePath = AppConst.hotfix_dir.Replace("file://", "");
                PlayUnity(cmd[1].Substring(idx + 1, len - idx - 1), true);
            }));
        }

        //        void SetUnityScreen(int posx, int posy, int width, int height)
        //        {
        //            Debug.LogFormat(" ****** SetUnityScreen ******  posx: {0}, posy: {1}, width {2}, height {3} ", posx, posy, width, height);
        //#if UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
        //            SetWindowLong(ParenthWnd, GWL_STYLE, WS_POPUP);
        //            bool s = SetWindowPos(ParenthWnd, 0, posx, posy, width, height, SHOW_WINDOW);
        //            Debug.LogFormat(" SetWindowPos Result : {0} ", s);
        //#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
        //            UnityEngine.Screen.SetResolution(width, height, false);
        //#endif
        //        }

        IEnumerator SetUnityScreen(int posx, int posy, int width, int height, Action callBack = null)
        {
            yield return 0;
            Debug.LogFormat(" ****** SetUnityScreen ******  posx: {0}, posy: {1}, width {2}, height {3} ", posx, posy, width, height);
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR_WIN
            //yield return new WaitForSeconds(0.1f);
            //SetWindowLong(ParenthWnd, GWL_STYLE, WS_POPUP); // 设置无边框
            SetWindowPos(ParenthWnd, 0, posx, posy, width, height, SHOW_WINDOW);
            //SetLayeredWindowAttributes(ParenthWnd, 0, 255, LWA_ALPHA);   // 设置窗口层级一直在最顶层，无法被别的窗口覆盖
#elif UNITY_STANDALONE_OSX && !UNITY_EDITOR_OSX
            UnityEngine.Screen.SetResolution(width, height, false);
#endif
            callBack?.Invoke();
        }

        public void PreLoad(string path,string[] courseParts, Action ac)
        {
            BellLoom.QueueActions((O) =>
            {
                HotfixManager.instance.totalPackage = courseParts.Length;
                PreLoadPackage(path,courseParts, 0, ac);
            }, null);
        }


        void PreLoadPackage(string path,string[] str, int i = 0, Action ac = null)
        {
            HotfixManager.instance.InstanceHotfixPackage(str[i], (p) =>
            {
                Debug.LogFormat(" Init {0} Success !!!! ", str[i]);
                if (i < str.Length - 1)
                    PreLoadPackage(path, str, ++i, ac);
                else
                    ac?.Invoke();
            }, path);
        }
    }
}