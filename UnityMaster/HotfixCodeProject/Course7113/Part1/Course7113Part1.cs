using Spine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.Video;
using Animation = UnityEngine.Animation;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course7113Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        private GameObject _car;
        private GameObject _clickCar;
        private GameObject _clickSmallWheel;
        private GameObject _clickBigWheel;

        private GameObject _3DObj;
        private GameObject _3DCenter1;
        private GameObject _3DCenter2;
        //第一关齿轮拖拽物件与动画
        private GameObject _3DSmallGearGreenRight;
        private GameObject _3DSmallGearYellowRight;
        private GameObject _3DSmallGearYellowLeft;
        private Animation _3DSmallGearYellowLeftAni;
        private Animation _3DSmallGearYellowRightAni;


        //第二关齿轮拖拽物件与动画
        private GameObject _3DSmallGearLeft;
        private GameObject _3DSmallGearRight;
        private GameObject _3DBigGearLeft;
        private GameObject _3DBigGearRight;
        private Animation _3DSmallGearLeftAni;
        private Animation _3DSmallGearRightAni;
        private Animation _3DBigGearLeftAni;
        private Animation _3DBigGearRightAni;

        //最后环节齿轮物件与动画
        private GameObject _3DFinalObj1;
        private GameObject _3DFinalObj2;
        private Animation _3DFinalObjAni1;
        private Animation _3DFinalObjAni2;

        private GameObject _lastGear;       //第二关中替换的上一个齿轮
        private bool _currentPos;   //第二关中齿轮在哪根轴上，true为左，false为右

        private GameObject _level1;
        private GameObject _smallGearLevel1;
        private mILDrager _iLDragerLevel1;

        private GameObject _level2;
        private GameObject _smallGearLevel2;
        private GameObject _bigGearLevel2;
        private List<mILDrager> _iLDragerLevel2;
        private bool _firstPlayed;
        private bool _secondPlayed;

        private GameObject _dropEndLevel1;
        private bool _isDragingLevel1;
        private GameObject _dropEnd1Level2;
        private GameObject _dropEnd2Level2;
        private bool _isDragingLevel2;
        private int _currentDraging;

        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;
            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);

            _car = curTrans.GetGameObject("CarParent/Car");
            _clickCar = curTrans.GetGameObject("ClickEvent/ClickCar");
            _clickSmallWheel = curTrans.GetGameObject("ClickEvent/ClickSmallWheel");
            _clickBigWheel = curTrans.GetGameObject("ClickEvent/ClickBigWheel");
            _dropEndLevel1 = curTrans.GetGameObject("Level1/DropEnd");
            _dropEnd1Level2 = curTrans.GetGameObject("Level2/DropEnd1");
            _dropEnd2Level2 = curTrans.GetGameObject("Level2/DropEnd2");

            _3DObj = curTrans.GetGameObject("3DObjects");
            _3DCenter1 = curTrans.GetGameObject("3DObjects/Center1");
            _3DCenter2 = curTrans.GetGameObject("3DObjects/Center2");
            _3DSmallGearGreenRight = curTrans.GetGameObject("3DObjects/SmallGear_greenRight");
            _3DSmallGearYellowRight = curTrans.GetGameObject("3DObjects/SmallGear_yellowRight");
            _3DSmallGearYellowLeft = curTrans.GetGameObject("3DObjects/SmallGear_yellowLeft");
            _3DSmallGearYellowLeftAni = _3DSmallGearYellowLeft.GetComponent<Animation>();
            _3DSmallGearYellowRightAni = _3DSmallGearYellowRight.GetComponent<Animation>();

            _3DSmallGearLeft = curTrans.GetGameObject("3DObjects/SmallGear_Left");
            _3DSmallGearRight = curTrans.GetGameObject("3DObjects/SmallGear_Right");
            _3DBigGearLeft = curTrans.GetGameObject("3DObjects/BigGear_Left");
            _3DBigGearRight = curTrans.GetGameObject("3DObjects/BigGear_Right");
            _3DSmallGearLeftAni = _3DSmallGearLeft.GetComponent<Animation>();
            _3DSmallGearRightAni = _3DSmallGearRight.GetComponent<Animation>();
            _3DBigGearLeftAni = _3DBigGearLeft.GetComponent<Animation>();
            _3DBigGearRightAni = _3DBigGearRight.GetComponent<Animation>();

            _3DFinalObj1 = curTrans.GetGameObject("3DObjects/FinalObj1");
            _3DFinalObj2 = curTrans.GetGameObject("3DObjects/FinalObj2");
            _3DFinalObjAni1 = _3DFinalObj1.GetComponent<Animation>();
            _3DFinalObjAni2 = _3DFinalObj2.GetComponent<Animation>();

            _level1 = curTrans.GetGameObject("Level1");
            _smallGearLevel1 = curTrans.GetGameObject("Level1/SmallGear");
            _level2 = curTrans.GetGameObject("Level2");
            _smallGearLevel2 = curTrans.GetGameObject("Level2/SmallGear");
            _bigGearLevel2 = curTrans.GetGameObject("Level2/BigGear");
            ReStart(btnTest);
        }

        void ReStart(GameObject obj)
        {
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell.SetActive(true);

            Bg.SetActive(true);
            bellTextures = Bg.GetComponent<BellSprites>();

            _car.SetActive(true);
            SpineManager.instance.DoAnimation(_car, "animation2");
            
            _clickCar.SetActive(false);
            _clickSmallWheel.SetActive(false);
            _clickBigWheel.SetActive(false);

            ResetRotation();

            UIEventListener.Get(_dropEndLevel1).onEnter = OnEnterLevel1;
            UIEventListener.Get(_dropEndLevel1).onExit = OnExitLevel1;
            InitDragLevel1();
            _iLDragerLevel1.SetDragCallback(null, null, null, null);
            _iLDragerLevel1.DoReset();
            _isDragingLevel1 = false;
            
            UIEventListener.Get(_dropEnd1Level2).onEnter = OnEnterDrop1Level2;           
            UIEventListener.Get(_dropEnd1Level2).onExit = OnExitDrop1Level2;
            UIEventListener.Get(_dropEnd2Level2).onEnter = OnEnterDrop2Level2;
            UIEventListener.Get(_dropEnd2Level2).onExit = OnExitDrop2Level2;
            InitDragLevel2();
            foreach(var drager in _iLDragerLevel2)
            {
                drager.SetDragCallback(null, null, null, null);
                drager.DoReset();
            }
            _isDragingLevel2 = false;

            _3DObj.SetActive(false);
            _3DCenter1.SetActive(false);
            _3DCenter2.SetActive(false);
            _3DSmallGearGreenRight.SetActive(false);
            _3DSmallGearYellowRight.SetActive(false);
            _3DSmallGearYellowLeft.SetActive(false);
            _3DSmallGearLeft.SetActive(false);
            _3DSmallGearRight.SetActive(false);
            _3DBigGearLeft.SetActive(false);
            _3DBigGearRight.SetActive(false);
            _3DFinalObj1.SetActive(false);
            _3DFinalObj2.SetActive(false);

            _level1.SetActive(false);
            _level2.SetActive(false);

            _firstPlayed = false;
            _secondPlayed = false;
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            //if (bellTextures.texture.Length <= 0)
            //{
            //    Debug.LogError("@愚蠢！！ 哈哈哈 Bg上的BellSprites 里没有东西----------添加完删掉这个打印");
            //}
            //else
            //{
            //    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            //}
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                SpineManager.instance.DoAnimation(_car, "animation");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            },
            () =>
            {
                SpineManager.instance.DoAnimation(_car, "animation2");
                _clickCar.SetActive(true);
                Util.AddBtnClick(_clickCar, ClickCar);
                
            }, 0));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        //自定义动画协程，len1为中间第一次停顿时长，len2为中间第二次停顿时长
        IEnumerator AniCoroutine(Action method_1 = null, Action method_2 = null, float len1 = 0)
        {
            SoundManager.instance.SetShield(false);
            if (method_1 != null)
            {
                method_1();
            }
            if (len1 > 0)
            {
                yield return new WaitForSeconds(len1);
            }
            if (method_2 != null)
            {
                method_2();
            }
            SoundManager.instance.SetShield(true);
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null,
                () =>
                {
                    _iLDragerLevel1.canMove = true;
                    AddDragEventLevel1();
                }, 0));
            }
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                () =>
                {
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        _iLDragerLevel1.SetDragCallback(null, null, null, null);
                        _level1.SetActive(false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        _3DSmallGearYellowLeftAni.Play();
                        _3DSmallGearYellowRightAni.Play();
                    },
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }, 6.0f));
                }, null, 0));
            }
            if (talkIndex == 3)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                () =>
                {
                    _3DSmallGearYellowLeft.SetActive(false);
                    _3DSmallGearYellowRight.SetActive(false);
                    _3DCenter1.SetActive(false);
                    _3DCenter2.SetActive(true);
                    _level2.SetActive(true);
                    _smallGearLevel2.SetActive(true);
                    _bigGearLevel2.SetActive(true);
                    foreach (var drager in _iLDragerLevel2)
                        drager.canMove = false;
                },
                () =>
                {
                    foreach (var drager in _iLDragerLevel2)
                        drager.canMove = true;
                    AddDragEventLevel2();
                }, 0));
            }
            if (talkIndex == 4)
            {
                foreach (var drager in _iLDragerLevel2)
                {
                    drager.DoReset();
                    drager.SetDragCallback(null, null, null, null);
                }
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7,
                () =>
                {
                    _level2.SetActive(false);
                    _3DCenter2.SetActive(false);
                    _3DBigGearLeft.SetActive(false);
                    _3DBigGearRight.SetActive(false);
                    _3DSmallGearLeft.SetActive(false);
                    _3DSmallGearRight.SetActive(false);
                    _3DFinalObj1.SetActive(true);
                    _3DFinalObj2.SetActive(true);
                    mono.StartCoroutine(AniCoroutine(
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        _3DFinalObjAni1.Play();
                    },
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                        _3DFinalObjAni2.Play();
                    }, 4.5f));
                },
                () =>
                {
                    //打开右侧系统UI面板，点击下一步，跳转至下一环节TODO
                    //_videoPlayer01.SetActive(false);
                    //_videoPlayer02.SetActive(false);
                }, 0));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音与成功激励语音
        //失败激励语音
        //private void BtnPlaySoundFail()
        //{
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        //}
        //成功激励语音
        //private void BtnPlaySoundSuccess()
        //{
        //    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        //}

        void ResetRotation()
        {
            //GameObject[] obj = UnityEngine.Object.FindObjectsOfType(typeof(GameObject)) as GameObject[]; //关键代码，获取所有gameobject元素给数组obj
            //foreach (GameObject child in obj)    //遍历所有gameobject
            //{
            //    //Debug.Log(child.gameObject.name);  //可以在unity控制台测试一下是否成功获取所有元素
            //    if (child.gameObject.tag == "Gear")    //进行操作
            //    {
            //        child.transform.localEulerAngles = new Vector3(0, 0, 0);
            //    }
            //}
            _3DSmallGearGreenRight.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DSmallGearYellowRight.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DSmallGearYellowLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DSmallGearLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DSmallGearRight.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DBigGearLeft.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DBigGearRight.transform.localEulerAngles = new Vector3(0, 0, 0);
            _3DFinalObj1.transform.GetChild(1).localEulerAngles = new Vector3(0, 0, 0);
            _3DFinalObj1.transform.GetChild(2).localEulerAngles = new Vector3(0, 0, 0);
            _3DFinalObj2.transform.GetChild(1).localEulerAngles = new Vector3(0, 0, 0);
            _3DFinalObj2.transform.GetChild(2).localEulerAngles = new Vector3(0, 0, 0);
        }

        //重置某个齿轮的旋转角度
        void ResetRotation(GameObject obj)
        {
            obj.transform.localEulerAngles = new Vector3(0, 0, 0);
        }

        #region 小车事件
        //点击小车事件
        void ClickCar(GameObject obj)
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
            () =>
            {
                bell.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(_car, "animation4", false);
                obj.SetActive(false);
            },
            () =>
            {
                _clickSmallWheel.SetActive(true);
                Util.AddBtnClick(_clickSmallWheel, ClickSmallWheel);
            }, 0));
        }

        //点击小车轮事件
        void ClickSmallWheel(GameObject obj)
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
            () =>
            {
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(_car, "animation5", true);
                    obj.SetActive(false);
                },
                () =>
                {
                    SpineManager.instance.DoAnimation(_car, "animation6");
                }, 1.5f));
            },
            () =>
            {
                _clickBigWheel.SetActive(true);
                Util.AddBtnClick(_clickBigWheel, ClickBigWheel);
            }, 0));
        }

        //点击大车轮事件
        void ClickBigWheel(GameObject obj)
        {
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
            () =>
            {
                _car.SetActive(false);
                obj.SetActive(false);
                //点击上面的车轮，画面中间切换出齿轮机构
                _3DObj.SetActive(true);
                _3DSmallGearYellowLeft.SetActive(true);
                _3DCenter1.SetActive(true);
                _level1.SetActive(true);
                _smallGearLevel1.SetActive(true);
                _iLDragerLevel1.canMove = false;
            },
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }, 0));
        }
        #endregion

        #region 齿轮拖拽事件Level1

        //第一关初始化拖拽物品
        void InitDragLevel1()
        {
            _iLDragerLevel1 = _smallGearLevel1.GetComponent<mILDrager>();
        }

        //第一关添加拖拽事件
        void AddDragEventLevel1()
        {
            _iLDragerLevel1.SetDragCallback(BeginDragLevel1, DragingLevel1, EndDragLevel1, null);
        }

        //第一关开始拖拽事件
        private void BeginDragLevel1(Vector3 vector3, int dragType, int dragIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
        }

        //第一关拖拽中事件
        void DragingLevel1(Vector3 dragPosition, int dragType, int dragIndex)
        {
            _isDragingLevel1 = true;
        }

        //第一关拖拽结束事件
        void EndDragLevel1(Vector3 dragPosition, int dragType, int dragIndex, bool dragBool)
        {
            JudgeGearOnCenter();
            _isDragingLevel1 = false;
            if (dragBool)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                _iLDragerLevel1.SetDragCallback(null, null, null, null);
                _smallGearLevel1.SetActive(false);
                _3DSmallGearGreenRight.SetActive(false);
                _3DCenter1.SetActive(true);
                _3DSmallGearYellowLeft.SetActive(true);
                _3DSmallGearYellowRight.SetActive(true);
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    _3DSmallGearYellowLeftAni.Play();
                    _3DSmallGearYellowRightAni.Play();
                },
                () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }, 6.0f));
            }
            else
            {
                _smallGearLevel1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                _iLDragerLevel1.DoReset();
            }
        }

        //判断轴上是否有齿轮(保险)
        void JudgeGearOnCenter()
        {
            if(_3DSmallGearGreenRight.activeSelf || _3DSmallGearYellowRight.activeSelf)
            {
                _smallGearLevel1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
                _iLDragerLevel1.DoReset();
            }
        }

        //进入正确区域的事件
        private void OnEnterLevel1(GameObject go)
        {
            if(_isDragingLevel1)
            {
                _3DSmallGearGreenRight.SetActive(true);
                _smallGearLevel1.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            }
        }
        
        //退出正确区域的事件
        private void OnExitLevel1(GameObject go)
        {
            _3DSmallGearGreenRight.SetActive(false);
            _smallGearLevel1.GetComponent<Image>().color = new Color(255, 255, 255, 255);
        }
        #endregion

        #region 齿轮拖拽事件Level2
        //第二关初始化拖拽组件
        void InitDragLevel2()
        {
            _iLDragerLevel2 = new List<mILDrager>();
            for (int i = 0; i < _level2.transform.childCount; i++)
            {
                if(_level2.transform.GetChild(i).transform.GetComponent<mILDrager>())
                    _iLDragerLevel2.Add(_level2.transform.GetChild(i).transform.GetComponent<mILDrager>());
            }
        }

        //第二关添加拖拽事件
        void AddDragEventLevel2()
        {
            foreach (var drager in _iLDragerLevel2)
                drager.SetDragCallback(BeginDragLevel2, DragingLevel2, EndDragLevel2, null);
        }

        //第二关开始拖拽事件
        private void BeginDragLevel2(Vector3 vector3, int dragType, int dragIndex)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
        }

        //第二关小齿轮拖拽事件
        void DragingLevel2(Vector3 vector3, int dragType, int dragIndex)
        {
            _isDragingLevel2 = true;
            _currentDraging = dragIndex;
        }

        //第二关拖拽结束事件
        void EndDragLevel2(Vector3 dragPosition, int dragType, int dragIndex, bool dragBool)
        {
            _isDragingLevel2 = false;
            if (dragBool)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                //_lastGear不为空时则说明有替换，进行替换显示
                if (_lastGear != null)
                {
                    if (dragIndex == 0)
                    {
                        //有_lastGear则证明有替换，另一个齿轮返回原位
                        _bigGearLevel2.SetActive(true);
                        _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                        _iLDragerLevel2[1].DoReset();
                        _lastGear = null;
                    }
                    if (dragIndex == 1)
                    {
                        //有_lastGear则证明有替换，另一个齿轮返回原位
                        _smallGearLevel2.SetActive(true);
                        _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                        _iLDragerLevel2[0].DoReset();
                        _lastGear = null;
                    }
                }
                //_lastGear为空时则说明无替换，直接显示
                else
                {
                    //_currentPos为true则说明齿轮在左边
                    if (_currentPos)
                    {
                        if (dragIndex == 0)
                        {
                            //拖动的是小齿轮，则显示左小齿轮
                            _3DSmallGearLeft.SetActive(true);
                            _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            _iLDragerLevel2[0].DoReset();
                        }
                        if (dragIndex == 1)
                        {
                            //拖动的是大齿轮，则显示左大齿轮
                            _3DBigGearLeft.SetActive(true);
                            _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            _iLDragerLevel2[1].DoReset();
                        }
                    }
                    //_currentPos为true则说明齿轮在右边
                    else
                    {
                        if (dragIndex == 0)
                        {
                            //拖动的是小齿轮，则显示右小齿轮
                            _3DSmallGearRight.SetActive(true);
                            _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            _iLDragerLevel2[0].DoReset();
                        }
                        if (dragIndex == 1)
                        {
                            //拖动的是大齿轮，则显示右大齿轮
                            _3DBigGearRight.SetActive(true);
                            _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                            _iLDragerLevel2[1].DoReset();
                        }
                    }
                }
            }
            else
            {
                _iLDragerLevel2[dragIndex].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                _iLDragerLevel2[dragIndex].DoReset();
                _lastGear = null;
            }

            JudgeFullGear();
        }

        //判断bug是否存在与应对方法
        void JudgeFullGear()
        {
            //拖动完判断，大齿轮存在，则大齿轮图片消隐，且此时没有任何一个小齿轮，则小齿轮图片出现（保险）
            if (_3DBigGearRight.activeSelf || _3DBigGearLeft.activeSelf)
            {
                _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                _iLDragerLevel2[1].DoReset();
                _bigGearLevel2.SetActive(false);
                if (!_3DSmallGearRight.activeSelf && !_3DSmallGearLeft.activeSelf)
                {
                    _smallGearLevel2.SetActive(true);
                    _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[0].DoReset();
                }
            }
            
            //拖动完判断，小齿轮存在，则小齿轮图片消隐，且此时没有任何一个大齿轮，则大齿轮图片出现（保险）
            if (_3DSmallGearRight.activeSelf || _3DSmallGearLeft.activeSelf)
            {
                _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                _iLDragerLevel2[0].DoReset();
                _smallGearLevel2.SetActive(false);
                if (!_3DBigGearRight.activeSelf && !_3DBigGearLeft.activeSelf)
                {
                    _bigGearLevel2.SetActive(true);
                    _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[1].DoReset();
                }
            }

            //如果不小心卡到bug，大小齿轮同时出现在第一根轴上（保险）
            if (_3DSmallGearLeft.activeSelf && _3DBigGearLeft.activeSelf)
            {
                if(_currentDraging == 0)
                {
                    _3DBigGearLeft.SetActive(false);
                    _bigGearLevel2.SetActive(true);
                    _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[1].DoReset();
                }
                if (_currentDraging == 1)
                {
                    _3DSmallGearLeft.SetActive(false);
                    _smallGearLevel2.SetActive(true);
                    _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[0].DoReset();
                }
            }

            //如果不小心卡到bug，大小齿轮同时出现在第二根轴上（保险）
            if (_3DSmallGearRight.activeSelf && _3DBigGearRight.activeSelf)
            {
                if (_currentDraging == 0)
                {
                    _3DBigGearRight.SetActive(false);
                    _bigGearLevel2.SetActive(true);
                    _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[1].DoReset();
                }
                if (_currentDraging == 1)
                {
                    _3DSmallGearRight.SetActive(false);
                    _smallGearLevel2.SetActive(true);
                    _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[0].DoReset();
                }
            }

            //左边大齿轮，右边小齿轮，则播放动画
            if (_3DSmallGearRight.activeSelf && _3DBigGearLeft.activeSelf)
            {
                _firstPlayed = true;
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    _3DBigGearLeftAni.Play();
                    _3DSmallGearRightAni.Play();
                },
                () =>
                {
                    ResetRotation(_3DSmallGearRight);
                    ResetRotation(_3DBigGearLeft);
                    _3DSmallGearRight.SetActive(false);
                    _3DBigGearLeft.SetActive(false);
                    _smallGearLevel2.SetActive(true);
                    _bigGearLevel2.SetActive(true);
                    foreach (var drager in _iLDragerLevel2)
                        drager.DoReset();
                    _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    if (_firstPlayed == true && _secondPlayed == true)
                        SoundManager.instance.ShowVoiceBtn(true);
                    else
                        SoundManager.instance.ShowVoiceBtn(false);
                }, 6.0f));
            }

            //左边小齿轮，右边大齿轮，则播放视频
            if (_3DSmallGearLeft.activeSelf && _3DBigGearRight.activeSelf)
            {
                _secondPlayed = true;
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(AniCoroutine(
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    _3DSmallGearLeftAni.Play();
                    _3DBigGearRightAni.Play();
                },
                () =>
                {
                    ResetRotation(_3DSmallGearLeft);
                    ResetRotation(_3DBigGearRight);
                    _3DSmallGearLeft.SetActive(false);
                    _3DBigGearRight.SetActive(false);
                    _smallGearLevel2.SetActive(true);
                    _bigGearLevel2.SetActive(true);
                    foreach (var drager in _iLDragerLevel2)
                        drager.DoReset();
                    _iLDragerLevel2[0].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    _iLDragerLevel2[1].GetComponent<Image>().color = new Color(255, 255, 255, 255);
                    if (_firstPlayed == true && _secondPlayed == true)
                        SoundManager.instance.ShowVoiceBtn(true);
                    else
                        SoundManager.instance.ShowVoiceBtn(false);
                }, 6.0f));
            }
        }

        //第二关进入正确区域1的事件
        private void OnEnterDrop1Level2(GameObject go)
        {
            if (_isDragingLevel2)
            {
                Debug.Log("进入区域一");
                if (_currentDraging == 0)
                {
                    if (_3DBigGearLeft.activeSelf)
                    {
                        _lastGear = _3DBigGearLeft;
                        _3DBigGearLeft.SetActive(false);
                    }
                    _3DSmallGearLeft.SetActive(true);
                    _currentPos = true;
                    _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                }
                if (_currentDraging == 1)
                {
                    if (_3DSmallGearLeft.activeSelf)
                    {
                        _lastGear = _3DSmallGearLeft;
                        _3DSmallGearLeft.SetActive(false);
                    }
                    _3DBigGearLeft.SetActive(true);
                    _currentPos = true;
                    _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                }
            }
        }

        //第二关退出正确区域1的事件
        private void OnExitDrop1Level2(GameObject go)
        {
            if (_isDragingLevel2)
            {
                Debug.Log("退出区域一");
                if (_currentDraging == 0)
                {
                    _3DSmallGearLeft.SetActive(false);
                    _3DSmallGearRight.SetActive(false);
                }
                if (_currentDraging == 1)
                {
                    _3DBigGearLeft.SetActive(false);
                    _3DBigGearRight.SetActive(false);
                }
                if (_lastGear != null)
                {
                    _lastGear.SetActive(true);
                    _lastGear = null;
                }
                _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
        }
        
        //第二关进入正确区域2的事件
        private void OnEnterDrop2Level2(GameObject go)
        {
            if (_isDragingLevel2)
            {
                Debug.Log("进入区域二");
                if (_currentDraging == 0)
                {
                    if (_3DBigGearRight.activeSelf)
                    {
                        _lastGear = _3DBigGearRight;
                        _3DBigGearRight.SetActive(false);
                    }
                    _3DSmallGearRight.SetActive(true);
                    _currentPos = false;
                    _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                }
                if (_currentDraging == 1)
                {
                    if (_3DSmallGearRight.activeSelf)
                    {
                        _lastGear = _3DSmallGearRight;
                        _3DSmallGearRight.SetActive(false);
                    }
                    _3DBigGearRight.SetActive(true);
                    _currentPos = false;
                    _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 0);
                }
            }
        }

        //第二关退出正确区域2的事件
        private void OnExitDrop2Level2(GameObject go)
        {
            if (_isDragingLevel2)
            {
                Debug.Log("退出区域二");
                if (_currentDraging == 0)
                {
                    _3DSmallGearLeft.SetActive(false);
                    _3DSmallGearRight.SetActive(false);
                }
                if (_currentDraging == 1)
                {
                    _3DBigGearLeft.SetActive(false);
                    _3DBigGearRight.SetActive(false);
                }
                if (_lastGear != null)
                {
                    _lastGear.SetActive(true);
                    _lastGear = null;
                }
                _iLDragerLevel2[_currentDraging].GetComponent<Image>().color = new Color(255, 255, 255, 255);
            }
        }

        #endregion
    }
}
