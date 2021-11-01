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
    public class TD6722Part5
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
        private bool _canClickBtn = true;
        private GameObject mask;

        #region 成功
        private GameObject successSpine;
        private BellSprites _succeessCount;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion
        
        #region 田丁对话

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        #endregion

        #endregion

        #endregion

        private GameObject _th;
        private GameObject _thTrigger;
        private GameObject _xem;
        private Transform _xemPar;
        private GameObject _xemShou;
        private GameObject _ttq;
        private BellSprites _ttqSprite;
        private MonoScripts _monoScripts;
        private int _aniRan;
        private int[] _XEMRan;
        private int _ranCount;

        private GameObject _timeBg;
        private GameObject _time;
        private Transform _clock;
        private Transform _clockStartPos;
        private Transform _clockEndPos;

        private Transform _xemEnd1;
        private Transform _xemEnd2;
        private float _speed;
        private int _count;
        private BellSprites _countSprite;

        private bool _isStart = false;
        private bool _isEnd = false;
        private bool _gameStart = false;
        private bool _canSpeedUp = false;
        private bool _playClipOne;
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

            _th = curTrans.GetGameObject("TH");
            _thTrigger = curTrans.GetGameObject("TH/Trigger");
            _xem = curTrans.GetGameObject("XEM/XEM");
            _xemPar = _xem.transform.parent;
            _xemShou = curTrans.GetGameObject("XEM/XEMShou");
            _ttq = curTrans.GetGameObject("XEM/TTQ");
            _ttqSprite = _ttq.GetComponent<BellSprites>();
            _XEMRan = new int[5] { 1, 2, 3, 4, 5 };
            _monoScripts = Bg.GetComponent<MonoScripts>();
            _monoScripts.FixedUpdateCallBack = SFixedUpdate;

            _timeBg = curTrans.GetGameObject("TimeBg");
            _time = curTrans.GetGameObject("TimeBg/time");
            _clock = curTrans.Find("TimeBg/clock");
            _clockStartPos = curTrans.Find("TimeBg/clockStartPos");
            _clockEndPos = curTrans.Find("TimeBg/clockEndPos");

            _xemEnd1 = curTrans.Find("XEMEnd1");
            _xemEnd2 = curTrans.Find("XEMEnd2");
            _countSprite = curTrans.Find("CountBg/Count").GetComponent<BellSprites>();

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            textSpeed = 0.1f;
            _speed = 500.0f;
            _count = 0;
            _ranCount = 0;
            _time.GetComponent<Image>().fillAmount = 0;

            _xem.Show();
            _ttq.Hide();
            _xemShou.Hide();
            for (int i = 0; i < _thTrigger.transform.childCount; i++)
            {
                _thTrigger.transform.GetChild(i).gameObject.Hide();
            }

            SetPos(_clock.GetRectTransform(), _clockStartPos.GetRectTransform().anchoredPosition);
            SetPos(_xemPar.GetRectTransform(), new Vector2(0, -(_xemPar.GetRectTransform().rect.height / 2 + 50)));
            SetPos(_th.transform.GetRectTransform(), new Vector2(0, _th.transform.GetRectTransform().rect.height / 2));
            SetPos(_ttq.transform.GetRectTransform(), curTrans.Find("XEM/TTQ_pos").GetRectTransform().anchoredPosition);

            DOTween.KillAll();
            UpdateCount();

            _th.GetComponent<mILDrager>().canMove = true;
            SpineManager.instance.DoAnimation(_timeBg, "time", false);
            RandomXemAni();

            _playClipOne = true;
            _canSpeedUp = false;
            if(_isEnd)
            {
                _isEnd = false;
                _isStart = true;
                TimeDown();
                StartGame();
                InitTTQImg();
            }
        }

        void GameStart()
        {
            //田丁开始游戏
            TDGameStart();
        }

        #region 田丁
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, 
                () =>
                {
                    ShowDialogue("继续吃吧，把身体全部变成食物吧！！！", devilText);
                }, 
                () =>
                {
                    buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                        {
                            ShowDialogue("啊，我不要！", bdText);
                        }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                    });
                }));
            });
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

            if (speaker != bd)
            {
                SpineManager.instance.DoAnimation(speaker, "daiji");
                yield return new WaitForSeconds(len);

                float ind = SoundManager.instance.PlayClip(type, clipIndex);
                SpineManager.instance.DoAnimation(speaker, "speak");

                method_1?.Invoke();

                yield return new WaitForSeconds(ind);
                SpineManager.instance.DoAnimation(speaker, "daiji");
                SoundManager.instance.SetShield(true);

                method_2?.Invoke();
            }
            else
            {
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
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    break;
            }
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            buDing.SetActive(false);
            devil.SetActive(false);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, 
            () => 
            { 
                mask.SetActive(false); 
                bd.SetActive(false);
                WaitTimeAndExcuteNext(0.5f,
                () =>
                {
                    StartGame();
                    InitTTQImg();
                    _isStart = true;
                    TimeDown();
                });
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
            int random;
            do
            {
                random = Random.Range(0, 4);
            }
            while (random == 0);

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            int random;
            do
            {
                random = Random.Range(4, 10);
            }
            while( random == 4 || random == 8);

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
            Tween tw = rect.DOMove(v2, duration).OnComplete(() => { callBack?.Invoke(); });
            tw.SetEase(Ease.Linear);
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
              //任务对话方法加载
              TDLoadDialogue();
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
        /// 加载对话环节
        /// </summary>
        void TDLoadDialogue()
        {
            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdText.text = "";
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilText.text = "";
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");

            buDing.SetActive(true);
            devil.SetActive(true);
        }

        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            _succeessCount = curTrans.Find("mask/successSpine/Count").GetComponent<BellSprites>();
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-";
            sz = "6-12";
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
            _canClickBtn = true;
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
            ChangeClickArea();
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
                            anyBtns.gameObject.SetActive(false); GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); _isEnd = true; });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 4)); });
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

        #region 田丁对话方法

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        #endregion
        
          #region 田丁成功动画

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            _succeessCount.transform.GetComponent<RawImage>().texture = _succeessCount.texture[_count];
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
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
                            caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                        });
                });
        }

        #endregion

        #endregion

        #region 游戏方法

        #region 拖拽相关

        #endregion

        #region 其他

        //小恶魔随机动画
        void RandomXemAni()
        {
            if (_ranCount == 0 || _ranCount % 5 == 0)
                RandomXEMArray();
            _aniRan = _XEMRan[_ranCount % 5];
            PlaySpineAni(_xem, "xem-" + _aniRan.ToString(), true);
            _ranCount++;
        }

        //小恶魔移动
        void StartMove()
        {
            _th.GetComponent<mILDrager>().canMove = true;
            int random = Random.Range(0, 2);
            float dis;
            if (random == 0)
            {
                dis = Vector2.Distance(_xem.transform.position, _xemEnd1.position);
                if (dis >= 100)
                    SetMove(_xemPar.GetRectTransform(), _xemEnd1.position, dis / _speed, () => { StartMove(); });
                else
                {
                    dis = Vector2.Distance(_xem.transform.position, _xemEnd2.position);
                    SetMove(_xemPar.GetRectTransform(), _xemEnd2.position, dis / _speed, () => { StartMove(); });
                }
            }
            else
            {
                dis = Vector2.Distance(_xem.transform.position, _xemEnd2.position);
                if (dis >= 100)
                    SetMove(_xemPar.GetRectTransform(), _xemEnd2.position, dis / _speed, () => { StartMove(); });
                else
                {
                    dis = Vector2.Distance(_xem.transform.position, _xemEnd1.position);
                    SetMove(_xemPar.GetRectTransform(), _xemEnd1.position, dis / _speed, () => { StartMove(); });
                }
            }
        }

        //设置投甜甜圈时间
        void SetStop()
        {
            int random;
            if (_canSpeedUp)
                random = Random.Range(15, 20);
            else
                random = Random.Range(25, 40);
            WaitTimeAndExcuteNext(random * 0.1f,
            () =>
            {
                _ttq.Show();
                _xemShou.Show();
                PlaySpineAni(_xem, "xem-f-0", false);
                WaitTimeAndExcuteNext(0.33f, () => { SetMove(_ttq.transform.GetRectTransform(), new Vector2(_ttq.transform.position.x, _th.transform.Find("tanhuang").position.y - 150), 0.5f); });

                PlaySpineAni(_xemShou, "xem-shou2", false, () => 
                {
                    _xemShou.Hide();
                    PlaySpineAni(_xem, "xem-f-1", true);
                });
            });
        }

        void StartGame()
        {
            StartMove();
            SetStop();
        }

        void UpdateCount()
        {
            _countSprite.transform.GetComponent<RawImage>().texture = _countSprite.texture[_count];
            _countSprite.transform.GetComponent<RawImage>().SetNativeSize();
        }

        private void SFixedUpdate()
        {
            //判断接球
            if (_gameStart)
            {
                if (_ttq.transform.position.y - _thTrigger.transform.position.y <= (_thTrigger.transform.GetRectTransform().rect.height + _ttq.transform.GetRectTransform().rect.height) / 2 && _ttq.transform.position.y - _thTrigger.transform.position.y > 0)
                {
                    if (Mathf.Abs(_ttq.transform.position.x - _thTrigger.transform.position.x) <= (_ttq.transform.GetRectTransform().rect.width + _thTrigger.transform.GetRectTransform().rect.width) / 2)
                    {
                        if (_aniRan == 1 || _aniRan == 4 || _aniRan == 5)
                            SuccessTTQ();
                        else
                            FalseAndNormalTTQ(false);
                    }
                }

                if (curTrans.Find("TH/tanhuang").position.y - _ttq.transform.position.y >= 100f)
                {
                    if (_aniRan == 2 || _aniRan == 3)
                        FalseAndNormalTTQ(true);
                    else
                        FalseAndNormalTTQ(false);
                }
            }

            if(_isStart)
            {
                _time.GetComponent<Image>().fillAmount += 0.0004445f;
            }

            if(_time.GetComponent<Image>().fillAmount >= 0.667f && _time.GetComponent<Image>().fillAmount != 1.0f)
            {
                _canSpeedUp = true;
            }

            if (_time.GetComponent<Image>().fillAmount >= 0.90009f && _playClipOne)
            {
                _playClipOne = false;
                if (SpineManager.instance.GetCurrentAnimationName(_timeBg) != "time2")
                {
                    SpineManager.instance.DoAnimation(_timeBg, "time2", true);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);
            }
        }

        void SuccessTTQ()
        {
            _gameStart = false;
            _thTrigger.transform.GetChild(0).gameObject.Show();
            _count++;
            UpdateCount();
            PlaySpineAni(_th.transform.GetGameObject("tanhuang"), "th", false);
            BtnPlaySoundSuccess();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SetMove(_ttq.transform.GetRectTransform(), new Vector2(_ttq.transform.position.x, 1500), 0.5f,
            () =>
            {
                RandomXemAni();
                InitTTQImg();
                SetStop();
                _thTrigger.transform.GetChild(0).gameObject.Hide();
            });
        }

        void FalseAndNormalTTQ(bool normal)
        {
            _gameStart = false;

            if (!normal)
            {
                BtnPlaySoundFail();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            }

            WaitTimeAndExcuteNext(0.25f, () => 
            { 
                RandomXemAni();
                InitTTQImg();
                SetStop();
            });
        }

        void InitTTQImg()
        {
            _ttq.GetComponent<RawImage>().texture = _ttqSprite.texture[_aniRan - 1];
            SetPos(_ttq.transform.GetRectTransform(), curTrans.Find("XEM/TTQ_pos").GetRectTransform().anchoredPosition);
            _ttq.Hide();
            _gameStart = true;
        }

        void TimeDown()
        {
            Tween tw = _clock.transform.DOMove(_clockEndPos.transform.position, 45).OnComplete(
            () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SpineManager.instance.DoAnimation(_timeBg, "time", false);
                _gameStart = false;
                _isEnd = true;

                _xem.Hide();
                _ttq.Hide();
                _th.GetComponent<mILDrager>().canMove = false;
                mono.StopAllCoroutines();
                DOTween.KillAll();
                playSuccessSpine();
            });
            tw.SetEase(Ease.Linear);
        }

        //甜甜圈出现顺序乱序
        void RandomXEMArray()
        {
            for (int i = 0; i < _XEMRan.Length; i++)
            {
                int ran = Random.Range(0, 5);
                int temp = _XEMRan[ran];
                _XEMRan[ran] = _XEMRan[_XEMRan.Length - ran - 1];
                _XEMRan[_XEMRan.Length - ran - 1] = temp;
            }
        }
        #endregion

        #endregion
    }
}
