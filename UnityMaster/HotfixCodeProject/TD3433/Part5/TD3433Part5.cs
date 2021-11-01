using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD3433Part5
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

        private GameObject _dialogue;
        private GameObject _xueGo;


        private GameObject _xiaoEmo;
        private GameObject _sDD;


        private Transform _spines;
        private Transform _curSpineTra;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长


        private Vector2 _roleStartPos;
        private Vector2 _roleEndPos;

        private Vector2 _enemyStartPos;
        private Vector2 _enemyEndPos;

        private RectTransform _dingDing;
        private RectTransform _devil;

        private Text _dingDingTxt;
        private Text _devilTxt;

        private List<string> _dialogues;
        private List<int> _randomIndex;
        private int _curLevelId;

        private bool _isPlaying;
        private int _onClickNumMax;
        private int _curClickNums;

        private string _curShowLevelName;

        private GameObject _zui3;
        private GameObject _zui2;
        private GameObject _zui4;
        private GameObject _tg;

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
          
            _dDD = curTrans.GetGameObject("DDD");
            _sDD = curTrans.GetGameObject("sDD");
            _dingDing = curTrans.GetRectTransform("dialogue/dingDing");
            _devil = curTrans.GetRectTransform("dialogue/devil");

            _dingDingTxt = curTrans.GetText("dialogue/dingDing/Text");
            _devilTxt = curTrans.GetText("dialogue/devil/Text");

            _dialogue = curTrans.GetGameObject("dialogue");

            _spines = curTrans.Find("Spines");

            _xueGo = curTrans.GetGameObject("Bg/XEM");
            _xiaoEmo = curTrans.GetGameObject("xiaoemo");

            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = false;
            _dingDingTxt.text = string.Empty;
            _devilTxt.text = string.Empty;
            _onClickNumMax = 4;

            _dialogues = new List<string> {
                "我讨厌一切，是不会让你们得逞的！",
                "小朋友们，根据鲨鱼妈妈身体的颜色，点击并选择与身体颜色为相近色的牙齿",
                "即可成功解下鱼钩，拯救鲨鱼妈妈哦！"
            };
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = 0;
            _moveValue = 1920; _duration = 1.0f;

            _roleStartPos = new Vector2(-2170, 539); _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);

            SetPos(_devil, _enemyStartPos); SetPos(_dingDing, _roleStartPos);

            _curLevelId = 0;
            _randomIndex = new List<int>();
            RandomIndex();
        }

        void GameInit()
        {
            InitData();
         
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); 
             _dDD.Hide(); _dialogue.Hide();
            _spines.gameObject.Show(); _xiaoEmo.Hide(); _sDD.Hide();

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
         
            for (int i = 0; i < _spines.childCount; i++)
            {
                var level = _spines.GetChild(i);
                level.gameObject.Show();
                var spines = level.Find("Spines");
                var onClicks = level.Find("OnClicks");
            
                for (int j = 0; j < spines.childCount; j++)
                {
                    var spine = spines.GetChild(j);
                    spine.gameObject.Show();
                    spine.GetComponent<SkeletonGraphic>().Initialize(true);
                    if (spine.name == "zui-TG")
                    {
                        PlaySpine(spine.gameObject, "kong");
                        continue;
                    }

                    if (spine.name == "zui")
                        PlaySpine(spine.gameObject, spine.name, null, true);
                    else
                        PlaySpine(spine.gameObject, spine.name);
                }

                for (int z = 0; z < onClicks.childCount; z++)
                {
                    var onClick = onClicks.GetChild(z);
                    onClick.GetEmpty4Raycast().raycastTarget = true;                
                    AddEvent(onClick.gameObject, OnClickTooths);
                }
                level.gameObject.Hide();
            }

         //   _xueGo.GetComponent<SkeletonGraphic>().Initialize(true);
            PlaySpine(_xueGo, "kong2");
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); _dialogue.Show();

            ShowLevel();
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayCommonBgm(8);
                       _startSpine.Hide();
                        StartDialogue();
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
                 
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void ShowLevel()
        {

            if (_curLevelId==_randomIndex.Count)
            {              
                _spines.gameObject.Hide();
                GameSuccess();
                return;               
            }

            _curClickNums = 0;
            int index=_randomIndex[_curLevelId];
            Transform level = _spines.GetChild(index);
            Transform spines = level.Find("Spines");
            _curSpineTra = spines;
            _curShowLevelName = level.name;

            _zui2 = spines.Find("zui2").gameObject;
            _zui3 = spines.Find("zui3").gameObject;
            _tg = spines.Find("zui-TG").gameObject;
            _zui4 = spines.Find("zui4").gameObject;
            level.gameObject.Show();
                    
        }

        private void RandomIndex()
        {
            while (true)
            {
                var index = Random.Range(0, 3);
                var isContain = _randomIndex.Contains(index);

                if (!isContain)                
                    _randomIndex.Add(index);

                if (_randomIndex.Count == 3)
                    break;
            }
      
        }

        private void OnClickTooths(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            var name = go.name;
            var spine = _curSpineTra.Find(name).gameObject;
            var isCorrect = go.transform.childCount == 1;

            if (isCorrect)
            {
                go.transform.GetEmpty4Raycast().raycastTarget = false;
                var spineName = go.transform.GetChild(0).name;
                PlaySpine(spine, spineName);
                Delay(PlaySuccessSound(), () => {

                    _isPlaying = false;
                    _curClickNums++;
                    if (_curClickNums == _onClickNumMax)
                    {
                        _isPlaying = true;
                        _xueGo.GetComponent<SkeletonGraphic>().Initialize(true);
                        PlaySpine(_xueGo, "kong2");
                        _curLevelId++;
                      
                        Delay(0.05f, () => { _zui3.Hide(); _zui4.Hide(); }); 
                        PlayVoice(0);
                 
                          PlaySpine(_tg, _tg.name, () => {
                            _spines.gameObject.Hide();
                            _spines.Find(_curShowLevelName).gameObject.Hide();

                            PlaySpine(_xueGo, _xueGo.name, () =>
                            {
                                _spines.gameObject.Show();
                                _isPlaying = false;
                                ShowLevel();
                            });
                        });                                      
                    }                                                   
                });
            }
            else
            {
                PlaySpine(spine, name[0]+"2");
                Delay(PlayFailSound(),()=>{ _isPlaying = false; });
            }
        }

        /// <summary>
        /// 开始对话
        /// </summary>
        private void StartDialogue()
        {
            _xiaoEmo.Show();

            BellSpeck(_xiaoEmo, 0, null, () => {
                _xiaoEmo.Hide();
                _sDD.Show();
                BellSpeck(_sDD, 1, null, () => { StartGame(); _sDD.Hide(); }, false);
            });

        
            //SetMove(_devil, _enemyEndPos, 1.0f, () => {

            //    ShowDialogue(_dialogues[0], _devilTxt);
            //    Delay(PlaySound(0), DingDing);

            //});

            //void DingDing()
            //{
            //    SetMove(_dingDing, _roleEndPos, 1.0f, () => {
            //    ShowDialogue(_dialogues[1], _dingDingTxt);
            //    Delay(9.1f, () => {
            //        _dingDingTxt.text = string.Empty;
            //        ShowDialogue(_dialogues[2], _dingDingTxt);
            //    });
            //    Delay(PlaySound(1), ()=> {
            //        _dialogue.Hide();
            //        StartGame();
            //    });

            //    });
            //}
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();  
        }

        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show(); _okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                   
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();
                        PlayCommonBgm(8);
                        GameInit();
                        Delay(0.15f, ShowLevel);
                       // ();
                        StartGame();                    
                                
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound(); PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 2,null,null,false);
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

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
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

        #region 左右切换

        private void SlideSwitchPage(GameObject rayCastTarget, Action leftCallBack, Action rightCallBack)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
                    if (!isRight)
                        leftCallBack?.Invoke();
                    else
                        rightCallBack?.Invoke();
                }
            };
        }

        private void LeftSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex <= _pageMinIndex)
                return;
            _curPageIndex--;
            SetMoveAncPosX(rect, value, duration, callBack1, callBack2);
        }

        private void RightSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;
            _curPageIndex++;
            SetMoveAncPosX(rect, value, duration, callBack1, callBack2);
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

        private GameObject FindSpineGo(Transform parent, string goName)
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBD = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBD));
        }

      

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBD = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            if (isBD)
            {
                daiJi = "daiji"; speak = "speak";
            }
            else
            {
                daiJi = "animation"; speak = "animation2";
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
