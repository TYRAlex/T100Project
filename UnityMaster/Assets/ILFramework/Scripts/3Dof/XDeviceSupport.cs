using LuaInterface;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace bell.ai.t100.remotecontrol
{
    public class XDeviceSupport : MonoBehaviour
    {

        public GameObject touch_point;
        Sim3dof dof;


        public delegate void XPointChangeEvent(Vector3 position);

        public XPointChangeEvent changeEvent;

        // Use this for initialization
        private void Awake()
        {
            DelegateFactory.Init();
        }
        void Start()
        {

            //touchPointPosChange = new XDevicePosEvent();

            changeEvent = new XPointChangeEvent((a)=> { });

            dof = FindObjectOfType<Sim3dof>();
            touch_point = dof.touchPoint;
            lastPosition = touch_point.transform.position;
            //touchPointPosChange.Invoke(dof.mousePoint);
            changeEvent(dof.mousePoint);
        }


        Vector3 lastPosition;

        // Update is called once per frame
        void Update()
        {
            if (lastPosition == touch_point.transform.position)
                return;

            lastPosition = touch_point.transform.position;           
            changeEvent(dof.mousePoint);


        }



        /*
        public void addLuaListener(LuaFunction luaFunction)
        {

            touchPointPosChange.

            touchPointPosChange.AddListener((a)=> {
                if(!luaFunction.IsAlive)
                    luaFunction.Call<Vector2>(a);

            });
            Debug.LogError("绑定成功--------------------------");
        }
        */





    }


   



    public class XDevicePosEvent : UnityEvent<Vector3>
    {
        
    }

}
