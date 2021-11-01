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
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}

    public enum DirectionType
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class TD6765Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;		
	    
        private GameObject _dDD;
	    private GameObject _sDD;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        #region 游戏变量
        private GameObject _point;  //得分
        private GameObject _xem;
        public GameObject _shine;
        private GameObject _sPumpkin;
        private GameObject _dPumpkin;
        private GameObject _sPumpkinAni;
        private RectTransform _rockerBg;
        private RectTransform _rockerBar;
        private Transform _trigger;
        private Transform _boom;
        private Transform _mashAni;
        private Transform _anotherPar;
        private Transform _allPosition;

        private DirectionType _curDirectionType;
        private float _barRadius;   //摇杆半径
        private float _barX;
        private float _barY;
        private bool _canDrag;
        private bool _canMove;
        private int _moveSpeed;
        private int _boomSpeed;
        private bool _canPlayAni;   //能否循环播放南瓜动画
        private int _getPoint;
        private int _scrollNum1;
        private int _scrollNum2;
        private int _scrollNum3;
        private string _closeMouseAni;
        private string _openMouseAni;
        private string _stayAni;
        #endregion

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");			
									
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");
			
			_dDD = curTrans.GetGameObject("dDD");
         	_sDD = curTrans.GetGameObject("sDD");

            _point = curTrans.GetGameObject("BG/Point");
            _xem = curTrans.GetGameObject("BG/Xem");
            _shine = curTrans.GetGameObject("BG/Shine");
            _sPumpkin = curTrans.GetGameObject("BG/sPumpkin");
            _dPumpkin = curTrans.GetGameObject("BG/dPumpkin");
            _sPumpkinAni = curTrans.GetGameObject("BG/sPumpkin/PumpkinAni");
            _trigger = curTrans.Find("BG/Trigger");
            _boom = curTrans.Find("BG/Boom");
            _mashAni = curTrans.Find("BG/MashAni");
            _anotherPar = curTrans.Find("BG/Parent");
            _allPosition = curTrans.Find("BG/AllPosition");
            _rockerBg = curTrans.GetGameObject("BG/RockerBg").GetComponent<RectTransform>();
            _rockerBar = curTrans.GetGameObject("BG/RockerBg/RockerBar").GetComponent<RectTransform>();

            _sPumpkin.GetComponent<EventDispatcher>().CollisionEnter2D -= OnCollision;
            _sPumpkin.GetComponent<EventDispatcher>().CollisionEnter2D += OnCollision;
            _sPumpkin.GetComponent<EventDispatcher>().TriggerEnter2D -= OnTriggerBoom;
            _sPumpkin.GetComponent<EventDispatcher>().TriggerEnter2D += OnTriggerBoom;
            _sPumpkinAni.GetComponent<EventDispatcher>().TriggerEnter2D -= OnTrigger;
            _sPumpkinAni.GetComponent<EventDispatcher>().TriggerEnter2D += OnTrigger;

            GameInit();
            GameStart();
        }

        void Update()
        {
            //摇杆不可超过摇杆台的大小
            if (_rockerBar.anchoredPosition.magnitude > _barRadius)
                _rockerBar.anchoredPosition = _rockerBar.anchoredPosition.normalized * _barRadius;

            _barX = _rockerBar.anchoredPosition.x;
            _barY = _rockerBar.anchoredPosition.y;

            if ((Mathf.Abs(_barX) >= 2 || (Mathf.Abs(_barY) >= 2)) && _canDrag)
            {
                if (_barX > 0)
                {
                    if (_barY > 0)
                    {
                        if (Mathf.Abs(_barX) >= Mathf.Abs(_barY))
                            ChangeDirection(DirectionType.Right);
                        else
                            ChangeDirection(DirectionType.Up);
                    }
                    else
                    {
                        if (Mathf.Abs(_barX) >= Mathf.Abs(_barY))
                            ChangeDirection(DirectionType.Right);
                        else
                            ChangeDirection(DirectionType.Down);
                    }
                }
                else
                {
                    if (_barY > 0)
                    {
                        if (Mathf.Abs(_barX) >= Mathf.Abs(_barY))
                            ChangeDirection(DirectionType.Left);
                        else
                            ChangeDirection(DirectionType.Up);
                    }
                    else
                    {
                        if (Mathf.Abs(_barX) >= Mathf.Abs(_barY))
                            ChangeDirection(DirectionType.Left);
                        else
                            ChangeDirection(DirectionType.Down);
                    }
                }
            }
        }

        void FixedUpdate()
        {
            PumpkinMove();

            if (_canDrag)
                BoomMove();
        }

        void InitData()
        {
            _succeedSoundIds = new List<int> { 5, 6, 7, 9 };
            _failSoundIds = new List<int> { 1, 2, 3 };
		    
            _barRadius = _rockerBg.sizeDelta.x * 0.5f;
            _moveSpeed = 150;
            _boomSpeed = 2;
            _getPoint = 0;
            _scrollNum1 = 0;
            _scrollNum2 = 0;
            _scrollNum3 = 0;
            _openMouseAni = "nangua2";
            _closeMouseAni = "nangua";
            _stayAni = "nangua7";

            _curDirectionType = DirectionType.Right;
            _canMove = false;
            _canDrag = false;
            _canPlayAni = false;
        }

        void GameInit()
        {
            ResetPar();
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Input.multiTouchEnabled = false;
            StopAllAudio(); 
			StopAllCoroutines();
            

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
		     _dDD.Hide(); 
			 _sDD.Hide(); 
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            #region 游戏初始化
            //位置、大小、旋转
            SetPos(_sPumpkin.transform.GetRectTransform(), _allPosition.Find("sPumpkinPos").GetRectTransform().anchoredPosition);
            _sPumpkinAni.transform.localScale = new Vector3(1, 1, 0);
            _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, 0);

            //显示、动画
            for (int i = 0; i < _trigger.childCount; i++)
            {
                Transform trans = _trigger.GetChild(i);
                for (int j = 0; j < trans.childCount; j++)
                {
                    trans.GetChild(j).gameObject.Show();
                }
            }
            for (int i = 0; i < _mashAni.childCount; i++)
            {
                _mashAni.GetChild(i).gameObject.Show();
                string name = "h" + _mashAni.GetChild(i).name;
                SetPos(_mashAni.GetChild(i).GetRectTransform(), _allPosition.Find(name).GetRectTransform().anchoredPosition);
                InitSpine(_mashAni.GetChild(i).gameObject);
                PlaySpine(_mashAni.GetChild(i).gameObject, name, null, true);
                _mashAni.GetChild(i).localScale = new Vector3(1, 1, 1);
            } 
            StartMashAni();
            for (int i = 0; i < _boom.childCount; i++)
            {
                _boom.GetChild(i).GetComponent<Scrollbar>().value = 0;
                _boom.GetChild(i).GetChild(0).gameObject.Show();
                SpineManager.instance.ClearTrack(_boom.GetChild(i).GetChild(0).gameObject);
                InitSpine(_boom.GetChild(i).GetChild(0).gameObject);
                PlaySpine(_boom.GetChild(i).GetChild(0).gameObject, "zd", null, true);
            }

            _dPumpkin.Hide();
            _shine.Hide();
            _rockerBar.transform.GetComponent<RawImage>().raycastTarget = true;

            _boom.GetChild(0).gameObject.Show();
            _boom.GetChild(1).gameObject.Hide();
            _boom.GetChild(2).gameObject.Hide();

            InitSpine(_xem);
            PlaySpine(_xem, "xem1", null, true);
            SpineManager.instance.ClearTrack(_point);
            InitSpine(_point);
            PlaySpine(_point, "0", null, false);
            PlaySpine(_sPumpkinAni, "nangua7", null, true);
            #endregion
        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);
                        _startSpine.Hide();
                        _sDD.Show(); 
                        BellSpeck(_sDD, 0, null, 
                        ()=> 
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        });
                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () => { StartGame(); });
                    break;                                
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _dDD.Hide();
            _sDD.Hide();

            _canDrag = true;
            _canMove = true;
            _canPlayAni = true;

            _boomSpeed = 2;
            StartPumpkinAni();
        }

        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
			_okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayBgm(0);
                        GameInit();       
                        
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();

                        _dDD.Show(); BellSpeck(_dDD, 2);						
                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);			
			 PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });         
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

        void ChangeDirection(DirectionType directionType)
        {
            if(_curDirectionType != directionType)
            {
                _curDirectionType = directionType;
                if (_curDirectionType == DirectionType.Right)
                {
                    _sPumpkinAni.transform.localScale = new Vector3(1, 1, 0);
                    _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                if (_curDirectionType == DirectionType.Left)
                {
                    _sPumpkinAni.transform.localScale = new Vector3(-1, 1, 0);
                    _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, 0);
                }
                if (_curDirectionType == DirectionType.Up)
                {
                    _sPumpkinAni.transform.localScale = new Vector3(-1, 1, 0);
                    _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, -90);
                }
                if (_curDirectionType == DirectionType.Down)
                {
                    _sPumpkinAni.transform.localScale = new Vector3(-1, 1, 0);
                    _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, 90);
                }

                _canMove = true;
            }
        }

        //南瓜移动
        void PumpkinMove()
        {
            if(_canMove)
            {
                if (_curDirectionType == DirectionType.Right)
                    _sPumpkin.transform.GetComponent<Rigidbody2D>().velocity = _sPumpkin.transform.right * _moveSpeed;
                if (_curDirectionType == DirectionType.Left)
                    _sPumpkin.transform.GetComponent<Rigidbody2D>().velocity = -_sPumpkin.transform.right * _moveSpeed;
                if (_curDirectionType == DirectionType.Up)
                    _sPumpkin.transform.GetComponent<Rigidbody2D>().velocity = _sPumpkin.transform.up * _moveSpeed;
                if (_curDirectionType == DirectionType.Down)
                    _sPumpkin.transform.GetComponent<Rigidbody2D>().velocity = -_sPumpkin.transform.up * _moveSpeed;
            }
            else
                _sPumpkin.transform.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        }
        
        //炸弹移动
        void BoomMove()
        {
            if (_boom.GetChild(0).gameObject.activeSelf)
                _boom.GetChild(0).GetComponent<Scrollbar>().value = Mathf.PingPong(_scrollNum1 += _boomSpeed, 2000) / 2000;
            if (_boom.GetChild(1).gameObject.activeSelf)
                _boom.GetChild(1).GetComponent<Scrollbar>().value = Mathf.PingPong(_scrollNum2 += _boomSpeed, 2000) / 2000;
            if (_boom.GetChild(2).gameObject.activeSelf)
                _boom.GetChild(2).GetComponent<Scrollbar>().value = Mathf.PingPong(_scrollNum3 += _boomSpeed, 2000) / 2000;
        }

        private void StartMashAni()
        {
            _mono.StartCoroutine(IEDelayAni(_mashAni, 0.1f));
        }

        IEnumerator IEDelayAni(Transform parent, float delay)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var obj = parent.GetChild(i).gameObject;
                string name = "h" + obj.name;
                PlaySpine(obj, name, null, true);
                yield return new WaitForSeconds(delay);
            }
        }

        private void StartPumpkinAni()
        {
            PlaySpine(_sPumpkinAni, _openMouseAni,
            () => 
            {
                if (_canPlayAni)
                    Delay(0.2f,
                    () =>
                    {
                        if (_canPlayAni)
                        {
                            PlaySound(0, false);
                            PlaySpine(_sPumpkinAni, _closeMouseAni,
                            () =>
                            {
                                Delay(0.2f,
                                () =>
                                {
                                    StartPumpkinAni();
                                });
                            }, false);
                        }
                        else
                            PlaySpine(_sPumpkinAni, _stayAni, null, true);
                    });
                else
                    PlaySpine(_sPumpkinAni, _stayAni, null, true);
            }, false);
        }

        //触发器委托
        private void OnTrigger(Collider2D other, int time)
        {
            //碰到花与蘑菇
            if(other.transform.parent.name == "other")
            {
                _canMove = false;
                _canDrag = false;
                _canPlayAni = false;
                PlaySuccessSound();
                PlaySound(1, false);
                string name = other.transform.name;
                other.gameObject.Hide();
                PlayMashAni(name);
            }
            else if(other.name == "BoomTri")
            {
                return;
            }
            else if(other.transform.parent.name == "Wall")
            {
                //穿墙bug
                Debug.LogError("我撞墙了，但我不让你吃，也不让你穿，气不气！");
                _canMove = false;
                return;
            }
            else
            {
                other.gameObject.Hide();
            }
        }

        //炸弹触发器委托
        private void OnTriggerBoom(Collider2D other, int time)
        {
            if (other.name == "BoomTri")
            {
                _canPlayAni = false;
                _canMove = false;
                _canDrag = false;
                PlayFailSound();
                PlaySound(3, false);
                PlaySpine(other.gameObject, "zd2", 
                ()=> 
                {
                    SetPos(_sPumpkin.transform.GetRectTransform(), _allPosition.Find("sPumpkinPos").GetRectTransform().anchoredPosition);
                    _sPumpkinAni.transform.localScale = new Vector3(1, 1, 0);
                    _sPumpkinAni.transform.localEulerAngles = new Vector3(0, 0, 0);
                    _curDirectionType = DirectionType.Right;
                    other.transform.parent.GetComponent<Scrollbar>().value = 0;

                    if (other.transform.parent.name == "Boom1")
                        _scrollNum1 = 0;
                    else if (other.transform.parent.name == "Boom2")
                        _scrollNum2 = 0;
                    else
                        _scrollNum3 = 0;

                    _canMove = true;
                    _canDrag = true;
                    _canPlayAni = true;
                    PlaySpine(other.gameObject, "zd", null, true);
                    StartPumpkinAni();
                }, false);
            }
        }

        //碰撞器委托
        private void OnCollision(Collision2D other, int time)
        {
            //碰到墙体停止移动
            if(other.transform.parent.name == "Wall")
                _canMove = false;
        }

        void ResetPar()
        {
            int index = _anotherPar.childCount;
            for (int i = 0; i < index; i++)
            {
                _anotherPar.GetChild(0).SetParent(_mashAni);
            }
        }

        //拿花得分效果
        void PlayMashAni(string name)
        {
            string aniName = null;
            //播放花变大动画
            switch (name)
            {
                case "1":
                case "2":
                case "3":
                    aniName = "animation4";
                    break;
                case "4":
                case "7":
                    aniName = "animation3";
                    break;
                case "5":
                case "6":
                    aniName = "animation2";
                    break;
                default:
                    break;
            }
            _mashAni.Find(name).SetParent(_anotherPar);
            PlaySpine(_anotherPar.Find(name).gameObject, aniName, null, false);

            _anotherPar.Find(name).transform.DOMove(_shine.transform.position, 0.8f).OnComplete(
            () =>
            {
                _shine.Show();
                InitSpine(_shine);

                PlaySound(5, false);
                PlaySpine(_shine, "animation",
                () =>
                {
                    PlaySound(2, false);
                    _shine.Hide();
                    _anotherPar.Find(name).DOMove(_point.transform.position, 0.7f);
                    _anotherPar.Find(name).DOScale(new Vector3(0, 0, 0), 0.7f).OnComplete(
                    () =>
                    {
                        _getPoint += 1;

                        //调整物体移动速度与动画
                        if (_getPoint >= 4)
                        {
                            _openMouseAni = "nangua5";
                            _closeMouseAni = "nangua6";
                            _stayAni = "nangua9";
                            _boomSpeed = 4;
                            _moveSpeed = 250;
                            if (!_boom.GetChild(2).gameObject.activeSelf)
                                _boom.GetChild(2).gameObject.Show();
                        }
                        else if (_getPoint >= 2)
                        {
                            _openMouseAni = "nangua3";
                            _closeMouseAni = "nangua4";
                            _stayAni = "nangua8";
                            _boomSpeed = 3;
                            _moveSpeed = 200;
                            if (!_boom.GetChild(1).gameObject.activeSelf)
                                _boom.GetChild(1).gameObject.Show();
                        }
                        else
                        {
                            _openMouseAni = "nangua2";
                            _closeMouseAni = "nangua";
                            _stayAni = "nangua7";
                            _boomSpeed = 2;
                            _moveSpeed = 150;
                        }

                        PlaySpine(_point, _getPoint.ToString(),
                        () =>
                        {
                            //游戏胜利
                            if (_getPoint == 7)
                            {
                                DPumpkinAni();
                            }
                            else
                            {
                                _canMove = true;
                                _canDrag = true;
                                _canPlayAni = true;
                                StartPumpkinAni();
                            }
                        }, false);
                    });
                }, false);
            });
        }

        //大南瓜砸小恶魔效果
        void DPumpkinAni()
        {
            _dPumpkin.Show();
            InitSpine(_dPumpkin);
            PlaySound(5, false);
            PlaySpine(_dPumpkin, "danangua",
            () =>
            {
                PlaySound(4, false);
                PlaySpine(_dPumpkin, "danangua2",
                () =>
                {
                    PlaySpine(_dPumpkin, "danangua3", null, false);
                    PlaySpine(_xem, "xem-y",
                    () =>
                    {
                        PlaySpine(_xem, "xem-y2", null, true);
                        Delay(2.0f, () => { GameSuccess(); });
                    }, false);
                }, false);
            }, false);
        }
        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
        }


        #endregion

        #region Spine相关

        private void InitSpine(GameObject obj)
        {
            obj.GetComponent<SkeletonGraphic>().Initialize(true);
        }

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Adult, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
			{
				case RoleType.Bd:
				     daiJi = "bd-daiji"; speak = "bd-speak";
				break;
				case RoleType.Xem:
				     daiJi = "daiji"; speak = "speak";
				break;
				case RoleType.Child:
				     daiJi = "animation"; speak = "animation2";
				 break;
				case RoleType.Adult:
				     daiJi = "daiji"; speak = "speak";
				break;
			}				
						 
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

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

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
