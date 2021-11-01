using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public enum ColorEnum
    {
        red,
        green,
        blue,
        orange,
        purple,
        yellow,
    }

    public class TD8931Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject tt;
        private GameObject dtt;

        #region Mask
        private Transform anyBtns;
        bool canClickBtn;
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

        private Transform _life;
        private GameObject _xem;
        private GameObject _guangxiao;
        private GameObject _shandian;

        private Transform _level1;
        private Image[] _clickImg1;
        private GameObject _black1;
        private GameObject _topWindow1;
        private GameObject _bottomWindow1;
        private GameObject _allWindow1;
        private Dictionary<GameObject, ColorEnum> _colorDic1;

        private Transform _level2;
        private Image[] _clickImg2;
        private GameObject _black2;
        private GameObject _topWindow2;
        private GameObject _bottomWindow2;
        private GameObject _allWindow2;
        private Dictionary<GameObject, ColorEnum> _colorDic2;

        private Transform _level3;
        private Image[] _clickImg3;
        private GameObject _black3;
        private GameObject _topWindow3;
        private GameObject _bottomWindow3;
        private GameObject _allWindow3;
        private Dictionary<GameObject, ColorEnum> _colorDic3;

        private int _level;
        private bool _canClickImg;
        private GameObject _lastClick;
        private GameObject _curClick;
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

            _life = curTrans.Find("Life");
            _xem = curTrans.GetGameObject("xem");
            _guangxiao = curTrans.GetGameObject("guangxiao");
            _shandian = curTrans.GetGameObject("shandian");

            _level1 = curTrans.Find("1");
            _clickImg1 = _level1.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < _clickImg1.Length; i++)
            {
                _clickImg1[i].eventAlphaThreshold = 0.5f;
                Util.AddBtnClick(_clickImg1[i].gameObject, OnPoint);
            }
            _black1 = _level1.GetGameObject("Black");
            _topWindow1 = _level1.GetGameObject("Other/hh");
            _bottomWindow1 = _level1.GetGameObject("Other/ii");
            _allWindow1 = _level1.GetGameObject("Other/jj");

            _level2 = curTrans.Find("2");
            _clickImg2 = _level2.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < _clickImg2.Length; i++)
            {
                _clickImg2[i].eventAlphaThreshold = 0.5f;
                Util.AddBtnClick(_clickImg2[i].gameObject, OnPoint);
            }

            _black2 = _level2.GetGameObject("Black");
            _topWindow2 = _level2.GetGameObject("Other/jj");
            _bottomWindow2 = _level2.GetGameObject("Other/kk");
            _allWindow2 = _level2.GetGameObject("Other/ll");

            _level3 = curTrans.Find("3");
            _clickImg3 = _level3.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < _clickImg3.Length; i++)
            {
                _clickImg3[i].eventAlphaThreshold = 0.5f;
                Util.AddBtnClick(_clickImg3[i].gameObject, OnPoint);
            }
            _black3 = _level3.GetGameObject("Black");
            _topWindow3 = _level3.GetGameObject("Other/l");
            _bottomWindow3 = _level3.GetGameObject("Other/m");
            _allWindow3 = _level3.GetGameObject("Other/n");

            AddDictionary();
            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            canClickBtn = true;
            _canClickImg = true;
            talkIndex = 1;
            _level = 0;
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
            _shandian.Hide();
            InitializeAni(_xem);
            SpineManager.instance.DoAnimation(_xem, "XEM", true);
            _guangxiao.Hide();
            for (int i = 0; i < _life.childCount; i++)
            {
                _life.GetChild(i).gameObject.Show();
            }

            LoadLevel();
        }

        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                speaker = tt;
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
                    mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null, 
                    () => 
                    {
                        mask.SetActive(false); 
                        tt.SetActive(false); 
                    }));

                    break;
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
            tt.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true);}));
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

        private void InitializeAni(GameObject o)
        {
            o.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
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

            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dtt = curTrans.Find("mask/DTT").gameObject;
            dtt.SetActive(false);
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
            canClickBtn = true;
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (canClickBtn)
            {
                canClickBtn = false;
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); LoadLevel(); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dtt.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dtt, SoundManager.SoundType.VOICE, 2)); });
                    }
                });
            }
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
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion

        #region 游戏方法

        void AddDictionary()
        {
            _colorDic1 = new Dictionary<GameObject, ColorEnum>();
            _colorDic1.Add(_level1.GetGameObject("b"), ColorEnum.yellow);
            _colorDic1.Add(_level1.GetGameObject("c"), ColorEnum.green);
            _colorDic1.Add(_level1.GetGameObject("d"), ColorEnum.orange);
            _colorDic1.Add(_level1.GetGameObject("e"), ColorEnum.purple);
            _colorDic1.Add(_level1.GetGameObject("f"), ColorEnum.red);
            _colorDic1.Add(_level1.GetGameObject("g"), ColorEnum.blue);

            _colorDic2 = new Dictionary<GameObject, ColorEnum>();
            _colorDic2.Add(_level2.GetGameObject("b"), ColorEnum.yellow);
            _colorDic2.Add(_level2.GetGameObject("c"), ColorEnum.red);
            _colorDic2.Add(_level2.GetGameObject("d"), ColorEnum.blue);
            _colorDic2.Add(_level2.GetGameObject("e"), ColorEnum.purple);
            _colorDic2.Add(_level2.GetGameObject("f"), ColorEnum.orange);
            _colorDic2.Add(_level2.GetGameObject("g"), ColorEnum.yellow);
            _colorDic2.Add(_level2.GetGameObject("h"), ColorEnum.green);
            _colorDic2.Add(_level2.GetGameObject("i"), ColorEnum.purple);

            _colorDic3 = new Dictionary<GameObject, ColorEnum>();
            _colorDic3.Add(_level3.GetGameObject("b"), ColorEnum.red);
            _colorDic3.Add(_level3.GetGameObject("c"), ColorEnum.purple);
            _colorDic3.Add(_level3.GetGameObject("d"), ColorEnum.yellow);
            _colorDic3.Add(_level3.GetGameObject("e"), ColorEnum.green);
            _colorDic3.Add(_level3.GetGameObject("f"), ColorEnum.blue);
            _colorDic3.Add(_level3.GetGameObject("g"), ColorEnum.red);
            _colorDic3.Add(_level3.GetGameObject("h"), ColorEnum.yellow);
            _colorDic3.Add(_level3.GetGameObject("i"), ColorEnum.orange);
            _colorDic3.Add(_level3.GetGameObject("j"), ColorEnum.green);
            _colorDic3.Add(_level3.GetGameObject("k"), ColorEnum.purple);
        }

        //加载关卡
        void LoadLevel()
        {
            _lastClick = null;
            _curClick = null;
            _canClickImg = true;
            if (_level == 0)
            {
                _level1.gameObject.Show();
                _level2.gameObject.Hide();
                _level3.gameObject.Hide();

                LoadAllObj(_black1, _clickImg1, _topWindow1, _bottomWindow1, _allWindow1);
            }

            if (_level == 1)
            {
                _level1.gameObject.Hide();
                _level2.gameObject.Show();
                _level3.gameObject.Hide();

                LoadAllObj(_black2, _clickImg2, _topWindow2, _bottomWindow2, _allWindow2);
            }

            if (_level == 2)
            {
                _level1.gameObject.Hide();
                _level2.gameObject.Hide();
                _level3.gameObject.Show();

                LoadAllObj(_black3, _clickImg3, _topWindow3, _bottomWindow3, _allWindow3);
            }
        }

        //加载关卡下的所有物体
        void LoadAllObj(GameObject black, Image[] clickImg, GameObject topWindow, GameObject bottomWindow, GameObject allWindow)
        {
            black.Show();
            InitializeAni(black);
            SpineManager.instance.DoAnimation(black, "a", false);
            for (int i = 0; i < clickImg.Length; i++)
            {
                clickImg[i].transform.gameObject.Show();
                InitializeAni(clickImg[i].transform.GetChild(0).gameObject);
                SpineManager.instance.DoAnimation(clickImg[i].transform.GetChild(0).gameObject, clickImg[i].transform.GetChild(0).gameObject.name, false);
            }

            InitializeAni(topWindow);
            SpineManager.instance.DoAnimation(topWindow, topWindow.name, false);
            InitializeAni(bottomWindow);
            SpineManager.instance.DoAnimation(bottomWindow, bottomWindow.name, false);
            InitializeAni(allWindow);
            SpineManager.instance.DoAnimation(allWindow, "kong", false);
            allWindow.transform.GetGameObject(allWindow.name).Hide();
        }

        //点击事件
        private void OnPoint(GameObject obj)
        {
            if(_canClickImg)
            {
                _canClickImg = false;
                SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6, false);
                string curAni = SpineManager.instance.GetCurrentAnimationName(obj.transform.GetChild(0).gameObject);
                if(curAni == obj.name)
                {
                    if(_lastClick == null)
                    {
                        _lastClick = obj;
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "2", true);
                        _canClickImg = true;
                    }
                    else
                    {
                        _curClick = obj;
                        SpineManager.instance.DoAnimation(_curClick.transform.GetChild(0).gameObject, _curClick.name + "2", true);
                        if(_level == 0)
                            JudgeColor(_colorDic1[_lastClick], _colorDic1[_curClick]);
                        else if(_level == 1)
                            JudgeColor(_colorDic2[_lastClick], _colorDic2[_curClick]);
                        else
                            JudgeColor(_colorDic3[_lastClick], _colorDic3[_curClick]);
                    }
                }
                else
                {
                    _lastClick = null;
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, true);
                    _canClickImg = true;
                }
            }
        }

        //判断颜色是否凑成互补色
        private void JudgeColor(ColorEnum color1, ColorEnum color2)
        {
            Debug.Log(color1);
            Debug.Log(color2);
            //黄对紫
            if((color1 == ColorEnum.yellow && color2 == ColorEnum.purple) || (color1 == ColorEnum.purple && color2 == ColorEnum.yellow))
            {
                IsSuccess();
            }
            //蓝对橙
            else if ((color1 == ColorEnum.blue && color2 == ColorEnum.orange) || (color1 == ColorEnum.orange && color2 == ColorEnum.blue))
            {
                IsSuccess();
            }
            //红对绿
            else if ((color1 == ColorEnum.red && color2 == ColorEnum.green) || (color1 == ColorEnum.green && color2 == ColorEnum.red))
            {
                IsSuccess();
            }
            //配对失败
            else
            {
                IsFalse();
            }
        }

        void IsSuccess()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
            WaitTimeAndExcuteNext(0.5f, 
            () => 
            {
                _lastClick.Hide();
                _curClick.Hide();
                WaitTimeAndExcuteNext(0.2f, 
                () => 
                {
                    _lastClick = null;
                    _curClick = null;
                    JudgeEnd();
                });
            });
        }

        void IsFalse()
        {
            WaitTimeAndExcuteNext(0.5f,
            () =>
            {
                SpineManager.instance.DoAnimation(_lastClick.transform.GetChild(0).gameObject, _lastClick.name, false);
                SpineManager.instance.DoAnimation(_curClick.transform.GetChild(0).gameObject, _curClick.name, false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                SpineManager.instance.DoAnimation(_xem, "XEM2", false);
                WaitTimeAndExcuteNext(1.5f, 
                () => 
                {
                    _lastClick = null;
                    _curClick = null;
                    SpineManager.instance.DoAnimation(_xem, "XEM", true);
                    _canClickImg = true;
                });
            });
        }

        //判断关卡结束
        private void JudgeEnd()
        {
            bool canEnd = true;
            if(_level == 0)
            {
                for (int i = 0; i < _clickImg1.Length; i++)
                {
                    if(_clickImg1[i].transform.gameObject.activeSelf)
                    {
                        canEnd = false;
                        break;
                    }
                }
            }
            else if (_level == 1)
            {
                for (int i = 0; i < _clickImg2.Length; i++)
                {
                    if (_clickImg2[i].transform.gameObject.activeSelf)
                    {
                        canEnd = false;
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < _clickImg3.Length; i++)
                {
                    if (_clickImg3[i].transform.gameObject.activeSelf)
                    {
                        canEnd = false;
                        break;
                    }
                }
            }

            if(canEnd)
            {
                if(_level == 0)
                    EndAction(_black1, _topWindow1, _bottomWindow1, _allWindow1);
                else if (_level == 1)
                    EndAction(_black2, _topWindow2, _bottomWindow2, _allWindow2);
                else
                    EndAction(_black3, _topWindow3, _bottomWindow3, _allWindow3);
            }
            else
            {
                _canClickImg = true;
            }

        }

        //关卡结束的一系列动画
        private void EndAction(GameObject black, GameObject topWindow, GameObject bottomWindow, GameObject allWindow)
        {
            SpineManager.instance.DoAnimation(black, "a2", false,
            () =>
            {
                SpineManager.instance.DoAnimation(black, "kong", false);
                SpineManager.instance.DoAnimation(topWindow, topWindow.name + "2", false);
                SpineManager.instance.DoAnimation(bottomWindow, bottomWindow.name + "2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(topWindow, "kong", false);
                    SpineManager.instance.DoAnimation(bottomWindow, "kong", false);
                    SpineManager.instance.DoAnimation(allWindow, allWindow.name, false,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        GameObject o = allWindow.transform.GetChild(0).gameObject;
                        o.Show();
                        SpineManager.instance.DoAnimation(o, o.name + "4", true);
                        _guangxiao.Show();
                        InitializeAni(_guangxiao);
                        SpineManager.instance.DoAnimation(_guangxiao, _guangxiao.name + "1", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_guangxiao, _guangxiao.name + "2", false,
                            () =>
                            {
                                _guangxiao.Hide();
                                _shandian.Show();
                                InitializeAni(_shandian);
                                SpineManager.instance.DoAnimation(_shandian, _shandian.name, true);
                                SpineManager.instance.DoAnimation(_xem, "XEM4", false,
                                () =>
                                {
                                    o.Hide();
                                    _shandian.Hide();
                                    _life.GetGameObject(_level.ToString()).Hide();
                                    if (_level != 2)
                                    {
                                        SpineManager.instance.DoAnimation(_xem, "XEM", true);
                                        _level++;
                                        WaitTimeAndExcuteNext(3.0f, () => 
                                        {
                                            mask.Show();
                                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                            anyBtns.gameObject.SetActive(true);
                                            anyBtns.GetChild(0).gameObject.SetActive(true);
                                            anyBtns.GetChild(1).gameObject.SetActive(false);
                                            ChangeClickArea();
                                        });
                                    }
                                    else
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                                        SpineManager.instance.DoAnimation(_xem, "XEM5", true);
                                        WaitTimeAndExcuteNext(3.0f, () => { playSuccessSpine(); });
                                    }
                                });
                            });
                        });
                    });
                });
            });
        }
        #endregion
    }
}
