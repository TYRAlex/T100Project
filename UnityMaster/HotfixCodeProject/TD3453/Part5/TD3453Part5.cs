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

    public class TD3453Part5
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

        private GameObject _dBD;

        private GameObject _sBD;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        private bool _isPlaying;
        bool _isPause;

        private GameObject ygbj;
        private float radius;
        private Vector3 stickPos;
        private RectTransform content;
        private MonoScripts _monos;
        private GameObject yu;
        private Transform yupos;
        private Transform xempos;
        private Rigidbody2D yuspeed;
        private bool _canc;
        private bool _canmove;
        private int number;
        private GameObject level1;
        private GameObject level2;
        private GameObject level3;
        private GameObject tiao;
        private int levelnumber;

        private EventDispatcher eventDispatcher;
        private GameObject yushow;
        private GameObject xemshow;

        private Vector2[] _point;
        private GameObject mymask;

        BoxCollider2D yuCollider2D;

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

            _dBD = curTrans.GetGameObject("dBD");
            _sBD = curTrans.GetGameObject("sBD");

            level1 = curTrans.Find("level1").gameObject;
            level2 = curTrans.Find("level2").gameObject;
            level3 = curTrans.Find("level3").gameObject;
            tiao = curTrans.Find("tiao").GetChild(1).gameObject;
            ygbj = curTrans.Find("ygbj").gameObject;
            content = ygbj.GetComponent<ScrollRect>().content;
            radius = ygbj.GetComponent<ScrollRect>().viewport.rect.width / 2;
            _monos = curTrans.Find("monos").GetComponent<MonoScripts>();
            _monos.UpdateCallBack = Mys;
            yu = curTrans.Find("yu").gameObject;
            yuspeed = yu.GetComponent<Rigidbody2D>();
            yushow = curTrans.Find("yushow").gameObject;
            xemshow = curTrans.Find("xemshow").gameObject;
            yupos = curTrans.Find("yupos");
            xempos = curTrans.Find("xempos");
            eventDispatcher = yu.GetComponent<EventDispatcher>();

            mymask = curTrans.Find("mymask").gameObject;

            yuCollider2D = yu.GetComponent<BoxCollider2D>();


            //for (int i = 0; i < level1.transform.childCount; i++)
            //{
            //    eventDispatchers1[i] = level1.transform.GetChild(i).GetComponent<EventDispatcher>();
            //}

            //_point = new Vector2[5];
            //_point[0] = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
            //_point[1] = new Vector2(0.5f * Screen.width, -0.5f * Screen.height);
            //_point[2] = new Vector2(-0.5f * Screen.width, -0.5f * Screen.height);
            //_point[3] = new Vector2(-0.5f * Screen.width, 0.5f * Screen.height);
            //_point[4] = new Vector2(0.5f * Screen.width, 0.5f * Screen.height);
            //curTrans.Find("bj").GetComponent<EdgeCollider2D>().points = _point;

            SetFloorEc(curTrans);
            GameInit();
            GameStart();
        }

        //根据分辨率初始化(边界)
        void SetFloorEc(Transform tra)
        {
            var sD = tra.GetRectTransform().sizeDelta;
            var w = sD.x / 2;
            var h = sD.y / 2;
            float offset = 0f;

            Vector2[] tempPoints = new Vector2[5];
            tempPoints[0] = new Vector2(w, h);
            tempPoints[1] = new Vector2(w, -h + offset);
            tempPoints[2] = new Vector2(-w, -h + offset);
            tempPoints[3] = new Vector2(-w, h);
            tempPoints[4] = new Vector2(w, h);


            tra.Find("bj").GetComponent<EdgeCollider2D>().points = tempPoints;
        }

        void InitData()
        {
            _isPlaying = true;
            _isPause = true;
            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };
        }

        void GameInit()
        {
            InitData();

            mymask.SetActive(false);
            _talkIndex = 1;
            HideVoiceBtn();
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            StopAllAudio();
            StopAllCoroutines();

            _mask.Hide(); _replaySpine.Hide(); _startSpine.Hide(); _okSpine.Hide(); _successSpine.Hide();
            _dBD.Hide();
            _sBD.Hide();
            RemoveEvent(_startSpine); RemoveEvent(_okSpine); RemoveEvent(_replaySpine);
            _canc = true;
            _canmove = true;
            number = 0;
            tiao.GetComponent<Image>().fillAmount = number / 10f;
            levelnumber = 1;
            level1.SetActive(true);
            level2.SetActive(false);
            level3.SetActive(false);
            yu.SetActive(true);
            yu.transform.localRotation = yupos.localRotation;
            yu.transform.localPosition = yupos.localPosition;
            eventDispatcher.CollisionEnter2D += Touch;
            SpineManager.instance.DoAnimation(yu, "yu-s3", true);
            ygbj.GetComponent<RawImage>().texture = ygbj.GetComponent<BellSprites>().texture[0];
            ygbj.transform.GetChild(0).GetComponent<RawImage>().texture = ygbj.transform.GetChild(0).GetComponent<BellSprites>().texture[0];
            ygbj.transform.GetChild(0).GetComponent<RawImage>().raycastTarget = false;
            tiao.transform.parent.GetChild(2).GetComponent<RawImage>().texture = tiao.transform.parent.GetChild(2).GetComponent<BellSprites>().texture[0];
            yushow.SetActive(false);
            xemshow.SetActive(false);
            yushow.GetComponent<SkeletonGraphic>().Initialize(true);
            yushow.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            yushow.transform.GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
            xemshow.GetComponent<SkeletonGraphic>().Initialize(true);
            xemshow.transform.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            content.gameObject.transform.localPosition = new Vector3(0, 0, 0);
            for (int i = 0; i < 16; i++)
            {
                level1.transform.GetChild(i).gameObject.SetActive(true);
                level2.transform.GetChild(i).gameObject.SetActive(true);
                level3.transform.GetChild(i).gameObject.SetActive(true);
                Physics2D.IgnoreCollision(yu.GetComponent<Collider2D>(), level1.transform.GetChild(i).GetComponent<Collider2D>(), false);
                Physics2D.IgnoreCollision(yu.GetComponent<Collider2D>(), level2.transform.GetChild(i).GetComponent<Collider2D>(), false);
                Physics2D.IgnoreCollision(yu.GetComponent<Collider2D>(), level3.transform.GetChild(i).GetComponent<Collider2D>(), false);
            }

            _mono.StartCoroutine(shake(level1));
            //for (int i = 0; i < 16; i++)
            //{
            //    eventDispatchers1[i].gameObject.SetActive(false);
            //    eventDispatchers1[i].gameObject.SetActive(true);
            //    eventDispatchers1[i].TriggerEnter2D += Trigger;
            //}

            level1.transform.Find("xem").GetComponent<BoxCollider2D>().enabled = true;
            level2.transform.Find("xem").GetComponent<BoxCollider2D>().enabled = true;
            level3.transform.Find("xem").GetComponent<BoxCollider2D>().enabled = true;

            yuCollider2D.enabled = true;
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
                        _sBD.SetActive(true);
                        _mask.SetActive(true);
                        _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, _sBD, null,
                            () =>
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                                _isPause = false;
                            }
                            ));
                        //ToDo...


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
                    _mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, _sBD, null,
                        () =>
                        {
                            _sBD.SetActive(false);
                            _mask.SetActive(false);
                            StartGame();
                        }
                        ));
                    break;
            }
            _talkIndex++;
        }

        #region 游戏逻辑

        private void Mys()
        {

            //位置到原点的模大于半径
            if (content.localPosition.magnitude > radius)
            {
                content.localPosition = content.localPosition.normalized * radius;
            }

            //移动方向标准化
            stickPos = content.localPosition.normalized;

            //不点击时归零
            if (!Input.GetMouseButton(0) || _isPause)
            {
                stickPos = Vector3.zero;
                content.localPosition = Vector3.zero;
            }

            //旋转🐟的方向
            if (content.localPosition.normalized.x < 0)
            {
                yu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 180, 0), 0.1f).SetEase(Ease.Linear);
            }

            if (content.localPosition.normalized.x > 0)
            {
                yu.transform.DOLocalRotateQuaternion(Quaternion.Euler(0, 0, 0), 0.1f).SetEase(Ease.Linear);
            }

            yuspeed.velocity = stickPos * 350f;
        }

        private void Touch(Collision2D c, int time)
        {
            if (c.collider.name == "bj") return;
            else
            {
                if (c.collider.name != "xem")
                {
                    PlaySound(0);
                    Collider2D temp = c.collider;
                    Physics2D.IgnoreCollision(c.otherCollider, temp);
                    number++;

                    _mono.StartCoroutine(DomenCollider(yu, 0.1f));

                    //进度条
                    float _value = 0;

                    if (levelnumber == 1)
                    {
                        _value = number / 10f;
                    }
                    else if (levelnumber == 2)
                    {
                        _value = number / 13f;
                    }
                    else if (levelnumber == 3)
                    {
                        _value = number / 16f;
                    }

                    tiao.GetComponent<Image>().DOFillAmount(_value, 0.5f).SetEase(Ease.Linear);

                    Jugle();

                    //球消失
                    SpineManager.instance.DoAnimation(temp.gameObject, "pao-p" + temp.gameObject.name, false, () =>
                    {
                        temp.gameObject.SetActive(false);
                    });
                }
                else
                {
                    PlaySound(1);
                    Collider2D temp = c.collider;

                    if (levelnumber == 1)
                    {
                        SpineManager.instance.DoAnimation(yu, "yu-s6", false,
                        () => { SpineManager.instance.DoAnimation(yu, "yu-s3", true); }
                        );
                    }
                    else if (levelnumber == 2)
                    {
                        SpineManager.instance.DoAnimation(yu, "yu-s5", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(yu, "yu-s2", true);
                        });
                    }
                    else if (levelnumber == 3)
                    {
                        SpineManager.instance.DoAnimation(yu, "yu-s4", false, () =>
                        {
                            SpineManager.instance.DoAnimation(yu, "yu-s", true);
                        });
                    }

                    float _time = SpineManager.instance.DoAnimation(temp.gameObject, "xem-jx", false, () =>
                    {
                        SpineManager.instance.DoAnimation(temp.gameObject, "xem2", true);
                    });

                    _mono.StartCoroutine(DomenCollider(temp.gameObject, _time + 0.5f));
                }
            }
        }

        //成功判断
        private void Jugle()
        {
            if (levelnumber == 1 && number == 10)
            {
                _isPause = true;

                mymask.SetActive(true);
                _mono.StartCoroutine(Wait(0.8f, () =>
                     {
                         yu.GetComponent<SkeletonGraphic>().Initialize(true);
                         yu.SetActive(false);
                         yushow.SetActive(true);
                         level1.SetActive(false);
                         PlaySound(2);
                         SpineManager.instance.DoAnimation(yushow.transform.GetChild(0).gameObject, "yu-m3", false);
                         SpineManager.instance.DoAnimation(yushow.transform.GetChild(1).gameObject, "star", true);
                         SpineManager.instance.DoAnimation(yushow, "light", false,
                             () =>
                             {
                                 SpineManager.instance.DoAnimation(yushow, "light", false,
                                     () =>
                                     {
                                         yushow.SetActive(false);
                                         levelnumber++;
                                         Xemshow(
                                             () =>
                                             {
                                                 number = 0;
                                                 tiao.GetComponent<Image>().fillAmount = number / 13f;
                                                 level1.SetActive(false);
                                                 level2.SetActive(true);
                                                 yu.SetActive(true);
                                                 eventDispatcher.CollisionEnter2D += Touch;
                                                 yu.transform.localPosition = yupos.localPosition;
                                                 yu.transform.localRotation = yupos.localRotation;
                                                 SpineManager.instance.DoAnimation(yu, "yu-s2", true);
                                                 _mono.StartCoroutine(shake(level2));
                                                 ygbj.GetComponent<RawImage>().texture = ygbj.GetComponent<BellSprites>().texture[1];
                                                 ygbj.transform.GetChild(0).GetComponent<RawImage>().texture = ygbj.transform.GetChild(0).GetComponent<BellSprites>().texture[1];
                                                 tiao.transform.parent.GetChild(2).GetComponent<RawImage>().texture = tiao.transform.parent.GetChild(2).GetComponent<BellSprites>().texture[1];
                                                 mymask.SetActive(false);
                                                 _isPause = false;
                                             }
                                             );
                                     });
                             }
                             );
                     }));
            }
            else if (levelnumber == 2 && number == 13)
            {
                mymask.SetActive(true);
                _mono.StartCoroutine(Wait(0.8f,
                    () =>
                    {
                        _isPause = true;
                        yu.SetActive(false);
                        yushow.SetActive(true);
                        level2.SetActive(false);
                        SpineManager.instance.DoAnimation(yushow.transform.GetChild(0).gameObject, "yu-m", false);
                        SpineManager.instance.DoAnimation(yushow.transform.GetChild(1).gameObject, "star", true);
                        SpineManager.instance.DoAnimation(yushow, "light", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(yushow, "light", false,
                                    () =>
                                    {
                                        yushow.SetActive(false);
                                        levelnumber++;
                                        Xemshow(
                                            () =>
                                            {
                                                number = 0;
                                                tiao.GetComponent<Image>().fillAmount = number / 16f;
                                                level2.SetActive(false);
                                                level3.SetActive(true);
                                                yu.SetActive(true);
                                                eventDispatcher.CollisionEnter2D += Touch;
                                                yu.transform.localPosition = yupos.localPosition;
                                                yu.transform.localRotation = yupos.localRotation;
                                                SpineManager.instance.DoAnimation(yu, "yu-s", true);
                                                _mono.StartCoroutine(shake(level3));
                                                ygbj.GetComponent<RawImage>().texture = ygbj.GetComponent<BellSprites>().texture[2];
                                                ygbj.transform.GetChild(0).GetComponent<RawImage>().texture = ygbj.transform.GetChild(0).GetComponent<BellSprites>().texture[2];
                                                tiao.transform.parent.GetChild(2).GetComponent<RawImage>().texture = tiao.transform.parent.GetChild(2).GetComponent<BellSprites>().texture[2];
                                                mymask.SetActive(false);
                                                _isPause = false;
                                            }
                                            );
                                    });
                            }
                            );
                    }
                    ));

            }
            else if (levelnumber == 3 && number == 16)
            {
                mymask.SetActive(true);
                _mono.StartCoroutine(Wait(0.8f, () =>
                 {
                     _isPause = true;
                     yu.SetActive(false);
                     yushow.SetActive(true);
                     level3.SetActive(false);
                     SpineManager.instance.DoAnimation(yushow.transform.GetChild(0).gameObject, "yu-m2", false);
                     SpineManager.instance.DoAnimation(yushow.transform.GetChild(1).gameObject, "star", true);
                     SpineManager.instance.DoAnimation(yushow, "light", false,
                         () =>
                         {
                             SpineManager.instance.DoAnimation(yushow, "light", false,
                                 () =>
                                 {
                                     yushow.SetActive(false);
                                     Xemshow(() =>
                                     {
                                         _isPause = true;
                                         GameSuccess();
                                     });
                                 });
                         }
                         );
                 }));

            }
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator shake(GameObject level)
        {
            for (int i = 0; i < 16; i++)
            {
                SpineManager.instance.DoAnimation(level.transform.GetChild(i).gameObject, "pao" + level.transform.GetChild(i).gameObject.name, true);
                yield return new WaitForSeconds(0.1f);
            }
            yield break;
        }

        //撞到小恶魔
        IEnumerator DomenCollider(GameObject obj, float _time = 1f)
        {
            BoxCollider2D boxCollider2D = obj.GetComponent<BoxCollider2D>();

            boxCollider2D.enabled = false;

            yield return new WaitForSeconds(_time);

            boxCollider2D.enabled = true;
        }

        private void Xemshow(Action callback = null)
        {
            _isPause = true;

            xemshow.SetActive(true);
            xemshow.transform.localPosition = xempos.localPosition;
            SpineManager.instance.DoAnimation(xemshow, "xem1", true);
            PlaySound(3);

            float _time = SpineManager.instance.DoAnimation(xemshow.transform.GetChild(0).gameObject, "xem-boom", false);

            _mono.StartCoroutine(Wait(_time - 0.1f, () =>
            {
                SpineManager.instance.DoAnimation(xemshow, "xem-y", true);
                xemshow.GetComponent<RectTransform>().DOAnchorPosY(-900f, 2f).OnComplete(() =>
                {
                    xemshow.SetActive(false);
                    callback?.Invoke();
                    if (levelnumber != 3) _isPause = false;
                });
            }));
        }

        /// <summary>
        /// 开始游戏
        /// </summary>
        private void StartGame()
        {
            ygbj.transform.GetChild(0).GetComponent<RawImage>().raycastTarget = true;
            _mask.Hide();
            //_mono.StartCoroutine(mydate());
            _isPause = false;
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
                        PlayBgm(0);
                        //PlayCommonBgm(8); //ToDo...改BmgIndex
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
                   // PlayBgm(0);
                    RemoveEvent(_replaySpine); RemoveEvent(_okSpine);
                    var time = PlaySpine(_okSpine, "ok");
                    Delay(time, () =>
                    {
                        _replaySpine.Hide();
                        _dBD.Show();
                        BellSpeck(_dBD, 2);
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

        private void BellSpeck(GameObject go, int index, Action specking = null, Action speckend = null, RoleType roleType = RoleType.Bd, SoundManager.SoundType type = SoundManager.SoundType.VOICE)
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
