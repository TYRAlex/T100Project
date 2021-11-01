using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LuaInterface;
using UnityEngine.EventSystems;

namespace LuaFramework
{
    public class ModelClick : MonoBehaviour
    { 
        public bool isActive = false;
        public GameObject modelGo;
        public GameObject[] hitGos;

        public bool horiztalLock = false;
        public bool verticalLock = false;

        LuaFunction clickFunc = null;
        bool isContinue = true;

        Vector3 modelOriPos;
        Vector3 modelOriRot;

        public float speedx = 10f;
        public float speedy = 10f;
        public float deltaTime = 0.02f;
        float anglex = 0;
        float angley = 0;

        public float move_x = 5;
        public float move_y = 5;

        private Vector3 curPos;
        private bool isMove;
        private bool isDown = false;//是否为点击，而不是拖动
        private bool isStart = true;//是否开始
        private void Start()
        {
            modelOriPos = modelGo.transform.localPosition;
            modelOriRot = modelGo.transform.localEulerAngles;
        }

        private void Update()
        {
            if (!isActive)
            {
                isStart = false;
                isDown = false;
                return;
            }

            if (Input.GetMouseButtonDown(0))
            {
                isDown = true;
                isStart = true;
                curPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0) && isStart)
            {
                if (!horiztalLock)
                {
                    float x = Input.mousePosition.x - curPos.x;
                    anglex += x * speedx * deltaTime;
                    isMove = Mathf.Abs(x) > move_x;
                    if (isMove) isDown = false;
                }
                if (!verticalLock)
                {
                    float y = Input.mousePosition.y - curPos.y;
                    angley -= y * speedy * deltaTime;
                    if (!isMove)
                        isMove = Mathf.Abs(y) > move_y;
                }
                if (isMove)
                    modelGo.transform.localRotation = Quaternion.Euler(angley, anglex, 0);
                curPos = Input.mousePosition;
            }
            if (Input.GetMouseButtonUp(0) && isContinue && !isMove && isDown)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit raycast;
                if (Physics.Raycast(ray, out raycast))
                {
                    for (int i = 0; i < hitGos.Length; i++)
                    {
                        if (raycast.collider.gameObject.name == hitGos[i].name)
                        {
                            Debug.LogFormat(" CurClick Obj: {0}", hitGos[i].name);
                            if (clickFunc != null)
                            {
                                clickFunc.Call(hitGos[i]);
                            }
                            break;
                        }
                    }
                }
            }
        }

        private void OnDisable()
        {
            //modelGo.transform.localPosition = modelOriPos;
            //modelGo.transform.localEulerAngles = modelOriRot;
        }

        public void Reset(float newAnglex, float newAngley)
        {
            anglex = newAnglex;
            angley = newAngley;
        }

        public void SetClickFunction(LuaFunction clickCb = null)
        {
            clickFunc = clickCb;
        }

        public void SetRayEnable(bool status)
        {
            isContinue = status;
        }

    }
}

