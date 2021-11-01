using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class TD8923Part7
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
        private GameObject _bD;
        private GameObject _dBD;

        private GameObject _xemSpine;
        private GameObject _xemFlashSpine;

        private Transform _fangZiTra;
        private Transform _fangziDropers;
        private Transform _bridTra;
       
        private Image _hP;

        private GameObject _curBrid;
        private Empty4Raycast _curBirdE4Raycast;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private List<int> _recordRandomIndex;
        private int _curBridIndex;

        private int _curPageIndex;  //当前页签索引
        private int _pageMinIndex;  //页签最小索引
        private int _pageMaxIndex;  //页签最大索引
        private Vector2 _prePressPos;

        private float _moveValue;
        private float _duration;    //切换时长

        private float _hpValue;

       
        private List<mILDroper> _mILDropers;

        private Vector2 _bridInitPos;
        private Vector2 _bridTreePos;

        private float _bridEndX;

        private Transform _xfsTra;
       

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
            _bD = curTrans.GetGameObject("BD");
            _dBD = curTrans.GetGameObject("DBD");

            _xemSpine = curTrans.GetGameObject("Spines/xem2");
            _xemFlashSpine = curTrans.GetGameObject("Spines/xem-flash");
            _fangZiTra = curTrans.Find("Spines/fangzi");
            _bridTra = curTrans.Find("Spines/brids");
            _fangziDropers = curTrans.Find("Spines/fangziDropers");

              _hP = curTrans.GetImage("Spines/Hps/Red");
            _xfsTra = curTrans.Find("Spines/xfs");
          
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _bridInitPos = new Vector2(300, -231);
            _bridTreePos = new Vector2(-187, -231);
            _bridEndX = -2300f;

            _recordRandomIndex = new List<int>();
            _succeedSoundIds = new List<int> { 4, 5, 6,7, 8,9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curPageIndex = 0; _pageMinIndex = 0; _pageMaxIndex = 0;
            _moveValue = 1920; _duration = 1.0f;
            _hP.fillAmount = 1;
            _hpValue = 0.15f;
            _curBridIndex = 0;

        }

        void GameInit()
        {
            InitData();
           RandomBridsIndex();
            _talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio(); StopAllCoroutines();
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _xemFlashSpine.Hide();
            _bD.Hide(); _dBD.Hide(); HideAllChilds(_xfsTra);
            _xemSpine.Show();
            InitSpines(_fangZiTra, false); HideAllChilds(_bridTra);
            Delay(PlaySpine(_xemSpine, "kong"), () => { PlaySpine(_xemSpine, _xemSpine.name, null, true); });
           

           var tempMILDrag=  SetDragerCallBack(_bridTra, DragStart, null, DragEnd);

            foreach (var item in tempMILDrag)            
                item.DoReset();
            
            _mILDropers = SetDroperCallBack(_fangziDropers, DropFail);

           

            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
        }

       

        void GameStart()
        {
            _mask.Show(); _startSpine.Show();
          //  
            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine); ShowBrid();
                    PlaySpine(_startSpine, "bf", () => {
                        PlayCommonBgm(8);
                        _bD.Show(); _startSpine.Hide();
                        BellSpeck(_bD, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); });
                    });
                });
            });
        }


        void TalkClick()
        {
            PlayOnClickSound();
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_bD,1, null, StartGame);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _bD.Hide(); _mask.Hide();        
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
                        StartGame();
                        ShowBrid();
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
                        _dBD.Show();
                        BellSpeck(_dBD, 2);
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
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
        }


        private void RandomBridsIndex()
        {
           
            while (true)
            {
                int index = Random.Range(0, 5);
                var isExist = _recordRandomIndex.Contains(index);

                if (!isExist)                
                    _recordRandomIndex.Add(index);

                if (_recordRandomIndex.Count == 5)
                    break;
                
            }       
        }

        private void ShowBrid()
        {

            if (_curBridIndex >= _recordRandomIndex.Count)
            {
                Delay(2, () => {
                    GameSuccess();
                    Delay(4f, GameReplayAndOk);
                });
                //EnemyDie(()=> {
                   
                //});          
                return;
            }

         
            var index = _recordRandomIndex[_curBridIndex];
          
            ShowChilds(_bridTra, index, (go)=> {
                var child = go.transform.GetChild(0).gameObject;
                _curBrid = go;
                var rect = go.transform.GetRectTransform();
                _curBirdE4Raycast = go.GetComponent<Empty4Raycast>();
                PlaySpine(child, child.name, null, true);
                _curBirdE4Raycast.raycastTarget = false;
                StopAudio(SoundManager.SoundType.VOICE);
                Delay(0.5f, () => { PlayVoice(2, true); });
                rect.DOAnchorPos(_bridTreePos, 2f).SetEase(Ease.Linear)
                .OnComplete(()=> {
                    StopAudio(SoundManager.SoundType.VOICE);
                    PlayVoice(1, true);
                    var spineName = "m-a" + child.name[child.name.Length - 1];
                    PlaySpine(child, spineName, null, true);
                    _curBirdE4Raycast.raycastTarget = true;
                });
                 _curBridIndex++;
            });
        }

        private void DragStart(Vector3 pos, int type, int index)
        {
            var spineGo = _curBrid.transform.GetChild(0).gameObject;
            var spineName = spineGo.name.Substring(0,2)+"f" + spineGo.name[spineGo.name.Length-1];
            PlaySpine(spineGo, spineName, null, true);
            StopAudio(SoundManager.SoundType.VOICE);

            PlayVoice(2,true);
           
        }

        private void DragEnd(Vector3 pos, int type, int index, bool isMatch)
        {
            if (isMatch)
            {
                var droperName = GetDroperName(type);
              
                var spineGo= FindSpineGo(_fangZiTra, droperName);
                var spineName = spineGo.name.Substring(0, 3);              
                PlaySpine(spineGo, spineName);
                var curBridGo = _curBrid.transform.GetChild(0).gameObject;
                PlaySpine(curBridGo, "m-f0",null,true);
                _curBirdE4Raycast.raycastTarget = false;

               
                var rect = _curBrid.transform.GetRectTransform();
                var curXF = _xfsTra.Find(_curBrid.name);
                curXF.transform.GetRectTransform().anchoredPosition = rect.anchoredPosition;
                curXF.gameObject.Show();
                var xfGo = curXF.GetChild(0).gameObject;          
                PlaySpine(xfGo, xfGo.name,()=> { curXF.gameObject.Hide(); });

              
                //飞出屏幕
                rect.DOAnchorPosX(_bridEndX, 2f).SetEase(Ease.Linear)
                    .OnComplete(()=> {
                    StopAudio(SoundManager.SoundType.VOICE);
                    _curBrid.Hide();
                    _curBrid.GetComponent<mILDrager>().DoReset();
                    _curBirdE4Raycast.raycastTarget = true;
                      
                    Delay(1, ShowBrid);
                }); ;

                PlaySuccessSound();
               
                UpDataHp();

             
               
            }
        }

        private void DropFail(int type)
        {
            _curBirdE4Raycast.raycastTarget = false;
            var time = PlayFailSound();
            var droperName = GetDroperName(type);
            var spineGo = FindSpineGo(_fangZiTra, droperName);
            var spineName = spineGo.name.Substring(0, 3)+"3";
            PlaySpine(spineGo, spineName);
            Delay(time, () => {
                _curBirdE4Raycast.raycastTarget = true;
            });

        }

        private string GetDroperName(int type)
        {
            string name = string.Empty;
            foreach (var item in _mILDropers)
            {
                if (item.dropType==type)
                {
                    name = item.name;
                    break;
                }
            }
            return name;
        }

        private void UpDataHp()
        {
            _hP.fillAmount = _hP.fillAmount - _hpValue;
            _xemFlashSpine.Show();
            Delay(0.6f, () => { _xemSpine.Hide(); });
            Delay(0.2f, () => { PlayVoice(0); });
            Delay(0.7f, () => { PlaySound(3); });
            PlaySpine(_xemFlashSpine, _xemFlashSpine.name);
            Delay(1.2f, () => { _xemSpine.Show();

                if (_curBridIndex >= _recordRandomIndex.Count)
                    PlaySpine(_xemSpine, "xem-y2",null,true);
                else
                    PlaySpine(_xemSpine, _xemSpine.name,null,true);
            });         
        }

        private void EnemyDie(Action callBack=null)
        {
            PlayVoice(0);
            PlaySpine(_xemSpine,"xem-y2",()=> {
                _xemSpine.Hide();
            });

            var time =  PlaySound(3);
            Delay(time, callBack);
        }

        #endregion

        #region 常用函数

        #region 隐藏和显示

        private void HideAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Hide();
        }

        private void ShowChilds(Transform parent,int index,Action<GameObject> callBack=null)
        {
            var go= parent.GetChild(index).gameObject;
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
        private List<mILDrager> SetDragerCallBack(Transform parent,Action<Vector3,int,int> dragStart=null, Action<Vector3,int,int> draging=null, Action<Vector3,int,int,bool> dragEnd=null, Action<int> onClick=null)
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
        private List<mILDroper> SetDroperCallBack(Transform parent,Action<int> failCallBack=null)
        {
            var temp = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var droper = parent.GetChild(i).GetComponent<mILDroper>();
                temp.Add(droper);
                droper.SetDropCallBack(null,null, failCallBack);
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
            var time= SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
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
                daiJi = "bd-daiji"; speak = "bd-speak";
            }
            else
            {
                daiJi = "daiji"; speak = "speak";
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
