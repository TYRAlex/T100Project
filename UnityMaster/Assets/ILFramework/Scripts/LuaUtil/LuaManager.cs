using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using ILFramework;
using LuaInterface;
using UnityEngine.Networking;

public class LuaManager : Manager<LuaManager>
{
    public LuaState GlobalLua = null;
    LuaLooper loop = null;
    LuaResLoader lualoader = null;

    string[] toLuaAB = new string[]
    {
        "lua.unity3d",
        "lua_cjson.unity3d",
        "lua_jit.unity3d",
        "lua_lpeg.unity3d",
        "lua_misc.unity3d",
        "lua_protobuf.unity3d",
        "lua_socket.unity3d",
        "lua_system.unity3d",
        "lua_system_injection.unity3d",
        "lua_system_reflection.unity3d",
        "lua_unityengine.unity3d",
    };

    private void Start()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        StartCoroutine(GenerateToLuaRes());
#else
        Init();
#endif
    }

    // public void GenerateAllLuaRes()
    // {
    //     StartCoroutine(GenerateToLuaRes());
    // }

    void Init()
    {
        lualoader = new LuaResLoader();
        lualoader.beZip = true;
        LoadToLua();
        gameObject.AddComponent<LuaClient>();
        // GlobalLua = new LuaState();
        // GlobalLua.LuaSetTop(0);
        // LuaBinder.Bind(GlobalLua);
        // LuaCoroutine.Register(GlobalLua, this);

        //GlobalLua.Start();
        //loop = gameObject.AddComponent<LuaLooper>();
        //loop.luaState = GlobalLua;
        GlobalLua = LuaClient.GetMainState();
        Debug.LogFormat(" LuaState Start.... {0} ", GlobalLua);
    }

    void LoadToLua()
    {
        for (int i = 0; i < toLuaAB.Length; i++)
        {
            AddBundle(AppConst.luaResDir + "/" + toLuaAB[i]);
        }
    }

    void AddBundle(string bundleName)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        string bundlePath = Application.persistentDataPath + "/" + bundleName;
#else
        string bundlePath = Application.streamingAssetsPath + "/" + bundleName;
