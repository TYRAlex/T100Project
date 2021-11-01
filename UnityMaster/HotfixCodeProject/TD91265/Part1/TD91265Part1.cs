using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;
using Object = UnityEngine.Object;

namespace ILFramework.HotClass
{

    public enum RoleType
    {
        Bd,
        Xem,
        Child,
        Adult,
    }

    public class TD91265Part1
    {

        private MonoBehaviour _mono;


        private GameObject _mask;         //黑色mask
        private GameObject _replaySpine;   //重玩Spine
        private GameObject _startSpine;    //开始Spine
        private GameObject _okSpine;       //OkSpine
        private GameObject _successSpine;  //成功Spine
        private GameObject _spSpine;       //彩带Spine
        private GameObject _dTT;           //中间田田Spine
        private GameObject _sTT;           //左边田田Spine
        private GameObject _g1;            //开场光Spine
        private GameObject _countDown;     //倒计时Spine
        private GameObject _xem;           //右下角小恶魔Spine
        private GameObject _xiangzi;       //右下角箱子Spine   
        private GameObject _progress;      //进度条
        private GameObject _move;          //移动父物体
        private GameObject _daxiangTou;    //大象头部Spine   
        private GameObject _daxiangShenTi; //大象身体Spine
        private GameObject _daxiang;       //大象整体
        private GameObject _box;           //方块
        private GameObject _boom;          //炸弹

        private Transform _daxiangTra;       //大象整个Tra
        private Transform _daxiangsIconTra;  //大象左上角Icon父物体
        private Transform _xemsIconTra;      //小恶魔右上角Icon父物体
        private Transform _spinesTra;      //Spines父物体
        private Transform _boxCreateTra;   //箱子生成的父物体


        private Image _maskImg;            //maskImg;
        private Image _barImg;            //进度条Img;

        private int _xemsIconNum;          //小恶魔头像数量 初始值为2
        private int _daxiangIconNum;       //大象头像数量   初始值为2

        private CanvasGroup _leftCanvas;    //左移CanvasGroup   用来控制按钮透明度
        private CanvasGroup _rightCanvas;  //右移CanvasGroup   用来控制按钮透明度

        private RectTransform _leftRect;    //左移RectTransform  用来控制按钮缩放
        private RectTransform _rightRect;   //右移RectTransform   用来控制按钮缩放
       
        private RectTransform _daxiangTouRect; //大象头部RectTransform  用来控制大象头部旋转
        private RectTransform _daxiangQiu;    //大象脚下面的球  用来控制球旋转
        

        private LongPressButton _leftLongBtn;  //左长按
        private LongPressButton _rigthLongBtn; //右长按

        private bool _isDown;                //是否按下
        private bool _isLeft;                //是否左边移动

        private Coroutine _createCor;       //生成的协程

        private Rigidbody2D _daxiangR2d;     //大象整个R2d
        private Rigidbody2D _daxiangtouR2d;  //大象头R2d

        private GameObject _zlyc;   //再来一次动画

