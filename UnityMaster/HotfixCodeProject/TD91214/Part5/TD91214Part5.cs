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
    public class TD91214Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject xem;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        //private GameObject pageBar;
        //private Transform SpinePage;

        //private Empty4Raycast[] e4rs;

        //private GameObject rightBtn;
        //private GameObject leftBtn;

        //private GameObject btnBack;

        //private int curPageIndex;  //当前页签索引
        //private Vector2 _prePressPos;

        //private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        //private GameObject buDing;
        //private Text bdText;
        //private GameObject devil;
        //private Text devilText;

        //private Transform bdStartPos;
        //private Transform bdEndPos;
        //private Transform devilStartPos;
        //private Transform devilEndPos;
      
        //胜利动画名字
        private string tz;
        private string sz;
        //bool isPlaying = false;
        //bool isPressBtn = false;

        private GameObject _fair;
        private Transform _fairBtn;
        private GameObject _lion;
        private Transform _pos;
        private GameObject _devil;
        private string[] _devilAni;
        private MonoScripts _monoScripts;

        private Tween _devilTween;
        private Tween _timeTween;
        private bool _startMove;
        private bool _isBoom;
        private bool _gameEnd;
        private bool _canClick;

        private int _count;

        private Transform _devilCount;
        private GameObject _clock;
        private Image _time;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);

            //buDing = curTrans.Find("mask/buDing").gameObject;
            //bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            //bdStartPos = curTrans.Find("mask/bdStartPos");
            //buDing.transform.position = bdStartPos.position;
            //bdEndPos = curTrans.Find("mask/bdEndPos");

            //devil = curTrans.Find("mask/devil").gameObject;
            //devilText = devil.transform.GetChild(0).GetComponent<Text>();
            //devilStartPos = curTrans.Find("mask/devilStartPos");
            //devil.transform.position = devilStartPos.position;
            //devilEndPos = curTrans.Find("mask/devilEndPos");

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
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

            //pageBar = curTrans.Find("PageBar").gameObject;
            //SlideSwitchPage(pageBar);
            //SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            //SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            //e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            //for (int i = 0, len = e4rs.Length; i < len; i++)
            //{
            //    Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            //}

            //leftBtn = curTrans.Find("L2/L").gameObject;
            //rightBtn = curTrans.Find("R2/R").gameObject;

            //Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            //Util.AddBtnClick(rightBtn, OnClickBtnRight);

            //btnBack = curTrans.Find("btnBack").gameObject;
            //Util.AddBtnClick(btnBack, OnClickBtnBack);
            //btnBack.SetActive(false);

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            _fair = curTrans.GetGameObject("Fair");
            _fairBtn = curTrans.Find("FairBtn");
            for (int i = 0; i < _fairBtn.childCount; i++)
            {
                Util.AddBtnClick(_fairBtn.GetChild(i).gameObject, ClickFair);
            }

            _devil = curTrans.GetGameObject("Devil");
            _lion = curTrans.GetGameObject("Lion");
            _pos = curTrans.Find("Pos");
            _monoScripts = curTrans.Find("MonoScripts").GetComponent<MonoScripts>();
            _monoScripts.FixedUpdateCallBack = SUpdate;
            _devilAni = new string[6] { "xem-a", "xem-b", "xem-c", "xem-e", "xem-f", "notSameAni" };

            _devilCount = curTrans.Find("CountBg/XEM");
            _time = curTrans.Find("TimeBg/Time").GetComponent<Image>();
            _clock = curTrans.GetGameObject("TimeBg/Clock");

            _gameEnd = false;
            GameInit();
            //GameStart();
        }

        //private void OnClickBtnRight(GameObject obj)
        //{
        //    //if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
        //    //    return;
        //    //isPressBtn = true;
        //    BtnPlaySound();
        //    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        //}

        //private void OnClickBtnLeft(GameObject obj)
        //{
        //    if (curPageIndex <= 0 || isPlaying || isPressBtn)
        //        return;
        //    isPressBtn = true;
        //    BtnPlaySound();
        //    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        //}

        //private GameObject tem;
        //private void OnClickBtnBack(GameObject obj)
        //{
        //    if (isPressBtn)
        //        return;
        //    isPressBtn = true;
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
        //    SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false); isPlaying = false; isPressBtn = false; }); });
        //}

        //private void OnClickShow(GameObject obj)
        //{
        //    if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
        //    {
        //        SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
        //    }
        //    if (isPlaying || isPressBtn)
        //        return;
        //    isPlaying = true;
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
        //    tem = obj.transform.parent.gameObject;
        //    tem.transform.SetAsLastSibling();
        //    SpineManager.instance.DoAnimation(tem, obj.name, false, () => { SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () => { btnBack.SetActive(true); }); });
        //}


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
                else if(obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd ,SoundManager.SoundType.VOICE, 6)); });
                }
               
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            _count = 0;
            _time.fillAmount = 0;
            _startMove = true;
            _clock.transform.localPosition = curTrans.Find("TimeBg/StartPos").localPosition;

            SpineManager.instance.DoAnimation(_lion, "ss2", false);
            SpineManager.instance.DoAnimation(_devil, "kong", false);
            SpineManager.instance.DoAnimation(_fair, "kong", false);
            SpineManager.instance.DoAnimation(_fairBtn.GetChild(0).gameObject, "red2", false);
            SpineManager.instance.DoAnimation(_fairBtn.GetChild(1).gameObject, "blue2", false);

            JudgeCount();
            StartMove();
            DevilMove();
            if (!_gameEnd)
            {
                _devil.Hide();
                StopMove();
                _isBoom = true;
            }
            else
            {
                RandomDevilAni();
                _isBoom = false;
            }
            _gameEnd = false;
            _canClick = true;
            _fair.transform.position = _pos.GetChild(0).position;
            _devil.transform.position = _pos.GetChild(1).position;
            //curPageIndex = 0;
            //isPressBtn = false;
            //textSpeed =0.1f;
            //SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            //for (int i = 0; i < SpinePage.childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            //}

            //SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            //SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(XemSpeckerCoroutine(SoundManager.SoundType.VOICE, 0, 
            () => 
            {
                mask.Show();
                xem.Show();
                _startMove = false;
                StopMove();
            }, 
            () => 
            {
                xem.Hide();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                    _startMove = true;
                    _isBoom = false;
                    _devil.Show();
                    RandomDevilAni();
                    DevilMove();
                }));
            }));
            
            //devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, () =>
            //    {
            //        ShowDialogue("", devilText);
            //    }, () =>
            //    {
            //        buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //        {
            //            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 1, () =>
            //            {
            //                ShowDialogue("", bdText);
            //            }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //        });
            //    }));

            //});
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
        IEnumerator SpeckerCoroutine(GameObject speakObj, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (speakObj == null)
                speakObj = bd;
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speakObj, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speakObj, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speakObj, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        IEnumerator XemSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }
        //    private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        //    {
        //        if (isPlaying)
        //            return;
        //        isPlaying = true;
        //        curPageIndex -= LorR;
        //        SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        //    }

        //    private void SlideSwitchPage(GameObject rayCastTarget)
        //    {
        //        UIEventListener.Get(rayCastTarget).onDown = downData =>
        //        {
        //            _prePressPos = downData.pressPosition;
        //        };

        //        UIEventListener.Get(rayCastTarget).onUp = upData =>
        //        {
        //            float dis = (upData.position - _prePressPos).magnitude;
        //            bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

        //            if (dis > 100)
        //            {
        //                if (!isRight)
        //                {
        //                    if (curPageIndex <= 0 || isPlaying)
        //                        return;
        //                    SetMoveAncPosX(1);
        //                }
        //                else
        //                {
        //                    if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
        //                        return;
        //                    SetMoveAncPosX(-1);
        //                }
        //            }
        //        };
        //    }

        //    void ShowDialogue(string str, Text text, Action callBack = null)
        //    {
        //        mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        //    }

        //    IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        //    {
        //        int i = 0;
        //        str = str.Replace(" ", "\u00A0");  //空格非换行
        //        while (i <= str.Length - 1)
        //        {
        //            yield return new WaitForSeconds(textSpeed);
        //            text.text += str[i];
        //            if (i==25)
        //            {
        //                text.text = "";
        //            }
        //            i++;
        //        }
        //        callBack?.Invoke();
        //        yield break;
        //    }

        //点击火焰按钮
        private void ClickFair(GameObject obj)
        {
            if(_startMove && _canClick)
            {
                _canClick = false;
                SpineManager.instance.DoAnimation(obj, obj.name, false);
                SpineManager.instance.DoAnimation(_lion, "ss", false,
                ()=> 
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(_fair, "huo-" + obj.name, true);
                    _startMove = false;
                });
            }
        }

        private void SUpdate()
        {
            //控制火焰前进
            if (!SpineManager.instance.GetCurrentAnimationName(_fair).Equals("kong") && !SpineManager.instance.GetCurrentAnimationName(_fair).Equals("boom"))
            {
                _fair.transform.position = Vector2.Lerp(_fair.transform.position, new Vector2(_fair.transform.position.x + Screen.width/2, _fair.transform.position.y), Time.deltaTime);
                //判断火焰是否撞到了小恶魔
                if (_fair.transform.position.x >= _devil.transform.position.x && !_isBoom)
                {
                    string fairName = SpineManager.instance.GetCurrentAnimationName(_fair);
                    string devilName = SpineManager.instance.GetCurrentAnimationName(_devil);
                    //如果火焰与恶魔颜色相对应，即正确
                    if ((fairName.Equals("huo-red") && (devilName.Equals("xem-b") || devilName.Equals("xem-e"))) || (fairName.Equals("huo-blue") && (devilName.Equals("xem-a") || devilName.Equals("xem-c") || devilName.Equals("xem-f"))))
                    {
                        
                        _isBoom = true;
                        _count++;
                        JudgeCount();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        RandomDevilAni();
                        mono.StartCoroutine(WaitCoroutine(() => { StopMove(); }, 0.2f));
                        SpineManager.instance.DoAnimation(_fair, "boom", false,
                        () =>
                        {
                            _fair.transform.position = _pos.GetChild(0).position;
                            SpineManager.instance.DoAnimation(_fair, "kong", false);
                            if(_count == 5)
                            {
                                _gameEnd = true;
                                DOTween.KillAll();
                                SpineManager.instance.SetFreeze(_devil, false);
                                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 4,
                                () =>
                                {
                                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                                    SpineManager.instance.DoAnimation(_devil, "xem-h", true);
                                },
                                () =>
                                {
                                    mono.StartCoroutine(WaitCoroutine(() => { playSuccessSpine(); }, 0.5f));
                                }));
                            }
                            else
                            {
                                DevilMove();
                                _canClick = true;
                                _startMove = true;
                                _isBoom = false;
                            }
                        });
                    }
                    //不对应则失败
                    else
                    {
                        _fair.transform.position = _pos.GetChild(0).position;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        StopMove();
                        SpineManager.instance.DoAnimation(_fair, "kong", false);
                        mono.StartCoroutine(WaitCoroutine(
                        () =>
                        {
                            RandomDevilAni();
                            DevilMove();
                            _canClick = true;
                            _startMove = true;
                        }, 0.2f));
                    }
                }
            }
            
            //控制小恶魔前进
            //if (_startMove)
            //{
            //    _devil.transform.position = Vector2.Lerp(_devil.transform.position, new Vector2(_devil.transform.position.x - _moveDistance / 500, _devil.transform.position.y), 0.2f);
            //}

            //当前时间 - 上一次启动时间 <= 剩余时间 则游戏失败
            if((_time.fillAmount == 1 || _devil.transform.localPosition.x == _pos.GetChild(0).localPosition.x) && !_gameEnd)
            {
                _time.fillAmount = 1;
                _clock.transform.localPosition = curTrans.Find("TimeBg/EndPos").localPosition;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                _gameEnd = true;
                StopMove();
                DOTween.KillAll();
                mask.Show();
                anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                anyBtns.gameObject.SetActive(true);
                anyBtns.GetChild(0).gameObject.SetActive(true);
                anyBtns.GetChild(1).gameObject.SetActive(false);
            }

            if(!_isBoom)
            {
                _time.fillAmount += 0.0005714f;
            }
        }

        //小恶魔随机动画
        void RandomDevilAni()
        {
            int random = 5;
            do
            {
                random = Random.Range(0, 5);
                SpineManager.instance.SetFreeze(_devil, false);
            }
            while (_devilAni[random] == SpineManager.instance.GetCurrentAnimationName(_devil));
            SpineManager.instance.DoAnimation(_devil, _devilAni[random], true);
        }
        
        //开始DOTween
        void StartMove()
        {
            _devilTween = _devil.transform.DOLocalMoveX(_pos.GetChild(0).localPosition.x, 35);
            _devilTween.SetEase(Ease.Linear);

            _timeTween = _clock.transform.DOLocalMoveX(curTrans.Find("TimeBg/EndPos").localPosition.x, 35);
            _timeTween.SetEase(Ease.Linear);
        }

        //重启DOTween
        void DevilMove()
        {
            _startMove = true;
            SpineManager.instance.SetFreeze(_devil, false);
            _devil.transform.DOPlay();
            _clock.transform.DOPlay();
        }

        //暂停DOTween
        void StopMove()
        {
            _startMove = false;
            SpineManager.instance.SetFreeze(_devil, true);
            _devil.transform.DOPause();
            _clock.transform.DOPause();
        }

        //分数显示
        void JudgeCount()
        {
            for (int i = 0; i < _devilCount.childCount; i++)
            {
                _devilCount.GetChild(i).gameObject.Hide();
            }
            _devilCount.GetChild(_count).gameObject.Show();
        }
    }
}
