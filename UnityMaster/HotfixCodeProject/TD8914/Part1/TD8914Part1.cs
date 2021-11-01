using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
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

    public class TD8914Part1
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

        //胜利动画名字
        private string tz;
        private string sz;

        //---------------------------------------------------------------------
        private GameObject _parent;
        private List<ILDrager> _dragList;
        private List<ILDroper> _droperList;
        private List<GameObject> _liftList;
        private GameObject _dd;


        private GameObject _dunPai;
        private GameObject _ani;
        private GameObject _d1;
        private BellSprites _d1Sprites;
        private GameObject _d2;
        private BellSprites _d2Sprites;
        private GameObject _d3;
        private BellSprites _d3Sprites;
        private GameObject _dg;

        private int _life;

        private int _score;
        private bool isstart;
        private int _level;

        //-------------------------------------------------------------------------
        private GameObject _timeDown;

        private BellSprites _timeSprites;

        private int _index;

        int layer;
        //-------------------------------------------------------------------------


        //---------------------------------------------------------------------

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
            mask.SetActive(false);

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
            btnBack.SetActive(false);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);


            //---------------------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            _liftList = new List<GameObject>();
            _dragList = new List<ILDrager>();
            _droperList = new List<ILDroper>();
            var parentTransform = _parent.transform;
            _dd = parentTransform.GetGameObject("dd");

            _dunPai = parentTransform.GetGameObject("dunpai");
            _d1 = _dunPai.transform.GetGameObject("d1");
            _d2 = _dunPai.transform.GetGameObject("d2");
            _d3 = _dunPai.transform.GetGameObject("d3");
            _d1Sprites = _d1.GetComponent<BellSprites>();
            _d2Sprites = _d2.GetComponent<BellSprites>();
            _d3Sprites = _d3.GetComponent<BellSprites>();
            _ani = _dunPai.transform.GetGameObject("ani");
            _dg = parentTransform.GetGameObject("dg");
            for (int i = 0; i < parentTransform.GetGameObject("life").transform.childCount; i++)
            {
                var child = parentTransform.GetGameObject("life").transform.GetChild(i).gameObject;
                _liftList.Add(child);
            }

            for (int i = 0; i < parentTransform.GetGameObject("drages").transform.childCount; i++)
            {
                var child = parentTransform.GetGameObject("drages").transform.GetChild(i).GetComponent<ILDrager>();
                _dragList.Add(child);
                child.SetDragCallback(StarDrag, null, SetPos);
            }

            for (int i = 0; i < parentTransform.GetGameObject("drops").transform.childCount; i++)
            {
                var child = parentTransform.GetGameObject("drops").transform.GetChild(i).GetComponent<ILDroper>();
                _droperList.Add(child);
            }


            //------------------------------------------------------------------------------
            _timeDown = parentTransform.GetGameObject("timeDown");
            _timeSprites = _timeDown.transform.GetChild(0).GetComponent<BellSprites>();
            //---------------------------------------------------------------------------------


            GameInit();
            IsStart();
            //GameStart();
        }

        private void StarDrag(Vector3 arg1, int arg2, int index)
        {
            layer = _dragList[index].transform.GetSiblingIndex();
            _dragList[index].transform.SetAsLastSibling();
        }

        void IsStart()
        {
            mask.Show();
            anyBtns.transform.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.gameObject.Show();
        }


        private Tween tw;


        void TimeDown()
        {
            _timeDown.transform.GetChild(0).GetComponent<Image>().sprite = _timeSprites.sprites[_index];
            if (_index > 0)
            {
                Wait(1f, () =>
                {
                    _index--;
                    _timeDown.transform.GetChild(0).GetComponent<Image>().sprite = _timeSprites.sprites[_index];
                    TimeDown();
                });
            }

            if (_index == 0)
            {

                btnBack.Show();
                _dg.Hide();
                Wait(0.5f,
                    () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 3,
                            () => { }, () => { }));
                    });
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                SpineManager.instance.DoAnimation(_dd, "dd-dg", false, () =>
                {
                    _dg.Show();
                    _liftList[_life].Hide();
                    _life++;
                    _score++;

                    SpineManager.instance.DoAnimation(_dd, "dd", true);
                    SpineManager.instance.DoAnimation(_dg, "dg2", true);

                    if (_life < 5)
                    {
                        Wait(3f, () =>
                        {
                            NextSeence(_score);
                            btnBack.Hide();
                        });
                    }
                    else
                    {
                        Wait(2f, () =>
                        {
                            btnBack.Hide();
                            mask.Show();
                            anyBtns.gameObject.Show();
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(0).gameObject.Show();
                            anyBtns.GetChild(1).gameObject.Hide();
                        });
                    }
                });
                Debug.Log("Score:        " + _score);


            }
        }


        private void SetPos(Vector3 endPos, int type, int index, bool setTrue)
        {
            _dragList[index].transform.SetSiblingIndex(layer);
            if (setTrue)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                if (_index != 0)
                {
                    switch (_score)
                    {

                        case 0:
                            switch (index)
                            {
                                case 0:
                                    _d2.Show();
                                    _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-b", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-b2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 1:
                                    _d3.Show();
                                    _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-c", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-c2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 2:
                                    _d1.Show();
                                    _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-a", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-a2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                            }

                            break;
                        case 1:
                            switch (index)
                            {
                                case 0:
                                    _d3.Show();
                                    _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-c", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-c2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 1:
                                    _d1.Show();
                                    _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-a", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-a2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 2:
                                    _d2.Show();
                                    _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-b", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-b2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                            }

                            break;
                        case 2:
                            switch (index)
                            {
                                case 0:
                                    _d2.Show();
                                    _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-b", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-b2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 1:
                                    _d3.Show();
                                    _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-c", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-c2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 2:
                                    _d1.Show();
                                    _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-a", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-a2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                            }

                            break;
                        case 3:
                            switch (index)
                            {
                                case 0:
                                    _d1.Show();
                                    _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-a", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-a2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;

                                case 1:
                                    _d3.Show();
                                    _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-c", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-c2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 2:
                                    _d2.Show();
                                    _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-b", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-b2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                            }

                            break;
                        case 4:
                            switch (index)
                            {
                                case 0:
                                    _d2.Show();
                                    _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-b", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-b2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 1:
                                    _d3.Show();
                                    _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-c", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-c2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                                case 2:
                                    _d1.Show();
                                    _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[_score];
                                    SpineManager.instance.DoAnimation(_ani, "dun-a", false,
                                        () => { SpineManager.instance.DoAnimation(_ani, "dun-a2", false); });
                                    _dragList[index].gameObject.Hide();
                                    _dragList[index].DoReset();
                                    break;
                            }

                            break;
                    }

                    _level++;
                    _dragList[index].gameObject.Hide();
                    if (_level == 3)
                    {
                        mono.StopAllCoroutines();
                        btnBack.Show();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                        SpineManager.instance.DoAnimation(_ani, "dun", false,
                            () => { SpineManager.instance.DoAnimation(_ani, "dun2", false, () => { }); });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        SpineManager.instance.DoAnimation(_dg, "dg", false,
                            () => { SpineManager.instance.DoAnimation(_dg, "dg2", false, () => { }); });
                        _score++;
                        if (_score < 5)
                        {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 2, null,
                                () =>
                                {
                                    Wait(2f, () =>
                                    {
                                        NextSeence(_score);
                                        btnBack.Hide();
                                    });
                                }));
                        }

                        if (_score == 5 && _life < 5)
                        {
                            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 2, null,
                                () =>
                                {
                                    Wait(1f, () =>
                                    {
                                        playSuccessSpine();
                                        btnBack.Hide();
                                    });
                                }));
                        }
                    }
                }
                else
                {
                    //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                    _dragList[index].DoReset();

                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                _dragList[index].DoReset();
            }
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


        void InitDun(int index)
        {
            for (int i = 0; i < _parent.transform.GetGameObject("drages").transform.childCount; i++)
            {
                var child = _parent.transform.GetGameObject("drages").transform.GetChild(i).gameObject;
                child.GetComponent<Image>().sprite = child.GetComponent<BellSprites>().sprites[index];
                child.GetComponent<ILDrager>().canMove = true;
            }
        }


        void NextSeence(int score)
        {
            _level = 0;


            SpineManager.instance.DoAnimation(_dg, "dg2", true);
            SpineManager.instance.DoAnimation(_ani, "dun1", true);

            switch (score)
            {
                case 0:
                    InitDun(score);
                    InitSprites(score);
                    _dragList[0].dragType = 1;
                    _dragList[1].dragType = 2;
                    _dragList[2].dragType = 0;
                    _dragList[0].drops[0] = _droperList[1];
                    _dragList[1].drops[0] = _droperList[2];
                    _dragList[2].drops[0] = _droperList[0];
                    _index = 10;
                    break;
                case 1:

                    InitSprites(score);
                    InitDun(score);
                    _dragList[0].dragType = 2;
                    _dragList[1].dragType = 0;
                    _dragList[2].dragType = 1;
                    _dragList[0].drops[0] = _droperList[2];
                    _dragList[1].drops[0] = _droperList[0];
                    _dragList[2].drops[0] = _droperList[1];
                    _index = 9;
                    TimeDown();

                    break;
                case 2:

                    InitSprites(score);
                    InitDun(score);
                    _dragList[0].dragType = 1;
                    _dragList[1].dragType = 2;
                    _dragList[2].dragType = 0;
                    _dragList[0].drops[0] = _droperList[1];
                    _dragList[1].drops[0] = _droperList[2];
                    _dragList[2].drops[0] = _droperList[0];
                    _index = 8;
                    TimeDown();

                    break;
                case 3:

                    InitSprites(score);
                    InitDun(score);
                    _dragList[0].dragType = 0;
                    _dragList[1].dragType = 2;
                    _dragList[2].dragType = 1;
                    _dragList[0].drops[0] = _droperList[0];
                    _dragList[1].drops[0] = _droperList[2];
                    _dragList[2].drops[0] = _droperList[1];
                    _index = 7;
                    TimeDown();

                    break;
                case 4:

                    InitSprites(score);
                    InitDun(score);
                    _dragList[0].dragType = 1;
                    _dragList[1].dragType = 2;
                    _dragList[2].dragType = 0;
                    _dragList[0].drops[0] = _droperList[1];
                    _dragList[1].drops[0] = _droperList[2];
                    _dragList[2].drops[0] = _droperList[0];
                    _index = 6;
                    TimeDown();
                    break;
                case 5:
                    playSuccessSpine();
                    break;
            }
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
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                        TimeDown();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, dbd, 4, null,
                            () => { }));
                    });
                }
            });
        }

        private void GameInit()
        {
            _level = 0;
            _score = 0;
            NextSeence(0);
            _life = 0;
            for (int i = 0; i < _liftList.Count; i++)
            {
                _liftList[i].Show();
            }

            _index = 10;
            _timeDown.transform.GetChild(0).GetComponent<Image>().sprite = _timeSprites.sprites[10];
            isstart = false;
            talkIndex = 1;
            SpineManager.instance.DoAnimation(_ani, "dun1", false, () => { });
            SpineManager.instance.DoAnimation(_dd, "dd", true, () => { });
            SpineManager.instance.DoAnimation(_dg, "dg2", false, () => { });
        }

        void InitSprites(int index)
        {
            _d1.GetComponent<Image>().sprite = _d1Sprites.sprites[index];
            _d2.GetComponent<Image>().sprite = _d2Sprites.sprites[index];
            _d3.GetComponent<Image>().sprite = _d3Sprites.sprites[index];
            for (int i = 0; i < _dragList.Count; i++)
            {
                _dragList[i].gameObject.Show();
                _dragList[i].DoReset();
                _dragList[i].GetComponent<Image>().sprite = _dragList[i].GetComponent<BellSprites>().sprites[index];
            }

            _d1.Hide();
            _d2.Hide();
            _d3.Hide();
        }


        void GameStart()
        {
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 5, null,
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, GameObject bd, int clipIndex,
            Action method_1 = null,
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
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null, () =>
                {
                    isstart = true;
                    mask.SetActive(false);
                    TimeDown();
                    bd.SetActive(false);
                }));
            }

            if (talkIndex == 2)
            {
                
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
                            anyBtns.gameObject.Show();
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.GetChild(0).gameObject.Show();
                            anyBtns.GetChild(1).gameObject.Show();
                            SpineManager.instance.DoAnimation(anyBtns.GetChild(1).gameObject, "ok2", true);
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
            mono.StartCoroutine(WaitForSencondDoSomthing(time, method));
        }

        IEnumerator WaitForSencondDoSomthing(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
    }
}