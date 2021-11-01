using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
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

    public class TD3411Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject dd;
        private GameObject caidaiSpine;
        private GameObject successSpine;
        private GameObject mask;


        //---------------------------------------
        private GameObject _parent; //父节点
        private GameObject _animation; //下面一排的动画
        private GameObject _animal; //左侧的动物
        private GameObject _panda; //拼图
        private GameObject _giraffe;
        private GameObject _monoster;

        private GameObject _ani; //aaaa10静态spine
        private GameObject _em; //恶魔spine
        private GameObject _blood; //血量
        private BellSprites _bloodSprites; //血量图片切换；
        private GameObject _touchParent; //拖拽的父物体
        private List<GameObject> _touchObjects; //拖拽物体

        private GameObject _fh;
        private GameObject _ok;
        private GameObject _bf;

        private int _blueIndex;
        private int _eyesIndex;
        private int _redIndex;
        private int _yellowIndex;
        private int _mouseIndex;
        private int _orangeIndex;
        private int _greenIndex;
        private int _successIndex;
        private ILDrager[] _tDrager; //拖拽数组

        private GameObject _aniMask;

        private GameObject _dd;
        int _index;

        //---------------------------------------
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();


            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            Input.multiTouchEnabled = false;
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //-------------------------------------------------------
            _parent = curTrans.GetGameObject("Parent");
            var _parentTrans = _parent.transform;
            _ani = _parentTrans.GetGameObject("ani");
            _animal = _parentTrans.GetGameObject("animal");
            _animation = _parentTrans.GetGameObject("animation");
            _em = _parentTrans.GetGameObject("em");
            _blood = _parentTrans.GetGameObject("blood");
            _bloodSprites = _blood.GetComponent<BellSprites>();
            _panda = _parentTrans.GetGameObject("Panda");
            _giraffe = _parentTrans.GetGameObject("Giraffe");
            _monoster = _parentTrans.GetGameObject("monster");
            Util.AddBtnClick(_monoster.transform.GetChild(0).gameObject, ClickOk);
            _touchParent = _parentTrans.GetGameObject("touchObj");
            _touchObjects = new List<GameObject>();
            _tDrager = new ILDrager[_touchParent.transform.childCount];
            _fh = mask.transform.GetGameObject("fh");
            _bf = mask.transform.GetGameObject("bf");
            _ok = mask.transform.GetGameObject("ok");
            Util.AddBtnClick(_fh, RePlay);
            Util.AddBtnClick(_bf, OnClickAnyBtn);
            Util.AddBtnClick(_ok, OnClickAnyBtn);
            for (int i = 0; i < _touchParent.transform.childCount; i++)
            {
                var child = _touchParent.transform.GetChild(i).gameObject;
                var drag = child.GetComponent<ILDrager>();
                drag.SetDragCallback(StartDrag, null, SetPos, null);
                _tDrager[i] = drag;
                _touchObjects.Add(child);
            }

            _aniMask = curTrans.GetGameObject("aniMask");
            _dd = mask.transform.GetGameObject("dd");
            //-------------------------------------------------------
            GameInit();
            //GameStart();
            IsStart();
        }

        private void StartDrag(Vector3 arg1, int arg2, int dragIndex)
        {
            _index = _tDrager[dragIndex].transform.GetSiblingIndex();
            _tDrager[dragIndex].transform.SetAsLastSibling();
        }

        void IsStart()
        {
            mask.Show();
            _bf.Show();
        }

        private void RePlay(GameObject obj)
        {
            SpineManager.instance.DoAnimation(_fh.transform.GetChild(0).gameObject, "fh", false);
            GameInit();
        }


        private void ClickOk(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
            DoAnimation(obj.transform.GetChild(0).gameObject, "ok", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            DoAnimation(_em, "dj2", false, () => {      _em.Hide();});
            _blood.GetComponent<Image>().sprite = _bloodSprites.sprites[3];
            Wait(1.5f, () =>
            {
                mask.Show();
                //_dd.Show();
           
                playSuccessSpine();
                Wait(3f, () =>
                {
                    successSpine.SetActive(false);
                    caidaiSpine.Hide();
                    Wait(1f, () =>
                    {
                        _fh.Show();
                        _ok.Show();
                    });
                });

            });
            //Wait(1f, () => { });
        }


        void DoAnimation(GameObject obj, string name, bool isLoop, Action methos = null)
        {
            SpineManager.instance.DoAnimation(obj, name, isLoop, methos);
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum)
        {
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

            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                    _bf.Hide();
                }
                else if (obj.name == "ok")
                {
                    //GameInit();
                    _dd.Show();
                    _ok.Hide();
                    _fh.Hide();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, _dd, null, () =>
                    {
                        //_dd.Hide();

                    }));
                }

                //mask.gameObject.SetActive(false);
            });
        }

        void GameStart()
        {
            Debug.Log("aaaaa");
            mask.Show();
            dd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, dd, null,
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
        }

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            _blueIndex = 0;
            _eyesIndex = 0;
            _successIndex = 0;
            _dd.Hide();
            _redIndex = 0;
            _yellowIndex = 0;
            _mouseIndex = 0;
            _orangeIndex = 0;
            _greenIndex = 0;
            PictureInit();
            _fh.Hide();
            mask.Hide();
            _ok.Hide();
            _aniMask.Hide();
            dd.Hide();
            for (int i = 0; i < _touchParent.transform.childCount; i++)
            {
                if (i > 6)
                {
                    GameObject.Destroy(_touchParent.transform.GetChild(i).gameObject);
                }
            }

            SpineManager.instance.DoAnimation(_ani, "aaaa10", true);
            SpineManager.instance.DoAnimation(_animation, "aaaa10", true);
            SpineManager.instance.DoAnimation(_em, "dj", true);
            SpineManager.instance.DoAnimation(_animal, "c2", true);
            SpineManager.instance.DoAnimation(_fh.transform.GetChild(0).gameObject, "fh2", false);
            SpineManager.instance.DoAnimation(_bf.transform.GetChild(0).gameObject, "bf2", false);
            SpineManager.instance.DoAnimation(_ok.transform.GetChild(0).gameObject, "ok2", false);
            _blood.GetComponent<Image>().sprite = _bloodSprites.sprites[0];
        }


        //拖拽松开事件
        private void SetPos(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {

            if (_tDrager[dragIndex].transform.localPosition.y > -200)
            {

                // _aniMask.Hide();
                //panda
                if (_successIndex < 9)
                {
                    _aniMask.Show();
                    switch (dragIndex)
                    {
                        //脸
                        case 0:
                            switch (_redIndex)
                            {
                                case 0:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(120f, 140f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _panda.transform.GetChild(0).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                case 1:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-10f, 310f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _panda.transform.GetChild(8).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                case 2:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(290f, 310f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _panda.transform.GetChild(7).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            break;
                        //正方形
                        case 1:
                            _tDrager[dragIndex].DoReset();
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            SpineManager.instance.DoAnimation(_animation, "aaaa2", false,
                                () => { SpineManager.instance.DoAnimation(_animation, "aaaa10", false, () => { }); });
                            break;

                        //长方形
                        case 3:
                            _tDrager[dragIndex].DoReset();
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            SpineManager.instance.DoAnimation(_animation, "aaaa10", false,
                                () => { SpineManager.instance.DoAnimation(_animation, "aaaa10", false, () => { }); });
                            break;
                        //三角形
                        case 2:
                            if (_yellowIndex == 0)
                            {
                                _tDrager[dragIndex].transform.DOLocalMove(new Vector3(120f, 60f), 0.5f, false).OnComplete(
                                    () =>
                                    {
                                        _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 180), 1f).OnComplete(() =>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                            _tDrager[dragIndex].gameObject.Hide();
                                            _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 0), 0.1f).OnComplete(() => { _tDrager[dragIndex].gameObject.Show(); });
                                            _panda.transform.GetChild(5).gameObject.Show();
                                            _tDrager[dragIndex].DoReset();
                                            _successIndex++;
                                            IsSuccess();
                                        });
                                    });

                                _yellowIndex++;
                            }
                            else
                            {
                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                _tDrager[dragIndex].DoReset();
                            }

                            break;
                        //椭圆
                        case 4:
                            switch (_blueIndex)
                            {
                                case 0:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(20f, 140f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {

                                                _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 35), 1f)
                                                    .OnComplete(
                                                        () =>
                                                        {
                                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                            _tDrager[dragIndex].gameObject.Hide();
                                                            _panda.transform.GetChild(1).gameObject.Show();
                                                            _tDrager[dragIndex].DoReset();
                                                            _tDrager[dragIndex].transform
                                                                .DORotate(new Vector3(0, 0, 0), 0.1f);
                                                            _tDrager[dragIndex].gameObject.Show();
                                                            _successIndex++;
                                                            _blueIndex++;
                                                            IsSuccess();
                                                        });
                                            });
                                    break;
                                case 1:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(200f, 140f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {

                                                _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, -35), 1f)
                                                    .OnComplete(
                                                        () =>
                                                        {
                                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                            _tDrager[dragIndex].gameObject.Hide();
                                                            _panda.transform.GetChild(2).gameObject.Show();
                                                            _tDrager[dragIndex].DoReset();
                                                            _tDrager[dragIndex].transform
                                                                .DORotate(new Vector3(0, 0, 0), 0.1f);
                                                            _tDrager[dragIndex].gameObject.Show();
                                                            _successIndex++;
                                                            _blueIndex++;
                                                            IsSuccess();
                                                        });
                                            });
                                    break;
                                default:
                                    _tDrager[dragIndex].DoReset();
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    break;
                            }

                            break;
                        //眼睛
                        case 5:
                            switch (_eyesIndex)
                            {
                                case 0:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(20f, 150f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _panda.transform.GetChild(3).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });

                                    break;
                                case 1:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(205f, 150f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _panda.transform.GetChild(4).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            _eyesIndex++;
                            break;
                        //嘴巴
                        case 6:
                            if (_mouseIndex == 0)
                            {
                                _tDrager[dragIndex].transform.DOLocalMove(new Vector3(110f, 5f), 0.5f, false).OnComplete(
                                    () =>
                                    {
                                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                        _tDrager[dragIndex].gameObject.Hide();
                                        _panda.transform.GetChild(6).gameObject.Show();
                                        _tDrager[dragIndex].DoReset();
                                        _tDrager[dragIndex].gameObject.Show();
                                        _successIndex++;
                                        IsSuccess();
                                        _mouseIndex++;
                                    });
                            }
                            else
                            {
                                _tDrager[dragIndex].DoReset();
                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 2, _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            }

                            break;
                    }
                }

                //giraffe
                if (_successIndex >= 9 && _successIndex < 26)
                {
                    _aniMask.Show();
                    switch (dragIndex)
                    {
                        //红色
                        case 0:
                            switch (_redIndex)
                            {
                                case 3:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(120f, 260f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(9).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                case 4:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(240f, 250f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(10).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                case 5:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(140f, 360f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(12).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                case 6:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(40f, 360f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(11).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _redIndex++;
                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            break;
                        //橙色正方形
                        case 1:
                            switch (_orangeIndex)
                            {
                                case 0:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-60f, 140f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(13).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _orangeIndex++;
                                    break;
                                case 1:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(90f, 40f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(14).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _orangeIndex++;
                                    break;
                                case 2:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(30f, -20f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(15).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _orangeIndex++;
                                    break;
                                case 3:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(70f, -90f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(16).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                            });
                                    _orangeIndex++;
                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            break;
                        //黄色三角形
                        case 2:
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            _tDrager[dragIndex].DoReset();
                            break;

                        //绿色长方形
                        case 3:
                            switch (_greenIndex)
                            {
                                case 0:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(40f, 80f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 90), 0.5f)
                                                    .OnComplete(
                                                        () =>
                                                        {
                                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                            _tDrager[dragIndex].gameObject.Hide();
                                                            _giraffe.transform.GetChild(5).gameObject.Show();
                                                            _tDrager[dragIndex].DoReset();
                                                            _tDrager[dragIndex].transform
                                                                .DORotate(new Vector3(0, 0, 0), 0.1f);
                                                            _tDrager[dragIndex].gameObject.Show();
                                                            _successIndex++;
                                                            _greenIndex++;
                                                            IsSuccess();
                                                        });
                                            });

                                    break;

                                case 1:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(100f, 330f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 90), 0.5f)
                                                    .OnComplete(
                                                        () =>
                                                        {
                                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                            _tDrager[dragIndex].gameObject.Hide();
                                                            _giraffe.transform.GetChild(6).gameObject.Show();
                                                            _tDrager[dragIndex].DoReset();
                                                            _tDrager[dragIndex].transform
                                                                .DORotate(new Vector3(0, 0, 0), 0.1f);
                                                            _tDrager[dragIndex].gameObject.Show();
                                                            _successIndex++;
                                                            _greenIndex++;
                                                            IsSuccess();
                                                        });
                                            });

                                    break;
                                case 2:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(40f, 330f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                _tDrager[dragIndex].transform.DORotate(new Vector3(0, 0, 90), 0.5f)
                                                    .OnComplete(
                                                        () =>
                                                        {
                                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                            _tDrager[dragIndex].gameObject.Hide();
                                                            _giraffe.transform.GetChild(7).gameObject.Show();
                                                            _tDrager[dragIndex].DoReset();
                                                            _tDrager[dragIndex].transform
                                                                .DORotate(new Vector3(0, 0, 0), 0.1f);
                                                            _tDrager[dragIndex].gameObject.Show();
                                                            _successIndex++;
                                                            _greenIndex++;
                                                            IsSuccess();
                                                        });
                                            });

                                    break;

                                case 3:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-40f, 270f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(0).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                                _greenIndex++;
                                            });

                                    break;
                                case 4:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-40f, 190f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(1).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                                _greenIndex++;
                                            });
                                    break;
                                case 5:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-40f, 100f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(2).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                                _greenIndex++;
                                            });
                                    break;
                                case 6:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-40f, -60f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(3).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                IsSuccess();
                                                _greenIndex++;
                                            });

                                    break;
                                case 7:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(-50f, -110f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(4).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                _greenIndex++;
                                                IsSuccess();
                                            });

                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            break;

                        //蓝色
                        case 4:
                            switch (_blueIndex)
                            {
                                case 2:
                                    _tDrager[dragIndex].transform.DOLocalMove(new Vector3(50f, 260f), 0.5f, false)
                                        .OnComplete(
                                            () =>
                                            {
                                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                                                _tDrager[dragIndex].gameObject.Hide();
                                                _giraffe.transform.GetChild(8).gameObject.Show();
                                                _tDrager[dragIndex].DoReset();
                                                _tDrager[dragIndex].gameObject.Show();
                                                _successIndex++;
                                                _blueIndex++;
                                                IsSuccess();
                                            });

                                    break;
                                default:
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                                    _tDrager[dragIndex].DoReset();
                                    break;
                            }

                            break;

                        case 5:
                            _tDrager[dragIndex].DoReset();
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            break;
                        case 6:
                            _tDrager[dragIndex].DoReset();
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), _dd, () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));

                            break;
                    }
                }

                //monster
                if (_successIndex == 26)
                {
                    if (_tDrager[dragIndex].transform.localPosition.x < 420 &&
                        _tDrager[dragIndex].transform.localPosition.x > -170 &&
                        _tDrager[dragIndex].transform.localPosition.y < 360 &&
                        _tDrager[dragIndex].transform.localPosition.y > -190)
                    {
                        var pos = _tDrager[dragIndex].transform.position;
                        var go = GameObject.Instantiate(_tDrager[dragIndex], _touchParent.transform, false);
                        go.transform.position = pos;
                        go.GetComponent<ILDrager>().DragRect = _monoster.transform.GetRectTransform();
                        go.transform.GetTransform().localScale = new Vector3(1, 1, 1);
                        _tDrager[dragIndex].DoReset();
                    }
                    else
                    {
                        _tDrager[dragIndex].DoReset();
                    }
                }
                Debug.Log("successIndex:            " + _successIndex);
            }
            else
            {
                _tDrager[dragIndex].DoReset();
            }
            _tDrager[dragIndex].transform.SetSiblingIndex(_index);
        }


        void IsSuccess()
        {
            if (_successIndex == 9)
            {
                //成功音效
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 4);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                DoAnimation(_em, "dj2", false, () => { DoAnimation(_em, "dj", true); });
                _blood.GetComponent<Image>().sprite = _bloodSprites.sprites[1];
                Wait(2f, () =>
                {
                    DoAnimation(_animal, "c3", false);
                    _panda.Hide();
                    _giraffe.Show();
                });
            }

            if (_successIndex == 26)
            {

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 5);
                DoAnimation(_em, "dj2", false, () => { DoAnimation(_em, "dj", true); });
                _blood.GetComponent<Image>().sprite = _bloodSprites.sprites[2];
                Wait(3f, () =>
                {
                    _aniMask.Show();
                    DoAnimation(_animal, "c1", false);
                    _panda.Hide();
                    _giraffe.Hide();
                    _monoster.Show();
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, dd, () => { },
                        () => { _aniMask.Hide(); }));
                    DoAnimation(_monoster.transform.GetChild(0).transform.GetChild(0).gameObject, "ok2", false);
                });
            }
        }

        //拼图初始化
        void PictureInit()
        {
            for (int i = 0; i < _panda.transform.childCount; i++)
            {
                var child = _panda.transform.GetChild(i).gameObject;
                if (i != _panda.transform.childCount - 1)
                {
                    child.Hide();
                }
                else
                {
                    child.Show();
                }
            }

            for (int i = 0; i < _giraffe.transform.childCount; i++)
            {
                var child = _giraffe.transform.GetChild(i).gameObject;
                if (i != _giraffe.transform.childCount - 1)
                {
                    child.Hide();
                }
                else
                {
                    child.Show();
                }
            }

            _panda.Show();
            _giraffe.Hide();
            _monoster.Hide();
        }


        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitSecondS(time, method));
        }


        IEnumerator WaitSecondS(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject gameObject,
            Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(gameObject, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(gameObject, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(gameObject, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, dd, null,
             () =>
             {
                 dd.SetActive(false);
                 mask.SetActive(false);
             }));
            }

            talkIndex++;
        }


        // 播放成功动画
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            caidaiSpine.SetActive(true);
            successSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
            {
                SpineManager.instance.DoAnimation(successSpine, "3-5-z2", true, () =>
                {
                    /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                    //successSpine.SetActive(false);
                    ac?.Invoke();
                });
            });
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }
    }
}