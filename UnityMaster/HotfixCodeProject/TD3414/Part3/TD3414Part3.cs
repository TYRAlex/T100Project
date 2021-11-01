using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;
using Spine.Unity;

namespace ILFramework.HotClass
{

    public class TD3414Part3
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        GameObject _curGo;

        private GameObject _bd;
        private GameObject _bD;
        private GameObject _mask;
        private GameObject _kP;
        private Transform _spines;
        
        private Dictionary<string, List<float>> _line1InitPosDic;   //第一行元素数据 List[0]是X值，List[1]是Y值 ,List[2]是缩放，List[3]是层级顺序，List[4]是飞X，List[5]是飞Y
        private Dictionary<string, List<float>> _line2InitPosDic;   //第二行元素数据 List[0]是X值，List[1]是Y值 ,List[2]是缩放，List[3]是层级顺序，List[4]是飞X，List[5]是飞Y
        private Dictionary<string, Coroutine> _moveCorDic;

        private Dictionary<int, List<string>> _levelDic;

        private List<int> _successSoundId;
        private List<int> _failSoundIds;

        private float _line1StartX;
        private float _line1EndX;

        private float _line2StartX;
        private float _line2EndX;
        private int _curLevelId;
        private float _moveSpeed;


        private Transform _level1Tra;
        private Transform _level2Tra;
        private Transform _level3Tra;

        private GameObject _xemGo;
        private GameObject _jueseGo;

        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;

        private GameObject _blackMaskGo;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _dialogue;
        private bool _isPause; //是否暂停
        private int _isFinish;

        private Text _tianTianTxt;
        private Text _devilTxt;

        private RectTransform _tianTian;
        private RectTransform _devil;

        private List<string> _dialogues;

        private Vector2 _roleStartPos;
        private Vector2 _roleEndPos;

        private Vector2 _enemyStartPos;
        private Vector2 _enemyEndPos;
        private GameObject _hPsGo;
        private Transform _hpTra;
        private bool _isPlaying;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
            _bd = curTrans.GetGameObject("bd");
            _bD = curTrans.GetGameObject("BD");
            _mask = curTrans.GetGameObject("mask");
            _spines = curTrans.Find("Spines");
            _hPsGo = curTrans.GetGameObject("Hps");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
            _dialogue = curTrans.GetGameObject("dialogue");
            _level1Tra = curTrans.Find("Levels/Level1");
            _level2Tra = curTrans.Find("Levels/Level2");
            _level3Tra = curTrans.Find("Levels/Level3");

            _kP = curTrans.GetGameObject("kp");
            _xemGo = curTrans.GetGameObject("Levels/xem");
            _jueseGo = curTrans.GetGameObject("Levels/juese");

            _blackMaskGo = curTrans.GetGameObject("blackMask");
            _successSpine = curTrans.GetGameObject("SuccessSpine");
            _spSpine = curTrans.GetGameObject("SuccessSpine/SpSpine");

            _tianTianTxt = curTrans.GetText("dialogue/tianTian/Text");
            _devilTxt = curTrans.GetText("dialogue/devil/Text");

            _tianTian = curTrans.GetRectTransform("dialogue/tianTian");
            _devil = curTrans.GetRectTransform("dialogue/devil");

            _hpTra = curTrans.Find("Hps/Hp");

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            _isPlaying = false;
            _tianTianTxt.text = string.Empty; _devilTxt.text = string.Empty;
           _isPause = false;
            StopAllAudio();
            StopAllCoroutines();
            _successSoundId = new List<int> { 4, 5, 7, 8, 9 }; _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _xemGo.Hide();  _jueseGo.Show(); _blackMaskGo.Show(); _startSpine.Show(); _kP.Hide();
            _successSpine.Hide(); _bd.Hide(); _bD.Hide();_replaySpine.Hide(); _okSpine.Hide(); _hPsGo.Hide();

