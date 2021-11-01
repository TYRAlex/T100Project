using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MonoScripts : MonoBehaviour
{

    public Action AwakeCallBack;
    public Action OnEnableCallBack;
    public Action StartCallBack;
    public Action FixedUpdateCallBack;
    public Action UpdateCallBack;
    public Action LateUpdateCallBack;
    public Action OnDisableCallBack;
    public Action OnDestroyCallBack;

    void DeleteAllCallBack()
    {
        if (AwakeCallBack != null)
            AwakeCallBack = null;
        if (OnEnableCallBack != null)
            OnEnableCallBack = null;
        if (StartCallBack != null)
            StartCallBack = null;
        if (FixedUpdateCallBack != null)
            FixedUpdateCallBack = null;
        if (UpdateCallBack != null)
            UpdateCallBack = null;
        if (LateUpdateCallBack != null)
            LateUpdateCallBack = null;
        if (OnDisableCallBack != null)
            OnDisableCallBack = null;
        if (OnDestroyCallBack != null)
            OnDestroyCallBack = null;
    }

    public void Awake() { AwakeCallBack?.Invoke(); }
    public void OnEnable() { OnEnableCallBack?.Invoke(); }

    public void Start()
    {
        StartCallBack?.Invoke();
    }

    public void FixedUpdate()
    {
        FixedUpdateCallBack?.Invoke();
    }
    public void Update()
    {
        UpdateCallBack?.Invoke();
    }



    public void LateUpdate()
    {
        LateUpdateCallBack?.Invoke();
    }

    public void OnDisable()
    {
        OnDisableCallBack?.Invoke();
        DeleteAllCallBack();
    }

    public void OnDestroy()
    {
        OnDestroyCallBack?.Invoke();
        DeleteAllCallBack();
    }

    
}
