using System;
using LuaFramework;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Ximmerse.InputSystem;

/// <summary>
/// 3dof模拟操作类
/// </summary>
public class Sim3dof : MonoBehaviour
{

    public GameObject origin;
    GameObject directObj;
    public GameObject touchPoint;
    public GetXDeviceState xDevice;
    public Camera guiCamera;
    public GameObject canvasObj;
    GraphicRaycaster graphicRaycaster;
    EventSystem eventSystem;
    RaycastHit hit;
    public LineRenderer line;
    List<GameObject> dragers = new List<GameObject>();
    public Vector2 mousePoint;
    Vector2 hitPosition;
    GameObject NewTouchPoint2D;
    //插值速度
    public float smooting = 2;

    bool isDragStart = false;
    bool onSend = false;//用来间隔多次发送 
    bool doCheck = false;


    // Use this for initialization
    void Start()
    {
        touchPoint.SetActive(false);
        DontDestroyOnLoad(gameObject);
        onSend = false;
        directObj = origin.transform.GetChild(0).gameObject;
        
        

    }

    public void ClickDownFunc()
    {
        SetTouchDown(true);
        if (isDragStart == false)
        {
            var messages = new string[3] { "OnPointerDown", "OnBeginDrag","OnPointerClick" };
            DoGraphicRaycaster(mousePoint, messages);
            isDragStart = true;
        }
    }

    public void ClickUpFunc()
    {
        SetTouchDown(false);
        var messages = new string[] {"OnPointerUp", "OnEndDrag"};
        dragers.Clear();
        DoGraphicRaycaster(mousePoint, messages);
        isDragStart = false;
    }

    public void ClickStayFunc()
    {
        if (isDragStart) return;
        DoGraphicRaycaster(mousePoint, new string[] { "OnPointerClick" });
    }


    // Update is called once per frame
    void Update()
    {
        if (!LuaFramework.Util.GetSim3dofEnable())
        {
            touchPoint.SetActive(false);
            return;
        }
       
        touchPoint.SetActive(true);
        

        touchPoint.GetComponent<SpriteRenderer>().enabled = LuaFramework.Util.GetTouchPointEnable();
        
        if (xDevice == null)
        {
            initDevice();
        }
        else
        {
            //判断设备连接
            if (xDevice.GetDeviceState() == DeviceConnectionState.Connected)
            {
                touchPoint.SetActive(true);
            }
            else
            {
                touchPoint.SetActive(false);
            };
        }
        if (canvasObj == null) initCanvas();
        Vector3 direct = directObj.transform.position - origin.transform.position;
        bool grounded = Physics.Raycast(origin.transform.position, direct, out hit);
        if (grounded)
        {
            hitResult(hit);
        }
        if (isDragStart)
        {
            DoGraphicRaycaster(mousePoint, new string[] { "OnDrag" }, dragers);
        }
        //if (Input.GetMouseButton(0))
        //{
        //    DoGraphicRaycaster(mousePoint);
        //}
        if (doCheck)
        {
            doCheck = false;
            StartCoroutine(DelayCheckSend(0.2f));
        }
    }

    void initCanvas()
    {
        //canvasObj = GameObject.FindGameObjectWithTag("Canvas");
        canvasObj = GameObject.Find("MainCanvas");
        graphicRaycaster = canvasObj.GetComponent<GraphicRaycaster>();
        eventSystem = canvasObj.GetComponent<EventSystem>();
    }


    void initDevice()
    {
        xDevice = ILFramework.Util.GetXDevice();
        if (xDevice)
        {
          //  Debug.Log("init xDevice");
            xDevice.ugetRotateCallBack = (rotate) =>
            {
                if (origin)
                {
                    float w = 16.5f;//x
                    float v = 28f;//y
                    var y = rotate.y;
                    var x = rotate.x;
                    if (rotate.y >= v && rotate.y <= 180) y = v;
                    if (rotate.y <= 360 - v && rotate.y > 180) y = 360 - v;
                    if (rotate.x <= 360 - w && rotate.x > 180) x = 360 - w;
                    if (rotate.x >= w && rotate.x <= 180) x = w;
                    origin.transform.rotation = Quaternion.Euler(new Vector3(x, y, rotate.z));
                }
            };
            xDevice.uclickCallback = (clickIndex) =>
            {
                if (isDragStart) return;
              //  Debug.Log("ClickIndex:" + clickIndex);
                if (clickIndex == ControllerButton.Back || clickIndex == ControllerButton.PrimaryTrigger)
                {
                    DoGraphicRaycaster(mousePoint, new string[] { "OnPointerClick" });
                }
            };
            xDevice.uclickDownCallback = (clickIndex) =>
            { 
                if (clickIndex == ControllerButton.Back || clickIndex == ControllerButton.PrimaryTrigger)
                {
                    SetTouchDown(true);
                    if (isDragStart == false)
                    {
                        var messages = new string[2] { "OnPointerDown", "OnBeginDrag" };
                        DoGraphicRaycaster(mousePoint, messages);
                        isDragStart = true;
                    }
                }
            };
            xDevice.uclickUpCallback = (clickIndex) =>
            {
                if (clickIndex == ControllerButton.Back || clickIndex == ControllerButton.PrimaryTrigger)
                {
                    SetTouchDown(false);
                 //   Debug.Log("click:" + mousePoint.x + "|" + mousePoint.y);
                    var messages = new string[] { "OnPointerUp", "OnEndDrag" };
                    dragers.Clear();
                    DoGraphicRaycaster(mousePoint, messages);
                    isDragStart = false;
                }
            };
        }
    }

