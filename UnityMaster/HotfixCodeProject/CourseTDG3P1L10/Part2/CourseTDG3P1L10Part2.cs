using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG3P1L10Part2
    {
        GameObject curGo;
        Transform content;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            content = curTrans.Find("ScrollRect_Parent/Viewport/Content");
            content.localPosition = new Vector3(4800f, 0, 0);

            if (curTrans.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() == null)
            {
                curTrans.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            }

            ScrollRectMoveManager.instance.index = 0;
            ScrollRectMoveManager.instance.CreateManager(curTrans.Find("ScrollRect_Parent"));
        }

        void OnDisable()
        {
            curGo.transform.Find("ScrollRect_Parent").RemoveComponent<ScrollRectMoveManager>();
        }
    }
}
