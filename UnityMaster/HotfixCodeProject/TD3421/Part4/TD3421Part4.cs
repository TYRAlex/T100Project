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
    public class TD3421Part4
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


        //我写的
        private Transform spine;
        private Transform spine1;
        private Transform spine2;
        private Transform spine3;
        private Transform _return;
        private Transform Mask1;
        private bool _canClick;
        private bool _canPlay;
        private int index1;
        private List<string> name1;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

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
            bd.SetActive(true);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);
            //我写的
            spine = curTrans.Find("spine");
            spine1 = curTrans.Find("spine/spine1");
            spine2 = curTrans.Find("spine/spine2");
            spine3 = curTrans.Find("spine/spine3");
            _return = curTrans.Find("return");
            Mask1 = curTrans.Find("Mask1");


            index1 = 0;
            name1 = new List<string>();
            _canClick = false;
            _canPlay = false;

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
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.1f;
            flag = 0;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            Mask1.gameObject.SetActive(false);
            _return.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(spine1.gameObject,"e1",false);
            SpineManager.instance.DoAnimation(spine2.gameObject, "f1", false);
            SpineManager.instance.DoAnimation(spine3.gameObject, "g1", false);

            Util.AddBtnClick(spine1.GetChild(0).gameObject, ClickEventLevel1);
            Util.AddBtnClick(spine2.GetChild(0).gameObject, ClickEventLevel2);
            Util.AddBtnClick(spine3.GetChild(0).gameObject, ClickEventLevel3);

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);

            //小朋友们~你们做的很棒哦~那我们来看看虞美人的花朵都是什么样子的吧~
            mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 0, () =>
            { _canClick = false; },
            () =>
            {
                _canClick = true;
                bd.gameObject.SetActive(false);
               // SoundManager.instance.ShowVoiceBtn(true);
            }));
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
                _canClick = false;
                bd.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 4, () =>
                {
                    SpineManager.instance.DoAnimation(bd, "bd-speak", true);

                }, () =>
                {
                    SpineManager.instance.DoAnimation(bd, "bd-daiji", true);
                }));
                //点击标志位
                //flag = 0;
                //buDing.SetActive(false);
                //devil.SetActive(false);
                //bd.SetActive(false);
                //_canClick = true;
              //  mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));

            }
            if (talkIndex == 2) 
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
        private void ClickEventLevel1(GameObject obj)
        {

            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                if (obj.name == "e")
                {

                    _return.gameObject.Show();
                    spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(spine1.gameObject, obj.name + "2", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(part2Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "b", false);
                        //Mask.gameObject.Show();
                        JudgeClick1(obj);

                        Util.AddBtnClick(Mask1.gameObject, (g) =>
                        {

                            Mask1.gameObject.Hide();

                            //     SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(spine1.gameObject, obj.name + "4", false, () =>
                            {
                                _return.gameObject.Hide();
                                if (index1 == 3)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                        });

                    });

                }


            }
        }
        private void ClickEventLevel2(GameObject obj)
        {

            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                if (obj.name == "f")
                {

                    _return.gameObject.Show();
                    spine2.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(spine2.gameObject, obj.name + "2", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(part2Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "b", false);
                        //Mask.gameObject.Show();
                        JudgeClick2(obj);

                        Util.AddBtnClick(Mask1.gameObject, (g) =>
                        {

                            Mask1.gameObject.Hide();

                            //     SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(spine2.gameObject, obj.name + "4", false, () =>
                            {
                                _return.gameObject.Hide();
                                if (index1 == 3)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                        });

                    });

                }


            }
        }
        private void ClickEventLevel3(GameObject obj)
        {

            if (_canClick)
            {
                _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                if (obj.name == "g")
                {

                    _return.gameObject.Show();
                    spine3.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(spine3.gameObject, obj.name + "2", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(part2Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "b", false);
                        //Mask.gameObject.Show();
                        JudgeClick3(obj);

                        Util.AddBtnClick(Mask1.gameObject, (g) =>
                        {

                            Mask1.gameObject.Hide();

                            //     SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(spine3.gameObject, obj.name + "4", false, () =>
                            {
                                _return.gameObject.Hide();
                                if (index1 == 3)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                        });

                    });

                }


            }
        }
        void JudgeClick1(GameObject obj)
        {
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index1++;
            }
            if (obj.name.Equals("e"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 1, ()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                }, () =>
                {
                    Mask1.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                }));

                if (_canPlay)
                {
                    //  SoundManager.instance.ShowVoiceBtn(true);
                }

            }
        }
        void JudgeClick2(GameObject obj)
        {
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index1++;
            }
            if (obj.name.Equals("f"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 2, ()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                }, () =>
                {
                    Mask1.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                }));

                if (_canPlay)
                {
                    //  SoundManager.instance.ShowVoiceBtn(true);
                }

            }
        }
        void JudgeClick3(GameObject obj)
        {
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index1++;
            }
            if (obj.name.Equals("g"))
            {
                mono.StartCoroutine(SpeckerCoroutine(bd.gameObject, SoundManager.SoundType.SOUND, 3,()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                }, () =>
                {
                    Mask1.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                }));

                if (_canPlay)
                {
                    //  SoundManager.instance.ShowVoiceBtn(true);
                }

            }
        }
    }
}
