using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TLDroper : MonoBehaviour
{
    public int dropType = 0;
    public bool isActived = true;
    public int index = 0;
    public GameObject effect;
    Func<int, int, int, bool> luaAfter = null;
    Action<bool, int, int> luaEffect = null;
    Action<int> fail = null;

    void Awake()
    {
        if (effect) effect.SetActive(false);
    }


    public bool doAfter(int type = 0)
    {
        if (!isActived) return false;
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
        if (fail != null)
            fail.Invoke(type);
    }


    public void DoReset()
    {
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

    public void SetDropCallBack(Func<int, int, int, bool> after = null, Action<bool, int, int> effect = null, Action<int> failDroper = null)
    {
        luaAfter = after;
        luaEffect = effect;
        fail = failDroper;
    }
}
