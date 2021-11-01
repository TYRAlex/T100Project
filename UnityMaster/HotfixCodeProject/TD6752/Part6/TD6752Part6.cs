using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD6752Part6
    {
        #region 变量
        GameObject curGo;//画布物体
        GameObject tem;//放大动画物件
        GameObject pageBar;//画板滑动
        GameObject rightBtn;//右滑动
        GameObject leftBtn;//左滑动
        GameObject btnBack;
        GameObject leftBtnPar;
        GameObject rightBtnPar;

        Transform curTrans;//画布位置
        Transform SpinePage;//画板动画

        MonoBehaviour mono;
        Empty4Raycast[] e4rs;//用来存储放大动画

        int curPageIndex;  //当前页数

        bool isPressBtn = false;//是否画板处于放大状态
        bool isPlaying = false;//是否正在播放动画

        Vector2 _prePressPos;//按下的坐标位置
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;

            //加载所有新画布时停止所有协程和声音
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            //加载游戏物体方法
            TDLoadGameProperty();

            //初始化
            TDGameInit();
        }

        //初始化
        void TDGameInit()
        {
            curPageIndex = 0;
            isPressBtn = false;

            leftBtnPar.GetComponent<SkeletonGraphic>().Initialize(true);
            rightBtnPar.GetComponent<SkeletonGraphic>().Initialize(true);

            //重置画板状态  
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpinePage.GetChild(i).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);

                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false); 
            }

            SpinePage.GetRectTransform().anchoredPosition = Vector2.zero;

            LRBtnUpdate();
        }

        //田丁加载所有物体
        void TDLoadGameProperty()
        {
            //加载点击滑动图片
            pageBar = curTrans.Find("PageBar").gameObject;
            pageBar.SetActive(true);

            //为PageBar添加左右滑动功能
            SlideSwitchPage(pageBar);

            SpinePage = curTrans.Find("PageBar/Mask/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            //为每个画板都添加点击放大方法
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;
            leftBtnPar = leftBtn.transform.parent.gameObject;
            rightBtnPar = rightBtn.transform.parent.gameObject;

            leftBtnPar.SetActive(true);
            rightBtnPar.SetActive(true);

            //为左右滑块添加切换页数的方法
            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            //让放大的图片缩小回原来的大小
            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
        }

        //点击向右滑动
        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPressBtn || isPlaying) return;

            isPressBtn = true;
            SoundManager.instance.PlayClip(9);

            SetMoveAncPosX(-1);

            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => 
            { 
                isPressBtn = false; 
            });
        }

        //点击向左滑动
        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPressBtn || isPlaying) return;
            
            isPressBtn = true;
            SoundManager.instance.PlayClip(9);

            SetMoveAncPosX(1);

            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => 
            { 
                isPressBtn = false; 
            });
        }

        //点击放大的画板还原原来大小
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying) return;

            obj.SetActive(false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);

            //缩小动画
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    isPressBtn = false;
                });
            });
        }

        private void OnClickShow(GameObject obj)
        {

            if (isPressBtn || isPlaying) return;

            isPressBtn = true;
            isPlaying = true;

            btnBack.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);

            tem = obj.transform.parent.gameObject;

            //将tem的层级放在最后,渲染在上层
            tem.transform.SetAsLastSibling();

            //播放放大动画
            SpineManager.instance.DoAnimation(tem, obj.name + "1", false, () =>
            {
                isPlaying = false;
            });
        }

        //左右滑动(切换页面)后初始化
        private void SetMoveAncPosX(int lorR, float duration = 1f, Action callBack = null)
        {
            //防止播放动画时再次点击
            if (isPlaying) return;
            isPlaying = true;

            curPageIndex -= lorR;//页数变化

            //横向移动
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + lorR * 1920, duration).OnComplete(() => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
        }

        //左右滑块的点击动画
        private void LRBtnUpdate()
        {
            //如果是第一页或者最后一页或者。。。播放滑块spin
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex == SpinePage.childCount - 1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
        }

        //(鼠标)手指滑动浏览画板方法
        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            //当画板被按下时获得鼠标按下的位置
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            //当鼠标在画板上松开时
            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                //当滑动距离>300时切换页数
                if (dis > 300)
                {
                    //判断方向
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying || isPressBtn) return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn) return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }
    }
}
