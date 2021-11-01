using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L09Part4
    {
        GameObject curGo;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            if (curGo.GetComponent<ContactPartManager>() == null)
            {
                curGo.AddComponent<ContactPartManager>();
            }
            ContactPartManager.instance.PlayAnimationVoice(curGo, 0.2f);
        }
    }
}
