using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ILRuntime.Runtime;
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
    }

    public class TD91212Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private Transform Bg;
       

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        //胜利动画名字
        private string sz;
        bool isPlaying = false;
        private bool isSlideTheObject = false;
        #region 游戏物体

        private Transform _gamePanel;
        private Transform _circleParent;
        private List<Transform> _circleList;
        private Dictionary<string, GameObject> _circleUnfinishedBoxDic;

        private Transform _bugPanel;
        private List<Transform> _bugList;
        private List<Transform> _bugCurrentList;

        private List<Transform> _currentShowBugList;

        private bool isRight = false;
        private Vector2 _prePressPos;

        private Transform _waveButton;

        private int _currentGameLevel = 1;
        private Transform _blockPanel;
        private Dictionary<string, GameObject> _blockItemDic;
        private List<GameObject> _leftBlockItemList;

        private Transform _panelScore;
        private Dictionary<int, Dictionary<int, GameObject>> _panelNumberDic;

        private Transform _addScore;
        private Transform _minusScore;

        private int _totalScore;
        private Transform _totalPanelScore;
        private Dictionary<int, Dictionary<int, GameObject>> _totalPanelNumberDic;
        private GameObject _winPanel;

        private GameObject _blockAni;
        private GameObject _handAni;

        private bool _isWaitVoiceDown = false;
        
        #endregion


        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            //加载原有的场景中的物体
            LoadOriginalGameObject();
            //加载新添加的游戏中的物体
            LoadNewGameObject();
            GameInit();
           
            //GameStart();
        }

        void LoadOriginalGameObject()
        {
            Bg = curTrans.Find("BG");
           

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }

            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            //替换胜利动画需要替换spine 

            sz = "6-12";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }

        void LoadNewGameObject()
        {
            _gamePanel = curTrans.GetTransform("GamePanel");
            _circleParent = _gamePanel.GetTransform("Circles");
            _circleList = new List<Transform>();
            _circleUnfinishedBoxDic = new Dictionary<string, GameObject>();
            

            _bugPanel = _gamePanel.GetTransform("BugPanel");
            _bugList = new List<Transform>();
           _bugCurrentList=new List<Transform>(12);
            _currentShowBugList=new List<Transform>();
            _blockPanel = _gamePanel.GetTransform("BlockPanel");
            _blockItemDic=new Dictionary<string, GameObject>();
            
            _waveButton = _gamePanel.GetTransform("WaveButton");
            SlideSwitchPage(_gamePanel.gameObject);
            _panelScore = _gamePanel.GetTransform("PanelScore");
            _panelNumberDic=new Dictionary<int, Dictionary<int, GameObject>>();
            for (int i = 0; i < _panelScore.childCount; i++)
            {
                Transform target = _panelScore.GetChild(i);
                int number = Convert.ToInt32(target.name);
                //Debug.Log("1:"+number);
                _panelNumberDic.Add(number, new Dictionary<int, GameObject>());
                for (int j = 0; j < target.childCount; j++)
                {
                    Transform targetNumber = target.GetChild(j);
                    int number2 = Convert.ToInt32(targetNumber.name);
                    //Debug.Log("2"+number2);
                    _panelNumberDic[number].Add(number2, targetNumber.gameObject);
                }
            }
            _leftBlockItemList=new List<GameObject>();
            _totalPanelScore = successSpine.transform.GetTransform("PanelScore");
            _totalPanelNumberDic=new Dictionary<int, Dictionary<int, GameObject>>();
            for (int i = 0; i < _totalPanelScore.childCount; i++)
            {
                Transform target = _totalPanelScore.GetChild(i);
                int number = Convert.ToInt32(target.name);
                //Debug.Log("1:"+number);
                _totalPanelNumberDic.Add(number, new Dictionary<int, GameObject>());
                for (int j = 0; j < target.childCount; j++)
                {
                    Transform targetNumber = target.GetChild(j);
                    int number2 = Convert.ToInt32(targetNumber.name);
                    //Debug.Log("2"+number2);
                    _totalPanelNumberDic[number].Add(number2, targetNumber.gameObject);
                }
            }

            _blockAni = _gamePanel.GetTransform("BlockPanelAni").GetGameObject("1");
            _handAni=_gamePanel.GetGameObject("Hand");
            //ShowTheNumber(9999);
        }

        void ResetGameProperty()
        {
            SoundManager.instance.Stop("bgm");
            _isWaitVoiceDown = false;
            _circleList.Clear();
            _circleUnfinishedBoxDic.Clear();
            for (int i = 0; i < _circleParent.childCount; i++)
            {
                Transform target = _circleParent.GetChild(i);
                if (target.name.Contains("CircleBG"))
                {
                    for (int j = 0; j < target.childCount; j++)
                    {
                        Transform boxParent = target.GetChild(j).GetChild(0);
                        for (int k = 0; k < boxParent.childCount; k++)
                        {
                            GameObject unfinishedBox = boxParent.GetChild(k).gameObject;
                            //Debug.Log(unfinishedBox.name);
                            _circleUnfinishedBoxDic.Add(unfinishedBox.name, unfinishedBox);
                        }
                    }
                }
                else
                {
                    _circleList.Add(target);
                    //target.gameObject.Show();
                    target.localScale = new Vector3(0.1f, 0.1f, 0);
                }

              
                
            }
            
            _bugList.Clear();
            _bugCurrentList.Clear();
            for (int i = 0; i < _bugPanel.childCount; i++)
            {
                Transform target = _bugPanel.GetChild(i);
                _bugList.Add(target);
                
            }
           
        
            for (int i = 0; i < _bugList.Count; i++)
            {
                for (int j = i+1; j < _bugList.Count; j++)
                {
                    int last = Convert.ToInt32(_bugList[i].name);
                    int next = Convert.ToInt32(_bugList[j].name);
                 
                    if (next<last)
                    {
                      
                        Transform temp = _bugList[i];
                        _bugList[i] = _bugList[j];
                        _bugList[j] = temp;
                      
                    }
                }
            }
            ResetBugCurrentList();
            
            _blockItemDic.Clear();
            for (int i = 0; i < _blockPanel.childCount; i++)
            {
                Transform target = _blockPanel.GetChild(i);
                _blockItemDic.Add(target.name, target.gameObject);
            }
            _leftBlockItemList.Clear();
            _addScore = _gamePanel.GetTransform("AddScore");
            _minusScore = _gamePanel.GetTransform("MinusScore");
            _addScore.gameObject.Hide();
            _minusScore.gameObject.Hide();
            _winPanel = _gamePanel.GetGameObject("WinPanel");
            _winPanel.Hide();
        }

        void ResetBugCurrentList()
        {
            _bugCurrentList.Clear();
            for (int i = 0; i < _bugList.Count; i++)
            {
                _bugCurrentList.Add(_bugList[i]);
               
            }
        }

        void ShowTheNumber(int targetNumber)
        {
            
            int length = Math.Abs(targetNumber).ToString().Length;
            foreach (Dictionary<int,GameObject> dic in _panelNumberDic.Values)
            {
                foreach (GameObject target in dic.Values)
                {
                    target.Hide();
                }
            }

            for (int i = 0; i < length; i++)
            {
                int number = targetNumber % 10;
                
                ShowTheSingleNumber(i, number);
                targetNumber = targetNumber / 10;
            }
        }

        void ShowTheTotalNumber(int targetNumber)
        {
            int length = Math.Abs(targetNumber).ToString().Length;
            foreach (Dictionary<int,GameObject> dic in _totalPanelNumberDic.Values)
            {
                foreach (GameObject target in dic.Values)
                {
                    target.Hide();
                }
            }

            for (int i = 0; i < length; i++)
            {
                int number = targetNumber % 10;
                
                ShowTheTotalSingleNumber(i, number);
                targetNumber = targetNumber / 10;
            }
        }

        void ShowTheCurrentBG()
        {
            for (int i = 0; i < Bg.childCount; i++)
            {
                GameObject target = Bg.GetChild(i).gameObject;
                if(target.name==_currentGameLevel.ToString())
                    target.Show();
                else
                {
                    target.Hide();
                }
            }
        }

        void ShowTheSingleNumber(int wei, int number)
        {
            foreach (GameObject obj in _panelNumberDic[wei+1].Values)
            {
                if (_panelNumberDic[wei+1][number] == obj)
                {
                    obj.Show();
                }
                else
                {
                    obj.Hide();
                }
            }
        }

        void ShowTheTotalSingleNumber(int wei, int number)
        {
            foreach (GameObject obj in _totalPanelNumberDic[wei+1].Values)
            {
                if (_totalPanelNumberDic[wei+1][number] == obj)
                {
                    obj.Show();
                }
                else
                {
                    obj.Hide();
                }
            }
        }

        void ShowTheBlockPanel()
        {
            _leftBlockItemList.Clear();
            if (_currentGameLevel == 1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    GameObject target = _blockItemDic[i.ToString()];
                    //target.Show();
                    _leftBlockItemList.Add(target);
                }

                for (int i = 1; i <= 12; i++)
                {
                    _blockItemDic[i.ToString()].Hide();
                }
            }
            else if (_currentGameLevel == 2)
            {
                for (int i = 1; i <= 12; i++)
                {
                    _blockItemDic[i.ToString()].Hide();
                }

                for (int i = 5; i <= 8; i++)
                {
                    
                    GameObject target = _blockItemDic[i.ToString()];
                    //target.Show();
                    _leftBlockItemList.Add(target);
                }

                
            }
            else if (_currentGameLevel == 3)
            {
                for (int i = 1; i <= 12; i++)
                {
                    _blockItemDic[i.ToString()].Hide();
                }

                for (int i = 9; i <= 12; i++)
                {
                    GameObject target = _blockItemDic[i.ToString()];
                   
                    _leftBlockItemList.Add(target);
                }
            }
        }


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
                default:
                    break;
            }

            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        bd.Show();
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null,
                            () => SoundManager.instance.ShowVoiceBtn(true)));
                        
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        GameStart();
                    });
                }
                else if(obj.name=="ok")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        SoundManager.instance.Stop("bgm");
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3));
                    });
                }

            });
        }

        private void GameInit()
        {
            ResetGameProperty();
            talkIndex = 1;
            isPlaying = true;
            isRight = false;
            isSlideTheObject = false;
            _currentGameLevel = 1;
            _totalScore = 1000;
            _currentNeedRandomLevel = 1;
            ShowTheHandGuide(false);
            ShowTheCurrentBG();
            ShowTheNumber(_totalScore);
        }
        
        void GameStart()
        {
            
            
            ShowTheHandGuide(true);
            StartLoadTheGamePanel();
        }

        void ShowTheHandGuide(bool isShow)
        {
            if (isShow)
            {
                _handAni.Show();
                SpineManager.instance.DoAnimation(_handAni, "shou", true);
            }
            else
            {
                _handAni.Hide();
            }

            
        }

        private void TurnToTheNexCircleAndPlay()
        {
            if (_currentGameLevel < 3)
                _currentGameLevel++;
            else
            {
                WinTheGame();
                return;
            }
            _winPanel.Show();
            SpineManager.instance.DoAnimation(_winPanel, "guangxiao", false, () =>
            {
                _winPanel.Hide();
                ResetBugCurrentList();
                ShowTheBlockPanel();
                ShowTheCircleBg();
                ShowTheCurrentBG();
            });
           
            //StartLoadTheGamePanel();
        }

        void WinTheGame()
        {
            isPlaying = false;
            _winPanel.Show();
            for (int i = 0; i < _circleList.Count; i++)
            {
                _circleList[i].gameObject.Hide();
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
            SpineManager.instance.DoAnimation(_winPanel, "guangxiao", false, () =>
            {
                _winPanel.Hide();
                playSuccessSpine();
            });
        }

        void ShowTheCircleBg()
        {
            Transform targetPanel = _circleParent.GetTransform("CircleBG");
            for (int i = 0; i < targetPanel.childCount; i++)
            {
                GameObject circleBg = targetPanel.GetChild(i).gameObject;
                if (circleBg.name == _currentGameLevel.ToString())
                {
                    circleBg.Show();
                    Transform boxParent = circleBg.transform.GetChild(0);
                    for (int j = 0; j < boxParent.childCount; j++)
                    {
                        Transform circleBox = boxParent.GetChild(j);
                        circleBox.gameObject.Show();
                    }
                }
                else
                {
                    circleBg.Hide();
                }
            }
        }


        void StartLoadTheGamePanel()
        {
            ShowTheBlockPanel();
            ShowTheCircleBg();
            mono.StartCoroutine(StartTheCircle());
            HitTheBug();
        }
        
        IEnumerator StartTheCircle()
        {
            _currentShowBugList.Clear();
            for (int i = 0; i < _circleList.Count; i++)
            {
                Transform target = _circleList[i];
                
                CircleGrowing(target);
                
                yield return new WaitForSeconds(2.5f);
            }
        }

        private int _currentNeedRandomLevel = 1;

        Transform GetTargetBug(string name)
        {
            Transform target = null;
            for (int i = 0; i < _bugList.Count; i++)
            {
                Transform ta = _bugList[i];
                if (ta.name == name)
                {
                    target = ta;
                }
            }

            return target;
        }

        Transform GetRandomTargetBug()
        {
            Transform randomNumberBug = null;
            Debug.Log("CurrentNeedRandomLevel"+_currentNeedRandomLevel);
            if (_currentNeedRandomLevel == 1)
            {
                if (_currentGameLevel == 1)
                {
                    int randomNumber = Random.Range(0, _leftBlockItemList.Count);
                    randomNumberBug = _leftBlockItemList[randomNumber].transform;
                    randomNumberBug = GetTargetBug(randomNumberBug.name);
                    Debug.Log("1" + randomNumberBug);
                }
                else
                {
                    int randomNumber = Random.Range(1, 5);
                    randomNumberBug = GetTargetBug(randomNumber.ToString());
                    Debug.Log("2" + randomNumberBug);
                }
            }
            else if (_currentNeedRandomLevel == 2)
            {
                if (_currentGameLevel == 2)
                {
                    int randomNumber = Random.Range(0, _leftBlockItemList.Count);
                    randomNumberBug = _leftBlockItemList[randomNumber].transform;
                    randomNumberBug = GetTargetBug(randomNumberBug.name);
                    Debug.Log("3" + randomNumberBug);
                }
                else
                {
                    int randomNumber = Random.Range(5, 9);
                    // if (randomNumber >= 5 && randomNumber <= 8)
                    // {
                    //     randomNumber += 4;
                    // }
                    randomNumberBug = GetTargetBug(randomNumber.ToString());
                    Debug.Log("4" + randomNumberBug);
                }
            }
            else if (_currentNeedRandomLevel == 3)
            {
                if (_currentGameLevel == 3)
                {
                    int randomNumber = Random.Range(0, _leftBlockItemList.Count);
                    randomNumberBug = _leftBlockItemList[randomNumber].transform;
                    randomNumberBug = GetTargetBug(randomNumberBug.name);
                    Debug.Log("5" + randomNumberBug);
                }
                else
                {
                    int randomNumber = Random.Range(9, 13);
                    randomNumberBug = GetTargetBug(randomNumber.ToString());
                    Debug.Log("6" + randomNumberBug);
                }
            }

            if (_currentNeedRandomLevel < 3)
                _currentNeedRandomLevel++;
            else
            {
                _currentNeedRandomLevel = 1;
            }
            return randomNumberBug;
        }

        void CircleGrowing(Transform target)
        {
            
            if (isPlaying)
            {
                
                target.gameObject.Show();
               
                target.localScale = new Vector3(0.1f, 0.1f, 0);
                target.SetAsFirstSibling();
                if (isRight)
                {
                    target.localRotation = Quaternion.identity;
                }
                else
                {
                    target.localRotation = Quaternion.Euler(new Vector3(0, 0, -90f));
                }


                Transform bug = GetRandomTargetBug();
                // int randomNumber = Random.Range(0, _bugCurrentList.Count);
                // Transform bug = _bugCurrentList[randomNumber];
              
                //Debug.Log(target.name + "5555555555" + isPlaying + " Bug:" + bug.name);
                bug.gameObject.Show();
               
                _bugCurrentList.Remove(bug);
              
                bug.SetParent(target.GetChild(0));
                _currentShowBugList.Add(bug);
                bug.localPosition = Vector3.zero;
                bug.localScale = new Vector3(0.4f, 0.4f, 0);
               
                if (!isRight)
                    bug.localRotation = Quaternion.Euler(bug.eulerAngles.x, bug.eulerAngles.y, 40f);
                isRight = !isRight;
                
                target.DOScale(new Vector3(0.9f,0.9f,0.9f), 5f).SetEase(Ease.Linear).OnComplete(() =>
                {
                  
                    bug.SetParent(_bugPanel);                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                                        
                    bug.transform.localPosition = new Vector3(-1300f, 0, 0);
                    bool isHitTheRightBug = CheckBugIsClear(bug.name);
                    if (isHitTheRightBug)
                    {
                        //Debug.Log("还需要添加进去：" + bug.name+" "+_bugCurrentList.Count);
                        _bugCurrentList.Add(bug);
                    }
                    // else
                    // {
                    //     Debug.Log("已经有了，不许要添加" + bug.name + "Count" + _bugCurrentList.Count);
                    // }
                    _currentShowBugList.Remove(bug);
                    CircleGrowing(target);
                });
            }
            else
            {
                target.localScale = new Vector3(0.1f, 0.1f, 0);
                target.gameObject.Show();
            }
        }

        void HitTheBug()
        {
            mono.StartCoroutine(CheckDistance());
        }


        IEnumerator CheckDistance()
        {
            Transform wave = _waveButton.GetChild(0);
            while (isPlaying)
            {
                if (_currentShowBugList.Count > 0)
                {
                    for (int i = 0; i < _currentShowBugList.Count; i++)
                    {
                        Transform target = _currentShowBugList[i];
                        if (Vector3.Distance(wave.position, target.position) < 50f)
                        {
                            //Debug.Log("碰撞成功" + target.name);
                            JudgeHitAndGetScore(target);
                            RemoveTheTarget(target);
                            
                        }

                        //Debug.Log(target.name + "的距离为：" + Vector3.Distance(wave.position, target.position));
                    }
                }

                yield return new WaitForFixedUpdate();
            }
        }

        void JudgeHitAndGetScore(Transform target)
        {
            switch (target.name)
            {
                case "1":
                case "2":
                case "3":
                case "4":
                    if (_currentGameLevel == 1)
                    {
                        
                        ShowAndAddScore(target);
                        
                    }
                    else
                    {
                        WrongAndminusScore(target);
                    }

                    break;
                case "5":
                case "6":
                case "7":
                case "8":
                    if (_currentGameLevel == 2)
                    {
                        ShowAndAddScore(target);
                    }
                    else
                    {
                        WrongAndminusScore(target);
                    }

                    break;
                case "9":
                case "10":
                case "11":
                case "12":
                    if (_currentGameLevel == 3)
                    {
                        ShowAndAddScore(target);
                    }
                    else
                    {
                        WrongAndminusScore(target);
                    }
                    break;
            }
        }

        void ShowAndAddScore(Transform target)
        {
            
            _totalScore += 50;
            _bugCurrentList.Remove(target);
            ShowTheNumber(_totalScore);
            _addScore.gameObject.Show();
            _addScore.position = target.parent.position;
            _addScore.DOMove(new Vector3(_addScore.position.x, _addScore.position.y, _addScore.position.z + 10f), 1f)
                .OnComplete(() => _addScore.gameObject.Hide());
            SpineManager.instance.DoAnimation(_blockAni, "sk" + target.name, false, () =>
            {
                SpineManager.instance.DoAnimation(_blockAni, "000", false);
                _blockItemDic[target.name].Show();
                _circleUnfinishedBoxDic[target.name].Hide();
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            CountLeftBlockAndExcuteNext(target);
            BtnPlaySoundSuccess();
            
        }

        bool CheckBugIsClear(string name)
        {
            if (_leftBlockItemList.Count > 0)
            {
                int targetNumber = Convert.ToInt32(name);
                int minNumber = 0;
                int maxNumber = 0;
                if (_currentGameLevel == 1)
                {
                    minNumber = 1;
                    maxNumber = 4;
                    
                }
                else if (_currentGameLevel == 2)
                {
                    minNumber = 5;
                    maxNumber = 8;
                }
                else if (_currentGameLevel == 3)
                {
                    minNumber = 9;
                    maxNumber = 12;
                }
                Debug.Log("targetNumber:"+targetNumber);
                if (targetNumber <= maxNumber && targetNumber >= minNumber)
                {
                    Debug.Log("DDDDDDD");
                    for (int i = 0; i < _leftBlockItemList.Count; i++)
                    {
                        GameObject target = _leftBlockItemList[i];
                        Debug.Log(" 检查是否需要清理"+target.name +" "+ name);
                        if (target.name == name)
                            return true;
                    }
                    return false;
                }
                else
                {
                    return true;
                }
                
                
            }


            //Debug.Log("剩下的方块已经用尽，请检查方法！");
            return false;
        }

        void CountLeftBlockAndExcuteNext(Transform target)
        {
            
            //_leftBlockItemList.Clear();
            _leftBlockItemList.Remove(_blockItemDic[target.name]);
            //Debug.Log("还剩:"+_leftBlockItemList.Count);
            if (_leftBlockItemList.Count <= 0)
            {
                //Debug.Log("碰撞完成，进入下一个环节");
                TurnToTheNexCircleAndPlay();
            }
        }

        void WrongAndminusScore(Transform target)
        {
            //BtnPlaySoundFail();
            _isWaitVoiceDown = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            float timer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
            mono.StartCoroutine(WaitTimerAndExcuteNext(timer, () =>
            {
                _isWaitVoiceDown = false;
            }));
            _totalScore -= 50;
            if (_totalScore <= 0)
                _totalScore = 0;
            ShowTheNumber(_totalScore);
            _minusScore.gameObject.Show();
            _minusScore.position = target.parent.position;
            _minusScore.DOMove(new Vector3(_minusScore.position.x, _minusScore.position.y, _minusScore.position.z + 10f), 1f)
                .OnComplete(() => _minusScore.gameObject.Hide());
            
        }

        void RemoveTheTarget(Transform target)
        {
            target.SetParent(_bugPanel);
            target.localPosition=new Vector3(-1300f,0,0);
            target.gameObject.Hide();
            _currentShowBugList.Remove(target);
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
        IEnumerator SpeckerCoroutine(GameObject target, SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(target, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(target, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(target, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                    GameStart();
                }));
            }

            talkIndex++;
        }

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
            ShowTheTotalNumber(_totalScore);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "-2", false,
                        () =>
                        {
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
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
        private void BtnPlaySoundSuccess(Action callback=null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            float timter= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
            mono.StartCoroutine(WaitTimerAndExcuteNext(timter, callback));
        }

        IEnumerator WaitTimerAndExcuteNext(float timer,Action callback)
        {
            yield return new WaitForSeconds(timer);
            callback?.Invoke();
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                ShowTheHandGuide(false);
                if (!_isWaitVoiceDown)
                    _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                if (!_isWaitVoiceDown)
                {
                    float dis = Math.Abs( upData.position.x - _prePressPos.x);
                    bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                    if (dis > 100)
                    {
                        if (isRight)
                        {
                            if (isSlideTheObject)
                                return;
                            isSlideTheObject = true;
                            _waveButton.DOLocalRotate(new Vector3(0, 0, -45f), 0.5f).OnComplete(() =>
                            {
                                _waveButton.DOLocalRotate(Vector3.zero, 0.5f);
                                isSlideTheObject = false;
                            });
                       
                        
                        }
                        else
                        {
                            if(isSlideTheObject)
                                return;
                            isSlideTheObject = true;
                            _waveButton.DOLocalRotate(new Vector3(0, 0, 45f), 0.5f).OnComplete(() =>
                            {
                                _waveButton.DOLocalRotate(Vector3.zero, 0.5f);
                                isSlideTheObject = false;
                            });
                        }
                    }
                }

                
            };
        }
    }
}

