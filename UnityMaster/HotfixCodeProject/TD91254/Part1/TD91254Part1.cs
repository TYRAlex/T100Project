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

    public class TD91254Part1
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

        private int levelnumber;
        private bool _canClick;
        private bool _canDo;
        private string tempname;
        private int[] number1;
        private int[] number2;
        private int[] number3;
        private GameObject Box1;
        private Transform Box1pos;
        private GameObject Box2;
        private Transform Box2pos;
        private GameObject Box3;
        private Transform Box3pos;
        private GameObject show1;
        private GameObject show2;
        private GameObject show3;
        private GameObject qiang;
        private GameObject gamer;
        private GameObject xem;
        private Transform gamerpos;

        private bool _isPlaying;

        RawImage blackRaw;

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

            Box1 = curTrans.Find("Box1").gameObject;
            Box2 = curTrans.Find("Box2").gameObject;
            Box3 = curTrans.Find("Box3").gameObject;
            Box1pos = curTrans.Find("Box1pos");
            Box2pos = curTrans.Find("Box2pos");
            Box3pos = curTrans.Find("Box3pos");
            show1 = curTrans.Find("show1").gameObject;
            show2 = curTrans.Find("show2").gameObject;
            show3 = curTrans.Find("show3").gameObject;
            qiang = curTrans.Find("BG").Find("qiang").gameObject;
            gamer = curTrans.Find("BG").Find("gamer").gameObject;
            xem = curTrans.Find("xem").gameObject;
            gamerpos = curTrans.Find("BG").Find("gamerpos");
            for (int i = 0; i < Box1.transform.childCount; i++)
            {
                Util.AddBtnClick(Box1.transform.GetChild(i).gameObject, Click);
            }
            for (int i = 0; i < Box2.transform.childCount; i++)
            {
                Util.AddBtnClick(Box2.transform.GetChild(i).gameObject, Click);
            }
            for (int i = 0; i < Box3.transform.childCount; i++)
            {
                Util.AddBtnClick(Box3.transform.GetChild(i).gameObject, Click);
            }

            blackRaw = curTrans.Find("blackMask").GetRawImage();

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

            levelnumber = 1;
            qiang.GetComponent<RectTransform>().anchoredPosition = new Vector2(78, 602);
            number1 = new int[5] { 2, 4, 1, 3, 0 };
            number2 = new int[6] { 3, 1, 5, 4, 0, 2 };
            number3 = new int[7] { 2, 4, 0, 3, 1, 6, 5 };

            InitBox(Box1, Box1pos, number1.Length);
            InitBox(Box2, Box2pos, number2.Length);
            InitBox(Box3, Box3pos, number3.Length);

            gamer.transform.localPosition = gamerpos.localPosition;
            gamer.transform.localScale = gamerpos.localScale;
            show1.SetActive(false);
            show1.transform.GetChild(0).gameObject.SetActive(true);
            show2.SetActive(false);
            show2.transform.GetChild(0).gameObject.SetActive(true);
            show2.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(306, -220);
            show3.SetActive(false);
            show3.transform.GetChild(0).gameObject.SetActive(true);
            _canClick = true;
            _canDo = false;
            SpineManager.instance.DoAnimation(gamer, "yc", true);
            Box1.SetActive(true);
            Box2.SetActive(false);
            Box3.SetActive(false);
            gamer.SetActive(true);
            xem.SetActive(true);
            blackRaw.gameObject.SetActive(false);
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
                        //PlayCommonBgm(8);//ToDo...改BmgIndex
                        PlayBgm(0);
                        _startSpine.Hide();

                        //ToDo...
                        _startSpine.Hide();
                        _sDD.SetActive(true);
                        _mask.SetActive(true);
                        _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, _sDD, null,
                            () => { SoundManager.instance.ShowVoiceBtn(true); }
                            ));

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
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, _sDD, null,
                     () =>
                     {
                         _sDD.SetActive(false);
                         _mask.SetActive(false);
                         StartGame();
                     }
                     ));
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void Click(GameObject obj)
        {
            if (_canClick)
            {
                PlaySound(6);
                _canClick = false;
                obj.transform.SetAsLastSibling();
                obj.transform.GetChild(0).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "xx", true);
                _mono.StartCoroutine(Wait(0.4f, () => { _canDo = true; tempname = obj.name; }));
            }
            if (_canDo)
            {
                PlaySound(6);
                obj.transform.SetAsLastSibling();
                _canDo = false;
                if (obj.name == tempname)
                {
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "kong", false);
                    _canClick = true;
                }
                else
                {

                    obj.transform.GetChild(0).gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "xx", true);
                    _mono.StartCoroutine(Wait(0.2f,
                        () =>
                        {
                            PlaySound(0);
                            _mono.StartCoroutine(ChangeMove(obj, obj.transform.parent.Find(tempname).gameObject));
                        }
                        ));
                    _mono.StartCoroutine(Wait(0.8f,
                        () =>
                        {
                            _canClick = true;
                            switch (levelnumber)
                            {
                                case 1:
                                    int temp = number1[Convert.ToInt32(obj.name)];
                                    number1[Convert.ToInt32(obj.name)] = number1[Convert.ToInt32(tempname)];
                                    number1[Convert.ToInt32(tempname)] = temp;
                                    //show(1);
                                    Jugle(1);
                                    break;
                                case 2:
                                    int temp1 = number2[Convert.ToInt32(obj.name)];
                                    number2[Convert.ToInt32(obj.name)] = number2[Convert.ToInt32(tempname)];
                                    number2[Convert.ToInt32(tempname)] = temp1;
                                    //show(2);
                                    Jugle(2);
                                    break;
                                case 3:
                                    int temp2 = number3[Convert.ToInt32(obj.name)];
                                    number3[Convert.ToInt32(obj.name)] = number3[Convert.ToInt32(tempname)];
                                    number3[Convert.ToInt32(tempname)] = temp2;
                                    //show(3);
                                    Jugle(3);
                                    break;
                            }
                        }
                        ));

                }
            }
        }

        //黑幕转场
        IEnumerator BlackCurtainTransition(RawImage _raw, Action method = null)
        {
            _raw.color = new Color(0, 0, 0, 0);
            _raw.gameObject.SetActive(true);
            _raw.DOColor(Color.black, 0.3f).SetEase(Ease.Linear);

            yield return new WaitForSeconds(0.4f);

            _raw.DOColor(new Color(0, 0, 0, 0), 0.3f).SetEase(Ease.Linear).OnComplete(() => _raw.gameObject.SetActive(false));

            method?.Invoke();
        }

        private void Jugle(int level)
        {
            switch (level)
            {
                case 1:
                    for (int i = 0; i < number1.Length; i++)
                    {
                        if (number1[i] != i)
                            return;
                    }
                    _canClick = false;
                    for (int i = 0; i < number1.Length; i++)
                    {
                        SpineManager.instance.DoAnimation(Box1.transform.GetChild(i).GetChild(1).gameObject, "xx", true);
                    }
                    Down("yc");

                    break;
                case 2:
                    for (int i = 0; i < number2.Length; i++)
                    {
                        if (number2[i] != i)
                            return;
                    }
                    _canClick = false;
                    for (int i = 0; i < number2.Length; i++)
                    {
                        SpineManager.instance.DoAnimation(Box2.transform.GetChild(i).GetChild(1).gameObject, "xx", true);
                    }
                    Down("ng");
                    break;
                case 3:
                    for (int i = 0; i < number3.Length; i++)
                    {
                        if (number3[i] != i)
                            return;
                    }
                    _canClick = false;
                    for (int i = 0; i < number3.Length; i++)
                    {
                        SpineManager.instance.DoAnimation(Box3.transform.GetChild(i).GetChild(1).gameObject, "xx", true);
                    }
                    Down("sjd");
                    break;
            }
        }

        private void Down(string n)
        {
            PlaySound(1);
            qiang.GetComponent<RectTransform>().DOAnchorPosY(228f, 2f).SetEase(Ease.InOutSine).OnComplete(() => { _mono.StartCoroutine(Move(n)); });
        }

        IEnumerator Move(string name)
        {
            SpineManager.instance.DoAnimation(gamer, name + "2", true);

            gamer.GetComponent<RectTransform>().DOScale(Vector2.one * 0.75f, 3f).SetEase(Ease.InQuad);
            _mono.StartCoroutine(Wait(0.1f, ()=> gamer.GetComponent<RectTransform>().DOAnchorPosX(-150, 2.9f).SetEase(Ease.Linear)));

            yield return new WaitForSeconds(3f);

            float waitTime = 0.75f;
            SpineManager.instance.DoAnimation(gamer, name, true);

            _mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
            {
                gamer.SetActive(false);
                xem.SetActive(false);
                _mask.SetActive(true);

                float _time = 1.5f;

                switch (name)
                {
                    case "yc":

                        show1.SetActive(true);

                        InitSpine(show1.transform.GetChild(1).gameObject, "yc", false);

                        _mono.StartCoroutine(Wait(_time, ()=>
                        {
                            _mono.StartCoroutine(Wait(0.3f, ()=>PlaySound(2)));

                            _mono.StartCoroutine(Wait(waitTime, () =>
                            {
                                show1.transform.GetChild(0).gameObject.SetActive(false);
                            }));

                            SpineManager.instance.DoAnimation(show1.transform.GetChild(1).gameObject, "yc3", false, () =>
                            {
                                SpineManager.instance.DoAnimation(show1.transform.GetChild(1).gameObject, "yc", false);

                                _mono.StartCoroutine(Wait(1f, () =>
                                _mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
                                NextGame("ng", show1)))));
                            });
                        }));
                        break;

                    case "ng":
                        
                        show2.SetActive(true);
                        InitSpine(show2.transform.GetChild(1).gameObject, "ng", false);

                        _mono.StartCoroutine(Wait(_time, () =>
                        {
                            _mono.StartCoroutine(Wait(waitTime, () =>
                            {
                                PlaySound(3);

                                SpineManager.instance.DoAnimation(show2.transform.GetChild(2).gameObject, "xem-boom", false);
                                show2.transform.GetChild(0).gameObject.SetActive(false);
                            }));

                            _mono.StartCoroutine(NgMove());

                            SpineManager.instance.DoAnimation(show2.transform.GetChild(1).gameObject, "ng3", false, () =>
                            {
                                SpineManager.instance.DoAnimation(show2.transform.GetChild(1).gameObject, "ng", false);

                                _mono.StartCoroutine(Wait(1f, () =>
                                _mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
                                NextGame("sjd", show2)))));
                            });
                        }));
                        break;

                    case "sjd":
                        
                        show3.SetActive(true);

                        InitSpine(show3.transform.GetChild(1).gameObject, "sjd4", false);

                        _mono.StartCoroutine(Wait(_time, () =>
                        {
                            PlaySound(4);

                            _mono.StartCoroutine(Wait(waitTime, () =>
                            {
                                show3.transform.GetChild(0).gameObject.SetActive(false);
                            }));

                            SpineManager.instance.DoAnimation(show3.transform.GetChild(1).gameObject, "sjd3", false, () =>
                            {
                                SpineManager.instance.DoAnimation(show3.transform.GetChild(1).gameObject, "sjd", false);

                                _mono.StartCoroutine(Wait(1f, () =>
                                _mono.StartCoroutine(BlackCurtainTransition(blackRaw, () =>
                                {
                                    show3.SetActive(false);

                                    _mono.StartCoroutine(Wait(1f, () =>
                                    GameSuccess()));
                                }))));
                            });
                        }));
                        break;
                }
            }));

            yield break;

        }

        //下一关
        void NextGame(string animation, GameObject obj)
        {
            obj.SetActive(false);
            _mask.SetActive(false);

            //关闭选项
            Box1.SetActive(false);
            Box2.SetActive(false);

            if (levelnumber == 1) Box2.SetActive(true);
            if (levelnumber == 2) Box3.SetActive(true);

            SpineManager.instance.DoAnimation(gamer, animation, true);

            gamer.transform.localPosition = gamerpos.localPosition;
            gamer.transform.localScale = gamerpos.localScale;
            gamer.SetActive(true);
            xem.SetActive(true);

            qiang.GetComponent<RectTransform>().anchoredPosition = new Vector2(78, 602);
            _canClick = true;
            levelnumber++;
        }

        IEnumerator NgMove()
        {
            float temp = 0;
            while (true)
            {
                temp += 10;
                show2.transform.GetChild(1).GetComponent<RectTransform>().anchoredPosition = new Vector2(306f + (-542) * temp / 300, -220f);
                yield return new WaitForSeconds(0.03f);
                if (temp == 300)
                    break;

            }
            yield break;
        }

        IEnumerator ChangeMove(GameObject obj, GameObject obj2)
        {

            float temp = 0;
            Vector2 objpos = obj.transform.localPosition;
            Vector2 objpos2 = obj2.transform.localPosition;
            obj.transform.GetChild(0).gameObject.SetActive(false);
            obj2.transform.GetChild(0).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "kong", false);
            SpineManager.instance.DoAnimation(obj2.transform.GetChild(1).gameObject, "kong", false);
            while (true)
            {
                temp += 1;
                obj.transform.localPosition = new Vector2(objpos.x + (objpos2.x - objpos.x) * temp / 25, objpos.y);
                obj2.transform.localPosition = new Vector2(objpos2.x + (objpos.x - objpos2.x) * temp / 25, objpos2.y);
                yield return new WaitForSeconds(0.02f);
                if (temp == 25)
                    break;
            }
            yield break;
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
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
                    PlayOnClickSound();
                    RemoveEvent(_replaySpine);
                    RemoveEvent(_okSpine);
                    var time = PlaySpine(_replaySpine, "fh");
                    Delay(time, () =>
                    {
                        _okSpine.Hide();
                        //PlayCommonBgm(8); //ToDo...改BmgIndex
                        PlayBgm(0);
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
                        _dDD.SetActive(true);
                        _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, _dDD, null, null)); ;
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
            PlaySpine(_successSpine, "6-12-z", () => { PlaySpine(_successSpine, "6-12-z2"); });
            PlaySpine(_spSpine, "kong", () => { PlaySpine(_spSpine, _spSpine.name); });
            Delay(4.0f, GameReplayAndOk);
        }

        void InitBox(GameObject obj, Transform tra, int num)
        {
            for (int i = 0; i < num; i++)
            {
                InitSpine(obj.transform.GetChild(i).GetChild(1).gameObject, "", false);
                obj.transform.Find(i.ToString()).transform.localPosition = tra.Find(i.ToString()).localPosition;
                obj.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
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
