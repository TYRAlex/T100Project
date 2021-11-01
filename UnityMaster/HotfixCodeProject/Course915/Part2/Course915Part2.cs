using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course915Part2
    {
        
        private enum E_BagType
        {
            WP1=1,
            WP2,
            WP3,
            WP4,
            WP5
        }

        private E_BagType _currentBage = E_BagType.WP1;
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private GameObject _bg2;
        private GameObject _statu;
        private GameObject _mainTarget;

        private Transform _objectParent;
        private Transform _objectButton;
        private Transform _bgFrame;
        private Transform _rightPos;
        private Transform _middlePos;
        private Transform _middlePos2;
        private Transform _leftPos;

        private GameObject _resultAni;
        private GameObject _resetButton;

        private GameObject _passButtonAni;
        private GameObject _failButtonAni;
        
        private GameObject _humanAni;
        private GameObject _humanAni2;

        private StringBuilder _currentTargetName;

        private List<GameObject> _objectList;

        private Dictionary<string, GameObject> _eachBagDic;

        private Dictionary<string, bool> _checkIfPass;

        private List<string[]> _bagInfoList;

        private List<GameObject> _bagButtonParentList;

        // private string[] _object1;
        // private string[] _object2;
        // private string[] _object3;
        // private string[] _object4;
        // private string[] _object5;

       
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
          
            ReStart();
        }

        void ReStart()
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);
            Bg = curTrans.Find("Bg").gameObject;
           
           InializedGameProperty();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();

        }

        /// <summary>
        /// 初始化所有的游戏属性
        /// </summary>
        void InializedGameProperty()
        {
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            LoadGameObject();
            SetGameInfo();
            _objectList=new List<GameObject>();
            _eachBagDic=new Dictionary<string, GameObject>();
            _checkIfPass=new Dictionary<string, bool>();
            _currentTargetName=new StringBuilder();
            _bagButtonParentList=new List<GameObject>();
            for (int i = 0; i < _objectParent.childCount; i++)
            {
                _objectList.Add(_objectParent.GetChild(i).gameObject);
            }

            for (int i = 0; i < _objectButton.childCount; i++)
            {
                Transform parent = _objectButton.GetChild(i);
                _bagButtonParentList.Add(parent.gameObject);
                for (int j = 0; j < parent.childCount; j++)
                {
                    var child = parent.GetChild(j).gameObject;
                    
                    PointerClickListener.Get(child).onClick = null;
                    PointerClickListener.Get(child).onClick = OnClickEvent;
                }
            }
            bell.transform.SetAsLastSibling();
            SetButtonClickVisible(true);
            PointerClickListener.Get(_failButtonAni.transform.GetGameObject("FailedButton")).onClick = OnClickEvent;
            PointerClickListener.Get(_passButtonAni.transform.GetGameObject("PassButton")).onClick = OnClickEvent;
            PointerClickListener.Get(curTrans.GetGameObject("ResetButton")).onClick = OnClickEvent;
            InializedStartStatu();   
        }

        /// <summary>
        /// 回到初始化状态
        /// </summary>
        void InializedStartStatu()
        {
            _currentBage = E_BagType.WP1;
            _humanAni.Hide();
            _humanAni2.Hide();
            _statu.Show();
            _statu.transform.SetAsFirstSibling();
            SpineManager.instance.DoAnimation(_statu, "JD1_ANIME", false);
            SetTheResultVisible(false,false);
            SpineManager.instance.DoAnimation(_mainTarget, "JIQI", false);
            InializedAllObject(_bagInfoList[0]);
            _objectParent.gameObject.Hide();
            SetButtonClickVisible(false);
            HideAllBagButton();
        }

        /// <summary>
        /// 设置游戏的物品信息
        /// </summary>
        void SetGameInfo()
        {
            _bagInfoList=new List<string[]>();
            _bagInfoList.Add(new[] {"WP1_DAO", "WP1_MAOZI", "WP1_QIANG", "WP1_YIFU"});
            _bagInfoList.Add(new[] {"WP2_MAOZI", "WP2_XIANGJI", "WP2_XIEZI", "WP2_YIFU"});
            _bagInfoList.Add(new[] {"WP3_MAOZI", "WP3_SHUIXIE", "WP3_YANJING", "WP3_YASHUA", "WP3_YUEQI"});
            _bagInfoList.Add(new[] {"WP4_MAOJIN", "WP4_MAOZI", "WP4_PENZAI", "WP4_WAZI"});
            _bagInfoList.Add(new[] {"WP5_JIU", "WP5_MAOZI", "WP5_PAO", "WP5_XIANGJIAO", "WP5_YIFU"});
            
            
            // _object1=new []{"WP1_DAO", "WP1_MAOZI", "WP1_QIANG", "WP1_YIFU"};
            // _object2 = new[] {"WP2_MAOZI", "WP2_XIANGJI", "WP2_XIEZI", "WP2_YIFU"};
            // _object3 = new[] {"WP3_MAOZI", "WP3_SHUXIE", "WP3_YANJING", "WP3_YASHUA", "WP3_YUEQI"};
            // _object4 = new[]  {"WP4_MAOJIN", "WP4_MAOZI", "WP4_PENZAI", "WP4_WAZI"};
            // _object5 = new[]  {"WP5_JIU", "WP5_MAOZI", "WP5_PAO", "WP5_XIANGJIAO", "WP5_YIFU"};
        }

        /// <summary>
        /// 加载游戏所需要的对象
        /// </summary>
        void LoadGameObject()
        {
            _humanAni = curTrans.GetGameObject("Human");
            _humanAni2 = curTrans.GetGameObject("Human2");
            _mainTarget = curTrans.GetGameObject("MainTarget");
            _statu = curTrans.GetGameObject("Statu");
            _resultAni = curTrans.GetGameObject("Result");
            _bg2 = curTrans.GetGameObject("BG2");
            _bgFrame=curTrans.GetTransform("ObjectInCase/BGFrame");
            _objectParent = _bgFrame.GetTransform("ObjectParent");
            _objectButton = _bgFrame.GetTransform("ObjectButton");
            _resetButton = curTrans.GetGameObject("ResetButton");
            _rightPos = _bgFrame.GetTransform("RightPos");
            _leftPos = _bgFrame.GetTransform("LeftPos");
            _middlePos = _bgFrame.GetTransform("MiddlePos");
            _middlePos2 = _bgFrame.GetTransform("MiddlePos2");
            _passButtonAni = curTrans.GetGameObject("Pass");
            _failButtonAni = curTrans.GetGameObject("Failed");
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
                () => SoundManager.instance.ShowVoiceBtn(true)));
            // _failButtonAni.transform.GetChild(0).gameObject.SetActive(true);
            // _passButtonAni.transform.GetChild(0).gameObject.Show();
            //StartCheckObject();
            //SetButtonClickVisible(true);
        }

        /// <summary>
        /// 开始检查第一个包
        /// </summary>
        void StartCheckObject()
        {
            _currentBage = E_BagType.WP1;
            _statu.transform.SetAsLastSibling();
            TurnToTheTargetBagStartStatu(1);
            // MoveToTheMiddle();
            // SpineManager.instance.DoAnimation(_mainTarget, "B_1_IN", false);
             
        }

        
        void Test(GameObject go)
        {
            SetObjectButtonVisible((int) _currentBage, true);
           
            TurnToTheTargetBagStartStatu((int) _currentBage);
            
        }

        void Test2(GameObject go)
        {
            int number = (int) _currentBage;
            //MoveToTheLeft(number);
            MoveToTheLeft(number);
            number++;
            _currentBage = (E_BagType) number;
        }

        /// <summary>
        /// 点击事件返回的方法
        /// </summary>
        /// <param name="go">点击的物体</param>
        private void OnClickEvent(GameObject go)
        {
            JudgeTargetAndExcuteNext(go);
        }

        /// <summary>
        /// 判断点击的物体，然后执行下一步
        /// </summary>
        /// <param name="go">点击的物体</param>
        void JudgeTargetAndExcuteNext(GameObject go)
        {
            if (String.Compare(go.name, "PassButton", StringComparison.Ordinal)==0)
            {
               PassButtonEvent();
            }
            else if (String.Compare(go.name, "FailedButton", StringComparison.Ordinal)==0)
            {
                FailButtonEvent();
            }
            else if (String.Compare(go.name, "ResetButton", StringComparison.Ordinal) == 0)
            {
                _checkIfPass.Clear();
                SetTheResultVisible(false);
                MoveToTheRight(() => TurnToTheTargetBagStartStatu((int) _currentBage, false));

            }
            else
            {
                _currentTargetName.Append(go.transform.parent.name+"_");
                _currentTargetName.Append(go.name);
                //_currentTargetName.Append(go.name);
                //var name = go.transform.parent.name + go.name;
                //Debug.Log("1" + _currentTargetName.ToString());
                GameObject target= FindTargetSpineAni(_currentTargetName.ToString());
                PlayTargetAni(target, _currentTargetName + "_ANIME");
                //Debug.Log("2" + _currentTargetName);
                _currentTargetName.Clear();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                JudgeNameAndPlayTargetVoice(go.name);
            }

        }

        void TurnToTheTargetBagStartStatu(int bagNumber,bool isPlayRollingSound=true)
        {
            _checkIfPass.Clear();
            SetTheResultVisible(false);
            
            SpineManager.instance.DoAnimation(_statu, "JD"+bagNumber+"_ANIME", false);
            // if (bagNumber > 2)
            // {
            //     Debug.Log("11111");
            //     MoveToTheLeft(bagNumber - 1);
            //     SpineManager.instance.DoAnimation(_mainTarget, "B_"+(bagNumber-1)+"_OUT", false,
            //         () =>
            //         {
            //             Debug.Log("22222");
            //             InializedAllObject(_bagInfoList[bagNumber-1]);
            //             MoveToTheMiddle();
            //             SpineManager.instance.DoAnimation(_mainTarget, "B_"+bagNumber+"_IN", false,()=>
            //             {
            //                 SetButtonClickVisible(true);
            //                 SetObjectButtonVisible(bagNumber, true);
            //             });
            //         });
            // }
            // else
            // {
                MoveToTheMiddle();
                InializedAllObject(_bagInfoList[bagNumber - 1]);
                //if (isPlayRollingSound)
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(_mainTarget, "B_" + bagNumber + "_IN", false, () =>
                {
                    SetButtonClickVisible(true);
                    if (bagNumber > 1 && bagNumber < 5)
                    {
                        SetObjectButtonVisible(bagNumber, true);
                    }
                });


                // }
        }

        void SetObjectButtonVisible(int bagNumber,bool isShow)
        {
            if (isShow)
            {
                HideAllBagButton();
            }
            _bagButtonParentList[bagNumber-1].SetActive(isShow);
        }

        

        void HideAllBagButton()
        {
            for (int i = 0; i < _bagButtonParentList.Count; i++)
            {
                if (_bagButtonParentList[i].activeSelf)
                {
                    _bagButtonParentList[i].gameObject.Hide();
                }
            }
        }

        /// <summary>
        /// 切换到下一个包，属性重置
        /// </summary>
        void GoToTheNextBag()
        {
            //_checkIfPass.Clear();
            //SetTheResultVisible(true, false);
            //MoveToTheLeft();
            switch (_currentBage)
            {
                case E_BagType.WP1:
                    _currentBage = E_BagType.WP2;
                    TurnToTheTargetBagStartStatu(2);
                    break;
                case E_BagType.WP2:
                    _currentBage = E_BagType.WP3;
                    TurnToTheTargetBagStartStatu(3);
                    break;
                case E_BagType.WP3:
                    _currentBage = E_BagType.WP4;
                    TurnToTheTargetBagStartStatu(4);
                    break;
                case E_BagType.WP4:
                    _currentBage = E_BagType.WP5;
                    TurnToTheTargetBagStartStatu(5);
                    break;
                case E_BagType.WP5:
                    InializedStartStatu();
                    SetTheResultVisible(true, true, true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10);
                    break;
            }
        }

        /// <summary>
        /// 点击通过按钮返回返回的方法
        /// </summary>
        void PassButtonEvent()
        {
            SpineManager.instance.DoAnimation(_passButtonAni, "AN_Y_ANIME", false,
                () => SpineManager.instance.DoAnimation(_passButtonAni, "AN_Y", false));
            SetButtonClickVisible(false);
            switch (_currentBage)
            {
                case E_BagType.WP1:
                    SetTheResultVisible(true, false);
                    break;
                case E_BagType.WP2:
                case E_BagType.WP3:
                case E_BagType.WP4:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    MoveToTheLeft((int)_currentBage,()=>GoToTheNextBag());
                    HideAllBagButton();
                    break;
                case E_BagType.WP5:
                    SetTheResultVisible(true, false);
                    break;
            }
        }


        /// <summary>
        /// 点击不通过按钮返回的方法
        /// </summary>
        void FailButtonEvent()
        {
            SpineManager.instance.DoAnimation(_failButtonAni, "AN_N_ANIME", false,
                () => SpineManager.instance.DoAnimation(_failButtonAni, "AN_N", false));
            SetButtonClickVisible(false);
            switch (_currentBage)
            {
                case E_BagType.WP1:
                    
                    SetTheResultVisible(true, true);
                    break;
                case E_BagType.WP2:
                    _humanAni.gameObject.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(WaitTimeAndPlayVoice(11));
                    SpineManager.instance.DoAnimation(_humanAni, "REN3_IN", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(_mainTarget, "JIQI", false);
                        // _objectParent.gameObject.Hide();
                        MoveToTheRight(() =>
                        {
                            TurnToTheTargetBagStartStatu(2,false);
                            _humanAni.Hide();
                        });
                        SpineManager.instance.DoAnimation(_humanAni, "REN3_IN2", false);

                    });
                    break;
                case E_BagType.WP3:
                    _humanAni.gameObject.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(WaitTimeAndPlayVoice(6));
                    SpineManager.instance.DoAnimation(_humanAni, "REN2_IN", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(_mainTarget, "JIQI", false);
                        // _objectParent.gameObject.Hide();
                        MoveToTheRight(() =>
                        {
                            TurnToTheTargetBagStartStatu(3,false);
                            _humanAni.Hide();
                        });
                        SpineManager.instance.DoAnimation(_humanAni, "REN2_IN2", false);

                    });
                    break;
                case E_BagType.WP4:
                    _humanAni2.gameObject.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    mono.StartCoroutine(WaitTimeAndPlayVoice(7));
                    SpineManager.instance.DoAnimation(_humanAni2, "ren", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(_mainTarget, "JIQI", false);
                        // _objectParent.gameObject.Hide();
                        MoveToTheRight(() =>
                        {
                            TurnToTheTargetBagStartStatu(4,false);
                            _humanAni2.Hide();
                        });
                        SpineManager.instance.DoAnimation(_humanAni2, "ren2", false);

                    });
                    break;
                case E_BagType.WP5:
                    SetTheResultVisible(true, true);
                    break;
            }
        }

        IEnumerator WaitTimeAndPlayVoice(int clipIndex)
        {
            yield return new WaitForSeconds(1f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, clipIndex, false);
        }

        /// <summary>
        /// 设置结束界面的开关以及相应的流程
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="isWin"></param>
        /// <param name="isShowTheWinPanel"></param>
        void SetTheResultVisible(bool isShow, bool isWin = true,bool isShowTheWinPanel=false)
        {
            
           
            if (isShow)
            {
                if (isWin)
                {

                    if (isShowTheWinPanel == false)
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                        {
                            SetObjectButtonVisible((int) _currentBage, true);
                        }));
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    }
                    else
                    {
                        _bg2.Show();
                        _resultAni.Show();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        SpineManager.instance.DoAnimation(_resultAni, "C2", false);
                    }
                }
                else
                {
                    _bg2.Show();
                    _resultAni.Show();
                    // bell.transform.SetAsLastSibling();
                    //SpineManager.instance.DoAnimation(bell, "ku", false);
                    _resetButton.gameObject.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    //SpineManager.instance.DoAnimation(_mainTarget, "JIQI", false);
                    SpineManager.instance.DoAnimation(_resultAni, "S2", false,
                        () => SpineManager.instance.DoAnimation(_resultAni, "S", false));
                }
            }
            else
            {
                _bg2.Hide();
                _resultAni.Hide();
                _resetButton.gameObject.Hide();
                // if (isWin)
                //     bell.transform.SetAsFirstSibling();
            }
        }

        
        /// <summary>
        /// 根据物品的名字判断执行相应的动画和音频，以及如果达到条件执行下一步
        /// </summary>
        /// <param name="name"></param>
        void JudgeNameAndPlayTargetVoice(string name)
        {
            
            if (String.Compare(name, "QIANG", StringComparison.Ordinal)==0)
            {
                SetObjectButtonVisible((int) _currentBage, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null,
                    () =>
                    {
                        CheckIfFindTheTarget(name);
                    }));
            }
            else if (String.Compare(name, "DAO", StringComparison.Ordinal) == 0)
            {
                SetObjectButtonVisible((int) _currentBage, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null,
                    () => CheckIfFindTheTarget(name)));
            }
            else if(String.Compare(name, "PAO", StringComparison.Ordinal)==0)
            {
                SetObjectButtonVisible((int) _currentBage, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, null,
                    () => CheckIfFindTheTarget(name)));

            }
            else if (String.Compare(name, "JIU", StringComparison.Ordinal) == 0)
            {
                SetObjectButtonVisible((int) _currentBage, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null,
                    () => CheckIfFindTheTarget(name)));
            }
        }

        /// <summary>
        /// 检查一下是否发现想要的物品
        /// </summary>
        /// <param name="name">物品名字</param>
        void CheckIfFindTheTarget(string name)
        {
            //Debug.Log("Name:" + name);
            if (_checkIfPass.ContainsKey(name))
            {
                _checkIfPass[name] = true;
            }
            else
            {
                _checkIfPass.Add(name, true);
            }

            string anotherTargetName = "";
            if (name == "QIANG")
            {
                anotherTargetName = "DAO";
            }else if (name == "DAO")
            {
                anotherTargetName = "QIANG";
            }
            else if (name == "PAO")
            {
                anotherTargetName = "JIU";
            }
            else if (name == "JIU")
            {
                anotherTargetName = "PAO";
            }

            if (_checkIfPass.ContainsKey(anotherTargetName) && _checkIfPass[anotherTargetName])
            {
                //MoveToTheLeft((int) _currentBage,()=>GoToTheNextBag());
                MoveToTheRight(() => GoToTheNextBag());
                //SpineManager.instance.DoAnimation(_mainTarget, "B_" + (int) _currentBage + "_OUT", false,()=>GoToTheNextBag());
            }
            else
            {
                SetObjectButtonVisible((int) _currentBage, true);
            }
        }

        /// <summary>
        /// 根据名字查找相应的Spine动画物体
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        GameObject FindTargetSpineAni(string name)
        {
            GameObject targetAni = null;
            
            if (_eachBagDic.TryGetValue(name, out targetAni))
            {
                return targetAni;
            }
            else
            {
                Debug.LogError("查找有误，请检查名字！:" + name);
                _currentTargetName.Clear();
                return null;
            }
        }

        /// <summary>
        /// 播放物品的Spine动画
        /// </summary>
        /// <param name="target">对应物品</param>
        /// <param name="name">动画名字</param>
        /// <param name="callBack">执行完回调</param>
        /// <param name="isLoop">是否循环</param>
        void PlayTargetAni(GameObject target, string name,Action callBack=null ,bool isLoop =false)
        {
            SpineManager.instance.DoAnimation(target,name,isLoop,callBack);
        }

        /// <summary>
        /// 初始化包里面的物品
        /// </summary>
        /// <param name="target"></param>
        void InializedAllObject(string[] target)
        {
            //Debug.Log(target.Length);
            if (target.Length == 4)
            {
                _objectList[4].Hide();
            }
            else if(target.Length==5)
            {
                _objectList[4].Show();
            }
            _eachBagDic.Clear();
            for (int i = 0; i < target.Length; i++)
            {
                SpineManager.instance.DoAnimation(_objectList[i], target[i], false);
                _eachBagDic.Add(target[i], _objectList[i]);
            }
        }

        void MoveToTheRight(Action callback=null)
        {
            if (!_objectParent.gameObject.activeSelf)
            {
                _objectParent.gameObject.Show();
            }

            if (_currentBage == E_BagType.WP1 || _currentBage == E_BagType.WP3)
                _objectParent.position = _middlePos.position;
            else
            {
                _objectParent.position = _middlePos2.position;
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(_mainTarget, "B_" + (int) _currentBage + "_IN2", false, callback);
            Tweener tw = null;
            
            if (_currentBage == E_BagType.WP1)
            {
                //Debug.Log("1111");
                tw = _objectParent.DOMove(_rightPos.position, 2.35f);
            }
            else if (_currentBage == E_BagType.WP2)
            {
                //yield return new WaitForSeconds(0.1f);
                tw = _objectParent.DOMove(_rightPos.position, 2f);
            }
            else
            {
                //yield return new WaitForSeconds(0.1f);
                tw = _objectParent.DOMove(_rightPos.position, 2f);
            }


            tw.SetEase(Ease.Linear);
            //mono.StartCoroutine(MoveToTheRightIE());
            
        }

        IEnumerator MoveToTheRightIE()
        {
           
            yield return null;
        }

        /// <summary>
        /// 包里面的物品移到中间
        /// </summary>
        void MoveToTheMiddle()
        {
            
            
            if (!_objectParent.gameObject.activeSelf)
            {
                _objectParent.gameObject.Show();
            }

            _objectParent.position = _rightPos.position;
            mono.StartCoroutine(WaitTimeAndMove());


        }

        IEnumerator WaitTimeAndMove()
        {
            Tweener tw = null;
            
            if (_currentBage == E_BagType.WP3)
            {
                yield return new WaitForSeconds(0.5f);
                tw = _objectParent.DOMove(_middlePos.position, 1.9f);
            }
            else if (_currentBage == E_BagType.WP1)
            {
                yield return new WaitForSeconds(0.35f);
                tw = _objectParent.DOMove(_middlePos.position, 2f);
            }
            else if (_currentBage == E_BagType.WP2)
            {
                yield return new WaitForSeconds(0.4f);
                tw = _objectParent.DOMove(_middlePos2.position, 2f);
            }
            else
            {
                yield return new WaitForSeconds(0.4f);
                tw = _objectParent.DOMove(_middlePos2.position, 2f);
            }

            
            tw.SetEase(Ease.Linear);
            

            
        }

        IEnumerator MoveToTheLeftIE(int bagNumber,Action callback=null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(_mainTarget, "B_"+bagNumber+"_OUT", false,callback);
            Tweener tw = null;
            if (bagNumber == 4)
            {
                
                tw = _objectParent.DOMove(_leftPos.position, 1.95f).OnComplete(() => _objectParent.gameObject.Hide());
                
            }
            else if (bagNumber == 1)
            {
                tw = _objectParent.DOMove(_leftPos.position, 2.1f).OnComplete(() => _objectParent.gameObject.Hide());
            }
            else if(bagNumber==2)
            {
               
               tw = _objectParent.DOMove(_leftPos.position, 1.7f).OnComplete(() => _objectParent.gameObject.Hide());
               
            }
            else if (bagNumber == 3)
            {
                tw = _objectParent.DOMove(_leftPos.position, 1.7f).OnComplete(() => _objectParent.gameObject.Hide());
            }
            else
            {
                tw = _objectParent.DOMove(_leftPos.position, 1.75f).OnComplete(() => _objectParent.gameObject.Hide());
            }
           

            tw.SetEase(Ease.Linear);
            yield return null;
            //MoveToTheLeft();
        }

        /// <summary>
        /// 包里面的物品移到左边移除
        /// </summary>
        void MoveToTheLeft(int bagNumber,Action callback=null)
        {
            _objectParent.gameObject.Show();
            if (_currentBage == E_BagType.WP1 || _currentBage == E_BagType.WP3)
                _objectParent.position = _middlePos.position;
            else
                _objectParent.position = _middlePos2.position;
            if (!_objectParent.gameObject.activeSelf)
            {
                _objectParent.gameObject.Show();
            }

            mono.StartCoroutine(MoveToTheLeftIE(bagNumber,callback));
            
        }

        void SetButtonClickVisible(bool isShow)
        {
            _passButtonAni.transform.GetGameObject("PassButton").SetActive(isShow);
            _failButtonAni.transform.GetGameObject("FailedButton").SetActive(isShow);
        }


        float TransferScreenValue(float targetValue, bool isWidth)
        {
            if (isWidth)
            {
                return targetValue * 1920f / Screen.width;
            }
            else
            {
                return targetValue * 1080f / Screen.height;
            }
        }

        Vector2 TransferScreenPoint(Vector2 targetPoint)
        {
            return new Vector2(targetPoint.x * 1920f / Screen.width, targetPoint.y * 1080f / Screen.height);
        }

        #region BellFunc

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    //点击语音键，播放Bell说话动画，同时语音：检查时，请认真盯控屏幕，不要漏检任何可疑物品！
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        bell.transform.SetAsFirstSibling();
                        
                        StartCheckObject();
                    }));
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
            }
            talkIndex++;
        }
        
        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
        

        #endregion
        
        
    }
}
