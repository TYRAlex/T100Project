using UnityEngine;
using System.Collections;
using System.IO;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;
using System.Collections.Concurrent;
using UnityEngine.UI;
using System;
using System.Text;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;
using ILRuntime.Mono.Cecil.Pdb;
using OneID;
using Thirds.NewWayToPlayAudio.Encoder;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine.Networking;
using UnityEngine.Serialization;

namespace ILFramework
{
    public class HotfixManager : Manager<HotfixManager>
    {

        public static Dictionary<string, HotfixPackage> hotfixDict = new Dictionary<string, HotfixPackage>();
        public HotfixPackage curShowPackage = null;

        public Slider progress;
        public Text progressTxt;


        public float LineSpeed;
        public float LineStartPos;
        public float LinesEndPos;

        public RectTransform Line1Rect;
        public RectTransform Line2Rect;

        public GameObject LoadingPanel;
        public GameObject WaterMark;
        public GameObject RessLossMark;

        //float p;
        bool binit;
        //UnityWebRequest abRequest;
        AssetBundleRequest abRequest;
        UnityWebRequest ws;
        int _totalPackage;
        public int totalPackage { set { _totalPackage = value; totalPro = 4 * value; } get { return _totalPackage; } }
        public int downloadPackage = 0;
        float totalPro, curPro;

        [HideInInspector]
        public bool isCommonLoaded = false;
        [HideInInspector]
        public bool isStartLoadCommon = false;

        private WaitForEndOfFrame waitForEndOfFrame;

        [HideInInspector]
        public string AndroidCurrentPath = string.Empty;

        public string CourseName = string.Empty;
        
        private StringBuilder _progressStrBuilder=new StringBuilder("已加载0%");
        private string _lastValue="0";
        [HideInInspector]
        public string CurrentDynamicPath=String.Empty;
        

        private void FixedUpdate()
        {
            LineMove(Line1Rect);
            LineMove(Line2Rect);

            if (abRequest != null)
            {
                progress.value = ((abRequest.progress == 1 ? 0 : abRequest.progress) / totalPro) + (curPro / totalPro);
                if (abRequest.progress <= 0 || float.IsNaN(progress.value))
                    progress.value = 0;
                _progressStrBuilder.Replace(_lastValue, (progress.value * 100).ToString("#0"));
                
                // if (_progressStrBuilder.ToString().Equals("NaN"))
                // {
                //     Debug.LogError("匹配到特殊字符");
                //     _progressStrBuilder.Replace("NaN", "0");
                // }

                progressTxt.text = _progressStrBuilder.ToString();
                _lastValue = (progress.value * 100).ToString("#0");
                //progressTxt.text = "已加载" + (progress.value * 100).ToString("#0") + "%";
                //Debug.LogFormat(" progress : {0} , curPro : {1} , totalPro: {2} , abrp: {3} , ddd: {4}", progress.value, curPro, totalPro, abRequest.progress, (curPro / totalPro));
            }
            else
            {
                binit = false;
                curPro = 0;
                downloadPackage = 0;
            }

#if UNITY_EDITOR
            if (Input.GetKeyDown(KeyCode.KeypadPlus))
            {
                Time.timeScale *= 1.5f;
            }
            else if (Input.GetKeyDown(KeyCode.KeypadMinus))
            {
                Time.timeScale /= 1.5f;
                if (Time.timeScale <= 1)
                {
                    Time.timeScale = 1;
                }
            }
#endif
            
        }

        void ResetProgress()
        {
            progress.value = 0;
            progressTxt.text = "";
        }

        void ResetLoading(bool isShow)
        {
            LoadingPanel.SetActive(isShow);
        }


        private void LineMove(RectTransform lineRect)
        {
            if (lineRect.localPosition.x <= LinesEndPos)
                lineRect.localPosition = new Vector3(LineStartPos, lineRect.localPosition.y, 0);

            lineRect.Translate(Vector3.left * LineSpeed * Time.deltaTime);
        }

        

        void Start()
        {
            isCommonLoaded = false;
            isStartLoadCommon = false;
            ResetProgress();
            waitForEndOfFrame = new WaitForEndOfFrame();
            RessLossMark.Hide();
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
             WaterMark.Show();
#endif

#if !UNITY_EDITOR
            StartCoroutine(LoadCommonAssets());
#endif
        }

       

