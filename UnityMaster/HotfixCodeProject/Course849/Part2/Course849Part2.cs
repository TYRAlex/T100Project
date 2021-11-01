using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course849Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _bell;

        private GameObject _fengMianBtn;
        private GameObject _leftBtn;
        private GameObject _rightBtn;

        private GameObject _diSpine;
        private GameObject _lastPageSpine;
        private GameObject _nextPageSpine;
        private GameObject _curPageSpine;
        private GameObject _bookSpine;

        private GameObject _e1Spine;
        private GameObject _aSpine;
        private GameObject _bSpine;
        private GameObject _cSpine;
        private GameObject _dSpine;
        private GameObject _d2Spine;
        private Transform _onClicksTra;
        private Transform _resultsTra;
        private GameObject _lankuangGo;

        private int _curPageIndex;


        private bool _isPlaying;

        private Dictionary<int, List<string>> _rightTurns;  //value顺序 last,cur,next
        private Dictionary<int, List<string>> _leftTurns;


        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;


            _bell = curTrans.GetGameObject("bell");

            _fengMianBtn = curTrans.GetGameObject("OnClicks/fengmian");
            _leftBtn = curTrans.GetGameObject("OnClicks/left");
            _rightBtn = curTrans.GetGameObject("OnClicks/right");

            _onClicksTra = curTrans.Find("OnClicks");
            _resultsTra = curTrans.Find("Results");

            _diSpine = curTrans.GetGameObject("Spines/di");
            _bookSpine = curTrans.GetGameObject("Spines/Book");
            _lastPageSpine = curTrans.GetGameObject("Spines/LastPage");
            _curPageSpine = curTrans.GetGameObject("Spines/CurPage");
            _nextPageSpine = curTrans.GetGameObject("Spines/NextPage");
            _e1Spine = curTrans.GetGameObject("Results/0/e1");
            _aSpine = curTrans.GetGameObject("Results/1/a");
            _bSpine = curTrans.GetGameObject("Results/2/b");
            _cSpine = curTrans.GetGameObject("Results/3/c");
            _dSpine = curTrans.GetGameObject("Results/4/d");
            _d2Spine = curTrans.GetGameObject("Results/4/d2");
            _lankuangGo = curTrans.GetGameObject("Results/2/lankuang");

            GameInit();
            GameStart();
        }




        private void InitData()
        {
            _talkIndex = 1;
            _isPlaying = true;
            _curPageIndex = 0;

            _leftTurns = new Dictionary<int, List<string>>
            {
                {0,new List<string>{"1","b1","2" } }, // 篮球场平面图
                {1,new List<string>{ "3","b2","4"} },//球员位置分布图
                {2,new List<string>{ "5","b3","6"} }, //球员投篮图
                {3,new List<string>{ "7","b4","8"} } //比赛计时规则画面
            };

            _rightTurns = new Dictionary<int, List<string>>
            {
                {0,new List<string>{ "1","1","2"  } },   // 篮球场平面图
                {1,new List<string>{ "1","a1","4" } },  //球员位置分布图
                {2,new List<string>{ "3","a2","6" } },  //球员投篮图
                {3,new List<string>{ "5","a3","8" } },  //比赛计时规则画面
                {4,new List<string>{ "7","a4","10"} }   //加时赛画面
            };

        }


        private void GameInit()
        {
            InitData();


            StopAllAudio(); StopAllCoroutines();
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _bell.Show();
            _fengMianBtn.Show(); _leftBtn.Hide(); _rightBtn.Hide();
            _diSpine.Hide(); ;

            HideAllChildsSpine(_resultsTra, tra => {
                for (int i = 0; i < tra.childCount; i++)
                {
                    var childGo = tra.GetChild(i).gameObject;
                    PlaySpine(childGo, "kong");
                }
            });

            PlaySpine(_lastPageSpine, "kong");
            PlaySpine(_curPageSpine, "kong");
            PlaySpine(_nextPageSpine, "kong");

            AddEvent(_fengMianBtn, OnClickFengMian);
            AddEvent(_leftBtn, OnClickLeftBtn);
            AddEvent(_rightBtn, OnClickRightBtn);

        }





        void GameStart()
        {
            PlayCommonBgm(2);
            BellSpeck(_bell, 0, null, () => { _isPlaying = false;_bell.Hide(); });

            PlaySpine(_bookSpine, "kong", () => {   PlaySpine(_bookSpine, "fengmian"); });

        }

        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();

            switch (_talkIndex)
            {
                case 1:
                   
                    break;
               
            }
            _talkIndex++;
        }


        #region 游戏逻辑

        private void OnClickFengMian(GameObject go)
        {
            if (_isPlaying)
                return;

        
            PlayVoice(0);
            _isPlaying = true;
            go.Hide();
            PlaySpine(_bookSpine, "dakai", () => {
                _leftBtn.Show();
                _rightBtn.Show();
                PlaySpine(_bookSpine, "kong");
                _diSpine.Show();
                PlayRightTurns(_curPageIndex);
                PlayCurPageSpine();
            });
        }

        private void OnClickLeftBtn(GameObject go)
        {
            if (_isPlaying)
                return;
         
            _isPlaying = true;
            _curPageIndex--;
            Debug.LogError("左index：" + _curPageIndex);
            PlayLeftTurns(_curPageIndex, StartPlayTurn, PlayCurPageSpine);
        }

        private void OnClickRightBtn(GameObject go)
        {
            if (_isPlaying)
                return;
          
            _isPlaying = true;
            _curPageIndex++;
            Debug.LogError("右index：" + _curPageIndex);
            PlayRightTurns(_curPageIndex,StartPlayTurn,PlayCurPageSpine );
        }

        private void PlayLeftTurns(int curPageIndex, Action<bool,int> startTurn = null, Action callBack = null)
        {
            var isKey = _leftTurns.ContainsKey(curPageIndex);
            startTurn?.Invoke(isKey, curPageIndex+1);
            if (!isKey)
            {
                PlayVoice(8);
                Debug.LogError("左翻结束");
                _diSpine.Hide();
                PlaySpine(_lastPageSpine, "kong");
                PlaySpine(_curPageSpine, "kong");
                PlaySpine(_nextPageSpine, "kong");
          
                PlaySpine(_bookSpine, "guanbi", () => { _isPlaying = false; });
                _leftBtn.Hide();
                _rightBtn.Hide();
                _fengMianBtn.Show();
                _curPageIndex = 0;
            }
            else
            {

                var value = _leftTurns[curPageIndex];
                var lastName = value[0];
                var curName = value[1];
                var nextName = value[2];
                PlayVoice(9);
                PlaySpine(_lastPageSpine, lastName);
                var time = PlaySpine(_curPageSpine, curName);
                Delay(time, () => { callBack?.Invoke(); PlaySpine(_nextPageSpine, nextName); });
            }
        }

        private void PlayRightTurns(int curPageIndex, Action<bool,int> startTurn=null, Action callBack = null)
        {
            var isKey = _rightTurns.ContainsKey(curPageIndex);
            startTurn?.Invoke(isKey,curPageIndex-1);
            if (!isKey)
            {
                PlayVoice(8);
                Debug.LogError("右翻结束");
                _diSpine.Hide();
                PlaySpine(_lastPageSpine, "kong");
                PlaySpine(_curPageSpine, "kong");
                PlaySpine(_nextPageSpine, "kong");
               
                PlaySpine(_bookSpine, "guanbi2", () => { _isPlaying = false; });
                _leftBtn.Hide();
                _rightBtn.Hide();
                _fengMianBtn.Show();
                _curPageIndex = 0;
            }
            else
            {
                var value = _rightTurns[curPageIndex];
                var lastName = value[0];
                var curName = value[1];
                var nextName = value[2];

                if (curPageIndex!=0)              
                PlayVoice(9);

                PlaySpine(_lastPageSpine, lastName);
                PlaySpine(_curPageSpine, curName, callBack);
                PlaySpine(_nextPageSpine, nextName);
            }
        }

        private void StartPlayTurn(bool isKey,int index)
        {
            if (isKey)
            {
                HideChildsSpine(_resultsTra, index, g => {
                    var tra = g.transform;
                    for (int i = 0; i < tra.childCount; i++)
                        PlaySpine(tra.GetChild(i).gameObject, "kong");
                });         
            }
            else
            {
                HideAllChildsSpine(_resultsTra, tra => {
                    for (int i = 0; i < tra.childCount; i++)
                    {
                        var childGo = tra.GetChild(i).gameObject;
                        PlaySpine(childGo, "kong");
                    }
                });
            }
        }

        private void PlayCurPageSpine()
        {
            switch (_curPageIndex)
            {
                case 0:
                    BellSpeck(_bell, 1, null, () => { _isPlaying = false; });
                    ShowChildsSpine(_resultsTra, 0); 
                    Delay(1f, () => {
                        PlaySpine(_e1Spine,_e1Spine.name);
                        PlaySequenceVoice(new List<float> { 0f,1.5f,1.0f,1.5f,2.0f }, 1);                                   
                    });
                   
                    break;
                case 1:
                    BellSpeck(_bell, 2, null, () => { _isPlaying = false; });                  
                    ShowChildsSpine(_resultsTra, 1);
                    var delays = new List<float> { 7.4f, 1f, 1f, 1f, 1f };
                    PlaySequenceSpine(_aSpine, delays, new List<string> { "a1", "a2", "a3","a4","a5" });
                    PlaySequenceVoice(delays, 2);
                    break;
                case 2:
                    BellSpeck(_bell, 3);
                    _lankuangGo.Hide();
                    ShowChildsSpine(_resultsTra, 2);
                    Delay(3.5f, () => {  _lankuangGo.Show(); });
                    Delay(5.0f, () => { _lankuangGo.Hide(); });
                    Delay(8.0f, () => {  _lankuangGo.Show(); });
                    Delay(10f, () => { _lankuangGo.Hide(); });
                    Delay(12f, () => { _lankuangGo.Show(); });
                
                    PlaySequenceSpine(_bSpine, new List<string> { "b3","b1","b4"},()=> { _isPlaying = false; });
                    Delay(PlayVoice(3), () => { Delay(1.2f, () => { PlayVoice(4); });  });
                    Delay(5.2f, () => { Delay(PlayVoice(3), () => { Delay(1.2f, () => { PlayVoice(4); }); }); });
                    Delay(9.2f, () => { Delay(2.8f, () => { PlayVoice(4); }); });
                    
                    break;
                case 3:
                    BellSpeck(_bell, 4);                   
                    ShowChildsSpine(_resultsTra, 3);
                    PlayVoice(5);
                    PlaySequenceSpine(_cSpine, new List<string> { "c2","c3" }, () => { _isPlaying = false; });
                    break;
                case 4:
                    BellSpeck(_bell, 5, null, () => { _isPlaying = false; });                                    
                    ShowChildsSpine(_resultsTra, 4);
                    PlaySpine(_dSpine, "d5");
                    Delay(2.9f, () => { PlayVoice(6); PlaySequenceSpine(_d2Spine,new List<string>{"d2","d4" }); });
                    Delay(5.3f, () => { PlayVoice(7); PlaySpine(_dSpine, "d6"); });
                    break;
            }
        }

        #endregion

        #region 常用函数

        #region 语音键显示隐藏
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

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

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

        private void HideAllChildsSpine(Transform parent,Action<Transform>callBack=null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i);
                child.GetComponent<CanvasGroup>().alpha = 0;
                callBack?.Invoke(child);
            }
        }

        private void HideChildsSpine(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.GetComponent<CanvasGroup>().alpha = 0;
            callBack?.Invoke(go);
        }

        private void ShowChildsSpine(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.GetComponent<CanvasGroup>().alpha = 1;
            callBack?.Invoke(go);
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

        private void PlaySequenceSpine(GameObject go, List<string> spineNames, Action callBack = null)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames, callBack));
        }
        private void PlaySequenceSpine(GameObject go, List<float> times, List<string> spineNames, Action callBack = null)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, times,spineNames, callBack));
        }

        private void PlaySequenceVoice(List<float> times,int index,Action callBack=null)
        {
            _mono.StartCoroutine(IEPlaySequenceVoice(times, index, callBack));
        }
        #endregion

        #region 音频相关



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

        private Coroutine UpDate(bool isStart, float delay, Action callBack)
        {
            return _mono.StartCoroutine(IEUpdate(isStart, delay, callBack));
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

        IEnumerator IEPlaySequenceSpine(GameObject go, List<float> times,List<string> spineNames, Action callBack = null)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = times[i];
                yield return new WaitForSeconds(delay);
                PlaySpine(go, name);
               
            }
            callBack?.Invoke();
        }

        IEnumerator IEPlaySequenceVoice(List<float> times,int index,Action callBack=null)
        {
            for (int i = 0; i < times.Count; i++)
            {
                var delay = times[i];
                yield return new WaitForSeconds(delay);
                PlayVoice(index);
            }
            callBack?.Invoke();
        }

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames, Action callBack = null)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
            }
            callBack?.Invoke();
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, bool isBell = true, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, isBell));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, bool isBell = true, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            if (isBell)
            {
                daiJi = "DAIJI"; speak = "DAIJIshuohua";
            }
            else
            {
                Debug.LogError("Role Spine Name...");
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

        #endregion
    }
}
