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
    public class TD8941Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject dd;
        private GameObject ddd;

        #region Mask
        private Transform anyBtns;
        private GameObject mask;
        private bool _canClickBtn;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion

        #endregion
        #endregion

        private GameObject[] _click1;
        private GameObject[] _click2;
        private GameObject[] _click3;
        private Transform _pos;
        private GameObject _cam;
        private GameObject _photo;
        private GameObject _xem;
        private GameObject _caidai;

        private bool _canClick;
        private int _level;
        private int _speed;
        private string _curAniName;
        private GameObject _lastClick;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            _click1 = new GameObject[curTrans.Find("1").childCount];
            for (int i = 0; i < curTrans.Find("1").childCount; i++)
            {
                curTrans.Find("1").GetGameObject((i + 1).ToString()).transform.SetSiblingIndex(i);
            }
            for (int i = 0; i < curTrans.Find("1").childCount; i++)
            {
                _click1[i] = curTrans.Find("1").GetChild(i).gameObject;
                Util.AddBtnClick(_click1[i], ClickEvent);
            }

            _click2 = new GameObject[curTrans.Find("2").childCount];
            for (int i = 0; i < curTrans.Find("2").childCount; i++)
            {
                curTrans.Find("2").GetGameObject((i + 1).ToString()).transform.SetSiblingIndex(i);
            }
            for (int i = 0; i < curTrans.Find("2").childCount; i++)
            {
                _click2[i] = curTrans.Find("2").GetChild(i).gameObject;
                Util.AddBtnClick(_click2[i], ClickEvent);
            }

            _click3 = new GameObject[curTrans.Find("3").childCount];
            for (int i = 0; i < curTrans.Find("3").childCount; i++)
            {
                curTrans.Find("3").GetGameObject((i + 1).ToString()).transform.SetSiblingIndex(i);
            }
            for (int i = 0; i < curTrans.Find("3").childCount; i++)
            {
                _click3[i] = curTrans.Find("3").GetChild(i).gameObject;
                Util.AddBtnClick(_click3[i], ClickEvent);
            }
            _cam = curTrans.GetGameObject("cam");
            _photo = curTrans.GetGameObject("photo");
            _xem = curTrans.GetGameObject("xem");
            _caidai = curTrans.GetGameObject("caidai");
            _pos = curTrans.Find("Pos");

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
            //_speed = 100*Screen.width/Screen.height;
            _canClick = true;
            _lastClick = null;
            _canClickBtn = true;

            _cam.Hide();
            _photo.Hide();
            _caidai.Hide();

            InitAni(_xem);
            SpineManager.instance.DoAnimation(_xem, "xem1", true);

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
            InitPos();
            InitClickAni();
            UpdateLevel();
        }
        
        void InitPos()
        {
            for (int i = 0; i < _click1.Length; i++)
            {
                SetPos(_click1[i].transform, _pos.Find("1").GetChild(i).position);
            }
            for (int i = 0; i < _click2.Length; i++)
            {
                SetPos(_click2[i].transform, _pos.Find("2").GetChild(i).position);
            }
            for (int i = 0; i < _click3.Length; i++)
            {
                SetPos(_click3[i].transform, _pos.Find("3").GetChild(i).position);
            }
        }

        void InitClickAni()
        {
            for (int i = 0; i < _click1.Length; i++)
            {
                GameObject o = _click1[i].transform.GetChild(0).gameObject;
                InitAni(o);
                SpineManager.instance.DoAnimation(o, "ye-a" + (o.name == "1" ? null : o.name), true);
            }
            for (int i = 0; i < _click2.Length; i++)
            {
                GameObject o = _click2[i].transform.GetChild(0).gameObject;
                InitAni(o);
                SpineManager.instance.DoAnimation(o, "j-a" + (o.name == "1" ? null : o.name), true);
            }
            for (int i = 0; i < _click3.Length; i++)
            {
                GameObject o = _click3[i].transform.GetChild(0).gameObject;
                InitAni(o);
                SpineManager.instance.DoAnimation(o, "ice-a" + (o.name == "1" ? null : o.name), true);
            }
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
                speaker = dd;
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

        void Wait(float len = 0, Action method_1 = null)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
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
                    mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    {
                        dd.Hide();
                        mask.Hide();
                        _canClick = true;
                    }));
                    break;
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
            _canClick = false;
            dd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
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
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        void InitAni(GameObject o)
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

        private void SetPos(Transform trans, Vector2 pos)
        {
            trans.position = pos;
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
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(false);
            ddd = curTrans.Find("mask/DDD").gameObject;
            ddd.SetActive(false);
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
            _canClickBtn = true;
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (_canClickBtn)
            {
                _canClickBtn = false;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                    }
                    else if (obj.name == "next")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, 
                        () => 
                        { 
                            anyBtns.gameObject.SetActive(false); 
                            mask.SetActive(false); 

                            _photo.Hide();
                            _canClick = true;
                            _lastClick = null;
                            _level++;

                            UpdateLevel();
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); ddd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(ddd, SoundManager.SoundType.VOICE, 2)); });
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

        #region 游戏
        //修改背景
        void ChangeBg(int index)
        {
            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[index];
        }

        //修改动画前缀
        void UpdateAniName()
        {
            _curAniName = _level == 1 ? "ye-" : (_level == 2 ? "j-" : "ice-");
        }

        //换关卡
        void UpdateLevel()
        {
            curTrans.Find("1").gameObject.Hide();
            curTrans.Find("2").gameObject.Hide();
            curTrans.Find("3").gameObject.Hide();
            curTrans.Find(_level.ToString()).gameObject.Show();

            UpdateAniName();
            ChangeBg(_level - 1);
        }

        //点击事件
        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                if(_lastClick == null)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    _lastClick = obj;
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, _curAniName + "d" + (obj.name == "1" ? null : obj.name), true);
                    _canClick = true;
                }
                else
                {
                    if(obj == _lastClick)
                    {
                        _lastClick = null;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, _curAniName + "a" + (obj.name == "1" ? null : obj.name), true);
                        _canClick = true;
                    }
                    else
                    {
                        bool _canStopAni = false;
                        //float dis = Mathf.Abs(obj.transform.position.x - _lastClick.transform.position.x);
                        _lastClick.transform.SetAsLastSibling();
                        obj.transform.SetAsLastSibling();
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, _curAniName + "b" + (obj.name == "1" ? null : obj.name), false, 
                        ()=> 
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, _curAniName + "a" + (obj.name == "1" ? null : obj.name), true);
                            //if (_canStopAni)
                            //{
                            //    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            //    SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, _curAniName + "a" + (obj.name == "1" ? null : obj.name), true);
                            //}
                        });
                        SpineManager.instance.DoAnimation(_lastClick.transform.GetChild(0).gameObject, _curAniName + "b" + (_lastClick.name == "1" ? null : _lastClick.name), false, 
                        ()=> 
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SpineManager.instance.DoAnimation(_lastClick.transform.GetChild(0).gameObject, _curAniName + "a" + (_lastClick.name == "1" ? null : _lastClick.name), true);
                            //if (_canStopAni&& _lastClick)
                            //{
                            //    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            //    SpineManager.instance.DoAnimation(_lastClick.transform.GetChild(0).gameObject, _curAniName + "a" + (_lastClick.name == "1" ? null : _lastClick.name), true);
                            //}
                        });

                        float len = SpineManager.instance.GetAnimationLength(_lastClick.transform.GetChild(0).gameObject, _curAniName + "b" + (_lastClick.name == "1" ? null : _lastClick.name));
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, _level, true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, _level, true);
                        Tween tw1 = obj.transform.DOMoveX(_lastClick.transform.position.x, len);
                        Tween tw2 = _lastClick.transform.DOMoveX(obj.transform.position.x, len).OnComplete(
                        ()=> 
                        {
                            _canStopAni = true;
                            JudgeSuccess(obj, _lastClick);
                        });

                        tw1.SetEase(Ease.Linear);
                        tw2.SetEase(Ease.Linear);
                    }
                }
            }
        }

        //判断是否已完成
        void JudgeSuccess(GameObject obj1, GameObject obj2)
        {
            bool allTrue = true;
            for (int i = 0; i < (_level == 1 ? _click1.Length : (_level == 2 ? _click2.Length : _click3.Length)) - 1; i++)
            {
                GameObject first = (_level == 1 ? curTrans.Find("1").GetGameObject((i + 1).ToString()) : (_level == 2 ? curTrans.Find("2").GetGameObject((i + 1).ToString()) : curTrans.Find("3").GetGameObject((i + 1).ToString())));
                GameObject second = (_level == 1 ? curTrans.Find("1").GetGameObject((i + 2).ToString()) : (_level == 2 ? curTrans.Find("2").GetGameObject((i + 2).ToString()) : curTrans.Find("3").GetGameObject((i + 2).ToString())));
                if (first.transform.position.x > second.transform.position.x)
                {
                    allTrue = false;
                    break;
                }
            }

            Wait(1.0f, 
            ()=> 
            {
                if (allTrue)
                {
                    //成功
                    for (int i = 0; i < (_level == 1 ? _click1.Length : (_level == 2 ? _click2.Length : _click3.Length)); i++)
                    {
                        GameObject o = (_level == 1 ? _click1[i].transform.GetChild(0).gameObject : (_level == 2 ? _click2[i].transform.GetChild(0).gameObject : _click3[i].transform.GetChild(0).gameObject));
                        float ranTime = Random.Range(0, 5) * 0.1f;
                        Wait(ranTime, () => { SpineManager.instance.DoAnimation(o, _curAniName + "c" + (o.name == "1" ? null : o.name), true); });
                    }

                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                    _caidai.Show();
                    InitAni(_caidai);
                    SpineManager.instance.DoAnimation(_caidai, "sp", false);
                    InitAni(_xem);
                    if (_level != 3)
                        SpineManager.instance.DoAnimation(_xem, "xem-y", false, () => { SpineManager.instance.DoAnimation(_xem, "xem1", true); });
                    else
                        SpineManager.instance.DoAnimation(_xem, "xem-y", false, 
                        () => 
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                            SpineManager.instance.DoAnimation(_xem, "xem-y2", true); 
                        });

                    Wait(3.0f,
                    () =>
                    {
                        _cam.Show();
                        InitAni(_cam);
                        SpineManager.instance.DoAnimation(_cam, "cam", false, () => { _cam.Hide(); });
                        Wait(1.2f,
                        () =>
                        {
                            _photo.Show();
                            InitAni(_photo);
                            SpineManager.instance.DoAnimation(_photo, "photo" + (_level == 2 ? null : (_level == 1 ? 2.ToString() : 3.ToString())), false,
                            () =>
                            {
                                if (_level == 3)
                                {
                                    Wait(2.5f,
                                    () =>
                                    {
                                        playSuccessSpine();
                                    });
                                }
                                else
                                {
                                    Wait(2.5f,
                                    () =>
                                    {
                                        mask.Show();
                                        anyBtns.gameObject.SetActive(true);
                                        anyBtns.GetChild(0).gameObject.SetActive(true);
                                        anyBtns.GetChild(1).gameObject.SetActive(false);
                                        anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                    });
                                }
                            });
                        });
                    });
                }
                else
                {
                    //暂时未成功
                    _canClick = true;
                    _lastClick = null;
                    SpineManager.instance.DoAnimation(obj1.transform.GetChild(0).gameObject, _curAniName + "a" + (obj1.name == "1" ? null : obj1.name), true);
                    SpineManager.instance.DoAnimation(obj2.transform.GetChild(0).gameObject, _curAniName + "a" + (obj2.name == "1" ? null : obj2.name), true);
                }
            });
        }

        #endregion
    }
}
