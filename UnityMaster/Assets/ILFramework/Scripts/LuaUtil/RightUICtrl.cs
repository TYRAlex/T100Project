using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ILFramework;
using System;
using LuaInterface;

public class RightUICtrl
{
    static LuaFunction _clickTalkCallback;
    public static LuaFunction clickTalkCallback
    {
        get { return _clickTalkCallback; }
        set
        {
            _clickTalkCallback = value;
            SoundManager.instance.SetLuaVoiceCallBack(value);
        }
    }

    public static void EnableTalkBtn(bool enable, GameObject sheild = null)
    {
        SoundManager.instance.sheild = sheild;
    }

    public static void ShowTalk(bool isShow)
    {
        SoundManager.instance.ShowVoiceBtn(isShow);
    }

    public static void ShowArrow(){ }

    public static void Show(bool isShow) { }

    static LuaFunction _clickRetryCallback;
    public static LuaFunction clickRetryCallback
    {
        get { return _clickRetryCallback; }
        set
        {
            _clickRetryCallback = value;
            LogicManager.instance.SetReplayEvent(_clickRetryCallback);
        }
    }

    public static void EnableRetryBtn(bool isOn, GameObject sheild = null)
    {
        LogicManager.instance.shiled = sheild;
    }

    public static void ShowRetryBtn(bool isShow)
    {
        LogicManager.instance.ShowReplayBtn(isShow);
    }
}
