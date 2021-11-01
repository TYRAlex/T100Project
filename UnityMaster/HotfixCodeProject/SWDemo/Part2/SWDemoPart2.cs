using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework.Scripts.Utility;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    enum InputType
    {
        Null,
        Straight,
        Left,
        Right,
    }

    enum MaxDirection
    {
        front,
        back,
        left,
        right,
    }

    enum KongLongDirection
    {
        front,
        back,
        left,
        right,
    }

    public class SWDemoPart2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private Transform _ani1;
        private GameObject _left;
        private GameObject _right;
        private GameObject _max;
        private GameObject _maxZ;
        private MaxDirection _curDirection;

        private Transform _ani2;
        private GameObject _startBtn;

        private Transform _ani3;
        private GameObject _clickZhiXing;
        private Transform _block;

        private Transform _konglong;
        private Transform _lan;
        private Transform _hei;
        private Transform _hong;
        private Transform _lv;
        private KongLongDirection _lanDirection;
        private KongLongDirection _heiDirection;
        private KongLongDirection _hongDirection;
        private KongLongDirection _lvDirection;
        private List<InputType> _lanInput;
        private List<InputType> _heiInput;
        private List<InputType> _hongInput;
        private List<InputType> _lvInput;
        private Dictionary<string, bool> _isDinosuarWalkingDic;

        private GameObject _endUI;
        private int _row;
        private int _col;
        private bool _canClick;
        private bool _cando;
        private GameObject _mask;
        private bool _isPlaying;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _mask = curTrans.Find("mask").gameObject;

            bell = curTrans.Find("bell").gameObject;

            _ani1 = curTrans.Find("Ani1");
            _left = _ani1.GetGameObject("anb/click");
            _right= _ani1.GetGameObject("ana/click");
            Util.AddBtnClick(_left, LeftChange);
            Util.AddBtnClick(_right, RightChange);
            _max = _ani1.GetGameObject("max");
            _maxZ = _ani1.GetGameObject("maxz");

            _ani2 = curTrans.Find("Ani2");
            _startBtn = _ani2.GetGameObject("startBtn");

            _ani3 = curTrans.Find("Ani3");
            _clickZhiXing = _ani3.GetGameObject("anb/click");
            Util.AddBtnClick(_clickZhiXing, ClickZhiXing);
            _block = _ani3.Find("Block");

            _konglong = curTrans.Find("konglong");
            _lan = _konglong.Find("lan");
            _hei = _konglong.Find("hei");
            _hong = _konglong.Find("hong");
            _lv = _konglong.Find("lv");
            _lanInput = new List<InputType>();
            _lvInput = new List<InputType>();
            _heiInput = new List<InputType>();
            _hongInput = new List<InputType>();

            _endUI = curTrans.GetGameObject("EndUI");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _isDinosuarWalkingDic=new Dictionary<string, bool>();
            TabletWebsocketController.DinosaurMove = DinosaurMove;
            TabletWebsocketController.Instance.GetBleDevices();
            TabletWebsocketController.Instance.SubscribeToGetDeviceMsg();
            TabletWebsocketController.ProgramBoardAction = null;

            Util.AddBtnClick(_startBtn.transform.GetChild(0).gameObject,ClickStart);

            GameInit();
            GameStart();
        }

        private void DinosaurMove(string dinosaurName, List<string> steps)
        {
            if(!_isPlaying)
                return;
            List<InputType> gameSteps = new List<InputType>();
            if(!_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                _isDinosuarWalkingDic.Add(dinosaurName,false);

            if(_isDinosuarWalkingDic[dinosaurName])
                return;

            _isDinosuarWalkingDic[dinosaurName] = true;
            if (steps.Count > 1)
            {
                for (int i = 0; i < steps.Count; i++)
                {
                    InputType inputInfo = InputType.Null;
                    if (steps[i] == "前进一格")
                        inputInfo = InputType.Straight;
                    else if (steps[i] == "右转90度")
                        inputInfo = InputType.Right;
                    else if (steps[i] == "左转90度")
                        inputInfo = InputType.Left;
                    if (inputInfo != InputType.Null)
                        gameSteps.Add(inputInfo);
                }
            }
            if (gameSteps.Count <= 0)
            {
                _isDinosuarWalkingDic[dinosaurName] = false;
                return;
            }
            GameObject dinosaur = FindDinosaurObjectByName(dinosaurName);
            if (dinosaur == _lan.gameObject)
            {
                
                if (GetInBlock(_lan.gameObject) != 0)
                    KongLongRun(_lan.gameObject, gameSteps);
            }

            if (dinosaur == _hei.gameObject)
            {
                
                if (GetInBlock(_hei.gameObject) != 6)
                    KongLongRun(_hei.gameObject, gameSteps);
            }

            if (dinosaur == _hong.gameObject)
            {
                
                if (GetInBlock(_hong.gameObject) != 2)
                    KongLongRun(_hong.gameObject, gameSteps);
            }

            if (dinosaur == _lv.gameObject)
            {
                if (GetInBlock(_lv.gameObject) != 8)
                    KongLongRun(_lv.gameObject, gameSteps);
            }

            
            
            
           
        }

        GameObject FindDinosaurObjectByName(string name)
        {
            GameObject target = null;
            switch (name)
            {
                case "lan":
                    target = _lan.gameObject;
                    break;
                case "lv":
                    target = _lv.gameObject;
                    break;
                case "hong":
                    target = _hong.gameObject;
                    break;
                case "hei":
                    target = _hei.gameObject;
                    break;
            }

            return target;
        }


        private void GameInit()
        {
            _isPlaying = false;
            _cando = false;
            _mask.SetActive(false);
            talkIndex = 1;
            _row = 4;
            _col = 3;
            _canClick = false;

            ChangeBg(0);
            _ani1.gameObject.Show();
            _ani2.gameObject.Hide();
            _ani3.gameObject.Hide();
            _konglong.gameObject.Hide();
            _endUI.Hide();
            _max.Show();
            _maxZ.Hide();

            SetScale(_max, new Vector3(1, 1, 1));

            InitAni(_max);
            SpineManager.instance.DoAnimation(_max, "bei", true);
            InitAni(_left.transform.parent.gameObject);
            InitAni(_right.transform.parent.gameObject);
            SpineManager.instance.DoAnimation(_left.transform.parent.gameObject, "anb1", true);
            SpineManager.instance.DoAnimation(_right.transform.parent.gameObject, "ana1", true);

            InitDinosaurWalkingStatu();

            _curDirection = MaxDirection.back;
        }

        void GameStart()
        {
            SoundManager.instance.Stop("bgm");
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); _canClick = true; }));
        }

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
                speaker = bell;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void SetScale(GameObject obj, Vector3 scale)
        {
            obj.transform.localScale = scale;
        }

        void InitAni(GameObject ani)
        {
            ani.transform.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
        }

        void ChangeBg(int index)
        {
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[index];
        }

        void Wait(Action method_1 = null, float len = 0)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null,
                    () => { SoundManager.instance.ShowVoiceBtn(true); }));

                _max.Show();
                _maxZ.Hide();
                SetScale(_max, new Vector3(1, 1, 1));
                InitAni(_max);
                SpineManager.instance.DoAnimation(_max, "bei", true);

                SpineManager.instance.DoAnimation(_left.transform.parent.gameObject, "anb3", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_left.transform.parent.gameObject, "anb1", false);
                    _curDirection = MaxDirection.left;
                    MaxShow();

                    Wait(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_right.transform.parent.gameObject, "ana3", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_right.transform.parent.gameObject, "ana1", false);
                            _curDirection = MaxDirection.back;
                            MaxShow();
                        });
                    }, 4.3f);
                });
            }
            if(talkIndex == 2)
            {
                ChangeBg(1);
                _ani1.gameObject.Hide();
                _ani2.gameObject.Show();
                InitAni(_startBtn);
                SpineManager.instance.DoAnimation(_startBtn, "ui", false);
                _canClick = false;

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, 
                () => 
                {
                    _cando = true;
                }));
            }
            talkIndex++;
        }

        private void ClickStart(GameObject obj)
        {
            if(_cando)
            {
                SoundManager.instance.Stop("sound");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                _cando = false;
                SpineManager.instance.DoAnimation(_startBtn, "ui2", false,
                    () =>
                    {
                        SoundManager.instance.Stop("bgm");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                        _ani2.gameObject.Hide();
                        _ani3.gameObject.Show();
                        ChangeBg(2);
                        _konglong.gameObject.Show();
                        InitGame();

                        _canClick = true;
                    });
            }
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        #region 第一环节
        //左转右转操作
        private void LeftChange(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.Stop("sound");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                _canClick = false;
                if (_curDirection == MaxDirection.front)
                    _curDirection = MaxDirection.right;
                else if (_curDirection == MaxDirection.back)
                    _curDirection = MaxDirection.left;
                else if (_curDirection == MaxDirection.left)
                    _curDirection = MaxDirection.front;
                else
                    _curDirection = MaxDirection.back;

                SpineManager.instance.DoAnimation(_left.transform.parent.gameObject, "anb2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_left.transform.parent.gameObject, "anb1", false);

                    MaxShow();
                    SoundManager.instance.ShowVoiceBtn(true);
                    _canClick = true;
                });
            }
        }

        private void RightChange(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.Stop("sound");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                _canClick = false;
                if (_curDirection == MaxDirection.front)
                    _curDirection = MaxDirection.left;
                else if (_curDirection == MaxDirection.back)
                    _curDirection = MaxDirection.right;
                else if (_curDirection == MaxDirection.left)
                    _curDirection = MaxDirection.back;
                else
                    _curDirection = MaxDirection.front;

                SpineManager.instance.DoAnimation(_right.transform.parent.gameObject, "ana2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_right.transform.parent.gameObject, "ana1", false);

                    MaxShow();
                    SoundManager.instance.ShowVoiceBtn(true);
                    _canClick = true;
                });
            }
        }

        private void MaxShow()
        {
            if (_curDirection == MaxDirection.front)
            {
                _max.Hide();
                _maxZ.Show();
            }
            if (_curDirection == MaxDirection.back)
            {
                _maxZ.Hide();
                _max.Show();
                InitAni(_max);
                SpineManager.instance.DoAnimation(_max, "bei", true);
                SetScale(_max, new Vector3(1, 1, 1));
            }
            if (_curDirection == MaxDirection.left)
            {
                _maxZ.Hide();
                _max.Show();
                InitAni(_max);
                SpineManager.instance.DoAnimation(_max, "ce", true);
                SetScale(_max, new Vector3(1, 1, 1));
            }
            if (_curDirection == MaxDirection.right)
            {
                _maxZ.Hide();
                _max.Show();
                InitAni(_max);
                SpineManager.instance.DoAnimation(_max, "ce", true);
                SetScale(_max, new Vector3(-1, 1, 1));
            }
        }
        #endregion

        #region 游戏环节

        #region 可通用方法
        /// <summary>
        /// 添加指令
        /// </summary>
        /// <param name="type"></param>
        /// <param name="list"></param>
        void AddInputList(InputType type, List<InputType> list)
        {
            list.Add(type);
        }

        /// <summary>
        /// 清空指令
        /// </summary>
        private void ClearInputList()
        {
            _lanInput.Clear();
            _heiInput.Clear();
            _hongInput.Clear();
            _lvInput.Clear();
        }

        /// <summary>
        /// 获取现在在哪个格子中
        /// </summary>
        /// <param name="o"></param>
        /// <returns></returns>
        private int GetInBlock(GameObject o)
        {
            float dis = Vector2.Distance(o.transform.position, _block.GetChild(0).position);
            int lastIndex = 0;
            for (int i = 0; i < _block.childCount; i++)
            {
                if (Vector2.Distance(o.transform.position, _block.GetChild(i).position) <= dis)
                {
                    dis = Vector2.Distance(o.transform.position, _block.GetChild(i).position);
                    lastIndex = i;
                }
            }

            return lastIndex;
        }

        /// <summary>
        /// 判断是否获胜
        /// </summary>
        /// <returns></returns>
        private bool IsWin()
        {
            List<GameObject> bindDinasourList = new List<GameObject>();
            for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
            {
                string deviceAdress = TabletWebsocketController.Instance.CurrDevices[i].adress;
                // if (!deviceName.Contains("OPB"))
                //     continue;
                // string nameString = deviceName.Split(new string[] { "OPB" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                //Debug.Log("最后的DeviceName为：" + deviceName);
                foreach (var bindDevice in TabletWebsocketController.Instance.AnimalBind2Device)
                {
                    //Debug.Log("BindDevice.Value:" + bindDevice.Value);
                    if (bindDevice.Value == deviceAdress)
                    {
                        bindDinasourList.Add(FindDinosaurObjectByName(bindDevice.Key));
                    }
                }
            }
            int winCount = 0;
            for (int i = 0; i < bindDinasourList.Count; i++)
            {
                GameObject target = bindDinasourList[i];
                //Debug.Log(target.name+"GetInBlick:"+ GetInBlock(target)+" lan:"+_lan.name);
                if (target.name == _lan.name && GetInBlock(_lan.gameObject) == 0
                || target.name == _hei.name && GetInBlock(_hei.gameObject) == 6
                || target.name == _hong.name && GetInBlock(_hong.gameObject) == 2
                || target.name == _lv.name && GetInBlock(_lv.gameObject) == 8)
                {
                    ExcuteDinosaurWinStatu(target.transform);
                    winCount++;
                }
                // else if (target.name == _hei.name && GetInBlock(_hei.gameObject) == 6)
                // {
                //     winCount++;
                // }
                // else if (target.name == _hong.name && GetInBlock(_hong.gameObject) == 2)
                // {
                //     winCount++;
                // }
                // else if (target.name == _lv.name && GetInBlock(_lv.gameObject) == 8)
                // {
                //     winCount++;
                // }
            }
            Debug.Log("WinCount:" + winCount + " BindDinosaur.Count" + bindDinasourList.Count);
            if (winCount.Equals(bindDinasourList.Count))
            {
                return true;
            }
            else
            {
                return false;
            }
            // if (GetInBlock(_lan.gameObject) == 0 && GetInBlock(_hei.gameObject) == 6 && GetInBlock(_hong.gameObject) == 2 && GetInBlock(_lv.gameObject) == 8)
            //     return true;
            // else
            //     return false;
        }

        /// <summary>
        /// 胜利时的恐龙动画效果
        /// </summary>
        void WinKongLong()
        {
            for (int i = 0; i < _lan.childCount; i++)
                _lan.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _hong.childCount; i++)
                _hong.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _hei.childCount; i++)
                _hei.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _lv.childCount; i++)
                _lv.GetChild(i).gameObject.Hide();

            _lan.GetGameObject("zhengmian").Show();
            _hong.GetGameObject("zhengmian").Show();
            _hei.GetGameObject("zhengmian").Show();
            _lv.GetGameObject("zhengmian").Show();

            SetScale(_lan.gameObject, new Vector3(1, 1, 1));
            SetScale(_hong.gameObject, new Vector3(1, 1, 1));
            SetScale(_hei.gameObject, new Vector3(1, 1, 1));
            SetScale(_lv.gameObject, new Vector3(1, 1, 1));

            InitAni(_lan.GetGameObject("zhengmian"));
            InitAni(_hong.GetGameObject("zhengmian"));
            InitAni(_hei.GetGameObject("zhengmian"));
            InitAni(_lv.GetGameObject("zhengmian"));

            SpineManager.instance.DoAnimation(_lan.transform.GetGameObject("zhengmian"), "lan2", true);
            SpineManager.instance.DoAnimation(_hei.transform.GetGameObject("zhengmian"), "hei2", true);
            SpineManager.instance.DoAnimation(_hong.transform.GetGameObject("zhengmian"), "hong2", true);
            SpineManager.instance.DoAnimation(_lv.transform.GetGameObject("zhengmian"), "lv2", true);
        }

        void ExcuteDinosaurWinStatu(Transform dinosaurObject)
        {
            for (int i = 0; i < dinosaurObject.childCount; i++)
                dinosaurObject.GetChild(i).gameObject.Hide();
            dinosaurObject.GetGameObject("zhengmian").Show();
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SetScale(dinosaurObject.gameObject, new Vector3(1, 1, 1));
            InitAni(dinosaurObject.GetGameObject("zhengmian"));
            SpineManager.instance.DoAnimation(dinosaurObject.transform.GetGameObject("zhengmian"), dinosaurObject.name+"2", true);
        }

        #endregion

        void InitGame()
        {
            _lan.position = _block.GetChild(3).position;
            _hong.position = _block.GetChild(5).position;
            _hei.position = _block.GetChild(9).position;
            _lv.position = _block.GetChild(11).position;

            for (int i = 0; i < _lan.childCount; i++)
                _lan.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _hong.childCount; i++)
                _hong.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _hei.childCount; i++)
                _hei.GetChild(i).gameObject.Hide();
            for (int i = 0; i < _lv.childCount; i++)
                _lv.GetChild(i).gameObject.Hide();

            _lan.GetGameObject("cemian").Show();
            _hong.GetGameObject("cemian").Show();
            _hei.GetGameObject("cemian").Show();
            _lv.GetGameObject("cemian").Show();

            SetScale(_lan.gameObject, new Vector3(1, 1, 1));
            SetScale(_hong.gameObject, new Vector3(1, 1, 1));
            SetScale(_hei.gameObject, new Vector3(1, 1, 1));
            SetScale(_lv.gameObject, new Vector3(1, 1, 1));

            InitAni(_lan.GetGameObject("cemian"));
            InitAni(_hong.GetGameObject("cemian"));
            InitAni(_hei.GetGameObject("cemian"));
            InitAni(_lv.GetGameObject("cemian"));

            SpineManager.instance.DoAnimation(_lan.GetGameObject("cemian"), "lan", true);
            SpineManager.instance.DoAnimation(_hong.GetGameObject("cemian"), "hong", true);
            SpineManager.instance.DoAnimation(_hei.GetGameObject("cemian"), "hei", true);
            SpineManager.instance.DoAnimation(_lv.GetGameObject("cemian"), "lv", true);

            _lanDirection = KongLongDirection.right;
            _heiDirection = KongLongDirection.right;
            _lvDirection = KongLongDirection.right;
            _hongDirection = KongLongDirection.right;
            _isPlaying = true;
            InitDinosaurWalkingStatu();

            ClearInputList();
            
        }

        void InitDinosaurWalkingStatu(bool isFinish = false)
        {
            if (_isDinosuarWalkingDic.Count <= 0)
                return;
            List<string> dinosaurNamelist = new List<string>();
            foreach (var item in _isDinosuarWalkingDic)
            {
                string target = item.Key;
                dinosaurNamelist.Add(target);

            }

            for (int i = 0; i < dinosaurNamelist.Count; i++)
            {
                _isDinosuarWalkingDic[dinosaurNamelist[i]] = isFinish;
            }
        }
        void InitTargetDinosaurIfNotFinish(Transform target,Action callback=null)
        {
            if (target == _lan)
            {
                _lan.position = _block.GetChild(3).position;
                _lanDirection = KongLongDirection.right;
            }
            else if (target == _hong)
            {
                _hong.position = _block.GetChild(5).position;
                _hongDirection = KongLongDirection.right;
            }
            else if (target == _hei)
            {
                _hei.position = _block.GetChild(9).position;
                _heiDirection = KongLongDirection.right;
            }
            else if (target == _lv)
            {
                _lv.position = _block.GetChild(11).position;
                _lvDirection = KongLongDirection.right;
            }

            for (int i = 0; i < target.childCount; i++)
                target.GetChild(i).gameObject.Hide();
            SetScale(target.gameObject, new Vector3(1, 1, 1));
            target.GetGameObject("cemian").Show();
            InitAni(target.GetGameObject("cemian"));
            SpineManager.instance.DoAnimation(target.GetGameObject("cemian"), target.name, true);
            callback?.Invoke();
        }

        bool JudgeIsWinAndExcuteNext()
        {
            bool isWin = false;
            if (IsWin())
            {
                isWin = true;
                _canClick = false;
                _isPlaying = false;
                //WinKongLong();
                Wait(
                    () =>
                    {
                        SoundManager.instance.Stop("sound");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, null));
                        _mask.SetActive(true);
                        _endUI.Show();
                        InitAni(_endUI);
                        SpineManager.instance.DoAnimation(_endUI, "uiD", false,
                            () =>
                            {
                                
                                SpineManager.instance.DoAnimation(_endUI, "uiD2", true);
                            });
                    }, 2.0f);
            }

            return isWin;
        }


        /// <summary>
        /// 点击执行指令
        /// </summary>
        /// <param name="obj"></param>
        private void ClickZhiXing(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                if(GetInBlock(_lan.gameObject) != 0)
                    KongLongRun(_lan.gameObject, _lanInput);
                if (GetInBlock(_hei.gameObject) != 6)
                    KongLongRun(_hei.gameObject, _heiInput);
                if (GetInBlock(_lv.gameObject) != 8)
                    KongLongRun(_lv.gameObject, _lvInput);
                if (GetInBlock(_hong.gameObject) != 2)
                    KongLongRun(_hong.gameObject, _hongInput);

                Wait(
                () => 
                { 
                    // if(IsWin())
                    // {
                    //     _canClick = false;
                    //     WinKongLong();
                    //     Wait(
                    //     () =>
                    //     {
                    //         mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, null));
                    //         _endUI.Show();
                    //         InitAni(_endUI);
                    //         SpineManager.instance.DoAnimation(_endUI, "uiD", false, 
                    //         () => 
                    //         {
                    //             SpineManager.instance.DoAnimation(_endUI, "uiD2", true);
                    //         });
                    //     }, 2.0f);
                    // }
                    if(!JudgeIsWinAndExcuteNext())
                        _canClick = true;
                }, 2.0f);
            }
        }

        private void KongLongRun(GameObject konglong, List<InputType> list)
        {
            if (list.Count > 0)
                ZhiXingInput(konglong, list, 0);
        }

        void DinosaurDizzy(GameObject konglong,string directionName)
        {
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject(directionName), konglong.name + "8", false,
                () =>
                {
                    InitTargetDinosaurIfNotFinish(konglong.transform);
                    _isDinosuarWalkingDic[konglong.name] = false;
                });
        }

        /// <summary>
        /// 执行输入的指令
        /// </summary>
        /// <param name="konglong"></param>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void ZhiXingInput(GameObject konglong, List<InputType> list, int index)
        {
            //处于家中的格子
            if ((konglong.name == "lan" && GetInBlock(konglong) == 0) || (konglong.name == "hong" && GetInBlock(konglong) == 2) || (konglong.name == "lv" && GetInBlock(konglong) == 8) || (konglong.name == "hei" && GetInBlock(konglong) == 6))
            {
                // if (IsWin())
                // {
                //     _canClick = false;
                //     WinKongLong();
                //     Wait(
                //         () =>
                //         {
                //             mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, null));
                //             _mask.SetActive(true);
                //             _endUI.Show();
                //             InitAni(_endUI);
                //             SpineManager.instance.DoAnimation(_endUI, "uiD", false,
                //                 () =>
                //                 {
                //                     SpineManager.instance.DoAnimation(_endUI, "uiD2", true);
                //                 });
                //         }, 2.0f);
                // }
                JudgeIsWinAndExcuteNext();
                _isDinosuarWalkingDic[konglong.name] = false;
                return;
            }
            SoundManager.instance.Stop("sound");
           
            //先获得当前恐龙朝向
            KongLongDirection curDirection;
            if (konglong.name == "lan")
                curDirection = _lanDirection;
            else if (konglong.name == "hei")
                curDirection = _heiDirection;
            else if (konglong.name == "lv")
                curDirection = _lvDirection;
            else
                curDirection = _hongDirection;

            //无论哪个朝向都可以左转右转，但只有背向才能直走
            if(curDirection == KongLongDirection.front)
            {
                if(list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.right;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.right;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.right;
                    else
                        _hongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "cemian");
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(-1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.left;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.left;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.left;
                    else
                        _hongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "cemian");
                    }
                }
                else
                {
                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "8", false,
                    // () =>
                    // {
                    //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name, true);
                    //     InitTargetDinosaurIfNotFinish(konglong.transform);
                    //     _isDinosuarWalkingDic[konglong.name] = false;
                    // });
                    DinosaurDizzy(konglong, "zhengmian");
                    return;
                }    
            }
            if (curDirection == KongLongDirection.back)
            {
                if (list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(-1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.left;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.left;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.left;
                    else
                        _hongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "cemian");
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.right;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.right;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.right;
                    else
                        _hongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "cemian");
                    }
                }
                else
                {
                    int curBlock = GetInBlock(konglong);

                    //撞墙的可能
                    if ((konglong.name == "lan" && (curBlock - _col) != 0) || (konglong.name == "hong" && (curBlock - _col) != 2) || (konglong.name == "lv" && (curBlock - _col) != 8) || (konglong.name == "hei" && (curBlock - _col) != 6))
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "8", false,
                        // () =>
                        // {
                        //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name, true);
                        //     InitTargetDinosaurIfNotFinish(konglong.transform);
                        //     _isDinosuarWalkingDic[konglong.name] = false;
                        // });
                        DinosaurDizzy(konglong, "beimian");
                        return;
                    }
                    else
                    {
                        curBlock -= _col;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "2", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name, true);
                            if ((index + 1) < list.Count)
                                Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                            else
                            {
                                // if (IsWin())
                                // {
                                //     _canClick = false;
                                //     WinKongLong();
                                //     Wait(
                                //         () =>
                                //         {
                                //             mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, null));
                                //             _endUI.Show();
                                //             InitAni(_endUI);
                                //             SpineManager.instance.DoAnimation(_endUI, "uiD", false,
                                //                 () =>
                                //                 {
                                //                     SpineManager.instance.DoAnimation(_endUI, "uiD2", true);
                                //                 });
                                //         }, 2.0f);
                                // }
                                // if (konglong.name == _lan.name && GetInBlock(_lan.gameObject) != 0
                                //     || (konglong.name == _hei.name && GetInBlock(_hei.gameObject) != 6)
                                //     || (konglong.name == _hong.name && GetInBlock(_hong.gameObject) != 2)
                                //     || (konglong.name == _lv.name && GetInBlock(_lv.gameObject) != 8)
                                //     )
                                // {
                                //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "3", true);
                                // }
                                // else
                                    JudgeIsWinAndExcuteNext();
                                    _isDinosuarWalkingDic[konglong.name] = false;
                            }
                        });
                    }
                }
            }
            if (curDirection == KongLongDirection.left)
            {
                if (list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("zhengmian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("zhengmian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.front;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.front;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.front;
                    else
                        _hongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "zhengmian");
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("beimian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("beimian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.back;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.back;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.back;
                    else
                        _hongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "beimian");
                    }
                }
                else
                {
                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                    // () =>
                    // {
                    //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);
                    //     InitTargetDinosaurIfNotFinish(konglong.transform);
                    //     _isDinosuarWalkingDic[konglong.name] = false;
                    // });
                    DinosaurDizzy(konglong, "cemian");
                    return;
                }
            }
            if (curDirection == KongLongDirection.right)
            {
                if (list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("beimian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("beimian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.back;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.back;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.back;
                    else
                        _hongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "beimian");
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("zhengmian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("zhengmian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name, true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.front;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.front;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.front;
                    else
                        _hongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         _isDinosuarWalkingDic[konglong.name] = false;
                        //     });
                        DinosaurDizzy(konglong, "zhengmian");
                    }
                }
                else
                {
                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                    // () =>
                    // {
                    //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);
                    //     InitTargetDinosaurIfNotFinish(konglong.transform);
                    //     _isDinosuarWalkingDic[konglong.name] = false;
                    // });
                    DinosaurDizzy(konglong, "cemian");
                    return;
                }
            }
        }
        #endregion
    }
}
