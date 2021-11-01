using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spine;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course711Part2
    {
        private enum E_CrankType
        {
            Parallel,
            Reverse,
            Unequal
        }

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        private GameObject _mainTarget;

        private GameObject _unEqualCrank;

        private GameObject _mainText;

        private GameObject _playButton;

        private Dictionary<string, bool> _currentStateDic;
        private Dictionary<GameObject, Dictionary<string,GameObject>> _targetPointDic;

        private const string Left = "Left";

        private const string Right = "Right";

        private const string Middle = "Middle";

        private const string _point1 = "Point1";
        private const string _point2 = "Point2";
        private const string _point3 = "Point3";

        private E_CrankType _currentCrankType = E_CrankType.Parallel;

        private GameObject _parallelDoubleCrank;
        private GameObject _reverseDoubleCrank;
        private GameObject _unequalLengthDoubleCrank;

        private GameObject _trail1;
        private GameObject _trail2;
        private GameObject _trail3;

        private GameObject _unequalTrail1;
        private GameObject _unequalTrail2;

        private GameObject _leftButton;
        private GameObject _rightButton;


        private bool _isPlay = false;
        private GameObject _playAni;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            _targetPointDic=new Dictionary<GameObject, Dictionary<string, GameObject>>();
            bell = curTrans.Find("bell").gameObject;
            _mainTarget = curTrans.Find("Main").gameObject;
            _mainText = curTrans.Find("MainText").gameObject;
            _playButton = curTrans.GetGameObject("Play");
            _unEqualCrank = curTrans.Find("UnequealCrank").gameObject;
            _parallelDoubleCrank = curTrans.Find("ParallelDoubleCrank").gameObject;
            _reverseDoubleCrank = curTrans.Find("ReverseDoubleCrank").gameObject;
            _unequalLengthDoubleCrank = curTrans.Find("UnequalLengthDoubleCrank").gameObject;
            _leftButton = curTrans.GetGameObject("Left");
            _rightButton = curTrans.GetGameObject("Right");
            talkIndex = 1;
            _trail1 = curTrans.Find("Trail1").gameObject;
            _trail2 = curTrans.Find("Trail2").gameObject;
            _trail3 = curTrans.Find("Trail3").gameObject;
            _unequalTrail1 = _unEqualCrank.transform.Find("Trail1").gameObject;
            _unequalTrail2 = _unEqualCrank.transform.Find("Trail2").gameObject;
            _playAni = curTrans.Find("Play/PlayAni").gameObject;
            _currentStateDic =new Dictionary<string, bool>();
            RemoveAllButtonEvent(curTrans);
            AddAllButtonEvent(curTrans);
            _currentStateDic.Add(Left,false);
            _currentStateDic.Add(Right,false);
            _currentStateDic.Add(Middle,false);
            _playButton.Hide();
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            InitPointDic();
            GameStart();
        }

        void InitPointDic()
        {
            _targetPointDic.Add(_parallelDoubleCrank,new Dictionary<string, GameObject>());
            _targetPointDic.Add(_reverseDoubleCrank,new Dictionary<string, GameObject>());
            _targetPointDic.Add(_unequalLengthDoubleCrank,new Dictionary<string, GameObject>());
            for (int i = 0; i < _parallelDoubleCrank.transform.childCount; i++)
            {
                GameObject target = _parallelDoubleCrank.transform.GetChild(i).gameObject;
                _targetPointDic[_parallelDoubleCrank]
                    .Add(target.name,target);
            }

            for (int i = 0; i < _reverseDoubleCrank.transform.childCount; i++)
            {
                GameObject target = _reverseDoubleCrank.transform.GetChild(i).gameObject;
                _targetPointDic[_reverseDoubleCrank]
                    .Add(target.name, target);
            }

            for (int i = 0; i < _unequalLengthDoubleCrank.transform.childCount; i++)
            {
                GameObject target = _unequalLengthDoubleCrank.transform.GetChild(i).gameObject;
                _targetPointDic[_unequalLengthDoubleCrank]
                    .Add(target.name, target);
            }

            // foreach (Dictionary<string,GameObject> list in _targetPointDic.Values)
            // {
            //     for (int i = 0; i < list.Count; i++)
            //     {
            //        Debug.LogError(list[i].name);
            //     }
            // }
        }

        void HideAllPoint()
        {
            foreach (Dictionary<string,GameObject> list in _targetPointDic.Values)
            {
                foreach (GameObject target in list.Values)
                {
                    target.transform.Find("LightPoint").gameObject.Hide();
                }
            }
        }

        void HidePoint(GameObject parent,string point)
        {
            //_targetPointDic[parent].Find(p => p.name == point).transform.Find("LightPoint").gameObject.Hide();
            _targetPointDic[parent][point].transform.Find("LightPoint").gameObject.Hide();
        }

        void ShowAndDoPointAni(GameObject parent,string point,Action callback=null)
        {
            //Debug.Log(_targetPointDic[parent].Find(p => p.name == point));
            //Debug.Log("sd " + point+" :"+_targetPointDic[parent][point].name);
            GameObject targetPoint = _targetPointDic[parent][point].transform.Find("LightPoint").gameObject;
            targetPoint.Show();
            SpineManager.instance.DoAnimation(targetPoint,
                "caidianfaguang",
                true,callback);
        }

        void GameStart()
        {
            InitGameProperty();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SetLeftRightButtonVisible(true);
                _playButton.Show();
            }));
        }

        void ResetAllState()
        {
            _currentStateDic[Left] = false;
            _currentStateDic[Right] = false;
            _currentStateDic[Middle] = false;
            _isPlay = false;
        }

        void InitGameProperty()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SpineManager.instance.DoAnimation(_mainTarget, "a5",false);
            SpineManager.instance.DoAnimation(_mainText, "btg3", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SwitchToNextScene(E_CrankType.Parallel);
            _currentCrankType = E_CrankType.Parallel;
            _unEqualCrank.Hide();
            SetLeftRightButtonVisible(false);
           
         
            //_parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
           
            //_parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            
            _isPlay = false;
            ResetAllState();
            //_parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = new Color(1, 1, 1, 0);
        }

        void RemoveAllButtonEvent(Transform curTrans)
        {
            _parallelDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.RemoveAllListeners();
            _parallelDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.RemoveAllListeners();
            _parallelDoubleCrank.transform.Find("Point3").GetComponent<Button>().onClick.RemoveAllListeners();
           
            _reverseDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.RemoveAllListeners();
            _reverseDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.RemoveAllListeners();
           
            _unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.RemoveAllListeners();
            _unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.RemoveAllListeners();

            curTrans.Find("Play").GetComponent<Button>().onClick.RemoveAllListeners();
            curTrans.Find("Left").GetComponent<Button>().onClick.RemoveAllListeners();
            curTrans.Find("Right").GetComponent<Button>().onClick.RemoveAllListeners();
        }

        void AddAllButtonEvent(Transform curTrans)
        {
            
           
            _parallelDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.AddListener(ParaLeft);
            _parallelDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.AddListener(ParaMiddle);
            _parallelDoubleCrank.transform.Find("Point3").GetComponent<Button>().onClick.AddListener(ParaRight);
           
            _reverseDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.AddListener(ReverseLeft);
            _reverseDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.AddListener(ReverseRight);
           
            _unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Button>().onClick.AddListener(UnequalLeft);
            _unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Button>().onClick.AddListener(UnequalRight);

            curTrans.Find("Play").GetComponent<Button>().onClick.AddListener(Play);
            curTrans.Find("Left").GetComponent<Button>().onClick.AddListener(MoveLeft);
            curTrans.Find("Right").GetComponent<Button>().onClick.AddListener(MoveRight);
        }

        void SetLeftRightButtonVisible(bool isShow)
        {
            _leftButton.SetActive(isShow);
            _rightButton.SetActive(isShow);
        }

        void MoveLeft()
        {
            //todo 切换上一个轨迹
            _isPlay = false;
            ResetAllState();
            BtnPlaySound();
            if (_currentCrankType == E_CrankType.Parallel)
            {
                _currentCrankType = E_CrankType.Unequal;
                SwitchToNextScene(E_CrankType.Unequal);
            }
            else if (_currentCrankType == E_CrankType.Unequal)
            {
                _currentCrankType = E_CrankType.Reverse;
                SwitchToNextScene(E_CrankType.Reverse);
            }
            else if (_currentCrankType == E_CrankType.Reverse)
            {
                _currentCrankType = E_CrankType.Parallel;
                SwitchToNextScene(E_CrankType.Parallel);
            }
        }

        void SwitchToNextScene(E_CrankType targetCrankType)
        {
            //string targetAniName = "";
            _trail1.Hide();
            _trail2.Hide();
            _trail3.Hide();
            _unequalTrail1.Hide();
            _unequalTrail2.Hide();
            HideAllPoint();
            
            if (targetCrankType == E_CrankType.Parallel)
            {
                _mainTarget.Show();
                _unEqualCrank.Hide();
                
                _parallelDoubleCrank.Show();
                SpineManager.instance.DoAnimation(_mainText, "btg3", false);
                
                // _parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                // _parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                // _parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _reverseDoubleCrank.Hide();
                _unequalLengthDoubleCrank.Hide();
             
                SpineManager.instance.DoAnimation(_mainTarget, "a5", false);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            }
            else if (targetCrankType == E_CrankType.Reverse)
            {
                _mainTarget.Show();
                _unEqualCrank.Hide();
                _reverseDoubleCrank.Show();
                SpineManager.instance.DoAnimation(_mainText, "btg2", false);
                _parallelDoubleCrank.Hide();
                _unequalLengthDoubleCrank.Hide();
                

                SpineManager.instance.DoAnimation(_mainTarget, "b4", false);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                // _reverseDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                // _reverseDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            else if (targetCrankType == E_CrankType.Unequal)
            {
                _mainTarget.Hide();
                _unEqualCrank.Show();
                
                _unequalLengthDoubleCrank.Show();
                SpineManager.instance.DoAnimation(_mainText, "btg1", false);
                _parallelDoubleCrank.Hide();
                _reverseDoubleCrank.Hide();
                SpineManager.instance.DoAnimation(_unEqualCrank, "animation2", false);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                // _unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                // _unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            

        }

        void MoveRight()
        {
            //todo 切换下一个轨迹
            _isPlay = false;
           ResetAllState();
           BtnPlaySound();
            if (_currentCrankType == E_CrankType.Parallel)
            {
                _currentCrankType = E_CrankType.Reverse;
                SwitchToNextScene(E_CrankType.Reverse);
            }
            else if (_currentCrankType == E_CrankType.Unequal)
            {
                _currentCrankType = E_CrankType.Parallel;
                SwitchToNextScene(E_CrankType.Parallel);
            }
            else if (_currentCrankType == E_CrankType.Reverse)
            {
                _currentCrankType = E_CrankType.Unequal;
                SwitchToNextScene(E_CrankType.Unequal);
            }
        }

        private int _clickJudgeNumber = 0;

        void ClickJudge()
        {
            if (_clickJudgeNumber == 0)
            {
                
                mono.StartCoroutine(ClickJudgeIE());
            }

            _clickJudgeNumber++;

        }

        IEnumerator ClickJudgeIE()
        {
            if(Input.GetTouch(0).tapCount==2)
            while (true)
            {
                if (_clickJudgeNumber > 1)
                {

                    _clickJudgeNumber = 0;
                    break;
                }

                yield return new WaitForFixedUpdate();
            }
        }

        void Play()
        {
            // ClickeEvent();
            SpineManager.instance.DoAnimation(_playAni, "kg2", false,
                () => SpineManager.instance.DoAnimation(_playAni, "kg1", false));
            //bool isClickTwice = OnPlayClicked();
            if (_isPlay)
            {
                _isPlay = false;
                Debug.Log("两次");
                if (_currentCrankType == E_CrankType.Parallel)
                {
                    if (_currentStateDic[Left])
                    {
                        _currentStateDic[Left] = false;
                        _trail1.Hide();
                        HidePoint(_parallelDoubleCrank,_point1);
                        //_parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }

                    if (_currentStateDic[Middle])
                    {
                        _currentStateDic[Middle] = false;
                        _trail2.Hide();
                        HidePoint(_parallelDoubleCrank,_point2);
                        //_parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }

                    if (_currentStateDic[Right])
                    {
                        _currentStateDic[Right] = false;
                        _trail3.Hide();
                        HidePoint(_parallelDoubleCrank, _point3);
                        //_parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }

                    SpineManager.instance.DoAnimation(_mainTarget, "a1", false);
                }
                else if (_currentCrankType == E_CrankType.Reverse)
                {
                    if (_currentStateDic[Left])
                    {
                        _currentStateDic[Left] = false;
                        _trail1.Hide();
                        HidePoint(_reverseDoubleCrank, _point1);
                        //_reverseDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }
                    if (_currentStateDic[Right])
                    {
                        _currentStateDic[Right] = false;
                        _trail3.Hide();
                        HidePoint(_reverseDoubleCrank, _point2);
                        //_reverseDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }

                    SpineManager.instance.DoAnimation(_mainTarget, "b1", false);
                }
                else if (_currentCrankType == E_CrankType.Unequal)
                {
                    if (_currentStateDic[Left])
                    {
                        _currentStateDic[Left] = false;
                        //_trail1.Hide();
                        _unequalTrail1.Hide();
                        HidePoint(_unequalLengthDoubleCrank, _point1);
                        //_unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }
                    if (_currentStateDic[Right])
                    {
                        _currentStateDic[Right] = false;
                        //_trail3.Hide();
                        _unequalTrail2.Hide();
                        HidePoint(_unequalLengthDoubleCrank, _point2);
                        //_unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    }

                    SpineManager.instance.DoAnimation(_unEqualCrank, "animation", false);
                }

                
            }
            else
            {
                Debug.Log("一次");
                _isPlay = true;
                if (_currentCrankType == E_CrankType.Parallel)
                {
                    Debug.Log("Left:" + _currentStateDic[Left]);
                    if (_currentStateDic[Left])
                    {
                        _trail1.Show();
                        SpineManager.instance.DoAnimation(_trail1, "a2", false);
                        HidePoint(_parallelDoubleCrank, _point1);
                        //_parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }

                    if (_currentStateDic[Middle])
                    {
                        _trail2.Show();
                        SpineManager.instance.DoAnimation(_trail2, "a3", false);
                        HidePoint(_parallelDoubleCrank, _point2);
                        //_parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }

                    if (_currentStateDic[Right])
                    {
                        _trail3.Show();
                        SpineManager.instance.DoAnimation(_trail3, "a4", false);
                        HidePoint(_parallelDoubleCrank, _point3);
                        //_parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }
                    SpineManager.instance.DoAnimation(_mainTarget, "a1", true);
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1));
                }
                else if (_currentCrankType == E_CrankType.Reverse)
                {
                    if (_currentStateDic[Left])
                    {
                        _trail1.Show();
                        SpineManager.instance.DoAnimation(_trail1, "b2", false);
                        HidePoint(_reverseDoubleCrank, _point1);
                        //_reverseDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }
                    if (_currentStateDic[Right])
                    {
                        _trail3.Show();
                        SpineManager.instance.DoAnimation(_trail3, "b3", false);
                        HidePoint(_reverseDoubleCrank, _point2);
                        //_reverseDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }
                    SpineManager.instance.DoAnimation(_mainTarget, "b1", true);
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2));
                }
                else if (_currentCrankType == E_CrankType.Unequal)
                {
                    if (_currentStateDic[Left])
                    {
                        //_trail1.Show();
                        
                        _unequalTrail1.Show();
                        
                        SpineManager.instance.DoAnimation(_unequalTrail1, "q2", false);
                        HidePoint(_unequalLengthDoubleCrank, _point1);
                        //_unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }
                    if (_currentStateDic[Right])
                    {
                       
                        _unequalTrail2.Show();
                        SpineManager.instance.DoAnimation(_unequalTrail2, "q1", false);
                        HidePoint(_unequalLengthDoubleCrank, _point2);
                        //_unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);

                    }

                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3));
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                    SpineManager.instance.DoAnimation(_unEqualCrank, "animation", true);

                }
            }

            
        }

        //private float _lastClickedTime = 0;
        //private float _clickedInterval = 0.5f;
        private int _playClickCount = 0;

        // private int _pointClickCount = 0;
        // //private int _clickedCount = 2;
        // bool OnClicked()
        // {
        //     bool isClickedTwice = false;
        //     if (_pointClickCount == 0)
        //     {
        //         Debug.Log("0");
        //         _pointClickCount = 1;
        //         
        //     }
        //     else if(_pointClickCount == 1)
        //     {
        //         Debug.Log("1");
        //         _pointClickCount = 0;
        //         isClickedTwice = true;
        //     }
        //
        //     return isClickedTwice;
        // }

        bool OnPlayClicked()
        {
            bool isClickedTwice = false;
            if (_playClickCount == 0)
            {
                //Debug.Log("0");
                _playClickCount = 1;

            }
            else if (_playClickCount == 1)
            {
                //Debug.Log("1");
                _playClickCount = 0;
                isClickedTwice = true;
            }

            return isClickedTwice;
        }

        // bool OnClicked2()
        // {
        //     bool isClickedTwice = false;
        //     float interval = Time.realtimeSinceStartup - _lastClickedTime;
        //     // Debug.LogError("interval"+interval+" ClickedInterval:"+_clickedInterval);
        //     if (interval <= 0.5f)
        //     {
        //         _count++;
        //         //Debug.LogError("Count:"+_count);
        //         if (_count == 1)
        //         {
        //             isClickedTwice= true;
        //         }
        //     }
        //     else
        //     {
        //         isClickedTwice = false;
        //         _count = 0;
        //     }
        //     _lastClickedTime=Time.realtimeSinceStartup;
        //     return isClickedTwice;
        // }

        

        void ParaLeft()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_parallelDoubleCrank,_point1);
            //     _currentStateDic[Left] = false;
            // }
            // else
            // {
            //     //_parallelDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_parallelDoubleCrank, _point1);
            //     _currentStateDic[Left] = true;
            // }
            if (_currentStateDic[Left])
            {
                HidePoint(_parallelDoubleCrank, _point1);
                _currentStateDic[Left] = false;
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                ShowAndDoPointAni(_parallelDoubleCrank, _point1);
                _currentStateDic[Left] = true;
            }


        }

        void ParaMiddle()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_parallelDoubleCrank, _point2);
            //     _currentStateDic[Middle] = false;
            // }
            // else
            // {
            //     //_parallelDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_parallelDoubleCrank, _point2);
            //     _currentStateDic[Middle] = true;
            // }
            if (_currentStateDic[Middle])
            {
                _currentStateDic[Middle] = false;
                HidePoint(_parallelDoubleCrank, _point2);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                ShowAndDoPointAni(_parallelDoubleCrank, _point2);
                _currentStateDic[Middle] = true;
            }
        }

        void ParaRight()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_parallelDoubleCrank, _point3);
            //     _currentStateDic[Right] = false;
            // }
            // else
            // {
            //     //_parallelDoubleCrank.transform.Find("Point3").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_parallelDoubleCrank, _point3);
            //     _currentStateDic[Right] = true;
            // }
            if (_currentStateDic[Right])
            {
                _currentStateDic[Right] = false;
                HidePoint(_parallelDoubleCrank, _point3);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _currentStateDic[Right] = true;
                ShowAndDoPointAni(_parallelDoubleCrank, _point3);
            }
        }

        void ReverseLeft()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_reverseDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_reverseDoubleCrank, _point1);
            //     _currentStateDic[Left] = false;
            // }
            // else
            // {
            //     //_reverseDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_reverseDoubleCrank, _point1);
            //     _currentStateDic[Left] = true;
            // }
            if (_currentStateDic[Left])
            {
                HidePoint(_reverseDoubleCrank, _point1);
                _currentStateDic[Left] = false;
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _currentStateDic[Left] = true;
                ShowAndDoPointAni(_reverseDoubleCrank, _point1);
            }
        }

        void ReverseRight()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_reverseDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_reverseDoubleCrank, _point2);
            //     _currentStateDic[Right] = false;
            // }
            // else
            // {
            //     //_reverseDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_reverseDoubleCrank, _point2);
            //     _currentStateDic[Right] = true;
            // }
            if (_currentStateDic[Right])
            {
                _currentStateDic[Right] = false;
                HidePoint(_reverseDoubleCrank, _point2);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _currentStateDic[Right] = true;
                ShowAndDoPointAni(_reverseDoubleCrank, _point2);
            }

        }

        void UnequalLeft()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_unequalLengthDoubleCrank, _point1);
            //     _currentStateDic[Left] = false;
            // }
            // else
            // {
            //     //_unequalLengthDoubleCrank.transform.Find("Point1").GetComponent<Image>().color = Color.white;
            //     ShowAndDoPointAni(_unequalLengthDoubleCrank, _point1);
            //     _currentStateDic[Left] = true;
            // }
            if (_currentStateDic[Left])
            {
                _currentStateDic[Left] = false;
                HidePoint(_unequalLengthDoubleCrank, _point1);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _currentStateDic[Left] = true;
                ShowAndDoPointAni(_unequalLengthDoubleCrank, _point1);
            }

        }

        void UnequalRight()
        {
            // bool isClickTwice = OnClicked();
            // if (isClickTwice)
            // {
            //     //_unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Image>().color = new Color(1, 1, 1, 0);
            //     HidePoint(_unequalLengthDoubleCrank, _point2);
            //     _currentStateDic[Right] = false;
            // }
            // else
            // {
            //     //_unequalLengthDoubleCrank.transform.Find("Point2").GetComponent<Image>().color =Color.white;
            //     ShowAndDoPointAni(_unequalLengthDoubleCrank, _point2);
            //     _currentStateDic[Right] = true;
            // }
            if (_currentStateDic[Right])
            {
                _currentStateDic[Right] = false;
                HidePoint(_unequalLengthDoubleCrank, _point2);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                _currentStateDic[Right] = true;
                ShowAndDoPointAni(_unequalLengthDoubleCrank, _point2);
            }
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if(talkIndex == 1)
            {

            }
            talkIndex++;
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }
    }
}
