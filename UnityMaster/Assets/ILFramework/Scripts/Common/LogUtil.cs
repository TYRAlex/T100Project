#define USE_TESTCONSOLE
using System;
using System.Collections.Generic;
using System.IO;
using bell.ai.course2.unity.websocket;
using UnityEngine;

namespace Consolation
{
    /// <summary>
    /// A console to display Unity's debug logs in-game.
    /// </summary>
    class LogUtils : MonoBehaviour
    {
#if USE_TESTCONSOLE
        struct Log
        {
            public string message;
            public string stackTrace;
            public LogType type;
        }

        #region Inspector Settings

        /// <summary>
        /// The hotkey to show and hide the console window.
        /// </summary>
        public KeyCode toggleKey = KeyCode.BackQuote;

        /// <summary>
        /// Whether to open the window by shaking the device (mobile-only).
        /// </summary>
        public bool shakeToOpen = true;

        /// <summary>
        /// The (squared) acceleration above which the window should open.
        /// </summary>
        public float shakeAcceleration = 3f;

        /// <summary>
        /// Whether to only keep a certain number of logs.
        ///
        /// Setting this can be helpful if memory usage is a concern.
        /// </summary>
        public bool restrictLogCount = true;

        /// <summary>
        /// Number of logs to keep before removing old ones.
        /// </summary>
        public int maxLogs = 50;

        #endregion

        readonly List<Log> logs = new List<Log>();
        Vector2 scrollPosition;
        bool visible;
        bool collapse;
        bool autoRoll;

        // Visual elements:

        static readonly Dictionary<LogType, Color> logTypeColors = new Dictionary<LogType, Color>
                {
                    { LogType.Assert, Color.white },
                    { LogType.Error, Color.red },
                    { LogType.Exception, Color.red },
                    { LogType.Log, Color.white },
                    { LogType.Warning, Color.yellow },
                };

        const string windowTitle = "Console";
        const int margin = 20;
        static readonly GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");
        static readonly GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");
        static readonly GUIContent autoRollLabel = new GUIContent("Reverse", "");

        readonly Rect titleBarRect = new Rect(0, 0, 10000, 20);
        Rect windowRect = new Rect(margin, margin, Screen.width - (margin * 2), Screen.height - (margin * 2));

        void OnEnable()
        {
            
#if UNITY_2018
            Application.logMessageReceived += HandleLog;
#else
                    Application.RegisterLogCallback(HandleLog);
#endif
        }

        void OnDisable()
        {
            
#if UNITY_2018
            Application.logMessageReceived -= HandleLog;
#else
                    Application.RegisterLogCallback(null);
#endif
        }

        public void Show(bool isShow)
        {
            visible = isShow;
        }

        void Update()
        {
            if (Input.GetKeyDown(toggleKey))
            {
                visible = !visible;
            }

            if (shakeToOpen && Input.acceleration.sqrMagnitude > shakeAcceleration)
            {
                visible = true;
            }
        }

        void OnGUI()
        {
            if (!visible)
            {
                return;
            }

            windowRect = GUILayout.Window(123456, windowRect, DrawConsoleWindow, windowTitle);
        }

        /// <summary>
        /// Displays a window that lists the recorded logs.
        /// </summary>
        /// <param name="windowID">Window ID.</param>
        void DrawConsoleWindow(int windowID)
        {
            DrawLogsList();
            DrawToolbar();

            // Allow the window to be dragged by its title bar.
            GUI.DragWindow(titleBarRect);
        }

        /// <summary>
        /// Displays a scrollable list of logs.
        /// </summary>
        void DrawLogsList()
        {

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (autoRoll)
            {
                for (var i = logs.Count - 1; i >= 0; i--)
                {
                    var log = logs[i];

                    // Combine identical messages if collapse option is chosen.
                    if (collapse && i > 0)
                    {
                        var previousMessage = logs[i - 1].message;

                        if (log.message == previousMessage)
                        {
                            continue;
                        }
                    }
                    GUI.contentColor = logTypeColors[log.type];
                    GUILayout.Label("[" + i + "]:" + log.message);
                }
            }
            else
            {
                // Iterate through the recorded logs.
                for (var i = 0; i < logs.Count; i++)
                {
                    var log = logs[i];

                    // Combine identical messages if collapse option is chosen.
                    if (collapse && i > 0)
                    {
                        var previousMessage = logs[i - 1].message;

                        if (log.message == previousMessage)
                        {
                            continue;
                        }
                    }
                    GUI.contentColor = logTypeColors[log.type];
                    GUILayout.Label("[" + i + "]:" + log.message);
                }
            }

            GUILayout.EndScrollView();

            // Ensure GUI colour is reset before drawing other components.
            GUI.contentColor = Color.white;
        }

