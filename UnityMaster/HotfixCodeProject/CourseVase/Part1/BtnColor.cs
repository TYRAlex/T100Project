using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CourseVasePart
{
    class BtnColor
    {
        GameObject nameObj;
        SpriteRenderer[] image;
        string nameAni;
        Image left;
        Image right;
        public int index { get; set; }
        public BtnColor(string strName, SpriteRenderer[] image,int num)
        {
            nameObj = CourseVasePart1.curGo.transform.Find("btn/" + strName).gameObject;
            Util.AddBtnClick(nameObj.transform.GetChild(0).gameObject, BtnOnClick);
            left = CourseVasePart1.curGo.transform.Find("leftImg").GetComponent<Image>();
            right = CourseVasePart1.curGo.transform.Find("rightImg").GetComponent<Image>();
            nameAni = strName;
            this.image = image;
            index = num;
            StateStart();
        }
        public BtnColor(SpriteRenderer[] image, int num)
        {
            left = CourseVasePart1.curGo.transform.Find("leftImg").GetComponent<Image>();
            right = CourseVasePart1.curGo.transform.Find("rightImg").GetComponent<Image>();
            this.image = image;
            index = num;
        }
        public void StateStart()
        {
            SpineManager.instance.PlayAnimationState(nameObj.GetComponent<SkeletonGraphic>(), nameObj.name);
        }
        public void StateEnd()
        {
            SpineManager.instance.PlayAnimationState(nameObj.GetComponent<SkeletonGraphic>(), nameObj.name,"0.8|0.8");
        }
        public void SetImage()
        {
            left.sprite = image[index].sprite;
            right.sprite = image[index].sprite;
        }
        public void BtnOnClick(GameObject btn)
        {
            MesManager.instance.Dispatch("CourseVasePart1", 1);
            SpineManager.instance.DoAnimation(btn.transform.parent.gameObject, btn.transform.parent.name, false);
            right.sprite = image[index].sprite;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
        }
    }
}
