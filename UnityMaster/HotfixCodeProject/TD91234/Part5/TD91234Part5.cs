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
    public class TD91234Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;

        #region Mask
        private Transform anyBtns;
        private GameObject mask;
        private GameObject _mask;
        private GameObject _mask1;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion

        #region 田丁对话

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        private Transform SpineShow;

        private List<GameObject> RandomLevel;
        private List<GameObject> RandomDrag;
        private Transform randomLevel;
        private Transform randomDrag;
        private Transform drag;
        private Transform drag2;
        private Transform drag3;
        private Transform drag4;
        private Transform drag5;
        private Transform Level1;
        private Transform Level2;
        private Transform Level3;
        private Transform Level4;
        private Transform Level5;
        private int randomIndex;
        private int isMatchIndex;
        private Transform Drop;
        private Transform life;
        private Transform xem;
        private Transform bg;
        private int drop_index;
        private Transform xemeffect;

        private List<mILDrager> _iLDragers;
        private List<mILDrager> _iLDragers2;
        private List<mILDrager> _iLDragers3;
        private List<mILDrager> _iLDragers4;
        private List<mILDrager> _iLDragers5;
        private List<RectTransform> _DropRectList;

        private Transform Drag_1_yd;
        private Transform Drag_1_od;
        private Transform Drag_1_rd;

        private Transform Drag_2_yd;
        private Transform Drag_2_od;
        private Transform Drag_2_rd;

        private Transform Drag_3_yd;
        private Transform Drag_3_od;
        private Transform Drag_3_rd;

        private Transform Drag_4_yd;
        private Transform Drag_4_od;
        private Transform Drag_4_rd;

        private Transform Drag_5_yd;
        private Transform Drag_5_od;
        private Transform Drag_5_rd;

        private bool isplaying;
        #endregion

        #endregion

        #region 点击滑动图片

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;
        private GameObject rightBtn;
        private GameObject leftBtn;
        private GameObject btnBack;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        #endregion



        bool isPressBtn = false;
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
        #endregion



        bool isPlaying = false;


        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            xemeffect = curTrans.Find("xemeffect");
            bg = curTrans.Find("Bg");
            randomLevel = curTrans.Find("randomLevel");
            randomDrag = curTrans.Find("randomDrag");
            Bg = curTrans.Find("Bg").gameObject;
            drag = curTrans.Find("randomDrag/Drag"); drag2 = curTrans.Find("randomDrag/Drag2"); drag3 = curTrans.Find("randomDrag/Drag3"); drag4 = curTrans.Find("randomDrag/Drag4"); drag5 = curTrans.Find("randomDrag/Drag5");
            Level1 = curTrans.Find("randomLevel/Level1");
            Level2 = curTrans.Find("randomLevel/Level2");
            Level3 = curTrans.Find("randomLevel/Level3");
            Level4 = curTrans.Find("randomLevel/Level4");
            Level5 = curTrans.Find("randomLevel/Level5");

            Drag_1_yd = curTrans.Find("randomDrag/Drag/yD");
            Drag_1_od = curTrans.Find("randomDrag/Drag/oD");
            Drag_1_rd = curTrans.Find("randomDrag/Drag/rD");

            Drag_2_yd = curTrans.Find("randomDrag/Drag2/yD");
            Drag_2_od = curTrans.Find("randomDrag/Drag2/oD");
            Drag_2_rd = curTrans.Find("randomDrag/Drag2/rD");

            Drag_3_yd = curTrans.Find("randomDrag/Drag3/yD");
            Drag_3_od = curTrans.Find("randomDrag/Drag3/oD");
            Drag_3_rd = curTrans.Find("randomDrag/Drag3/rD");

            Drag_4_yd = curTrans.Find("randomDrag/Drag4/yD");
            Drag_4_od = curTrans.Find("randomDrag/Drag4/oD");
            Drag_4_rd = curTrans.Find("randomDrag/Drag4/rD");

            Drag_5_yd = curTrans.Find("randomDrag/Drag5/yD");
            Drag_5_od = curTrans.Find("randomDrag/Drag5/oD");
            Drag_5_rd = curTrans.Find("randomDrag/Drag5/rD");

         

            life = curTrans.Find("Bg/life");
            drop_index = -1;
            randomIndex = 0;
            isMatchIndex = 0;
            Drop = curTrans.Find("Drop");
            xem = curTrans.Find("Bg/xem");
            isplaying = false;
            _mask = curTrans.Find("_mask").gameObject;
            _mask1 = curTrans.Find("_mask1").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
           
            GameInit();
       

            InitILDragers();
            InitDropers();
            AddDragersEvent();
            //GameStart();
        }






        #region 初始化和游戏开始方法

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            _mask1.gameObject.SetActive(false);
            RandomLevel = null;
            RandomDrag = null;
            randomIndex = 0;
            _mask.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(xemeffect.gameObject,"kong",false);
            InitDragLayers();
            RandomInit();
            RandomStart();
            CWLevelSpine();
            //田丁初始化
            TDGameInit();
        }



        void GameStart()
        {
            //田丁开始游戏
            // TDGameStart();
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true); 
            mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE,1,null,()=> { SoundManager.instance.ShowVoiceBtn(true); }));
        }


        #region 田丁

        void TDGameInit()
        {

            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
                {
                    ShowDialogue("", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
                        {
                            ShowDialogue("", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
            });
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

        IEnumerator SpeckerCoroutineDTT(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
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
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
            //点击标志位
            flag = 0;
            buDing.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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

        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;

            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
        }
        private void LRBtnUpdate()
        {
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
            //任务对话方法加载
            TDLoadDialogue();
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();
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

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
        }

        /// <summary>
        /// 加载对话环节
        /// </summary>
        void TDLoadDialogue()
        {
            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");
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
        /// 加载按钮
        /// </summary>
        void TDLoadButton()
        {
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }
        /// <summary>
        /// 加载点击滑动环节
        /// </summary>
        void TDLoadPageBar()
        {
            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);
        }
        /// <summary>
        /// 加载点击材料环节
        /// </summary>
        void LoadSpineShow()
        {
            SpineShow = curTrans.Find("SpineShow");
            SpineShow.gameObject.SetActive(false);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }
        }


        #endregion

        #region 鼠标滑动图片方法

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
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }


        #endregion

        #region 点击材料环节

        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
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

        #region 点击移动图片环节

        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        }

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

        private void OnClickShow(GameObject obj)
        {
            if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            {
                SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            }
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () =>
                {
                    isPressBtn = true;
                    btnBack.SetActive(true);
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
            });
        }

        #endregion

        #region 切换游戏按键方法

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            isplaying = false;
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (isplaying == false) 
            {
                isplaying = true;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false); GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); CW(); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutineDTT(dbd, SoundManager.SoundType.VOICE,3)); });
                    }

                });
            }
         
        }
        void InitILDragers()
        {
            _iLDragers = new List<mILDrager>();
            for (int i = 0; i < drag.childCount; i++)
            {
                var _iLDrager = drag.GetChild(i).GetComponent<mILDrager>();
                _iLDragers.Add(_iLDrager);
            }
            _iLDragers2 = new List<mILDrager>();
            for (int i = 0; i < drag2.childCount; i++)
            {
                var _iLDrager = drag2.GetChild(i).GetComponent<mILDrager>();
                _iLDragers2.Add(_iLDrager);
            }
            _iLDragers3 = new List<mILDrager>();
            for (int i = 0; i < drag3.childCount; i++)
            {
                var _iLDrager = drag3.GetChild(i).GetComponent<mILDrager>();
                _iLDragers3.Add(_iLDrager);
            }
            _iLDragers4 = new List<mILDrager>();
            for (int i = 0; i < drag4.childCount; i++)
            {
                var _iLDrager = drag4.GetChild(i).GetComponent<mILDrager>();
                _iLDragers4.Add(_iLDrager);
            }
            _iLDragers5 = new List<mILDrager>();
            for (int i = 0; i < drag5.childCount; i++)
            {
                var _iLDrager = drag5.GetChild(i).GetComponent<mILDrager>();
                _iLDragers5.Add(_iLDrager);
            }
        }
        void InitDropers()
        {
            _DropRectList = new List<RectTransform>();
            for (int i = 0; i < Drop.childCount; i++)
            {
                // var Rect = 
                _DropRectList.Add(Drop.GetChild(i).GetRectTransform());
            }


        }
        void AddDragersEvent()
        {
            foreach (var drager in _iLDragers)
            {
                drager.SetDragCallback(DragStart, Draging, DragEnd);
            }
            foreach (var drager in _iLDragers2)
            {
                drager.SetDragCallback(DragStart2, Draging, DragEnd2);
            }
            foreach (var drager in _iLDragers3)
            {
                drager.SetDragCallback(DragStart3, Draging, DragEnd3);
            }
            foreach (var drager in _iLDragers4)
            {
                drager.SetDragCallback(DragStart4, Draging, DragEnd4);
            }
            foreach (var drager in _iLDragers5)
            {
                drager.SetDragCallback(DragStart5, Draging, DragEnd5);
            }
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            _iLDragers[index].transform.SetSiblingIndex(2);
            _iLDragers[index].transform.position = Input.mousePosition;
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            _iLDragers2[index].transform.SetSiblingIndex(2);
            _iLDragers2[index].transform.position = Input.mousePosition;
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            _iLDragers3[index].transform.SetSiblingIndex(2);
            _iLDragers3[index].transform.position = Input.mousePosition;
        }
        private void DragStart4(Vector3 position, int type, int index)
        {
            _iLDragers4[index].transform.SetSiblingIndex(2);
            _iLDragers4[index].transform.position = Input.mousePosition;
        }
        private void DragStart5(Vector3 position, int type, int index)
        {
            _iLDragers5[index].transform.SetSiblingIndex(2);
            _iLDragers5[index].transform.position = Input.mousePosition;
        }
        private void Draging(Vector3 position, int type, int index)
        {
            for (int i = 0; i < Drop.childCount; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint(_DropRectList[i], Input.mousePosition))
                {
                    drop_index = i;
                }

            }
            Debug.Log(drop_index);
        }

        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(isMatch);
            Debug.Log(drop_index == 0);
            _mask.gameObject.SetActive(true);
            if (isMatch)
            {
              
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false), () => 
                {
                    _mask.gameObject.SetActive(false);
                });
                isMatchIndex++;
                Level1_DragEndEvent();
                _iLDragers[index].DoReset();
                Jump(drag, type);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false), () =>
                {
                   
                        _mask.gameObject.SetActive(false);
                    
                });
                _iLDragers[index].DoReset();
               
              
            }
            if (isMatchIndex == 4)
            {
                _mask1.gameObject.SetActive(true);
                life.GetChild(randomIndex - 1).GetComponent<RawImage>().texture = life.GetChild(randomIndex - 1).GetComponent<BellSprites>().texture[1];
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SpineManager.instance.DoAnimation(xem.gameObject, "xem-y", false, () => { SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true); });
                
                if (randomIndex == 3) 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0,false);
               
                    SpineManager.instance.DoAnimation(xemeffect.gameObject, "sc-biu", false, () =>
                    {
                     
                            SpineManager.instance.DoAnimation(xem.gameObject, "xem-y2", true, () => { });
               
                    });
                }
                Waiting(3f, () =>
                 {
                    // SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true);
                     RandomStart();
                  
                         Level1.gameObject.SetActive(false);
                         drag.gameObject.SetActive(false);
                   
                   
                 });
            }
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(isMatch);
            Debug.Log(drop_index == 0); _mask.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false), () =>
                {
                    _mask.gameObject.SetActive(false);
                });
                isMatchIndex++;
                Level2_DragEndEvent();
                _iLDragers2[index].DoReset(); Jump(drag2, type);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false), () =>
                {
                    
                        _mask.gameObject.SetActive(false);
                    
                });
                _iLDragers2[index].DoReset();
              
            }
            if (isMatchIndex == 4)
            {
                _mask1.gameObject.SetActive(true);
                life.GetChild(randomIndex - 1).GetComponent<RawImage>().texture = life.GetChild(randomIndex - 1).GetComponent<BellSprites>().texture[1];
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SpineManager.instance.DoAnimation(xem.gameObject, "xem-y",false, () => { SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true); });
               
                if (randomIndex == 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SpineManager.instance.DoAnimation(xemeffect.gameObject, "sc-biu", false, () =>
                    {
                       
                            SpineManager.instance.DoAnimation(xem.gameObject, "xem-y2", true, () => { });
                    
                    });
                }
                Waiting(3f, () =>
                {
                
                    RandomStart();
                    Level2.gameObject.SetActive(false);
                    drag2.gameObject.SetActive(false);
                });
            }
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(isMatch);
            Debug.Log(drop_index == 0); _mask.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false), () =>
                {
                    _mask.gameObject.SetActive(false);
                });
                isMatchIndex++;
                Level3_DragEndEvent();
                _iLDragers3[index].DoReset(); Jump(drag3, type);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false), () =>
                {
                    
                        _mask.gameObject.SetActive(false);
                    
                });
                _iLDragers3[index].DoReset();
               
            }
            if (isMatchIndex == 4)
            {
                _mask1.gameObject.SetActive(true);
                life.GetChild(randomIndex - 1).GetComponent<RawImage>().texture = life.GetChild(randomIndex - 1).GetComponent<BellSprites>().texture[1];
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SpineManager.instance.DoAnimation(xem.gameObject, "xem-y", false, () => { SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true); });
               
                if (randomIndex == 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SpineManager.instance.DoAnimation(xemeffect.gameObject, "sc-biu", false, () =>
                    {
                      
                            SpineManager.instance.DoAnimation(xem.gameObject, "xem-y2", true, () => { });
                        
                    });
                }
                Waiting(3f, () =>
                {
                    
                    RandomStart();
                    Level3.gameObject.SetActive(false);
                    drag3.gameObject.SetActive(false);
                });
            }
        }
        private void DragEnd4(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(isMatch);
            Debug.Log(drop_index == 0); _mask.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false), () =>
                {
                    _mask.gameObject.SetActive(false);
                });
                isMatchIndex++;
                Level4_DragEndEvent();
                _iLDragers4[index].DoReset(); Jump(drag4, type);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false), () =>
                {
                    
                        _mask.gameObject.SetActive(false);
                    
                });
                _iLDragers4[index].DoReset();
              
            }
            if (isMatchIndex == 4)
            {
                _mask1.gameObject.SetActive(true);
                life.GetChild(randomIndex - 1).GetComponent<RawImage>().texture = life.GetChild(randomIndex - 1).GetComponent<BellSprites>().texture[1];
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SpineManager.instance.DoAnimation(xem.gameObject, "xem-y", false, () => { SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true); });
               
                if (randomIndex == 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false); 
                    SpineManager.instance.DoAnimation(xemeffect.gameObject, "sc-biu", false, () =>
                    {
                       
                            SpineManager.instance.DoAnimation(xem.gameObject, "xem-y2", true, () => { });
                      
                    });
                }
                Waiting(3f, () =>
                {
                    
                    RandomStart();
                    Level4.gameObject.SetActive(false);
                    drag4.gameObject.SetActive(false);
                });
            }
        }
        private void DragEnd5(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(isMatch);
            Debug.Log(drop_index == 0); _mask.gameObject.SetActive(true);
            if (isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false), () =>
                {
                    _mask.gameObject.SetActive(false);
                });
                isMatchIndex++;
                Level5_DragEndEvent();
                _iLDragers5[index].DoReset(); Jump(drag5, type);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false), () =>
                {
                   
                        _mask.gameObject.SetActive(false);
                    
                   
                });
                _iLDragers5[index].DoReset();
      
            }
            if (isMatchIndex == 4)
            {
                _mask1.gameObject.SetActive(true);
                life.GetChild(randomIndex - 1).GetComponent<RawImage>().texture = life.GetChild(randomIndex - 1).GetComponent<BellSprites>().texture[1];
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                SpineManager.instance.DoAnimation(xem.gameObject, "xem-y", false, () => { SpineManager.instance.DoAnimation(xem.gameObject, "xem-jx", true); });
               
               if (randomIndex == 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false); 
                    SpineManager.instance.DoAnimation(xemeffect.gameObject, "sc-biu", false, () => 
                    {
                       
                            SpineManager.instance.DoAnimation(xem.gameObject, "xem-y2", true, () => { });
                     
                    });
                  
                }
                Waiting(3f, () =>
                {
                   
                    Level5.gameObject.SetActive(false);
                    drag5.gameObject.SetActive(false);
                    RandomStart();
              
                });
            }
        }
        private void Level1_DragEndEvent()
        {
            Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            // Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            if (drop_index == 0)
            {
               
                Level1.GetChild(6).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level1.GetChild(6).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level1.GetChild(6).GetChild(0).gameObject, "jz-1", true);
                });

            }
            if (drop_index == 1)
            {
                Level1.GetChild(7).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level1.GetChild(7).GetChild(0).gameObject, "cm-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level1.GetChild(7).GetChild(0).gameObject, "cm-1", true);
                });
            }
            if (drop_index == 3)
            {
                Level1.GetChild(8).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level1.GetChild(8).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level1.GetChild(8).GetChild(0).gameObject, "xj-1", true);
                });
            }
            if (drop_index == 8)
            {
                Level1.GetChild(9).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level1.GetChild(9).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level1.GetChild(9).GetChild(0).gameObject, "jz-1", true);
                });
            }
        }
      
        private void Level2_DragEndEvent()
        {
            Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            // Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            if (drop_index == 1)
            {

                Level2.GetChild(6).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level2.GetChild(6).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level2.GetChild(6).GetChild(0).gameObject, "xj-1", true);
                });

            }
            if (drop_index == 5)
            {
                Level2.GetChild(7).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level2.GetChild(7).GetChild(0).gameObject, "cm-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level2.GetChild(7).GetChild(0).gameObject, "cm-1", true);
                });
            }
            if (drop_index == 6)
            {
                Level2.GetChild(8).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level2.GetChild(8).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level2.GetChild(8).GetChild(0).gameObject, "jz-1", true);
                });
            }
            if (drop_index == 8)
            {
                Level2.GetChild(9).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level2.GetChild(9).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level2.GetChild(9).GetChild(0).gameObject, "xj-1", true);
                });
            }
        }
        private void Level3_DragEndEvent()
        {
            Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            // Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            if (drop_index == 0)
            {

                Level3.GetChild(6).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level3.GetChild(6).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level3.GetChild(6).GetChild(0).gameObject, "xj-1", true);
                });

            }
            if (drop_index == 1)
            {
                Level3.GetChild(7).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level3.GetChild(7).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level3.GetChild(7).GetChild(0).gameObject, "jz-1", true);
                });
            }
            if (drop_index == 6)
            {
                Level3.GetChild(8).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level3.GetChild(8).GetChild(0).gameObject, "cm-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level3.GetChild(8).GetChild(0).gameObject, "cm-1", true);
                });
            }
            if (drop_index == 8)
            {
                Level3.GetChild(9).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level3.GetChild(9).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level3.GetChild(9).GetChild(0).gameObject, "jz-1", true);
                });
            }
        }
        private void Level4_DragEndEvent()
        {
            Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            // Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            if (drop_index == 2)
            {

                Level4.GetChild(6).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level4.GetChild(6).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level4.GetChild(6).GetChild(0).gameObject, "jz-1", true);
                });

            }
            if (drop_index == 3)
            {
                Level4.GetChild(7).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level4.GetChild(7).GetChild(0).gameObject, "cm-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level4.GetChild(7).GetChild(0).gameObject, "cm-1", true);
                });
            }
            if (drop_index == 6)
            {
                Level4.GetChild(8).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level4.GetChild(8).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level4.GetChild(8).GetChild(0).gameObject, "jz-1", true);
                });
            }
            if (drop_index == 7)
            {
                Level4.GetChild(9).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level4.GetChild(9).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level4.GetChild(9).GetChild(0).gameObject, "xj-1", true);
                });
            }
        }
        private void Level5_DragEndEvent()
        {
            Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            // Drop.GetChild(drop_index).GetComponent<mILDroper>().isActived = false;
            if (drop_index == 1)
            {

                Level5.GetChild(6).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level5.GetChild(6).GetChild(0).gameObject, "cm-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level5.GetChild(6).GetChild(0).gameObject, "cm-1", true);
                });

            }
            if (drop_index == 2)
            {
                Level5.GetChild(7).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level5.GetChild(7).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level5.GetChild(7).GetChild(0).gameObject, "xj-1", true);
                });
            }
            if (drop_index == 3)
            {
                Level5.GetChild(8).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level5.GetChild(8).GetChild(0).gameObject, "xj-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level5.GetChild(8).GetChild(0).gameObject, "xj-1", true);
                });
            }
            if (drop_index == 8)
            {
                Level5.GetChild(9).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Level5.GetChild(9).GetChild(0).gameObject, "jz-2", false, () =>
                {
                    SpineManager.instance.DoAnimation(Level5.GetChild(9).GetChild(0).gameObject, "jz-1", true);
                });
            }
        }
        private void RandomInit()//随机列表初始化
        {
            RandomLevel = new List<GameObject>();
            RandomDrag = new List<GameObject>();
            for (int i = 0; i < randomLevel.childCount; i++)
            {
                RandomLevel.Add(randomLevel.GetChild(i).gameObject);
                randomLevel.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < randomDrag.childCount; i++)
            {
                RandomDrag.Add(randomDrag.GetChild(i).gameObject);
                randomDrag.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void RandomStart() 
        {

         
            int i= Random.Range(0,RandomLevel.Count);
            isMatchIndex = 0;
            RandomLevel[i].SetActive(true);
            RandomDrag[i].SetActive(true);

            _mask1.gameObject.SetActive(false);
            for (int a = 0; a<Drop.childCount; a++) 
            {
                Drop.GetChild(a).GetComponent<mILDroper>().isActived = true;
            }

            RandomLevel.Remove(RandomLevel[i]);
            RandomDrag.Remove(RandomDrag[i]);

            if (randomIndex == 3)
            {

                playSuccessSpine();
                return;
            }
            randomIndex++;
        }
        IEnumerator wait(float time,Action method_1) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void Waiting(float time, Action method_1=null)
        {
            mono.StartCoroutine(wait(time,method_1));
        }
        #endregion

        #region 田丁对话方法

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        #endregion
        
          #region 田丁成功动画

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);

                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }
        private void CW() //重玩
        {
            RandomLevel = null;
            RandomDrag = null;
            _mask1.SetActive(false);
            randomIndex = 0;
            SpineManager.instance.DoAnimation(xemeffect.gameObject, "kong", false);
            RandomInit();
            RandomStart();
            CWLevelSpine();

        }
        private void CWLevelSpine() 
        {
            for (int j = 0; j < randomLevel.childCount; j++) //关卡的spine初始化
            {
                for (int i = 6; i < randomLevel.GetChild(j).childCount; i++)
                {
                    SpineManager.instance.DoAnimation(randomLevel.GetChild(j).GetChild(i).gameObject, "kong", false);
                    randomLevel.GetChild(j).GetChild(i).gameObject.SetActive(false);
                }
            }

            for (int i = 0; i < life.childCount; i++) //小恶魔生命
            {
                life.GetChild(i).GetComponent<RawImage>().texture = life.GetChild(i).GetComponent<BellSprites>().texture[0];
            }
            SpineManager.instance.DoAnimation(xem.gameObject,"xem-jx",true);
          

            //for (int i = 6; i < randomLevel.GetChild(0).childCount; i++) 
            //{
            //    SpineManager.instance.DoAnimation(randomLevel.GetChild(0).GetChild(i).gameObject,"kong",false);
            //    randomLevel.GetChild(0).GetChild(i).gameObject.SetActive(false);
            //}
            //for (int i = 6; i < randomLevel.GetChild(1).childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(randomLevel.GetChild(1).GetChild(i).gameObject, "kong", false);
            //    randomLevel.GetChild(1).GetChild(i).gameObject.SetActive(false);
            //}
            //for (int i = 6; i < randomLevel.GetChild(2).childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(randomLevel.GetChild(2).GetChild(i).gameObject, "kong", false);
            //    randomLevel.GetChild(2).GetChild(i).gameObject.SetActive(false);
            //}
            //for (int i = 6; i < randomLevel.GetChild(3).childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(randomLevel.GetChild(3).GetChild(i).gameObject, "kong", false);
            //    randomLevel.GetChild(3).GetChild(i).gameObject.SetActive(false);
            //}
            //for (int i = 6; i < randomLevel.GetChild(4).childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(randomLevel.GetChild(4).GetChild(i).gameObject, "kong", false);
            //    randomLevel.GetChild(4).GetChild(i).gameObject.SetActive(false);
            //}
        }

        private void Jump(Transform obj,int type) 
        {
            switch (type)
            {
                case 1:
                    SpineManager.instance.DoAnimation(obj.Find("yD/yellow").gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(obj.Find("yD/yellow").gameObject, "xj-3", false,()=> { SpineManager.instance.DoAnimation(obj.Find("yD/yellow").gameObject,"xj-1",true); }); });
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(obj.Find("oD/orange").gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(obj.Find("oD/orange").gameObject, "jz-3", false,()=> { SpineManager.instance.DoAnimation(obj.Find("oD/orange").gameObject, "jz-1", true); }); });
                    break;
                case 3:
                    SpineManager.instance.DoAnimation(obj.Find("rD/red").gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(obj.Find("rD/red").gameObject, "cm-3", false, () => { SpineManager.instance.DoAnimation(obj.Find("rD/red").gameObject, "cm-1", true); }); });
                    break;
            }
        }
        private void InitDragLayers() 
        {
            Drag_1_yd.SetAsFirstSibling();
            Drag_1_od.SetSiblingIndex(1);
            Drag_1_rd.SetAsLastSibling();

            Drag_2_yd.SetAsFirstSibling();
            Drag_2_od.SetSiblingIndex(1);
            Drag_2_rd.SetAsLastSibling();

            Drag_2_yd.SetAsFirstSibling();
            Drag_2_od.SetSiblingIndex(1);
            Drag_2_rd.SetAsLastSibling();

            Drag_3_yd.SetAsFirstSibling();
            Drag_3_od.SetSiblingIndex(1);
            Drag_3_rd.SetAsLastSibling();

            Drag_4_yd.SetAsFirstSibling();
            Drag_4_od.SetSiblingIndex(1);
            Drag_4_rd.SetAsLastSibling();

            Drag_5_yd.SetAsFirstSibling();
            Drag_5_od.SetSiblingIndex(1);
            Drag_5_rd.SetAsLastSibling();
        }
        #endregion
       

        #endregion

        

        

    }
}
