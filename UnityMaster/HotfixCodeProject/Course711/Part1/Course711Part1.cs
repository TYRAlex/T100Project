using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using DG.Tweening;
using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;
using Vectrosity;
using Object = System.Object;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course711Part1
    {
        

        private enum E_Subject
        {
            No1,
            No2
           
        }

        private enum E_Answer
        {
            None,Left,Right,All
        }


        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;

        
        private E_Subject _myESubject=E_Subject.No1;

        //private E_Answer _myAnswer = E_Answer.All;
        /// <summary>
        /// 火车的动画对象
        /// </summary>
        private Transform _train;
        /// <summary>
        /// 轮子和线条的动画对象
        /// </summary>
        private GameObject _wheel;

        private GameObject _wheelTrail1;

        private GameObject _wheelTrail2;

        private GameObject _wheelTrail3;
        //private List<Button> _trainPart;

        private Transform _hand;
        private Vector3 _startHandPos;

        private List<Vector2> _posList;
        private List<GameObject> _lineList;
        private Transform _lineParent;
        private VectorObject2D _line;
        private Vector2 _t0;
        private List<Vector3[]> _allFinishLineList;
        private bool _isDrawingByMySelf = false;

        private Vector3 _lineStartPoint;
        private Vector3 _lineEndPoint;

        private Transform _linePoint1;
        private Transform _linePoint2;
        private Transform _linePoint3;

        private Transform _linePoint4;

        private Transform _linePoint5;

        private Transform _handEndPos;

        private GameObject _buttonList;

        private GameObject _playButtonAni;

        private GameObject _resetButtonAni;
        
        private Button _leftButton;
        private Button _rightButton;

        private GameObject _updateScripts;

        /// <summary>
        /// 随机播放失败语音的下标参数
        /// </summary>
        private int _currentTargetVoiceIndex = 1;
        //private List<Button> _buttonList;


        //private SkeletonAnimation _testAni;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bell = curTrans.Find("bell").gameObject;

            talkIndex = 1;

            _allFinishLineList = new List<Vector3[]>();
            _buttonList = curTrans.Find("ButtonList").gameObject;
            _train = curTrans.Find("Train");
            _wheel = curTrans.Find("LineGroup").Find("Line").gameObject;
            _wheelTrail1 = curTrans.Find("LineGroup").Find("LineTrail1").gameObject;
            _wheelTrail1.Hide();
            _wheelTrail2 = curTrans.Find("LineGroup").Find("LineTrail2").gameObject;
            _wheelTrail2.Hide();
            _wheelTrail3 = curTrans.Find("LineGroup").Find("LineTrail3").gameObject;
            _wheelTrail3.Hide();
            _hand = curTrans.Find("Hand");
            _hand.gameObject.Hide();
            _linePoint1 = curTrans.Find("Point1");
            _linePoint2 = curTrans.Find("Point2");
            _linePoint3 = curTrans.Find("Point3");
            _linePoint4 = curTrans.Find("Point4");
            _linePoint5 = curTrans.Find("Point5");
            _startHandPos = _hand.position;
            _handEndPos = curTrans.GetTransform("HandEndPos");
            //_trainPart=new List<Button>();
            //AddAllTranPartToTheList();//弃用
            AddAllViewButtonEvent(curTrans);
            ResetLineProperty(curTrans);
            // _testAni = _train.GetComponent<SkeletonAnimation>();
            // _testAni.
            //PrintAssetStringName();
            // if (_updateScripts == null)
            // {
            //     _updateScripts = new GameObject("MonoScriptsBase");
            //     _updateScripts.transform.SetParent(curTrans);
            //     MonoScripts monoscripts = _updateScripts.AddComponent<MonoScripts>();
            //     monoscripts.UpdateCallBack = Update;
            // }

            _updateScripts = FindUpdateScriptsInScenes(curTrans).gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            

            GameStart();
        }

        Transform FindUpdateScriptsInScenes(Transform curTrans)
        {
            Transform targetUpdateScripts = null;
            targetUpdateScripts = curTrans.Find("MonoScriptsBase");
            if (targetUpdateScripts == null)
            {
                targetUpdateScripts = new GameObject("MonoScriptsBase").transform;
                targetUpdateScripts.SetParent(curTrans);
                targetUpdateScripts.gameObject.AddComponent<MonoScripts>().UpdateCallBack = Update;
            }

            
            return targetUpdateScripts;
        }

        VectorObject2D _currentTarget = null;

        

        void GameStart()
        {
            InitGameProperty();
            _myESubject = E_Subject.No1;
            SpineManager.instance.DoAnimation(_train.gameObject, "donglun2", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                    () => bell.transform.SetAsLastSibling(),
                    () => SoundManager.instance.ShowVoiceBtn(true)));
            });
            //mono.StartCoroutine(WaitTimeAndExcuteNext(3f));

            //DrawExampleLine();
        }

        void InitGameProperty()
        {
            SoundManager.instance.StopAudio();
            bell.transform.SetAsFirstSibling();
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            _train.gameObject.Show();
            SpineManager.instance.DoAnimation(_wheel, "lunzi", false);
            //_buttonList.Hide();
            SetButtonListVisible(false);
            _firstNo2Success = false;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
        }

        IEnumerator WaitTimeAndExcuteNext(float timer)
        {
            yield return new WaitForSeconds(timer);
            Debug.Log("开始播放语音");
            //todo...播放语音 :蒸汽火车的出现影响了世界的发展，请你们仔细观察，它和当代火车的不同之处在哪里呢？。
            yield return SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => bell.transform.SetAsLastSibling(),
                () => SoundManager.instance.ShowVoiceBtn(true));
            //mono.StartCoroutine(SpeckerCoroutine())
            //SpineManager.instance.DoAnimation(bell,"DAIJI")

        }

        void ShowTheWheelRotate()
        {
            _train.gameObject.SetActive(false);
            _wheel.SetActive(true);
            
            SpineManager.instance.DoAnimation(_wheel, "lunzi",true);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            mono.StartCoroutine(WaitTimeAndSetFreeze());
            //_wheel.GetComponent<SkeletonGraphic>().freeze = true;
        }

        IEnumerator WaitTimeAndSetFreeze()
        {
            yield return new WaitForSeconds(0.02f);
            SpineManager.instance.SetFreeze(_wheel, true);
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
            switch (talkIndex)
            {
                case 1:
                    //Debug.LogError("执行下一个步骤");
                    //todo...Bell说话语音：与当代火车的不同之处在于蒸汽火车的动力来源是蒸汽，但是蒸汽并没有直接推动火车的轮子转动起来，轮子是如何与发动机相连的呢？请探究蒸汽火车轮子的秘密。
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                        () =>
                        {
                            ShowTheWheelRotate();
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                    
                    
                    break;
                case 2:
                    //Debug.LogError("执行下两个步骤");
                    
                    //todo...点击语音键，左边的轮子原地循环转动，同时bell做说话动作，并播放语音：我们先来做个小测试，请想一想，在只有一个动力源的情况下怎么样才能让另一个轮子也动起来呢？
                    //todo...结束之后出现语音键
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                    {
                        SpineManager.instance.SetFreeze(_wheel, false);
                        SpineManager.instance.DoAnimation(_wheel, "lunzi", true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                    }, () =>
                    {
                        
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                    
                    
                    break;
                case 3:
                    //Debug.LogError("执行下三个步骤");
                    //todo...点击语音键，轮子淡化成线条示意图，左侧竖着的线条依然转动，同时播放语音：这样是否清晰一点呢？请将你的想法画出来吧。
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SpineManager.instance.DoAnimation(_wheel, "lunzi2", false,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SpineManager.instance.DoAnimation(_wheel, "2", true);
                            });

                    }, () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SpineManager.instance.DoAnimation(_wheel, "2", false);
                        //todo...语毕后左侧竖着的线条再转两圈后停止，停在如图所示的初始位置。
                        //todo..语毕出现小手，从左边点画出直线，做引导示范用，3秒后消失：
                        //_buttonList.Show();
                        SetButtonListVisible(true);
                        DrawExampleLine();
                    }));
                    
                    
                    
                    
                    //DrawExampleLine();

                    break;
                case 4:
                    //Debug.LogError("执行下四个步骤");
                    //todo.. 如何让左右两边都转动起来呢？画出你的想法。.
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,null, () =>
                    {
                        //_buttonList.Show();
                        SetButtonListVisible(true);
                        _isDrawingByMySelf = true;
                        //todo..语毕后左侧竖着的线条再转两圈后停止，停在如图所示的初始位置。
                    }));
                    

                    break;
                case 5:
                    //Debug.LogError("执行第五个步骤");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10));
                    break;
                    

            }
            talkIndex++;
        }

        void Step3()
        {
            DrawExampleLine();
        }

        void DrawExampleLine()
        {
            //Debug.LogError("进入携程");
            mono.StartCoroutine(DrawExampleLineIE());
        }

        IEnumerator DrawExampleLineIE()
        {
            float timer = 0f;
            //_t0 = _startHandPos;
            _t0 = TransferScreenPoint(_startHandPos);
            VectorObject2D currentTarget = GameObjectPool(_line.gameObject);
            _lineList.Add(currentTarget.gameObject);
            bool isAutoDrawing = true;
            _hand.gameObject.SetActive(true);
            bool isResetAllLineOnce = false;
            bool isPlaySound = false;
            while (true)
            {
                if (isAutoDrawing)
                {
                    // _hand.position = Vector3.Lerp(_hand.position, _startHandPos + new Vector3(TransferScreenValue(270f,true), 0, 0),
                    //     0.8f * Time.fixedDeltaTime);
                    //Debug.Log(_handEndPos.position.x);
                    _hand.DOLocalMoveX(_handEndPos.localPosition.x, 2f);
                    DrawingLine(currentTarget, _t0, TransferScreenValue(_hand.position.x, true),
                        TransferScreenValue(_hand.position.y, false));
                    //DrawingLineByScreen(currentTarget, _t0, TransferScreenPoint(_hand.position));
                }

                if (isPlaySound == false)
                {
                    isPlaySound = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                }

                if (timer >= 3f)
                {
                    _hand.gameObject.SetActive(false);
                    isAutoDrawing = false;
                    if (isResetAllLineOnce == false)
                    {
                        isResetAllLineOnce = true;
                        ResetAllLine();
                    }
                }

                if (timer > 10f)
                {
                    timer = 0f;
                    _hand.gameObject.SetActive(true);
                    _t0 = TransferScreenPoint(_startHandPos);
                    _hand.position = _startHandPos;
                    isAutoDrawing = true;
                    isResetAllLineOnce = false;
                    currentTarget = GameObjectPool(_line.gameObject);
                    _lineList.Add(currentTarget.gameObject);
                    isPlaySound = false;
                }
                if (Input.GetMouseButton(0))
                {
                    timer = 3f;
                    isAutoDrawing = false;
                    ResetAllLine();
                    _hand.gameObject.SetActive(false);
                    isPlaySound = false;
                    break;
                }
                yield return new WaitForFixedUpdate();
                timer += Time.fixedDeltaTime;
            }
            yield return new WaitForSeconds(0.2f);
            _isDrawingByMySelf = true;
        }
        void Update()
        {
            if (_isDrawingByMySelf)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.LogError("初始化" + Input.mousePosition);
                    // _t0 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    // _lineStartPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    _t0 = TransferMousePos();
                    _lineStartPoint = TransferMousePos();
                    //Debug.LogError("起点坐标："+_lineStartPoint);
                    if (_currentTarget == null)
                    {
                        _currentTarget = GameObjectPool(_line.gameObject);
                        // _currentTarget.vectorLine = new VectorLine("Line", new List<Vector3>(2), 10f);
                        _lineList.Add(_currentTarget.gameObject);
                    }
                }
                else if (Input.GetMouseButton(0))
                {
                    if (_currentTarget != null)
                    {
                        DrawingLineByScreen(_currentTarget, _t0, TransferMousePos());
                        //DrawingLine(_currentTarget, _t0, Input.mousePosition.x, Input.mousePosition.y);
                    }
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    
                    //_lineEndPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    _lineEndPoint = TransferMousePos();
                    //Debug.Log("终点坐标：" + _lineEndPoint);
                    if (Vector3.Distance(_lineStartPoint, _lineEndPoint) > 10f)
                    {
                        //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        _currentTarget = null;
                        Vector3[] target = new[] {_lineStartPoint, _lineEndPoint};
                        if (!_allFinishLineList.Contains(target))
                            _allFinishLineList.Add(target);
                    }
                    
                    //Debug.Log("增加后的数量："+_allFinishLineList.Count);
                }
            }
            
        }

        Vector2 TransferMousePos()
        {
            //Debug.Log("鼠标：" + Input.mousePosition);
            Vector2 target = TransferScreenPoint(Input.mousePosition);
            // Vector2 target1 = new Vector2(Input.mousePosition.x * 1920f / Screen.width,
            //     Input.mousePosition.y * 1080f / Screen.height);
            return target;
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

        /// <summary>
        /// 判断两条线的连接是否完成
        /// </summary>
        /// <returns>是否完成的回复</returns>
        bool JudgeTwoLineIsConnected()
        {
            for (int i = 0; i < _allFinishLineList.Count; i++)
            {
                if(Vector3.Distance(_lineStartPoint,_lineEndPoint)<5)
                    continue;
                _lineStartPoint = _allFinishLineList[i][0];
                _lineEndPoint = _allFinishLineList[i][1];

                if (_allFinishLineList[i][0].x > _lineEndPoint.x)
                {
                    _lineStartPoint = _allFinishLineList[i][1];
                    _lineEndPoint = _allFinishLineList[i][0];
                }

                Vector3 lineNor = (_lineEndPoint - _lineStartPoint).normalized;
                bool isLeftPointConnect = false;
                bool isRightPointConnect = false;

                Vector3 line1Pos = TransferScreenPoint(_linePoint1.position);
                Vector3 line2Pos = TransferScreenPoint(_linePoint2.position);

                if (Vector3.Distance(_lineStartPoint, line1Pos) <= 10f)
                {

                    //Debug.LogError("左进入距离范围" + Vector3.Distance(_lineStartPoint, _linePoint1.position));
                    isLeftPointConnect = true;

                }
                else if (Vector3.Distance(_lineStartPoint, line1Pos) > 10f)
                {
                    // Debug.LogError("开始的点：" + _lineStartPoint + "第一个点：" + _linePoint1.position);
                    // Debug.LogError("左距离大于10,真实距离：" + Vector3.Distance(_lineStartPoint, _linePoint1.position));
                    if (_lineStartPoint.x < line1Pos.x)
                    {
                        Vector3 nor1 = (line1Pos - _lineStartPoint).normalized;
                        Vector3 nor2 = (line1Pos - _lineEndPoint).normalized;
                        //Debug.LogError("左边的角度：" + Vector3.Angle(nor1, lineNor) + "右边的角度：" + Vector3.Angle(nor2, lineNor));
                        if (Vector3.Angle(nor1, lineNor) < 15f && Vector3.Angle(nor2, -lineNor) < 15f)
                        {
                            isLeftPointConnect = true;
                        }
                    }

                }

                if (Vector3.Distance(_lineEndPoint, line2Pos) <= 10f)
                {
                    // Debug.LogError("开始的点：" + _lineEndPoint + "第二个点：" + _linePoint2.position);
                    // Debug.LogError("右进入距离范围" + Vector3.Distance(_lineEndPoint, _linePoint2.position));
                    isRightPointConnect = true;
                }
                else if (Vector3.Distance(_lineEndPoint, line2Pos) > 10f)
                {
                    //Debug.LogError("右距离大于10,真实距离：" + Vector3.Distance(_lineEndPoint, _linePoint2.position));
                    if (_lineEndPoint.x > line2Pos.x)
                    {
                        Vector3 nor1 = (line2Pos - _lineStartPoint).normalized;
                        Vector3 nor2 = (line2Pos - _lineEndPoint).normalized;
                        //Debug.LogError("左边的角度：" + Vector3.Angle(nor1, lineNor) + "右边的角度：" + Vector3.Angle(nor2, -lineNor));
                        if (Vector3.Angle(nor1, lineNor) < 15f && Vector3.Angle(nor2, -lineNor) < 15f)
                        {
                            isRightPointConnect = true;
                        }
                    }
                }

                if (isLeftPointConnect && isRightPointConnect)
                {
                    return true;
                }
            }

            
            
            return false;
        }
        /// <summary>
        /// 判断三条线的连接是否完成
        /// </summary>
        /// <returns>几种完成方式</returns>
        E_Answer JudgeThreeLineConnect()
        {
            //Debug.Log("数量"+_allFinishLineList.Count);
            for (int i = 0; i < _allFinishLineList.Count; i++)
            {

                _lineStartPoint = _allFinishLineList[i][0];
                _lineEndPoint = _allFinishLineList[i][1];
                if (_allFinishLineList[i][0].x > _lineEndPoint.x)
                {
                    _lineStartPoint = _allFinishLineList[i][1];
                    _lineEndPoint = _allFinishLineList[i][0];
                }

                Vector3 lineNor = (_lineEndPoint - _lineStartPoint).normalized;
                bool isLeftPointConnect = false;
                bool isRightPointConnect = false;
                bool isMiddlePointConnect = false;

                Vector3 line3Pos = TransferScreenPoint(_linePoint3.position);
                Vector3 line4Pos = TransferScreenPoint(_linePoint4.position);
                Vector3 line5Pos = TransferScreenPoint(_linePoint5.position);
                
                if (Vector3.Distance(_lineStartPoint, line3Pos) <= 10f)
                {
                    //Debug.Log("左边的距离："+ Vector3.Distance(_lineStartPoint, _linePoint3.position));
                    isLeftPointConnect = true;

                }
                else if (Vector3.Distance(_lineStartPoint, line3Pos) > 10f)
                {
                    if (_lineStartPoint.x < line3Pos.x)
                    {
                        Vector3 nor1 = (line3Pos - _lineStartPoint).normalized;
                        Vector3 nor2 = (line3Pos - _lineEndPoint).normalized;
                        //Debug.Log("左边角度" + Vector3.Angle(nor1, lineNor) + " 和" + Vector3.Angle(nor2, -lineNor));
                        if (Vector3.Angle(nor1, lineNor) < 25f && Vector3.Angle(nor2, -lineNor) < 25f)
                        {
                           
                            isLeftPointConnect = true;
                        }

                        if (Vector3.Angle(nor1, lineNor) == 0f && Vector3.Angle(nor2, -lineNor) == 0f)
                        {
                            isLeftPointConnect = false;
                        }
                    }

                }

                Vector3 middleNor1 = (line4Pos - _lineStartPoint).normalized;
                Vector3 middleNo2 = (line4Pos - _lineEndPoint).normalized;
                //Debug.Log("中间角度" + Vector3.Angle(middleNor1, lineNor) + " 和" + Vector3.Angle(middleNo2, -lineNor));
                if (Vector3.Angle(middleNor1, lineNor) < 25f && Vector3.Angle(middleNo2, -lineNor) < 25f)
                {
                    isMiddlePointConnect = true;
                }

                if (Vector3.Distance(_lineStartPoint, line4Pos) < 10f ||
                    Vector3.Distance(_lineEndPoint, line4Pos) < 10f)
                {
                    isMiddlePointConnect = true;
                }

                if (Vector3.Distance(_lineEndPoint, line5Pos) <= 10f)
                {
                    //Debug.Log("右边的距离：" + Vector3.Distance(_lineEndPoint, _linePoint5.position));
                    isRightPointConnect = true;
                }
                else if (Vector3.Distance(_lineEndPoint, line5Pos) > 10f)
                {
                    //Debug.Log("超出时两个点的距离" + _lineEndPoint.x + " Point5:" + _linePoint5.position.x);
                    if (_lineEndPoint.x > line5Pos.x)
                    {
                        Vector3 nor1 = (line5Pos - _lineStartPoint).normalized;
                        Vector3 nor2 = (line5Pos - _lineEndPoint).normalized;
                        //Debug.Log("右边角度" + Vector3.Angle(nor1, lineNor) + " 和" + Vector3.Angle(nor2, -lineNor));
                        if (Vector3.Angle(nor1, lineNor) < 25f && Vector3.Angle(nor2, -lineNor) < 25f)
                        {
                            isRightPointConnect = true;
                        }

                        if (Vector3.Angle(nor1, lineNor) == 0f && Vector3.Angle(nor2, -lineNor) == 0f)
                        {
                            isRightPointConnect = false;
                        }
                    }

                }

                // Debug.Log("left" + isLeftPointConnect + " Right" + isRightPointConnect + " Middle:" +
                //           isMiddlePointConnect);
                if (isLeftPointConnect && isMiddlePointConnect && isRightPointConnect)
                    return E_Answer.All;
                else if (isLeftPointConnect && isMiddlePointConnect)
                    return E_Answer.Left;
                else if (isMiddlePointConnect && isRightPointConnect)
                    return E_Answer.Right;
               
            }

            return E_Answer.None;
        }
        /// <summary>
        /// 弃用的方法
        /// </summary>
        /// <returns></returns>
        IEnumerator DrawingTheLineByMySelf()
        {
            VectorObject2D currentTarget = null;
            while (true)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    //Debug.LogError("初始化");
                    _t0 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    currentTarget = GameObjectPool(_line.gameObject);
                    if (!_lineList.Contains(currentTarget.gameObject))
                        _lineList.Add(currentTarget.gameObject);
                }
                else if (Input.GetMouseButton(0))
                {
                    DrawingLine(currentTarget, _t0, Input.mousePosition.x, Input.mousePosition.y);
                }
                else if (Input.GetMouseButtonUp(0))
                {
                    Debug.LogError("起来");
                    currentTarget = null;
                    //ResetAllLine();
                }

                yield return new WaitForFixedUpdate();
            }
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }

        #region 点击事件监听
        /// <summary>
        /// 第一幕火车里面点击播放相应的动画的事件(弃用)
        /// </summary>
        void AddAllTranPartToTheList()
        {
            //string myName=String.Empty;
            for (int i = 0; i < _train.childCount; i++)
            {
                //_trainPart.Add(_train.GetChild(i).GetComponent<Button>());

                string myName = _train.GetChild(i).name;
                _train.GetChild(i).GetComponent<Button>().onClick.AddListener(
                    delegate ()
                    {
                        AddAniToTheButton(_train.gameObject, myName);
                    }
                );
            }
        }

        void AddAniToTheButton(GameObject target, string targetName)
        {
            SpineManager.instance.DoAnimation(target, targetName, false);
        }

        void RemoveAllButtonEvent()
        {
            Button playButton = _buttonList.transform.Find("PlayButton").GetComponent<Button>();
            Button resetButton = _buttonList.transform.Find("ResetButton").GetComponent<Button>();
            _leftButton = _buttonList.transform.Find("LeftButton").GetComponent<Button>();
            _rightButton = _buttonList.transform.Find("RightButton").GetComponent<Button>();
            _playButtonAni = playButton.transform.Find("PlayAni").gameObject;
            _resetButtonAni = resetButton.transform.Find("ResetAni").gameObject;
            playButton.onClick.RemoveAllListeners();
            resetButton.onClick.RemoveAllListeners();
            _leftButton.onClick.RemoveAllListeners();
            _rightButton.onClick.RemoveAllListeners();
        }

        /// <summary>
        /// 增加场景里面所有的Button的点击事件
        /// </summary>
        /// <param name="curTrans"></param>
        void AddAllViewButtonEvent(Transform curTrans)
        {
            RemoveAllButtonEvent();
            
            //Transform buttonListParent = curTrans.Find("ButtonList");
            Button playButton = _buttonList.transform.Find("PlayButton").GetComponent<Button>();
            
            
            playButton.onClick.AddListener(PlayButton);
            Button resetButton = _buttonList.transform.Find("ResetButton").GetComponent<Button>();
            
            
            resetButton.onClick.AddListener(ResetButton);
            
            _leftButton.onClick.AddListener(LeftButton);
            
            _rightButton.onClick.AddListener(RightButton);
            _leftButton.transform.GetChild(0).gameObject.Hide();
            _rightButton.transform.GetChild(0).gameObject.Hide();
        }

        void SetButtonListVisible(bool isShow)
        {
            _buttonList.SetActive(isShow);
            if (isShow)
            {
                if (_myESubject == E_Subject.No1)
                {
                    _leftButton.transform.GetChild(0).gameObject.SetActive(true);
                    _rightButton.transform.GetChild(0).gameObject.SetActive(false);
                }
                else if (_myESubject == E_Subject.No2)
                {
                    _leftButton.transform.GetChild(0).gameObject.SetActive(false);
                    _rightButton.transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }



        /// <summary>
        /// 随机播放失败的语音
        /// </summary>
        /// <returns></returns>
        int PlayRandomFailVoiceIndex()
        {
            int currentPlayIndex = 5;
            switch (_currentTargetVoiceIndex)
            {
                case 1:
                    currentPlayIndex = 5;
                    break;
                case 2:
                    currentPlayIndex = 8;
                    break;
                    
                case 3:
                    currentPlayIndex = 9;
                    break;
            }
            _currentTargetVoiceIndex++;
            return currentPlayIndex;
            
        }

        private bool _firstNo2Success = false;

        void PlayButton()
        {
            SpineManager.instance.DoAnimation(_playButtonAni, "kg2", false,
                () => SpineManager.instance.DoAnimation(_playButtonAni, "kg1", false));
            _isDrawingByMySelf = false;
            if (_myESubject == E_Subject.No1)
            {
                bool isFinished = JudgeTwoLineIsConnected();
                _wheelTrail1.SetActive(true);
                if (isFinished)
                {
                    //todo... 语音：非常好，你做到了
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                    {
                        ResetAllLine();
                        //_wheelTrail2.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheel, "3", true);
                        SpineManager.instance.DoAnimation(_wheelTrail1, "1", true);
                        _wheelTrail2.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail2, "2", true);
                    }));
                    //todo...两条竖边同时做循环的圆周运动，画完一圈后，运动轨迹淡化消失，再转圈时接着画线，循环
                   
                }
                else
                {
                    //todo... 语音：加油，请再想一想。/请再试一次吧。/还记得连杆机构吗？请从这个方向想一想
                    //todo... 只有左侧竖着的线做圆周运动，同时出现运动轨迹虚线。
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, PlayRandomFailVoiceIndex(), () =>
                    {
                        ResetAllLine();

                        SpineManager.instance.DoAnimation(_wheel, "2", true);
                        SpineManager.instance.DoAnimation(_wheelTrail1, "1", true);
                    }));
                    
                }
            }
            else if (_myESubject == E_Subject.No2)
            {
                E_Answer currentAnswer = JudgeThreeLineConnect();
                if (currentAnswer == E_Answer.Left)
                {
                    //todo...左边两条线运动
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () =>
                    {
                        Debug.LogError("左边两条线运动");
                        SpineManager.instance.DoAnimation(_wheel, "7", true);
                        _wheelTrail1.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail1, "4", true);
                        _wheelTrail2.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail2, "5", true);
                        ResetAllLine();
                    }));
                    
                    //todo...语音：另外一边呢？再思考一下。
                    
                }
                else if (currentAnswer == E_Answer.Right)
                {
                    //todo... 右边两条线运动
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () =>
                    {
                        Debug.LogError("右边两条线运动");
                        SpineManager.instance.DoAnimation(_wheel, "8", true);
                        _wheelTrail1.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail1, "4", true);
                        _wheelTrail2.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail2, "6", true);
                        //todo。。。 语音：另外一边呢？再思考一下。
                        ResetAllLine();
                    }));
                    
                }
                else if (currentAnswer == E_Answer.All)
                {
                    //todo... 所有线运动
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () =>
                    {
                        Debug.LogError("三条线运动");
                        ResetAllLine();
                        SpineManager.instance.DoAnimation(_wheel, "6", true);
                        _wheelTrail1.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail1, "4", true);
                        _wheelTrail2.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail2, "5", true);
                        _wheelTrail3.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_wheelTrail3, "6", true);
                        if (_firstNo2Success == false)
                        {
                            _firstNo2Success = true;
                            //todo。。 当切换到第二题，并首次完成连接三条竖边后，出现语音键，点击语音键
                            SoundManager.instance.ShowVoiceBtn(true);
                        }
                    }));
                   

                    //todo... 语音：非常好，你做到了 
                    
                    //todo... 点击的步骤5： 语音：没错，这就是双曲柄机构了。做圆周运动的两条杆是连架杆，下面的杆是机架，上面的杆是连杆。只要在平面四杆机构中两条连架杆都是做圆周运动的，我们就可以说它是双曲柄机构。注意，机架不一定都是可见的。
                }
                else if (currentAnswer == E_Answer.None)
                {
                    //todo...只有一条线运动
                    Debug.LogError("只有一条线运动");
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, PlayRandomFailVoiceIndex(),
                        () =>
                        {
                            ResetAllLine();
                            SpineManager.instance.DoAnimation(_wheel, "5", true);
                            _wheelTrail2.SetActive(true);
                            SpineManager.instance.DoAnimation(_wheelTrail2, "4", true);
                        }));
                    
                    //todo... 语音：加油，请再想一想。/请再试一次吧。/还记得连杆机构吗？请从这个方向想一想。
                }
            }
            _allFinishLineList.Clear();

        }

        void ResetButton()
        {
            SpineManager.instance.DoAnimation(_resetButtonAni, "kg4", false,
                () => SpineManager.instance.DoAnimation(_resetButtonAni, "kg3", false));
            _isDrawingByMySelf = true;
            if (_myESubject == E_Subject.No1)
            {
                ResetAllLine();
                SpineManager.instance.DoAnimation(_wheel, "1", false);
                if (_wheelTrail1.activeSelf) _wheelTrail1.SetActive(false);
                if (_wheelTrail2.activeSelf) _wheelTrail2.SetActive(false);
            }
            if (_myESubject == E_Subject.No2)
            {
                ResetAllLine();
                SpineManager.instance.DoAnimation(_wheel, "4", false);
                if (_wheelTrail1.activeSelf) _wheelTrail1.SetActive(false);
                if(_wheelTrail2.activeSelf) _wheelTrail2.SetActive(false);
                if (_wheelTrail3.activeSelf) _wheelTrail3.SetActive(false);
            }


            //todo...停止运动。回到初始位置，游戏继续
        }

        void LeftButton()
        {
            if (_myESubject == E_Subject.No2)
            {
                BtnPlaySound();
                _myESubject = E_Subject.No1;
                SpineManager.instance.DoAnimation(_wheel, "1", false);
                _wheelTrail1.SetActive(false);
                _wheelTrail2.SetActive(false);
                _wheelTrail3.SetActive(false);
                ResetAllLine();
                _isDrawingByMySelf = true;
                SetButtonListVisible(true);
                //todo... 切换到上一题
            }
        }

        private bool _isFirstTimeToNo2 = false;

        void RightButton()
        {
            if (_myESubject == E_Subject.No1)
            {
                BtnPlaySound();
                _myESubject = E_Subject.No2;
                
                _isDrawingByMySelf = false;
                ResetAllLine();
                if (_isFirstTimeToNo2 == false)
                {
                    
                    _isFirstTimeToNo2 = true;
                    // _buttonList.Hide();
                    SetButtonListVisible(false);
                    SoundManager.instance.ShowVoiceBtn(true);
                    //Debug.LogError("zhixingfangfa ");
                    //步骤4：播放语音
                    SpineManager.instance.DoAnimation(_wheel, "5", false);
                }
                else
                {
                    SetButtonListVisible(true);
                    SpineManager.instance.DoAnimation(_wheel, "5", false, () => _isDrawingByMySelf = true);
                }
                


                _wheelTrail1.SetActive(false);
                _wheelTrail2.SetActive(false);
                //_isDrawingByMySelf = true;

                //todo..切换到下一题
            }
        }

        #endregion



        #region 画线部分

        void ResetLineProperty(Transform curTrans)
        {
            _posList = new List<Vector2>(2);
            _posList.Add(new Vector2(0, 0));
            _posList.Add(new Vector2(1, 1));
            _lineList = new List<GameObject>();
            _lineParent = curTrans.Find("LineList");
            _line = _lineParent.Find("Line").GetComponent<VectorObject2D>();
            _lineList.Add(_line.gameObject);
        }

        VectorObject2D GameObjectPool(GameObject target)
        {
            if (_lineList.Contains(target))
            {
                //print("1");
                // GameObject newTarget = _lineList.Find(p => !p.activeSelf);
                GameObject newTarget = null;
                for (int i = 1; i < _lineList.Count; i++)
                {
                    if (!_lineList[i].activeInHierarchy)
                    {
                        newTarget = _lineList[i];
                    }
                }
                if (newTarget == null)
                {
                    //Debug.LogError("2");
                    newTarget = UnityEngine.Object.Instantiate(_line.gameObject, _lineParent);
                }
                if(newTarget.activeSelf==false)
                {
                    //Debug.LogError("显示出来了");
                    newTarget.SetActive(true);

                }

                return newTarget.GetComponent<VectorObject2D>();
            }
            else
            {
                Debug.LogError("LineList没找到目标，请检查！ " + target.name);
                return null;
            }
        }

        void ResetAllLine()
        {
            //Debug.LogError("长度：" + _lineList.Count + " " + _lineList[0].gameObject+_lineList[1].gameObject);
            
            // if (_lineList.Count > 1)
            // {
                for (int i = 1; i < _lineList.Count; i++)
                {
                    DrawingLine(_lineList[i].GetComponent<VectorObject2D>(), Vector2.zero, -20f, -20f);
                    _lineList[i].Hide();
                    //UnityEngine.Object.Destroy(_lineList[i]);
                }
                _allFinishLineList.Clear();
                
               
            // }
            //Debug.LogError("长度：" + _lineList.Count + " " + _lineList[0].gameObject);



        }

        void DrawingLine(VectorObject2D line, Vector2 origin, float targetX, float targetY)
        {
            _posList[0] = origin;
            _posList[1] = new Vector2(targetX, targetY);
            line.vectorLine.points2 = _posList;
            line.vectorLine.Draw();
        }

        void DrawingLineByScreen(VectorObject2D line, Vector2 origin, Vector2 end)
        {
            _posList[0] = origin;
            _posList[1] = end;
            line.vectorLine.points2 = _posList;
            line.vectorLine.Draw();
        }

        #endregion
    }
}
