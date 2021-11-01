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

    public class TD91244Part5
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

        private GameObject _dTT;

        private GameObject _sTT;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;


        private bool _isPlaying;

        private GameObject Box;
        private List<int> _BoxList;
        private int[] Boxnumber;
        private bool[] boolnumber;
        private bool _isdoing;
        private bool _canClick;
        private int[] _jugle;
        private int time;
        private GameObject sz;
        private GameObject lz;
        private GameObject xem;
        private GameObject hua;
        private Transform curTrans;
        private GameObject em;
        private Vector2 xempos;
        void Start(object o)
        {
            _curGo = (GameObject)o;
            _mono = _curGo.GetComponent<MonoBehaviour>();
             curTrans = _curGo.transform;

            _mask = curTrans.GetGameObject("mask");
            _replaySpine = curTrans.GetGameObject("replaySpine");
            _startSpine = curTrans.GetGameObject("startSpine");
            _okSpine = curTrans.GetGameObject("okSpine");
            _successSpine = curTrans.GetGameObject("successSpine");
            _spSpine = curTrans.GetGameObject("successSpine/sp");

            _dTT = curTrans.GetGameObject("dTT");
            _sTT = curTrans.GetGameObject("sTT");

            Box = curTrans.Find("Box").gameObject;
            sz = curTrans.Find("sz").GetChild(0).gameObject;
            lz = curTrans.Find("BG").GetChild(1).gameObject;
            em = curTrans.Find("em").gameObject;
            xem = curTrans.Find("em").GetChild(0).gameObject;
            hua = curTrans.Find("hua").gameObject;
            xempos = curTrans.Find("xempos").localPosition;
            _BoxList = new List<int>();
            _jugle = new int[2];
            Boxnumber = new int[12];
            boolnumber = new bool[12];
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

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _dTT.Hide();
            _sTT.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);

            for (int i = 0; i < 12; i++)
            {
                _BoxList.Add(i);
                Util.AddBtnClick(Box.transform.GetChild(i).gameObject, Click);
            }
            for (int i = 0; i < 12; i++)
            {
                Boxnumber[i] = _BoxList[Random.Range(0, _BoxList.Count)];
                _BoxList.Remove(Boxnumber[i]);
                boolnumber[i] = false;
            }
            _jugle[0] = -1;
            _jugle[1]= -1;
            time = 15;
            sz.transform.parent.gameObject.SetActive(true);
            sz.GetComponent<RawImage>().texture = sz.GetComponent<BellSprites>().texture[15];
            sz.GetComponent<RawImage>().SetNativeSize();
            SpineManager.instance.DoAnimation(xem, "xem", true);
            for (int i = 0; i < Box.transform.childCount; i++)
            {
                Box.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
                Box.transform.GetChild(i).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(i).gameObject,"pai",false);
                Box.transform.GetChild(i).GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                Box.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                Box.transform.GetChild(i).GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            em.transform.localPosition = curTrans.Find("empos").localPosition;
            em.transform.rotation = Quaternion.Euler(0, 0, 0);
            xem.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xem,"xem",true);
            sz.transform.parent.GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hua.SetActive(false);
            hua.transform.GetChild(1).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            hua.transform.GetChild(3).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _canClick = false;
            lz.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(lz,"lianzi",false);
        }

        private void Click(GameObject obj)
        {
            if (_isdoing || !_canClick)
                return;
            PlaySound(5);
            if (!boolnumber[Convert.ToInt32(obj.name)])
            {
                _isdoing = true;
                show(Convert.ToInt32(obj.name));
                boolnumber[Convert.ToInt32(obj.name)] = true;
                _mono.StartCoroutine(wait(1.1f,
                    () =>
                    {
                        if (_jugle[0] == -1||_jugle[0]== Convert.ToInt32(obj.name))
                        {
                            _jugle[0] = Convert.ToInt32(obj.name);
                            _isdoing = false;
                        }
                        else
                        {
                            _jugle[1] = Convert.ToInt32(obj.name);
                            Jugle();
                        }
                    }
                    ));
            }
            else
            {
                _isdoing = true;
                showback(Convert.ToInt32(obj.name));
                boolnumber[Convert.ToInt32(obj.name)] = false;
                _jugle[0] = -1;
            }
        }

        private void Jugle()
        {
            if (Boxnumber[_jugle[0]] + Boxnumber[_jugle[1]] == 11)
            {
                PlaySound(8);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(1).gameObject, "right", false);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(1).gameObject, "right", false,
                    () => { SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(1).gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(1).gameObject, "kong", false);
                    }
                    );
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(0).gameObject, "right1", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(0).gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(0).gameObject, "kong", false);
                        Box.transform.GetChild(_jugle[0]).GetComponent<Spine.Unity.SkeletonGraphic>().color = new Vector4(255,255, 255,0);
                        Box.transform.GetChild(_jugle[1]).GetComponent<Spine.Unity.SkeletonGraphic>().color = new Vector4(255, 255, 255, 0);
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(1).gameObject, "yan", false,
                            () =>
                            {
                                Box.transform.GetChild(_jugle[0]).gameObject.SetActive(false);
                                Box.transform.GetChild(_jugle[1]).gameObject.SetActive(false);
                                _isdoing = false;
                                _jugle[0] = -1;
                                _jugle[1] = -1;
                                JugleWin();
                            });
                    }
                    );
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(0).gameObject, "right1", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(1).gameObject, "yan", false);
                    }
                    );
            }
            else
            {
                PlaySound(9);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(1).gameObject, "wrong", false);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(1).gameObject, "wrong", false,
                    () => {
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(1).gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(1).gameObject, "kong", false);
                    }
                    );
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(0).gameObject, "wrong1", false);
                SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(0).gameObject, "wrong1", false,
                    () => 
                    {
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[0]).GetChild(0).gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(Box.transform.GetChild(_jugle[1]).GetChild(0).gameObject, "kong", false);
                        showback(_jugle[0]);
                        showback(_jugle[1]);
                        SpineManager.instance.DoAnimation(xem,"xem4",false,
                            () => { SpineManager.instance.DoAnimation(xem, "xem", true); }
                            );
                        boolnumber[_jugle[0]] = false;
                        boolnumber[_jugle[1]] = false;
                        _jugle[0] = -1;
                        _jugle[1] = -1;
                       
                    });
            }
        }

        private void JugleWin()
        {
            for (int i = 0; i < boolnumber.Length; i++)
            {
                if (!boolnumber[i])
                    return;
            }
            SpineManager.instance.DoAnimation(lz, "lianzi2", false,
                () =>
                {
                    hua.SetActive(true);
                    Delay(PlaySound(6), () => { PlaySound(6); });
                    SpineManager.instance.DoAnimation(hua.transform.GetChild(1).gameObject,"ren",true);
                    SpineManager.instance.DoAnimation(hua.transform.GetChild(3).gameObject,"shengbo",true);
                    SpineManager.instance.DoAnimation(xem,"xem3",true);
                    em.GetComponent<RectTransform>().DOAnchorPos(xempos, 2f);
                    em.transform.DORotate(new Vector3(0, 0, 270), 2f);
                    _mono.StartCoroutine(wait(5f,()=> { GameSuccess(); }));
                    //_mono.StartCoroutine(moveandrotate(4f));
                });
        }

        IEnumerator moveandrotate(float time)
        {
            float temp = 0;
            while(true)
            {
                yield return new WaitForSeconds(0.01f);
                temp += 200;
                em.transform.rotation = Quaternion.Euler(0,0,temp/50);
                em.transform.localPosition = new Vector2(em.transform.localPosition.x+(xempos.x- em.transform.localPosition.x)*(temp/(20000*time)), em.transform.localPosition.y + (xempos.y - em.transform.localPosition.y) * (temp / (20000 * time)));
                if(temp==20000*time)
                {
                    break;
                }
            }
            GameSuccess();
            yield break;
        }
        private void show(int number)
        {
            string temp = string.Empty;
            switch (Boxnumber[number])
            {
                case 0:
                    temp = "A";
                    break;
                case 1:
                    temp = "B";
                    break;
                case 2:
                    temp = "G";
                    break;
                case 3:
                    temp = "E";
                    break;
                case 4:
                    temp = "H";
                    break;
                case 5:
                    temp = "I";
                    break;
                case 6:
                    temp = "J";
                    break;
                case 7:
                    temp = "L";
                    break;
                case 8:
                    temp = "F";
                    break;
                case 9:
                    temp = "K";
                    break;
                case 10:
                    temp = "D";
                    break;
                case 11:
                    temp = "C";
                    break;
            }
            SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai" + temp + "2", false,
                        () => { SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai" + temp, false);  }
                        );
                }
                );
        }
        private void showback(int number)
        {
            string temp = string.Empty;
            switch (Boxnumber[number])
            {
                case 0:
                    temp = "A";
                    break;
                case 1:
                    temp = "B";
                    break;
                case 2:
                    temp = "G";
                    break;
                case 3:
                    temp = "E";
                    break;
                case 4:
                    temp = "H";
                    break;
                case 5:
                    temp = "I";
                    break;
                case 6:
                    temp = "J";
                    break;
                case 7:
                    temp = "L";
                    break;
                case 8:
                    temp = "F";
                    break;
                case 9:
                    temp = "K";
                    break;
                case 10:
                    temp = "D";
                    break;
                case 11:
                    temp = "C";
                    break;
            }

            SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai" + temp + "3", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai3", false,
                        () => { SpineManager.instance.DoAnimation(Box.transform.GetChild(number).gameObject, "pai", false); _isdoing = false; _canClick = true; }
                        );
                }
                );

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
                        PlaySound(3, true);
                        _startSpine.Hide();
                        _sTT.SetActive(true);
                        BellSpeck(_sTT, 0, null,ShowVoiceBtn );

                     

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
                    BellSpeck(_sTT,1,null,
                        () => {
                            _mask.SetActive(false);
                            _sTT.SetActive(false);
                            StartGame();
                        }
                        );
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
            _mono.StartCoroutine(wait(1f,()=> { PlaySound(7); }));
            _mono.StartCoroutine(TimeUp());
            _mask.Hide();
            PlaySound(4);
            for (int i = 0; i < Box.transform.childCount; i++)
            {
                show(i);
            }
            _mono.StartCoroutine(wait(15f,
                () =>
                {
                    //sz.transform.parent.gameObject.SetActive(false);
                    for (int i = 0; i < Box.transform.childCount; i++)
                    {
                        showback(i);
                    }
                    sz.transform.parent.gameObject.SetActive(false);
                }
                ));
            //测试代码记得删

        }

        IEnumerator TimeUp()
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);
                time--;
                sz.GetComponent<RawImage>().texture = sz.GetComponent<BellSprites>().texture[time];
                sz.GetComponent<RawImage>().SetNativeSize();
                SpineManager.instance.DoAnimation(sz.transform.parent.GetChild(1).gameObject,"shizhong",false);
                if(time ==0)
                {
                    break;
                }
            }
            yield break;
        }

        IEnumerator wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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
                        
                        _mask.SetActive(false);
                        GameInit();
                        PlaySound(3, true);
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

                        _dTT.SetActive(true);
                        BellSpeck(_dTT, 2, null, null);
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
