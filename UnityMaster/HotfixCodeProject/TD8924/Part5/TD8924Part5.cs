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
    public class TD8924Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;
        
          #region Mask
        private Transform anyBtns;
        private bool _canClickBtns;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion


        #endregion
        #endregion

        #region 游戏变量
        private Transform _life;
        private GameObject _xem;
        private GameObject _boom;
        private GameObject _bird;
        private Transform _block;
        private Transform _smallQiuYin;
        private GameObject[] _smallQiuYinObj;
        private mILDrager[] _smallQiuYinDrager;
        private mILDroper[] _smallQiuYinDroper;
        private GameObject _bigQiuYin;
        private GameObject _xia;
        private GameObject _yuGan;
        private mILDrager _yuGanDrager;
        private GameObject _ygL;
        private Transform _xiaRandomPos;
        private Transform _xiaEndPos;
        private Transform _yuGanStartPos;
        private Transform _blockStartPos;
        private Transform _blockEndPos;
        private Transform _birdStartPos;
        private Transform _birdEndPos1;
        private Transform _birdEndPos2;
        private Transform _birdEndPos3;

        private int _smallQiuYinCurSibling;
        private GameObject _smallQiuYinCurObj;
        private int[] _xiaIndex;    //虾的编号数组（乱序）
        private int _dragIndex1;   //正确拖拽的虾的编号
        private int _dragIndex2;   //正确拖拽的虾的编号
        private int _count;
        private float _xiaWidth;
        private float _xiaHeight;

        private mILDroper _otherTrueDrop;
        private bool _isDragingYuGan;
        private bool _isEnd = false;

        #endregion

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

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _life = curTrans.Find("Life");
            _xem = curTrans.GetGameObject("XEM");
            _boom = curTrans.GetGameObject("Boom");
            _bird = curTrans.GetGameObject("Bird");
            _block = curTrans.Find("Block");
            _smallQiuYin = _block.Find("SmallQiu");
            _bigQiuYin = curTrans.GetGameObject("BigQiu");
            _smallQiuYinObj = new GameObject[_smallQiuYin.childCount];
            _smallQiuYinDrager = new mILDrager[_smallQiuYin.childCount];
            _smallQiuYinDroper = new mILDroper[_smallQiuYin.childCount];
            for (int i = 0; i < _smallQiuYin.childCount; i++)
            {
                _smallQiuYinObj[i] = _smallQiuYin.GetChild(i).gameObject;
                _smallQiuYinDrager[i] = _smallQiuYin.GetChild(i).gameObject.GetComponent<mILDrager>();
                _smallQiuYinDroper[i] = _smallQiuYin.GetChild(i).gameObject.GetComponent<mILDroper>();
            }

            _xia = curTrans.GetGameObject("Xia");
            _yuGan = curTrans.GetGameObject("YuGan");
            _yuGanDrager = _yuGan.GetComponent<mILDrager>();
            _ygL = curTrans.GetGameObject("YuGan/YuGan/yg-l");
            _xiaRandomPos = curTrans.Find("Pos/XiaRandomPos");
            _xiaEndPos = curTrans.Find("Pos/XiaEndPos");
            _yuGanStartPos = curTrans.Find("Pos/YuGanStartPos");
            _blockStartPos = curTrans.Find("Pos/BlockStartPos");
            _blockEndPos = curTrans.Find("Pos/BlockEndPos");
            _birdStartPos = curTrans.Find("Pos/BirdStartPos");
            _birdEndPos1 = curTrans.Find("Pos/BirdEndPos1");
            _birdEndPos2 = curTrans.Find("Pos/BirdEndPos2");
            _birdEndPos3 = curTrans.Find("Pos/BirdEndPos3");
            _otherTrueDrop = curTrans.Find("Pos/OtherTrueDrop").GetComponent<mILDroper>();
            _xiaIndex = new int[5] { 1, 2, 3, 4, 5 };

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _count = 0;
            _isDragingYuGan = false;
            _xiaWidth = _xia.transform.GetRectTransform().rect.width;
            _xiaHeight = _xia.transform.GetRectTransform().rect.height;
            DOTween.KillAll();
            //打乱虾的出现顺序
            RandomXia();
            //田丁初始化
            TDGameInit();
        }

        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
        }

        
        #region 田丁

        void TDGameInit()
        {

            //动画初始化
            SpineManager.instance.DoAnimation(_bird, "he0", false);
            SpineManager.instance.DoAnimation(_xem, "xem0", true);
            SpineManager.instance.DoAnimation(_boom, "kong", false);
            SpineManager.instance.DoAnimation(_yuGan.transform.GetGameObject("YuGan"), "yg-l", false);

            //展示隐藏初始化
            _bigQiuYin.Hide();
            _xia.Hide();
            _yuGan.Show();
            for (int i = 0; i < _smallQiuYinObj.Length; i++)
                _smallQiuYinObj[i].Show();
            for (int i = 0; i < _life.childCount; i++)
            {
                _life.GetChild(i).gameObject.Show();
            }

            //位置初始化
            _bird.transform.position = _birdStartPos.position;
            _block.position = _blockStartPos.position;
            _yuGan.transform.position = _yuGanStartPos.position;

            canNotXiaoQiuYinDrag();
            canNotYuGanDrag();

            if(_isEnd)
            {
                Game();
                _isEnd = false;
            }
            else
                _isEnd = false;
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //田丁游戏开始方法
            TDGameStartFunc();
        }

        #endregion

        #endregion

        #region 说话语音

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
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法
        
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    {
                        mask.SetActive(false);
                        bd.SetActive(false);
                        Game();
                    }));
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        #endregion

        #region 通用方法
        
        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target,string name,bool isLoop=false,Action callback=null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }
        
        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index,Action goingEvent=null,Action finishEvent=null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target,SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
        }
        
        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex,Action callback=null)
        {
            float voiceTimer= SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }
        
        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }
        
        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer,Action callback)
        {
            
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer,Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
            
        }
        
        
        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        
        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            int random = Random.Range(0, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            int random = Random.Range(4, 10);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }
        
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

        #endregion
        

        #endregion


        #region 田丁
        
          #region 田丁加载

          /// <summary>
          /// 田丁加载所有物体
          /// </summary>
          void TDLoadGameProperty()
          {
              mask = curTrans.Find("mask").gameObject;
              mask.SetActive(true);
              //加载人物
              TDLoadCharacter();
              //加载成功界面
              TDLoadSuccessPanel();
              //加载游戏按钮
              TDLoadButton();
          }

          /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            
            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
        }
       

        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }
        /// <summary>
        /// 加载按钮
        /// </summary>
        void TDLoadButton()
        {
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
        }
        

        #endregion

          #region 切换游戏按键方法

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
            _canClickBtns = true;
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if(_canClickBtns)
            {
                BtnPlaySound();
                _canClickBtns = false;
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
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

        #endregion

        #region 田丁成功动画

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
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true); 
                            ChangeClickArea();
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion

        #region 游戏方法

        //开始游戏，每一关的
        void Game()
        {
            _isDragingYuGan = false;
            _yuGan.Show();
            _yuGan.transform.position = _yuGanStartPos.position;
            RandomXiaPos();
            MoveBlock(_blockEndPos, () => { AddXiaoQiuYinDrag(); });
        }

        //选择框移动
        void MoveBlock(Transform endPos, Action ac = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            _block.DOMove(endPos.position, 1.0f).OnComplete(()=> { ac?.Invoke(); });
        }

        //大蚯蚓移动
        void MoveBigQiuYin()
        {
            _bigQiuYin.transform.DOMove(_ygL.transform.position, 1.0f).OnComplete(
            () => 
            {
                SpineManager.instance.DoAnimation(_bigQiuYin.transform.GetGameObject("0"), "c-x" + _xiaIndex[_count].ToString(), false,
                () =>
                {
                    AddYuGanDrag();
                });
            });
        }

        //虾移动
        void MoveXia()
        {
            if (_count == 4)
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            else
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            _bigQiuYin.Hide();
            _yuGan.Hide();
            SpineManager.instance.DoAnimation(_xia, "x-d" + _xiaIndex[_count].ToString(), false,
            () =>
            {
                _xia.transform.DOMove(_xiaEndPos.position, 1.0f).OnComplete(()=> { SpineManager.instance.DoAnimation(_xia, "kong", false); MoveBird(); });
            });
        }

        //火烈鸟移动
        void MoveBird()
        {
            SpineManager.instance.DoAnimation(_bird, "he1", true);
            Tween tw = _bird.transform.DOMoveX(_birdEndPos1.position.x, 1.5f).OnComplete(
            ()=> 
            {
                SpineManager.instance.DoAnimation(_bird, "he2", false, 
                ()=> 
                {
                    if(_count == 4)
                    {
                        JudgeLife();
                        _count += 1;
                        SpineManager.instance.DoAnimation(_xem, "xem1", false);
                        mono.StartCoroutine(WaitTimeAndExcuteNextIE(1.0f, 
                        () => 
                        {
                            SpineManager.instance.DoAnimation(_boom, "xem-boom", false, ()=> { SpineManager.instance.DoAnimation(_boom, "kong", false); });
                            mono.StartCoroutine(WaitTimeAndExcuteNextIE(0.2f,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_xem, "kong", false);
                                mono.StartCoroutine(WaitTimeAndExcuteNextIE(2.0f,
                                () =>
                                {
                                    _isEnd = true;
                                    playSuccessSpine();
                                }));
                            }));
                        }));
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(_xem, "xem1", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_xem, "xem0", true);
                            SpineManager.instance.DoAnimation(_bird, "he1", true);
                            JudgeLife();
                            _count += 1;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                            _bird.transform.DOMoveX(_birdEndPos2.position.x, 1.5f).OnComplete(
                            () =>
                            {
                                _bird.transform.position = _birdEndPos3.position;
                                _bird.transform.DOMoveX(_birdStartPos.position.x, 1.5f).OnComplete(
                                () =>
                                {
                                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                    SpineManager.instance.DoAnimation(_bird, "he0", false);
                                    Game();
                                });
                            });
                        });
                    }
                });
            });

            tw.SetEase(Ease.Linear);
        }

        //虾的顺序乱序
        void RandomXia()
        {
            int random = 0;
            int temp = 0;
            for (int i = 0; i < _xiaIndex.Length; i++)
            {
                random = Random.Range(0, 5);
                temp = _xiaIndex[random];
                _xiaIndex[random] = _xiaIndex[_xiaIndex.Length - random - 1];
                _xiaIndex[_xiaIndex.Length - random - 1] = temp;
            }
        }

        //虾的位置随机
        void RandomXiaPos()
        {
            if(_xiaIndex[_count] == 1)
            {
                _dragIndex1 = 0;
                _dragIndex2 = 1;
            }
            if (_xiaIndex[_count] == 2)
            {
                _dragIndex1 = 0;
                _dragIndex2 = 3;
            }
            if (_xiaIndex[_count] == 3)
            {
                _dragIndex1 = 1;
                _dragIndex2 = 3;
            }
            if (_xiaIndex[_count] == 4)
            {
                _dragIndex1 = 1;
                _dragIndex2 = 2;
            }
            if (_xiaIndex[_count] == 5)
            {
                _dragIndex1 = 2;
                _dragIndex2 = 3;
            }

            _xia.Show();
            SpineManager.instance.DoAnimation(_xia, "kong", false, ()=> { SpineManager.instance.DoAnimation(_xia, "x-a" + _xiaIndex[_count].ToString(), false); });

            int ranX = Random.Range((int)(_xiaRandomPos.position.x - _xiaRandomPos.GetRectTransform().rect.width / 2), (int)(_xiaRandomPos.position.x + _xiaRandomPos.GetRectTransform().rect.width / 2));
            int ranY = Random.Range((int)(_xiaRandomPos.position.y - _xiaRandomPos.GetRectTransform().rect.height / 2), (int)(_xiaRandomPos.position.y + _xiaRandomPos.GetRectTransform().rect.height / 2));
            _xia.transform.position = new Vector2(ranX, ranY);
            XiaChangePos();
        }

        void XiaChangePos()
        {
            int ranX = Random.Range((int)(_xiaRandomPos.position.x - _xiaRandomPos.GetRectTransform().rect.width / 2), (int)(_xiaRandomPos.position.x + _xiaRandomPos.GetRectTransform().rect.width / 2));
            int ranY = Random.Range((int)(_xiaRandomPos.position.y - _xiaRandomPos.GetRectTransform().rect.height / 2), (int)(_xiaRandomPos.position.y + _xiaRandomPos.GetRectTransform().rect.height / 2));
            float ranTime = Random.Range(20, 50) * 0.1f;
            _xia.transform.DOMove(new Vector3(ranX, ranY, 0), ranTime).OnComplete(() => { XiaChangePos(); });
        }

        void JudgeLife()
        {
            _life.GetChild(_count).gameObject.Hide();
        }
        #endregion

        #region 小蚯蚓拖拽方法

        void AddXiaoQiuYinDrag()
        {
            for (int i = 0; i < _smallQiuYinDrager.Length; i++)
            {
                _smallQiuYinDrager[i].canMove = true;
                _smallQiuYinDrager[i].SetDragCallback(SmallQiuYinDragStart, null, SmallQiuYinDragEnd, null);
                _smallQiuYinDrager[i].drops = new mILDroper[1] { _otherTrueDrop };
                _smallQiuYinObj[i].GetComponent<RawImage>().raycastTarget = true;
            }

            _smallQiuYinDrager[_dragIndex1].drops = new mILDroper[1] { _smallQiuYinDroper[_dragIndex2] };
            _smallQiuYinDrager[_dragIndex2].drops = new mILDroper[1] { _smallQiuYinDroper[_dragIndex1] };
        }

        void canNotXiaoQiuYinDrag()
        {
            for (int i = 0; i < _smallQiuYinObj.Length; i++)
            {
                _smallQiuYinObj[i].GetComponent<RawImage>().raycastTarget = false;
                _smallQiuYinDrager[i].SetDragCallback(null, null, null, null);
            }
        }

        private void SmallQiuYinDragStart(Vector3 dragPos, int dragType, int dragIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            _smallQiuYinCurSibling = _smallQiuYinObj[dragIndex].transform.GetSiblingIndex();
            _smallQiuYinCurObj = _smallQiuYinObj[dragIndex];
            _smallQiuYinCurObj.transform.SetAsLastSibling();
        }

        private void SmallQiuYinDragEnd(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _smallQiuYinCurObj.transform.SetSiblingIndex(_smallQiuYinCurSibling);
            _smallQiuYinDrager[dragIndex].DoReset();
            if (dragBool)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                BtnPlaySoundSuccess();
                canNotXiaoQiuYinDrag();
                _smallQiuYinObj[_dragIndex1].Hide();
                _smallQiuYinObj[_dragIndex2].Hide();
                _bigQiuYin.Show();
                if(_dragIndex1 == dragIndex)
                    _bigQiuYin.transform.position = _smallQiuYinObj[_dragIndex2].transform.position;
                else
                    _bigQiuYin.transform.position = _smallQiuYinObj[_dragIndex1].transform.position;
                SpineManager.instance.DoAnimation(_bigQiuYin.transform.GetGameObject("0"), "c-d" + _xiaIndex[_count].ToString(), false,
                () =>
                {
                    MoveBigQiuYin();
                    MoveBlock(_blockStartPos, ()=> 
                    {
                        _smallQiuYinObj[_dragIndex1].Show();
                        _smallQiuYinObj[_dragIndex2].Show();
                    });
                });
            }
            else
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
                BtnPlaySoundFail();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            }
        }
        #endregion

        #region 鱼竿拖拽方法

        void AddYuGanDrag()
        {
            _yuGanDrager.canMove = true;
            _yuGanDrager.SetDragCallback(YuGanDragStart, YuGanDraging, YuGanDragEnd, null);
            _yuGan.GetComponent<Empty4Raycast>().raycastTarget = true;
        }

        private void YuGanDraging(Vector3 dragPos, int dragType, int dragIndex)
        {
            _bigQiuYin.transform.position = _ygL.transform.position;
            if(_ygL.transform.position.x <= _xia.transform.position.x + _xiaWidth / 2 && _ygL.transform.position.x >= _xia.transform.position.x - _xiaWidth / 2)
            {
                if (_ygL.transform.position.y <= _xia.transform.position.y + _xiaHeight / 2 && _ygL.transform.position.y >= _xia.transform.position.y - _xiaHeight / 2)
                {
                    if (_isDragingYuGan)
                    {
                        canNotYuGanDrag();
                        _isDragingYuGan = false;
                        DOTween.KillAll();
                        MoveXia();
                    }
                }
            }
        }

        void canNotYuGanDrag()
        {
            _yuGanDrager.SetDragCallback(null, null, null, null);
            _yuGan.GetComponent<Empty4Raycast>().raycastTarget = false;
        }


        private void YuGanDragStart(Vector3 dragPos, int dragType, int dragIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            _isDragingYuGan = true;
        }

        

        private void YuGanDragEnd(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _isDragingYuGan = false;
        }

        #endregion
    }
}
