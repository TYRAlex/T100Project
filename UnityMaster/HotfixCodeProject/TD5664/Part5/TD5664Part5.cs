using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
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

    public class TD5664Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;


        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;


        private GameObject _dDD;

        private GameObject _sDD;


        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform gunPos;
        private Transform railgun;



        private GameObject xem;
        private GameObject niao;
        private GameObject niao2;
        private GameObject jdt;
        private GameObject beach;
        private GameObject go;
        private GameObject gun;
        private GameObject again;
        private GameObject mask2;


        private RectTransform Bg1;
        private RectTransform Bg2;
        private RectTransform Fz1;
        private RectTransform Fz2;
        private RectTransform yun;
        private RectTransform yun2;

        private RectTransform far;
        private RectTransform far2;

        private RectTransform middle;
        private RectTransform middle2;

        private Rigidbody2D rb;

        
        private Image mask2_image;


        Coroutine shoot1;








        private bool canAgain;


        private bool _isPlaying;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");

            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");


            _dDD = curTrans.GetGameObject("dDD");



            _sDD = curTrans.GetGameObject("sDD");

            gunPos = curTrans.Find("xem/gunPos");
            railgun = curTrans.Find("railgun");



            xem = curTrans.GetGameObject("xem");
            gun = curTrans.GetGameObject("xem/gun");
            niao = curTrans.GetGameObject("niao");
            niao2 = curTrans.GetGameObject("niao2");
            jdt = curTrans.GetGameObject("nengliangtiao");
            beach = curTrans.GetGameObject("beach");
            go = curTrans.GetGameObject("go");
            again = curTrans.GetGameObject("zailaiyici");
            mask2 = curTrans.GetGameObject("mask2");

            Bg1 = curTrans.GetGameObject("BG").GetComponent<RectTransform>();
            Bg2 = curTrans.GetGameObject("BG2").GetComponent<RectTransform>();

            Fz1 = curTrans.GetGameObject("Fz1").GetComponent<RectTransform>();
            Fz2 = curTrans.GetGameObject("Fz2").GetComponent<RectTransform>();

            far = curTrans.GetGameObject("far").GetComponent<RectTransform>();
            far2 = curTrans.GetGameObject("far2").GetComponent<RectTransform>();

            middle = curTrans.GetGameObject("middle").GetComponent<RectTransform>();
            middle2 = curTrans.GetGameObject("middle2").GetComponent<RectTransform>();

            yun = curTrans.GetGameObject("yun").GetComponent<RectTransform>();
            yun2 = curTrans.GetGameObject("yun2").GetComponent<RectTransform>();



            rb = niao.GetComponent<Rigidbody2D>();


            Util.AddBtnClick(again.transform.GetChild(0).gameObject,ClickAgain);




            Bg1.anchoredPosition = new Vector2(0, 0);
            Bg2.anchoredPosition = new Vector2(1920, 0);

            Fz1.anchoredPosition = new Vector2(0, -125);
            Fz2.anchoredPosition = new Vector2(1920, -665);


            GameInit();
            GameStart();
        }
        float gravity;
        void InitData()
        {
            _isPlaying = true;
            _canCollission = true;
            speed = 200;
            gunSpeed = 250;
            _startXPos = 1920;
            _endXPos = -1920;
            upForce = 20000;
            gravity = 50;

            canDown = false;
            canUp = true;

        


            niao.transform.GetRectTransform().anchoredPosition = new Vector2(175, -332);
            xem.transform.GetRectTransform().anchoredPosition = new Vector2(-100, -332);
            niao.GetComponent<PolygonCollider2D>().enabled = true;
            niao.GetComponent<EventDispatcher>().CollisionEnter2D += OnCollisionEnter2D;
            isPass = true;
            canAgain = false;
            _canInput = true;
            jdt.transform.GetComponent<SkeletonGraphic>().freeze = false;
            InitSpine(jdt.transform, jdt.name + 3);
            /*jdt.transform.GetComponent<SkeletonGraphic>().freeze = false;
            InitSpine(jdt.transform, jdt.name + 4);     
            jdt.transform.GetComponent<SkeletonGraphic>().freeze = true;*/
            InitSpine(xem.transform, "fei");
            InitSpine(niao, "niao",true);
            InitSpine(niao2.transform, "");
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            niao.transform.GetRectTransform().rotation = Quaternion.Euler(0, 0, 0);
        
            InitSpine(beach.transform, "");
            InitSpine(go.transform, "");
            InitSpine(again.transform, "");
            gun.transform.DOMove(gunPos.position, 0);
            gun.SetActive(false);
            for (int i = 0; i < railgun.childCount; i++)
            {
                Object.Destroy(railgun.transform.GetChild(i).gameObject);
            }

            rb.gravityScale = 0;

            mask2_image = mask2.GetComponent<Image>();

            mask2_image.color = new Color(255,255,255,0);



           _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        void GameInit()
        {
            DOTween.KillAll();
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();



            _dDD.Hide();



            _sDD.Hide();







            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

        }


        
        void GameStart()
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
                        _sDD.Show();
                        BellSpeck(_sDD, 0,null,ShowVoiceBtn);
                       
                        

                    });
                });
            });
        }


        void TalkClick()
        {
            HideVoiceBtn();
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide();_mask.Hide();StartGame(); });
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        //Spine初始化
        void InitSpine(Transform _tra, string animation)
        {
            SkeletonGraphic _ske = _tra.GetComponent<SkeletonGraphic>();
            _ske.startingAnimation = animation;
         
            _ske.Initialize(true);
        }
        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }

        #region 背景移动
        #region 背景循环
        bool isPass;
        int _startXPos, _endXPos;
        void LeftMove(RectTransform rect, int startXPos, int endXPos, float speed)
        {
            if (Convert.ToInt32( rect.anchoredPosition.x )<= endXPos)
                rect.anchoredPosition = new Vector2(startXPos, Convert.ToInt32( rect.anchoredPosition.y));
            rect.Translate(Vector2.left * (speed*2*(Screen.width/1920f)));
        }
        #endregion
        bool _canInput;

        void Update()
        {
            if (!isPass)
            {
                XemMove();
                NiaoMove();
            }
            if (niao.transform.GetRectTransform().anchoredPosition.y > -130)
            {
                niao.transform.GetRectTransform().anchoredPosition = new Vector2(niao.transform.GetRectTransform().anchoredPosition.x, -130);
                rb.velocity = Vector2.zero;
            }
        }

        void FixedUpdate()
        {

            if (!isPass)
            {
                LeftMove(Bg1, _startXPos, _endXPos, 1.5f);
                LeftMove(Bg2, _startXPos, _endXPos, 1.5f);

                LeftMove(far, _startXPos, _endXPos, 0.9f);
                LeftMove(far2, _startXPos, _endXPos, 0.9f);

                LeftMove(middle, _startXPos, _endXPos, 1.5f);
                LeftMove(middle2, _startXPos, _endXPos, 1.5f);

                LeftMove(Fz1, _startXPos, _endXPos, 4f);
                LeftMove(Fz2, _startXPos, _endXPos, 4f);
                LeftMove(yun, _startXPos, _endXPos, 0.6f);
                LeftMove(yun2, _startXPos, _endXPos, 0.6f);
               

            }
         

        }


    

        #endregion

        #region 小鸟移动
        float upForce;
        void NiaoMove()
        {
            if (Input.GetMouseButtonDown(0) && (!isPass)&&_canInput)
            {
                rb.velocity = Vector2.zero;
                rb.AddForce(Vector2.up * upForce);

            }
        }
        #endregion

        #region 小鸟被抓
        bool _canCollission;
        void OnCollisionEnter2D(Collision2D c, int time)
        {
            if (!_canCollission)
                return;
            _canInput = false;
            _mono.StopCoroutine(shoot1);
            isPass = true;
            jdt.transform.GetComponent<SkeletonGraphic>().freeze = true;
            _canCollission = false;
            InitSpine(xem, "xiao", true);

            if (c.gameObject.name == "gun")
            {
                c.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(niao, "niao3", false);  //被抓住

                PlayVoice(7);
            }          
            else
            {
                SpineManager.instance.DoAnimation(niao, "niao2", false);  //被撞倒
                PlayVoice(5);
            }
            Delay(0.3f,()=> {
                niao.GetComponent<PolygonCollider2D>().enabled = false;
            });
            Delay(1, () => { PlayVoice(2); OnceAgain(); });

        }

        #endregion

        #region 再来一次
        void OnceAgain()
        {
            
        

            again.GetComponent<SkeletonGraphic>().Initialize(true);
           
          
            SpineManager.instance.DoAnimation(again, again.name, false, () =>
            {
                SpineManager.instance.DoAnimation(again, again.name + 2, true);
                canAgain = true;

            });

        }

        void ClickAgain(GameObject obj) {
            if (!canAgain)
                return;
            canAgain = false;
            PlayOnClickSound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.transform.parent.gameObject.name + 3, false, () => {

                GameInit();
                PlayBgm(0);
                StartGame();
            });
        }

        #endregion

        #region 恶魔移动
        float speed;
        bool canDown;
        bool canUp;
        void XemMove()
        {

            if (-124 - xem.transform.GetRectTransform().anchoredPosition.y > 0.5f && canUp)
            {
                xem.transform.Translate(Vector2.up * speed * Time.deltaTime);
            }
            else
            {
                canUp = false;
                canDown = true;
                if (canDown)
                    xem.transform.Translate(Vector2.down * speed * Time.deltaTime);
                if (xem.transform.GetRectTransform().anchoredPosition.y - (-503) < 0.5f)
                {
                    canDown = false;
                    canUp = true;
                }


            }
        }
        #endregion

        #region 恶魔发射
        float gunSpeed;
        bool canShoot;
        IEnumerator Shoot()
        {
            while (!isPass)
            {
                
               
                yield return new WaitForSeconds(5f);
                if (isPass)
                    break;
                SpineManager.instance.DoAnimation(xem, "miao", false, () =>
                {
                    GameObject obj = Object.Instantiate(gun);
                    obj.name = "gun";
                    obj.transform.SetParent(railgun);
                    obj.transform.DOMove(gunPos.position, 0);

                    obj.SetActive(true);

                    obj.transform.GetRectTransform().DOAnchorPosX(1111, 5).OnComplete(() => { Object.Destroy(obj); });
                    PlayVoice(3);
                    SpineManager.instance.DoAnimation(xem, "she", false, () =>
                    {
                        SpineManager.instance.DoAnimation(xem, "fei", true);
                    });
                });






            }
            yield return null;
        }

        #endregion

        #region 游戏开场
        void StartVideo()
        {

            SpineManager.instance.DoAnimation(niao, "niao", true);
            SpineManager.instance.DoAnimation(xem, "fei", true);
   
            xem.transform.GetRectTransform().DOAnchorPos(new Vector2(173, -332), 4f);
            niao.transform.GetRectTransform().DOAnchorPos(new Vector2(960, -332), 3f).OnComplete(() =>
            {
                _mono.StartCoroutine(CountDownVoice());
                SpineManager.instance.DoAnimation(go, "go", false, () =>
                {
                    isPass = false;            
                    shoot1 = _mono.StartCoroutine(Shoot());
    
                    SpineManager.instance.DoAnimation(jdt, jdt.name + 4, false, () => {
                        _canInput = false;
                        niao.GetComponent<PolygonCollider2D>().enabled = false;
                        SpineManager.instance.DoAnimation(jdt, jdt.name, false, () => {
                            EndVideo();
                        });
                    });
                    canShoot = true;
                    shoot1 = _mono.StartCoroutine(Shoot());
                    rb.gravityScale = gravity;
                });
            });


        }

        IEnumerator CountDownVoice()
        {
            for (int i = 0; i < 4; i++)
            {            
                if (i==3)                
                    PlayVoice(0);                
                else               
                    PlayVoice(1);
                yield return new WaitForSeconds(1);
            }
        }

        #endregion
        //物体渐变显示或者消失
        void ColorDisPlay(Image raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }
        #region 游戏结束
        void EndVideo()
        {
            
            InitSpine(xem.transform, "");
            InitSpine(niao2.transform, "");
            isPass = true;
            _mono.StopCoroutine(shoot1);
          
            niao.transform.GetRectTransform().DOAnchorPos(new Vector2(2200, -332), 3f).OnComplete(()=> {
                    ColorDisPlay(mask2_image, true, null, 0.5f);                   
                    Delay(0.5f,()=> {
                        PlayVoice(6);
                        SpineManager.instance.DoAnimation(beach, "animation", true);
                        ColorDisPlay(mask2_image, false, null, 0.5f);
                    });
                    Delay(0,()=> {
                      
                        
                        SpineManager.instance.DoAnimation(niao2, "niao", false);

                    });

                    Delay(4f,()=> { GameSuccess(); });
                });
                
            
           

        }
        #endregion











        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

            StartVideo();
       
        }






        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
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
                        PlayBgm(0);
                        GameInit();
                      		
                        StartGame();
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

                        _dDD.Show();
                        BellSpeck(_dDD, 2);

                    });
                });
            });

        }

        /// <summary>
        /// 游戏成功界面
        /// </summary>
        private void GameSuccess()
        {
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }




        #endregion

        #region 常用函数

        #region 语音按钮

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region 拖拽相关

        /// <summary>
        /// 设置Drager回调
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="dragStart"></param>
        /// <param name="draging"></param>
        /// <param name="dragEnd"></param>
        /// <param name="onClick"></param>
        /// <returns></returns>
        private List<mILDrager> SetDragerCallBack(Transform parent, Action<Vector3, int, int> dragStart = null, Action<Vector3, int, int> draging = null, Action<Vector3, int, int, bool> dragEnd = null, Action<int> onClick = null)
        {
            var temp = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drager = parent.GetChild(i).GetComponent<mILDrager>();
                temp.Add(drager);
                drager.SetDragCallback(dragStart, draging, dragEnd, onClick);
            }

            return temp;
        }

        /// <summary>
        /// 设置Droper回调(失败)
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="failCallBack"></param>
        /// <returns></returns>
        private List<mILDroper> SetDroperCallBack(Transform parent, Action<int> failCallBack = null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null, null, failCallBack);
            }
            return temp;
        }


        #endregion

        #region Spine相关

        private void InitSpines(Transform parent, bool isKong = true, Action initCallBack = null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (isNullSpine)
                    continue;
                if (isKong)
                    PlaySpine(child, "kong", () => { PlaySpine(child, child.name); });
                else
                    PlaySpine(child, child.name);
            }
            initCallBack?.Invoke();
        }

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

        private GameObject FindGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private float PlayBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, index, isLoop);
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, index, isLoop);
            return time;
        }

        private float PlaySound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index, isLoop);
            return time;
        }

        private float PlayCommonBgm(int index, bool isLoop = true)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, index, isLoop);
            return time;
        }

        private float PlayCommonVoice(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, index, isLoop);
            return time;
        }

        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private void StopAllAudio()
        {
            SoundManager.instance.StopAudio();
        }

        private void StopAudio(SoundManager.SoundType type)
        {
            SoundManager.instance.StopAudio(type);
        }

        private void StopAudio(string audioName)
        {
            SoundManager.instance.Stop(audioName);
        }

        #endregion

        #region 延时相关

        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }

        private void UpDate(bool isStart, float delay, Action callBack)
        {
            _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
        }

        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        IEnumerator IEUpdate(bool isStart, float delay, Action callBack)
        {
            while (isStart)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
            }
        }

        #endregion

        #region 停止协程

        private void StopAllCoroutines()
        {
            _mono.StopAllCoroutines();
        }

        private void StopCoroutines(string methodName)
        {
            _mono.StopCoroutine(methodName);
        }

        private void StopCoroutines(IEnumerator routine)
        {
            _mono.StopCoroutine(routine);
        }

        private void StopCoroutines(Coroutine routine)
        {
            _mono.StopCoroutine(routine);
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
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

        private void AddEvents(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;
                RemoveEvent(child);
                AddEvent(child, callBack);
            }
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #region 修改Rect相关

        private void SetPos(RectTransform rect, Vector2 pos)
        {
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            callBack1?.Invoke();
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack2?.Invoke(); });
        }


        #endregion

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行        
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }
        #endregion

        #endregion

    }
}
