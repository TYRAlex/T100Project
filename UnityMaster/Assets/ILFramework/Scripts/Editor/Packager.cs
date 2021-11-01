using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEditor;
using UnityEngine;
using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Linq;

namespace ILFramework
{
    public class HotfixAssetPackager : EditorWindow
    {

        enum GUIState
        {
            AUTO_CREATE_HOTFIX_PACKAGE = 0,
            AB_BUILD,
            NEW_AUTO_CREATE,  // 创建新热更工程
            NEW_BUILD, // 打包
            ADD_NEWPART,  // 已有课程添加新环节
        };

        static HotfixAssetPackager window1;
        static HotfixAssetPackager window2;
        static bool[] select_bools;
        static string[] hot_asset_dirs_full;
        static string[] hot_asset_dirs_name;
        static GUIState CGUIState;

        //需要热更的资源请放在这个文件夹
        static string HotFixPackageDir = "Assets/HotFixPackage";

        //ab包和程序的dll包会出现在这里
        static string OutHotfixAssetPackage = "Assets/../OutHotfixAssetPackage";
        static string outHotfixAssetZipDir = "Assets/../OutHotfixAssetZip";

        static string HotFixCodePackageDir = "Assets/../HotfixCodeProject";
        static string AudioPrefabDir = "Assets/Editor/Template";
        static string dllPath = "Assets/Plugins/packageDll/";  // cs工程引用dll

        static List<AssetBundleBuild> prefabMaps = new List<AssetBundleBuild>();
        static List<AssetBundleBuild> dllMaps = new List<AssetBundleBuild>();


        //[MenuItem("HotfixAssetPackage/创建热更工程")]
        static void CreateCodeProject()
        {
            CGUIState = GUIState.AUTO_CREATE_HOTFIX_PACKAGE;

            Rect _rect = new Rect(0, 0, 400, 100);
            window1 = (HotfixAssetPackager)EditorWindow.GetWindowWithRect(typeof(HotfixAssetPackager), _rect, true);
            window1.Show();
        }

        //[MenuItem("HotfixAssetPackage/Build")]
        static void HotfixBuilder()
        {

          
            hot_asset_dirs_full = Directory.GetDirectories(HotFixPackageDir);
            if (hot_asset_dirs_full == null)
                return;

            hot_asset_dirs_name = new string[hot_asset_dirs_full.Length];
            for (int i = 0; i < hot_asset_dirs_full.Length; i++)
            {
#if UNITY_EDITOR_OSX
                string[] tmp = hot_asset_dirs_full[i].Split('/');
#elif UNITY_EDITOR && UNITY_ANDROID || UNITY_EDITOR_WIN
                string[] tmp = hot_asset_dirs_full[i].Split('\\');
#endif
                hot_asset_dirs_name[i] = tmp[tmp.Length - 1];
            }

            select_bools = new bool[hot_asset_dirs_name.Length];

            CGUIState = GUIState.AB_BUILD;

            prefabMaps.Clear();

            window2 = GetWindow<HotfixAssetPackager>();
            window2.Show();

        }


        private void OnGUI()
        {
            switch (CGUIState)
            {
                case GUIState.AB_BUILD: BuildABGUI(); break;
                //case GUIState.AUTO_CREATE_HOTFIX_PACKAGE: CreateHotfixProjectGUI(); break;
                case GUIState.NEW_AUTO_CREATE: CreateNewCourse(); break;
                case GUIState.NEW_BUILD: BuildABInspector(); break;
                case GUIState.ADD_NEWPART: AddNewPartInspector(); break;
                default: break;
            }
        }

        private void Awake()
        {
            parts = 1;
            text = "";
            isSecond = false;
        }

        /// <summary>
        /// 一键创建工程
        /// </summary>
        static string text;
        static void CreateHotfixProjectGUI()
        {
            //文本框
            text = EditorGUILayout.TextField("输入名字", text);


            if (GUILayout.Button("创建工程"))
            {
                DoCreateHotfixProject();
                window1.Close();
            }

        }


        static void DoCreateHotfixProject()
        {
            string name = text;
            if (!Directory.Exists(HotFixCodePackageDir + "/" + name) && !Directory.Exists(HotFixPackageDir + "/" + name))
            {
                Directory.CreateDirectory(HotFixCodePackageDir + "/" + name);
                Directory.CreateDirectory(HotFixPackageDir + "/" + name);
            }
            else
            {
                Debug.LogError("工程已存在，请删除后再创建");
                return;
            }

            CreatePrefabProject(name);
            CreatCodeProject(name);

            AssetDatabase.Refresh();
            EditorApplication.Beep();
        }

