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
    public class TD3422Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject tt;
        private GameObject dtt;
        private GameObject xem;
        private Transform Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;
        private bool _canClickBtn = true;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        //胜利动画名字
        private string tz;
        private string sz;

        private GameObject _ball;
        private GameObject _startBallBg;
        private GameObject _ballHoop;
        private MonoScripts _monoScripts;
        private Transform _pos;
        private Transform _timeBg;
        private Transform _time;
        private GameObject _clock;
        private GameObject _clickBall;
        private GameObject[] _fruit;
        private GameObject[] _xem;
        private bool _canMove;
        private float _speed;

        private EventDispatcher _ballEvent;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg");
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            tt = curTrans.Find("mask/TT").gameObject;
            tt.SetActive(false);
            dtt = curTrans.Find("mask/DTT").gameObject;
            dtt.SetActive(false);
            xem = curTrans.Find("mask/XEM").gameObject;
            xem.SetActive(false);
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

            _ball = curTrans.GetGameObject("Ball");
            _ballHoop = curTrans.GetGameObject("BallHoop");
            _startBallBg = curTrans.GetGameObject("StartBallBg");
            _timeBg = curTrans.Find("TimeBg");
            _time = _timeBg.Find("Time");
            _clock = _timeBg.GetGameObject("Clock");
            _clickBall = curTrans.GetGameObject("ClickBall");
            _monoScripts = curTrans.GetGameObject("MonoScripts").GetComponent<MonoScripts>();
            _pos = curTrans.Find("Pos");
            _fruit = new GameObject[Bg.Find("Fruit").childCount];
            for (int i = 0; i < Bg.Find("Fruit").childCount; i++)
            {
                _fruit[i] = Bg.Find("Fruit").GetChild(i).gameObject;
            }
            _xem = new GameObject[Bg.Find("XEM").childCount];
            for (int i = 0; i < Bg.Find("XEM").childCount; i++)
            {
                _xem[i] = Bg.Find("XEM").GetChild(i).gameObject;
            }

            if (_ball.GetComponent<EventDispatcher>())
            {
                Component.DestroyImmediate(_ball.GetComponent<EventDispatcher>());
                _ball.AddComponent<EventDispatcher>();
                _ballEvent = _ball.GetComponent<EventDispatcher>();
            }

            _monoScripts.FixedUpdateCallBack = SFixedUpdate;
            GameInit();
            //GameStart();
        }

        private void BallEvent(Collider2D other, int time)
        {
            if (other.transform.parent.name == "Fruit")
            {
               _canMove = false;
                DeleteDrag();
                other.transform.gameObject.Hide();
                _speed += 0.09f;
                SpineManager.instance.DoAnimation(_ball, "q3", true);
                if (_startBallBg.activeSelf)
                    _startBallBg.Hide();
                float aniTime = SpineManager.instance.GetAnimationLength(_ball, "q3");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                float audioTime = BtnPlaySoundSuccess();
                mono.StartCoroutine(WaitCoroutine(
                () =>
                {
                    _canMove = true;
                    StartDrag();
                    SpineManager.instance.DoAnimation(_ball, "q4", true);
                }, audioTime > aniTime ? audioTime : aniTime));
            }

            if (other.transform.parent.name == "XEM")
            {
                _canMove = false;
                DeleteDrag();
                other.transform.gameObject.Hide();
                _speed -= 0.1f;
                SpineManager.instance.DoAnimation(_ball, "q7", true);
                if (_startBallBg.activeSelf)
                    _startBallBg.Hide();
                float audioTime = BtnPlaySoundFail();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                mono.StartCoroutine(WaitCoroutine(
                () =>
                {
                    _canMove = true;
                    StartDrag();
                    SpineManager.instance.DoAnimation(_ball, "q4", true);
                }, audioTime));
            }
        }

        private void SFixedUpdate()
        {
            if(_canMove)
            {
                Bg.transform.position = Vector2.Lerp(Bg.transform.position, new Vector2(Bg.transform.position.x, Bg.transform.position.y - 2.5f), _speed);
                _clock.transform.position = Vector2.Lerp(_clock.transform.position, new Vector2(_clock.transform.position.x - 0.455f, _clock.transform.position.y), 0.5f);
                _time.GetComponent<Image>().fillAmount -= 0.000571f;
            }

            if((_time.GetComponent<Image>().fillAmount == 0 || !_fruit[6].activeSelf || _ball.transform.position.y - _fruit[6].transform.position.y >= 100) && _canMove)
            {
                LastAction();
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
            _canClickBtn = true;
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
            ChangeClickArea();
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
                            xem.SetActive(true);
                            GameStart();
                        });
                    }
                    else if (obj.name == "fh")
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); _canMove = true; });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dtt.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dtt, SoundManager.SoundType.VOICE, 2)); });
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

            anyBtns.GetComponent<RectTransform>().sizeDelta = activeCount == 2 ? new Vector2(680, 240) : new Vector2(240, 240);
        }

        #endregion

        private void GameInit()
        {
            talkIndex = 1;
            _speed = 0.55f;
            _time.GetComponent<Image>().fillAmount = 1;
            _canMove = false;

            for (int i = 0; i < _xem.Length; i++)
            {
                _xem[i].Show();
            }
            for (int i = 0; i < _fruit.Length; i++)
            {
                _fruit[i].Show();
            }
            _ball.Show();
            _ballHoop.Hide();
            _startBallBg.Show();
            _clickBall.Hide();

            _ball.transform.position = _pos.Find("BallStartPos").position;
            _ballHoop.transform.position = _pos.Find("BallHoopStartPos").position;
            Bg.transform.position = _pos.Find("BgPos").position;
            _clock.transform.position = _timeBg.Find("ClockStartPos").position;

            SpineManager.instance.DoAnimation(_ball, "q4", false);

            _ballEvent.TriggerEnter2D -= BallEvent;
            _ballEvent.TriggerEnter2D += BallEvent;
            StartDrag();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(XEMCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                xem.Hide();
                tt.Show();
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    tt.Hide();
                    mask.Hide();
                    _canMove = true;
                }));
            }));
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
                speaker = tt;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator XEMCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(xem, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(xem, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(xem, "daiji");
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
                tt.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); tt.SetActive(false); }));
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
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    ChangeClickArea();
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
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
        private float BtnPlaySoundFail()
        {
            int random;
            do
            {
                random = Random.Range(0, 4);
            }
            while (random == 0);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
            return time;
        }
        //成功激励语音
        private float BtnPlaySoundSuccess()
        {
            int random;
            do
            {
                random = Random.Range(4, 10);
            }
            while (random == 4 || random == 8);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, random, false);
            return time;
        }

        #region 拖拽事件
        //小球拖拽中事件
        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            if (_startBallBg.activeSelf)
                _startBallBg.Hide();
            if (SpineManager.instance.GetCurrentAnimationName(_ball) != "q")
                SpineManager.instance.DoAnimation(_ball, "q", true);
        }

        //小球拖拽结束事件
        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            SpineManager.instance.DoAnimation(_ball, "q2", false);
        }

        //移除拖拽事件
        void DeleteDrag()
        {
            _ball.GetComponent<mILDrager>().canMove = false;
            _ball.GetComponent<mILDrager>().SetDragCallback(null, null, null, null);
        }

        //添加拖拽事件
        void StartDrag()
        {
            _ball.GetComponent<mILDrager>().canMove = true;
            _ball.GetComponent<mILDrager>().SetDragCallback(null, Draging, EndDrag, null);
        }
        #endregion

        #region 其他
        void LastAction()
        {
            _canMove = false;
            DeleteDrag();
            SpineManager.instance.DoAnimation(_ball, "q4", false);
            for (int i = 0; i < _xem.Length; i++)
            {
                _xem[i].Hide();
            }
            for (int i = 0; i < _fruit.Length; i++)
            {
                _fruit[i].Hide();
            }
            if (_startBallBg.activeSelf)
                _startBallBg.Hide();
            _ballHoop.Show();
            _ballHoop.transform.DOMove(_pos.Find("BallHoopEndPos").position, 2.0f);
            mono.StartCoroutine(WaitCoroutine(
            () =>
            {
                _clickBall.Show();
                Util.AddBtnClick(_clickBall, ThrowBall);
            }, 2.0f));
        }

        //投球操作
        private void ThrowBall(GameObject obj)
        {
            _clickBall.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(_ball, "q6", false);
            _ball.transform.DOMove(_pos.transform.Find("ThrowPos").position, 1.0f);
            mono.StartCoroutine(WaitCoroutine(() => { playSuccessSpine(); }, 2.5f));
        }
        #endregion
    }
}