    void hitResult(RaycastHit hit)
    {
        if (touchPoint)
        {

            touchPoint.transform.position = hit.point;

            hitPosition = hit.point;
            //Debug.LogError("hit:" + hit.point);
            //Debug.LogError("X:" + (hit.point.x + Screen.width / 2) + " Y:" + (hit.point.x + Screen.width / 2));
            //mousePoint = Vector2.Lerp(mousePoint, new Vector2(hit.point.x + Screen.width / 2, hit.point.y + Screen.height / 2), Time.deltaTime * smooting);
            mousePoint = new Vector2(hit.point.x + Screen.width / 2 - 20000f, hit.point.y + Screen.height / 2 - 20000f);
            //touchPoint.transform.position = mousePoint;
            if (!NewTouchPoint2D)
            {
                NewTouchPoint2D = GameObject.Find("NewTouchPoint2D");
            }
            else
            {
                NewTouchPoint2D.transform.position = mousePoint;
            }



            var direct = hit.point - origin.transform.position;
            if (line)
            {
                line.SetPosition(0, origin.transform.position);
                line.SetPosition(1, direct * 100);
            }


        }
    }

    void DoGraphicRaycaster(Vector2 position, string[] messages, List<GameObject> objs = null)
    {
        if (graphicRaycaster)
        {
            PointerEventData eventData = new PointerEventData(eventSystem);//与指针（鼠标/触摸）事件相关的事件负载。(PointerEventData)

            //指针位置
            eventData.pressPosition = position;
            eventData.position = position;//当前指针位置。
            eventData.button = PointerEventData.InputButton.Left;
            eventData.clickCount = 1;
            eventData.clickTime = 0.2f;
            //eventData.eligibleForClick = true;
            
            if (objs != null)
            {
                foreach (GameObject g in objs)
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                        //Debug.Log("对应的RaycastResult:" + messages[i] + " index:" + i);
                        g.SendMessage(messages[i], eventData, SendMessageOptions.DontRequireReceiver);
                    }
                }
                return;
            }

