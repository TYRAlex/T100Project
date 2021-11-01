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
    public class TD6711Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject dd;
        private GameObject ddd;
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
        private int _level;

        private GameObject _a;
        private GameObject _b;
        private GameObject _aPos;
        private GameObject _bPos;
        private GameObject _endPos;
        private float _moveDistance;
        private bool _canClick;
        private float _limitTime;
        private bool _isWin;

        private GameObject _b2;
        private GameObject _b2Rect;
        private GameObject _diamond;
        private MonoScripts _monoScipts;
        private Vector2 _b2NewPos;
        private float _b2Width;
        private float _b2Height;
        private float _speed;
        private float _newTime;
        private bool _canMove;
        private int _getCount;
        Tweener tw2;
        Tweener tw3;
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

            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(false);
            ddd = curTrans.Find("mask/DDD").gameObject;
            ddd.SetActive(false);
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
            //SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
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

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);


            _a = curTrans.GetGameObject("a");
            _b = curTrans.GetGameObject("b");
            _b.Show();
            _aPos = curTrans.GetGameObject("aPos");
            _bPos = curTrans.GetGameObject("bPos");
            _endPos = curTrans.GetGameObject("EndPos");

            _b2 = curTrans.GetGameObject("b2");
            _b2.Hide();
            _b2Rect = curTrans.GetGameObject("b2Rect");
            _diamond = curTrans.GetGameObject("Diamond");
            _diamond.Hide();
            _monoScipts = curTrans.GetGameObject("MonoScripts").GetComponent<MonoScripts>();
            _monoScipts.UpdateCallBack = SUpdate;

            _b2Width = _b2.GetComponent<RectTransform>().rect.width / 2;
            _b2Height = _b2.GetComponent<RectTransform>().rect.height / 2;
            _canMove = false;
            GameInit();
        }

        private void SUpdate()
        {
            //第一关

            if (_level == 1 && _isWin == false)
            {
                //小河马赢
                if (_a.transform.position.x < _endPos.transform.position.x && _b.transform.position.x >= _endPos.transform.position.x)
                {
                    _isWin = true;
                    _canClick = false;
                    SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b3", true);
                    SpineManager.instance.DoAnimation(_a, "a1", true);
                    DOTween.KillAll();
                    mono.StopAllCoroutines();

                    mask.Show();
                    dd.Hide();
                    caidaiSpine.Show();
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.next, 1);
                    BtnPlaySoundSuccess();
                    SpineManager.instance.DoAnimation(caidaiSpine, "kong", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                    });
                }
                //小恶魔赢
                if (_a.transform.position.x >= _endPos.transform.position.x && _b.transform.position.x < _endPos.transform.position.x)
                {
                    _isWin = true;
                    _canClick = false;
                    SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b1", true);
                    SpineManager.instance.DoAnimation(_a, "a2", true);
                    mask.Show();
                    dd.Hide();
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(false);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    BtnPlaySoundFail();
                }
            }

            //第二关

            //检测获取宝石
            if (_level == 2)
            {
                _b2NewPos = _b2.transform.position;
                for (int i = 0; i < _diamond.transform.childCount; i++)
                {
                    if (_diamond.transform.GetChild(i).gameObject.activeSelf)
                    {
                        if (_diamond.transform.GetChild(i).transform.position.x <= (_b2NewPos.x + _b2Width) && _diamond.transform.GetChild(i).transform.position.x >= (_b2NewPos.x - _b2Width))
                        {
                            if (_diamond.transform.GetChild(i).transform.position.y <= (_b2NewPos.y + _b2Height) && _diamond.transform.GetChild(i).transform.position.y >= (_b2NewPos.y - _b2Height))
                            {
                                _diamond.transform.GetChild(i).gameObject.Hide();
                                _getCount++;
                                NewTween(NewTime());
                                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                                SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b2", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b1", true);
                                });
                            }
                        }
                    }
                }
            }

            //检测游戏结束
            if (_level == 2 && _isWin == false)
            {
                //小恶魔赢
                if (_a.transform.position.x >= _endPos.transform.position.x && _b2.transform.position.x < _endPos.transform.position.x)
                {
                    _isWin = true;
                    DOTween.KillAll();
                    SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b1", true);
                    SpineManager.instance.DoAnimation(_a, "a2", true);
                    mask.Show();
                    dd.Hide();
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(false);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    BtnPlaySoundFail();
                }
                //小河马赢
                if (_a.transform.position.x < _endPos.transform.position.x && _b2.transform.position.x >= _endPos.transform.position.x)
                {
                    _isWin = true;
                    DOTween.KillAll();
                    SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b3", true);
                    SpineManager.instance.DoAnimation(_a, "a1", true);
                    playSuccessSpine();
                }
            }
        }

        //private void OnClickBtnRight(GameObject obj)
        //{
        //    if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
        //        return;
        //    isPressBtn = true;
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
                        dd.Show();
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    Debug.LogError(_level);
                    SpineManager.instance.DoAnimation(obj, "kong", false,
                    () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        if (_level == 1)
                            StartLevel1();
                        if (_level == 2)
                            StartLevel2();
                    });
                }
                else if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(caidaiSpine, "kong", false);
                    SpineManager.instance.DoAnimation(obj, "kong", false, 
                    () => 
                    { 
                        anyBtns.gameObject.SetActive(false); 
                        dd.SetActive(true); 
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, 
                        ()=> 
                        {
                            dd.SetActive(false);
                            mask.SetActive(false);
                            StartLevel2();
                        })); 
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); ddd.SetActive(true); mono.StartCoroutine(DDDSpeckerCoroutine(SoundManager.SoundType.VOICE, 3)); });
                }
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            _level = 1;
            DOTween.KillAll();
            _a.transform.localPosition = _aPos.transform.localPosition;
            _b.transform.localPosition = _bPos.transform.localPosition;
            _b2.transform.localPosition = _bPos.transform.localPosition;
            _moveDistance = (float)((Mathf.Abs(_b.transform.localPosition.x - _endPos.transform.localPosition.x)) / 40);
            Util.AddBtnClick(_b, ClickEvent);

            SpineManager.instance.DoAnimation(caidaiSpine, "kong", false);
            SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b1", true);
            SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b1", true);
            SpineManager.instance.DoAnimation(_a, "a1", true);
        }

        private void StartLevel1()
        {
            _canClick = true;
            _isWin = false;
            _a.transform.localPosition = _aPos.transform.localPosition;
            _b.transform.localPosition = _bPos.transform.localPosition;
            SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b1", true);
            SpineManager.instance.DoAnimation(_a, "a1", true);

            _limitTime = 10.0f;
            Tweener tw = _a.transform.DOLocalMoveX(_endPos.transform.localPosition.x, _limitTime);
            tw.SetEase(Ease.Linear);
            //curPageIndex = 0;
            //isPressBtn = false;
            //textSpeed =0.5f;
            //SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            //for (int i = 0; i < SpinePage.childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            //}

            //SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            //SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }

        private void StartLevel2()
        {
            talkIndex = 1;
            _level = 2;
            _getCount = 0;
            _speed = 75;
            _isWin = false;
            _b.Hide();
            _b2.Show();
            _diamond.Show();
            for (int i = 0; i < _diamond.transform.childCount; i++)
            {
                _diamond.transform.GetChild(i).gameObject.Show();
            }
            _a.transform.localPosition = _aPos.transform.localPosition;
            _b2.transform.localPosition = _bPos.transform.localPosition;
            _b2Rect.transform.localPosition = new Vector3(_bPos.transform.localPosition.x, _b2Rect.transform.localPosition.y);
            SpineManager.instance.DoAnimation(_b2.transform.GetGameObject("b"), "b1", true);
            SpineManager.instance.DoAnimation(_a, "a1", true);
            _canMove = true;

            _newTime = NewTime();
            Tweener tw1 = _a.transform.DOLocalMoveX(_endPos.transform.localPosition.x, _newTime - 5);
            tw1.SetEase(Ease.Linear);
            tw2 = _b2.transform.DOLocalMoveX(_endPos.transform.localPosition.x, _newTime);
            tw2.SetEase(Ease.Linear);
            tw3 = _b2Rect.transform.DOLocalMoveX(_endPos.transform.localPosition.x, _newTime);
            tw3.SetEase(Ease.Linear);
        }

        void GameStart()
        {
            //buDing.transform.DOMove(bdEndPos.position,1f).OnComplete(()=> {/*正义的一方对话结束 devil开始动画*/

            //    devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() => {/*对话*/ });
            //});


            //isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); /*isPlaying = false;*/ }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dd, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dd, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dd, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //大丁丁说话
        IEnumerator DDDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(ddd, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(ddd, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(ddd, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //等待协程
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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, 
                () => 
                {
                    dd.SetActive(false);
                    mask.SetActive(false);
                    StartLevel1();
                }));
            }
            //if (talkIndex == 2)
            //{
            //    _a.transform.localPosition = _aPos.transform.localPosition;
            //    InitLevel2();
            //    _b.Hide();
            //    _b2.Show();
            //    InitDrag();
            //    _limitTime = 15.0f;
            //    _a.transform.DOLocalMoveX(_endPos.transform.localPosition.x, _limitTime);
            //    _startTime = Time.realtimeSinceStartup;
            //    mono.StartCoroutine(WaitCoroutine(() => { JudgeEnd(); }, _limitTime));
            //}
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
                    if (_level == 1)
                        anyBtns.GetChild(1).name = getBtnName(BtnEnum.next, 1);
                    else
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(1, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            int ranfom = Random.Range(4, 10);
            if (ranfom == 8 || ranfom == 4)
                ranfom = 6;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, ranfom, false);
        }

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }
        //private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        //{
        //    if (isPlaying)
        //        return;
        //    isPlaying = true;
        //    curPageIndex -= LorR;
        //    SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        //}

        //private void SlideSwitchPage(GameObject rayCastTarget)
        //{
        //    UIEventListener.Get(rayCastTarget).onDown = downData =>
        //    {
        //        _prePressPos = downData.pressPosition;
        //    };

        //    UIEventListener.Get(rayCastTarget).onUp = upData =>
        //    {
        //        float dis = (upData.position - _prePressPos).magnitude;
        //        bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

        //        if (dis > 100)
        //        {
        //            if (!isRight)
        //            {
        //                if (curPageIndex <= 0 || isPlaying)
        //                    return;
        //                SetMoveAncPosX(1);
        //            }
        //            else
        //            {
        //                if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
        //                    return;
        //                SetMoveAncPosX(-1);
        //            }
        //        }
        //    };
        //}

        //void ShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        //}

        //IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        //{
        //    int i = 0;
        //    str = str.Replace(" ", "\u00A0");  //空格非换行
        //    while (i <= str.Length - 1)
        //    {
        //        yield return new WaitForSeconds(textSpeed);
        //        text.text += str[i];
        //        i++;
        //    }
        //    callBack?.Invoke();
        //    yield break;
        //}

        #region 关卡一
        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                obj.transform.localPosition = new Vector3(obj.transform.localPosition.x + _moveDistance, obj.transform.localPosition.y, obj.transform.localPosition.z);
                SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b2", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(_b.transform.GetGameObject("b"), "b1", true);
                });
            }
        }
        #endregion

        #region 关卡二

        //更新速度
        float NewTime()
        {
            float s = _endPos.transform.localPosition.x - _b2.transform.localPosition.x;
            return _newTime = ((s / _speed) * (5 - _getCount)) / 5;
        }

        void NewTween(float time)
        {
            tw2 = _b2.transform.DOLocalMoveX(_endPos.transform.localPosition.x, time);
            tw2.SetEase(Ease.Linear);
            tw3 = _b2Rect.transform.DOLocalMoveX(_endPos.transform.localPosition.x, time);
            tw3.SetEase(Ease.Linear);
        }

        #endregion
    }
}