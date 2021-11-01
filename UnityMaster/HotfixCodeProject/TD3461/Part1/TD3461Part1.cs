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
	
	public enum RoleType
	{
	   Bd,
       Xem,
       Child,
       Adult,		
	}
	
    public class TD3461Part1
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
        private GameObject _sDD;
   
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private Transform _spines;
        private Transform _leftSpines;
        private Transform _rightSpines;
        private Transform _bottomDrags;
        private Transform _bottomSpines;
        private Transform _bottomDragsPos;
        private Transform _rightDrops;  

        private string[] _level1DragSpineName;
        private string[] _level2DragSpineName;
        private string[] _level3DragSpineName;

        private string[] _level1XingSpineName;
        private string[] _level2XingSpineName;
        private string[] _level3XingSpineName;

        private mILDrager _curDrag;
        private Vector2[] _curDragInitPoses;

        private int _curLevelId;
        private int _curSuccessNum;
        private int _curLevelNeedSuccessNum;

        private List<int> _recordIndex;

        private string[] _curLevelDragSpineName;
        private string[] _curLevelXingSpineName;

        private Transform _hintTra;
        private GameObject _hintSpine;
        private GameObject _hintYun;
        private GameObject _hintXem;
        private GameObject _hintBoom;
        private GameObject _bgSpines;
        private GameObject _hintJian;
        private RectTransform _hintYunRect;
        private Vector3 _curDragInitPos;

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
			_dDD = curTrans.GetGameObject("dDD");			           							
         	_sDD = curTrans.GetGameObject("sDD");

            _spines = curTrans.Find("Spines");
            _leftSpines  = curTrans.Find("Spines/leftspines");
            _rightSpines = curTrans.Find("Spines/rightspines");
            _bottomDrags = curTrans.Find("Spines/bottomdrags");
            _bottomSpines = curTrans.Find("Spines/bottomspines");
            _bottomDragsPos = curTrans.Find("Spines/bottomdragsPos");
            _rightDrops = curTrans.Find("Spines/rightdrops");
            _hintSpine = curTrans.GetGameObject("hint/hintSpine");
            _bgSpines = curTrans.GetGameObject("BG/bgspines");
            _hintYun = curTrans.GetGameObject("hint/hintyun");
            _hintXem = curTrans.GetGameObject("hint/hintxem");
            _hintBoom = curTrans.GetGameObject("hint/hintboom");
            _hintTra = curTrans.Find("hint");
            _hintYunRect = curTrans.GetRectTransform("hint/hintyun");
            _hintJian = curTrans.GetGameObject("hint/hintjian");
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _curLevelId = 0;
        
             _talkIndex = 1;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

            _level1DragSpineName = new string[3] { "xuan1", "xuan2", "xuan4" };
            _level2DragSpineName = new string[3] { "xuan4", "xuan5", "xuan3" };
            _level3DragSpineName = new string[3] { "xuan5", "xuan1", "xuan2" };

            _level1XingSpineName = new string[9] { "xx1", "xx1", "xx1", "xx1", "xx5", "xx5", "xx5", "xx2", "xx3" };
            _level2XingSpineName = new string[9] { "xx1", "xx1", "xx1", "xx4", "xx4", "xx6", "xx6", "xx5", "xx5" };
            _level3XingSpineName = new string[9] { "xx1", "xx1", "xx2", "xx2", "xx2", "xx3", "xx3", "xx6", "xx6" };

            _recordIndex = new List<int>();

            Shuffle(_level1XingSpineName);
            Shuffle(_level2XingSpineName);
            Shuffle(_level3XingSpineName);

        }

        void GameInit()
        {
            InitData();

            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            HideVoiceBtn(); StopAllAudio(); StopAllCoroutines();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);        		
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _dDD.Hide(); _sDD.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            InitGameSpines(_hintTra, null);
         
            //初始化Drag相关          
            InitGameDrags(_bottomDrags, drag => {
                drag.isActived = false;
                drag.SetDragCallback(StartDrag, null, EndDrag);
                InitSpine(drag.transform.GetChild(0).gameObject, "", false);
            });

            for (int i = 0; i < _bottomSpines.childCount; i++)
            {
                GameObject obj = _bottomSpines.GetChild(i).GetChild(0).gameObject;
                InitSpine(obj, "", false);
            }

            InitChildSpine(_leftSpines);
            InitChildSpine(_rightSpines);
        }

        void GameStart()
        {
            _mask.Show(); _startSpine.Show(); _bgSpines.Hide();

            PlaySpine(_startSpine, "bf2", () => {
                AddEvent(_startSpine, (go) => {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () => {
                        PlayBgm(0);
                        _startSpine.Hide();
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, ShowVoiceBtn);				
                        
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
                    BellSpeck(_sDD, 1, null, ()=> { _sDD.Hide(); _bgSpines.Show(); StartGame(); });
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
            _mask.Hide(); _bgSpines.Show();_spines.gameObject.Show(); 
            _hintYunRect.anchoredPosition = new Vector2(1370, 0);
            _hintYunRect.localScale = new Vector2(0.4f, 0.4f);
            PlaySpine(_hintXem, "xem1",null,true);
            PlaySpine(_hintYun, "o1", null, true);
            InitLevel(_curLevelId);
        }
	
        /// <summary>
        /// 游戏重玩和Ok界面
        /// </summary>
        private void GameReplayAndOk()
        {
            _mask.Show();
            _replaySpine.Show();
			_okSpine.Show();
            _successSpine.Hide();
            PlaySpine(_replaySpine, "fh2", () => {
                AddEvent(_replaySpine, (go) => {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine); 
					RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () => {
                        _okSpine.Hide();                     
                        GameInit();                            			
                        StartGame();
                        PlayBgm(0);
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () => {
                AddEvent(_okSpine, (go) => {
                    PlayOnClickSound();
					
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () => {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 2);
										
                       
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

        private void InitLevel(int curLevelId)
        {
            SetDragPos(_bottomDragsPos, _bottomDrags);

            switch (curLevelId)
            {
                case 0:
                    _curLevelDragSpineName = _level1DragSpineName;
                    _curLevelXingSpineName = _level1XingSpineName;
                    _curLevelNeedSuccessNum = 5;
                   
                    break;
                case 1:
                    _curLevelDragSpineName = _level2DragSpineName;
                    _curLevelXingSpineName = _level2XingSpineName;
                    _curLevelNeedSuccessNum = 6;
                    break;
                case 2:
                    _curLevelDragSpineName = _level3DragSpineName;
                    _curLevelXingSpineName = _level3XingSpineName;
                    _curLevelNeedSuccessNum = 7;
                    break;
            }
     
            InitGameSpines(_spines, null);

            _recordIndex.Clear();
            _curSuccessNum = 0;

            var count = _bottomDrags.childCount;

            for (int i = 0; i < count; i++)
            {
                var drag = _bottomDrags.GetChild(i).GetChild(0).gameObject;
                var spine = _bottomSpines.GetChild(i).GetChild(0).gameObject;
                var name = _curLevelDragSpineName[i];
                PlaySpine(drag, name, null, true);
                PlaySpine(spine, name, null, true);
            }

            string hintSpineName = "r" + (curLevelId + 1);
            PlayVoice(6);
            PlaySpine(_hintSpine, hintSpineName, () => { 
                
                PlaySpine(_hintSpine, "kong");

                count = _leftSpines.childCount;

                for (int i = 0; i < count; i++)
                {
                    var go = _leftSpines.GetChild(i).gameObject;
                    var name = _curLevelXingSpineName[i];
                    PlaySpine(go, name);
                }

                InitGameDrags(_bottomDrags, drag => { drag.isActived = true; });
            });    
        }

        private void StartDrag(Vector3 pos, int dragType, int index)
        {
            PlayVoice(2);
            GetCurDrag(_bottomDrags, index);
            _curDragInitPos = _curDragInitPoses[index];
            _curDrag.transform.SetAsLastSibling();
        }

        private void EndDrag(Vector3 pos, int dragType, int index, bool isMatch)
        {
            int xingIndex=-1;
            bool isOverlap = IsOverlapPoint(ref xingIndex);

            if (isOverlap)
            {
                string xingName = _curLevelXingSpineName[xingIndex];
                string yunName = _curLevelDragSpineName[index];

                if (xingName == "xx1")
                    ResetPos();
                else
                {
                    bool isContain = _recordIndex.Contains(xingIndex);

                    if (!isContain)
                    {
                        int xingInt = int.Parse(xingName.Replace("xx", string.Empty));
                        int xuanInt = int.Parse(yunName.Replace("xuan", string.Empty));

                        bool isOffset1 = xingInt - xuanInt == 1;

                        if (isOffset1)
                        {
                            var go = _rightSpines.GetChild(xingIndex).gameObject;
                            PlaySpine(go, yunName, null, true);
                            _recordIndex.Add(xingIndex);

                            ResetPos(0);
                            _curSuccessNum++;

                            if (_curSuccessNum == _curLevelNeedSuccessNum)
                            {
                                _curLevelId++;
                                InitGameDrags(_bottomDrags, drag => { drag.isActived = false; });
                                PlayVoice(5);
                                PlaySpine(_hintSpine, "r4", () =>
                                {

                                    if (_curLevelId > 2)
                                    {
                                       
                                        Delay(2, () => {
                                            PlaySpine(_hintSpine, "kong");
                                            PlaySpine(_hintYun, "o2",()=> { PlaySpine(_hintYun,"o1",null,true); });
                                            Delay(0.66f,()=> {
                                                PlayVoice(4);
                                                PlaySpine(_hintJian,"o2a",()=> { PlaySpine(_hintJian,"kong"); }); 
                                            });
                                            Delay(1, () => {
                                                PlaySpine(_hintBoom, "sc-boom2");
                                                Delay(0.1f, () => { PlaySpine(_hintXem,"kong"); });
                                               
                                                Delay(2, () => {
                                                    _spines.gameObject.Hide();
                                                    _bgSpines.Hide();
                                                    var yunRect = _hintYun.transform.GetRectTransform();
                                                    yunRect.DOAnchorPos(new Vector2(0, 0), 1f);
                                                    yunRect.DOScale(new Vector2(1, 1), 1f);
                                                    Delay(1f, () => {
                                                        PlayVoice(3);
                                                        PlaySpine(_hintYun, "oend",()=> { PlaySpine(_hintYun, "o1", null, true); });//, null, true);
                                                        Delay(3, GameSuccess);
                                                    });
                                                });
                                              
                                            });
                                        });
                                        return;
                                    }

                                    Delay(0.5f, () => { InitLevel(_curLevelId); });
                                });
                            }
                            
                               
                        }
                        else
                            ResetPos();
                    }
                    else
                        ResetPos();
                }
            }
            else
            {
                _curDrag.transform.GetRectTransform().anchoredPosition = _curDragInitPos;
                PlayVoice(1);
            }
            
        }

        private void ResetPos(int index=1)
        {
            _curDrag.transform.GetRectTransform().anchoredPosition = _curDragInitPos; 
            
            PlayVoice(index);

            if (index==1)
            {
               
                //小恶魔笑
                PlaySpine(_hintXem, "xem-jx",()=> { PlaySpine(_hintXem, "xem1", null, true); });
                //云开伤心
                PlaySpine(_hintYun, "o3", () => { PlaySpine(_hintYun, "o1", null, true); });
            }
            else
            {
                //云开心
                PlaySpine(_hintYun, "o4", () => { PlaySpine(_hintYun, "o1", null, true); });
            }
            
        }

        private bool IsOverlapPoint(ref int index)
        {
            bool isOverlap = false;
            var box2Ds = _rightDrops.GetComponentsInChildren<PolygonCollider2D>();

            for (int i = 0; i < box2Ds.Length; i++)
            {             
                if (box2Ds[i].OverlapPoint(Input.mousePosition))
                {
                    isOverlap = true;
                    index = i;
                    break;
                }
            }
            return isOverlap;
        }

        private void GetCurDrag(Transform parent, int index)
        {
            var mILDragers = parent.GetComponentsInChildren<mILDrager>(true);

            for (int i = 0; i < mILDragers.Length; i++)
            {
                if (mILDragers[i].index == index)
                {
                    _curDrag = mILDragers[i];
                    break;
                }
            }
        }
        #endregion

        //初始化所有子类的Spine
        void InitChildSpine(Transform tra)
        {
            for (int i = 0; i < tra.childCount; i++)
            {
                GameObject obj = tra.GetChild(i).gameObject;
                InitSpine(obj, "", false);
            }
        }

        //Spine初始化
        float InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);

            if (animation == "") return 0f;
            else return _ske.AnimationState.Data.SkeletonData.FindAnimation(animation).Duration;
        }

        #region 常用函数

        private void SetDragPos(Transform posTra, Transform tra)
        {
            _curDragInitPoses = new Vector2[posTra.childCount];

            for (int i = 0; i < posTra.childCount; i++)
            {
                var child = posTra.GetChild(i);
                var name = child.name;

                var drag = tra.Find(name);
                drag.localPosition = child.localPosition;
                drag.SetSiblingIndex(i);

                _curDragInitPoses[drag.GetComponent<mILDrager>().index] = drag.transform.GetRectTransform().anchoredPosition;
            }
        }

        void Shuffle<T>(T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
           
        }

        private void InitGameSpines(Transform parent, Action<SkeletonGraphic> callBack)
        {
            Gets<SkeletonGraphic>(parent, (spine) => {
                spine.gameObject.Show();
                spine.Initialize(true);
                callBack?.Invoke(spine);
            });
        }


        private void InitGameDrags(Transform parent, Action<mILDrager> callBack)
        {
            Gets<mILDrager>(parent, drag => { callBack?.Invoke(drag); });
        }

        private void Gets<T>(Transform parent, Action<T> callBack, bool includeInactive = true)
        {
            var components = parent.GetComponentsInChildren<T>(includeInactive);

            for (int i = 0; i < components.Length; i++)
            {
                var component = components[i];
                callBack?.Invoke(component);
            }
        }

    
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

        private void HideChilds(Transform parent,int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Hide();
            callBack?.Invoke(go);
        }

        private void ShowChilds(Transform parent, int index, Action<GameObject> callBack = null)
        {
            var go = parent.GetChild(index).gameObject;
            go.Show();
            callBack?.Invoke(go);
        }
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
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

        private GameObject FindGo(Transform parent, string goName)
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

			switch(roleType)
			{
				case RoleType.Bd:
				     daiJi = "bd-daiji"; speak = "bd-speak";
				break;
				case RoleType.Xem:
				     daiJi = "daiji"; speak = "speak";
				break;
				case RoleType.Child:
				     daiJi = "animation"; speak = "animation2";
				 break;
				case RoleType.Adult:
				     daiJi = "daiji"; speak = "speak";
				break;
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