            List<RaycastResult> list = new List<RaycastResult>();
            graphicRaycaster.Raycast(eventData, list);//如同Physics.Raycast(ray, out hit)绑定相应的射线检测信息
         //   Debug.Log(list.Count);
            Debug.DrawRay(position, position);
            bool doPop = true;//往下检测
            if (list.Count > 0)
            {
                //foreach (RaycastResult result in list)
                for(int j=0;j<list.Count;j++)
                {
                    RaycastResult result = list[j];
                    if (doPop == false) break;
                    //print("UI物体："+result.gameObject.name);//输出相应的Ui物体
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (messages[i] == "OnPointerClick")
                        {
                            //var raw = result.gameObject.GetComponent<RawImage>();
                            var image = result.gameObject.GetComponent<MaskableGraphic>();
                            var text = result.gameObject.GetComponent<Text>();
                            //Debug.Log("MaskableGraphic:" + image.name);
                            if (((image != null && image.raycastTarget == true) && text == null) && onSend == false)
                            {
                                //Debug.Log("RaycastResult OnPointerClick:" + onSend);
                                result.gameObject.SendMessage("OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
                                onSend = true;
                                doCheck = true;
                                doPop = false;
                            }

                            //检测toggle
                            var t1 = result.gameObject.transform.parent.GetComponent<Toggle>();
                            if (t1)
                            {
                                t1.gameObject.SendMessage("OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
                            }
                            var t2 = result.gameObject.transform.parent.transform.parent.GetComponent<Toggle>();
                            if (t2)
                            {
                                t2.gameObject.SendMessage("OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
                            }
                        }
                        else
                        {
                            if (messages[i] == "OnBeginDrag")
                            {
                                var image = result.gameObject.GetComponent<MaskableGraphic>();
                                var text = result.gameObject.GetComponent<Text>();
                             //   Debug.Log("MaskableGraphic:" + image.name);
                                if (((image != null && image.raycastTarget == true) && text == null))
                                {
                                    dragers.Add(result.gameObject);
                                    //检测RectScoll
                                    var rectScoll = result.gameObject.transform.parent.GetComponent<ScrollRect>();
                                    if (rectScoll)
                                    {
                                        dragers.Add(rectScoll.gameObject);
                                    }
                                    //检测ScollBar
                                    var sbr = result.gameObject.transform.parent.transform.parent.GetComponent<Scrollbar>();
                                    if (sbr)
                                    {
                                        dragers.Add(sbr.gameObject);
                                    }
                                    doPop = false;
                                }
                            }
                         //   Debug.Log("RaycastResult:" + messages[i] + " index:" + i);
                            //检测RectScoll                          
                            result.gameObject.SendMessage(messages[i], eventData, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }

        GraphicRaycaster rag = getPhotoRaycast();
        if (rag)
        {
            PointerEventData eventData = new PointerEventData(eventSystem);//与指针（鼠标/触摸）事件相关的事件负载。(PointerEventData)

            //指针位置
            eventData.pressPosition = position;
            eventData.position = position;//当前指针位置。
            eventData.button = PointerEventData.InputButton.Left;
            eventData.clickCount = 1;
            eventData.clickTime = 0.2f;
            //eventData.eligibleForClick = true;

            if (objs != null)
            {
                foreach (GameObject g in objs)
                {
                    for (int i = 0; i < messages.Length; i++)
                    {
                       // Debug.Log("RaycastResult:" + messages[i] + " index:" + i);
                        g.SendMessage(messages[i], eventData, SendMessageOptions.DontRequireReceiver);
                    }
                }
                return;
            }

            List<RaycastResult> list = new List<RaycastResult>();
            rag.Raycast(eventData, list);//如同Physics.Raycast(ray, out hit)绑定相应的射线检测信息
          //  Debug.Log(list.Count);
            Debug.DrawRay(position, position);
            bool doPop = true;//往下检测
            if (list.Count > 0)
            {
                //foreach (RaycastResult result in list)
                for(int j=0;j<list.Count;j++)
                {
                    RaycastResult result = list[j];
                    if (doPop == false) break;
                    //print(result.gameObject.name);//输出相应的Ui物体
                    for (int i = 0; i < messages.Length; i++)
                    {
                        if (messages[i] == "OnPointerClick")
                        {
                            //var raw = result.gameObject.GetComponent<RawImage>();
                            var image = result.gameObject.GetComponent<MaskableGraphic>();
                            var text = result.gameObject.GetComponent<Text>();
                         //   Debug.Log("MaskableGraphic:" + image.name);
                            if (((image != null && image.raycastTarget == true) && text == null) && onSend == false)
                            {
                        //        Debug.Log("RaycastResult OnPointerClick:" + onSend);
                                result.gameObject.SendMessage("OnPointerClick", eventData, SendMessageOptions.DontRequireReceiver);
                                onSend = true;
                                doCheck = true;
                                doPop = false;
                            }
                        }
                        else
                        {
                            if (messages[i] == "OnBeginDrag")
                            {
                                var image = result.gameObject.GetComponent<MaskableGraphic>();
                                var text = result.gameObject.GetComponent<Text>();
                           //     Debug.Log("MaskableGraphic:" + image.name);
                                if (((image != null && image.raycastTarget == true) && text == null))
                                {
                                    dragers.Add(result.gameObject);
                                    doPop = false;
                                }
                            }
                         //   Debug.Log("RaycastResult:" + messages[i] + " index:" + i);
                            result.gameObject.SendMessage(messages[i], eventData, SendMessageOptions.DontRequireReceiver);
                        }
                    }
                }
            }
        }
    }


    GraphicRaycaster getPhotoRaycast()
    {
        GameObject tg = GameObject.Find("PhotoUICanvas");
        if (tg == null)
            return null;

        if (!tg.activeInHierarchy)
            return null;

        return tg.GetComponent<GraphicRaycaster>();
    }

    IEnumerator DelayCheckSend(float t)
    {
        yield return new WaitForSeconds(t);
        onSend = false;
       // Debug.Log("onSend:" + onSend);
    }

    public void SetTouchDown(bool isDown)
    {
        if (touchPoint == null || touchPoint.activeSelf == false) return;
        if (isDown)
        {
            touchPoint.GetComponent<SpriteRenderer>().sprite = touchPoint.GetComponent<BellSprites>().sprites[1];

            if (NewTouchPoint2D)
                NewTouchPoint2D.transform.GetChild(0).gameObject.SetActive(isDown);

        }
        else
        {
            touchPoint.GetComponent<SpriteRenderer>().sprite = touchPoint.GetComponent<BellSprites>().sprites[0];

            if (NewTouchPoint2D)
                NewTouchPoint2D.transform.GetChild(0).gameObject.SetActive(isDown);
        }


    }
}