        IEnumerator LoadCommonAssets()
        {
            //Debug.LogError("加载了通用一次~");
            isStartLoadCommon = true;
            AssetBundleCreateRequest commonws1 = null;
            UnityWebRequest commonws = null;
     
            // 加载通用资源AB
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            
            var commonws = UnityWebRequestAssetBundle.GetAssetBundle(Application.streamingAssetsPath + "/" + AppConst.common + "/windows/" + AppConst.commonAB_Audio.ToLower());
#elif !UNITY_EDITOR && UNITY_STANDALONE_OSX
           
            var commonws = UnityWebRequestAssetBundle.GetAssetBundle("file://"+Application.streamingAssetsPath + "/" + AppConst.common + "/osx/" + AppConst.commonAB_Audio.ToLower());
#elif UNITY_ANDROID && !UNITY_EDITOR

            string currentPath =
                AppConst.hotfix_dir.Replace("ai.bell.BeBOCourseDefaultPlayer", "bell.ai.bebo.launcher");
             currentPath = currentPath.Replace("file://", "");
            commonws1 = AssetBundle.LoadFromFileAsync(currentPath + "/" + AppConst.common +
                                                      AppConst.hotfix_platform + AppConst.commonAB_Audio.ToLower());

            //commonws = UnityWebRequestAssetBundle.GetAssetBundle(currentPath + AppConst.hotfix_platform +AppConst.common+"/"+ AppConst.commonAB_Audio.ToLower());
#else
            // commonws1 = AssetBundle.LoadFromFileAsync(AppConst.hotfix_dir + "/" + AppConst.common +
            //                                           AppConst.hotfix_platform + AppConst.commonAB_Audio.ToLower());
            commonws = UnityWebRequestAssetBundle.GetAssetBundle(AppConst.hotfix_dir + "/" + AppConst.common + AppConst.hotfix_platform + AppConst.commonAB_Audio.ToLower());
#endif
 #if UNITY_ANDROID && !UNITY_EDITOR
             if (!commonws1.isDone)
             {
                 //Debug.Log(commonws1.progress);
                 yield return new WaitForEndOfFrame();
             }

             commonws1.completed += (asyn) =>
             {
                 if (!asyn.isDone)
                 {
                     Debug.LogErrorFormat(" 加载场景资源 {0} 失败! ", AppConst.hotfix_dir + "/" + AppConst.common +
                                                              AppConst.hotfix_platform + AppConst.commonAB_Audio.ToLower());
                     return;
                 }
             
                 Debug.Log(" 通用音频prefab加载成功! "+ AppConst.hotfix_dir + "/" + AppConst.common +
                                                          AppConst.hotfix_platform + AppConst.commonAB_Audio.ToLower());
                 
                 AssetBundle audioAB = commonws1.assetBundle;
                 
                 GameObject go;
                 AudioClip[] clips;
                 
                 for (int i = 3; i < 6; i++)
                 {
                     string cname = SoundManager.instance.audiosPrefabName[i];
                     //go = audioAB.LoadAsset<GameObject>(cname + AppConst.abExt);
                     AssetBundleRequest abRequest1 = audioAB.LoadAssetAsync<GameObject>(cname + AppConst.abExt);
                     //abRequest = abRequest1;
                     abRequest1.completed += (asyn1) =>
                     {
                         if (!asyn1.isDone)
                         {
                             Debug.LogErrorFormat(" 加载场景资源 {0} 失败! ", cname + AppConst.abExt);
                             return;
                         }

                         
                         go=abRequest1.asset as GameObject;
                         clips = go.GetComponent<BellAudiosClip>().clips;
                        
                         //Debug.LogError("Clips:"+go.name);
                         // for (int j = 0; j < clips.Length; j++)
                         // {
                         //     print(clips[j].name);
                         // }
                         int currentIndex = 0;
                         if (go.name.Equals("CommonRes_Bgm_htp"))
                             currentIndex = 3;
                         else if (go.name.Equals("CommonRes_Sound_htp"))
                             currentIndex = 4;
                         else if (go.name.Equals("CommonRes_Voice_htp"))
                             currentIndex = 5;
                         if (!SoundManager.instance.commonClips.ContainsKey(SoundManager.instance.soundTypes[currentIndex]))
                         {
                             SoundManager.instance.commonClips.Add(SoundManager.instance.soundTypes[currentIndex], clips);
                             //Debug.LogError("添加成功！："+SoundManager.instance.commonClips.Count);
                         }
                         if (GetCurrenAndroidVersion.GetVersionInt() >= 29)
                         {
                             for (int j = 0; j < clips.Length; j++)
                             {
                                 AudioClip targetClip = clips[j];
                                 LoadAudioclipToMp3AndDownloadToLoal(SoundManager.instance.soundTypes[currentIndex], targetClip,
                                     j.ToString());
                             }
                             for (int j = 0; j < clips.Length; j++)
                             {
                                 AndroidNativeAudioMgr.LoadLongAudio(j.ToString(), SoundManager.instance.soundTypes[currentIndex]);
                             }
                         }
                         curPro++;
                     };
                 }

                 abRequest = audioAB.LoadAssetAsync<GameObject>("Common1_Audios_htp");
                 abRequest.completed += (asyn2) =>
                 {
                     if (!asyn2.isDone)
                     {
                         Debug.LogErrorFormat(" 加载场景资源 {0} 失败! ", "Common1_Audios_htp");
                         return;
                     }
                     go=abRequest.asset as GameObject;
                     clips = go.GetComponent<BellAudiosClip>().clips;
                     for (int i = 0; i < clips.Length; i++)
                     {
                         AudioClip targetClip = clips[i];
                         LoadBeBO1CommonClip(targetClip, i.ToString());
                     }
                     Debug.LogError("所有BeBO1资源下载成功，现在开始加载");
                     // for (int i = 0; i < clips.Length; i++)
                     // {
                     //     AndroidNativeAudioMgr.LoadBeBO1CommonClip(i.ToString());
                     // }
                     // Debug.LogError("所有BeBO1资源加载成功");
                     SoundManager.instance.bebo1_commonClips = clips;
                     curPro++;
                     audioAB.Unload(false);
                 };
             
                 // 加载1.0通用音频资源
                 // go = audioAB.LoadAsset<GameObject>("Common1_Audios_htp");
                 // clips = go.GetComponent<BellAudiosClip>().clips;
                 // SoundManager.instance.bebo1_commonClips = clips;
                 // audioAB.Unload(false);
             
             };
 #else

            yield return commonws.SendWebRequest();
            if (commonws.isNetworkError || commonws.isHttpError)
                Debug.LogErrorFormat(" 初始化通用音频失败：{0}, wspath: {1} ", commonws.error, commonws.url);
            else
            {
                //Debug.Log(" 通用音频prefab加载成功! ");
                AssetBundle audioAB = DownloadHandlerAssetBundle.GetContent(commonws);
                GameObject go;
                AudioClip[] clips;
                for (int i = 3; i < 6; i++)
                {
                    string cname = SoundManager.instance.audiosPrefabName[i];
                    go = audioAB.LoadAsset<GameObject>(cname + AppConst.abExt);
                    clips = go.GetComponent<BellAudiosClip>().clips;
                    if (!SoundManager.instance.commonClips.ContainsKey(SoundManager.instance.soundTypes[i]))
                    {
                        SoundManager.instance.commonClips.Add(SoundManager.instance.soundTypes[i], clips);
                    }
                }
            
                // 加载1.0通用音频资源
                go = audioAB.LoadAsset<GameObject>("Common1_Audios_htp");
                clips = go.GetComponent<BellAudiosClip>().clips;
                SoundManager.instance.bebo1_commonClips = clips;
                audioAB.Unload(false);
                commonws.Dispose();
                commonws = null;
            }
 #endif


#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
            Debug.LogFormat(" 初始化公共动态加载资源：{0} ", Application.streamingAssetsPath + "/" + AppConst.common + "/windows/" + AppConst.commonDynamic.ToLower());
            commonws = UnityWebRequestAssetBundle.GetAssetBundle(Application.streamingAssetsPath + "/" + AppConst.common + "/windows/" + AppConst.commonDynamic.ToLower());
#elif !UNITY_EDITOR && UNITY_STANDALONE_OSX
            commonws = UnityWebRequestAssetBundle.GetAssetBundle("file://"+Application.streamingAssetsPath + "/" + AppConst.common + "/osx/" + AppConst.commonDynamic.ToLower());
#elif UNITY_ANDROID && !UNITY_EDITOR
            //commonws = UnityWebRequestAssetBundle.GetAssetBundle(currentPath + AppConst.hotfix_platform + AppConst.commonDynamic.ToLower());
            var commonws2 = AssetBundle.LoadFromFileAsync(currentPath + "/" + AppConst.common +
                                                         AppConst.hotfix_platform + AppConst.commonDynamic.ToLower());
#else
            commonws = UnityWebRequestAssetBundle.GetAssetBundle(AppConst.hotfix_dir + "/" + AppConst.common + AppConst.hotfix_platform + AppConst.commonDynamic.ToLower());
#endif

#if UNITY_ANDROID && !UNITY_EDITOR
            commonws2.completed += (asyn) =>
            {
                if (!asyn.isDone)
                {
                    Debug.LogErrorFormat(" 加载场景资源 {0} 失败! ", AppConst.hotfix_dir + "/" + AppConst.common +
                                                             AppConst.hotfix_platform + AppConst.commonDynamic.ToLower());
                    return;
                }
            
                Debug.Log(" 通用公共动态加载资源prefab成功! ");
                ResourceManager.instance.commonResList = commonws2.assetBundle;
                // string[] commonlist = ResourceManager.instance.commonResList.GetAllAssetNames();
                // for (int i = 0; i < commonlist.Length; i++)
                // {
                //     Debug.LogError("已经加载的动态资源名称为："+commonlist[i]);
                // }
                // string[] commonPathlist = ResourceManager.instance.commonResList.GetAllScenePaths();
                // for (int i = 0; i < commonPathlist.Length; i++)
                // {
                //     Debug.LogError("已经加载的动态资源名称为："+commonPathlist[i]);
                // }
                commonws2=null;
                
            
            };
            //isCommonLoaded = true;
            
#endif
            Debug.LogError("ssss");
            if (commonws != null)
            {
                yield return commonws.SendWebRequest();
                if (commonws.isNetworkError || commonws.isHttpError)
                    Debug.LogErrorFormat(" 初始化公共动态加载资源失败：{0} ", commonws.error);
                else
                {
                    ResourceManager.instance.commonResList = DownloadHandlerAssetBundle.GetContent(commonws);
                    //Debug.LogError("初始化公共动态资源加载成功！"+ResourceManager.instance.commonResList);
                    commonws.Dispose();
                    commonws = null;
                }
            }

            
// #endif
            
            isCommonLoaded = true;
        }

