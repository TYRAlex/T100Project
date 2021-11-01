
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace TDCreate
{

    public static class FileUtils
    {

       public static string GetLastFullName(string path)
       {
            string name = string.Empty;

            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();
            name = directoryInfos[directoryInfos.Length - 1].Name;
            return name;
       }

       public static string[] GetSubfolders(string path)
       {
           var s= Directory.GetDirectories(path);
            foreach (var item in s)
            {
                Debug.LogError(item);
            }
            return s;
       }

        public static bool CheckFolderExists(string path,bool isCreate=true)
        {
            var isExists = Directory.Exists(path);

            if (!isExists&& isCreate)
                Directory.CreateDirectory(path);

            if (isExists)
                Debug.LogError("文件夹已存在：" + path);

            return isExists;
        }

        public static void AddMorePartFolder(int courseNums,int startI, string rootPath)
        {
            for (int i = 0; i < courseNums; i++)
            {
                var folderName = "Part" + (i + 1+ startI); //一级目录
                var path = rootPath + "/" + folderName;
                CheckFolderExists(path);
            }
        }

        public static void CreateMorePartFolder(int courseNums, string rootPath)
        {
            for (int i = 0; i < courseNums; i++)
            {
                var folderName = "Part" + (i + 1); //一级目录
                var path = rootPath + "/" + folderName;
                CheckFolderExists(path);
            }
        }

        public static void AddMorePartChildFolder( List<string> partNames,string rootPath)
        {
            List<string> folders = new List<string> { "Audios", "AudiosPrefab", "LoadResource", "Spines", "Textures" };

            DirectoryInfo directoryInfo = new DirectoryInfo(rootPath);
            DirectoryInfo[] directoryInfos = directoryInfo.GetDirectories();

            foreach (var info in directoryInfos)
            {
                var name = info.Name;
                var isContain= partNames.Contains(name);
                if (isContain)
                {
                    string partPath = info.FullName.Replace("\\", "/");
                    for (int i = 0; i < folders.Count; i++)
                    {
                        var path = partPath + "/" + folders[i];
                        CheckFolderExists(path);
                    }
                }
            }       
        }


        public static void CreateMorePartChildFolder(string rootPath)
        {
            List<string> folders = new List<string> { "Audios", "AudiosPrefab", "LoadResource", "Spines", "Textures" };

            var parts = Directory.GetDirectories(rootPath);

            for (int i = 0; i < parts.Length; i++)
            {
                string partPath = parts[i].Replace("\\", "/");
                for (int j = 0; j < folders.Count; j++)
                {
                    var path = partPath + "/" + folders[j];
                    CheckFolderExists(path);
                }
            }
        }


        public static void AddMoreAudiosChildFolder(List<string> partNames, string rootPath)
        {
            List<string> folders = new List<string> { "Bgm", "Sound", "Voice" };
            Dictionary<string, string> audioPreInfos = new Dictionary<string, string>
            {
                {"Bgm"  ,Config.BgmPart },
                {"Sound",Config.SoundPart},
                {"Voice",Config.VoicePart},
            };
            var parts = Directory.GetDirectories(rootPath);

            var courseName = rootPath.Replace("Assets/HotFixPackage/", string.Empty);

            for (int i = 0; i < parts.Length; i++)
            {
                var partPath = parts[i].Replace("\\", "/");
                var partName = partPath.Replace("Assets/HotFixPackage/" + courseName + "/", string.Empty);

                var isContain= partNames.Contains(partName);

                if (!isContain)
                    continue;

                string audiosPath = partPath + "/Audios";
                for (int j = 0; j < folders.Count; j++)
                {
                    var path = audiosPath + "/" + folders[j];
                    var isExist = CheckFolderExists(path);
                    if (!isExist)
                    {
                        var key = folders[j];
                        string suffixName = string.Empty;
                        switch (key)
                        {
                            case "Bgm":
                                suffixName = "_Bgm_htp.prefab";
                                break;
                            case "Sound":
                                suffixName = "_Sound_htp.prefab";
                                break;
                            case "Voice":
                                suffixName = "_Voice_htp.prefab";
                                break;
                        }

                        var infoPath = audioPreInfos[folders[j]];
                        File.Copy(infoPath, partPath + "/AudiosPrefab/" + courseName + partName + suffixName);
                    }
                }
            }
        }

        public static void CreateMoreAudiosChildFolder(string rootPath)
        {
            List<string> folders = new List<string> { "Bgm", "Sound", "Voice" };
            Dictionary<string, string> audioPreInfos = new Dictionary<string, string>
            {
                {"Bgm"  ,Config.BgmPart },
                {"Sound",Config.SoundPart},
                {"Voice",Config.VoicePart},
            };
            var parts = Directory.GetDirectories(rootPath);

            var courseName = rootPath.Replace("Assets/HotFixPackage/", string.Empty);

            for (int i = 0; i < parts.Length; i++)
            {
                var partPath = parts[i].Replace("\\", "/");
                var partName = partPath.Replace("Assets/HotFixPackage/" + courseName + "/", string.Empty);              
                string audiosPath = partPath + "/Audios";
                for (int j = 0; j < folders.Count; j++)
                {
                    var path = audiosPath + "/" + folders[j];
                    var isExist = CheckFolderExists(path);
                    if (!isExist)
                    {
                        var key = folders[j];
                        string suffixName = string.Empty;
                        switch (key)
                        {
                            case "Bgm":
                                suffixName = "_Bgm_htp.prefab";
                                break;
                            case "Sound":
                                suffixName = "_Sound_htp.prefab";
                                break;
                            case "Voice":
                                suffixName = "_Voice_htp.prefab";
                                break;
                        }

                        var infoPath = audioPreInfos[folders[j]];
                        File.Copy(infoPath, partPath + "/AudiosPrefab/" + courseName + partName + suffixName);
                    }
                }
            }
        }


        public static void CopyDirectory(string sourceDirectory, string destDirectory,Action callBack =null)
        {
            //判断源目录和目标目录是否存在，如果不存在，则创建一个目录
            if (!Directory.Exists(sourceDirectory))
            {
                Directory.CreateDirectory(sourceDirectory);
            }
            if (!Directory.Exists(destDirectory))
            {
                Directory.CreateDirectory(destDirectory);
            }
            //拷贝文件
            CopyFile(sourceDirectory, destDirectory);
            //拷贝子目录       
            //获取所有子目录名称
            string[] directionName = Directory.GetDirectories(sourceDirectory);
            foreach (string directionPath in directionName)
            {
                //根据每个子目录名称生成对应的目标子目录名称
                string directionPathTemp = Path.Combine(destDirectory, directionPath.Substring(sourceDirectory.Length + 1));// destDirectory + "\\" + directionPath.Substring(sourceDirectory.Length + 1);
                                                                                                                            //递归下去
                CopyDirectory(directionPath, directionPathTemp);
            }
        }


        public static void CopyFile(string sourceDirectory, string destDirectory)
        {
            //获取所有文件名称
            string[] fileName = Directory.GetFiles(sourceDirectory);
            foreach (string filePath in fileName)
            {
                //根据每个文件名称生成对应的目标文件名称
                string filePathTemp = Path.Combine(destDirectory, filePath.Substring(sourceDirectory.Length + 1));// destDirectory + "\\" + filePath.Substring(sourceDirectory.Length + 1);
                                                                                                                  //若不存在，直接复制文件；若存在，覆盖复制
                if (File.Exists(filePathTemp))
                {
                    File.Copy(filePath, filePathTemp, true);
                }
                else
                {
                    File.Copy(filePath, filePathTemp);
                }
            }
        }


        public static void DeleteFolder(string path)
        {
            Directory.Delete(path, true);
        }
    }

}

 


