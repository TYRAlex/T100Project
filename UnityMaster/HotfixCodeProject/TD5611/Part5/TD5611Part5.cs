using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD5611Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject bd;
        private GameObject dbd;
        private GameObject xem;
        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject pageBar;
        private Transform SpinePage;
        private Empty4Raycast[] e4rs;
        private GameObject rightBtn;
        private GameObject leftBtn;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private GameObject _complete;
        private GameObject _flash;
        private GameObject _dragItem;
        private GameObject[] _dragItems;
        private mILDrager[] _mILDragers;
        private float _canDrayYMin;
        private float _canDrayYMax;
        bool canDrag;

        private bool _canClick;
        private Vector3 _lastPos;
        private GameObject _lastClickObj;   //上一次点击的小物体

        //双击属性
        private float _lastTime;
        private string _clickObjName;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

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

            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            //SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            leftBtn = curTrans.Find("L").gameObject;
            rightBtn = curTrans.Find("R").gameObject;
            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            _dragItem = curTrans.GetGameObject("DragItem");
            _complete = curTrans.GetGameObject("Complete");
            _flash = curTrans.GetGameObject("Flash");

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            talkIndex = 1;
            _canClick = true;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
            GameInit();
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
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); SwitchBGM(); dbd.SetActive(true); mono.StartCoroutine(DBDSpeckerCoroutine(SoundManager.SoundType.VOICE,2)); });
                }

            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            curPageIndex = 0;
            _lastTime = 0;
            _clickObjName = "0";
            _canDrayYMax = curTrans.Find("Block").position.y + curTrans.Find("Block").GetComponent<RectTransform>().rect.height / 2;
            _canDrayYMin = curTrans.Find("Block").position.y - curTrans.Find("Block").GetComponent<RectTransform>().rect.height / 2;
            _dragItems = new GameObject[15];
            _mILDragers = new mILDrager[15];
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);

            _complete.Hide();
            SpineManager.instance.DoAnimation(_flash, "kong", false, () => { _flash.Hide(); });

            for (int i = 0; i < pageBar.transform.Find("MaskImg/SpinePage").childCount; i++)
            {
                Transform trans = pageBar.transform.Find("MaskImg/SpinePage").GetChild(i);
                for (int j = 0; j < trans.childCount; j++)
                {
                    GameObject o = trans.GetChild(j).GetChild(0).GetChild(0).gameObject;
                    GameObject o_2 = o.transform.GetChild(0).gameObject;
                    o.Show();
                    SpineManager.instance.DoAnimation(o_2, "kong", false, 
                    () => 
                    {
                        SpineManager.instance.DoAnimation(o_2, o_2.name + "7", false);
                    });
                    Util.AddBtnClick(o, ClickSmallObj);
                }
            }

            for (int i = 0; i < _dragItem.transform.childCount; i++)
            {
                GameObject o = _dragItem.transform.GetChild(i).gameObject;
                o.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                _dragItems[i] = o;
                _mILDragers[i] = o.GetComponent<mILDrager>();
                o.Hide();
            }
            SpineManager.instance.DoAnimation(leftBtn, "r", true);
            SpineManager.instance.DoAnimation(rightBtn, "r", true);

            InitDrag();
            StartDrag();
        }

        ////双击翻转方法
        //private void clickEvent(GameObject obj)
        //{
        //    float time = Time.realtimeSinceStartup;
        //    if (obj.name.Equals(_clickObjName))
        //    {
        //        if (time - _lastTime <= 0.2f)
        //        {
        //            GameObject parent = obj.transform.parent.gameObject;
        //            if (parent.GetComponent<RectTransform>().localScale.x == 1)
        //                parent.transform.DOScaleX(-1, 0.3f);
        //            else
        //                parent.transform.DOScaleX(1, 0.3f);

        //            mono.StartCoroutine(WaitCoroutine(
        //            () =>
        //            {
        //                _clickObjName = "0";
        //                _lastTime = Time.realtimeSinceStartup;
        //            }, 0.3f));
        //        }
        //    }
        //    else
        //    {
        //        _clickObjName = obj.name;
        //        _lastTime = Time.realtimeSinceStartup;
        //    }
        //}

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            xem.Show();
            mono.StartCoroutine(XEMSpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null,
            () =>
            {
                xem.Hide();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    bd.Hide();
                    mask.Hide();
                    _canClick = true;
                }));
            }));
        }

        //等待协程
        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        //布丁说话协程
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

        //大布丁说话协程
        IEnumerator DBDSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dbd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dbd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        //恶魔说话协程
        IEnumerator XEMSpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
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

        void SwitchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        //点击工具栏的小物品
        private void ClickSmallObj(GameObject obj)
        {
            if(_canClick)
            {
                isPlaying = true;
                GameObject aniObj = obj.transform.GetChild(0).gameObject;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                _canClick = false;
                SpineManager.instance.DoAnimation(aniObj, aniObj.name, false,
                () =>
                {
                    GameObject o = _dragItem.transform.GetGameObject(obj.name);
                    o.Show();
                    o.transform.position = obj.transform.position;
                    SpineManager.instance.DoAnimation(obj, "kong", false);
                    obj.Hide();
                    _lastClickObj = obj;
                });
            }
        }

        #region 滑动工具栏

        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; _canClick = true; });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
                if (_prePressPos.y <= _canDrayYMax && _prePressPos.y >= _canDrayYMin)
                    canDrag = true;
                else
                    canDrag = false;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100 && canDrag == true)
                {
                    _canClick = false;
                    if (!isRight)
                    {
                        if (curPageIndex <= 0 || isPlaying)
                            return;
                        SetMoveAncPosX(1);
                    }
                    else
                    {
                        if (curPageIndex >= SpinePage.childCount - 1 || isPlaying)
                            return;
                        SetMoveAncPosX(-1);
                    }
                }
            };
        }

        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            _canClick = false;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, "r2", false,
            () =>
            {
                SpineManager.instance.DoAnimation(obj, "r", true);
                SetMoveAncPosX(-1);
                isPressBtn = false;
            });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            _canClick = false;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, "r2", false, 
            () => 
            {
                SpineManager.instance.DoAnimation(obj, "r", true);
                SetMoveAncPosX(1);
                isPressBtn = false; 
            });
        }
        #endregion

        #region 拖拽相关
        void InitDrag()
        {
            for (int i = 0; i < _mILDragers.Length; i++)
            {
                _mILDragers[i].canMove = false;
                _mILDragers[i].DoReset();
                _mILDragers[i].SetDragCallback(null, null, null, null);
            }
        }

        void StartDrag()
        {
            for (int i = 0; i < _mILDragers.Length; i++)
            {
                _mILDragers[i].canMove = true;
                _mILDragers[i].SetDragCallback(BeaginDrag, Draging, EndDragDoReset, clickItem);
            }
        }

        private void clickItem(int dragIndex)
        {
            GameObject obj = _dragItems[dragIndex];
            float time = Time.realtimeSinceStartup;
            if (obj.name.Equals(_clickObjName))
            {
                if (time - _lastTime <= 0.2f)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    if (obj.GetComponent<RectTransform>().localScale.x == 1)
                        obj.transform.DOScaleX(-1, 0.3f);
                    else
                        obj.transform.DOScaleX(1, 0.3f);

                    mono.StartCoroutine(WaitCoroutine(
                    () =>
                    {
                        _lastTime = Time.realtimeSinceStartup;
                        _clickObjName = "0";
                    }, 0.3f));
                }
                else
                    _lastTime = Time.realtimeSinceStartup;
            }
            else
            {
                _clickObjName = obj.name;
                _lastTime = Time.realtimeSinceStartup;
            }
        }

        private void BeaginDrag(Vector3 dragPos, int dragType, int dragIndex)
        {
            _clickObjName = _dragItems[dragIndex].gameObject.name;
            _lastPos = _dragItems[dragIndex].transform.position;
            isPlaying = true;
        }

        private void Draging(Vector3 dragPos, int dragType, int dragIndex)
        {
            
        }

        private void EndDragDoReset(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                if (_dragItems[dragIndex].name.Equals("b") || _dragItems[dragIndex].name.Equals("c"))
                {
                    _mILDragers[dragIndex].canMove = false;
                    _canClick = true;
                    _mILDragers[dragIndex].SetDragCallback(null, null, null, clickItem);
                    GameObject moveEnd = curTrans.GetGameObject("DropEnd/" + _dragItems[dragIndex].name);
                    _dragItems[dragIndex].transform.DOMove(moveEnd.transform.position, 0.3f);
                }
                else
                {
                    _canClick = true;
                    _mILDragers[dragIndex].SetDragCallback(BeaginDrag, Draging, EndDrag, clickItem);
                }
                JudgeSuccess();
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                _dragItems[dragIndex].transform.DOMove(_lastClickObj.transform.position, 0.2f);
                _lastClickObj.Show();
                GameObject objAni = _lastClickObj.transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(objAni, "kong", false, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(objAni, objAni.name + "7", false);
                    _canClick = true;
                    _dragItems[dragIndex].Hide();
                    _dragItems[dragIndex].GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
                });
            }
            isPlaying = false;
        }

        private void EndDrag(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                _canClick = true;
                JudgeSuccess();
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                _dragItems[dragIndex].transform.position = _lastPos;
                _canClick = true;
            }

            isPlaying = false;
        }

        //拖拽后的胜利判定
        void JudgeSuccess()
        {
            bool allClick = true;
            for (int i = 0; i < _dragItems.Length; i++)
            {
                if(_dragItems[i].activeSelf)
                    allClick = true;
                else
                {
                    allClick = false;
                    return;
                }
            }

            if(allClick)
            {
                _complete.Show();
                Util.AddBtnClick(_complete, CompleteClick);
            }
        }

        //点击完成
        private void CompleteClick(GameObject obj)
        {
            _flash.Show();
            SpineManager.instance.DoAnimation(_flash, "xing-c", false, 
            () => 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(_flash, "xing", false, 
                ()=> 
                {
                    SpineManager.instance.DoAnimation(_flash, "kong", false, ()=> { _flash.Hide(); });
                    playSuccessSpine();
                });
            });
        }
        #endregion
    }
}