        /// <summary>
        /// Displays options for filtering and changing the logs list.
        /// </summary>
        void DrawToolbar()
        {
            GUILayout.BeginHorizontal();

            if (GUILayout.Button(clearLabel))
            {
                logs.Clear();
            }

            collapse = GUILayout.Toggle(collapse, collapseLabel, GUILayout.ExpandWidth(false));

            autoRoll = GUILayout.Toggle(autoRoll, autoRollLabel, GUILayout.ExpandWidth(false));

            GUILayout.Label("Max logs:" + maxLogs, GUILayout.ExpandWidth(false));
            maxLogs = (int)GUILayout.HorizontalSlider(maxLogs, 0, 500, GUILayout.Width(200));
            if (GUILayout.Button("Close", GUILayout.MaxWidth(100))) visible = false;

            GUILayout.EndHorizontal();
        }

        public static bool IsNumber(string s)
        {
            if (string.IsNullOrWhiteSpace(s)) return false;
            const string pattern = "^[0-9]*$";
            System.Text.RegularExpressions.Regex rx = new System.Text.RegularExpressions.Regex(pattern);
            return rx.IsMatch(s);
        }

        /// <summary>
        /// Records a log from the log callback.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="stackTrace">Trace of where the message came from.</param>
        /// <param name="type">Type of message (error, exception, warning, assert).</param>
        void HandleLog(string message, string stackTrace, LogType type)
        {
            logs.Add(new Log
            {
                message = message,
                stackTrace = stackTrace,
                type = type,
            });
            //在每次打开课程的时候写入Log文件
#if !UNITY_EDITOR && UNITY_ANDROID
            ReceiveInfoHandle(message, stackTrace, type);
#endif
            TrimExcessLogs();
        }

        /// <summary>
        /// Removes old logs that exceed the maximum number allowed.
        /// </summary>
        void TrimExcessLogs()
        {
            if (!restrictLogCount)
            {
                return;
            }

            var amountToRemove = Mathf.Max(logs.Count - maxLogs, 0);

            if (amountToRemove == 0)
            {
                return;
            }

            logs.RemoveRange(0, amountToRemove);
        }
        
//-----------------------------------------------------LogWriteToNative---------------------------------------------------

        private string _m_LogPath = null;
        private bool _isStartRecord = false;
        public string CourseData = "Course000Part1";
        /// <summary>
        /// 重置写入参数，创建文件
        /// </summary>
        /// <param name="logName"></param>
        public void InitWrite(string logName)
        {
            GetLogPath(logName);
            CreateLogFile();
            _isStartRecord = true;
        }


        /// <summary>
        /// 设置及获取对应发布平台的日记存放路径
        /// </summary>
        /// <param name="logName"></param>
        private void GetLogPath(string logName)
        {
            switch (Application.platform)
            {
                case RuntimePlatform.Android:
                case RuntimePlatform.OSXPlayer:
                case RuntimePlatform.OSXEditor:
                    _m_LogPath = string.Format("{0}/{1}.txt", Application.persistentDataPath+"/UnityLog", logName);
                    break;
                case RuntimePlatform.WindowsPlayer:
                case RuntimePlatform.WindowsEditor:
                    _m_LogPath = string.Format("{0}/../{1}.txt", Application.dataPath, logName);
                    break;
            }
        }
        
        /// <summary>
        /// 根据路径创建日记文件，并注册文件写入的函数
        /// </summary>
        private void CreateLogFile()
        {
            String path = null;
#if UNITY_ANDROID  || UNITY_STANDALONE_OSX
            path = Application.persistentDataPath + "/UnityLog";
#elif UNITY_STANDALONE_WIN
            path=Application.dataPath+"/UnityLog";
#endif
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            // if (File.Exists(_m_LogPath))
            // {
            //     File.Delete(_m_LogPath);
            // }
            try
            {
                if (!File.Exists(_m_LogPath))
                {
                    FileStream fs = File.Create(_m_LogPath);
                    fs.Close();
                }

                
                // if (File.Exists(_m_LogPath))
                // {
                //     Debug.Log(string.Format("Creat file={0}", _m_LogPath));
                // }
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                throw;
            }
        }

