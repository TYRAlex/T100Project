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
    class TargetBtn: ColorBtn
    {
        public TargetBtn(GameObject go,Sprite sprite):base(go,sprite)
        {
            this.name = go.name;
            slotName = "ui_1";
            animation_name = "ui_1";
            OnInit();
            ChooseImage(null);
        }
    }
}
