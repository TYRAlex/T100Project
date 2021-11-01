using CourseColorfulWorldPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseColorfulWorldPart1
    {
        static public GameObject curGo;
        ZhuanpanClass target;
        GameObject bg;
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            target = new ZhuanpanClass();
            bg = curGo.transform.Find("UI3D/Size/bg").gameObject;
            SpineManager.instance.DoAnimation(bg, "animation");
            curGo.AddComponent<MesManager>();
            SoundManager.instance.BgSoundPart1();
            AddListener();
        }
        void AddListener()
        {
            MesManager.instance.Register("CourseColorfulWorldPart1", 0, Finily);
        }
        private void Finily(object[] param)
        {
            target.InitZhuanpan();
        }
        public void FixedUpdate()
        {
            target.FixedUpdate();
        }
    }
}
