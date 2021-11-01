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
    public class TD5633Part1
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
        #endregion

        #region 点击滑动图片

        private GameObject pageBar;      
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;
        private GameObject btnBack;
        private int curPageIndex;  //当前页签索引       

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

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            //田丁初始化
            TDGameInit();
        }
        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();

        }
        #endregion
        void TDGameInit()
        {

            curPageIndex = 0;
            isPressBtn = false;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpinePage.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name.Substring(0), false);
            }

        }
        void TDGameStart()
        {
            isPlaying = true;
            //播放温和BGM
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                //SoundManager.instance.ShowVoiceBtn(true);
                bd.SetActive(false);
                isPlaying = false;
            }));
        }
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
        /// <summary>
        /// 显示BD并讲话
        /// </summary>
        private void ShowBDAndSpeaker()
        {
            bd.SetActive(true);
            isPressBtn = true;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 12, null, () =>{ }));
        }
        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            bd.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 8, null, () =>
            {
                bd.SetActive(false);
                //SpineShow.gameObject.SetActive(false);
                SpineShow.DOMoveX(-1920, 1);
                pageBar.SetActive(true);
                isPlaying = false;
                SetMoveAncPosX(-1);
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

            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(
                () => { callBack?.Invoke(); isPlaying = false; });
        }

        #region 田丁加载
        /// <summary>
        /// 田丁加载所有物体
        /// </summary>
        void TDLoadGameProperty()
        {
            //加载人物
            TDLoadCharacter();
            //加载点击滑动图片
            TDLoadPageBar();
            //加载材料环节
            LoadSpineShow();

        }

        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {

            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
        }

        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;          
            //隐藏pageBar
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
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
            SpineShow.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(SpineShow.gameObject, "0", false);
            SpineShow.DOMoveX(0, 0);
        }
        #endregion       

        #region 点击材料环节
        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
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
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) + 9, null, () =>
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

    }
}
