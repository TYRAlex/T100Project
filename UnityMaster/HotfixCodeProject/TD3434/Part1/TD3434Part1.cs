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
    public class TD3434Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform Part1;
        private Transform Part2;

        private Transform _return;
        private Transform Mask1;

        private bool _canClick;
        private int  index = 0;
        private int index2 = 0;
        private List<string> name1;
        private List<string> name2;

        private Transform p2_Level1;
        private Transform p2_Level2;
        private Transform p2_Level3;
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

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);


            Part1 = curTrans.Find("part1");
            Part2 = curTrans.Find("part2");
            _canClick = false;
            name1 = new List<string>();
            name2 = new List<string>();
            _return = curTrans.Find("_return");
            Mask1 = curTrans.Find("Mask1");
            p2_Level1 = curTrans.Find("part2/Level1");
            p2_Level2 = curTrans.Find("part2/Level2");
            p2_Level3 = curTrans.Find("part2/Level3");
            AddDragClickEvent();
            InitLayers();
            GameInit();
            //GameStart();
        }

        void AddDragClickEvent()
        {
            for (int i = 0; i < Part1.GetChild(1).childCount; i++)
            {
              
                Util.AddBtnClick(Part1.GetChild(1).GetChild(i).gameObject, ClickEventLevel1);
            }
            for (int i = 0; i < Part2.childCount; i++)
            {
                
                Util.AddBtnClick(Part2.GetChild(i).GetChild(1).gameObject, ClickEventLevel2);
            }

        }
        private void ClickEventLevel1(GameObject obj)
        {

            if (_canClick)
            {
                SoundManager.instance.ShowVoiceBtn(false);
                BtnPlaySound();
                //  SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 1, false);
                _canClick = false;
                JudgeClick(obj);
                string name = obj.name;
                SpineManager.instance.DoAnimation(Part1.GetChild(0).gameObject, obj.name, false);
            }
        }
        private void InitLayers() 
        {
            p2_Level1.SetAsFirstSibling();
            p2_Level2.SetSiblingIndex(1);
            p2_Level3.SetAsLastSibling();
        }
        private void ClickEventLevel2(GameObject obj)
        {
            ;
            if (_canClick)
            {
                obj.transform.parent.SetAsLastSibling();
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                if (obj.name == "a")
                {

                    MovePartOpenandClose(obj);
                }
                else if (obj.name == "b")
                {

                    MovePartOpenandClose(obj);
                }
                else if (obj.name == "c")
                {

                    MovePartOpenandClose(obj);
                }
            }
        }
        void JudgeClick(GameObject obj)
        {
            Debug.Log(obj.name);
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }


            if (obj.name.Equals("1"))
            {

                Debug.Log(name1.Contains(obj.name));
                Debug.Log(index);
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 2, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                
                    _canClick = true;
                  
                }));
            }
            if (obj.name.Equals("2"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 3, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                 
                    _canClick = true;
                  
                }));
            }
            if (obj.name.Equals("3"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 4, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                 
                    _canClick = true;
                   
                }));
            }
            if (obj.name.Equals("4"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND,5, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                  
                    _canClick = true;
                  
                }));
            }
            if (obj.name.Equals("5"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 6, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
               
                    _canClick = true;
                  
                }));
            }
            if (obj.name.Equals("6"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 7, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                  
                    _canClick = true;
                   
                }));
            }
            if (obj.name.Equals("7"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 8, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
            if (obj.name.Equals("8"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 9, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
            if (obj.name.Equals("9"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 10, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
            if (obj.name.Equals("10"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 11, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
            if (obj.name.Equals("11"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 12, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
            if (obj.name.Equals("12"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 13, null, () =>
                {
                    if (index == 12)
                    { SoundManager.instance.ShowVoiceBtn(true); }

                    _canClick = true;

                }));
            }
        }
        void JudgeClick2(GameObject obj)
        {
            if (name2.Contains(obj.name) == false)
            {
                name2.Add(obj.name);
                index2++;
            }
            if (obj.name.Equals("a"))
            {

                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 16, null, () =>
                {
                    Mask1.gameObject.Show();
                   
                    _canClick = true;
                   
                }));
            }
            if (obj.name.Equals("b"))
            {

                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 17, null, () =>
                {

                    Mask1.gameObject.Show();
                
                    _canClick = true;
                    
                }));
            }
            if (obj.name.Equals("c"))
            {

                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 18, null, () =>
                {
                    Mask1.gameObject.Show();
               
                    _canClick = true;
                  
                }));
            }
       
        }
        void MovePartOpenandClose(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);

            _return.gameObject.SetActive(true);
            //播放open
            SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(0).gameObject, obj.name, false, () =>
            {
                JudgeClick2(obj);
                Util.AddBtnClick(Mask1.gameObject, (g) =>
                {
                    Mask1.gameObject.Hide();

                    //   SoundManager.instance.ShowVoiceBtn(true);//播放close
                    SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(0).gameObject, obj.name + "2", false, () =>
                    {

                        _return.gameObject.Hide();

                        if (index2 == 3)
                        {
                            SoundManager.instance.ShowVoiceBtn(true);

                        }
                    });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);



                });
            });
        }


        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            Input.multiTouchEnabled = false;
            _return.gameObject.SetActive(false);
            Mask1.gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            // _Mask.GetComponent<Image>().raycastTarget = false;
            Part1.transform.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            Part2.transform.GetRectTransform().anchoredPosition = new Vector2(1920, 0);
            
            SpineManager.instance.DoAnimation(Part1.GetChild(0).gameObject, "0", false);
            SpineManager.instance.DoAnimation(Part2.GetChild(0).GetChild(0).gameObject,"a3",false);
            SpineManager.instance.DoAnimation(Part2.GetChild(1).GetChild(0).gameObject, "b3", false);
            SpineManager.instance.DoAnimation(Part2.GetChild(2).GetChild(0).gameObject, "c3", false);
            //我们比比谁最棒
            mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND,0,null,()=> 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
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

            LRBtnUpdate();
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
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
                    //   TDGameStartFunc();
                    //小朋友们来认识一下材料吧
                    mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 1, null, () => 
                    {
                        bd.gameObject.SetActive(false);
                        _canClick = true;
                    }));
                    break;
                case 2:
                    //移动到下一环节
                    MovePart1();
                    break;
                case 3:
                    //开启遮罩
                    _return.gameObject.SetActive(true);
                    bd.gameObject.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 15, null,()=> 
                    {

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
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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
              mask.SetActive(false);
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
            
            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
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

        void MovePart1()
        {
            _canClick = false;
            //  _Mask.GetComponent<Image>().raycastTarget = false;
            bd.gameObject.SetActive(false);

            // LRBtnUpdate();
            Part1.GetRectTransform().DOAnchorPosX(-1920, 1f);
            Part2.GetRectTransform().DOAnchorPosX(0, 1f).OnComplete(() =>
            {
                bd.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(bd.gameObject, "bd-daiji", true);
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 14, () =>
                {
                    SpineManager.instance.DoAnimation(bd.gameObject, "bd-speak", true);
                    _return.gameObject.SetActive(true);
               
                }, () =>
                {
                    bd.gameObject.SetActive(false);
                  
                    _return.gameObject.SetActive(false);
                    _canClick = true;
                }));
            });
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
            if (isPlaying)
                return;
            isPlaying = true;
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.COMMONVOICE, 0)); });
                }

            });
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion
       

        #endregion

        

        

    }
}