        public void InstanceHotfixPackage(string name, Action<HotfixPackage> instancedCallback,string path=null)
        {
            HotfixPackage hotfixPackage = new HotfixPackage();
            hotfixPackage.Name = name;
            hotfixPackage.Path = path;
            AndroidCurrentPath = path;
            CourseName = name;
            if (!binit)
            {
                binit = true;
                ResetProgress();
                ResetLoading(true);
            }
            DoInstanceDllAndPrefab(hotfixPackage, instancedCallback);
        }

        void DoInstanceDllAndPrefab(HotfixPackage package, Action<HotfixPackage> callback)
        {
            Caching.ClearCache();
//#if !UNITY_EDITOR
            if (!isStartLoadCommon && !isCommonLoaded && SoundManager.instance.commonClips.Count == 0)
            {
                StartCoroutine(LoadCommonAssets());
            }
//#endif 
            //// 加载资源
 #if UNITY_EDITOR
             StartCoroutine(LoadAudioInEditor(package, callback)) ;
 #else
            StartCoroutine(LoadCourseAudioIE(package, callback));
#endif


        }

        private IEnumerator LoadAudioInEditor(HotfixPackage package, Action<HotfixPackage> callback)
        {
            yield return null;
#if UNITY_EDITOR

            
            yield return new WaitUntil(() => isCommonLoaded);
            
            Debug.Log("进入编辑模式下加载的状态");
            Dictionary<SoundManager.SoundType, AudioClip[]> dic=new Dictionary<SoundManager.SoundType, AudioClip[]>();
            string packageName = package.Name.Replace("/", "");
            for (int i = 0; i < 3; i++)
            {
                string cname = packageName + "_" + SoundManager.instance.audiosPrefabName[i];
                // GameObject audioResource =
                //     Resources.Load<GameObject>("HotFixPackage/" + package.Name + "/AudiosPrefab/" + cname +
                //                                AppConst.abExt);
                GameObject audioResource = AssetDatabase.LoadAssetAtPath<GameObject>(
                    "Assets/HotFixPackage/" + package.Name + "/AudiosPrefab/" + cname +
                    AppConst.abExt+".prefab");
                
                Debug.Log("传入的路径为：HotFixPackage/" + package.Name + "/AudiosPrefab/" + cname +
                          AppConst.abExt);
                
                AudioClip[] clips = audioResource.GetComponent<BellAudiosClip>().clips;
                dic.Add(SoundManager.instance.soundTypes[i], clips);
            }

            if (SoundManager.instance.audioClips.ContainsKey(packageName))
                SoundManager.instance.audioClips.Add(packageName, dic);
            else
            {
                SoundManager.instance.audioClips[packageName] = dic;
            }
            //Debug.Log("11111111111111111111111");
            LoadCoursePartInEditor(package,callback);
#endif
        }