#endif

        Debug.LogFormat(" Tolua Add Bundle bundlePath : {0} ", bundlePath);
        if (File.Exists(bundlePath))
        {
#if UNITY_EDITOR || UNITY_ANDROID
            AssetBundle ab = AssetBundle.LoadFromFile(bundlePath);
            // AssetBundleCreateRequest abcreateRequest = AssetBundle.LoadFromFileAsync(bundlePath);
            // abcreateRequest.completed += (asyn) =>
            // {
            //     if (abcreateRequest.isDone)
            //     {
            //         bundleName = bundleName.Replace(AppConst.luaResDir + "/", "").Replace(".unity3d", "");
            //         Debug.LogFormat(" Tolua Add Bundle: {0} ", bundleName);
            //         lualoader.AddSearchBundle(bundleName, abcreateRequest.assetBundle);
            //     }
            // };
            if (ab != null)
            {
                bundleName = bundleName.Replace(AppConst.luaResDir + "/", "").Replace(".unity3d", "");
                Debug.LogFormat(" Tolua Add Bundle: {0} ", bundleName);
                lualoader.AddSearchBundle(bundleName, ab);
            }
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
             byte[] bytes = File.ReadAllBytes(bundlePath);
             AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
            if (ab != null)
            {
                bundleName = bundleName.Replace(AppConst.luaResDir + "/", "").Replace(".unity3d", "");
                Debug.LogFormat(" Tolua Add Bundle: {0} ", bundleName);
                lualoader.AddSearchBundle(bundleName, ab);
            }
#endif
            
            
            
        }
        else
            Debug.LogErrorFormat("路径:{0}不存在，缺少 ToLua 资源，无法加载 ToLua！", bundlePath);
    }

    /// <summary>
    /// 添加课程 Lua AB包
    /// </summary>
    /// <param name="courseName"> 课程名称 </param>
    public bool AddLuaCourseBundle(string courseName)
    {
        string zipKey = AppConst.bundleLua + courseName.ToLower();
        if (lualoader.IsLoadZipMap(zipKey))
        {
            Debug.LogFormat("当前课程 {0} lua资源已加载!", courseName);
            return false;
        }
        Debug.LogFormat(" 当前课程 {0} lua资源未加载,开始加载!", courseName);
        string coursePath = courseName;

#if UNITY_ANDROID && UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
        int idx = coursePath.LastIndexOf('P');
        if (idx > 0)
        {
            string tempPart = coursePath.Substring(idx);
            coursePath = coursePath.Replace(tempPart, "/") + tempPart;
        }
#endif
#if UNITY_ANDROID && !UNITY_EDITOR
        string bundlePath=HotfixManager.instance.AndroidCurrentPath+"/"+coursePath + AppConst.hotfix_platform + AppConst.bundleLua + courseName.ToLower() + "_htp";
#else
        string bundlePath = AppConst.luaCoursePath + "/" + coursePath + AppConst.hotfix_platform + AppConst.bundleLua + courseName.ToLower() + "_htp";
#endif
        
        
        if (File.Exists(bundlePath))
        {
            Debug.Log("1");
            byte[] bytes = HotfixManager.instance.DncryptionAssetBundle(File.ReadAllBytes(bundlePath)); //File.ReadAllBytes(bundlePath);
            //20210120 添加修改成LoadFromeFile形式
            Debug.Log("2");
            HotfixManager.instance.WriteDncryptionDataToFile(bytes,bundlePath);
            
#if UNITY_EDITOR || UNITY_ANDROID
            //20210120 修改成LoadFromeFile形式
            AssetBundle ab = AssetBundle.LoadFromFile(bundlePath+"1");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
            //byte[] bytes = File.ReadAllBytes(bundlePath);
            //之前的方式
            Debug.Log("3");
            AssetBundle ab = AssetBundle.LoadFromMemory(bytes);
#endif
            
            
            
            if (ab != null)
                lualoader.AddSearchBundle(zipKey, ab);
            else
                Debug.LogErrorFormat(" 当前课程 {0} lua资源加载失败! ", zipKey);
        }
        else
            Debug.LogErrorFormat(" 课程 {0} 缺少 lua 资源, 无法加载！ 路径为: {1}", zipKey, bundlePath);
        return true;
    }

    /// <summary>
    /// 拷贝streamingAssetsPath目录下的ToLua资源到persistentDataPath目录
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateToLuaRes()
    {
        string toluaStrem = Application.streamingAssetsPath + "/" + AppConst.osDir;
        string toluaPer = Application.persistentDataPath + "/" + AppConst.luaResDir;
        string filePath = toluaPer + "/";

        for (int i = 0; i < toLuaAB.Length; i++)
        {
            UnityWebRequest ws = UnityWebRequest.Get(toluaStrem + "/" + toLuaAB[i]);
            Debug.LogFormat(" GenerateToLuaRes  ws: {0} ", toluaStrem + "/" + toLuaAB[i]);
            yield return ws.SendWebRequest();
            if (ws.isHttpError || ws.isNetworkError)
                Debug.LogError(ws.error);
            byte[] ab = ws.downloadHandler.data;

            string pStr = filePath + toLuaAB[i];
            Debug.LogFormat(" GenerateToLuaRes  pStr: {0} ", pStr);
            if (!Directory.Exists(toluaPer))
                Directory.CreateDirectory(toluaPer);
            if (!File.Exists(pStr))
            {
                FileStream file = File.Create(pStr);
                file.Write(ab, 0, ab.Length);
                file.Flush();
                file.Close();
            }
        }
        Init();
    }

    public void RemoveLuaZipMap(string zipName)
    {
        lualoader.RemoveZipMap(zipName);
    }

    private void OnDestroy()
    {
        if (GlobalLua != null)
        {
            GlobalLua.Dispose();
            GlobalLua = null;
            loop = null;
            lualoader = null;
        }
    }
}