        private float _moveSpeed;
        private int _getCount;  //接到后同类方块相消的次数，初始为0

       
        void Start(object o)
        {
            var curGo = (GameObject)o;
            _mono = curGo.GetComponent<MonoBehaviour>();
            var curTrans = curGo.transform;
          
            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");
            _dTT = curTrans.GetGameObject("dTT");
            _sTT = curTrans.GetGameObject("sTT");
            _g1 = curTrans.GetGameObject("Spines/guang/g1");
            _countDown = curTrans.GetGameObject("countDown/3");
            _xem = curTrans.GetGameObject("Spines/xem/xc1");
            _xiangzi = curTrans.GetGameObject("Spines/xem/qt1");
            _progress = curTrans.GetGameObject("Spines/xem/progress");
            _move = curTrans.GetGameObject("Spines/move");
            _daxiang = curTrans.GetGameObject("Spines/daxiang");
            _daxiangTou = curTrans.GetGameObject("Spines/daxiang/dx/dx1");
            _daxiangShenTi = curTrans.GetGameObject("Spines/daxiang/dx0");
            _box = curTrans.GetGameObject("Spines/boxs/box");
            _boom = curTrans.GetGameObject("Spines/boxs/boom");

            _daxiangTra = curTrans.Find("Spines/daxiang");
            _daxiangsIconTra = curTrans.Find("Spines/uis/daxiangs");
            _xemsIconTra = curTrans.Find("Spines/uis/xems");
            _spinesTra = curTrans.Find("Spines");
            _boxCreateTra = curTrans.Find("Spines/boxCreate");

            _maskImg = curTrans.GetImage("mask");
            _barImg = curTrans.GetImage("Spines/xem/progress/bar");

            _leftCanvas = curTrans.Find("Spines/move/Left").GetComponent<CanvasGroup>();
            _rightCanvas = curTrans.Find("Spines/move/Right").GetComponent<CanvasGroup>();

            _leftRect = curTrans.GetRectTransform("Spines/move/Left");
            _rightRect = curTrans.GetRectTransform("Spines/move/Right");
           
            _daxiangTouRect = curTrans.GetRectTransform("Spines/daxiang/dx");
            _daxiangQiu = curTrans.GetRectTransform("Spines/daxiang/qiu1");
    
            _zlyc = curTrans.GetGameObject("Spines/zlyc");

            _leftLongBtn = curTrans.Find("Spines/move/Left").GetComponent<LongPressButton>();
            _rigthLongBtn = curTrans.Find("Spines/move/Right").GetComponent<LongPressButton>();

            _daxiangR2d = curTrans.Find("Spines/daxiang").GetComponent<Rigidbody2D>();
            _daxiangtouR2d = curTrans.Find("Spines/daxiang/dx").GetComponent<Rigidbody2D>();

            GameInit();
            GameStartPanel();
        }


        /// <summary>
        /// 游戏初始化
        /// </summary>
        void GameInit()
        {
            _xemsIconNum = _daxiangIconNum = 2;
            _barImg.fillAmount = 0;

            _leftCanvas.alpha = 0.75f; _leftRect.localScale = Vector3.one;
            _rightCanvas.alpha = 0.75f; _rightRect.localScale = Vector3.one;

            _daxiangTra.localPosition = new Vector2(0, -225);
            _daxiangQiu.localEulerAngles = Vector3.zero;
            _daxiangTouRect.localEulerAngles = Vector3.zero;
            _daxiangTouRect.anchoredPosition = new Vector2(-40, -43);
         
            _moveSpeed =  12000f;
            _getCount = 0;

            _isLeft = false;
            _isDown = false;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            HideVoiceBtn();
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _dTT.Hide(); _sTT.Hide(); _progress.Hide();_move.Hide(); _daxiang.Hide();

            //游戏开始时清除所有子物体
            for (int i = 0; i < _boxCreateTra.childCount; i++)
                GameObject.Destroy(_boxCreateTra.GetChild(i).gameObject);
            _zlyc.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine); RemoveLongBtnEvent(_leftLongBtn);RemoveLongBtnEvent(_rigthLongBtn);

            AddLongBtnEvent(_leftLongBtn, LeftOnDown, LeftOnUp); AddLongBtnEvent(_rigthLongBtn, RightOnDown, RightOnUp);


            _maskImg.color = new Color(0, 0, 0, 200 / 255f);

            InitSpines(_spinesTra,spine=> {

                var name = spine.name;

                switch (name)
                {
                    case"xc1":
                        spine.timeScale = 1;
                        break;
                }
            });

            SpineInitialize(_countDown);

            SetBellSprites(_spinesTra, bellSprites => {

                var img = bellSprites.transform.GetImage();
                var sprites = bellSprites.sprites;
                img.sprite = sprites[0];
                bellSprites.gameObject.Hide();
                img.color = new Color(1, 1, 1, 1);
            });
     
        }

        #region 游戏逻辑


