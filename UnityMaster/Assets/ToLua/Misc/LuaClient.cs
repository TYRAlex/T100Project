/*
Copyright (c) 2015-2017 topameng(topameng@qq.com)

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
*/
using UnityEngine;
using System.Collections.Generic;
using LuaInterface;
using System.Collections;
using System.IO;
using System;
using System.Threading;
#if UNITY_5_4_OR_NEWER
using UnityEngine.SceneManagement;
#endif

public class LuaClient : MonoBehaviour
{
    public static LuaClient Instance
    {
        get;
        protected set;
    }

    protected LuaState luaState = null;
    protected LuaLooper loop = null;
    protected LuaFunction levelLoaded = null;

    protected bool openLuaSocket = false;
    protected bool beZbStart = false;

    protected virtual LuaFileUtils InitLoader()
    {
        return LuaFileUtils.Instance;       
    }

    protected virtual void LoadLuaFiles()
    {
        OnLoadFinished();
    }

    protected virtual void OpenLibs()
    {
        luaState.OpenLibs(LuaDLL.luaopen_pb);
        luaState.OpenLibs(LuaDLL.luaopen_struct);
        luaState.OpenLibs(LuaDLL.luaopen_lpeg);
#if UNITY_STANDALONE_OSX || UNITY_EDITOR_OSX
        luaState.OpenLibs(LuaDLL.luaopen_bit);
#endif

        if (LuaConst.openLuaSocket)
        {
            OpenLuaSocket();            
        }        

        if (LuaConst.openLuaDebugger)
        {
            OpenZbsDebugger();
        }
    }

