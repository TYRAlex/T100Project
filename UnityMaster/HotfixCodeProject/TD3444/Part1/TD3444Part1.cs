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

    public class TD3444Part1
    {
        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo, _mask, _replaySpine, _startSpine, _okSpine,_dragMask,_dragEndSpine ;
        private GameObject _successSpine, _spSpine, _dDD, _sDD,_overContentGo,_lightSpine,_contentSocreGo,_lightStarSpine;      
        private RectTransform _paperRect, _xemRect, _dragsRect, _overRect;
        private bool _isPlaying;
        private Transform _endImgTra,_dragsTra,_socresTra,_overTra;
       
        private List<int> _succeedSoundIds, _failSoundIds;
        private List<string> _showSort;
        private int _curShowIndex;

        private List<mILDrager> _mILDragers;
        private mILDrager _curDrager;
        private List<string> _endImgChildNames;
        private bool _isOver;
        private List<int> _overSpriteIndexs;
      

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

            _paperRect = curTrans.GetRectTransform("BG/paper");
            _xemRect = curTrans.GetRectTransform("BG/xem");
            _dragsRect = curTrans.GetRectTransform("Contents/drags");

            _endImgTra = curTrans.Find("Contents/endImg");
            _dragsTra = curTrans.Find("Contents/drags");
            _socresTra = curTrans.Find("scores");
            _overTra = curTrans.Find("OverContent/over");

            _overRect = curTrans.GetRectTransform("OverContent/over");
            _overContentGo = curTrans.GetGameObject("OverContent");
            _lightSpine = curTrans.GetGameObject("OverContent/light");
            _lightStarSpine = curTrans.GetGameObject("OverContent/light-star");
            _contentSocreGo = curTrans.GetGameObject("Contents/scores");
            _dragEndSpine = curTrans.GetGameObject("Contents/dragEndSpine");
            _dragMask = curTrans.GetGameObject("dragMask");
           
            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;
            _isOver = false;
         
            _curShowIndex = 0;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _showSort = new List<string> { "light", "eye", "nose", "ear", "zm", "maozi", "finish" };
            _overSpriteIndexs = new List<int>();
            _endImgChildNames = new List<string>();
            for (int i = 0; i < _endImgTra.childCount; i++)
            {
                var name = _endImgTra.GetChild(i).name;
                _endImgChildNames.Add(name);
            }

        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _dDD.Hide(); _sDD.Hide(); _overContentGo.Hide(); _dragMask.Hide();
            _contentSocreGo.Show(); _socresTra.gameObject.Hide();

            _paperRect.anchoredPosition = new Vector2(575, 0);
            _xemRect.localEulerAngles = new Vector3(0, 0, 30);
            _dragsRect.anchoredPosition = new Vector2(575, 0);
            _overRect.localScale = new Vector3(0, 0, 0);
            _paperRect.gameObject.Hide(); _xemRect.gameObject.Hide();

           
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
            InitializeSpine(_lightSpine); InitializeSpine(_lightStarSpine); InitializeSpine(_successSpine);
            InitScores(); InitImgs(_endImgTra); InitImgs(_overTra); HideAllChilds(_dragsTra);
        }



        void GameStart()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    PlayOnClickSound(); RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayCommonBgm(8);
                        _startSpine.Hide();
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, () => { ShowVoiceBtn(); }, RoleType.Child);
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
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide();StartGame(); }, RoleType.Child);
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void InitScores()
        {
            for (int i = 0; i < _socresTra.childCount; i++)
            {
                var child = _socresTra.GetChild(i);
                child.Find("icons/x1").gameObject.Show();
                child.Find("icons/x2").gameObject.Hide();
                child.Find("boom").gameObject.Hide();
                var ani = child.Find("Ani");
                var aniChild = ani.Find("-");
                ani.GetRectTransform().anchoredPosition = new Vector2(110, 20);
                ani.GetImage().color = new Color(1, 1, 1, 0);
                aniChild.GetImage().color = new Color(1, 1, 1, 0);
            }
        }

        private void PlayScoreAni(int index)
        {
            var child = _socresTra.GetChild(index);
            var x1 = child.Find("icons/x1").gameObject;
            var x2 = child.Find("icons/x2").gameObject;
            var boom = child.Find("boom").gameObject;
            boom.Show();
            Delay(0.1f, () => { x1.Hide(); x2.Show(); });
            PlaySpine(boom, boom.name,()=> {  });

            var aniRect = child.Find("Ani").GetRectTransform();
            var aniImg = child.Find("Ani").GetImage();
            var aniChildImg = child.Find("Ani/-").GetImage();
            var t1= aniRect.DOAnchorPosY(100, 1f);
            var t2 = aniImg.DOColor(new Color(1, 1, 1, 1), 0.5f).OnComplete(()=> { aniImg.DOColor(new Color(1, 1, 1, 0), 0.5f); });
            var t3 = aniChildImg.DOColor(new Color(1, 1, 1, 1), 0.5f).OnComplete(() => { aniChildImg.DOColor(new Color(1, 1, 1, 0), 0.5f); });
            DOTween.Sequence().Append(t1).Join(t2).Join(t3);

        }

        private void InitImgs(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var img = parent.GetChild(i).GetImage();
                img.color = new Color(1, 1, 1, 0);
                img.sprite = null;
            }
        }

        
        private void ShowOverImgs()
        {

            for (int i = 0; i < _showSort.Count-1; i++)
            {
                var name = _showSort[i];
                var index = _overSpriteIndexs[i];
                var img = FindGo(_overTra, name).transform.GetImage();
                var sprite = FindGo(_endImgTra, name).transform.GetComponent<BellSprites>().sprites[index];
                ShowImg(img, sprite);
            }
        }

        private void ShowImg(Image img,Sprite sprite)
        {
            img.sprite = sprite;
            img.color = new Color(1, 1, 1, 1);
        }

        private void EnterAni(float duration=0.5f)
        {
            var name = _showSort[_curShowIndex];
            ShowChilds(_dragsTra,name,go=> {
              
                var drag = go.transform.Find("drag");
               _mILDragers = SetDragerCallBack(drag, DragStart, null, DragEnd, DragOnClick);

                foreach (var drager in _mILDragers)
                {
                    if (name!="finish")                  
                    drager.canMove = false;

                    drager.DoReset();
                    drager.gameObject.Show();
                    drager.transform.GetImage().raycastTarget = true;
                }           
            });
            PlayVoice(0);
            _dragsRect.DOAnchorPosX(0, duration);
            _paperRect.DOAnchorPosX(0, duration);
            _xemRect.DOLocalRotate(Vector3.zero, duration);
          
        }


        private void QuitAni(Action callBack = null,float duration = 0.5f)
        {
            _dragsRect.DOAnchorPosX(575, duration);
            _paperRect.DOAnchorPosX(575, duration);
            _xemRect.DOLocalRotate(new Vector3(0, 0, 30), duration);
            Delay(duration, () => { HideAllChilds(_dragsTra); callBack?.Invoke(); });
        }



        private void DragStart(Vector3 pos, int dragType, int index)
        {
            foreach (var drager in _mILDragers)
            {
                if (drager.index == index)
                {
                    _curDrager = drager;
                    break;
                }
            }
            _curDrager.transform.SetAsLastSibling();
        }

       

        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {

            if (_curShowIndex >=_showSort.Count-1)           
                isMatch = _overSpriteIndexs[0] == int.Parse(_curDrager.name)&& isMatch;



            if (_curDrager.canMove)
            {
                _dragMask.Show();
                if (isMatch)
                { var time = PlaySuccessSound(); Delay(time, () => { _dragMask.Hide(); }); }
                else
                { var time = PlayFailSound(); Delay(time, () => { _dragMask.Hide(); }); }
            }

            if (isMatch)
            {

                if(_curShowIndex< _showSort.Count - 1)
                    PlaySpine(_dragEndSpine,"star2",()=> { InitializeSpine(_dragEndSpine); });

                var lineGo = FindGo(_endImgTra, "line");
                var lineImg = lineGo.transform.GetImage();
                lineImg.color = new Color(1, 1, 1, 0);
                _curDrager.gameObject.Hide();
                foreach (var drager in _mILDragers)                
                    drager.transform.GetImage().raycastTarget = false;

                if (_curShowIndex == 3) 
                    PlayScoreAni(2);
                else if(_curShowIndex == 5)
                    PlayScoreAni(1);               

                ShowEndImg(_showSort[_curShowIndex], int.Parse(_curDrager.name));

                if (_isOver)
                {
                    Over();
                    return;
                }
                _overSpriteIndexs.Add(int.Parse(_curDrager.name));
                QuitAni(()=> {  _curShowIndex++;  EnterAni(); });
            }

           

            _curDrager.DoReset();
        }

        private void DragOnClick(int index)
        {                                 
            PlayOnClickSound();
            //显示引导线
            var lineGo = FindGo(_endImgTra, "line");
            var lineImg = lineGo.transform.GetImage();
            var lineSprits = lineGo.transform.GetComponent<BellSprites>();
            var isExist = _curShowIndex <= lineSprits.sprites.Length - 1;
            if (isExist)
            {
               
                ShowImg(lineImg, lineSprits.sprites[_curShowIndex]);

                foreach (var drager in _mILDragers)                                  
                    drager.canMove = true;                             
            }
            else
                lineImg.color = new Color(1, 1, 1, 0);
        }

        private void ShowEndImg(string name,int index)
        {     
           var isExist = _endImgChildNames.Contains(name);
            if (!isExist)
            {
                _isOver = true;
                return;
            }   
            
           var child= _endImgTra.Find(name);
           var img = child.GetImage();
           var bellSprites = child.GetComponent<BellSprites>();
           var sprite = bellSprites.sprites[index];
           ShowImg(img, sprite);
        }

      
        private void Over()
        {
            _mask.Show();           
            _overContentGo.Show();
            ShowOverImgs();
            PlaySpine(_lightSpine, _lightSpine.name, () => { PlaySpine(_lightSpine,_lightSpine.name+"2",null,true); });
            PlaySpine(_lightStarSpine, _lightStarSpine.name, null, true);
            var t1 = _overRect.DOScale(1.2f, 0.3f);
            var t2 = _overRect.DOScale(1, 0.3f);
            DOTween.Sequence().Append(t1)
                              .Append(t2)
                              .OnComplete(()=> {
                                  PlayScoreAni(0);
                                  Delay(4, () => { _socresTra.gameObject.Hide(); _overContentGo.Hide(); GameSuccess(); });
                              });
                                        
        }
            

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _paperRect.gameObject.Show(); _xemRect.gameObject.Show();
            _contentSocreGo.Hide(); _socresTra.gameObject.Show();
            EnterAni();          
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
            PlaySpine(_replaySpine, "fh2", () =>
            {
                AddEvent(_replaySpine, (go) =>
                {
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();
                        PlayCommonBgm(8); //ToDo...改BmgIndex
                        GameInit();                       					
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    PlayOnClickSound();
                    PlayCommonBgm(4);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show();
                        BellSpeck(_dDD, 2, null, null, RoleType.Child);
                        //ToDo...
                        //显示Middle角色并且说话  _dBD.Show(); BellSpeck(_dBD,0);						

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

        private void HideChilds(Transform parent, int index, Action<GameObject> callBack = null)
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

        private void ShowChilds(Transform parent, string name, Action<GameObject> callBack = null)
        {
            var go = parent.Find(name).gameObject;
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

        private void InitializeSpine(GameObject go)
        {
            go.GetComponent<SkeletonGraphic>().Initialize(true);
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Bd, float len = 0)
        {

            string daiJi = string.Empty;
            string speak = string.Empty;

            switch (roleType)
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