        /// <summary>
        /// 游戏开始界面
        /// </summary>
        void GameStartPanel()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayBgm(0);
                        _startSpine.Hide();
                        _sTT.Show();
                        Speck(_sTT, 0, null, StartGame);
                    });
                });
            });
        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccessPanel()
        {
            _maskImg.color = new Color(0, 0, 0, 200 / 255f);
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOkPanel);
        }

        /// <summary>
        /// 游戏失败界面
        /// </summary>
        private void GameFailPanel()
        {
            _zlyc.Show();
            PlaySpine(_zlyc, _zlyc.name, 
            ()=>
            {
                Delay(2.0f, 
                () => 
                {
                    GameInit();
                    StartGame();
                });
            }, false);
        }

        /// <summary>
        /// 重玩和Ok界面
        /// </summary>
        private void GameReplayAndOkPanel()
        {
            _mask.Show();
            _replaySpine.Show();
            _okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () =>
            {
                AddEvent(_replaySpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();

                        GameInit();
                        StartGame();
                        PlayBgm(0);
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dTT.Show();
                        Speck(_dTT, 1);
                    });
                });
            });

        }



        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _sTT.Hide();
            _mask.Show();
            CountDownAni(_countDown, new string[5] { "3", "2", "1", "4", "5" }, () => {

                _maskImg.DOColor(new Color(0, 0, 0, 0), 1f).OnComplete(() => 
                { 
                    _mask.Hide();
                    _createCor = CreateBox(_box);
                });

                PlaySpine(_g1, "g1", () => { 
                    PlaySpine(_g1, "g2", null, true);
                    SetBellSprites(_spinesTra,bellSprites=> { bellSprites.gameObject.Show(); });
                    _progress.Show();_move.Show(); _daxiang.Show();
                    PlaySpine(_xem, "xc1", null, true);
                    PlaySpine(_xiangzi, "qt1");
                    PlaySpine(_daxiangTou,"dx1",null,true);
                    PlaySpine(_daxiangShenTi, "dx0", null, true);
                    _leftCanvas.blocksRaycasts = true;
                    _rightCanvas.blocksRaycasts = true;
                });
            });

        }

        /// <summary>
        /// 小恶魔被拳头打和右上角小恶魔爆炸动画
        /// </summary>
        private void XemAni()
        {
            PlayVoice(3);
            PlaySpine(_xiangzi, "qt2");
            _xem.GetComponent<SkeletonGraphic>().timeScale = 4;

            Delay(0.5f, () => {
                _barImg.fillAmount =0;
                IocnBoomAni(true);
                _mono.StartCoroutine(IEXemAni());
            });        
        }

        IEnumerator IEXemAni()
        {
            while (_xem.GetComponent<SkeletonGraphic>().timeScale>=1)
            {
                yield return new WaitForSeconds(0.02f);
                _xem.GetComponent<SkeletonGraphic>().timeScale -= 0.02f;
            }          
            _xem.GetComponent<SkeletonGraphic>().timeScale = 1;
        }
   
        /// <summary>
        /// 倒计时动画
        /// </summary>
        private void CountDownAni(GameObject go, string[] spinesName, Action callBack)
        {
            _mono.StartCoroutine(IECountDownAni(go, spinesName, callBack));
        }

        private IEnumerator IECountDownAni(GameObject go, string[] spinesName, Action callBack)
        {
            for (int i = 0; i < spinesName.Length; i++)
            {
                bool isGo = i == spinesName.Length - 1;
                var index = isGo ? 1 : 0;
                PlayVoice(index);

                string name = spinesName[i];
                PlaySpine(go, name);
                yield return new WaitForSeconds(1);
            }

            callBack?.Invoke();
        }

        /// <summary>
        /// Icon爆炸动画
        /// </summary>
        /// <param name="isXem"></param>
        private void IocnBoomAni(bool isXem)
        {
            var parent = isXem ? _xemsIconTra:_daxiangsIconTra;
            var index = isXem ? _xemsIconNum : _daxiangIconNum;
            Transform curChild = parent.GetChild(index);

            PlayVoice(4);
            if (isXem)                         
                _xemsIconNum--;            
            else                       
                _daxiangIconNum--;
                            
            if (curChild!=null)
            {
                var icon = curChild.Find("icon");
                var img = icon.GetImage();
                var bellSpirtes = icon.GetComponent<BellSprites>();
                var boom = curChild.Find("sc-boom").gameObject;

                PlaySpine(boom, "sc-boom",()=> { img.DOColor(new Color(1,1,1,0),0.3f); });
                if (_xemsIconNum < 0)
                    Delay(2.0f, () => { GameSuccessPanel(); });
                else if (_daxiangIconNum < 0)
                    GameFailPanel();
                else
                    EndPause();

                Delay(0.2f, 
                () => 
                {
                    img.sprite = bellSpirtes.sprites[1];
                });             
            }
        }

        /// <summary>
        /// 左边按下
        /// </summary>
        private void LeftOnDown()
        {
            OnDown(true);
            
        }

        /// <summary>
        /// 左边抬起
        /// </summary>
        private void LeftOnUp()
        {
            OnUp(true);
          
        }

        /// <summary>
        /// 右边按下
        /// </summary>
        private void RightOnDown()
        {
            OnDown(false);        
        }

        /// <summary>
        /// 右边抬起
        /// </summary>
        private void RightOnUp()
        {
            OnUp(false);
           
        }
     
        private void OnDown(bool isLeft,float delay=0.3f)
        {
            var rect = isLeft ? _leftRect : _rightRect;
            var cavas = isLeft ? _leftCanvas : _rightCanvas;
            _isLeft = isLeft;
            if (!_isDown)
            {
                PlayVoice(2);
                rect.DOScale(1.5f, delay);
                cavas.DOFade(1, delay);
                _isDown = true;
            }     
        }

        private void OnUp(bool isLeft, float delay = 0.3f)
        {
            var rect = isLeft ? _leftRect : _rightRect;
            var cavas = isLeft ? _leftCanvas : _rightCanvas;

            rect.DOScale(1, delay);
            cavas.DOFade(0.75f, delay);
            
            Delay(delay, () => {
                _isDown = false;              
            });
        }
      
        void FixedUpdate()
        {

            _daxiangTouRect.anchoredPosition = new Vector2(_daxiangTouRect.anchoredPosition.x, -43);

            if (_isDown)
            {
                if (_isLeft)
                {

                    if (_daxiangTra.localPosition.x>-550)
                    {
                        _daxiangR2d.velocity = Vector2.left * _moveSpeed * Time.deltaTime;
                        _daxiangtouR2d.velocity = Vector2.left * _moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _daxiangR2d.velocity = Vector2.zero;
                        _daxiangtouR2d.velocity = Vector2.zero;
                    }
                   
                     _daxiangQiu.localEulerAngles = new Vector3(0, 0, _daxiangQiu.localEulerAngles.z + 2);

                }
                else
                {

                    if (_daxiangTra.localPosition.x<550)
                    {
                        _daxiangR2d.velocity = Vector2.right * _moveSpeed * Time.deltaTime;
                        _daxiangtouR2d.velocity = Vector2.right * _moveSpeed * Time.deltaTime;
                    }
                    else
                    {
                        _daxiangR2d.velocity = Vector2.zero;
                        _daxiangtouR2d.velocity = Vector2.zero;
                    }
                   
                    _daxiangQiu.localEulerAngles = new Vector3(0, 0, _daxiangQiu.localEulerAngles.z - 2);
                }
            }
            else
            {
                _daxiangR2d.velocity = Vector2.zero;
                _daxiangtouR2d.velocity = Vector2.zero;
            }       
        }

        /// <summary>
        /// 生成盒子
        /// </summary>
        /// <param name="go"></param>
        /// <returns></returns>
        private Coroutine CreateBox(GameObject go)
        {
            return _mono.StartCoroutine(IECreateBox(go));
        }

        private IEnumerator IECreateBox(GameObject go)
        {
            while (true)
            {
                yield return new WaitForSeconds(2f);

                GameObject newGo = null;
  
                int isBoom = Random.Range(0, 8);
                if(isBoom == 0)
                {
                    newGo = Object.Instantiate(_boom, _boxCreateTra, false);
                    if (newGo.GetComponent<EventDispatcher>() == null)
                    {
                        newGo.AddComponent<EventDispatcher>();
                    }

                    var eD = newGo.GetComponent<EventDispatcher>();

                    eD.CollisionEnter2D += OnBoom;
                   
                    PlaySpine(newGo, "zd", null, true);
                }
                else
                {
                    newGo = Object.Instantiate(go, _boxCreateTra, false);
                    int random = Random.Range(0, 5);
                    newGo.transform.GetComponent<Image>().sprite = newGo.transform.GetComponent<BellSprites>().sprites[random];
                    if (newGo.GetComponent<EventDispatcher>() == null)
                    {
                        newGo.AddComponent<EventDispatcher>();
                    }

                    var eD = newGo.GetComponent<EventDispatcher>();

                    eD.CollisionEnter2D += OnWall;
                    var bellSprite = newGo.GetComponent<BellSprites>();
                }

                newGo.Show();
                newGo.transform.GetChild(0).gameObject.Hide();

                var tra = newGo.transform;
                var x = Random.Range(-450, 450);
                tra.localPosition = new Vector2(x, 0);
            }
        }

        private void OnWall(Collision2D c, int time)
        {
           var curBox =  c.otherCollider.gameObject;
           var name =  c.gameObject.name;

            if (name=="taiz")
            {
                Delay(0.5f, () => {
                    Component.Destroy(curBox.transform.GetComponent<EventDispatcher>());                  
                    curBox.transform.GetComponent<Image>().DOColor(new Color(1, 1, 1, 0), 0.5f).OnComplete(() => { GameObject.Destroy(curBox.gameObject); });
                });
                
            }
            if (name == "dx")
            {
                Rigidbody2D rigidbody2D = curBox.GetComponent<Rigidbody2D>();
                rigidbody2D.gravityScale = 100;
            }
            if (name == "zone")
            {
                //超出边界直接删除
                GameObject.Destroy(curBox.gameObject);
            }
            if (c.gameObject.GetComponent<Image>())
            {
                if (curBox.GetComponent<Image>().sprite.texture.name == c.gameObject.GetComponent<Image>().sprite.texture.name)
                {
                    PlayVoice(5);
                    Pause();
                    PlaySpine(_daxiangTou, "dx2", null, true);
                    GameObject anotherBox = c.gameObject;
                   
                    Component.Destroy(curBox.transform.GetComponent<EventDispatcher>());
                    Component.Destroy(anotherBox.transform.GetComponent<EventDispatcher>());
                    curBox.transform.GetChild(0).gameObject.Show();
                    Delay(0.5f, 
                    () => 
                    {
                        curBox.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                        anotherBox.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                    });
                    PlaySpine(curBox.transform.GetChild(0).gameObject, "star", 
                    () => 
                    {
                        _barImg.fillAmount += 0.5f;
                        GameObject.Destroy(curBox.gameObject);
                        GameObject.Destroy(anotherBox.gameObject);
                        if (++_getCount >= 2)
                        {                          
                            _getCount = 0;
                            XemAni();
                        }
                        else
                            EndPause();
                    }, false);
                }
            }
        }

        private void OnBoom(Collision2D c, int time)
        {
            var curBoom = c.otherCollider.gameObject;
            var name = c.gameObject.name;

            if (name == "taiz")
            {
                PlayVoice(6);
                Component.Destroy(curBoom.transform.GetComponent<EventDispatcher>());
                curBoom.transform.GetChild(0).gameObject.Show();
               
                curBoom.transform.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                PlaySpine(curBoom.transform.GetChild(0).gameObject, "sc-boom3", ()=> { GameObject.Destroy(curBoom.gameObject); }, false);
            }
            if (name == "dx")
            {
                PlayVoice(6);
                Pause();
                Component.Destroy(curBoom.transform.GetComponent<EventDispatcher>());
                curBoom.transform.GetChild(0).gameObject.Show();
                curBoom.transform.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                PlaySpine(_daxiangTou, "dx3", null, true);
                PlaySpine(curBoom.transform.GetChild(0).gameObject, "sc-boom3", 
                () => 
                { 
                    GameObject.Destroy(curBoom.gameObject);
                    IocnBoomAni(false);
                }, false);
            }
            if (name == "zone")
            {
                //超出边界直接删除
                GameObject.Destroy(curBoom.gameObject);
            }
            if (name == "box(Clone)")
            {
                PlayVoice(6);
                GameObject another = c.gameObject;
                Component.Destroy(curBoom.transform.GetComponent<EventDispatcher>());
                curBoom.transform.GetChild(0).gameObject.Show();
                curBoom.transform.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                another.transform.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                PlaySpine(curBoom.transform.GetChild(0).gameObject, "sc-boom3", 
                () => 
                { 
                    GameObject.Destroy(curBoom.gameObject);
                    GameObject.Destroy(another.gameObject);
                }, false);
            }
        }

        /// <summary>
        /// 触发效果反馈时物体停止掉落与生成
        /// </summary>
        void Pause()
        {
            StopCoroutines(_createCor);
            _createCor = null;
            for (int i = 0; i < _boxCreateTra.childCount; i++)
                _boxCreateTra.GetChild(i).GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;

            _leftCanvas.blocksRaycasts = false;
            _rightCanvas.blocksRaycasts = false;
            _isDown = false;
        }

        /// <summary>
        /// 结束暂停
        /// </summary>
        void EndPause()
        {
            _createCor = CreateBox(_box);
            for (int i = 0; i < _boxCreateTra.childCount; i++)
                _boxCreateTra.GetChild(i).GetComponent<Rigidbody2D>().constraints = ~RigidbodyConstraints2D.FreezeAll;
            _leftCanvas.blocksRaycasts = true;
            _rightCanvas.blocksRaycasts = true;
            PlaySpine(_daxiangTou, "dx1", null, true);
        }

        #endregion

        #region 常用函数

        private T[] Gets<T>(Transform parent)
        {
            return parent.GetComponentsInChildren<T>(true);
        }

        #region BellSprites相关

        private void SetBellSprites(Transform parent,Action<BellSprites> callBack)
        {
            var sprites = Gets<BellSprites>(parent);
            for (int i = 0; i < sprites.Length; i++)
                callBack?.Invoke(sprites[i]);
        }

        #endregion


        #region 语音按钮

        /// <summary>
        /// Show语音健
        /// </summary>
        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        /// <summary>
        /// Hide语音健
        /// </summary>
        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        /// <summary>
        /// 显示All子物体
        /// </summary>
        /// <param name="parent"></param>
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        /// <summary>
        /// 隐藏All子物体
        /// </summary>
        /// <param name="parent">父物体Tra</param>
        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        /// <summary>
        /// Show子物体
        /// </summary>
        /// <param name="parent">父物体Tra</param>
        /// <param name="name">子物体Name</param>
        /// <param name="callBack">完成回调</param>
        private void ShowChild(Transform parent, string name, Action<GameObject> callBack = null)
        {
            var go = parent.Find(name).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }

        /// <summary>
        /// Hide子物体
        /// </summary>
        /// <param name="parent">父物体Tra</param>
        /// <param name="name">子物体Name</param>
        /// <param name="callBack">完成回调</param>
        private void HideChild(Transform parent, string name, Action<GameObject> callBack = null)
        {
            var go = parent.Find(name).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        #endregion

        #region Spine相关


        /// <summary>
        /// Spine Initialize
        /// </summary>
        /// <param name="go"></param>
        private void SpineInitialize(GameObject go)
        {
            var spine = go.GetComponent<SkeletonGraphic>();
            spine.Initialize(true);
        }

        /// <summary>
        /// 初始化Spines
        /// </summary>
        /// <param name="parent">Spines父物体</param>
        /// <param name="callBack">完成回调</param>
        private void InitSpines(Transform parent,Action<SkeletonGraphic> callBack=null)
        {
            var spines = Gets<SkeletonGraphic>(parent);
            for (int i = 0; i < spines.Length; i++)
            {
                var spine = spines[i];
                spine.Initialize(true);
                callBack?.Invoke(spine);
            }
        }

        /// <summary>
        /// 播放Spine
        /// </summary>
        /// <param name="go">Spine对象GameObject</param>
        /// <param name="name">Spine名</param>
        /// <param name="callBack">完成Spine回调</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns>返回Spine耗时</returns>
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        #endregion

        #region 播放音频

        /// <summary>
        /// 播放OnClick音效
        /// </summary>
        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        /// <summary>
        /// 播放Bgm
        /// </summary>
        /// <param name="index">Bgm索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        /// <summary>
        /// 播放Voice
        /// </summary>
        /// <param name="index">Voice索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        /// <summary>
        /// 播放Sound
        /// </summary>
        /// <param name="index">Sound索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }


        /// <summary>
        /// 播放CommonBgm
        /// </summary>
        /// <param name="index">CommonBgm索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }


        /// <summary>
        /// 播放CommonVoice
        /// </summary>
        /// <param name="index">CommonVoice索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        /// <summary>
        /// 播放CommonSound
        /// </summary>
        /// <param name="index">CommonSound索引</param>
        /// <param name="isLoop">是否Loop</param>
        /// <returns></returns>
        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        #endregion

        #region 停止音频

        /// <summary>
        /// 停止All音频
        /// </summary>
        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        /// <summary>
        /// 停止音频
        /// </summary>
        /// <param name="type">音频类型</param>
        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        #endregion

        #region 延时相关

        /// <summary>
        /// 延时
        /// </summary>
        /// <param name="delay">时间</param>
        /// <param name="callBack">回调</param>
        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }


        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
     
        #endregion

        #region 停止协程

        /// <summary>
        /// 停止All协程
        /// </summary>
        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        /// <summary>
        /// 停止协程
        /// </summary>
        /// <param name="routine">协程Coroutine</param>
        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        /// <summary>
        /// 人物讲话
        /// </summary>
        /// <param name="go">讲话对象GameObject</param>
        /// <param name="index">讲话index</param>
        /// <param name="specking">讲话中回调</param>
        /// <param name="speckend">讲话结束回调</param>
        /// <param name="roleType">角色类型</param>
        /// <param name="type">音频类型</param>
        private void Speck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Adult, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
            {
                case RoleType.Bd:
                    daiJi = "bd-daiji"; speak = "bd-speak";
                    break;
                case RoleType.Xem:
                    daiJi = "daiji"; speak = "speak";
                    break;
                case RoleType.Child:
                    daiJi = "animation"; speak = "animation2";
                    break;
                case RoleType.Adult:
                    daiJi = "daiji"; speak = "speak";
                    break;
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, daiJi);
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, speak);

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, daiJi);
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 监听相关

        /// <summary>
        /// Add事件
        /// </summary>
        /// <param name="go"></param>
        /// <param name="callBack">事件回调</param>
        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }
        
        /// <summary>
        /// Add长按事件
        /// </summary>
        /// <param name="longPressButton">长按组件</param>
        /// <param name="onDown">按下</param>
        /// <param name="onUp">抬起</param>
        private void AddLongBtnEvent(LongPressButton longPressButton,Action onDown,Action onUp)
        {
            longPressButton.OnDown = onDown;
            longPressButton.OnUp = onUp;
        }

        /// <summary>
        /// Remove事件
        /// </summary>
        /// <param name="go">事件对象GameObject</param>
        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }

        /// <summary>
        /// Remove长按事件
        /// </summary>
        private void RemoveLongBtnEvent(LongPressButton longPressButton)
        {         
            longPressButton.OnDown = null;
            longPressButton.OnUp = null;
        }

        #endregion

        #endregion

    }
}
