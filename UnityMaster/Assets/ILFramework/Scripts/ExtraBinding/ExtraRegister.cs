using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using ILRuntime.Runtime.Enviorment;
using System;

namespace ILFramework
{
    public class ExtraRegister
    {
        public static void RegisterDelegate(ILRuntime.Runtime.Enviorment.AppDomain appDomain)
        {
            // 注册委托
            appDomain.DelegateManager.RegisterMethodDelegate<float, float, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector3, int, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<Vector3, int, int, bool>();
            appDomain.DelegateManager.RegisterFunctionDelegate<int, int, int, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<bool, int, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<Collision2D, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<Collision, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<Collider2D, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<Collider, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject, int>();
            appDomain.DelegateManager.RegisterMethodDelegate<int>();
            appDomain.DelegateManager.RegisterMethodDelegate<GameObject>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, ElapsedEventArgs>();
            appDomain.DelegateManager.RegisterMethodDelegate<string>();
            appDomain.DelegateManager.RegisterMethodDelegate<object, string>();
            appDomain.DelegateManager.RegisterMethodDelegate<object[]>();
            appDomain.DelegateManager.RegisterMethodDelegate<bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<float>();

            appDomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.GameObject, System.Boolean>();
            appDomain.DelegateManager.RegisterFunctionDelegate<object, object, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<Spine.TrackEntry, Spine.Event>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();
            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3, bool>();
            appDomain.DelegateManager.RegisterMethodDelegate<Spine.TrackEntry>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.String, System.Collections.Generic.List<System.String>>();
            appDomain.DelegateManager.RegisterMethodDelegate<System.String, System.String>();

            
            appDomain.DelegateManager.RegisterDelegateConvertor<ILFramework.Scripts.Utility.TabletWebsocketController.DinosaurMoveDelegate>((act) =>
            {
                return new ILFramework.Scripts.Utility.TabletWebsocketController.DinosaurMoveDelegate((DinosaurName, steps) =>
                {
                    ((Action<System.String, System.Collections.Generic.List<System.String>>)act)(DinosaurName, steps);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<ILFramework.Scripts.Utility.TabletWebsocketController.MyAction>((act) =>
            {
                return new ILFramework.Scripts.Utility.TabletWebsocketController.MyAction((device, msg) =>
                {
                    ((Action<System.String, System.String>)act)(device, msg);
                });
            });



            appDomain.DelegateManager.RegisterDelegateConvertor<System.Predicate<UnityEngine.GameObject>>((act) =>
            {
                return new System.Predicate<UnityEngine.GameObject>((obj) =>
                {
                    return ((Func<UnityEngine.GameObject, System.Boolean>)act)(obj);
                });
            });

            

            appDomain.DelegateManager.RegisterDelegateConvertor<Spine.AnimationState.TrackEntryEventDelegate>((act) =>
            {
                return new Spine.AnimationState.TrackEntryEventDelegate((trackEntry, e) =>
                {
                    ((Action<Spine.TrackEntry, Spine.Event>)act)(trackEntry, e);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
            {
                return new UnityEngine.Events.UnityAction(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Boolean>((arg0) =>
                {
                    ((Action<System.Boolean>)act)(arg0);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<ILFramework.FunctionOf3Dof.The3DofCallBack>((act) =>
            {
                return new ILFramework.FunctionOf3Dof.The3DofCallBack(() =>
                {
                    ((Action)act)();
                });
            });


            appDomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Single>>((act) =>
            {
                return new UnityEngine.Events.UnityAction<System.Single>((arg0) =>
                {
                    ((Action<System.Single>)act)(arg0);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<ILFramework.MesManager.EventMgr>((act) =>
            {
                return new ILFramework.MesManager.EventMgr((param) =>
                {
                    ((Action<System.Object[]>)act)(param);
                });
            });


            appDomain.DelegateManager.RegisterDelegateConvertor<global::PointerClickListener.VoidDelegate>((act) =>
           {
               return new global::PointerClickListener.VoidDelegate((go) =>
               {
                   ((Action<UnityEngine.GameObject>)act)(go);
               });
           });

            // 委托适配
            appDomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
            {
                return new DG.Tweening.TweenCallback(() =>
                {
                    ((Action)act)();
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<EventHandler<string>>((act) =>
            {
                return new EventHandler<string>((sender, e) =>
                {
                    ((Action<object, string>)act)(sender, e);
                });
            });

            appDomain.RegisterCrossBindingAdaptor(new CoroutineAdapter());

            appDomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.PointerEventData>();

            appDomain.DelegateManager.RegisterDelegateConvertor<global::UIEventListener.VoidDelegate2>((act) =>
            {
                return new global::UIEventListener.VoidDelegate2((eventData) =>
                {
                    ((Action<UnityEngine.EventSystems.PointerEventData>)act)(eventData);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<global::EventDispatcher.CollisionHandler2D>((act) =>
            {
                return new global::EventDispatcher.CollisionHandler2D((c, time) =>
                {
                    ((Action<UnityEngine.Collision2D, System.Int32>)act)(c, time);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<global::EventDispatcher.TriggerHandler2D>((act) =>
            {
                return new global::EventDispatcher.TriggerHandler2D((other, time) =>
                {
                    ((Action<UnityEngine.Collider2D, System.Int32>)act)(other, time);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<global::EventDispatcher.CollisionHandler>((act) =>
            {
                return new global::EventDispatcher.CollisionHandler((c, time) =>
                {
                    ((Action<UnityEngine.Collision, System.Int32>)act)(c, time);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<global::EventDispatcher.TriggerHandler>((act) =>
            {
                return new global::EventDispatcher.TriggerHandler((other, time) =>
                {
                    ((Action<UnityEngine.Collider, System.Int32>)act)(other, time);
                });
            });
            appDomain.DelegateManager.RegisterDelegateConvertor<global::EventDispatcher.EventHandler>((act) =>
            {
                return new global::EventDispatcher.EventHandler((e) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(e);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<global::UIEventListener.VoidDelegate>((act) =>
            {
                return new global::UIEventListener.VoidDelegate((go) =>
                {
                    ((Action<UnityEngine.GameObject>)act)(go);
                });
            });

            appDomain.DelegateManager.RegisterDelegateConvertor<Spine.AnimationState.TrackEntryDelegate>((act) =>
            {
                return new Spine.AnimationState.TrackEntryDelegate((trackEntry) =>
                {
                    ((Action<Spine.TrackEntry>)act)(trackEntry);
                });
            });
        }
    }
}
