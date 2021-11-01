using DG.Tweening;
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
    public class TD6734Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        Transform SpineShow;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject mask;
        #endregion

        #region 点击滑动图片

        private GameObject pageBar;       
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;       


        private GameObject btnBack;
        private GameObject rightBtn;
        private GameObject leftBtn;
        private GameObject lBtn;
        private GameObject rBtn;

        private int curPageIndex;  //当前页签索引 
        private Vector2 _prePressPos;

        #endregion        

        bool isPressBtn = false;
        bool isPlaying = false;       
        //创作指引完全结束
        bool isEnd = false;
        private int flag = 0;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;
            lBtn = curTrans.Find("L2").gameObject;
            rBtn = curTrans.Find("R2").gameObject;
            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);
            lBtn.transform.DOMoveX(1920, 0);
            rBtn.transform.DOMoveX(1920, 0);

            lBtn.SetActive(true);
            rBtn.SetActive(true);
            leftBtn.SetActive(true);
            rightBtn.SetActive(true);
           

            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            Util.AddBtnClick(btnBack, OnClickBtnBack);

            //田丁加载游戏物体方法
            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);
            //加载点击滑动图片
            TDLoadPageBar();            
            //加载材料环节
            LoadSpineShow();
            GameInit();
            GameStart();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }        
        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            pageBar.SetActive(false);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);                               
            }
        }        
        /// <summary>
        /// 加载点击材料环节
        /// </summary>
        void LoadSpineShow()
        {
            SpineShow = curTrans.Find("SpineShow");
            SpineShow.gameObject.SetActive(true);
            SpineShow.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(SpineShow.gameObject, "0", false);
            SpineShow.DOMoveX(0, 0);
        }
        #region 初始化和游戏开始方法
        private void GameInit()
        {
            talkIndex = 1;
            //田丁初始化
            curPageIndex = 0;
            isPressBtn = false;           
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpinePage.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name.Substring(0), false);
            }
            LRBtnUpdate();
        }
        void GameStart()
        {
            //播放温和BGM
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    mask.SetActive(false);
                    ClickSpineShow();
                }));
            }));

        }        
        /// <summary>
        /// 点击材料环节—事件绑定
        /// </summary>
        private void ClickSpineShow()
        {
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
        }
        #endregion            
        #region 说话语音
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    break;
                case 2:
                    ShowBDAndSpeaker();
                    break;
            }

            talkIndex++;
        }
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            mask.SetActive(true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE,7, null, () =>
            {
                bd.SetActive(false);
                mask.SetActive(false);

                lBtn.SetActive(true);
                rBtn.SetActive(true);
                leftBtn.SetActive(true);
                rightBtn.SetActive(true);
                lBtn.transform.DOMoveX(0, 1);
                rBtn.transform.DOMoveX(0, 1);
                curPageIndex = -1;
                //SpineShow.gameObject.SetActive(false);
                SpineShow.DOMoveX(-1920, 1);
                SetMoveAncPosX(-1);
                pageBar.SetActive(true);                            
                
            }));
        }
        /// <summary>
        /// 显示BD并讲话
        /// </summary>
        private void ShowBDAndSpeaker()
        {
            bd.SetActive(true);
            mask.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 13, null, () => 
            {
                //mask.SetActive(false);
                //bd.SetActive(false);
                //pageBar.SetActive(true);               
                Util.AddBtnClick(btnBack,OnClickBtnBackPage);
            }));
        }
      
        #endregion       
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;           
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
        }             
        #region 点击材料环节
        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 2, null, () =>
            {
                isPlaying = false;
                if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                {
                    flag += 1 << obj.transform.GetSiblingIndex();
                }
                if (flag == (Mathf.Pow(2, SpineShow.childCount) - 1))
                {
                    SoundManager.instance.ShowVoiceBtn(true);                  
                }
            }));
            SpineManager.instance.DoAnimation(SpineShow.gameObject, obj.name, false);
        }
        #endregion
        #region 点击放大显示
        /// <summary>
        /// 点击放大显示
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickShow(GameObject obj)
        {

            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                isPressBtn = true;
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) +8, null, () =>
                {
                    btnBack.SetActive(true);
                    //用于标志是否点击过展示板
                    if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                    {
                        flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                    }
                    isPressBtn = false;

                }));

            });
        }     
        #endregion    
        #region 点击返回原大小和位置
        /// <summary>
        /// 点击返回原大小和位置
        /// </summary>
        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false;
                    if (flag == (Mathf.Pow(2, SpinePage.childCount) - 1) && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }       
        /// <summary>
        /// 点击单个图片返回原大小
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickBtnBackPage(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false;
                });
            });
        }
        /// <summary>
        /// 左按钮
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });           
        }       
        /// <summary>
        /// 右按钮
        /// </summary>
        /// <param name="obj"></param>
        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 2 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false;  });           
        }
        /// <summary>
        /// 左右按钮更新
        /// </summary>
        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex == SpinePage.childCount - 2)
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
        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };
            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 300)
                {
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 2 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }
        #endregion       

    }
}
