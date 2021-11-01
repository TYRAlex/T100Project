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
        next,
    }
    public class TD91231Part5
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
        private bool isPlaying;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion

        #endregion

        #endregion

        private GameObject _human;
        private GameObject _snake;
        private GameObject _xem;
        private GameObject _wxp;
        private Transform _quan;
        private Transform _quanPos;
        private Vector2 _quanRePos;

        private Vector2[] _ranPos;      //随机位置
        private GameObject[] _block;    //两个音符框
        private GameObject[] _yfObj;    //飘动的音符本体
        private GameObject[] _yfAni;    //飘动的音符动画
        private mILDrager[] _yfDrag;    //拖动组件
        private GameObject[] _blockYF;  //音符框里的音符
        private Vector2 _yfStartPos;

        private Transform _dropEnd;

        private Tween[] _moveTween;
        private Tween _tw1;
        private Tween _tw2;
        private Tween _tw3;
        private Tween _tw4;
        private Tween _tw5;
        private Tween _tw6;
        private Tween _tw7;

        int _level;
        int _count;
        int[] ranArray;
        bool _isEnd = false;
        bool _canStopAni;

        string _dragingAnisStr;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            _human = curTrans.GetGameObject("human");
            _wxp = curTrans.GetGameObject("wxp");
            _snake = curTrans.GetGameObject("snake");
            _xem = curTrans.GetGameObject("xem");
            _quan = curTrans.Find("Quan");
            _quanPos = curTrans.Find("QuanPos");
            _quanRePos = curTrans.Find("QuanRePos").position;

            _dropEnd = curTrans.Find("Block/DropEnd");
            _ranPos = new Vector2[curTrans.Find("Pos").childCount];
            for (int i = 0; i < _ranPos.Length; i++)
            {
                _ranPos[i] = curTrans.Find("Pos").GetChild(i).position;
            }

            _block = new GameObject[2];
            for (int i = 0; i < _block.Length; i++)
            {
                _block[i] = curTrans.Find("Block").GetChild(i).gameObject;
            }

            _yfObj = new GameObject[curTrans.Find("YF").childCount];
            _yfAni = new GameObject[curTrans.Find("YF").childCount];
            for (int i = 0; i < _yfAni.Length; i++)
            {
                _yfObj[i] = curTrans.Find("YF").GetChild(i).gameObject;
                _yfAni[i] = _yfObj[i].transform.GetChild(0).gameObject;
            }

            _blockYF = new GameObject[curTrans.Find("Blockyf").childCount];
            for (int i = 0; i < _blockYF.Length; i++)
            {
                _blockYF[i] = curTrans.Find("Blockyf").GetChild(i).GetChild(0).gameObject;
            }
            ranArray = new int[7] { 0, 1, 2, 3, 4, 5, 6 };

            _moveTween = new Tween[7] { _tw1, _tw2, _tw3, _tw4, _tw5, _tw6, _tw7 };
            for (int i = 0; i < _moveTween.Length; i++)
            {
                _moveTween[i].Kill();
            }

            _yfStartPos = curTrans.Find("YFStartPos").position;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _level = 1;
            _count = 0;

            isPlaying = false;
            for (int i = 0; i < _moveTween.Length; i++)
            {
                _moveTween[i].Kill();
            }
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
            for (int i = 0; i < _yfObj.Length; i++)
            {
                _yfObj[i].Show();
            }
            SetBlock();

            for (int i = 0; i < _blockYF.Length; i++)
            {
                SpineManager.instance.DoAnimation(_blockYF[i], "kong", true);
            }
            for (int i = 0; i < _yfAni.Length; i++)
            {
                SpineManager.instance.DoAnimation(_yfAni[i], "kong", true);
            }
            for (int i = 0; i < _quan.childCount; i++)
            {
                _quan.GetChild(i).position = _quanRePos;
                _quan.GetChild(i).localScale = new Vector3(0, 0, 0);
            }

            SpineManager.instance.DoAnimation(_human, "ren", true);
            SpineManager.instance.DoAnimation(_snake, "snake", true);
            _wxp.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_wxp, "kong", false);
            _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_xem, "xem1", true);

            if(_isEnd)
            {
                InitGame();
            }

            //bug问题，将组件删除重加就可以，暂未找到源头
            for (int i = 0; i < curTrans.Find("YF").childCount; i++)
            {
                Transform tra = curTrans.Find("YF").GetChild(i);
                if (tra.GetComponent<mILDrager>() == true)
                    Component.DestroyImmediate(tra.GetComponent<mILDrager>());
                tra.gameObject.AddComponent<mILDrager>();
            }
            _yfDrag = curTrans.Find("YF").GetComponentsInChildren<mILDrager>();
            InitDrag();

            _isEnd = false;
        }

        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //田丁游戏开始方法
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
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => 
                    {
                        mask.SetActive(false);
                        bd.SetActive(false);
                        InitGame();
                    }));
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
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
            ChangeClickArea();
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
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            isPlaying = false;
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
                else if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); InitGame(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
        }

        #region 根据按钮数量调整点击区域
        void ChangeClickArea()
        {
            int activeCount = 0;
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                if (anyBtns.GetChild(i).gameObject.activeSelf)
                    activeCount += 1;
            }

            anyBtns.GetComponent<RectTransform>().sizeDelta = activeCount == 2 ? new Vector2(680, 240) : new Vector2(240, 240);
        }

        #endregion

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
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            ChangeClickArea();
                            caidaiSpine.SetActive(false); 
                            successSpine.SetActive(false); 
                            ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion


        #region 游戏方法

        void InitDrag()
        {
            for (int i = 0; i < _yfDrag.Length; i++)
            {
                Debug.LogError(_yfDrag[i].transform.name);
            }

            for (int i = 0; i < _yfDrag.Length; i++)
            {
                _yfDrag[i].index = _yfDrag[i].transform.gameObject.transform.GetSiblingIndex();
                _yfDrag[i].DragRect = Bg.transform.GetRectTransform();
                _yfDrag[i].IsNeedOffset = false;
                if (i <= 4)
                    _yfDrag[i].drops = new mILDroper[1] { _dropEnd.GetChild(i).GetComponent<mILDroper>() };
                else
                    _yfDrag[i].drops = new mILDroper[1] { _dropEnd.GetChild(5).GetComponent<mILDroper>() };
                _yfDrag[i].SetDragCallback(StartDrag, Draging, EndDrag, null);
            }
        }

        void CanDrag()
        {
            for (int i = 0; i < _yfObj.Length; i++)
                _yfObj[i].transform.GetComponent<Empty4Raycast>().raycastTarget = true;
        }

        void NotDrag()
        {
            for (int i = 0; i < _yfObj.Length; i++)
                _yfObj[i].transform.GetComponent<Empty4Raycast>().raycastTarget = false;
        }

        private void StartDrag(Vector3 dragPos, int dragType, int dragIndex)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            if (dragIndex <= 4)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                _dragingAnisStr = SpineManager.instance.GetCurrentAnimationName(_yfAni[dragIndex]);
                SpineManager.instance.DoAnimation(_yfAni[dragIndex], _level == 1 ? "YF-B" + (dragIndex + 1).ToString() : "YF-A" + (dragIndex + 1).ToString(), true);
                _moveTween[dragIndex].Kill();
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                _yfDrag[dragIndex].canMove = false;
                _moveTween[dragIndex].Kill();
                SpineManager.instance.DoAnimation(_yfAni[dragIndex], "YF-A" + (dragIndex + 6).ToString(), true);
            }
        }

        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            
        }

        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                _count++;
                _yfObj[dragIndex].Hide();
                NotDrag();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(_blockYF[dragIndex], _level == 1 ? "yf-b" + (dragIndex + 6).ToString() : "yf-a" + (dragIndex + 6).ToString(), false,
                () =>
                {
                    CanDrag();
                    if (_count == 5)
                    {
                        NotDrag();
                        _canStopAni = false;
                        for (int i = 0; i < _yfObj.Length; i++)
                        {
                            _yfObj[i].Hide();
                        }

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, _level == 1 ? 3 : 4, false);
                        //关卡结束
                        for (int i = 0; i < _blockYF.Length; i++)
                        {
                            int ranTime = Random.Range(10, 50);
                            GameObject o = _blockYF[i];
                            WaitTimeAndExcuteNext(ranTime * 0.01f,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(o, _level == 1 ? "yf-b" + (o.transform.parent.GetSiblingIndex() + 1).ToString() : "yf-a" + (o.transform.parent.GetSiblingIndex() + 1).ToString(), true,
                                () =>
                                {
                                    if (_canStopAni)
                                    {
                                        SpineManager.instance.DoAnimation(o, _level == 1 ? "yf-b" + (o.transform.parent.GetSiblingIndex() + 1).ToString() : "yf-a" + (o.transform.parent.GetSiblingIndex() + 1).ToString(), false);
                                    }
                                });
                            });
                        }

                        //下一关判定
                        EndLevel();
                    }
                });
            }
            else
            {
                if (dragIndex <= 4)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    NotDrag();
                    SpineManager.instance.DoAnimation(_xem, "xem-jx", false, 
                    () => 
                    {
                        SpineManager.instance.DoAnimation(_xem, "xem1", true);
                        CanDrag();
                    });
                    SpineManager.instance.DoAnimation(_yfAni[dragIndex], _level == 1 ? "YF-B" + (dragIndex + 6).ToString() : "YF-A" + (dragIndex + 6).ToString(), false);
                    _yfObj[dragIndex].transform.position = _ranPos[ranArray[dragIndex]];
                    RandomMove(dragIndex);
                }
                else
                {
                    SpineManager.instance.DoAnimation(_yfAni[dragIndex], "YF-A" + (dragIndex + 8).ToString(), false);
                    RandomMove(dragIndex);
                    CanDrag();
                }
            }
        }

        void EndLevel()
        {
            for (int i = 0; i < _quan.childCount; i++)
            {
                _quan.GetChild(i).position = _quanRePos;
                _quan.GetChild(i).localScale = new Vector3(0, 0, 0);
            }

            WaitTimeAndExcuteNext(2.0f,
            () =>
            {
                _canStopAni = true;
                SpineManager.instance.DoAnimation(_snake, "snake2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_snake, "snake", true);
                    SpineManager.instance.DoAnimation(_xem, "xem-du", false,
                    () =>
                    {
                        if(_count == 5)
                            _level++;
                        if (_level > 2)
                        {
                            //游戏结束
                            SpineManager.instance.DoAnimation(_xem, "xem-y2", true);
                            WaitTimeAndExcuteNext(2.0f,
                            () =>
                            {
                                _isEnd = true;
                                playSuccessSpine();
                            });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(_xem, "xem1", true);
                            WaitTimeAndExcuteNext(2.0f,
                            () =>
                            {
                                mask.Show();
                                anyBtns.gameObject.SetActive(true);
                                anyBtns.GetChild(0).gameObject.SetActive(true);
                                anyBtns.GetChild(1).gameObject.SetActive(false);
                                anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                ChangeClickArea();
                            });
                        }
                    });
                });
            });
        }

        void InitGame()
        {
            DOTween.KillAll();
            _count = 0;

            SetBlock();
            NotDrag();
            _wxp.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_wxp, "kong", false);

            for (int i = 0; i < _blockYF.Length; i++)
            {
                SpineManager.instance.DoAnimation(_blockYF[i], "kong", true);
            }
            for (int i = 0; i < _yfAni.Length; i++)
            {
                SpineManager.instance.DoAnimation(_yfAni[i], "kong", true);
            }
            for (int i = 0; i < _yfObj.Length; i++)
            {
                _yfObj[i].Show();
            }
            for (int i = 0; i < _yfAni.Length; i++)
            {
                _yfObj[i].transform.position = _yfStartPos;
                _yfObj[i].transform.localScale = new Vector3(0, 0, 0);
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, _level == 1 ? 6 : 7, false);
            SpineManager.instance.DoAnimation(_human, "ren", false, 
            () => 
            {
                SpineManager.instance.DoAnimation(_wxp, "wxp", false,
                () =>
                {
                    RandomQuan();
                    RandomPos();
                    StartYF(0);
                });
            });
        }

        void SetBlock()
        {
            if(_level == 1)
            {
                _block[1].Show();
                _block[0].Hide();
            }
            else
            {
                _block[0].Show();
                _block[1].Hide();
            }
        }

        void RandomQuan()
        {
            for (int i = 0; i < _quan.childCount; i++)
            {
                _quan.GetChild(i).position = _quanRePos;
                _quan.GetChild(i).localScale = new Vector3(0, 0, 0);
            }

            for (int i = 0; i < _quan.childCount; i++)
            {
                float scale = Random.Range(4, 11) * 0.1f;
                _quan.GetChild(i).DOScale(new Vector3(scale, scale, 1), 3.0f);
                _quan.GetChild(i).DOMove(_quanPos.GetChild(i).position, 3.0f);
                int ani = Random.Range(0, 20);
                SpineManager.instance.SetTimeScale(_quan.GetChild(i).gameObject, 1 + ani * 0.05f);
                if (ani < 10)
                    SpineManager.instance.DoAnimation(_quan.GetChild(i).gameObject, "quanquan", true);
                else
                    SpineManager.instance.DoAnimation(_quan.GetChild(i).gameObject, "quanquan2", true);
            }
        }

        void RandomPos()
        {
            for (int i = 0; i < ranArray.Length; i++)
            {
                int random = Random.Range(0, ranArray.Length);
                int temp = ranArray[random];
                ranArray[random] = ranArray[ranArray.Length - random - 1];
                ranArray[ranArray.Length - random - 1] = temp;
            }
        }

        void StartYF(int index)
        {
            if(_level == 1)
            {
                if (index <= 4)
                {
                    SpineManager.instance.DoAnimation(_yfAni[index], "YF-B" + (index + 6).ToString(), false);
                    _yfObj[index].transform.DOScale(new Vector3(1, 1, 1), 1.0f);
                    _yfObj[index].transform.DOMove(_ranPos[ranArray[index]], 1.0f).OnComplete(
                     () =>
                     {
                         int i = index + 1;
                         StartYF(i);
                     });
                }
                else
                {
                    _yfObj[5].Hide();
                    _yfObj[6].Hide();
                    CanDrag();
                    RandomMove(0);
                    RandomMove(1);
                    RandomMove(2);
                    RandomMove(3);
                    RandomMove(4);
                }
            }
            else
            {
                if (index <= 4)
                {
                    SpineManager.instance.DoAnimation(_yfAni[index], "YF-A" + (index + 6).ToString(), false);
                    _yfObj[index].transform.DOScale(new Vector3(1, 1, 1), 1.0f);
                    _yfObj[index].transform.DOMove(_ranPos[ranArray[index]], 1.0f).OnComplete(
                     () =>
                     {
                         int i = index + 1;
                         StartYF(i);
                     });
                }
                else if(index <= 6 && index > 4)
                {
                    _yfObj[index].Show();
                    SpineManager.instance.DoAnimation(_yfAni[index], "YF-A" + (index + 8).ToString(), false);
                    _yfObj[index].transform.DOScale(new Vector3(1, 1, 1), 1.0f);
                    _yfObj[index].transform.DOMove(_ranPos[ranArray[index]], 1.0f).OnComplete(
                     () =>
                     {
                         int i = index + 1;
                         StartYF(i);
                     });
                }
                else
                {
                    CanDrag();
                    RandomMove(0);
                    RandomMove(1);
                    RandomMove(2);
                    RandomMove(3);
                    RandomMove(4);
                    RandomMove(5);
                    RandomMove(6);
                }
            }
        }

        void RandomMove(int i)
        {
            float ranTime;

            if (_level == 1)
                ranTime = Random.Range(50, 65) * 0.1f;
            else
                ranTime = Random.Range(35, 50) * 0.1f;
            int ranX = Random.Range(0, Screen.width);
            int ranY = Random.Range(0, Screen.height);
            _moveTween[i] = _yfObj[i].transform.DOMove(new Vector2(ranX, ranY), ranTime).OnComplete(() => { RandomMove(i); });
            _moveTween[i].SetEase(Ease.Linear);
        }
        #endregion
    }
}
