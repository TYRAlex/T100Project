using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ILFramework.Scripts.Utility;
using ILRuntime.Runtime;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    enum InputType
    {
        Null,
        Straight,
        Left,
        Right,
    }

    public class SWDemoPart1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject _gezi;

        private GameObject bell;

        private Transform _ani1;
        private Empty4Raycast[] _clikAni1;
        private GameObject _pan;
        private GameObject _max;

        private Transform _ani2;
        private GameObject _bj;
        private GameObject _startBtn;

        private Transform _topLevelImageParent;
        private Transform _topLevelAni3Panel;
        private Transform _topLevelAni4Panel;

        private Transform _ani3;
        private Transform _block;
        private GameObject _zhixing;


        private Transform _ani4;
        private Transform _block2;
        private GameObject _zhixing2;

        private Transform _konglong;
        private Transform _lan;
        private Transform _hei;
        private Transform _hong;
        private Transform _lv;
        private List<InputType> _lanInput;
        private List<InputType> _heiInput;
        private List<InputType> _hongInput;
        private List<InputType> _lvInput;

        private Transform _UI;
        private GameObject _uiE;
        private GameObject _xyg;
        private GameObject _next;

        private Transform _pos;
        private float speed;
        private bool _canClick;
        private bool _alreadyWin;
        private bool _canShowVoice;
        private Dictionary<GameObject,bool> _dizzyingDic;

        private int dinosaurIndex = 1;
        private Transform _choosePanel;
        private Transform _devicePanel;
        private Dictionary<string, GameObject> _dinosaurDic;
        private Dictionary<GameObject, bool> _isBindTheDinosaur;
        private Dictionary<string, GameObject> _deviceObjectDic;
        private GameObject _confirmBtn;
        private GameObject _choseDinosaur = null;
        private GameObject _chosePBDevice = null;
        private List<GameObject> _pbConnectedDinosaurList;
        private Dictionary<string, string> _pbDeviceNameTransferGameName;
        private Dictionary<string, bool> _isDinosuarWalkingDic;
        private GameObject _mask;
        private bool _isPlaying;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;
            _choseDinosaur = null;
            _chosePBDevice = null;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            _gezi = curTrans.Find("gezi").gameObject;

            bell = curTrans.Find("bell").gameObject;

            _mask = curTrans.Find("UI").Find("mask").gameObject;

            _ani1 = curTrans.Find("Ani1");
            _clikAni1 = _ani1.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clikAni1.Length; i++)
            {
                Util.AddBtnClick(_clikAni1[i].gameObject, ClickBtn);
            }
            _pan = _ani1.Find("pan").gameObject;
            _max = _ani1.Find("max").gameObject;

            _ani2 = curTrans.Find("Ani2");
            _bj = _ani2.transform.GetGameObject("bj");
            _startBtn = _ani2.transform.GetGameObject("startBtn");
            Util.AddBtnClick(_startBtn, ClickStart);

            _topLevelImageParent = curTrans.GetTransform("TopLevelImage");
            _topLevelAni3Panel = _topLevelImageParent.GetTransform("Ani3");
            _topLevelAni4Panel = _topLevelImageParent.GetTransform("Ani4");

            _ani3 = curTrans.Find("Ani3");
            _block = _ani3.transform.Find("Block");
            _zhixing = _ani3.GetGameObject("zhixing");
            Util.AddBtnClick(_zhixing.transform.GetGameObject("1"), ClickZhiXing);

            _ani4 = curTrans.Find("Ani4");
            _block2 = _ani4.transform.Find("Block");
            _zhixing2 = _ani4.GetGameObject("zhixing");
            Util.AddBtnClick(_zhixing2.transform.GetGameObject("1"), ClickZhiXing);

            _konglong = curTrans.Find("konglong");
            _lan = _konglong.transform.Find("lan");
            _hei = _konglong.transform.Find("hei");
            _hong = _konglong.transform.Find("hong");
            _lv = _konglong.transform.Find("lv");
            InitDinosaurDizzyStatu();
            _UI = curTrans.Find("UI");
            _uiE = _UI.GetGameObject("uiE");
            _xyg = _UI.GetGameObject("xyg");
            _next = _UI.GetGameObject("next");
            Util.AddBtnClick(_next, ClickNext);

            _pos = curTrans.Find("Pos");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _isDinosuarWalkingDic=new Dictionary<string, bool>();

            _lanInput = new List<InputType>();
            _heiInput = new List<InputType>();
            _hongInput = new List<InputType>();
            _lvInput = new List<InputType>();
            _pbConnectedDinosaurList=new List<GameObject>();
            _choosePanel = curTrans.GetTransform("Choose");
            _deviceObjectDic=new Dictionary<string, GameObject>();
            _dinosaurDic=new Dictionary<string, GameObject>();
            _isBindTheDinosaur=new Dictionary<GameObject, bool>();
            _pbDeviceNameTransferGameName=new Dictionary<string, string>();
            Transform daKongLongPanel = _choosePanel.GetTransform("daKongLong");
            for (int i = 0; i < daKongLongPanel.childCount; i++)
            {
                Transform target = daKongLongPanel.GetChild(i);
                _dinosaurDic.Add(target.name, target.gameObject);
                
            }
            
            _devicePanel = _choosePanel.GetTransform("panel");
            
            for (int i = 0; i < _devicePanel.childCount; i++)
            {
                Transform target = _devicePanel.GetChild(i);
                _deviceObjectDic.Add(target.name, target.gameObject);
                PointerClickListener.Get(target.gameObject).clickDown = ClickSelectItem;
            }
            
            for (int i = 0; i < _konglong.childCount; i++)
            {
                GameObject target = _konglong.GetChild(i).gameObject;
                _isBindTheDinosaur.Add(target, false);
                PointerClickListener.Get(target).clickDown = ClickDinosaurAndOpenTheChoosePanel;
            }
            TabletWebsocketController.Instance.AnimalBind2Device.Clear();
            TabletWebsocketController.DinosaurMove = DinosaurMove;
            _confirmBtn = _choosePanel.GetGameObject("Btn");
            PointerClickListener.Get(_confirmBtn).clickDown = ConfirmSelectedDinosaur;
            TabletWebsocketController.Instance.GetBleDevices();
            TabletWebsocketController.Instance.SubscribeToGetDeviceMsg();
            TabletWebsocketController.ProgramBoardAction = ClickRunButtonEvent;
            Debug.Log("准备关掉所有面板上的Item");
            
            for (int i = 0; i < _devicePanel.childCount; i++)
            {
                Transform targetItem = _devicePanel.GetChild(i);
                targetItem.GetTransform("xx").gameObject.Hide();
                targetItem.gameObject.SetActive(false);
            }
            Debug.Log("所有面板上的Item都已经关闭");

           
            
            GameInit();
            GameStart();
           
        }

       


        

        void InitDinosaurDizzyStatu()
        {
            _dizzyingDic=new Dictionary<GameObject, bool>();
            if (_dizzyingDic.ContainsKey(_lan.gameObject))
                _dizzyingDic[_lan.gameObject] = false;
            else
                _dizzyingDic.Add(_lan.gameObject, false);
            if (_dizzyingDic.ContainsKey(_hei.gameObject))
                _dizzyingDic[_hei.gameObject] = false;
            else
                _dizzyingDic.Add(_hei.gameObject, false);
            if (_dizzyingDic.ContainsKey(_hong.gameObject))
                _dizzyingDic[_hong.gameObject] = false;
            else
                _dizzyingDic.Add(_hong.gameObject, false);
            if (_dizzyingDic.ContainsKey(_lv.gameObject))
                _dizzyingDic[_lv.gameObject] = false;
            else
                _dizzyingDic.Add(_lv.gameObject, false);
        }

        #region 编程版连接代码

        public void ClickRunButtonEvent(string deviceAdress, string msg)
        {
            Debug.Log("广播：" + deviceAdress + "  :" + msg);
            // if (msg.Equals("MSG_RUN"))
            // {
            //     if (_choosePanel.gameObject.activeSelf)
            //     {
            //         string itemName = _pbDeviceNameTransferGameName[deviceAdress];
            //         if (itemName != null)
            //         {
            //             RawImage targetImage = _choosePanel.GetTransform(itemName).GetTargetComponent<RawImage>(itemName);
            //             targetImage.DOColor(Color.clear, 0.5f).OnComplete(() => targetImage.DOColor(Color.white, 0.5f));
            //         }
            //     }
            // }
        }

        /// <summary>
        /// 点击选择相应的编程版
        /// </summary>
        /// <param name="target">编程版</param>
        void ClickSelectItem(GameObject target)
        {
            if (target.activeSelf)
            {
                target.transform.GetGameObject("xx").Show();
                _chosePBDevice = target;
                for (int i = 0; i < _devicePanel.childCount; i++)
                {
                    Transform targetItem = _devicePanel.GetChild(i);
                    if (targetItem.name != target.name)
                    {
                        targetItem.GetGameObject("xx").Hide();
                    }
                }
            }
        }

        /// <summary>
        /// 确认之后绑定相应的恐龙
        /// </summary>
        /// <param name="go"></param>
        void ConfirmSelectedDinosaur(GameObject go)
        {
            if (_chosePBDevice != null && _choseDinosaur != null)
            {
                Debug.Log("_chosePBDevice:" + _chosePBDevice.name + " _choseDinosaur:" + _choseDinosaur.name);
                if (!_isBindTheDinosaur[_choseDinosaur])
                {
                    Debug.Log("开始绑定");
                    TabletWebsocketController.Instance.BindDinosaurNameToDeviceName(_choseDinosaur.name,
                        GetDeviceAdressByPanelItemName(_chosePBDevice.name));
                    _pbConnectedDinosaurList.Add(_choseDinosaur);
                    _isBindTheDinosaur[_choseDinosaur] = true;
                }
                else
                {
                    Debug.Log("绑定错误，请检查!" + _choseDinosaur.name);
                }
            }
            _choosePanel.gameObject.Hide();
        }

        /// <summary>
        /// 通过定义的编程版名字来确认绑定的设备蓝牙地址名称
        /// </summary>
        /// <param name="itemName"></param>
        /// <returns></returns>
        string GetDeviceAdressByPanelItemName(string itemName)
        {
            string deviceName = String.Empty;
            foreach (var keyValueDic in _pbDeviceNameTransferGameName)
            {
                if (keyValueDic.Value.Equals(itemName))
                {
                    deviceName = keyValueDic.Key;
                }
            }

            return deviceName;
        }

        /// <summary>
        /// 点击恐龙打开面板
        /// </summary>
        /// <param name="target">点击的恐龙</param>
        void ClickDinosaurAndOpenTheChoosePanel(GameObject target)
        {
            TabletWebsocketController.Instance.GetBleDevices();
            if (_isBindTheDinosaur[target])
                return;
            _chosePBDevice = null;
            _choosePanel.gameObject.Show();
            _choseDinosaur = target;
            Transform daKongLong = _choosePanel.GetTransform("daKongLong");
            for (int i = 0; i < daKongLong.childCount; i++)
            {
                Transform targetKongLong = daKongLong.GetChild(i);
                if (targetKongLong.name.Equals(target.name))
                {
                    targetKongLong.gameObject.Show();
                }
                else
                {
                    targetKongLong.gameObject.Hide();
                }
            }


            if (TabletWebsocketController.Instance.CurrDevices.Length > 0)
            {
                Debug.Log("遍历编程板：");

                for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
                {
                    if (TabletWebsocketController.Instance.CurrDevices[i].connected == false || !TabletWebsocketController.Instance.CurrDevices[i].name.Equals("CodingBoard"))
                        continue;
                    string deviceAdress = TabletWebsocketController.Instance.CurrDevices[i].adress;
                    Debug.Log("Device adress:" + deviceAdress);
                    
                    bool isMatchTheBindAdress = false;
                    foreach (var keyValue in TabletWebsocketController.Instance.AnimalBind2Device)
                    {
                        string targetValue = keyValue.Value;
                        Debug.Log("keyValue:"+ targetValue + "    kkkk:"+ _pbDeviceNameTransferGameName.ContainsKey(targetValue));
                        // if (_pbDeviceNameTransferGameName.ContainsKey(targetValue))
                        // {
                        //     isMatchTheBindAdress = true; 
                        // }
                        if (deviceAdress.Equals(targetValue))
                            isMatchTheBindAdress = true;
                    }

                    //string nameString = deviceName.Split(new string[] { "OPB" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                    
                    if (!_pbDeviceNameTransferGameName.ContainsKey(deviceAdress)&&!isMatchTheBindAdress)
                    {
                        Debug.Log("加入地址：" + deviceAdress + " index:" + dinosaurIndex);
                        _pbDeviceNameTransferGameName.Add(deviceAdress, dinosaurIndex.ToString());
                        dinosaurIndex++;
                    }
                    //Debug.Log("编程板名字“：" + nameString);
                }

                if (TabletWebsocketController.Instance.CurrDevices.Length != _pbDeviceNameTransferGameName.Count)
                {
                    Dictionary<string, string> newDic = new Dictionary<string, string>();
                    foreach (string adress in _pbDeviceNameTransferGameName.Keys)
                    {
                        for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
                        {
                            string deviceAdress = TabletWebsocketController.Instance.CurrDevices[i].adress;
                            if (deviceAdress.Equals(adress))
                            {
                                newDic.Add(adress, _pbDeviceNameTransferGameName[adress]);
                            }
                        }
                    }

                    _pbDeviceNameTransferGameName = newDic;
                }


                for (int j = 0; j < _devicePanel.childCount; j++)
                {
                    Transform device = _devicePanel.GetChild(j);
                    device.GetGameObject("xx").Hide();
                    string deviceAdress = GetDeviceAdressByPanelItemName(device.name);
                    Debug.Log("根据Panel名字"+device.name+"查到的地址为：" + deviceAdress);
                    if (deviceAdress != String.Empty && _pbDeviceNameTransferGameName[deviceAdress].Equals(device.name))
                    {
                        Debug.Log("显示" + deviceAdress + "对应的Panel" + device.name);
                        device.gameObject.Show();
                    }
                    else
                    {
                        device.gameObject.Hide();
                    }
                }

                foreach (var keyValue in TabletWebsocketController.Instance.AnimalBind2Device)
                {
                    string targetValue = keyValue.Value;
                    //Debug.Log("keyValue:"+ targetValue + "    kkkk:"+deviceName);
                    if (_pbDeviceNameTransferGameName.ContainsKey(targetValue))
                    {
                        _devicePanel.GetGameObject(_pbDeviceNameTransferGameName[targetValue]).Hide();
                    }

                    // if (keyValue.Value == deviceAdress)
                    // {
                    //     Debug.Log("编程板地址" + deviceAdress + "已经绑定");
                    //     GameObject targetDevice =
                    //         _devicePanel.GetGameObject(_pbDeviceNameTransferGameName[deviceAdress]);
                    //     targetDevice.Hide();
                    //     //targetDevice.transform.GetChild(0).gameObject.Show();
                    // }
                }
            }



        }

        #endregion





        private void GameInit()
        {
            _isPlaying = false;
            InitDinosaurDizzyStatu();
            _mask.SetActive(false);
            for (int i = 0; i < 4; i++)
            {
                _konglong.GetChild(i).GetChild(0).gameObject.SetActive(true);
                _konglong.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
            dinosaurIndex = 1;
            talkIndex = 1;
            speed = 250f;
            _canClick = false;
            _canShowVoice = false;
            _alreadyWin = false;
            ClearInputList();

            _ani1.gameObject.Show();
            _ani2.gameObject.Hide();
            _ani3.gameObject.Hide();
            _ani4.gameObject.Hide();
            _topLevelAni3Panel.gameObject.Hide();
            _topLevelAni4Panel.gameObject.Hide();
            _konglong.gameObject.Hide();
            _UI.gameObject.Show();
            _uiE.Hide();
            _xyg.Hide();
            _next.Hide();
            _gezi.Hide();
            _pan.Hide();
            _max.Hide();
            _choosePanel.gameObject.Hide();
            for (int i = 0; i < 3; i++)
            {
                GameObject o = _clikAni1[i].transform.parent.gameObject;
                o.Show();
                InitAni(o);
                SpineManager.instance.DoAnimation(o, o.name + "1", false);
            }

            InitDinosaurWalkingStatu();
            _max.transform.position = _pos.Find("maxPos1").position;

            ChangeBg(0);

            
        }

        void InitDinosaurWalkingStatu(bool isFinish=false)
        {
            // if(_isDinosuarWalkingDic.Count<=0)
            //     return;
            // List<string> dinosaurNamelist = new List<string>();
            // foreach (var item in _isDinosuarWalkingDic)
            // {
            //     string target = item.Key;
            //     dinosaurNamelist.Add(target);
            //
            // }
            //
            // for (int i = 0; i < dinosaurNamelist.Count; i++)
            // {
            //     _isDinosuarWalkingDic[dinosaurNamelist[i]] = isFinish;
            // }
            ChangeDinosaurStatu("lan", isFinish);
            ChangeDinosaurStatu("hei", isFinish);
            ChangeDinosaurStatu("hong", isFinish);
            ChangeDinosaurStatu("lv", isFinish);
        }

        void ChangeDinosaurStatu(string dinosaurName, bool isWalking)
        {
            if (!_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                _isDinosuarWalkingDic.Add(dinosaurName, isWalking);
            _isDinosuarWalkingDic[dinosaurName] = isWalking;
        }

        void GameStart()
        {
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); _canClick = true; }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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

                for (int i = 0; i < _clikAni1.Length; i++)
                    _clikAni1[i].transform.parent.gameObject.Hide();
                _pan.Show();
                _max.Show();
                _gezi.Show();

                InitAni(_pan);
                InitAni(_max);
                SpineManager.instance.DoAnimation(_pan, "pan2", false);
                SpineManager.instance.DoAnimation(_max, "ce", true);

                bell.Show();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5, null,
                () =>
                {
                    bell.Hide();
                    Wait(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_pan, "pan", false);
                        Wait(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_max, "ce2", true);
                            Tween tw1 = _max.transform.DOMoveX(_pos.Find("maxPos2").position.x, 1.0f).OnComplete(
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_max, "ce", true);
                                SoundManager.instance.ShowVoiceBtn(true);
                            });
                            tw1.SetEase(Ease.Linear);
                        }, 1.0f);
                    }, 1.0f);
                }));
            }
            if (talkIndex == 2)
            {
                bell.Show();
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 6, null,
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 3)
            {
                _canClick = false;
                ChangeBg(1);
                _ani1.gameObject.Hide();
                _ani2.gameObject.Show();
                _gezi.Hide();
                InitAni(_bj);
                SpineManager.instance.DoAnimation(_bj, "animation", true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 7, null,
                () =>
                {
                    bell.Hide();
                    _canClick = true;
                }));
            }
            talkIndex++;
        }

        void InitAni(GameObject ani)
        {
            ani.transform.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //第一关点击按钮
        private void ClickBtn(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.ShowVoiceBtn(false);
                GameObject o = obj.transform.parent.gameObject;

                if (o.name == "anc")
                {
                    _canShowVoice = true;
                    //随机成功语音TODO
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, SuccessVoice(), false);
                    Wait(() =>
                    {
                        if (_canShowVoice)
                            SoundManager.instance.ShowVoiceBtn(true);
                        else
                            SoundManager.instance.ShowVoiceBtn(false);
                        _canClick = true;
                    }, timer);
                }
                else
                {
                    //随机遗憾语音TODO
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                     float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, FalseVoice(), false);
                    Wait(() =>
                    {
                        if (_canShowVoice)
                            SoundManager.instance.ShowVoiceBtn(true);
                        else
                            SoundManager.instance.ShowVoiceBtn(false);
                        _canClick = true;
                    }, timer);
                }

                SpineManager.instance.DoAnimation(o, o.name + "2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(o, o.name + "1", false);
                    
                    
                });
            }
        }

        int SuccessVoice()
        {
            return Random.Range(3, 5);
        }

        int FalseVoice()
        {
            return Random.Range(1, 3);
        }

        #region 游戏相关

        //点击开始按钮
        private void ClickStart(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(_bj, "animation2", false,
                () =>
                {
                    _isPlaying = true;
                    SoundManager.instance.Stop("bgm");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                    ChangeBg(2);
                    _ani2.gameObject.Hide();
                    _ani3.gameObject.Show();
                    _konglong.gameObject.Show();
                    InitAni(_lan.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_lan.GetGameObject("cemian"), "lan", true);
                    InitAni(_hei.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_hei.GetGameObject("cemian"), "hei", true);
                    InitAni(_hong.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_hong.GetGameObject("cemian"), "hong", true);
                    InitAni(_lv.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_lv.GetGameObject("cemian"), "lv", true);
                    _topLevelAni3Panel.gameObject.Show();
                    _lan.transform.position = _block.GetChild(0).position;
                    _hei.transform.position = _block.GetChild(3).position;
                    _hong.transform.position = _block.GetChild(6).position;
                    _lv.transform.position = _block.GetChild(9).position;

                    //测试用，记得删除
                    //TestInput();

                    _canClick = true;
                });
            }
        }

        //添加指令
        void AddInputList(InputType type, List<InputType> list)
        {
            list.Add(type);
        }

        //清空指令
        private void ClearInputList()
        {
            _lanInput.Clear();
            _heiInput.Clear();
            _hongInput.Clear();
            _lvInput.Clear();
        }

        //点击执行命令
        private void ClickZhiXing(GameObject obj)
        {
            if (_canClick)
            {
                Debug.LogError("执行命令");
                _canClick = false;

                if(_lanInput.Count > 0)
                    KongLongStraight(_lan.gameObject, 0, _ani3.gameObject.activeSelf ? 2 : 4, _lanInput);
                if (_heiInput.Count > 0)
                    KongLongStraight(_hei.gameObject, 0, _ani3.gameObject.activeSelf ? 5 : 9, _heiInput);
                if (_hongInput.Count > 0)
                    KongLongStraight(_hong.gameObject, 0, _ani3.gameObject.activeSelf ? 8 : 14, _hongInput);
                if (_lvInput.Count > 0)
                    KongLongStraight(_lv.gameObject, 0, _ani3.gameObject.activeSelf ? 11 : 19, _lvInput);
            }
        }



        void DinosaurMove(string dinosaurName, List<string> steps)
        {
            Debug.Log("走起~"+steps.Count+"IsPlaying:"+_isPlaying+" dinosaurName:"+dinosaurName);
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
                    Debug.Log("传入信息：" + steps[i]);
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
                //if (_lanInput.Count > 0)
                    KongLongStraight(_lan.gameObject, 0, _ani3.gameObject.activeSelf ? 2 : 4, gameSteps);
            }

            if (dinosaur == _hei.gameObject)
            {
                //if (_heiInput.Count > 0)
                    KongLongStraight(_hei.gameObject, 0, _ani3.gameObject.activeSelf ? 5 : 9, gameSteps);
            }

            if (dinosaur == _hong.gameObject)
            {
                //if (_hongInput.Count > 0)
                    KongLongStraight(_hong.gameObject, 0, _ani3.gameObject.activeSelf ? 8 : 14, gameSteps);
            }

            if (dinosaur == _lv.gameObject)
            {
                //if (_lvInput.Count > 0)
                    KongLongStraight(_lv.gameObject, 0, _ani3.gameObject.activeSelf ? 11 : 19, gameSteps);
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

        //获取现在在哪个格子中
        private int GetInBlock(GameObject o)
        {
            Transform block;
            if (_ani3.gameObject.activeSelf)
                block = _block;
            else
                block = _block2;

            float dis = Vector2.Distance(o.transform.position, block.GetChild(0).position);
            int lastIndex = 0;
            for (int i = 0; i < block.childCount; i++)
            {
                if (Vector2.Distance(o.transform.position, block.GetChild(i).position) <= dis)
                {
                    dis = Vector2.Distance(o.transform.position, block.GetChild(i).position);
                    lastIndex = i;
                }
            }

            return lastIndex;
        }

        //执行命令效果
        private void KongLongStraight(GameObject konglong, int count, int MaxIndex, List<InputType> list)
        {
            if(_dizzyingDic.ContainsKey(konglong)&&_dizzyingDic[konglong])
                return;
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            InputType input = list[count];
            Debug.Log("走了" + list.Count + "步");
            if (input == InputType.Straight)
            {
                int blockIndex = GetInBlock(konglong);
                if (blockIndex == MaxIndex)
                {
                    _canClick = false;
                    _isDinosuarWalkingDic[konglong.name] = false;
                    JudgeWinAndeExcuteNext();
                    return;
                }
                else
                {
                    float endX = _ani3.gameObject.activeSelf ? _block.GetChild(blockIndex + 1).position.x : _block2.GetChild(blockIndex + 1).position.x;
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "2", true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    Tween tw = konglong.transform.DOMoveX(endX, 1.0f).OnComplete(
                    () =>
                    {
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name, true);
                        Wait(
                        () => 
                        {
                            if (count + 1 < list.Count)
                                KongLongStraight(konglong, (count + 1), MaxIndex, list);
                            else
                            {
                                //Debug.Log("当恐龙走完的时候发起判断");
                                int blockIndex2 = GetInBlock(konglong);
                                if (blockIndex2 >= MaxIndex)
                                {
                                    _canClick = false;
                                    _isDinosuarWalkingDic[konglong.name] = false;
                                    JudgeWinAndeExcuteNext();
                                }
                                else
                                {
                                    _dizzyingDic[konglong] = true;
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false,
                                        () =>
                                        {
                                            switch (konglong.name)
                                            {
                                                case "lan":
                                                    _lan.transform.position = _ani3.gameObject.activeSelf
                                                        ? _block.GetChild(0).position
                                                        : _block2.GetChild(0).position;
                                                    break;
                                                case "hei":
                                                    _hei.transform.position = _ani3.gameObject.activeSelf
                                                        ? _block.GetChild(3).position
                                                        : _block2.GetChild(5).position;
                                                    break;
                                                case "hong":
                                                    _hong.transform.position = _ani3.gameObject.activeSelf
                                                        ? _block.GetChild(6).position
                                                        : _block2.GetChild(10).position;
                                                    break;
                                                case "lv":
                                                    _lv.transform.position = _ani3.gameObject.activeSelf
                                                        ? _block.GetChild(9).position
                                                        : _block2.GetChild(15).position;
                                                    break;

                                            }
                                            _isDinosuarWalkingDic[konglong.name] = false;
                                            _dizzyingDic[konglong] = false;
                                            SpineManager.instance.DoAnimation(
                                                konglong.transform.GetGameObject("cemian"), konglong.name, true);
                                            
                                        });
                                   
                                }
                            }
                        }, 0.5f);
                    });
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "8", false, 
                ()=> 
                {
                    switch (konglong.name)
                    {
                        case "lan":
                            _lan.transform.position = _ani3.gameObject.activeSelf
                                ? _block.GetChild(0).position
                                : _block2.GetChild(0).position;
                            break;
                        case "hei":
                            _hei.transform.position = _ani3.gameObject.activeSelf
                                ? _block.GetChild(3).position
                                : _block2.GetChild(5).position;
                            break;
                        case "hong":
                            _hong.transform.position = _ani3.gameObject.activeSelf
                                ? _block.GetChild(6).position
                                : _block2.GetChild(10).position;
                            break;
                        case "lv":
                            _lv.transform.position = _ani3.gameObject.activeSelf
                                ? _block.GetChild(9).position
                                : _block2.GetChild(15).position;
                            break;

                    }
                    _isDinosuarWalkingDic[konglong.name] = false;
                    _dizzyingDic[konglong] = false;
                    SpineManager.instance.DoAnimation(
                        konglong.transform.GetGameObject("cemian"), konglong.name, true);
                    
                    _canClick = true;
                    return;
                });
            }
        }

        void JudgeWinAndeExcuteNext()
        {
            if (IsWin() == true && _alreadyWin == false)
            {
                _canClick = false;
                _alreadyWin = true;
                _isPlaying = false;
                WinKongLong();
                Wait(
                    () =>
                    {
                        SoundManager.instance.Stop("sound");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        _mask.SetActive(true);
                        _uiE.Show();
                        InitAni(_uiE);
                        _canClick = false;
                        SpineManager.instance.DoAnimation(_uiE, "uiE", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_uiE, "uiE2", true);
                                if (_ani3.gameObject.activeSelf)
                                {
                                    InitDinosaurWalkingStatu(true);
                                    _canClick = false;
                                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 8, null,
                                        () =>
                                        {
                                            InitDinosaurWalkingStatu(false);
                                            _canClick = true;
                                        }));
                                    
                                    _xyg.Show();
                                    _next.Show();
                                    InitAni(_xyg);
                                    SpineManager.instance.DoAnimation(_xyg, "xyg", false);
                                }
                                else
                                {
                                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 10, null, null));
                                }
                            });
                    }, 2.0f);
            }
        }


        //判断是否获胜
        private bool IsWin()
        {
            //Debug.Log("开始判断");
            List<GameObject> bindDinasourList = new List<GameObject>();
            for (int i = 0; i < TabletWebsocketController.Instance.CurrDevices.Length; i++)
            {
                string deviceAdress = TabletWebsocketController.Instance.CurrDevices[i].adress;
                // if(!deviceName.Contains("OPB"))
                //     continue;
                // string nameString = deviceName.Split(new string[] { "OPB" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                Debug.Log("最后的Device Adress为：" + deviceAdress);
                foreach (var bindDevice in TabletWebsocketController.Instance.AnimalBind2Device)
                {
                    Debug.Log("BindDevice.Value:" + bindDevice.Value);
                    if (bindDevice.Value == deviceAdress)
                    {
                        bindDinasourList.Add(FindDinosaurObjectByName(bindDevice.Key));
                        Debug.Log("加入之后的物体为：" + FindDinosaurObjectByName(bindDevice.Key).name);
                    }
                }
            }

            //Debug.Log("绑定的恐龙的数量为："+bindDinasourList.Count);
            if (_ani3.gameObject.activeSelf)
            {
                int winCount = 0;
                for (int i = 0; i < bindDinasourList.Count; i++)
                {
                    GameObject target = bindDinasourList[i];
                    //Debug.Log(target.name+"GetInBlick:"+ GetInBlock(target)+" lan:"+_lan.name);
                    if (target.name == _lan.name && GetInBlock(_lan.gameObject) == 2
                        || target.name == _hei.name && GetInBlock(_hei.gameObject) == 5
                        || target.name == _hong.name && GetInBlock(_hong.gameObject) == 8
                        || target.name == _lv.name && GetInBlock(_lv.gameObject) == 11
                    )
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        target.transform.GetGameObject("zhengmian").Show();
                        target.transform.GetGameObject("cemian").Hide();
                        SpineManager.instance.DoAnimation(target.transform.GetGameObject("zhengmian"),
                            target.name + "2", true);
                        _isDinosuarWalkingDic[target.name] = true;
                        winCount++;
                    }

                    // else if (target.name == _hei.name && GetInBlock(_hei.gameObject) == 5)
                    // {
                    //     winCount++;
                    // }
                    // else if (target.name == _hong.name && GetInBlock(_hong.gameObject) == 8)
                    // {
                    //     winCount++;
                    // }
                    // else if (target.name == _lv.name && GetInBlock(_lv.gameObject) == 11)
                    // {
                    //     winCount++;
                    // }
                }
                Debug.Log("WinCount:"+winCount+" BindDinosaur.Count"+bindDinasourList.Count);
                if (winCount.Equals(bindDinasourList.Count))
                {
                    return true;
                }
                else
                {
                    return false;
                }

                // if (GetInBlock(_lan.gameObject) == 2 && GetInBlock(_hei.gameObject) == 5 && GetInBlock(_hong.gameObject) == 8 && GetInBlock(_lv.gameObject) == 11)
                //     return true;
                // else
                //     return false;
            }
            else
            {
                int winCount = 0;
                for (int i = 0; i < bindDinasourList.Count; i++)
                {
                    GameObject target = bindDinasourList[i];
                    if (target.name == _lan.name && GetInBlock(_lan.gameObject) == 4
                    || target.name == _hei.name && GetInBlock(_hei.gameObject) == 9
                    || target.name == _hong.name && GetInBlock(_hong.gameObject) == 14
                    || target.name == _lv.name && GetInBlock(_lv.gameObject) == 19)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        target.transform.GetGameObject("zhengmian").Show();
                        target.transform.GetGameObject("cemian").Hide();
                        SpineManager.instance.DoAnimation(target.transform.GetGameObject("zhengmian"), target.name+"2", true);
                        _isDinosuarWalkingDic[target.name] = true;
                        winCount++;
                    }
                    // else if (target.name == _hei.name && GetInBlock(_hei.gameObject) == 9)
                    // {
                    //     SpineManager.instance.DoAnimation(target.transform.GetGameObject("zhengmian"), target.name + "2", true);
                    //     winCount++;
                    // }
                    // else if (target.name == _hong.name && GetInBlock(_hong.gameObject) == 14)
                    // {
                    //     SpineManager.instance.DoAnimation(target.transform.GetGameObject("zhengmian"), target.name + "2", true);
                    //     winCount++;
                    // }
                    // else if (target.name == _lv.name && GetInBlock(_lv.gameObject) == 19)
                    // {
                    //     SpineManager.instance.DoAnimation(target.transform.GetGameObject("zhengmian"), target.name + "2", true);
                    //     winCount++;
                    // }
                }

                if (winCount.Equals(bindDinasourList.Count))
                {
                    return true;
                }
                else
                {
                    return false;
                }
                // if (GetInBlock(_lan.gameObject) == 4 && GetInBlock(_hei.gameObject) == 9 && GetInBlock(_hong.gameObject) == 14 && GetInBlock(_lv.gameObject) == 19)
                //     return true;
                // else
                //     return false;
            }
        }

        //胜利时的恐龙动画效果
        void WinKongLong()
        {
            // for (int i = 0; i < 4; i++)
            // {
            //     _konglong.GetChild(i).GetChild(0).gameObject.SetActive(false);
            //     _konglong.GetChild(i).GetChild(1).gameObject.SetActive(true);
            // }
            // SpineManager.instance.DoAnimation(_lan.transform.GetGameObject("zhengmian"), "lan2", true);
            // SpineManager.instance.DoAnimation(_hei.transform.GetGameObject("zhengmian"), "hei2", true);
            // SpineManager.instance.DoAnimation(_hong.transform.GetGameObject("zhengmian"), "hong2", true);
            // SpineManager.instance.DoAnimation(_lv.transform.GetGameObject("zhengmian"), "lv2", true);
        }

        //点击下一关
        private void ClickNext(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(_xyg, "xyg2", false, 
                ()=> 
                {
                    for (int i = 0; i < 4; i++)
                    {
                        _konglong.GetChild(i).GetChild(0).gameObject.SetActive(true);
                        _konglong.GetChild(i).GetChild(1).gameObject.SetActive(false);
                    }
                    bell.Show();
                    _uiE.Hide();
                    _xyg.Hide();
                    _next.Hide();
                    _mask.SetActive(false);
                    ChangeBg(3);
                    _ani3.gameObject.Hide();
                    _ani4.gameObject.Show();
                    _topLevelAni3Panel.gameObject.Hide();
                    _topLevelAni4Panel.gameObject.Show();
                    _lan.transform.position = _block2.GetChild(0).position;
                    _hei.transform.position = _block2.GetChild(5).position;
                    _hong.transform.position = _block2.GetChild(10).position;
                    _lv.transform.position = _block2.GetChild(15).position;

                    InitAni(_lan.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_lan.GetGameObject("cemian"), "lan", true);
                    InitAni(_hei.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_hei.GetGameObject("cemian"), "hei", true);
                    InitAni(_hong.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_hong.GetGameObject("cemian"), "hong", true);
                    InitAni(_lv.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(_lv.GetGameObject("cemian"), "lv", true);

                    _alreadyWin = false;
                    ClearInputList();
                    
                    //测试用，记得删除
                    //TestInput();
                    AddInputList(InputType.Straight, _lanInput);
                    _isPlaying = false;
                    InitAni(bell);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 09, null, () => { bell.Hide();  _canClick = true;
                        _isPlaying = true;
                        InitDinosaurWalkingStatu(false);
                    }));
                });
            }
        }

        //测试用
        //void TestInput()
        //{
        //    AddInputList(InputType.Straight, _lanInput);
        //    AddInputList(InputType.Straight, _lanInput);
        //    AddInputList(InputType.Straight, _heiInput);
        //    AddInputList(InputType.Left, _heiInput);
        //    AddInputList(InputType.Straight, _hongInput);
        //    AddInputList(InputType.Right, _hongInput);
        //    AddInputList(InputType.Straight, _lvInput);
        //}
        #endregion
    }
}
