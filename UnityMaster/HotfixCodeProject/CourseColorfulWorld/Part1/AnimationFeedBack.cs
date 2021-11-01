using DG.Tweening;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseColorfulWorldPart
{
    class AnimationFeedBack
    {
        GameObject right;
        GameObject left;
        GameObject right_ball;
        GameObject right_ball_1;
        GameObject left_ball;
        GameObject left_ball_1;
        ILObject3DAction right_onclick;
        ILObject3DAction left_onclick;
        public string curName;
        GameObject lizi;
        public AnimationFeedBack()
        {
            right = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/right").gameObject;
            left = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/left").gameObject;
            right_ball = right.transform.Find("ball").gameObject;
            left_ball = left.transform.Find("ball").gameObject;
            right_ball_1 = right.transform.Find("ball_1").gameObject;
            left_ball_1 = left.transform.Find("ball_1").gameObject;
            right_onclick = right.transform.Find("onclick").GetComponent<ILObject3DAction>();
            right_onclick.OnMouseDownLua = OnMouseDown;
            left_onclick = left.transform.Find("onclick").GetComponent<ILObject3DAction>();
            left_onclick.OnMouseDownLua = OnMouseDown;
            Idle();
            lizi = CourseColorfulWorldPart1.curGo.transform.Find("UI3D/Size/lizi").gameObject;
            lizi.transform.localScale = Vector3.zero;
            SpineManager.instance.PlayAnimationState(lizi.GetComponent<SkeletonAnimation>(), "lizi");
            OnclickState(false);
        }
        public void OnclickState(bool ison)
        {
            left_onclick.GetComponent<PolygonCollider2D>().enabled = ison;
            right_onclick.GetComponent<PolygonCollider2D>().enabled = ison;
        }
        private void OnMouseDown(int obj)
        {
            OnclickState(false);
            string name = obj == 1 ? "right" : "left";
            //正确点击反馈
            if (name == curName)
            {
                float time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Selected(name);
            }
            else//错误点击反馈
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                GameObject go = null;
                if (name == "right")
                {
                    go = right_ball.transform.localScale.x == 1 ? right_ball : right_ball_1;
                }
                else
                {
                    go = left_ball.transform.localScale.x == 1 ? left_ball : left_ball_1;
                }
                SpineManager.instance.DoAnimation(go, "error", false, ()=> 
                {
                    SpineManager.instance.DoAnimation(go, "idle");
                    OnclickState(true);
                });
                
            }
        }
        public void Idle()
        {
            SpineManager.instance.DoAnimation(right, "idle");
            SpineManager.instance.DoAnimation(left, "idle");
            SpineManager.instance.DoAnimation(right_ball, "idle");
            SpineManager.instance.DoAnimation(right_ball_1, "idle");
            SpineManager.instance.DoAnimation(left_ball, "idle");
            SpineManager.instance.DoAnimation(left_ball_1, "idle");
        }
        public void Selected(string name)
        {
            if (name == "right")//right_ball用于播放right动画，right_ball_1用于待机idle状态
            {
                right_ball.transform.localScale = Vector3.one;
                right_ball_1.transform.localScale = Vector3.zero;
                SpineManager.instance.DoAnimation(right, name, false, () => SpineManager.instance.DoAnimation(right, "idle"));
                SpineManager.instance.DoAnimation(right_ball, name, false, () => 
                {
                    right_ball.transform.localScale = Vector3.zero;
                    right_ball_1.transform.localScale = Vector3.one;
                    SpineManager.instance.DoAnimation(right_ball, "idle");

                    //气球上升的动画
                    right_ball_1.transform.localPosition = new Vector3(right_ball_1.transform.localPosition.x, -9, right_ball_1.transform.localPosition.z);
                    SpineManager.instance.DoAnimation(right_ball_1, "idle");
                    right_ball_1.transform.DOLocalMoveY(3.3f, 0.5f).OnComplete(()=> SendMessage());
                });
                SpineManager.instance.DoAnimation(left, name, false, () => SpineManager.instance.DoAnimation(left, "idle"));

                //播放彩带动画
                SpineManager.instance.DoAnimation(lizi, "lizi", false, LiziEnd);
                lizi.transform.localScale = Vector3.one;
            }
            else if (name == "left")
            {
                name = "right";
                left_ball.transform.localScale = Vector3.one;
                left_ball_1.transform.localScale = Vector3.zero;
                SpineManager.instance.DoAnimation(left, name, false, () => SpineManager.instance.DoAnimation(left, "idle"));
                SpineManager.instance.DoAnimation(left_ball, name, false, () =>
                {
                    left_ball.transform.localScale = Vector3.zero;
                    left_ball_1.transform.localScale = Vector3.one;

                    left_ball_1.transform.localPosition = new Vector3(left_ball_1.transform.localPosition.x, -9, left_ball_1.transform.localPosition.z);
                    SpineManager.instance.DoAnimation(left_ball_1, "idle");
                    left_ball_1.transform.DOLocalMoveY(3.3f, 0.5f).OnComplete(() => SendMessage());
                });
                SpineManager.instance.DoAnimation(right, name, false, () => SpineManager.instance.DoAnimation(right, "idle"));
                SpineManager.instance.DoAnimation(lizi, "lizi", false, LiziEnd);
                lizi.transform.localScale = Vector3.one;
            }
        }
        public void LiziEnd()
        {
            lizi.transform.localScale = Vector3.zero;
            SpineManager.instance.PlayAnimationState(lizi.GetComponent<SkeletonAnimation>(), "lizi");
        }
        //发送消息告知本回合播放完毕
        public void SendMessage()
        {
            MesManager.instance.Dispatch("CourseColorfulWorldPart1", 0);
        }
    }
}
