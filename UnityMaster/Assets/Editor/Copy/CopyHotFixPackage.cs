
using System.IO;
using UnityEditor;
using UnityEngine;

public class CopyHotFixPackage : EditorWindow
{
    private static CopyHotFixPackage window;

    private static string BaseHotFixPackagePath = Application.dataPath.Replace("Assets", "") + "BaseHotFixPackage";
    private static string HotFixPackagePath = Application.dataPath + "/HotFixPackage";



    [MenuItem("复制/打开BaseHotFixPackage文件夹", priority = 0)]
    static void OpenBaseHotFixPackageFolder()
    {
        string url = BaseHotFixPackagePath;
        Application.OpenURL(url);
    }


    private static string CourseName = string.Empty;
    private static string DeleteCourseName = string.Empty;

    [MenuItem("复制/拷贝单个", priority = 0)]
    private static void CopyOneHotFixPackage()
    {

         window = GetWindow<CopyHotFixPackage>();
         CourseName = string.Empty;
         DeleteCourseName = string.Empty;
         window.Show();

    }

    private void OnGUI()
    {
        CourseName = EditorGUILayout.TextField("拷贝课程名:", CourseName);

        if (GUILayout.Button("拷贝到BaseHotFixPackage"))
        {
            if (CourseName == string.Empty)
            {
                Debug.LogError("请输入文件夹名");
                return;
            }

            string sourceFileName = HotFixPackagePath + "/" + CourseName;
            string destFileName = BaseHotFixPackagePath + "/" + CourseName;

            var isExists = Directory.Exists(sourceFileName);

            if (!isExists)
            {
                Debug.LogError("文件不存在：" + sourceFileName);
                return;
            }


            TDCreate.FileUtils.CopyDirectory(sourceFileName, destFileName);
            AssetDatabase.Refresh();

            Application.OpenURL(destFileName);

            Debug.Log(CourseName+"：HotFixPackage———>BaseHotFixPackage 拷贝成功");
            window.Close();
        }

        if (GUILayout.Button("拷贝到HotFixPackage"))
        {
            if (CourseName == string.Empty)
            {
                Debug.LogError("请输入文件夹名");
                return;
            }

            string sourceFileName = BaseHotFixPackagePath + "/" + CourseName;
            string destFileName = HotFixPackagePath + "/" + CourseName;

            var isExists = Directory.Exists(sourceFileName);

            if (!isExists)
            {
                Debug.LogError("文件夹不存在：" + sourceFileName);
                return;
            }

            TDCreate.FileUtils.CopyDirectory(sourceFileName, destFileName);
            AssetDatabase.Refresh();

            Debug.Log(CourseName+"：BaseHotFixPackage———>HotFixPackage拷贝成功");
            window.Close();
        }

        DeleteCourseName = EditorGUILayout.TextField("删除HotFixPackage下课程名:", DeleteCourseName);

        if (GUILayout.Button("删除HotFixPackage下单个课程"))
        {
            if (DeleteCourseName == string.Empty)
            {
                Debug.LogError("请输入要删除的文件夹名");
                return;
            }

            string deleteFilePath = HotFixPackagePath + "/" + DeleteCourseName;
            var isExists = Directory.Exists(deleteFilePath);

            if (!isExists)
            {
                Debug.LogError("要删除的文件夹不存在：" + deleteFilePath);
                return;
            }

            TDCreate.FileUtils.DeleteFolder(deleteFilePath);
            AssetDatabase.Refresh();

            Debug.Log(deleteFilePath + "：删除成功");
            window.Close();
        }
    }






}
