using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course732Part2
    {

        public enum gameLevelEnum
        {
            one,//左转
            two,//前行
            three//右转
        }
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject _spine0;
        private GameObject _car;
        private GameObject _leftWheel;
        private GameObject _rightWheel;
        private GameObject _red;
        private GameObject _red_0;
        private GameObject _red_1;

        private Transform _ilDragerParent;
        private Transform _ilDroperParent;
        private ILDrager[] _ilDragers;
        private ILDroper[] _iLDropers;

        private Transform _ilDragerLStartPos;
        private Transform _ilDragerRStartPos;
        

        private Transform _clickBtn;
        private PolygonCollider2D[] _clickPo;       
              
        private int droperIndex;       

        private Transform _sliderPanel;

        private GameObject _levelTwo;
        private Image _A4;
        private Image _A5;
        private Image _A6;
        private Image _A7;
        private Transform _A4StartPos;
        private Transform _A5StartPos;
        private Transform _A4EndPos;
        private Transform _A5EndPos;

        private string rotateNameL;
        private string rotateNameR;
        private gameLevelEnum _gameLevelEnum = gameLevelEnum.one;

        private bool isShowCenterClick;
        private int isShowClick;

        private GameObject _sliderAreaL;
        private GameObject _sliderAreaR;
        private Transform _sliderPos;
        private Transform[] _sliderPoss;      

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;
            rotateNameL = string.Empty;
            rotateNameR = string.Empty;
            droperIndex = -1;
            isShowClick = 0;
            isShowCenterClick = true;

            _gameLevelEnum = gameLevelEnum.one;

            _spine0 = curTrans.Find("spineManager/0").gameObject;
            _car = curTrans.Find("spineManager/car").gameObject;

            _leftWheel = curTrans.Find("spineManager/leftWheel").gameObject;
            _leftWheel.Show();
            _leftWheel.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.SetTimeScale(_leftWheel, 0.5f);           
            SpineManager.instance.DoAnimation(_leftWheel, "zuol2", true);            

            _rightWheel = curTrans.Find("spineManager/rightwheel").gameObject;
            _rightWheel.Show();
            _rightWheel.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.SetTimeScale(_rightWheel, 0.5f);
            SpineManager.instance.DoAnimation(_rightWheel, "youl2", true);          

            _red = curTrans.Find("spineManager/red").gameObject;
            SpineManager.instance.DoAnimation(_red, "kong", false);
            _red.Hide();

            _red_0 = curTrans.Find("spineManager/red_0").gameObject;
            SpineManager.instance.DoAnimation(_red_0, "kong", false);
            _red_0.Hide();

            _red_1 = curTrans.Find("spineManager/red_1").gameObject;
            SpineManager.instance.DoAnimation(_red_1, "kong", false);
            _red_1.Hide();

            _sliderPanel = curTrans.Find("sliderPanel");
            _sliderPanel.gameObject.Show();


            _sliderAreaL = curTrans.Find("sliderPanel/sliderAreaL").gameObject;
            _sliderAreaR = curTrans.Find("sliderPanel/sliderAreaR").gameObject;

            _sliderPos = curTrans.Find("sliderPanel/sliderPos/");
            _sliderPoss = new Transform[_sliderPos.childCount];
            for (int i = 0; i < _sliderPoss.Length; i++)
            {
                _sliderPoss[i] = _sliderPos.GetChild(i);
            }
           

            _ilDragerLStartPos = curTrans.Find("sliderPanel/drager/dragerLStartPos");
            _ilDragerRStartPos = curTrans.Find("sliderPanel/drager/dragerRStartPos");

            _ilDragerParent = curTrans.Find("sliderPanel/drager");
            _ilDragers = _ilDragerParent.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < _ilDragers.Length; i++)
            {
                _ilDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                _ilDragers[i].isActived = false;
            }
            _ilDragers[0].transform.position = _ilDragerLStartPos.position;
            _ilDragers[1].transform.position = _ilDragerRStartPos.position;           

            _ilDragers[0].gameObject.Hide();
            _ilDragers[1].gameObject.Hide();

            _ilDroperParent = curTrans.Find("sliderPanel/droper");
            _iLDropers = _ilDroperParent.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < _iLDropers.Length; i++)
            {
                _iLDropers[i].index = i;
                _iLDropers[i].SetDropCallBack(OnAfter);
            }
            

            _clickBtn = curTrans.Find("sliderPanel/clickBtn");
            _clickPo = _clickBtn.GetComponentsInChildren<PolygonCollider2D>(true);
            for (int i = 0; i < _clickPo.Length; i++)
            {
                Util.AddBtnClick(_clickPo[i].gameObject, PoClickBtnEvent);
            }
            ShowOrHideClickBtn(_clickPo, false);

            _levelTwo = curTrans.Find("levelTwo").gameObject;
            _A4 = curTrans.Find("levelTwo/A4").GetComponent<Image>();
            _A5 = curTrans.Find("levelTwo/A5").GetComponent<Image>();
            _A6 = curTrans.Find("levelTwo/A6").GetComponent<Image>();
            _A7 = curTrans.Find("levelTwo/A7").GetComponent<Image>();


            _A6.sprite = _A6.GetComponent<BellSprites>().sprites[0];
           

            _A4StartPos = curTrans.Find("levelTwo/A4StartPos");
            _A5StartPos = curTrans.Find("levelTwo/A5StartPos");

            _A4EndPos = curTrans.Find("levelTwo/A4EndPos");
            _A5EndPos = curTrans.Find("levelTwo/A5EndPos");

            _A4.gameObject.Show();
            _A5.gameObject.Show();
            _A6.gameObject.Hide();
            _A7.gameObject.Hide();

            _A4.transform.position = _A4StartPos.position;
            _A5.transform.position = _A5StartPos.position;
            _levelTwo.Hide();

            _spine0.Show();
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine0, "mianban", false);
   
            _car.Show();
            _car.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            _car.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_car, "che", false);

        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                //Max.SetActive(false);
                isPlaying = false;
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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        IEnumerator WaitCoroutine( Action method_2 = null, float len = 0)
        {            
            yield return new WaitForSeconds(len);           
            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                Max.SetActive(false);
                _ilDragers[0].gameObject.Show();
                _ilDragers[1].gameObject.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {                                                                              
                    IsCanDrager(_ilDragers, true);
                    _clickPo[0].gameObject.Show();
                }));
                mono.StartCoroutine(WaitCoroutine(() => 
                {                                       
                    SpineManager.instance.SetTimeScale(_leftWheel, 0.5f);
                    SpineManager.instance.SetTimeScale(_rightWheel, 0.5f);
                    SpineManager.instance.DoAnimation(_leftWheel, "zuol", false);
                    SpineManager.instance.DoAnimation(_rightWheel, "youl", false, () =>
                    {
                        SpineManager.instance.SetTimeScale(_leftWheel, 0.5f);
                        SpineManager.instance.SetTimeScale(_rightWheel, 0.5f);
                        SpineManager.instance.DoAnimation(_leftWheel, "zuol2", true);
                        SpineManager.instance.DoAnimation(_rightWheel, "youl2", true);
                        mono.StartCoroutine(WaitCoroutine(() =>
                        {
                            _red.Show();                          
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);                            
                            SpineManager.instance.DoAnimation(_red, "hong", false, () =>
                            {
                                _red.Hide();                                                               
                            });
                        }, 12.0f));
                    });

                }, 3.5f));               

            }
            else if (talkIndex == 2)
            {
                IsCanDrager(_ilDragers, false);
                ShowOrHideClickBtn(_clickPo, false);
                mono.StartCoroutine(WaitCoroutine(() =>
                {                   
                    _red_0.Show();                    
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);                                      
                    SpineManager.instance.DoAnimation(_red_0, "hong3", false, () =>
                    {                     
                        _red_0.Hide();                                                     
                    });
                }, 2.5f));
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 8, null, () =>
                {
                    _clickPo[0].gameObject.Show();
                    IsCanDrager(_ilDragers, true);
                    _gameLevelEnum = gameLevelEnum.two;
                }));
            }
            else if (talkIndex == 3)
            {
                IsCanDrager(_ilDragers, false);
                ShowOrHideClickBtn(_clickPo, false);
                mono.StartCoroutine(WaitCoroutine(() =>
                {
                    _red_1.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                    SpineManager.instance.DoAnimation(_red_1, "hong2", false, () =>
                    {
                        _red_1.Hide();
                    });
                }, 2.5f));
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 9, null, () =>
                {
                    _clickPo[0].gameObject.Show();
                    IsCanDrager(_ilDragers, true);
                    _gameLevelEnum = gameLevelEnum.three;
                }));
            }
            else if (talkIndex == 4)
            {
                _sliderPanel.gameObject.Hide();
                _spine0.Hide();
                _leftWheel.Hide();
                _rightWheel.Hide();
                _red.Hide();
                _car.Hide();
               
                Max.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 11, null,()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));

                _levelTwo.Show();
                _A4.transform.DOMove(_A4EndPos.position, 1.0f).SetEase(Ease.Flash);
                _A5.transform.DOMove(_A5EndPos.position, 1.0f).SetEase(Ease.Flash).OnComplete(() => 
                {
                    mono.StartCoroutine(WaitCoroutine(() => 
                    {
                        _A6.gameObject.Show();                       
                        _A6.sprite = _A6.GetComponent<BellSprites>().sprites[0]; //序号1
                        _A6.SetNativeSize();

                        mono.StartCoroutine(WaitCoroutine(() =>
                        {
                            _A6.gameObject.Hide();
                            _A7.gameObject.Show();

                            mono.StartCoroutine(WaitCoroutine(() =>
                            {
                                _A6.gameObject.Show();
                                _A6.sprite = _A6.GetComponent<BellSprites>().sprites[1];//序号3
                                _A6.SetNativeSize();

                                _A7.gameObject.Hide();

                                mono.StartCoroutine(WaitCoroutine(() =>
                                {
                                    _A6.gameObject.Show();
                                    _A6.sprite = _A6.GetComponent<BellSprites>().sprites[2];//序号4
                                    _A6.SetNativeSize();

                                    //SoundManager.instance.ShowVoiceBtn(true);
                                }, 2.0f));

                            }, 2.0f));
                        }, 1.0f));
                    }, 3.0f));
                });               
            }
            else if (talkIndex == 5)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 12, null, () =>
                {
                    //SoundManager.instance.ShowVoiceBtn(true); 
                }));
            }
            talkIndex++;
        }        
        private void PoClickBtnEvent(GameObject obj)
        {
            ShowOrHideClickBtn(_clickPo, false);
            IsCanDrager(_ilDragers, false);
            SoundManager.instance.ShowVoiceBtn(false);

            //Debug.LogError("rotateNameL:" + rotateNameL + "=======rotateNameR:" + rotateNameR);

            if(_gameLevelEnum == gameLevelEnum.two) //左转
            {
                if ((rotateNameL == string.Empty && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == string.Empty))
                {                   
                    _car.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                    SpineManager.instance.DoAnimation(_car, "che9", false,()=> 
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, () =>
                        {                            
                            _clickPo[0].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                            SoundManager.instance.ShowVoiceBtn(true);                           
                        }));
                    });                   
                }
                else if ((rotateNameL == string.Empty && rotateNameR == "R-0") || (rotateNameL == "L-1" && rotateNameR == "R-0") || (rotateNameL == "L-2" && rotateNameR == "R-1")|| (rotateNameL == "L-2" && rotateNameR == string.Empty))//左转
                {                   
                    _car.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                    SpineManager.instance.DoAnimation(_car, "che10", false,()=> 
                    {                       
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, () =>
                        {
                            _clickPo[0].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    });
                   
                }               
                else if ((rotateNameL == "L-2" && rotateNameR == "R-0") || (rotateNameL == "L-0" && rotateNameR == "R-2"))//危险
                {
                    _car.Show();
                    SpineManager.instance.DoAnimation(_car, "che", false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                    {
                        _clickPo[0].gameObject.Show();
                        IsCanDrager(_ilDragers, true);
                    }));
                }
                else if ((rotateNameL == string.Empty && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == "R-2") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-0" && rotateNameR == "R-0"))//前进
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, () =>
                    {
                        _clickPo[0].gameObject.Show();
                        IsCanDrager(_ilDragers, true);
                    }));
                }
            }
            else if(_gameLevelEnum == gameLevelEnum.one)//前行
            {
                if (obj.name == "2")
                {
                    RotateCar();
                }
                else if (obj.name == "3")
                {
                    if ((rotateNameL == string.Empty && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == string.Empty))//右转
                    {                        
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "che8", false, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 13, null, () =>
                            {
                                isShowCenterClick = true;
                                _clickPo[0].gameObject.Show();
                                IsCanDrager(_ilDragers, true);
                                SoundManager.instance.ShowVoiceBtn(true);                              
                            }));
                        });
                    }
                    else if ((rotateNameL == string.Empty && rotateNameR == "R-0") || (rotateNameL == "L-1" && rotateNameR == "R-0") || (rotateNameL == "L-2" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == string.Empty))//左转
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5, null, () =>
                        {
                            _clickPo[1].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                    else if ((rotateNameL == "L-2" && rotateNameR == "R-0") || (rotateNameL == "L-0" && rotateNameR == "R-2"))//危险
                    {                        
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                        {
                            _clickPo[1].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                    else if ((rotateNameL == string.Empty && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == "R-2") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-0" && rotateNameR == "R-0"))//前进
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
                        {
                            _clickPo[1].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                }
                else if (obj.name == "4")
                {
                    if ((rotateNameL == string.Empty && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == string.Empty))//右转
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
                        {
                            _clickPo[2].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                    else if ((rotateNameL == string.Empty && rotateNameR == "R-0") || (rotateNameL == "L-1" && rotateNameR == "R-0") || (rotateNameL == "L-2" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == string.Empty))//左转
                    {
                     
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "che7", false, () =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 13, null, () =>
                            {
                                isShowCenterClick = true;
                                _clickPo[0].gameObject.Show();
                                IsCanDrager(_ilDragers, true);
                                SoundManager.instance.ShowVoiceBtn(true);                                
                            }));
                        });                      
                    }
                    else if ((rotateNameL == "L-2" && rotateNameR == "R-0") || (rotateNameL == "L-0" && rotateNameR == "R-2"))//危险
                    {                     
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                        {
                            _clickPo[2].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                    else if ((rotateNameL == string.Empty && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == "R-2") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-0" && rotateNameR == "R-0"))//前进
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
                        {
                            _clickPo[2].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    }
                }        
            }
            else if (_gameLevelEnum == gameLevelEnum.three)//右转
            {
                if ((rotateNameL == string.Empty && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == string.Empty))//右转
                {                  
                    _car.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                    SpineManager.instance.DoAnimation(_car, "che9", false, () =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
                        {
                            _clickPo[0].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                        }));
                    });
                }
                else if ((rotateNameL == string.Empty && rotateNameR == "R-0") || (rotateNameL == "L-1" && rotateNameR == "R-0") || (rotateNameL == "L-2" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == string.Empty))//左转
                {                 
                    _car.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                    SpineManager.instance.DoAnimation(_car, "che10", false, () =>
                    {                      
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 10, null, () =>
                        {                           
                            _clickPo[0].gameObject.Show();
                            IsCanDrager(_ilDragers, true);
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                    });
                }
                else if ((rotateNameL == "L-2" && rotateNameR == "R-0") || (rotateNameL == "L-0" && rotateNameR == "R-2"))//危险
                {
                    _car.Show();
                    SpineManager.instance.DoAnimation(_car, "che", false);
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                    {
                        _clickPo[0].gameObject.Show();
                        IsCanDrager(_ilDragers, true);
                    }));
                }
                else if ((rotateNameL == string.Empty && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == "R-2") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-0" && rotateNameR == "R-0"))//前进
                {
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null, () =>
                    {
                        _clickPo[0].gameObject.Show();
                        IsCanDrager(_ilDragers, true);
                    }));
                }
            }            

        }

        private void RotateCar()
        {
            if ((rotateNameL == string.Empty && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-2") || (rotateNameL == "L-0" && rotateNameR == string.Empty))//左转
            {              
                _car.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                SpineManager.instance.DoAnimation(_car, "che2", false, () =>
                {
                    isShowCenterClick = false;
                    isShowClick = 1;
                    _clickPo[1].gameObject.Show();
                    IsCanDrager(_ilDragers, true);                    
                });
            }
            else if ((rotateNameL == string.Empty && rotateNameR == "R-0") || (rotateNameL == "L-1" && rotateNameR == "R-0") || (rotateNameL == "L-2" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == string.Empty))//右转
            {               
                _car.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                SpineManager.instance.DoAnimation(_car, "che3", false, () =>
                {
                    isShowCenterClick = false;
                    isShowClick = 2;
                    _clickPo[2].gameObject.Show();
                    IsCanDrager(_ilDragers, true);                   
                });
            }
            else if ((rotateNameL == "L-2" && rotateNameR == "R-0") || (rotateNameL == "L-0" && rotateNameR == "R-2"))//危险
            {
                _car.Show();
                
                SpineManager.instance.DoAnimation(_car, "che", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    _clickPo[0].gameObject.Show();
                    IsCanDrager(_ilDragers, true);
                }));
            }
            else if ((rotateNameL == string.Empty && rotateNameR == "R-1") || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == "L-2" && rotateNameR == "R-2") || (rotateNameL == string.Empty && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == string.Empty) || (rotateNameL == "L-1" && rotateNameR == "R-1") || (rotateNameL == string.Empty && rotateNameR == string.Empty)|| (rotateNameL == "L-0" && rotateNameR == "R-0"))//前进
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    _clickPo[0].gameObject.Show();
                    IsCanDrager(_ilDragers, true);
                }));
            }
        }
        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {
            droperIndex = index;
            return dragType == dropType;
        }
        private void IsCanDrager(ILDrager[] _ilDrager,bool IsActived)
        {
            for (int i = 0; i < _ilDrager.Length; i++)
            {
                _ilDrager[i].isActived = IsActived;
            }
        }
        private void ShowOrHideClickBtn(PolygonCollider2D[]_clickPo,bool IsShow)
        {
            for (int i = 0; i < _clickPo.Length; i++)
            {
                _clickPo[i].gameObject.SetActive(IsShow);
            }
        }       
        private void OnBeginDrag(Vector3 pos, int type, int index){}
        private void OnDrag(Vector3 pos, int type, int index){

            if (_ilDragers[0].transform.position.y < _sliderPoss[1].position.y + 5 )
            {
                _ilDragers[0].transform.position = /*_sliderDownLimitPosL*/ new Vector3(_ilDragers[0].transform.position.x, _sliderPoss[1].position.y + 5,0);
                return;
            }
            if (_ilDragers[0].transform.position.y > _sliderPoss[0].position.y - 5)
            {
                _ilDragers[0].transform.position = /*_sliderUpLimitPosL*/new Vector3(_ilDragers[0].transform.position.x, _sliderPoss[0].position.y -5, 0);               
                return;
            }
            if (_ilDragers[1].transform.position.y < _sliderPoss[1].position.y + 5)
            {
                _ilDragers[1].transform.position = /*_sliderDownLimitPosR*/new Vector3(_ilDragers[1].transform.position.x, _sliderPoss[1].position.y + 5, 0);
                return;
            }
            if (_ilDragers[1].transform.position.y > _sliderPoss[0].position.y - 5)
            {
                _ilDragers[1].transform.position = /*_sliderUpLimitPosR*/new Vector3(_ilDragers[1].transform.position.x, _sliderPoss[0].position.y - 5, 0);
                return;
            }
        }      

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (isMatch)
            {              
                IsCanDrager(_ilDragers, false);
                ShowOrHideClickBtn(_clickPo, false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, false);
                if (_iLDropers[droperIndex].name == "L-0")//低
                {                 
                    rotateNameL = "L-0";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }


                    SpineManager.instance.SetTimeScale(_leftWheel, 0.1f);
                    SpineManager.instance.DoAnimation(_leftWheel, "zuol2", true);
                }
                else if (_iLDropers[droperIndex].name == "L-1")//中
                {
                    rotateNameL = "L-1";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }

                    SpineManager.instance.SetTimeScale(_leftWheel, 0.5f);
                    SpineManager.instance.DoAnimation(_leftWheel, "zuol2", true);
                }
                else if (_iLDropers[droperIndex].name == "L-2")//高
                {
                    rotateNameL = "L-2";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }

                    SpineManager.instance.SetTimeScale(_leftWheel, 1);
                    SpineManager.instance.DoAnimation(_leftWheel, "zuol2", true);
                }
                else if (_iLDropers[droperIndex].name == "R-0")
                {
                    rotateNameR = "R-0";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }

                    SpineManager.instance.SetTimeScale(_rightWheel, 0.1f);
                    SpineManager.instance.DoAnimation(_rightWheel, "youl2", true);
                }
                else if (_iLDropers[droperIndex].name == "R-1")
                {
                    rotateNameR = "R-1";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }

                    SpineManager.instance.SetTimeScale(_rightWheel, 0.5f);
                    SpineManager.instance.DoAnimation(_rightWheel, "youl2", true);
                }
                else if (_iLDropers[droperIndex].name == "R-2")
                {
                    rotateNameR = "R-2";
                    IsCanDrager(_ilDragers, true);

                    if (isShowCenterClick)
                    {
                        _clickPo[0].gameObject.Show();
                    }
                    else
                    {
                        if (isShowClick == 1)
                        {
                            _clickPo[1].gameObject.Show();
                        }
                        else if (isShowClick == 2)
                        {
                            _clickPo[2].gameObject.Show();
                        }
                    }

                    SpineManager.instance.SetTimeScale(_rightWheel, 1);
                    SpineManager.instance.DoAnimation(_rightWheel, "youl2", true);
                }

            }           
        }       
    }
}
