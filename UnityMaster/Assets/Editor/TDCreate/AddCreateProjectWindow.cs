using System;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using Sirenix.Utilities;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEngine;
namespace TDCreate
{
    public class AddCreateProjectWindow : OdinEditorWindow
    {

        private static AddCreateProjectWindow _window;


        protected override void OnEnable()
        {
            base.OnEnable();
            CourseName = String.Empty;
            CourseNums = 0;
            _lastName = string.Empty;
            _isExists = false;
            _startI = 0;
            if (_window != null)
                _window = null;
        }

        [MenuItem("田丁美术/增加环节")]
        private static void OpenWindow()
        {
            _window = GetWindow<AddCreateProjectWindow>();
            _window.position = GUIHelper.GetEditorWindowRect().AlignCenter(800, 1000);
        }

        private bool _isExists;
        private string _lastName;
        private int _startI;
        [InfoBox("请输入新增环节的课程名：eg:TD8923")]
        [PropertyOrder(1)]
        [DelayedProperty]
        [OnValueChanged("InitCourseNameCallBack")]
        public string CourseName;

        private void InitCourseNameCallBack()
        {
            var assetRootPath = Config.HotFixPackageRootPath + CourseName;
            _isExists = FileUtils.CheckFolderExists(assetRootPath,false);

            if (!_isExists)
                Debug.LogError(string.Format("此课程{0}还未创建,无法新增", CourseName));
            else
            _lastName=FileUtils.GetLastFullName(assetRootPath);
        }

        [ShowIf("_isExists")]
        [InfoBox("新增环节数量")]
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
                _startI = int.Parse(_lastName.Replace("Part", string.Empty));
               
                if (isExceedDicCount)
                {
                    
                    
                    for (int i = 0; i < CourseNums; i++)
                    {
                        var key = "Part" + (i + 1+ _startI);
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
                        var key = "Part" + (i + 1+ _startI);
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

        [ShowIf("_isExists")]
        [PropertyOrder(4)]
        [DictionaryDrawerSettings(KeyLabel = "环节名称", ValueLabel = "环节具体设置")]
        public Dictionary<string, PartData> Parts = new Dictionary<string, PartData>();

        [ShowIf("_isExists")]
        [PropertyOrder(5)]
        [Button("生成文件夹", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void OnClickCreateFolder()
        {
            List<string> partNames = new List<string>();
            foreach (var item in Parts.Keys)            
                partNames.Add(item);
            
           
            var assetRootPath = Config.HotFixPackageRootPath + CourseName;
        
            FileUtils.AddMorePartFolder(CourseNums,_startI, assetRootPath);
            AssetDatabase.Refresh();
            FileUtils.AddMorePartChildFolder(partNames, assetRootPath);
            FileUtils.AddMoreAudiosChildFolder(partNames, assetRootPath);

            Debug.LogError("生成Asset文件夹成功");


            var codeRootPath = Config.HotFixCodePackageRootPath + CourseName;
     

            FileUtils.AddMorePartFolder(CourseNums, _startI,codeRootPath);

            Debug.LogError("生成Code文件夹成功");


            AssetDatabase.Refresh();

        }



        [ShowIf("_isExists")]
        [PropertyOrder(6)]
        [Button("生成预制件和脚本", ButtonSizes.Large, ButtonStyle.CompactBox)]
        private void OnClickCreateBtn()
        {
            var isCanCreate = IsCanCreate();
            if (!isCanCreate)
                return;

            foreach (var item in Parts)
                item.Value.BassPart.Create();

            //打开脚本批处理文件
            string openURL = Path.Combine(Application.dataPath, "../", "HotfixCodeProject/" + Config.TDBatPart);
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