            PlaySpine(_xemGo, "kong", () => { PlaySpine(_xemGo, _xemGo.name,null,true); });
            PlaySpine(_jueseGo, "kong", () => { PlaySpine(_jueseGo, _jueseGo.name); });

            SetParentToSpines(_level1Tra);
            SetParentToSpines(_level2Tra);
            SetParentToSpines(_level3Tra);
            InitData();
            InitSpine();
            InitLevelDic();
            _talkIndex = 1; _curLevelId = 1; _isFinish = 0;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            AddBtnsEvent(_spines, OnClick);
            AddEvent(_replaySpine, OnClickReplay);
            AddEvent(_startSpine, OnClickStart);
            AddEvent(_okSpine, OnClickOk);
          



        }
  
        void PlayXing()
        {
            _kP.Show(); PlayVoice(3);
            PlaySpine(_kP, "kong",()=> {
                PlaySpine(_kP, _kP.name);
            });
        }

        void GameStart()
        {
            _dialogue.Show();
            PlaySpine(_startSpine, "bf2");       
        }

        /// <summary>
        /// 开始情景对话
        /// </summary>
        private void StartDialogue()
        {
            SetMove(_devil, _enemyEndPos, 1.0f, () => {

                ShowDialogue(_dialogues[0], _devilTxt);
                RoleSpeck(0, TianDing);
            });

            //田丁
            void TianDing()
            {
                SetMove(_tianTian, _roleEndPos, 1.0f, () => {

                    ShowDialogue(_dialogues[1], _tianTianTxt);
                    RoleSpeck(1, StartDialogueOver);
                });
            }
        }

        /// <summary>
        /// 情景对话结束
        /// </summary>
        private void StartDialogueOver()
        {
            _dialogue.Hide();
            _bd.Show();
            BellSpeck(2, _bd, () => { _mask.Show(); },()=> { _bd.Hide(); _mask.Hide();_blackMaskGo.Hide();

                _isPlaying = true;
                var time =  PlaySound(3);
                Delay(time, () => { _isPlaying = false; });
                _xemGo.Show(); _hPsGo.Show();
                PlaySpine(_xemGo, "kong", () => { PlaySpine(_xemGo, _xemGo.name, null, true); });
            });
        }

        void OnClickReplay(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
            PlaySpine(_replaySpine, "fh", () => {
                PlayBgm(2, true, SoundManager.SoundType.COMMONBGM);
                GameInit();
                _okSpine.Hide(); _replaySpine.Hide();
                InitMoveCorDic();
                ShowLevel(_curLevelId);
                _blackMaskGo.Hide(); _startSpine.Hide(); PlaySound(3);_hPsGo.Show();
                _xemGo.Show();
                PlaySpine(_xemGo, "kong", () => { PlaySpine(_xemGo, _xemGo.name, null, true); });

            }); 
        }

        void OnClickStart(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_startSpine);
            PlaySpine(_startSpine, "bf", () => {
                PlayBgm(2,true, SoundManager.SoundType.COMMONBGM);
                StartDialogue();
                InitMoveCorDic();
                ShowLevel(_curLevelId);
                _startSpine.Hide();      
            });
        }

        void OnClickOk(GameObject go)
        {
            PlayOnClickSound();
            RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
          //  PlaySpine(_replaySpine, "fh");
            var time = PlaySpine(_okSpine, "ok");

            Delay(time, () => {
                _replaySpine.Hide(); _okSpine.Hide();
                _bD.Show(); BellSpeck(8, _bD);
                PlayBgm(4, true, SoundManager.SoundType.COMMONBGM);
            });
        }

        void InitSpine()
        {
            for (int i = 0; i < _spines.childCount; i++)
            {
                var spineParent = _spines.GetChild(i);
                InitSpines(spineParent);
            }
        }

        void InitLevelDic()
        {
            _levelDic = new Dictionary<int, List<string>>();
            _levelDic.Add(1, new List<string> { "g-a","g-b","g-c","g-d"});        //公主
            _levelDic.Add(2, new List<string> { "w-a","w-b","w-c","w-d"});            //王子
            _levelDic.Add(3, new List<string> { "m-a","m-b","m-c","m-d" });
        }

        void ShowLevel(int levelId)
        {
            switch (levelId)
            {
                case 1:
                    _level1Tra.gameObject.Show();
                    _level2Tra.gameObject.Hide();
                    _level3Tra.gameObject.Hide();

                    for (int i = 0; i < _hpTra.childCount; i++)                    
                        _hpTra.GetChild(i).gameObject.Show();
                    
                    break;
                case 2:
                    _isPlaying = true;
                    var time = PlaySound(4);
                    Delay(time, () => { _isPlaying = false; });
                    _level1Tra.gameObject.Hide();
                    _level2Tra.gameObject.Show();
                    _level3Tra.gameObject.Hide();

                    for (int i = 0; i < _hpTra.childCount; i++)
                    {
                        var child = _hpTra.GetChild(i).gameObject;

                        if (i>=_hpTra.childCount-1)                       
                           child.Hide();
                        else
                            child.Show();                     
                    }

                    break;
                case 3:
                    _isPlaying = true;
                    var time1 = PlaySound(5);
                    Delay(time1, () => { _isPlaying = false; });
                    _level1Tra.gameObject.Hide();
                    _level2Tra.gameObject.Hide();
                    _level3Tra.gameObject.Show();

                    for (int i = 0; i < _hpTra.childCount; i++)
                    {
                        var child = _hpTra.GetChild(i).gameObject;

                        if (i >= _hpTra.childCount - 2)
                            child.Hide();
                        else
                            child.Show();
                    }
                    break;
            }
        }

        #region 位置缩放层级相关
        void InitData()
        {
            _line1StartX = -1714f; _line1EndX = 1286f;
            _line2StartX = 1714f; _line2EndX = -1286;
            _moveSpeed = 5f * (Screen.width / 1920f);
           

            _line1InitPosDic = new Dictionary<string, List<float>>();
            _line1InitPosDic.Add("g-a", new List<float> { -1714f, 225f, 0.5f, 0,-764f,-295f });
            _line1InitPosDic.Add("m-b", new List<float> { -1214f, 225f, 0.6f, 1,-604f,-371f });
            _line1InitPosDic.Add("g-c", new List<float> { -714f, 225f, 0.6f, 2,-758f,-468f });
            _line1InitPosDic.Add("g-b", new List<float> { -214f, 225f, 1.0f, 3 ,-741,-146});
            _line1InitPosDic.Add("g-d", new List<float> { 286f, 225f, 0.7f, 4 ,-758f,-395f});
            _line1InitPosDic.Add("m-a", new List<float> { 786f, 225f, 0.6f, 5 ,-754f,-272});
         
            _line2InitPosDic = new Dictionary<string, List<float>>();
            _line2InitPosDic.Add("m-c", new List<float> { -786f, 0, 0.8f, 6 ,-758f,-382f});
            _line2InitPosDic.Add("m-d", new List<float> { -286f, 0, 0.8f, 7,-724f,-493f });
            _line2InitPosDic.Add("w-a", new List<float> { 214f, 0, 0.8f, 8 ,-761f,-199f});
            _line2InitPosDic.Add("w-b", new List<float> { 714f, 0, 0.8f, 9,-752f,-484f });
            _line2InitPosDic.Add("w-c", new List<float> { 1214f, 0, 1.0f, 10 ,-760f,-120f});
            _line2InitPosDic.Add("w-d", new List<float> { 1714f, 0, 1.0f, 11,-750f,-386f});
          
            for (int i = 0; i < _spines.childCount; i++)
            {
                var child = _spines.GetChild(i);
                var rect = child.GetRectTransform();
                var name = child.name;
                var isLine1 = _line1InitPosDic.ContainsKey(name);
                var isLine2 = _line2InitPosDic.ContainsKey(name);

                if (isLine1)
                    InitRect(rect, name, _line1InitPosDic);

                if (isLine2)
                    InitRect(rect, name, _line2InitPosDic);
            }

            _dialogues = new List<string> {
                "嘿嘿嘿，我不会让你们顺利开始的！",
                "我们也不会让你得逞的！"
            };
            _roleStartPos = new Vector2(-2170, 539); _roleEndPos = new Vector2(-960, 539);
            _enemyStartPos = new Vector2(200, 540); _enemyEndPos = new Vector2(-994, 540);
            SetPos(_devil, _enemyStartPos); SetPos(_tianTian, _roleStartPos);
        }

        void InitRect(RectTransform rect, string name, Dictionary<string, List<float>> dic)
        {
            var value = dic[name];        
            rect.localPosition = new Vector2(value[0], value[1]);
            rect.localScale = new Vector2(value[2], value[2]);         
            rect.GetEmpty4Raycast().enabled = true;
        }

        void SetParentToSpines(Transform levelTra)
        {
            for (int i = 0; i < levelTra.childCount; i++)
            {
                var parent = levelTra.GetChild(i);
                var isChild = parent.childCount == 0;

                if (!isChild)
                {
                   var child = parent.GetChild(0);                   
                    child.SetParent(_spines);                  
                }
            }

        }
        #endregion

     
        private void OnClick(GameObject go)
        {
            if (_isPlaying)
                return;

            _isPlaying = true;

            PlayOnClickSound();
            _mask.Show();
            var name = go.name;
            _isPause = true;
            go.transform.SetAsLastSibling();
            var spineGo = go.transform.GetChild(0).gameObject;
            var spineName1 = name;     //发光
            var spineName2 = name+"3";       //错误抖动

            var levelData = _levelDic[_curLevelId];
            var isCorrect = levelData.Contains(name);   //是否是正确的

            if (isCorrect)
            {
                var time=  PlaySuccessSound();
                Delay(time, () => { _isPlaying = false; });
                PlaySpine(spineGo, spineName1,()=> { MoveToPos(name,()=> { _isPause = false; _mask.Hide(); }); });    //播放发光音效
                StopCoroutines(_moveCorDic[name]);   //停止移动           
                var rect = _spines.Find(name).GetRectTransform();
                rect.GetEmpty4Raycast().enabled = false;
                SetScale(rect, Vector2.one);            
            }
            else
            {
                var time = PlayFailSound();
                Delay(time, () => { _isPlaying = false; });
                PlaySpine(spineGo, spineName2,()=> { _isPause = false; _mask.Hide(); });
            }
                 
        }

       
        private void MoveToPos(string name,Action moveEnd)
        {
            var isLine1 = _line1InitPosDic.ContainsKey(name);
            var isLine2 = _line2InitPosDic.ContainsKey(name);

            if (isLine1)            
                MoveToPos(name, _line1InitPosDic, moveEnd);           

            if (isLine2)          
                MoveToPos(name, _line2InitPosDic, moveEnd);            
        }


        private void MoveToPos(string name,Dictionary<string,List<float>> dic,Action moveEnd)
        {
            var rect = _spines.Find(name).GetRectTransform();
            var value = dic[name];
            rect.SetParent(GetParent(_curLevelId, name));
            rect.DOAnchorPos(new Vector2(value[4], value[5]), 0.5f).OnComplete(() => {
                _isFinish++;
            
                if (_isFinish == 4)
                {
                    var spineName = GetBoomName(_curLevelId);
                        HideLevel(_curLevelId);
                       _mask.Show();

                    var time = PlaySpine(_jueseGo, spineName);
                    PlayVoice(0);
                    Delay(time+1, () => {
                       
                        _curLevelId++; _isFinish = 0;
                        ShowLevel(_curLevelId);
                        if (_curLevelId <= 3)
                        {
                            PlaySpine(_jueseGo, _jueseGo.name);
                            Delay(0.5f, () => { _xemGo.Show(); });                                                                 
                        }
                        else
                        {
                            for (int i = 0; i < _hpTra.childCount; i++)
                                _hpTra.GetChild(i).gameObject.Hide();

                          var time1=  PlaySound(7); _xemGo.Hide();

                            Delay(time1, () => {
                                _blackMaskGo.Show();
                                PlayXing();

                                Delay(5f, () => {
                                    _kP.Hide();
                                    GameOver(); });
                            });
                            
                        }
                        _mask.Hide();
                    });
                    Delay(1.3f, () => { PlaySound(6); });
                    Delay(1.5f, () => { _xemGo.Hide(); });
                }
                moveEnd?.Invoke();
            });
        }

        private void HideLevel(int levelId)
        {
            switch (levelId)
            {
                case 1:
                    _level1Tra.gameObject.Hide();
                    break;
                case 2:
                    _level2Tra.gameObject.Hide();
                    break;
                case 3:
                    _level3Tra.gameObject.Hide();
                    break;
            }
        }

        private string GetBoomName(int levelId)
        {
            string spineName = string.Empty;
            switch (_curLevelId)
            {
                case 1:
                    spineName = "g-boom";
                    break;
                case 2:
                    spineName = "w-boom";
                    break;
                case 3:
                    spineName = "m-boom";
                    break;
                default:
                    spineName = string.Empty;
                    break;
            }
            return spineName;
        }

        private Transform GetParent(int levelId,string name)
        {
            var strs = name.Split('-');
            string parentName = strs[1];

            Transform parent = null;
            switch (levelId)
            {
                case 1:
                    parent = _level1Tra.Find(parentName);
                    break;
                case 2:
                    parent = _level2Tra.Find(parentName);
                    break;
                case 3:
                    parent = _level3Tra.Find(parentName);
                    break;
            }
            return parent;
        }


        private void GameOver()
        {
            _blackMaskGo.Show();PlayVoice(3, false, SoundManager.SoundType.COMMONSOUND);
            PlaySuccessSpine(()=> {

                Delay(4f, () => {
                    _successSpine.Hide();
                    _okSpine.Show(); _replaySpine.Show();
                    PlaySpine(_okSpine, "ok2");
                    PlaySpine(_replaySpine, "fh2");                  
                });
            });
            
        }

        private void PlaySuccessSpine(Action callBack)
        {
            _successSpine.Show();
            PlaySpine(_spSpine, "kong",()=> { PlaySpine(_spSpine, "sp"); });
           
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2", null,true); callBack?.Invoke(); });
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject go, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(go, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(go, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(go, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            PlayOnClickSound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (_talkIndex)
            {
                case 1:
                    break;
            }
            _talkIndex++;
        }



        #region 移动相关

        private void InitMoveCorDic()
        {
            _moveCorDic = new Dictionary<string, Coroutine>();
            for (int i = 0; i < _spines.childCount; i++)
            {
                var child = _spines.GetChild(i);
                var rect = child.GetRectTransform();
                var name = child.name;
                var isLine1 = _line1InitPosDic.ContainsKey(name);
                var isLine2 = _line2InitPosDic.ContainsKey(name);

                if (isLine1)  //向右移动
                {
                    var value = CorMove(rect, _line1StartX, _line1EndX, _moveSpeed, RightMove);
                    _moveCorDic.Add(name, value);
                }

                if (isLine2)  //向左移动
                {
                    var value = CorMove(rect, _line2StartX, _line2EndX, _moveSpeed, LeftMove);
                    _moveCorDic.Add(name, value);
                }
            }
        }

        void LeftMove(RectTransform rect, float startXPos, float endXPos, float speed)
        {
            if (rect.anchoredPosition.x <= endXPos)
                rect.anchoredPosition = new Vector2(startXPos, rect.anchoredPosition.y);
            rect.Translate(Vector2.left * speed);
        }

        void RightMove(RectTransform rect, float startXPos, float endXPos, float speed)
        {
            if (rect.anchoredPosition.x >= endXPos)
                rect.anchoredPosition = new Vector2(startXPos, rect.anchoredPosition.y);
            rect.Translate(Vector2.right * speed);
        }

        Coroutine CorMove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            return _mono.StartCoroutine(IEMove(rect, startXPos, endXPos, speed, moveCallBack));
        }

        IEnumerator IEMove(RectTransform rect, float startXPos, float endXPos, float speed, Action<RectTransform, float, float, float> moveCallBack)
        {
            while (true)
            {
                if (!_isPause)
                {
                    yield return new WaitForSeconds(0.02f);
                    moveCallBack?.Invoke(rect, startXPos, endXPos, speed);
                }
                else
                {
                    yield return null;
                }

            }
        }

        #endregion


        #region 播放Spine

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);      
            return time;
        }

        private void InitSpines(Transform parent, bool isKong = true, Action callBack = null)
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
            callBack?.Invoke();
        }

        private GameObject GetSpineGo(Transform parent, string goName)
        {
            return parent.Find(goName).gameObject;
        }

        #endregion

        #region 播放Audio

        private float PlayBgm(int index, bool isLoop = true, SoundManager.SoundType type = SoundManager.SoundType.BGM)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop); 
            return time;
        }

        private float PlayVoice(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);  
            return time;
        }

        private float PlaySound(int index, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = SoundManager.instance.PlayClip(type, index, isLoop);  
            return time;
        }

        /// <summary>
        /// 播放点击声音
        /// </summary>
        private void PlayOnClickSound()
        {
            SoundManager.instance.PlayClip(9);
        }

        /// <summary>
        /// 播放失败声音
        /// </summary>
        private float PlayFailSound()
        {
            PlayVoice(2);
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
           var time =   SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        /// <summary>
        /// 播放成功声音
        /// </summary>
        private float PlaySuccessSound()
        {
            PlayVoice(1);
            var index = Random.Range(0, _successSoundId.Count);
            var id = _successSoundId[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        #endregion

        #region 停止Audio
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
        private void BellSpeck(int index,GameObject go, Action specking = null, Action speckend = null, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {      
            _mono.StartCoroutine(SpeckerCoroutine(go, type, index, specking, speckend));
        }
        private void RoleSpeck(int index, Action callBack = null, bool isLoop = false, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            var time = PlaySound(index, isLoop, type);
            Delay(time, callBack);
        }


        #endregion

        #region 添加Btn监听
        private void AddBtnsEvent(Transform parent, PointerClickListener.VoidDelegate callBack, Action callBack1 = null)
        {         
            for (int i = 0; i < parent.childCount; i++)
            {
                var child = parent.GetChild(i).gameObject;

                var isNullSpine = child.GetComponent<SkeletonGraphic>() == null;
                if (!isNullSpine)
                    continue;

                RemoveEvent(child);
                AddEvent(child, callBack);
            }
            callBack1?.Invoke();
        }

        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
           
            PointerClickListener.Get(go).onClick = g =>
            {            
                callBack?.Invoke(g);
            };
        }

        private void RemoveEvent(GameObject go)
        {        
            PointerClickListener.Get(go).onClick = null;
        }

        #endregion

        #region 修改Rect
        private void SetPos(RectTransform rect, Vector2 pos)
        {        
            rect.anchoredPosition = pos;
        }

        private void SetScale(RectTransform rect, Vector3 v3)
        {
            rect.localScale = v3;
        }

        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack = null)
        {
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        private void SetMove(RectTransform rect, Vector2 v2, float duration, Action callBack = null)
        {
            rect.DOAnchorPos(v2, duration).OnComplete(() => { callBack?.Invoke(); });
        }

        #endregion

        #region 延时
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

        #region 打字机
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            text.text = string.Empty;
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
    }
}
