using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spine.Unity;
using UnityEngine;
using UnityEngine.EventSystems;
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

    public class TD6724Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        // 田丁
        private GameObject tt;
        private GameObject dd;

        //Mask
        private Transform anyBtns;
        private GameObject mask;

        #region 成功

        private GameObject successSpine;

        private GameObject caidaiSpine;

        //胜利动画名字
        private string tz;
        private string sz;

        #endregion


        #region Game1 GameObject

        private bool isOnclick;
        private GameObject _other;
        private List<EventDispatcher> _dispatcherList;
        private GameObject _game1;
        private GameObject _fishes;
        private GameObject _shuicao;
        private GameObject _ai;
        private GameObject _player;
        private GameObject _px;
        private string spinename;
        private GameObject _aniMask;
        bool isPlaying = false;
        private GameObject _enter;
        private GameObject _w;
        private GameObject _sm;
        private GameObject _sm1;
        private GameObject _hl;
        private bool isFishEnter = false;
        private bool isScEnter = false;
        private bool isEndEnter = false;

        #endregion

        #region Game2 GameObject

        private GameObject _game2;
        private GameObject _game2Player;
        private GameObject _Box;
        private GameObject _wy;
        private GameObject _wy1;
        private GameObject _car;
        private bool isCarEnter;
        private string _game2SpineName;
        int number = 0;

        #endregion


        #region Game3 GameObject

        private GameObject _game3;
        private GameObject _ani;

        #endregion

        private GameObject _next;
        private bool _nextOnClick;


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
            GameStart();
        }


        #region 初始化和游戏开始方法

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            isOnclick = false;
            DOTween.KillAll();
            talkIndex = 1;
            Game1Init();
            Game2Init();
            _aniMask.Hide();
        }

        #endregion

        #region Game1 方法

        /// <summary>
        /// Game1 初始化
        /// </summary>
        void Game1Init()
        {
            _game1.Show();
            _game2.Hide();
            _game3.Hide();
            spinename = null;
            spinename = "y-";
            isFishEnter = false;
            isEndEnter = false;
            isScEnter = false;
            _nextOnClick = false;
            for (int i = 0; i < _dispatcherList.Count; i++)
            {
                var temp = _dispatcherList[i];
                temp.TriggerEnter2D += OnEnter;
            }

            for (int i = 0; i < _fishes.transform.childCount; i++)
            {
                var child = _fishes.transform.GetChild(i).gameObject;
                PlaySpineAni(child, child.name, true);
            }

            for (int i = 0; i < _other.transform.childCount; i++)
            {
                var child = _other.transform.GetChild(i).gameObject;
                if (i > 1) PlaySpineAni(child, child.name, true);
                else
                {
                    PlaySpineAni(child, child.name, false);
                }

                if (i == 1)
                    child.Hide();
            }

            //鱼运动
            ShuiCaoDoMove();
            FishDoMove();
            //网运动
            OtherMove(_w, 0, new Vector3(_w.transform.localPosition.x, 340),
                new Vector3(_w.transform.localPosition.x, 700));

            //螃蟹运动
            OtherMove(_px, 0, new Vector3(-200, _px.transform.localPosition.y),
                new Vector3(400, _px.transform.localPosition.y),
                () => { PlaySpineAni(_px, "px3", true); }, () => { PlaySpineAni(_px, "px2", true); });
            for (int i = 0; i < _ai.transform.childCount; i++)
            {
                var child = _ai.transform.GetChild(i).gameObject;
                child.Show();
                PlaySpineAni(child, "y-" + child.name + "2", true);
                _ai.transform.GetChild(i).GetComponent<EventDispatcher>().TriggerEnter2D += FishOnEnter;
            }

            for (int i = 0; i < _enter.transform.childCount; i++)
            {
                var tem = _enter.transform.GetChild(i).GetComponent<EventDispatcher>();
                tem.TriggerEnter2D += OnEnter;
                if (i != 1)
                    PlaySpineAni(_enter.transform.GetChild(i).gameObject, _enter.transform.GetChild(i).name, true);
                else PlaySpineAni(_enter.transform.GetChild(i).gameObject, "sm", true);
            }

            _player.Show();
            _player.GetComponent<mILDrager>().DragRect =
                _game1.transform.GetGameObject("rect").transform.GetRectTransform();
            _player.GetComponent<CanvasGroup>().alpha = 1;

            _hl.GetComponent<EventDispatcher>().TriggerEnter2D += EndEnter;
            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
            _next.Hide();
            _player.transform.localPosition = new Vector3(-810, -345);
            PlaySpineAni(_player, "y", true);
        }


        /// <summary>
        /// 水母，水草，网碰撞事件
        /// </summary>
        /// <param name="other"></param>
        /// <param name="time"></param>
        private void OnEnter(Collider2D other, int time)
        {
            if (other.name == "player")
            {
                if (!isEndEnter)
                {
                    if (!isScEnter)
                    {
                        _aniMask.Show();
                        isScEnter = true;
                        _player.GetComponent<mILDrager>().canMove = false;


                        PlaySound(1);
                        PlaySpineAni(_player, spinename + "5", false, () =>
                        {
                            isScEnter = false;
                            _player.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(() =>
                            {
                                _player.transform.localPosition = new Vector3(-810, -345);

                                _player.GetComponent<CanvasGroup>().alpha = 1;

                                PlaySpineAni(_player, spinename + "2", true, () => { });
                                _aniMask.Hide();
                            });
                        });
                    }
                }
            }
        }


        #region 水草运动

        void ShuiCaoDoMove()
        {
            for (int i = 0; i < _shuicao.transform.childCount; i++)
            {
                var child = _shuicao.transform.GetChild(i).gameObject;
                ShuiCaoMove(child, i);
            }
        }


        void ShuiCaoMove(GameObject go, int i)
        {
            if (i % 2 == 0)
            {
                var t = go.transform.DOMove(new Vector3(1920, go.transform.position.y), Random.Range(5, 10)).OnComplete(
                    () => { ShuiCaoMove(go, i + 1); });
                t.SetEase(Ease.Linear);
            }
            else
            {
                var t = go.transform.DOMove(new Vector3(500, go.transform.position.y), Random.Range(5, 10)).OnComplete(
                    () => { ShuiCaoMove(go, i + 1); });
                t.SetEase(Ease.Linear);
            }
        }

        #endregion


        #region 背景鱼运动

        void FishDoMove()
        {
            for (int i = 0; i < _fishes.transform.childCount; i++)
            {
                var temp = _fishes.transform.GetChild(i).gameObject;
                FishMove(temp, i);
            }
        }

        void FishMove(GameObject go, int i)
        {
            if (i % 2 == 0)
            {
                go.transform.localScale = new Vector3(1, 1, 1);
                var t = go.transform.DOMove(new Vector3(1920, go.transform.position.y), Random.Range(10, 20))
                    .OnComplete(
                        () => { FishMove(go, i + 1); });
                t.SetEase(Ease.Linear);
            }
            else
            {
                go.transform.localScale = new Vector3(-1, 1, 1);
                var t = go.transform.DOMove(new Vector3(0, go.transform.position.y), Random.Range(10, 20)).OnComplete(
                    () => { FishMove(go, i + 1); });
                t.SetEase(Ease.Linear);
            }
        }

        #endregion


        /// <summary>
        /// game1加载
        /// </summary>
        void Game1Load()
        {
            _game1 = curTrans.GetGameObject("game1");
            _other = _game1.transform.GetGameObject("other");
            _fishes = _game1.transform.GetGameObject("fishes");
            _shuicao = _game1.transform.GetGameObject("shuicao");
            _ai = _game1.transform.GetGameObject("ai");
            _player = _game1.transform.GetGameObject("player");

            _aniMask = curGo.transform.GetGameObject("animask");
            _dispatcherList = new List<EventDispatcher>();
            _enter = _game1.transform.GetGameObject("Enter");
            _w = _enter.transform.GetGameObject("w");
            _px = _enter.transform.GetGameObject("px");
            _sm = _enter.transform.GetGameObject("sm");
            _sm1 = _enter.transform.GetGameObject("sm1");
            _next = curTrans.GetGameObject("mask/next");
            _hl = _other.transform.GetGameObject("hl");
            mILDrager m = _player.GetComponent<mILDrager>();
            m.SetDragCallback(StarDrag, Draging, null);

            Util.AddBtnClick(_next.transform.GetChild(0).gameObject, NextGame);

            _hl.GetComponent<EventDispatcher>().TriggerEnter2D -= EndEnter;
            for (int i = 0; i < _shuicao.transform.childCount; i++)
            {
                var child = _shuicao.transform.GetChild(i).gameObject;
                _dispatcherList.Add(child.GetComponent<EventDispatcher>());
            }


            for (int i = 0; i < _enter.transform.childCount; i++)
            {
                var tem = _enter.transform.GetChild(i).GetComponent<EventDispatcher>();
                tem.TriggerEnter2D -= OnEnter;
            }

            for (int i = 0; i < _ai.transform.childCount; i++)
            {
                var temp = _ai.transform.GetChild(i).GetComponent<EventDispatcher>();
                temp.TriggerEnter2D -= FishOnEnter;
            }

            for (int i = 0; i < _dispatcherList.Count; i++)
            {
                var temp = _dispatcherList[i];
                temp.TriggerEnter2D -= OnEnter;
            }
        }

        private void StarDrag(Vector3 arg1, int arg2, int arg3)
        {
            _player.GetComponent<mILDrager>().canMove = true;
            _game2Player.GetComponent<mILDrager>().canMove = true;
        }


        //下一关
        private void NextGame(GameObject obj)
        {
            if (!_nextOnClick)
            {
                _nextOnClick = true;
                BtnPlaySound();
                PlaySpineAni(_next, "next", false, () =>
                {
                    PlaySpineAni(_next, "kong", false);
                    tt.Show();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);

                    _game1.Hide();
                    _game2.Show();
                    _aniMask.Hide();
                    PlaySound(2);
                    PlaySpineAni(_game2Player, "che-1", false,
                        () =>
                        {
                            PlaySpineAni(_wy, "wy", true);
                            PlaySpineAni(_wy1, "wy", true);
                            OtherMove(_wy, 0, new Vector3(-220, -380), new Vector3(660, 240));
                            OtherMove(_wy1, 0, new Vector3(-640, 400), new Vector3(570, -200));
                            PlaySpineAni(_game2Player, "che-0", true,
                                () => { });
                        });
                    Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                    Talk(tt, 3, () => { }, () =>
                    {
                        mask.Hide();
                        tt.Hide();
                    });
                });
            }
        }


        //结束
        private void EndEnter(Collider2D other, int time)
        {
            if (other.name == "player")
            {
                if (!isEndEnter)
                {
                    isEndEnter = true;
                    _aniMask.Show();
                    DOTween.KillAll();
                    _other.transform.GetChild(1).gameObject.Show();
                    _player.GetComponent<mILDrager>().canMove = false;
                    for (int i = 0; i < _enter.transform.childCount; i++)
                    {
                        var tem = _enter.transform.GetChild(i).GetComponent<EventDispatcher>();
                        tem.TriggerEnter2D -= OnEnter;
                    }

                    for (int i = 0; i < _ai.transform.childCount; i++)
                    {
                        var temp = _ai.transform.GetChild(i).GetComponent<EventDispatcher>();
                        temp.TriggerEnter2D -= FishOnEnter;
                    }

                    for (int i = 0; i < _dispatcherList.Count; i++)
                    {
                        var temp = _dispatcherList[i];
                        temp.TriggerEnter2D -= OnEnter;
                    }


                    PlaySpineAni(_other.transform.GetChild(1).gameObject, _other.transform.GetChild(1).name, false,
                        () =>
                        {
                            BtnPlaySoundSuccess();
                            _other.transform.GetChild(1).gameObject.Hide();
                            PlaySpineAni(_hl, "hl");
                            _player.GetComponent<CanvasGroup>().DOFade(0, 1);
                            PlaySound(3);
                            _player.transform.DOLocalMove(new Vector3(660, -20), 1f).OnComplete(() => { });
                            WaitTimeAndExcuteNext(3f, () =>
                            {
                                mask.Show();
                                tt.Hide();
                                dd.Hide();
                                _next.Show();
                                _player.Hide();
                                PlaySpineAni(_next, "next2", true);
                            });
                        });
                }
            }
        }

        private void Draging(Vector3 pos, int arg2, int arg3)
        {
        }


        /// <summary>
        ///  选择鱼
        /// </summary>
        /// <param name="other"></param>
        /// <param name="time"></param>
        private void FishOnEnter(Collider2D other, int time)
        {
            if (other.name == "player")
            {
                _aniMask.Show();
                if (!isFishEnter)
                {
                    isFishEnter = true;
                    if (other.transform.localPosition.y < 0)
                    {
                        spinename += "c";
                    }
                    else if (other.transform.localPosition.y < 200 && other.transform.position.y > 100)
                    {
                        spinename += "b";
                    }
                    else
                    {
                        spinename += "a";
                    }
                }

                _player.GetComponent<mILDrager>().canMove = false;

                for (int i = 0; i < _ai.transform.childCount; i++)
                {
                    _ai.transform.GetChild(i).gameObject.Hide();
                }

                PlaySound(0);
                PlaySpineAni(_player, spinename + "1", false, () =>
                {
                    _aniMask.Hide();

                    OtherMove(_sm, 0, new Vector3(_sm.transform.localPosition.x, -160),
                        new Vector3(_sm.transform.localPosition.x, 400));
                    OtherMove(_sm1, 1, new Vector3(_sm1.transform.localPosition.x, -160),
                        new Vector3(_sm1.transform.localPosition.x, 400));
                    _player.GetComponent<mILDrager>().DragRect = _game1.transform.GetRectTransform();
                    PlaySpineAni(_player, spinename + "2", true,
                        () => { });
                });
            }
        }


        #region 水母，螃蟹 网运动

        void OtherMove(GameObject go, int i, Vector3 endpos1, Vector3 endpos2, Action method1 = null,
            Action method2 = null)
        {
            if (i % 2 == 0)
            {
                var t = go.transform.DOLocalMove(endpos1, Random.Range(5, 10)).OnComplete(
                    () => { OtherMove(go, i + 1, endpos1, endpos2, method1, method2); });
                method1?.Invoke();
                t.SetEase(Ease.Linear);
            }
            else
            {
                var t = go.transform.DOLocalMove(endpos2, Random.Range(5, 10)).OnComplete(
                    () => { OtherMove(go, i + 1, endpos1, endpos2, method1, method2); });
                method2?.Invoke();
                t.SetEase(Ease.Linear);
            }
        }

        #endregion

        #endregion


        #region Game2 方法

        /// <summary>
        /// Game2 加载
        /// </summary>
        void Game2Load()
        {
            _game2 = curTrans.GetGameObject("game2");
            _game2Player = _game2.transform.GetGameObject("player");
            _Box = _game2.transform.GetGameObject("Box");
            _wy = _game2.transform.GetGameObject("wy");
            _wy1 = _game2.transform.GetGameObject("wy1");
            _car = _game2.transform.GetGameObject("car");
            for (int i = 0; i < _Box.transform.childCount; i++)
            {
                var child = _Box.transform.GetChild(i).gameObject;
                child.GetComponent<EventDispatcher>().TriggerEnter2D -= OnCarEnter;
            }

            for (int i = 0; i < _car.transform.childCount; i++)
            {
                var child = _car.transform.GetChild(i).gameObject;
                child.GetComponent<EventDispatcher>().TriggerEnter2D -= EnterCar;
            }

            _game2Player.GetComponent<mILDrager>().SetDragCallback(StarDrag);

            _wy.GetComponent<EventDispatcher>().TriggerEnter2D -= OnCarEnter;
            _wy1.GetComponent<EventDispatcher>().TriggerEnter2D -= OnCarEnter;
        }


        /// <summary>
        /// Game2 初始化
        /// </summary>
        void Game2Init()
        {
            _game2SpineName = null;
            _game2SpineName = "che-";
            isCarEnter = false;
            number = 0;

            PlaySpineAni(_wy, "wy", true);
            PlaySpineAni(_wy1, "wy", true);
            Debug.Log("SpineName2:    " + _game2SpineName);
            _game2Player.GetComponent<CanvasGroup>().alpha = 1;
            _game2Player.transform.localPosition = new Vector3(-770, -370);
            PlaySpineAni(_game2Player, _game2SpineName + "0", true);


            for (int i = 0; i < _car.transform.childCount; i++)
            {
                var temp = _car.transform.GetChild(i).gameObject;
                temp.Show();
                PlaySpineAni(temp, _game2SpineName + temp.name + "3", true);
                temp.GetComponent<EventDispatcher>().TriggerEnter2D += EnterCar;
            }


            for (int i = 0; i < _Box.transform.childCount; i++)
            {
                var child = _Box.transform.GetChild(i).gameObject;
                child.GetComponent<EventDispatcher>().TriggerEnter2D += OnCarEnter;
            }

            _wy.GetComponent<EventDispatcher>().TriggerEnter2D += OnCarEnter;
            _wy1.GetComponent<EventDispatcher>().TriggerEnter2D += OnCarEnter;
        }


        //碰撞车子
        private void EnterCar(Collider2D other, int time)
        {
            if (other.name == "player" && !isCarEnter)
            {
                isCarEnter = true;
                if (_game2Player.transform.localPosition.x < -500)
                {
                    _game2SpineName = null;
                    _game2SpineName = "che-";
                    _game2SpineName += "a";
                    _car.transform.GetGameObject("a").Hide();
                }

                if (_game2Player.transform.localPosition.x > -500 && _game2Player.transform.localPosition.x < -100)
                {
                    _game2SpineName = null;
                    _game2SpineName = "che-";
                    _game2SpineName += "b";
                    _car.transform.GetGameObject("b").Hide();
                }

                if (_game2Player.transform.localPosition.x > -100 && _game2Player.transform.localPosition.x < 300)
                {
                    _game2SpineName = null;
                    _game2SpineName = "che-";
                    _game2SpineName += "c";
                    _car.transform.GetGameObject("c").Hide();
                }
                else if (_game2Player.transform.localPosition.x > 300)
                {
                    _game2SpineName = null;
                    _game2SpineName = "che-";
                    _game2SpineName += "d";
                    _car.transform.GetGameObject("d").Hide();
                }

                Debug.Log("SpineName:    " + _game2SpineName);
                _game2Player.GetComponent<mILDrager>().canMove = false;
             
                PlaySound(0);
                PlaySpineAni(_game2Player, _game2SpineName + "1", false, () =>
                {
                    BtnPlaySoundSuccess();
                    isCarEnter = false;
                    number = 0;
                    for (int i = 0; i < _car.transform.childCount; i++)
                    {
                        var child = _car.transform.GetChild(i).gameObject;
                        if (!child.gameObject.activeSelf)
                        {
                            number++;
                        }

                        Debug.Log("Number " + number);
                        if (number == 4)
                        {
                            DOTween.KillAll();
                            _aniMask.Show();
                            WaitTimeAndExcuteNext(2f, () =>
                            {
                                Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[2];
                                _game3.Show();
                                _game2.Hide();

                                PlaySpineAni(_ani, "kong", false,
                                    () =>
                                    {
                                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                                        // PlaySound(4);
                                        var t = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                        WaitTimeAndExcuteNext(t, () => { PlaySound(5); });
                                        PlaySpineAni(_ani, "hl", false,
                                            () => { WaitTimeAndExcuteNext(2f, () => { playSuccessSpine(); }); });
                                    });
                            });
                        }
                    }

                    PlaySpineAni(_game2Player, _game2SpineName + "3", true, () => { });
                });
            }
        }

        //碰撞其他
        private void OnCarEnter(Collider2D other, int time)
        {
            if (other.name == "player")
            {
                _game2Player.GetComponent<mILDrager>().canMove = false;
                Debug.Log(_game2Player.GetComponent<mILDrager>().canMove);
                if (!isCarEnter)
                {
                    _aniMask.Show();

                    isCarEnter = true;
                    PlaySound(1);
                    Debug.Log("Name  " + _game2SpineName);
                    if (number == 0)
                    {
                        PlaySpineAni(_game2Player, "che-2", false);
                        _game2Player.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(() =>
                        {
                            _game2Player.transform.localPosition = new Vector3(-770, -370);
                        });
                        WaitTimeAndExcuteNext(1.5f, () =>
                        {
                            PlaySound(2);
                            PlaySpineAni(_game2Player, "che-1", false,
                                () => { PlaySpineAni(_game2Player, "che-0", true, () => { }); });


                            isCarEnter = false;

                            _game2Player.GetComponent<CanvasGroup>().alpha = 1;
                            _aniMask.Hide();
                        });
                    }
                    else
                    {
                        PlaySpineAni(_game2Player, _game2SpineName + "4", false);
                        _game2Player.GetComponent<CanvasGroup>().DOFade(0, 1).OnComplete(() =>
                        {
                            _game2Player.transform.localPosition = new Vector3(-770, -370);
                        });

                        WaitTimeAndExcuteNext(1.5f, () =>
                        {
                            PlaySound(2);

                            _game2Player.GetComponent<CanvasGroup>().alpha = 1;
                            PlaySpineAni(_game2Player, _game2SpineName + "5", false,
                                () => { PlaySpineAni(_game2Player, _game2SpineName + "3", true, () => { }); });
                            isCarEnter = false;
                            _aniMask.Hide();
                        });
                    }
                }
            }
        }

        #endregion


        #region Game3方法

        void Game3Load()
        {
            _game3 = curTrans.GetGameObject("game3");
            _ani = _game3.transform.GetGameObject("hl");
        }

        #endregion


        void GameStart()
        {
            mask.Show();
            anyBtns.gameObject.Show();
            anyBtns.transform.GetChild(0).gameObject.Show();
            anyBtns.transform.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
        }


        #region 田丁

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
                speaker = dd;
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

                    mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        mask.Hide();
                        tt.Hide();
                    }));

                    break;
            }

            talkIndex++;
        }

        /// <summary>
        /// 田丁游戏开始方法
        /// </summary>
        void TDGameStartFunc()
        {
            tt.SetActive(true);
            PlayBGM();
            mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 0, null,
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
  
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
            mono.StartCoroutine(SpeckerCoroutine(target, SoundManager.SoundType.VOICE, index, goingEvent, finishEvent));
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
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
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
            mask.SetActive(false);
            //加载人物
            TDLoadCharacter();
            //加载成功界面
            TDLoadSuccessPanel();
            //加载游戏按钮
            TDLoadButton();
            //Game1加载
            Game1Load();
            //Game2加载
            Game2Load();
            //Game3加载
            Game3Load();
        }

        /// <summary>
        /// 加载人物
        /// </summary>
        void TDLoadCharacter()
        {
            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(false);
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
            for (int i = 0; i < anyBtns.childCount; i++)
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
            isOnclick = false;
            string result = string.Empty;
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
            if (!isOnclick)
            {
                isOnclick = true;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            TDGameStartFunc(); //
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            mask.SetActive(false);
                            GameInit();
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            switchBGM();
                            anyBtns.gameObject.SetActive(false);
                            dd.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(dd, SoundManager.SoundType.VOICE, 2));
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

        #endregion

        #endregion
    }
}