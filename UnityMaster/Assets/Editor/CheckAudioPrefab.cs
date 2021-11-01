using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;


namespace UnityEditor
{
    
    public class CheckAudioPrefab:MonoBehaviour
    {
        [MenuItem("HotfixAssetPackage/检查Common音频是否Miss")]
        public static void CheckCommonAudio()
        {           
            string rootPath = "Assets/ILFramework/CommonRes/AudiosPrefab/";
            string [] paths = Directory.GetFiles(rootPath, "*.prefab");
         
            foreach (var path in paths)            
                IsMiss(path);
            
            Debug.Log("Common检查完成");
            
        }
       
        [MenuItem("Assets/CheckCourseAudio(检查课程音频预制体是否Miss音频)", priority = 0)]
        public static void CheckCourseAudio()
        {
            System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
            
            string[] selectsGUID = Selection.assetGUIDs;
            int temp = 0;
            int allPartNums = 0;

            for (int i = 0; i < selectsGUID.Length; i++)
            {
                string coursePath = AssetDatabase.GUIDToAssetPath(selectsGUID[i]);
                allPartNums += Directory.GetDirectories(coursePath).Length;
            }
          

            List<string> allPath = new List<string>();

            for (int i = 0; i < selectsGUID.Length; i++)
            {
                string coursePath = AssetDatabase.GUIDToAssetPath(selectsGUID[i]);             
                string courseName = coursePath.Replace("Assets/HotFixPackage/", string.Empty);


                Loom.RunAsync(() =>
                {

                    string[] parts = Directory.GetDirectories(coursePath);
                 
                    for (int j = 0; j < parts.Length; j++)
                    {
                        string part = parts[j].Replace("\\", "/");
                       
                        string partName = part.Replace("Assets/HotFixPackage/" + courseName+"/", string.Empty);
                        string rootPath = part+ "/AudiosPrefab/";

                        string bgmPrePath = rootPath + courseName + partName + "_Bgm_htp.prefab";
                        string soundPrePath = rootPath + courseName + partName + "_Sound_htp.prefab";
                        string voicePreName = rootPath + courseName + partName + "_Voice_htp.prefab";


                        allPath.Add(bgmPrePath);
                        allPath.Add(soundPrePath);
                        allPath.Add(voicePreName);

                        temp++;
                    }

                });
            }

            while (true)
            {             
                if (temp >= allPartNums)
                    break;
            }
      
         
            foreach (var path in allPath)
                IsMiss(path);

            sw.Stop();
            
            long time = sw.ElapsedMilliseconds;
            Debug.LogError("<color='#00ff66'> ====== 总耗时：" + time + "s</color>");
        }




        private static void IsMiss(string path)
        {         
            GameObject go =  AssetDatabase.LoadAssetAtPath<GameObject>(path);
            var bellAC = go.GetComponent<BellAudiosClip>();
            bool isZero = bellAC.clips.Length == 0;

            if (isZero)
                return;

            foreach (var item in bellAC.clips)
            {
                if (item == null)
                {
                    Debug.LogError("存在Miss：" + path);
                    continue; 
                }
            }         
        }

    }
}

