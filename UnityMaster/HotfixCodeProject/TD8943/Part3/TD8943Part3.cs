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
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD8943Part3
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        //UI游戏物体
        private GameObject bd;
        private GameObject btnBack;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;

        private int curPageIndex;


        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        bool isPressBtn = false;
        bool isPlaying = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            //UI查找初始化
            FindInit();
            //UI显示与隐藏
            ShowOrHide();
            //布丁说话
            TDSpecke();

            Util.AddBtnClick(btnBack, OnClickBtnBack);

            //加载点击滑动图片
            TDLoadPageBar();
            GameInit();
            GameStart();
        }
        /// <summary>
        /// 查找UI初始化
        /// </summary>
        private void FindInit()
        {
            bd = curTrans.Find("BD").gameObject;
            btnBack = curTrans.Find("btnBack").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            pageBar = curTrans.Find("PageBar").gameObject;
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            bellTextures = Bg.GetComponent<BellSprites>();
        }
        /// <summary>
        /// UI显示与隐藏
        /// </summary>
        private void ShowOrHide()
        {
            btnBack.SetActive(false);
            bd.SetActive(true);
            pageBar.SetActive(true);
        }
        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpinePage.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name.Substring(0), false);
            }
        }
        void GameStart()
        {
            //田丁开始游戏
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //显示语音键
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }
        /// <summary>
        /// 田丁说话
        /// </summary>
        private void TDSpecke()
        {
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bd.SetActive(false);
                isPlaying = false;
            }));
        }
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

        /// <summary>
        /// 语音键对应方法
        /// </summary>
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
            }
            talkIndex++;
        }

        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            bd.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, null));
        }
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
        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }
        }

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
                btnBack.SetActive(true); //int.Parse(obj.transform.GetChild(0).name) + 1
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) + 1, null, () =>
                {
                    //用于标志是否点击过展示板
                    if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                    {
                        flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                    }
                    isPressBtn = false;                    
                }));

            });
        }

    }
}