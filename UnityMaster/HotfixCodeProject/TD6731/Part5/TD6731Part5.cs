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
    public class TD6731Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform obj;
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

        private Transform Level1;
        private Transform Level2;
        private Transform Level3;
        private Transform Level4;
        private Transform Level5;
        private Transform cakeFire;
        private Transform Music;
        private GameObject gift;
        private bool IsArriveInHightPoint;
        private bool isPlay;
        private Transform effect;
        private Transform xem;
        private Transform lion;
        private Transform zg;

        private Transform pos;
        private bool isLevel1;
        private bool isLevel2;
        private bool isLevel3;
        private bool isLevel4;
        private bool isLevel5;
        private bool isBoolSuccess;
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

        private bool isplaying;

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

            obj = curTrans.Find("Level1/1");
            gift = curTrans.Find("gift").gameObject;

            Bg = curTrans.Find("Bg").gameObject;
            Level1 = curTrans.Find("Level1"); Level2 = curTrans.Find("Level2");
            Level3= curTrans.Find("Level3"); Level4 = curTrans.Find("Level4");
            Level5 = curTrans.Find("Level5"); 
            Music = curTrans.Find("music");
            cakeFire = curTrans.Find("cakefire");
            effect = curTrans.Find("effect");
            xem = curTrans.Find("xem");
            zg = curTrans.Find("zg");
            lion = curTrans.Find("lion");
            pos = curTrans.Find("Pos");
            isLevel1 = false;
            isLevel2 = false;
            isLevel3 = false; 
            isLevel4 = false; 
            isLevel5 = false;
            isBoolSuccess=false;
            IsArriveInHightPoint = true;
            isPlay = false;
            isplaying = false;
            bellTextures = Bg.GetComponent<BellSprites>();

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
            buDing.SetActive(true);
            devil.SetActive(true);
            devilText.text = "";
            bdText.text = "";
            Input.multiTouchEnabled = false;
            //田丁初始化
            buDing.SetActive(true);devil.SetActive(true);
            CW();
            TDGameInit();
            CheckIsHasButton();
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
                mono.StartCoroutine(SpeckerCoroutineDD(bd, SoundManager.SoundType.SOUND, 0, () =>
                {
                    ShowDialogue("哈哈哈哈,本大王来啦!", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutineDD(bd, SoundManager.SoundType.SOUND, 1, () =>
                        {
                            ShowDialogue("小朋友们,小恶魔又来了,快和我们一起打跑它吧!", bdText);
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
                    Waiting(8f, () =>
                    {
                        SpineManager.instance.DoAnimation(gift, "h2", false, () =>
                        {
                            SpineManager.instance.DoAnimation(cakeFire.GetChild(0).gameObject, "lz1", true);
                            SpineManager.instance.DoAnimation(gift, "h3", false,()=> 
                            {
                                Level1_Up();
                                SpineManager.instance.DoAnimation(gift, "h4", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(gift,"h3",true);
                                });
                             
                                mono.StartCoroutine(CheckActived());
                            });

                          
                         

                        });
                    });
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
            mono.StartCoroutine(SpeckerCoroutineDD(bd, SoundManager.SoundType.SOUND, 2, null, () => { mask.SetActive(false); bd.SetActive(false); }));
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
            //if (isPlaying)
            //    return;
            //isPlaying = true;
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); CW(); first(); buDing.SetActive(false);devil.SetActive(false); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutineDD(dbd, SoundManager.SoundType.SOUND, 3)); });
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
            str = str.Replace(" ", "\u00A0");  //空格 换行
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
            buDing.SetActive(false);
            devil.SetActive(false);
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

        #endregion


        #endregion

        IEnumerator Move(GameObject obj)
        {

            float y = 0;
            float x = 0;
            while (true)
            {
                if (y < 900) { x += 3; } else { x += 0.01f; }

                yield return new WaitForSeconds(0.01f);
                // vector = new Vector3(x, y,0);
                y = -0.009f * x * x + 4.32f * x;
                //   vector2 = new Vector3(x, y,0);
                // Debug.DrawLine(curTrans.Find("GameObject").TransformPoint(vector), curTrans.Find("GameObject").TransformPoint(vector2), Color.yellow);
                obj.transform.position = new Vector2(x, y);

            }
        }

        private void Level1_Up()
        {
            Level1.gameObject.SetActive(true);
            // Move(obj.gameObject);
            mono.StartCoroutine(Move2(Level1.GetChild(0).gameObject, new Vector2(300, 800)));
            mono.StartCoroutine(Move2(Level1.GetChild(1).gameObject, new Vector2(250, 700)));
            mono.StartCoroutine(Move2(Level1.GetChild(2).gameObject, new Vector2(-250, 700)));
            mono.StartCoroutine(Move2(Level1.GetChild(3).gameObject, new Vector2(-250, 800)));
            mono.StartCoroutine(Move2(Level1.GetChild(4).gameObject, new Vector2(190, 650)));
        }
        private void Level2_Up()
        {
            Level2.gameObject.SetActive(true);
            // Move(obj.gameObject);
            mono.StartCoroutine(Move2(Level2.GetChild(0).gameObject, new Vector2(300, 800)));
            mono.StartCoroutine(Move2(Level2.GetChild(1).gameObject, new Vector2(250, 700)));
            mono.StartCoroutine(Move2(Level2.GetChild(2).gameObject, new Vector2(-250, 700)));
            mono.StartCoroutine(Move2(Level2.GetChild(3).gameObject, new Vector2(-250, 800)));
            mono.StartCoroutine(Move2(Level2.GetChild(4).gameObject, new Vector2(190, 650)));
            IsArriveInHightPoint = true;
        }
        private void Level3_Up()
        {
            Level3.gameObject.SetActive(true);
            // Move(obj.gameObject);
            mono.StartCoroutine(Move2(Level3.GetChild(0).gameObject, new Vector2(-300, 800)));
            mono.StartCoroutine(Move2(Level3.GetChild(1).gameObject, new Vector2(-250, 700)));
            mono.StartCoroutine(Move2(Level3.GetChild(2).gameObject, new Vector2(250, 700)));
            mono.StartCoroutine(Move2(Level3.GetChild(3).gameObject, new Vector2(250, 800)));
    
            IsArriveInHightPoint = true;
        }
        private void Level4_Up()
        {
            Level4.gameObject.SetActive(true);
            // Move(obj.gameObject);
            mono.StartCoroutine(Move2(Level4.GetChild(0).gameObject, new Vector2(300, 800)));
            mono.StartCoroutine(Move2(Level4.GetChild(1).gameObject, new Vector2(250, 700)));
            mono.StartCoroutine(Move2(Level4.GetChild(2).gameObject, new Vector2(-250, 700)));
            mono.StartCoroutine(Move2(Level4.GetChild(3).gameObject, new Vector2(-250, 800)));
            mono.StartCoroutine(Move2(Level4.GetChild(4).gameObject, new Vector2(190, 650)));
            IsArriveInHightPoint = true;
        }
        private void Level5_Up()
        {
            Level5.gameObject.SetActive(true);
            // Move(obj.gameObject);
            mono.StartCoroutine(Move2(Level5.GetChild(0).gameObject, new Vector2(-300, 800)));
            mono.StartCoroutine(Move2(Level5.GetChild(1).gameObject, new Vector2(-250, 700)));
            mono.StartCoroutine(Move2(Level5.GetChild(2).gameObject, new Vector2(250, 700)));
            mono.StartCoroutine(Move2(Level5.GetChild(3).gameObject, new Vector2(250, 800)));

            IsArriveInHightPoint = true;
        }
        IEnumerator Move2(GameObject obj, Vector2 vector)
        {
            //参数的vector是抛物线顶点坐标
            float a = -(vector.y / (vector.x * vector.x));
            float b = -2 * a * vector.x;
            float x = 0;
            float y = 0;
            while (true)
            {
                //IsArriveInHightPoint是否抵达最高点
                if (IsArriveInHightPoint) { x += (vector.y / vector.x); } else { x += 0.4f * (vector.y / vector.x); }

                if (vector.y - obj.transform.localPosition.y < 0.1f)
                {
                    IsArriveInHightPoint = false;
                }


                yield return new WaitForSeconds(0.01f);
                y = a * x * x + b * x;
                obj.transform.localPosition = new Vector2(x, y);

                if (vector.y - obj.transform.localPosition.y < 0.1f)
                {
                    Debug.Log("达到最高点" + obj.name);
                    Util.AddBtnClick(obj, ClickEvent);
                    
                }

                if (obj.transform.position.y < -10)
                {
                    obj.gameObject.SetActive(false);
                    yield break;
                }
            }
        }
        IEnumerator CheckActived() 
        {
            while (true)
            {
                
                yield return new WaitForSeconds(0.1f);
              
                    ChecKActive();
                if (isBoolSuccess == true) 
                {
                    yield break;
                }
               
            }
        }
        IEnumerator Wait(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
        private void Waiting(float time, Action method = null)
        {
            mono.StartCoroutine(Wait(time, method));
        }
        private void ClickEvent(GameObject obj)
        {

            switch (obj.name)
            {
                case "1":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                   // Level1.GetChild(0).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(0).gameObject, "yinfu-c1", false, () =>
                       {
                           SpineManager.instance.DoAnimation(Music.GetChild(0).gameObject, "yinfu1", true);
                       });
                    break;
                case "2":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    //  Level1.GetChild(1).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(1).gameObject, "yinfu-c2", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(1).gameObject, "yinfu2", true);
                    });
                    break;
                case "3":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                    //  Level1.GetChild(2).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(2).gameObject, "yinfu-c3", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(2).gameObject, "yinfu3", true);
                    });
                    break;
                case "4":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                    // Level2.GetChild(0).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(3).gameObject, "yinfu-c4", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(3).gameObject, "yinfu4", true);
                    });
                    break;
                case "5":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                    // Level2.GetChild(1).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(4).gameObject, "yinfu-c5", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(4).gameObject, "yinfu5", true);
                    });
                    break;
                case "6":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                    //  Level2.GetChild(2).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(5).gameObject, "yinfu-c6", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(5).gameObject, "yinfu6", true);
                    });
                    break;
                case "7":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,6);
                    // Level3.GetChild(0).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(6).gameObject, "yinfu-c7", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(6).gameObject, "yinfu7", true);
                    });
                    break;
                case "8":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,7 );
                    // Level3.GetChild(1).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(7).gameObject, "yinfu-c8", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(7).gameObject, "yinfu8", true);
                    });
                    break;
                case "9":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8);
                    // Level4.GetChild(0).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(8).gameObject, "yinfu-c9", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(8).gameObject, "yinfu9", true);
                    });
                    break;
                case "10":
                    obj.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9);
                    // Level4.GetChild(1).RemoveComponent<Empty4Raycast>();
                    SpineManager.instance.DoAnimation(Music.GetChild(9).gameObject, "yinfu-c10", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(9).gameObject, "yinfu10", true);
                    });
                    break;
                case "11":
                    obj.SetActive(false);
                    //  Level5.GetChild(0).RemoveComponent<Empty4Raycast>();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10);
                    SpineManager.instance.DoAnimation(Music.GetChild(10).gameObject, "yinfu-c11", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(10).gameObject, "yinfu11", true);
                    });
                    break;
                case "12":
                    obj.SetActive(false);
                    //   Level5.GetChild(1).RemoveComponent<Empty4Raycast>();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                    SpineManager.instance.DoAnimation(Music.GetChild(11).gameObject, "yinfu-c12", false, () =>
                    {
                        SpineManager.instance.DoAnimation(Music.GetChild(11).gameObject, "yinfu12", true);
                    });
                    break;
                case "boom":
                    //  Level1.GetChild(3).RemoveComponent<Empty4Raycast>();
                    obj.GetComponent<Empty4Raycast>().raycastTarget = false;
                    
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16);
                    SpineManager.instance.DoAnimation(Level1.GetChild(3).GetChild(0).gameObject,"z-boom",false,()=> { Level1.gameObject.SetActive(false); obj.SetActive(false); });
                  //  Level1.GetChild(3).gameObject.SetActive(false);

                    break;
                case "boom1":
                    obj.GetComponent<Empty4Raycast>().raycastTarget = false;
                    //   Level3.GetChild(2).RemoveComponent<Empty4Raycast>();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16);
                    SpineManager.instance.DoAnimation(Level3.GetChild(2).GetChild(0).gameObject, "z-boom", false, () => { Level3.gameObject.SetActive(false); obj.SetActive(false); });
                 //   Level3.GetChild(2).RemoveComponent<Empty4Raycast>();

                    break;
                case "black":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 11);
                    Level1.GetChild(4).gameObject.SetActive(false);
                 //   Level1.GetChild(4).RemoveComponent<Empty4Raycast>();
                    break;
                case "black1":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 12);
                    Level2.GetChild(3).gameObject.SetActive(false);
                  //  Level2.GetChild(3).RemoveComponent<Empty4Raycast>();
               
                    break;
                case "black2":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13);
                    Level2.GetChild(4).gameObject.SetActive(false);
                 //   Level2.GetChild(4).RemoveComponent<Empty4Raycast>();
                    break;
                case "black3":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14);
                    Level3.GetChild(3).gameObject.SetActive(false);
                  //  Level3.GetChild(3).RemoveComponent<Empty4Raycast>();
                    break;
                case "black4":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15);
                    Level4.GetChild(2).gameObject.SetActive(false);
                 //   Level4.GetChild(2).RemoveComponent<Empty4Raycast>();
                    break;
                case "black5":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 11);
                    Level4.GetChild(3).gameObject.SetActive(false);
                  //  Level4.GetChild(3).RemoveComponent<Empty4Raycast>();
                    break;
                case "black6":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 12);
                    Level4.GetChild(4).gameObject.SetActive(false);
                  //  Level4.GetChild(4).RemoveComponent<Empty4Raycast>();
                    break;
                case "black7":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13);
                    Level5.GetChild(2).gameObject.SetActive(false);
                //    Level5.GetChild(2).RemoveComponent<Empty4Raycast>();
                    break;
                case "black8":
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14);
                    Level5.GetChild(3).gameObject.SetActive(false);
                 //   Level5.GetChild(3).RemoveComponent<Empty4Raycast>();
                    break;

            }


        }
        
        private void ChecKActive()
        {
            if (Level1.GetChild(0).gameObject.activeInHierarchy == false && Level1.GetChild(1).gameObject.activeInHierarchy == false && Level1.GetChild(2).gameObject.activeInHierarchy == false && Level1.GetChild(3).gameObject.activeInHierarchy == false && Level1.GetChild(4).gameObject.activeInHierarchy == false&&isLevel2==false)
            {
                isLevel2 = true;
              
            
                Waiting(3f, () =>
                {
                    SpineManager.instance.DoAnimation(cakeFire.GetChild(1).gameObject, "lz2", true);
                    SpineManager.instance.DoAnimation(gift, "h3", false,()=> 
                        {
                            Level2_Up();
                            SpineManager.instance.DoAnimation(gift, "h4", false, () => 
                            {
                                SpineManager.instance.DoAnimation(gift,"h3",true);
                            });
                        
                        });
                        
                });
            }
            if (Level2.GetChild(0).gameObject.activeInHierarchy == false && Level2.GetChild(1).gameObject.activeInHierarchy == false && Level2.GetChild(2).gameObject.activeInHierarchy == false && Level2.GetChild(3).gameObject.activeInHierarchy == false && isLevel3== false) 
            {
                isLevel3 = true;
          
                Waiting(3f, () =>
                {
                    SpineManager.instance.DoAnimation(cakeFire.GetChild(2).gameObject, "lz3", true);
                    SpineManager.instance.DoAnimation(gift, "h3", false, () =>
                    {
                        Level3_Up();
                        SpineManager.instance.DoAnimation(gift, "h4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(gift, "h3", true);
                        });

                    });

                });
            }
            if (Level3.GetChild(0).gameObject.activeInHierarchy == false && Level3.GetChild(1).gameObject.activeInHierarchy == false && Level3.GetChild(2).gameObject.activeInHierarchy == false && Level3.GetChild(3).gameObject.activeInHierarchy == false && isLevel4 == false)
            {
                isLevel4 = true;

                Waiting(3f, () =>
                {
                    SpineManager.instance.DoAnimation(cakeFire.GetChild(3).gameObject, "lz4", true);
                    SpineManager.instance.DoAnimation(gift, "h3", false, () =>
                    {
                        Level4_Up();
                        SpineManager.instance.DoAnimation(gift, "h4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(gift, "h3", true);
                        });

                    });

                });
            }
            if (Level4.GetChild(0).gameObject.activeInHierarchy == false && Level4.GetChild(1).gameObject.activeInHierarchy == false && Level4.GetChild(2).gameObject.activeInHierarchy == false && Level4.GetChild(3).gameObject.activeInHierarchy == false && Level4.GetChild(4).gameObject.activeInHierarchy == false && isLevel5 == false)
            {
                isLevel5 = true;

                Waiting(3f, () =>
                {
                    SpineManager.instance.DoAnimation(cakeFire.GetChild(4).gameObject, "lz5",true);
                    SpineManager.instance.DoAnimation(gift, "h3", false, () =>
                    {
                        Level5_Up();
                        SpineManager.instance.DoAnimation(gift, "h4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(gift, "h3", true);
                        });

                    });

                });
            }
            if (Level5.GetChild(0).gameObject.activeInHierarchy == false && Level5.GetChild(1).gameObject.activeInHierarchy == false && Level5.GetChild(2).gameObject.activeInHierarchy == false && Level5.GetChild(3).gameObject.activeInHierarchy == false && isBoolSuccess == false) 
            {
                isBoolSuccess = true;
                Waiting(0.01f,()=> 
                {
                 
                    SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem2", true);
                    xem.transform.GetRectTransform().DOAnchorPosY(550,0.5f).OnComplete(()=>
                    {
                        Music.gameObject.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,17,false);
                        SpineManager.instance.DoAnimation(effect.gameObject, "xem-yinfu", false, () =>
                        {
                            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem3", false, () =>
                            {
                                SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "xem4", true);
                            });
                            SpineManager.instance.DoAnimation(zg.gameObject, "dw", true);
                            SpineManager.instance.DoAnimation(lion.gameObject, "shizi2", true);
                            gift.SetActive(false);
                            Waiting(2f, () => { playSuccessSpine(); });
                        });
                    });
                   


              
                });
            
            }
        }
        private void CW() 
        {
            CWSetAtive();
            isLevel1 = false;
            isLevel2 = false;
            isLevel3 = false;
            isLevel4 = false;
            isLevel5 = false;
            isBoolSuccess = false;
            IsArriveInHightPoint = true;

         

        }
        private void CWSetAtive() 
        {
            Level1.gameObject.SetActive(true); Level2.gameObject.SetActive(true); Level3.gameObject.SetActive(true); Level4.gameObject.SetActive(true); Level5.gameObject.SetActive(true);
            Music.gameObject.SetActive(true);gift.SetActive(true);

          
            for (int i = 0; i < Music.childCount; i++) { SpineManager.instance.DoAnimation(Music.GetChild(i).gameObject, "kong", false); }

            for (int i = 0; i < Level1.childCount; i++) { Level1.GetChild(i).gameObject.SetActive(true); Level1.GetChild(i).transform.localPosition = pos.localPosition; }
            for (int i = 0; i < Level2.childCount; i++) { Level2.GetChild(i).gameObject.SetActive(true); Level2.GetChild(i).transform.localPosition = pos.localPosition; }
            for (int i = 0; i < Level3.childCount; i++) { Level3.GetChild(i).gameObject.SetActive(true); Level3.GetChild(i).transform.localPosition = pos.localPosition; }
            for (int i = 0; i < Level4.childCount; i++) { Level4.GetChild(i).gameObject.SetActive(true); Level4.GetChild(i).transform.localPosition = pos.localPosition; }
            for (int i = 0; i < Level5.childCount; i++) { Level5.GetChild(i).gameObject.SetActive(true); Level5.GetChild(i).transform.localPosition = pos.localPosition; }


            //for (int i = 0; i < Level1.childCount-1; i++) {  Level1.GetChild(i).gameObject.AddComponent<Empty4Raycast>(); }
            //for (int i = 0; i < Level2.childCount-2; i++) {  Level2.GetChild(i).gameObject.AddComponent<Empty4Raycast>(); }
            //for (int i = 0; i < Level3.childCount-1; i++) { Level3.GetChild(i).gameObject.AddComponent<Empty4Raycast>(); }
            //for (int i = 0; i < Level4.childCount-3; i++) { Level4.GetChild(i).gameObject.AddComponent<Empty4Raycast>(); }
            //for (int i = 0; i < Level5.childCount-2; i++) { Level5.GetChild(i).gameObject.AddComponent<Empty4Raycast>(); }

            //Level1.GetChild(4).gameObject.GetComponent<RawImage>().raycastTarget = true;
            //Level2.GetChild(3).gameObject.GetComponent<RawImage>().raycastTarget = true; Level2.GetChild(4).gameObject.GetComponent<RawImage>().raycastTarget = true;
            //Level3.GetChild(3).gameObject.GetComponent<RawImage>().raycastTarget = true;
            //Level4.GetChild(2).gameObject.GetComponent<RawImage>().raycastTarget = true; Level4.GetChild(3).gameObject.GetComponent<RawImage>().raycastTarget = true; Level4.GetChild(4).gameObject.GetComponent<RawImage>().raycastTarget = true;
            //Level5.GetChild(2).gameObject.GetComponent<RawImage>().raycastTarget = true; Level5.GetChild(3).gameObject.GetComponent<RawImage>().raycastTarget = true;
            //for (int i = 0; i < Level2.childCount-2; i++) {  Level2.GetChild(i).gameObject.GetComponent<RawImage>().raycastTarget = true; }
            //for (int i = 0; i < Level3.childCount-1; i++) {  Level3.GetChild(i).gameObject.GetComponent<RawImage>().raycastTarget = true; }
            //for (int i = 0; i < Level4.childCount-3; i++) {  Level4.GetChild(i).gameObject.GetComponent<RawImage>().raycastTarget = true; }
            //for (int i = 0; i < Level5.childCount-2; i++) {  Level5.GetChild(i).gameObject.GetComponent<RawImage>().raycastTarget = true; }

            Level1.transform.GetChild(3).GetComponent<Empty4Raycast>().raycastTarget = true;
            Level3.transform.GetChild(2).GetComponent<Empty4Raycast>().raycastTarget = true;
            SpineManager.instance.DoAnimation(Level1.transform.GetChild(3).GetChild(0).gameObject,"z",true);
            SpineManager.instance.DoAnimation(Level3.transform.GetChild(2).GetChild(0).gameObject, "z", true);
            SpineManager.instance.DoAnimation(lion.gameObject, "shizi1", true);
            for (int i = 0; i < cakeFire.childCount; i++) { SpineManager.instance.DoAnimation(cakeFire.GetChild(i).gameObject,"kong",false); }

            SpineManager.instance.DoAnimation(gift,"h",true);
            SpineManager.instance.DoAnimation(effect.gameObject, "kong", true);
            xem.GetRectTransform().DOAnchorPosY(1256, 0.1f);
            SpineManager.instance.DoAnimation(xem.GetChild(0).gameObject, "kong", true);
            
            SpineManager.instance.DoAnimation(zg.gameObject, "kong", true);
           
            
        }

        private void RemoveRaycast() 
        {
            for (int i = 0; i < Level1.childCount; i++) 
            {
                Level1.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        private void RemoveRaycast2()
        {
            for (int i = 0; i < Level2.childCount; i++)
            {
                Level2.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        private void RemoveRaycast3() {
            for (int i = 0; i < Level3.childCount; i++)
            {
                Level3.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        private void RemoveRaycast4()
        {
            for (int i = 0; i < Level4.childCount; i++)
            {
                Level4.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        private void RemoveRaycast5()
        {
            for (int i = 0; i < Level5.childCount; i++)
            {
                Level5.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
            }
        }
        private void first() 
        {
            Waiting(2f, () =>
            {
                SpineManager.instance.DoAnimation(gift, "h2", false, () =>
                {
                    SpineManager.instance.DoAnimation(cakeFire.GetChild(0).gameObject, "lz1", true);
                    SpineManager.instance.DoAnimation(gift, "h3", false, () =>
                    {
                        Level1_Up();
                        SpineManager.instance.DoAnimation(gift, "h4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(gift, "h3", true);
                        });

                        mono.StartCoroutine(CheckActived());
                    });




                });
            });
        }
        private void CheckIsHasButton() 
        {
            if (Level1.GetChild(0).GetComponent<Button>())
            {
                RemoveRaycast();
            }

            if (Level2.GetChild(0).GetComponent<Button>())
            {
                RemoveRaycast2();
            }
            if (Level3.GetChild(0).GetComponent<Button>())
            {
                RemoveRaycast3();
            }
            if (Level4.GetChild(0).GetComponent<Button>())
            {
                RemoveRaycast4();
            }
            if (Level5.GetChild(0).GetComponent<Button>())
            {
                RemoveRaycast5();
            }
        }
    }
}
