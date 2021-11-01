using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;

namespace TDCreate {

    public class CreateProjectWindow : OdinEditorWindow
    {

        private static CreateProjectWindow _window;

        protected override void OnEnable()
        {
            base.OnEnable();
            CourseName = String.Empty;
            CourseNums = 0;
            if (_window != null)
                _window = null;
        }


        [MenuItem("田丁美术/创建新课程")]
        private static void OpenWindow()
        {
            _window = GetWindow<CreateProjectWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 1000);
        }





        [InfoBox("课程名称：田丁课程名称格式 TD开头，eg:TD8923")]
        [PropertyOrder(1)]
        [DelayedProperty]
        public string CourseName;



        [InfoBox("课程环节数量")]
        [DelayedProperty]
        [PropertyOrder(2)]
        [OnValueChanged("InputCourseNumsCallBack")]
        public int CourseNums;

        private void InputCourseNumsCallBack()
        {
            var isClear = CourseNums == 0;

            if (!isClear)
            {
                var isExceedDicCount = CourseNums >= Parts.Count;

                if (isExceedDicCount)
                {
                    for (int i = 0; i < CourseNums; i++)
                    {
                        var key = "Part" + (i + 1);
                        var isKey = Parts.ContainsKey(key);

                        if (!isKey)
                        {
                            var value = new PartData();
                            value.CourseName = CourseName;
                            value.PathName = key;
                            Parts.Add(key, value);
                        }
                    }
                }
                else
                {
                    for (int i = Parts.Count - 1; i >= CourseNums; i--)
                    {
                        var key = "Part" + (i + 1);
                        var isKey = Parts.ContainsKey(key);
                        if (isKey)
                            Parts.Remove(key);
                    }
                }
            }
            else
            {
                Parts.Clear();
            }
        }



        [PropertyOrder(5)]
        [Button("生成文件夹", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void OnClickCreateFolder()
        {
            var assetRootPath = Config.HotFixPackageRootPath + CourseName;
            var isExists = FileUtils.CheckFolderExists(assetRootPath);

            if (isExists)
                return;

            FileUtils.CreateMorePartFolder(CourseNums, assetRootPath);
            FileUtils.CreateMorePartChildFolder(assetRootPath);
            FileUtils.CreateMoreAudiosChildFolder(assetRootPath);

            Debug.LogError("生成Asset文件夹成功");



            var codeRootPath = Config.HotFixCodePackageRootPath + CourseName;
            isExists = FileUtils.CheckFolderExists(codeRootPath);

            if (isExists)
                return;

            FileUtils.CreateMorePartFolder(CourseNums, codeRootPath);

            Debug.LogError("生成Code文件夹成功");


            AssetDatabase.Refresh();
            
        }

        [PropertyOrder(4)]
        [DictionaryDrawerSettings(KeyLabel = "环节名称", ValueLabel = "环节具体设置")]
        public Dictionary<string, PartData> Parts = new Dictionary<string, PartData>();


        [PropertyOrder(5)]
        [Button("生成预制件和脚本", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void OnClickCreateBtn()
        {
            var isCanCreate = IsCanCreate();
            if (!isCanCreate)
                return;

            foreach (var item in Parts)            
                item.Value.BassPart.Create();
            
            //打开脚本批处理文件
            string openURL = Path.Combine(Application.dataPath, "../", "HotfixCodeProject/"+Config.TDBatPart);
            Application.OpenURL(openURL);

            _window.Close();
            _window.OnDestroy();
        }


        private bool IsCanCreate()
        {
            bool isCan = true;
            foreach (var item in Parts)
            {
                var key = item.Key;
                var vaule = item.Value;

                if (vaule.PartType == PartType.Null)
                {
                    isCan = false;
                    Debug.LogError(string.Format("环节{0}的类型为{1}，请设置好参数", key, vaule.PartType));
                    break;
                }
            }

            return isCan;
        }





    }
}


   


  

   

  
    
  
    
  

  
    
