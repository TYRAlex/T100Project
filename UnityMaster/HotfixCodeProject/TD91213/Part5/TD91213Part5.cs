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

    public class TD91213Part5
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;

        private GameObject _mask;
        private GameObject _replaySpine;
        private GameObject _startSpine;
        private GameObject _okSpine;
        private GameObject _successSpine;
        private GameObject _spSpine;
        private GameObject _bD;
        private GameObject _dBD;
        private GameObject _uiMask;
        private GameObject _overMask;

        private Transform _level1Tra;
        private Transform _level2Tra;
        private Transform _level3Tra;




        private RectTransform _spinesRect;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private int _curLevelId;

        private List<LevelInfo> _levelDatas;
        private bool _isPlayingVoice;

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
            _uiMask = curTrans.GetGameObject("UIMask");
            _overMask = curTrans.GetGameObject("OverMask");
            _level1Tra = curTrans.Find("Spines/Level1");
            _level2Tra = curTrans.Find("Spines/Level2");
            _level3Tra = curTrans.Find("Spines/Level3");

            _spinesRect = curTrans.GetRectTransform("Spines");

            GameInit();
            GameStart();
        }

        void InitData()
        {
            _talkIndex = 1;
            _succeedSoundIds = new List<int> { 4, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _curLevelId = 1;
            _isPlayingVoice = false;
            InitLevelDatas();

        }

        void GameInit()
        {
            InitData();
        
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);         
            StopAllAudio(); StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide();_okSpine.Hide();_successSpine.Hide();
            _bD.Hide(); _dBD.Hide(); _uiMask.Hide(); _overMask.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
           
        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); ShowLevel(_curLevelId);
            PlaySpine(_startSpine, "bf2",()=> {
                   AddEvent(_startSpine, (go) => {
                       PlayOnClickSound(); RemoveEvent(_startSpine);
                       PlaySpine(_startSpine, "bf",()=> {
                           PlayBgm(0); _bD.Show(); _startSpine.Hide();
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
                    BellSpeck(_bD, 1, null,StartGame);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        private void InitLevelDatas()
        {
            _levelDatas = new List<LevelInfo>();

            var level1Drags = _level1Tra.Find("Drags");
            var level1DragsEnd = _level1Tra.Find("DragsEnd");
            _levelDatas.Add(new LevelInfo(_spinesRect,1, GetDrags(level1Drags), GetDrops(level1DragsEnd)));

            var level2Drags = _level2Tra.Find("Drags");
            var level2DragsEnd = _level2Tra.Find("DragsEnd");
            _levelDatas.Add(new LevelInfo(_spinesRect,2, GetDrags(level2Drags), GetDrops(level2DragsEnd)));

            var level3Drags = _level3Tra.Find("Drags");
            var level3DragsEnd = _level3Tra.Find("DragsEnd");
            _levelDatas.Add(new LevelInfo(_spinesRect,3, GetDrags(level3Drags), GetDrops(level3DragsEnd)));

        }


        private List<mILDrager> GetDrags(Transform parent)
        {
            var list = new List<mILDrager>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drag = parent.GetChild(i).GetComponent<mILDrager>();
                list.Add(drag);
            }

            return list;
        }

        private List<mILDroper> GetDrops(Transform parent)
        {
            var list = new List<mILDroper>();

            for (int i = 0; i < parent.childCount; i++)
            {
                var drop = parent.GetChild(i).GetComponent<mILDroper>();
                list.Add(drop);
            }
            return list;
        }

      

        private void ShowLevel(int curLevelId)
        {
            switch (curLevelId)
            {
                case 1:
                    _level1Tra.gameObject.Show();
                    _level2Tra.gameObject.Hide();
                    _level3Tra.gameObject.Hide();
                    var level1Info = GetCurLevelInfo(curLevelId);
                    ChildsHide(_level1Tra.Find("spines"));
                 
                    SetDragCallBack(level1Info.MILDragers);
                    SetDropCallBack(level1Info.MILDropers);
                    break;
                case 2:
                    _level1Tra.gameObject.Hide();
                    _level2Tra.gameObject.Show();
                    _level3Tra.gameObject.Hide();
                    var level2Info = GetCurLevelInfo(curLevelId);
                    ChildsHide(_level2Tra.Find("spines"));

                    SetDragCallBack(level2Info.MILDragers);
                    SetDropCallBack(level2Info.MILDropers);
                    break;

                case 3:
                    _level1Tra.gameObject.Hide();
                    _level2Tra.gameObject.Hide();
                    _level3Tra.gameObject.Show();
                    var level3Info = GetCurLevelInfo(curLevelId);
                    ChildsHide(_level3Tra.Find("spines"));

                    SetDragCallBack(level3Info.MILDragers);
                    SetDropCallBack(level3Info.MILDropers);
                    break;
                default:
                    
                    Debug.LogError("游戏三关完成了");
                    _overMask.Show();
                    Delay(2, () => {
                      
                        GameSuccess();
                        Delay(4, () => { GameReplayAndOk(); });
                    });                                   
                    break;
            }
        }


       
        private void SetDropCallBack(List<mILDroper> mILDropers)
        {
            foreach (var item in mILDropers)
            {
                item.SetDropCallBack(null, null, DropFail);
            }
        }

        private void DropFail(int obj)
        {
            PlayCommonSound(5);

            if (!_isPlayingVoice)
            {
                _uiMask.Show();
                _isPlayingVoice = true;
                var time = PlaySound(3);
                Delay(time, () => {
                    _isPlayingVoice = false;
                    _uiMask.Hide();
                });
            }
        }

        private void SetDragCallBack(List<mILDrager> mILDragers)
        {
            foreach (var item in mILDragers)
            {
                item.DoReset(); item.gameObject.Show();
                item.SetDragCallback(DragStart, null, DragEnd);
            }
                      
        }

        private void DragStart(Vector3 arg1, int type, int index)
        {
            var curLevelInfo = GetCurLevelInfo(_curLevelId);
            var curDragGo = curLevelInfo.GetCurDragGo(index,type);
            curDragGo.transform.SetAsLastSibling();
        }

        private void DragEnd(Vector3 pos, int type, int index, bool isMatch)
        {

            var curLevelInfo = GetCurLevelInfo(_curLevelId);
        
            if (isMatch)
            {
                _uiMask.Show();
                curLevelInfo.DrageSuccess(type,index);
                var curDropGo = curLevelInfo.CurDropGo;
                var sParent = curDropGo.transform.parent.parent.Find("spines");
                var curDropSpineGo = sParent.Find(curDropGo.name).gameObject;
                curDropSpineGo.Show();
                PlaySpine(curDropSpineGo, curDropSpineGo.name,()=> { _uiMask.Hide(); });

                var curDragGo = curLevelInfo.CurDragGo;
                curDragGo.Hide();
                curDragGo.GetComponent<mILDrager>().DoReset();            
                PlaySuccessSound();
                PlayCommonSound(4);
            }
            else
            {
                curLevelInfo.DragFail(index, type);             
            }

            if (curLevelInfo.IsPass)
            {
                _overMask.Show();
                var curLevelName = "Level" + curLevelInfo.LevelId;
                var curLevelTra= _spinesRect.Find(curLevelName);
                var parent = curLevelTra.Find("spines");

                Delay(1.0f, () => {
                    PlayPassSpine(parent);
                    Delay(3, () => {
                        _curLevelId++;
                        Delay(2, () => { _overMask.Hide(); ShowLevel(_curLevelId);  });
                    });
                });
               
                                       
            }
        }

        private void ChildsHide(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)            
                parent.GetChild(i).gameObject.Hide();            
        }

        private LevelInfo GetCurLevelInfo(int curLevelId)
        {
            LevelInfo info = null;
            foreach (var item in _levelDatas)
            {
                if (item.LevelId == curLevelId)
                {
                    info = item;
                    break;
                }
            }
            return info;
        }

        private void StartGame()
        {
            _bD.Hide(); _mask.Hide();
        }

        private void PlayPassSpine(Transform parent)
        {
            For(parent, 0.5f, (go) => {

                PlaySpine(go, go.name);
                PlayCommonSound(4);              
            });

       

        }

        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show(); _okSpine.Show();
            _successSpine.Hide();
           
            PlaySpine(_replaySpine, "fh2",()=> {
                AddEvent(_replaySpine,(go)=> {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time= PlaySpine(_replaySpine, "fh");// PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _okSpine.Hide();
                       PlayBgm(0);
                        GameInit();
                        StartGame();
                        ShowLevel(_curLevelId);
                    });
                });
            });
            PlaySpine(_okSpine, "ok2",()=> {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound(); PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time=  PlaySpine(_okSpine, "ok");// PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _dBD.Show();
                        BellSpeck(_dBD,4);
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
            PlaySpine(_successSpine, "6-12-z",()=> { PlaySpine(_successSpine,"6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
        }

        #endregion

        #region 常用函数

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

        private void PlayFailSound()
        {             
            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        private void PlaySuccessSound()
        {
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
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

        private void For(Transform parent, float delay, Action<GameObject> callBack = null)
        {
            _mono.StartCoroutine(IEFor(parent, delay, callBack));
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
            yield return null;
        }

        IEnumerator IEFor(Transform parent, float delay, Action<GameObject> callBack=null)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                yield return new WaitForSeconds(delay);
                callBack?.Invoke(parent.GetChild(i).gameObject);
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



        public class LevelInfo
        {
            public int LevelId;
            public bool IsPass;
            public List<mILDrager> MILDragers;
            public List<mILDroper> MILDropers;
            private List<string> _records;
            public GameObject CurDragGo;
            public GameObject CurDropGo;
            private RectTransform _rect;

          
            public LevelInfo(RectTransform rect, int levelId,List<mILDrager> mILDragers,List<mILDroper> mILDropers)
            {
                LevelId = levelId;
                IsPass = false;
                MILDragers = mILDragers;
                MILDropers = mILDropers;          
                _records = new List<string>();
                _rect = rect;
        
                SetSuccessAndFailDrop();
            }


            private void SetSuccessAndFailDrop()
            {
                for (int i = 0; i < MILDragers.Count; i++)
                {
                    var curDrag = MILDragers[i];
                    curDrag.DragRect = _rect;
                    
                    var successDrop = new List<mILDroper>();
                    var failDrop = new List<mILDroper>();

                    for (int j = 0; j < MILDropers.Count; j++)
                    {
                        var curDrop = MILDropers[j];

                        if (curDrag.dragType== curDrop.dropType)                       
                            successDrop.Add(curDrop);                       
                        else                        
                            failDrop.Add(curDrop);                        
                    }

                    curDrag.drops = successDrop.ToArray();
                    curDrag.failDrops = failDrop.ToArray();
                }
            }

            public void DrageSuccess(int type,int index)
            {
               
                CurDragGo = GetCurDragGo(index, type);

                CurDropGo = GetCurDropGo(type);
              
                var name = CurDropGo.name;
               
                bool isContains =  _records.Contains(name);
                if (!isContains)               
                    _records.Add(name);

                if (_records.Count== MILDropers.Count &&  _records.Count!=0)                
                     IsPass = true;
                
            }


            private GameObject GetCurDropGo(int type)
            {
                GameObject go = null;
                foreach (var item in MILDropers)
                {
                    if (item.dropType == type)
                    {
                        go = item.gameObject;
                        break;
                    }
                }
                return go;
            }

            public GameObject GetCurDragGo(int index, int type)
            {
                GameObject go = null;

                foreach (var item in MILDragers)
                {
                    if (item.index == index && item.dragType == type)
                    {
                        go = item.gameObject;
                        break; 
                    }                   
                }
                return go;
            }


            public void DragFail(int index, int type)
            {
                foreach (var item in MILDragers)
                {
                    if (item.index==index && item.dragType == type)
                    {
                        item.DoReset();
                        break;
                    }
                }
            }
           
        }
    }
}
