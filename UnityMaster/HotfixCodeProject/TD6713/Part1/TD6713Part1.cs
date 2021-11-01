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
    public class TD6713Part1
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


        //开始声明
        private List<string> name1;
        private List<string> name2;
        private int index;
        private int index2;
        private Transform Movepart;
        private Transform part1;
        private bool _canClick;
        private Transform animObj1;
        private bool _canPlay;
        private Transform L2;
        private Transform R2;
        private Transform Mask;//用于遮罩 以及退回的动画
        private int _curPageIndex;//当前页签索引
        private int _pageMinIndex; //页签最小索引
        private int _pageMaxIndex;//页签最大索引
        private float _moveValue;
        private float _duration;    //切换时长
        private Transform _return;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private Transform part2Spine1;
        private Transform part2Spine2;
        private Transform part2Spine3;
        private Transform part3Spine1;
        private Transform part3Spine2;
        private Transform part4Spine1;
        private Transform part4Spine2;
        private Transform part5Spine1;
        private Transform Move;
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

            //start
            name1 = new List<string>();
            name2 = new List<string>();
            index = 0;
            index2 = 0;
            _canClick = false;
            _canPlay = false;
            animObj1 = curTrans.Find("part1/spine");
            Movepart = curTrans.Find("Move/Mask/Part");
            Move = curTrans.Find("Move");
            part1 = curTrans.Find("part1");
            L2 = curTrans.Find("L2");
            R2 = curTrans.Find("R2");
            Mask = curTrans.Find("Mask");
            _return = curTrans.Find("return");
            part2Spine1 = curTrans.Find("Move/Mask/Part/part2/spine1");
            part2Spine2 = curTrans.Find("Move/Mask/Part/part2/spine2");
            part2Spine3 = curTrans.Find("Move/Mask/Part/part2/spine3");
            part3Spine1 = curTrans.Find("Move/Mask/Part/part3/spine1");
            part3Spine2 = curTrans.Find("Move/Mask/Part/part3/spine2");
            part4Spine1 = curTrans.Find("Move/Mask/Part/part4/spine1");
            part4Spine2 = curTrans.Find("Move/Mask/Part/part4/spine2");
            part5Spine1 = curTrans.Find("Move/Mask/Part/part5/spine1");

            _curPageIndex = 0;
            _pageMinIndex = 0;
            _pageMaxIndex =Movepart.childCount - 1;
            _moveValue = 1920;
            _duration = 0.5f;

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

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
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed =0.5f;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);

            var rect = Movepart.GetRectTransform();
            SlideSwitchPage(Move.gameObject,
                () => { LeftSwitchPage(rect, _moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); }); },
                () => { RightSwitchPage(rect, -_moveValue, _duration, () => { _return.gameObject.Show(); }, () => { _return.gameObject.Hide(); }); });

            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
            //添加点击事件1
            for (int i = 0; i < part1.GetChild(0).childCount; i++) 
            {
                Util.AddBtnClick(part1.GetChild(0).GetChild(i).gameObject, ClickEventLevel1);
            }
            //添加点击事件2
            for (int i = 0; i < Movepart.GetChild(0).childCount; i++) 
            {
                Util.AddBtnClick(Movepart.GetChild(0).GetChild(i).GetChild(0).gameObject, ClickEventLevel2);
            }
            //添加点击事件3
            for (int i = 0; i < Movepart.GetChild(1).childCount; i++) 
            {
                Util.AddBtnClick(Movepart.GetChild(1).GetChild(i).GetChild(0).gameObject, ClickEventLevel3);
            }
            //添加点击事件4
            for (int i = 0; i < Movepart.GetChild(2).childCount; i++)
            {
                Util.AddBtnClick(Movepart.GetChild(2).GetChild(i).GetChild(0).gameObject, ClickEventLevel4);
            }
            //添加点击事件5
            //for (int i = 0; i < Movepart.GetChild(3).childCount; i++)
            //{
            //    Util.AddBtnClick(Movepart.GetChild(3).GetChild(i).GetChild(0).gameObject, ClickEventLevel5);
            //}

            Util.AddBtnClick(L2.GetChild(0).gameObject,OnClickArrows);
            Util.AddBtnClick(R2.GetChild(0).gameObject, OnClickArrows);

        }

        void GameStart()
        {
            isPlaying = true;
            isPressBtn = true;
            _canClick = false;
            _canPlay=true;
            Mask.gameObject.SetActive(false);

            part1.DOLocalMove(new Vector3(0, 0, 0), 1.0f);
            Move.DOLocalMove(new Vector3(1920f, 0, 0), 1.0f);
            L2.gameObject.SetActive(false);
          //  SpineManager.instance.DoAnimation(L2.gameObject, "L2", true);
            R2.gameObject.SetActive(false);
            // SpineManager.instance.DoAnimation(R2.gameObject, "R2", true);
            index = 0;
            index2 = 0;
            curPageIndex = 0;
            SetPos(Movepart.GetRectTransform(), new Vector2(0, 0));
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(part2Spine1.gameObject, "a4", true);
            SpineManager.instance.DoAnimation(part3Spine1.gameObject, "b4", true);
            SpineManager.instance.DoAnimation(part4Spine1.gameObject, "c4", true);
            _return.gameObject.SetActive(false);

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, ()=> 
            {
                Mask.gameObject.SetActive(false);
            }, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
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
        //点击事件1
        private void ClickEventLevel1(GameObject obj)
        {
            Debug.Log("123");
            Debug.Log(obj);
            if (_canClick)
            {
                _canClick = false;
                JudgeClick(obj);
                string name = obj.name;
                SpineManager.instance.DoAnimation(animObj1.gameObject, "YLFG_LC4-1_" + obj.name, false);
            }
        }
        //判断点击的物体是哪一个1
        void JudgeClick(GameObject obj)
        {
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }
            if (obj.name.Equals("01"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("02"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("03"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                     //   SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("04"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("05"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("06"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("07"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                     //   SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("08"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, null, () =>
                {
                    if (index == 7)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        //SoundManager.instance.ShowVoiceBtn(true);
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
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14, false);
                if (obj.name == "a")
                {
                    Debug.Log("1");
                    _return.gameObject.Show();
                    part2Spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part2Spine1.gameObject, obj.name + "1", false, () =>
                    {
                       SpineManager.instance.DoAnimation(part2Spine1.gameObject,obj.name + "2", false);
                       SoundManager.instance.ShowVoiceBtn(false);
                        JudgeClick2(obj);
                       
                            Util.AddBtnClick(Mask.gameObject, (g) => {

                                Mask.gameObject.Hide();
                                _return.gameObject.Hide();
                             //   SoundManager.instance.ShowVoiceBtn(true);
                                SpineManager.instance.DoAnimation(part2Spine1.gameObject, obj.name + "3", false,()=> 
                                {
                                    if (index2 == 3) 
                                    {
                                        SoundManager.instance.ShowVoiceBtn(true);
                                    }
                                });
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                            });
                        
                       

                    });

                } 
            }
        }
        void JudgeClick2(GameObject obj)
        {
            if (name2.Contains(obj.name) == false)
            {
                name2.Add(obj.name);
                index2++;
            }
            if (obj.name.Equals("a"))
            {
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null, () =>
                {
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
        private void ClickEventLevel3(GameObject obj)
        {
          
            Debug.Log(obj);
            if (_canClick)
            {
                  _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14, false);
                if (obj.name == "b")
                {
                    
                    _return.gameObject.Show();
                    part3Spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part3Spine1.gameObject, obj.name + "1", false, () =>
                    {
                        SpineManager.instance.DoAnimation(part3Spine1.gameObject, obj.name + "2", false);
                        //Mask.gameObject.Show();
                        SoundManager.instance.ShowVoiceBtn(false);
                        JudgeClick3(obj);

                        Util.AddBtnClick(Mask.gameObject, (g) => {

                            Mask.gameObject.Hide();
                            _return.gameObject.Hide();
                          //  SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(part3Spine1.gameObject, obj.name + "3", false,()=> 
                            {
                                if (index2 == 3)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                        });

                    });

                }

            }
        }
        void JudgeClick3(GameObject obj)
        {
            if (name2.Contains(obj.name) == false)
            {
                name2.Add(obj.name);
                index2++;
            }
               Debug.Log(index2);
            if (obj.name.Equals("b"))
            {
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, null, () =>
                {
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
        private void ClickEventLevel4(GameObject obj)
        {
           
            Debug.Log(obj);
            if (_canClick)
            {
                //  _canClick = false;
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14, false);
                if (obj.name == "c")
                {
                   
                    _return.gameObject.Show();
                    part4Spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part4Spine1.gameObject, obj.name + "1", false, () =>
                    {
                        SpineManager.instance.DoAnimation(part4Spine1.gameObject,obj.name + "2", false);
                        //Mask.gameObject.Show();
                        SoundManager.instance.ShowVoiceBtn(false);
                        JudgeClick4(obj);

                        Util.AddBtnClick(Mask.gameObject, (g) => {

                            Mask.gameObject.Hide();
                            _return.gameObject.Hide();
                         //   SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(part4Spine1.gameObject,obj.name + "3", false,()=> 
                            {
                                if (index2 == 3)
                                {
                                    SoundManager.instance.ShowVoiceBtn(true);
                                }
                            });
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                        });

                    });

                }          
            }
        }
        void JudgeClick4(GameObject obj)
        {
            if (name2.Contains(obj.name) == false)
            {
                name2.Add(obj.name);
                index2++;
            }
            Debug.Log(index2);
            if (obj.name.Equals("c"))
            {
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, null, () =>
                {
                    Mask.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                      
                    }
                }));
            }
           

        }
        private void ClickEventLevel5(GameObject obj)
        {
            Debug.Log(obj);
            if (_canClick)
            {
                //  _canClick = false;
                string name = obj.name;
                if (obj.name == "8")
                {
                    Debug.Log("1");
                    _return.gameObject.Show();
                    part5Spine1.SetAsLastSibling();
                    SpineManager.instance.DoAnimation(part5Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "a", false, () =>
                    {
                        // SpineManager.instance.DoAnimation(part2Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "b", false);
                        //Mask.gameObject.Show();
                         JudgeClick5(obj);

                        Util.AddBtnClick(Mask.gameObject, (g) => {

                            Mask.gameObject.Hide();
                            _return.gameObject.Hide();
                            SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(part5Spine1.gameObject, "YLFG_LC4-2_" + obj.name + "c", false);
                        });

                    });

                } 
            }
        }
        void JudgeClick5(GameObject obj)
        {
            if (obj.name.Equals("8"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    Mask.gameObject.Show();
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    //小朋友们接下来我们来认识一下绘画沿途风景所需要的材料吧~接下来我们需要用到什么材料呢？-
                    bd.SetActive(false);
                    //  SoundManager.instance.ShowVoiceBtn(true); //待处理
                    //isPlaying = false;
                    Mask.gameObject.SetActive(false);
                    _canClick = true;
                }));



                //bd.SetActive(false);
                //mask.SetActive(false);
                //part1.DOLocalMove(new Vector3(-1920,0,0),1.0f);
                //Move.DOLocalMove(new Vector3(0,0,0),1.0f);
                //L2.gameObject.SetActive(true);
                //SpineManager.instance.DoAnimation(L2.gameObject, "L2", true);
                //R2.gameObject.SetActive(true);
                //SpineManager.instance.DoAnimation(R2.gameObject, "R2", true);
                //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, ()=> 
                //{
                //    bd.gameObject.SetActive(true);
                //    SpineManager.instance.DoAnimation(bd.gameObject, "bd-speak", true);
                //    _canClick = false;
                //}, ()=>
                //{
                //    bd.gameObject.SetActive(false);
                //    _canClick = true;

                //}));
            }
            if (talkIndex == 2)
            {
                
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () =>
                {
                    bd.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(bd.gameObject, "bd-speak", true);
                    _canClick = false;
                },
                 () =>
                 {
                     bd.gameObject.SetActive(false);
                     _canClick = true;
                     part1.DOLocalMove(new Vector3(-1920,0,0),1.0f);
                     Move.DOLocalMove(new Vector3(0,0,0),1.0f);
                     
                     L2.gameObject.SetActive(true);
                     SpineManager.instance.DoAnimation(L2.gameObject, "L2", true);
                     R2.gameObject.SetActive(true);
                     SpineManager.instance.DoAnimation(R2.gameObject, "R2", true);
                 }, 0));
            }
            if (talkIndex == 3) 
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, () =>
                  {
                      bd.gameObject.SetActive(true);
                      SpineManager.instance.DoAnimation(bd.gameObject, "bd-speak", true);
                      _canClick = false;
                      _return.gameObject.SetActive(true);
                  },()=> 
                  {
                      SpineManager.instance.DoAnimation(bd.gameObject, "bd-daiji", true);
                      
                  }));
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        /// 
        //左右按钮点击事件
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


        private void SetMoveAncPosX(int LorR, float duration = 1f, Action callBack = null)//用不到
        {
            if (isPlaying)
                return;
            isPlaying = true;
            curPageIndex -= LorR;
            SpinePage.GetRectTransform().DOAnchorPosX(SpinePage.GetRectTransform().anchoredPosition.x + LorR * 1920, duration).OnComplete(() => { callBack?.Invoke(); isPlaying = false; });
        }
        private void SetMoveAncPosX(RectTransform rect, float value, float duration, Action callBack = null)//用得到
        {
            value = rect.anchoredPosition.x + value;
            rect.DOAnchorPosX(value, duration).OnComplete(() => { callBack?.Invoke(); });
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
        }//用不到
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
        private void SetPos(RectTransform rect, Vector2 pos)
        {
            string log = string.Format("rect:{0}---pos:{1}", rect.name, pos);
            Debug.Log(log);
            rect.anchoredPosition = pos;
        }

    }
}