        /// <summary>
        /// 停止写入输出信息
        /// </summary>
        public void StopRecord()
        {
            _isStartRecord = false;
        }

        /// <summary>
        /// 接收的信息回调
        /// </summary>
        /// <param name="condition">回调的信息</param>
        /// <param name="stackTrace">stack信息</param>
        /// <param name="type">信息类型</param>
        private void ReceiveInfoHandle(string condition,string stackTrace,LogType type)
        {
            // if(!_isStartRecord)
            //     return;

            if (type == LogType.Error|| type==LogType.Exception)
            {
                //Debug.Log("-------------------------进入---------------------");
                string targetDate = $"{DateTime.Now:yyyy-MM-dd}";
                InitWrite("Unity-" + targetDate);
                // InitWrite("Unity-" +
                //           DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                //           DateTime.Now.Day);
                
                if (File.Exists(_m_LogPath))
                {
                    var fileStream = File.Open(_m_LogPath, FileMode.Append);
                    using (StreamWriter sw=new StreamWriter(fileStream))
                    {
                        string logStr = string.Empty;
                        // switch (type)
                        // {
                        //     case LogType.Log:
                        //         logStr = string.Format("{0}: {1}\n", type, condition);
                        //         break;
                        //     case LogType.Assert:
                        //     case LogType.Warning:
                        //     case LogType.Exception: 
                        //     case LogType.Error:
                        //         logStr = string.IsNullOrEmpty(stackTrace)
                        //             ? string.Format("{0}: {1}\n{2}\n", type, condition, System.Environment.StackTrace)
                        //             : string.Format("{0}: {1}\n{2}\n", type, condition, stackTrace);
                        //         break;
                        // }
                        string Detail = "课程："+CourseData+ " 具体时间：" + DateTime.Now.Year + "-" + DateTime.Now.Month + "-" +
                                       DateTime.Now.Day + "-" +
                                       DateTime.Now.Hour +
                                       "-" + DateTime.Now.Minute + "-" + DateTime.Now.Second;
                        logStr = string.IsNullOrEmpty(stackTrace)
                            ? string.Format("{0}: {1}\n{2}\n{3}\n", type, condition,Detail, System.Environment.StackTrace)
                            : string.Format("{0}: {1}\n{2}\n{3}\n", type, condition,Detail, stackTrace);

                        sw.WriteLine(logStr);
                    }
                    fileStream.Close();
                    //Debug.Log("-------------------------出去---------------------");
                }
                else
                {
                    Debug.LogError(string.Format("not Exists File ={0}!", _m_LogPath));
                }
            }

            
        }

        /// <summary>
        /// 输出设备信息参数
        /// </summary>
        private void OutPutSystemInfo()
        {
            string str2 = string.Format("日志记录开始时间：{0},版本{1}.", System.DateTime.Now.ToString(), Application.unityVersion);
            string systemInfo = SystemInfo.operatingSystem + " " + SystemInfo.processorType + " " +
                                SystemInfo.processorCount + "  储存容量：" + SystemInfo.systemMemorySize + " 图形设备:" +
                                SystemInfo.graphicsDeviceName + " 供应商：" + SystemInfo.graphicsDeviceVendor + "  存储容量：" +
                                SystemInfo.graphicsMemorySize + " " + SystemInfo.graphicsDeviceVersion;
            Debug.Log(string.Format("{0}\n{1}", str2, systemInfo));
        }

//-------------------------------------------LogVisible--------------------------------------------------
       

#endif
    }
    
    public class BellLog
    {
        /// <summary>
        /// 带LOG条件的Debug输出
        /// </summary>
        /// <param name="str"></param>
        [System.Diagnostics.Conditional("LOG")]
        public static void LogInfo(object str)
        {
            Debug.Log(str);
        }
        
        /// <summary>
        /// 带LOG条件的DebugError输出
        /// </summary>
        /// <param name="str"></param>
        [System.Diagnostics.Conditional("LOG")]
        public static void LogErrorInfo(object str)
        {
            Debug.LogError(str);
        }
    }
    
}