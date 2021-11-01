using DG.Tweening;
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
    public class TD6761Part5
    {


        private int talkIndex;
        private int bfIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bd;
        private GameObject dbd;


        //bool isPlaying;
        private GameObject PageBar;
        private Transform SpinePage;
        private Transform SpineShow;
        private GameObject btnBack;
        private GameObject bf;
        private GameObject mask;
        private GameObject BtnSpines;
        private GameObject hudong;
        private GameObject beike;
        private GameObject qrj;
        private GameObject qrj2;
        private GameObject qrj3;
        private GameObject zz1;
        private GameObject zz2;
        private GameObject zz3;
        private GameObject px1;
        private GameObject px2;
        private GameObject px3;
        private GameObject px4;
        private GameObject gz;
        private GameObject up;
        private GameObject down;

        private GameObject jingyu;
        private GameObject xem;
        private GameObject xuetiao;
        private GameObject ke;
        private GameObject ywj2;
        private bool _isOnClick;

        private GameObject success;
        private GameObject caidai;
        Transform jingyuPos;
        private Transform beikepos;
        private Transform kepos;
        private Transform ywjpos;

        private int[] a;
        private int[] a2;
        private int[] a3;
        private GameObject btns;
        //private int lena;
        //int curPageIndex = 0;

        private List<int> _succeedSoundIds;
        private List<int> _failSoundIds;

        GameObject btnSwitch;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            PageBar = curTrans.Find("PageBar").gameObject;
            SpinePage = PageBar.transform.Find("MaskImg/SpinePage");
            SpineShow = curTrans.Find("SpineShow");
            btnBack = curTrans.Find("btnBack").gameObject;
            bf = curTrans.Find("BtnSpines/Bf/bf").gameObject;
            bd = curTrans.Find("mask/BD").gameObject;
            dbd = curTrans.Find("mask/DBD").gameObject;
            mask = curTrans.Find("mask").gameObject;
            BtnSpines = curTrans.Find("BtnSpines").gameObject;
            hudong = curTrans.Find("hudong").gameObject;
            beike = curTrans.Find("hudong/jingyu/beike").gameObject;
            qrj = curTrans.Find("hudong/qrj").gameObject;
            qrj2 = curTrans.Find("hudong/qrj2").gameObject;
            qrj3 = curTrans.Find("hudong/qrj3").gameObject;
            ke = curTrans.Find("hudong/jingyu/ke").gameObject;
            ywj2 = hudong.transform.Find("jingyu/ywj2").gameObject;
            a3 = new int[5];
            a2 = new int[4];
            a = new int[3];
            zz1 = beike.transform.Find("beike1/zz1").gameObject;
            zz2 = beike.transform.Find("beike2/zz2").gameObject;
            zz3 = beike.transform.Find("beike3/zz3").gameObject;
            px1 = ke.transform.Find("ke1/panxieA").gameObject;
            px2 = ke.transform.Find("ke2/panxieB").gameObject;
            px3 = ke.transform.Find("ke3/panxieC").gameObject;
            px4 = ke.transform.Find("ke4/px4").gameObject;
            gz = hudong.transform.Find("jingyu/ywj").gameObject;
            btns = mask.transform.Find("Btns").gameObject;
            btnSwitch = curTrans.Find("btn").gameObject;
          
            success = mask.transform.GetChild(0).gameObject;
            caidai = mask.transform.GetChild(1).gameObject;

            up = curTrans.transform.Find("up").gameObject;
            down = curTrans.transform.Find("down").gameObject;
            jingyu = hudong.transform.Find("jingyu").gameObject;
            xem = hudong.transform.Find("xem").gameObject;
            xuetiao = hudong.transform.Find("xuetiao").gameObject;
            jingyuPos = hudong.transform.Find("jingyuPos");
            beikepos = jingyuPos.transform.Find("beike");
            kepos = jingyuPos.transform.Find("ke");
            ywjpos = jingyuPos.transform.Find("ywj");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            Util.AddBtnClick(bf, btnBfM);
            Empty4Raycast[] e4r = beike.transform.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < e4r.Length; i++)
                Util.AddBtnClick(e4r[i].gameObject, ClickBk);

            Empty4Raycast[] e4r2 = ke.transform.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < e4r2.Length; i++)
                Util.AddBtnClick(e4r2[i].gameObject, ClickK);

            Empty4Raycast[] e4r3 = gz.transform.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < e4r3.Length; i++)
                Util.AddBtnClick(e4r3[i].gameObject, ClickG);

            Util.AddBtnClick(qrj.transform.GetChild(0).gameObject, ClickQrj);
            Util.AddBtnClick(qrj2.transform.GetChild(0).gameObject, ClickQrj2);
            Util.AddBtnClick(qrj3.transform.GetChild(0).gameObject, ClickQrj3);
            Util.AddBtnClick(btns.transform.GetChild(0).gameObject, clickFH);
            Util.AddBtnClick(btns.transform.GetChild(1).gameObject, clickOK);


            GameInit();
            GameStart();
        }
        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }

        private void GameInit()
        {

            _succeedSoundIds = new List<int> { 4, 5, 6, 7, 8, 9 };
            _failSoundIds = new List<int> { 0, 1, 2, 3 };

            _isOnClick = false;
            Init1();

            Init2();
            hudong.SetActive(true);
            for (int i = 0; i < xuetiao.transform.childCount; i++)
            {
                xuetiao.transform.GetChild(i).gameObject.SetActive((true));
            }
            InitSpine(jingyu, "jingyu");
            InitSpine(xem, "xem1");
            //InitGame(jingyu.transform.GetChild(0), -1);
            //level = 0;
        }


        void Init1()
        {
            hudong.SetActive(false);
            dbd.SetActive(false);
            mask.SetActive(true);
            BtnSpines.SetActive(true);
            btns.SetActive(false);

            bf.transform.parent.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(bf.transform.parent.gameObject, "bf2", false);
            qrj.SetActive(false);
            qrj2.SetActive(false);
            qrj3.SetActive(false);

            beike.SetActive(false);
            ke.SetActive(false);
            gz.SetActive(false);
            ywj2.SetActive(false);
            success.SetActive(false);
            caidai.SetActive(false);
        }
        void Init2()
        {
            talkIndex = 1;
            bfIndex = 1;
            _canClick = false;
            isPlaying = true;
            isCorrect = false;
            time = 0;
            isPlaying = false;
            px1.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px2.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px3.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px4.transform.GetComponent<SkeletonGraphic>().Initialize(true);

        }
        void Init3()
        {
            hudong.SetActive(true);
            dbd.SetActive(false);
            mask.SetActive(false); ;
            BtnSpines.SetActive(false);
            btns.SetActive(false);
            bf.transform.parent.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(bf.transform.parent.gameObject, "bf2", false);
            qrj.SetActive(false);
            qrj2.SetActive(false);
            qrj3.SetActive(false);
            ywj2.SetActive(false);
            beike.SetActive(false);
            ke.SetActive(false);
            gz.SetActive(false);
            success.SetActive(false);
            caidai.SetActive(false);


            talkIndex = 1;
            bfIndex = 2;
            _canClick = false;
            isPlaying = true;
            isCorrect = false;
            time = 0;
            isPlaying = false;

            dbd.SetActive(false);
            mask.SetActive(false);
            hudong.SetActive(true);
            for (int i = 0; i < xuetiao.transform.childCount; i++)
            {
                xuetiao.transform.GetChild(i).gameObject.SetActive((true));
            }
            beike.SetActive(true);
            SpineManager.instance.DoAnimation(xem, "xem1", true);
            ShowAndSwitch();
            px1.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px2.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px3.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            px4.transform.GetComponent<SkeletonGraphic>().Initialize(true);
            for (int i = 0; i < xuetiao.transform.childCount; i++)
            {
                xuetiao.transform.GetChild(i).gameObject.SetActive((true));
            }
            InitSpine(jingyu, "jingyu");
            InitSpine(xem, "xem1");
            // InitGame(beike, 0);

        }


        void GameStart()
        {
            
            mask.SetActive(true);
            BtnSpines.SetActive(true);
            

        }

        void btnBfM(GameObject obj)
        {
            BtnPlaySound();
          
            switch (bfIndex)
            {
                case 1:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "bf", false, () =>
                    {
                        BtnSpines.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 0, null, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                    });
                    break;
                case 2:
                    beike.SetActive(false);
                    ke.SetActive(true);
                    BtnSpines.SetActive(false);
                    ShowK();
                    break;
                case 3:
                    ke.SetActive(false);
                    gz.SetActive(true);
                    BtnSpines.SetActive(false);
                    ShowG();

                    break;

                default:
                    break;
            }
            bfIndex++;

        }

        #region 贝壳初始化
        #region 随机数组
        int[] randVector(int len)
        {
            int[] num = new int[len];
            int[] newNum = new int[len];
            int i, r = len - 1;
            int n;
            int tmp;
            System.Random rand = new System.Random();
            for (i = 0; i < len; i++)
            {
                num[i] = i;
            }

            for (i = 0; i < len; i++)
            {
                n = rand.Next(0, r);
                newNum[i] = num[n];
                tmp = num[n];
                num[n] = num[r];
                num[r] = tmp;
                r--;


            }
            return newNum;

        }
        #endregion
        void ShowAndSwitch()
        {
            _canClick = false;
            isPlaying = true;
            //lena = 3;
            a = randVector(3);
            for (int i = 0; i < 3; i++)
            {
                beike.transform.GetChild(a[i]).transform.position = beikepos.GetChild(i).position;
            }
            for (int i = 0; i < 3; i++)
            {
                GameObject obj = beike.transform.Find("beike" + (i + 1)).gameObject;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(obj, "beike3", false);
                zz1.SetActive(true);
                zz2.SetActive(true);
                zz3.SetActive(true);
                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(obj, "beike4", false, () =>
                    {

                        zz1.SetActive(false);
                        zz2.SetActive(false);
                        zz3.SetActive(false);
                        _canClick = true;
                        isPlaying = false;
                    });
                }, 3));
            }



        }

        #endregion

        #region 贝壳点击
        bool _canClick;
        int time;
        bool isPlaying;

        //Vector3 temp;
        GameObject obj1;
        GameObject obj2;

        void ClickBk(GameObject obj)
        {


            if (!_canClick && isPlaying)
                return;


            time++;
            
            switch (time)
            {
                case 1:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "beike2", false);
                    obj1 = obj.transform.parent.gameObject;
                    break;
                case 2:
                    _canClick = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    if (obj.name == obj1.name)
                    {
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "beike", false,()=> { time--;_canClick = true; });
                        
                        return;
                    }

                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "beike2", false);
                    obj2 = obj.transform.parent.gameObject;

                    time = 0;
                    if (!isPlaying)
                    {
                        isPlaying = true;
                        obj2.transform.DOMove(obj1.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj2, "beike", false);
                        });
                        obj1.transform.DOMove(obj2.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj1, "beike", false);
                            isPlaying = false;
                            qrj.SetActive(true);
                            _canClick = true;

                        });
                    }
                    else
                    {

                        return;
                    }

                    break;


            }


        }

        #endregion
        #region 确认键
        bool isCorrect;

        void ClickQrj(GameObject obj)
        {
            if (!_canClick)
                return;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + 2, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false);

            });
            _canClick = false;
            isPlaying = true;
            for (int i = 0; i < 3; i++)
            {
                GameObject obj2 = beike.transform.Find("beike" + (i + 1)).gameObject;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(obj2, "beike3", false, () =>
                {
                    zz1.SetActive(true);
                    zz2.SetActive(true);
                    zz3.SetActive(true);
                });
            }


            for (int i = 0; i < 3; i++)
            {

                if (beike.transform.GetChild(i).transform.position == beikepos.GetChild(i).position)
                {
                    isCorrect = true;
                }
                else
                {
                    isCorrect = false;
                    break;
                }
            }
            //成功
            if (isCorrect)
            {
                PlaySuccessSound();

                jingyu.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(jingyu, "jingyu2", false, () =>
                {
                    SpineManager.instance.DoAnimation(jingyu, "jingyu", true);
                });
                SpineManager.instance.DoAnimation(xem, "xem2", false, () =>
                {
                    SpineManager.instance.DoAnimation(xem, "xem1", true);
                });
                xuetiao.transform.GetChild(2).gameObject.SetActive(false);
                qrj.SetActive(false);


                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    BtnSpines.SetActive(true);
                    SpineManager.instance.DoAnimation(bf.transform.parent.gameObject, "next2", false);
                }, 2));

            }
            //错误
            else
            {
                PlayFailSound();

                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    for (int i = 0; i < 3; i++)
                    {

                        GameObject obj2 = beike.transform.Find("beike" + (i + 1)).gameObject;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        SpineManager.instance.DoAnimation(obj2, "beike4", false, () =>
                        {
                            zz1.SetActive(false);
                            zz2.SetActive(false);
                            zz3.SetActive(false);
                            _canClick = true;
                            isPlaying = false;

                        });
                    }
                }, 3));

                qrj.SetActive(false);
                


            }

        }

        #endregion






        #region 壳初始化

        void ShowK()
        {
            _canClick = false;
            isPlaying = true;
            a2 = randVector(4);
            for (int i = 0; i < 4; i++)
            {
                ke.transform.GetChild(a2[i]).transform.position = kepos.GetChild(i).position;
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            for (int i = 0; i < 4; i++)
            {
                int j = i;
                GameObject obj = ke.transform.Find("ke" + (i + 1)).gameObject;
                
                SpineManager.instance.DoAnimation(obj, "ke3", false,()=> {
                    Debug.Log(ke.transform.GetChild(j));
                    ke.transform.GetChild(j).GetChild(0).gameObject.SetActive(true);
                    ke.transform.GetChild(j).GetChild(0).transform.GetComponent<SkeletonGraphic>().Initialize(true);

                    //px1.SetActive(true);
                    //px2.SetActive(true);
                    //px3.SetActive(true);
                    //px4.SetActive(true);
                    //px1.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                    //px2.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                    //px3.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                    //px4.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(px1, "panxieA2", false);
                    SpineManager.instance.DoAnimation(px2, "panxieB2", false);
                    SpineManager.instance.DoAnimation(px3, "panxieC2", false);
                    SpineManager.instance.DoAnimation(px4, "panxieD2", false);



                });


                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(px1, "panxieA3", false, () =>
                        {
                            px1.SetActive(false);
                            
                            SpineManager.instance.DoAnimation(ke.transform.GetChild(0).gameObject, "ke4", false);
                            _canClick = true;
                            isPlaying = false;
                        });
                        SpineManager.instance.DoAnimation(px2, "panxieB3", false, () => { px2.SetActive(false);
                            SpineManager.instance.DoAnimation(ke.transform.GetChild(1).gameObject, "ke4", false);
                            //SpineManager.instance.DoAnimation(obj, "ke4", false); 
                        });
                        SpineManager.instance.DoAnimation(px3, "panxieC3", false, () => { px3.SetActive(false);
                            SpineManager.instance.DoAnimation(ke.transform.GetChild(2).gameObject, "ke4", false);
                            // SpineManager.instance.DoAnimation(obj, "ke4", false);
                        });
                        SpineManager.instance.DoAnimation(px4, "panxieD3", false, () => {
                            px4.SetActive(false);
                            SpineManager.instance.DoAnimation(ke.transform.GetChild(3).gameObject, "ke4", false);
                            // SpineManager.instance.DoAnimation(obj, "ke4", false);
                        });

                       

                    
                }, 3));
            }

        }

        #endregion

        #region 壳点击



        void ClickK(GameObject obj)
        {

            if (!_canClick && isPlaying)
                return;
            time++;

            switch (time)
            {
                case 1:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ke2", false);
                    obj1 = obj.transform.parent.gameObject;
                    break;
                case 2:
                    _canClick = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    if (obj.name == obj1.name)
                    {
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ke", false,()=> { time--; _canClick = true; });
                       
                        return;
                    }
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ke2", false);
                    obj2 = obj.transform.parent.gameObject;
                    time = 0;
                    if (!isPlaying)
                    {
                        isPlaying = true;
                        obj2.transform.DOMove(obj1.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj2, "ke1", false);
                            time--; 
                            _canClick = true;
                        });
                        obj1.transform.DOMove(obj2.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj1, "ke1", false);
                            isPlaying = false;
                            qrj2.SetActive(true);
                            _canClick = true;

                        });
                    }
                    else
                    {

                        return;
                    }

                    break;


            }



        }

        #endregion
        #region 确认键2

        void ClickQrj2(GameObject obj)
        {
            if (!_canClick)
                return;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + 2, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false);

            });
            _canClick = false;
            isPlaying = true;
            for (int i = 0; i < 4; i++)
            {
                GameObject obj2 = ke.transform.Find("ke" + (i + 1)).gameObject;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(obj2, "ke3", false, () =>
                {
                    px1.SetActive(true);
                    px2.SetActive(true);
                    px3.SetActive(true);
                    px4.SetActive(true);
                    SpineManager.instance.DoAnimation(px1, "panxieA2", false);
                    SpineManager.instance.DoAnimation(px2, "panxieB2", false);
                    SpineManager.instance.DoAnimation(px3, "panxieC2", false);
                    SpineManager.instance.DoAnimation(px4, "panxieD2", false);
                });
            }


            for (int i = 0; i < 4; i++)
            {

                if (ke.transform.GetChild(i).transform.position == kepos.GetChild(i).position)
                {
                    isCorrect = true;
                }
                else
                {
                    isCorrect = false;
                    break;
                }
            }
            //成功
            if (isCorrect)
            {

                PlaySuccessSound();

                jingyu.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(jingyu, "jingyu2", false, () =>
                {
                    SpineManager.instance.DoAnimation(jingyu, "jingyu", true);
                });
                SpineManager.instance.DoAnimation(xem, "xem2", false, () =>
                {
                    SpineManager.instance.DoAnimation(xem, "xem1", true);
                });
                xuetiao.transform.GetChild(1).gameObject.SetActive(false);
                qrj2.SetActive(false);


                mono.StartCoroutine(WaitCoroutine(() => { BtnSpines.SetActive(true); SpineManager.instance.DoAnimation(bf.transform.parent.gameObject, "next2", false); }, 2));
            }
            //错误
            else
            {
                PlayFailSound();
                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    for (int i = 0; i < 4; i++)
                    {

                        GameObject obj2 = ke.transform.Find("ke" + (i + 1)).gameObject;

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3,false);
                        SpineManager.instance.DoAnimation(px1, "panxieA3", false,()=> { px1.SetActive(false);
                                SpineManager.instance.DoAnimation(ke.transform.GetChild(0).gameObject, "ke4", false);
                            });
                            SpineManager.instance.DoAnimation(px2, "panxieB3", false, () => { px2.SetActive(false);
                                SpineManager.instance.DoAnimation(ke.transform.GetChild(1).gameObject, "ke4", false); });
                            SpineManager.instance.DoAnimation(px3, "panxieC3", false, () => { px3.SetActive(false);
                                SpineManager.instance.DoAnimation(ke.transform.GetChild(2).gameObject, "ke4", false);
                            });
                            SpineManager.instance.DoAnimation(px4, "panxieD3", false, () => { px4.SetActive(false);
                                SpineManager.instance.DoAnimation(ke.transform.GetChild(3).gameObject, "ke4", false);
                            });
                           
                        
                            _canClick = true;
                            isPlaying = false;

                        
                    }
                }, 3));

                qrj2.SetActive(false);


            }

        }

        #endregion


        #region 罐子初始化

        void ShowG()
        {
            _canClick = false;
            isPlaying = true;
            a3 = randVector(5);
            ywj2.SetActive(true);
            for (int i = 0; i < 5; i++)
            {
                gz.transform.GetChild(a3[i]).transform.position = ywjpos.GetChild(i).position;
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SmUp();
            mono.StartCoroutine(WaitCoroutine(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SmDown(() =>
                {
                    _canClick = true;
                    isPlaying = false;
                    ywj2.SetActive(false);
                });

            }, 3));




        }

        #endregion

        #region 罐子点击



        void ClickG(GameObject obj)
        {

            if (!_canClick && isPlaying)
                return;
            time++;
            switch (time)
            {
                case 1:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ywj2", false);
                    obj1 = obj.transform.parent.gameObject;
                    break;
                case 2:
                    _canClick = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    if (obj.name == obj1.name)
                    {
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ywj", false,()=> { _canClick = true;time--; });
                        return;
                    }

                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "ywj2", false);
                    obj2 = obj.transform.parent.gameObject;
                    time = 0;
                    if (!isPlaying)
                    {
                        isPlaying = true;
                        obj2.transform.DOMove(obj1.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj2, "ywj", false);
                        });
                        obj1.transform.DOMove(obj2.transform.position, 1f).SetEase(Ease.InSine).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(obj1, "ywj", false);
                            isPlaying = false;
                            qrj3.SetActive(true);
                            _canClick = true;

                        });
                    }
                    else
                    {

                        return;
                    }

                    break;


            }



        }

        #endregion
        #region 确认键3

        void ClickQrj3(GameObject obj)
        {
            if (!_canClick)
                return;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);

            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + 2, false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false);

            });
            ywj2.SetActive(true);
            _canClick = false;
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SmUp();



            for (int i = 0; i < 5; i++)
            {

                if (gz.transform.GetChild(i).transform.position == ywjpos.GetChild(i).position)
                {
                    isCorrect = true;
                }
                else
                {
                    isCorrect = false;
                    break;
                }
            }
            //成功
            if (isCorrect)
            {
                PlaySuccessSound();

                jingyu.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                SpineManager.instance.DoAnimation(jingyu, "jingyu2", false, () =>
                {
                    SpineManager.instance.DoAnimation(jingyu, "jingyu", true);
                });
                SpineManager.instance.DoAnimation(xem, "xem3", false);
                xuetiao.transform.GetChild(0).gameObject.SetActive(false);
                qrj3.SetActive(false);


                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    mask.SetActive(true);
                    success.SetActive(true);
                    caidai.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                    SpineManager.instance.DoAnimation(success, "6-12-z", false, () =>
                    {
                        success.transform.GetComponent<SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(success, "6-12-z2", true);
                    });

                    SpineManager.instance.DoAnimation(caidai, "sp", false);
                }, 3));


                mono.StartCoroutine(WaitCoroutine(() =>
                {

                    success.SetActive(false);
                    caidai.SetActive(false);
                    btns.SetActive(true);
                    btns.transform.GetChild(0).gameObject.SetActive(true);
                    btns.transform.GetChild(1).gameObject.SetActive(true);

                    SpineManager.instance.DoAnimation(btns.transform.GetChild(0).gameObject, "fh2", false);
                    SpineManager.instance.DoAnimation(btns.transform.GetChild(1).gameObject, "ok2", false);


                }, 5));


                //BtnSpines.SetActive(true);
                //SpineManager.instance.DoAnimation(bf.transform.parent.gameObject, "next2", false);
            }
            //错误
            else
            {
                PlayFailSound();

                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SmDown(() =>
                    {
                        _canClick = true;
                        isPlaying = false;
                        ywj2.SetActive(false);
                    });

                }, 3));

                qrj3.SetActive(false);




            }

        }


        void clickFH(GameObject obj)
        {

            if (_isOnClick)           
                return;
            
            _isOnClick = true;

            BtnPlaySound();
            SpineManager.instance.DoAnimation(btns.transform.GetChild(0).gameObject, "fh", false,()=> {
                _isOnClick = false;
                Init3();
            });
           
            /* bfIndex = 2;
             btnBfM(bf);*/
            //GameInit();
        }
        void clickOK(GameObject obj)
        {
            if (_isOnClick)           
                return;
            
            _isOnClick = true;

            BtnPlaySound();
            SpineManager.instance.DoAnimation(btns.transform.GetChild(1).gameObject, "ok", false,()=> {
                btns.gameObject.Hide();
                dbd.gameObject.Show();
                mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 2));
            });
           
        }


        private float PlayCommonSound(int index, bool isLoop = false)
        {
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, index, isLoop);
            return time;
        }

        private float PlayFailSound()
        {
            PlayCommonSound(5);

            var index = Random.Range(0, _failSoundIds.Count);
            var id = _failSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }

        private float PlaySuccessSound()
        {
            PlayCommonSound(4);
            var index = Random.Range(0, _succeedSoundIds.Count);
            var id = _succeedSoundIds[index];
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, id, false);
            return time;
        }


        #endregion


        #region  水母移动
        void SmUp()
        {
  
            for (int i = 0; i < 5; i++)
            {
                GameObject obj;
                obj = gz.transform.GetChild(i).GetChild(0).gameObject;
                obj.SetActive(true);
                obj.transform.DOMoveY(up.transform.position.y, 1f).SetEase(Ease.InSine);
            }


        }
        void SmDown(Action callback)
        {
            for (int i = 0; i < 5; i++)
            {

                GameObject obj = gz.transform.GetChild(i).GetChild(0).gameObject;


                obj.transform.DOMoveY(down.transform.position.y, 1f).SetEase(Ease.InSine).OnComplete(() =>
                {
                    obj.SetActive(false);
                    callback?.Invoke();
                    
                });
            }
           

        }


        #endregion


        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                    mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 1, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(false);
                    },
                    () =>
                    {
                        dbd.SetActive(false);
                        mask.SetActive(false);
                        hudong.SetActive(true);
                        for (int i = 0; i < xuetiao.transform.childCount; i++)
                        {
                            xuetiao.transform.GetChild(i).gameObject.SetActive((true));
                        }
                        //InitGame(jingyu.transform.GetChild(level), level);
                        beike.SetActive(true);
                        SpineManager.instance.DoAnimation(xem, "xem1", true);
                        ShowAndSwitch();
                    }));
                    break;
                case 2:

                    break;
                case 3:

                    break;
            }


            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
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
        IEnumerator WaitCoroutine(Action method_2 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }

        //int level = 0;
        //private void OnClickSwitch(GameObject obj) 
        //{
        //    InitGame(jingyu.transform.GetChild(level), level);
        //    level++;
        //}
        /// <summary>
        /// 关卡初始化(重构）
        /// </summary>
        /// <param name="tran">鲸鱼子物体</param>
        /// <param name="level"></param>
        /// 
       /* private void InitGame(Transform tran, int level)
        {

            for (int i = 0; i < jingyu.transform.childCount; i++)
            {
                for (int j = 0; j < jingyu.transform.GetChild(i).childCount; j++)
                {
                    SpineManager.instance.DoAnimation(jingyu.transform.GetChild(i).GetChild(j).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(jingyu.transform.GetChild(i).GetChild(j).GetChild(0).gameObject, "kong", false);               
                }
              
            }

          *//*  if (level < 0)
                return;*//*
          
            for (int i = 0; i < tran.childCount; i++)
            {
                
                Transform temTran = tran.GetChild(i);
                int temIndex = i;
                SpineManager.instance.DoAnimation(temTran.gameObject, tran.name + (level == 2 ? "" : "3"), false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(temTran.GetChild(0).gameObject, temTran.GetChild(0).name+(level == 1 ? "2" : ""), false);
                        
                    });
            }
        }*/
    }
}
