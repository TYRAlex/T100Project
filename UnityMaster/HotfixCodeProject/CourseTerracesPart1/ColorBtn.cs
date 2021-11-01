using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseTerracesPart
{
    class ColorBtn:baseBtn
    {
        private SkeletonAnimation animation;
        public string slotName;
        public string animation_name;
        
        public ColorBtn(GameObject go, Sprite sprite) : base(go,sprite)
        {
            this.curName = go.name;
            this.name = (int.Parse(go.name) - 50).ToString();
            this.obj.index = int.Parse(go.name);
            this.animation = go.GetComponent<SkeletonAnimation>();
            slotName = "ui_1_1";
            animation_name = "ui_1_1";
        }
        public  void OnInit()
        {
            SpineManager.instance.PlayAnimationState(animation, animation_name);
        }
        public override void BtnOnclick(Action action)
        {
            Debug.Log("animation_name--------" + animation_name);
            CourseTerracesPart1.mask.SetActive(true);
            SpineManager.instance.DoAnimation(animation.gameObject, animation_name, false, delegate ()
            {
                action();
            });
        }

        public override void ChooseImage(Sprite _sprite)
        {
            if (_sprite != null) sprite = _sprite;
            Shader shader = animation.GetComponent<MeshRenderer>().material.shader;
            SpineManager.instance.CreateRegionAttachmentByTexture(animation, slotName, sprite, shader);
        }
    }
}
