using ILFramework;
using ILFramework.HotClass;
using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseColorfulWorldPart
{
    class ZhuanpanClass
    {
        GameObject zhuanpan1;
        GameObject zhuanpan2;
        Bone root1;
        Bone root2;
        bool isRotation;
        ILObject3DAction obj;
        float maxSpeed;
        float curSpeed;
        Vector2 startPos;
        Vector2 movPos;
        float times;
        Vector3 Direction;
        bool isDown;
        float radius;
        GameObject mask;
        static public Vector2 center;
        TargetSelected block;
        AnimationFeedBack feedBack;
        GameObject dingding;
        bool isSlow;
        Transform ui3d;
        public ZhuanpanClass()
        {
            ui3d = CourseColorfulWorldPart1.curGo.transform.Find("UI3D");
            zhuanpan1 = ui3d.Find("Size/zhuanpan01").gameObject;
            zhuanpan2 = ui3d.Find("Size/zhuanpan02").gameObject;
            root1 = zhuanpan1.GetComponent<SkeletonAnimation>().Skeleton.FindBone("zp_0");
            root2 = zhuanpan2.GetComponent<SkeletonAnimation>().Skeleton.FindBone("zp_0");
            isRotation = false;
            obj = zhuanpan1.GetComponent<ILObject3DAction>();
            obj.OnMouseDownLua = OnMouseDown;
            obj.OnMouseUpLua = OnMouseUpLua;
            obj.OnMouseDragLua = OnMouseDrag;
            maxSpeed = 2;
            radius = 200;
            curSpeed = 0;
            startPos = Vector3.zero;
            movPos = Vector3.zero;
            times = 0;
            Direction = Vector3.zero;
            mask = ui3d.Find("Size/mask").gameObject;
            mask.SetActive(false);
            center = ui3d.Find("Size/center").transform.position;
            center = ui3d.GetComponent<Camera>().WorldToScreenPoint(center);
            zhuanpan1.SetActive(true);
            zhuanpan2.transform.localScale = Vector3.zero;
            block = new TargetSelected();
            feedBack = new AnimationFeedBack();
            dingding = CourseColorfulWorldPart1.curGo.transform.Find("tiantian").gameObject;
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND,0, ()=>mask.SetActive(true), () => 
            {
                mask.SetActive(false);
            });
            isSlow = false;
            new TargetTrigger();
        }
        private void OnMouseUpLua(int obj)
        {
            isDown = false;
            times = 0;
        }
        private void OnMouseDrag(int obj)
        {
            movPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - center;
            float angle = Vector2.Distance(startPos, movPos);
            //Debug.Log("angle--------" + angle);
            if (angle > 0)
            {
                float leng = Mathf.PI * radius * 2 *  angle /360;
                
                if (times > 0.05)
                {
                    curSpeed = leng / times;
                    if (curSpeed >= maxSpeed)
                    {
                        Direction = Vector3.Cross(startPos, movPos);
                        Debug.Log("gogogogo----->times:" + times + ",leng:" + leng);
                        Debug.Log("curSpeed: " + curSpeed);
                        Debug.Log("angle: " + angle);
                        isDown = false;
                        isRotation = true;
                        mask.SetActive(true);
                        times = 0;
                    }
                }
            }
        }
        private void OnMouseDown(int obj)
        {
            isDown = true;
            startPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - center;
            movPos = new Vector2(Input.mousePosition.x, Input.mousePosition.y) - center;
        }
        public void FixedUpdate()
        {
            if (isDown)
            {
                times += Time.deltaTime;
                Debug.Log("Time.deltaTime:" + Time.deltaTime);
            }
            else
            {
                if (isRotation)
                {
                    int num = Direction.z > 0 ? 1 : -1;
                    root1.Rotation = Mathf.Lerp(root1.Rotation, root1.Rotation + 4 * num, Time.deltaTime * curSpeed);
                    curSpeed -= curSpeed/20;
                    if(curSpeed < 30 && isSlow == false)
                    {
                        isSlow = true;
                    }
                    if (curSpeed <= 1f)
                    {
                        isSlow = false;
                        isRotation = false;
                        curSpeed = 0;
                        root2.Rotation = root1.Rotation;
                        block.FrontBlock();
                        feedBack.curName = block.ShowSetlectdBlock();
                        if (feedBack.curName == "")
                        {
                            mask.SetActive(false);
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                            feedBack.OnclickState(true);
                        }
                    }
                }
            }
        }
        public void InitZhuanpan()
        {
            zhuanpan2.transform.localScale = Vector3.zero;
            //feedBack.Idle();
            mask.SetActive(false);
        }
    }
}