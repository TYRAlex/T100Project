using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Xml;
using System.Text;
using System;
using System.Collections.Generic;

public class CourseMigrateTool : EditorWindow
{
    static CourseMigrateTool window;

    static string packageDir = "Assets/HotFixPackage";
    static string slnProjectDir = "Assets/../HotfixCodeProject";
    static string audioPrefabDir = "Assets/Editor/Template";
    static string dllPath = "Assets/Plugins/packageDll/";
    static string classSign = "MigrateSeed";
    static string luaSign = "originLua";

    static string unityPath = EditorApplication.applicationContentsPath;
    static string managedPath = "/Managed/UnityEngine/";

    enum InspectorType
    {
        CREATE_COURSE = 0,
        CONVERT_DLLPATH,
    }
    static InspectorType curType;

    static string inputCourseName;
    static string selectPath = "E:/T100Pro/T100";
    static int inputCoursePart;
    static int inputLuaCode;
    [MenuItem("CourseMigrate/CreateCourse")]
    public static void CreateCourse()
    {
        curType = InspectorType.CREATE_COURSE;
        inputCourseName = "Course";
        inputCoursePart = 1;
        inputLuaCode = 1;

        window = GetWindow<CourseMigrateTool>();
        window.Show();
    }

    public static void CreatePrefabProject(string name)
    {
        Directory.CreateDirectory(packageDir + "/" + name + "/Audios");
        Directory.CreateDirectory(packageDir + "/" + name + "/AudiosPrefab");
        Directory.CreateDirectory(packageDir + "/" + name + "/Spines");
        Directory.CreateDirectory(packageDir + "/" + name + "/Textures");
        Directory.CreateDirectory(packageDir + "/" + name + "/CourseLua");
        Directory.CreateDirectory(packageDir + "/" + name + "/LoadResource");

        string prefabName = name.Replace("/", "");
        File.Copy(audioPrefabDir + "/AudiosPrefab/CourseBgm_htp.prefab", packageDir + "/" + name + "/AudiosPrefab/" + prefabName + "_Bgm_htp.prefab");
        File.Copy(audioPrefabDir + "/AudiosPrefab/CourseSound_htp.prefab", packageDir + "/" + name + "/AudiosPrefab/" + prefabName + "_Sound_htp.prefab");
        File.Copy(audioPrefabDir + "/AudiosPrefab/CourseVoice_htp.prefab", packageDir + "/" + name + "/AudiosPrefab/" + prefabName + "_Voice_htp.prefab");

        string luaCodePath = selectPath + "/Assets/LuaFramework/GameLua/Course" + inputLuaCode;
        string className = "C" + inputLuaCode + "Game" + name.Substring(name.Length - 1)+".lua";
        string content = File.ReadAllText(luaCodePath + "/" + className);
        
        // 修改lua脚本
        content = content.Replace("Common/", "").Replace("newObject(objs[0])", "objs").Replace("this.Show(false)", "--").Replace("RightUICtrl:ShowArrow", "RightUICtrl.ShowArrow");
        content = content.Replace("spineMgr:DoAnimation", "spineMgr:DoAnimationLua");
        content = content.Replace("'course" + inputLuaCode + "game'", "'" + prefabName + "'");
        content = content.Replace("Logic/", "").Replace("this,shield", "this.shield");

        File.WriteAllText(packageDir + "/" + name + "/CourseLua/" + className, content);
    }

    public static void CreatCodeProject(string name, string luaName)
    {
        string templatePath = slnProjectDir + "/MigrateSln/" + classSign + "/" + classSign + "/" + classSign + ".cs";
        string content = File.ReadAllText(templatePath);

        content = content.Replace(classSign, name.Replace("/", ""));
        content = content.Replace(luaSign, luaName);

        string str = slnProjectDir + "/" + name + "/" + name.Replace("/", "") + ".cs";

        File.WriteAllText(str, content);
        CreateCsProject(name);
    }

