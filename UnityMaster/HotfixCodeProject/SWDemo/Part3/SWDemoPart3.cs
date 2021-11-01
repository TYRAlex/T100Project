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

    public class SWDemoPart3
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private Transform _start;
        private GameObject _xiaoKongLong;
        private GameObject _startBtn;
        private GameObject _startClick;
        private Transform _block;
        private GameObject _endUI;
        private GameObject _clickZhiXing;

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

        private GameObject _curControl;
        private bool _canClick;
        private int _row;
        private int _col;

        private string _currentDeviceDinosaurName;
        private int _dinosaurDeviceIndex;

        private bool _isPlaying;

        private GameObject _mask;
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

            _start = curTrans.Find("Start");
            _xiaoKongLong = _start.GetGameObject("xiaoKongLong");
            _startBtn = _start.GetGameObject("StartBtn");
            _startClick = _start.GetGameObject("StartBtn/Start");
            Util.AddBtnClick(_startClick, StartClick);
            _block = curTrans.Find("Block");
            _endUI = curTrans.GetGameObject("EndUI");
            _clickZhiXing = curTrans.GetGameObject("click/click");
            Util.AddBtnClick(_clickZhiXing, ClickZhiXing);

            _konglong = curTrans.Find("konglong");
            _lan = _konglong.Find("lan");
            _hei = _konglong.Find("hei");
            _hong = _konglong.Find("hong");
            _lv = _konglong.Find("lv");
            _lanInput = new List<InputType>();
            _lvInput = new List<InputType>();
            _heiInput = new List<InputType>();
            _hongInput = new List<InputType>();
            _isDinosuarWalkingDic=new Dictionary<string, bool>();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            TabletWebsocketController.DinosaurMove = DinosaurMove;
            TabletWebsocketController.Instance.GetBleDevices();
            TabletWebsocketController.Instance.SubscribeToGetDeviceMsg();
            GameInit();
            GameStart();
        }

        private void DinosaurMove(string dinosaurName, List<string> steps)
        {
            if(!_isPlaying)
                return;
            
            if (!_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                _isDinosuarWalkingDic.Add(dinosaurName, false);

            Debug.Log("恐龙名字：" + dinosaurName + "是否回归初始状态："+ _isDinosuarWalkingDic[dinosaurName]);

            if (_isDinosuarWalkingDic[dinosaurName])
                return;

            _isDinosuarWalkingDic[dinosaurName] = true;
            List<InputType> gameSteps = new List<InputType>();
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
            //GameObject dinosaur = FindDinosaurObjectByName(dinosaurName);
            if (gameSteps.Count <= 0)
            {
                _isDinosuarWalkingDic[dinosaurName] = false;
                return;
            }
            if (_currentDeviceDinosaurName!=null&& dinosaurName == _currentDeviceDinosaurName)
            {
                KongLongRun(_curControl, gameSteps,dinosaurName);
                // if (dinosaur == _lan.gameObject)
                // {
                //     KongLongRun(_lan.gameObject, gameSteps);
                // }
                //
                // if (dinosaur == _hei.gameObject)
                // {
                //     KongLongRun(_hei.gameObject, gameSteps);
                // }
                //
                // if (dinosaur == _hong.gameObject)
                // {
                //     KongLongRun(_hong.gameObject, gameSteps);
                // }
                //
                // if (dinosaur == _lv.gameObject)
                // {
                //     KongLongRun(_lv.gameObject, gameSteps);
                // }
            }
        }

        void SwitchDeviceControl()
        {
            List<string> bindDinosaurDeviceNameList=new List<string>();
            foreach (var dinosaurKeyValue in TabletWebsocketController.Instance.AnimalBind2Device)
            {
                bindDinosaurDeviceNameList.Add(dinosaurKeyValue.Key);
            }

            if (_dinosaurDeviceIndex > bindDinosaurDeviceNameList.Count - 1)
                _dinosaurDeviceIndex = 0;
            if (bindDinosaurDeviceNameList.Count > 0)
            {
                _currentDeviceDinosaurName = bindDinosaurDeviceNameList[_dinosaurDeviceIndex];
                Debug.Log("切换之后的恐龙名字：" + _currentDeviceDinosaurName);
                _dinosaurDeviceIndex++;
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
            for (int i = 0; i < 4; i++)
            {
                curTrans.Find("konglong").GetChild(i).gameObject.SetActive(true);
            }

            _mask.SetActive(false);
            talkIndex = 1;
            _row = 4;
            _col = 6;
            _canClick = false;

            _konglong.gameObject.Hide();
            _block.gameObject.Hide();
            _endUI.Hide();
            _start.gameObject.Show();

            InitAni(_startBtn);
            SpineManager.instance.DoAnimation(_startBtn, "an2", false);
            _xiaoKongLong.Hide();
            InitDinosaurWalkingStatu();
        }

        void InitDinosaurWalkingStatu(bool isFinish = false)
        {
            if (_isDinosuarWalkingDic.Count <= 0)
                return;
            //List<string> dinosaurNamelist = new List<string>();
            // foreach (var item in _isDinosuarWalkingDic)
            // {
            //     string target = item.Key;
            //     dinosaurNamelist.Add(target);
            //
            // }
            // for (int i = 0; i < dinosaurNamelist.Count; i++)
            // {
            //     _isDinosuarWalkingDic[dinosaurNamelist[i]] = isFinish;
            // }
            ChangeDinosaurStatu("lan",isFinish);
            ChangeDinosaurStatu("hei", isFinish);
            ChangeDinosaurStatu("hong", isFinish);
            ChangeDinosaurStatu("lv", isFinish);
        }

        void ChangeDinosaurStatu(string dinosaurName,bool isWalking)
        {
            if(!_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                _isDinosuarWalkingDic.Add(dinosaurName,isWalking);
            _isDinosuarWalkingDic[dinosaurName] = isWalking;
        }

        void GameStart()
        {
            ChangeBg(0);
            bell.SetActive(true);
            _xiaoKongLong.Show();
            InitAni(_xiaoKongLong);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(_xiaoKongLong, "xkl", false, 
            () => 
            {
                SpineManager.instance.DoAnimation(_xiaoKongLong, "xkl2", true);
                InitAni(bell);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); _canClick = true;  }));
            });
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                
            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //点击开始游戏
        private void StartClick(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.Stop("sound");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(_startBtn, "an", false, 
                ()=> 
                {
                    ChangeBg(1);
                    _konglong.gameObject.Show();
                    _block.gameObject.Show();
                    _start.gameObject.Hide();
                    InitGame();

                  

                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () => { _canClick = true; _isPlaying = true; }));
                });
            }
        }

        
        #region 游戏

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
        #endregion

        private void InitGame()
        {
            _lan.position = _block.GetChild(0).position;
            _hong.position = _block.GetChild(20).position;
            _hei.position = _block.GetChild(13).position;
            _lv.position = _block.GetChild(9).position;

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

            SpineManager.instance.DoAnimation(_lan.GetGameObject("cemian"), "lan3", true);
            SpineManager.instance.DoAnimation(_hong.GetGameObject("cemian"), "hong", true);
            SpineManager.instance.DoAnimation(_hei.GetGameObject("cemian"), "hei", true);
            SpineManager.instance.DoAnimation(_lv.GetGameObject("cemian"), "lv", true);

            _lanDirection = KongLongDirection.right;
            _heiDirection = KongLongDirection.right;
            _lvDirection = KongLongDirection.right;
            _hongDirection = KongLongDirection.right;

            _curControl = _lan.gameObject;
            //_currentDeviceDinosaurName = _curControl.name;
            _dinosaurDeviceIndex = 0;
            SwitchDeviceControl();
           InitDinosaurWalkingStatu();
          
        }

        void InitTargetDinosaurIfNotFinish(Transform target)
        {
            Debug.Log("执行失败之后初始化~");
            if (target == _lan)
            {
                _lan.position = _block.GetChild(0).position;
                _lanDirection = KongLongDirection.right;
            }
            else if (target == _hong)
            {
                _hong.position = _block.GetChild(20).position;
                _hongDirection = KongLongDirection.right;
            }
            else if (target == _hei)
            {
                _hei.position = _block.GetChild(13).position;
                _heiDirection = KongLongDirection.right;
            }
            else if (target == _lv)
            {
                _lv.position = _block.GetChild(9).position;
                _lvDirection = KongLongDirection.right;
            }

            for (int i = 0; i < target.childCount; i++)
                target.GetChild(i).gameObject.Hide();

            SetScale(target.gameObject, new Vector3(1, 1, 1));
            target.GetGameObject("cemian").Show();
            InitAni(target.GetGameObject("cemian"));
            SpineManager.instance.DoAnimation(target.GetGameObject("cemian"), target.name+"3", true);
            
        }

        /// <summary>
        /// 点击执行指令
        /// </summary>
        /// <param name="obj"></param>
        private void ClickZhiXing(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                List<InputType> nList;
                if (_curControl == _lv.gameObject)
                    nList = _lvInput;
                else if (_curControl == _lan.gameObject)
                    nList = _lanInput;
                else if(_curControl == _hong.gameObject)
                    nList = _hongInput;
                else
                    nList = _heiInput;

                //KongLongRun(_curControl, nList);
            }
        }

        private void KongLongRun(GameObject konglong, List<InputType> list,string dinosaurName)
        {
           
            if (list.Count > 0)
                ZhiXingInput(konglong, list, 0, dinosaurName);
        }

        void DinosaurDizzy(GameObject konglong,string directionName,string dinosaurName)
        {
            string dinosaurAniIndex = "5";
            if (directionName.Equals("zhengmian"))
                dinosaurAniIndex = "7";
            else if (directionName.Equals("beimian"))
                dinosaurAniIndex = "3";
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject(directionName), konglong.name + dinosaurAniIndex, false,
                () =>
                {
                    WalkToWrongPlace(dinosaurName);
                    InitTargetDinosaurIfNotFinish(konglong.transform);

                });
        }

        /// <summary>
        /// 执行输入的指令
        /// </summary>
        /// <param name="konglong"></param>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void ZhiXingInput(GameObject konglong, List<InputType> list, int index,string dinosaurName="")
        {
           
            int _goalIndex;     //目标旗子位置
            int[] _falseIndex = new int[3];     //错误旗子位置
            KongLongDirection curDirection;     //当前恐龙朝向
            SoundManager.instance.Stop("sound");
            
            if (konglong.name == "lan")
            {
                curDirection = _lanDirection;
                _goalIndex = 13;
                _falseIndex = new int[4] { 0, 9,17, 20 };
            }
            else if (konglong.name == "hei")
            {
                curDirection = _heiDirection;
                _goalIndex = 20;
                _falseIndex = new int[4] { 0, 9, 13,17 };
            }
            else if (konglong.name == "lv")
            {
                curDirection = _lvDirection;
                _goalIndex = 17;
                _falseIndex = new int[4] { 0, 9,13, 20};
            }
            else
            {
                curDirection = _hongDirection;
                _goalIndex = 9;
                _falseIndex = new int[4] { 0, 13, 17,20};
            }

            if (curDirection == KongLongDirection.front)
            {
                if (list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.right;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.right;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.right;
                    else
                        _hongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        //     () =>
                        //     {
                        //         WalkToWrongPlace(dinosaurName);
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //        
                        //     });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                        _canClick = true;
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(-1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.left;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.left;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.left;
                    else
                        _hongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        _canClick = true;
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        //     () =>
                        //     {
                        //         WalkToWrongPlace(dinosaurName);
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         
                        //     });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                    }
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock + _col >= _block.childCount)
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "7", false,
                        // () =>
                        // {
                        //     _canClick = true;
                        //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", false);
                        //     InitTargetDinosaurIfNotFinish(konglong.transform);
                        //    
                        // });
                        DinosaurDizzy(konglong, "zhengmian", dinosaurName);
                        return;
                    }
                    //三种可能：走到目标格子（接力或胜利）；走到错误格子（游戏失败）；其他格子（可继续执行指令）
                    else
                    {
                        curBlock += _col;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "6", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", true);
                            if(curBlock == _goalIndex)
                            {
                                WinOrNextControl(konglong);
                                if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                    _isDinosuarWalkingDic[dinosaurName] = false;
                            }
                            else if(curBlock == _falseIndex[0] || curBlock == _falseIndex[1] || curBlock == _falseIndex[2])
                            {
                                // mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () =>
                                // {
                                //     _canClick = true;
                                //     if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                //         _isDinosuarWalkingDic[dinosaurName] = false;
                                // }));
                                //游戏失败
                                //InitGame();
                                ClearInputList();
                                Debug.Log("4");
                                // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "7", false,
                                //     () =>
                                //     {
                                //         _canClick = true;
                                //         Debug.Log("44");
                                //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", false);
                                //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                //         
                                //     });
                                DinosaurDizzy(konglong, "zhengmian", dinosaurName);
                                return;
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                                else
                                {
                                    // WalkToWrongPlace(dinosaurName);
                                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "7", false,
                                    //     () =>
                                    //     {
                                    //         _canClick = true;
                                    //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", false);
                                    //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                    //         
                                    //     });
                                   DinosaurDizzy(konglong, "zhengmian", dinosaurName);
                                }
                            }
                        });
                    }
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.left;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.left;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.left;
                    else
                        _hongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         
                        //     });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                        _canClick = true;
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.right;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.right;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.right;
                    else
                        _hongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //        
                        //     });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                        _canClick = true;
                    }
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock - _col < 0)
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "3", false,
                        // () =>
                        // {
                        //     _canClick = true;
                        //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);
                        //     InitTargetDinosaurIfNotFinish(konglong.transform);
                        //     
                        // });
                        DinosaurDizzy(konglong, "beimian", dinosaurName);
                        return;
                    }
                    //三种可能：走到目标格子（接力或胜利）；走到错误格子（游戏失败）；其他格子（可继续执行指令）
                    else
                    {
                        curBlock -= _col;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "5", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);
                            if (curBlock == _goalIndex)
                            {
                                WinOrNextControl(konglong);
                                if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                    _isDinosuarWalkingDic[dinosaurName] = false;
                            }
                            else if (curBlock == _falseIndex[0] || curBlock == _falseIndex[1] || curBlock == _falseIndex[2])
                            {
                                // mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () =>
                                // {
                                //     _canClick = true;
                                //     if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                //         _isDinosuarWalkingDic[dinosaurName] = false;
                                // }));
                                //
                                // //游戏失败
                                //
                                // Debug.Log("1");
                                // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "3", false,
                                //     () =>
                                //     {
                                //         _canClick = true;
                                //         Debug.Log("11");
                                //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);
                                //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                //         
                                //     });
                                DinosaurDizzy(konglong, "beimian", dinosaurName);

                                return;
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                                else
                                {
                                    // WalkToWrongPlace(dinosaurName);
                                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "3", false,
                                    //     () =>
                                    //     {
                                    //         _canClick = true;
                                    //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);
                                    //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                    //         
                                    //     });
                                    DinosaurDizzy(konglong, "beimian", dinosaurName);
                                }
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.front;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.front;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.front;
                    else
                        _hongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "7", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         
                        //     });
                        DinosaurDizzy(konglong, "zhengmian", dinosaurName);
                        _canClick = true;
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("beimian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("beimian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.back;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.back;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.back;
                    else
                        _hongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //     });
                        DinosaurDizzy(konglong, "beimian", dinosaurName);
                        _canClick = true;
                    }
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock == 0 || curBlock == 6 || curBlock == 12 || curBlock == 18)
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        // () =>
                        // {
                        //     _canClick = true;
                        //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                        //     InitTargetDinosaurIfNotFinish(konglong.transform);
                        //    
                        // });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                        return;
                    }
                    //三种可能：走到目标格子（接力或胜利）；走到错误格子（游戏失败）；其他格子（可继续执行指令）
                    else
                    {
                        curBlock -= 1;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "4", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                            if (curBlock == _goalIndex)
                            {
                                WinOrNextControl(konglong);
                                if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                    _isDinosuarWalkingDic[dinosaurName] = false;
                            }
                            else if (curBlock == _falseIndex[0] || curBlock == _falseIndex[1] || curBlock == _falseIndex[2])
                            {
                                // mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () =>
                                // {
                                //     _canClick = true;
                                //     if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                //         _isDinosuarWalkingDic[dinosaurName] = false;
                                // }));
                                //游戏失败
                                // InitGame();
                                // ClearInputList();
                                // if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                //     _isDinosuarWalkingDic[dinosaurName] = false;
                                // _canClick = true;
                                Debug.Log("2");
                                // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                                //     () =>
                                //     {
                                //         _canClick = true;
                                //         Debug.Log("22");
                                //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                                //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                //         
                                //     });
                                DinosaurDizzy(konglong, "cemian", dinosaurName);
                                _canClick = true;
                                return;
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                                else
                                {
                                    WalkToWrongPlace(dinosaurName);
                                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                                    //     () =>
                                    //     {
                                    //         _canClick = true;
                                    //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                                    //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                    //     });
                                    DinosaurDizzy(konglong, "cemian", dinosaurName);
                                    _canClick = true;
                                }
                            }
                        });
                    }
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "4", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.back;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.back;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.back;
                    else
                        _hongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "8", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         
                        //     });
                        DinosaurDizzy(konglong, "beimian", dinosaurName);
                        _canClick = true;
                    }
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("zhengmian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("zhengmian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "4", true);

                    if (konglong.name == "lan")
                        _lanDirection = KongLongDirection.front;
                    else if (konglong.name == "hei")
                        _heiDirection = KongLongDirection.front;
                    else if (konglong.name == "lv")
                        _lvDirection = KongLongDirection.front;
                    else
                        _hongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                    else
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "7", false,
                        //     () =>
                        //     {
                        //         InitTargetDinosaurIfNotFinish(konglong.transform);
                        //         
                        //     });
                        DinosaurDizzy(konglong, "zhengmian", dinosaurName);
                        _canClick = true;
                    }
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock == 5 || curBlock == 11 || curBlock == 17 || curBlock == 23)
                    {
                        // WalkToWrongPlace(dinosaurName);
                        // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                        // () =>
                        // {
                        //     _canClick = true;
                        //     SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                        //     InitTargetDinosaurIfNotFinish(konglong.transform);
                        //     
                        // });
                        DinosaurDizzy(konglong, "cemian", dinosaurName);
                        return;
                    }
                    //三种可能：走到目标格子（接力或胜利）；走到错误格子（游戏失败）；其他格子（可继续执行指令）
                    else
                    {
                        curBlock += 1;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "4", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                            if (curBlock == _goalIndex)
                            {
                                WinOrNextControl(konglong);
                                if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                    _isDinosuarWalkingDic[dinosaurName] = false;
                            }
                            else if (curBlock == _falseIndex[0] || curBlock == _falseIndex[1] || curBlock == _falseIndex[2])
                            {
                                // mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () =>
                                // {
                                //     _canClick = true;
                                //     if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                //         _isDinosuarWalkingDic[dinosaurName] = false;
                                // }));
                                // //游戏失败
                                // // InitGame();
                                // // ClearInputList();
                                // // if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                                // //     _isDinosuarWalkingDic[dinosaurName] = false;
                                // // _canClick = true;
                                // Debug.Log("3");
                                //
                                // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                                //     () =>
                                //     {
                                //         _canClick = true;
                                //         Debug.Log("33");
                                //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                                //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                //         
                                //     });
                                DinosaurDizzy(konglong, "cemian", dinosaurName);
                                return;
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1,dinosaurName); }, 0.5f);
                                else
                                {
                                    // WalkToWrongPlace(dinosaurName);
                                    // SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "5", false,
                                    //     () =>
                                    //     {
                                    //         _canClick = true;
                                    //         SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "3", true);
                                    //         InitTargetDinosaurIfNotFinish(konglong.transform);
                                    //         
                                    //     });
                                    DinosaurDizzy(konglong, "cemian", dinosaurName);
                                }
                            }
                        });
                    }
                }
            }
        }


        void WalkToWrongPlace(string dinosaurName)
        {
            float timer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
            Wait(() =>
            {
                if (_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                    _isDinosuarWalkingDic[dinosaurName] = false;
            }, timer);
        }

        /// <summary>
        /// 判断执行胜利操作或接力操作
        /// </summary>
        /// <param name="konglong"></param>
        private void WinOrNextControl(GameObject konglong)
        {
            bool _isWin = false;
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SwitchDeviceControl();
            //接力或胜利
            if (konglong == _lan.gameObject)
                _curControl = _hei.gameObject;
            else if (konglong == _hei.gameObject)
                _curControl = _hong.gameObject;
            else if (konglong == _hong.gameObject)
                _curControl = _lv.gameObject;
            else
                _isWin = true;
            Debug.Log("游戏内部切换恐龙之后：" + _curControl.name);
            if (!_isWin)
            {
                konglong.SetActive(false);
                for (int i = 0; i < konglong.transform.childCount; i++)
                    konglong.transform.GetChild(i).gameObject.Hide();
                konglong.transform.GetGameObject("cemian").Show();
                SetScale(konglong.transform.gameObject, new Vector3(1, 1, 1));
                InitAni(konglong.transform.GetGameObject("cemian"));

                SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);
                SpineManager.instance.DoAnimation(_curControl.transform.GetGameObject("cemian"), _curControl.name + "3", true);
                InitDinosaurWalkingStatu();
                _canClick = true;
                return;
            }
            else
            {
                _isPlaying = false;
                konglong.Hide();
                WinKongLong();
                Wait(
                () =>
                {
                    SoundManager.instance.Stop("sound");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, null));
                    _endUI.Show();
                    _mask.SetActive(true);
                    InitAni(_endUI);
                    SpineManager.instance.DoAnimation(_endUI, "uiA", false,
                    () =>
                    {
                        
                        SpineManager.instance.DoAnimation(_endUI, "uiA2", true);
                        
                    });
                }, 2.0f);
            }
        }

        #endregion
    }
}
