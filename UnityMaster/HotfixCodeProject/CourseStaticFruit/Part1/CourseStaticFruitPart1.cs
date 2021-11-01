using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseStaticFruitPart1
    {
        GameObject curGo, flowerSpine, grassSpine, can, triggerObj, cansImg;
        GameObject[] collisions, cansImgs, points, area;
        ILObject3DAction objAction,trigger;
        int tempIndex, lastIndex;
        float[] width;
        Camera sceneCamera;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            //GameScene SceneCamera
            flowerSpine = curTrans.Find("SceneCamera/background/flowerSpine").gameObject;
            grassSpine = curTrans.Find("SceneCamera/background_front/grassSpine").gameObject;
            can = curTrans.Find("SceneCamera/can").gameObject;
            //triggerObj = can.transform.Find("trigger").gameObject;
            cansImg = curTrans.Find("SceneCamera/cansImg").gameObject;
            objAction = can.GetComponent<ILObject3DAction>();
            //trigger = triggerObj.GetComponent<ILObject3DAction>();
            sceneCamera = curTrans.Find("SceneCamera/sceneCamera").GetComponent<Camera>();
            collisions = curTrans.GetChildren(curTrans.Find("SceneCamera/collisions").gameObject);
            cansImgs = curTrans.GetChildren(cansImg);
            area = curTrans.GetChildren(curTrans.Find("SceneCamera/area").gameObject);
            points = curTrans.GetChildren(curTrans.Find("SceneCamera/points").gameObject);

            SpineManager.instance.DoAnimation(flowerSpine, "animation", true);
            SpineManager.instance.DoAnimation(grassSpine, "animation", true);

            objAction.OnMouseDownLua = OnMouseDown;
            objAction.OnMouseDragLua = OnMouseDrag;
            objAction.OnMouseUpLua = OnMouseUp;
            objAction.OnTriggerStay2DLua = OnTriggerStay;
            tempIndex = 0;
            can.SetActive(true);
            cansImgs[1].transform.SetParent(can.transform);
            cansImgs[1].transform.Identity();
            cansImgs[0].transform.SetParent(can.transform);
            cansImgs[0].transform.position = cansImgs[1].transform.position + new Vector3(0, 0, -0.01f);
            width = new float[] { 4, 0.5f, 1, 1, 1, 1.5f, 1, 2f, 2f, 2, 6 };

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
           // SoundManager.instance.bgmSource.volume = 0.3f;
            //trigger.OnTriggerEnter2DLua = OnTriggerEnter;
            //trigger.OnTriggerStay2DLua = OnTriggerStay;
            //trigger.OnTriggerExit2DLua = OnTriggerExit;
        }

        void Update()
        {            
            //Debug.DrawRay(triggerObj.transform.position, new Vector3(0, 0, 500), Color.red);
            //RaycastHit2D hitInfo = Physics2D.Raycast(triggerObj.transform.position, Vector2.zero);
            //if (hitInfo.collider != null)
            //{
            //    Debug.Log(111);
            //    int num = Convert.ToInt32(hitInfo.transform.name);
            //    int countNum = triggerObj.transform.childCount;

            //    for (int i = 0; i < countNum; i++)
            //    {
            //        triggerObj.transform.GetChild(i).transform.SetParent(cansImg.transform);
            //    }

            //    cansImgs[num].transform.SetParent(triggerObj.transform);
            //}
        }

        void OnMouseDown(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
        }

        void OnMouseDrag(int index)
        {
            Vector3 pos = sceneCamera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            Vector3 worldPos = new Vector3(pos.x, pos.y, objAction.transform.position.z);
            objAction.transform.position = worldPos;
            Debug.Log("objAction.transform.position:" + objAction.transform.position);
            Debug.Log("Input.mousePosition:" + Input.mousePosition);
        }

        void OnMouseUp(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
        }

        void OnTriggerStay(Collider2D collision, int index)
        {
            int num = Convert.ToInt32(collision.transform.name);
            if (num != tempIndex)
            {
                //if (num == lastIndex)
                //{
                //    can.transform.GetChild(can.transform.childCount - 1).transform.SetParent(cansImg.transform);
                //    //lastIndex = tempIndex;
                //    //tempIndex = num;
                //}
                //lastIndex = tempIndex;
                tempIndex = num;
                cansImgs[num].transform.SetParent(cansImg.transform);
                cansImgs[num].transform.SetParent(can.transform);
                cansImgs[num].transform.Identity();
                //Debug.LogWarningFormat("lastIndex:{0},tempIndex:{1}",lastIndex, tempIndex);
                int countNum = can.transform.childCount - 2;
                if (countNum > -1)
                {
                    cansImgs[num].transform.position = can.transform.GetChild(countNum).transform.position + new Vector3(0, 0, -0.01f);
                    SpriteRenderer firstSprite = can.transform.GetChild(countNum).GetComponent<SpriteRenderer>();
                    firstSprite.color = new Color(1, 1, 1, 1);
                    Debug.Log("collision.transform.name:" + collision.transform.name);
                    for (int i = 0; i < countNum; i++)
                    {
                        can.transform.GetChild(i).GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                        can.transform.GetChild(i).transform.SetParent(cansImg.transform);
                    }
                }                
            }
            else
            {
                Vector2 pos1 = can.transform.position;
                Vector2 pos2 = points[num].transform.position;
                float dis = Vector2.Distance(pos1, pos2);
                SpriteRenderer secondSprite = cansImgs[num].transform.GetComponent<SpriteRenderer>();
                secondSprite.color = new Color(1, 1, 1,width[num] / dis );
                Debug.LogWarning("dis:" + dis + "num:" + num);
                Debug.LogWarning("area[num].transform as RectTransform).rect.width:" + (width[num] / dis));
                //secondSprite.color = new Color(1, 1, 1, dis / (area[num].transform as RectTransform).rect.width);
                //Debug.LogWarning("dis:" + dis + "num:" + num);
                //Debug.LogWarning("area[num].transform as RectTransform).rect.width:" + (dis / (area[num].transform as RectTransform).rect.width));

            }
        }        
    }
}
