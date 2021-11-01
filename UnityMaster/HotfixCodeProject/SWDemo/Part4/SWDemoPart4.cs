using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework.Scripts.Utility;
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

    public class SWDemoPart4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject _area;
        private GameObject _area2;
        private Transform _panel2;
        private Transform _drag1;
        private Transform _drag2;
        private mILDrager[] _dragArray1;
        private mILDrager[] _dragArray2;
        private Transform _drop;
        private Transform _pos;
        private Transform _UI;
        private GameObject _uiA;
        private GameObject _uiB;

        private Transform _block;
        private Transform _konglong;
        private Transform _bawang;
        private Transform _ji;
        private Transform _sha;
        private Transform _yong;
        private KongLongDirection _bawangDirection;
        private KongLongDirection _jiDirection;
        private KongLongDirection _shaDirection;
        private KongLongDirection _yongDirection;
        private List<InputType> _bawangInput;
        private List<InputType> _jiInput;
        private List<InputType> _shaInput;
        private List<InputType> _yongInput;

        private GameObject _curControl;
        private GameObject _clickZhiXing;
        private bool _canClick;
        private int _row;
        private int _col;

        private MonoScripts _monoScripts;
        private GameObject _mask;
        private Transform Drag1pos;
        private List<int> _list;
        private Vector3 _vector3temp;

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

        private bool _isPlaying;
        private GameObject _replayBtn;

        private GameObject _meatGameObject;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            _mask = curTrans.Find("UI").Find("mask").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            _monoScripts = Bg.GetComponent<MonoScripts>();
            _monoScripts.OnDisableCallBack = OnDisable;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            _area = curTrans.Find("Panel/Area").gameObject;
            _area2 = curTrans.Find("Panel/Area2").gameObject;
            _meatGameObject = _area2.transform.GetGameObject("rou");
            _panel2 = curTrans.Find("Panel2");
            _drag1 = curTrans.Find("Drag1");
            _drag2 = curTrans.Find("Drag2");
            _dragArray1 = new mILDrager[_drag1.childCount];
            _dragArray2 = new mILDrager[_drag2.childCount];
            for (int i = 0; i < _drag1.childCount; i++)
            {
                _dragArray1[i] = _drag1.GetChild(i).GetComponent<mILDrager>();
                _dragArray1[i].SetDragCallback(null, null, null, null);
            }
            for (int i = 0; i < _drag2.childCount; i++)
            {
                _dragArray2[i] = _drag2.GetChild(i).GetComponent<mILDrager>();
                _dragArray2[i].SetDragCallback(null, null, null, null);
            }
            _drop = curTrans.Find("Drop");
            _pos = curTrans.Find("Pos");
            _UI = curTrans.Find("UI");
            _uiA = _UI.GetGameObject("uiA");
            _uiB = _UI.GetGameObject("uiB");

            _block = curTrans.Find("Block");
            _konglong = curTrans.Find("konglong");
            _bawang = _konglong.Find("a");
            _ji = _konglong.Find("b");
            _sha = _konglong.Find("c");
            _yong = _konglong.Find("d");
            _replayBtn = curTrans.GetGameObject("Replay");
            _bawangInput = new List<InputType>();
            _jiInput = new List<InputType>();
            _shaInput = new List<InputType>();
            _yongInput = new List<InputType>();

            _clickZhiXing = curTrans.GetGameObject("click/click");
            Util.AddBtnClick(_clickZhiXing, ClickZhiXing);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _isDinosuarWalkingDic=new Dictionary<string, bool>();

            _pbConnectedDinosaurList = new List<GameObject>();
            _choosePanel = curTrans.GetTransform("Choose");
            _deviceObjectDic = new Dictionary<string, GameObject>();
            _dinosaurDic = new Dictionary<string, GameObject>();
            _isBindTheDinosaur = new Dictionary<GameObject, bool>();
            _pbDeviceNameTransferGameName = new Dictionary<string, string>();
            TabletWebsocketController.Instance.AnimalBind2Device.Clear();
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
            
            for (int i = 0; i < _drag2.childCount; i++)
            {
                GameObject target = _drag2.GetChild(i).gameObject;
                _isBindTheDinosaur.Add(target, false);
                PointerClickListener.Get(target).clickDown = ClickDinosaurAndOpenTheChoosePanel;
            }
            for (int i = 0; i < _devicePanel.childCount; i++)
            {
                Transform targetItem = _devicePanel.GetChild(i);
                targetItem.GetTransform("xx").gameObject.Hide();
                targetItem.gameObject.SetActive(false);
            }
            
            _confirmBtn = _choosePanel.GetGameObject("Btn");
            PointerClickListener.Get(_confirmBtn).clickDown = ConfirmSelectedDinosaur;
            TabletWebsocketController.DinosaurMove = DinosaurMove;
            TabletWebsocketController.Instance.GetBleDevices();
            TabletWebsocketController.Instance.SubscribeToGetDeviceMsg();

            Drag1pos = curTrans.Find("Drag1pos");
            _list = new List<int>();
            for (int i = 0; i < 6; i++)
            {
                _list.Add(i);
            }
            //TabletWebsocketController.Instance.ClearAllDeviceInfo();
            SetReplayBtnVisible(true);
            GameObject _replayBtnc = _replayBtn.transform.GetGameObject("Btn");
            PointerClickListener.Get(_replayBtnc).clickDown = ReplayEvent;
            // LogicManager.instance.SetReplayEvent(() =>
            // {
            //     //GameInit();
            //     //GameStart();
            //     NextLevel();
            //     CanDrag2();
            //     LogicManager.instance.ShowReplayBtn(false);
            // });
            // LogicManager.instance.ShowReplayBtn(false);
            SetReplayBtnVisible(false);
            GameInit();
            GameStart();
        }

        void ReplayEvent(GameObject go)
        {
            _meatGameObject.Show();
            NextLevel();
            CanDrag2();
            SetReplayBtnVisible(false);
        }

        void SetReplayBtnVisible(bool isShow)
        {
            if (_replayBtn.activeSelf != isShow)
                _replayBtn.SetActive(isShow);
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

        void PlaySound(int index)
        {
            SoundManager.instance.Stop("sound");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, false);
        }

        #region 编程版连接代码


        /// <summary>
        /// 点击选择相应的编程版
        /// </summary>
        /// <param name="target">编程版</param>
        private void ClickSelectItem(GameObject target)
        {
            if (target.activeSelf)
            {
                PlaySound(7);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
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
                    //string nameString = deviceName.Split(new string[] { "OPB" }, System.StringSplitOptions.RemoveEmptyEntries)[0];
                    Debug.Log("Device adress:" + deviceAdress);
                    bool isMatchTheBindAdress = false;
                    foreach (var keyValue in TabletWebsocketController.Instance.AnimalBind2Device)
                    {
                        string targetValue = keyValue.Value;
                        //Debug.Log("keyValue:"+ targetValue + "    kkkk:"+deviceName);
                        // if (_pbDeviceNameTransferGameName.ContainsKey(targetValue))
                        // {
                        //     isMatchTheBindAdress = true;
                        // }
                        if (deviceAdress.Equals(targetValue))
                            isMatchTheBindAdress = true;
                    }
                    if (!_pbDeviceNameTransferGameName.ContainsKey(deviceAdress) && !isMatchTheBindAdress)
                    {
                        Debug.Log("加入地址：" + deviceAdress + " index:" + dinosaurIndex);
                        _pbDeviceNameTransferGameName.Add(deviceAdress, dinosaurIndex.ToString());
                        dinosaurIndex++;
                    }
                    // else
                    // {
                    //     _pbDeviceNameTransferGameName[deviceAdress] = dinosaurIndex.ToString();
                    //     dinosaurIndex++;
                    // }




                    //Debug.Log("编程板名字“：" + nameString);



                }



                for (int j = 0; j < _devicePanel.childCount; j++)
                {
                    Transform device = _devicePanel.GetChild(j);
                    device.GetGameObject("xx").Hide();
                    string deviceAdress = GetDeviceAdressByPanelItemName(device.name);
                    if (deviceAdress != String.Empty && _pbDeviceNameTransferGameName[deviceAdress].Equals(device.name))
                    {
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
                    InitDrag2();
                }
                else
                {
                    Debug.Log("绑定错误，请检查!" + _choseDinosaur.name);
                }
            }
            _choosePanel.gameObject.Hide();
        }


        #endregion



        private void OnDisable()
        {
            for (int i = 0; i < _drag1.childCount; i++)
            {
                Transform tran = _drag1.Find(i.ToString());
                tran.SetSiblingIndex(i);
            }

            _drag2.Find("bawang").SetSiblingIndex(0);
            _drag2.Find("ji").SetSiblingIndex(1);
            _drag2.Find("sha").SetSiblingIndex(2);
            _drag2.Find("yong").SetSiblingIndex(3);
        }

        private void DinosaurMove(string dinosaurName, List<string> steps)
        {
            if(!_isPlaying)
                return;
            
            if (!_isDinosuarWalkingDic.ContainsKey(dinosaurName))
                _isDinosuarWalkingDic.Add(dinosaurName, false);
            if(_isDinosuarWalkingDic[dinosaurName])
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
            GameObject dinosaur = FindDinosaurObjectByName(dinosaurName);
            if (gameSteps.Count <= 0)
            {
                _isDinosuarWalkingDic[dinosaurName] = false;
                return;
            }
            if (dinosaur == _curControl)
            {
                //Debug.Log("符合：");
                KongLongRun(_curControl, gameSteps);
            }
            // if (_curControl == _bawang.gameObject)
            // {
            //     KongLongRun(_bawang.gameObject, gameSteps);
            // }
            //
            // if (_curControl == _ji.gameObject)
            // {
            //     KongLongRun(_ji.gameObject, gameSteps);
            // }
            //
            // if (_curControl == _sha.gameObject)
            // {
            //     KongLongRun(_sha.gameObject, gameSteps);
            // }
            //
            // if (_curControl == _yong.gameObject)
            // {
            //     KongLongRun(_yong.gameObject, gameSteps);
            // }
        }

        GameObject FindDinosaurObjectByName(string name)
        {
            GameObject target = null;
            switch (name)
            {
                case "bawang":
                    target = _bawang.gameObject;
                    break;
                case "ji":
                    target = _ji.gameObject;
                    break;
                case "sha":
                    target = _sha.gameObject;
                    break;
                case "yong":
                    target = _yong.gameObject;
                    break;
            }
            // switch (name)
            // {
            //     case "lan":
            //         target = _bawang.gameObject;
            //         break;
            //     case "hei":
            //         target = _ji.gameObject;
            //         break;
            //     case "hong":
            //         target = _sha.gameObject;
            //         break;
            //     case "lv":
            //         target = _yong.gameObject;
            //         break;
            // }

            return target;
        }

        private void GameInit()
        {
            _isPlaying = false;
            _mask.SetActive(false);
            dinosaurIndex = 1;
            talkIndex = 1;
            _row = 5;
            _col = 3;
            _canClick = false;

            _area.Show();
            _meatGameObject.Show();
            _area2.Hide();
            //_panel2.GetGameObject("1").Show();
            _panel2.GetGameObject("2").Hide();
            _drag1.gameObject.Show();
            _drag2.gameObject.Hide();
            _konglong.gameObject.Hide();
            _UI.gameObject.Hide();
            _uiA.Hide();
            _uiB.Hide();
            _choosePanel.gameObject.Hide();
            InitDinosaurWalkingStatu();
            InitDrag1();
            StopDrag1();
        }

        void GameStart()
        {
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { bell.SetActive(false); CanDrag1(); }));
            SoundManager.instance.Stop("bgm");
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

        int SuccessVoice()
        {
            return Random.Range(1, 4);
        }

        int FalseVoice()
        {
            return Random.Range(4, 7);
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

        #region 拼图游戏

        void InitDrag1()
        {
            for (int i = 0; i < _dragArray1.Length; i++)
            {
                Debug.Log(_dragArray1[i]);

                _dragArray1[i].SetDragCallback(StartDrag1, null, EndDrag1, null);

                Transform tra = _dragArray1[i].transform;
                tra.position = _pos.Find(tra.name).position;

                //随机位置
                int temp = _list[Random.Range(0, _list.Count)];
                _list.Remove(temp);
                tra.position = new Vector2(tra.position.x,Drag1pos.Find(temp.ToString()).position.y);
                
                SetScale(tra.GetChild(0).gameObject, new Vector3(0.37f, 0.37f, 1));
                tra.GetChild(0).localPosition = new Vector3(0, 0, 0);
                tra.GetComponent<Empty4Raycast>().raycastTarget = true;
                _dragArray1[i].canMove = true;
            }
        }

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        /// <param name="dragBool"></param>
        private void StartDrag1(Vector3 dragPos, int dragType, int dragIndex)
        {
            Transform trans = _dragArray1[dragIndex].transform;
            trans.SetAsLastSibling();
            _vector3temp = trans.position;
        }

        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        /// <param name="dragBool"></param>
        private void EndDrag1(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (!_dragArray1[dragIndex].canMove)
                return;
            
            if (dragBool)
            {
                PlaySound(6);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, SuccessVoice(), null,
                    () => 
                    {
                        if (AllTrue())
                        {
                            //拼图完成
                            PlaySound(1);
                            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                            bell.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 7, null,
                            () =>
                            {
                                bell.SetActive(false);
                                NextLevel();
                                CanDrag2();
                            }));

                            _UI.gameObject.Show();
                            _mask.SetActive(true);
                            _uiA.Show();
                            InitAni(_uiA);
                            SpineManager.instance.DoAnimation(_uiA, "uiA", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_uiA, "uiA2", true);
                                //LogicManager.instance.ShowReplayBtn(true);
                            });
                        }
                        else
                            CanDrag1();
                    }
                    ));
                StopDrag1();
                _dragArray1[dragIndex].canMove = false;
                Transform trans = _dragArray1[dragIndex].transform.GetChild(0);
                Tween tw = trans.DOMove(_drop.Find(trans.name).position, 1.2f).OnComplete
                (() =>
                {
                    //if (AllTrue())
                    //{
                    //    //拼图完成
                    //    bell.SetActive(true);
                    //    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 7, null,
                    //    () =>
                    //    {
                    //        bell.SetActive(false);
                    //        NextLevel();
                    //        CanDrag2();
                    //     }));

                    //    _UI.gameObject.Show();
                    //    _uiA.Show();
                    //    InitAni(_uiA);
                    //    SpineManager.instance.DoAnimation(_uiA, "uiA", false, 
                    //    () => 
                    //    {
                    //        SpineManager.instance.DoAnimation(_uiA, "uiA2", true);
                    //    });
                    //}
                    //else
                    //    CanDrag1();
                });
                tw.SetEase(Ease.Linear);
                trans.DOScale(new Vector3(1, 1, 1), 1.0f);
            }
            else
            {
                PlaySound(9);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9, false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, FalseVoice(), null, null));
                Transform trans = _dragArray1[dragIndex].transform;
                trans.position = _pos.Find(trans.name).position;

                trans.position = _vector3temp;
            }
        }

        /// <summary>
        /// 允许拖拽
        /// </summary>
        void CanDrag1()
        {
            for (int i = 0; i < _dragArray1.Length; i++)
                _dragArray1[i].transform.GetComponent<Empty4Raycast>().raycastTarget = true;
        }

        /// <summary>
        /// 不允许拖拽
        /// </summary>
        void StopDrag1()
        {
            for (int i = 0; i < _dragArray1.Length; i++)
                _dragArray1[i].transform.GetComponent<Empty4Raycast>().raycastTarget = false;
        }

        /// <summary>
        /// 判断拼图是否已全部拼完
        /// </summary>
        /// <returns></returns>
        bool AllTrue()
        {
            bool win = true;
            for (int i = 0; i < _dragArray1.Length; i++)
            {
                if (_dragArray1[i].canMove == true)
                {
                    win = false;
                    return win;
                }
            }

            return win;
        }

        #endregion

        #region 拖拽恐龙

        /// <summary>
        /// 下一关
        /// </summary>
        void NextLevel()
        {
            _mask.SetActive(false);
            _area2.Show();
            _area.Hide();
            _uiA.Hide();
            _UI.gameObject.Hide();
            _panel2.GetGameObject("2").Show();
            _panel2.GetGameObject("1").Hide();
            _drag2.gameObject.Show();
            _drag1.gameObject.Hide();

            for (int i = 0; i < _drag2.childCount; i++)
                _drag2.GetChild(i).gameObject.Show();
            
            InitDinosaurWalkingStatu();

            InitDrag2();
            StopDrag2();
        }

        void InitDrag2()
        {
            for (int i = 0; i < _dragArray2.Length; i++)
            {
               

                Transform tra = _dragArray2[i].transform;
                tra.position = _pos.Find(tra.name).position;
                // if (tra.name == "bawang" && TabletWebsocketController.Instance.AnimalBind2Device.ContainsKey("lan")
                //     || tra.name == "ji" && TabletWebsocketController.Instance.AnimalBind2Device.ContainsKey("hei")
                //     || tra.name == "sha" && TabletWebsocketController.Instance.AnimalBind2Device.ContainsKey("hong")
                //     || tra.name == "yong" && TabletWebsocketController.Instance.AnimalBind2Device.ContainsKey("lv")
                // )
                // {
                //     _dragArray2[i].SetDragCallback(StartDrag2, null, EndDrag2, null);
                //     tra.GetComponent<Empty4Raycast>().raycastTarget = true;
                // }
                Debug.Log("初始化的时候 tra的名字：" + tra.name);
                if (TabletWebsocketController.Instance.AnimalBind2Device.ContainsKey(tra.name))
                {
                    Debug.Log("在绑定的名单里面 tra的名字：" + tra.name);
                    _dragArray2[i].canMove = true;
                    _dragArray2[i].SetDragCallback(StartDrag2, null, EndDrag2, null);
                    tra.GetComponent<Empty4Raycast>().raycastTarget = true;
                    tra.GetComponent<PointerClickListener>().enabled = false;
                }
                else
                {
                    _dragArray2[i].canMove = false;
                    //tra.GetComponent<Empty4Raycast>().raycastTarget = false;
                    tra.GetComponent<PointerClickListener>().enabled = true;
                }

            }
        }

        

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        /// <param name="dragBool"></param>
        private void StartDrag2(Vector3 dragPos, int dragType, int dragIndex)
        {
            Transform trans = null;
            if(dragIndex == 6)
                trans = _drag2.Find("bawang");
            else if (dragIndex == 7)
                trans = _drag2.Find("ji");
            else if (dragIndex == 8)
                trans = _drag2.Find("sha");
            else
                trans = _drag2.Find("yong");
            trans.SetAsLastSibling();
        }

        private void EndDrag2(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if(dragBool)
            {
                PlaySound(4);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                StopDrag2();
                Transform konglong = _bawang;
                string name = null;
                switch (dragIndex)
                {
                    case 6:
                        konglong = _bawang;
                        name = "bawang";
                        break;
                    case 7:
                        konglong = _ji;
                        name = "ji";
                        break;
                    case 8:
                        konglong = _sha;
                        name = "sha";
                        break;
                    case 9:
                        konglong = _yong;
                        name = "yong";
                        break;
                }

                _drag2.GetGameObject(name).Hide();
                InitKongLong(konglong);
            }
            else
            {
                Transform tra = null;
                if (dragIndex == 6)
                    tra = _drag2.Find("bawang");
                else if (dragIndex == 7)
                    tra = _drag2.Find("ji");
                else if (dragIndex == 8)
                    tra = _drag2.Find("sha");
                else
                    tra = _drag2.Find("yong");
                tra.position = _pos.Find(tra.name).position;
            }
        }

        /// <summary>
        /// 允许拖拽
        /// </summary>
        void CanDrag2()
        {
            for (int i = 0; i < _dragArray2.Length; i++)
                _dragArray2[i].transform.GetComponent<Empty4Raycast>().raycastTarget = true;
        }

        /// <summary>
        /// 不允许拖拽
        /// </summary>
        void StopDrag2()
        {
            for (int i = 0; i < _dragArray2.Length; i++)
                _dragArray2[i].transform.GetComponent<Empty4Raycast>().raycastTarget = false;
        }

        #endregion

        #region 恐龙行进

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
            _bawangInput.Clear();
            _jiInput.Clear();
            _shaInput.Clear();
            _yongInput.Clear();
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


        void LoseGame()
        {
            PlaySound(2);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            _isPlaying = false;
            _mask.Show();
            _UI.gameObject.Show();
            _uiB.Show();
            InitAni(_uiB);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 9, null, 
            () => 
            {
                if (EndOrNextControl())
                {
                    return;
                }
                else
                {
                    _konglong.gameObject.Hide();
                    _UI.gameObject.Hide();
                    _uiB.Hide();
                    CanDrag2();
                }
            }));
            SpineManager.instance.DoAnimation(_uiB, "uiC", false,
            () =>
            {
                SpineManager.instance.DoAnimation(_uiB, "uiC2", true);
                //LogicManager.instance.ShowReplayBtn(true);
                SetReplayBtnVisible(true);
                _isDinosuarWalkingDic[_curControl.name] = false;
                for (int i = 0; i < _konglong.childCount; i++)
                {
                    _konglong.GetChild(i).gameObject.Hide();
                }
            });
        }

        /// <summary>
        /// 胜利时的恐龙动画效果
        /// </summary>
        void WinKongLong(GameObject konglong)
        {
            PlaySound(3);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            _isPlaying = false;
            for (int i = 0; i < konglong.transform.childCount; i++)
                konglong.transform.GetChild(i).gameObject.Hide();
            konglong.transform.GetGameObject("zhengmian").Show();
            SetScale(konglong, new Vector3(1, 1, 1));
            InitAni(konglong.transform.GetGameObject("zhengmian"));
            _meatGameObject.Hide();
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 8, null,
            () =>
            {
                if (EndOrNextControl())
                {
                    return;
                }
                else
                {
                    _konglong.gameObject.Hide();
                    _UI.gameObject.Hide();
                    _uiB.Hide();
                    CanDrag2();
                }
            }));

            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "c3", true);
            _UI.gameObject.Show();
            _uiB.Show();
            InitAni(_uiB);
            _mask.Show();
            SpineManager.instance.DoAnimation(_uiB, "uiB", false, 
            () =>
            {
                SpineManager.instance.DoAnimation(_uiB, "uiB2", true);
                
                //LogicManager.instance.ShowReplayBtn(true);
                SetReplayBtnVisible(true);
                _isDinosuarWalkingDic[_curControl.name] = false;
              
                for (int i = 0; i < _konglong.childCount; i++)
                {
                    _konglong.GetChild(i).gameObject.Hide();
                }
            });
        }
        #endregion

        //void Test()
        //{
        //    _bawangInput.Add(InputType.Straight);
        //    _bawangInput.Add(InputType.Straight);
        //    _bawangInput.Add(InputType.Right);
        //    _bawangInput.Add(InputType.Straight);
        //    _bawangInput.Add(InputType.Straight);
        //    _bawangInput.Add(InputType.Left);
        //    _bawangInput.Add(InputType.Straight);
        //    _bawangInput.Add(InputType.Straight);

        //    _jiInput.Add(InputType.Straight);
        //    _jiInput.Add(InputType.Straight);
        //    _jiInput.Add(InputType.Right);
        //    _jiInput.Add(InputType.Right);
        //    _jiInput.Add(InputType.Right);
        //    _jiInput.Add(InputType.Right);
        //    _jiInput.Add(InputType.Right);
        //    _jiInput.Add(InputType.Straight);
        //    _jiInput.Add(InputType.Straight);
        //    _jiInput.Add(InputType.Left);
        //    _jiInput.Add(InputType.Straight);
        //    _jiInput.Add(InputType.Straight);

        //    _yongInput.Add(InputType.Straight);
        //    _yongInput.Add(InputType.Straight);
        //    _yongInput.Add(InputType.Right);
        //    _yongInput.Add(InputType.Straight);
        //    _yongInput.Add(InputType.Straight);

        //    _shaInput.Add(InputType.Straight);
        //    _shaInput.Add(InputType.Right);
        //    _shaInput.Add(InputType.Straight);
        //    _shaInput.Add(InputType.Right);
        //    _shaInput.Add(InputType.Straight);
        //    _shaInput.Add(InputType.Straight);
        //}

        void InitKongLong(Transform curkonglong)
        {
            _konglong.gameObject.Show();
            for (int i = 0; i < _konglong.childCount; i++)
                _konglong.GetChild(i).gameObject.Hide();
            curkonglong.gameObject.Show();
            curkonglong.transform.position = _block.GetChild(12).position;

            for (int i = 0; i < curkonglong.childCount; i++)
                curkonglong.GetChild(i).gameObject.Hide();
            curkonglong.GetGameObject("beimian").Show();

            _bawangDirection = KongLongDirection.back;
            _jiDirection = KongLongDirection.back;
            _yongDirection = KongLongDirection.back;
            _shaDirection = KongLongDirection.back;
            InitAni(curkonglong.GetGameObject("beimian"));
            SpineManager.instance.DoAnimation(curkonglong.GetGameObject("beimian"), curkonglong.name + "a2", true);

            _canClick = true;
            _curControl = curkonglong.gameObject;
            _isPlaying = true;
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
                if (_curControl == _bawang.gameObject)
                    nList = _bawangInput;
                else if (_curControl == _ji.gameObject)
                    nList = _jiInput;
                else if (_curControl == _sha.gameObject)
                    nList = _shaInput;
                else
                    nList = _yongInput;

                KongLongRun(_curControl, nList);
            }
        }

        private void KongLongRun(GameObject konglong, List<InputType> list)
        {
            if (list.Count > 0)
                ZhiXingInput(konglong, list, 0);
        }

        /// <summary>
        /// 执行输入的指令
        /// </summary>
        /// <param name="konglong"></param>
        /// <param name="list"></param>
        /// <param name="index"></param>
        private void ZhiXingInput(GameObject konglong, List<InputType> list, int index)
        {
            int _goalIndex = 2;     //目标位置
            
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            KongLongDirection curDirection;
            if (konglong.name == "a")
                curDirection = _bawangDirection;
            else if (konglong.name == "b")
                curDirection = _jiDirection;
            else if (konglong.name == "c")
                curDirection = _shaDirection;
            else
                curDirection = _yongDirection;

            if (curDirection == KongLongDirection.front)
            {
                if (list[index] == InputType.Left)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.right;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.right;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.right;
                    else
                        _yongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(-1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.left;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.left;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.left;
                    else
                        _yongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock == 7 || curBlock == 8 || curBlock == 12)
                    {
                        LoseGame();
                        return;
                    }
                    else
                    {
                        curBlock += _col;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "c1", true);
                        PlaySound(0);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "c2", true);
                            if (curBlock == _goalIndex)
                            {
                                WinKongLong(konglong);
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                                else
                                    LoseGame();
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.left;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.left;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.left;
                    else
                        _yongDirection = KongLongDirection.left;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("cemian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("cemian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.right;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.right;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.right;
                    else
                        _yongDirection = KongLongDirection.right;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock == 6 || curBlock == 7)
                    {
                        LoseGame();
                    }
                    else
                    {
                        curBlock -= _col;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "a1", true);
                        PlaySound(0);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "a2", true);
                           
                            if (curBlock == _goalIndex)
                            {
                                
                                WinKongLong(konglong);
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                                else
                                    LoseGame();
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "c2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.front;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.front;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.front;
                    else
                        _yongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("beimian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("beimian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "a2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.back;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.back;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.back;
                    else
                        _yongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock != 7 && curBlock != 8)
                    {
                        LoseGame();
                    }
                    else
                    {
                        curBlock -= 1;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "c1", true);
                        PlaySound(0);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "c2", true);
                            if (curBlock == _goalIndex)
                            {
                                WinKongLong(konglong);
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                                else
                                    LoseGame();
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
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("beimian"), konglong.name + "a2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.back;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.back;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.back;
                    else
                        _yongDirection = KongLongDirection.back;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        LoseGame();
                }
                else if (list[index] == InputType.Right)
                {
                    for (int i = 0; i < konglong.transform.childCount; i++)
                        konglong.transform.GetChild(i).gameObject.Hide();
                    konglong.transform.GetGameObject("zhengmian").Show();

                    SetScale(konglong, new Vector3(1, 1, 1));
                    InitAni(konglong.transform.GetGameObject("zhengmian"));
                    SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("zhengmian"), konglong.name + "c2", true);

                    if (konglong.name == "a")
                        _bawangDirection = KongLongDirection.front;
                    else if (konglong.name == "b")
                        _jiDirection = KongLongDirection.front;
                    else if (konglong.name == "c")
                        _shaDirection = KongLongDirection.front;
                    else
                        _yongDirection = KongLongDirection.front;

                    if ((index + 1) < list.Count)
                        Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                    else
                        _canClick = true;
                }
                else
                {
                    int curBlock = GetInBlock(konglong);
                    //超出行走范围
                    if (curBlock != 6 && curBlock != 7)
                    {
                        LoseGame();
                    }
                    else
                    {
                        curBlock += 1;
                        SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b1", true);
                        PlaySound(0);
                        konglong.transform.DOMove(_block.GetChild(curBlock).position, 1.0f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(konglong.transform.GetGameObject("cemian"), konglong.name + "b2", true);
                            if (curBlock == _goalIndex)
                            {
                                WinKongLong(konglong);
                            }
                            else
                            {
                                if ((index + 1) < list.Count)
                                    Wait(() => { ZhiXingInput(konglong, list, index + 1); }, 0.5f);
                                else
                                    LoseGame();
                            }
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 判断执行胜利操作或接力操作
        /// </summary>
        /// <param name="konglong"></param>
        private bool EndOrNextControl()
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

            bool kongLongVisible = true;
            for (int i = 0; i < bindDinasourList.Count; i++)
            {
                GameObject target = bindDinasourList[i];
                for (int j = 0; j < _drag2.childCount; j++)
                {
                    GameObject panelKonglong = _drag2.GetChild(i).gameObject;
                    if (target.name == panelKonglong.name && panelKonglong.activeSelf)
                    {
                        kongLongVisible = false;
                    }
                }
            }

            return kongLongVisible;

            // bool isEnd = true;
            // for (int i = 0; i < _drag2.childCount; i++)
            // {
            //     if(_drag2.GetChild(i).gameObject.activeSelf)
            //     {
            //         isEnd = false;
            //         return isEnd;
            //     }
            // }
            //
            // return isEnd;
        }
        #endregion
    }
}