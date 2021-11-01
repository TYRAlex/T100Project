using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
        next,
    }

    public enum XingXingForm
    {
        XXxingxing,
        Xxingxing,
        Daxingxing,
    }

    public class TD6742Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

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

        private GameObject _level1;
        private GameObject _xx;
        private Transform _ani;
        private GameObject[] _aniArray;
        private GameObject[] _clickArray;
        private Transform _ly;
        private GameObject _pc;
        private Vector3 _pcPos1;
        private Vector3 _pcPos2;

        private GameObject _level2;
        private GameObject _bg2;
        private GameObject _bg2_2;
        private GameObject _bg2_3;
        private GameObject _bg2_4;
        private GameObject _XXX;
        private GameObject _XXXGuangxiao;
        private Transform _xiangjiao;
        private GameObject[] _xiangjiaoAni;
        private Transform _zhadan;
        private Transform _point;
        private Empty4Raycast[] _clickEmpty;
        private GameObject _xem;
        private Transform _pos;
        private Transform _dangteng1;
        private GameObject _dang1_1;
        private GameObject _dang1_2;
        private Transform _dangteng2;
        private GameObject _dang2_1;
        private GameObject _dang2_2;

        private bool _canClick;
        private int _curPoint;
        private int _getCount;
        private XingXingForm _curXingXingForm;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            _level1 = curTrans.GetGameObject("1");
            _xx = curTrans.GetGameObject("1/XX");
            _ani = curTrans.Find("1/Ani");
            _aniArray = new GameObject[_ani.childCount];
            _clickArray = new GameObject[_ani.childCount];
            for (int i = 0; i < _ani.childCount; i++)
            {
                _aniArray[i] = _ani.GetChild(i).gameObject;
                _clickArray[i] = _ani.GetChild(i).GetChild(0).gameObject;
                Util.AddBtnClick(_clickArray[i], clickMaoEvent);
            }
            _ly = curTrans.Find("1/ly");
            _pc = curTrans.GetGameObject("1/pc");
            _pcPos1 = curTrans.Find("1/pcPos1").position;
            _pcPos2 = curTrans.Find("1/pcPos2").position;

            _level2 = curTrans.GetGameObject("2");
            _bg2 = curTrans.GetGameObject("2/Bg");
            _bg2_2 = curTrans.GetGameObject("2/Bg2");
            _bg2_3 = curTrans.GetGameObject("2/Bg3");
            _bg2_4 = curTrans.GetGameObject("2/Bg4");
            _XXX = curTrans.GetGameObject("2/XXX");
            _XXXGuangxiao = curTrans.GetGameObject("2/guangxiao");
            _xiangjiao = curTrans.Find("2/Bg2/xiangjiao");
            _zhadan = curTrans.Find("2/Bg2/zhadan");
            _xiangjiaoAni = new GameObject[_xiangjiao.childCount];
            for (int i = 0; i < _xiangjiao.childCount; i++)
            {
                _xiangjiaoAni[i] = _xiangjiao.GetChild(i).gameObject;
            }
            _point = curTrans.Find("2/Bg/point");
            _clickEmpty = _point.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickEmpty.Length; i++)
            {
                Util.AddBtnClick(_clickEmpty[i].gameObject, ClickShuEvent);
            }
            _xem = curTrans.GetGameObject("2/Bg/xem");
            _pos = curTrans.Find("2/pos");
            _dangteng1 = curTrans.Find("2/Bg/dangteng1");
            _dang1_1 = _dangteng1.GetGameObject("1");
            _dang1_2 = _dangteng1.GetGameObject("2");
            _dangteng2 = curTrans.Find("2/Bg/dangteng2");
            _dang2_1 = _dangteng2.GetGameObject("1");
            _dang2_2 = _dangteng2.GetGameObject("2");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _canClick = false;
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
            _level1.Show();
            _level2.Hide();
            InitAni(_xx);
            SetAniSpeed(_xx, 1.0f);
            SpineManager.instance.DoAnimation(_xx, "XX", true);
            _ani.gameObject.Show();
            for (int i = 0; i < _aniArray.Length; i++)
            {
                InitAni(_aniArray[i]);
                SpineManager.instance.DoAnimation(_aniArray[i], "kong", false);
            }
            _pc.transform.position = _pcPos1;
            SpineManager.instance.DoAnimation(_pc.transform.GetGameObject("pc"), "pc1", true);
            for (int i = 0; i < _ly.childCount; i++)
            {
                SpineManager.instance.DoAnimation(_ly.GetChild(i).gameObject, "kong", false);
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
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(_aniArray[0], "ka2", false, 
                        () => 
                        {
                            SpineManager.instance.DoAnimation(_aniArray[0], "ka3", true);
                            SpineManager.instance.DoAnimation(_aniArray[1], "kb2", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_aniArray[1], "kb3", true);
                                SpineManager.instance.DoAnimation(_aniArray[2], "kc2", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_aniArray[2], "kc3", true);
                                    _canClick = true;
                                });
                            });
                        });
                    }));
                    break;
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
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

        void SetAniTimeScale(GameObject ani, float speed)
        {
            SpineManager.instance.SetTimeScale(ani, speed);
        }

        void InitAni(GameObject o)
        {
            o.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
        }

        void SetAniSpeed(GameObject o, float speed)
        {
            SpineManager.instance.SetTimeScale(o, speed);
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(1, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess2()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            int random;
            do
            {
                random = Random.Range(4, 10);
            }
            while (random == 4 || random == 8);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); SecondLevel(); });
                    }
                    else if (obj.name == "next")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            dd.Show();
                            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 2, null,
                            () =>
                            {
                                dd.Hide();
                                mask.SetActive(false);
                                SecondLevel();
                            }));
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); ddd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(ddd, SoundManager.SoundType.VOICE, 3)); });
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

        #region 第一关

        //点击毛发
        private void clickMaoEvent(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                if (obj.name == "0")
                {
                    GameObject ani = _aniArray[int.Parse(obj.name)];
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    BtnPlaySoundSuccess();
                    SpineManager.instance.DoAnimation(ani, "xiaoguo", false,
                    () =>
                    {
                        _ani.gameObject.Hide();
                        _pc.transform.DOMove(_pcPos2, 3.0f);
                        SpineManager.instance.DoAnimation(_pc.transform.GetGameObject("pc"), "pc2", false, 
                        ()=> 
                        {
                            SpineManager.instance.DoAnimation(_pc.transform.GetGameObject("pc"), "pc3", false, 
                            ()=> 
                            {
                                SpineManager.instance.DoAnimation(_pc.transform.GetGameObject("pc"), "pc1", true);
                            });
                        });

                        for (int i = 0; i < _ly.childCount; i++)
                        {
                            SpineManager.instance.DoAnimation(_ly.GetChild(i).gameObject, _ly.GetChild(i).gameObject.name, false);
                        }

                        Wait(0.6f, 
                        () => 
                        {
                            SetAniSpeed(_xx, 0.4f);
                        });
                        Wait(2.8f,
                        () =>
                        {
                            SetAniSpeed(_xx, 1.0f);
                        });

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        SpineManager.instance.DoAnimation(_xx, "XX2", false,
                        () =>
                        {
                            Wait(2.0f,
                            () =>
                            {
                                mask.Show();
                                anyBtns.gameObject.SetActive(true);
                                anyBtns.GetChild(0).gameObject.SetActive(true);
                                anyBtns.GetChild(1).gameObject.SetActive(false);
                                anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                ChangeClickArea();
                            });
                        });
                    });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    GameObject ani = _aniArray[int.Parse(obj.name)];
                    SpineManager.instance.DoAnimation(ani, ani.name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(ani, ani.name + "3", false);
                        _canClick = true;
                    });
                }
            }
        }
        #endregion


        #region 第二关

        //进入第二关
        private void SecondLevel()
        {
            _level1.Hide();
            _level2.Show();

            _XXX.Show();
            _XXXGuangxiao.Hide();
            _dang1_2.Hide();
            _dang2_2.Hide();

            _canClick = true;
            _curPoint = 0;
            _getCount = 0;
            _curXingXingForm = XingXingForm.XXxingxing;
            InitPos();

            InitAni(_XXX);
            SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
            InitAni(_xem);
            SpineManager.instance.DoAnimation(_xem, "xem2", true);
            InitAni(_dang1_1);
            SpineManager.instance.DoAnimation(_dang1_1, "XXxingxing11", true);
            InitAni(_dang2_1);
            SpineManager.instance.DoAnimation(_dang2_1, "Xxingxing11", true);

            for (int i = 0; i < _xiangjiaoAni.Length; i++)
            {
                _xiangjiaoAni[i].Show();
                _xiangjiaoAni[i].transform.GetChild(0).gameObject.Hide();
                InitAni(_xiangjiaoAni[i]);
                SpineManager.instance.DoAnimation(_xiangjiaoAni[i], "xiangjiao", true);
            }
            for (int i = 0; i < _zhadan.childCount; i++)
            {
                SpineManager.instance.DoAnimation(_zhadan.GetChild(i).gameObject, "baozha3", true);
            }
            for (int i = 0; i < _clickEmpty.Length; i++)
            {
                GameObject o = _clickEmpty[i].transform.parent.gameObject;
                if (o.name == "duan")
                {
                    InitAni(o);
                    SpineManager.instance.DoAnimation(o, "adsz2", false);
                }
                if (o.name == "hua")
                {
                    InitAni(o);
                    SpineManager.instance.DoAnimation(o, "arsz2", false);
                }
                if (o.name == "tan")
                {
                    InitAni(o);
                    SpineManager.instance.DoAnimation(o, "sz2", false);
                }
            }
        }

        private void InitPos()
        {
            _XXX.transform.position = _pos.Find("XXXPos").position;
            _bg2.transform.position = _pos.Find("BgPos").position;
            _bg2_2.transform.position = _pos.Find("BgPos").position;
            _bg2_3.transform.position = _pos.Find("BgPos").position;
            _bg2_4.transform.position = _pos.Find("BgPos").position;
            _xem.transform.position = _pos.Find("xemPos").position;
        }

        private void ClickShuEvent(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                if(obj.transform.parent.parent.name == _curPoint.ToString())
                {
                    if(_curXingXingForm == XingXingForm.XXxingxing)
                        SpineManager.instance.DoAnimation(_XXX, "XXxingxing2", false);
                    else if (_curXingXingForm == XingXingForm.Xxingxing)
                        SpineManager.instance.DoAnimation(_XXX, "Xxingxing2", false);
                    else
                        SpineManager.instance.DoAnimation(_XXX, "Daxingxing2", false);

                    float disX = Mathf.Abs(_XXX.transform.position.x - obj.transform.position.x);
                    Wait(0.1f,
                    () =>
                    {
                        if(int.Parse(obj.transform.parent.parent.name) >= 12)
                        {
                            Vector3 endPos;
                            if (int.Parse(obj.transform.parent.parent.name) == 13)
                                endPos = obj.transform.parent.parent.Find("pos").position;
                            else
                                endPos = obj.transform.position;

                            if (obj.transform.parent.name == "tan")
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                Tween tw1 = _XXX.transform.DOMove(endPos, 0.55f).OnComplete(
                                () =>
                                {
                                    TanXem(obj);
                                });
                                tw1.SetEase(Ease.Linear);
                            }
                            else
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                Tween tw1 = _XXX.transform.DOMove(endPos, 0.6f).OnComplete(
                                () =>
                                {
                                    Wait(0.1f,
                                    () =>
                                    {
                                        if (obj.transform.parent.name == "duan")
                                            FallDown(obj);
                                        else if (obj.transform.parent.name == "hua")
                                            SlipDown(obj);
                                        else if (obj.transform.parent.name == "zha")
                                            BoomDown(int.Parse(obj.transform.parent.parent.name));
                                        else
                                            TrueAction();
                                    });
                                });
                                tw1.SetEase(Ease.Linear);
                            }
                        }
                        else
                        {
                            if (int.Parse(obj.transform.parent.parent.name) == 4)
                                DangTeng1(obj, disX);
                            else if(int.Parse(obj.transform.parent.parent.name) == 9)
                                DangTeng2(obj, disX);
                            else
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                Tween tw1 = _bg2.transform.DOMoveX(_bg2.transform.position.x - disX, 0.6f).OnComplete(
                                () =>
                                {
                                    Wait(0.1f,
                                    () =>
                                    {
                                        if (obj.transform.parent.name == "duan")
                                            FallDown(obj);
                                        else if (obj.transform.parent.name == "hua")
                                            SlipDown(obj);
                                        else if (obj.transform.parent.name == "zha")
                                            BoomDown(int.Parse(obj.transform.parent.parent.name));
                                        else
                                            TrueAction();
                                    });
                                });

                                Tween tw2;
                                if (int.Parse(obj.transform.parent.parent.name) == 9)
                                    tw2 = _XXX.transform.DOMoveY(obj.transform.parent.parent.Find("pos").position.y, 0.6f);
                                else
                                    tw2 = _XXX.transform.DOMoveY(obj.transform.position.y, 0.6f);

                                Tween tw3 = _bg2_2.transform.DOMoveX(_bg2_2.transform.position.x - disX, 0.6f);
                                Tween tw4 = _bg2_3.transform.DOMoveX(_bg2_3.transform.position.x - (disX / 3), 0.6f);
                                Tween tw5 = _bg2_4.transform.DOMoveX(_bg2_4.transform.position.x - (disX * 1.5f), 0.6f);

                                tw1.SetEase(Ease.Linear);
                                tw2.SetEase(Ease.Linear);
                                tw3.SetEase(Ease.Linear);
                                tw4.SetEase(Ease.Linear);
                                tw5.SetEase(Ease.Linear);
                            }
                        }
                    });
                }
                else
                    _canClick = true;
            }
        }

        //掉落操作
        private void FallDown(GameObject obj)
        {
            if (_curXingXingForm == XingXingForm.XXxingxing)
                SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
            else if (_curXingXingForm == XingXingForm.Xxingxing)
                SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
            else
                SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);

            GameObject o = obj.transform.parent.gameObject;
            SpineManager.instance.DoAnimation(o, "adsz", false);

            Wait(0.4f, 
            () => 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9, false);
                BtnPlaySoundFail();
                _XXX.transform.DOMoveY(_XXX.transform.position.y - Screen.height, 0.8f);

                if (_curXingXingForm == XingXingForm.XXxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "XXxingxing7", false);
                else if (_curXingXingForm == XingXingForm.Xxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "Xxingxing8", false);
                else
                    SpineManager.instance.DoAnimation(_XXX, "Daxingxing4", false);
            });

            Wait(2.5f,
            () =>
            {
                SecondLevel();
            });
        }

        //滑落操作
        private void SlipDown(GameObject obj)
        {
            if (_curXingXingForm == XingXingForm.XXxingxing)
                SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
            else if (_curXingXingForm == XingXingForm.Xxingxing)
                SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
            else
                SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);

            GameObject o = obj.transform.parent.gameObject;
            SpineManager.instance.DoAnimation(o, "arsz", false);

            Wait(0.2f,
            () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9, false);
                BtnPlaySoundFail();
                _XXX.transform.DOMoveY(_XXX.transform.position.y - Screen.height, 0.8f);

                if (_curXingXingForm == XingXingForm.XXxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "XXxingxing7", false);
                else if (_curXingXingForm == XingXingForm.Xxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "Xxingxing8", false);
                else
                    SpineManager.instance.DoAnimation(_XXX, "Daxingxing4", false);
            });

            Wait(2.5f,
            () =>
            {
                SecondLevel();
            });
        }

        //炸弹操作
        private void BoomDown(int index)
        {
            if (_curXingXingForm == XingXingForm.XXxingxing)
                SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
            else if (_curXingXingForm == XingXingForm.Xxingxing)
                SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
            else
                SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);

            GameObject o = _zhadan.GetGameObject(index.ToString());
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 10, false);
            BtnPlaySoundFail();
            SpineManager.instance.DoAnimation(o, "baozha2", false, 
            ()=> 
            {
                SpineManager.instance.DoAnimation(o, "kong", false);
            });

            Wait(0.2f,
            () =>
            {
                if (_curXingXingForm == XingXingForm.Xxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "Xxingxing3", false);
                else
                    SpineManager.instance.DoAnimation(_XXX, "Daxingxing3", false);
            });

            Wait(2.5f,
            () =>
            {
                SecondLevel();
            }); ;
        }

        //弹飞小恶魔操作
        private void TanXem(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
            SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);
            _XXX.transform.DOMoveY(_XXX.transform.position.y - 100, 0.1f).OnComplete(
            () =>
            {
                _XXX.transform.DOMoveY(_XXX.transform.position.y + 150, 0.1f).OnComplete(
                ()=> 
                {
                    _XXX.transform.DOMoveY(_XXX.transform.position.y - 50, 0.3f);
                });
                
                Wait(0.1f,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11, false);
                    SpineManager.instance.DoAnimation(_xem, "xem-y", true);
                    _xem.transform.DOMove(new Vector3(_xem.transform.position.x + Screen.height, _xem.transform.position.y + Screen.height, 0), 1.0f);
                });
            });

            GameObject o = obj.transform.parent.gameObject;
            SpineManager.instance.DoAnimation(o, "sz", false,
            () =>
            {
                Wait(2.0f,
                () =>
                {
                    playSuccessSpine();
                });
            });
        }

        //正确树枝
        private void TrueAction()
        {
            if (_curPoint == 1 || _curPoint == 3 || _curPoint == 4 || _curPoint == 7 || _curPoint == 9)
            {
                //吃香蕉
                BtnPlaySoundSuccess2();
                GameObject guang = _xiangjiaoAni[_getCount].transform.GetChild(0).gameObject;
                guang.Show();
                InitAni(guang);
                SpineManager.instance.DoAnimation(guang, "guangxiao", false, 
                () => 
                {
                    _xiangjiaoAni[_getCount].Hide();
                    _getCount++;
                    _curPoint++;

                    Wait(0.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false); });
                    if (_curXingXingForm == XingXingForm.XXxingxing)
                    {
                        SpineManager.instance.DoAnimation(_XXX, "XXxingxing6", false,
                        () =>
                        {
                            if (_getCount == 3)
                            {
                                _XXXGuangxiao.transform.position = _XXX.transform.GetChild(0).position;
                                _XXXGuangxiao.Show();
                                InitAni(_XXXGuangxiao);
                                SpineManager.instance.DoAnimation(_XXXGuangxiao, "guangxiao", false, 
                                () => 
                                {
                                    _XXXGuangxiao.Hide();
                                });

                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                                SpineManager.instance.DoAnimation(_XXX, "XXxingxing9", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
                                    _curXingXingForm = XingXingForm.Xxingxing;
                                    _canClick = true;
                                });
                            }
                            else
                            {
                                SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
                                _canClick = true;
                            }
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(_XXX, "Xxingxing7", false,
                        () =>
                        {
                            if (_getCount == 5)
                            {
                                _XXXGuangxiao.transform.position = _XXX.transform.GetChild(0).position;
                                _XXXGuangxiao.Show();
                                InitAni(_XXXGuangxiao);
                                SpineManager.instance.DoAnimation(_XXXGuangxiao, "guangxiao", false,
                                () =>
                                {
                                    _XXXGuangxiao.Hide();
                                });

                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                                SpineManager.instance.DoAnimation(_XXX, "Xxingxing10", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);
                                    _curXingXingForm = XingXingForm.Daxingxing;
                                    _canClick = true;
                                });
                            }
                            else
                            {
                                SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
                                _canClick = true;
                            }
                        });
                    }
                });
            }
            else
            {
                _curPoint++;
                _canClick = true;

                if (_curXingXingForm == XingXingForm.XXxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "XXxingxing", true);
                else if (_curXingXingForm == XingXingForm.Xxingxing)
                    SpineManager.instance.DoAnimation(_XXX, "Xxingxing", true);
                else
                    SpineManager.instance.DoAnimation(_XXX, "Daxingxing", true);
            }
        }

        //荡树藤操作1
        void DangTeng1(GameObject obj, float disX)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            _XXX.transform.DOMoveY(_dangteng1.Find("pos").position.y, 0.2f);
            float newX = Mathf.Abs(_XXX.transform.position.x - _dangteng1.Find("pos").position.x);
            Tween tw1 = _bg2.transform.DOMoveX(_bg2.transform.position.x - newX, 0.2f);
            tw1.SetEase(Ease.Linear);
            Tween tw2 = _bg2_2.transform.DOMoveX(_bg2_2.transform.position.x - newX, 0.2f);
            tw2.SetEase(Ease.Linear);
            Tween tw5 = _bg2_3.transform.DOMoveX(_bg2_3.transform.position.x - (disX / 3), 1.167f);
            tw5.SetEase(Ease.Linear);
            Tween tw6 = _bg2_4.transform.DOMoveX(_bg2_4.transform.position.x - (disX * 1.5f), 1.167f);
            tw6.SetEase(Ease.Linear);

            Wait(0.2f,
            () =>
            {
                Tween tw3 = _bg2.transform.DOMoveX(_bg2.transform.position.x - (disX - newX), 0.967f);
                tw3.SetEase(Ease.Linear);
                Tween tw4 = _bg2_2.transform.DOMoveX(_bg2_2.transform.position.x - (disX - newX), 0.967f);
                tw4.SetEase(Ease.Linear);

                _XXX.Hide();
                _XXX.transform.position = new Vector2(_XXX.transform.position.x, obj.transform.position.y);
                _dang1_2.transform.position = _dangteng1.Find("pos1").position;

                SpineManager.instance.DoAnimation(_dang1_1, "XXxingxing3", false, 
                () => 
                {
                    SpineManager.instance.DoAnimation(_dang1_1, "XXxingxing4", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_dang1_1, "XXxingxing11", true);
                    });
                    _dang1_2.Show();
                    InitAni(_dang1_2);
                    SpineManager.instance.DoAnimation(_dang1_2, "XXxingxing5", false);
                    _dang1_2.transform.DOLocalMove(obj.transform.parent.name == "hua" ? _dangteng1.Find("pos3").localPosition : _dangteng1.Find("pos2").localPosition, 0.6f).OnComplete(
                    () => 
                    {
                        Wait(0.02f, () => { _dang1_2.Hide(); });
                        _XXX.Show();
                        InitAni(_XXX);
                        SpineManager.instance.DoAnimation(_XXX, "XXxingxing", false);

                        Wait(0.1f,
                        () =>
                        {
                            if (obj.transform.parent.name == "hua")
                                SlipDown(obj);
                            else
                                TrueAction();
                        });
                    });
                });
            });
        }

        //荡树藤操作2
        void DangTeng2(GameObject obj, float disX)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            _XXX.transform.DOMoveY(_dangteng2.Find("pos").position.y, 0.2f);
            float newX = Mathf.Abs(_XXX.transform.position.x - _dangteng2.Find("pos").position.x);
            Tween tw1 = _bg2.transform.DOMoveX(_bg2.transform.position.x - newX, 0.2f);
            tw1.SetEase(Ease.Linear);
            Tween tw2 = _bg2_2.transform.DOMoveX(_bg2_2.transform.position.x - newX, 0.2f);
            tw2.SetEase(Ease.Linear);
            Tween tw5 = _bg2_3.transform.DOMoveX(_bg2_3.transform.position.x - (disX / 3), 1.167f);
            tw5.SetEase(Ease.Linear);
            Tween tw6 = _bg2_4.transform.DOMoveX(_bg2_4.transform.position.x - (disX * 1.5f), 1.167f);
            tw6.SetEase(Ease.Linear);

            Wait(0.2f,
            () =>
            {
                Tween tw3 = _bg2.transform.DOMoveX(_bg2.transform.position.x - (disX - newX), 0.967f);
                tw3.SetEase(Ease.Linear);
                Tween tw4 = _bg2_2.transform.DOMoveX(_bg2_2.transform.position.x - (disX - newX), 0.967f);
                tw4.SetEase(Ease.Linear);

                _XXX.Hide();
                _XXX.transform.position = new Vector2(_XXX.transform.position.x, obj.transform.parent.parent.GetGameObject("pos").transform.position.y);
                _dang2_2.transform.position = _dangteng2.Find("pos1").position;

                SpineManager.instance.DoAnimation(_dang2_1, "Xxingxing4", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_dang2_1, "Xxingxing5", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_dang2_1, "Xxingxing11", true);
                    });
                    _dang2_2.Show();
                    InitAni(_dang2_2);
                    SpineManager.instance.DoAnimation(_dang2_2, "Xxingxing6", false);
                    _dang2_2.transform.DOLocalMove(_dangteng2.Find("pos2").localPosition, 0.6f).OnComplete(
                    () =>
                    {
                        Wait(0.02f, () => { _dang2_2.Hide(); });
                        _XXX.Show();
                        InitAni(_XXX);
                        SpineManager.instance.DoAnimation(_XXX, "Xxingxing", false);

                        Wait(0.1f,
                        () =>
                        {
                            TrueAction();
                        });
                    });
                });
            });
        }
        #endregion
    }
}