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
    public class TD91232Part5
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
        bool canClickBtn;
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

        private GameObject _tree;
        private GameObject _t;
        private GameObject _xem;
        private Transform _pos;
        private Vector2[] _posVector;
        private Transform _flower;
        private mILDrager[] _flowerDrager;
        private GameObject[] _flowerAni;
        private Transform _inBlock;

        private string[] _flowerAniL1;
        private string[] _flowerAniL2;
        private int[] _randomPos;
        private int _level;
        private bool _isEnd = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            DOTween.KillAll();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();

            _tree = curTrans.GetGameObject("tree");
            _t = curTrans.GetGameObject("t");
            _xem = curTrans.GetGameObject("xem");
            _pos = curTrans.Find("Pos");
            _posVector = new Vector2[_pos.childCount];
            for (int i = 0; i < _pos.childCount; i++)
            {
                _posVector[i] = _pos.GetChild(i).position;
            }
            _flower = curTrans.Find("Flower");
            _inBlock = curTrans.Find("InBlock");

            _flowerAniL1 = new string[8] { "h-r1", "h-r2", "h-r3", "h-r4", "h-r3", "h-r4", "h-r1", "h-r2" };
            _flowerAniL2 = new string[8] { "h-o1", "h-o2", "h-o3", "h-o4", "h-o3", "h-o4", "h-o1", "h-o2" };
            _randomPos = new int[8];

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }
        #region 初始化和游戏开始方法

        private void GameInit()
        {
            talkIndex = 1;
            _level = 1;
            canClickBtn = true;

            for (int i = 0; i < _flower.childCount; i++)
            {
                GameObject o = _flower.GetGameObject(i.ToString());
                o.transform.SetSiblingIndex(i);
            }

            _flowerDrager = new mILDrager[_flower.childCount];
            _flowerAni = new GameObject[_flower.childCount];
            for (int i = 0; i < _flower.childCount; i++)
            {
                _flowerDrager[i] = _flower.GetChild(i).GetComponent<mILDrager>();
                _flowerAni[i] = _flower.GetChild(i).GetGameObject("ani");
            }

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
            InitAll();
            if(_isEnd)
                TweenFlower(0);

            _isEnd = false;
        }
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                    //田丁游戏开始方法
                    TDGameStartFunc();
                    break;
            }
            
            talkIndex++;
        }
        
        void TDGameStartFunc()
        {
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, 
            () => 
            { 
                mask.SetActive(false); 
                bd.SetActive(false);
                TweenFlower(0);
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
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            canClickBtn = true;
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (!canClickBtn)
                return;
            canClickBtn = false;
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); _isEnd = true; GameInit(); });
                }
                else if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); NextLevel(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2)); });
                }

            });
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
                            caidaiSpine.SetActive(false); 
                            successSpine.SetActive(false); 
                            ac?.Invoke();
                        });
                });
        }

        #endregion


        #endregion

        #region 拖拽

        void StartDrag()
        {
            for (int i = 0; i < _flowerDrager.Length; i++)
            {
                _flowerDrager[i].canMove = true;
                _flowerDrager[i].transform.GetComponent<Empty4Raycast>().raycastTarget = true;
                _flowerDrager[i].SetDragCallback(OnDrag, null, EndDrag, null);
            }
        }

        void StopDrag()
        {
            for (int i = 0; i < _flowerDrager.Length; i++)
            {
                _flowerDrager[i].canMove = false;
                _flowerDrager[i].transform.GetComponent<Empty4Raycast>().raycastTarget = false;
                _flowerDrager[i].SetDragCallback(null, null, null, null);
            }
        }
        private void OnDrag(Vector3 dragPos, int dragType, int dragIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
            _flowerDrager[dragIndex].transform.SetAsLastSibling();
        }

        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            _flowerDrager[dragIndex].transform.SetSiblingIndex(dragIndex);
            if (dragBool)
            {
                StopDrag();
                _flowerDrager[dragIndex].transform.gameObject.Hide();
                int i = _level == 1 ? 0 : 4;
                _inBlock.GetChild(dragIndex + i).gameObject.Show();

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                GameObject hua = _inBlock.GetChild(dragIndex + i).GetGameObject("hua");
                hua.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                hua.Show();
                SpineManager.instance.DoAnimation(hua, "h" + _level.ToString(), false, 
                () => 
                { 
                    hua.Hide();
                    _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    SpineManager.instance.DoAnimation(_xem, "xem2", false);
                    JudgeSuccess();
                });
            }
            else
            {
                StopDrag();
                _flowerDrager[dragIndex].transform.position = _posVector[_randomPos[dragIndex]];
                _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                SpineManager.instance.DoAnimation(_xem, "xem3", true);
                WaitTimeAndExcuteNext(1.0f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_xem, "xem", true);
                    StartDrag();
                });
            }
        }


        #endregion


        #region 游戏方法

        void RandomPos()
        {
            _randomPos = new int[9] { 0, 0, 0, 0, 0, 0, 0, 0, 0};
            for (int i = 0; i < _randomPos.Length; i++)
            {
                int random = 0;
                do
                {
                    random = Random.Range(0, 9);
                }
                while (_randomPos[random] != 0);

                _randomPos[random] = i;
            }
        }

        void InitAll()
        {
            RandomPos();

            for (int i = 0; i < _flowerAni.Length; i++)
            {
                GameObject o = _flowerAni[i];
                o.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(o, _level == 1 ? _flowerAniL1[i] : _flowerAniL2[i], true);
            }
            _tree.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_tree, "shu" + _level.ToString(), true);
            _t.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_t, "t" + _level.ToString(), false);
            _xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_xem, "xem", true);

            for (int i = 0; i < _flower.childCount; i++)
            {
                _flower.GetChild(i).gameObject.Show();
                _flower.GetChild(i).position = new Vector2(_posVector[_randomPos[i]].x, _posVector[_randomPos[i]].y + Screen.height);
            }

            for (int i = 0; i < _flowerDrager.Length; i++)
            {
                Transform trans = _flowerDrager[i].transform;
                trans.GetGameObject("1").Hide();
                trans.GetGameObject("2").Hide();
                trans.GetGameObject(_level.ToString()).Show();
            }

            for (int i = 0; i < _inBlock.childCount; i++)
            {
                _inBlock.GetChild(i).gameObject.Hide();
                _inBlock.GetChild(i).GetChild(0).gameObject.Hide();
            }

            StopDrag();
        }

        void TweenFlower(int i)
        {
            if(i < 8)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, i % 4, false);
                Tween tw = _flower.GetChild(i).DOMove(_posVector[_randomPos[i]], 3.0f);
                tw.SetEase(Ease.OutSine);
                WaitTimeAndExcuteNext(1.5f, 
                () => 
                {
                    TweenFlower(i + 1); 
                });
            }
            else
            {
                WaitTimeAndExcuteNext(2.0f, 
                () => 
                { 
                    DOTween.KillAll(); 
                    StartDrag(); 
                });
            }
        }

        void JudgeSuccess()
        {
            bool allShow = true;
            if (_level == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (!_inBlock.GetChild(i).gameObject.activeSelf)
                    {
                        allShow = false;
                        break;
                    }
                }
            }
            if (_level == 2)
            {
                for (int i = 4; i < 8; i++)
                {
                    if (!_inBlock.GetChild(i).gameObject.activeSelf)
                    {
                        allShow = false;
                        break;
                    }
                }
            }

            if (allShow)
            {
                WaitTimeAndExcuteNext(1.0f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_xem, "xem", true);
                    JudgeNextOrOver();
                });
            }
            else
                WaitTimeAndExcuteNext(1.0f,
                () =>
                {
                    SpineManager.instance.DoAnimation(_xem, "xem", true);
                    StartDrag();
                });
        }

        void JudgeNextOrOver()
        {
            if(_level == 1)
            {
                WaitTimeAndExcuteNext(2.0f,
                () =>
                {
                    mask.Show();
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(false);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                SpineManager.instance.DoAnimation(_xem, "xem4", true);
                WaitTimeAndExcuteNext(2.0f,
                () =>
                {
                    mask.Show();
                    playSuccessSpine();
                });
            }
        }

        void NextLevel()
        {
            _level++;
            InitAll();
            TweenFlower(0);
        }
        #endregion
    }
}
