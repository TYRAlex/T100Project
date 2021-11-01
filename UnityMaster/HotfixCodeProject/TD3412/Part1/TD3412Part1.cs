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
        next,
    }

    public class TD3412Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject tt;      //小田田
        private GameObject dtt;     //大田田
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject _clickEvent;
        private Transform _clickCopy;
        private GameObject[] _click;
        private bool _canClick;

        private int _successCount;

        //胜利动画名字
        private string tz;
        private string sz;

        private Transform anyBtns;
        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

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

            _clickEvent = curTrans.Find("ClickEvent").gameObject;
            _clickCopy = curTrans.Find("ClickCopy").transform;

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dtt = curTrans.Find("mask/DTT").gameObject;
            dtt.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            talkIndex = 1;
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            GameInit();
        }

        private void AfterEvent(GameObject obj)
        {
            return;
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                string name = obj.name;
                Transform trans = obj.transform;
                if (name == "p1" || name == "p2" || name == "p3")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    float waitTime = BtnPlaySoundFail();
                    mono.StartCoroutine(WaitCoroutine(waitTime, () => { _canClick = true; }));
                    SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, name, false,
                    ()=>
                    {
                        SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, name + "-1", true);
                    });
                }
                else
                {
                    Util.AddBtnClick(obj, AfterEvent);
                    mono.StartCoroutine(WaitCoroutine(1.0f,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    }));
                    float waitTime = BtnPlaySoundSuccess();
                    float time = (waitTime > 1.333) ? waitTime : 1.333f;
                    mono.StartCoroutine(WaitCoroutine(time + 1.0f,
                    () =>
                    {
                        _canClick = true;
                        _successCount += 1;
                        JudgeSuccess();
                    }));
                    SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, name + "3", false,
                        ()=>
                        {
                            Vector3 endPos = curTrans.GetGameObject(name + "_End").transform.localPosition;
                            obj.transform.DOScale(new Vector3(0.8f, 0.8f, 0), 0.5f);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                            obj.transform.DOLocalMove(endPos, 0.8f);
                        });
                    });
                }
            }
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
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
                case BtnEnum.next:
                    result = "next";
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
                        tt.Show();
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false,
                    () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                    });
                }
                else if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); dtt.SetActive(true); mono.StartCoroutine(DBDSpeckerCoroutine(SoundManager.SoundType.VOICE, 2)); });
                }
            });
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
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
            });
        }

        void GameInit()
        {
            DOTween.KillAll();
            _click = new GameObject[_clickEvent.transform.childCount];
            for (int i = 0; i < _clickEvent.transform.childCount; i++)
            {
                _click[i] = _clickEvent.transform.GetChild(i).gameObject;
                Util.AddBtnClick(_click[i], ClickEvent);
            }
            ResetAniAndPos();

            _canClick = true;
            _successCount = 0;
        }

        void GameStart()
        {
            tt.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            _canClick = false;
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        //等待协程
        IEnumerator WaitCoroutine(float len, Action method = null)
        {
            yield return new WaitForSeconds(len);
            method?.Invoke();
        }

        //田田说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(tt, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(tt, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(tt, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //大田田说话
        IEnumerator DBDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dtt, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dtt, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dtt, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    _canClick = true;
                    tt.Hide();
                    mask.Hide();
                }));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private float BtnPlaySoundFail()
        {
            int random = Random.Range(0, 4);
            return SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }
        //成功激励语音
        private float BtnPlaySoundSuccess()
        {
            int random = Random.Range(4,10);
            return SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
        }

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        //全动画初始化与位置复原
        void ResetAniAndPos()
        {
            for (int i = 0; i < _click.Length; i++)
            {
                int random = Random.Range(0, 10);
                _click[i].GetComponent<RectTransform>().anchoredPosition = _clickCopy.GetChild(i).GetComponent<RectTransform>().anchoredPosition;
                _click[i].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 0);
                Transform trans = _click[i].transform;
                if (_click[i].name == "p1" || _click[i].name == "p2" || _click[i].name == "p3")
                {
                    mono.StartCoroutine(WaitCoroutine(0.04f * random,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, trans.GetChild(0).gameObject.name + "-1", true);
                    }));
                }
                else
                {
                    mono.StartCoroutine(WaitCoroutine(0.04f * random,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(trans.GetChild(0).gameObject, trans.GetChild(0).gameObject.name + "2", true);
                    }));
                }
            }
        }

        //胜利判断
        void JudgeSuccess()
        {
            if (_successCount == 4)
            {
                _canClick = false;
                mono.StartCoroutine(WaitCoroutine(2.0f, () => { playSuccessSpine(); }));
            }
        }
    }
}
