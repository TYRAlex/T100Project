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

    public class TD8914Part6
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject btnBack;

        //-----------------------------------------
        private GameObject _parent;
        private GameObject _dxg;
        private ILDrager[] _birdsList;
        private ILDroper[] _dropers;
        ArrayList drops;
        private GameObject _em;

        private List<GameObject> _setGames;

        private List<GameObject> _lifeList;
        private int level;

        private int score;

        private int _index;

        //------------------------------------------
        private string tz;
        private string sz;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            mask = curTrans.Find("mask").gameObject;
            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }

            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);


            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //----------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            var parentTrans = _parent.transform;
            _em = parentTrans.GetGameObject("em");
            _dxg = parentTrans.GetGameObject("dxg");
            _birdsList = parentTrans.GetGameObject("birds").GetComponentsInChildren<ILDrager>(true);
            _dropers = parentTrans.GetGameObject("drops").GetComponentsInChildren<ILDroper>(true);


            _setGames = new List<GameObject>();
            drops = new ArrayList(_dropers);
            _lifeList = new List<GameObject>();
            for (int i = 0; i < parentTrans.GetGameObject("life").transform.childCount; i++)
            {
                var child = parentTrans.GetGameObject("life").transform.GetChild(i).gameObject;
                _lifeList.Add(child);
            }
            //----------------------------------------------------------------


            GameInit();
            IsStart();
            //GameStart();
        }

        private void StartDrage(Vector3 arg1, int arg2, int index)
        {
            _index = _birdsList[index].transform.GetSiblingIndex();
            _birdsList[index].transform.SetAsLastSibling();
        }

        void IsStart()
        {
            mask.Show();
            anyBtns.gameObject.Show();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.Show();
        }

        //拖拽完成
        private void SetPos(Vector3 endPos, int type, int index, bool isTure)
        {
            if (isTure)
            {
                SetDrops(index);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                _setGames.Add(_birdsList[index].gameObject);
                level++;
                _birdsList[index].canMove = false;
                switch (level)
                {
                    case 1:
                        if (_birdsList[index].transform.localPosition.x <= -140)
                        {
                            _birdsList[index].transform.localPosition = _dropers[0].transform.localPosition;
                            RemoveDrops(index, _dropers[0]);
                        }
                        else if (_birdsList[index].transform.localPosition.x > 80)
                        {
                            _birdsList[index].transform.localPosition = _dropers[2].transform.localPosition;
                            RemoveDrops(index, _dropers[2]);
                        }
                        else
                        {
                            _birdsList[index].transform.localPosition = _dropers[1].transform.localPosition;
                            RemoveDrops(index, _dropers[1]);
                        }

                        SpineManager.instance.DoAnimation(_dxg, "x1", false,
                            () => { SpineManager.instance.DoAnimation(_dxg, "x1-", true); });
                        break;
                    case 2:
                        if (_birdsList[index].transform.localPosition.x <= -140)
                        {
                            _birdsList[index].transform.localPosition = _dropers[0].transform.localPosition;
                            RemoveDrops(index, _dropers[0]);
                        }
                        else if (_birdsList[index].transform.localPosition.x > 80)
                        {
                            _birdsList[index].transform.localPosition = _dropers[2].transform.localPosition;
                            RemoveDrops(index, _dropers[2]);
                        }
                        else
                        {
                            _birdsList[index].transform.localPosition = _dropers[1].transform.localPosition;
                            RemoveDrops(index, _dropers[1]);
                        }

                        SpineManager.instance.DoAnimation(_dxg, "x2", false, () =>
                        {
                            for (int i = 0; i < _setGames.Count; i++)
                            {
                                var child = _setGames[i];
                                child.transform.DOLocalMove(
                                    new Vector3(child.transform.localPosition.x, 85f), 0.2f);
                            }

                            SpineManager.instance.DoAnimation(_dxg, "x2-", true);
                        });
                        break;
                    case 3:
                        if (_birdsList[index].transform.localPosition.x <= -140)
                        {
                            _birdsList[index].transform.localPosition =
                                new Vector3(_dropers[0].transform.localPosition.x, 85f);
                            RemoveDrops(index, _dropers[0]);
                        }
                        else if (_birdsList[index].transform.localPosition.x > 80)
                        {
                            _birdsList[index].transform.localPosition =
                                new Vector3(_dropers[2].transform.localPosition.x, 85f);
                            RemoveDrops(index, _dropers[2]);
                        }
                        else
                        {
                            _birdsList[index].transform.localPosition =
                                new Vector3(_dropers[1].transform.localPosition.x, 85f);
                            RemoveDrops(index, _dropers[1]);
                        }


                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd, Random.Range(4,9),()=> { 
                                btnBack.Show();}, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 3, () =>
                            {
                                Wait(0.2f, () =>
                        {
                            for (int i = 0; i < _setGames.Count; i++)
                            {


                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                                var child = _setGames[i];
                                child.transform.DOLocalRotate(new Vector3(0, 0, 360), 0.3f);
                                child.transform.DOLocalMove(new Vector3(-30f, 0f), 0.3f).OnComplete(() =>
                                {
                                    _lifeList[score - 1].Hide();
                                    child.Hide();
                                    if (child.name == "a")
                                    {
                                        SpineManager.instance.DoAnimation(_em, "xem-boom", false, () =>
                                        {
                                            _lifeList[score - 1].Hide();
                                            SpineManager.instance.DoAnimation(_em, "xem", true);
                                            if (score < 3)
                                            {
                                                Wait(2f,
                                                    () => { NextGame(index); });
                                            }
                                            else
                                            {
                                                _em.Hide();

                                                Wait(1f,
                                                    () =>
                                                    {
                                                        playSuccessSpine();
                                                        btnBack.Hide();
                                                    });
                                            }
                                        });
                                    }

                                    if (child.name == "f")
                                    {
                                        SpineManager.instance.DoAnimation(_em, "xem-boom2", false, () =>
                                        {
                                            SpineManager.instance.DoAnimation(_em, "xem", true);
                                            if (score < 3)
                                            {
                                                Wait(2f,
                                                    () => { NextGame(index); });
                                            }
                                            else
                                            {
                                                _em.Hide();
                                                Wait(1f,
                                                    () =>
                                                    {
                                                        playSuccessSpine();
                                                        btnBack.Hide();
                                                    });
                                            }
                                        });
                                    }

                                    if (child.name == "c")
                                    {
                                        SpineManager.instance.DoAnimation(_em, "xem-boom3", false, () =>
                                        {
                                            SpineManager.instance.DoAnimation(_em, "xem", true);
                                            if (score < 3)
                                            {
                                                Wait(2f,
                                                    () => { NextGame(index); });
                                            }
                                            else
                                            {
                                                _em.Hide();
                                                Wait(1f,
                                                    () =>
                                                    {
                                                        playSuccessSpine();
                                                        btnBack.Hide();
                                                    });
                                            }
                                        });
                                    }
                                });

                            }
                            btnBack.Hide();

                            SpineManager.instance.DoAnimation(_dxg, "x3", false,
                                () => { SpineManager.instance.DoAnimation(_dxg, "x2-", true); });
                        });
                            }));
                        }));
                        btnBack.Show();
                        score++;

                        break;
                }

                Debug.Log("set success!");
            }
            else
            {
                if (_birdsList[index].canMove)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd,
                        4, () =>
                        {
                            btnBack.Show();
                            _birdsList[index].DoReset();
                        },
                        () => { btnBack.Hide(); }));
                }
            }

            _birdsList[index].transform.SetSiblingIndex(_index);
        }


        //下一关
        void NextGame(int index)
        {
            drops = new ArrayList(_dropers);
            level = 0;
            SpineManager.instance.DoAnimation(_dxg, "x0", false, () => { btnBack.Hide(); });
            SpineManager.instance.DoAnimation(_em, "xem", true);
            for (int i = 0; i < _birdsList.Length; i++)
            {
                var child = _birdsList[i];
                child.drops = _dropers;
                switch (index)
                {
                    case 0:
                    case 1:
                    case 3:
                        _birdsList[0].DoReset();
                        _birdsList[0].gameObject.Hide();
                        _birdsList[1].DoReset();
                        _birdsList[1].gameObject.Hide();
                        _birdsList[3].DoReset();
                        _birdsList[3].gameObject.Hide();
                        break;
                    case 5:
                    case 6:
                    case 7:
                        _birdsList[5].DoReset();
                        _birdsList[5].gameObject.Hide();
                        _birdsList[6].DoReset();
                        _birdsList[6].gameObject.Hide();
                        _birdsList[7].DoReset();
                        _birdsList[7].gameObject.Hide();
                        break;
                    case 2:
                    case 4:
                    case 8:
                        _birdsList[2].DoReset();
                        _birdsList[2].gameObject.Hide();
                        _birdsList[4].DoReset();
                        _birdsList[4].gameObject.Hide();
                        _birdsList[8].DoReset();
                        _birdsList[8].gameObject.Hide();
                        break;
                }
            }
        }

        //设置不同颜色拖拽失败
        void SetDrops(int index)
        {
            switch (index)
            {
                case 0:
                case 1:
                case 3:

                    for (int i = 0; i < _birdsList.Length; i++)
                    {
                        if (i == 0 || i == 1 || i == 3)
                        {
                            _birdsList[i].drops = _dropers;
                        }
                        else
                        {
                            _birdsList[i].drops = new ILDroper[0];
                        }
                    }

                    break;
                case 5:
                case 6:
                case 7:
                    for (int i = 0; i < _birdsList.Length; i++)
                    {
                        if (i == 5 || i == 6 || i == 7)
                        {
                            _birdsList[i].drops = _dropers;
                        }
                        else
                        {
                            _birdsList[i].drops = new ILDroper[0];
                        }
                    }

                    break;
                case 2:
                case 4:
                case 8:
                    for (int i = 0; i < _birdsList.Length; i++)
                    {
                        if (i == 2 || i == 4 || i == 8)
                        {
                            _birdsList[i].drops = _dropers;
                        }
                        else
                        {
                            _birdsList[i].drops = new ILDroper[0];
                        }
                    }

                    break;
            }
        }

        //设置相同颜色不能重复拖拽一个地方
        void RemoveDrops(int index, ILDroper droper)
        {
            drops.Remove(droper);
            switch (index)
            {
                case 0:
                    _birdsList[1].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[3].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 1:
                    _birdsList[0].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[3].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 3:
                    _birdsList[1].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[0].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 2:
                    _birdsList[4].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[8].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 4:
                    _birdsList[2].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[8].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 8:
                    _birdsList[4].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[2].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 5:
                    _birdsList[5].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[7].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 6:
                    _birdsList[5].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[7].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
                case 7:
                    _birdsList[5].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    _birdsList[6].drops = (ILDroper[])drops.ToArray(typeof(ILDroper));
                    break;
            }
        }


        private void OnClickBtnBack(GameObject obj)
        {
        }


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
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

            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
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
                        mask.Hide();
                        anyBtns.gameObject.SetActive(false);
                        GameInit();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        dbd.Show();
                        bd.Hide();
                        anyBtns.gameObject.Hide();
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, dbd,
                            2, () => { },
                            () => { }));
                    });
                }
            });
        }

        private void GameInit()
        {
            for (int i = 0; i < _birdsList.Length; i++)
            {
                var child = _birdsList[i];
                child.SetDragCallback(StartDrage, null, SetPos);
            }

            Input.multiTouchEnabled = false;
            talkIndex = 1;
            score = 0;
            level = 0;
            drops = new ArrayList(_dropers);
            _setGames.Clear();
            for (int i = 0; i < _birdsList.Length; i++)
            {
                _birdsList[i].DoReset();
                _birdsList[i].canMove = true;
                _birdsList[i].drops = _dropers;
                var temp = _birdsList[i].gameObject;
                temp.Show();
                var child = temp.transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(child, child.name, true);
            }

            for (int i = 0; i < _lifeList.Count; i++)
            {
                _lifeList[i].Show();
            }
            _em.Show();
            SpineManager.instance.DoAnimation(_dxg, "x0", true);
            SpineManager.instance.DoAnimation(_em, "xem", true);
        }

        void GameStart()
        {
            mask.Show();
            bd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, null,
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }

        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, GameObject bd, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");
            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mask.Show();
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null, () =>
                {
                    mask.Hide();
                    bd.Hide();
                }));
            }

            talkIndex++;
        }

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
                            SpineManager.instance.DoAnimation(anyBtns.GetChild(1).gameObject, "ok2");
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
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


        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSencondsDo(time, method));
        }

        IEnumerator WaitForSencondsDo(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
    }
}