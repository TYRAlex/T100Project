using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework
{
    public class AppConst
    {
        public static float TimerInterval = 1.0f;

        public static string AppName;
        public static string AssetDir;
        public static bool DebugMode = true;
        public static string FrameworkRoot;
        public static string bundleLua = "lua_";
        public static string osDir = LuaConst.osDir;
        public static string commonResDir = "Assets/ILFramework/CommonRes";
        public static string abExt = "_htp";
        public static string common = "Common";
        public static string commonAB_Audio = "CommonAudio_htp";
        public static string commonDynamic = "CommonDynamic_htp";

#if UNITY_EDITOR && UNITY_ANDROID
        public static string hotfix_dir = Application.dataPath + "/../OutHotfixAssetPackage";
        public static string hotfix_platform = "/android/";
        public static string luaCoursePath = hotfix_dir;
        public static string str_OS = "android";
        public static string luaResDir = "Android";
        public static string zipPre = "and_";
#elif !UNITY_EDITOR && UNITY_ANDROID
        public static string hotfix_dir = "file://" + Application.persistentDataPath + "/static/resource";
        public static string hotfix_platform = "/";
        public static string luaCoursePath = Application.persistentDataPath + "/static/resource";
        public static string str_OS = "";
        public static string luaResDir = "ToluaRes";
        public static string zipPre = "and_";
#elif !UNITY_EDITOR && UNITY_STANDALONE_WIN || !UNITY_EDITOR && UNITY_STANDALONE_OSX
        public static string hotfix_dir = ""; //Application.persistentDataPath + "/data";
        public static string hotfix_platform = "/";
        public static string luaCoursePath = "";
        public static string str_OS = "";
        public static string luaResDir = "Win";
        public static string zipPre = "";
#elif UNITY_EDITOR_WIN
        public static string hotfix_dir = Application.dataPath + "/../OutHotfixAssetPackage";
        public static string hotfix_platform = "/windows/";
        public static string luaCoursePath = hotfix_dir;
        public static string str_OS = "windows";
        public static string luaResDir = "Win";
        public static string zipPre = "win_";
#elif UNITY_EDITOR_OSX
        public static string hotfix_dir = "file://" + Application.dataPath + "/../OutHotfixAssetPackage";
        public static string hotfix_platform = "/osx/";
        public static string luaCoursePath = hotfix_dir.Replace("file://", "");
        public static string str_OS = "osx";
        public static string luaResDir = "Win";
        public static string zipPre = "mac_";
#endif
        public const string HOTFIXNAMESPACE = "ILFramework.HotClass.";

    }
}
