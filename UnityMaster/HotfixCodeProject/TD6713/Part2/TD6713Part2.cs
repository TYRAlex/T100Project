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
    public class TD6713Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        //我写的
        private List<string> name;
        private int index1;
        private Transform part1;
        private Transform part2;
        private Transform part1_spine1;
        private Transform part2_spine1;
        private Transform part2_spine2;
        private Transform part2_spine3;
        private bool _canClick;
        private bool _canPlay;
        private Transform _return;
        private Transform Mask;
        private Transform L2;
        private Transform R2;
        private Transform Movepart;
        private Transform Move;
        private int _curPageIndex;//当前页签索引
        private int _pageMinIndex; //页签最小索引
        private int _pageMaxIndex;//页签最大索引
        private float _moveValue;
        private float _duration;    //切换时长
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

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();


            bd = curTrans.Find("BD").gameObject;
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            Util.AddBtnClick(anyBtn, OnClickAnyBtn);

            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickShow);
            }

            leftBtn = curTrans.Find("L2/L").gameObject;
            rightBtn = curTrans.Find("R2/R").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);




            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            name = new List<string>();
            index1 = 0;
            //
            L2 = curTrans.Find("L2");
            R2 = curTrans.Find("R2");
            _return = curTrans.Find("return");
            Mask = curTrans.Find("Mask");
            part1 = curTrans.Find("Move/Mask/Part/part1");
            part2 = curTrans.Find("Move/Mask/Part/part2");
            part1_spine1 = curTrans.Find("Move/Mask/Part/part1/spine1");
            part2_spine1 = curTrans.Find("Move/Mask/Part/part2/spine1");
            part2_spine2 = curTrans.Find("Move/Mask/Part/part2/spine2");
            part2_spine3 = curTrans.Find("Move/Mask/Part/part2/spine3");
            Movepart = curTrans.Find("Move/Mask/Part");
            Move = curTrans.Find("Move");
            _canClick = false;
            _canPlay = false;

            _curPageIndex = 0;
            _pageMinIndex = 0;
            _pageMaxIndex = Movepart.childCount - 1;
            _moveValue = 1920;
            _duration = 0.5f;

            GameInit();
            GameStart();
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
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () => { SpineManager.instance.DoAnimation(tem, tem.name, false, () => { obj.SetActive(false); isPlaying = false; isPressBtn = false; }); });
        }

        private void OnClickShow(GameObject obj)
        {
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () => { SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () => { btnBack.SetActive(true); }); });
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
                else if(obj.name == "fh")
                {
                    GameInit();
                }
                else
                {

                }
                mask.gameObject.SetActive(false);
                anyBtn.name = "Btn";
            });
        }

        private void GameInit()
        {
            _canClick = true;
            _canPlay = true ;
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed =0.5f;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            L2.gameObject.SetActive(true);
            R2.gameObject.SetActive(true);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            var rect = Movepart.GetRectTransform();
            SlideSwitchPage(Move.gameObject,
                () => { LeftSwitchPage(rect, _moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); }); },
                () => { RightSwitchPage(rect, -_moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); }); });

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            //添加事件1
            for (int i = 0; i < 1; i++)
            {
                Util.AddBtnClick(part1.GetChild(0).GetChild(0).gameObject, ClickEventLevel1);
            }
            for (int i = 0; i < part2.childCount; i++)
            {
                Util.AddBtnClick(part2.GetChild(i).GetChild(0).gameObject, ClickEventLevel2);
            }
            Util.AddBtnClick(L2.GetChild(0).gameObject, OnClickArrows);
            Util.AddBtnClick(R2.GetChild(0).gameObject, OnClickArrows);
        }

        void GameStart()
        {
            index1 = 0;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            _return.gameObject.SetActive(false);
            Mask.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(part1_spine1.gameObject, "d4", true);
            SpineManager.instance.DoAnimation(part2_spine1.gameObject, "e4", true);
            SetPos(Movepart.GetRectTransform(), new Vector2(0, 0));
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, ()=> 
            {
                _return.gameObject.SetActive(true);
                _canClick = false;
                bd.SetActive(true);
            }, () => 
            { //SoundManager.instance.ShowVoiceBtn(true);
                _return.gameObject.SetActive(false);
                _canClick = true;
                bd.SetActive(false);
            }));
            curPageIndex = 0;

           

          
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
                bd.SetActive(false);
                mask.SetActive(false);
                _return.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,1,()=> 
                {
                    _return.gameObject.SetActive(true);
                    bd.SetActive(true);
                    SpineManager.instance.DoAnimation(bd, "bd-speak", true);
                },()=> 
                {
                   // _return.gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(bd, "bd-daiji", true);
                    _canClick = false;
                }));
            }
          
            talkIndex++;
        }
        private void ClickEventLevel1(GameObject obj)
        {
            Debug.Log(obj);
            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                if (obj.name == "d")
                {
                    Debug.Log("1");
                    _return.gameObject.Show();
                    part1_spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part1_spine1.gameObject,obj.name + "1", false, () =>
                    {
                        SpineManager.instance.DoAnimation(part1_spine1.gameObject, obj.name + "2", false);
                        //Mask.gameObject.Show();
                        SoundManager.instance.ShowVoiceBtn(false);
                        JudgeClick1(obj);

                        Util.AddBtnClick(Mask.gameObject, (g) => {

                            Mask.gameObject.Hide();
                            _return.gameObject.Hide();
                           // SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(part1_spine1.gameObject, obj.name + "3", false,()=> 
                            {
                                if (index1 == 2)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                  
                                   
                                }
                               
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                        });

                    });

                } 
            }
        }
        void JudgeClick1(GameObject obj)
        {
            if (name.Contains(obj.name) == false)
            {
                name.Add(obj.name);
                index1++;
            }
            if (obj.name.Equals("d"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    _return.gameObject.SetActive(false);
                    Mask.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                       // SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
        }
        private void ClickEventLevel2(GameObject obj)
        {
            Debug.Log(obj);
            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                if (obj.name == "e")
                {
                    Debug.Log("1");
                    _return.gameObject.Show();
                    part2_spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part2_spine1.gameObject, obj.name + "1", false, () =>
                    {
                         SpineManager.instance.DoAnimation(part2_spine1.gameObject,obj.name + "2", false);
                        SoundManager.instance.ShowVoiceBtn(false);
                        //Mask.gameObject.Show();
                        JudgeClick2(obj);

                        Util.AddBtnClick(Mask.gameObject, (g) => {

                            Mask.gameObject.Hide();
                            _return.gameObject.Hide();
                          //  SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(part2_spine1.gameObject, obj.name + "3", false,()=> 
                            {
                                if (index1==2) 
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                    
                                }
                                 });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                        });

                    });

                }
            }
        }
        void JudgeClick2(GameObject obj)
        {
            if (name.Contains(obj.name) == false)
            {
                name.Add(obj.name);
                index1++;
            }
            if (obj.name.Equals("e"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    _return.gameObject.SetActive(false);
                    Mask.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
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
                {  /* anyBtn.name = getBtnName(BtnEnum.fh);*/
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

        private void OnClickArrows(GameObject go)
        {
            BtnPlaySound();
            var name = go.name;
            if (name == "R")
            {
                SpineManager.instance.DoAnimation(R2.gameObject, name, false);
            }
            if (name == "L")
            {
                SpineManager.instance.DoAnimation(L2.gameObject, name, false);
            }

            var rect = Movepart.GetRectTransform();

            switch (name)
            {
                case "L":
                    LeftSwitchPage(rect, _moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); });
                    break;
                case "R":
                    RightSwitchPage(rect, -_moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); });
                    break;
            }
        }
        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack = null)//用得到
        {
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack?.Invoke(); });
        }
        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)//用不到
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

        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        private void LeftSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex <= _pageMinIndex)
                return;
            _curPageIndex--;
            callBack1?.Invoke();
            SetMoveAncPosX(rect, value, duration, callBack2);
        }

        private void RightSwitchPage(RectTransform rect, float value, float duration, Action callBack1 = null, Action callBack2 = null)
        {
            if (_curPageIndex >= _pageMaxIndex)
                return;
            _curPageIndex++;
            callBack1?.Invoke();
            SetMoveAncPosX(rect, value, duration, callBack2);
        }
        private void SlideSwitchPage(GameObject rayCastTarget, Action leftCallBack, Action rightCallBack)
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
                        leftCallBack?.Invoke();
                    else
                        rightCallBack?.Invoke();
                }


            };

        }//用得到
        private void SetPos(RectTransform rect, Vector2 pos)
        {
            string log = string.Format("rect:{0}---pos:{1}", rect.name, pos);
            Debug.Log(log);
            rect.anchoredPosition = pos;
        }
    }
}