    public void OpenZbsDebugger(string ip = "localhost")
    {
        if (!Directory.Exists(LuaConst.zbsDir))
        {
            Debugger.LogWarning("ZeroBraneStudio not install or LuaConst.zbsDir not right");
            return;
        }

        if (!LuaConst.openLuaSocket)
        {                            
            OpenLuaSocket();
        }

        if (!string.IsNullOrEmpty(LuaConst.zbsDir))
        {
            luaState.AddSearchPath(LuaConst.zbsDir);
        }

        luaState.LuaDoString(string.Format("DebugServerIp = '{0}'", ip), "@LuaClient.cs");
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Socket_Core(IntPtr L)
    {        
        return LuaDLL.luaopen_socket_core(L);
    }

    [MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
    static int LuaOpen_Mime_Core(IntPtr L)
    {
        return LuaDLL.luaopen_mime_core(L);
    }

    protected void OpenLuaSocket()
    {
        LuaConst.openLuaSocket = true;

        luaState.BeginPreLoad();
        luaState.RegFunction("socket.core", LuaOpen_Socket_Core);
        luaState.RegFunction("mime.core", LuaOpen_Mime_Core);                
        luaState.EndPreLoad();                     
    }

    //cjson 比较特殊，只new了一个table，没有注册库，这里注册一下
    protected void OpenCJson()
    {
        luaState.LuaGetField(LuaIndexes.LUA_REGISTRYINDEX, "_LOADED");
        luaState.OpenLibs(LuaDLL.luaopen_cjson);
        luaState.LuaSetField(-2, "cjson");

        luaState.OpenLibs(LuaDLL.luaopen_cjson_safe);
        luaState.LuaSetField(-2, "cjson.safe");                               
    }

    protected virtual void CallMain()
    {
        LuaFunction main = luaState.GetFunction("Main");
        main.Call();
        main.Dispose();
        main = null;                
    }

    protected virtual void StartMain()
    {
        luaState.DoFile("Main.lua");
        levelLoaded = luaState.GetFunction("OnLevelWasLoaded");
        CallMain();
    }

    protected void StartLooper()
    {
        loop = gameObject.AddComponent<LuaLooper>();
        loop.luaState = luaState;
    }

    protected virtual void Bind()
    {        
        LuaBinder.Bind(luaState);
        //StartCoroutine(BindTest());
        DelegateFactory.Init();   
        LuaCoroutine.Register(luaState, this);        
    }

    IEnumerator BindTest()
    {
	    LuaState L = luaState;
	    
        float t = Time.realtimeSinceStartup;
		L.BeginModule(null);
		LuaInterface_DebuggerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		LuaProfilerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		LuaManagerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		ObjectClickEffectWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UIClickEffectWrap.Register(L);
		yield return new WaitForFixedUpdate();
		RightUICtrlWrap.Register(L);
		yield return new WaitForFixedUpdate();
		Object3DActionWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DrawableGunWrap.Register(L);
		yield return new WaitForFixedUpdate();
		GunFollowWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DrawableWrap.Register(L);
		yield return new WaitForFixedUpdate();
		GetXDeviceStateWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UGUIPointerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		ScrollPageWrap.Register(L);
		yield return new WaitForFixedUpdate();
		BellPaintViewWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.BeginModule("LuaInterface");
		yield return new WaitForFixedUpdate();
		LuaInterface_LuaInjectionStationWrap.Register(L);
		yield return new WaitForFixedUpdate();
		LuaInterface_InjectTypeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.BeginModule("DG");
		yield return new WaitForFixedUpdate();
		L.BeginModule("Tweening");
		yield return new WaitForFixedUpdate();
		DG_Tweening_DOTweenWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_TweenWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_SequenceWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_TweenerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_LoopTypeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_PathModeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_PathTypeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_RotateModeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		DG_Tweening_EaseWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.RegFunction("TweenCallback", LuaBinder.DG_Tweening_TweenCallback);
		yield return new WaitForFixedUpdate();
		L.BeginModule("Core");
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_float", LuaBinder.DG_Tweening_Core_DOGetter_float);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_float", LuaBinder.DG_Tweening_Core_DOSetter_float);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_double", LuaBinder.DG_Tweening_Core_DOGetter_double);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_double", LuaBinder.DG_Tweening_Core_DOSetter_double);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_int", LuaBinder.DG_Tweening_Core_DOGetter_int);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_int", LuaBinder.DG_Tweening_Core_DOSetter_int);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_uint", LuaBinder.DG_Tweening_Core_DOGetter_uint);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_uint", LuaBinder.DG_Tweening_Core_DOSetter_uint);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_long", LuaBinder.DG_Tweening_Core_DOGetter_long);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_long", LuaBinder.DG_Tweening_Core_DOSetter_long);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_ulong", LuaBinder.DG_Tweening_Core_DOGetter_ulong);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_ulong", LuaBinder.DG_Tweening_Core_DOSetter_ulong);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_string", LuaBinder.DG_Tweening_Core_DOGetter_string);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_string", LuaBinder.DG_Tweening_Core_DOSetter_string);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Vector2", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Vector2);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Vector2", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Vector2);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Vector3", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Vector3);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Vector3", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Vector3);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Vector4", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Vector4);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Vector4", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Vector4);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Quaternion", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Quaternion);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Quaternion", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Quaternion);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Color", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Color);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Color", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Color);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_Rect", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_Rect);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_Rect", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_Rect);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOGetter_UnityEngine_RectOffset", LuaBinder.DG_Tweening_Core_DOGetter_UnityEngine_RectOffset);
		yield return new WaitForFixedUpdate();
		L.RegFunction("DOSetter_UnityEngine_RectOffset", LuaBinder.DG_Tweening_Core_DOSetter_UnityEngine_RectOffset);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.BeginModule("UnityEngine");
		yield return new WaitForFixedUpdate();
		UnityEngine_ComponentWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_TransformWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_LightWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_MaterialWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_CameraWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_AudioSourceWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_RectTransformWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_LineRendererWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_BehaviourWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_MonoBehaviourWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_GameObjectWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_TrackedReferenceWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_ApplicationWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_PhysicsWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_ColliderWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_TimeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_TextureWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_Texture2DWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_ShaderWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_RendererWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_WWWWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_ScreenWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_CameraClearFlagsWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_AudioClipWrap.Register(L);
		UnityEngine_AssetBundleWrap.Register(L);
		UnityEngine_ParticleSystemWrap.Register(L);
		UnityEngine_AsyncOperationWrap.Register(L);
		UnityEngine_LightTypeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_SleepTimeoutWrap.Register(L);
		UnityEngine_AnimatorWrap.Register(L);
		UnityEngine_InputWrap.Register(L);
		UnityEngine_KeyCodeWrap.Register(L);
		UnityEngine_SkinnedMeshRendererWrap.Register(L);
		UnityEngine_SpaceWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_AnimationBlendModeWrap.Register(L);
		UnityEngine_QueueModeWrap.Register(L);
		UnityEngine_PlayModeWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_WrapModeWrap.Register(L);
		UnityEngine_QualitySettingsWrap.Register(L);
		UnityEngine_RenderSettingsWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_ResourcesWrap.Register(L);
		UnityEngine_RectWrap.Register(L);
		UnityEngine_SpriteWrap.Register(L);
		UnityEngine_RectOffsetWrap.Register(L);
		UnityEngine_CanvasWrap.Register(L);
		UnityEngine_DebugWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_BoxCollider2DWrap.Register(L);
		UnityEngine_Rigidbody2DWrap.Register(L);
		UnityEngine_PolygonCollider2DWrap.Register(L);
		UnityEngine_CollisionWrap.Register(L);
		UnityEngine_Collision2DWrap.Register(L);
		UnityEngine_AudioBehaviourWrap.Register(L);
		UnityEngine_Collider2DWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.BeginModule("UI");
		UnityEngine_UI_TextWrap.Register(L);
		UnityEngine_UI_ImageWrap.Register(L);
		UnityEngine_UI_InputFieldWrap.Register(L);
		UnityEngine_UI_ToggleWrap.Register(L);
		UnityEngine_UI_GridLayoutGroupWrap.Register(L);
		UnityEngine_UI_RawImageWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_UI_MaskableGraphicWrap.Register(L);
		UnityEngine_UI_GraphicWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_UI_SelectableWrap.Register(L);
		UnityEngine_UI_LayoutGroupWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.BeginModule("InputField");
		L.RegFunction("OnValidateInput", LuaBinder.UnityEngine_UI_InputField_OnValidateInput);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		L.BeginModule("Video");
		UnityEngine_Video_VideoPlayerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.BeginModule("VideoPlayer");
		L.RegFunction("EventHandler", LuaBinder.UnityEngine_Video_VideoPlayer_EventHandler);
		L.RegFunction("ErrorEventHandler", LuaBinder.UnityEngine_Video_VideoPlayer_ErrorEventHandler);
		yield return new WaitForFixedUpdate();
		L.RegFunction("TimeEventHandler", LuaBinder.UnityEngine_Video_VideoPlayer_TimeEventHandler);
		L.RegFunction("FrameReadyEventHandler", LuaBinder.UnityEngine_Video_VideoPlayer_FrameReadyEventHandler);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		L.BeginModule("EventSystems");
		UnityEngine_EventSystems_PointerEventDataWrap.Register(L);
		UnityEngine_EventSystems_UIBehaviourWrap.Register(L);
		yield return new WaitForFixedUpdate();
		UnityEngine_EventSystems_BaseEventDataWrap.Register(L);
		UnityEngine_EventSystems_AbstractEventDataWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.BeginModule("Events");
		L.RegFunction("UnityAction", LuaBinder.UnityEngine_Events_UnityAction);
		L.RegFunction("UnityAction_Ximmerse_InputSystem_ControllerButton", LuaBinder.UnityEngine_Events_UnityAction_Ximmerse_InputSystem_ControllerButton);
		L.RegFunction("UnityAction_UnityEngine_Vector3", LuaBinder.UnityEngine_Events_UnityAction_UnityEngine_Vector3);
		yield return new WaitForFixedUpdate();
		L.RegFunction("UnityAction_UnityEngine_Vector3_UnityEngine_Vector3_UnityEngine_Vector3", LuaBinder.UnityEngine_Events_UnityAction_UnityEngine_Vector3_UnityEngine_Vector3_UnityEngine_Vector3);
		L.EndModule();
		L.BeginModule("Camera");
		L.RegFunction("CameraCallback", LuaBinder.UnityEngine_Camera_CameraCallback);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.BeginModule("RectTransform");
		L.RegFunction("ReapplyDrivenProperties", LuaBinder.UnityEngine_RectTransform_ReapplyDrivenProperties);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.BeginModule("Application");
		L.RegFunction("AdvertisingIdentifierCallback", LuaBinder.UnityEngine_Application_AdvertisingIdentifierCallback);
		yield return new WaitForFixedUpdate();
		L.RegFunction("LowMemoryCallback", LuaBinder.UnityEngine_Application_LowMemoryCallback);
		L.RegFunction("LogCallback", LuaBinder.UnityEngine_Application_LogCallback);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.BeginModule("AudioClip");
		L.RegFunction("PCMReaderCallback", LuaBinder.UnityEngine_AudioClip_PCMReaderCallback);
		L.RegFunction("PCMSetPositionCallback", LuaBinder.UnityEngine_AudioClip_PCMSetPositionCallback);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.BeginModule("Canvas");
		L.RegFunction("WillRenderCanvases", LuaBinder.UnityEngine_Canvas_WillRenderCanvases);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		L.BeginModule("LuaFramework");
		yield return new WaitForEndOfFrame();
		LuaFramework_LuaBehaviourWrap.Register(L);
		LuaFramework_UtilWrap.Register(L);
		LuaFramework_DragerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		LuaFramework_DroperWrap.Register(L);
		LuaFramework_ModelClickWrap.Register(L);
		L.EndModule();
		L.BeginModule("ILFramework");
		yield return new WaitForFixedUpdate();
		ILFramework_SoundManagerWrap.Register(L);
		ILFramework_SpineManagerWrap.Register(L);
		ILFramework_ResourceManagerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		ILFramework_XDeviceManagerWrap.Register(L);
		ILFramework_LogicManagerWrap.Register(L);
		ILFramework_ConnectAndroidWrap.Register(L);
		ILFramework_FunctionOf3DofWrap.Register(L);
		ILFramework_Manager_LuaManagerWrap.Register(L);
		ILFramework_BaseWrap.Register(L);
		yield return new WaitForFixedUpdate();
		ILFramework_Manager_ILFramework_SoundManagerWrap.Register(L);
		ILFramework_Manager_ILFramework_SpineManagerWrap.Register(L);
		ILFramework_Manager_ILFramework_ResourceManagerWrap.Register(L);
		yield return new WaitForFixedUpdate();
		ILFramework_Manager_ILFramework_XDeviceManagerWrap.Register(L);
		ILFramework_Manager_ILFramework_LogicManagerWrap.Register(L);
		L.BeginModule("FunctionOf3Dof");
		L.RegFunction("The3DofCallBack", LuaBinder.ILFramework_FunctionOf3Dof_The3DofCallBack);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.BeginModule("System");
		L.RegFunction("Action", LuaBinder.System_Action);
		L.RegFunction("Predicate_int", LuaBinder.System_Predicate_int);
		L.RegFunction("Action_int", LuaBinder.System_Action_int);
		L.RegFunction("Comparison_int", LuaBinder.System_Comparison_int);
		yield return new WaitForFixedUpdate();
		L.RegFunction("Func_int_int", LuaBinder.System_Func_int_int);
		L.RegFunction("Action_bool", LuaBinder.System_Action_bool);
		L.RegFunction("Func_bool", LuaBinder.System_Func_bool);
		L.RegFunction("Action_UnityEngine_AsyncOperation", LuaBinder.System_Action_UnityEngine_AsyncOperation);
		yield return new WaitForFixedUpdate();
		L.RegFunction("Action_int_int", LuaBinder.System_Action_int_int);
		L.BeginModule("Collections");
		System_Collections_ArrayListWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		L.BeginModule("Vectrosity");
		Vectrosity_VectorObject2DWrap.Register(L);
		yield return new WaitForFixedUpdate();
		Vectrosity_VectorLineWrap.Register(L);
		yield return new WaitForEndOfFrame();
		L.EndModule();
		L.BeginModule("bell");
		L.BeginModule("ai");
		L.BeginModule("t100");
		L.BeginModule("remotecontrol");
		bell_ai_t100_remotecontrol_XDeviceSupportWrap.Register(L);
		yield return new WaitForFixedUpdate();
		L.BeginModule("XDeviceSupport");
		L.RegFunction("XPointChangeEvent", LuaBinder.bell_ai_t100_remotecontrol_XDeviceSupport_XPointChangeEvent);
		L.EndModule();
		L.EndModule();
		L.EndModule();
		L.EndModule();
		yield return new WaitForFixedUpdate();
		L.EndModule();
		L.EndModule();
		L.BeginPreLoad();
		yield return new WaitForFixedUpdate();
		L.AddPreLoad("UnityEngine.MeshRenderer", LuaBinder.LuaOpen_UnityEngine_MeshRenderer, typeof(UnityEngine.MeshRenderer));
		L.AddPreLoad("UnityEngine.BoxCollider", LuaBinder.LuaOpen_UnityEngine_BoxCollider, typeof(UnityEngine.BoxCollider));
		L.AddPreLoad("UnityEngine.MeshCollider", LuaBinder.LuaOpen_UnityEngine_MeshCollider, typeof(UnityEngine.MeshCollider));
		yield return new WaitForFixedUpdate();
		L.AddPreLoad("UnityEngine.SphereCollider", LuaBinder.LuaOpen_UnityEngine_SphereCollider, typeof(UnityEngine.SphereCollider));
		L.AddPreLoad("UnityEngine.CharacterController", LuaBinder.LuaOpen_UnityEngine_CharacterController, typeof(UnityEngine.CharacterController));
		L.AddPreLoad("UnityEngine.CapsuleCollider", LuaBinder.LuaOpen_UnityEngine_CapsuleCollider, typeof(UnityEngine.CapsuleCollider));
		L.AddPreLoad("UnityEngine.Animation", LuaBinder.LuaOpen_UnityEngine_Animation, typeof(UnityEngine.Animation));
		yield return new WaitForEndOfFrame();
		L.AddPreLoad("UnityEngine.AnimationClip", LuaBinder.LuaOpen_UnityEngine_AnimationClip, typeof(UnityEngine.AnimationClip));
		L.AddPreLoad("UnityEngine.AnimationState", LuaBinder.LuaOpen_UnityEngine_AnimationState, typeof(UnityEngine.AnimationState));
		yield return new WaitForFixedUpdate();
		L.AddPreLoad("UnityEngine.BlendWeights", LuaBinder.LuaOpen_UnityEngine_BlendWeights, typeof(UnityEngine.BlendWeights));
		L.AddPreLoad("UnityEngine.RenderTexture", LuaBinder.LuaOpen_UnityEngine_RenderTexture, typeof(UnityEngine.RenderTexture));
		L.AddPreLoad("UnityEngine.Rigidbody", LuaBinder.LuaOpen_UnityEngine_Rigidbody, typeof(UnityEngine.Rigidbody));
		L.EndPreLoad();
		Debugger.Log("Register lua type cost time: {0}", Time.realtimeSinceStartup - t);
		DelegateFactory.Init();   
		LuaCoroutine.Register(luaState, this); 
		LoadLuaFiles();    
    }

    protected void Init()
    {        
        InitLoader();
        luaState = new LuaState();
        OpenLibs();
        luaState.LuaSetTop(0);
        Bind();   
        LoadLuaFiles();        
    }

    object lockd=new object();

    void StartThread()
    {
        Thread athread = new Thread(new ThreadStart(GoThread));

        athread.IsBackground = false;//防止后台线成。相反需要后台线程就设为false
        athread.Start();
    }

    void GoThread()
    {
        int index = 0;
        while (true)
        {
            lock (lockd)//防止其他线程访问当前线程使用的数据
            {
                Debug.Log("in thread");
                Bind(); 
                // index++;
                // if (index == 100)
                // {
                //     Thread.Sleep(10000);//   将当前线程挂起指定的时间 毫秒  时间结束后 继续执行下一步  和yield类似
                // }
                // else if (index == 200)
                // {
                //     break;//该函数执行完自动结束该线程
                // }
            }
        }
    }

    protected void Awake()
    {
        Instance = this;
        Init();

#if UNITY_5_4_OR_NEWER
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif        
    }

    protected virtual void OnLoadFinished()
    {
        luaState.Start();
        StartLooper();
        StartMain();        
    }

    void OnLevelLoaded(int level)
    {
        if (levelLoaded != null)
        {
            levelLoaded.BeginPCall();
            levelLoaded.Push(level);
            levelLoaded.PCall();
            levelLoaded.EndPCall();
        }

        if (luaState != null)
        {            
            luaState.RefreshDelegateMap();
        }
    }