        private void LoadCoursePartInEditor(HotfixPackage package, Action<HotfixPackage> callback)
        {
#if UNITY_EDITOR
            string packageName = package.Name.Replace("/", "");
            // GameObject ins = Instantiate(Resources.Load<GameObject>("HotFixPackage/" + package.Name+"/"+
            //                                                         packageName +
            //                                                         AppConst.abExt));
            GameObject ins = Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(
                "Assets/HotFixPackage/" + package.Name + "/" +
                packageName +
                AppConst.abExt+".prefab"));
            Debug.LogFormat(" 加载场景 {0} 成功 ", packageName);
            ins.Hide();
            ins.name = packageName;
            package.RootObject = ins;
            ins.transform.parent = null;
            ins.transform.localPosition=Vector3.zero;
            ins.transform.localEulerAngles=Vector3.zero;
            LoadingPanel.SetActive(false);
            if (GameManager.instance.IsPlayingOneIDCourse)
                OneIDSceneManager.Instance.AddGameSceneObject(ins);
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX 
                            WaterMark.transform.SetParent(ins.transform);
                            WaterMark.transform.localScale = Vector3.one;
                            WaterMark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                            WaterMark.transform.SetAsLastSibling();
#endif
            if (hotfixDict.ContainsKey(packageName))
                hotfixDict[packageName] = package;
            else
                hotfixDict.Add(packageName, package);
            callback?.Invoke(package);
#endif
        }

