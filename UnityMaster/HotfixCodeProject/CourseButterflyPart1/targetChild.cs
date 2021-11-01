using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CourseButterflyPart
{
    public class targetChild
    {
        public GameObject right;
        Vector3 position;
        Object3DAction btn1;
        Object3DAction btn2;
        GameObject skeObj;
        LightClass lightClass;
        RotationClass rotationClass;
        public GameObject curGo;
        public List<SpriteRenderer> spriteRenderers;
        public List<SkeletonAnimation> skeletonAnimations;
        public targetChild(GameObject curGo)
        {
            right = curGo.transform.Find("right").gameObject;
            btn1 = curGo.transform.Find("left/left/Image").GetComponent<Object3DAction>();
            btn1.OnMouseDownLua += OnMouseDown;
            btn2 = curGo.transform.Find("right/Image").GetComponent<Object3DAction>();
            btn2.OnMouseDownLua += OnMouseDown;
            skeObj = curGo.transform.Find("left/left/light").gameObject;
            this.curGo = curGo;
            position = right.transform.localPosition;
            spriteRenderers = new List<SpriteRenderer>();
            skeletonAnimations = new List<SkeletonAnimation>();
            FindAllCommpentType(curGo.transform, ref spriteRenderers);
            FindAllCommpentType(curGo.transform, ref skeletonAnimations);
            Debug.Log("curGo-----" + curGo.name);
            Debug.Log("spriteRenderers------" + spriteRenderers.Count);
            //Debug.Log("skeletonAnimations------" + skeletonAnimations.Count);
            OnInit();
        }
        public void BtnSate(bool ison)
        {
            btn1.transform.GetComponent<PolygonCollider2D>().enabled = ison;
            btn2.transform.GetComponent<PolygonCollider2D>().enabled = ison;
        }
        public void FindAllCommpentType(Transform tras,ref List<SpriteRenderer> types)
        {
            int count = tras.childCount;
            for(int i = 0;i < count; i++)
            {
                if(tras.GetChild(i).GetComponent<SpriteRenderer>() != null)
                {
                    types.Add(tras.GetChild(i).GetComponent<SpriteRenderer>());
                }
                FindAllCommpentType(tras.GetChild(i),ref types);
            }
        }
        public void FindAllCommpentType(Transform tras, ref List<SkeletonAnimation> types)
        {
            int count = tras.childCount;
            for (int i = 0; i < count; i++)
            {
                if (tras.GetChild(i).GetComponent<SkeletonAnimation>() != null)
                {
                    types.Add(tras.GetChild(i).GetComponent<SkeletonAnimation>());
                }
                FindAllCommpentType(tras.GetChild(i), ref types);
            }
        }
        public void PushList(bool ison,int size)
        {
            Debug.Log("PushList-----------------------1--"+ spriteRenderers.Count);
            foreach (SpriteRenderer v in spriteRenderers)
            {
                Debug.Log(v.name);
                v.transform.GetComponent<SpriteRenderer>().enabled = ison;
            }
            foreach (SkeletonAnimation v in skeletonAnimations)
            {
                Debug.Log(v.name);
                v.transform.localScale = new Vector3(size, size, size);
            }
            Debug.Log("PushList-----------------------2--");
            BtnSate(ison);
        }
        public void OnInit()
        {
            PushList(false, 0);
            SpineManager.instance.PlayAnimationState(skeObj.GetComponent<SkeletonAnimation>(), curGo.name);
            right.transform.localRotation = Quaternion.identity;
            right.transform.localPosition = position;
            right.transform.GetComponent<SpriteRenderer>().color = Color.white;
            right.SetActive(true);
        }
        
        public void OnMouseDown(int index)
        {
            Debug.Log("-----------OnMouseDown-----------");
            if (index == 1)
            {
                BtnSate(false);
                AddRotation();
            }
        }
        
        public void AddRotation()
        {
            rotationClass = new RotationClass(right);
            object[] obj = new object[] { right };
            rotationClass.AddTask_Run(obj, rotationClass);
        }
        public void AddLight()
        {
            lightClass = new LightClass(skeObj,curGo.name);
            object[] obj = new object[] { skeObj, curGo.name };
            lightClass.AddTask_Run(obj, lightClass);
        }
    }
}
