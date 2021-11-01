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
    }

    public enum DragDirection
    {
        left,
        right,
        up,
        down,
        leftup,
        leftdown,
        rightup,
        rightdown,
        stop,
    }
    public class TD3432Part1
    {
        public class TheZone
        {
            public bool IsRoad;
            public float X;
            public float Y;
            public Vector3[] VertexPos;

            public TheZone(RectTransform rect)
            {
                var localPos = rect.localPosition;
                X = localPos.x;
                Y = localPos.y;
                VertexPos = GetVertexLocalPosition(rect);
            }

            private Vector3[] GetVertexLocalPosition(RectTransform rectTransform)
            {
                Vector3[] vertexPos = new Vector3[4];

                var rect = rectTransform.rect;

                var localPosition = rectTransform.localPosition;
                var localPosX = localPosition.x;
                var localPosY = localPosition.y;

                var xMin = rect.xMin; var yMin = rect.yMin;
                var xMan = rect.xMax; var yMax = rect.yMax;

                Vector3 leftDown = new Vector3(localPosX + xMin, localPosY + yMin, 0);
                Vector3 leftUp = new Vector3(localPosX + xMin, localPosY + yMax, 0);
                Vector3 rightUp = new Vector3(localPosX + xMan, localPosY + yMax, 0);
                Vector3 rightDown = new Vector3(localPosX + xMan, localPosY + yMin, 0);

                vertexPos[0] = leftDown;
                vertexPos[1] = leftUp;
                vertexPos[2] = rightUp;
                vertexPos[3] = rightDown;

                return vertexPos;
            }
        }

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject tt;
        private GameObject dtt;
        private GameObject Bg;
        private BellSprites bellTextures;

        private bool _canClickBtn;
        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject _octopus;    //动画

        //碰撞与移动用
        private MonoScripts _monoScripts;
        private Transform _wall;
        private Transform _road;
        public Dictionary<string, TheZone> _zoneDic;
        public GameObject _player;              //玩家（Obj）  
        public float _moveSpeed;                //移动速度
        public float _xMax; public float _yMax;
        public float _xMin; public float _yMin;
        private Vector2 _tempV2 = new Vector2(0, 0);
        private Vector3[] _playerVertexPos;
        private Vector2 _lastPos;

        private DragDirection _dragDirection;
        private mILDrager _dragBall;
        private bool _isDraging;
        private bool _canMove;
        private bool _canTrigger;
        private Vector3 _dragRePos;

        //游戏方面
        private EventDispatcher _playerEvent;
        private string _curAni;

        //胜利动画名字
        private string tz;
        private string sz;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            //buDing = curTrans.Find("mask/buDing").gameObject;
            //bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            //bdStartPos = curTrans.Find("mask/bdStartPos");
            //buDing.transform.position = bdStartPos.position;
            //bdEndPos = curTrans.Find("mask/bdEndPos");

            //devil = curTrans.Find("mask/devil").gameObject;
            //devilText = devil.transform.GetChild(0).GetComponent<Text>();
            //devilStartPos = curTrans.Find("mask/devilStartPos");
            //devil.transform.position = devilStartPos.position;
            //devilEndPos = curTrans.Find("mask/devilEndPos");

            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dtt = curTrans.Find("mask/DTT").gameObject;
            dtt.SetActive(false);
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
            ChangeClickArea();

            //pageBar = curTrans.Find("PageBar").gameObject;
            //SlideSwitchPage(pageBar);
            //SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            //SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            //e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            //for (int i = 0, len = e4rs.Length; i < len; i++)
            //{
            //    Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            //}

            //leftBtn = curTrans.Find("L2/L").gameObject;
            //rightBtn = curTrans.Find("R2/R").gameObject;

            //Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            //Util.AddBtnClick(rightBtn, OnClickBtnRight);

            //btnBack = curTrans.Find("btnBack").gameObject;
            //Util.AddBtnClick(btnBack, OnClickBtnBack);
            //btnBack.SetActive(false);

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _wall = curTrans.Find("Wall");
            _road = curTrans.Find("Road");
            _zoneDic = new Dictionary<string, TheZone>(_wall.childCount + _road.childCount);
            for (int i = 0; i < _wall.childCount; i++)
            {
                TheZone zone = new TheZone(_wall.GetChild(i).gameObject.GetComponent<RectTransform>());
                Debug.Log("Wall[" + i + "].name :" + _wall.GetChild(i).name);
                _zoneDic.Add(_wall.GetChild(i).name, zone);
            }
            for (int i = 0; i < _road.childCount; i++)
            {
                TheZone zone = new TheZone(_road.GetChild(i).gameObject.GetComponent<RectTransform>());
                Debug.Log("Road[" + i + "].name :" + _road.GetChild(i).name);
                _zoneDic.Add(_road.GetChild(i).name, zone);
            }
            _monoScripts = curTrans.GetGameObject("MonoScripts").GetComponent<MonoScripts>();
            _monoScripts.UpdateCallBack = SUpdate;

            _dragBall = curTrans.GetGameObject("dragBall").GetComponent<mILDrager>();
            _dragRePos = curTrans.GetGameObject("dragBall").transform.localPosition;

            _player = curTrans.GetGameObject("Octopus");
            _octopus = curTrans.GetGameObject("Octopus/Octopus");
            _playerEvent = _player.GetComponent<EventDispatcher>();
            InitData();
            GameInit();
        }


        //private void OnClickBtnRight(GameObject obj)
        //{
        //    if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
        //        return;
        //    isPressBtn = true;
        //    BtnPlaySound();
        //    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        //}

        //private void OnClickBtnLeft(GameObject obj)
        //{
        //    if (curPageIndex <= 0 || isPlaying || isPressBtn)
        //        return;
        //    isPressBtn = true;
        //    BtnPlaySound();
        //    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        //}

        //private GameObject tem;
        //private void OnClickBtnBack(GameObject obj)
        //{
        //    if (isPressBtn)
        //        return;
        //    isPressBtn = true;
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
        //    SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false); isPlaying = false; isPressBtn = false; }); });
        //}

        //private void OnClickShow(GameObject obj)
        //{
        //    if (isPlaying || isPressBtn)
        //        return;
        //    isPlaying = true;
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
        //    tem = obj.transform.parent.gameObject;
        //    tem.transform.SetAsLastSibling();
        //    SpineManager.instance.DoAnimation(tem, obj.name, false, () => { SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () => { btnBack.SetActive(true); }); });
        //}


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
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
            _canClickBtn = true;
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if(_canClickBtn)
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); dtt.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dtt,SoundManager.SoundType.VOICE, 2)); });
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

        private void GameInit()
        {
            talkIndex = 1;

            _dragDirection = DragDirection.stop;
            _moveSpeed = 0;

            _canClickBtn = true;
            _canTrigger = true;
            _isDraging = false;
            _canMove = true;

            _playerEvent.TriggerEnter2D -= PlayerEvent;
            _playerEvent.TriggerEnter2D += PlayerEvent;

            InitAni();
            InitDrag();
            //curPageIndex = 0;
            //isPressBtn = false;
            //textSpeed =0.5f;
            ////SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            //for (int i = 0; i < SpinePage.childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            //}

            //SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            //SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }

        //动画、位置等初始化
        void InitAni()
        {
            //动画
            _octopus.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_octopus, "zya", true);
            //显示隐藏

            //位置
            _player.transform.position = curTrans.GetGameObject("PlayerPos").transform.position;
            _player.transform.localScale = new Vector3(1, 1, 1);

        }

        void GameStart()
        {

            tt.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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

        //等待协程
        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    tt.SetActive(false);
                    mask.SetActive(false);
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                () =>
                {  /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
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
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        #region 移动
        private void SUpdate()
        {
            Move();
        }

        void Move()
        {
            if (_isDraging && _canMove)
            {
                if (_dragDirection == DragDirection.left)
                {
                    _player.transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (_dragDirection == DragDirection.right)
                {
                    _player.transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (_dragDirection == DragDirection.up)
                {
                    _player.transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
                }
                else if (_dragDirection == DragDirection.down)
                {
                    _player.transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
                }
                else if (_dragDirection == DragDirection.rightup)
                {
                    _player.transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
                    _player.transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (_dragDirection == DragDirection.rightdown)
                {
                    _player.transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
                    _player.transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (_dragDirection == DragDirection.leftup)
                {
                    _player.transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
                    _player.transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (_dragDirection == DragDirection.leftdown)
                {
                    _player.transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
                    _player.transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
                    _player.transform.localScale = new Vector3(-1, 1, 1);
                }
                var playerRect = _player.GetComponent<RectTransform>();
                GetPlayerVertexLocalPos(playerRect);
                var curLocalPos = playerRect.localPosition;
                var curX = curLocalPos.x;
                var curY = curLocalPos.y;
                GetCurGrid();

                IsExceedMaxRanage(curX, curY, playerRect);

                _lastPos = playerRect.localPosition;
            }
        }
        #endregion

        #region 碰撞检测
        void InitData()
        {
            for (int i = 0; i < _road.childCount; i++)
            {
                int key = Convert.ToInt32(_road.GetChild(i).gameObject.name);
                TheZone z = _zoneDic[key.ToString()];
                z.IsRoad = true;
            }

            for (int i = 0; i < _wall.childCount; i++)
            {
                int key = Convert.ToInt32(_wall.GetChild(i).gameObject.name);
                TheZone z = _zoneDic[key.ToString()];
                z.IsRoad = false;
            }

            //var mapVertexPos = GetVertexLocalPos(MapParent);
            //var plaerRect = Player.GetComponent<RectTransform>().rect;
            //XMin = mapVertexPos[0].x + plaerRect.xMax;
            //YMin = mapVertexPos[0].y + plaerRect.yMax;
            //XMax = mapVertexPos[2].x + plaerRect.xMin;
            //YMax = mapVertexPos[2].y + plaerRect.yMin;
            _xMin = -Screen.width / 2;
            _yMin = -Screen.height / 2;
            _xMax = Screen.width / 2;
            _yMax = Screen.height / 2;

            _playerVertexPos = GetVertexLocalPos(_player.GetComponent<RectTransform>());
            _lastPos = _player.GetComponent<RectTransform>().localPosition;

        }

        /// <summary>
        /// 获取当前在哪个格子上
        /// </summary>
        void GetCurGrid()
        {
            // RectTransform curGrid = null;

            var playerVerLocalPos = _playerVertexPos;
            var playerPos = _player.GetComponent<RectTransform>().localPosition;
            var leftDown = playerVerLocalPos[0];
            var leftUp = playerVerLocalPos[1];
            var rightUp = playerVerLocalPos[2];
            var rightDown = playerVerLocalPos[3];

            foreach (var value in _zoneDic.Values)
            {
                var myItem = value;
                var xMin = myItem.VertexPos[0].x;
                var xMax = myItem.VertexPos[2].x;
                var yMin = myItem.VertexPos[0].y;
                var yMax = myItem.VertexPos[2].y;

                bool isLeftDown = (xMin < leftDown.x && leftDown.x < xMax) && (yMin < leftDown.y && leftDown.y < yMax);
                bool isLeftUp = (xMin < leftUp.x && leftUp.x < xMax) && (yMin <= leftUp.y && leftUp.y < yMax);
                bool isRightUp = (xMin < rightUp.x && rightUp.x < xMax) && (yMin < rightUp.y && rightUp.y < yMax);
                bool isRightDown = (xMin < rightDown.x && rightDown.x < xMax) && (yMin < rightDown.y && rightDown.y < yMax);

                if (isLeftDown || isLeftUp || isRightUp || isRightDown)
                {
                    if (!myItem.IsRoad)
                    {
                        _player.GetComponent<RectTransform>().localPosition = _lastPos;
                        break;
                    }
                }
            }
            //if (!curGrid.GetComponent<MyItem>().IsWay)        
            //    Player.GetComponent<RectTransform>().localPosition = _lastPos;

            //return curGrid;
        }


        /// <summary>
        /// 是否超出最大范围
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        bool IsExceedMaxRanage(float x, float y, RectTransform rect)
        {
            bool isXMin = x < _xMin; bool isYMin = y < _yMin;
            bool isXMax = x > _xMax; bool isYMax = y > _yMax;

            bool isX = isXMin || isXMax;
            bool isY = isYMin || isYMax;
            bool result = isX || isY;

            //Debug.LogError("result：" + result + " - isXMin："+ isXMin + " - isXMax：" + isXMax + " - isYMin：" + isYMin + " - isYMax：" + isYMax);

            if (result)
            {
                var localPosition = rect.localPosition;

                if (isXMin)
                    rect.localPosition = SetPos(_xMin, localPosition.y);

                if (isYMin)
                    rect.localPosition = SetPos(localPosition.x, _yMin);

                if (isXMax)
                    rect.localPosition = SetPos(_xMax, localPosition.y);

                if (isYMax)
                    rect.localPosition = SetPos(localPosition.x, _yMax);

                if (isXMin && isYMax)
                    rect.localPosition = SetPos(_xMin, _yMax);

                if (isXMin && isYMin)
                    rect.localPosition = SetPos(_xMin, _yMin);

                if (isXMax && isYMax)
                    rect.localPosition = SetPos(_xMax, _yMax);

                if (isXMax && isYMin)
                    rect.localPosition = SetPos(_xMax, _yMin);
            }
            return result;
        }

        Vector2 SetPos(float x, float y)
        {
            _tempV2.x = x;
            _tempV2.y = y;
            return _tempV2;
        }

        private Vector3[] GetVertexLocalPos(RectTransform rectTransform)
        {
            Vector3[] vertexPos = new Vector3[4];

            var rect = rectTransform.rect;

            var localPosition = rectTransform.localPosition;
            var localPosX = localPosition.x;
            var localPosY = localPosition.y;

            var xMin = rect.xMin; var yMin = rect.yMin;
            var xMan = rect.xMax; var yMax = rect.yMax;

            Vector3 leftDown = new Vector3(localPosX + xMin, localPosY + yMin, 0);
            Vector3 leftUp = new Vector3(localPosX + xMin, localPosY + yMax, 0);
            Vector3 rightUp = new Vector3(localPosX + xMan, localPosY + yMax, 0);
            Vector3 rightDown = new Vector3(localPosX + xMan, localPosY + yMin, 0);

            vertexPos[0] = leftDown;
            vertexPos[1] = leftUp;
            vertexPos[2] = rightUp;
            vertexPos[3] = rightDown;

            return vertexPos;
        }

        private Vector3[] GetPlayerVertexLocalPos(RectTransform rectTransform)
        {
            var rect = rectTransform.rect;

            var localPosition = rectTransform.localPosition;
            var localPosX = localPosition.x;
            var localPosY = localPosition.y;

            var xMin = rect.xMin; var yMin = rect.yMin;
            var xMan = rect.xMax; var yMax = rect.yMax;

            _playerVertexPos[0].x = localPosX + xMin;
            _playerVertexPos[0].y = localPosY + yMin;

            _playerVertexPos[1].x = localPosX + xMin;
            _playerVertexPos[1].y = localPosY + yMax;

            _playerVertexPos[2].x = localPosX + xMan;
            _playerVertexPos[2].y = localPosY + yMax;

            _playerVertexPos[3].x = localPosX + xMan;
            _playerVertexPos[3].y = localPosY + yMin;

            return _playerVertexPos;
        }
        #endregion

        #region 摇杆拖拽

        void InitDrag()
        {
            _dragBall.DoReset();
            _dragBall.canMove = true;
            _dragBall.SetDragCallback(null, Draging, EndDrag, null);
        }

        //拖拽中
        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            _isDraging = true;
            float dis = Vector3.Distance(dragPos, _dragRePos);
            float x = dragPos.x - _dragRePos.x;
            float y = dragPos.y - _dragRePos.y;

            if (dis == 0)
                _moveSpeed = 0;
            else if (dis > 0 && dis <= 20)
                _moveSpeed = 50;
            else if (dis > 20 && dis <= 40)
                _moveSpeed = 100;
            else
                _moveSpeed = 200;

            //摇杆向右
            if (x > 0 && Math.Abs(x) > Math.Abs(y))
                _dragDirection = DragDirection.right;
            //摇杆向左
            else if (x < 0 && Math.Abs(x) > Math.Abs(y))
                _dragDirection = DragDirection.left;
            //摇杆向上
            else if (y > 0 && Math.Abs(y) > Math.Abs(x))
                _dragDirection = DragDirection.up;
            //摇杆向下
            else if (y < 0 && Math.Abs(y) > Math.Abs(x))
                _dragDirection = DragDirection.down;
            //摇杆向右上
            else if (x > 0 && y > 0 && Math.Abs(x) == Math.Abs(y))
                _dragDirection = DragDirection.rightup;
            //摇杆向右下
            else if (x > 0 && y < 0 && Math.Abs(x) == Math.Abs(y))
                _dragDirection = DragDirection.rightdown;
            //摇杆向左上
            else if (x < 0 && y > 0 && Math.Abs(x) == Math.Abs(y))
                _dragDirection = DragDirection.leftup;
            //摇杆向左下
            else if (x < 0 && y < 0 && Math.Abs(x) == Math.Abs(y))
                _dragDirection = DragDirection.leftdown;
            else
                _dragDirection = DragDirection.stop;
        }

        //结束拖拽
        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _isDraging = false;
            _dragBall.DoReset();
            _dragDirection = DragDirection.stop;
            _moveSpeed = 0;
        }

        #endregion

        #region 游戏流程相关

        //触发事件
        private void PlayerEvent(Collider2D other, int time)
        {
            if(_canTrigger)
            {
                _curAni = SpineManager.instance.GetCurrentAnimationName(_octopus);
                if (other.name == "Arrow")
                {
                    //游戏结束
                    _canTrigger = false;
                    FinalSuccess();
                }
                else
                {
                    _canTrigger = false;
                    //一开始碰撞与其他颜色的碰撞
                    if (_curAni == "zya" || _curAni[3].ToString() != other.name)
                    {
                        _canMove = false;
                        string nextAni = other.name;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SpineManager.instance.DoAnimation(_octopus, "zyb" + nextAni, false,
                        () =>
                        {
                            _canTrigger = true; _canMove = true;
                            SpineManager.instance.DoAnimation(_octopus, "zya" + nextAni, true);
                        });
                    }
                    else
                    {
                        _canTrigger = true;
                    }
                }
            }
        }

        //胜利界面
        void FinalSuccess()
        {
            _dragBall.canMove = false;
            _canMove = false;
            _isDraging = false;
            _dragBall.DoReset();

            string s = SpineManager.instance.GetCurrentAnimationName(_octopus)[3].ToString();
            SpineManager.instance.DoAnimation(_octopus, "zyc" + s, true);
            _player.transform.DOMove(curTrans.Find("PlayerEndPos").position, 3.0f);
            mono.StartCoroutine(WaitCoroutine(
            () =>
            {
                playSuccessSpine(
                () =>
                {
                    _canClickBtn = true;
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    ChangeClickArea();
                });
            }, 4.0f));
        }
        #endregion
    }
}