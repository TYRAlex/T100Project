using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace ILFramework
{
    public class RootILBehaviour : ILBehavior
    {
        private HotfixPackage rootPackage;

        private Dictionary<string, ChildILBehaviour> allChildren = new Dictionary<string, ChildILBehaviour>();

        public HotfixPackage RootPackage { get => rootPackage; set => rootPackage = value; }
//解决1.0课程点击没有语音的问题
        public ObjectClickEffect[] objectClickEffects;
        public void SetHotfixPackage(HotfixPackage package)
        {
            gamePrefab = package.RootObject;
            package.PackageDomain.DebugService.StartDebugService(56000);
            this.rootPackage = package;
            this.InitILBehaviour(package.PackageDomain, package.Name);
            objectClickEffects =  gamePrefab.transform.GetComponentsInChildren<ObjectClickEffect>(true);
            for (int i = 0; i < objectClickEffects.Length; i++)
            {
                objectClickEffects[i].is3DClick = true;
            }
        }


    }
}