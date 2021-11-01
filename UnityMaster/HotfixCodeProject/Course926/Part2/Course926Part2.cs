using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course926Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject bell;

        private GameObject _ani;
        private GameObject _shou;
        private GameObject _click;
        private GameObject _second;
        private float _number;
        private Transform _numberText;
        private GameObject _reset;
        private GameObject _stop;
        private GameObject _return;
        private MonoScripts _monoScripts;

        private bool _canClick;
        private bool _canMove;
        private bool _isEnd;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            _ani = curTrans.GetGameObject("Ani");
            _click = curTrans.GetGameObject("Ani/Click");
            _shou = curTrans.GetGameObject("Shou");
            _second = curTrans.GetGameObject("Second");
            _numberText = curTrans.Find("Number");
            _reset = curTrans.GetGameObject("Second/Reset");
            _stop = curTrans.GetGameObject("Second/Stop");
            _return = curTrans.GetGameObject("Return");
            _monoScripts = Bg.GetComponent<MonoScripts>();
            _monoScripts.FixedUpdateCallBack = SUpdate;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void SUpdate()
        {
            if(_canMove && _numberText.gameObject.activeSelf)
            {
                _number += 0.021f;
                if (_number >= 99.999f)
                {
                    _numberText.GetComponent<Text>().text = string.Format("{0:f2}", _number);
                }
                else
                    _numberText.GetComponent<Text>().text = string.Format("{0:f3}", _number);
            }
        }

        private void GameInit()
        {
            talkIndex = 1;
            _number = 0.000f;

            _canClick = false;
            _canMove = false;
            _isEnd = false;

            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _ani.Show();
            _numberText.gameObject.Hide();
            _second.Hide();
            _return.Hide();
            _shou.Hide();

            Util.AddBtnClick(_click, ClickEvent);
            Util.AddBtnClick(_reset, ResetEvent);
            Util.AddBtnClick(_stop, StopEvent);
            Util.AddBtnClick(_return, ReturnEvent);
        }

        void GameStart()
        {
            bell.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SpineManager.instance.DoAnimation(_ani, "kong", false, ()=> { SpineManager.instance.DoAnimation(_ani, "animation", false); });
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; _click.Show(); _shou.Show(); }));
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
                speaker = bell;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                _shou.Hide();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                SpineManager.instance.DoAnimation(_ani, "animation3", false,
                () =>
                {
                    _ani.Hide();
                    _click.Hide();
                    _second.Show();
                    _reset.Hide();
                    _second.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(_second, "animation", false,
                    () =>
                    {
                        _number = 0;
                        _numberText.gameObject.Show();
                        _numberText.GetComponent<Text>().text = "0.000";
                        if (_isEnd)
                        {
                            _canClick = true;
                            _canMove = true;
                            _reset.Show();
                            _return.Show();
                        }
                        else
                            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () => { _reset.Show(); _canClick = true; _canMove = true; }));
                    });
                });
            }
        }

        private void StopEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(_second, "animation4", false, 
                ()=> 
                {
                    if (_canMove)
                        _canMove = false;
                    else
                        _canMove = true;
                    _canClick = true; 
                });
            }
        }

        private void ResetEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                if (!_isEnd)
                {
                    _number = 0;
                    _numberText.GetComponent<Text>().text = string.Format("{0:f3}", _number);

                    _canMove = false;
                    _isEnd = true;
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () => { _canMove = true; _canClick = true; _return.Show(); }));
                }
                else
                {
                    _canMove = false;
                    _number = 0;
                    _numberText.GetComponent<Text>().text = string.Format("{0:f3}", _number);

                    mono.StartCoroutine(WaitCoroutine(
                    () => 
                    {
                        _canClick = true;
                        _canMove = true;
                        _return.Show();
                    }, 2.0f));
                }
            }
        }

        private void ReturnEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                _return.Hide();
                _reset.Hide();
                _second.Hide();
                _numberText.gameObject.Hide();
                _ani.Show();
                SpineManager.instance.DoAnimation(_ani, "animation2", false, ()=> { _canClick = true; _click.Show(); });
            }
        }
    }
}
