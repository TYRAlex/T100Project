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
    public class TD91232Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;

        #region Mask
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion

        #region 田丁对话

        private Transform SpineShow;

        #endregion

        #endregion

        #region 点击滑动图片

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;

        private GameObject btnBack;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        #endregion

        bool isPressBtn = false;
        private int flag = 0;
        #endregion

        bool isPlaying = false;
        bool isClick = false;


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


        #region 田丁

        void TDGameInit()
        {
            curPageIndex = 0;
            isPressBtn = false;
            flag = 0;
            SetPos(SpinePage.GetRectTransform(), new Vector2(0, 0));
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name + "3", false);
            }
        }

        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            TDGameStartFunc();
        }

        #endregion

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
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    {
                        mask.SetActive(false);
                        bd.SetActive(false);
                    }));
                    break;
                case 2:
                    flag = 0;
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 7,
                    () =>
                    {
                        mask.SetActive(true);
                        bd.SetActive(true);
                    },
                    () =>
                    {
                        mask.SetActive(false);
                        bd.SetActive(false);
                        SwitchPageAndShow(1.0f, null);
                    }));
                    break;
                case 3:
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 11,
                    () =>
                    {
                        mask.SetActive(true);
                        bd.SetActive(true);
                    }, null));
                    break;
            }
            talkIndex++;
        }

        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        #endregion

        #region 通用方法

        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index, Action goingEvent = null, Action finishEvent = null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex, Action callback = null)
        {
            float voiceTimer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }

        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }

        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer, Action callback)
        {

            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer, Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();

        }


        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }

        #endregion

        #region 监听相关

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        #endregion

        #region 田丁

        #region 田丁加载

        /// <summary>
        /// 田丁加载所有物体
        /// </summary>
        void TDLoadGameProperty()
        {
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
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
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }

        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            //位置初始化
            SetPos(pageBar.GetComponent<RectTransform>(), new Vector2(1920, 0));
        }
        /// <summary>
        /// 加载点击材料环节
        /// </summary>
        void LoadSpineShow()
        {
            SpineShow = curTrans.Find("SpineShow/Ani");
            SpineShow.gameObject.SetActive(true);

            SpineShow.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(SpineShow.gameObject, "0", false);
            //位置初始化
            SetPos(curTrans.Find("SpineShow").GetComponent<RectTransform>(), new Vector2(0, 0));
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
        }

        #endregion

        #endregion

        #region 点击材料环节

        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
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

        #region 点击移动图片环节

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name + "3", false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false;
                    if (flag == (Mathf.Pow(2, SpinePage.childCount) - 1))
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

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
                btnBack.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name) + 8, null, () =>
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

        #endregion

        #region 材料认识与图片点击环节切换

        //注意需要多加一个空物体（名暂定“Ani”），空物体占满全屏，把材料环节放进空物体中作为子类，认识图片的pageBar物体放在第二页位置
        private void SwitchPageAndShow(float duration = 1f, Action callBack = null)
        {
            isPlaying = true;
            SpineShow.parent.GetRectTransform().DOAnchorPosX(SpineShow.parent.GetRectTransform().anchoredPosition.x - 1920, duration);
            pageBar.transform.GetRectTransform().DOAnchorPosX(pageBar.transform.GetRectTransform().anchoredPosition.x - 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        }
        #endregion
    }
}