        public void LoadCourseDll(HotfixPackage package, Action call)
        {
            StartCoroutine(LoadDll(package, call));
        }

        IEnumerator LoadDll(HotfixPackage package, Action call)
        {
            string packageName = package.Name.Replace("/", "");
            AppDomain appdomain = new AppDomain();
            Debug.LogFormat("Read dll: {0}", package.Name);
#if UNITY_ANDROID && !UNITY_EDITOR
            ws = UnityWebRequest.Get("file://" + package.Path + "/" + package.Name + "/" + packageName + ".dll");
#else
            ws = UnityWebRequest.Get(AppConst.hotfix_dir + "/" + package.Name + "/" + packageName + ".dll");
#endif
            
            yield return ws.SendWebRequest();
            if (ws.isHttpError || ws.isNetworkError)
                Debug.LogError(ws.error);

            byte[] dll = ws.downloadHandler.data;
            ws.Dispose();
#if UNITY_ANDROID && UNITY_EDITOR || UNITY_EDITOR_WIN
            Debug.Log("Read pdb");
            ws = UnityWebRequest.Get(AppConst.hotfix_dir + "/" + package.Name + "/" + packageName + ".pdb");
            yield return ws.SendWebRequest();
            if (ws.isHttpError || ws.isNetworkError)
                Debug.LogError(ws.error);

            byte[] pdb = ws.downloadHandler.data; //www.bytes;
            ws.Dispose();
#endif
            package.Fs = new MemoryStream(dll);
#if UNITY_ANDROID && UNITY_EDITOR || UNITY_EDITOR_WIN
            package.P = new MemoryStream(pdb);
#endif

#if UNITY_ANDROID && !UNITY_EDITOR || UNITY_STANDALONE_WIN && !UNITY_EDITOR || UNITY_STANDALONE_OSX && !UNITY_EDITOR
            appdomain.LoadAssembly(package.Fs);
#elif UNITY_ANDROID && UNITY_EDITOR || UNITY_EDITOR_WIN || UNITY_EDITOR_OSX
            appdomain.LoadAssembly(package.Fs, package.P, new PdbReaderProvider());
#endif
            RootILBehaviour ib = package.RootObject.GetComponent<RootILBehaviour>();
            if (ib == null)
                ib = package.RootObject.AddComponent<RootILBehaviour>();
            package.IlBehaviorIns = ib;
            package.PackageDomain = appdomain;
            ib.SetHotfixPackage(package);
            ExtraRegister.RegisterDelegate(appdomain);
            Debug.LogFormat(" Load DLL over");
            call?.Invoke();
        }

