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
    public class TD3412Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;

        private GameObject successSpine;
        private GameObject mask;

        private Transform _click;
        private Transform _ani;
        private GameObject _return;
        private bool[] _clickAll;
        private bool _canClick;
        private GameObject _curObj;
        private int _curIndex;

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

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);

            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            Util.AddBtnClick(anyBtn, OnClickAnyBtn);

            _click = curTrans.Find("Click");
            _ani = curTrans.Find("Ani");
            _return = curTrans.GetGameObject("Return");

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        /// <summary>
        /// ????????????mode
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
            SpineManager.instance.DoAnimation(anyBtn, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(anyBtn, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                }
                else
                {
                    GameInit();
                }
                mask.gameObject.SetActive(false);

            });
        }

        private void GameInit()
        {
            talkIndex = 1;

            _clickAll = new bool[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
                Util.AddBtnClick(_click.GetChild(i).gameObject, ClickEvent);
            }
            Util.AddBtnClick(_return, ReturnEvent);

            for (int i = 0; i < _ani.childCount; i++)
            {
                GameObject o = _ani.GetChild(i).gameObject;
                o.Show();
                SpineManager.instance.DoAnimation(o, "kong", false, 
                () => 
                {
                    SpineManager.instance.DoAnimation(o, o.name + "3", false);
                });
            }

            _return.Hide();
            _canClick = false;
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //mono.startcoroutine(speckercoroutine(soundmanager.soundtype.commonvoice, 0, null, () => { soundmanager.instance.showvoicebtn(true); }));
            //????????????????????????????????????????????????????????????????????????????????????????????????
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                bd.SetActive(false);
                mask.SetActive(false);
                _canClick = true;
            }));
        }
        //bell????????????
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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
                _canClick = false;
                bd.SetActive(true);
                //??????????????????????????????????????????
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, null));
            }
            talkIndex++;
        }
        /// <summary>
        /// ??????????????????
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(successSpine, "1", false, () => { SpineManager.instance.DoAnimation(successSpine, "2", false, () => {/* anyBtn.name = getBtnName(BtnEnum.fh);*/ successSpine.SetActive(false); ac?.Invoke(); }); });
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //??????????????????
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //??????????????????
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1, false);
                _curIndex = Convert.ToInt32(obj.name);
                _curObj = _ani.GetChild(_curIndex).gameObject;
                _clickAll[_curIndex] = true;

                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _curIndex + 1, 
                ()=> 
                {
                    _curObj.transform.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(_curObj, _curObj.name, false);
                }, 
                () =>
                {
                    _return.Show();
                }, 0));
            }
        }
        private void ReturnEvent(GameObject obj)
        {
            _return.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(_curObj, _curObj.name + "2", false, 
            () => 
            {
                _curObj.transform.SetSiblingIndex(_curIndex);
                _canClick = true;
                JudgeAllClick();
            });
        }

        //????????????????????????????????????
        void JudgeAllClick()
        {
            bool allClick = true;
            for (int i = 0; i < _clickAll.Length; i++)
            {
                if (_clickAll[i] != true)
                {
                    allClick = false;
                }
            }

            if (allClick)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
    }
}
