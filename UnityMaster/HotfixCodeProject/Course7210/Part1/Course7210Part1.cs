using ILRuntime.Runtime;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;

namespace ILFramework.HotClass
{
    public class Course7210Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private Transform _allGear;
        private GameObject _bigGear;
        private GameObject _anniu;
        private Transform _pos;
        private List<ILDrager> _iLDragers;   //拖拽物体数组
        private Vector3 _stopPos;        //第四流程中其他齿轮在第四齿轮旁的位置

        void Start(object o)
        {
            curGo = (GameObject)o;
            RestartGame(curGo);
        }

        void RestartGame(GameObject obj)
        {
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            Input.multiTouchEnabled = false;
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(true);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            _allGear = curTrans.Find("AllGear");
            _bigGear = curTrans.GetGameObject("BigGear/Gear");
            _allGear.gameObject.SetActive(true);
            _anniu = curTrans.GetGameObject("Anniu");
            _anniu.SetActive(false);
            _stopPos = new Vector2(5000, 5000);
            _pos = curTrans.Find("Pos");

            InitDrager();
            StopDrag();

            SpineManager.instance.DoAnimation(_bigGear, "kong", false);
            talkIndex = 1;
            //Util.AddBtnClick(_restartBtn, RestartGame);
            GameStart();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //流程一
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
            () =>
            {
                for (int i = 0; i < _allGear.childCount; i++)
                {
                    GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                    SpineManager.instance.DoAnimation(o, "kong", false, 
                    () => 
                    { 
                        SpineManager.instance.DoAnimation(o, o.name, false); 
                    });
                }
                for (int i = 0; i < _allGear.childCount; i++)
                {
                    _allGear.GetChild(i).position = _pos.GetChild(i).position;
                }
            },
            () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
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

            if (method_2 != null)
            {
                method_2();
            }
        }

        //自定义动画协程
        IEnumerator AniCoroutine(Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (method_1 != null)
            {
                method_1();
            }

            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            //流程二
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                        SpineManager.instance.DoAnimation(o, o.name + "1", false);
                    }
                    mono.StartCoroutine(AniCoroutine(null,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        for (int i = 0; i < _allGear.childCount; i++)
                        {
                            GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                            SpineManager.instance.DoAnimation(o, o.name + "2", true);
                        }
                        _anniu.SetActive(true);
                    }, 4.8f));
                }, 
                ()=> 
                {
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        SpineManager.instance.DoAnimation(o, o.name, false);
                    }
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }

            //流程三
            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                        SpineManager.instance.DoAnimation(o, o.name, false);
                    }
                    //移动三个齿轮位置往下
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        _allGear.GetChild(i).position = _pos.GetChild(i + 3).position;
                    }
                    SpineManager.instance.DoAnimation(_bigGear, "kong", false, 
                    ()=> 
                    {
                        SpineManager.instance.DoAnimation(_bigGear, "d1", false);
                    });
                    _anniu.SetActive(false);
                },
                () =>
                {
                    //语音播放完可以开始添加拖拽事件等
                    AddDragEvent();
                }
                ));
            }

            //流程四语音部分
            if (talkIndex == 3)
            {
                StopDrag();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, null, 
                ()=> 
                {
                    AddDragEvent();
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }
            if (talkIndex == 4)
            {
                StopDrag();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, null, 
               () => 
               {
                   AddDragEvent();
                   SoundManager.instance.ShowVoiceBtn(true); 
               }));
            }
            if (talkIndex == 5)
            {
                StopDrag();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, null, 
                () => 
                {
                    AddDragEvent();
                    SoundManager.instance.ShowVoiceBtn(true); 
                }));
            }

            //流程五
            if (talkIndex == 6)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                () =>
                {
                    StopDrag();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        GameObject o = _allGear.GetChild(i).GetChild(0).gameObject;
                        SpineManager.instance.DoAnimation(o, "kong", false, 
                        ()=> 
                        {
                            SpineManager.instance.DoAnimation(o, o.name, false);
                        });
                    }
                    for (int i = 0; i < _allGear.childCount; i++)
                    {
                        _allGear.GetChild(i).position = _pos.GetChild(i).position;
                    }
                    SpineManager.instance.DoAnimation(_bigGear, "kong", false);
                    _anniu.SetActive(true);
                }, null));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }


        #region Drag事件
        //初始化拖拽数组
        void InitDrager()
        {
            _iLDragers = new List<ILDrager>();
            for (int i = 0; i < _allGear.transform.childCount; i++)
            {
                _iLDragers.Add(_allGear.transform.GetChild(i).transform.GetComponent<ILDrager>());
            }
        }

        //添加拖拽事件
        void AddDragEvent()
        {
            foreach (var drager in _iLDragers)
            {
                drager.SetDragCallback(null, DragingEvent, EndDragEvent, null);
                drager.canMove = true;
            }
        }

        //停止拖拽
        void StopDrag()
        {
            foreach (var drager in _iLDragers)
            {
                drager.SetDragCallback(null, null, null, null);
                drager.canMove = false;
            }
        }

        //拖拽中事件
        void DragingEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            GameObject o = _allGear.GetChild(dragIndex).GetChild(0).gameObject;
            if (SpineManager.instance.GetCurrentAnimationName(o) != o.name + "2")
                SpineManager.instance.DoAnimation(o, o.name + "2", true);
        }

        //拖拽结束事件
        void EndDragEvent(Vector3 dragPosition, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                //齿轮至指定区域，播放齿轮4动画
                GameObject o = _allGear.GetChild(dragIndex).gameObject;
                string gearName = o.transform.GetChild(0).name + "1";
                string bigGearName = "d" + (dragIndex + 2).ToString();
                FullGearAni(o, gearName, bigGearName, dragIndex + 7);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                //若其他齿轮位于齿轮四旁则返回原位
                if (_allGear.GetChild(0).position != _pos.GetChild(3).position && o.name != "0")
                {
                    GearReturnAni(0);
                }
                if (_allGear.GetChild(1).position != _pos.GetChild(4).position && o.name != "1")
                {
                    GearReturnAni(1);
                }
                if (_allGear.GetChild(2).position != _pos.GetChild(5).position && o.name != "2")
                {
                    GearReturnAni(2);
                }
                SoundManager.instance.ShowVoiceBtn(true);
            }
            //如果拖拽至错误区域则返回原点
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                _allGear.GetChild(dragIndex).position = _pos.GetChild(dragIndex + 3).position;
                GameObject o = _allGear.GetChild(dragIndex).GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(o, o.name);
            }

        }

        //新齿轮替换原齿轮，原齿轮返回方法
        void GearReturnAni(int gearNum)
        {
            //使用协程实现以大变小效果
            mono.StartCoroutine(AniCoroutine(
            () =>
            {
                _allGear.GetChild(gearNum).position = _pos.GetChild(gearNum + 3).position;
            },
            () =>
            {
                GameObject aniObj = _allGear.GetChild(gearNum).GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(aniObj, aniObj.name);
            }, 0.1f));
        }

        //非完整齿轮咬合完整齿轮动画效果
        void FullGearAni(GameObject currentGear, string currentGearAni, string bigGearAni, int soundIndex)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, soundIndex, true);
            SpineManager.instance.DoAnimation(currentGear.transform.GetChild(0).gameObject, currentGearAni, false);
            currentGear.transform.position = _stopPos;
            SpineManager.instance.DoAnimation(_bigGear, bigGearAni);
        }
        #endregion
    }
}
