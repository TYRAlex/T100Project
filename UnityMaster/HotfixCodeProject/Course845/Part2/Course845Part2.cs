using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course845Part2
    {

        private int _talkIndex;
        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _bell;


        private GameObject _fengMianBtn;
        private GameObject _leftBtn;
        private GameObject _rightBtn;

        private GameObject _bookSpine;
        private GameObject _diSpine;
        private GameObject _lastPageSpine;
        private GameObject _nextPageSpine;
        private GameObject _curPageSpine;
     


        //第1页
        private GameObject _fgSpine;
        private GameObject _fg2Spine;
        private GameObject _fg3Spine;
        private GameObject _fg4Spine;

        //第2页
        private GameObject _jianSpine;

        //第3页
        private GameObject _bai3Spine;
        private GameObject _hei3Spine;
        private Text _time3Txt;
        private Text _hei3Txt;
        private Text _bai3Txt;
        private int _hei3Score;
        private int _bai3Score;
        private int _time3;

        //第4页
        private GameObject _bai4Spine;
        private GameObject _hei4Spine;
        private GameObject _fs4Spine;
        private Text _time4Txt;
        private Text _hei4Txt;
        private Text _bai4Txt;
        private int _hei4Score;
        private int _bai4Score;
        private int _time4;

        //第5页
        private GameObject _j1Spine;
        private GameObject _j2Spine;
        private GameObject _j3Spine;

        private Transform _resultsTra;     
        private Transform _onClicksTra;

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

            _bookSpine = curTrans.GetGameObject("Spines/Book");
            _diSpine = curTrans.GetGameObject("Spines/di");

            _lastPageSpine = curTrans.GetGameObject("Spines/LastPage");
            _curPageSpine = curTrans.GetGameObject("Spines/CurPage");
            _nextPageSpine = curTrans.GetGameObject("Spines/NextPage");

        

            _fgSpine = curTrans.GetGameObject("Results/0/fg");
            _fg2Spine = curTrans.GetGameObject("Results/0/fg2");
            _fg3Spine = curTrans.GetGameObject("Results/0/fg3");
            _fg4Spine = curTrans.GetGameObject("Results/0/fg4");

            _jianSpine = curTrans.GetGameObject("Results/1/jian");

            _bai3Spine = curTrans.GetGameObject("Results/2/bai");
            _hei3Spine = curTrans.GetGameObject("Results/2/hei");
            _time3Txt = curTrans.GetText("Results/2/time");
            _hei3Txt = curTrans.GetText("Results/2/heiTxt");
            _bai3Txt = curTrans.GetText("Results/2/baiTxt");

            _bai4Spine = curTrans.GetGameObject("Results/3/bai");
            _hei4Spine = curTrans.GetGameObject("Results/3/hei");
            _fs4Spine = curTrans.GetGameObject("Results/3/fs");
            _time4Txt = curTrans.GetText("Results/3/time");
            _hei4Txt = curTrans.GetText("Results/3/heiTxt");
            _bai4Txt = curTrans.GetText("Results/3/baiTxt");

            _j1Spine = curTrans.GetGameObject("Results/4/j1");
            _j2Spine = curTrans.GetGameObject("Results/4/j2");
            _j3Spine = curTrans.GetGameObject("Results/4/j3");

            _resultsTra = curTrans.Find("Results");

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
                {0,new List<string>{"1","b1","2" } }, //比赛场地
                {1,new List<string>{ "3","b2","4"} },//致礼
                {2,new List<string>{ "5","b3","6"} },//得分规则
                {3,new List<string>{ "7","b4","8"} } //出界与犯规
            };

            _rightTurns = new Dictionary<int, List<string>>
            {
                {0,new List<string>{ "1","1","2"  }},  //比赛场地
                {1,new List<string>{ "1","a1","4" }},  //致礼
                {2,new List<string>{ "3","a2","6" }},  //得分规则
                {3,new List<string>{ "5","a3","8" }}, //出界与犯规
                {4,new List<string>{ "7","a4","10"} }  //三大剑种有效得分区               
            };

            var bai4Rect = _bai4Spine.GetComponent<RectTransform>();
            var hei4Rect = _hei4Spine.GetComponent<RectTransform>();
            bai4Rect.anchoredPosition = Vector2.zero;
            hei4Rect.anchoredPosition = Vector2.zero;

            _time3Txt.text = string.Empty;
            _bai3Txt.text = string.Empty;
            _hei3Txt.text = string.Empty;
            _bai3Score = 0;
            _hei3Score = 0;
            _time3 = 30;


            _time4Txt.text = string.Empty;
            _bai4Txt.text = string.Empty;
            _hei4Txt.text = string.Empty;
            _bai4Score = 0;
            _hei4Score = 0;
            _time4 = 30;

      
        }




        private void GameInit()
        {
            InitData();

            StopAllAudio();
            StopAllCoroutines();
            HideVoiceBtn();

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _fengMianBtn.Show(); _leftBtn.Hide(); _rightBtn.Hide();
            _diSpine.Hide();


            HideAllChildsSpine(_resultsTra, tra => {
                for (int i = 0; i < tra.childCount; i++)
                {
                    var childGo = tra.GetChild(i).gameObject;
                    var isSpineCom = childGo.GetComponent<SkeletonGraphic>() == null;
                    if (!isSpineCom)                   
                        PlaySpine(childGo, "kong");                                    
                }
            });


            PlaySpine(_lastPageSpine, "animation");
            PlaySpine(_curPageSpine, "animation");
            PlaySpine(_nextPageSpine, "animation");
          
            AddEvent(_fengMianBtn, OnClickFengMian);
            AddEvent(_leftBtn, OnClickLeftBtn);
            AddEvent(_rightBtn, OnClickRightBtn);

         
        }



        void GameStart()
        {
            PlayCommonBgm(2);
            BellSpeck(_bell, 0, null, () => { _isPlaying = false; });
            PlaySpine(_bookSpine, "animation", () => {
                PlaySpine(_bookSpine, "fengmian");
            });

        }


        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();

            if (_talkIndex == 1)
            {

            }

            _talkIndex++;
        }


        #region 游戏逻辑

   

        private Coroutine MyUpdate(bool isLevel3)
        {
            return _mono.StartCoroutine(IEMyUpdate(isLevel3));
        }

        private IEnumerator IEMyUpdate(bool isLevel3)
        {
            var time = 0;
            Text text = null;
            if (isLevel3)
            {
                time = _time3;
                text = _time3Txt;
            }
            else
            {
                time = _time4;
                text = _time4Txt;
            }

            while (time >= 0)
            {
                text.text = time.ToString();
                yield return new WaitForSeconds(1.0f);
                time--;
            }

            yield return null;
        }
       

        private void PlayCurPageSpine()
        {
            switch (_curPageIndex)
            {
                case 0:
                    BellSpeck(_bell, 1, null, () => { _isPlaying = false; });
                 
                    ShowChildsSpine(_resultsTra, 0);
                    Delay(1.8f, () => { PlayVoice(3); PlaySpine(_fgSpine, _fgSpine.name,null,true); });
                    Delay(4.1f, () => { PlaySpine(_fg2Spine, _fg2Spine.name, null, true); });
                    Delay(6.8f, () => { PlaySpine(_fg3Spine, _fg3Spine.name, null, true); });
                    Delay(8.0f, () => { PlaySpine(_fg4Spine, _fg4Spine.name, null, true); });               
                    break;
                case 1:
                    BellSpeck(_bell, 2, null, () => { _isPlaying = false; });
                    ShowChildsSpine(_resultsTra, 1);
                    PlaySpine(_jianSpine, _jianSpine.name);
                    break;
                case 2:
                    BellSpeck(_bell, 3, null, () => { _isPlaying = false; });
                    ShowChildsSpine(_resultsTra, 2);
                    PlayVoice(4);
                    _bai3Score = 0; 
                    _hei3Score = 0;
                    _time3 = 30;
                    _bai3Txt.text = _bai3Score.ToString();
                    _hei3Txt.text = _hei3Score.ToString();
                    _time3Txt.text = _time3.ToString();
                    MyUpdate(true);

                    PlaySpine(_hei3Spine, "d2", () => { _hei3Score++; _hei3Txt.text = _hei3Score.ToString();});
                    PlaySpine(_bai3Spine, "animation", null, true);
                    Delay(0.5f, () => { PlaySpine(_bai3Spine, "g1", () => { PlaySpine(_bai3Spine, "animation", null, true); }); });
                              
                    break;
                case 3:
                    BellSpeck(_bell, 4, null, () => { _isPlaying = false; });
                    ShowChildsSpine(_resultsTra, 3);
                    PlayVoice(5);
                  
                    _bai4Score = 0;
                    _hei4Score = 0;
                    _time4 = 30;
                    _bai4Txt.text = _bai4Score.ToString();
                    _hei4Txt.text = _hei4Score.ToString();
                    _time4Txt.text = _time4.ToString();
                    MyUpdate(false);

                    PlaySpine(_hei4Spine, "qianjin", null, true);
                    PlaySpine(_bai4Spine, "houtui", null, true);
                    _hei4Spine.GetComponent<RectTransform>().DOAnchorPosX(260, 2.6f).OnComplete(() => { PlaySpine(_hei4Spine, "d", null, true); });
                    _bai4Spine.GetComponent<RectTransform>().DOAnchorPosX(260, 2.6f).OnComplete(() => { PlaySpine(_bai4Spine, "animation", null, true); });
                    Delay(2, () => { _hei4Score++; _hei4Txt.text = _hei4Score.ToString(); });
                 
                    Delay(9.6f, () => { PlaySpine(_fs4Spine, "huangpai"); PlayVoice(6); });
                    Delay(13.6f, () => { PlayVoice(6); PlaySpine(_fs4Spine, "heipai",()=> { Delay(1f, () => { PlaySpine(_fs4Spine, "animation"); }); }); });
                   
                    break;
                case 4:
                    BellSpeck(_bell, 5, null, () => { _isPlaying = false; });
                    ShowChildsSpine(_resultsTra, 4);
                 
                    Delay(10.2f, () => { PlayVoice(7); PlaySpine(_j1Spine, _j1Spine.name,null,true); });
                    Delay(13.3f, () => { PlaySpine(_j2Spine, _j2Spine.name,null,true); });
                    Delay(16.5f, () => { PlaySpine(_j3Spine, _j3Spine.name,null,true); });                  
                    break;
            }
        }

        private void PlayLeftTurns(int curPageIndex, Action<bool, int> startTurn = null, Action callBack=null)
        {

            var isKey = _leftTurns.ContainsKey(curPageIndex);
            startTurn?.Invoke(isKey, curPageIndex + 1);
            if (!isKey)
            {
                PlayVoice(2);
                
                _diSpine.Hide();
                PlaySpine(_lastPageSpine, "animation");
                PlaySpine(_curPageSpine, "animation");
                PlaySpine(_nextPageSpine, "animation");
                _bookSpine.Show();
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
                PlayVoice(1);
                PlaySpine(_lastPageSpine, lastName);
                var time = PlaySpine(_curPageSpine, curName);             
                Delay(time, () => { callBack?.Invoke(); PlaySpine(_nextPageSpine, nextName); });
            }
        }

        private void PlayRightTurns(int curPageIndex, Action<bool, int> startTurn = null,Action callBack=null)
        {
            var isKey = _rightTurns.ContainsKey(curPageIndex);
            startTurn?.Invoke(isKey, curPageIndex - 1);
            if (!isKey)
            {
                PlayVoice(2);
               
                _diSpine.Hide();
                PlaySpine(_lastPageSpine, "animation");
                PlaySpine(_curPageSpine, "animation");
                PlaySpine(_nextPageSpine, "animation");
                _bookSpine.Show();
                PlaySpine(_bookSpine, "guanbi2",()=> { _isPlaying = false; });
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

                if (curPageIndex != 0)
                    PlayVoice(1);
            
                PlaySpine(_lastPageSpine, lastName);
                PlaySpine(_curPageSpine, curName, callBack);
                PlaySpine(_nextPageSpine, nextName);
            }
        }

        private void StartPlayTurn(bool isKey, int index)
        {
            if (isKey)
            {
                HideChildsSpine(_resultsTra, index, g => {
                    var tra = g.transform;
                    for (int i = 0; i < tra.childCount; i++)
                    {
                        var childGo = tra.GetChild(i).gameObject;
                        bool isSpine = childGo.GetComponent<SkeletonGraphic>() == null;
                        if (!isSpine)                        
                            PlaySpine(childGo, "kong");                                              
                    }                      
                });
            }
            else
            {
                HideAllChildsSpine(_resultsTra, tra => {
                    for (int i = 0; i < tra.childCount; i++)
                    {
                        var childGo = tra.GetChild(i).gameObject;
                        bool isSpine = childGo.GetComponent<SkeletonGraphic>() == null;
                        if (!isSpine)
                            PlaySpine(childGo, "kong");
                    }
                });
            }
        }

        private void OnClickFengMian(GameObject go)
        {
            if (_isPlaying)
                return;

            PlayVoice(0);

            _isPlaying = true;
            go.Hide();
            PlaySpine(_bookSpine, "dakai",()=> {             
                _leftBtn.Show();
                _rightBtn.Show();

                PlaySpine(_bookSpine,"animation");
                _diSpine.Show(); 
                PlayRightTurns(_curPageIndex);
                PlayCurPageSpine();
            });

        }

        private void OnClickLeftBtn(GameObject go)
        {
            if (_isPlaying)
                return;

        //    PlayOnClickSound();
            _isPlaying = true;
            _curPageIndex--;
         
            PlayLeftTurns(_curPageIndex, StartPlayTurn, PlayCurPageSpine);
        }

        private void OnClickRightBtn(GameObject go)
        {
            if (_isPlaying)
                return;
       //     PlayOnClickSound();
            _isPlaying = true;
            _curPageIndex++;
         
            PlayRightTurns(_curPageIndex,StartPlayTurn, PlayCurPageSpine);
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


        private void HideAllChildsSpine(Transform parent, Action<Transform> callBack = null)
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

        private void PlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            _mono.StartCoroutine(IEPlaySequenceSpine(go, spineNames));
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

        IEnumerator IEPlaySequenceSpine(GameObject go, List<string> spineNames)
        {
            for (int i = 0; i < spineNames.Count; i++)
            {
                var name = spineNames[i];
                var delay = PlaySpine(go, name);
                yield return new WaitForSeconds(delay);
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
