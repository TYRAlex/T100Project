using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using System.Collections.Generic;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course928Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;
        private GameObject _curGo;      
        private GameObject _bell;
        private GameObject _di;
        private bool _isPlaying;

        private GameObject _menSpine;
        private GameObject _succeedSpine;

        private CanvasGroup _spineCanv;
        private CanvasGroup _sunccedCanv;


        private Transform _spinesTra;
        private Transform _levelsTra;


        private string _levelOneNum;
        private Transform _onClicksLevel1Tra;
        private Text _levelInputTxt;

        private string _levelTwoNum;
        private Transform _onClicksLevel2Tra;
        private Text _level2InputTxt;

        private Transform _drags;
        private List<mILDrager> _mILDragers;
        private mILDrager _curDrag;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;
                   
            _bell = curTrans.GetGameObject("bell");
            _di = curTrans.GetGameObject("di");
            _menSpine = curTrans.GetGameObject("Spines/animation2");
            _succeedSpine = curTrans.GetGameObject("SucceedSpines/Spine");
            _spinesTra = curTrans.Find("Spines");
            _levelsTra = curTrans.Find("Levels");

            _onClicksLevel1Tra = curTrans.Find("Levels/1/OnClicks");
            _levelInputTxt = curTrans.GetText("Levels/1/Text");


            _onClicksLevel2Tra = curTrans.Find("Levels/2/OnClicks");
            _level2InputTxt = curTrans.GetText("Levels/2/Text");

            _spineCanv = _spinesTra.GetComponent<CanvasGroup>();
            _sunccedCanv = curTrans.Find("SucceedSpines").GetComponent<CanvasGroup>();

            _drags = curTrans.Find("Levels/3/Drags");


            GameInit();
            GameStart();
        }




        private void InitData()
        {
            _talkIndex = 1;
            _isPlaying = false;
            _spineCanv.alpha = 0;
            _sunccedCanv.alpha = 0;
            _levelInputTxt.text = string.Empty;
            _levelOneNum = string.Empty;

            _level2InputTxt.text = string.Empty;
            _levelTwoNum = string.Empty;

            _mILDragers = new List<mILDrager>();
        }


        private void GameInit()
        {
            InitData();
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllCoroutines();
            StopAllAudio();

            _bell.Show(); _di.Show();

            HideAllChilds(_levelsTra); InitKuang(_onClicksLevel1Tra); InitKuang(_onClicksLevel2Tra);

            AddEvents(_onClicksLevel1Tra, OnClickLevelOneNums);
            AddEvents(_onClicksLevel2Tra, OnClickLevelTwoNums);
            PlaySpine(_menSpine, _menSpine.name);

            for (int i = 0; i < _drags.childCount; i++)
            {
                var darg = _drags.GetChild(i).GetComponent<mILDrager>();
                var img = _drags.GetChild(i).GetImage();

                _mILDragers.Add(darg);
               darg.DoReset();
                darg.gameObject.Show();
               
                img.raycastTarget = true;
                darg.SetDragCallback(StartDrag, null, DragEnd);

            }
        }

       

        void GameStart()
        {
            PlayCommonBgm(2);
            BellSpeck(_bell, 0, null, () => {
                _bell.Hide(); _di.Hide();
                _spineCanv.alpha = 1;
                ShowChilds(_levelsTra, 0);
            });

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


        private void InitKuang(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
            {
                var num = parent.GetChild(i);
                var kuang = num.Find("kuang");
                kuang.GetComponent<CanvasGroup>().alpha = 0;
            }
        }

        private void KuangAni(Transform parent,Action callBack=null)
        {
            var kuang = parent.Find("kuang").GetComponent<CanvasGroup>();
            var tw1=  kuang.DOFade(1, 0.25f);
            var tw2 = kuang.DOFade(0, 0.25f);
            
            DOTween.Sequence()
                .Append(tw1)
                .Append(tw2)             
                .OnComplete(()=> {
                    callBack?.Invoke();
                });
        }

        private void OnClickLevelOneNums(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

            
            KuangAni(go.transform, () => { _isPlaying = false; });
            var name = go.name;
         
            int result = -1;
            var isNum = int.TryParse(name,out result);

            if (isNum)
            {
                PlayVoice(0);
                if (_levelOneNum.Length>=2)                                
                    return;
                               
                _levelOneNum += name;
                _levelInputTxt.text = _levelOneNum;
            }
            else
            {
                switch (name)
                {
                    case "Cancel":
                        PlayVoice(1);
                        if (_levelOneNum.Length != 0)
                        {                          
                            _levelOneNum = _levelOneNum.Remove(_levelOneNum.Length - 1, 1);
                            _levelInputTxt.text = _levelOneNum;
                        }
                        break;
                    case "Ok":
                        if (_levelOneNum!=string.Empty)
                        {
                            bool isOk = _levelOneNum == "40";
                            if (isOk)
                            {
                                PlayVoice(3);
                                Delay(4f, () => { PlayVoice(4);});
                                Delay(0.5f, () => {
                                    HideChilds(_levelsTra, 0);                                 
                                    PlaySpine(_menSpine, "animation", () => {                                                                   
                                        Delay(1.0f, () => { PlaySpine(_menSpine, "animation2",()=> {
                                            Delay(0.5F, () => { ShowChilds(_levelsTra, 1); });
                                           
                                        }); });
                                    });
                                });                                                                                            
                            }
                            else
                            {                             
                                PlayVoice(2);
                            }
                        }
                        break;
                }
            }

        }


        private void OnClickLevelTwoNums(GameObject go)
        {
            if (_isPlaying)
                return;
            _isPlaying = true;

         
            KuangAni(go.transform, () => { _isPlaying = false; });
            var name = go.name;

            int result = -1;
            var isNum = int.TryParse(name, out result);

            if (isNum)
            {
                PlayVoice(0);
                if (_levelTwoNum.Length >= 3)                                
                    return;
                
                _levelTwoNum += name;
                _level2InputTxt.text = _levelTwoNum;
            }
            else
            {
                switch (name)
                {
                    case "Cancel":
                        PlayVoice(1);
                        if (_levelTwoNum.Length != 0)
                        {
                            _levelTwoNum = _levelTwoNum.Remove(_levelTwoNum.Length - 1, 1);
                            _level2InputTxt.text = _levelTwoNum;
                        }
                        break;
                    case "Ok":
                        if (_levelTwoNum != string.Empty)
                        {
                            bool isOk = _levelTwoNum == "100";
                            if (isOk)
                            {
                                PlayVoice(3);
                                Delay(4f, () => { PlayVoice(4); });
                                Delay(0.5f, () => {
                                    HideChilds(_levelsTra, 1);                                
                                    PlaySpine(_menSpine, "animation", () => {                                                                        
                                        Delay(1.0f, () => {
                                            PlaySpine(_menSpine, "animation2", () => {
                                                Delay(0.5F, () => { ShowChilds(_levelsTra, 2); });                                                
                                            });
                                        });
                                    });
                                });
                            }
                            else
                            {                              
                                PlayVoice(2);
                            }
                        }
                        break;
                }
            }

        }

        private void StartDrag(Vector3 pos, int dragType, int index)
        {
            _curDrag = GetCurDrag(index);
            SetDragRay(_curDrag);
            _curDrag.transform.SetAsLastSibling();

        }

        private void DragEnd(Vector3 arg1, int dragType, int index, bool isMatch)
        {
            if (isMatch)
            {             
                _curDrag.gameObject.Hide();
                PlayVoice(3);
                HideChilds(_levelsTra, 2);
                PlaySpine(_menSpine, "animation", () => {
                    _sunccedCanv.alpha = 1;
                    _di.Show(); _bell.Show();
                    BellSpeck(_bell, 1);
                    PlayVoice(5);
                    PlaySpine(_succeedSpine, "animation", () => {
                        PlaySpine(_succeedSpine, "animation2", null, true);
                    });
                });
            }
            else
            {
                _curDrag.DoReset();
                SetAllDragRay();
                PlayCommonSound(5);
            }
        }

        private mILDrager GetCurDrag(int index)
        {
            mILDrager drag = null;
            foreach (var item in _mILDragers)
            {
                if (item.index== index)
                {
                    drag = item;
                    break;
                }
            }
            return drag;
        }
     
        private void SetDragRay(mILDrager drag)
        {
            foreach (var item in _mILDragers)
            {
                if (drag!=item)
                {
                    var img = item.transform.GetImage();
                    img.raycastTarget = false;
                }
            }
        }

        private void SetAllDragRay(bool isRay=true)
        {
            foreach (var item in _mILDragers)
            {
                var img = item.transform.GetImage();
                img.raycastTarget = isRay;            
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
