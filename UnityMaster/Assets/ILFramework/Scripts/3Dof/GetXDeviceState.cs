using LuaInterface;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Ximmerse.InputSystem;
using Ximmerse.VR;

public class GetXDeviceState : MonoBehaviour
{

    public Transform target;

    public ControllerType source;
    public string sourceName;

    public bool trackPosition = true;
    public bool trackRotation = true;

    public bool checkParent = false;
    public bool canRecenter = true;
    public LuaFunction clickCallback = null;
    public LuaFunction clickDownCallback = null;
    public LuaFunction clickUpCallback = null;
    public LuaFunction getRotateCallBack = null;
    public LuaFunction getSensorCallBack = null;

    public UnityAction<ControllerButton> uclickCallback = null;
    public UnityAction<ControllerButton> uclickDownCallback = null;
    public UnityAction<ControllerButton> uclickUpCallback = null;
    public UnityAction<Vector3> ugetRotateCallBack = null;
    public UnityAction<Vector3, Vector3, Vector3> ugetSensorCallBack = null;

    [System.NonSerialized] protected ControllerInput m_ControllerInput;
    public ControllerInput controllerInput { get { return m_ControllerInput; } }
    [System.NonSerialized] protected TrackingResult m_PrevTrackingResult;

    bool isExit = false;

    // Use this for initialization
    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (checkParent)
        {
            Transform p = VRContext.GetAnchor(VRNode.TrackingSpace);
            if (p != null)
            {
                target.SetParent(p, true);
            }
        }
        //
        if (source == ControllerType.None && string.IsNullOrEmpty(sourceName))
        {
            if (name.ToLower().IndexOf("left") != -1)
            {
                source = ControllerType.LeftController;
            }
            else if (name.ToLower().IndexOf("right") != -1)
            {
                source = ControllerType.RightController;
            }
        }
        if (source != ControllerType.None)
        {
            m_ControllerInput = ControllerInputManager.instance.GetControllerInput(source);
        }
        else if (!string.IsNullOrEmpty(sourceName))
        {
            m_ControllerInput = ControllerInputManager.instance.GetControllerInput(sourceName);
        }
        //
        VRNode node = VRNode.None;
        switch (source)
        {
            case ControllerType.LeftController:
                node = VRNode.LeftHand;
                break;
            case ControllerType.RightController:
                node = VRNode.RightHand;
                break;
        }
        if (node != VRNode.None)
        {
            VRContext.SetAnchor(node, target);
        }
        //
        if (m_ControllerInput != null && canRecenter)
        {
            // Like SteamVR,hmd and controllers don't need to reset yaw angle.
            canRecenter = !m_ControllerInput.isAbsRotation;
        }
        //
        if (canRecenter)
        {
            // Invoke Recenter() on VRContext recenter event.
            VRContext ctx = VRContext.main;
            if (ctx != null)
            {
                ctx.onRecenter += Recenter;
            }
        }
        //
        isExit = false;
        return;
    }

    // Update is called once per frame
    void Update()
    {
        if (isExit) return;
        //遍历枚举
        foreach (ControllerButton buttonIndex in Enum.GetValues(typeof(ControllerButton)))
        {
            if (m_ControllerInput.GetButton(buttonIndex))
            {
                if (clickCallback != null) clickCallback.Call((int)buttonIndex);
                if (uclickCallback != null) uclickCallback.Invoke(buttonIndex);
            }
            if (m_ControllerInput.GetButtonDown(buttonIndex))
            {
                if (clickDownCallback != null) clickDownCallback.Call((int)buttonIndex);
                if (uclickDownCallback != null) uclickDownCallback.Invoke(buttonIndex);
            }
            if (m_ControllerInput.GetButtonUp(buttonIndex))
            {
                if (clickUpCallback != null) clickUpCallback.Call((int)buttonIndex);
                if (uclickUpCallback != null) uclickUpCallback.Invoke(buttonIndex);
            }
        }
        if (getRotateCallBack != null)
        {
            getRotateCallBack.Call(GetRotation().x, GetRotation().y, GetRotation().z);
        }
        if (ugetRotateCallBack != null) ugetRotateCallBack.Invoke(GetRotation());
        if (getSensorCallBack != null) getSensorCallBack.Call(GetRotation(), m_ControllerInput.GetAccelerometer(), m_ControllerInput.GetGyroscope());
        if (ugetSensorCallBack != null) ugetSensorCallBack.Invoke(GetRotation(), m_ControllerInput.GetAccelerometer(), m_ControllerInput.GetGyroscope());
    }

    /// <summary>
	/// Get transform informations.
	/// </summary>
	void GetTransform(ref Vector3 position, ref Quaternion rotation)
    {
        //
        if (m_ControllerInput != null)
        {
            position = m_ControllerInput.GetPosition();
            rotation = m_ControllerInput.GetRotation();
        }
    }

    public Vector3 GetRotation()
    {
        Vector3 r = new Vector3();
        try
        {
            r = m_ControllerInput.GetRotation().eulerAngles;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return r;
    }

    public Vector3 GetPosition()
    {
        Vector3 r = new Vector3();
        try
        {
            m_ControllerInput.GetPosition();
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
        return r;
    }

    /// <summary>
    /// Resets the yaw of this object.
    /// </summary>
    public void Recenter()
    {
        if (m_ControllerInput != null)
        {
            m_ControllerInput.Recenter();
        }
    }

    public DeviceConnectionState GetDeviceState()
    {
        return m_ControllerInput.connectionState;
    }

    /// <summary>
    /// 释放回调设备，防止报错
    /// </summary>
    public void ReleaseCallback()
    {
        clickCallback = null;
        clickDownCallback = null;
        clickUpCallback = null;
        getRotateCallBack = null;
        getSensorCallBack = null;
    }

    public void Exit()
    {
        isExit = true;
        Debug.Log("Exit 3dof:" + isExit);
        m_ControllerInput = null;
        XDevicePlugin.Exit();
    }
}
