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




    public class TD8961Part1
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

        private Transform _posTra;
        private Transform _spinesTra;
        private Transform _xemsTra;

        private bool _isPlaying;
        private List<string> _onOnClicks;
        private Dictionary<int, int> _difficultyDic;
        private int _curDifficultyIndex;



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

            _spinesTra = curTrans.Find("Spines");
            _posTra = curTrans.Find("Pos");
            _xemsTra = curTrans.Find("xems");

            GameInit();
            GameStart();
        }

        void GameInit()
        {

            _onOnClicks = new List<string>();
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
            _talkIndex = 1;
            _isPlaying = false;
            _curDifficultyIndex = 0;

            _difficultyDic = new Dictionary<int, int>();

            for (int i = 0; i < _posTra.childCount; i++)
            {
                var key = i;
                var value = Random.Range(0, _posTra.GetChild(i).childCount);
                _difficultyDic.Add(key, value);
            }



            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.StopAudio();
            _mono.StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide(); _dDD.Hide(); _sDD.Hide();


            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);


            //初始化xemSpine
            InitSpines(_xemsTra);

            //初始化xemIcon
            var iocns = _xemsTra.GetComponentsInChildren<Image>(true);
            for (int i = 0; i < iocns.Length; i++)
                iocns[i].gameObject.Show();


            //初始化泡泡Spine
            InitSpines(_spinesTra, spine =>
            {
                var go = spine.gameObject;
                var parent = go.transform.parent;
                string name = string.Empty;
                if (parent.name == "0")
                    name = "pao" + Random.Range(1, 9);
                else if (parent.name == "1")
                    name = go.name;
                PlaySpine(go, name, null, true);
            });

            //初始化不规则点击
            InitCustomImgOnClcick(_spinesTra, OnClickIcon);

            //初始化位置
            InitPos(_curDifficultyIndex);

        }


        void GameStart()
        {
            _mask.Show(); _startSpine.Show();

            PlaySpine(_startSpine, "bf2", () =>
            {
                AddEvent(_startSpine, (go) =>
                {
                    SoundManager.instance.PlayClip(9);
                    RemoveEvent(_startSpine);
                    PlaySpine(_startSpine, "bf", () =>
                    {
                        PlayCommonBgm(0);
                        _startSpine.Hide();
                        _sDD.Show();
                        BellSpeck(_sDD, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); });                                           
                    });
                });
            });
        }


        void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.PlayClip(9);
            switch (_talkIndex)
            {
                case 1:
                    BellSpeck(_sDD, 1, null, () => { _sDD.Hide(); StartGame(); });
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑


        /// <summary>
        /// 初始化Spine
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="callBack"></param>
        private void InitSpines(Transform parent, Action<SkeletonGraphic> callBack = null)
        {
            var spines = parent.GetComponentsInChildren<SkeletonGraphic>(true);
            for (int i = 0; i < spines.Length; i++)
            {
                var spine = spines[i];
                spine.Initialize(true);
                callBack?.Invoke(spine);
            }
        }

        /// <summary>
        /// 初始化不规则点击
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="callBack"></param>
        private void InitCustomImgOnClcick(Transform parent, PointerClickListener.VoidDelegate callBack)
        {
            var customImgs = parent.GetComponentsInChildren<CustomImage>(true);
            for (int i = 0; i < customImgs.Length; i++)
            {
                var customImg = customImgs[i];
                var go = customImg.gameObject;
                AddEvent(go, callBack);

            }
        }

        private void InitPos(int curDifficultyIndex)
        {
            bool isOver = _difficultyDic.ContainsKey(curDifficultyIndex);

            if (!isOver)
            {
               // Debug.LogError("游戏结束");
                _isPlaying = true;
                Delay(1, GameSuccess);
               
                return;
            }

            var spine1Tra = _spinesTra.Find("1");
            var posTra = _posTra.GetChild(curDifficultyIndex).GetChild(_difficultyDic[curDifficultyIndex]);
           // Debug.LogErrorFormat("难度位置：Pos/{0}/{1}", _posTra.GetChild(curDifficultyIndex).name, posTra.name);
            for (int i = 0; i < posTra.childCount; i++)
            {
                var child = posTra.GetChild(i);
                var spineChild = spine1Tra.Find(child.name);

                spineChild.localPosition = child.localPosition;
                spineChild.localScale = child.localScale;
                spineChild.localEulerAngles = child.localEulerAngles;
                spineChild.SetSiblingIndex(i);
              //  spineChild.GetChild(0).GetComponent<CustomImage>().raycastTarget = true;
                var name = spineChild.name;

                if (name == "xem-a4" || name == "xem-a5" || name == "xem-a6")
                    PlaySpine(spineChild.gameObject, name, null, true);

            }


        }

        private void OnClickIcon(GameObject go)
        {

            var parent = go.transform.parent;
            var parentGo = parent.gameObject;
            var parentName = parent.name;

            bool isContain = _onOnClicks.Contains(parentName);
            if (isContain)
            {
                return;
            }


            if (_isPlaying)
                return;
            _isPlaying = true;

            SoundManager.instance.PlayClip(9);

          

            bool isCorrect = false;

            string spineName = string.Empty;
            string spineName2 = string.Empty;

            if (parentName == "xem-a4" || parentName == "xem-a5" || parentName == "xem-a6")
                isCorrect = true;

            if (isCorrect)
            {
              //  go.GetComponent<CustomImage>().raycastTarget = false;
                switch (parentName)
                {
                    case "xem-a4":
                        spineName = "xem-b1";
                        spineName2 = "xem-b4";
                        break;
                    case "xem-a5":
                        spineName = "xem-b2";
                        spineName2 = "xem-b5";
                        break;
                    case "xem-a6":
                        spineName = "xem-b3";
                        spineName2 = "xem-b6";
                        break;
                }

               

                if (!isContain)
                    _onOnClicks.Add(parentName);

                parent.SetAsLastSibling();
                PlaySpine(parentGo, spineName, () => { PlaySpine(parentGo, spineName2, null, true); });
                Delay(PlaySuccessSound(), () =>
                {
                    _isPlaying = false;
                    if (_onOnClicks.Count == 3)
                    {
                        var icon = _xemsTra.Find(_curDifficultyIndex.ToString()).Find("Icon").gameObject;
                        var boom = _xemsTra.Find(_curDifficultyIndex.ToString()).Find("boom").gameObject;
                        icon.Hide();
                        PlaySpine(boom, boom.name);
                        PlayVoice(0);
                        _onOnClicks.Clear();
                        _curDifficultyIndex++;
                        InitPos(_curDifficultyIndex);
                       // return;
                    }
                   
                });
            }
            else
            {
                spineName = parentName.Replace("a", "b");
                PlaySpine(parentGo, spineName, () => { PlaySpine(parentGo, parentName, null, true); });
                Delay(PlayFailSound(), () => { _isPlaying = false; });

            }
        }


        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();

           
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
                    SoundManager.instance.PlayClip(9);
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();
                        PlayCommonBgm(0); 
                        GameInit();
                        		
                        StartGame();
                    });
                });
            });

            PlaySpine(_okSpine, "ok2", () =>
            {
                AddEvent(_okSpine, (go) =>
                {
                    SoundManager.instance.PlayClip(9);
                   
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dDD.Show(); BellSpeck(_dDD, 2);                    					
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
            Delay(4.0f, GameReplayAndOk);
        }




        #endregion

        #region 常用函数

        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }

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

        #endregion

        #region 延时相关
        private void Delay(float delay, Action callBack)
        {
            _mono.StartCoroutine(IEDelay(delay, callBack));
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        #endregion

        #region Bell讲话

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Adult, SoundManager.SoundType type = SoundManager.SoundType.SOUND)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Adult, float len = 0)
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
            RemoveEvent(go);
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }

        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
        }
        #endregion

        #endregion

    }
}
