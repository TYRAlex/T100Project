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
    

    
    public class TD6732Part5
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

        private GameObject clickBox1;
        private GameObject clickBox2;
        private GameObject dropBox1;
        private GameObject dropBox2;
        private bool[] _jugle;
        private bool[] _jugle2;
        private GameObject show;
        //private GameObject[] _clickBox1;
        //private GameObject[] _clickBox2;
        private Vector2 startPos;
        private GameObject mymask;
        private GameObject boom;
        private GameObject xemboom;
        private int number;
        private GameObject shan;
        private bool _canbf;
        private bool _canok;
        private bool _canfh;
        private bool _cantouch;
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

            clickBox1 = curTrans.Find("clickBox1").gameObject;
            clickBox2 = curTrans.Find("clickBox2").gameObject;
            dropBox1 = curTrans.Find("dropBox1").gameObject;
            dropBox2 = curTrans.Find("dropBox2").gameObject;
            show = curTrans.Find("show").gameObject;
            boom = curTrans.Find("boom").gameObject;
            mymask = curTrans.Find("mymask").gameObject;
            xemboom = curTrans.Find("xemboom").gameObject;
            shan = curTrans.Find("shan").gameObject;

            _jugle = new bool[4];
            _jugle2 = new bool[4];

            for (int i = 0; i < 4; i++)
            {
                clickBox1.transform.GetChild(i+1).GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndDrag, null);
                clickBox2.transform.GetChild(i+1).GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndDrag2, null);
                dropBox1.transform.GetChild(i).GetComponent<mILDroper>().SetDropCallBack(null, null, DropFalse);
                dropBox2.transform.GetChild(i).GetComponent<mILDroper>().SetDropCallBack(null, null, DropFalse); 
            }
            
            GameInit();
            //GameStart();
        }

        private void BeginDragMoveEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            startPos = dragPosition;
            clickBox1.transform.Find(dragIndex.ToString()).SetAsLastSibling();
        }

        private void EndDrag(Vector3 dragPosition, int dragType, int dragIndex, bool isMatch)
        {
            if(isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,6,false);
                mymask.SetActive(true);
                int temp = Random.Range(4, 10);
                mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.COMMONVOICE,temp,null,
                    () => { mymask.SetActive(false); }
                    ));
                clickBox1.transform.Find(dragIndex.ToString()).gameObject.SetActive(false);
                show.transform.Find(dragIndex.ToString()).Find("kb1").gameObject.SetActive(false);

                _jugle[dragIndex] = true;
                Jugle();
            }
            else
            {
                clickBox1.transform.Find(dragIndex.ToString()).localPosition = startPos;
            }
        }

        private void EndDrag2(Vector3 dragPosition, int dragType, int dragIndex, bool isMatch)
        {
            if (isMatch)
            {
                
                _jugle2[dragIndex] = true;
                clickBox2.transform.Find(dragIndex.ToString()).gameObject.SetActive(false);
                show.transform.Find(dragIndex.ToString()).Find("kb2").gameObject.SetActive(false);
                int temp = Random.Range(4,10);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE,temp,false);
                for (int i = 0; i < shan.transform.childCount; i++)
                {
                    SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(dragIndex).gameObject, "star-" + (i + 1).ToString(), false);
                }
                if (dragIndex == 0)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    mymask.SetActive(true);
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("rect").gameObject,"a4",false,
                        ()=>
                        {
                            xemboom.transform.Find(number.ToString()).gameObject.SetActive(false);
                            number++;
                            xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[1];
                            boom.SetActive(true);
                            SpineManager.instance.DoAnimation(boom,"xem-boom",false,
                                () => { boom.SetActive(false); xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[0]; mymask.SetActive(false);
                                    _jugle2[dragIndex] = true; Jugle2();
                                }
                                );
                        }
                        );
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("a").gameObject,"a3",false,
                        ()=>
                        { SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("a").gameObject, "a", true); }
                        );
                    
                }
                if (dragIndex == 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    mymask.SetActive(true);
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("rect").gameObject, "b4", false,
                        ()=>
                        {
                            xemboom.transform.Find(number.ToString()).gameObject.SetActive(false);
                            number++;
                            xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[1];
                            boom.SetActive(true);
                            SpineManager.instance.DoAnimation(boom, "xem-boom", false,
                                () => { boom.SetActive(false);  xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[0]; mymask.SetActive(false);
                                    _jugle2[dragIndex] = true; Jugle2();
                                }
                                );
                        }
                        );
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("b").gameObject, "b3", false,
                        () =>
                        { SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("b").gameObject, "b", true); }
                        );
                }
                if (dragIndex == 2)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    mymask.SetActive(true);
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("rect").gameObject, "c4", false,
                        ()=>
                        {
                            xemboom.transform.Find(number.ToString()).gameObject.SetActive(false);
                            number++;
                            xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[1];
                            boom.SetActive(true);
                            SpineManager.instance.DoAnimation(boom, "xem-boom", false,
                                () => { boom.SetActive(false); xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[0]; mymask.SetActive(false);
                                    _jugle2[dragIndex] = true; Jugle2();
                                }
                                );
                        }
                        );
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("c").gameObject, "c3", false,
                        () =>
                        { SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("c").gameObject, "c", true); }
                        );
                }
                if (dragIndex == 3)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mymask.SetActive(true);
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("rect").gameObject, "d4", false,
                        ()=>
                        {
                            xemboom.transform.Find(number.ToString()).gameObject.SetActive(false);
                            number++;
                            xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[1];
                            boom.SetActive(true);
                            SpineManager.instance.DoAnimation(boom, "xem-boom", false,
                                () => { boom.SetActive(false); xemboom.transform.Find("xem").GetComponent<RawImage>().texture = bellTextures.texture[0]; mymask.SetActive(false);
                                    _jugle2[dragIndex] = true; Jugle2();
                                }
                                );
                        }
                        );
                    SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("d").gameObject, "d3", false,
                        () =>
                        { SpineManager.instance.DoAnimation(show.transform.Find(dragIndex.ToString()).Find("d").gameObject, "d", true); }
                        );
                }
            }
            else
            {
                clickBox2.transform.Find(dragIndex.ToString()).localPosition = startPos;
            }
        }
        private void DropFalse(int dropType)
        {
            //错误音效
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            int temp = Random.Range(0, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE,temp,false);
        }

        private void Jugle()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_jugle[i])
                    return;
            }
            clickBox1.SetActive(false);
            dropBox1.SetActive(false);
            clickBox2.SetActive(true);
            dropBox2.SetActive(true);
        }
        private void Jugle2()
        {
            for (int i = 0; i < 4; i++)
            {
                if (!_jugle2[i])
                    return;
            }
            clickBox2.SetActive(false);
            xemboom.SetActive(false);
            curTrans.Find("guang").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(curTrans.Find("guang").gameObject,"g",true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,5,false);
            SpineManager.instance.DoAnimation(show.transform.Find("0").Find("a").gameObject, "a3", true);
            SpineManager.instance.DoAnimation(show.transform.Find("1").Find("b").gameObject, "b3", true);
            SpineManager.instance.DoAnimation(show.transform.Find("2").Find("c").gameObject, "c3", true);
            SpineManager.instance.DoAnimation(show.transform.Find("3").Find("d").gameObject, "d3", true);
            for (int i = 0; i < shan.transform.childCount; i++)
            {
                SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(0).gameObject, "star-" + (i + 1).ToString(), true);
                SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(1).gameObject, "star-" + (i + 1).ToString(), true);
                SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(2).gameObject, "star-" + (i + 1).ToString(), true);
                SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(3).gameObject, "star-" + (i + 1).ToString(), true);
            }
            mono.StartCoroutine(Wait(3f,
                () => { playSuccessSpine(); }
                ));
           
        }
        #region 初始化和游戏开始方法

        IEnumerator Wait(float time ,Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private void GameInit()
        {
            _canbf = true;
            _canfh = true;
            _canok = true;
            _cantouch = true;
            for (int i = 0; i < 4; i++)
            {
                _jugle[i] = false;
                _jugle2[i] = false;
            }
            for (int i = 0; i < 5; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    shan.transform.GetChild(i).GetChild(j).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(shan.transform.GetChild(i).GetChild(j).gameObject,"kong",false);
                }
            }
            curTrans.Find("guang").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(curTrans.Find("guang").gameObject, "kong", false);
            talkIndex = 1;
            mymask.SetActive(false);
            number = 0;
            boom.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(boom, "kong", false);
            xemboom.SetActive(true);
            xemboom.transform.GetChild(1).GetComponent<RawImage>().texture = bellTextures.texture[0];
            for (int i = 0; i < 6; i++)
            {
                xemboom.transform.GetChild(i).gameObject.SetActive(true);
            }
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < show.transform.GetChild(i).childCount; j++)
                {
                    show.transform.GetChild(i).GetChild(j).gameObject.SetActive(true);
                }
                show.transform.GetChild(i).Find("rect").GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(show.transform.GetChild(i).Find("rect").gameObject, "kong", false);
                clickBox1.transform.Find(i.ToString()).localPosition = curTrans.Find("clickBox1Pos").Find(i.ToString()).localPosition;
                clickBox2.transform.Find(i.ToString()).localPosition = curTrans.Find("clickBox2Pos").Find(i.ToString()).localPosition;
            }
            show.transform.GetChild(0).Find("a").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            show.transform.GetChild(1).Find("b").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            show.transform.GetChild(2).Find("c").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            show.transform.GetChild(3).Find("d").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(show.transform.GetChild(0).Find("a").gameObject, "a5", false);
            SpineManager.instance.DoAnimation(show.transform.GetChild(1).Find("b").gameObject, "b5", false);
            SpineManager.instance.DoAnimation(show.transform.GetChild(2).Find("c").gameObject, "c5", false);
            SpineManager.instance.DoAnimation(show.transform.GetChild(3).Find("d").gameObject, "d5", false);
            clickBox1.SetActive(true);
            clickBox2.SetActive(false);
            dropBox1.SetActive(true);
            dropBox2.SetActive(false);
            for (int i = 0; i < 5; i++)
            {
                clickBox1.transform.GetChild(i).gameObject.SetActive(true);
                clickBox2.transform.GetChild(i).gameObject.SetActive(true);
            }
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
            _cantouch = true;
            devil.SetActive(true);
            buDing.SetActive(true);
            devil.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            buDing.transform.GetChild(0).GetComponent<Text>().text = string.Empty;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, () =>
                {
                    ShowDialogue("哼，我是不会让你们顺利回去杰洛特那里", devilText);
                }, () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            ShowDialogue("小恶魔来了，小朋友们快击退他吧", bdText);
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
            ////加载点击滑动图片
            //TDLoadPageBar();
            ////加载材料环节
            //LoadSpineShow();

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
            SpineShow.gameObject.SetActive(true);
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
            if (!_cantouch)
                return;
            _cantouch = false;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf"&&_canbf)
                {
                    
                    _canbf = false;
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh"&&_canfh)
                {
                    _canfh = false;
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else if(_canok)
                {
                    _canok = false;
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3)); });
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
                yield return new WaitForSeconds(0.1f);
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

        

        

    }
}
