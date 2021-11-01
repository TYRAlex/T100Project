using ILRuntime.Runtime.Enviorment;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


namespace ILFramework
{
    public class HotfixPackage
    {
        private string _path;
        private string name;
        private GameObject rootObject;
        private AppDomain packageDomain;
        private MemoryStream fs;
        private MemoryStream p;
        private object hotfixInstance;

        public bool connectDevices;
        public GameObject RootObject { get => rootObject; set => rootObject = value; }
        public AppDomain PackageDomain { get => packageDomain; set => packageDomain = value; }
        public MemoryStream Fs { get => fs; set => fs = value; }
        public MemoryStream P { get => p; set => p = value; }
        public string Path { get => _path; set => _path = value;}
        public string Name { get => name; set => name = value; }   // 课程包的路径名称
        public string CourseName { get => name.Replace("/", ""); }  // 课程包的正确名称
        public ILBehavior IlBehaviorIns;
        public object HotfixInstance { get => hotfixInstance; set => hotfixInstance = value; }

        public void DestoryHotfixpackage()
        {
            if (Fs != null)
            {
                Fs.Dispose();
                Fs = null;
            }
            if (P != null)
            {
                P.Dispose();
                P = null;
            }
            if (PackageDomain!=null)
            {
                PackageDomain.DebugService.StopDebugService();
            }

            GameObject.DestroyImmediate(RootObject);
            RootObject = null;
            LuaManager.instance.RemoveLuaZipMap(CourseName);
        }
    }
}

