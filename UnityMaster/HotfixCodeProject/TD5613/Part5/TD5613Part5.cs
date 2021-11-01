using DG.Tweening;
using Spine;
using Spine.Unity;
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
    public class TD5613Part5
    {
        /// <summary>
        /// 拖拽
        /// </summary>
        private List<mILDrager> _iLDragers;
        private List<mILDrager> _iLDragers2;
        private List<mILDrager> _iLDragers3;
       
        /// <summary>
        /// 释放
        /// </summary>
        /// 
        


        private Transform ans_level1Tra;
        private Transform ans_level2Tra;
        private Transform ans_level3Tra;
        private Transform que_level1Tra;
        private Transform que_level2Tra;
        private Transform que_level3Tra;
        private GameObject left_spine;
        private GameObject left_spine2;
        private GameObject left_spine3;
        private Transform Boom;
        private Transform Boom2;
        private Transform Boom3;
        private Transform BoomPos;
        private Transform enemy1;
        private Transform enemy2;
        private Transform enemy3;
        private Transform enemylife;
        private Transform stop;
        private Transform play;
        private Transform playerLife;
        private Transform stopMask;
        private Transform cw;
        private Transform ok;
        private Transform StartPlay;
        private Transform endingSpine;

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

        private GameObject dagong;
        private Transform Level_Drager;
        private Transform Level_Drager2;

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


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
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

            //赋值
            ans_level1Tra = curTrans.Find("Content/levels/level1");
            ans_level2Tra = curTrans.Find("Content/levels/level2");
            ans_level3Tra = curTrans.Find("Content/levels/level3");
            que_level1Tra = curTrans.Find("Content/left/Levels/Level1");
            que_level2Tra = curTrans.Find("Content/left/Levels/Level2");
            que_level3Tra = curTrans.Find("Content/left/Levels/Level3");
            left_spine = curTrans.Find("Content/left/Levels/Level1/Droper/1").gameObject;
            left_spine2 = curTrans.Find("Content/left/Levels/Level2/Droper/1").gameObject;
            left_spine3 = curTrans.Find("Content/left/Levels/Level3/Droper/1").gameObject;
            Boom = curTrans.Find("Boom");
            Boom2 = curTrans.Find("Boom2");
            Boom3 = curTrans.Find("Boom3");
            BoomPos = curTrans.Find("Boompos");
            enemy1 = curTrans.Find("Enemy/enemy1");
            enemy2 = curTrans.Find("Enemy/enemy2");
            enemy3 = curTrans.Find("Enemy/enemy3");
            enemylife = curTrans.Find("EnemyLife/life");
            stop = curTrans.Find("Stop");
            play = curTrans.Find("Play");
            playerLife = curTrans.Find("PlayerLife");
            stopMask = curTrans.Find("PlayerLife/mask");
            StartPlay = curTrans.Find("mask/StartPlay");
            endingSpine = curTrans.Find("mask/endingSpine");
            Level_Drager = curTrans.Find("Content/levels/level1/Drager");
            Level_Drager2 = curTrans.Find("Content/levels/level1/Drager2");
            cw = curTrans.Find("mask/cw");
            Util.AddBtnClick(cw.gameObject, OnClickRePlayBtn);
            ok = curTrans.Find("mask/ok");
            Util.AddBtnClick(ok.gameObject, OnClickOkBtn);

          //  Util.AddBtnClick(stop.gameObject, OnStop);
            Util.AddBtnClick(play.gameObject, OnPlay);
            Util.AddBtnClick(StartPlay.gameObject, OnStartPlay);
            //
            

            tz = "3-5-z";
            sz = "6-12-z";

            mask.SetActive(true);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            

            GameInit();
        
            GameStart();
            InitILDragers();
            AddDragersEvent();
        }
        void InitILDragers() 
        {
            _iLDragers = new List<mILDrager>();
            _iLDragers2 = new List<mILDrager>();
            _iLDragers3 = new List<mILDrager>();
            for (int i =0;i<ans_level1Tra.childCount;i++) 
            {
                var miLDrager = ans_level1Tra.GetChild(i).GetComponent<mILDrager>();
                _iLDragers.Add(miLDrager);
            }
            for (int i = 0; i < ans_level2Tra.childCount; i++)
            {
                var miLDrager = ans_level2Tra.GetChild(i).GetComponent<mILDrager>();
                _iLDragers2.Add(miLDrager);
            }
            for (int i = 0; i < ans_level3Tra.childCount; i++)
            {
                var miLDrager = ans_level3Tra.GetChild(i).GetComponent<mILDrager>();
                _iLDragers3.Add(miLDrager);
            }
        }
        private void AddDragersEvent()
        {
            foreach (var drager in _iLDragers)
                drager.SetDragCallback(DragStart, Draging, DragEnd);
            foreach (var drager in _iLDragers2)
                drager.SetDragCallback(DragStart2, Draging, DragEnd2);
            foreach (var drager in _iLDragers3)
                drager.SetDragCallback(DragStart3, Draging, DragEnd3);
        }
        private void DragStart(Vector3 position, int type, int index)
        {
            Debug.Log(_iLDragers[index]);
            dagong = _iLDragers[index].gameObject;
        }
        private void DragStart2(Vector3 position, int type, int index)
        {
            Debug.Log(_iLDragers2[index]);
            dagong = _iLDragers2[index].gameObject;
        }
        private void DragStart3(Vector3 position, int type, int index)
        {
            Debug.Log(_iLDragers3[index]);
            dagong = _iLDragers3[index].gameObject;
        }
        private void Draging(Vector3 position, int type, int index)
        {
      
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
         
            if (isMatch)
            {
                randomPlayVoiceRight();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);//点击正确爆炸音效
              //  playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
                SpineManager.instance.DoAnimation(left_spine, "TL_LC7_001-right", false);
                ans_level1Tra.GetChild(0).gameObject.SetActive(false);
                Boom.gameObject.SetActive(true);
                Boom.GetChild(1).gameObject.SetActive(true);

                Boom.DOMove(enemy1.position, 2.0f).OnComplete(() =>
                {
                    Boom.GetChild(1).gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Boom.GetChild(0).gameObject, "TL_LC7_baozha", false, () =>
                 {
                   //  playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
                     enemy1.gameObject.SetActive(false);
                     enemylife.GetChild(0).gameObject.SetActive(false);
                     enemylife.GetChild(1).gameObject.SetActive(true);
                     que_level1Tra.gameObject.SetActive(false);
                     que_level2Tra.gameObject.SetActive(true);
                     _iLDragers[index].DoReset();
                     
                     ans_level1Tra.gameObject.SetActive(false);
                     ans_level2Tra.gameObject.SetActive(true);
                     //SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => 
                     //{ if (dagong != null) 
                     //    { dagong.GetComponent<ILDrager>().canMove = false; }
                     //    playSuccessSpine(() => 
                     //    { cw.gameObject.SetActive(true);
                     //        ok.gameObject.SetActive(true);
                     //        dagong.GetComponent<ILDrager>().canMove= true; 
                     //    });
                     //});
                     //playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
                     //SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false);
                     ;
                     
                 });
                    SpineManager.instance.DoAnimation(que_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_001-right2", false);
                    SpineManager.instance.DoAnimation(enemy1.gameObject, "TL_LC7_XEM_02", false);
                });
               
            }
            else 
            {
                randomPlayVoiceFalse();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10,false);
                _iLDragers[index].DoReset();
            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-wrong", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(1).GetChild(0).gameObject, "TL_LC7_001-wrong", false);
                    break;

            }
        }
        private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(type);
            Debug.Log(index);
            Debug.Log(isMatch);
            if (isMatch)
            {
                randomPlayVoiceRight();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);//点击正确爆炸音效
               // playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
                SpineManager.instance.DoAnimation(left_spine2, "TL_LC7_002-right", false);
                ans_level2Tra.GetChild(0).gameObject.SetActive(false);
                Boom2.gameObject.SetActive(true);
                Boom2.GetChild(1).gameObject.SetActive(true);

                Boom2.DOMove(enemy2.position, 2.0f).OnComplete(() =>
                {
                    Boom2.GetChild(1).gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Boom2.GetChild(0).gameObject, "TL_LC7_baozha", false, () =>
                    {
                        enemy2.gameObject.SetActive(false);
                        enemylife.GetChild(1).gameObject.SetActive(false);
                        enemylife.GetChild(2).gameObject.SetActive(true);
                        que_level2Tra.gameObject.SetActive(false);
                        que_level3Tra.gameObject.SetActive(true);
                        _iLDragers2[index].DoReset();
                       
                        ans_level2Tra.gameObject.SetActive(false);
                        ans_level3Tra.gameObject.SetActive(true);
                       // SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => 
                       // { if (dagong != null) { dagong.GetComponent<ILDrager>().canMove = false; } playSuccessSpine(() => { cw.gameObject.SetActive(true); ok.gameObject.SetActive(true); if (dagong != null) { dagong.GetComponent<ILDrager>().canMove = true; } }); });
                       //// playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
                        // SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false);

                    });
                    SpineManager.instance.DoAnimation(que_level2Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-right2", false);
                    SpineManager.instance.DoAnimation(enemy2.gameObject, "TL_LC7_XEM_02", false);
                });

            }
            else
            {
                randomPlayVoiceFalse();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, false);
                _iLDragers2[index].DoReset();
            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(ans_level2Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-wrong", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(ans_level2Tra.GetChild(1).GetChild(0).gameObject, "TL_LC7_002-wrong", false);
                    break;

            }
        }
        private void DragEnd3(Vector3 position, int type, int index, bool isMatch)
        {
            Debug.Log(type);
            Debug.Log(index);
            Debug.Log(isMatch);
            if (isMatch)
            {
                randomPlayVoiceRight();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);//点击正确爆炸音效
              //  playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
                SpineManager.instance.DoAnimation(left_spine3, "TL_LC7_003-right", false);
                ans_level3Tra.GetChild(0).gameObject.SetActive(false);
                Boom3.gameObject.SetActive(true);
                Boom3.GetChild(1).gameObject.SetActive(true);
                Boom3.DOMove(enemy3.position, 2.0f).OnComplete(() =>
                {
                    Boom3.GetChild(1).gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(Boom3.GetChild(0).gameObject, "TL_LC7_baozha", false, () =>
                    {
                        enemy3.gameObject.SetActive(false);
                        enemylife.GetChild(2).gameObject.SetActive(false);
                        enemylife.GetChild(3).gameObject.SetActive(true);
                        _iLDragers3[index].DoReset();

                       
                            playSuccessSpine(() =>
                            {
                                Debug.Log("运行了动画了");
                                cw.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh2", false);
                                ok.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok2", false);

                            });
                        

                      
                        
                    });
                    SpineManager.instance.DoAnimation(que_level3Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-right2", false);
                    SpineManager.instance.DoAnimation(enemy3.gameObject, "TL_LC7_XEM_02", false);
                });

            }
            else
            {
                randomPlayVoiceFalse();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, false);
                _iLDragers3[index].DoReset();
            }
            switch (index)
            {
                case 0:
                    SpineManager.instance.DoAnimation(ans_level3Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-right1", false);
                    break;
                case 1:
                    SpineManager.instance.DoAnimation(ans_level3Tra.GetChild(1).GetChild(0).gameObject, "TL_LC7_003-wrong", false);
                    break;

            }
        }
        private void OnStop(GameObject obj) 
        {
            Debug.Log("OnStop执行了");
            stop.gameObject.SetActive(false);
            play.gameObject.SetActive(true);
            playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
            stopMask.gameObject.SetActive(true);
        }
        private void OnPlay(GameObject obj)
        {
            Debug.Log("OnPlay执行了");
            stop.gameObject.SetActive(true);
            play.gameObject.SetActive(false);
           // playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
            stopMask.gameObject.SetActive(false);
        }
        private void OnStartPlay(GameObject obj)
        {
            Debug.Log("我是开始按钮");
            SpineManager.instance.DoAnimation(StartPlay.GetChild(0).gameObject,"bf", false,()=> 
            {
                StartPlay.gameObject.SetActive(false);
                bd.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, null, () =>
                {
                    mask.SetActive(false);
                  // playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
                }));
            });

          
          
        }
        private void OnClickRePlayBtn(GameObject obj)
        {
           
            SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject,"fh", false, () => 
            { 
                obj.SetActive(false);

                ok.gameObject.SetActive(false);
                GameDataInit();
                mask.SetActive(false); 
                
            });
          
        }
        private void OnClickOkBtn(GameObject obj)
        {
            Debug.Log("OKbtn正常运行");
           // SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh", false);
            SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok", false, () =>
            { 
                obj.SetActive(false);
                cw.gameObject.SetActive(false);
                endingSpine.gameObject.SetActive(true);
                
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9,()=> 
                {
                    SpineManager.instance.DoAnimation(endingSpine.gameObject, "animation2", true);
                    
                },()=> 
                {
                    SpineManager.instance.DoAnimation(endingSpine.gameObject, "animation", true);
                })); 
            });

           
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
            Debug.Log("我被调用了");
            talkIndex = 1;
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
            //初始化 
            //level1
            enemy1.gameObject.SetActive(true); enemy2.gameObject.SetActive(true); enemy3.gameObject.SetActive(true);
            enemylife.GetChild(0).gameObject.SetActive(true); enemylife.GetChild(3).gameObject.SetActive(false); enemylife.GetChild(1).gameObject.SetActive(false); enemylife.GetChild(2).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-wrong1", false);
            ans_level1Tra.GetChild(0).transform.position = curTrans.Find("Drager").position;
            ans_level1Tra.GetChild(0).gameObject.SetActive(true);
            ans_level2Tra.GetChild(0).transform.position = curTrans.Find("Drager").position;
            ans_level2Tra.GetChild(0).gameObject.SetActive(true);
            ans_level3Tra.GetChild(0).transform.position = curTrans.Find("Drager").position;
            ans_level3Tra.GetChild(0).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-wrong1", false);
            
            SpineManager.instance.DoAnimation(ans_level2Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_001-wrong01", false);
            SpineManager.instance.DoAnimation(ans_level3Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-right01", false);
            //SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => { playSuccessSpine(() => { cw.gameObject.SetActive(true); ok.gameObject.SetActive(true); }); });
           // SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => { if (dagong != null) { dagong.gameObject.GetComponent<ILDrager>().canMove = false; } playSuccessSpine(() => { cw.gameObject.SetActive(true); ok.gameObject.SetActive(true);if (dagong != null) { dagong.gameObject.GetComponent<ILDrager>().canMove = true; } }); });
           // playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
            SpineManager.instance.DoAnimation(enemy1.gameObject, "TL_LC7_XEM_01", true);
            SpineManager.instance.DoAnimation(enemy2.gameObject, "TL_LC7_XEM_01", true);
            SpineManager.instance.DoAnimation(enemy3.gameObject, "TL_LC7_XEM_01", true);
            Boom.DOMove(BoomPos.position,0.5f);
            Boom2.DOMove(BoomPos.position, 0.5f);
            Boom3.DOMove(BoomPos.position, 0.5f);
            ans_level1Tra.gameObject.SetActive(true); ans_level2Tra.gameObject.SetActive(false); ans_level3Tra.gameObject.SetActive(false);
            que_level1Tra.gameObject.SetActive(true); que_level2Tra.gameObject.SetActive(false); que_level3Tra.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(que_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_001-right2", true);
            SpineManager.instance.DoAnimation(que_level2Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-right2", true);
            SpineManager.instance.DoAnimation(que_level3Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-right2", true);
            //
        }
        private void GameDataInit() 
        {
            Debug.Log("我被调用了");
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
            //初始化 
            //level1
            enemy1.gameObject.SetActive(true); enemy2.gameObject.SetActive(true); enemy3.gameObject.SetActive(true);
            enemylife.GetChild(0).gameObject.SetActive(true); enemylife.GetChild(3).gameObject.SetActive(false); enemylife.GetChild(1).gameObject.SetActive(false); enemylife.GetChild(2).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_002-wrong1", false);
            SpineManager.instance.DoAnimation(ans_level1Tra.GetChild(1).GetChild(0).gameObject, "TL_LC7_001-wrong01", false);
            SpineManager.instance.DoAnimation(ans_level2Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_001-wrong01", false);
            SpineManager.instance.DoAnimation(ans_level2Tra.GetChild(1).GetChild(0).gameObject, "TL_LC7_002-wrong1", false);
            SpineManager.instance.DoAnimation(ans_level3Tra.GetChild(0).GetChild(0).gameObject, "TL_LC7_003-right01", false);
            //SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => { playSuccessSpine(() => { cw.gameObject.SetActive(true); ok.gameObject.SetActive(true); }); });
            //SpineManager.instance.DoAnimation(playerLife.GetChild(0).gameObject, "TL_LC7_shi jian jing du tiao", false, () => 
            //{
            //    if (dagong != null) 
            //    {
            //        Debug.Log("不为空");
            //        Level_Drager.gameObject.GetComponent<ILDrager>().canMove = false;
            //        Level_Drager2.gameObject.GetComponent<ILDrager>().canMove = false;
            //    }       
            //    playSuccessSpine(() => 
            //    {
            //        cw.gameObject.SetActive(true);
            //        ok.gameObject.SetActive(true);
            //        if (dagong != null)
            //        {
            //            //   Level_Drager2.gameObject.GetComponent<ILDrager>().isActived = true;
            //            //  Level_Drager.gameObject.GetComponent<ILDrager>().isActived = true;
            //        }
            //    });


            //});

            //  playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 1;
            SpineManager.instance.DoAnimation(enemy1.gameObject, "TL_LC7_XEM_01", true);
            SpineManager.instance.DoAnimation(enemy2.gameObject, "TL_LC7_XEM_01", true);
            SpineManager.instance.DoAnimation(enemy3.gameObject, "TL_LC7_XEM_01", true);
            Boom.DOMove(BoomPos.position, 0.5f);
            Boom2.DOMove(BoomPos.position, 0.5f);
            Boom3.DOMove(BoomPos.position, 0.5f);
            ans_level1Tra.gameObject.SetActive(true); ans_level2Tra.gameObject.SetActive(false); ans_level3Tra.gameObject.SetActive(false);
            ans_level1Tra.GetChild(0).gameObject.SetActive(true);
            ans_level2Tra.GetChild(0).gameObject.SetActive(true);
            ans_level3Tra.GetChild(0).gameObject.SetActive(true);
            que_level1Tra.gameObject.SetActive(true); que_level2Tra.gameObject.SetActive(false); que_level3Tra.gameObject.SetActive(false);
            
        }

        void GameStart()
        {
            DOTween.KillAll();
          //  playerLife.GetChild(0).gameObject.GetComponent<SkeletonGraphic>().timeScale = 0;
            StartPlay.gameObject.SetActive(true);
            
            Boom.localPosition = new Vector3(-585f, -120.0004f);
            Boom.gameObject.SetActive(false);
            Boom2.localPosition = new Vector3(-585f, -120.0004f);
            Boom2.gameObject.SetActive(false);
            Boom3.localPosition = new Vector3(-585f, -120.0004f);
            Boom3.gameObject.SetActive(false);
           
            SpineManager.instance.DoAnimation(Boom.GetChild(0).gameObject, "TL_LC7_003-right3", true);
            SpineManager.instance.DoAnimation(Boom2.GetChild(0).gameObject, "TL_LC7_003-right3", true);
            SpineManager.instance.DoAnimation(Boom3.GetChild(0).gameObject, "TL_LC7_003-right3", true);

            //ans_level1Tra.transform.GetChild(0).localPosition = new Vector3(-217.8f, -10f);
            ans_level1Tra.transform.GetChild(0).gameObject.SetActive(true);
           // ans_level1Tra.transform.GetChild(1).localPosition = new Vector3(211f, -2.1f);
            ans_level1Tra.transform.GetChild(1).gameObject.SetActive(true);

            SpineManager.instance.DoAnimation(StartPlay.GetChild(0).gameObject, "bf2", true);
            que_level1Tra.gameObject.SetActive(true);
            que_level2Tra.gameObject.SetActive(false);
            que_level3Tra.gameObject.SetActive(false);
            ans_level1Tra.gameObject.SetActive(true);
            ans_level2Tra.gameObject.SetActive(false);
            ans_level3Tra.gameObject.SetActive(false);
            enemy1.gameObject.SetActive(true);
            enemy2.gameObject.SetActive(true);
            enemy3.gameObject.SetActive(true);
            enemylife.GetChild(0).gameObject.SetActive(true);
            enemylife.GetChild(1).gameObject.SetActive(false);
            enemylife.GetChild(2).gameObject.SetActive(false);
            cw.gameObject.SetActive(false);
            ok.gameObject.SetActive(false);
            successSpine.SetActive(false);
            isPlaying = true;
            endingSpine.gameObject.SetActive(false);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
           // mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));
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
                bd.SetActive(false);
                mask.SetActive(false);
            }
            talkIndex++;
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            Debug.Log("我被调用了 我是successspine");
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            bd.SetActive(false);
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
            SpineManager.instance.DoAnimation(ok.GetChild(0).gameObject, "ok2", true);
            SpineManager.instance.DoAnimation(cw.GetChild(0).gameObject, "fh2", true);

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
        private void randomPlayVoiceRight()
        {
            int i = Random.Range(1, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);

        }
        private void randomPlayVoiceFalse()
        {
            int i = Random.Range(4, 7);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, i, false);

        }
IEnumerator IEWait( float delay, Action callBack)
{
    
        yield return new WaitForSeconds(delay);
        callBack?.Invoke();
    
}

    }
}
