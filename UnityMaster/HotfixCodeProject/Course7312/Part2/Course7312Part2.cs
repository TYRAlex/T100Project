using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course7312Part2
    {

        /// <summary>
        /// 车在水平位置上的位置
        /// </summary>
        public enum CarPosStateEnum
        {
            qian,
            zhong,
            hou,
            Null

        }
        /// <summary>
        /// 货架的位置
        /// </summary>
        private enum CarState
        {
            di,
            zhong,
            gao,
            Null
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

        private GameObject _spine1;
        private GameObject _spine2;
        private GameObject _car;

        private GameObject _bg0;

        private GameObject _ui_a;
        private GameObject _ui_b;
        private GameObject _ui_c;
        private GameObject _ui_d;

        private GameObject _succeed;

        private Transform _clickBtn;
        private Empty4Raycast[] _clickBtns;

        private GameObject _box;

        private Transform _startCarPos;
        private Transform _boxPos;
        private Transform _tartgetPos;
        private Transform _normalPos;
        private GameObject _targetBox;

        private CarPosStateEnum _carStateEnum;
        private CarState _carState;

        private GameObject _mask;

        private bool isBox;//是否有货物



        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.KillAll();
            Input.multiTouchEnabled = false;


            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 6, true);
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _carStateEnum = CarPosStateEnum.hou;
            _carState = CarState.zhong;           
            isBox = false;

            _spine0 = curTrans.Find("spineManager/0").gameObject;
            _spine0.Show();

            _spine1 = curTrans.Find("spineManager/1").gameObject;
            _spine1.Show();
            SpineManager.instance.DoAnimation(_spine1, "kong", false);

            _spine2 = curTrans.Find("spineManager/2").gameObject;
            _spine2.Show();
            SpineManager.instance.DoAnimation(_spine2, "kong", false);

            _car = curTrans.Find("spineManager/car").gameObject;
            _car.Show();
            SpineManager.instance.DoAnimation(_car, "kong", false);

            _bg0 = curTrans.Find("Bg/0").gameObject;
            _bg0.Show();


            _mask = curTrans.Find("mask").gameObject;
            _mask.Hide();

            _ui_a = curTrans.Find("spineManager/ui_a").gameObject;
            _ui_a.Show();
            _ui_a.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            SpineManager.instance.DoAnimation(_ui_a, "a", false);

            _ui_b = curTrans.Find("spineManager/ui_b").gameObject;
            _ui_b.Show();
            _ui_b.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            SpineManager.instance.DoAnimation(_ui_b, "b", false);

            _ui_c = curTrans.Find("spineManager/ui_c").gameObject;
            _ui_c.Show();
            _ui_c.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            SpineManager.instance.DoAnimation(_ui_c, "c", false);

            _ui_d = curTrans.Find("spineManager/ui_d").gameObject;
            _ui_d.Show();
            _ui_d.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            SpineManager.instance.DoAnimation(_ui_d, "d", false);           

            _succeed = curTrans.Find("succeedPanel/succeed").gameObject;
            _succeed.Hide();


            _box = curTrans.Find("boxManager/0").gameObject;
            _box.Hide();

            _startCarPos = curTrans.Find("spineManager/carStartPos");
            _boxPos = curTrans.Find("spineManager/boxPos");
            _tartgetPos = curTrans.Find("spineManager/tartgetPos");

            _targetBox = curTrans.Find("spineManager/tartgetBox").gameObject;
            _targetBox.Hide();

            _normalPos = curTrans.Find("spineManager/normalPos");
            _car.transform.position = _normalPos.position;

            _clickBtn = curTrans.Find("clickBtn");
            _clickBtns = _clickBtn.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickBtns.Length; i++)
            {
                Util.AddBtnClick(_clickBtns[i].gameObject, ClickBtnEvent);                
            }

            ShowOrHide(_clickBtns, false);

        }

        private void ShowOrHide(Empty4Raycast[]e4r,bool isShow)
        {
            for (int i = 0; i < e4r.Length; i++)
            {
                e4r[i].gameObject.SetActive(isShow);
            }
        }


        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            _spine0.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_spine0, "animation", false,()=> 
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 5, false);
                SpineManager.instance.SetTimeScale(_spine0, 0.8f);               
                SpineManager.instance.DoAnimation(_spine0, "animation2", false,()=> 
                {
                    SpineManager.instance.DoAnimation(_spine0, "animation3", false,()=> 
                    {
                        SpineManager.instance.SetTimeScale(_spine0, 1);
                    });
                });
            });                          //流程1
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
        IEnumerator WaiteCoroutine(Action method_2 = null, float len = 0)
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
                _bg0.Hide();
                _box.Show();
                SpineManager.instance.DoAnimation(_spine0, "kong", false);
                SpineManager.instance.DoAnimation(_spine1, "kong", false);
                SpineManager.instance.DoAnimation(_car, "daiji", true);
                SpineManager.instance.DoAnimation(_spine2, "jiantou", true);                             
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            else if (talkIndex == 2)
            {
                Max.Hide();               
                ShowOrHide(_clickBtns, true);              
            }

            talkIndex++;
        }
        private float aniTime;
        private float time;
        private void ClickBtnEvent(GameObject obj)
        {
            ShowOrHide(_clickBtns, false);
            SoundManager.instance.ShowVoiceBtn(false);            
            //Debug.LogError("isBox:" + isBox + "----_carStateEnum:" + _carStateEnum + "----_carState:" + _carState + "---isGo:" + isGO);

            if (obj.name == "0")//后退
            {
                if (_carStateEnum == CarPosStateEnum.zhong && isBox == false) //无货物
                {
                    if (_carState == CarState.di)
                    {
                        _carStateEnum = CarPosStateEnum.hou;
                        _carState = CarState.di;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htd1", false);
                        _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                    else if (_carState == CarState.zhong)
                    {
                        _carStateEnum = CarPosStateEnum.hou;
                        _carState = CarState.zhong;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htp1", false);
                        _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                    else if (_carState == CarState.gao)
                    {
                        _carStateEnum = CarPosStateEnum.hou;
                        _carState = CarState.gao;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htg1", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.zhong && isBox)//有货物
                {
                    if (_carState == CarState.di)
                    {
                        _carStateEnum = CarPosStateEnum.zhong;
                        _carState = CarState.di;
                        ShowOrHide(_clickBtns, true);
                    }
                    else if (_carState == CarState.zhong)
                    {                       
                        _carStateEnum = CarPosStateEnum.hou;
                        _carState = CarState.zhong;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htp2", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                    else if (_carState == CarState.gao)
                    {                       
                        _carStateEnum = CarPosStateEnum.hou;
                        _carState = CarState.gao;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htg2", false);
                        _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.qian && (isBox || isBox == false))//前,(有或无）货物
                {
                    if (_carState == CarState.zhong)
                    {
                        _carStateEnum = CarPosStateEnum.zhong;
                        _carState = CarState.zhong;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htp2", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                    else if (_carState == CarState.gao)
                    {
                        _carStateEnum = CarPosStateEnum.zhong;
                        _carState = CarState.gao;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "htg2", false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            ShowOrHide(_clickBtns, true);
                        });
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.hou)
                {
                    _carStateEnum = CarPosStateEnum.hou;
                    ShowOrHide(_clickBtns, true);
                }
            }
            else if (obj.name == "1")//前进
            {
                if (_carStateEnum == CarPosStateEnum.hou && isBox == false)//无货物,后--------无问题
                {
                    if (_carState == CarState.zhong)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);

                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        mono.StartCoroutine(WaiteCoroutine(() =>
                        {
                            time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        }, time));

                        aniTime = SpineManager.instance.DoAnimation(_car, "qjp1", false);
                        _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            aniTime = SpineManager.instance.DoAnimation(_car, "htp1", false);
                            _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                _carState = CarState.zhong;
                                _carStateEnum = CarPosStateEnum.hou;
                                ShowOrHide(_clickBtns, true);
                            });
                        });
                    }
                    else if (_carState == CarState.gao)//无货物,高--------无问题
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);

                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        mono.StartCoroutine(WaiteCoroutine(() =>
                        {
                            time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        }, time));

                        aniTime = SpineManager.instance.DoAnimation(_car, "qjg1", false);
                        _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            aniTime = SpineManager.instance.DoAnimation(_car, "htg1", false);
                            _car.transform.DOMove(_startCarPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                            {
                                _carStateEnum = CarPosStateEnum.hou;
                                _carState = CarState.gao;
                                ShowOrHide(_clickBtns, true);
                            });
                        });
                    }
                    else if (_carState == CarState.di)//无货物,低(装货物）
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "qjd1", false);
                        _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                            isBox = true;                           
                            aniTime = SpineManager.instance.DoAnimation(_car, "djd2", false, () =>
                            {
                                _box.Hide();
                                _carStateEnum = CarPosStateEnum.zhong;
                                _carState = CarState.di;
                                ShowOrHide(_clickBtns, true);
                            });
                        });
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.hou && isBox)//有货物，后
                {
                    if (_carState == CarState.zhong)//有货物,中
                    {
                        _carStateEnum = CarPosStateEnum.qian;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);                        
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "qjp2", false);
                        _car.transform.DOMove(_tartgetPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                                //ShowOrHide(_clickBtns, true);
                            });
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.gao)//有货物，高
                    {
                        _carStateEnum = CarPosStateEnum.qian;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);                       
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "qjg2", false);
                        _car.transform.DOMove(_tartgetPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                                //ShowOrHide(_clickBtns, true);
                            });
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.di)//有货物
                    {
                        _carStateEnum = CarPosStateEnum.hou;
                        ShowOrHide(_clickBtns, true);
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.zhong && isBox)//有货物，中
                {
                    if (_carState == CarState.zhong)//有货物 ，中
                    {
                        _carStateEnum = CarPosStateEnum.qian;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);                       
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "qjp2", false);
                        _car.transform.DOMove(_tartgetPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                                //ShowOrHide(_clickBtns, true);
                            });
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.gao)//有货物,高
                    {
                        _carStateEnum = CarPosStateEnum.qian;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);                       
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                        aniTime = SpineManager.instance.DoAnimation(_car, "qjg2", false);
                        _car.transform.DOMove(_tartgetPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                        {
                                //ShowOrHide(_clickBtns, true);
                            });
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.di)//有货物，低
                    {
                        _carStateEnum = CarPosStateEnum.zhong;
                        ShowOrHide(_clickBtns, true);
                    }
                }
                else if (_carStateEnum == CarPosStateEnum.qian && isBox)
                {
                    _carStateEnum = CarPosStateEnum.qian;
                    ShowOrHide(_clickBtns, true);
                }

            }
            else if (obj.name == "2")//抬起
            {             
                if (isBox)//有货物
                {
                    if (_carState == CarState.di)
                    {                       
                        _carState = CarState.zhong;                                              
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "gw2", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.zhong)
                    {                       
                        _carState = CarState.gao;                       
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "ss2", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.gao)
                    {
                        ShowOrHide(_clickBtns, true);
                    }
                }
                else //无货物
                {
                    if (_carState == CarState.di)
                    {                       
                        _carState = CarState.zhong;                      
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        _carState = CarState.zhong;
                        SpineManager.instance.DoAnimation(_car, "gw1", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.zhong)
                    {                       
                        _carState = CarState.gao;                       
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "ss1", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    } 
                    else if(_carState == CarState.gao)
                    {
                        ShowOrHide(_clickBtns, true);
                    }
                }
               
            }
            else if (obj.name == "3")//放下
            {
                if (isBox)//有货物
                {
                    if (_carState == CarState.zhong)
                    {                       
                        _carState = CarState.di;                                            
                        if (_carStateEnum == CarPosStateEnum.qian) //卸货物
                        {                                                       
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);

                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                            SpineManager.instance.DoAnimation(_car, "xf2", false, () =>
                            {
                                _targetBox.Show();
                                SpineManager.instance.DoAnimation(_spine2, "kong", false);
                                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 4, false);
                                SpineManager.instance.DoAnimation(_car, "djd1", false, () =>
                                {
                                    time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 3, false);
                                    aniTime = SpineManager.instance.DoAnimation(_car, "htd1", false);
                                    _car.transform.DOMove(_boxPos.position, aniTime).SetEase(Ease.Linear).OnComplete(() =>
                                    {
                                        _mask.Show();
                                        _succeed.Show();
                                        isBox = false;
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 7, false);
                                        SpineManager.instance.DoAnimation(_succeed, "animation", false, () =>
                                        {
                                            SpineManager.instance.DoAnimation(_succeed, "animation2", false);
                                        });
                                    });
                                });
                            });

                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                            time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                            SpineManager.instance.DoAnimation(_car, "xf2", false);
                            mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                        }                        
                    }
                    else if (_carState == CarState.gao)
                    {                                        
                        _carState = CarState.zhong;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "gw4", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.di)
                    {                      
                        _carState = CarState.di;
                        ShowOrHide(_clickBtns, true);
                    }
                }
                else//无货物
                {
                    if (_carState == CarState.zhong )
                    {                       
                        _carState = CarState.di;                                               
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "xf1", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if (_carState == CarState.gao)
                    {                        
                        _carState = CarState.zhong;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 8, false);
                        time = SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "xf3", false);
                        mono.StartCoroutine(WaiteCoroutine(() => { ShowOrHide(_clickBtns, true); }, time));
                    }
                    else if(_carState == CarState.di)
                    {
                        _carState = CarState.di;
                        ShowOrHide(_clickBtns, true);
                    }
                }                             
            }
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
