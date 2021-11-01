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
    public class TD5613Part2
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

        private GameObject _L;
        private GameObject _R;

        private GameObject btnBack;
        //新的声明
        public List<string> name1;
          public List<string> name2;
        private Transform Mask1;
        private Transform Part1;//获取part下的儿子的物体
        private GameObject part1;
        private GameObject animObj1;//part1的动画物体
        private bool _canClick;//是否可以与物体交互
        private bool _canPlay;
        private GameObject animObj2;//part2的动画物体
        private Transform Part2;//获取part下的儿子的物体
        private GameObject part2;
        private GameObject _return;//触发返回事件的遮罩
        private Transform bd2;
        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;
        private int index;
        private int index2;
        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;
        void Start(object o)
        {
            ;
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;

            //重要mask.SetActive(false);

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();


            bd = curTrans.Find("mask/BD").gameObject;
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

            leftBtn = curTrans.Find("L2").gameObject;
            rightBtn = curTrans.Find("R2").gameObject;

            Util.AddBtnClick(leftBtn, OnClickBtnLeft);
            Util.AddBtnClick(rightBtn, OnClickBtnRight);

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            //新的对象赋值
            Mask1 = curTrans.Find("Mask1");
            Part1 = curTrans.Find("part1/Part1/1");//获取Part1的子对象
            Part2 = curTrans.Find("part2/Part2/2");//获取Part2的子对象
            part1 = curTrans.Find("part1").gameObject;
            part2 = curTrans.Find("part2").gameObject;
            _return = curTrans.Find("return").gameObject;
            animObj1 = curTrans.Find("part1/Part1/A-i").gameObject;//获取Part1的动画物体  一堆材料
            animObj2 = curTrans.Find("part2/Part2/paint").gameObject;//获取Part2的动画物体 三幅画
            _canClick = false;
            _canPlay = false;
            _L = curTrans.Find("L2").gameObject;
            _R = curTrans.Find("R2").gameObject;
            bd2 = curTrans.Find("BD2");
            index = 0;
            index2 = 0;
            name1 = new List<string>();
            name2 = new List<string>();
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            List<string> name = new List<string>();
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
           
            for (int i = 0; i < Part1.childCount; i++)
            {
                //PointerClickListener.Get(Part1.GetChild(i).gameObject).onClick = ClickEventLevel1;
               Util.AddBtnClick(Part1.GetChild(i).gameObject, ClickEventLevel1);
            }
            for (int i = 0; i < Part2.childCount; i++)
            {
                Util.AddBtnClick(Part2.GetChild(i).gameObject, ClickEventLevel2);
            }

            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed =0.5f;
            part1.transform.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            part2.transform.GetRectTransform().anchoredPosition= new Vector2(1920, 0);

            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);

            
            
        }

        void GameStart()
        {
            //新赋值
            index = 0;
            index2 = 0;
            _canClick = true;
            _return.Hide();
            Mask1.gameObject.Hide();
            //初始化spine的图形
            SpineManager.instance.DoAnimation(animObj2, "TL_LC5-2_123", false);
            _L.SetActive(false);
            _R.SetActive(false);

          //  mask.SetActive(true);

            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, ()=> 
            {
                bd2.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(bd2.gameObject, "bd-speak", true);
                _canClick = false;
            }, () => 
            {
                //小朋友们，西藏真是太神奇了、我们来认识一下今天绘画雪山的材料，试试点击看看吧
                SpineManager.instance.DoAnimation(bd2.gameObject, "bd-daiji", true);
                bd2.gameObject.SetActive(false);
                _canClick = true;
                // SoundManager.instance.ShowVoiceBtn(true);
                isPlaying = false;
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

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            //if (talkIndex == 1)
            //{
            //    bd.SetActive(false);
            //    mask.SetActive(false);
            //    _canClick = true;
            //    isPlaying = true;
            //    _L.SetActive(false);
            //    _R.SetActive(false);
            //}
            if (talkIndex == 1) 
            {
                // 布丁：“小朋友们认识了材料就快动手吧
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () =>
                {
                    bd2.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(bd2.gameObject, "bd-speak", true);

                 

                    _canClick = false;
                },
                 () =>
                 {
                     part1.transform.DOLocalMoveX(-1920, 1.0f);
                     part2.transform.DOLocalMoveX(0, 1.0f);
                     _L.SetActive(false);
                     _R.SetActive(false);

                     bd2.gameObject.SetActive(false);
                     _canClick = true;
                 }, 0));
            }
            if (talkIndex == 2) 
            {
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 14, () =>
                {
                    bd2.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(bd2.gameObject, "bd-speak", true);
                },()=> 
                {
                    SpineManager.instance.DoAnimation(bd2.gameObject, "bd-daiji", true);
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
        //点击事件1
        private void ClickEventLevel1(GameObject obj)
        {

            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                _canClick = false;
                JudgeClick(obj);
                string name = obj.name;
                SpineManager.instance.DoAnimation(animObj1, "TL_LC5-1_" + obj.name, false);
            }
        }
        //点击事件2
        private void ClickEventLevel2(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;

                Mask1.gameObject.SetActive(true);
                string name = obj.name;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                SpineManager.instance.DoAnimation(animObj2, "TL_LC5-2_" + obj.name + "-1", false, () =>
                {
                    SpineManager.instance.DoAnimation(animObj2, "TL_LC5-2_" + obj.name + "-2", false);
                   // _return.Show();
                    JudgeClick2(obj);
                  //  SoundManager.instance.ShowVoiceBtn(false);
                    Util.AddBtnClick(_return, (g) => {

                        _return.Hide();
                      
                        SpineManager.instance.DoAnimation(animObj2, "TL_LC5-2_" + obj.name + "-3", false,()=> 
                        {
                            if (index2 == 3)
                            {
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                        });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16, false);
                      //  SoundManager.instance.ShowVoiceBtn(true);
                    });

                });
            }
        }
        //判断点击的物体是哪一个1
        void JudgeClick(GameObject obj)
        {
            Debug.Log(obj.name);
            if (name1.Contains(obj.name) == false)
            {
                name1.Add(obj.name);
                index++;
            }
         
           
            if (obj.name.Equals("A"))
            {
              
                Debug.Log(name1.Contains(obj.name));
                Debug.Log(index);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        // SoundManager.instance.ShowVoiceBtn(true); 
                    }
                }));
            }
            if (obj.name.Equals("B"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        // SoundManager.instance.ShowVoiceBtn(true);
                       
                    }
                }));
            }
            if (obj.name.Equals("C"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                       
                        // SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("D"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                       
                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("E"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                       
                        //SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("F"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        
                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("G"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,7, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                       
                        //  SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("H"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,8, null, () =>
                {
                    if (index == 9)
                    { SoundManager.instance.ShowVoiceBtn(true); }
                    _canPlay = true;
                    _canClick = true;
                    if (_canPlay)
                    {
                        
                        // SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("i"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,9, null, () =>
                {
                    if (index == 9)
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
        void JudgeClick2(GameObject obj)
        {
            if (name2.Contains(obj.name) == false)
            {
                name2.Add(obj.name);
                index2++;
            }
            if (obj.name.Equals("1"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, ()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                },
                () =>
                {
                    _canPlay = true;
                    _canClick = true;
                    Mask1.gameObject.SetActive(false);
                    _return.Show();
                   
                    if (_canClick)
                    {
                       // SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("2"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, ()=> 
                {
                  SoundManager.instance.ShowVoiceBtn(false);
                }
                , 
                () =>
                {
                    _canPlay = true;
                    _canClick = true;
                    Mask1.gameObject.SetActive(false);
                    _return.Show();
                  
                    if (_canClick)
                    {
                        //SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }
            if (obj.name.Equals("3"))
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE,13, ()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(false);
                }, () =>
                {
                    _canPlay = true;
                    _canClick = true;
                    Mask1.gameObject.SetActive(false);
                    _return.Show();
                   
                    if (_canClick)
                    {
                        //SoundManager.instance.ShowVoiceBtn(true);
                    }
                }));
            }

        }
    }
}
