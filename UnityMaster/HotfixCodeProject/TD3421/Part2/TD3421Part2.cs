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
    public class TD3421Part2
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

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;

        private Transform Part1;
        private Transform Part2;
        private bool _canClick;
        private bool _canPlay;
        private Transform animObj1;
        private Transform animObj2;
        private List<string> name1;
        private int index;
        private Transform _return;
        private Transform Mask1;
 
        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        private Transform SpineShow;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;
        private bool _isSlide; //是否滑动中
        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;
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
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");

            bd = curTrans.Find("BD").gameObject;
           // bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //我写的
            Part1 = curTrans.Find("part1");
            Part2 = curTrans.Find("part2");
            _canClick = false;
            _canPlay = false;
            animObj1 = curTrans.Find("part1/spine");
            animObj2 = curTrans.Find("part2/spine");
            name1 = new List<string>();
            index = 0;
            _return = curTrans.Find("return");
            Mask1 = curTrans.Find("Mask1");

            _isSlide = false;




            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            pageBar = curTrans.Find("PageBar").gameObject;
            SlideSwitchPage(pageBar);
            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
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


            SpineShow = curTrans.Find("SpineShow");
          //  SpineShow.gameObject.SetActive(true);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            //GameStart();
        }


        private void OnClickPlay(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(bd,SoundManager.SoundType.VOICE, obj.transform.GetSiblingIndex() + 1, null, () =>
            {
                isPlaying = false;
                if ((flag & (1 << obj.transform.GetSiblingIndex())) == 0)
                {
                    flag += 1 << obj.transform.GetSiblingIndex();
                }
                if (flag == (Mathf.Pow(2, SpineShow.childCount) - 1))
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));
            SpineManager.instance.DoAnimation(SpineShow.gameObject, obj.name, false);
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
            SpineManager.instance.DoAnimation(tem, tem.transform.GetChild(0).name + "2", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name, false, () =>
                {
                    obj.SetActive(false); isPlaying = false; isPressBtn = false; 
                    if (flag == (Mathf.Pow(2, SpinePage.childCount) - 1) && !isEnd)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                });
            });
        }

        private void OnClickShow(GameObject obj)
        {
            if (SpinePage.GetComponent<HorizontalLayoutGroup>().enabled)
            {
                SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = false;
            }
            if (isPlaying || isPressBtn)
                return;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            tem = obj.transform.parent.gameObject;
            tem.transform.SetAsLastSibling();
            SpineManager.instance.DoAnimation(tem, obj.name, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false, () =>
                {
                    isPressBtn = true;
                    btnBack.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, int.Parse(obj.transform.GetChild(0).name)+1, null, () =>
                     {
                        //用于标志是否点击过展示板
                        if ((flag & (1 << int.Parse(obj.transform.GetChild(0).name))) == 0)
                         {
                             flag += 1 << int.Parse(obj.transform.GetChild(0).name);
                         }
                         isPressBtn = false;
                     }));
                });
            });
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.COMMONVOICE, 0)); });
                }

            });
        }

        private void GameInit()
        {
            _canClick = true;
            LRBtnUpdate();
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            Part1.GetRectTransform().DOAnchorPosX(0,0.1f);
            Part2.GetRectTransform().DOAnchorPosX(1920, 0.1f);
            SpineManager.instance.DoAnimation(animObj2.gameObject,"a1",false);
            SpineManager.instance.DoAnimation(animObj1.gameObject, "abcdef", false);
            bd.gameObject.SetActive(true);
            _return.gameObject.SetActive(false);
            Mask1.gameObject.SetActive(false);

            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            for (int i = 0; i < Part1.GetChild(1).childCount; i++)
            {
                //PointerClickListener.Get(Part1.GetChild(i).gameObject).onClick = ClickEventLevel1;
                Util.AddBtnClick(Part1.GetChild(1).GetChild(i).gameObject, ClickEventLevel1);
            }
            for (int i = 0; i < Part2.GetChild(1).childCount; i++)
            {
                //PointerClickListener.Get(Part1.GetChild(i).gameObject).onClick = ClickEventLevel1;
                Util.AddBtnClick(Part2.GetChild(1).GetChild(i).gameObject, ClickEventLevel2);
            }
            //来认识材料吧
            mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND,0,()=> { _canClick = false; },()=> { _canClick = true; }));

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
             {
                 ShowDialogue("", devilText);
             }, () =>
                 {
                     buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                     {
                         mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
                      {
                          ShowDialogue("", bdText);
                      }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                     });
                 }));

            });
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
        /// 
         //点击事件1
        private void ClickEventLevel1(GameObject obj)
        {

            if (_canClick)
            {
                BtnPlaySound();
                //  SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 1, false);
                _canClick = false;
                JudgeClick(obj);
                string name = obj.name;
                SpineManager.instance.DoAnimation(animObj1.gameObject,obj.name, false);
            }
        }
        private void ClickEventLevel2(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                _return.gameObject.Show();
               
                // SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 1, false);
                SpineManager.instance.DoAnimation(animObj2.gameObject, obj.name + "1", false, () =>
                {
                    SpineManager.instance.DoAnimation(animObj2.gameObject,obj.name + "2", false);
                    // _return.Show();
                    JudgeClick2(obj);
                    //  SoundManager.instance.ShowVoiceBtn(false);
                    Util.AddBtnClick(Mask1.gameObject, (g) => {

                        Mask1.gameObject.Hide();

                        SpineManager.instance.DoAnimation(animObj2.gameObject, obj.name + "4", false, () =>
                        {
                            _return.gameObject.Hide();
                            SoundManager.instance.ShowVoiceBtn(true);
                        });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                        //   SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16, false);
                        //  SoundManager.instance.ShowVoiceBtn(true);
                    });

                });
            }
        }
        void JudgeClick2(GameObject obj)
        {
            if (obj.name.Equals("a")) 
            {
              
                mono.StartCoroutine((SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 8, () =>
                  {
                      SoundManager.instance.ShowVoiceBtn(false);
                  },()=> 
                  {
                      _canPlay = true;
                      _canClick = true;
                      Mask1.gameObject.Show();
                     
                      if (_canClick)
                      {
                          // SoundManager.instance.ShowVoiceBtn(true);
                      }
                  })));
            }
            
        }
    void JudgeClick(GameObject obj)
        {
           
            SoundManager.instance.ShowVoiceBtn(false);
            Debug.Log(obj.name);
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }


            if (obj.name.Equals("a"))
            {

                Debug.Log(name1.Contains(obj.name));
                Debug.Log(index);
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 1, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        // SoundManager.instance.ShowVoiceBtn(true); 
                    }
                }));
            }
            if (obj.name.Equals("b"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 10, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        // SoundManager.instance.ShowVoiceBtn(true);

                    }
                }));
            }
            if (obj.name.Equals("c"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 3, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {

                        // SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("d"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 2, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {

                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("e"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 4, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {

                        //SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("f"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject,SoundManager.SoundType.SOUND, 5, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {

                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("g"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 6, null, () =>
                {
                    if (index == 7)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {

                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
        }
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //点击标志位
                //  flag = 0;
                // buDing.SetActive(false);
                // devil.SetActive(false);
                //  bd.SetActive(true);
                //认识了材料之后 我们来看一下虞美人的枝干是什么样子的吧~
                Part1.GetRectTransform().DOAnchorPosX(-1920f, 1f);
                Part2.GetRectTransform().DOAnchorPosX(0, 1f).OnComplete(()=> 
                {
                    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 7, () => { _canClick = false; },
                   () => {
                       mask.SetActive(false);
                       bd.SetActive(false);
                       _canClick = true;

                   }));
                });
               
            }
            if (talkIndex == 2) 
            {
                bd.gameObject.SetActive(true);
              
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 9, () =>
                {
                    _return.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(bd.gameObject, "bd-speak", true);
                }, () =>
                {
                   
                    SpineManager.instance.DoAnimation(bd.gameObject, "bd-daiji", true);
                    _canClick = false;
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
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
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


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            LRBtnUpdate();
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
                float dis = Math.Abs(upData.position.x - _prePressPos.x);
                bool isRight = (_prePressPos.x - upData.position.x) > 0 ? true : false;

                if (dis > 300)
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
        private void LRBtnUpdate()
        {
            if (curPageIndex == 0)
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.name + "4", false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
            else if (curPageIndex == SpinePage.childCount - 1)
            {
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.name + "4", false);
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            }
            else
            {
                SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
                SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            }
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
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

    }
}
