using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Spine.Unity;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD91221Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        bool hasbeenOnClick = false;
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;

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
        private Transform _Mask;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        private Transform SpineShow;

        private List<Coroutine> event1;

        private bool isNextLevel;
        private bool isNextLeve2;
        private float a; private float b;
        private Transform Huang;
        private Transform Lan;
        private Transform Hong;
        private Transform Lv;

        private Transform Huang1;
        private Transform Lan1;
        private Transform Hong1;
        private Transform Lv1;
        private Transform ShenHuang1;

        private Transform Shenhong2;
        private Transform lv2;
        private Transform hong2;
        private Transform qianhong2;
        private Transform huang2;
        private Transform lan2;

        private Transform spine_Mask;
        private Transform spine;
        private Transform Level1;
        private Transform Level2;
        private Transform Level3;
        private Transform hua;
        private Transform fashehua;
        private Transform uiPanel;
        private Transform texiao;
        private int Level2_index;
        private int Level3_index;
        //lv2
        private int JustOnlyOneIndex;
        private int JustOnlyOneIndex2;
        //lv3
        private int JustOnlyOneIndex3;
        private int JustOnlyOneIndex4;
        private int JustOnlyOneIndex5;

        private float _moveSpeed; private float _moveSpeed2; private float _moveSpeed3;
        private float _randomSpeed_L1;private float L1_speed;
        private Transform HuangHua1;
        private Transform HuangHua2;

        private Transform lanhua;
        private Transform huanghua;
        private Transform honghua;
        private bool _isPause;
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

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            isNextLevel = false;
            isNextLeve2 = false;
        a = 255;b = 0;
            Huang = curTrans.Find("spine/Level1/Huang");
            Hong = curTrans.Find("spine/Level1/Hong");
            Lan = curTrans.Find("spine/Level1/Lan");
            Lv = curTrans.Find("spine/Level1/Lv");

            Huang1 = curTrans.Find("spine/Level2/Huang1");
            Hong1 = curTrans.Find("spine/Level2/Hong1");
            Lan1 = curTrans.Find("spine/Level2/Lan1");
            Lv1 = curTrans.Find("spine/Level2/Lv1");
            ShenHuang1 = curTrans.Find("spine/Level2/ShenHuang1");

            Shenhong2 = curTrans.Find("spine/Level3/ShenHong2");
            hong2 = curTrans.Find("spine/Level3/Hong2");
            qianhong2 = curTrans.Find("spine/Level3/QianHong2");
            lan2 = curTrans.Find("spine/Level3/Lan2");
            huang2 = curTrans.Find("spine/Level3/Huang2");
            lv2 = curTrans.Find("spine/Level3/Lv2");

            _Mask = curTrans.Find("_mask");

            spine_Mask = curTrans.Find("spine/Mask");
            hua = curTrans.Find("hua");
            fashehua = curTrans.Find("fashehua");
            uiPanel = curTrans.Find("ui");
            texiao = curTrans.Find("texiao");
            spine_Mask.gameObject.SetActive(false);
            event1 = new List<Coroutine>();
            spine = curTrans.Find("spine");

            Level1 = curTrans.Find("spine/Level1");
            Level2 = curTrans.Find("spine/Level2");
            Level3 = curTrans.Find("spine/Level3");
            Level2_index = 0;
            Level3_index = 0;
            JustOnlyOneIndex = 0;
            JustOnlyOneIndex2 = 0;
            L1_speed = 1f;
            _moveSpeed = 5f * (Screen.width / 1920f);
            _moveSpeed2 = 10f * (Screen.width / 1920f);
            _moveSpeed3 = 15f * (Screen.width / 1920f);
            _randomSpeed_L1 = L1_speed*(Screen.width / 1920f);
            L1_speed = 1f;
            HuangHua1 = curTrans.Find("Huanghua1");
            HuangHua2 = curTrans.Find("Huanghua2");
            _isPause = false;

            lanhua = curTrans.Find("lanhua");
            huanghua = curTrans.Find("huanghua");
            honghua = curTrans.Find("honghua");
          
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        
        

      

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            InitOnClick();
            InitMove();
            Input.multiTouchEnabled = false;
            devil.gameObject.SetActive(true);
            buDing.gameObject.SetActive(true);
            Replay();
            _Mask.gameObject.SetActive(false);
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
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            bdText.text = "";
            devilText.text = "";

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.gameObject.SetActive(true);
            buDing.gameObject.SetActive(true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutineDD(bd, SoundManager.SoundType.SOUND, 0, () =>
                {
                    ShowDialogue("哼，我要让荷花也永远变得跟墨一样黑", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineDD(bd, SoundManager.SoundType.SOUND, 1, () =>
                        {
                            ShowDialogue("不好！小恶魔又来了，我们一定要阻止它", bdText);
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
        IEnumerator SpeckerCoroutineDD(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
        
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
                    //介绍游戏
                  //  mask.gameObject.SetActive(false);
                    buDing.gameObject.SetActive(false);
                    devil.gameObject.SetActive(false);
                    mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 2, ()=> 
                    { 
                        bd.gameObject.SetActive(true);
                        _Mask.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(bd.gameObject, "speak", true); 
                    }, () => 
                    {
                        
                        SpineManager.instance.DoAnimation(bd.gameObject, "daiji", true);
                        bd.gameObject.SetActive(false);
                        _Mask.gameObject.SetActive(false);
                        mask.SetActive(false);
                        TDGameStartFunc();
                    }));
                    break;
                case 2:
                    //介绍游戏
                   

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
            mask.SetActive(false);
            //bd.SetActive(true);
            //mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(true); bd.SetActive(false); }));
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
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }
        
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target,SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
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
        void WaitTimeAndExcuteNext(float timer,Action callback)
        {
            
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
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
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
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
            mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
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
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name)+1, null, () =>
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
            string result = string.Empty;
            hasbeenOnClick = false;
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

            if (!hasbeenOnClick)
            {

                hasbeenOnClick = true;

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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); Replay(); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutineDD(dbd, SoundManager.SoundType.SOUND, 5)); });
                    }

                });
            }
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
            bd.gameObject.SetActive(false);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
        ;
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

        #endregion

        private void Replay() //重玩
        {
            bd.gameObject.SetActive(false);
          
            bdText.text = "";
            devilText.text = "";
            devil.gameObject.SetActive(false);
            buDing.gameObject.SetActive(false);
            spine_Mask.gameObject.SetActive(false);
            Level1.gameObject.SetActive(true); Level2.gameObject.SetActive(false); Level3.gameObject.SetActive(false);
            lanhua.gameObject.SetActive(true);huanghua.gameObject.SetActive(false);honghua.gameObject.SetActive(false);
            uiPanel.GetChild(2).gameObject.SetActive(true); uiPanel.GetChild(3).gameObject.SetActive(true); uiPanel.GetChild(4).gameObject.SetActive(true);
            //小恶魔初始化
            SpineManager.instance.DoAnimation(uiPanel.GetChild(0).gameObject,"xem",true);
            //蓝花初始化
            lanhua.GetChild(0).gameObject.SetActive(true); lanhua.GetChild(1).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(lanhua.GetChild(2).gameObject, "0kong", true);
            //花瓣
            SpineManager.instance.DoAnimation(lanhua.GetChild(1).gameObject, "LanHuaA3", false);
            SpineManager.instance.DoAnimation(lanhua.GetChild(0).gameObject, "LanHuaA2", false);
            //黄花初始化
            huanghua.GetChild(0).gameObject.SetActive(true); huanghua.GetChild(1).gameObject.SetActive(true); huanghua.GetChild(2).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(huanghua.GetChild(3).gameObject, "0kong", true);
            //花瓣
            SpineManager.instance.DoAnimation(huanghua.GetChild(0).gameObject, "HuangHuaA12", true);
            SpineManager.instance.DoAnimation(huanghua.GetChild(1).gameObject, "HuangHuaA14", true);
            SpineManager.instance.DoAnimation(huanghua.GetChild(2).gameObject, "HuangHuaA13", true);
            //红花初始化
            honghua.GetChild(0).gameObject.SetActive(true); honghua.GetChild(1).gameObject.SetActive(true); honghua.GetChild(2).gameObject.SetActive(true);
            honghua.GetChild(3).gameObject.SetActive(true); honghua.GetChild(4).gameObject.SetActive(true); honghua.GetChild(5).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(honghua.GetChild(6).gameObject, "0kong", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(0).gameObject, "HongHuaB03", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(1).gameObject, "HongHuaB04", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(2).gameObject, "HongHuaB05", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(3).gameObject, "HongHuaB06", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(4).gameObject, "HongHuaB01", true);
            SpineManager.instance.DoAnimation(honghua.GetChild(5).gameObject, "HongHuaB02", true);
            //数据初始化
            JustOnlyOneIndex = 0; JustOnlyOneIndex2 = 0; JustOnlyOneIndex3 = 0; JustOnlyOneIndex4 = 0; JustOnlyOneIndex5 = 0;
            Level2_index = 0; Level3_index = 0; isNextLeve2 = false;
            isNextLevel = false;
            //荷叶位置初始化
            //第一关 荷花显示
            Level1.GetChild(0).gameObject.SetActive(true);
            Level1.GetChild(1).gameObject.SetActive(true);
            Level1.GetChild(2).gameObject.SetActive(true);
            Level1.GetChild(3).gameObject.SetActive(true);

            Level2.GetChild(0).gameObject.SetActive(true);
            Level2.GetChild(1).gameObject.SetActive(true);
            Level2.GetChild(2).gameObject.SetActive(true);
            Level2.GetChild(3).gameObject.SetActive(true);
            Level2.GetChild(4).gameObject.SetActive(true);

            Level3.GetChild(0).gameObject.SetActive(true);
            Level3.GetChild(1).gameObject.SetActive(true);
            Level3.GetChild(2).gameObject.SetActive(true);
            Level3.GetChild(3).gameObject.SetActive(true);
            Level3.GetChild(4).gameObject.SetActive(true); Level3.GetChild(5).gameObject.SetActive(true);
            //第二关
            Level2.GetChild(0).transform.position = curTrans.Find("Huang1Pos").position;
            Level2.GetChild(1).transform.position = curTrans.Find("Lan1Pos").position;
            Level2.GetChild(2).transform.position = curTrans.Find("Hong1Pos").position;
            Level2.GetChild(3).transform.position = curTrans.Find("ShenHuang1Pos").position;
            Level2.GetChild(4).transform.position = curTrans.Find("Lv1Pos").position;

            Level3.GetChild(0).transform.position = curTrans.Find("Lan2Pos").position;
            Level3.GetChild(1).transform.position = curTrans.Find("ShenHong2Pos").position;
            Level3.GetChild(2).transform.position = curTrans.Find("Lv2Pos").position;
            Level3.GetChild(3).transform.position = curTrans.Find("Huang2Pos").position;
            Level3.GetChild(4).transform.position = curTrans.Find("QianHong2Pos").position;
            Level3.GetChild(5).transform.position = curTrans.Find("Hong2Pos").position;
            //第三关

        }
        #region 点击相关
        private void InitOnClick() //点击相关
        {
            Util.AddBtnClick(Huang.gameObject, OnClick_1);
            Util.AddBtnClick(Hong.gameObject, OnClick_2);
            Util.AddBtnClick(Lv.gameObject, OnClick_3);
            Util.AddBtnClick(Lan.gameObject, OnClick_4);

            Util.AddBtnClick(Huang1.gameObject, OnClick_5);
            Util.AddBtnClick(Hong1.gameObject, OnClick_6);
            Util.AddBtnClick(Lv1.gameObject, OnClick_7);
            Util.AddBtnClick(Lan1.gameObject, OnClick_8);
            Util.AddBtnClick(ShenHuang1.gameObject, OnClick_9);

            Util.AddBtnClick(huang2.gameObject, OnClick_10);
            Util.AddBtnClick(hong2.gameObject, OnClick_11);
            Util.AddBtnClick(lv2.gameObject, OnClick_12);
            Util.AddBtnClick(lan2.gameObject, OnClick_13);
            Util.AddBtnClick(Shenhong2.gameObject, OnClick_14);
            Util.AddBtnClick(qianhong2.gameObject, OnClick_15);
        }
        //第一关
        private void OnClick_1(GameObject obj) 
        {
            //错误
            _isPause = true;
          
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject,SoundManager.SoundType.SOUND,4,()=> { _Mask.gameObject.SetActive(true); },()=> { _Mask.gameObject.SetActive(false); })) ;
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Huang.GetChild(0).gameObject, "huangHYa1", false);
         
            IEwait(1f,()=> { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_2(GameObject obj)
        {
            //错误
            _isPause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Hong.GetChild(0).gameObject, "hongHYc1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_3(GameObject obj)
        {
            //错误
            _isPause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Lv.GetChild(0).gameObject, "lvHY1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_4(GameObject obj)
        {
            Debug.Log("点击正确了");
           
            isNextLevel = true;
            _Mask.gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
            SpineManager.instance.DoAnimation(lanhua.GetChild(1).gameObject, "LanHuaA2BS", false, () =>
            {
                lanhua.GetChild(0).gameObject.SetActive(false); lanhua.GetChild(1).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(lanhua.GetChild(2).gameObject, "LanHuaA0", true);
            });


            //等待一会儿 炮弹特效出现 击中小恶魔
            IEwait(2f, () =>
            {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                {
                    SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    
                    SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                    {
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);
                        uiPanel.GetChild(4).gameObject.SetActive(false);

                      

                        //a = a - Time.deltaTime;
                        //  mono.StartCoroutine(ChangeColorA(a, 0.1f));
                        //  mono.StartCoroutine(ChangeColorA(a,0.5f));
                        //  huanghua.gameObject.SetActive(true);
                        // mono.StartCoroutine(ChangeColorB(b, 0.2f));
                    });
                });
            });

            // Level_success();
        }
        //第二关
        private void OnClick_5(GameObject obj)
        {
            //正确
            
            JustOnlyOneIndex++;
            
            if (JustOnlyOneIndex == 1)
            {
                _Mask.gameObject.SetActive(true);
                Level2_index++;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(huanghua.GetChild(2).gameObject, "HuangHuaA2BS", false,()=> 
                {
                    _Mask.gameObject.SetActive(false);
                });
            }
            else 
            {
                //错误
                _isPause = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
                spine_Mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Huang1.GetChild(0).gameObject, "huangHYa1", false);

                IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });

            }
            if (Level2_index == 2)
            {
                //    Level2_success();
                //   Level2_success();
                isNextLeve2 = true;
                _Mask.gameObject.SetActive(true);

                SpineManager.instance.DoAnimation(huanghua.GetChild(3).gameObject, "HuangHuaA0", true);
                huanghua.GetChild(0).gameObject.SetActive(false); huanghua.GetChild(1).gameObject.SetActive(false); huanghua.GetChild(2).gameObject.SetActive(false);


                //等待一会儿 炮弹特效出现 击中小恶魔
                IEwait(2f, () =>
                {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                    {
                        SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
              
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                        {
                            SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);
                            uiPanel.GetChild(3).gameObject.SetActive(false);

                           

                            //a = a - Time.deltaTime;
                            //  mono.StartCoroutine(ChangeColorA(a, 0.1f));
                            //  mono.StartCoroutine(ChangeColorA(a,0.5f));
                            //  huanghua.gameObject.SetActive(true);
                            // mono.StartCoroutine(ChangeColorB(b, 0.2f));
                        });
                    });
                });

            }
        }
        private void OnClick_9(GameObject obj)
        {

            //SpineManager.instance.DoAnimation(HuangHua2.gameObject, "HuangHuaA3BS", false, () => { SpineManager.instance.DoAnimation(HuangHua2.gameObject, "HuangHuaA3BS2", true); });
            // SpineManager.instance.DoAnimation(hua.gameObject, "HuangHuaA12", true);
            JustOnlyOneIndex2++;
            if (JustOnlyOneIndex2 == 1) 
            {
                _Mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Level2_index++;
                SpineManager.instance.DoAnimation(huanghua.GetChild(1).gameObject, "HuangHuaA3BS", false,()=> 
                {
                    _Mask.gameObject.SetActive(false);
                });
            }
            else
            {
                //错误
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
                _isPause = true;
                spine_Mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(ShenHuang1.GetChild(0).gameObject, "huangHYb1", false);

                IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
            }
            if (Level2_index == 2)
            {
                //   Level2_success();
                isNextLeve2 = true;
                _Mask.gameObject.SetActive(true);

                SpineManager.instance.DoAnimation(huanghua.GetChild(3).gameObject, "HuangHuaA0", true);
                huanghua.GetChild(0).gameObject.SetActive(false); huanghua.GetChild(1).gameObject.SetActive(false); huanghua.GetChild(2).gameObject.SetActive(false);


                //等待一会儿 炮弹特效出现 击中小恶魔
                IEwait(2f, () =>
                {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                    {
                        SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                        
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                        {
                            SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);
                            uiPanel.GetChild(3).gameObject.SetActive(false);

                           

                            //a = a - Time.deltaTime;
                            //  mono.StartCoroutine(ChangeColorA(a, 0.1f));
                            //  mono.StartCoroutine(ChangeColorA(a,0.5f));
                            //  huanghua.gameObject.SetActive(true);
                            // mono.StartCoroutine(ChangeColorB(b, 0.2f));
                        });
                    });
                });
            }
          
        }
        private void OnClick_6(GameObject obj)
        {
            //错误
            _isPause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Hong1.GetChild(0).gameObject, "hongHYa1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_7(GameObject obj)
        {
            //错误
            _isPause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Lv1.GetChild(0).gameObject, "lvHY1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_8(GameObject obj)
        {
            //错误
            _isPause = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            spine_Mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Lan1.GetChild(0).gameObject, "lanHY1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        //第三关
        private void OnClick_10(GameObject obj)
        {
            //错误
            _isPause = true;
            spine_Mask.gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            SpineManager.instance.DoAnimation(huang2.GetChild(0).gameObject, "huangHYa1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_12(GameObject obj)
        {
            //错误
            _isPause = true;
            spine_Mask.gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            SpineManager.instance.DoAnimation(lv2.GetChild(0).gameObject, "lvHY1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_13(GameObject obj)
        {
            //错误
            _isPause = true;
            spine_Mask.gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
            SpineManager.instance.DoAnimation(lan2.GetChild(0).gameObject, "lanHY1", false);

            IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
        }
        private void OnClick_11(GameObject obj)
        {

            JustOnlyOneIndex3++;

            if (JustOnlyOneIndex3 == 1)
            {
                _Mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                Debug.LogError("正确");
                Level3_index++;
                SpineManager.instance.DoAnimation(honghua.GetChild(3).gameObject, "HongHuaB06BS", false,()=> 
                {
                    _Mask.gameObject.SetActive(false);
                });
            }
            else
            {
                Debug.LogError("错误");
                //错误
                _isPause = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
                spine_Mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(hong2.GetChild(0).gameObject, "hongHYa1", false);

                IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
            }
            if (Level3_index == 3)
            {
                 Level3_success(); 
             
            }
        }
        private void OnClick_14(GameObject obj)
        {
            JustOnlyOneIndex4++;

            if (JustOnlyOneIndex4 == 1)
            {
                Level3_index++;
                _Mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(honghua.GetChild(2).gameObject, "HongHuaB05BS", false,()=> 
                {
                    _Mask.gameObject.SetActive(false);
                });
            }
            else
            {
                //错误
                _isPause = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
                spine_Mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Shenhong2.GetChild(0).gameObject, "hongHYb1", false);

                IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
            }
            if (Level3_index == 3)
            {
               Level3_success();
            }
        }
        private void OnClick_15(GameObject obj)
        {
            JustOnlyOneIndex5++;

            if (JustOnlyOneIndex5 == 1)
            {
                Level3_index++;
                _Mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2);
                SpineManager.instance.DoAnimation(honghua.GetChild(1).gameObject, "HongHuaB04BS",false,()=> 
                {
                    _Mask.gameObject.SetActive(false);
                });
            }
            else
            {
                //错误
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                mono.StartCoroutine(SpeckerCoroutineDD(bd.gameObject, SoundManager.SoundType.SOUND, 4, () => { _Mask.gameObject.SetActive(true); }, () => { _Mask.gameObject.SetActive(false); }));
                _isPause = true;
                spine_Mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(qianhong2.GetChild(0).gameObject, "hongHYc1", false);

                IEwait(1f, () => { _isPause = false; spine_Mask.gameObject.SetActive(false); });
            }
            if (Level3_index == 3) 
            {
              Level3_success();
            }
        }
        #endregion

        private void Level_success() 
        {
            StopAllCoroutines();

            SpineManager.instance.DoAnimation(lanhua.GetChild(1).gameObject, "LanHuaA2BS", false, () =>
            {
                lanhua.GetChild(0).gameObject.SetActive(false); lanhua.GetChild(1).gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(lanhua.GetChild(2).gameObject, "LanHuaA0", true);
            });


            //等待一会儿 炮弹特效出现 击中小恶魔
            IEwait(2f, () =>
            {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                {
                    SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);

                    SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                    {
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);
                        uiPanel.GetChild(4).gameObject.SetActive(false);
                    });
                });
            });




            IEwait(3.5f, () => //下一关
            {
                for (int i = 0; i < Level2.childCount; i++)
                {
                    var child = Level2.GetChild(i);
                    var rect = child.GetRectTransform();
                    IEmove2(rect, -1177f, 1174f, _moveSpeed2, RightMove);

                }

                spine_Mask.gameObject.SetActive(false);
                Level1.gameObject.SetActive(false);
                Level2.gameObject.SetActive(true);

                //   SpineManager.instance.DoAnimation(hua.gameObject,"HuangHuaA1",true);
                lanhua.gameObject.SetActive(false);
                huanghua.gameObject.SetActive(true);
            });
        }
        private void Level2_success() 
        {
            StopAllCoroutines();
            spine_Mask.gameObject.SetActive(true);
            huanghua.GetChild(0).gameObject.SetActive(false); huanghua.GetChild(1).gameObject.SetActive(false); huanghua.GetChild(2).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(huanghua.GetChild(3).gameObject, "HuangHuaA0", true);

            IEwait(2f, () =>
            {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                {
                    SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);

                    SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                    {
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);
                        uiPanel.GetChild(3).gameObject.SetActive(false);
                    });
                });
            });



            IEwait(3.5f, () => //下一关
            {
                for (int i = 0; i < Level3.childCount; i++)
                {
                    var child = Level3.GetChild(i);
                    var rect = child.GetRectTransform();
                    IEmove2(rect, -1177f, 1174f, _moveSpeed3, RightMove);

                }

                spine_Mask.gameObject.SetActive(false);
                Level2.gameObject.SetActive(false);
                Level3.gameObject.SetActive(true);


                huanghua.gameObject.SetActive(false);
                honghua.gameObject.SetActive(true);
            });
        }
        private void Level3_success() 
        {
            StopAllCoroutines();
            _Mask.gameObject.SetActive(true);
            spine_Mask.gameObject.SetActive(true);
            honghua.GetChild(0).gameObject.SetActive(false);
            honghua.GetChild(1).gameObject.SetActive(false);
            honghua.GetChild(2).gameObject.SetActive(false);
            honghua.GetChild(3).gameObject.SetActive(false);
            honghua.GetChild(4).gameObject.SetActive(false);
            honghua.GetChild(5).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(honghua.GetChild(6).gameObject, "HongHuaA0", true);

            IEwait(2f, () =>
            {//1.炮弹特效播放完设为空2. 击败小恶魔 3.ui血条减少
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                SpineManager.instance.DoAnimation(texiao.gameObject, "jgsx", false, () =>
                {
                    SpineManager.instance.DoAnimation(texiao.gameObject, "0kong", false);

                    SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "xem-", false, () =>
                    {

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3);
                    
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(1).gameObject, "kong", false);//爆炸特效
                        SpineManager.instance.DoAnimation(uiPanel.GetChild(0).gameObject, "kong", false);//小恶魔消失
                        uiPanel.GetChild(2).gameObject.SetActive(false);
                      
                    });
                });
            });
            //成功动画
            IEwait(5f, () => { playSuccessSpine(); });
        }
        #region 移动相关方法
        private void InitMove() //移动相关
        {
            for (int i = 0; i < Level1.childCount; i++)
            {
                L1_speed = Random.Range(6.5f,10);
                var child = Level1.GetChild(i);
                var rect = child.GetRectTransform();
                IEmove2(rect, -1177f, 1174f, L1_speed, RightMove);
             
            }
 
        }
        private void InitMove_test() //移动相关
        {
            for (int i = 0; i < Level2.childCount; i++)
            {
                L1_speed = Random.Range(6.5f, 10);
                var child = Level2.GetChild(i);
                var rect = child.GetRectTransform();
                IEmove2(rect, -1177f, 1174f, L1_speed, RightMove);

            }
        }
        private void InitMove_test2() //移动相关
        {
            for (int i = 0; i < Level3.childCount; i++)
            {
                L1_speed = Random.Range(6.5f, 10);
                var child = Level3.GetChild(i);
                var rect = child.GetRectTransform();
                IEmove2(rect, -1177f, 1174f, L1_speed, RightMove);

            }
        }
        void LeftMove(RectTransform rect, float startXPos, float endXPos, float speed)
        {
            Debug.Log(speed);
            if (rect.anchoredPosition.x <= endXPos)
             rect.anchoredPosition = new Vector2(startXPos, rect.anchoredPosition.y);
            rect.Translate(Vector2.right * speed);
        }
        void RightMove(RectTransform rect, float startXPos, float endXPos, float speed)
        {
            if (isNextLevel && rect.anchoredPosition.x >= endXPos)
            {
                rect.gameObject.SetActive(false);
                
            }
            if (isNextLeve2 && rect.anchoredPosition.x >= endXPos) 
            {
                rect.gameObject.SetActive(false);
              
            }
            if (rect.anchoredPosition.x >= endXPos)
            { 
                rect.anchoredPosition = new Vector2(startXPos, rect.anchoredPosition.y);
            }
            //for (int i = 0; i < Level1.childCount; i++) 
            //{
                if (Level1.GetChild(0).gameObject.activeInHierarchy == false && Level1.GetChild(1).gameObject.activeInHierarchy == false && Level1.GetChild(2).gameObject.activeInHierarchy == false && Level1.GetChild(3).gameObject.activeInHierarchy == false&&isNextLevel==true) 
                {
                   Level2.gameObject.SetActive(true);
                lanhua.gameObject.SetActive(false);
                huanghua.gameObject.SetActive(true);
                    InitMove_test();
                    isNextLevel = false;
                _Mask.gameObject.SetActive(false);
            }

            if (Level2.GetChild(0).gameObject.activeInHierarchy == false && Level2.GetChild(1).gameObject.activeInHierarchy == false && Level2.GetChild(2).gameObject.activeInHierarchy == false && Level2.GetChild(3).gameObject.activeInHierarchy == false && Level2.GetChild(4).gameObject.activeInHierarchy == false && isNextLeve2 == true)
            {
                Level3.gameObject.SetActive(true);
               huanghua.gameObject.SetActive(false);
                honghua.gameObject.SetActive(true);
                InitMove_test2();
                isNextLeve2 = false;
                _Mask.gameObject.SetActive(false);
            }
            //}

            rect.Translate(Vector2.right * speed);
        }
        private void IEmove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack) 
        {
            mono.StartCoroutine(IEMove(rect, startXPos, endXPos,  speed, moveCallBack));
        }
        Coroutine IEmove2(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
             return mono.StartCoroutine(IEMove(rect, startXPos, endXPos, speed, moveCallBack));
        }
        IEnumerator IEMove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            while (true)
            {
                if (!_isPause)
                {
                    yield return new WaitForSeconds(0.02f);
                    moveCallBack?.Invoke(rect, startXPos, endXPos, speed);
                }
                else
                {
                    yield return null;
                }

            }
        }
        #endregion

        #region 协程相关
        private void StopCoroutines(IEnumerator routine)
        {
            mono.StopCoroutine(routine);
        }
        private void StopCoroutines2(Coroutine routine)
        {
          
            mono.StopCoroutine(routine);
        }
        private void IEwait(float time, Action method_2 = null) 
        {
            mono.StartCoroutine(IEwaitF(time,method_2));
        }
        private void StopAllCoroutines()
        {
            mono.StopAllCoroutines();
        }
        IEnumerator IEwaitF(float time, Action method_2 = null) 
        {
            yield return new WaitForSeconds(time);
            method_2?.Invoke();
        }
    
        //IEnumerator ChangeColorA(float aa,float time) 
        //{
        //    while (true) 
        //    {
        //        yield return new WaitForSeconds(time);
        //        a = a - 34;
        //        lanhua.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, a / 255);
        //        if (a < 0) 
        //        {
        //            lanhua.GetChild(2).gameObject.SetActive(false);
        //            break;
        //        }
        //    }
        //}
        //IEnumerator ChangeColorB(float aa, float time)
        //{
        //    while (true)
        //    {
        //        yield return new WaitForSeconds(time);
        //        b = b + 34;
        //        huanghua.GetChild(0).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, b / 255);
        //        huanghua.GetChild(1).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, b / 255);
        //        huanghua.GetChild(2).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, b / 255);
        //        if (b==255)
        //        {
        //            break;
        //        }
        //    }
        //}
        #endregion



        #endregion





    }
}
