using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LuaFramework
{
    public class Droper : MonoBehaviour
    {
        public int dropType = 0;
        public bool isActived = true;
        public int index = 0;
        public GameObject effect;
        LuaFunction luaAfter = null;
        LuaFunction luaEffect = null;

        // Use this for initialization
        void Awake()
        {
            if (effect) effect.SetActive(false);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public bool doAfter(int type = 0)
        {
            if (!isActived) return false;
            //dropType = type;
            if (luaAfter != null)
            {
                return luaAfter.Invoke<int, int, bool>(type, index);
            }
            else
            {
                return true;
            }
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
                if (luaEffect != null) luaEffect.Call<bool, int, int>(isOn, type, index);
            }
        }

        public void SetDropCallBack(LuaFunction after = null, LuaFunction effect = null)
        {
            luaAfter = after;
            luaEffect = effect;
        }
    }
}
