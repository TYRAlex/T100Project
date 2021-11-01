
using UnityEngine;
using System.Diagnostics;

namespace UnityEditor
{
    public static class Open
    {
      
        [MenuItem("Open常用/BellNAS")]
        public static void OpenBellNAS()
        {
            string url = "http://192.168.7.6:5000";
            Application.OpenURL(url);
        }
    
        [MenuItem("Open常用/T100多媒体课程资源平台(账号：15510267073  密码：10267073)")]
        public static void OpenT100CourseAssetPlatform()
        {
            string url = "http://192.168.7.21:8200";
            Application.OpenURL(url);

            
        }

        [MenuItem("Open常用/田丁美术进度表")]
        public static void OpenTDSchedule()
        {         
            string url = "https://docs.qq.com/sheet/DUmpzU2VmcVlDaWFB";
            Application.OpenURL(url);
        }

        [MenuItem("Open常用/机器人进度表")]
        public static void OpenRobotSchedule()
        {
            string url = "https://docs.qq.com/sheet/DUmtLTm5yWHFmdGVU";
            Application.OpenURL(url);
        }

        [MenuItem("Open常用/Unity资源备份表")]
        public static void OpenUnityAssetBackupsSchedule()
        {
            string url = "https://docs.qq.com/sheet/DV3JhVFRRZG1MeG9a";
            Application.OpenURL(url);
        }

        [MenuItem("Open常用/TAPD平台")]
        public static void OpenTapdPlatform()
        {
            string url = "https://www.tapd.cn/my_dashboard";
            Application.OpenURL(url);
        }

        [MenuItem("Open常用/BellGit")]
        public static void OpenBellGit()
        {
            string url = "https://git.bell.ai/";
            Application.OpenURL(url);
        }

        [MenuItem("Open常用/BeBO课程平台-测试(需要填写本地exe文件路径)")]
        public static void OpenBeboCoursePlatform()
        {
            string path = "C:\\Users\\User\\AppData\\Local\\Programs\\bebo-course-platform-test\\BeBO课程平台-测试.exe";
            Process.Start(path);
        }
       

        [MenuItem("Assets/Open课程脚本文件夹", priority = 0)]
        public static void OpenCourseCsFolder()
        {
            string[] selectsGUID = Selection.assetGUIDs;
            for (int i = 0; i < selectsGUID.Length; i++)
            {
                string coursePath = AssetDatabase.GUIDToAssetPath(selectsGUID[i]);
               
                if (!coursePath.Contains("Assets/HotFixPackage/"))
                    continue;           

                coursePath = coursePath.Replace("Assets/HotFixPackage", "HotfixCodeProject");
              
                string openURL =System.IO.Path.Combine(Application.dataPath, "../", coursePath);
                         
                Application.OpenURL(openURL);
            }
        }

        [MenuItem("Open常用/TD模板文件夹", priority = 0)]
        public static void OpenTDTemplateFolder()
        {
            string openURL = Application.dataPath.Replace("Assets", "") + "HotfixCodeProject/Template";
            Application.OpenURL(openURL);
        }

        [MenuItem("Open常用/结尾视频文件夹", priority = 1)]
        public static void OpenEndMp4Folder()
        {
            string openURL = Application.dataPath.Replace("Assets", "") + "EndMp4";
            Application.OpenURL(openURL);
        }
    }

}
