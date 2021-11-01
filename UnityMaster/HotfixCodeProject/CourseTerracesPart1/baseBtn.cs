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
    abstract class baseBtn
    {
        public string name { get; set; }
        public string curName { get; set; }
        public Object3DAction obj;
        public Sprite sprite;
      
        public baseBtn()
        {

        }
        public baseBtn(GameObject go ,Sprite sprite)
        {
            if(sprite != null)
            {
                this.sprite = sprite;
            }
            this.obj = go.GetComponent<Object3DAction>();
            this.obj.OnMouseDownLua = (CourseTerracesPart1.rootILBehaviour.IlObject as CourseTerracesPart1).OnMouseDown;
        }
        public abstract void ChooseImage(Sprite _sprite);

        public abstract void BtnOnclick(Action action);

    }
}