        IEnumerator LoadDynamicResource(HotfixPackage package)
        {
            while (!isCommonLoaded)
            {
                yield return null;
            }
            string packageName = package.Name.Replace("/", "");
            //添加动态加载资源
            string dynamicPath = AppConst.hotfix_dir + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp";
            Debug.LogFormat(" ------ LoadDynamicResource ------ path: {0} ", dynamicPath);
            UnityWebRequest dynamicws = UnityWebRequest.Get(dynamicPath); //UnityWebRequestAssetBundle.GetAssetBundle(dynamicPath);
            //UnityWebRequest dynamicws = UnityWebRequestAssetBundle.GetAssetBundle(dynamicPath);
            yield return dynamicws.SendWebRequest();
            if (dynamicws.isNetworkError || dynamicws.isHttpError)
                Debug.LogFormat(" 初始化动态加载资源失败：{0} ", dynamicws.error);
            else
            {
                Debug.Log(" 动态加载资源成功 ！ ");
#if UNITY_ANDROID && !UNITY_EDITOR
                WriteDncryptionDataToFile(DncryptionAssetBundle(dynamicws.downloadHandler.data), Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp");
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp"+"1");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromMemoryAsync(DncryptionAssetBundle(dynamicws.downloadHandler.data));
#else

                WriteDncryptionDataToFile(DncryptionAssetBundle(dynamicws.downloadHandler.data), dynamicPath);
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(dynamicPath + "1");
#endif

                while (!abCreatRequest.isDone)
                {
                    yield return null;
                }
                ResourceManager.instance.AddResourceAB(package, abCreatRequest.assetBundle);
            }
            dynamicws.Dispose();

        }

        IEnumerator LoadCourseAudioIE(HotfixPackage package, Action<HotfixPackage> callback)
        {

            while (!isCommonLoaded)
            {
                yield return null;
            }
            string packageName = package.Name.Replace("/", "");
            CurrentDynamicPath = package.Path;
            //添加动态加载资源
#if UNITY_ANDROID && !UNITY_EDITOR
            string dynamicPath = "file://" + package.Path + "/" + package.Name + AppConst.hotfix_platform +
                                 packageName.ToLower() + "dynamic_htp";
#else
            string dynamicPath = AppConst.hotfix_dir + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp";
#endif
            
            
            //Debug.LogFormat(" ------ LoadDynamicResource ------ path: {0} ", dynamicPath);
            UnityWebRequest dynamicws = UnityWebRequest.Get(dynamicPath); //UnityWebRequestAssetBundle.GetAssetBundle(dynamicPath);
            //UnityWebRequest dynamicws = UnityWebRequestAssetBundle.GetAssetBundle(dynamicPath);
            yield return dynamicws.SendWebRequest();
            if (dynamicws.isNetworkError || dynamicws.isHttpError)
                Debug.LogFormat(" 初始化动态加载资源失败：{0} ", dynamicws.error);
            else
            {
                yield return waitForEndOfFrame;
#if UNITY_ANDROID && !UNITY_EDITOR
                WriteDncryptionDataToFile(DncryptionAssetBundle(dynamicws.downloadHandler.data),
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                    "dynamic_htp");
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                    "dynamic_htp" + "1");
                // WriteDncryptionDataToFile(DncryptionAssetBundle(dynamicws.downloadHandler.data), Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp");
                // AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "dynamic_htp"+"1");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromMemoryAsync(DncryptionAssetBundle(dynamicws.downloadHandler.data));
#else

                WriteDncryptionDataToFile(DncryptionAssetBundle(dynamicws.downloadHandler.data), dynamicPath);
                 //DownloadHandler downloadHandlerBuffer = new DownloadHandlerBuffer();
                // downloadHandlerBuffer = dynamicws.downloadHandler;
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(dynamicPath + "1");
#endif
                while (!abCreatRequest.isDone)
                {
                    yield return null;
                }
                Debug.Log(" 动态资源AB包加载成功 ！ ");
                ResourceManager.instance.AddResourceAB(package, abCreatRequest.assetBundle);
            }
            dynamicws.Dispose();

            // 加载音频prefab
#if UNITY_ANDROID && !UNITY_EDITOR
            string clipPath = "file://" + package.Path + "/" + package.Name + AppConst.hotfix_platform +
                              packageName.ToLower() + "audio_htp";
#else
            string clipPath = AppConst.hotfix_dir + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "audio_htp";
#endif
                            
            //Debug.LogFormat(" ------ LoadAudioClip ------ path: {0} ", clipPath);
            UnityWebRequest audioWs = UnityWebRequest.Get(clipPath);
            yield return audioWs.SendWebRequest();
            if (audioWs.isNetworkError || audioWs.isHttpError)
            {
                Debug.LogErrorFormat(" 初始化音频文件prefab失败：{0} ,路径为{1}", audioWs.error, clipPath);
            }
            else
            {
                Debug.Log(" 加载音频prefab成功 ！ ");
                var dic = new Dictionary<SoundManager.SoundType, AudioClip[]>();
                yield return waitForEndOfFrame;
#if UNITY_ANDROID && !UNITY_EDITOR          
                WriteDncryptionDataToFile(DncryptionAssetBundle(audioWs.downloadHandler.data),
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "audio_htp");
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                    "audio_htp1");
                // WriteDncryptionDataToFile(DncryptionAssetBundle(audioWs.downloadHandler.data), Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "audio_htp");
                // AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "audio_htp"+"1");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromMemoryAsync(DncryptionAssetBundle(audioWs.downloadHandler.data));          
#else 
                WriteDncryptionDataToFile(DncryptionAssetBundle(audioWs.downloadHandler.data), clipPath);
                AssetBundleCreateRequest abCreatRequest = AssetBundle.LoadFromFileAsync(clipPath + "1");
#endif

                while (!abCreatRequest.isDone)
                {
                    yield return null;
                }
#if UNITY_ANDROID && !UNITY_EDITOR
                File.Delete(package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "audio_htp"+"1");
#elif UNITY_EDITOR && !UNITY_EDITOR_OSX
                File.Delete(clipPath + "1");
#endif
                //Debug.Log(" 加载音频AB包成功 ！ ");
                ResourceManager.instance.AddAudioResAB(abCreatRequest.assetBundle);//将音频的AB包资源加入数组
                //Debug.Log("现在开始载入音频");
                LoadCourseAudio(package, callback, 0, audioWs, abCreatRequest.assetBundle, dic, packageName);
            }
        }

        void LoadCourseAudio(HotfixPackage package, Action<HotfixPackage> callback, int index, UnityWebRequest audioWs, AssetBundle audioAB, Dictionary<SoundManager.SoundType, AudioClip[]> dic, string packageName)
        {
            string cname = packageName + "_" + SoundManager.instance.audiosPrefabName[index];
            abRequest = audioAB.LoadAssetAsync<GameObject>(cname + AppConst.abExt);
            abRequest.completed += (asyn) =>
            {
                if (asyn.isDone)
                {
                    AudioClip[] clips;
                    GameObject go = abRequest.asset as GameObject;
                    clips = go.GetComponent<BellAudiosClip>().clips;
                    //Debug.LogFormat(" LoadCourseAudio soundType: {0} , clipsLen: {1} , curPackage: {2}", SoundManager.instance.soundTypes[index], clips.Length, packageName);
                    dic.Add(SoundManager.instance.soundTypes[index], clips);
                    
#if UNITY_ANDROID && !UNITY_EDITOR
                     Debug.LogError("开始加载所有音频");
                     if (GetCurrenAndroidVersion.GetVersionInt() >= 29)
                     {
                        for (int i = 0; i < clips.Length; i++)
                        {
                            AudioClip targetClip = clips[i];
                            LoadAudioclipToMp3AndDownloadToLoal(SoundManager.instance.soundTypes[index], targetClip,
                                i.ToString(),package);
                        }
                        for (int i = 0; i < clips.Length; i++)
                        {
                            AndroidNativeAudioMgr.LoadLongAudio(i.ToString(), SoundManager.instance.soundTypes[index]);
                        }
                     }
#endif
                    
                    curPro++;
                    if (index == 2)
                    {
                        //Debug.Log("载入音频完毕  开始载入场景");
                        //audioAB.Unload(false);
                        if (!SoundManager.instance.audioClips.ContainsKey(packageName))
                            SoundManager.instance.audioClips.Add(packageName, dic);
                        audioWs.Dispose();
                        audioWs = null;
                        StartCoroutine(LoadCoursePart(package, callback));
                    }
                    else
                        LoadCourseAudio(package, callback, index + 1, audioWs, audioAB, dic, packageName);
                }
            };
        }

        void LoadAudioclipToMp3AndDownloadToLoal(SoundManager.SoundType soundType, AudioClip clip,string indexName,HotfixPackage package=null)
        {
            
            // string path = Application.platform == RuntimePlatform.Android
            //     ? Application.persistentDataPath+ "/static/resource/Audio/"+package.Name+"/" + soundType.ToString() 
            //     : Application.dataPath + "/Audio/" + soundType.ToString();
            string path = string.Empty;
            switch (soundType)
            {
                case SoundManager.SoundType.BGM:
                case SoundManager.SoundType.SOUND:
                case SoundManager.SoundType.VOICE:
                    Debug.LogError("加载普通音频：");
                    path = Application.platform == RuntimePlatform.Android
                        ? package.Path+"/" + package.Name + "/Audio/" + soundType.ToString()
                        : Application.dataPath + "/Audio/" + soundType.ToString();
                    break;
                case SoundManager.SoundType.COMMONBGM:
                case SoundManager.SoundType.COMMONSOUND:
                case SoundManager.SoundType.COMMONVOICE:
                    // string currentPath = Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                    //     "bell.ai.bebo.launcher");
                    path = Application.platform == RuntimePlatform.Android
                        ? Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                            "bell.ai.bebo.launcher") + "/static/resource/Common/Audio/" + soundType.ToString()
                        : Application.dataPath + "/../OutHotfixAssetPackage/Audio" + soundType.ToString();
                   
                    break; 
            }
            
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            if (!File.Exists(path + "/" + indexName + ".mp3"))
            {
                Debug.LogError("加载音频："+path+ "/" + indexName + ".mp3");
                EncodeMP3.convert(clip, path+ "/" + indexName + ".mp3", 512);
            }
        }

        void LoadBeBO1CommonClip(AudioClip clip,string indexName)
        {
            string path = Application.platform == RuntimePlatform.Android
                ? Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                    "bell.ai.bebo.launcher") + "/static/resource/Common/Audio/BeBO1Audio" 
                : Application.dataPath + "/../OutHotfixAssetPackage/Audio/BeBO1Audio";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            
            if (!File.Exists(path + "/" + indexName + ".mp3"))
            {
                EncodeMP3.convert(clip, path+ "/" + indexName + ".mp3", 512);
            }
        }

        IEnumerator LoadCoursePart(HotfixPackage package, Action<HotfixPackage> callback)
        {
            Debug.Log("@-----------------package.Name:"+ package.Name);
            string packageName = package.Name.Replace("/", "");
#if UNITY_ANDROID && !UNITY_EDITOR
            string coursePath = "file://" + package.Path + "/" + package.Name + AppConst.hotfix_platform +
                                packageName.ToLower() + "_htp";
#else
            string coursePath = AppConst.hotfix_dir + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() + "_htp";
#endif
            
            UnityWebRequest courseWs = UnityWebRequest.Get(coursePath);
            yield return courseWs.SendWebRequest();
            if (courseWs.isHttpError || courseWs.isNetworkError)
                Debug.LogError(courseWs.error);
            else
            {
                yield return waitForEndOfFrame;
                //Debug.Log("开始加载课程资源");
                AssetBundleCreateRequest abCreatRequest = null;
#if UNITY_ANDROID && !UNITY_EDITOR
                WriteDncryptionDataToFile(DncryptionAssetBundle(courseWs.downloadHandler.data),
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                    "_htp");
                abCreatRequest = AssetBundle.LoadFromFileAsync(
                    package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                    "_htp" + "1");
                // WriteDncryptionDataToFile(DncryptionAssetBundle(courseWs.downloadHandler.data),
                //     Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                //     "_htp");
                // abCreatRequest = AssetBundle.LoadFromFileAsync(
                //     Application.persistentDataPath + "/static/resource" + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                //     "_htp"+"1");
#elif UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX
                    abCreatRequest = AssetBundle.LoadFromMemoryAsync((DncryptionAssetBundle(courseWs.downloadHandler.data)));                           
#else
                WriteDncryptionDataToFile(DncryptionAssetBundle(courseWs.downloadHandler.data),coursePath);
                yield return waitForEndOfFrame;
                abCreatRequest = AssetBundle.LoadFromFileAsync(coursePath + "1");
                //abCreatRequest = AssetBundle.LoadFromMemoryAsync((DncryptionAssetBundle(courseWs.downloadHandler.data)));
#endif

                if (!abCreatRequest.isDone)
                {
                    yield return null;
                }
                abCreatRequest.completed += (asyn2) =>
                    {

                        ResourceManager.instance.AddCourseResAB(abCreatRequest.assetBundle);//将课程资源的AB包加入数组，方便释放
                        abRequest = abCreatRequest.assetBundle.LoadAssetAsync<GameObject>(packageName + AppConst.abExt);

                        abRequest.completed += (asyn) =>
                        {
                            if (!asyn.isDone)
                            {
                                Debug.LogErrorFormat(" 加载场景资源 {0} 失败! ", packageName + AppConst.abExt);
                                return;
                            }

                            Debug.LogFormat(" 加载场景 {0} 成功 ", packageName);
                            GameObject ins = Instantiate(abRequest.asset as GameObject);
                            downloadPackage++;
                            curPro++;
                            ins.SetActive(false);
                            courseWs.Dispose();
                            courseWs = null;
                            if (downloadPackage == totalPackage)
                            {
                                //isCommonLoaded = false;
                                abRequest = null;
                                ResetProgress();
                                ResetLoading(false);
                            }

                            ins.name = packageName;
                            package.RootObject = ins;
                            ins.transform.parent = null;
                            ins.transform.localPosition = Vector3.zero;
                            //    ins.transform.rotation = Quaternion.identity;
                            ins.transform.localEulerAngles = Vector3.zero;
                            if (GameManager.instance.IsPlayingOneIDCourse)
                                OneIDSceneManager.Instance.AddGameSceneObject(ins);
#if UNITY_STANDALONE_WIN || UNITY_STANDALONE_OSX 
                            WaterMark.transform.SetParent(ins.transform);
                            WaterMark.transform.localScale = Vector3.one;
                            WaterMark.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, -80f);
                            WaterMark.transform.SetAsLastSibling();
#endif
                            if (hotfixDict.ContainsKey(packageName))
                                hotfixDict[packageName] = package;
                            else
                                hotfixDict.Add(packageName, package);
                            callback?.Invoke(package);

#if UNITY_ANDROID && !UNITY_EDITOR
                 File.Delete(package.Path + "/" + package.Name + AppConst.hotfix_platform + packageName.ToLower() +
                     "_htp"+"1");
#elif UNITY_EDITOR && !UNITY_EDITOR_OSX
                            File.Delete(AppConst.hotfix_dir + "/" + package.Name + AppConst.hotfix_platform +
                                        packageName.ToLower() +
                                        "_htp" + "1");
#endif
                        };
                    };

            }
        }



        public void ShowHotfixPackage(string packageName = "")
        {
            packageName = packageName.Replace("/", "");
            curShowPackage = null;
            foreach (var p in hotfixDict)
            {
                p.Value.RootObject.SetActive(p.Key == packageName);
                if (p.Key == packageName)
                {
                    curShowPackage = p.Value;
                    curShowPackage.IlBehaviorIns.OnShow();
                }
            }
        }

        public byte[] DncryptionAssetBundle(byte[] encryptionData)
        {
            int temLen = encryptionData.Length;
            //Debug.Log("@开始解密:" + temLen);
            for (int i = 0; i < temLen; i++)
            {
                encryptionData[i] -= 1;
            }
            return encryptionData;
        }

        public void WriteDncryptionDataToFile(byte[] data, string path)
        {
            //string targetPath = path + "/MyResource";
            // if (!Directory.Exists(targetPath))
            //     Directory.CreateDirectory(targetPath);
            try
            {
                File.WriteAllBytes(path + "1", data);
            }
            catch (Exception e)
            {
                Debug.LogError("写入失败，请检查！" + e);
            }
        }
    }
}