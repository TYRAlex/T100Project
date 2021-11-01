using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course845Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;

        private GameObject _bell;       
        private Transform _btns;

        private GameObject _startBtn;
        private GameObject _startBtnSpine;
        private GameObject _mask;
        private GameObject _bai;
        private GameObject _hei;
        private RectTransform _heiRect;
        private Text _timeTxt;
        private Text _heiScoreTxt;
        private Text _baiSocreTxt;

        private int _time;
        private int _heiScore;
        private int _baiScore;


        private Image _maskImg;

        private List<string> _faGuangs;
        private List<string> _hongGuangs;
        private List<UserData> _userDatas;

        private List<int> _randomIndex;
        private int _curIndex;
        private bool _isGameOver;

        private bool _is6M;
        private bool _isPlaying;
        private bool _isSuccess;
        private GameObject _overGo;
        private GameObject _overSpine;
        private GameObject _overBtn;

        private Vector2 _heiInitPos;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var  curTrans = _curGo.transform;


            _bell = curTrans.GetGameObject("bell");
            _btns = curTrans.Find("Spines/Btns");
            _startBtn = curTrans.GetGameObject("StartBtn");
            _startBtnSpine = curTrans.GetGameObject("StartBtn/Spine");
            _mask = curTrans.GetGameObject("mask");
            _maskImg = curTrans.GetImage("mask");

            _bai = curTrans.GetGameObject("Spines/bai");
            _hei = curTrans.GetGameObject("Spines/hei");
            _heiRect = curTrans.GetRectTransform("Spines/hei");
            _timeTxt = curTrans.GetText("Bg/2/5/TimeTxt");
            _heiScoreTxt = curTrans.GetText("Bg/2/5/HeiTxt");
            _baiSocreTxt = curTrans.GetText("Bg/2/5/BaiTxt");

            _overGo = curTrans.GetGameObject("Over");
            _overSpine = curTrans.GetGameObject("Over/OverSpine");
            _overBtn = curTrans.GetGameObject("Over/OverBtn");

            GameInit();
            GameStart();
        }

        #region 固定

        private void InitData()
        {
            _time = 60;
            _heiScore = 0;
            _baiScore = 0;


            _timeTxt.text = _time.ToString();
            _heiScoreTxt.text = _heiScore.ToString();
            _baiSocreTxt.text = _baiScore.ToString();

            _is6M = false;
            _isPlaying = false;
            _isGameOver = false;
          
            _talkIndex = 1;
            _maskImg.color = new Color(1, 1, 1, 1);
            _startBtn.transform.GetEmpty4Raycast().raycastTarget = true;
            _hongGuangs = new List<string> { "h2","h1","h3","h4","h5","h6" };
            _faGuangs = new List<string> { "g2", "g1", "g3", "g4", "g5", "g6" };
            _userDatas = new List<UserData>();

            for (int i = 0; i < _btns.childCount; i++)
            {
                var child = _btns.GetChild(i);
                var spineGo = child.GetChild(0).gameObject;
                child.GetEmpty4Raycast().raycastTarget = false;
                PlaySpine(spineGo, child.name);

                string faGuangName = _faGuangs[i];
                string hongGuangName = _hongGuangs[i];
                string showBtnName = child.name;
                var userData = new UserData(faGuangName, showBtnName, hongGuangName);
                _userDatas.Add(userData);
            }

            RandomIndex();

            _heiInitPos = new Vector2(-43, 62);
            _heiRect.anchoredPosition = _heiInitPos;

            PlaySpine(_hei, "d", null, true);
            PlaySpine(_bai, "animation", null, true);
            StopAllCoroutines();
        }

        private void GameInit()
        {

            InitData();

            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
         
            StopAllAudio();

            _startBtn.Hide(); _bell.Show();_mask.Show();
                    
            _overGo.Hide(); _overBtn.Hide();
            AddEvent(_overBtn, OnClickOverBtn);
            AddEvent(_startBtn, OnClickStartBtn);
            AddEvents(_btns, OnClickBtn);
                  
        }

      

        void GameStart()
        {
            PlayCommonBgm(2);
            BellSpeck(_bell, 0, null, () => {_startBtn.Show();PlaySpine(_startBtnSpine, "ks"); });
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

        #endregion

        #region 游戏逻辑



        private void CountDown()
        {
            MyUpDate(1.0f, 
            () => {            
                _timeTxt.text = _time.ToString();
                if (_time <= 6 && _time >= 0&& !_is6M)
                {
                    _is6M = true;
                    PlayVoice(6);
                }
            },
            () => {
                _time--;
            },
            () => {
                _isGameOver = true;
                _mask.Show();
                _maskImg.DOColor(new Color(1, 1, 1, 1), 0f);
                Debug.LogError("游戏结束");
                _is6M = false;
            });
        }

        private void MyUpDate(float delay, Action callBack, Action callBack2, Action overCallBack)
        {
            _mono.StartCoroutine(IEMyUpDate(delay, callBack, callBack2,overCallBack));
        }

        IEnumerator IEMyUpDate(float delay,Action callBack, Action callBack2,Action overCallBack)
        {
            while (_time>=0)
            {
                callBack?.Invoke();
                yield return new WaitForSeconds(delay);
                callBack2?.Invoke();
                              
            }
            overCallBack?.Invoke();
            yield return null;
        }



        private void RandomIndex()
        {
            _randomIndex = new List<int>();

            while (true)
            {
                var index = Random.Range(0, _btns.childCount);
                var isContain= _randomIndex.Contains(index);
                if (!isContain)
                    _randomIndex.Add(index);

                if (_randomIndex.Count== _btns.childCount)
                    break;                
            }  
            
        }

        private void OnClickStartBtn(GameObject go)
        {
            _startBtn.transform.GetEmpty4Raycast().raycastTarget = false;
            PlayVoice(0);
            PlaySpine(_startBtnSpine, "ks2",()=> { _startBtn.Hide(); });
            _maskImg.DOColor(new Color(1, 1, 1, 0), 1.5f);
            BellSpeck(_bell, 1, null, () => { _bell.Hide(); _mask.Hide(); RandomShowBtn(); CountDown(); });
        }

        private void OnClickBtn(GameObject go)
        {
            if (_isPlaying)            
                return;

            _isPlaying = true;
            PlayVoice(7);
          
          
            var curUserData = _userDatas[_curIndex];
            curUserData.IsOnClickBtn = true;
            var spineGo = go.transform.GetChild(0).gameObject;
            var spineName = go.name[0] + "2";
            PlaySpine(spineGo, spineName,()=> {
                Delay(0.2f, () => { PlaySpine(spineGo, go.name); });
            });
        }


        private void OnClickOverBtn(GameObject go)
        {
            PlayOnClickSound();
            _overBtn.Hide();
            string str = string.Empty;
           
          


            if (_heiScore > _baiScore)   //成功
            {
                str = "cg3";
            }
            else if (_heiScore < _baiScore) //失败
            {
                str = "sb3";
            }
            else  //平局
            {
                str = "pj3";
            }

            PlaySpine(_overSpine, str, () => {
                _overGo.Hide();
                _mask.Hide();
                InitData();
                RandomShowBtn(); CountDown();                            
            });
        }

        private void GameOver()
        {
            _overGo.Show();
         

            

            string str1 = string.Empty;
            string str2 = string.Empty;



            if (_heiScore > _baiScore)   //成功
            {
                PlaySound(3);
                PlayVoice(1);
                str1 = "cg1"; str2 = "cg2";            
            }
            else if(_heiScore < _baiScore) //失败
            {
                PlaySound(2);
                PlayVoice(2);
                str1 = "sb1"; str2 = "sb2";
            }
            else  //平局
            {
                PlaySound(4);
                PlayVoice(9);
                str1 = "pj1"; str2 = "pj2";
            }

            PlaySpine(_overSpine, str1, () => { PlaySpine(_overSpine, str2, null, true); });
            Delay(1.0f, () => { _overBtn.Show(); });
        }

        private void RandomShowBtn()
        {
            if (_isGameOver)
            {
                GameOver();
                return;
            }
            
            _curIndex = CurIndex();         
            if (_curIndex==-1)
            {
                Debug.LogError("重新在随机一轮");
             
                RandomIndex();
                foreach (var item in _userDatas)               
                    item.IsOnClickBtn = false;
                RandomShowBtn();
            }
            else
            {
                var curUserData = _userDatas[_curIndex];
                var showBtnName = curUserData.ShowBtnName;
                var faGuangName = curUserData.FaGuangName;
                var hongGuangName = curUserData.HongGuangName;
               
                var btn = _btns.Find(showBtnName);
            
                var e4Ray = btn.GetEmpty4Raycast();
                e4Ray.raycastTarget = true;

                PlayVoice(3);
                PlaySpine(_bai, faGuangName,()=> { PlaySpine(_bai, faGuangName, ()=> { PlaySpine(_bai, "animation", null, true); });});

                Delay(1.5f, () =>
                {                  
                    e4Ray.raycastTarget = false;

                    var isOnClick = curUserData.IsOnClickBtn;

                    if (isOnClick)
                    {
                        PlayVoice(8);
                        _isPlaying = false;
                        _heiScore++;
                        _heiScoreTxt.text = _heiScore.ToString();

                        //d8 前进  d9后退  hei (-43,62)
                        switch (showBtnName)
                        {
                            case "11":    //头部  d3  (300,62)
                                HeiAttack("d3", hongGuangName);
                                break;
                            case "21":    //身体  d2  (300,62)
                                HeiAttack("d2", hongGuangName);
                                break;
                            case "31":    //左手   d4  (252,62)
                                HeiAttack("d4", hongGuangName, 252f);
                                break;
                            case "41":    //右手    d5  (300,62)
                                HeiAttack("d5", hongGuangName);
                                break;
                            case "51":     //左脚    d6  (300,62)
                                HeiAttack("d6", hongGuangName);
                                break;
                            case "61":     //右脚    d7  (300,62)
                                HeiAttack("d7", hongGuangName);
                                break;
                        }
                    }
                    else
                    {
                        _baiScore++;
                        _baiSocreTxt.text = _baiScore.ToString();

                        switch (showBtnName)
                        {
                            case "11":    //头部  d3  (300,62)
                                HeiAttackFail("adt2", "d3");
                                break;
                            case "21":    //身体  d2  (300,62)
                                HeiAttackFail("adshen", "d2");
                                break;
                            case "31":    //左手   d4  (252,62)
                                HeiAttackFail("ads3", "d4", 252f);
                                break;
                            case "41":    //右手    d5  (300,62)
                                HeiAttackFail("ads4", "d5");
                                break;
                            case "51":     //左脚    d6  (300,62)
                                HeiAttackFail("adj3", "d6");
                                break;
                            case "61":     //右脚    d7  (300,62)
                                HeiAttackFail("adj4", "d7");
                                break;
                        }
                    }
                });
            }
        }

      
        private void HeiAttack(string attackSpineName, string hongGuangSpineName, float moveX=300f)
        {
            PlaySpine(_hei, "d8", null, true);
            _heiRect.DOAnchorPosX(moveX, 2.5f);
            Delay(2.1f, () => {

                Delay(0.5f, () => { PlaySpine(_bai, hongGuangSpineName, () => { PlaySpine(_bai, "animation", null, true); }); });
                PlaySpine(_hei, attackSpineName, () => {
                 
                    PlaySpine(_hei, "d9", null, true);
                    _heiRect.DOAnchorPosX(_heiInitPos.x, 2.5f);
                    Delay(2.1f, () => {
                        PlaySpine(_hei, "d", null, true);
                        Delay(1.0f, RandomShowBtn);
                    });
                });
            });
        }



        private void HeiAttackFail( string avoidSpineName,string attackSpineName,float moveX =300f)
        {
            PlaySpine(_hei, "d8", null, true);
            PlayVoice(5);
            _heiRect.DOAnchorPosX(moveX, 2.5f);
            Delay(2.1f, () => {
                PlaySpine(_hei, attackSpineName,()=> {
                   
                    PlaySpine(_hei, "d9", null, true);
                    _heiRect.DOAnchorPosX(_heiInitPos.x, 2.5f);
                    Delay(2.1f, () => {
                        PlaySpine(_hei, "d", null, true);
                        Delay(1.0f, RandomShowBtn);
                    });
                });                                        
            });
            Delay(2.0f,()=>{
                PlaySpine(_bai, avoidSpineName,()=> { PlaySpine(_bai,"animation",null,true); });
            });


        }

       

        /// <summary>
        /// 当前随机的inde：返回-1游戏结束
        /// </summary>
        /// <returns></returns>
        private int CurIndex()
        {
            int index = -1;

            bool isOver = true;
            foreach (var item in _userDatas)
            {
                if (!item.IsOnClickBtn)
                {
                    isOver = false;
                    break;
                }
            }

            if (isOver)            
                return index;

            while (true)
            {
                index = Random.Range(0, _btns.childCount);
                var randomUserData = _userDatas[index];
                if (!randomUserData.IsOnClickBtn)
                    break;
            }

            return index;
          
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


    public class UserData
    {
        public string HongGuangName;
        public string FaGuangName;
        public string ShowBtnName;
        public bool IsOnClickBtn;
       
        public UserData(string faGuangName,string showBtnName,string hongGuangName)
        {
            FaGuangName = faGuangName;
            ShowBtnName = showBtnName;
            HongGuangName = hongGuangName;
            IsOnClickBtn = false;
        }

    }
}
