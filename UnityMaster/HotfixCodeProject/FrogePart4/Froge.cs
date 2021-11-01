using System;
using System.Collections.Generic;
using ILFramework;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace FrogePart4
{
    class Froge
    {
        public GameObject frogSp;
        public GameObject frogNumSp;
        public Transform oriParent;

        private GameObject frogeGo;
        public GameObject FrogeGo
        {
            set
            {
                frogeGo = value;
                frogeNameText = frogeGo.transform.Find("fname").GetComponent<Text>();
                frogeColorText = frogeGo.transform.Find("color").GetComponent<Text>();
                frogSp = frogeGo.transform.Find("spine").gameObject;
                frogNumSp = frogeGo.transform.Find("spine/number").gameObject;
            }
            get { return frogeGo; }
        }

        public int frogIndex;

        private string name;
        public string Name   // 青蛙名称
        {
            set
            {
                name = value;
                if (frogeNameText != null)
                    frogeNameText.text = value;
            }
            get { return name; }
        }

        public Text frogeNameText;
        public Text frogeColorText;
        public Vector3 Position // 青蛙位置
        {
            set { frogeGo.transform.localPosition = value; }
            get { return frogeGo.transform.localPosition; }
        }
        public Vector3 oriPosition;  // 青蛙原始位置

        public string mabotAddress;
        public string mabotType;
        private bool active;
        public bool Active    // 青蛙激活状态
        {
            set
            {
                active = value;
                //if (frogeGo != null)
                //    frogeGo.GetComponent<Image>().color = value ? Color.green : Color.grey;
            }
            get { return active; }
        }
        public bool success;    // 青蛙成功状态
        public bool takeOff;    // 青蛙是否正在起跳状态
        public bool fallDown;   // 青蛙是否落水
        public Leaf curLeaf = null;

        public Func<Froge, Vector3> calPos = null;
        public Func<Froge, int, bool> canTake = null;

        public Func<ConstVariable.LColor, Froge, Leaf> vertifyPos = null;
        public Func<Vector3> getLandPos = null;
        public Action gameOver = null;

        float takeOffTime = 0.2f;
        float takeSpScale = 3.5f;
        public float radius = 300;
        public float pointY = 459;  // 终点坐标 y

        public Froge(GameObject frogeItem)
        {
            FrogeGo = frogeItem;
            oriPosition = frogeItem.transform.localPosition;
            //SpineManager.instance.DoAnimation(frogSp, ConstVariable.GetSpineByColor(ConstVariable.LColor.Green));
        }

        public void DoConnectAni()
        {
            SpineManager.instance.DoAnimation(frogSp, ConstVariable.frogConnect, false, ()=> {
                SpineManager.instance.DoAnimation(frogSp, ConstVariable.frogConnectIdle);
            });
        }

        public void Reset()
        {
            Name = "未连接";
            Active = false;
            mabotAddress = "";
            success = false;
            Position = oriPosition;
            SpineManager.instance.DoAnimation(frogSp, ConstVariable.frogDisConnectIdle);
            SpineManager.instance.DoAnimation(frogNumSp, ConstVariable.numberIdle[frogIndex]);
        }

        public void TakeOff(int color)
        {
            if (takeOff)
                return;
            var lcolor = ConstVariable.GetColorByInt(color);
            Leaf l = vertifyPos(lcolor, this);
            if (l != null)
            {
                frogeGo.transform.SetParent(l.leafGo.transform.parent);
                frogeGo.transform.up = (l.Position - Position).normalized;

                DoTakeOffAni();
                takeOff = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                frogeGo.transform.DOLocalMove(l.Position, takeOffTime).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                    SpineManager.instance.DoAnimation(l.leafSpine, l.leafAnis[1], false, () => { SpineManager.instance.DoAnimation(l.leafSpine, l.leafAnis[0]); });
                    frogeGo.transform.localPosition = l.Position;
                    frogeGo.transform.SetParent(l.leafGo.transform);
                    takeOff = false;
                });
                curLeaf = l;
            }
            else
            {
                Vector3 pos = curLeaf == null ? Position : curLeaf.Position;
                if (lcolor == ConstVariable.LColor.Yellow && (pointY - pos.y) <= radius)
                {
                    takeOff = true;
                    Vector3 landPos = getLandPos();
                    frogeGo.transform.SetParent(oriParent);
                    frogeGo.transform.up = (landPos - Position).normalized;
                    DoTakeOffAni();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                    frogeGo.transform.DOLocalMove(landPos, takeOffTime).OnComplete(() => { 
                        success = true;
                        frogeGo.transform.localEulerAngles = Vector3.zero;
                        gameOver();
                    });
                }
            }
        }

        void DoTakeOffAni()
        {
            SpineManager.instance.SetTimeScale(frogNumSp, takeSpScale);
            SpineManager.instance.SetTimeScale(frogSp, takeSpScale);
            SpineManager.instance.DoAnimation(frogSp, ConstVariable.frogTakeOff, false, ()=> {
                SpineManager.instance.DoAnimation(frogSp, ConstVariable.frogConnectIdle);
                SpineManager.instance.SetTimeScale(frogSp, 1);
                SpineManager.instance.SetTimeScale(frogNumSp, 1);
            });
            SpineManager.instance.DoAnimation(frogNumSp, ConstVariable.numberTake[frogIndex], false, ()=> {
                SpineManager.instance.DoAnimation(frogNumSp, ConstVariable.numberIdle[frogIndex]);
            });
        }

        void DoChangeColor(ConstVariable.LColor curColor, ConstVariable.LColor newColor)
        {
            string s1 = ConstVariable.GetColorToSpine(curColor);
            string s2 = ConstVariable.GetColorToSpine(newColor);
            Debug.LogFormat(" DoChangeColor aniName: {0} ", s1 + s2);
            SpineManager.instance.SetTimeScale(frogSp, 3.5f);
            SpineManager.instance.DoAnimation(frogSp, s1 + s2, false, () => {
                //SpineManager.instance.DoAnimation(frogSp, ConstVariable.GetSpineByColor(newColor));
                SpineManager.instance.SetTimeScale(frogSp, 1);
            });
        }



        //public void TakeOff(int color)
        //{
        //    frogeColorText.text = color.ToString();
        //    Debug.LogFormat(" 青蛙起跳颜色: {0} , 起跳成功: {1}", color, canTake(this, color));
        //    if (!canTake(this, color))
        //        return;
        //    Vector3 nextPos = calPos(this);
        //    Debug.LogFormat(" 青蛙: {0} 起跳至 {1} 点", name, nextPos);
        //    takeOff = true;
        //    frogeGo.transform.DOLocalMove(nextPos, takeOffTime).OnComplete(() => { 
        //        takeOff = false;
        //        if (fallDown)
        //        {
        //            Revive();
        //        }
        //    });
        //}

        public void Revive()
        {
            Debug.LogFormat(" 青蛙 {0} 复活! ", name);
            takeOff = false;
            fallDown = false;
            curLeaf = null;
            frogeGo.transform.localPosition = oriPosition;
        }
    }
}
