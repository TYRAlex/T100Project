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
    public class TD3412Part2
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

        private GameObject _pos1;
        private GameObject _pos2;
        private GameObject _pos3;

        private Transform _level1;
        private bool _canClick;
        private bool _isPlayed1;
        private bool _isPlayed2;

        private Transform _level2;
        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        bool isPlaying = false;
        bool isPressBtn = false;

        private int _curIndex;
        private string[] _allClick;
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

            _pos1 = curTrans.GetGameObject("Pos1");
            _pos2 = curTrans.GetGameObject("Pos2");
            _pos3 = curTrans.GetGameObject("Pos3");
            _level1 = curTrans.Find("Level1");
            _level2 = curTrans.Find("Level2");
            Util.AddBtnClick(_level1.GetGameObject("a"), ClickEventLevel1);
            Util.AddBtnClick(_level1.GetGameObject("b"), ClickEventLevel1);

            pageBar = _level2.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = _level2.Find("PageBar/MaskImg/SpinePage");
            //SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = _level2.Find("L2/L").gameObject;
            rightBtn = _level2.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = _level2.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            _isPlayed1 = false;
            _isPlayed2 = false;
            _canClick = false;

            _allClick = new string[6] { "a", "b", "c", "d", "e", "f" };
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameInit();
            GameStart();
        }

        /// <summary>
        /// 定义按钮mode
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

        void GameInit()
        {
            for (int i = 0; i < _level1.childCount; i++)
            {
                GameObject obj = _level1.GetChild(i).GetGameObject(_level1.GetChild(i).name);
                SpineManager.instance.DoAnimation(obj, obj.name + "2", false);
            }

            curPageIndex = 0;
            isPressBtn = false;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);

            _level1.GetComponent<RectTransform>().anchoredPosition = _pos2.GetComponent<RectTransform>().anchoredPosition;
            _level2.GetComponent<RectTransform>().anchoredPosition = _pos3.GetComponent<RectTransform>().anchoredPosition;
        }


        private void OnClickBtnRight(GameObject obj)
        {
            if (curPageIndex >= SpinePage.childCount - 1 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(-1); isPressBtn = false; });
        }

        private void OnClickBtnLeft(GameObject obj)
        {
            if (curPageIndex <= 0 || isPlaying || isPressBtn)
                return;
            isPressBtn = true;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false, () => { SetMoveAncPosX(1); isPressBtn = false; });
        }

        private GameObject tem;
        private void OnClickBtnBack(GameObject obj)
        {
            if (isPressBtn)
                return;
            isPressBtn = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, 
            () => 
            {  
                SpineManager.instance.DoAnimation(tem, tem.name + "3", false, 
                () =>
                {
                    JudgeAllClick();
                    obj.SetActive(false); 
                    tem.transform.SetSiblingIndex(_curIndex); 
                    isPlaying = false; 
                    isPressBtn = false; 
                }); 
            });
        }

        private void OnClickShow(GameObject obj)
        {
            //if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            //{
            //    SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            //}
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            for (int i = 0; i < _allClick.Length; i++)
            {
                if(_allClick[i] == obj.name)
                {
                    _allClick[i] = "all";
                }
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            SoundManager.instance.ShowVoiceBtn(false);
            tem = obj.transform.parent.gameObject;
            _curIndex = tem.transform.GetSiblingIndex();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, curPageIndex == 0 ? _curIndex + 4 : _curIndex + 7,
            () =>
            {
                tem.transform.SetAsLastSibling();
                SpineManager.instance.DoAnimation(tem, obj.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "4", false);
                });
            }, 
            () =>
            {
                btnBack.SetActive(true);
            }));
            
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //小朋友们，梯子都有很多的材质，今天我们绘画的是木制梯子，绘画木质梯子需要的材料有什么呢？让我们看一下吧
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => 
            {
                mask.Hide();
                _canClick = true;
            }));
        }

        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        }

        private void SlideSwitchPage(GameObject rayCastTarget)
        {
            UIEventListener.Get(rayCastTarget).onDown = downData =>
            {
                _prePressPos = downData.pressPosition;
            };

            UIEventListener.Get(rayCastTarget).onUp = upData =>
            {
                float dis = (upData.position - _prePressPos).magnitude;
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 100)
                {
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

        //等待协程
        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);

            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        //DD说话协程
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
                //除了木梯，还有其他的梯子，也来认识一下吧
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
               () =>
               {
                   _canClick = false;
               },
               () =>
               {
                   _level1.transform.DOLocalMoveX(_pos1.transform.localPosition.x, 1.0f);
                   _level2.transform.DOLocalMoveX(_pos2.transform.localPosition.x, 1.0f);
                   bd.SetActive(false);
               }, 0));
            }
            if (talkIndex == 2)
            {
                isPlaying = true;
                isPressBtn = true;
                //接下来让我们拿起蜡笔，在牛皮纸梯子上绘画出漂亮的蜗牛线吧。
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10,
               () =>
               {
                   bd.SetActive(true);
               }, null, 0));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }

        void JudgeAllClick()
        {
            bool isAllClick = true;
            for (int i = 0; i < _allClick.Length; i++)
            {
                if(_allClick[i] != "all")
                {
                    isAllClick = false;
                }
            }

            if(isAllClick)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
        #region 第一关
        //点击事件
        private void ClickEventLevel1(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                JudgeClick(obj);
                GameObject o = obj.transform.GetChild(0).gameObject;
                string name = o.name;
                SpineManager.instance.DoAnimation(o, name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(o, name + "2", false);
                });
            }
        }

        //判断点击的是第几个
        void JudgeClick(GameObject obj)
        {
            if (obj.name.Equals("a"))
            {
                //还需要彩色的蜡笔
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null,
                () =>
                {
                    _isPlayed2 = true;
                    _canClick = true;
                    if (_isPlayed1 && _isPlayed2)
                        SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (obj.name.Equals("b"))
            {
                //一个牛皮纸的梯子
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null,
                () =>
                {
                    _isPlayed1 = true;
                    _canClick = true;
                    if (_isPlayed1 && _isPlayed2)
                        SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
        }
        #endregion

    }
}
