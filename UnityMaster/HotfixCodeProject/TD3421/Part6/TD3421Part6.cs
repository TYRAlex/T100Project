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
    public class TD3421Part6
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform xdd;
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
        private Transform flowerSpine;
        private Transform FlowerClickEvent;
        private int indexChangeColor;
        private bool _canClick;
        private Transform xem;
        private Transform dd;
        private Transform gamexem;
        private Transform wangdou;
        private Transform drag;
        private Transform drop;
        private Transform drop2;
        private Transform drop3;
        private Transform drop4;
        private Transform drop5;
        private Transform xiaoguo;
        //拖拽数组
        private List<mILDrager> _iLDragers;
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
            mask.SetActive(true);
            xdd = curTrans.Find("mask/Xdd");
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

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(true);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(1).gameObject.SetActive(false);

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
            //SpineShow.gameObject.SetActive(true);
            for (int i = 0; i < SpineShow.childCount; i++)
            {
                Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            }

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
            //我写的
            flowerSpine = curTrans.Find("flowerspine");
            FlowerClickEvent = curTrans.Find("FlowerClickEvent");
            indexChangeColor = 0;
            _canClick = true;
            xem = curTrans.Find("mask/xem");
            dd = curTrans.Find("mask/dd");
            gamexem = curTrans.Find("xem");
            wangdou = curTrans.Find("drag/wangdou");
            drag = curTrans.Find("drag");
            drop = curTrans.Find("drop");
            drop2 = curTrans.Find("drop2");
            drop3 = curTrans.Find("drop3");
            drop4 = curTrans.Find("drop4");
            drop5 = curTrans.Find("drop5");
            xiaoguo = curTrans.Find("xiaoguo");
            Util.AddBtnClick(FlowerClickEvent.gameObject,OnClickChangeColor);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            InitILDragers();
            AddDragersEvent();
            //GameStart();
        }
        void InitILDragers()
        {
            _iLDragers = new List<mILDrager>();
            var iLDrager = drag.GetComponent<mILDrager>();
            _iLDragers.Add(iLDrager);
        }
        private void AddDragersEvent()
        {
            foreach (var drager in _iLDragers)
                drager.SetDragCallback(DragStart, Draging, DragEnd);
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            Debug.Log("DragStart1执行成功");
            
            if (type == 1)
            {

               
                //UIEventListener.Get(drop.gameObject).onExit = OnExit;
                Debug.Log("DragStart1成功挂载执行成功");
            }
        }
        private void OnEnter(GameObject go)
        {
            Debug.Log("OnEnter1执行成功");
            SpineManager.instance.DoAnimation(flowerSpine.gameObject,"hua4",false);
            SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject,"wangdou1",false,()=> 
            {
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject,"xiaoguo",false);
            });
        }
        private void Draging(Vector3 position, int type, int index)
        {

        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            if (isMatch && type == 1)
            {
                _iLDragers[0].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                drag.GetComponent<mILDrager>().isActived = false;
             
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou4", false, () =>
                {
                    SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou", true);
                    SpineManager.instance.DoAnimation(xiaoguo.gameObject, "xiaoguo", false, () =>
                     {
                         drag.GetComponent<mILDrager>().isActived = true;
                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                         SpineManager.instance.DoAnimation(gamexem.gameObject, "xem-", false, () => 
                         {
                             
                             SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true);
                         });
                         SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", false);
                         SpineManager.instance.DoAnimation(flowerSpine.gameObject, "huaA2", true);
                         drop.GetComponent<mILDroper>().isActived = false;
                         drop2.GetComponent<mILDroper>().isActived = true;
                         drag.GetComponent<mILDrager>().dragType = 2;
                         drag.GetComponent<mILDrager>().index = 1;
                     });
                });
            }
            else if (isMatch && type == 2) 
            {
                _iLDragers[0].DoReset();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                drag.GetComponent<mILDrager>().isActived = false;
             SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou3", false, () => 
                {
                    SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou", true);
                    SpineManager.instance.DoAnimation(xiaoguo.gameObject, "xiaoguo", false, () => 
                    {
                        drag.GetComponent<mILDrager>().isActived = true;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(gamexem.gameObject, "xem-", false, () => { SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true); });
                        SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(flowerSpine.gameObject, "huaA3", true);
                        drop2.GetComponent<mILDroper>().isActived = false;
                        drop3.GetComponent<mILDroper>().isActived = true;
                        drag.GetComponent<mILDrager>().dragType = 3;
                        drag.GetComponent<mILDrager>().index = 2;
                    });
                });
            }
            else if (isMatch && type == 3)
            {
                _iLDragers[0].DoReset();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                drag.GetComponent<mILDrager>().isActived = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou2", false, () =>
                {
                    SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou", true);
                    SpineManager.instance.DoAnimation(xiaoguo.gameObject, "xiaoguo", false, () =>
                    {
                        drag.GetComponent<mILDrager>().isActived = true;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(gamexem.gameObject, "xem-", false, () => { SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true); });
                        SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(flowerSpine.gameObject, "huaA4", true);
                        drop3.GetComponent<mILDroper>().isActived = false;
                        drop4.GetComponent<mILDroper>().isActived = true;
                        drag.GetComponent<mILDrager>().dragType = 4;
                        drag.GetComponent<mILDrager>().index = 3;
                    });
                });
            }
            else if (isMatch && type == 4)
            {
                _iLDragers[0].DoReset();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                drag.GetComponent<mILDrager>().isActived = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou1", false, () =>
                {
                    SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou", true);
                    SpineManager.instance.DoAnimation(xiaoguo.gameObject, "xiaoguo", false, () =>
                    {
                        drag.GetComponent<mILDrager>().isActived = true;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(gamexem.gameObject, "xem-", false, () => { SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true); });
                        SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", false);
                        SpineManager.instance.DoAnimation(flowerSpine.gameObject, "huaA5", true);
                        drop4.GetComponent<mILDroper>().isActived = false;
                        drop5.GetComponent<mILDroper>().isActived = true;
                        drag.GetComponent<mILDrager>().dragType = 5;
                        drag.GetComponent<mILDrager>().index = 4;
                    });
                });
            }
            else if (isMatch && type == 5)
            {
                _iLDragers[0].DoReset();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                drag.GetComponent<mILDrager>().isActived = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
                SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou5",false,()=> 
                {
                    SpineManager.instance.DoAnimation(drag.GetChild(0).gameObject, "wangdou", true);
                    SpineManager.instance.DoAnimation(xiaoguo.gameObject,"xiaoguo",false,()=> 
                    {
                        drag.GetComponent<mILDrager>().isActived = true;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                        SpineManager.instance.DoAnimation(gamexem.gameObject, "xem-", false, () => { SpineManager.instance.DoAnimation(gamexem.gameObject, "kong", true); gamexem.gameObject.SetActive(false); });
                      
                        SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", false);
                        drop5.GetComponent<mILDroper>().isActived = false;
                        drag.GetComponent<mILDrager>().isActived = false;
                        drag.GetComponent<mILDrager>().canMove = false;

                        drag.GetComponent<mILDrager>().dragType = 1;
                        drag.GetComponent<mILDrager>().index = 0;
                        mono.StartCoroutine(WaitSecound(2f, () => { playSuccessSpine(); }));
                    });
                });
            }
            else
            {
                _iLDragers[0].DoReset();
            }
        }
        private void OnClickChangeColor(GameObject obj) 
        {
            if (_canClick&&indexChangeColor == 0)
            {
                BtnPlaySound();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua2", true);
                indexChangeColor++;
            }
            else if (_canClick && indexChangeColor ==1)
            {
                BtnPlaySound();
                SpineManager.instance.DoAnimation(flowerSpine.gameObject, "hua4", true);
                indexChangeColor++;
            }
            else if (_canClick && indexChangeColor == 2)
            {
                BtnPlaySound();
                indexChangeColor++;
            }
            //indexChangeColor =indexChangeColor % 3;
            if (_canClick&&indexChangeColor == 3) 
            {
                BtnPlaySound();
                mask.SetActive(true);
                //小恶魔出
                xem.gameObject.SetActive(true);
                
                
                devil.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutinexem(xem.gameObject, SoundManager.SoundType.SOUND, 3, () =>
                {
                    SpineManager.instance.DoAnimation(xem.gameObject, "speak", true);
                }, () =>
                {
                    SpineManager.instance.DoAnimation(xem.gameObject, "daiji", false);
                    SoundManager.instance.ShowVoiceBtn(true);
                    
                    
                  
                }));
                //mono.StartCoroutine(WaitSecound(2f,()=> 
                //{
                   


                //}));
               
            }
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
                    Debug.Log("111111223213");
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                    {
                        GameInit();
                        FlowerClickEvent.gameObject.SetActive(true);
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false); 
                        _canClick = true;
                        indexChangeColor = 0;
                        drag.gameObject.SetActive(false);
                       // xem.gameObject.SetActive(false);
                        SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true);
                        SpineManager.instance.DoAnimation(flowerSpine.gameObject,"hua1",true);
                        drag.GetComponent<mILDrager>().isActived = true;
                        drag.GetComponent<mILDrager>().canMove = true;
                        talkIndex = 2;
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutinedd(dbd, SoundManager.SoundType.SOUND,8)); });
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
           
            bdText.text = "";
            devilText.text = "";
            xdd.gameObject.SetActive(false);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }
            SpineManager.instance.DoAnimation(gamexem.gameObject, "xem", true);
            SpineManager.instance.DoAnimation(flowerSpine.gameObject,"hua1",true);//主花初始化
            SpineManager.instance.DoAnimation(wangdou.gameObject, "wangdou", true);
            SpineManager.instance.DoAnimation(xiaoguo.gameObject, "kong", true);
            devil.SetActive(true);
            xem.gameObject.SetActive(false);
            devilText.text = " ";
            gamexem.gameObject.SetActive(false);
            drag.gameObject.SetActive(false);
            dd.gameObject.SetActive(false);
            drag.GetComponent<mILDrager>().isActived =true;
            drag.GetComponent<mILDrager>().canMove = true;

            drag.GetComponent<mILDrager>().dragType = 1;
            //drop2.GetComponent<mILDrager>().isActived = false;
            //drop3.GetComponent<mILDrager>().isActived = false;
            //drop4.GetComponent<mILDrager>().isActived = false;
            //drop5.GetComponent<mILDrager>().isActived = false;
            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }

        void GameStart()
        {
         
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);


            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, true);
            buDing.gameObject.SetActive(true);
            devil.gameObject.SetActive(true);
            devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 0, () =>
             {
                 ShowDialogue("狂风吹起来吧!", devilText);
             }, () =>
                 {
                     buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                     {
                         mono.StartCoroutine(SpeckerCoroutinedd(bd, SoundManager.SoundType.SOUND, 1, () =>
                         {
                             ShowDialogue("不要伤害我的花！", bdText);
                         }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                     });
                     //devil.SetActive(false);
                     //dd.gameObject.SetActive(true);
                     //mono.StartCoroutine(SpeckerCoroutinedd(dd.gameObject,SoundManager.SoundType.COMMONVOICE,1,()=> 
                     //{
                     //    SpineManager.instance.DoAnimation(dd.gameObject,"animation2",true);
                     //},()=> 
                     //{
                     //    SpineManager.instance.DoAnimation(dd.gameObject, "animation", true);
                     //    SoundManager.instance.ShowVoiceBtn(true);
                     //}));

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
        IEnumerator SpeckerCoroutinexem(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator SpeckerCoroutinedd(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
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
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                //点击标志位
               // flag = 0;
              //  buDing.SetActive(false);
                devil.SetActive(false);
                buDing.SetActive(false);
                FlowerClickEvent.gameObject.SetActive(true);
                // bd.SetActive(true);
                //mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, null, () => { mask.SetActive(false); bd.SetActive(false); }));
                dd.gameObject.SetActive(false);
               // mask.SetActive(false);
                xdd.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutinedd(xdd.gameObject,SoundManager.SoundType.SOUND,2,()=> 
                {
                    
                    _canClick = false;
                },()=>
                {
                    xdd.gameObject.SetActive(false);
                    _canClick = true;
                    mask.SetActive(false);
                }));
            }
            if (talkIndex == 2)
            {
              //  mask.SetActive(false);
                xem.gameObject.SetActive(false);
                dd.gameObject.SetActive(false);
                _canClick = false;
                FlowerClickEvent.gameObject.SetActive(false);
                //dd出
                xdd.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutinedd(xdd.gameObject, SoundManager.SoundType.SOUND, 6, () =>
                {
                    SpineManager.instance.DoAnimation(xdd.gameObject, "animation2", true);
                    drag.GetComponent<mILDrager>().isActived = false;
                    _canClick = false;
                }, () =>
                {
                    SpineManager.instance.DoAnimation(xdd.gameObject, "animation", true);
                    drag.GetComponent<mILDrager>().isActived = true;
                    xdd.gameObject.SetActive(false);
                    _canClick = true;
                    mask.SetActive(false);
                }));

                //  xem.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(flowerSpine.gameObject,"huaA1",true);
                drop.gameObject.SetActive(true);
                drop.GetComponent<mILDroper>().isActived = true;
              
                drag.gameObject.SetActive(true);
                wangdou.gameObject.SetActive(true);
                gamexem.gameObject.SetActive(true);
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
        IEnumerator WaitSecound(float time,Action action=null) 
        {
            yield return new WaitForSeconds(time);
            action?.Invoke();
        }

    }
}
