using DG.Tweening;
using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace CourseButterflyPart
{
    class RotationClass : TaskBase
    {
        GameObject target;
        GameObject dian;
        float times = 1.5f;
        int index = 0;
        int max = 0;
        public RotationClass(GameObject go):base()
        {
            target = go;
            dian = go.transform.parent.Find("dian").gameObject;
            max = (int)(times / 0.1f);
        }
        public override void OnInit()
        {
            ClassCopyManager.instance.taskQueue.OnFinish = OnFinish;
        }
        public void Rotation()
        {
            PowerpointManager.instance.MaskHide();
            mono.StartCoroutine(waitRotation());
        }
        IEnumerator waitRotation()
        {
            while(index != max)
            {
                yield return new WaitForSeconds(0.1f);
                index++;
                Debug.Log(((float)index / (float)max) * 180);
                if(target.transform.localPosition.y <= 90)
                {
                    target.transform.GetComponent<SpriteRenderer>().color = new Color(1,1,1,1 - ((float)index/(float)(max/2))*0.3f);
                }
                if(index != max)
                {
                    target.transform.RotateAround(dian.transform.position, Vector3.up, 180/max);
                }
            }
            ClassCopyManager.instance.taskQueue.End();
        }
        public void OnFinish()
        {
            Debug.Log("--------OnFinish---------");
            target.SetActive(false);
            MesManager.instance.Dispatch("CourseButterflyPart1", (int)State.Rotation);
        }
    }
}
