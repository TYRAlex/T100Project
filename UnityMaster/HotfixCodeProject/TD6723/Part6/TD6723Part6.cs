using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }

    public class TD6723Part6
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        #region 田丁

        private GameObject dtt;
        private GameObject tt;
        private GameObject em;

        #endregion

        #region Mask

        private Transform anyBtns;
        private GameObject mask;
        private GameObject _aniMask;

        #endregion

        #region 成功

        private GameObject successSpine;

        private GameObject caidaiSpine;

        //胜利动画名字
        private string tz;
        private string sz;

        #endregion


        private bool isend = false;

        #region game

        private GameObject _page0;
        private GameObject _score;
        private GameObject _earth;
        private GameObject _moon;
        private GameObject _jupiter;
        private GameObject _mars;
        private GameObject _hj;

        private bool isOnClick;
        private Empty4Raycast[] _empty4;
        private List<mILDrager> _dragerList;
        private List<mILDroper> _droperList;
        private int score;

        private GameObject _dj;
        private GameObject _xem;

        private bool earthOnclick;
        private bool moonOnclick;
        private bool marsOnclick;
        private bool jupiterOnclick;
        private GameObject _next;

        private List<Vector3> _vector3s;

        #endregion

        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            //田丁加载游戏物体方法
            TDLoadGameProperty();


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            Debug.LogError("Width:"+Screen.width+"   Hieght"+Screen.height);
            //GameStart();
            IsStart();
        }

        void IsStart()
        {
            mask.Show();
            anyBtns.gameObject.Show();
            anyBtns.GetChild(0).gameObject.Show();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }


        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            isOnClick = false;
            talkIndex = 1;
            isend = false;
            InitGame();
            for (int i = 0; i < _dragerList.Count; i++)
            {
                var temp = _dragerList[i];
                temp.SetDragCallback(StartDrag, null, EndDraging);
            }
        }


        void GameStart()
        {
            mask.Show();
            em.Show();
            tt.Hide();
            dtt.Hide();
            PlayBGM();
            Talk(em, 0, null, () =>
            {
                em.Hide();
                tt.Show();
                Talk(tt, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); });
            });
        }


        #region 说话语音

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex,
            Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = tt;
            }

            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        #endregion

        #region 语音键对应方法

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    Talk(tt, 2, null, () =>
                    {
                        mask.Hide();
                        tt.Hide();
                    });

                    break;
            }

            talkIndex++;
        }

        #endregion

        #region 通用方法

        /// <summary>
        /// 播放Spine动画
        /// </summary>
        /// <param name="target">目标对象</param>
        /// <param name="name">目标名字</param>
        /// <param name="isLoop">是否循环</param>
        /// <param name="callback">完成之后回调</param>
        private void PlaySpineAni(GameObject target, string name, bool isLoop = false, Action callback = null)
        {
            SpineManager.instance.DoAnimation(target, name, isLoop, callback);
        }

        /// <summary>
        /// Bell说话
        /// </summary>
        /// <param name="index">Voice语音下标</param>
        /// <param name="goingEvent">同步执行的方法</param>
        /// <param name="finishEvent">完成回调</param>
        private void Talk(GameObject target, int index, Action goingEvent = null, Action finishEvent = null)
        {
            target.Show();
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent,
                finishEvent));
        }

        /// <summary>
        /// 播放语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        /// <param name="callback">播放完成回调</param>
        private void PlayVoice(int targetIndex, Action callback = null)
        {
            float voiceTimer = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, targetIndex);
            if (callback != null)
                WaitTimeAndExcuteNext(voiceTimer, callback);
        }

        /// <summary>
        /// 播放相应的Sound语音
        /// </summary>
        /// <param name="targetIndex">语音下标</param>
        private void PlaySound(int targetIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, targetIndex);
        }

        /// <summary>
        /// 延时执行
        /// </summary>
        /// <param name="timer"></param>
        /// <param name="callback"></param>
        void WaitTimeAndExcuteNext(float timer, Action callback)
        {
            mono.StartCoroutine(WaitTimeAndExcuteNextIE(timer, callback));
        }

        IEnumerator WaitTimeAndExcuteNextIE(float timer, Action callBack)
        {
            yield return new WaitForSeconds(timer);
            callBack?.Invoke();
        }


        /// <summary>
        /// 播放BGM（用在只有一个BGM的时候）
        /// </summary>
        private void PlayBGM()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }

        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail(Action method = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
            WaitTimeAndExcuteNext(time, method);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess(Action method = null)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
            WaitTimeAndExcuteNext(time, method);
        }


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

        #endregion


        #region 田丁

        #region 田丁加载

        /// <summary>
        /// 田丁加载所有物体
        /// </summary>
        void TDLoadGameProperty()
        {
            mask = curTrans.Find("mask").gameObject;
            _aniMask = curTrans.GetGameObject("animask");
            _aniMask.Hide();
            mask.SetActive(false);
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();
            //加载游戏环节
            LoadGame();
        }

        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            dtt = curTrans.Find("mask/DTT").gameObject;
            tt = curTrans.Find("mask/TT").gameObject;
            em = curTrans.Find("mask/em").gameObject;
            dtt.Hide();
            tt.Hide();
            em.Hide();
        }

        /// <summary>
        /// 游戏加载
        /// </summary>
        void LoadGame()
        {
            _page0 = curTrans.GetGameObject("page0");
            _score = curTrans.GetGameObject("score");
            _earth = curTrans.GetGameObject("earth");
            _mars = curTrans.GetGameObject("mars");
            _moon = curTrans.GetGameObject("moon");
            _jupiter = curTrans.GetGameObject("jupiter");
            _hj = curTrans.GetGameObject("hj");
            _empty4 = _page0.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            _xem = curTrans.GetGameObject("xem");
            _dj = curTrans.GetGameObject("dj");
            _next = curTrans.GetGameObject("next");
            Util.AddBtnClick(_next.transform.GetChild(0).gameObject, BackPage0);

            //drag 和 drop 
            _dragerList = new List<mILDrager>();
            _droperList = new List<mILDroper>();
            InitDropAndDrag(_earth);
            InitDropAndDrag(_moon);
            InitDropAndDrag(_mars);
            InitDropAndDrag(_jupiter);
        }

        /// <summary>
        /// 返回
        /// </summary>
        /// <param name="obj"></param>
        private void BackPage0(GameObject obj)
        {
            if (!isOnClick)
            {
                isOnClick = true;
                BtnPlaySound();
                PlaySpineAni(obj.transform.parent.gameObject, obj.name, false, () =>
                {
                    PlaySpineAni(obj.transform.parent.gameObject, "kong", false, () =>
                    {
                        _next.Hide();
                        _page0.Show();
                        _hj.Show();
                        _score.Hide();
                        /*for (int i = 0;
                            i < _empty4.Length;
                            i++)
                        {
                            Util.AddBtnClick(_empty4[i].gameObject, OnClickPlanet);
                            var child = _empty4[i].transform;
                            WaitTimeAndExcuteNext(Random.Range(0.1f, 1f),
                                () => { PlaySpineAni(child.parent.gameObject, child.parent.name, true); });
                        }

                        PlaySpineAni(_page0.transform.GetGameObject("lx"), "lx", true, () => { });
                        PlaySpineAni(_hj.transform.GetChild(0).gameObject, "hj", true);*/
                        _earth.Hide();
                        _mars.Hide();
                        _moon.Hide();
                        _jupiter.Hide();
                        _xem.Hide();
                        _dj.Hide();
                        mask.Hide();
                        _aniMask.Hide();
                    });
                });
            }
        }

        private void BackPage0()
        {
            // for (int i = 0; i < _empty4.Length; i++)
            // {
            //     var child = _empty4[i].transform;
            //     WaitTimeAndExcuteNext(Random.Range(0.1f, 1f),
            //         () => { PlaySpineAni(child.parent.gameObject, child.parent.name, true); });
            // }

            _next.Hide();
            _page0.Show();
            _hj.Show();
            _score.Hide();
            _earth.Hide();
            _mars.Hide();
            _moon.Hide();
            _jupiter.Hide();
            _xem.Hide();
            _dj.Hide();
            mask.Hide();
            _aniMask.Hide();
        }


        private int level;

        /// <summary>
        /// 开始拖拽
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        private void StartDrag(Vector3 pos, int type, int index)
        {
            level = _dragerList[index].transform.GetSiblingIndex();
            _dragerList[index].transform.SetAsLastSibling();
            _dragerList[index].GetComponent<Image>().sprite = _dragerList[index].GetComponent<BellSprites>().sprites[1];
            _dragerList[index].GetComponent<Image>().SetNativeSize();
        }


        /// <summary>
        /// 结束拖拽
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="arg4"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void EndDraging(Vector3 endpos, int type, int index, bool istrue)
        {
            _aniMask.Show();
            if (istrue)
            {
                var child = _droperList[index].transform.GetChild(0).gameObject;
                child.Show();
                _dragerList[index].gameObject.Hide();
                PlaySpineAni(child, child.name, false);

                int number = 0;
                for (int i = 0; i < _dragerList[index].transform.parent.transform.childCount; i++)
                {
                    var temp = _dragerList[index].transform.parent.transform.GetChild(i).gameObject;
                    if (!temp.activeSelf)
                    {
                        number++;
                        if (number == _dragerList[index].transform.parent.transform.childCount)
                        {
                            score++;

                            BtnPlaySoundSuccess();
                            _dj.Show();


                            WaitTimeAndExcuteNext(0.5f, () => { PlaySound(0); });

                            WaitTimeAndExcuteNext(1f, () =>
                            {
                                WaitTimeAndExcuteNext(0.5f, () => { PlaySound(2); });
                                WaitTimeAndExcuteNext(0.7f, () => { _xem.Hide(); });

                                PlaySpineAni(_dj, "hj" + Random.Range(1, 3).ToString(), false, () =>
                                {
                                    PlaySpineAni(_dj, "kong");
                                    _score.transform.GetGameObject("number").GetComponent<Image>().sprite =
                                        _score.transform.GetGameObject("number").GetComponent<BellSprites>()
                                            .sprites[score];
                                    _score.transform.GetGameObject("number").GetComponent<Image>().SetNativeSize();
                                    _score.transform.GetChild(score - 1).gameObject.Hide();
                                    if (score < 4)
                                    {
                                        _next.Show();
                                        _aniMask.Hide();
                                        PlaySpineAni(_next, "next2", true);
                                        isOnClick = false;
                                    }
                                    else
                                    {
                                        //_aniMask.Show();
                                        isend = true;
                                        anyBtns.gameObject.Hide();
                                        WaitTimeAndExcuteNext(1, () =>
                                        {
                                            BackPage0();
                                            WaitTimeAndExcuteNext(2, () =>
                                            {
                                                tt.Hide();
                                                dtt.Hide();
                                                playSuccessSpine();
                                            });
                                        });
                                    }
                                });
                            });
                        }
                    }
                }

                if (number != _dragerList[index].transform.parent.transform.childCount)
                {
                    BtnPlaySoundSuccess(() => { _aniMask.Hide(); });
                }
            }

            else
            {
                BtnPlaySoundFail(() => { _aniMask.Hide(); });
                _dragerList[index].transform.localPosition = _vector3s[index];
                _dragerList[index].GetComponent<Image>().sprite =
                    _dragerList[index].GetComponent<BellSprites>().sprites[0];
                _dragerList[index].GetComponent<Image>().SetNativeSize();
            }

            _dragerList[index].transform.SetSiblingIndex(level);
        }


        /// <summary>
        /// 初始化drop和
        /// </summary>
        /// <param name="planet"></param>
        void InitDropAndDrag(GameObject planet)
        {
            var drage = planet.transform.GetGameObject("drage");
            for (int i = 0;
                i < drage.transform.childCount;
                i++)
            {
                var temp = drage.transform.GetChild(i).gameObject;
                _dragerList.Add(temp.GetComponent<mILDrager>());
            }

            var drop = planet.transform.GetChild(0).gameObject;
            for (int i = 1;
                i < drop.transform.childCount;
                i++)
            {
                var d = drop.transform.GetChild(i).gameObject;
                _droperList.Add(d.GetComponent<mILDroper>());
            }
        }


        /// <summary>
        /// 加载成功环节
        /// </summary>
        void TDLoadSuccessPanel()
        {
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
        }

        /// <summary>
        /// 加载按钮
        /// </summary>
        void TDLoadButton()
        {
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0;
                i < anyBtns.childCount;
                i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }

            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }

        #region 切换游戏按键方法

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            isOnClick = false;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                default:
                    break;
            }

            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (!isOnClick)
            {
                isOnClick = true;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            mask.SetActive(false);
                            GameInit();
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            switchBGM();
                            anyBtns.gameObject.SetActive(false);
                            dtt.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(dtt, SoundManager.SoundType.VOICE, 10));
                        });
                    }
                });
            }
        }

        #endregion


        #region 田丁成功动画

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            tt.Hide();
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            PlaySpineAni(anyBtns.GetChild(1).gameObject, "ok2", true);
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
        }

        #endregion

        #endregion

        #region 游戏初始化

        void InitGame()
        {
            _vector3s = new List<Vector3>();
            _vector3s.Clear();
            _page0.Show();
            _hj.Show();
            score = 0;
            _score.Hide();
            _earth.Hide();
            _mars.Hide();
            _moon.Hide();
            _jupiter.Hide();
            _next.Hide();
            _xem.Hide();
            _dj.Hide();
            mask.Hide();
            _aniMask.Hide();
            earthOnclick = false;
            moonOnclick = false;
            marsOnclick = false;
            jupiterOnclick = false;
            for (int i = 0;
                i < _empty4.Length;
                i++)
            {
                Util.AddBtnClick(_empty4[i].gameObject, OnClickPlanet);
                var child = _empty4[i].transform;
                WaitTimeAndExcuteNext(Random.Range(0.1f, 1f),
                    () => { PlaySpineAni(child.parent.gameObject, child.parent.name, true); });
            }

            PlaySpineAni(_page0.transform.GetGameObject("lx"), "lx", true, () => { });
            PlaySpineAni(_hj.transform.GetChild(0).gameObject, "hj", true);
            for (int i = 0; i < _score.transform.childCount; i++)
            {
                var child = _score.transform.GetChild(i).gameObject;
                child.Show();
                if (i == 4)
                {
                    child.GetComponent<Image>().sprite = child.GetComponent<BellSprites>().sprites[0];
                    child.GetComponent<Image>().SetNativeSize();
                }
            }

            IninPlant(_earth);
            IninPlant(_moon);
            IninPlant(_mars);
            IninPlant(_jupiter);
        }

        /// <summary>
        ///关卡初始化
        /// </summary>
        /// <param name="planet"></param>
        void IninPlant(GameObject planet)
        {
            for (int i = 0;
                i < planet.transform.childCount;
                i++)
            {
                var child = planet.transform.GetChild(i).gameObject;
                if (i == 0 || i == 1)
                {
                    WaitTimeAndExcuteNext(Random.Range(0.1f, 0.5f), () => { PlaySpineAni(child, child.name, true); });
                }
            }

            var drage = planet.transform.GetGameObject("drage");
            for (int i = 0; i < drage.transform.childCount;
                i++)
            {
                var temp = drage.transform.GetChild(i).gameObject;
                temp.GetComponent<Image>().sprite = temp.GetComponent<BellSprites>().sprites[0];
                temp.GetComponent<Image>().SetNativeSize();
                var p = Random.Range(0, 2);
                if (p == 0)
                {
                    var pos = new Vector3(Random.Range(350f, 900f), Random.Range(-480f, 480f));
                    temp.transform.localPosition = pos;

                    for (int j = 0; j < temp.transform.parent.childCount; j++)
                    {
                        if (temp.transform.parent.GetChild(j).name != temp.name)
                        {
                            var dis = Vector3.Distance(temp.transform.position,
                                temp.transform.parent.GetChild(j).transform.position);
                            if (dis <= 200)
                            {
                                pos = new Vector3(Random.Range(290f, 900f), Random.Range(-480f, 480f));
                                temp.transform.localPosition = pos;
                            }
                        }
                    }

                    _vector3s.Add(pos);
                }
                else
                {
                    var pos = new Vector3(Random.Range(-660f, -360f), Random.Range(-480f, 200f));
                    temp.transform.localPosition = pos;
                    for (int j = 0; j < temp.transform.parent.childCount; j++)
                    {
                        if (temp.transform.parent.GetChild(j).name != temp.name)
                        {
                            var dis = Vector3.Distance(temp.transform.position,
                                temp.transform.parent.GetChild(j).transform.position);
                            if (dis <= 200)
                            {
                                pos = new Vector3(Random.Range(-660f, -360f), Random.Range(-480f, 200f));
                                temp.transform.localPosition = pos;
                            }
                        }
                    }

                    _vector3s.Add(pos);
                }

                temp.Show();
            }

            _hj.transform.localPosition = new Vector3(-40, -380);
            _hj.transform.localScale = new Vector3(1, 1, 1);

            var drop = planet.transform.GetChild(0).gameObject;
            for (int i = 0;
                i < drop.transform.childCount;
                i++)
            {
                var d = drop.transform.GetChild(i).gameObject;
                d.transform.GetChild(0).gameObject.Hide();
            }
        }


        /// <summary>
        /// 星球点击
        /// </summary>
        /// <param name="obj"></param>
        /// <exception cref="NotImplementedException"></exception>
        private void OnClickPlanet(GameObject obj)
        {
            int index;
            _aniMask.Show();
            if (Input.mousePosition.x > _hj.transform.position.x)
            {
                _hj.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                _hj.transform.localScale = new Vector3(1, 1, 1);
            }

            if (!isend)
            {
                switch (obj.name)
                {
                    case "a2":
                        index = 3;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () =>
                            {
                                if (!earthOnclick)
                                {
                                    earthOnclick = true;
                                    PlaySound(1);
                                    PlaySpineAni(obj.transform.parent.gameObject, obj.name, false, () =>
                                    {
                                        PlaySpineAni(obj.transform.parent.gameObject, obj.transform.parent.name, true,
                                            () => { });
                                        _aniMask.Hide();
                                        _page0.Hide();
                                        _score.Show();
                                        _earth.Show();
                                        _xem.Show();
                                        PlaySpineAni(_xem, "xem", true);
                                        var child = _earth.transform.GetChild(1).gameObject;
                                        SpineManager.instance.DoAnimation(child, child.name, true);
                                        _hj.Hide();
                                    });
                                }
                                else
                                {
                                    _aniMask.Hide();
                                }
                            });
                        });
                        break;
                    case "b2":
                        index = 4;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () =>
                            {
                                if (!moonOnclick)
                                {
                                    moonOnclick = true;
                                    PlaySound(1);
                                    PlaySpineAni(obj.transform.parent.gameObject, obj.name, false, () =>
                                    {
                                        PlaySpineAni(obj.transform.parent.gameObject, obj.transform.parent.name, true,
                                            () => { });
                                        _aniMask.Hide();
                                        _page0.Hide();
                                        _score.Show();
                                        _moon.Show();
                                        _xem.Show();
                                        PlaySpineAni(_xem, "xem", true);
                                        var child = _moon.transform.GetChild(1).gameObject;
                                        PlaySpineAni(child, child.name, true);
                                        ;
                                        _hj.Hide();
                                    });
                                }
                                else
                                {
                                    _aniMask.Hide();
                                }
                            });
                        });
                        break;
                    case "c2":
                        index = 5;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () =>
                            {
                                if (!marsOnclick)
                                {
                                    marsOnclick = true;
                                    PlaySound(1);
                                    PlaySpineAni(obj.transform.parent.gameObject, obj.name, false, () =>
                                    {
                                        _aniMask.Hide();
                                        PlaySpineAni(obj.transform.parent.gameObject, obj.transform.parent.name, true,
                                            () => { });
                                        _page0.Hide();
                                        _score.Show();
                                        _mars.Show();
                                        _xem.Show();
                                        PlaySpineAni(_xem, "xem", true);
                                        var child = _mars.transform.GetChild(1).gameObject;
                                        PlaySpineAni(child, child.name, true);
                                        _hj.Hide();
                                    });
                                }
                                else
                                {
                                    _aniMask.Hide();
                                }
                            });
                        });
                        break;
                    case "d2":
                        index = 6;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () =>
                            {
                                if (!jupiterOnclick)
                                {
                                    jupiterOnclick = true;
                                    PlaySound(1);
                                    PlaySpineAni(obj.transform.parent.gameObject, obj.name, false, () =>
                                    {
                                        PlaySpineAni(obj.transform.parent.gameObject, obj.transform.parent.name, true,
                                            () => { });
                                        _aniMask.Hide();
                                        _page0.Hide();
                                        _score.Show();
                                        _jupiter.Show();
                                        _xem.Show();
                                        PlaySpineAni(_xem, "xem", true);
                                        var child = _jupiter.transform.GetChild(1).gameObject;
                                        PlaySpineAni(child, child.name, true);
                                        _hj.Hide();
                                    });
                                }
                                else
                                {
                                    _aniMask.Hide();
                                }
                            });
                        });
                        break;
                    case "e":
                        index = 7;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () => { _aniMask.Hide(); });
                        });
                        break;
                    case "f":
                        index = 11;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () => { _aniMask.Hide(); });
                        });
                        break;
                    case "g":
                        index = 9;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () => { _aniMask.Hide(); });
                        });
                        break;
                    case "h":
                        index = 8;
                        _hj.transform.DOMove(Input.mousePosition, 1f).OnComplete(() =>
                        {
                            Talk(tt, index, null, () => { _aniMask.Hide(); });
                        });
                        break;
                }
            }
        }

        #endregion

        #endregion
    }
}