using DG.Tweening;
using Spine;
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
    public class TD3421Part1
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

        private Transform Xdd;
        private Transform _mask;
        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex;  //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;
        private Transform zq;
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
        private bool _canClick;
        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        private int flag = 0;
        //创作指引完全结束
        bool isEnd = false;


        private List<mILDrager> _iLDragers;
        private Transform ILDroper1;
        private Transform ILDroper2;
        private Transform ILDroper3;
        private Transform ILDroper4;
        private Transform ILDroper5;
        private Transform ILDroper6;
        private Transform ILDroper7;
        private Transform zhongzi;


        private Transform ILDrager1;
        private Transform ILDrager2;
        private Transform ILDrager3;
        private Transform ILDrager4;
        private Transform ILDrager5;
        private Transform ILDrager6;
        private Transform ILDrager7;

        private Transform workPan;
        private Transform Grown;
        private Transform Grown2;
        private Transform tudiguang;
        private Transform sashuihu;
        private Transform mainSpine;
        private Transform taiyangguang;
        private Transform XDD;

        private Transform DBD2;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            _canClick = true;
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);//开始的时候的mask

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");
            Xdd = curTrans.Find("mask/XDD1");
            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");
            zq = curTrans.Find("zq");
            DBD2 = curTrans.Find("mask/DBD2");

            _mask = curTrans.Find("_mask");
            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            workPan = curTrans.Find("Drag");
            Grown = curTrans.Find("MainSpine/Grown"); 
            tudiguang = curTrans.Find("MainSpine/tudiguang");
            sashuihu = curTrans.Find("MainSpine/sashuihu");
            mainSpine = curTrans.Find("MainSpine/mainSpine");
            taiyangguang = curTrans.Find("MainSpine/mainSpine/taiyangguang");
            XDD = curTrans.Find("mask/XDD");

            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            zhongzi = curTrans.Find("workPan/zhongzi");
            ILDroper1 = curTrans.Find("ILDroper/ILDroper1");
            ILDroper2 = curTrans.Find("ILDroper/ILDroper2");
            ILDroper3 = curTrans.Find("ILDroper/ILDroper3");
            ILDroper4 = curTrans.Find("ILDroper/ILDroper4");
            ILDroper5 = curTrans.Find("ILDroper/ILDroper5");
            ILDroper6 = curTrans.Find("ILDroper/ILDroper6");
            ILDroper7 = curTrans.Find("ILDroper/ILDroper7");

            ILDrager1 = curTrans.Find("Drag/zhongzi");
            ILDrager2 = curTrans.Find("Drag/huasa");
            ILDrager3 = curTrans.Find("Drag/huafei");
            ILDrager4 = curTrans.Find("Drag/taiyang");
            ILDrager5 = curTrans.Find("Drag/chanzi");
            ILDrager6 = curTrans.Find("Drag/shou");
            ILDrager7 = curTrans.Find("Drag/jiandao");
            //pageBar = curTrans.Find("PageBar").gameObject;
            // SlideSwitchPage(pageBar);
            //  SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            // SpinePage.GetComponent<HorizontalLayoutGroup>().enabled = true;
            // e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

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


            //SpineShow = curTrans.Find("SpineShow");
            //SpineShow.gameObject.SetActive(true);
            //for (int i = 0; i < SpineShow.childCount; i++)
            //{
            //    Util.AddBtnClick(SpineShow.GetChild(i).gameObject, OnClickPlay);
            //}

            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            InitILDragers();
            AddDragersEvent();
            Bilingevent();

            GameDataInit();
            GameInit();
            //GameStart();
        }

        void InitILDragers() 
        {
            _iLDragers = new List<mILDrager>();
            var iLDrager1= ILDrager1.GetComponent<mILDrager>();
            var iLDrager2 = ILDrager2.GetComponent<mILDrager>();
            var iLDrager3 = ILDrager3.GetComponent<mILDrager>();
            var iLDrager4 = ILDrager4.GetComponent<mILDrager>();
            var iLDrager5 = ILDrager5.GetComponent<mILDrager>();
            var iLDrager6 = ILDrager6.GetComponent<mILDrager>();
            var iLDrager7= ILDrager7.GetComponent<mILDrager>();
            //for (int i = 0; i < workPan.childCount; i++)
            //{
            //    var iLDrager = workPan.GetChild(i).GetComponent<mILDrager>();
            //    _iLDragers.Add(iLDrager);
            //}
            //var iLDrager = ILDrager1.GetComponent<mILDrager>();
            _iLDragers.Add(iLDrager1);
            _iLDragers.Add(iLDrager2);
            _iLDragers.Add(iLDrager3);
            _iLDragers.Add(iLDrager4);
            _iLDragers.Add(iLDrager5);
            _iLDragers.Add(iLDrager6);
            _iLDragers.Add(iLDrager7);
        }
       
        private void AddDragersEvent()
        {
            foreach (var drager in _iLDragers)
                drager.SetDragCallback(DragStart, Draging, DragEnd);
        }
       
        private void DragStart(Vector3 position, int type, int index)
        {
            Debug.Log(index);
            Debug.Log(type);
            Debug.Log("DragStart1执行成功");
            _iLDragers[index].transform.SetAsLastSibling();

        }
        private void Draging(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            Debug.Log("Draging执行成功");
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(type);
            Debug.Log(" DragEnd执行成功");
            Debug.Log(isMatch);
            if (_canClick&&isMatch && index == 0)
            {
                _mask.gameObject.SetActive(true);
                _canClick = false;
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                                                                                              //  zhongzi.gameObject.SetActive(false);
                ILDrager1.gameObject.SetActive(false);//种子image关闭
                                                                //    SpineManager.instance.DoAnimation(workPan.GetChild(0).GetChild(0).gameObject, "LC4000", true);//种子动画设为空



                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
                SpineManager.instance.DoAnimation(Grown.gameObject, "zhongzi3", false, () =>//土地里的种子发光然后静置
                 {
                     SpineManager.instance.DoAnimation(Grown.gameObject, "zhongzi4", true);
                     ILDroper1.GetComponent<mILDroper>().isActived = false;
                     ILDroper2.GetComponent<mILDroper>().isActived = true;
                     _canClick = true;
                     _mask.gameObject.SetActive(false);
                     WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager2.transform.GetChild(0).gameObject, "sashuihu", true); });
                     WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager2.transform.GetChild(0).gameObject, "sashuihu2", false); });
                 });
            }
            else if (_canClick && isMatch && index == 1)
            {
                _canClick = false;
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager2.gameObject.SetActive(false);//水壶image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                SpineManager.instance.DoAnimation(sashuihu.gameObject, "sashuihu3", false, () => //水壶洒水
                {
                    //  SpineManager.instance.DoAnimation(workPan.GetChild(1).GetChild(0).gameObject, "LC4000", true);//水壶动画设为空
                    _canClick = true;
                    _mask.gameObject.SetActive(false);
                });
                
                SpineManager.instance.DoAnimation(Grown.gameObject, "faya3", false, () =>//土地里的种子发芽
                {
                    SpineManager.instance.DoAnimation(Grown.gameObject, "faya", false,()=> 
                    {
                        SpineManager.instance.DoAnimation(Grown.gameObject, "faya2", false);
                    });
                    ILDroper2.GetComponent<mILDroper>().isActived = false;
                    ILDroper3.GetComponent<mILDroper>().isActived = true;
                  
                    WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager3.transform.GetChild(0).gameObject, "youjifei", true); });
                    WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager3.transform.GetChild(0).gameObject, "youjifei2", true); });
                });
            }
            else if (_canClick && isMatch && index == 2)
            {
                _canClick = false;
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager3.gameObject.SetActive(false);//化肥image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SpineManager.instance.DoAnimation(sashuihu.gameObject, "youjifei3", false, () =>//洒化肥
                   {

                   });
                SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang11", false, () =>//土地里的种子生长
                {
                    SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang12", true);
                    ILDroper3.GetComponent<mILDroper>().isActived = false;
                    ILDroper4.GetComponent<mILDroper>().isActived = true; _canClick = true; _mask.gameObject.SetActive(false);
                    WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager4.transform.GetChild(0).gameObject, "taiyang", true); });
                 //   WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager4.transform.GetChild(0).gameObject, "taiyang2", true); });
                });
            }
            else if (_canClick && isMatch && index == 3)
            {
                _canClick = false;
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager4.gameObject.SetActive(false);//太阳image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                SpineManager.instance.DoAnimation(taiyangguang.gameObject, "bj2", false, () => //背景太阳出现
                {
                    SpineManager.instance.DoAnimation(taiyangguang.gameObject, "LC4000", false);
                   
                });
                SpineManager.instance.DoAnimation(sashuihu.gameObject, "wendujj", false, () =>//温度计
                {

                    SpineManager.instance.DoAnimation(sashuihu.gameObject, "wendujj2", false, () =>
                     {
                         SpineManager.instance.DoAnimation(sashuihu.gameObject, "wendujj3", false, () =>
                           {
                               SpineManager.instance.DoAnimation(sashuihu.gameObject, "LC4000", true);
                               SpineManager.instance.DoAnimation(taiyangguang.gameObject, "LC4000", true);
                               _canClick = true;
                               _mask.gameObject.SetActive(false);
                               WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager5.transform.GetChild(0).gameObject, "chanzi", true); });
                               WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager5.transform.GetChild(0).gameObject, "chanzi2", true); });
                           });
                     });
                });
                SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang21", false, () =>//土地里的种子生长
                {
                    SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang22", true);
                    ILDroper4.GetComponent<mILDroper>().isActived = false;
                    ILDroper5.GetComponent<mILDroper>().isActived = true; 
                });

            }
            else if (_canClick && isMatch && index == 4)
            {
                _canClick = false;
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager5.gameObject.SetActive(false);//铲子image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                SpineManager.instance.DoAnimation(sashuihu.gameObject, "chanzi3", false, () =>
                 {
                     SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang23", false, () =>
                        {
                            SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang24", true);
                            ILDroper5.GetComponent<mILDroper>().isActived = false;
                            ILDroper6.GetComponent<mILDroper>().isActived = true; _canClick = true; _mask.gameObject.SetActive(false);
                            WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager6.transform.GetChild(0).gameObject, "shou", true); });
                            WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager6.transform.GetChild(0).gameObject, "shou2", true); });
                        });
                 });
            }
            else if (_canClick && isMatch && index == 5)
            {
                _canClick = false;
                _mask.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager6.gameObject.SetActive(false);//手image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6);
                SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang25", false, () =>
                   {
                       SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang31", false,()=> 
                       {
                           SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang32", true);
                           ILDroper6.GetComponent<mILDroper>().isActived = false;
                           ILDroper7.GetComponent<mILDroper>().isActived = true; _canClick = true; _mask.gameObject.SetActive(false);
                           WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager7.transform.GetChild(0).gameObject, "jiandao", true); });
                           WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager7.transform.GetChild(0).gameObject, "jiandao2", true); });
                       });
                   });
            }
            else if (_canClick && isMatch && index == 6)
            {
                _canClick = false;
                SpineManager.instance.DoAnimation(tudiguang.gameObject, "TuDiFaGuang", false);//土地发光
                ILDrager7.gameObject.SetActive(false);//剪刀image关闭
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7);
                SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang33", false, () =>
                   {
                       SpineManager.instance.DoAnimation(Grown.gameObject, "shengzhang34", true);
                       ILDroper7.GetComponent<mILDroper>().isActived = false;
                       //成功动画
                       SpineManager.instance.DoAnimation(zq.gameObject,"zq",true);
                     
                       WaitTiming(2f,()=> { playSuccessSpine(); SpineManager.instance.DoAnimation(zq.gameObject, "LC4000", true); });
                       
                   });
            }
            else
            {
                _iLDragers[0].DoReset(); _iLDragers[1].DoReset(); _iLDragers[2].DoReset(); _iLDragers[3].DoReset(); _iLDragers[4].DoReset(); _iLDragers[5].DoReset(); _iLDragers[6].DoReset();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9);
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
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    { anyBtns.gameObject.SetActive(false); 
                        mask.SetActive(false);
                        WaitTiming(1f, () => { SpineManager.instance.DoAnimation(_iLDragers[0].transform.GetChild(0).gameObject, "zhongzi", true); });
                        WaitTiming(3f, () => { SpineManager.instance.DoAnimation(_iLDragers[0].transform.GetChild(0).gameObject, "zhongzi2", true); });
                        //mono.StartCoroutine(SpeckerCoroutineXDD(Xdd.gameObject, SoundManager.SoundType.SOUND, 1, () =>
                        //{ Xdd.gameObject.SetActive(true); SpineManager.instance.DoAnimation(Xdd.gameObject, "animation2", true); _mask.gameObject.SetActive(true); }, () =>
                        //{
                        //    _mask.gameObject.SetActive(false);
                        //    SpineManager.instance.DoAnimation(Xdd.gameObject, "animation", false);
                        //    Xdd.gameObject.SetActive(false);
                        //}));
                        GameDataInit();//游戏重玩初始化
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => 
                    { switchBGM(); 
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true); 
                        mono.StartCoroutine(SpeckerCoroutineXDD(dbd, SoundManager.SoundType.SOUND, 2,null,()=> 
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        })); });
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
            mask.SetActive(true);
            _mask.gameObject.SetActive(false);
            Xdd.gameObject.SetActive(false);
            XDD.gameObject.SetActive(false);
            DBD2.gameObject.SetActive(false);

            ILDrager1.transform.SetAsFirstSibling();
            ILDrager2.transform.SetSiblingIndex(1);
            ILDrager3.transform.SetSiblingIndex(2);
            ILDrager4.transform.SetSiblingIndex(3);
            ILDrager5.transform.SetSiblingIndex(4);
            ILDrager6.transform.SetSiblingIndex(5);
            ILDrager7.transform.SetAsLastSibling();

            // SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            //for (int i = 0; i < SpinePage.childCount; i++)
            //{
            //    SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            //}

            //  SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            // SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
        }
        private void GameDataInit() 
        {
            _canClick = true;
            _mask.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(Grown.gameObject, "LC4000", false);//让植物为空
            for (int i = 0; i < workPan.childCount; i++) //让工具栏物品全都出现
            {
                workPan.GetChild(i).gameObject.SetActive(true);
            }

            SpineManager.instance.DoAnimation(ILDrager1.transform.GetChild(0).gameObject, "zhongzi2", true);
            SpineManager.instance.DoAnimation(ILDrager2.transform.GetChild(0).gameObject, "sashuihu2", true);
            SpineManager.instance.DoAnimation(ILDrager3.transform.GetChild(0).gameObject, "youjifei2", true);
            SpineManager.instance.DoAnimation(ILDrager4.transform.GetChild(0).gameObject, "taiyang2", true);
            SpineManager.instance.DoAnimation(ILDrager5.transform.GetChild(0).gameObject, "chanzi2", true);
            SpineManager.instance.DoAnimation(ILDrager6.transform.GetChild(0).gameObject, "shou2", true);
            SpineManager.instance.DoAnimation(ILDrager7.transform.GetChild(0).gameObject, "jiandao2", true);
            ILDrager1.transform.position = curTrans.Find("zhongzi").position;
            ILDrager2.transform.position = curTrans.Find("huasa").position;
            ILDrager3.transform.position = curTrans.Find("huafei").position;
            ILDrager4.transform.position = curTrans.Find("taiyang").position;
            ILDrager5.transform.position = curTrans.Find("chanzi").position;
            ILDrager6.transform.position = curTrans.Find("shou").position;
            ILDrager7.transform.position = curTrans.Find("jiandao").position;

            SpineManager.instance.DoAnimation(Grown.gameObject,"LC4000",false);
            SpineManager.instance.DoAnimation(sashuihu.gameObject, "LC4000", false);
            SpineManager.instance.DoAnimation(tudiguang.gameObject, "LC4000", false);
            SpineManager.instance.DoAnimation(taiyangguang.gameObject, "LC4000", false);
            ILDroper1.GetComponent<mILDroper>().isActived = true;

         
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //mask.SetActive(false);//按下播放 游戏开始
            XDD.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutineXDD(XDD.gameObject,SoundManager.SoundType.SOUND,0,()=> 
            {
                SpineManager.instance.DoAnimation(XDD.gameObject,"animation2",true);
            },()=> 
            {
                SpineManager.instance.DoAnimation(XDD.gameObject, "animation", true);
                SoundManager.instance.ShowVoiceBtn(true);
                // mask.SetActive(false);
                  


               
            }));
            //SpineManager.instance.DoAnimation(XDD.gameObject,"animation");
            //devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            //{
            //    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 0, () =>
            // {
            //     ShowDialogue("", devilText);
            // }, () =>
            //     {
            //         buDing.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            //         {
            //             mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, 1, () =>
            //          {
            //              ShowDialogue("", bdText);
            //          }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //         });
            //     }));

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
        IEnumerator SpeckerCoroutineXDD(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            //if (!speaker)
            //{
            //    speaker = bd;
            //}
            //SoundManager.instance.SetShield(false);

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
                XDD.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutineXDD(Xdd.gameObject, SoundManager.SoundType.SOUND, 1, () =>
                { Xdd.gameObject.SetActive(true); SpineManager.instance.DoAnimation(Xdd.gameObject, "animation2", true); _mask.gameObject.SetActive(true); }, () =>
                {
                   
                    SpineManager.instance.DoAnimation(Xdd.gameObject, "animation", false);
                    _mask.gameObject.SetActive(false);
                      Xdd.gameObject.SetActive(false);
                    mask.SetActive(false);
                    WaitTiming(1f, () => { SpineManager.instance.DoAnimation(ILDrager1.transform.GetChild(0).gameObject, "zhongzi", true); });
                    WaitTiming(3f, () => { SpineManager.instance.DoAnimation(ILDrager1.transform.GetChild(0).gameObject, "zhongzi2", true); });
                }));
            }
            //if (talkIndex == 2)
            //{
            //    //点击标志位
            //    flag = 0;
            //    buDing.SetActive(false);
            //    devil.SetActive(false);
            //    bd.SetActive(true);
            //    mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.SOUND, 2, null, () => { mask.SetActive(false); bd.SetActive(false); }));
            //}
            if (talkIndex == 2) 
            {
                dbd.gameObject.SetActive(false);
                DBD2.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(DBD2.gameObject, SoundManager.SoundType.SOUND, 3, null, () => 
                {
                  
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
            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine,  "3-5-z2", false,
                () =>
                {
                    

                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
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
        private void Bilingevent() 
        {
            UIEventListener.Get(ILDrager1.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager1.transform.GetChild(0).gameObject, "zhongzi", false);
            };
            UIEventListener.Get(ILDrager2.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager2.transform.GetChild(0).gameObject, "sashuihu", false);
            };
            UIEventListener.Get(ILDrager3.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager3.transform.GetChild(0).gameObject, "youjifei", false);
            };
            UIEventListener.Get(ILDrager4.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager4.transform.GetChild(0).gameObject, "taiyang", false);
            };
            UIEventListener.Get(ILDrager5.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager5.transform.GetChild(0).gameObject, "chanzi", false);
            };
            UIEventListener.Get(ILDrager6.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager6.transform.GetChild(0).gameObject, "shou", false);
            };
            UIEventListener.Get(ILDrager7.gameObject).onDown = OnDown =>
            {

                SpineManager.instance.DoAnimation(ILDrager7.transform.GetChild(0).gameObject, "jiandao", false);
            };
        }
        private void WaitTiming(float time,Action method_1=null)
        {
            mono.StartCoroutine(WaitTime(time,method_1));
        }
        IEnumerator WaitTime(float time,Action method_1=null) 
        {
            yield return new WaitForSeconds(time);
            method_1?.Invoke();
        }

        
    }
}
