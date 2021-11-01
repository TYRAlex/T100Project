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
    public class TD5613Part1
    {
        //我写的
        private Transform NextLevelMask;
        private Transform endingSpine;
        private GameObject enterArea;
        private GameObject ChangeColorPant1;
        private GameObject ChangeColorPant2;
        private GameObject ChangeColorPant3;
        private GameObject ChangeColorPant4;
        private int index;
        private Transform _levelsTra;
        private Transform _levelsTra2;
        private Transform _levelsTra3;
        private Transform _levelsTra4;
        private Transform _endPos;
        private Transform _endPos2;
        private Transform _endPos3;
        private Transform _endPos4;
        private GameObject _enterPos;
        private GameObject _enterPos2;
        private GameObject _enterPos3;
        private GameObject _enterPos4;
        private GameObject moutain_spine;
        private GameObject moutain_spine2;
        private GameObject moutain_spine3;
        private GameObject moutain_spine4;
        private GameObject _A6Spine;
        private GameObject _A7Spine;
        private GameObject _A8Spine;
        private GameObject _B6Spine;
        private GameObject _B7Spine;
        private GameObject _B8Spine;
        private GameObject _C6Spine;
        private GameObject _C7Spine;
        private GameObject _C8Spine;
        private GameObject _D6Spine;
        private GameObject _D7Spine;
        private GameObject _D8Spine;
        private GameObject _Level1;
        private GameObject _Level2;
        private GameObject _Level3;
        private GameObject _Level4;
        private GameObject Finish;
        private GameObject Play;
        private Transform PlayOnClick;
        private GameObject Startbg;
        private GameObject Levels;
        private Transform cw;
        private Transform ok;
        private Transform voiceBtn;
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        private GameObject bd1;
        private Transform drag;
        private Transform drag2;
        private Transform drag3;
        private Transform drag4; 

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


        /// <summary>
        /// 拖拽数组
        /// </summary>
        private List<mILDrager> _iLDragers;
        private List<mILDrager> _iLDragers2;
        private List<mILDrager> _iLDragers3;
        private List<mILDrager> _iLDragers4;
        /// <summary>
        /// 释放数组
        /// </summary>
        private ILDroper[] _iLDropers;
        private ILDroper[] _iLDropers2;
        private ILDroper[] _iLDropers3;
        private ILDroper[] _iLDropers4;
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
            //mask.SetActive(false);

            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();


            bd = curTrans.Find("mask/BD").gameObject;
            //bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            //赋值
            enterArea = curTrans.Find("area").gameObject;
            drag = curTrans.Find("Levels/Level1/drag");
            drag2 = curTrans.Find("Levels/Level2/drag");
            drag3 = curTrans.Find("Levels/Level3/drag");
            drag4 = curTrans.Find("Levels/Level4/drag");

            index = 0;
            endingSpine = curTrans.Find("mask/endingspine");
            ChangeColorPant1 = curTrans.Find("Levels/Level1/EnterArea").gameObject;
            ChangeColorPant2 = curTrans.Find("Levels/Level2/EnterArea").gameObject;
            ChangeColorPant3 = curTrans.Find("Levels/Level3/EnterArea").gameObject;
            ChangeColorPant4 = curTrans.Find("Levels/Level4/EnterArea").gameObject;
            _levelsTra = curTrans.Find("Levels/Level1/EnterArea/ChangeColorPan");
            _levelsTra2 = curTrans.Find("Levels/Level2/EnterArea/ChangeColorPan");
            _levelsTra3 = curTrans.Find("Levels/Level3/EnterArea/ChangeColorPan");
            _levelsTra4 = curTrans.Find("Levels/Level4/EnterArea/ChangeColorPan");
            _endPos = curTrans.Find("Levels/Level1/moutain/endPos");
            _endPos2 = curTrans.Find("Levels/Level2/moutain/endPos");
            _endPos3 = curTrans.Find("Levels/Level3/moutain/endPos");
            _endPos4 = curTrans.Find("Levels/Level4/moutain/endPos");
            _enterPos = curTrans.Find("Levels/Level1/endPos/IDropArea2").gameObject;
            _enterPos2 = curTrans.Find("Levels/Level2/endPos/IDropArea2").gameObject;
            _enterPos3 = curTrans.Find("Levels/Level3/endPos/IDropArea2").gameObject;
            _enterPos4 = curTrans.Find("Levels/Level4/endPos/IDropArea2").gameObject;
            moutain_spine = curTrans.Find("Levels/Level1/moutain/moutainSpine").gameObject;
            moutain_spine2 = curTrans.Find("Levels/Level2/moutain/moutainSpine").gameObject;
            moutain_spine3 = curTrans.Find("Levels/Level3/moutain/moutainSpine").gameObject;
            moutain_spine4 = curTrans.Find("Levels/Level4/moutain/moutainSpine").gameObject;
            _A6Spine = curTrans.Find("Levels/Level1/drag/1S/1S").gameObject;
            _A7Spine = curTrans.Find("Levels/Level1/drag/2S/2S").gameObject;
            _A8Spine = curTrans.Find("Levels/Level1/drag/3S/3S").gameObject;
            _B6Spine= curTrans.Find("Levels/Level2/drag/1S/1S").gameObject;
            _B7Spine = curTrans.Find("Levels/Level2/drag/2S/2S").gameObject;
            _B8Spine = curTrans.Find("Levels/Level2/drag/3S/3S").gameObject;
            _C6Spine = curTrans.Find("Levels/Level3/drag/1S/1S").gameObject;
            _C7Spine = curTrans.Find("Levels/Level3/drag/2S/2S").gameObject;
            _C8Spine = curTrans.Find("Levels/Level3/drag/3S/3S").gameObject;
            _D6Spine = curTrans.Find("Levels/Level4/drag/1S/1S").gameObject;
            _D7Spine = curTrans.Find("Levels/Level4/drag/2S/2S").gameObject;
            _D8Spine = curTrans.Find("Levels/Level4/drag/3S/3S").gameObject;

            _Level1 = curTrans.Find("Levels/Level1").gameObject;
            _Level2 = curTrans.Find("Levels/Level2").gameObject;
            _Level3 = curTrans.Find("Levels/Level3").gameObject;
            _Level4 = curTrans.Find("Levels/Level4").gameObject;
            voiceBtn = curTrans.Find("mask/voiceBtn");
            Finish = curTrans.Find("mask/Finish").gameObject;
            NextLevelMask = curTrans.Find("NextLevelMask");
            bd1 = curTrans.Find("BD1").gameObject;

            Play = curTrans.Find("mask/Play").gameObject;
            PlayOnClick = curTrans.Find("mask/Play");
            //playonclick按钮点击事件
            Util.AddBtnClick(PlayOnClick.GetChild(0).gameObject, PlayOnClickEvent);
            cw = curTrans.Find("mask/cw");
            Util.AddBtnClick(cw.gameObject, OnClickRePlayBtn);
            ok = curTrans.Find("mask/ok");
            Util.AddBtnClick(ok.gameObject, OnClickOkBtn);
            // Startbg = curTrans.Find("mask/bg").gameObject;
            Levels = curTrans.Find("Levels").gameObject;



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

            //触碰事件
            UIEventListener.Get(enterArea).onEnter = OnEnter;
            UIEventListener.Get(enterArea).onExit = OnExit;
           

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            PlayOnClick.gameObject.SetActive(true);

            talkIndex = 1;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed = 0.5f;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);


            Levels.SetActive(true);
            _Level1.SetActive(true); _Level2.SetActive(false); _Level3.SetActive(false); _Level4.SetActive(false);
            SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);

            InitILDragers();
            AddDragersEvent();
            // GameInit();
            GameStart();
           
          
        }

        void InitILDragers()
        {
            _iLDragers = new List<mILDrager>();

            for (int i = 0; i < _levelsTra.childCount; i++)
            {
                    var iLDrager =drag.GetChild(i).GetComponent<mILDrager>();
                    _iLDragers.Add(iLDrager);
            }
            _iLDragers2 = new List<mILDrager>();

            for (int i = 0; i < _levelsTra2.childCount; i++)
            {
                var iLDrager = drag2.GetChild(i).GetComponent<mILDrager>();
                _iLDragers2.Add(iLDrager);
            }
            _iLDragers3 = new List<mILDrager>();

            for (int i = 0; i < _levelsTra3.childCount; i++)
            {
                var iLDrager = drag3.GetChild(i).GetComponent<mILDrager>();
                _iLDragers3.Add(iLDrager);
            }
            _iLDragers4 = new List<mILDrager>();

            for (int i = 0; i < _levelsTra4.childCount; i++)
            {
                var iLDrager = drag4.GetChild(i).GetComponent<mILDrager>();
                _iLDragers4.Add(iLDrager);
            }
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
            Debug.Log("初始化调用");
            
            talkIndex = 2;
            curPageIndex = 0;
            isPressBtn = false;
            textSpeed =0.5f;
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(curPageIndex * 1920, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name, false);
            }

            SpineManager.instance.DoAnimation(leftBtn.transform.parent.gameObject, leftBtn.transform.parent.name, false);
            SpineManager.instance.DoAnimation(rightBtn.transform.parent.gameObject, rightBtn.transform.parent.name, false);
           
         
            Levels.SetActive(true);
            _Level1.SetActive(true); _Level2.SetActive(false); _Level3.SetActive(false); _Level4.SetActive(false);
            SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);
           // bd.SetActive(true);
            
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, ()=> 
            {
                bd1.SetActive(true);
                SpineManager.instance.DoAnimation(bd1, "animation2", true);
            },()=> 
            {
                SpineManager.instance.DoAnimation(bd1, "animation", true);
                SoundManager.instance.ShowVoiceBtn(true);
            }));

           
        }

        void GameStart()
        {
            Debug.Log("开始游戏");
            NextLevelMask.gameObject.SetActive(true);
            isPlaying = true;
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
            //释放代码赋值
           // _iLDropers = _endPos.GetComponentsInChildren<ILDroper>();
          //  _iLDropers2 = _endPos2.GetComponentsInChildren<ILDroper>();
            SoundManager.instance.ShowVoiceBtn(false);
            mask.SetActive(true);
            Bg.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(bd, "animation", true);
            PlayOnClick.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(PlayOnClick.gameObject, "bf2", true);
            endingSpine.gameObject.SetActive(false);

            ChangeColorPant1.SetActive(true);
            SpineManager.instance.DoAnimation(drag.GetChild(0).GetChild(0).gameObject, "000", false);
            drag.GetChild(0).GetRectTransform().DOAnchorPos(new Vector2(-816.93f, 223f), 0.1f);//第一个
            SpineManager.instance.DoAnimation(drag2.GetChild(1).GetChild(0).gameObject, "000", false);
            SpineManager.instance.DoAnimation(drag3.GetChild(1).GetChild(0).gameObject, "000", false);
            SpineManager.instance.DoAnimation(drag4.GetChild(0).GetChild(0).gameObject, "000", false);
            drag2.GetChild(1).GetRectTransform().DOAnchorPos(new Vector2(-812.33f, -44f), 0.1f);
            drag3.GetChild(2).GetRectTransform().DOAnchorPos(new Vector2(-812.33f, -269f), 0.1f);
            drag4.GetChild(0).GetRectTransform().DOAnchorPos(new Vector2(-816.93f, 219.8f), 0.1f);
            //for (int i = 0; i < 3; i++) 
            //{
            //    _iLDragers[i].DoReset();
            //    _iLDragers2[i].DoReset();
            //    _iLDragers3[i].DoReset();
            //    _iLDragers4[i].DoReset();
            //}

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

            SpineManager.instance.DoAnimation(bd, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null, () =>
                {
                    Bg.SetActive(false);
                    bd.SetActive(false);
                    mask.SetActive(false);
                    ChangeColorPant1.transform.GetRectTransform().DOAnchorPosX(-847f, -11f);
                    ChangeColorPant1.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                    ChangeColorPant1.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                    ChangeColorPant1.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                    drag.gameObject.SetActive(true);

                    //bd.SetActive(true);
                    bd1.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () =>
                    {
                        NextLevelMask.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(bd1, "animation2", true);
                    }, () =>
                    {
                        SpineManager.instance.DoAnimation(bd1, "animation", true);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));


                }));
            }
            if (talkIndex == 2) 
            {
                bd1.SetActive(false);
                NextLevelMask.gameObject.SetActive(false);
            }
            if (talkIndex == 3) 
            {
                bd1.SetActive(false);
                NextLevelMask.gameObject.SetActive(false);
            }
            if (talkIndex == 4)
            {
                bd1.SetActive(false);
                NextLevelMask.gameObject.SetActive(false);
            }
            if (talkIndex == 5)
            {
                bd1.SetActive(false);
                NextLevelMask.gameObject.SetActive(false);
            }
         
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {

            mask.SetActive(true);
            //bd.SetActive(true);
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

        //private ILDroper GetCurILDroper(int index)
        //{
        //    int type = _iLDragers[index].dragType;
        //    ILDroper iLDroper = null;
        //    for (int i = 0; i < _iLDropers.Length; i++)
        //        if (_iLDropers[i].dropType == type)
        //            iLDroper = _iLDropers[i];
        //    return iLDroper;
        //}
        /// <summary>
        /// 播放正确的山脉动画
        /// </summary>
    
       
     

        private void AddDragersEvent()
        {
            foreach (var drager in _iLDragers)
                drager.SetDragCallback(DragStart, Draging, DragEnd);
            foreach (var drager in _iLDragers2)
                drager.SetDragCallback(DragStart2, Draging2, DragEnd2);
            foreach (var drager in _iLDragers3)
                drager.SetDragCallback(DragStart3, Draging3, DragEnd3);
            foreach (var drager in _iLDragers4)
               drager.SetDragCallback(DragStart4, Draging4, DragEnd4);

        }
        private void DragStart(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            Debug.Log("DragStart1执行成功");
            if (type == 2)
            {
                
                UIEventListener.Get(_enterPos).onEnter = OnEnter;
                UIEventListener.Get(_enterPos).onExit = OnExit;
                Debug.Log("DragStart1成功挂载执行成功");
            }
            else
            {
               // _iLDragers[index].transform.parent.parent.SetAsLastSibling();
            }
            
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            Debug.Log("DragStart2执行成功");
            if (type == 2)
            {
               
                UIEventListener.Get(_enterPos2).onEnter = OnEnter2;
                UIEventListener.Get(_enterPos2).onExit = OnExit2;
                Debug.Log("DragStart2成功挂载执行成功");
            }
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            if (type == 2)
            {
                UIEventListener.Get(_enterPos3).onEnter = OnEnter3;
                UIEventListener.Get(_enterPos3).onExit = OnExit3;
            }

        }
        private void DragStart4(Vector3 position, int type, int index)
        {
            Debug.Log(type);
            if (type == 2)
            {
                UIEventListener.Get(_enterPos4).onEnter = OnEnter4;
                UIEventListener.Get(_enterPos4).onExit = OnExit4;
            }

        }
        private void Draging(Vector3 position, int type, int index)
        {
            //UIEventListener.Get(_enterPos);
            //Debug.Log(type);
            //if (type == 2)
            //{
            //    UIEventListener.Get(_enterPos).onEnter = OnEnter;
            //    UIEventListener.Get(_enterPos).onEnter = null;
            //}
            //else 
            //{
            //    UIEventListener.Get(_enterPos).onEnter = OnEnter1;
            //    UIEventListener.Get(_enterPos).onEnter = null;
            //}
        }
     
        private void OnEnter(GameObject go)
        {

            //SpineManager.instance.DoAnimation(xuanxianban, "TL_LC4_xuan xian ban-1/4", false);    
            SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1√2", false);
            //SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1√2", false);
            Debug.Log("OnEnter1执行成功");
        }
        private void OnEnter2(GameObject go)
        {
         
            //SpineManager.instance.DoAnimation(xuanxianban, "TL_LC4_xuan xian ban-1/4", false);      
            //SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1√2", false);
            SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1√2", false);
            Debug.Log("OnEnter2执行成功");
        }
        private void OnEnter3(GameObject go)
        {
            //SpineManager.instance.DoAnimation(xuanxianban, "TL_LC4_xuan xian ban-1/4", false);      
            //SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1√2", false);
            //SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1√2", false);
             SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1√2", false);
        }
        private void OnEnter4(GameObject go)
        {
            //SpineManager.instance.DoAnimation(xuanxianban, "TL_LC4_xuan xian ban-1/4", false);      
            //SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1√2", false);
           // SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1√2", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1√2", false);
            SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1√2", false);
        }
        private void OnExit(GameObject go)
        {
            // paint.transform.DOLocalMoveX(-238.0f,0.5f);
           
            SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            //SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);
            Debug.Log("OnExit1执行成功");
        }
        private void OnExit2(GameObject go)
        {
            // paint.transform.DOLocalMoveX(-238.0f,0.5f);
           
            //SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
             SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            //SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);
            Debug.Log("OnExit2执行成功");
        }

        private void OnExit3(GameObject go)
        {
            // paint.transform.DOLocalMoveX(-238.0f,0.5f);

            //SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
             SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            //SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);
        }

        private void OnExit4(GameObject go)
        {
            // paint.transform.DOLocalMoveX(-238.0f,0.5f);

           // SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1", false);
            // SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1", false);
            SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1", false);
        }

        private void Draging2(Vector3 position, int type, int index)
        {

        }
        private void Draging3(Vector3 position, int type, int index)
        {

        }
        private void Draging4(Vector3 position, int type, int index)
        {

        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            
            if (isMatch)
            {
 
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                randomPlayVoiceRight();
                ChangeColorPant1.SetActive(false);
                drag.gameObject.SetActive(false);

                SpineManager.instance.DoAnimation(moutain_spine, "TL_LC4_xuan xian ban-1/4-1√", false,()=> {

                    mono.StartCoroutine(IEWait(2f, () =>
                    {
                        ChangeColorPant1.SetActive(true);
                        _Level1.Hide();
                        
                        _Level2.Show();
                        ChangeColorPant2.SetActive(true);
                        ChangeColorPant2.transform.GetChild(0).gameObject.SetActive(true);
                        ChangeColorPant2.transform.GetRectTransform().DOAnchorPosX(-555f, -11f);
                        ChangeColorPant2.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        ChangeColorPant2.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        ChangeColorPant2.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                        drag2.gameObject.SetActive(true);
                        //drag.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_A6Spine.gameObject, "000", true);
                        _iLDragers[index].DoReset();
                        //  bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 14, () =>
                        {
                            //mask启动
                            NextLevelMask.gameObject.SetActive(true);
                            bd1.SetActive(true);
                            SpineManager.instance.DoAnimation(bd1, "animation2", true);
                        }, () =>
                        {
                            SpineManager.instance.DoAnimation(bd1, "animation", false);
                            SoundManager.instance.ShowVoiceBtn(true);

                        }));
                    }));

                   
                });
                
            }
            else 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                _iLDragers[index].DoReset();
                //物品所有未激活状态
                drag.transform.GetChild(0).GetComponent<mILDrager>().isActived = false;
                drag.transform.GetChild(1).GetComponent<mILDrager>().isActived = false;
                drag.transform.GetChild(2).GetComponent<mILDrager>().isActived = false;
                mono.StartCoroutine(randomPlayVoiceFalse(()=> 
                {
                    //激活所有物品
                    drag.transform.GetChild(0).GetComponent<mILDrager>().isActived = true;
                    drag.transform.GetChild(1).GetComponent<mILDrager>().isActived = true;
                    drag.transform.GetChild(2).GetComponent<mILDrager>().isActived = true;
                }));

            }
            switch (index) 
            {
                case 0:
                SpineManager.instance.DoAnimation(_A6Spine, "TL_LC4_xuan xian ban-A6XX", false);
                break;
                case 1:
                SpineManager.instance.DoAnimation(_A7Spine, "TL_LC4_xuan xian ban-A7XX", false);
                break;
                case 2:
                SpineManager.instance.DoAnimation(_A8Spine, "TL_LC4_xuan xian ban-A8XX", false);
                break;

            }
            UIEventListener.Get(_enterPos).onEnter = null;
            UIEventListener.Get(_enterPos).onExit = null;
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(type);
           
            Debug.Log(isMatch);
            if (isMatch)
            {
                ChangeColorPant2.SetActive(false);
                drag2.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                randomPlayVoiceRight();
                SpineManager.instance.DoAnimation(moutain_spine2, "TL_LC4_xuan xian ban-2/4-1√", false,()=> {
                    mono.StartCoroutine(IEWait(2f,()=> 
                    {
                        ChangeColorPant2.SetActive(true);
                        _Level2.Hide();
                        
                        _Level3.Show();
                        
                        ChangeColorPant3.SetActive(true);
                        ChangeColorPant3.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        ChangeColorPant3.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        ChangeColorPant3.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                        ChangeColorPant3.transform.GetChild(0).gameObject.SetActive(true);
                        drag3.gameObject.SetActive(true);
                        ChangeColorPant3.transform.GetRectTransform().DOAnchorPosX(-555f, -11f);
                        SpineManager.instance.DoAnimation(_B7Spine.gameObject, "000", true);
                        _iLDragers2[index].DoReset();
                       // bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () =>
                        {
                            NextLevelMask.gameObject.SetActive(true);

                            bd1.SetActive(true);
                            SpineManager.instance.DoAnimation(bd1, "animation2", true);
                        }, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(bd1, "animation", true);
                        }));
                    }));
                    

                });
            }
            else
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7));
                _iLDragers2[index].DoReset();
                drag2.transform.GetChild(0).GetComponent<mILDrager>().isActived = false;
                drag2.transform.GetChild(1).GetComponent<mILDrager>().isActived = false;
                drag2.transform.GetChild(2).GetComponent<mILDrager>().isActived = false;
                mono.StartCoroutine(randomPlayVoiceFalse(() =>
                {
                    //激活所有物品
                    drag2.transform.GetChild(0).GetComponent<mILDrager>().isActived = true;
                    drag2.transform.GetChild(1).GetComponent<mILDrager>().isActived = true;
                    drag2.transform.GetChild(2).GetComponent<mILDrager>().isActived = true;
                }));
            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(_B6Spine, "TL_LC4_xuan xian ban-B6XX", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(_B7Spine, "TL_LC4_xuan xian ban-B7XX", false);
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(_B8Spine, "TL_LC4_xuan xian ban-B8XX", false);
                    break;

            }
            UIEventListener.Get(_enterPos2).onEnter = null;
            UIEventListener.Get(_enterPos2).onExit = null;
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
         
            if (isMatch)
            {
                ChangeColorPant3.SetActive(false);
                drag3.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                randomPlayVoiceRight();
                SpineManager.instance.DoAnimation(moutain_spine3, "TL_LC4_xuan xian ban-3/4-1√", false, () =>
                {
                    mono.StartCoroutine(IEWait(2f,()=> 
                    {
                        ChangeColorPant3.SetActive(true);
                        _Level3.Hide();

                        _Level4.Show();
                        ChangeColorPant4.SetActive(true);
                        ChangeColorPant4.transform.GetChild(0).GetChild(0).gameObject.SetActive(false);
                        ChangeColorPant4.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
                        ChangeColorPant4.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
                        ChangeColorPant4.transform.GetChild(0).gameObject.SetActive(true);
                        ChangeColorPant4.transform.GetRectTransform().DOAnchorPosX(-555f, -11f);
                        drag4.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(_C8Spine.gameObject, "000", true);
                        _iLDragers3[index].DoReset();
                       // bd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, () =>
                        {
                            NextLevelMask.gameObject.SetActive(true);
                            bd1.SetActive(true);
                            SpineManager.instance.DoAnimation(bd1, "animation2", true);
                        }, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                            SpineManager.instance.DoAnimation(bd1, "animation", true);

                        }));
                    }));
                    
                });
            }
            else
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7));
                _iLDragers3[index].DoReset();
                drag3.transform.GetChild(0).GetComponent<mILDrager>().isActived = false;
                drag3.GetChild(1).GetComponent<mILDrager>().isActived = false;
                drag3.GetChild(2).GetComponent<mILDrager>().isActived = false;
                mono.StartCoroutine(randomPlayVoiceFalse(() =>
                {
                    //激活所有物品
                    drag3.GetChild(0).GetComponent<mILDrager>().isActived = true;
                    drag3.GetChild(1).GetComponent<mILDrager>().isActived = true;
                    drag3.GetChild(2).GetComponent<mILDrager>().isActived = true;
                }));
                


            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(_C6Spine, "TL_LC4_xuan xian ban-C6XX", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(_C7Spine, "TL_LC4_xuan xian ban-C7XX", false);
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(_C8Spine, "TL_LC4_xuan xian ban-C8XX", false);
                    break;

            }
            UIEventListener.Get(_enterPos3).onEnter = null;
            UIEventListener.Get(_enterPos3).onExit = null;
        }
        private void DragEnd4(Vector3 position, int type, int index, bool isMatch)
        {
          
            if (isMatch)
            {

                ChangeColorPant4.SetActive(false);
                drag4.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                randomPlayVoiceRight();
                SpineManager.instance.DoAnimation(moutain_spine4, "TL_LC4_xuan xian ban-4/4-1√", false,()=> {
                    //_Level4.Hide();      
                    mono.StartCoroutine(IEWait(2f,()=> 
                    {
                        PlayOnClick.gameObject.SetActive(false);
                       
                        playSuccessSpine(() =>
                        {
                            cw.gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh2", true);
                            ok.gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok2", true);
                        });
                        SpineManager.instance.DoAnimation(_D6Spine.gameObject, "000", true);
                        _iLDragers4[index].DoReset();
                        ChangeColorPant1.SetActive(true);
                        ChangeColorPant1.transform.DOLocalMoveX(-500f, 0.5f);
                        //ChangeColorPant4.transform.DOLocalMoveX(1f, 0.5f);
                    }));
                  

                });
              
            }
            else
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7));
                _iLDragers4[index].DoReset();
                //物品所有未激活状态
                drag.transform.GetChild(0).GetComponent<mILDrager>().isActived = false;
                drag.transform.GetChild(1).GetComponent<mILDrager>().isActived = false;
                drag.GetChild(2).GetComponent<mILDrager>().isActived = false;
                mono.StartCoroutine(randomPlayVoiceFalse(() =>
                {
                    //激活所有物品
                    drag.transform.GetChild(0).GetComponent<mILDrager>().isActived = true;
                    drag.transform.GetChild(1).GetComponent<mILDrager>().isActived = true;
                    drag.transform.GetChild(2).GetComponent<mILDrager>().isActived = true;
                }));

             


            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(_D6Spine, "TL_LC4_xuan xian ban-D6XX", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(_D7Spine, "TL_LC4_xuan xian ban-D7XX", false);
                    break;
                case 2:
                    SpineManager.instance.DoAnimation(_D8Spine, "TL_LC4_xuan xian ban-D8XX", false);
                    break;

            }
            UIEventListener.Get(_enterPos4).onEnter = null;
            UIEventListener.Get(_enterPos4).onExit = null;
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

        private void PlayOnClickEvent(GameObject obj)
        {
            SpineManager.instance.DoAnimation(Play, "bf", false,()=> 
            {
                ChangeColorPant4.SetActive(true);
                mask.SetActive(true);
                Play.SetActive(false);
                bd.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                // SpineManager.instance.DoAnimation(Play, "bf",false,()=> { Startbg.Show(); });
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, null, () => {

                    SoundManager.instance.ShowVoiceBtn(true);

                    // mask.SetActive(false);
                    //  Levels.SetActive(true);
                    // bd.SetActive(false);
                    // ChangeColorPant1.transform.DOLocalMoveX(1f, 0.5f);


                }));
            });

         
        }
        private void OnClickRePlayBtn(GameObject obj)
        {

            SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh", false, () => 
            {
                ChangeColorPant4.SetActive(true);
                obj.SetActive(false); 
                ok.gameObject.SetActive(false);
                mask.SetActive(false);
                ChangeColorPant1.transform.GetRectTransform().DOAnchorPosX(-848f, -11f);
                drag.gameObject.SetActive(true);
                GameInit();
            });
        }
        private void OnClickOkBtn(GameObject obj)
        {
            SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok", false, () =>
            {
               
                obj.SetActive(false);
                cw.gameObject.SetActive(false);
                endingSpine.gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 15,()=> 
                {
                SpineManager.instance.DoAnimation(endingSpine.gameObject, "animation2", true);
                    
                },()=> 
                {
                    SpineManager.instance.DoAnimation(endingSpine.gameObject, "animation", true);
                }));
               
              

            });
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
        private void randomPlayVoiceRight() 
        {
            int i =Random.Range(1, 4);
             SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);
            
        }
        IEnumerator randomPlayVoiceFalse(Action callBack)
        {
            int i = Random.Range(4, 7);
            float ind = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);
            yield return new WaitForSeconds(ind);
            callBack?.Invoke();
        }
        IEnumerator IEWait(float delay, Action callBack)
        {
           
                yield return new WaitForSeconds(delay);
                callBack?.Invoke();
           
        }
    }
}
