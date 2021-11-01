
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyHotFixPackageMore : EditorWindow
{

    private static CopyHotFixPackageMore window;

    private static string BaseHotFixPackagePath = Application.dataPath.Replace("Assets", "") + "BaseHotFixPackage";
    private static string HotFixPackagePath = Application.dataPath + "/HotFixPackage";
        
    private static bool  [] allSelectCourse;
    private static string[] allCourse;
    private static Vector2 v2;
    private static string btnName;
    private static string sourceFileName;
    private static string destFileName;


    private static bool isDelete = false;

    private static List<string> curSelectCourse=new List<string>();

    [MenuItem("复制/拷贝多个/拷贝到HotFixPackage")]
    public static void CopyMoreToHotFixPackage()
    {
        btnName = "拷贝到HotFixPackage";
        sourceFileName = BaseHotFixPackagePath;
        destFileName = HotFixPackagePath;
        isDelete = false;

        ShowSelectCoursePanel(BaseHotFixPackagePath);
    }

    [MenuItem("复制/拷贝多个/拷贝到BaseHotFixPackage")]
    public static void CopyMoreToBaseHotFixPackage()
    {
        btnName = "拷贝到BaseHotFixPackage";
        sourceFileName = HotFixPackagePath;
        destFileName = BaseHotFixPackagePath;

        isDelete = false;
        ShowSelectCoursePanel(HotFixPackagePath);
    }

    [MenuItem("复制/删除多个")]
    public static void DeleteMoreFolder()
    {
        btnName = "删除HotFixPackage多个文件";
        isDelete = true;
        ShowSelectCoursePanel(HotFixPackagePath);
        
    }

    private static void ShowSelectCoursePanel(string path)
    {
        OpenWindow();

        v2 = new Vector2(0, 0);
        string[] packages = Directory.GetDirectories(path);

        int num = packages.Length;

        allSelectCourse = new bool[num];

        List<string> courseList = new List<string>();

        for (int i = 0; i < packages.Length; i++)        
            courseList.Add( packages[i].Replace("\\", "/").Replace(path + "/", string.Empty)); //

        allCourse = courseList.ToArray();

    }

    private void OnGUI()
    {
        if (isDelete)
        {
            DeleteMore();
        }
        else
        {
            CopyMore();
        }
      
    }


    private void CopyMore()
    {     
        v2 = GUILayout.BeginScrollView(v2);
        for (int i = 0; i < allSelectCourse.Length; i++)
        {
            if (i % 5 == 0) EditorGUILayout.BeginHorizontal();
            var option = new[] { GUILayout.Width(150), GUILayout.MinWidth(120) };
            allSelectCourse[i] = GUILayout.Toggle(allSelectCourse[i], allCourse[i], option);
            if (i % 5 == 4) EditorGUILayout.EndHorizontal();
        }
        if (allSelectCourse.Length % 5 != 0)
            EditorGUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(btnName))
        {
            curSelectCourse.Clear();

            for (int i = 0; i < allSelectCourse.Length; i++)
            {
                if (allSelectCourse[i])                                
                    curSelectCourse.Add(allCourse[i]);               
            }

            window.Close();

            if (curSelectCourse.Count == 0)
                return;

            for (int i = 0; i < curSelectCourse.Count; i++)
            {
                TDCreate.FileUtils.CopyDirectory(sourceFileName + "/" + curSelectCourse[i], destFileName + "/" + curSelectCourse[i]);

                if(btnName== "拷贝到BaseHotFixPackage")
                    Application.OpenURL(BaseHotFixPackagePath+"/"+ curSelectCourse[i]);

               
            }

            
            AssetDatabase.Refresh();
           
        }



        EditorGUILayout.EndVertical();
    }

    private void DeleteMore()
    {
        v2 = GUILayout.BeginScrollView(v2);
        for (int i = 0; i < allSelectCourse.Length; i++)
        {
            if (i % 5 == 0) EditorGUILayout.BeginHorizontal();
            var option = new[] { GUILayout.Width(150), GUILayout.MinWidth(120) };
            allSelectCourse[i] = GUILayout.Toggle(allSelectCourse[i], allCourse[i], option);
            if (i % 5 == 4) EditorGUILayout.EndHorizontal();
        }
        if (allSelectCourse.Length % 5 != 0)
            EditorGUILayout.EndHorizontal();
        GUILayout.EndScrollView();

        EditorGUILayout.BeginVertical();
        if (GUILayout.Button(btnName))
        {
            curSelectCourse.Clear();

            for (int i = 0; i < allSelectCourse.Length; i++)
            {
                if (allSelectCourse[i])
                    curSelectCourse.Add(allCourse[i]);
            }

            window.Close();

            if (curSelectCourse.Count == 0)
                return;

            for (int i = 0; i < curSelectCourse.Count; i++)
            {
                string path = HotFixPackagePath + "/" + curSelectCourse[i];
                TDCreate.FileUtils.DeleteFolder(path);
                Debug.Log(path+"：删除成功");
            }


            AssetDatabase.Refresh();

        }



        EditorGUILayout.EndVertical();
    }

    private static void OpenWindow()
    {
       window = GetWindow<CopyHotFixPackageMore>();
       window.Show();

    }

}