        public static void CreatePrefabProject(string name)
        {
            File.Copy(HotFixCodePackageDir + "/seed/seed_htp.prefab", HotFixPackageDir + "/" + name + "/" + name + "_htp.prefab");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Audios");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Audios/Bgm");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Audios/Sound");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Audios/Voice");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/AudiosPrefab");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Spines");
            Directory.CreateDirectory(HotFixPackageDir + "/" + name + "/Textures");

            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseBgm_htp.prefab", HotFixPackageDir + "/" + name + "/AudiosPrefab/" + name + "_Bgm_htp.prefab");
            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseSound_htp.prefab", HotFixPackageDir + "/" + name + "/AudiosPrefab/" + name + "_Sound_htp.prefab");
            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseVoice_htp.prefab", HotFixPackageDir + "/" + name + "/AudiosPrefab/" + name + "_Voice_htp.prefab");

        }



        public static void CreatCodeProject(string name)
        {
            string seedPath = "";
            if (isTD)
            {
                seedPath = HotFixCodePackageDir + "/seed/Seed.cs";
            }
            else
            {
                seedPath = HotFixCodePackageDir + "/seed/RobotSeed.cs";
            }
            string content = File.ReadAllText(seedPath);

            content = content.Replace(isTD ? "Seed" : "RobotSeed", name.Replace("/", ""));

            string str = HotFixCodePackageDir + "/" + name + "/" + name.Replace("/", "") + ".cs";

            File.WriteAllText(str, content);
            CreateCsProject(name);
        }

        public static void CreateCsProject(string hotfix_project_name)
        {
            string unityPath = EditorApplication.applicationContentsPath;
            string managedPath = "/Managed/UnityEngine/";
            XmlDocument xml = new XmlDocument();
            string seedPath = "";
            if (isTD)
            {
                seedPath = "Assets/../HotfixCodeProject/seed/seed.csproj";
            }
            else
            {
                seedPath = "Assets/../HotfixCodeProject/seed/RobotSeed.csproj";
            }

            xml.Load(seedPath);
            XmlElement xmlRoot = xml.DocumentElement; //DocumentElement获取文档的跟
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Encoding = new UTF8Encoding(false);
            settings.Indent = true;

            string str = (CGUIState == GUIState.NEW_AUTO_CREATE || CGUIState == GUIState.ADD_NEWPART) ? hotfix_project_name.Replace("/", "") : hotfix_project_name;
            string outStr = hotfix_project_name.Replace("/", "\\");
            XmlWriter xw = XmlWriter.Create("Assets /../HotfixCodeProject/" + hotfix_project_name + "/" + str + ".csproj", settings);
            hotfix_project_name = hotfix_project_name.Replace("/", "");

            foreach (XmlNode node3 in xmlRoot.ChildNodes)
            {
                //Debug.Log(node3.Name);

                if (node3.HasChildNodes)
                {
                    foreach (XmlNode node4 in node3.ChildNodes)
                    {
                        //Debug.Log(node4.Name);

                        if (node4.Name == "RootNamespace")
                        {
                            SetXmlNodeInnerText(node4, hotfix_project_name);

                        }
                        else if (node4.Name.Equals("AssemblyName"))
                            node4.InnerText = hotfix_project_name;
                        else if (node4.Name.Equals("OutputPath"))
                        {
                            string outPath = (CGUIState == GUIState.NEW_AUTO_CREATE || CGUIState == GUIState.ADD_NEWPART) ? "..\\..\\..\\OutHotfixAssetPackage\\" + outStr : "..\\..\\OutHotfixAssetPackage\\" + outStr;
                            //Debug.Log(outPath);
                            node4.InnerText = outPath;
                        }

                        // 添加相关引用
                        else if (node4.Name.Equals("Reference"))
                        {
                            string attributes = node4.Attributes[0].Value;
                            string preStr = (CGUIState == GUIState.NEW_AUTO_CREATE || CGUIState == GUIState.ADD_NEWPART) ? "..\\..\\..\\" : "..\\..\\";
                            switch (attributes)
                            {
                                case "Assembly-CSharp":
                                    node4.FirstChild.InnerText = preStr + "Library\\ScriptAssemblies\\Assembly-CSharp.dll";
                                    break;
                                case "UnityEngine.CoreModule":
                                    node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.CoreModule.dll"; //preStr + dllPath + "/UnityEngine.CoreModule.dll";
                                    break;
                                case "UnityEngine.Physics2DModule":
                                    node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.Physics2DModule.dll"; //preStr + dllPath + "/UnityEngine.Physics2DModule.dll";
                                    break;
                                case "UnityEngine.PhysicsModule":
                                    node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.PhysicsModule.dll"; //preStr + dllPath + "/UnityEngine.PhysicsModule.dll";
                                    break;
                                case "UnityEngine":
                                    node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.dll"; //preStr + dllPath + "/UnityEngine.dll";
                                    break;
                                case "UnityEngine.UI":
                                    node4.FirstChild.InnerText = unityPath + "/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll"; //preStr + dllPath + "/UnityEngine.UI.dll";
                                    break;
                                case "DOTween":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTween/DOTween.dll";
                                    break;
                                case "DOTweenPro":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTweenPro/DOTweenPro.dll";
                                    break;
                                case "DOTween46":
                                    node4.FirstChild.InnerText = preStr + "Assets/Thirds/Demigiant/DOTween/DOTween46.dll";
                                    break;
                                case "UnityEngine.AudioModule":
                                    node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.AudioModule.dll"; //preStr + dllPath + "/UnityEngine.AudioModule.dll";
                                    break;
                                default:
                                    break;
                            }
                        }
                        else if (node4.Name.Equals("Compile"))
                        {
                            string csName = hotfix_project_name + ".cs";
                            node4.Attributes[0].Value = csName;
                        }

                    }

                }

            }

            xml.Save(xw);
            xw.Flush();
            xw.Close();
        }


        public static void SetXmlNodeInnerText(XmlNode node, string value)
        {
            XmlElement tmp = (XmlElement)node;
            tmp.InnerText = value;

        }



        /// <summary>
        /// build工程
        /// </summary>
        static void BuildABGUI()
        {
            GUILayout.Label("Packer Asset", EditorStyles.boldLabel);

            EditorGUILayout.BeginVertical();
            for (int i = 0; i < select_bools.Length; i++)
            {
                if (i % 5 == 0) EditorGUILayout.BeginHorizontal();
                var option = new[] { GUILayout.Width(150), GUILayout.MinWidth(120) };
                select_bools[i] = GUILayout.Toggle(select_bools[i], hot_asset_dirs_name[i], option);
                if (i % 5 == 4) EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            if (GUILayout.Button("make assetbundle")) DoOutput(select_bools, hot_asset_dirs_name);
        }

        static void DoOutput(bool[] select_bools, string[] hot_asset_dirs_name)
        {
            window2.Close();
            Debug.Log("make assetbundle");

            if (!Directory.Exists(OutHotfixAssetPackage))
                Directory.CreateDirectory(OutHotfixAssetPackage);

            AssetDatabase.Refresh();

            string AssetPath = "Assets/HotfixPackage/";
            for (int i = 0; i < select_bools.Length; i++)
            {
                prefabMaps.Clear();
                if (select_bools[i])
                {
                    // 场景prefab
                    var path = AssetPath + hot_asset_dirs_name[i] + "/";
                    string courseName = hot_asset_dirs_name[i].Replace("/", "");
                    Debug.LogWarning(path);
                    AddBuildMap(courseName + "_htp", "*.prefab", path, ref prefabMaps);

                    //音频预制
                    string audioPath = path + "AudiosPrefab";
                    AddBuildMap(courseName + "Audio_htp", "*.prefab", audioPath, ref prefabMaps);

                    // 动态加载资源
                    string dynamicPath = path + "LoadResource";
                    if (Directory.Exists(dynamicPath))
                        AddBuildMap(courseName + "dynamic_htp", new string[] { "*.prefab", "*.png", "*.jpg" }, dynamicPath, ref prefabMaps);

                    //兼容1.0课程 打包lua代码
                    string courseLuaPath = path + "CourseLua";
                    string tempDir = "Assets/Templua/";
                    if (Directory.Exists(courseLuaPath))
                    {
                        string temp = tempDir + hot_asset_dirs_name[i];
                        if (!Directory.Exists(temp))
                            Directory.CreateDirectory(temp);
                        ToLuaMenu.CopyLuaBytesFiles(courseLuaPath, temp);
                        AssetDatabase.Refresh();
                        AddBuildMap(AppConst.bundleLua + courseName + "_htp", "*.bytes", temp, ref prefabMaps);
                    }

                    var output_path = OutHotfixAssetPackage + "/" + hot_asset_dirs_name[i];
#if UNITY_ANDROID
                    output_path = output_path + "/android";
                    if (!Directory.Exists(output_path))
                        Directory.CreateDirectory(output_path);
                    else
                        DeleteOldAssetBundle(Directory.GetFileSystemEntries(output_path));
                    BuildPipeline.BuildAssetBundles(output_path, prefabMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
                    BuildPipeline.BuildAssetBundles(output_path, prefabMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.Android);
#elif UNITY_STANDALONE_WIN
                    output_path = output_path + "/windows";
                    if (!Directory.Exists(output_path))
                        Directory.CreateDirectory(output_path);
                    else
                        DeleteOldAssetBundle(Directory.GetFileSystemEntries(output_path));
                    BuildPipeline.BuildAssetBundles(output_path, prefabMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneWindows);
#elif UNITY_STANDALONE_OSX
                    output_path = output_path + "/osx";
                    if (!Directory.Exists(output_path))
                        Directory.CreateDirectory(output_path);
                    else
                        DeleteOldAssetBundle(Directory.GetFileSystemEntries(output_path));
                    BuildPipeline.BuildAssetBundles(output_path, prefabMaps.ToArray(), BuildAssetBundleOptions.ChunkBasedCompression, BuildTarget.StandaloneOSX);
#endif
                    if (Directory.Exists(tempDir))
                        Directory.Delete(tempDir, true);
                    EncryptionAssetBundle(prefabMaps, output_path);
                }
            }

            AssetDatabase.Refresh();
            Debug.Log(" Make AB success! ");
        }

        public static void AddBuildMap(string bundleName, string[] pattern, string path, ref List<AssetBundleBuild> maps)
        {
            List<string> assets = new List<string>();
            for (int i = 0; i < pattern.Length; i++)
            {
                string[] files = Directory.GetFiles(path, pattern[i], SearchOption.AllDirectories);
                if (files.Length == 0)
                    continue;
                for (int j = 0; j < files.Length; j++)
                {
                    files[j] = files[j].Replace("\\", "/");
                    Debug.Log(files[j]);
                    assets.Add(files[j]);
                }
            }
            if (assets.Count == 0)
                return;
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = assets.ToArray();
            maps.Add(build);
        }

        public static void AddBuildMap(string bundleName, string pattern, string path, ref List<AssetBundleBuild> maps)
        {
            string[] files = Directory.GetFiles(path, pattern);
            if (files.Length == 0) return;

            for (int i = 0; i < files.Length; i++)
            {
                files[i] = files[i].Replace('\\', '/');
            }
            AssetBundleBuild build = new AssetBundleBuild();
            build.assetBundleName = bundleName;
            build.assetNames = files;
            foreach (var f in files)
            {
                Debug.Log(f);
            }
            maps.Add(build);
        }

        #region 新目录结构 按照 CourseXX --> Part1/Part2/... 格式
        ///////// 更改工程目录结构 ///////////
        static void CreateSlnProject(string courseName, int partNum)
        {
            string str;
            for (int i = 0; i < partNum; i++)
            {
                str = courseName + "/Part" + (i + 1);
                if (!Directory.Exists(HotFixCodePackageDir + "/" + str))
                    Directory.CreateDirectory(HotFixCodePackageDir + "/" + str);
                CreatCodeProject(str);
            }
        }

        static void CreateHotFixPackage(string courseName, int partNum)
        {
            string str;
            for (int i = 0; i < partNum; i++)
            {
                str = courseName + "/Part" + (i + 1);
                if (!Directory.Exists(HotFixPackageDir + "/" + str))
                    Directory.CreateDirectory(HotFixPackageDir + "/" + str);
                CreateCourseAssets(str, courseName + "Part" + (i + 1));
            }
        }

        static void CreateCourseAssets(string coursePath, string prefabName)
        {
            string seedPath = "";
            if (isTD)
            {
                seedPath = HotFixCodePackageDir + "/seed/seed_htp.prefab";
            }
            else
            {
                seedPath = HotFixCodePackageDir + "/seed/RobotSeed_htp.prefab";
            }
            File.Copy(seedPath, HotFixPackageDir + "/" + coursePath + "/" + prefabName + "_htp.prefab");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Audios");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Audios/Bgm");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Audios/Sound");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Audios/Voice");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/AudiosPrefab");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Spines");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/Textures");
            Directory.CreateDirectory(HotFixPackageDir + "/" + coursePath + "/LoadResource");

            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseBgm_htp.prefab", HotFixPackageDir + "/" + coursePath + "/AudiosPrefab/" + prefabName + "_Bgm_htp.prefab");
            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseSound_htp.prefab", HotFixPackageDir + "/" + coursePath + "/AudiosPrefab/" + prefabName + "_Sound_htp.prefab");
            File.Copy(AudioPrefabDir + "/AudiosPrefab/CourseVoice_htp.prefab", HotFixPackageDir + "/" + coursePath + "/AudiosPrefab/" + prefabName + "_Voice_htp.prefab");
        }

        [MenuItem("HotfixAssetPackage/CreateNewCourse(新)")]
        static void CreateNewCourseTool()
        {
            CGUIState = GUIState.NEW_AUTO_CREATE;
            window1 = GetWindow<HotfixAssetPackager>();
            window1.Show();
        }

        static int parts;
        static bool isTD;
        static void CreateNewCourse()
        {
            text = EditorGUILayout.TextField("CourseName:", text);
            parts = EditorGUILayout.IntField("CoursePart:", parts);

            isTD = EditorGUILayout.ToggleLeft("田丁课程", isTD);

            if (GUILayout.Button("Create New Course"))
            {
                if (parts < 0 || parts > 8)
                {
                    Debug.LogError(" 输入CoursePart有误！范围为 1-8 ");
                    return;
                }

                if (!Directory.Exists(HotFixCodePackageDir + "/" + text) && !Directory.Exists(HotFixPackageDir + "/" + text))
                {
                    Directory.CreateDirectory(HotFixCodePackageDir + "/" + text);
                    Directory.CreateDirectory(HotFixPackageDir + "/" + text);
                }
                else
                {
                    Debug.LogError("工程已存在，请删除后再创建");
                    return;
                }

                CreateHotFixPackage(text, parts);
                CreateSlnProject(text, parts);
                window1.Close();
                AssetDatabase.Refresh();
                EditorApplication.Beep();
            }
        }
        #endregion

        #region 新目录结构资源打包
        static bool[] selectCourse;
        static string[] allCourse;

        static bool[] selectParts;
        static Dictionary<int, string[]> allParts = new Dictionary<int, string[]>();

        static bool isSecond;
        static Vector2 v2;

        [MenuItem("HotfixAssetPackage/BuildAssetBundle(新)")]
        public static void NewBuildAB()
        {
            CGUIState = GUIState.NEW_BUILD;
            v2 = new Vector2(0, 0);
            string[] packages = Directory.GetDirectories(HotFixPackageDir);
            int nums = 0;
            List<string> courseList = new List<string>();
            
            for (int i = 0; i < packages.Length; i++)
            {
                // 如果查询到该目录下的第一个文件夹名称中含有Part 则认定为新课程资源
#if UNITY_EDITOR_OSX
                string[] tempStr = Directory.GetDirectories(packages[i])[0].Split('/');
#else
                string[] tempStr = Directory.GetDirectories(packages[i])[0].Split('\\');
#endif
                if (tempStr[tempStr.Length - 1].Contains("Part"))
                {
                    nums++;
#if UNITY_EDITOR_OSX
                    tempStr = packages[i].Split('/');
#else
                    tempStr = packages[i].Split('\\');
#endif
                    courseList.Add(tempStr[tempStr.Length - 1]);
                }
            }
            selectCourse = new bool[nums];
            //courseList.Sort((x, y) =>
            //{
            //    if (x.Contains("Course") && y.Contains("Course"))
            //    {
            //        var x1 = int.Parse(x.Substring(x.IndexOf("Course") + 6));
            //        var y1 = int.Parse(y.Substring(y.IndexOf("Course") + 6));
            //        return x1 - y1;
            //    }
            //    else return x.CompareTo(y);
            //});
            allCourse = courseList.ToArray();
            prefabMaps.Clear();

            window2 = GetWindow<HotfixAssetPackager>();
            window2.Show();
        }

        static void BuildABInspector()
        {
            GUILayout.Label("Packer Asset", EditorStyles.boldLabel);
            if (!isSecond)
            {
                v2 = GUILayout.BeginScrollView(v2);
                for (int i = 0; i < selectCourse.Length; i++)
                {

                    if (i % 5 == 0) EditorGUILayout.BeginHorizontal();
                    var option = new[] { GUILayout.Width(150), GUILayout.MinWidth(120) };
                    selectCourse[i] = GUILayout.Toggle(selectCourse[i], allCourse[i], option);
                    if (i % 5 == 4) EditorGUILayout.EndHorizontal();
                }
                if (selectCourse.Length % 5 != 0)
                    EditorGUILayout.EndHorizontal();
                GUILayout.EndScrollView();
                EditorGUILayout.BeginVertical();
                if (GUILayout.Button("下一步"))
                {
                    allParts.Clear();
                    int nums = 0;
                    List<string> partsList = new List<string>();
                    for (int i = 0; i < selectCourse.Length; i++)
                    {
                        if (selectCourse[i])
                        {
                            string[] partsStr = Directory.GetDirectories(HotFixPackageDir + "/" + allCourse[i]);
                            for (int j = 0; j < partsStr.Length; j++)
                            {
#if UNITY_EDITOR_OSX
                                string[] s = partsStr[j].Split('/');
#else
                                string[] s = partsStr[j].Split('\\');
#endif
                                nums++;
                                partsList.Add(s[s.Length - 1]);
                            }
                            allParts.Add(i, partsList.ToArray());
                            partsList.Clear();
                        }
                    }

                    isSecond = selectCourse.Length > 0;
                    selectParts = new bool[nums];
                    //allParts = partsList.ToArray();
                }
                EditorGUILayout.EndVertical();
            }
            else
            {
                int idx = 0;
                v2 = GUILayout.BeginScrollView(v2);
                for (int i = 0; i < selectCourse.Length; i++)
                {
                    if (!selectCourse[i])
                        continue;
                    EditorGUILayout.BeginVertical();
                    GUILayout.Label(allCourse[i], EditorStyles.boldLabel);
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.BeginHorizontal();
                    for (int j = 0; j < allParts[i].Length; j++)
                    {
                        var option = new[] { GUILayout.Width(150), GUILayout.MinWidth(120) };
                        selectParts[idx] = GUILayout.Toggle(selectParts[idx], allParts[i][j], option);
                        //selectPath[idx] = allParts[i][j];
                        idx++;
                    }

                    EditorGUILayout.EndHorizontal();
                }
                GUILayout.EndScrollView();

                GUILayout.Space(20);
                if (GUILayout.Button("上一步"))
                    isSecond = false;
                GUILayout.Space(20);
                if (GUILayout.Button("全选"))
                {
                    for (int i = 0; i < selectParts.Length; i++)
                        selectParts[i] = true;
                }
                GUILayout.Space(20);
                if (GUILayout.Button("打包"))
                {
                    string[] p = new string[selectParts.Length];
                    idx = 0;
                    for (int i = 0; i < selectCourse.Length; i++)
                    {
                        if (!selectCourse[i])
                            continue;
                        for (int j = 0; j < allParts[i].Length; j++)
                        {
                            p[idx] = allCourse[i] + "/" + allParts[i][j];
                            idx++;
                        }
                    }
                    DoOutput(selectParts, p);

                    // 压缩资源成zip包
                    for (int i = 0; i < selectParts.Length; i++)
                    {
                        if (selectParts[i])
                            MakeCourseZip(p[i]);
                    }

                    UpdateZips(selectParts, p);
                }
            }
        }
        #endregion


        [MenuItem("HotfixAssetPackage/BuildCommon(通用资源打包)")]
        public static void BuildCommon()
        {
            prefabMaps.Clear();
            // 通用音频资源
            AddBuildMap(AppConst.commonAB_Audio, "*.prefab", AppConst.commonResDir + "/AudiosPrefab", ref prefabMaps);
            // 通用prefab, 图片等资源
            AddBuildMap(AppConst.commonDynamic, new string[] { "*.prefab", "*.png", "*.jpg" }, AppConst.commonResDir + "/LoadResource", ref prefabMaps);

            string outPath = OutHotfixAssetPackage + "/Common";
            string vName = "";
#if UNITY_ANDROID
            outPath = outPath + "/android";
            vName = outPath + "/abversion.txt";
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            BuildPipeline.BuildAssetBundles(outPath, prefabMaps.ToArray(), BuildAssetBundleOptions.UncompressedAssetBundle, BuildTarget.Android);
#elif UNITY_STANDALONE_WIN
            outPath = outPath + "/windows";
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            BuildPipeline.BuildAssetBundles(outPath, prefabMaps.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            // 公共资源打包放入StreamingAssets中
            string p = Application.streamingAssetsPath + "/Common/windows";
            vName = p + "/abversion.txt";
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);
            BuildPipeline.BuildAssetBundles(p, prefabMaps.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
#elif UNITY_STANDALONE_OSX
            outPath = outPath + "/osx";
            if (!Directory.Exists(outPath))
                Directory.CreateDirectory(outPath);
            BuildPipeline.BuildAssetBundles(outPath, prefabMaps.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
            // 公共资源打包放入StreamingAssets中
            string p = Application.streamingAssetsPath + "/Common/osx";
             vName = p + "/abversion.txt";
            if (!Directory.Exists(p))
                Directory.CreateDirectory(p);
            BuildPipeline.BuildAssetBundles(p, prefabMaps.ToArray(), BuildAssetBundleOptions.None, BuildTarget.StandaloneOSX);
#endif
            if (!File.Exists(vName))
            {
                StreamWriter sw = File.CreateText(vName);
                sw.WriteLine(DateTime.Now.ToString() + "------------: 打包的Common文件");
                sw.Dispose();
            }
            else
            {
                FileStream file = new FileStream(vName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new StreamWriter(file);
                sw.WriteLine(DateTime.Now.ToString() + "------------: 打包的Common文件");
                sw.Dispose();
            }
            AssetDatabase.Refresh();
            EditorApplication.Beep();
            Debug.Log(" 通用资源打包完成 ");
        }


        static void UpdateZips(bool[] selectParts, string[] paths)
        {
            Debug.LogError("开始生成需要更的Zip");
            if (Directory.Exists(GetOutUpdateZipsPath()))
                Directory.Delete(GetOutUpdateZipsPath(), true);
            CheckRootFileFolderIsExist(GetOutUpdateZipsPath());

            for (int i = 0; i < selectParts.Length; i++)
            {
                if (selectParts[i])
                {
                    string fileName = paths[i].Replace("/", "");
                    string filePath = GetOutUpdateZipsPath() + "/" + fileName;
                    Directory.CreateDirectory(filePath);

                    string sourcesFileName1 = GetOutZipsPath() + "/" + paths[i] + "/" + AppConst.str_OS;
                    string sourcesFileName2 = AppConst.zipPre + fileName + ".zip";

                    string sourcesFileName = sourcesFileName1 + "/" + sourcesFileName2;

                    File.Copy(sourcesFileName, Path.Combine(filePath, Path.GetFileName(sourcesFileName)));

                    using (Ionic.Zip.ZipFile zip = new Ionic.Zip.ZipFile(System.Text.Encoding.Default))
                    {
                        zip.AddDirectory(filePath, fileName);
                        zip.Save(filePath + ".zip");
                    }

                    Directory.Delete(filePath, true);

                }
            }
            string openURL = Path.Combine(Application.dataPath, "../", GetOutUpdateZipsPath());
            Application.OpenURL(openURL);
        }

        /// <summary>
        /// 获取需要更Zips文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetOutUpdateZipsPath()
        {
            return Application.dataPath.Replace("Assets", "UpdateZips");
        }


        /// <summary>
        /// 获取Zips文件路径
        /// </summary>
        /// <returns></returns>
        private static string GetOutZipsPath()
        {
            return Application.dataPath.Replace("Assets", "OutHotfixAssetZip");
        }

        /// <summary>
        /// 检查Root文件夹是否存在，不存在则创建
        /// </summary>
        /// <param name="path">路径</param>
        private static bool CheckRootFileFolderIsExist(string path)
        {
            if (Directory.Exists(path))
            {
                return true;
            }
            else
            {
                Directory.CreateDirectory(path);
                return false;
            }
        }



        public static void MakeCourseZip(string coursePath)
        {
            string courseName = coursePath.Replace("/", "");
            string path = outHotfixAssetZipDir + "/" + coursePath + "/" + AppConst.str_OS;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string zipName = path + "/" + AppConst.zipPre + courseName + ".zip";
            FileStream fs = File.Create(zipName);
            ZipOutputStream stream = new ZipOutputStream(fs);
            try
            {
                string str = "\\";
#if UNITY_EDITOR_OSX
                str = "/";
#endif
                MakeZip(courseName, OutHotfixAssetPackage + "/" + coursePath + str + courseName + ".dll", stream);
                MakeZip(courseName, OutHotfixAssetPackage + "/" + coursePath + str + courseName + ".pdb", stream);
                MakeZip(courseName, OutHotfixAssetPackage + "/" + coursePath + "/" + AppConst.str_OS, stream);
                MakeZip(courseName, HotFixPackageDir + "/" + coursePath + "/Videos", stream);
            }
            catch (System.Exception e)
            {
                Debug.LogErrorFormat(" Make Zip Error !  {0} ", e.Message);
                fs.Close();
                File.Delete(zipName);
                return;
            }
            stream.Finish();
            stream.Close();
            fs.Close();
        }

        static void MakeZip(string coursName, string sourcePath, ZipOutputStream outputStream)
        {
            if (File.Exists(sourcePath))
            {
                MakeEntry(coursName, sourcePath, outputStream);
                return;
            }

            if (!Directory.Exists(sourcePath))
                return;
            string[] files = Directory.GetFileSystemEntries(sourcePath);
            for (int i = 0; i < files.Length; i++)
            {
                if (Directory.Exists(files[i]))
                    MakeZip(coursName, files[i], outputStream);
                else
                {
                    if (files[i].EndsWith(".meta"))
                        continue;
                    MakeEntry(coursName, files[i], outputStream);
                }
            }
        }

        static void MakeEntry(string courseName, string file, ZipOutputStream outputStream)
        {
            using (FileStream s = File.OpenRead(file))
            {
                byte[] buffer = new byte[s.Length];
#if UNITY_EDITOR_OSX
                string curFileName = courseName + "/" + file.Substring(file.LastIndexOf("/") + 1);
#else
                string curFileName = courseName + "/" + file.Substring(file.LastIndexOf("\\") + 1);
#endif
                //Debug.LogFormat(" CompressZip curFileName : {0} ", curFileName);
                ZipEntry entry = new ZipEntry(curFileName)
                {
                    DateTime = System.DateTime.Now,
                    Size = s.Length,
                };

                outputStream.PutNextEntry(entry);
                int sourceBytes;
                do
                {
                    sourceBytes = s.Read(buffer, 0, buffer.Length);
                    outputStream.Write(buffer, 0, sourceBytes);
                } while (sourceBytes > 0);

                s.Close();
            }
        }

        static List<string> courseParts = new List<string>();
        static string[] courses;
        static int[] curPartNum;
        [MenuItem("Assets/AddNewPart(添加新环节)", priority = 0)]
        public static void AddNewPart()
        {
            CGUIState = GUIState.ADD_NEWPART;
            string[] selectsGUID = Selection.assetGUIDs;

            courseParts.Clear();
            courses = new string[selectsGUID.Length];
            curPartNum = new int[selectsGUID.Length];

            for (int i = 0; i < selectsGUID.Length; i++)
            {
                string coursePath = AssetDatabase.GUIDToAssetPath(selectsGUID[i]);
                curPartNum[i] = Directory.GetDirectories(coursePath).Length;
                int idx = coursePath.LastIndexOf('/');
                string coursName = coursePath.Substring(idx + 1, coursePath.Length - idx - 1);
                //Debug.LogFormat(" AddNewPart coursePath: {0} ; codePath: {1} , curPartNum {2}", coursePath, coursName, curPartNum);
                courses[i] = coursName;
            }
            window1 = GetWindow<HotfixAssetPackager>();
            window1.Show();
        }

        int num = 1;
        void AddNewPartInspector()
        {
            int index;
            num = EditorGUILayout.IntField("增加新环节个数:", num);
            if (GUILayout.Button("确定"))
            {
                if (num < 1 || num > 6)
                {
                    Debug.LogError(" 添加失败，添加环节个数有误，数值范围为 [1,6] ");
                    return;
                }

                for (int i = 0; i < courses.Length; i++)
                {
                    index = curPartNum[i];
                    for (int j = 0; j < num; j++)
                    {
                        string s = courses[i] + "/Part" + (++index);
                        courseParts.Add(s);
                        Debug.LogFormat(" AddNewPart Inspector c: {0} ", s);
                    }
                }

                for (int i = 0; i < courseParts.Count; i++)
                {
                    if (!Directory.Exists(HotFixPackageDir + "/" + courseParts[i]))
                        Directory.CreateDirectory(HotFixPackageDir + "/" + courseParts[i]);
                    CreateCourseAssets(courseParts[i], courseParts[i].Replace("/", ""));

                    if (!Directory.Exists(HotFixCodePackageDir + "/" + courseParts[i]))
                        Directory.CreateDirectory(HotFixCodePackageDir + "/" + courseParts[i]);
                    CreatCodeProject(courseParts[i]);
                }

                window1.Close();
                AssetDatabase.Refresh();
            }
        }

        static void EncryptionAssetBundle(List<AssetBundleBuild> assetBundleBuilds, string outPath)
        {
            string ecryptionPath = "";
            for (int i = 0; i < assetBundleBuilds.Count; i++)
            {
                ecryptionPath = outPath + "/" + assetBundleBuilds[i].assetBundleName.ToLower();
                Debug.LogFormat(" EncryptionAB url: {0}", ecryptionPath);
                byte[] data = File.ReadAllBytes(ecryptionPath);
                for (int j = 0; j < data.Length; j++)
                {
                    data[j] += 1;
                }
                File.WriteAllBytes(ecryptionPath, data);
            }
        }
        // 需要删除之前build过的AB资源，否则在第二次打包加密后无法播放
        static void DeleteOldAssetBundle(string[] assetsbudle)
        {
            for (int i = 0; i < assetsbudle.Length; i++)
            {
                if (File.Exists(assetsbudle[i]))
                    File.Delete(assetsbudle[i]);
            }
        }
    }
}


