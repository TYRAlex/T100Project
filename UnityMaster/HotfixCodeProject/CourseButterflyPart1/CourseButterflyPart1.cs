using CourseButterflyPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum State
    {
        Rotation,
        HuaXian
    }
    public class CourseButterflyPart1
    {
        GameObject curGo;
        targetChild[] targets;
        Transform tampParent;
        public static MonoBehaviour mono;
        otherCommon otherCommon;
        GameObject bg;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            curGo.AddComponent<PowerpointManager>();
            curGo.AddComponent<ClassCopyManager>();
            curGo.AddComponent<MesManager>();
            //Thread.Sleep(10000);
            targets = new targetChild[5];
            tampParent = curTrans.Find("tamp_Parent");
            bg = tampParent.Find("bg").gameObject;
            Debug.Log("-----------µÈ´ý½áÊø-------------");
            for (int i = 1; i < tampParent.childCount -1; i++)
            {
                targets[i - 1] = new targetChild(tampParent.GetChild(i).gameObject);
            }
            otherCommon = new otherCommon(curTrans.Find("other_common").gameObject);
            PowerpointManager.instance.CreatePower(curGo, new string[] { "1", "2", "3" ,"4","5"}, LightEnd);
            AddListener();
        }
        public void AddListener()
        {
            MesManager.instance.Register("CourseButterflyPart1", (int)State.Rotation, RotationEnd);
            MesManager.instance.Register("CourseButterflyPart1", (int)State.HuaXian, EnterRotation);
        }
        public void EnterRotation(object[] obj)
        {
            targets[PowerpointManager.instance.curIndex].PushList(true, 1);
            PowerpointManager.instance.MaskHide();
            bg.SetActive(true);
        }
        public void RotationEnd(object[] obj)
        { 
            Debug.Log("---------RotationEnd-----------");
            int _index = PowerpointManager.instance.curIndex;
            targets[_index].AddLight();
        }
        public void LightEnd()
        {
            Debug.Log("--------LightEnd---------"+PowerpointManager.instance.curIndex);
            PowerpointManager.instance.MaskShow();
            //targets[PowerpointManager.instance.curIndex].curGo.transform.localScale = Vector3.zero;
            bg.SetActive(false);
            otherCommon.OnInit(targets[PowerpointManager.instance.curIndex].right.GetComponent<SpriteRenderer>().sprite);
            for (int i = 0;i<targets.Length;i++)
            {
                if(i != PowerpointManager.instance.curIndex)
                {
                    targets[i].OnInit();
                }
            }
        }
    }
}
