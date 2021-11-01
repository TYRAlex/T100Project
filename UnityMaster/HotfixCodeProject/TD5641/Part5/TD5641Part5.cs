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

    public class TD5641Part5
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

        private GameObject _start;
        private GameObject _time;
        private GameObject _first;
        private GameObject _area;
        private GameObject _area2;

        private GameObject _yun;
        private GameObject _shu;
        private GameObject _yun2;
        private GameObject _shu2;
        private GameObject _flag1;
        private GameObject _flag2;

        private GameObject[] _life;
        private GameObject _tn;
        private GameObject _fk1;
        private GameObject _fk2;
        private GameObject _fk3;
        private GameObject[] _fk;
        private GameObject _click;
        private GameObject _photo;
        private Transform _pos;
        private GameObject _false;
        private MonoScripts _monoScripts;
        private EventDispatcher _tnEvent;

        private bool _canClick;
        private bool _canMove;
        private bool _getFK;
        private bool _isEnd = false;
        private float _speed;
        private int _count;
        private float _screenRatio;

        private Transform _panel;
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
            _panel = curTrans.Find("panel");
            _start = curTrans.GetGameObject("Start");
            _time = curTrans.GetGameObject("Start/time");
            _first = _panel.GetGameObject("First");
            _area = _panel.GetGameObject("Area");
            _area2 = _panel.GetGameObject("Area2");
            _yun = _panel.GetGameObject("yun");
            _shu = _panel.GetGameObject("shu");
            _yun2 = _panel.GetGameObject("yun2");
            _shu2 = _panel.GetGameObject("shu2");
            _flag1 = curTrans.GetGameObject("flag1");
            _flag2 = curTrans.GetGameObject("flag2");
            _life = new GameObject[curTrans.Find("Life").childCount];
            for (int i = 0; i < curTrans.Find("Life").childCount; i++)
            {
                _life[i] = curTrans.Find("Life").GetChild(i).gameObject;
            }
            _tn = curTrans.GetGameObject("tn");

            _click = curTrans.GetGameObject("Click");
            Util.AddBtnClick(_click, ClickEvent);
            _monoScripts = _click.GetComponent<MonoScripts>();
            _monoScripts.FixedUpdateCallBack = sFixedUpdate;

            _fk1 = curTrans.GetGameObject("fk1");
            _fk2 = curTrans.GetGameObject("fk2");
            _fk3 = curTrans.GetGameObject("fk3");
            _fk = new GameObject[3];
            _fk[0] = curTrans.Find("fk1/fk/0").gameObject;
            _fk[1] = curTrans.Find("fk2/fk/0").gameObject;
            _fk[2] = curTrans.Find("fk3/fk/0").gameObject;

            _photo = curTrans.GetGameObject("photo");
            _pos = curTrans.Find("Pos");
            _false = curTrans.GetGameObject("false");

            _tnEvent = _tn.transform.GetComponent<EventDispatcher>(); 
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _screenRatio = Screen.width / 1920f;
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
            _count = 0;
            _speed = 450f;
            _canClickBtn = true;
            _getFK = false;

            if (_isEnd)
            {
                _canMove = true;
                _canClick = true;

                _start.Hide();

                for (int i = 0; i < _life.Length; i++)
                {
                    GameObject o = _life[i];
                    o.Show();
                    o.transform.GetChild(0).gameObject.Hide();
                }
                SpineManager.instance.DoAnimation(_tn, "tn-P" + (_count == 0 ? null : (_count + 1).ToString()), true);
            }
            else
            {
                _canMove = false;
                _canClick = false;

                _start.Show();
                _time.Hide();

                for (int i = 0; i < _life.Length; i++)
                {
                    GameObject o = _life[i];
                    WaitTimeAndExcuteNext(Random.Range(1, 5) * 0.1f,
                    () =>
                    {
                        o.Show();
                        SpineManager.instance.DoAnimation(o, "xem", true);
                        o.transform.GetChild(0).gameObject.Hide();
                    });
                }
                SpineManager.instance.DoAnimation(_tn, "tn", true);
            }

            _fk1.Show();
            _first.Show();
            _photo.Hide();
            _false.Hide();
            InitPos();

            UpdateRock(_panel.Find("First/rock"));
            UpdateRock(_panel.Find("Area/rock"));
            UpdateRock(_panel.Find("Area2/rock"));
            UpdateFK();

            _tnEvent.TriggerEnter2D -= TriggerEvnet;
            _tnEvent.TriggerEnter2D += TriggerEvnet;
            _tnEvent.TriggerStay2D -= TriggerEvnet;
            _tnEvent.TriggerStay2D += TriggerEvnet;
            _isEnd = false;
        }

        void InitPos()
        {
            SetPos(_tn.transform, _pos.Find("tnPos").position);

            SetPos(_shu.transform, new Vector2(0, 0));
            SetPos(_yun.transform, new Vector2(0, 0));
            SetPos(_shu2.transform, _pos.Find("yunPos").position);
            SetPos(_yun2.transform, _pos.Find("yunPos").position);
            SetPos(_flag1.transform, _pos.Find("flag1").position);
            SetPos(_flag2.transform, _pos.Find("flag2").position);

            SetPos(_first.transform, new Vector2(0, 0));
            SetPos(_area.transform, _pos.Find("AreaPos").position);
            SetPos(_area2.transform, _pos.Find("AreaPos2").position);
            SetPos(_fk1.transform, new Vector2(0, 0));
            SetPos(_fk2.transform, _pos.Find("AreaPos").position);
            SetPos(_fk3.transform, _pos.Find("AreaPos2").position);
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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
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
                    mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    {
                        tt.SetActive(false);
                        mask.SetActive(false);
                        _time.Show();
                        InitAni(_time);
                        SpineManager.instance.DoAnimation(_time, "z0", false, 
                        () => 
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(_time, "z1", false, 
                            ()=> 
                            {
                                _start.Hide();
                                _canClick = true;
                                _canMove = true;
                                SpineManager.instance.DoAnimation(_tn, "tn-P" + (_count == 0 ? null : (_count + 1).ToString()), true);
                            });
                        });
                    }));
                    break;
            }

            talkIndex++;
        }

        void TDGameStartFunc()
        {
            tt.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 0, null,
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(1, 4), false);
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); _isEnd = true; GameInit(); });
                    }
                    else if (obj.name == "next")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); });
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
                            ChangeClickArea();
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion

        #region 游戏相关
        void UpdateFK()
        {
            for (int i = 0; i < _fk.Length; i++)
            {
                GameObject o = _fk[i];
                o.GetComponent<BoxCollider2D>().enabled = true;
                SpineManager.instance.DoAnimation(o, "fk" + (_count == 0 ? null : (2 * _count + 1).ToString()), false);
            }
        }

        void UpdateRock(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject rock = parent.GetChild(i).gameObject;
                rock.Show();
                int ran = Random.Range(0, 6);
                rock.GetComponent<RawImage>().texture = rock.GetComponent<BellSprites>().texture[ran];
                rock.GetComponent<BoxCollider2D>().enabled = true;
            }
        }

        private void TriggerEvnet(Collider2D other, int time)
        {
            if (other.transform.parent.name == "rock")
            {
                mono.StopAllCoroutines();
                DOTween.KillAll();
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                _canMove = false;
                _canClick = false;

                if(_tn.transform.position.y != curTrans.Find("Pos/tnPos").position.y)
                    _tn.transform.DOMoveY(curTrans.Find("Pos/tnPos").position.y, 0.1f);

                BtnPlaySoundFail();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                InitAni(_tn);
                SpineManager.instance.DoAnimation(_tn, "tn-Y" + (_count == 0 ? null : (_count + 1).ToString()), true);
                WaitTimeAndExcuteNext(1.5f,
                () =>
                {
                    _false.Show();
                    InitAni(_false);
                    SpineManager.instance.DoAnimation(_false, "paizi", false);
                    WaitTimeAndExcuteNext(1.5f,
                    () =>
                    {
                        _isEnd = true;
                        GameInit();
                    });
                });
            }
            else
            {
                _count++;
                other.gameObject.GetComponent<BoxCollider2D>().enabled = false;

                _getFK = true;
                _canClick = false;

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                for (int i = 0; i < _fk.Length; i++)
                {
                    SpineManager.instance.DoAnimation(_fk[i], "fk" + (_count * 2).ToString(), false);
                }

                if (_count == 3)
                {
                    Wait(0.5f,
                    () =>
                    {
                        _canMove = false;
                    });

                    Wait(1.8f,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        GameObject o = _life[_count - 1].transform.GetGameObject("boom");
                        o.Show();
                        SpineManager.instance.DoAnimation(o, "xem-boom", false,
                        () =>
                        {
                            _life[_count - 1].Hide();
                            WaitTimeAndExcuteNext(0.8f,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                _photo.Show();
                                InitAni(_photo);
                                SpineManager.instance.DoAnimation(_photo, "cam", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_photo, "photo", false,
                                    () =>
                                    {
                                        WaitTimeAndExcuteNext(2.0f, () => { playSuccessSpine(); });
                                    });
                                });
                            });
                        });
                    });
                }
                else
                {
                    Wait(0.5f,
                    () =>
                    {
                        _canMove = false;
                    });

                    Wait(1.8f,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        GameObject o = _life[_count - 1].transform.GetGameObject("boom");
                        o.Show();
                        SpineManager.instance.DoAnimation(o, "xem-boom", false,
                        () =>
                        {
                            o.transform.parent.gameObject.Hide();
                            SpineManager.instance.DoAnimation(_tn, "tn-P" + (_count == 0 ? null : (_count + 1).ToString()), true);
                            _canClick = true;
                            _canMove = true;
                        });
                    });
                }
            }
        }

        private void sFixedUpdate()
        {
            if(_canMove)
            {
                _first.transform.position = Vector3.Lerp(_first.transform.position, new Vector3(_first.transform.position.x - _speed, 0, 0), Time.deltaTime);
                _area.transform.position = Vector3.Lerp(_area.transform.position, new Vector3(_area.transform.position.x - _speed, 0, 0), Time.deltaTime);
                _area2.transform.position = Vector3.Lerp(_area2.transform.position, new Vector3(_area2.transform.position.x - _speed, 0, 0), Time.deltaTime);

                _fk1.transform.position = Vector3.Lerp(_fk1.transform.position, new Vector3(_fk1.transform.position.x - _speed, 0, 0), Time.deltaTime);
                _fk2.transform.position = Vector3.Lerp(_fk2.transform.position, new Vector3(_fk2.transform.position.x - _speed, 0, 0), Time.deltaTime);
                _fk3.transform.position = Vector3.Lerp(_fk3.transform.position, new Vector3(_fk3.transform.position.x - _speed, 0, 0), Time.deltaTime);

                _flag1.transform.position = Vector3.Lerp(_flag1.transform.position, new Vector3(_flag1.transform.position.x - _speed, _flag1.transform.position.y, 0), Time.deltaTime);
                _flag2.transform.position = Vector3.Lerp(_flag2.transform.position, new Vector3(_flag2.transform.position.x - _speed, _flag2.transform.position.y, 0), Time.deltaTime);
                _shu.transform.position = Vector3.Lerp(_shu.transform.position, new Vector3(_shu.transform.position.x - _speed / 2, 0, 0), Time.deltaTime);
                _yun.transform.position = Vector3.Lerp(_yun.transform.position, new Vector3(_yun.transform.position.x - _speed / 6, 0, 0), Time.deltaTime);
                _shu2.transform.position = Vector3.Lerp(_shu2.transform.position, new Vector3(_shu2.transform.position.x - _speed / 2, 0, 0), Time.deltaTime);
                _yun2.transform.position = Vector3.Lerp(_yun2.transform.position, new Vector3(_yun2.transform.position.x - _speed / 6, 0, 0), Time.deltaTime);
            }

            if (_shu.transform.position.x <= -_shu.GetComponent<RectTransform>().rect.width)
            {
                _shu.transform.position = new Vector2(_shu2.transform.position.x + _shu2.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }
            if (_yun.transform.position.x <= -_yun.GetComponent<RectTransform>().rect.width)
            {
                _yun.transform.position = new Vector2(_yun2.transform.position.x + _yun2.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }
            if (_shu2.transform.position.x <= -_shu2.GetComponent<RectTransform>().rect.width)
            {
                _shu2.transform.position = new Vector2(_shu.transform.position.x + _shu.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }
            if (_yun2.transform.position.x <= -_yun2.GetComponent<RectTransform>().rect.width)
            {
                _yun2.transform.position = new Vector2(_yun.transform.position.x + _yun.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }

            if (_first.transform.position.x <= -_first.GetComponent<RectTransform>().rect.width && _first.activeSelf)
            {
                _first.Hide();
                UpdateFK();
            }
            if (_area.transform.position.x <= -_area.GetComponent<RectTransform>().rect.width)
            {
                _area.transform.position = new Vector2(_area2.transform.position.x + _area2.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
                UpdateRock(_panel.Find("Area/rock"));
                UpdateFK();
            }
            if (_area2.transform.position.x <= -_area2.GetComponent<RectTransform>().rect.width)
            {
                _area2.transform.position = new Vector2(_area.transform.position.x + _area.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
                UpdateRock(_panel.Find("Area2/rock"));
                UpdateFK();
            }

            if (_fk1.transform.position.x <= -_fk1.GetComponent<RectTransform>().rect.width && _fk1.activeSelf)
            {
                _fk1.Hide();
            }
            if (_fk2.transform.position.x <= -_fk2.GetComponent<RectTransform>().rect.width)
            {
                _fk2.transform.position = new Vector2(_fk3.transform.position.x + _fk3.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }
            if (_fk3.transform.position.x <= -_fk3.GetComponent<RectTransform>().rect.width)
            {
                _fk3.transform.position = new Vector2(_fk2.transform.position.x + _fk2.GetComponent<RectTransform>().rect.width * _screenRatio, 0);
            }
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Tween tw1 = _tn.transform.DOMoveY(_tn.transform.position.y + Screen.height / 5, 0.933f / 2f).OnComplete(
                () =>
                {
                    Tween tw2 = _tn.transform.DOMoveY(_tn.transform.position.y - Screen.height / 5, 0.933f / 2f);
                    tw2.SetEase(Ease.Linear);
                });
                tw1.SetEase(Ease.InOutSine);

                SpineManager.instance.DoAnimation(_tn, "tn-J" + (_count == 0 ? null : (_count + 1).ToString()), false,
                () =>
                {
                    if(_getFK)
                    {
                        _getFK = false;
                        SpineManager.instance.DoAnimation(_tn, "tn" + (_count + 1).ToString(), true);
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(_tn, "tn-P" + (_count == 0 ? null : (_count + 1).ToString()), true);
                        _canClick = true;
                    }    
                });
            }
        }
        #endregion

    }
}
