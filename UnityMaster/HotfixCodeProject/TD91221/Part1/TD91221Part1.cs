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
    public class TD91221Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
       
        private BellSprites bellTextures;

        private Transform Bg;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;

        private Transform ar1;
        private Transform ar2;
        private Transform ar3;
        private Transform ar4;
        private Transform ar5;
        float a=255;
        
        private Transform yuPos;
        private Transform a1;
        private Transform a2;
        private Transform a3;
        private Transform a4;
        private Transform a5;
        private Transform soccer;
        private Transform spine;
        private Transform DTT;
        private Transform TT;
        private Transform _mask;
        private Transform ceshiweizhi;

        private int i;//i代表了语音序号

        private List<int> randomPlaySoundList;
        private bool isSuccessOnClick;
        private bool canClick;
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
        bool hasbeenOnClick = false;


        bool isPlaying = false;
       

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg");
            ar1 = curTrans.Find("ar1");
            ar2 = curTrans.Find("ar2");
            ar3 = curTrans.Find("ar3");
            ar4 = curTrans.Find("ar4");
            ar5 = curTrans.Find("ar5");
      
            yuPos = curTrans.Find("spine/yuPos");
            a1 = curTrans.Find("spine/a1");
            a2= curTrans.Find("spine/a2");
            a3 = curTrans.Find("spine/a3");
            a4 = curTrans.Find("spine/a4");
            a5 = curTrans.Find("spine/a5");
            soccer = curTrans.Find("spine/soccer");
            spine = curTrans.Find("spine");
            DTT = curTrans.Find("Bg/DTT");
            TT = curTrans.Find("TT");
            _mask = curTrans.Find("_mask");
            isSuccessOnClick = false;
            canClick = true;
            i = -1;
            //随机播放语音的List数组
            randomPlaySoundList = new List<int>();


            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
           // mono.StartCoroutine(Myxxx(0.2f));
            GameInit();
            //GameStart();
        }

        
        

      

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            Input.multiTouchEnabled = false;
            Util.AddBtnClick(ar1.gameObject,OnClickAr1);
            Util.AddBtnClick(ar2.gameObject, OnClickAr2);
            Util.AddBtnClick(ar3.gameObject, OnClickAr3);
            Util.AddBtnClick(ar4.gameObject, OnClickAr4);
            Util.AddBtnClick(ar5.gameObject, OnClickAr5);

            yuPos.DOLocalMove(new Vector3(655, -396f, 0), 0.1f);

            SpineManager.instance.DoAnimation(DTT.gameObject,"daiji",true);

            SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject,"yu",true);
      
            soccer.GetChild(0).gameObject.SetActive(true);
            soccer.GetChild(1).gameObject.SetActive(false);
            soccer.GetChild(2).gameObject.SetActive(false);
            soccer.GetChild(3).gameObject.SetActive(false);
            soccer.GetChild(4).gameObject.SetActive(false);
            soccer.GetChild(5).gameObject.SetActive(false);
            for (int i = 0; i < 5; i++) 
            {
                randomPlaySoundList.Add(i);
            }
            Bg.gameObject.SetActive(true);Bg.GetChild(0).gameObject.SetActive(true); Bg.GetChild(1).gameObject.SetActive(true); Bg.GetChild(2).gameObject.SetActive(true);
            spine.gameObject.SetActive(false);
            //田丁初始化
            TT.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject,"wuse2",true);
            TDGameInit();
        }

        

        void GameStart()
        {
            //田丁开始游戏
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            DTT.gameObject.SetActive(false);
            bd.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutineTT(bd,SoundManager.SoundType.SOUND,8,null,()=> 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            //mask.SetActive(false);
            _mask.gameObject.SetActive(true);

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

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }
        void TDGameStart()
        {
            Bg.GetChild(0).gameObject.SetActive(true); Bg.GetChild(1).gameObject.SetActive(true); Bg.GetChild(2).gameObject.SetActive(true);
    
            mono.StartCoroutine(SpeckerCoroutineTT(DTT.gameObject,SoundManager.SoundType.SOUND,0,null,()=> //介绍墨水的语音
            {

               


                SoundManager.instance.ShowVoiceBtn(true);

           
            }));
            Wait(13f, () => { SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject, "wuse3", false);SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,10); });

            Wait(18f, () => { SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject, "wuse4", false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10); });

            Wait(24f, () => { SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject, "wuse5", false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10); });

            Wait(29f, () => { SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject, "wuse6", false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10); });

            Wait(33f, () => { SpineManager.instance.DoAnimation(Bg.GetChild(0).gameObject, "wuse7", false); SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10); });
            //devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
            //    {
            //        ShowDialogue("", devilText);
            //    }, () =>
            //    {
            //        buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //        {
            //            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
            //            {
            //                ShowDialogue("", bdText);
            //            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //        });
            //    }));
            //});
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
        IEnumerator SpeckerCoroutineTT(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
          
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
                    mask.gameObject.SetActive(false);
                    bd.gameObject.SetActive(false);
                    DTT.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(DTT.gameObject, "daiji", true);
                    TDGameStart();
                    break;
                case 2:
                    //田丁游戏开始方法
                    //TDGameStartFunc();
                    _mask.gameObject.SetActive(false);
                    Bg.GetChild(0).gameObject.SetActive(false); Bg.GetChild(1).gameObject.SetActive(false); Bg.GetChild(2).gameObject.SetActive(false);
                    PlaynotClick(false);//点击事件失效
                    spine.gameObject.SetActive(true);
                    //游戏讲解解说 进入正式游戏界面
                    Wait(6f, () => //等两秒 然后播放第一个选项的语音  需要调节
                    {
                        Debug.Log("23");
                        mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND,3, () =>
                        {
                            canClick = false;
                        }, () =>
                        {
                            canClick = true;
                            i = 1;
                            PlaynotClick(true);//点击事件生效
                        }));
                    });
                    TT.gameObject.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutineTT(TT.gameObject, SoundManager.SoundType.SOUND, 1, null, ()=> 
                    {
                        TT.gameObject.SetActive(false);
                    }));
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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(true); bd.SetActive(false); }));
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
                anyBtns.GetChild(i).gameObject.SetActive(true);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(1).gameObject.SetActive(false);
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); fhBtn();/* GameInit(); */});
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutineTT(dbd, SoundManager.SoundType.SOUND, 7)); });
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
        private void PlaySound() 
        {
        
        }
        private void OnClickAr1(GameObject obj)
        {
            if (canClick)
            {
                SpineManager.instance.DoAnimation(a1.GetChild(0).gameObject, "a2", false);

            }
    
            if (obj.name == "ar1" && i == 4 && canClick)
            {
                PlaynotClick(false);
                Debug.Log("按到了ar1");
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0,false);
                yuPos.DOLocalMove(new Vector3(-197, 116, 0), 2.5f).OnComplete(() =>
                {
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                    soccer.GetChild(0).gameObject.SetActive(false);
                    soccer.GetChild(1).gameObject.SetActive(false);
                    soccer.GetChild(2).gameObject.SetActive(false);
                    soccer.GetChild(3).gameObject.SetActive(false);
                    soccer.GetChild(4).gameObject.SetActive(false);
                    soccer.GetChild(5).gameObject.SetActive(true);
                  
                        Wait(2.5f, () =>
                        {
                            yuPos.DOLocalMove(new Vector3(1053, -590.5f, 0), 0.1f).OnComplete(() =>
                            {
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", false);
                                });
                                yuPos.DOLocalMove(new Vector3(655, -396f, 0), 2f);
                                playSuccessSpine();
                            //mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND,3, () =>
                            //{
                            //    canClick = false;
                            //}, () =>
                            //{
                            //   canClick = true;
                            //    PlaynotClick(true);

                            //}));
                        });
                    });
                });

                isSuccessOnClick = true;
                //计数增加
                //鱼儿消失
                //泼墨特效

                
            }
            else 
            {
                if (canClick) 
                {
                    isSuccessOnClick = false;
                    RandomPlay();
                    Debug.Log("金鱼抖动");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                    });
                }
            
            }
        }
        private void OnClickAr2(GameObject obj)
        {
            if (canClick) 
            {
                SpineManager.instance.DoAnimation(a2.GetChild(0).gameObject, "b2", false);
            }
         
            if (obj.name == "ar2" && i == 3&&canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                PlaynotClick(false);
                Debug.Log("按到了ar2");
                yuPos.DOLocalMove(new Vector3(280, 116, 0), 1.5f).OnComplete(() =>
                {
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);
                    soccer.GetChild(0).gameObject.SetActive(false);
                    soccer.GetChild(1).gameObject.SetActive(false);
                    soccer.GetChild(2).gameObject.SetActive(false);
                    soccer.GetChild(3).gameObject.SetActive(false);
                    soccer.GetChild(4).gameObject.SetActive(true);
                    soccer.GetChild(5).gameObject.SetActive(false);

                    Wait(2.5f, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND, 4, () =>
                        {
                            canClick = false;
                            yuPos.DOLocalMove(new Vector3(1053, -590.5f, 0), 0.1f).OnComplete(() =>
                            {

                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", false);
                                });
                                yuPos.DOLocalMove(new Vector3(655, -396f, 0), 2f);

                            });
                        }, () =>
                        {
                            canClick = true;
                            i = 4;
                            PlaynotClick(true);
                        }));
                    });
                 
                });

                isSuccessOnClick = true;
                //计数增加
                //鱼儿消失
                //泼墨特效
               
            }
            else 
            {
                if (canClick)
                {
                    isSuccessOnClick = false;
                    RandomPlay();
                    Debug.Log("金鱼抖动");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                    });

                }
               
            }
        }
        private void OnClickAr3(GameObject obj)
        {
            if (canClick)
            { 
                SpineManager.instance.DoAnimation(a3.GetChild(0).gameObject, "c2", false);
            }
          
          
            if (obj.name == "ar3" && i == 2 && canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                PlaynotClick(false);
                Debug.Log("按到了ar3");

                yuPos.DOLocalMove(new Vector3(-531, -190, 0), 3f).OnComplete(() =>
                {
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                    soccer.GetChild(0).gameObject.SetActive(false);
                    soccer.GetChild(1).gameObject.SetActive(false);
                    soccer.GetChild(2).gameObject.SetActive(false);
                    soccer.GetChild(3).gameObject.SetActive(true);
                    soccer.GetChild(4).gameObject.SetActive(false);
                    soccer.GetChild(5).gameObject.SetActive(false);

                    Wait(2.5f, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND, 5, () =>
                        {
                            canClick = false;
                            yuPos.DOLocalMove(new Vector3(1053, -590.5f, 0), 0.1f).OnComplete(() =>
                            {
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", false);
                                });
                                yuPos.DOLocalMove(new Vector3(655, -396f, 0), 2f);

                            });
                        }, () =>
                        {
                            canClick = true;
                            i = 3;
                            PlaynotClick(true);
                        }));
                    });
                  
                });

                isSuccessOnClick = true;
                //计数增加
                //鱼儿消失
                //泼墨特效
             
            }
            else 
            {
                if (canClick) 
                {
                    isSuccessOnClick = false;
                    Debug.Log("金鱼抖动");
                    RandomPlay();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                    });
                }
            
            }
        }
        private void OnClickAr4(GameObject obj)
        {
            if (canClick) 
            {
                SpineManager.instance.DoAnimation(a4.GetChild(0).gameObject, "d2", false);
            }
           
           
            if (obj.name == "ar4" && i == 1 && canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                PlaynotClick(false);
                Debug.Log("按到了ar4");

                yuPos.DOLocalMove(new Vector3(34, -190, 0), 1.5f).OnComplete(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                  //  yuPos.gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject,"yu4",false);
                    //鱼透明

                    //myupdate(0.1f, () =>
                    //{
                    //    a -= 50;
                    //    yuPos.GetChild(0).gameObject.GetComponent<RawImage>().color = new Color(1, 1, 1, a / 255);
                    //    if (a<= 0)
                    //    {
                    //        Debug.LogError("1");
                            
                    //        return;
                    //    }
                    //});

                    soccer.GetChild(0).gameObject.SetActive(false);
                    soccer.GetChild(1).gameObject.SetActive(true);
                    soccer.GetChild(2).gameObject.SetActive(false);
                    soccer.GetChild(3).gameObject.SetActive(false);
                    soccer.GetChild(4).gameObject.SetActive(false);
                    soccer.GetChild(5).gameObject.SetActive(false);
                    Wait(2.5f, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND, 6, () =>
                        {
                            yuPos.DOLocalMove(new Vector3(1053, -590.5f, 0), 0.1f).OnComplete(() =>
                            {
                               // yuPos.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", false);
                                });
                                yuPos.DOLocalMove(new Vector3(655, -396f, 0), 2f);

                            });
                            canClick = false;
                        }, () =>
                        {
                            canClick = true;
                            i = 0;
                            PlaynotClick(true);
                        }));
                    });
                   
                });


                isSuccessOnClick = true;
                //计数增加
                //鱼儿消失
                //泼墨特效
              
            }
            else 
            {
                if (canClick) 
                {
                    isSuccessOnClick = false;
                    RandomPlay();
                    Debug.Log("金鱼抖动");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                    });
                }
             
            }
        }
        private void OnClickAr5(GameObject obj)
        {
            if (canClick) 
            {
                SpineManager.instance.DoAnimation(a5.GetChild(0).gameObject, "e2", false);
            }
          
          
            if (obj.name == "ar5" && i ==0 && canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                PlaynotClick(false);
                Debug.Log("按到了ar5");
                yuPos.DOLocalMove(new Vector3(610,-190,0), 0.6f).OnComplete(()=> 
                { //进了漩涡
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu4", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,3);
                    soccer.GetChild(0).gameObject.SetActive(false);
                    soccer.GetChild(1).gameObject.SetActive(false);
                    soccer.GetChild(2).gameObject.SetActive(true);
                    soccer.GetChild(3).gameObject.SetActive(false);
                    soccer.GetChild(4).gameObject.SetActive(false);
                    soccer.GetChild(5).gameObject.SetActive(false);
                    //回到初始位置

                    Wait(2.5f, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND, 2, () =>
                        {
                            canClick = false;
                            yuPos.DOLocalMove(new Vector3(1053, -590.5f, 0), 0.1f).OnComplete(() =>
                            {
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                                //（1053.3 -590.5 0）   （655, -396, 0)
                                SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu3", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", false);
                                });
                                yuPos.DOLocalMove(new Vector3(655, -396f, 0), 2f);

                            });
                        }, () =>
                        {
                            canClick = true;
                            i = 2;
                            PlaynotClick(true);
                        }));
                    });
                  
                });
                isSuccessOnClick = true;
                //计数增加
                //鱼儿消失
                //泼墨特效
               
            }
            else 
            {
               
                Debug.Log("金鱼抖动");

                if (canClick) 
                {
                    RandomPlay();

                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
                    });
                }
               
            }
        }
        private void fhBtn() 
        {
            //TT.gameObject.SetActive(true);
            //mono.StartCoroutine(SpeckerCoroutineTT(TT.gameObject, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
            //{
            //    TT.gameObject.SetActive(false);
            //}));

            spine.gameObject.SetActive(true);
            TT.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(yuPos.GetChild(0).gameObject, "yu", true);
            mono.StartCoroutine(SpeckerCoroutineTT(TT.gameObject, SoundManager.SoundType.SOUND, 1, null, () =>
            {
                TT.gameObject.SetActive(false);
                PlaynotClick(false);//点击事件失效
                
                //游戏讲解解说 进入正式游戏界面
              
            }));
            Wait(6f, () => //等两秒 然后播放第一个选项的语音
            {
                Debug.Log("23");
                mono.StartCoroutine(SpeckerCoroutineTT(bd.gameObject, SoundManager.SoundType.SOUND, 3, () =>
                {
                    canClick = false;
                }, () =>
                {
                    canClick = true;
                    i = 1;
                    PlaynotClick(true);//点击事件生效
                }));
            });

            //UI得分版初始化
            soccer.GetChild(0).gameObject.SetActive(true);
            soccer.GetChild(1).gameObject.SetActive(false);
            soccer.GetChild(2).gameObject.SetActive(false);
            soccer.GetChild(3).gameObject.SetActive(false);
            soccer.GetChild(4).gameObject.SetActive(false);
            soccer.GetChild(5).gameObject.SetActive(false);
            //鱼儿位置初始化
            yuPos.DOLocalMove(new Vector3(655, -396, 0), 1f);
            //游戏数据初始化
       
        }
        private void PlaynotClick(bool canClick) 
        {
            ar5.GetComponent<Empty4Raycast>().raycastTarget = canClick;
            ar4.GetComponent<Empty4Raycast>().raycastTarget = canClick;
            ar3.GetComponent<Empty4Raycast>().raycastTarget = canClick;
            ar2.GetComponent<Empty4Raycast>().raycastTarget = canClick;
            ar1.GetComponent<Empty4Raycast>().raycastTarget = canClick;
        }
        private void UpDate(float delay,Action callBack) 
        {
            mono.StartCoroutine(MyUpdate(delay,callBack));
        }
        private void Wait(float delay, Action callBack)
        {
            mono.StartCoroutine(MyWait(delay,callBack));
        }
        IEnumerator MyUpdate(float delay,Action callBack) 
        {
            while (true) 
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
                if (!isSuccessOnClick) 
                {
                    yield  break;
                }
            }
        }
        IEnumerator MyWait(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        #endregion

        IEnumerator Myxxx(float delay) 
        {
            
            SpineManager.instance.DoAnimation(a1.gameObject,"a1",true);
            yield return new WaitForSeconds(delay);
            SpineManager.instance.DoAnimation(a2.gameObject, "b1", true);
            yield return new WaitForSeconds(delay);
            SpineManager.instance.DoAnimation(a3.gameObject, "c1", true);
            yield return new WaitForSeconds(delay);
            SpineManager.instance.DoAnimation(a4.gameObject, "d1", true);
            yield return new WaitForSeconds(delay);
            SpineManager.instance.DoAnimation(a5.gameObject, "e1", true);
            yield return new WaitForSeconds(delay);

        }
        #endregion

        private void RandomPlay() 
        {
            int i = Random.Range(7,10);
            canClick = false;
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, i, null, ()=> 
            {
                isSuccessOnClick = false;
                canClick = true;
            }));
           
        }
        private void myupdate(float time, Action method_1 = null)
        {
            mono.StartCoroutine(MyUpdate1(time, method_1));

        }
        IEnumerator MyUpdate1(float time, Action method_1 = null)
        {

            while (true)
            {
                yield return new WaitForSeconds(time);
                method_1?.Invoke();
                if (a <= 0)
                {
                    yield break;
                }
            }
        }


    }
}
