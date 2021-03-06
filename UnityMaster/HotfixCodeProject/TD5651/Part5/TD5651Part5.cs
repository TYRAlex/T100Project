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

    public class TD5651Part5
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
        private bool _isPlaying;

        private GameObject Box;
        private Transform Boxpos;
        private List<int> _list;
        private bool _canClick;
        private bool _canPress;
        private string temp1;
        private string temp2;
        private int temp3;
        private GameObject show;
        private int number;
        private GameObject bg2;
        private GameObject music;
        private Transform mytrans;

        GameObject bird;
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

            mytrans = curTrans.Find("mytrans");

            Box = mytrans.Find("Box").gameObject;
            Boxpos = mytrans.Find("Boxpos");
            show = mytrans.Find("show").gameObject;
            _list = new List<int>();
            bg2 = mytrans.Find("bg2").gameObject;
            music = mytrans.Find("bg2/music").gameObject;
            bird = mytrans.Find("bird").gameObject;

            GameInit();
            GameStart();
        }

        void InitData()
        {
            _isPlaying = true;

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

        }

        void GameInit()
        {
            InitData();

            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();
            DOTween.Clear();
            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _dDD.Hide();
            _sDD.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            number = 0;
            for (int i = 0; i < 6; i++)
            {
                _list.Add(i);
                Box.transform.GetChild(i).GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
                Box.transform.GetChild(i).gameObject.SetActive(true);
                Box.transform.GetChild(i).GetComponent<RawImage>().color = new Color(1, 1, 1, 1);
                Util.AddBtnClick(Box.transform.GetChild(i).GetChild(0).gameObject, Click);
            }

            for (int i = 0; i < 6; i++)
            {
                int temp = _list[Random.Range(0, _list.Count)];
                _list.Remove(temp);
                Box.transform.GetChild(i).transform.localPosition = Boxpos.transform.GetChild(temp).localPosition;
                SpineManager.instance.DoAnimation(Box.transform.GetChild(i).GetChild(1).gameObject, "kp", false);
            }

            for (int i = 0; i < 3; i++)
            {
                show.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
            }
            bg2.GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
            bg2.transform.GetChild(1).GetComponent<RawImage>().color = new Color(1, 1, 1, 0);
            show.SetActive(true);
            for (int i = 0; i < 2; i++)
            {
                music.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
            }

            bird.GetComponent<SkeletonGraphic>().Initialize(true);
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
                        //PlayCommonBgm(8);//ToDo...???BmgIndex
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        _startSpine.Hide();

                        //ToDo...
                        _sDD.SetActive(true);
                        BellSpeck(_sDD, 0, null,
                            () => { SoundManager.instance.ShowVoiceBtn(true); }
                            );

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
                    BellSpeck(_sDD, 1, null,
                        () => { _sDD.SetActive(false); StartGame(); }
                        );
                    break;
            }
            _talkIndex++;
        }

        #region ????????????
        private void Click(GameObject obj)
        {
            RawImage raw = obj.transform.parent.GetComponent<RawImage>();

            //????????????????????????
            if (_canClick && _canPress)
            {
                PlaySound(0);
                _canClick = false;
                _canPress = false;

                ColorDisPlay(raw, false);

                SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp" + obj.transform.parent.name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "pk-g" + obj.transform.parent.name, false,
                    () =>
                    {
                        _canClick = true;
                        temp1 = obj.transform.parent.name;
                        temp2 = obj.name;
                        temp3 = obj.transform.parent.GetSiblingIndex();
                    });
                    });
            }

            //????????????????????????
            if (_canClick && !_canPress)
            {
                ColorDisPlay(raw, false);

                _canPress = true;
                _canClick = false;
                if (obj.transform.parent.name == temp1)
                {
                    //???????????????
                    if (obj.name == temp2)
                    {
                        PlaySound(0);
                        SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp" + (Convert.ToInt32(obj.transform.parent.name) + 3), false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp", false,
                            () => 
                            { 
                                _canClick = true;
                                ColorDisPlay(raw);
                            });
                            });
                    }
                    else
                    {
                        PlaySound(0);

                        SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp" + obj.transform.parent.name, false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "pk-g" + obj.transform.parent.name, false,
                            () =>
                            {
                                CommonSound();
                                PlaySound(2);
                                Box.transform.GetChild(temp3).SetAsLastSibling();
                                obj.transform.parent.SetAsLastSibling();
                                Box.transform.GetChild(4).GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 1f);
                                _mono.StartCoroutine(Wait(2.5f, () => { obj.transform.parent.gameObject.SetActive(false); Box.transform.GetChild(4).gameObject.SetActive(false); }));
                                obj.transform.parent.GetComponent<RectTransform>().DOAnchorPos(new Vector2(0, 0), 1f).OnComplete(() =>
                                {
                                    if (temp1 == "1")
                                    {
                                        SpineManager.instance.DoAnimation(show.transform.Find(temp1).gameObject, "light2", false,
                                            () =>
                                            {
                                                _canClick = true;
                                                number++;
                                                Jugle();
                                            }
                                            );
                                    }
                                    else if (temp1 == "2")
                                    {
                                        SpineManager.instance.DoAnimation(show.transform.Find(temp1).gameObject, "light", false,
                                            () =>
                                            {
                                                _canClick = true;
                                                number++;
                                                Jugle();
                                            }
                                            );
                                    }
                                    else
                                    {
                                        SpineManager.instance.DoAnimation(show.transform.Find(temp1).gameObject, "light3", false,
                                               () =>
                                               {
                                                   _canClick = true;
                                                   number++;
                                                   Jugle();
                                               }
                                               );

                                    }
                                });
                            });
                            });
                    }
                }
                //????????????
                else
                {
                    PlaySound(0);

                    SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp" + obj.transform.parent.name, false,
                            () =>
                            {
                                LCommonSound();
                                _mono.StartCoroutine(Wait(PlaySound(1) + 0.5f, () => { PlaySound(3); }));
                                SpineManager.instance.DoAnimation(Box.transform.GetChild(temp3).GetChild(1).gameObject, "pk-g" + (Convert.ToInt32(Box.transform.GetChild(temp3).gameObject.name) + 3), false);
                                SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "pk-g" + (Convert.ToInt32(obj.transform.parent.name) + 3), false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(Box.transform.GetChild(temp3).GetChild(1).gameObject, "kp" + (Convert.ToInt32(Box.transform.GetChild(temp3).gameObject.name) + 3), false);
                                SpineManager.instance.DoAnimation(obj.transform.parent.GetChild(1).gameObject, "kp" + (Convert.ToInt32(obj.transform.parent.name) + 3), false,
                                    () => 
                                    { 
                                        _canClick = true; 

                                        //????????????
                                        ColorDisPlay(raw);
                                        ColorDisPlay(Box.transform.GetChild(temp3).GetComponent<RawImage>(), true);
                                    });
                            });
                            });
                }
            }
        }

        private void CommonSound()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }
        private void LCommonSound()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        private void Jugle()
        {
            if (number == 3)
            {
                bg2.GetComponent<RawImage>().DOColor(new Color(1, 1, 1, 1), 2f);

                SpineManager.instance.DoAnimation(music.transform.GetChild(0).gameObject, "niao", false);
                SpineManager.instance.DoAnimation(bird, "niao4", false);
                show.SetActive(false);

                bg2.transform.GetChild(1).GetComponent<RawImage>().DOColor(new Color(1, 1, 1, 1), 2f).OnComplete(() =>
                {
                    
                    SpineManager.instance.DoAnimation(music.transform.GetChild(0).gameObject, "niao2", true);
                    SpineManager.instance.DoAnimation(bird, "niao3", true);

                    show.SetActive(false);
                    PlaySound(4, true);
                    _mono.StartCoroutine(Sing());
                    _mono.StartCoroutine(Wait(6.5f, () => { GameSuccess(); }));
                });
            }
        }
        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator Sing()
        {
            while (true)
            {
                SpineManager.instance.DoAnimation(music.transform.GetChild(1).gameObject, "music", false,
                    () => { SpineManager.instance.DoAnimation(music.transform.GetChild(1).gameObject, "music2", false); }
                    );
                yield return new WaitForSeconds(4f);
            }
        }

        /// <summary>
        /// ????????????
        /// </summary>
        private void StartGame()
        {
            _mask.Hide();
            _canClick = true;
            _canPress = true;

        }

        /// <summary>
        /// ???????????????Ok??????
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
                        //PlayCommonBgm(8); //ToDo...???BmgIndex
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        GameInit();
                        //ToDo...						
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

                        //ToDo...
                        //??????Middle??????????????????  _dBD.Show(); BellSpeck(_dBD,0);	
                        _dDD.SetActive(true);
                        BellSpeck(_dDD, 2);

                    });
                });
            });

        }

        /// <summary>
        /// ??????????????????
        /// </summary>
        private void GameSuccess()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            _mask.Show();
            _successSpine.Show();
            PlayCommonSound(3);
            PlaySpine(_successSpine, "3-5-z", () => { PlaySpine(_successSpine, "3-5-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

        #endregion

        //??????????????????????????????
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.3f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    method?.Invoke();
                });
            }
        }

        #region ????????????

        #region ????????????

        private void ShowVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void HideVoiceBtn()
        {
            SoundManager.instance.ShowVoiceBtn(false);
        }

        #endregion

        #region ???????????????

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
        private void ShowAllChilds(Transform parent)
        {
            for (int i = 0; i < parent.childCount; i++)
                parent.GetChild(i).gameObject.Show();
        }

        #endregion

        #region ????????????

        /// <summary>
        /// ??????Drager??????
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
        /// ??????Droper??????(??????)
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

        #region Spine??????

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

        #region ????????????

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

        #region ????????????

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

        #region ????????????

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

        #region Bell??????

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Child, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
        {
            _mono.StartCoroutine(SpeckerCoroutine(type, index, go, specking, speckend, roleType));
        }

        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject go, Action method_1 = null, Action method_2 = null, RoleType roleType = RoleType.Child, float len = 0)
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

        #region ????????????

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

        #region ??????Rect??????

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

        #region ?????????
        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            _mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //???????????????        
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