#if UNITY_5_4_OR_NEWER
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        OnLevelLoaded(scene.buildIndex);
    }
#else
    protected void OnLevelWasLoaded(int level)
    {
        OnLevelLoaded(level);
    }
#endif

    public virtual void Destroy()
    {
#if UNITY_EDITOR
#else
        if (luaState != null)
        {
#if UNITY_5_4_OR_NEWER
            SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
            luaState.Call("OnApplicationQuit", false);
            DetachProfiler();
            LuaState state = luaState;
            luaState = null;

            if (levelLoaded != null)
            {
                levelLoaded.Dispose();
                levelLoaded = null;
            }

            if (loop != null)
            {
                loop.Destroy();
                loop = null;
            }

            state.Dispose();
            Instance = null;
        }
#endif
    }

    protected void OnDestroy()
    {
        Debug.Log(" LuaClient  OnDestroy ! ");
        Destroy();
    }

    protected void OnApplicationQuit()
    {
        Debug.Log(" LuaClient  OnApplicationQuit ! ");
        //Destroy();
    }

    public static LuaState GetMainState()
    {
        return Instance.luaState;
    }

    public LuaLooper GetLooper()
    {
        return loop;
    }

    LuaTable profiler = null;

    public void AttachProfiler()
    {
        if (profiler == null)
        {
            profiler = luaState.Require<LuaTable>("UnityEngine.Profiler");
            profiler.Call("start", profiler);
        }
    }
    public void DetachProfiler()
    {
        if (profiler != null)
        {
            profiler.Call("stop", profiler);
            profiler.Dispose();
            LuaProfiler.Clear();
        }
    }
}
