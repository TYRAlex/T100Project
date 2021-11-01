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
    public class TD3432Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        
        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁
        private GameObject tt;
        private GameObject dtt;
        private GameObject dem;
        
          #region Mask
        private Transform anyBtns;
        private GameObject mask;
        private bool _canClickBtn;

        #region 成功
        private GameObject successSpine;
        private GameObject caidaiSpine;
        //胜利动画名字
        private string tz;
        private string sz;
        #endregion

        #endregion
        #endregion

        #region 游戏

        private Transform _life;
        private Transform _xemHead;
        private GameObject _mesh;
        private Transform _gate;
        private GameObject _xem;
        private GameObject _octopus;
        private Vector2 _octopusPos;
        private Transform _football;
        private Transform _footballPos;
        private GameObject[] _footballAni;
        private GameObject[] _footballClick;

        private int _count;
        private int _lifeCount;
        private int[] _typeArray;
        private Dictionary<int, int[]> _footballAniDic;
        private Tween _octopusTween;
        private bool _canClickBall;
        private float _speed;
        #endregion

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            _life = curTrans.Find("Life");
            _xemHead = curTrans.Find("xem_head");
            _mesh = curTrans.GetGameObject("QiuMen/Mesh");
            _gate = curTrans.Find("QiuMen/Gate");
            _xem = curTrans.GetGameObject("XEM");
            _octopus = curTrans.GetGameObject("Octopus");
            _octopusPos = curTrans.Find("OctopusPos").position;
            _football = curTrans.Find("FootBall");
            _footballPos = curTrans.Find("FootBallPos");
            _footballAni = new GameObject[_football.childCount];
            _footballClick = new GameObject[_football.childCount];
            for (int i = 0; i < _football.childCount; i++)
            {
                _footballAni[i] = _football.GetChild(i).gameObject;
                _footballClick[i] = _football.GetChild(i).GetChild(0).gameObject;
                Util.AddBtnClick(_footballClick[i], ClickBall);
            }

            _footballAniDic = new Dictionary<int, int[]>();
            _footballAniDic.Add(0, new int[4] { 5, 2, 4, 1 });
            _footballAniDic.Add(1, new int[4] { 8, 4, 3, 1 });
            _footballAniDic.Add(2, new int[4] { 7, 6, 5, 1 });

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }

        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _lifeCount = 3;
            _count = 0;
            _speed = 300f;
            _canClickBtn = true;
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
            _canClickBall = true;
            _xemHead.GetComponent<RawImage>().texture = _xemHead.GetComponent<BellSprites>().texture[0];
            _xem.transform.SetSiblingIndex(_football.transform.GetSiblingIndex() - 1);

            //动画初始化
            _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_xem, "xem", true);
            _octopus.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_octopus, "zy1", true);

            //显示与隐藏
            for (int i = 0; i < _life.childCount; i++)
            {
                _life.GetChild(i).gameObject.Show();
            }

            //位置初始化
            _octopus.transform.position = _octopusPos;

            //方法
            RandomTypeArray();
            UpdateFootball();
            UpdateGate();
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
                speaker = tt;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, speaker != dem ? "animation" : "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, speaker != dem ? "animation2" : "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, speaker != dem ? "animation" : "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void Wait(float len = 0, Action method_1 = null)
        {
            mono.StartCoroutine(WaitCoroutine(method_1, len));
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
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
                    mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 2, null,
                    () =>
                    {
                        tt.SetActive(false);
                        mask.SetActive(false);
                        _canClickBall = true;
                    }));
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            dem.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(dem, SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                dem.SetActive(false);
                tt.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
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
            
            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dtt = curTrans.Find("mask/DTT").gameObject;
            dtt.SetActive(false);
            dem = curTrans.Find("mask/DEM").gameObject;
            dem.SetActive(false);
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
                case BtnEnum.next:
                    result = "next";
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
                    else if (obj.name == "next")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); Next(); });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dtt.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dtt, SoundManager.SoundType.VOICE, 3)); });
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
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

        #region 游戏

        void RandomTypeArray()
        {
            _typeArray = new int[3] { 0, 0, 0 };
            for (int i = 0; i < _typeArray.Length; i++)
            {
                int random = 0;
                do
                {
                    random = Random.Range(0, _typeArray.Length);
                }
                while (_typeArray[random] != 0);

                _typeArray[random] = i;
            }
        }

        //更新球门颜色
        void UpdateGate()
        {
            _gate.GetComponent<RawImage>().texture = _gate.GetComponent<BellSprites>().texture[_typeArray[_count]];
        }

        //更新小球颜色
        void UpdateFootball()
        {
            for (int i = 0; i < _footballAni.Length; i++)
            {
                _footballAni[i].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                _footballAni[i].transform.position = _footballPos.GetChild(i).position;
                SpineManager.instance.DoAnimation(_footballAni[i], "q" + _footballAniDic[_typeArray[_count]][i].ToString(), false);
            }
        }

        void UpdateLife()
        {
            _lifeCount--;
            for (int i = 0; i < _life.childCount; i++)
            {
                if(_life.GetChild(i).gameObject.activeSelf)
                {
                    _life.GetChild(i).gameObject.Hide();
                    return;
                }
            }
        }

        void BallMove(GameObject o)
        {
            o.transform.DOMove(new Vector2(_gate.position.x, 167), 0.5f);
        }

        void NextLevel()
        {
            _count++;
            if (_lifeCount == 0)
            {
                _canClickBall = false;

                WaitTimeAndExcuteNext(1.0f,
                () =>
                {
                    //动画初始化
                    _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_xem, "xem", true);
                    _octopus.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(_octopus, "zy1", true);

                    //位置初始化
                    _octopus.transform.position = _octopusPos;
                    _count--;
                    UpdateFootball();
                    playSuccessSpine();
                });
            }
            else
            {
                if (_count % 3 != 0 || _count == 0)
                {
                    Next();
                }
                else
                {
                    _count = 0;
                    RandomTypeArray();
                    Next();
                }
            }
        }

        void Next()
        {
            _canClickBall = true;
            //动画初始化
            _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_xem, "xem", true);
            _octopus.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_octopus, "zy1", true);

            //位置初始化
            _octopus.transform.position = _octopusPos;

            _xem.transform.SetSiblingIndex(_football.transform.GetSiblingIndex() - 1);
            //方法
            UpdateFootball();
            UpdateGate();
        }

        //点击小球
        private void ClickBall(GameObject obj)
        {
            if(_canClickBall)
            {
                _canClickBall = false;
                float dis = Mathf.Abs(obj.transform.position.x - _octopus.transform.position.x);
                if (_typeArray[_count] == 0 && obj.name == "1")
                {
                    ClickTrue(obj, dis, "xem2", _footballAni[1], "1-ok");
                }
                else if(_typeArray[_count] == 1 && obj.name == "0")
                {
                    ClickTrue(obj, dis, "xem3", _footballAni[0], "2-ok");
                }
                else if (_typeArray[_count] == 2 && obj.name == "2")
                {
                    ClickTrue(obj, dis, "xem3", _footballAni[2], "3-ok");
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
                    SpineManager.instance.DoAnimation(_octopus, "zy2", true);
                    _octopusTween.SetEase(Ease.Linear);
                    _octopusTween = _octopus.transform.DOMove(new Vector2(obj.transform.position.x - 50, _octopus.transform.position.y), dis / _speed).OnComplete(
                    () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SpineManager.instance.DoAnimation(_octopus, "zy5", false, () => { SpineManager.instance.DoAnimation(_octopus, "zy6", false); });
                        Wait(1.0f,
                        () =>
                        {
                            int random = Random.Range(0, 2);
                            string aniName = SpineManager.instance.GetCurrentAnimationName(obj.transform.parent.gameObject);
                            SpineManager.instance.DoAnimation(_xem, random == 0 ? "xem4" : "xem5", false, 
                            () => 
                            {
                                _xem.transform.SetSiblingIndex(_football.transform.GetSiblingIndex() + 1);
                                SpineManager.instance.DoAnimation(_xem, "xem6", true); 
                            });
                            Wait(0.1f, 
                            () => 
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, random == 0 ? 3 : 2, false);
                                BallMove(obj.transform.parent.gameObject); 
                            });
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, random == 0 ? "qa" + aniName[1].ToString() : "qb" + aniName[1].ToString(), false,
                            () =>
                            {
                                BtnPlaySoundFail();
                                SpineManager.instance.DoAnimation(_octopus, "zy4", false, () => { SpineManager.instance.DoAnimation(_octopus, "zy1", true); });
                                Wait(2.5f, 
                                () => 
                                { 
                                    NextLevel(); 
                                });
                            });
                        });
                    });
                }
            }
        }

        //正确的踢足球
        void ClickTrue(GameObject clickobj, float dis, string xemAni, GameObject ballAniObj, string ballAni)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            SpineManager.instance.DoAnimation(_octopus, "zy2", true);
            _octopusTween.SetEase(Ease.Linear);
            _octopusTween = _octopus.transform.DOMove(new Vector2(clickobj.transform.position.x - 50, _octopus.transform.position.y), dis / _speed).OnComplete(
            () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SpineManager.instance.DoAnimation(_octopus, "zy5", false, () => { SpineManager.instance.DoAnimation(_octopus, "zy6", false); });
                Wait(1.0f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_xem, xemAni, false,
                    () =>
                    {
                        _xem.transform.SetSiblingIndex(_football.transform.GetSiblingIndex() + 1);
                        SpineManager.instance.DoAnimation(_xem, "xem", true);
                    });
                    Wait(0.1f, 
                    () => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        BallMove(ballAniObj); 
                    });
                    SpineManager.instance.DoAnimation(ballAniObj, ballAni, false,
                    () =>
                    {
                        BtnPlaySoundSuccess();
                        SpineManager.instance.DoAnimation(_octopus, "zy3", true);
                        UpdateLife();
                        if(_lifeCount == 0)
                            _xemHead.GetComponent<RawImage>().texture = _xemHead.GetComponent<BellSprites>().texture[1];
                        Wait(2.5f, () => { NextLevel(); });
                    });
                });
            });
        }
        #endregion

    }
}
