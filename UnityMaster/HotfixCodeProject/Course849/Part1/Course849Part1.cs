using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course849Part1
    {

        private int _talkIndex;
        private MonoBehaviour _mono;

        private GameObject _curGo;
        private GameObject _bell;
        private GameObject _b1Go;
        private GameObject _lanKuang;
        private GameObject _mask;

        private GameObject _qbSpine;
        private GameObject _guangSpine;
        private GameObject _z1Spine;

        private GameObject _kzSpine;
        private GameObject _kz2Spine;
        private GameObject _kz3Spine;

        private Transform _onClicks;
        private Transform _spines;
        private Transform _spines2;
        private Transform _dragsTra;


        private bool _isPlaying;


        private Coroutine _faguangCor;
        private List<mILDrager> _mILDragers;
        private mILDrager _curDrag;
        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;
        private int _dragNum;

        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
            var curTrans = _curGo.transform;


            _bell = curTrans.GetGameObject("bell");
            _b1Go = curTrans.GetGameObject("Bg/b1");
            _lanKuang = curTrans.GetGameObject("OnClicks/0/lankuang");
            _mask = curTrans.GetGameObject("mask");

            _qbSpine = curTrans.GetGameObject("Spines/0/qb");
            _guangSpine = curTrans.GetGameObject("Spines/0/guang");
            _z1Spine = curTrans.GetGameObject("Spines/1/z1");

            _kzSpine = curTrans.GetGameObject("Spines/2/kz");
            _kz2Spine = curTrans.GetGameObject("Spines/2/kz2");
            _kz3Spine = curTrans.GetGameObject("Spines/2/kz3");

            _spines = curTrans.Find("Spines");
            _onClicks = curTrans.Find("OnClicks");
            _spines2 = curTrans.Find("Spines/2");
            _dragsTra = curTrans.Find("OnClicks/2/start");

            Input.multiTouchEnabled = false;
            GameInit();
            GameStart();
        }




        private void InitData()
        {
            _talkIndex = 1;
            _isPlaying = false;
            _dragNum = 0;
            _mILDragers = new List<mILDrager>();
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < _dragsTra.childCount; i++)
            {
                var drag = _dragsTra.GetChild(i).GetComponent<mILDrager>();
                var e4Ray = _dragsTra.GetChild(i).GetEmpty4Raycast();
                e4Ray.raycastTarget = true;
                drag.gameObject.Show();
                drag.DoReset();
                _mILDragers.Add(drag);
            }
        }


        private void GameInit()
        {
            InitData();


            StopAllAudio(); StopAllCoroutines();
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _bell.Show(); _b1Go.Hide(); _mask.Hide();

            HideAllChilds(_onClicks);HideAllChildsSpine(_spines);

            PlaySpine(_qbSpine, "animation", () => { ShowChildsSpine(_spines, 0); PlaySpine(_qbSpine, _qbSpine.name); });

            PlaySpine(_guangSpine, "animation");
            PlaySpine(_z1Spine, "animation");

            PlaySpine(_kzSpine, "animation");
            PlaySpine(_kz2Spine, "animation");
            PlaySpine(_kz3Spine, "animation");

            AddEvent(_lanKuang, OnClickLanKuang);

            foreach (var item in _mILDragers)            
                item.SetDragCallback(StartDrag, null, DragEnd);
            
        }





        void GameStart()
        {
            PlayCommonBgm(2);
            BellSpeck(_bell, 0,
            () =>
            {
                Delay(4.0f, () => { PlaySpine(_guangSpine, "g1"); PlayCommonSound(6); });
                Delay(4.9f, () => { PlaySpine(_guangSpine, "g2"); PlayCommonSound(6); });
                Delay(5.6f, () => { PlaySpine(_guangSpine, "g3"); PlayCommonSound(6); });
            },
             ShowVoiceBtn);
        }

        void TalkClick()
        {
            PlayOnClickSound();
            HideVoiceBtn();

            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_bell, 1, () => { _faguangCor = UpDate(true, 1.0f, () => { PlaySpine(_guangSpine, "g3"); PlayCommonSound(6); }); },()=> { ShowChilds(_onClicks, 0); });
                    break;
                case 2:
                    _b1Go.Show();HideChildsSpine(_spines, 0);ShowChildsSpine(_spines, 1);
                    PlaySpine(_z1Spine, _z1Spine.name);PlayVoice(1);
                    BellSpeck(_bell, 3, null, ShowVoiceBtn);
                    break;
                case 3:
                    _mask.Show();
                    HideChildsSpine(_spines, 1);
                    InitSpines(_spines2, false, () => { ShowChildsSpine(_spines, 2); ShowChilds(_onClicks, 1); });
                    BellSpeck(_bell, 4,null,()=> { _mask.Hide();_bell.Hide(); });
                    break;
                case 4:
                    _bell.Show();
                    BellSpeck(_bell, 5);
                    break;
            }
            _talkIndex++;
        }


        #region 游戏逻辑

        private mILDrager GetCurDrag(int index)
        {
            mILDrager drag = null;
            foreach (var item in _mILDragers)
            {
                if (item.index ==index)
                {
                    drag = item;
                    break;
                }
            }
            return drag;
        }



        private void StartDrag(Vector3 pos, int dragType, int index)
        {
            _curDrag = GetCurDrag(index);
            _curDrag.transform.SetAsLastSibling();

            SetDargRay(false);
      


        }

        private void SetDargRay(bool isRay)
        {
            foreach (var item in _mILDragers)
            {
                if (_curDrag != item)
                    item.transform.GetEmpty4Raycast().raycastTarget = isRay;

            }
        }

        private void DragEnd(Vector3 pos, int dragType, int index, bool isMatch)
        {
            SetDargRay(false);

            if (isMatch)
            {
                _curDrag.gameObject.Hide();
                var name = _curDrag.gameObject.name;
                PlayCommonSound(4);
                GameObject spineGo = null;
                int voiceIndex = -1;
                switch (name)
                {
                    case "z1":
                        spineGo = _kzSpine;
                        voiceIndex = 2;
                        break;
                    case "z2":
                        spineGo = _kz2Spine;
                        voiceIndex = 3;
                        break;
                    case "z3":
                        spineGo = _kz3Spine;
                        voiceIndex = 4;
                        break;
                }
                PlayVoice(voiceIndex);
                PlaySpine(spineGo, name,()=> {
                    SetDargRay(true);
                    _dragNum++;

                    if (_dragNum==3)                    
                        ShowVoiceBtn();
                    
                });
            }
            else
            {
               
                _curDrag.DoReset();
                var time = PlayCommonSound(5);
                Delay(time, () => { SetDargRay(true); });
               
            }
        }

        private void OnClickLanKuang(GameObject go)
        {            
            HideChilds(_onClicks, 0);
            StopCoroutines(_faguangCor); StopAudio(SoundManager.SoundType.COMMONSOUND);

            PlayVoice(0);
            PlaySpine(_qbSpine, "lk",()=> { ShowVoiceBtn(); });
            BellSpeck(_bell, 2);
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

        private void HideAllChildsSpine(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.GetComponent<CanvasGroup>().alpha = 0;
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

        #endregion

        #region 音频相关

        private float PlayFailSound()
        {
            var index = UnityEngine.Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            return  SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
        }

        private float PlaySuccessSound()
        {
            var index = UnityEngine.Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            return SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
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
