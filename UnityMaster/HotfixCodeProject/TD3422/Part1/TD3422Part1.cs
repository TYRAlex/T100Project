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
    public class TD3422Part1
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
        private string tz;
        private string sz;

        private GameObject _plant;
        private GameObject _plantClick;
        private GameObject _dg;
        private GameObject _bian;
        private GameObject _light;
        private GameObject _shou;
        private GameObject _star;
        private Transform _copy;
        private GameObject[] _copyArray;
        private int _level;
        private int _clickCount;
        private string _plantName;
        private bool _canClick;

        private bool _canClickBtn = true;
        private GameObject _ok;
        private GameObject _next;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

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
            ChangeClickArea();

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _plant = curTrans.GetGameObject("Plant");
            _plantClick = curTrans.GetGameObject("Plant/0");
            _dg = curTrans.GetGameObject("DG");
            _bian = curTrans.GetGameObject("BIAN");
            _light = curTrans.GetGameObject("LIGHT");
            _shou = curTrans.GetGameObject("SHOU");
            _star = curTrans.GetGameObject("STAR");
            _copy = curTrans.Find("Copy");

            _copyArray = new GameObject[_copy.childCount];
            for (int i = 0; i < _copy.childCount; i++)
            {
                _copyArray[i] = _copy.GetChild(i).gameObject;
            }

            Util.AddBtnClick(_plantClick, ClickPlant);

            _ok = curTrans.GetGameObject("ok");
            _next = curTrans.GetGameObject("next");
            Util.AddBtnClick(_ok, NextLevel);
            Util.AddBtnClick(_next, NextLevel);
            GameInit();
            //GameStart();
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
            if(_canClickBtn)
            {
                _canClickBtn = false;
                BtnPlaySound();
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    if (obj.name == "bf")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                        {
                            anyBtns.gameObject.SetActive(false);
                            _canClickBtn = true;
                            GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); StartAni(); _canClickBtn = true; });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                        { 
                            switchBGM(); 
                            anyBtns.gameObject.SetActive(false); 
                            dbd.SetActive(true); 
                            _canClickBtn = true; 
                            mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2, null, 
                            ()=> 
                            {
                                mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3, null, null));
                            })); });
                    }

                });
            }
        }

        #region 根据按钮数量调整点击区域
        void ChangeClickArea()
        {
            int activeCount = 0;
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                if (anyBtns.GetChild(i).gameObject.activeSelf)
                    activeCount += 1;
            }

            Debug.LogError(activeCount);
            anyBtns.GetComponent<RectTransform>().sizeDelta = activeCount == 2 ? new Vector2(680, 240) : new Vector2(240, 240);
        }

        #endregion

        private void GameInit()
        {
            talkIndex = 1;
            _level = 1;
            _clickCount = 0;
            _canClick = false;
            _canClickBtn = true;

            _ok.Hide();
            _next.Hide();

            InitAni();
            ChangeCopyName();
        }

        void InitAni()
        {
            SpineManager.instance.DoAnimation(_dg, "kong", false);
            SpineManager.instance.DoAnimation(_bian, "kong", false);
            SpineManager.instance.DoAnimation(_light, "kong", false);
            SpineManager.instance.DoAnimation(_shou, "kong", false);
            SpineManager.instance.DoAnimation(_star, "kong", false);
            SpineManager.instance.DoAnimation(_plant, "kong", false);
            for (int i = 0; i < _copyArray.Length; i++)
            {
                SpineManager.instance.DoAnimation(_copyArray[i], "kong", false);
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }
        
        void StartAni()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SpineManager.instance.DoAnimation(_shou, "SHOU", true);
            SpineManager.instance.DoAnimation(_plant, _plantName + "0", true);
            mono.StartCoroutine(WaitCoroutine(
            () =>
            {
                SpineManager.instance.DoAnimation(_shou, "kong", false);
                SpineManager.instance.DoAnimation(_plant, _plantName + "00", false);
                _canClick = true;
            }, 2.0f));
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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, 
                () => 
                {
                    bd.Hide();
                    mask.Hide();
                    StartAni();
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
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                () =>
                {
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    ChangeClickArea();
                    caidaiSpine.SetActive(false); 
                    successSpine.SetActive(false);
                    ac?.Invoke();
                });
                });
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
            int random;
            do
            {
                random = Random.Range(4, 10);
            }
            while (random == 4 || random == 8);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }

        void ChangeCopyName()
        {
            if (_level == 1)
                _plantName = "cm";
            if (_level == 2)
                _plantName = "xiang";
            if (_level == 3)
                _plantName = "mihou";
        }


        private void ClickPlant(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                _clickCount++;
                string str = _plantName + _clickCount.ToString();
                if(_clickCount == 1)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_star, _star.name, true);
                    SpineManager.instance.DoAnimation(_light, _light.name, true);
                    SpineManager.instance.DoAnimation(_plant, str, true);
                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        mono.StartCoroutine(WaitCoroutine(
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        }, 1.0f));
                        SpineManager.instance.DoAnimation(_copyArray[_clickCount - 1], str + "-b", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_star, "kong", true);
                            SpineManager.instance.DoAnimation(_light, "kong", true);
                            _canClick = true;
                            ChangeCopyName();
                        });
                    }, 2.0f));
                }
                else
                {
                    mono.StartCoroutine(WaitCoroutine(
                    () => 
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false); 
                    }, 1.3f));
                    SpineManager.instance.DoAnimation(_dg, _dg.name, false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_dg, "kong", false);
                        SpineManager.instance.DoAnimation(_bian, _bian.name, false, 
                        () => 
                        {
                            SpineManager.instance.DoAnimation(_bian, "kong", false);
                            SpineManager.instance.DoAnimation(_star, _star.name, true);
                            SpineManager.instance.DoAnimation(_light, _light.name, true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            SpineManager.instance.DoAnimation(_plant, str, true);
                            mono.StartCoroutine(WaitCoroutine(
                            () => 
                            {
                                mono.StartCoroutine(WaitCoroutine(
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                }, 1.0f));
                                SpineManager.instance.DoAnimation(_copyArray[_clickCount - 1], str + "-b", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_star, "kong", true);
                                    SpineManager.instance.DoAnimation(_light, "kong", true);
                                    if (_clickCount >= 6)
                                    {
                                        BtnPlaySoundSuccess();
                                        mono.StartCoroutine(WaitCoroutine(
                                        () =>
                                        {
                                            if (_level >= 3)
                                            {
                                                _ok.Show();
                                                SpineManager.instance.DoAnimation(_ok, "ok2", false);
                                            }
                                            else
                                            {
                                                _next.Show();
                                                SpineManager.instance.DoAnimation(_next, "next2", false);
                                            }
                                        }, 2.0f));
                                    }
                                    else
                                        _canClick = true;
                                });
                            }, 2.0f));
                        });
                    });
                }
            }
        }

        void NextLevel(GameObject obj)
        {
            if(_canClickBtn)
            {
                _canClickBtn = false;
                _clickCount = 0;
                _level += 1;

                BtnPlaySound();
                if (_level > 3)
                {
                    SpineManager.instance.DoAnimation(_ok, obj.name, false,
                    () =>
                    {
                        _ok.Hide();
                        _canClickBtn = true;
                        playSuccessSpine();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(_next, obj.name, false,
                    () =>
                    {
                        _next.Hide();
                        _canClickBtn = true;
                        ChangeCopyName();
                        for (int i = 0; i < _copyArray.Length; i++)
                        {
                            SpineManager.instance.DoAnimation(_copyArray[i], "kong", false);
                        }
                        StartAni();
                    });
                }
            }
        }
    }
}
