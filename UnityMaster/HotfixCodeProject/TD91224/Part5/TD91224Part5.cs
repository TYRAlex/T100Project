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
        next,
    }
    public class TD91224Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private BellSprites bellTextures;

        #region 田丁
        private GameObject bd;
        private GameObject dbd;
        
          #region Mask
        private Transform anyBtns;
        private bool _canClickBtn;
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
        private Transform _gameBg;
        private BellSprites _gameBgSprites;
        private GameObject _xem;
        private GameObject _xemBoom;
        private Transform _life;

        private Transform _levelAll;
        private Transform _level1;
        private GameObject[] _click1;
        private Transform _level2;
        private GameObject[] _click2;
        private Transform _level3;
        private GameObject[] _click3;

        private Transform _level1Copy;
        private Transform _level2Copy;
        private Transform _level3Copy;

        private bool _canClickObj;
        private int _count;
        private float _moveSpeed;

        private string _curQianZhui;
        private GameObject _lastClickObj;
        private GameObject[] _needKongObj;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _gameBg = curTrans.Find("GameBg");
            _gameBgSprites = _gameBg.GetComponent<BellSprites>();
            _xem = _gameBg.GetGameObject("XEM");
            _xemBoom = _gameBg.GetGameObject("XEM_Boom");
            _life = _gameBg.Find("Life");

            _levelAll = curTrans.Find("Level");
            _level1 = _levelAll.Find("1");
            _level2 = _levelAll.Find("2");
            _level3 = _levelAll.Find("3");

            _click1 = new GameObject[_level1.childCount];
            for (int i = 0; i < _level1.childCount; i++)
            {
                _click1[i] = _level1.GetChild(i).gameObject;
                Util.AddBtnClick(_click1[i], ClickEvent);
            }
            _click2 = new GameObject[_level2.childCount];
            for (int i = 0; i < _level2.childCount; i++)
            {
                _click2[i] = _level2.GetChild(i).gameObject;
                Util.AddBtnClick(_click2[i], ClickEvent);
            }
            _click3 = new GameObject[_level3.childCount];
            for (int i = 0; i < _level3.childCount; i++)
            {
                _click3[i] = _level3.GetChild(i).gameObject;
                Util.AddBtnClick(_click3[i], ClickEvent);
            }

            _level1Copy = curTrans.Find("Level1Copy");
            _level2Copy = curTrans.Find("Level2Copy");
            _level3Copy = curTrans.Find("Level3Copy");

            _needKongObj = new GameObject[3] { _level1.GetGameObject("1/c"), _level2.GetGameObject("1/e"), _level3.GetGameObject("2/c") };
            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            DOTween.KillAll();

            talkIndex = 1;
            _count = 0;
            _moveSpeed = 350f * (Screen.width / 1080);

            _canClickBtn = true;
            _canClickObj = true;

            _lastClickObj = null;
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
            SpineManager.instance.DoAnimation(_xem, "xem", true);
            SpineManager.instance.DoAnimation(_xemBoom, "kong", true);

            //位置初始化
            for (int i = 0; i < _level1.childCount; i++)
            {
                _level1.GetGameObject(i.ToString()).transform.position = _level1Copy.GetGameObject(i.ToString()).transform.position;
            }
            for (int i = 0; i < _level2.childCount; i++)
            {
                _level2.GetGameObject(i.ToString()).transform.position = _level2Copy.GetGameObject(i.ToString()).transform.position;
            }
            for (int i = 0; i < _level3.childCount; i++)
            {
                _level3.GetGameObject(i.ToString()).transform.position = _level3Copy.GetGameObject(i.ToString()).transform.position;
            }

            //显示与隐藏
            _level1.gameObject.Hide();
            _level2.gameObject.Hide();
            _level3.gameObject.Hide();
            for (int i = 0; i < _life.childCount; i++)
            {
                _life.GetChild(i).gameObject.Show();
            }

            StartLevel();
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                    () => {
                        mask.SetActive(false);
                        bd.SetActive(false);
                    }));
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            int random;
            do
            {
                random = Random.Range(4, 10);
            }
            while (random == 4 || random == 8);
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
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
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
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                    }
                    else if (obj.name == "next")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false);  NextLevel(); });
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

        //水果与蔬菜的动画
        void ObjectAni()
        {
            Transform parent = _levelAll.GetChild(_count);
            _levelAll.gameObject.Show();
            parent.gameObject.Show();
            for (int i = 0; i < parent.childCount; i++)
            {
                GameObject o = parent.GetChild(i).GetChild(0).gameObject;
                float random = Random.Range(10, 20) * 0.01f;
                SpineManager.instance.DoAnimation(o, _curQianZhui + o.name, false,
                () =>
                {
                    SpineManager.instance.SetTimeScale(o, 1 + random);
                    SpineManager.instance.DoAnimation(o, _curQianZhui + o.name + "2", true);
                });
            }
        }

        //整理当前场景的层级
        void OrderTheObj()
        {
            Transform trans = _levelAll.GetChild(_count);
            for (int i = 0; i < _count + 4; i++)
            {
                trans.GetGameObject(i.ToString()).transform.SetSiblingIndex(i);
            }
        }


        //判断当前动画的前缀
        void GetCurQianZhui()
        {
            if(_count == 0)
            {
                _curQianZhui = "xhs-";
            }
            if (_count == 1)
            {
                _curQianZhui = "xj-";
            }
            if (_count == 2)
            {
                _curQianZhui = "bc-";
            }
        }

        private void StartLevel()
        {
            _levelAll.GetChild(_count).gameObject.Show();
            GetCurQianZhui();
            ObjectAni();
            OrderTheObj();
            _gameBg.GetComponent<RawImage>().texture = _gameBgSprites.texture[_count];
        }

        //点击下一关
        private void NextLevel()
        {
            _count++;
            _levelAll.GetChild(_count - 1).gameObject.Hide();
            StartLevel();
            _canClickObj = true;
        }

        //点击水果与蔬菜进行换位
        private void ClickEvent(GameObject obj)
        {
            if(_canClickObj)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                //判断是否执行过了换位操作
                if (_lastClickObj == null)
                {
                    _lastClickObj = obj;
                    GameObject o = obj.transform.GetChild(0).gameObject;
                    SpineManager.instance.DoAnimation(o, _curQianZhui + o.name + "4", true);
                }
                else
                {
                    //判断两次点击是不是同一个物体
                    if (obj.name == _lastClickObj.name)
                    {
                        GameObject o = obj.transform.GetChild(0).gameObject;
                        SpineManager.instance.DoAnimation(o, _curQianZhui + o.name + "2", true);
                        _lastClickObj = null;
                    }
                    else
                    {
                        _canClickObj = false;
                        _lastClickObj.transform.SetAsLastSibling();
                        obj.transform.SetAsLastSibling();
                        float X1 = _lastClickObj.transform.position.x;
                        float X2 = obj.transform.position.x;
                        GameObject o1 = _lastClickObj.transform.GetChild(0).gameObject;
                        GameObject o2 = obj.transform.GetChild(0).gameObject;

                        SpineManager.instance.DoAnimation(o1, _curQianZhui + o1.name + "3", true);
                        SpineManager.instance.DoAnimation(o2, _curQianZhui + o2.name + "3", true);

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                        Tween tw1 = _lastClickObj.transform.DOMoveX(X2, Mathf.Abs(X1 -X2) / _moveSpeed);
                        Tween tw2 = obj.transform.DOMoveX(X1, Mathf.Abs(X1 - X2) / _moveSpeed);
                        tw1.SetEase(Ease.Linear);
                        tw2.SetEase(Ease.Linear);

                        WaitTimeAndExcuteNext(Mathf.Abs(X1 - X2) / _moveSpeed,
                        () =>
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            SpineManager.instance.DoAnimation(o1, _curQianZhui + o1.name + "2", true);
                            SpineManager.instance.DoAnimation(o2, _curQianZhui + o2.name + "2", true);
                            _lastClickObj = null;
                            OrderTheObj();
                            JudgeOrder();
                        });
                    }
                }
            }
        }

        void JudgeOrder()
        {
            bool success = true;
            Transform parent = _levelAll.GetChild(_count);
            for (int i = 0; i < parent.childCount - 1; i++)
            {
                if(parent.GetChild(i).transform.position.x > parent.GetChild(i + 1).transform.position.x)
                {
                    success = false;
                    _canClickObj = true;
                    return;
                }
            }

            if (success)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                for (int i = 0; i < parent.childCount; i++)
                {
                    GameObject o = parent.GetChild(i).GetChild(0).gameObject;
                    SpineManager.instance.DoAnimation(o, _curQianZhui + o.name + "5", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(o, _curQianZhui + o.name + "2", true);
                    });
                }
                SuccesAction();
            }
        }

        private void SuccesAction()
        {
            WaitTimeAndExcuteNext(1.5f,
            () =>
            {
                SpineManager.instance.DoAnimation(_needKongObj[_count], "kong", false);

                String newStr = _curQianZhui.Remove(_curQianZhui.Length - 1, 1);
                SpineManager.instance.DoAnimation(_xemBoom, "xem-" + newStr, false, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_xemBoom, "kong", false);
                    SpineManager.instance.DoAnimation(_xem, "xem2", true);
                    _life.GetChild(_count).gameObject.Hide();
                    WaitTimeAndExcuteNext(1.0f,
                    () =>
                    {
                        BtnPlaySoundSuccess();
                        if (_count < 2)
                        {
                            SpineManager.instance.DoAnimation(_xem, "xem", true);
                            WaitTimeAndExcuteNext(2.0f, 
                            () => 
                            {
                                mask.Show();
                                anyBtns.gameObject.SetActive(true);
                                anyBtns.GetChild(0).gameObject.SetActive(true);
                                anyBtns.GetChild(1).gameObject.SetActive(false);
                                anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                ChangeClickArea();
                            });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(_xem, "kong", false);
                            WaitTimeAndExcuteNext(2.0f, () => { playSuccessSpine(); });
                        }
                    });
                });
            });
        }
        #endregion
    }
}
