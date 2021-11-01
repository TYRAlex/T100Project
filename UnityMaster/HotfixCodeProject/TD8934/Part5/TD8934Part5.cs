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
    public class TD8934Part5
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

        private Transform _mask;
        private Transform Map;
        private Transform tab;
        private Transform door;
        private GameObject star;
        private GameObject star2;
        private GameObject star3;
        private Transform people_shou;
        private Transform people_shou2;
        private Transform OnClickToNext;
        private Transform Drag1;
        private List<mILDrager> _iLDragers;
        private List<mILDrager> _iLDragers2;
        private List<mILDrager> _iLDragers3;

        private bool hasbeenOnClick;
        private Transform drag1_1;
        private Transform drag1_2;
        private Transform drag1_3;

        private Transform drag2_1;
        private Transform drag2_2;
        private Transform drag2_22;
        private Transform drag2_3;
        private Transform drag2_4;
        private Transform drag2_44;
        private Transform drag2_5;

        private Transform drag3_1;
        private Transform drag3_2;
        private Transform drag3_3;
        private Transform drag3_4;
        private Transform drag3_5;
        private Transform drag3_6;
        private Transform drag3_7;
     

        private int index1;

        private Transform Level1_Panel;
        private Transform Level2_Panel;
        private Transform Level3_Panel;

        private Transform Level1;
        private Transform Level1_Drag;
        private Transform Level1_Drop;


        private Transform Level2;
        private Transform Level2_Drag;
        private Transform Level2_Drop;


        private Transform Level3;
        private Transform Level3_Drag;
        private Transform Level3_Drop;

        private GameObject xem;

        private Dictionary<string, Vector2> dic;
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

            Bg = curTrans.Find("Bg1").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            hasbeenOnClick = false;
            dic = new Dictionary<string, Vector2>();
            star = curTrans.Find("drag1/star").gameObject;
            star2 = curTrans.Find("drag2/star").gameObject;
            star3 = curTrans.Find("drag3/star").gameObject;
            door = curTrans.Find("door");
            OnClickToNext = curTrans.Find("OnClick");
            Map = curTrans.Find("Bg1/map");
            people_shou = curTrans.Find("people_shou");
            people_shou2 = curTrans.Find("people_shou2");
            tab = curTrans.Find("Level2/tab");
        //    door = curTrans.Find("door");
            Drag1 = curTrans.Find("drag1");
            _mask = curTrans.Find("_mask");
            Level1_Panel = curTrans.Find("Level1/Panel");
            Level2_Panel = curTrans.Find("Level2/Panel");
            Level3_Panel = curTrans.Find("Level3/Panel");

            drag1_1 = curTrans.Find("drag1/1");
            drag1_2 = curTrans.Find("drag1/2");
            drag1_3 = curTrans.Find("drag1/3");

            drag2_1 = curTrans.Find("drag2/1");
            drag2_2 = curTrans.Find("drag2/2");
            drag2_22 = curTrans.Find("drag2/22");
            drag2_3 = curTrans.Find("drag2/3");
            drag2_4 = curTrans.Find("drag2/4");
            drag2_44 = curTrans.Find("drag2/44");
            drag2_5 = curTrans.Find("drag2/5");

            drag3_1 = curTrans.Find("drag3/1");
            drag3_2 = curTrans.Find("drag3/2");
            drag3_3 = curTrans.Find("drag3/3");
            drag3_4 = curTrans.Find("drag3/4");
            drag3_5 = curTrans.Find("drag3/5");
            drag3_6 = curTrans.Find("drag3/6");
            drag3_7 = curTrans.Find("drag3/7");

            Level1 = curTrans.Find("Level1");
            Level1_Drag = curTrans.Find("drag1");
            Level1_Drop = curTrans.Find("drop1");

            Level2 = curTrans.Find("Level2");
            Level2_Drag = curTrans.Find("drag2");
            Level2_Drop = curTrans.Find("drop2");

            Level3 = curTrans.Find("Level3");
            Level3_Drag = curTrans.Find("drag3");
            Level3_Drop = curTrans.Find("drop3");

            xem = curTrans.Find("Bg1/xem").gameObject;
            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            InitILDragersLayers();
            InitILDragers();
            AddDragerEvents();

           
            GameInit();
      
           
            DicAdd();
            InitScale();
            CW();
            //GameStart();
        }

        
        

      

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            Input.multiTouchEnabled = false;
            _mask.gameObject.SetActive(false);

         //   CW();
         
           // InitScale();
            AddBtnClick();
            //田丁初始化
            for (int i = 0; i < OnClickToNext.childCount; i++) 
            {
                OnClickToNext.GetChild(i).gameObject.SetActive(false);
            }
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

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutinTTe(bd, SoundManager.SoundType.VOICE, 6, () =>
                {
                    ShowDialogue("哈哈哈哈！本大王来了", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutinTTe(bd, SoundManager.SoundType.VOICE, 7, () =>
                        {
                            ShowDialogue("不好！小恶魔又来捣乱了，我们一起来阻止它", bdText);
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
        IEnumerator SpeckerCoroutinTTe(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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
            mono.StartCoroutine(SpeckerCoroutinTTe(bd, SoundManager.SoundType.VOICE, 8, null, () => 
            {
                mask.SetActive(false); bd.SetActive(false);
                _mask.gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                WaitTime(1.5f, () =>
                {
                    SpineManager.instance.DoAnimation(Map.GetChild(0).gameObject, "map-a1", false,()=> 
                    {
                        _mask.gameObject.SetActive(false);
                    });
                  
                    OnClickToNext.GetChild(0).gameObject.SetActive(true);
                    OnClickToNext.GetChild(1).gameObject.SetActive(true);
                    OnClickToNext.GetChild(2).gameObject.SetActive(true);
                    OnClickToNext.GetChild(3).gameObject.SetActive(true);

                   // OnClickToNext.GetChild(7).gameObject.SetActive(true);
                });
            }));
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
           
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete( () => { LRBtnUpdate(); callBack?.Invoke(); isPlaying = false; });
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
            mono.StartCoroutine(SpeckerCoroutinTTe(bd,SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
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
                    mono.StartCoroutine(SpeckerCoroutinTTe(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name)+1, null, () =>
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
            //if (isPlaying)
            //    return;
            //isPlaying = true;
            if (hasbeenOnClick == false) 
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            mask.SetActive(false);
                            GameInit();
                            CW();
                            InitScale();
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,2);
                            WaitTime(1.5f, () =>
                            {
                                SpineManager.instance.DoAnimation(Map.GetChild(0).gameObject, "map-a1", false);
                                OnClickToNext.GetChild(0).gameObject.SetActive(true);
                                OnClickToNext.GetChild(1).gameObject.SetActive(true);
                               OnClickToNext.GetChild(2).gameObject.SetActive(true);
                               OnClickToNext.GetChild(3).gameObject.SetActive(true);

                                //OnClickToNext.GetChild(7).gameObject.SetActive(true);
                            });
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutinTTe(dbd, SoundManager.SoundType.VOICE, 9)); });
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
            devil.SetActive(false);
            buDing.SetActive(false);
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
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                           
                          
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion

        void InitILDragers()
        {
            _iLDragers = new List<mILDrager>();
            for (int i = 0; i < Level1_Drag.childCount-1; i++)
            {

                var iLDrager = Level1_Drag.GetChild(i).GetComponent<mILDrager>();
                _iLDragers.Add(iLDrager);
            }
            _iLDragers2 = new List<mILDrager>();
            for (int i = 0; i < Level2_Drag.childCount-1; i++)
            {

                var iLDrager = Level2_Drag.GetChild(i).GetComponent<mILDrager>();
                _iLDragers2.Add(iLDrager);
            }
           
            _iLDragers3 = new List<mILDrager>();
            for (int i = 0; i < Level3_Drag.childCount-1; i++)
            {

                var iLDrager = Level3_Drag.GetChild(i).GetComponent<mILDrager>();
                _iLDragers3.Add(iLDrager);
            }
        }
        void AddDragerEvents() 
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
        }
        void InitILDragersLayers() 
        {
            drag1_1.transform.SetAsFirstSibling();
            drag1_2.transform.SetSiblingIndex(1);
            drag1_3.transform.SetSiblingIndex(2);

            drag2_1.transform.SetAsFirstSibling();
            drag2_2.transform.SetSiblingIndex(1);
            drag2_22.transform.SetSiblingIndex(2);
            drag2_3.transform.SetSiblingIndex(3);
            drag2_4.transform.SetSiblingIndex(4);
            drag2_44.transform.SetSiblingIndex(5);
            drag2_5.transform.SetSiblingIndex(6);

            drag3_1.transform.SetAsFirstSibling();
            drag3_2.transform.SetSiblingIndex(1);
            drag3_3.transform.SetSiblingIndex(2);
            drag3_4.transform.SetSiblingIndex(3);
            drag3_5.transform.SetSiblingIndex(4);
            drag3_6.transform.SetSiblingIndex(5);
            drag3_7.transform.SetSiblingIndex(6);
        }
        void AddBtnClick()
        {
           Util.AddBtnClick(OnClickToNext.GetChild(0).gameObject,Level1_Next);
            Util.AddBtnClick(OnClickToNext.GetChild(1).gameObject, Level1_Next);
            Util.AddBtnClick(OnClickToNext.GetChild(2).gameObject, Level1_Next);
            Util.AddBtnClick(OnClickToNext.GetChild(3).gameObject, Level1_Next);
            Util.AddBtnClick(OnClickToNext.GetChild(4).gameObject, Level2_Next);
           Util.AddBtnClick(OnClickToNext.GetChild(5).gameObject, Level2_Next);
            Util.AddBtnClick(OnClickToNext.GetChild(6).gameObject, Level3_Next);
            // Util.AddBtnClick(OnClickToNext.GetChild(7).gameObject, Level1_Next);
            // Util.AddBtnClick(OnClickToNext.GetChild(8).gameObject, Level2_Next);
            // Util.AddBtnClick(OnClickToNext.GetChild(9).gameObject, Level3_Next);
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            BtnPlaySound();

            _iLDragers[index].transform.SetSiblingIndex(2);
            if (index == 0)
            {
                // drag1_1.transform.DOScale(0.5f,0.1f);
                //  drag1_1.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(517 / 2, 125.2f / 2);
                drag1_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_1"]/2;
            }
            else if (index == 1) 
            {
                drag1_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_2"]/2;
            }
            else if (index == 2)
            {
                drag1_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_3"]/1.2f;
            }
            _iLDragers[index].transform.position = Input.mousePosition;
 
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            BtnPlaySound();
            Debug.Log(index);
            _iLDragers2[index].transform.SetSiblingIndex(6);
            if (index == 0)
            {
                // drag1_1.transform.DOScale(0.5f,0.1f);
                //  drag1_1.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(517 / 2, 125.2f / 2);
                drag2_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_1"] / 1.5f;
            }
            else if (index == 1)
            {
                //  drag2_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_2"] / 2f;
                drag2_2.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 120);
                drag2_2.GetComponent<RawImage>().texture = drag2_2.GetComponent<BellSprites>().texture[1]; 
            }
            else if (index == 2)
            {
                drag2_22.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_22"] / 1.5f;
            }
            else if (index == 3)
            {
                drag2_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_3"] / 4;
            }
            else if (index == 4)
            {
                drag2_4.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_4"] / 2f;
                drag2_4.transform.GetComponent<RectTransform>().sizeDelta = new Vector2(180, 120);
                drag2_4.GetComponent<RawImage>().texture = drag2_4.GetComponent<BellSprites>().texture[1];
            }
            else if (index == 5)
            {
                drag2_44.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_44"] / 1.5f;
            }
            else if (index == 6)
            {
                drag2_5.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_5"] / 1.5f;
            }
            _iLDragers2[index].transform.position = Input.mousePosition;
   
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            BtnPlaySound();
            _iLDragers3[index].transform.SetSiblingIndex(6);
            _iLDragers3[index].transform.position = Input.mousePosition;
      
        }
        private void Draging(Vector3 position, int type, int index)
        {

       
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
         
            if (isMatch)
            {
                RightVoice();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
                if (type == 1)
                {
                    index1++;
                    drag1_1.gameObject.SetActive(false);
                  //  Level1_Panel.GetChild(2).gameObject.SetActive(false);
                    Level1_Panel.GetChild(5).gameObject.SetActive(true);
                 
                    ShowStar(position);
                }
                if (type == 2)
                {
                    index1++;
                    drag1_2.gameObject.SetActive(false);
                  //  Level1_Panel.GetChild(0).gameObject.SetActive(false);
                    Level1_Panel.GetChild(3).gameObject.SetActive(true);
                 
                    ShowStar(position);
                }
                if (type == 3)
                {
                    index1++;
                    drag1_3.gameObject.SetActive(false);
                  //  Level1_Panel.GetChild(1).gameObject.SetActive(false);
                    Level1_Panel.GetChild(4).gameObject.SetActive(true);
               
                    ShowStar(position);
                }
                if (index1 == 3) 
                {
                    index1 = 0;
                    _mask.gameObject.SetActive(true);
                    WaitTime(2f,()=>
                    {
                        Level1.gameObject.SetActive(false);
                        Level1_Drag.gameObject.SetActive(false);
                        Level1_Drop.gameObject.SetActive(false);
                        
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        WaitTime(1.5f, () => 
                        {
                            SpineManager.instance.DoAnimation(Map.GetChild(0).gameObject, "map-b1", false);
                            SpineManager.instance.DoAnimation(Map.GetChild(1).gameObject,"map-a2",false,()=> 
                            {
                                _mask.gameObject.SetActive(false);

                            });
                           OnClickToNext.GetChild(4).gameObject.SetActive(true);
                            OnClickToNext.GetChild(5).gameObject.SetActive(true);

                           // OnClickToNext.GetChild(8).gameObject.SetActive(true);
                        });
                    });
                   
                }
            }
            else 
            {
                FaultVoice();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                _iLDragers[index].DoReset();
                if (index == 0) { drag1_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_1"]; }
                else if (index == 1) { drag1_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_2"]; }
                else if (index == 2) { drag1_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_3"]; }
            }
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            
            if (isMatch)
            {
             
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                if (type == 1)
                {
                    index1++;
                    drag2_1.gameObject.SetActive(false);
                   // Level2_Panel.GetChild(2).gameObject.SetActive(false);
                    Level2_Panel.GetChild(4).gameObject.SetActive(true);
                    ShowStar2(position);
                }
                if (type == 2)
                {
                    if (index == 4) 
                    { 
                        index1++;
                        drag2_4.gameObject.SetActive(false);
                        Level2_Panel.GetChild(5).gameObject.SetActive(true);
                        drag2_44.GetComponent<mILDrager>().isActived = false;
                        ShowStar2(position);
                    }
                    if (index == 5)
                    {
                        index1++;
                        drag2_44.gameObject.SetActive(false);
                        Level2_Panel.GetChild(5).gameObject.SetActive(true);
                        drag2_4.GetComponent<mILDrager>().isActived = false;
                        ShowStar2(position);
                    }   
                }
                if (type == 3)
                {
                    index1++;
                    drag2_3.gameObject.SetActive(false);  
                    Level2_Panel.GetChild(6).gameObject.SetActive(true);
                    ShowStar2(position);
                }
                if (type == 4)
                {
                    index1++;
                    drag2_5.gameObject.SetActive(false);
                    Level2_Panel.GetChild(7).gameObject.SetActive(true);
                    ShowStar2(position);
                }
                RightVoice2();
   
                if (index1 == 4)
                {
                  
                 
                    _mask.gameObject.SetActive(true);
                    WaitTime(2f, () =>
                    {
                        index1 = 0;
                        Level2.gameObject.SetActive(false);
                        Level2_Drag.gameObject.SetActive(false);
                        Level2_Drop.gameObject.SetActive(false);
                        door.gameObject.SetActive(false);
                        tab.gameObject.SetActive(false);
                        // door.gameObject.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                   

                        WaitTime(1.5f, () =>
                        {
                            SpineManager.instance.DoAnimation(Map.GetChild(1).gameObject, "map-b2", false,()=> 
                            {
                                SpineManager.instance.DoAnimation(Map.GetChild(2).gameObject, "map-a3", false, () =>
                                {
                                    _mask.gameObject.SetActive(false);
                                });
                            });
                         
                          
                            OnClickToNext.GetChild(6).gameObject.SetActive(true);

                           // OnClickToNext.GetChild(9).gameObject.SetActive(true);

                        });
                    });

                }
            }
            else
            {
                FaultVoice();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
               
             //   drag2_2.GetComponent<RawImage>().texture = drag2_2.GetComponent<BellSprites>().texture[0];
                _iLDragers2[index].DoReset();
                if (index == 0) { drag2_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_1"]; }
                else if (index == 1)
                {
                    drag2_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_2"]; 
                    drag2_2.GetComponent<RawImage>().texture = drag2_2.GetComponent<BellSprites>().texture[0];
                }
                else if (index == 2) { drag2_22.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_22"]; }
                else if (index == 3) { drag2_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_3"]; }
                else if (index == 4)
                {
                    drag2_4.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_4"];
                    drag2_4.GetComponent<RawImage>().texture = drag2_4.GetComponent<BellSprites>().texture[0];
                }
                else if (index == 5) { drag2_44.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_44"]; }
                else if (index == 6) { drag2_5.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_5"]; }
            }
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
           
            if (isMatch)
            {
                RightVoice();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                if (type == 1)
                {
                 
                    index1++;
                    drag3_2.gameObject.SetActive(false);
                  
                    Level3_Panel.GetChild(5).gameObject.SetActive(true);
                    ShowStar3(position);
                }
                if (type == 2)
                {
                    index1++;
                    drag3_3.gameObject.SetActive(false);
                   
                    Level3_Panel.GetChild(6).gameObject.SetActive(true);
                    ShowStar3(position);
                }
                if (type == 3)
                {
                    index1++;
                    drag3_1.gameObject.SetActive(false);
                 
                    Level3_Panel.GetChild(7).gameObject.SetActive(true);
                    ShowStar3(position);
                }
                if (type == 4)
                {
                    index1++;
                    drag3_4.gameObject.SetActive(false);
                    
                    Level3_Panel.GetChild(8).gameObject.SetActive(true);
                    ShowStar3(position);
                }
                if (type == 5)
                {
                    index1++;
                    drag3_5.gameObject.SetActive(false);
                    Level3_Panel.GetChild(9).gameObject.SetActive(true);
                    ShowStar3(position);
                }
                if (index1 == 5)
                {
                    WaitTime(2f, () =>
                    {
                        Level3.gameObject.SetActive(false);
                        Level3_Drag.gameObject.SetActive(false);
                        Level3_Drop.gameObject.SetActive(false);
                        people_shou.gameObject.SetActive(false);
                        people_shou2.gameObject.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        WaitTime(1.5f, () =>
                        {
                           
                            SpineManager.instance.DoAnimation(Map.GetChild(2).gameObject, "map-b3", false);
                            SpineManager.instance.DoAnimation(Map.gameObject, "map2", false,()=> 
                            {
                                SpineManager.instance.DoAnimation(xem, "xem2", false);
                            });
                           
                            WaitTime(3f, () => { playSuccessSpine(); });
                            //  SpineManager.instance.DoAnimation(Map.GetChild(3).gameObject, "map-a3", false);
                        });
                    });

                }
            }
            else
            {
                FaultVoice();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                _iLDragers3[index].DoReset();
            }
        }
        IEnumerator IEwait(float time,Action method_1) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }
        private void WaitTime(float time, Action method_1) 
        {
            mono.StartCoroutine(IEwait(time,method_1));
        }
        private void Level1_Next(GameObject obj) 
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(Map.GetChild(0).gameObject,"kong",false);
            Level1.gameObject.SetActive(true);
            Level1_Drag.gameObject.SetActive(true);
            Level1_Drop.gameObject.SetActive(true);
            OnClickToNext.GetChild(0).gameObject.SetActive(false);
           OnClickToNext.GetChild(1).gameObject.SetActive(false);
           OnClickToNext.GetChild(2).gameObject.SetActive(false);
           OnClickToNext.GetChild(3).gameObject.SetActive(false);

           // OnClickToNext.GetChild(7).gameObject.SetActive(false);
        }
        private void Level2_Next(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(Map.GetChild(1).gameObject, "kong", false);
            Level2.gameObject.SetActive(true);
            Level2_Drag.gameObject.SetActive(true);
            Level2_Drop.gameObject.SetActive(true);
            tab.gameObject.SetActive(true);
            door.gameObject.SetActive(true);
           OnClickToNext.GetChild(4).gameObject.SetActive(false);
            OnClickToNext.GetChild(5).gameObject.SetActive(false);

           // OnClickToNext.GetChild(8).gameObject.SetActive(false);
        }
        private void Level3_Next(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(Map.GetChild(2).gameObject, "kong", false);
            Level3.gameObject.SetActive(true);
           
            Level3_Drag.gameObject.SetActive(true);
            Level3_Drop.gameObject.SetActive(true);
            people_shou.gameObject.SetActive(true);
            people_shou2.gameObject.SetActive(true);
          OnClickToNext.GetChild(6).gameObject.SetActive(false);


          //  OnClickToNext.GetChild(9).gameObject.SetActive(false);
            InitLevel3PeoPle();
        }
        private void CW()
        {
            index1 = 0;
            devilText.text = "";
            bdText.text = "";
            SpineManager.instance.DoAnimation(xem, "kong", false);
            SpineManager.instance.DoAnimation(xem, "xem", true);
            Level1.gameObject.SetActive(false); Level2.gameObject.SetActive(false); Level3.gameObject.SetActive(false);
            Level1_Drag.gameObject.SetActive(false); Level1_Drop.gameObject.SetActive(false);
            Level2_Drag.gameObject.SetActive(false); Level2_Drop.gameObject.SetActive(false);
            Level3_Drag.gameObject.SetActive(false); Level3_Drop.gameObject.SetActive(false);
            buDing.SetActive(true);devil.SetActive(true);
            door.gameObject.SetActive(false);
            drag2_44.GetComponent<mILDrager>().isActived = true;
            drag2_4.GetComponent<mILDrager>().isActived = true;
            Level1_Panel.GetChild(3).gameObject.SetActive(false);
            Level1_Panel.GetChild(4).gameObject.SetActive(false);
            Level1_Panel.GetChild(5).gameObject.SetActive(false);


            Level2_Panel.GetChild(4).gameObject.SetActive(false);
            Level2_Panel.GetChild(5).gameObject.SetActive(false);
            Level2_Panel.GetChild(6).gameObject.SetActive(false);
            Level2_Panel.GetChild(7).gameObject.SetActive(false);

            Level3_Panel.GetChild(8).gameObject.SetActive(false);
            Level3_Panel.GetChild(5).gameObject.SetActive(false);
            Level3_Panel.GetChild(6).gameObject.SetActive(false);
            Level3_Panel.GetChild(7).gameObject.SetActive(false);
            Level3_Panel.GetChild(9).gameObject.SetActive(false);

            people_shou.gameObject.SetActive(false);
            people_shou2.gameObject.SetActive(false);


            SpineManager.instance.DoAnimation(Map.gameObject,"map",false);
        
            SpineManager.instance.DoAnimation(Map.GetChild(0).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(Map.GetChild(1).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(Map.GetChild(2).gameObject, "kong", false);

            drag2_4.GetComponent<RawImage>().texture = drag2_4.GetComponent<BellSprites>().texture[0];

            // InitPos();
            InitPos();
            InitILDragersLayers();
            CZPos();
        }
        private void InitPos() 
        {
            InitILDragersLayers();
            for (int i = 0; i < Level1_Drag.childCount - 1; i++) 
            {
                
                _iLDragers[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < Level2_Drag.childCount-1; i++)
            {
                
                _iLDragers2[i].gameObject.SetActive(true);
            }
            for (int i = 0; i < Level3_Drag.childCount-1; i++)
            {
                
                _iLDragers3[i].gameObject.SetActive(true);
            }
        }
        private void ShowStar(Vector3 position) 
        {
            star.transform.localPosition = position;
            star.SetActive(true);
            SpineManager.instance.DoAnimation(star, "star", false,()=> 
            {
                SpineManager.instance.DoAnimation(star,"kong",false);
            });
        }
        private void ShowStar2(Vector3 position)
        {
            star2.transform.localPosition = position;
            star2.SetActive(true);
            SpineManager.instance.DoAnimation(star2, "star", false, () =>
            {
                SpineManager.instance.DoAnimation(star2, "kong", false);
            });
        }
        private void ShowStar3(Vector3 position)
        {
            star3.transform.localPosition = position;
            star3.SetActive(true);
            SpineManager.instance.DoAnimation(star3, "star", false, () =>
            {
                SpineManager.instance.DoAnimation(star3, "kong", false);
            });
        }
        private void CZPos() 
        {
            drag1_1.GetRectTransform().position = curTrans.Find("L1_1").position;
            drag1_2.GetRectTransform().position = curTrans.Find("L1_2").position;
            drag1_3.GetRectTransform().position = curTrans.Find("L1_3").position;

            drag2_1.GetRectTransform().position = curTrans.Find("L2_1").position;
            drag2_2.GetRectTransform().position = curTrans.Find("L2_2").position;
            drag2_22.GetRectTransform().position = curTrans.Find("L2_22").position;
            drag2_3.GetRectTransform().position = curTrans.Find("L2_3").position;
            drag2_4.GetRectTransform().position = curTrans.Find("L2_4").position;
            drag2_44.GetRectTransform().position = curTrans.Find("L2_44").position;
            drag2_5.GetRectTransform().position = curTrans.Find("L2_5").position;

            drag3_1.GetRectTransform().position = curTrans.Find("L3_1").position;
            drag3_2.GetRectTransform().position = curTrans.Find("L3_2").position;
            drag3_3.GetRectTransform().position = curTrans.Find("L3_3").position;
            drag3_4.GetRectTransform().position = curTrans.Find("L3_4").position;
            drag3_5.GetRectTransform().position = curTrans.Find("L3_5").position;
            drag3_6.GetRectTransform().position = curTrans.Find("L3_6").position;
            drag3_7.GetRectTransform().position = curTrans.Find("L3_7").position;
        }
        private void DicAdd() 
        {
            dic.Add("L1_1",new Vector2(517.5f,125.2f));
            dic.Add("L1_2", new Vector2(393.76f, 120f));
            dic.Add("L1_3", new Vector2(298.63f, 89.94f));

            dic.Add("L2_1", new Vector2(242.93f, 104.17f));
            dic.Add("L2_2", new Vector2(1056.1f, 132f));
            dic.Add("L2_22", new Vector2(337.8f, 149f));
            dic.Add("L2_3", new Vector2(762.2f, 193.3f));
            dic.Add("L2_4", new Vector2(1084.2f, 139.8f));
            dic.Add("L2_44", new Vector2(355f, 129.65f));
            dic.Add("L2_5", new Vector2(319.8f, 115.4f));
        }
        private void InitScale() 
        {
            drag1_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_1"];
            drag1_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_2"];
            drag1_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L1_3"];

            drag2_1.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_1"];
            drag2_2.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_2"];
            drag2_22.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_22"];
            drag2_3.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_3"];
            drag2_4.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_4"];
            drag2_44.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_44"];
            drag2_5.transform.GetComponent<RectTransform>().sizeDelta = dic["L2_5"];
        }
        private void InitLevel3PeoPle() 
        {
        
            SpineManager.instance.DoAnimation(Level3.GetChild(0).GetChild(0).gameObject,"nh",true);
            SpineManager.instance.DoAnimation(drag3_1.GetChild(0).gameObject, "nh6", true);
            SpineManager.instance.DoAnimation(drag3_2.GetChild(0).gameObject, "nh4", true);
            SpineManager.instance.DoAnimation(drag3_3.GetChild(0).gameObject, "nh3", true);
            SpineManager.instance.DoAnimation(drag3_4.GetChild(0).gameObject, "nh8", true);
            SpineManager.instance.DoAnimation(drag3_5.GetChild(0).gameObject, "nh2", true);
            SpineManager.instance.DoAnimation(drag3_6.GetChild(0).gameObject, "nh5", true);
            SpineManager.instance.DoAnimation(drag3_7.GetChild(0).gameObject, "nh7", true);
        }
        private void RightVoice() 
        {
            int i  = Random.Range(3,6);
            _mask.gameObject.SetActive(true);
            
            waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false),()=> 
            {
                _mask.gameObject.SetActive(false);
            });
          
        }
        private void RightVoice2()
        {
            int i = Random.Range(3, 6);
            _mask.gameObject.SetActive(true);
            
            waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false), () =>
            {
                _mask.gameObject.SetActive(false);
            
                if (index1 == 4) 
                {
               
                    _mask.gameObject.SetActive(true);
                }
            });

        }
        private void FaultVoice() 
        {
            int i = Random.Range(0,3);
            _mask.gameObject.SetActive(true);
        
            waiting(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false), () =>
            {
                _mask.gameObject.SetActive(false);
            });
        }
        IEnumerator wait(float time,Action method=null) 
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
        private void waiting(float time, Action method = null) 
        {
            mono.StartCoroutine(wait(time,method));
        }
    }
}
