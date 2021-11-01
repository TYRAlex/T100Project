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
        stop,
    }
    public class TD6711Part1
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
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;
        private GameObject _final;

        private GameObject _startBg;
        private GameObject _qt;

        //碰撞与移动用
        private MonoScripts _monoScripts;
        private Transform _wall;
        private Transform _road;
        public Dictionary<string, TheZone> _zoneDic;
        public GameObject _player;              //玩家  
        public float _moveSpeed;                //移动速度
        public float _xMax; public float _yMax;
        public float _xMin; public float _yMin;
        private Vector2 _tempV2 = new Vector2(0, 0);
        private Vector3[] _playerVertexPos;
        private Vector2 _lastPos;

        private DragDirection _dragDirection;
        private GameObject _qianTing;
        private mILDrager _dragBall;
        private bool _isDraging;
        private bool _canMove;
        private Vector3 _dragRePos;

        //游戏方面
        private float _triggerXMin;
        private float _triggerXMax;
        private float _triggerYMin;
        private float _triggerYMax;
        private GameObject[] _triggerArray;
        private GameObject[] _hmArray;
        private GameObject[] _eyArray;
        private GameObject _chooseMask;
        private int _count;     //获得的分数
        private int _number;    //遭遇的鳄鱼数
        private GameObject _click1;
        private GameObject _click2;
        private bool _canClickDiamond;
        //private GameObject pageBar;
        //private Transform SpinePage;

        //private Empty4Raycast[] e4rs;

        //private GameObject rightBtn;
        //private GameObject leftBtn;

        //private GameObject btnBack;

        //private int curPageIndex;  //当前页签索引
        //private Vector2 _prePressPos;

        //private float textSpeed;

        ////用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        //private GameObject buDing;
        //private Text bdText;
        //private GameObject devil;
        //private Text devilText;

        //private Transform bdStartPos;
        //private Transform bdEndPos;
        //private Transform devilStartPos;
        //private Transform devilEndPos;

        //胜利动画名字
        private string tz;
        private string sz;
        //bool isPlaying = false;
        //bool isPressBtn = false;
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
            sz = "6-12";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _wall = curTrans.Find("Collider/Wall");
            _road = curTrans.Find("Collider/Road");
            _zoneDic = new Dictionary<string, TheZone>(_wall.childCount + _road.childCount);
            for (int i = 0; i < _wall.childCount; i++)
            {
                TheZone zone = new TheZone(_wall.GetChild(i).gameObject.GetComponent<RectTransform>());
                _zoneDic.Add(_wall.GetChild(i).name, zone);
            }
            for (int i = 0; i < _road.childCount; i++)
            {
                TheZone zone = new TheZone(_road.GetChild(i).gameObject.GetComponent<RectTransform>());
                _zoneDic.Add(_road.GetChild(i).name, zone);
            }
            _monoScripts = curTrans.GetGameObject("MonoScripts").GetComponent<MonoScripts>();
            _monoScripts.UpdateCallBack = SUpdate;
            _player = curTrans.GetGameObject("Player");

            _dragBall = curTrans.GetGameObject("DragBall").GetComponent<mILDrager>();
            _dragRePos = curTrans.GetGameObject("DragBall").transform.localPosition;
            _qianTing = _player.transform.GetGameObject("QianTing");
            _chooseMask = curTrans.GetGameObject("ChooseMask");

            _click1 = curTrans.GetGameObject("Click/0");
            _click2 = curTrans.GetGameObject("Click/1");
            Util.AddBtnClick(_click1, ClickEvent);
            Util.AddBtnClick(_click2, ClickEvent);
            _triggerArray = new GameObject[curTrans.Find("Trigger").childCount];
            for (int i = 0; i < curTrans.Find("Trigger").childCount; i++)
            {
                _triggerArray[i] = curTrans.Find("Trigger").GetChild(i).gameObject;
            }
            _hmArray = new GameObject[curTrans.Find("hm").childCount];
            for (int i = 0; i < curTrans.Find("hm").childCount; i++)
            {
                _hmArray[i] = curTrans.Find("hm").GetChild(i).gameObject;
            }
            _eyArray = new GameObject[curTrans.Find("ey").childCount];
            for (int i = 0; i < curTrans.Find("ey").childCount; i++)
            {
                _eyArray[i] = curTrans.Find("ey").GetChild(i).gameObject;
            }

            _startBg = curTrans.GetGameObject("StartBg");
            _qt = curTrans.GetGameObject("StartBg/QT");
            _final = successSpine.transform.GetGameObject("Final");
            _startBg.Show();
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
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); dbd.SetActive(true);  mono.StartCoroutine(DBDSpeckerCoroutine(SoundManager.SoundType.VOICE, 2)); });
                }

            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            _count = 0;
            _number = 0;

            _dragDirection = DragDirection.stop;
            _moveSpeed = 0;
            _moveSpeed = _moveSpeed * (Screen.width / 1920f);
            _isDraging = false;
            _canMove = true;
            _canClickDiamond = true;

            NewTriggerData();
            InitAni();
            JudgeCount();
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
            SpineManager.instance.DoAnimation(bd, "bd-daiji", true);
            SpineManager.instance.DoAnimation(_qianTing, "jq", true);
            SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("Choose"), "kong", false);
            SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("HM"), "kong", false);
            for (int i = 0; i < _hmArray.Length; i++)
            {
                string num = (i + 1).ToString();
                int ran = Random.Range(0, 5);
                GameObject o = _hmArray[i];
                SpineManager.instance.DoAnimation(o, "qhm" + num, true);
                mono.StartCoroutine(WaitCoroutine(
                () =>
                {
                    SpineManager.instance.DoAnimation(o, "qhm" + num, true);
                }, (ran * 0.05f)));
            }

            //显示隐藏
            _chooseMask.Hide();
            _final.Hide();
            for (int i = 0; i < _final.transform.childCount; i++)
            {
                _final.transform.GetChild(i).gameObject.Hide();
            }
            for (int i = 0; i < _eyArray.Length; i++)
            {
                _eyArray[i].Show();
            }

            //位置尺寸颜色等
            _qianTing.transform.localScale = new Vector3(1, 1, 1);
            for (int i = 0; i < _eyArray.Length; i++)
            {
                _eyArray[i].GetComponent<RawImage>().color = Color.white;
            }
            _player.transform.localPosition = curTrans.GetGameObject("PlayerPos").transform.localPosition;

            _qt.GetComponent<RectTransform>().localScale = new Vector3(0, 0, 0);
            _qt.GetComponent<CanvasGroup>().alpha = 0;

        }

        void GameStart()
        {
            //buDing.transform.DOMove(bdEndPos.position,1f).OnComplete(()=> {/*正义的一方对话结束 devil开始动画*/

            //    devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() => {/*对话*/ });
            //});
            //isPlaying = true;
            bd.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, ()=> { SoundManager.instance.ShowVoiceBtn(true); }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //大布丁说话
        IEnumerator DBDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dbd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, 
                () => 
                {
                    bd.SetActive(false);
                    mask.SetActive(false);
                    _qt.GetComponent<CanvasGroup>().DOFade(1, 3.5f);
                    _qt.GetComponent<RectTransform>().DOScale(new Vector3(1, 1, 1), 3.5f);
                    mono.StartCoroutine(WaitCoroutine(() => { SoundManager.instance.ShowVoiceBtn(true); }, 4.0f));
                }));
            }
            if(talkIndex == 2)
            {
                _dragBall.canMove = true;
                _startBg.Hide();
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
            mono.StartCoroutine(WaitCoroutine(
            () => 
            {
                _final.Show();
                _final.transform.GetChild(_count).gameObject.Show();
            }, 0.3f));
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "-2", false,
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
        //private void setmoveancposx(int lorr, float duration = 1f, action callback = null)
        //{
        //    if (isplaying)
        //        return;
        //    isplaying = true;
        //    curpageindex -= lorr;
        //    spinepage.getrecttransform().doanchorposx(spinepage.getrecttransform().anchoredposition.x + lorr * 1920, duration).oncomplete(() => { callback?.invoke(); isplaying = false; });
        //}

        //private void SlideSwitchPage(GameObject rayCastTarget)
        //{
        //    UIEventListener.Get(rayCastTarget).onDown = downData =>
        //    {
        //        _prePressPos = downData.pressPosition;
        //    };

        //    UIEventListener.Get(rayCastTarget).onUp = upData =>
        //    {
        //        float dis = (upData.position - _prePressPos).magnitude;
        //        bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

        //        if (dis > 100)
        //        {
        //            if (!isRight)
        //            {
        //                if (curPageIndex <= 0 || isPlaying)
        //                    return;
        //                SetMoveAncPosX(1);
        //            }
        //            else
        //            {
        //                if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
        //                    return;
        //                SetMoveAncPosX(-1);
        //            }
        //        }
        //    };
        //}

        //void ShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        //}

        //IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    int i = 0;
        //    str = str.Replace(" ", "\u00A0");  //空格非换行
        //    while (i <= str.Length - 1)
        //    {
        //        yield return new WaitForSeconds(textSpeed);
        //        text.text += str[i];
        //        i++;
        //    }
        //    callBack?.Invoke();
        //    yield break;
        //}

        #region 移动
        private void SUpdate()
        {
            Move();
            EnterTrigger();
        }

        void Move()
        {
            if (_isDraging && _canMove)
            {
                if (_dragDirection == DragDirection.left)
                {
                    _player.transform.Translate(Vector3.left * _moveSpeed * Time.deltaTime);
                    if(!SpineManager.instance.GetCurrentAnimationName(_qianTing).Equals("jq2"))
                        SpineManager.instance.DoAnimation(_qianTing, "jq2", true);
                    _qianTing.transform.localScale = new Vector3(-1, 1, 1);
                }
                else if (_dragDirection == DragDirection.right)
                {
                    _player.transform.Translate(Vector3.right * _moveSpeed * Time.deltaTime);
                    if (!SpineManager.instance.GetCurrentAnimationName(_qianTing).Equals("jq2"))
                        SpineManager.instance.DoAnimation(_qianTing, "jq2", true);
                    _qianTing.transform.localScale = new Vector3(1, 1, 1);
                }
                else if (_dragDirection == DragDirection.up)
                {
                    _player.transform.Translate(Vector3.up * _moveSpeed * Time.deltaTime);
                    if (!SpineManager.instance.GetCurrentAnimationName(_qianTing).Equals("jq"))
                        SpineManager.instance.DoAnimation(_qianTing, "jq", true);
                }
                else if (_dragDirection == DragDirection.down)
                {
                    _player.transform.Translate(Vector3.down * _moveSpeed * Time.deltaTime);
                    if (!SpineManager.instance.GetCurrentAnimationName(_qianTing).Equals("jq"))
                        SpineManager.instance.DoAnimation(_qianTing, "jq", true);
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
            _xMin = -790;
            _yMin = -270;
            _xMax = 700;
            _yMax = 405;

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

            // Debug.LogError("result：" + result + " - isXMin："+ isXMin + " - isXMax：" + isXMax + " - isYMin：" + isYMin + " - isYMax：" + isYMax);

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
            else if (dis > 40 && dis <= 60)
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
            else
                _dragDirection = DragDirection.stop;
        }

        //结束拖拽
        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _isDraging = false;
            _dragBall.DoReset();
            _dragDirection = DragDirection.stop;
            SpineManager.instance.DoAnimation(_qianTing, "jq", true);
            _moveSpeed = 0;
        }

        #endregion

        #region 游戏流程相关

        //更新得分
        void JudgeCount()
        {
            for (int i = 0; i < curTrans.Find("Count").childCount; i++)
            {
                curTrans.Find("Count").GetChild(i).gameObject.Hide();
            }
            curTrans.Find("Count").GetChild(_count).gameObject.Show();
        }

        //更新trigger范围
        void NewTriggerData()
        {
            if(_number < 4)
            {
                Rect rect = curTrans.Find("Trigger").GetChild(_number).GetComponent<RectTransform>().rect;
                Vector2 vector2 = curTrans.Find("Trigger").GetChild(_number).GetComponent<RectTransform>().anchoredPosition;
                _triggerXMin = vector2.x - rect.width / 2;
                _triggerXMax = vector2.x + rect.width / 2;
                _triggerYMin = vector2.y - rect.height / 2;
                _triggerYMax = vector2.y + rect.height / 2;
            }
            else
            {
                _triggerXMin = 5000;
                _triggerXMax = 5000;
                _triggerYMin = 5000;
                _triggerYMax = 5000;
            }
        }

        //如果进入鳄鱼的领域
        void EnterTrigger()
        {
            //处于Trigger领域中
            if(_player.transform.localPosition.x <= _triggerXMax && _player.transform.localPosition.x >= _triggerXMin && _player.transform.localPosition.y <= _triggerYMax && _player.transform.localPosition.y >= _triggerYMin)
            {
                _number += 1;
                if(_number <= 4)
                {
                    string aniName = "k" + _number.ToString();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    _canClickDiamond = true;
                    _chooseMask.Show();
                    SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("Choose"), aniName, false);
                    SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("HM"), "kong", false);
                    _dragBall.canMove = false;
                    _canMove = false;
                    NewTriggerData();
                }
            }
        }

        //宝石点击
        private void ClickEvent(GameObject obj)
        {
            if(_canClickDiamond)
            {
                _canClickDiamond = false;
                int index = 0;
                if (_number == 1)
                    index = _number + Convert.ToInt32(obj.name);
                if (_number == 2)
                    index = _number + 1 + Convert.ToInt32(obj.name);
                if (_number == 3)
                    index = _number + 2 + Convert.ToInt32(obj.name);
                if (_number == 4)
                    index = _number + 3 + Convert.ToInt32(obj.name);
                SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("Choose"), "an" + index + "-ani", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("Choose"), "kong", false);
                    JudgeClick(obj);
                });
            }
        }

        //判断点击是否正确
        void JudgeClick(GameObject o)
        {
            bool isSuccess = false;
            if(_number == 1)
            {
                if(o.name.Equals("0"))
                    isSuccess = true;
                else
                    isSuccess = false;
            }
            if (_number == 2)
            {
                if (o.name.Equals("1"))
                    isSuccess = true;
                else
                    isSuccess = false;
            }
            if (_number == 3)
            {
                if (o.name.Equals("0"))
                    isSuccess = true;
                else
                    isSuccess = false;
            }
            if (_number == 4)
            {
                if (o.name.Equals("1"))
                    isSuccess = true;
                else
                    isSuccess = false;
            }

            if (isSuccess)
            {
                _count += 1;
                //成功
                _eyArray[_number - 1].Hide();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                BtnPlaySoundSuccess();
                SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("HM"), "qp", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("HM"), "qp2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_chooseMask.transform.GetGameObject("HM"), "kong", false);
                        _chooseMask.Hide();
                        SpineManager.instance.DoAnimation(_hmArray[_number - 1], "hm" + _number.ToString(), true);
                        _dragBall.canMove = true;
                        _canMove = true;
                        JudgeCount();
                        FinalSuccess();
                    });
                });
            }
            else
            {
                _chooseMask.Hide();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                _eyArray[_number - 1].GetComponent<RawImage>().color = Color.grey;
                _dragBall.canMove = true;
                _canMove = true;
                FinalSuccess();
            }
        }

        //胜利界面
        void FinalSuccess()
        {
            if(_number >= 4)
            {
                _dragBall.canMove = false;
                _canMove = false;
                mono.StartCoroutine(WaitCoroutine(
                () => 
                {
                    playSuccessSpine(
                    () =>
                    {
                        anyBtns.gameObject.SetActive(true);
                        anyBtns.GetChild(0).gameObject.SetActive(true);
                        anyBtns.GetChild(1).gameObject.SetActive(true);
                        anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                        anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    });
                }, 2.0f));
            }
        }
        #endregion
    }
}
