using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace ILFramework
{
    public class ILDroper : MonoBehaviour
    {
        public int dropType = 0;
        public bool isActived = true;
        public int index = 0;
        public GameObject effect;
        Func<int, int, int, bool> luaAfter = null;
        Action<bool, int, int> luaEffect = null;
        Action<int> fail = null;
        // Use this for initialization
        void Awake()
        {
            if (effect) effect.SetActive(false);
        }

        //public override void Inited()
        //{
        //    base.Inited();
        //    RootPackage.PackageDomain.DelegateManager.RegisterFunctionDelegate<int, int, bool>();
        //    RootPackage.PackageDomain.DelegateManager.RegisterMethodDelegate<bool,int, int>();
        //}

        public bool doAfter(int type = 0)
        {
            if (!isActived) return false;
            //dropType = type;
            if (luaAfter != null)
            {
                return luaAfter.Invoke(type, index, dropType);
            }
            else
            {
                return true;
            }
        }

        public void DroperFail(int type)
        {
            if (fail!=null)            
                fail.Invoke(type);            
        }

        public void DoReset()
        {
            //dropType = 0;
            if (transform.childCount > 1)
            {
                if (effect) DestroyImmediate(transform.GetChild(1).gameObject);
            }
        }

        public void showEffect(bool isOn = true, int type = 0)
        {
            if (effect)
            {
                effect.SetActive(isOn);
                if (!isOn) return;
                if (luaEffect != null) luaEffect.Invoke(isOn, type, index);
            }
        }

        public void SetDropCallBack(Func<int, int, int, bool> after = null, Action<bool, int, int> effect = null,Action<int> failDroper=null)
        {
            luaAfter = after;
            luaEffect = effect;
            fail = failDroper;
        }
    }
}
