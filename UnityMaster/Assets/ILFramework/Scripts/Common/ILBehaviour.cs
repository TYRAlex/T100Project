using ILRuntime.CLR.Method;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using ILRuntime.Runtime.Enviorment;
using System;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace ILFramework
{
    public class ILBehavior : Base
    {

        private AppDomain currentDomain;
        private object ilObject;
        private string hotClassName;

        public GameObject gamePrefab;
        public object IlObject { get => ilObject; set => ilObject = value; }

        public void InitILBehaviour(AppDomain domain, string ClassName)
        {

            currentDomain = domain;

            hotClassName = AppConst.HOTFIXNAMESPACE + ClassName.Replace("/","");

            IlObject = currentDomain.Instantiate(hotClassName, new object[] { gameObject });


        }

        private void Start()
        {
            //OnShow();
        }

        public void OnShow()
        {
            if (gamePrefab.activeSelf)
                invoke("Start", new object[] { gamePrefab });
        }

        //protected virtual void Startin()
        //{
        //    if (gamePrefab.activeSelf)
        //        invoke("Start", new object[] { gamePrefab });
        //}

        private void Update()
        {
            if (gamePrefab.activeSelf)
                invoke("Update", null);
        }

        private void FixedUpdate()
        {
            if (gamePrefab.activeSelf)
                invoke("FixedUpdate", null);
        }

        private void OnApplicationPause(bool pause)
        {
            invoke("OnApplicationPause", new object[] { pause });
        }


        private void OnDisable()
        {
            invoke("OnDisable", null);
        }


        private void OnDestroy()
        {
            invoke("OnDestroy", null);
        }


        private void OnApplicationQuit()
        {
            invoke("OnApplicationQuit", null);
        }


        private void invoke(string method, object[] iparams)
        {
            if (currentDomain == null)
            {
                //Debug.LogError("null");
                return;
            }


            currentDomain.Invoke(hotClassName, method, IlObject, iparams);
        }


    }
}