    public static void CreateCsProject(string hotfix_project_name)
    {
        XmlDocument xml = new XmlDocument();
        xml.Load(slnProjectDir + "/MigrateSln/" + classSign + "/" + classSign + "/" + classSign + ".csproj");
        XmlElement xmlRoot = xml.DocumentElement;
        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = new UTF8Encoding(false);
        settings.Indent = true;

        string str = hotfix_project_name.Replace("/", "");
        string outStr = hotfix_project_name.Replace("/", "\\");
        XmlWriter xw = XmlWriter.Create(slnProjectDir + "/" + hotfix_project_name + "/" + str + ".csproj", settings);
        hotfix_project_name = hotfix_project_name.Replace("/", "");

        foreach (XmlNode node3 in xmlRoot.ChildNodes)
        {
            if (node3.HasChildNodes)
            {
                foreach (XmlNode node4 in node3.ChildNodes)
                {
                    if (node4.Name == "RootNamespace")
                    {
                        XmlElement tmp = (XmlElement)node4;
                        tmp.InnerText = hotfix_project_name;
                    }
                    else if (node4.Name.Equals("AssemblyName"))
                        node4.InnerText = hotfix_project_name;
                    else if (node4.Name.Equals("OutputPath"))
                    {
                        string outPath = "..\\..\\..\\OutHotfixAssetPackage\\" + outStr;
                        node4.InnerText = outPath;
                    }

                    // 添加相关引用
                    else if (node4.Name.Equals("Reference"))
                    {
                        string attributes = node4.Attributes[0].Value;
                        string preStr = "..\\..\\..\\";
                        switch (attributes)
                        {
                            case "Assembly-CSharp":
                                node4.FirstChild.InnerText = preStr + "Library\\ScriptAssemblies\\Assembly-CSharp.dll";
                                break;
                            case "UnityEngine.CoreModule":
                                node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.CoreModule.dll"; //preStr + dllPath + "/UnityEngine.CoreModule.dll";
                                break;
                            case "UnityEngine":
                                node4.FirstChild.InnerText = unityPath + managedPath + "UnityEngine.dll"; //preStr + dllPath + "/UnityEngine.dll";
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

    private void OnGUI()
    {
        switch (curType)
        {
            case InspectorType.CREATE_COURSE:
                CreateCourseOnGUI();
                break;
            case InspectorType.CONVERT_DLLPATH:
                ConverDllPathOnGUI();
                break;
            default:
                break;
        }
    }

    private void CreateCourseOnGUI()
    {
        GUILayout.Space(20);
        inputCourseName = EditorGUILayout.TextField("CourseName: ", inputCourseName);
        inputCoursePart = EditorGUILayout.IntField("CoursePart(范围1-6): ", inputCoursePart);
        inputLuaCode = EditorGUILayout.IntField("LuaCode(对应原课程ID): ", inputLuaCode);
        GUILayout.Space(20);
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField(" BeBo1.0工程地址: ", selectPath);
        if(GUILayout.Button("select"))
            selectPath = EditorUtility.OpenFolderPanel("BeBo1.0工程地址", "", "");
        EditorGUILayout.EndHorizontal();

        if (GUILayout.Button("Go"))
        {
            if (inputCoursePart < 0 || inputCoursePart > 6 || inputLuaCode <= 0)
                return;

            if (!Directory.Exists(packageDir + "/" + inputCourseName) && !Directory.Exists(packageDir + "/" + inputCourseName))
            {
                Directory.CreateDirectory(packageDir + "/" + inputCourseName);
                Directory.CreateDirectory(slnProjectDir + "/" + inputCourseName);
            }
            else
            {
                Debug.LogError("工程已存在，请删除后再创建");
                return;
            }

            string str;
            for (int i = 0; i < inputCoursePart; i++)
            {
                str = inputCourseName + "/Part" + (i + 1);

                if (!Directory.Exists(packageDir + "/" + str))
                    Directory.CreateDirectory(packageDir + "/" + str);
                CreatePrefabProject(str);

                if (!Directory.Exists(slnProjectDir + "/" + str))
                    Directory.CreateDirectory(slnProjectDir + "/" + str);
                CreatCodeProject(str, "C" + inputLuaCode + "Game" + (i + 1));
            }
            window.Close();
            AssetDatabase.Refresh();
        }
    }


    [MenuItem("Assets/MigrateAssets", priority = 0)]
    public static void MigrateAssets()
    {
        string[] selectPaths = Selection.assetGUIDs;
        string singlePath, courseName, destDir, tempDir;

        for (int i = 0; i < selectPaths.Length; i++)
        {
            singlePath = AssetDatabase.GUIDToAssetPath(selectPaths[i]);
            courseName = singlePath.Substring(singlePath.LastIndexOf("/") + 1);
            if (!Directory.Exists(packageDir + "/" + courseName))
            {
                Debug.LogErrorFormat(" {0} 课程不存在，跳过！", courseName);
                continue;
            }

            destDir = packageDir + "/" + courseName;

            string[] dirs; string pstr;
            pstr = singlePath + "/Audios";
            if (Directory.Exists(pstr))
            {
                // 音频文件剪切
                dirs = Directory.GetFileSystemEntries(pstr);
                tempDir = destDir + "/Part1/Audios/";
                CutAssets(dirs, tempDir);
            }

            // 音频组件复制
            dirs = Directory.GetFileSystemEntries(singlePath + "/AudioClips");
            int partNum = Directory.GetDirectories(destDir).Length;
            for (int num = 0; num < partNum; num++)
            {
                SearchAudioToCopy(dirs, destDir + "/Part" + (num + 1) + "/AudiosPrefab/");
            }            

            // 场景预制剪切
            dirs = Directory.GetFileSystemEntries(singlePath + "/CourseGame");
            for (int j = 0; j < dirs.Length; j++)
            {
                if (File.Exists(dirs[j]))
                {
                    int length = dirs[j].Length;
                    int pointIdx = dirs[j].LastIndexOf(".");
                    //Debug.LogFormat(" MigrateAssets files : {0} ", dirs[j]);
                    string extension = dirs[j].Substring(pointIdx);
                    if (extension == ".meta")
                        continue;

                    string name = dirs[j].Substring(0, pointIdx);
                    name = name.Substring(name.Length - 5);
                    if(name.Contains("Game"))
                    {
                        tempDir = destDir + "/Part" + name.Substring(name.Length - 1);
                        if (!Directory.Exists(tempDir))
                        {
                            Debug.LogErrorFormat(" {0} 文件夹路径不存在！", tempDir);
                            return;
                        }
                        File.Move(dirs[j], tempDir + "/" + courseName + tempDir.Substring(tempDir.LastIndexOf("/") + 1) + "_htp.prefab");
                    }
                }
            }

            pstr = singlePath + "/Spines";
            if (Directory.Exists(pstr))
            {
                // spine资源剪切
                dirs = Directory.GetFileSystemEntries(pstr);
                tempDir = destDir + "/Part1/Spines/";
                CutAssets(dirs, tempDir);
            }

            pstr = singlePath + "/Texture";
            if (Directory.Exists(pstr))
            {
                // Texture资源剪切
                dirs = Directory.GetFileSystemEntries(pstr);
                tempDir = destDir + "/Part1/Textures/";
                CutAssets(dirs, tempDir);
            }
        }

        AssetDatabase.Refresh();
    }

    static void CutAssets(string[] sourcePath, string destPath)
    {
        for (int j = 0; j < sourcePath.Length; j++)
        {
            string name = sourcePath[j].Substring(sourcePath[j].LastIndexOf("\\") + 1);
            if (Directory.Exists(sourcePath[j]))
                Directory.Move(sourcePath[j], destPath + name);
            else if (File.Exists(sourcePath[j]))
                File.Move(sourcePath[j], destPath + name);
        }
    }

    static void SearchAudioToCopy(string[] source, string path)
    {
        string[] dest = Directory.GetFileSystemEntries(path);
        for (int j = 0; j < source.Length; j++)
        {
            //Debug.LogFormat(" source: {0} , target: {1} : ", dirs[j], dest[j]);
            GameObject sourceGo = AssetDatabase.LoadAssetAtPath<GameObject>(source[j]);
            GameObject targetGo = AssetDatabase.LoadAssetAtPath<GameObject>(dest[j]);
            CopyAudioComponent(sourceGo, targetGo);
        }
    }

    static void CopyAudioComponent(GameObject go, GameObject target)
    {
        if (go == null || target == null)
            return;
        //Debug.LogFormat(" go: {0} , target: {1} : ", go, target);
        Component sc = go.GetComponent("BellAudioClips");        
        Type t = sc.GetType();
        FieldInfo field = t.GetField("clips");
        object obj = Activator.CreateInstance(t);
        AudioClip[] clips = (AudioClip[])field.GetValue(sc);
        target.GetComponent<BellAudiosClip>().clips = clips;
        PrefabUtility.SavePrefabAsset(target);
    }


    static string[] dlls = new string[]
    {
        "UnityEngine",
        "UnityEngine.CoreModule",
        "UnityEngine.PhysicsModule",
        "UnityEngine.Physics2DModule",
        "UnityEngine.AudioModule",
    };

    static bool IsStandDlls(string attribute)
    {
        for (int i = 0; i < dlls.Length; i++)
        {
            if (attribute == dlls[i])
                return true;
        }
        return false;
    }

     static List<string> csprojPath;
    // mac平台下修改cs工程dll引用路径
    [MenuItem("CourseMigrate/ModifyDllPath(ios)")]
    public static void ModifyDllPath()
    {
        //#if UNITY_EDITOR_OSX
        curType = InspectorType.CONVERT_DLLPATH;
        csprojPath = new List<string>();
        window = GetWindow<CourseMigrateTool>();
        window.Show();
        //#endif
    }

    void ConverDllPathOnGUI()
    {
        GUILayout.Space(20);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.LabelField("csproj 工程路径, 当前选择 ", csprojPath.Count.ToString());
        for (int i = 0; i < csprojPath.Count; i++)
        {
            EditorGUILayout.LabelField(i+".", csprojPath[i]);
        }
        EditorGUILayout.EndVertical();
        GUILayout.Space(20);
        if (GUILayout.Button("选择"))
        {
            string tempStr = EditorUtility.OpenFilePanel("csproj工程", slnProjectDir, "");
            csprojPath.Add(tempStr);
        }
        GUILayout.Space(20);
        if(GUILayout.Button("修改"))
        {
            //string extension = ".dll";
            for (int p = 0; p < csprojPath.Count; p++)
            {
                ConvertCsproj(csprojPath[p]);
                //XmlDocument xmlDoc = new XmlDocument();
                //Debug.LogFormat("CSProj : {0}", csprojPath[p]);
                //xmlDoc.Load(csprojPath[p]);

                //XmlNodeList xl = xmlDoc.GetElementsByTagName("Reference");
                //for (int i = 0; i < xl.Count; i++)
                //{
                //    string attribute = xl[i].Attributes[0].Value;
                //    if (IsStandDlls(attribute))
                //        xl[i].FirstChild.InnerText = unityPath + managedPath + attribute + extension;
                //    else
                //    {
                //        if (attribute == "UnityEngine.UI")
                //            xl[i].FirstChild.InnerText = unityPath + "/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll";
                //    }
                //}

                //xmlDoc.Save(csprojPath[p]);
            }
            window.Close();
        }
    }

    [MenuItem("Assets/ReplaceCS(更新CS脚本)", priority = 0)]
    public static void ReplaceCS()
    {
        string[] guids = Selection.assetGUIDs;
        string codePath, curCoursePath;
        string[] codes;
        //string templatePath = slnProjectDir + "/MigrateSln/" + classSign + "/" + classSign + "/" + classSign + ".cs";

        for (int i = 0; i < guids.Length; i++)
        {
            curCoursePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string course = curCoursePath.Substring(curCoursePath.LastIndexOf("/"));
            codePath = slnProjectDir + course;
            Debug.LogFormat("codePath:{0}    course:{1}  ////////{2}..........{3}",codePath,course, curCoursePath.LastIndexOf("/"),course);
            codes = Directory.GetDirectories(codePath);
            for (int j = 0; j < codes.Length; j++)
            {
                string part = codes[j].Substring(codes[j].LastIndexOf("\\") + 1);
                Debug.LogFormat(" codePath: {0}  part: {1}", codes[j], part);

                string str = codes[j] + "/" + course + part + ".cs";
                string content = File.ReadAllText(str);
                content = content.Replace("LuaState lua;", "LuaState lua; bool init;");
                content = content.Replace("lua.DoFile", "if(init){lua.DoFile");
                content = content.Replace("(curGo);", "(curGo);}");
                content = content.Replace("LuaManager.instance.AddLuaCourseBundle", "init = LuaManager.instance.AddLuaCourseBundle");
                File.WriteAllText(str, content);
            }
        }
    }

    [MenuItem("Assets/MoveZipAndVideo", priority = 0)]
    public static void MoveZipAndVideo()
    {
        string zipPath = "Assets/../OutHotfixAssetZip";
        string videoPath = "E:/bellT100/Videos/Videos/";
        string TargetPath = "Assets/../../Course_SVN";
        string[] guids = Selection.assetGUIDs;
        string codePath, curCoursePath;
        string[] codes;
        for (int i = 0; i < guids.Length; i++)
        {
            curCoursePath = AssetDatabase.GUIDToAssetPath(guids[i]);
            string course = curCoursePath.Substring(curCoursePath.LastIndexOf("/")+1);
            codePath = zipPath + "/" + course;
            codes = Directory.GetDirectories(codePath);
            string str_2 = TargetPath + "/" + course;
            if (!Directory.Exists(str_2 + "/Android"))
            {
                Directory.CreateDirectory(str_2+"/Android");
            }
            for (int j = 0; j < codes.Length; j++)
            {
                string part = codes[j].Substring(codes[j].LastIndexOf("\\") + 1);
                Debug.LogFormat(" codePath: {0}  part: {1}  course: {2}", codes[j], part, course);
                string str_1 = codes[j] +"/android/and_"+course + part + ".zip";
                File.Copy(str_1, str_2 + "/Android/and_"+course + part + ".zip", true);
            }
            string[] fileList = Directory.GetFileSystemEntries(videoPath+course+"#/");
            foreach (string file in fileList)
            {
                    File.Copy(file, TargetPath + "/" + course + "/" + Path.GetFileName(file), true);
            }
        }
    }

    [MenuItem("CourseMigrate/ConvertAllSln")]
    public static void ConvertAllSln()
    {
        string sign = "\\";
#if UNITY_EDITOR_OSX
        sign = "/";
#endif
        string[] coursesDir = Directory.GetDirectories(slnProjectDir);
        string courseName, partName, csproj;
        string[] partsDir;

        for (int i = 0; i < coursesDir.Length; i++)
        {
            courseName = coursesDir[i].Substring(coursesDir[i].LastIndexOf(sign) + 1);
            Debug.LogFormat(" ConvertAllSln courseName: {0} ", courseName);
            partsDir = Directory.GetDirectories(coursesDir[i]);
            for (int j = 0; j < partsDir.Length; j++)
            {
                partName = partsDir[j].Substring(partsDir[j].LastIndexOf(sign) + 1);
                Debug.LogFormat(" ConvetAllSln partName: {0} ", partName);
                csproj = Directory.GetFiles(partsDir[j], "*.csproj")[0];
                ConvertCsproj(csproj);
            }
        }
    }

    public static void ConvertCsproj(string csproj)
    {
        string extension = ".dll";
        XmlDocument xmlDoc = new XmlDocument();
        Debug.LogFormat("CSProj : {0}", csproj);
        xmlDoc.Load(csproj);
        XmlNodeList xl = xmlDoc.GetElementsByTagName("Reference");
        for (int i = 0; i < xl.Count; i++)
        {
            string attribute = xl[i].Attributes[0].Value;
            if (IsStandDlls(attribute))
                xl[i].FirstChild.InnerText = unityPath + managedPath + attribute + extension;
            else
            {
                if (attribute == "UnityEngine.UI")
                    xl[i].FirstChild.InnerText = unityPath + "/UnityExtensions/Unity/GUISystem/UnityEngine.UI.dll";
            }
        }
        xmlDoc.Save(csproj);
    }
}
