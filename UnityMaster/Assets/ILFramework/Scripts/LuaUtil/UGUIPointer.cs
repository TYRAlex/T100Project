using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using LuaInterface;

public class UGUIPointer : MonoBehaviour,IPointerUpHandler,IPointerDownHandler,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler {
    public int index = 0;
    public LuaFunction OnPointerClickLua;
    public LuaFunction OnPointerDownLua;
    public LuaFunction OnPointerEnterLua;
    public LuaFunction OnPointerExitLua;
    public LuaFunction OnPointerUpLua;

    public void OnPointerClick(PointerEventData eventData)
    {   
        if (OnPointerClickLua != null)
        {
            OnPointerClickLua.Call<PointerEventData, int>(eventData, index);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (OnPointerDownLua != null)
        {
            OnPointerDownLua.Call<PointerEventData, int>(eventData, index);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (OnPointerEnterLua != null)
        {
            OnPointerEnterLua.Call<PointerEventData, int>(eventData, index);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (OnPointerExitLua != null)
        {
            OnPointerExitLua.Call<PointerEventData, int>(eventData, index);
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (OnPointerUpLua != null)
        {
            OnPointerUpLua.Call<PointerEventData, int>(eventData, index);
        }
    }
}
