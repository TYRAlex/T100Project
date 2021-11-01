using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course834Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        GameObject max;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _robot;
        GameObject _first;
        GameObject _p5;
        GameObject _p2;
        GameObject _p3;
        GameObject _p4;
        GameObject _ani;
        GameObject _mask;
        GameObject _Robots;
        GameObject _Postion;
        mILDrager[] _robots;
        mILDroper[] _pos;
        GameObject _image;
        GameObject _end;


        List<GameObject> _list;

        private List<Action> _callBacks;
        List<float> _times;

        void Start(object o)
        {
            curGo = (GameObject) o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            _robot = curTrans.GetGameObject("Image/robot");
            _first = curTrans.GetGameObject("Image/p1");
            _p5 = curTrans.GetGameObject("Image/p5");
            _p2 = curTrans.GetGameObject("Image/p2");
            _p3 = curTrans.GetGameObject("Image/p3");
            _p4 = curTrans.GetGameObject("Image/p4");
            max = curTrans.GetGameObject("Image/Max");
            _ani = curTrans.GetGameObject("ani");
            _mask = curTrans.GetGameObject("mask");
            _Robots = curTrans.GetGameObject("R");
            _Postion = curTrans.GetGameObject("Pos");
            _image = curTrans.GetGameObject("Image/image");
            _robots = new mILDrager[_Robots.transform.childCount];
            _pos = new mILDroper[_Postion.transform.childCount];
            _end = curTrans.GetGameObject("end");
            _list = new List<GameObject>();
            _callBacks = new List<Action>();
            _times = new List<float>();

            for (int i = 0; i < _image.transform.childCount; i++)
            {
                _list.Add(_image.transform.GetChild(i).gameObject);
            }

            Util.AddBtnClick(btnTest, ReStart);
            btnTest.SetActive(false);
            ReStart(btnTest);
        }


        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            _robot.Show();
            _first.Show();
            _p5.Show();
            _p2.Show();
            _p3.Show();
            _p4.Show();
            _ani.Hide();
            _mask.Hide();
            _image.Hide();
            _Postion.Hide();
            _Robots.Hide();
            _end.Hide();
            bell.Show();
            max.Show();
            _robot.transform.DOKill();
            _robot.transform.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            foreach (var item in _list)
            {
                item.Show();
            }

            talkIndex = 1;

            for (int i = 0; i < _Robots.transform.childCount; i++)
            {
                _robots[i] = _Robots.transform.GetChild(i).GetComponent<mILDrager>();
                Debug.Log(_robots[i].name);

            }

            foreach (var item in _robots)
            {
                item.SetDragCallback(null, null, SetPos, PlayAni);
                item.transform.localScale = new Vector3(1f, 1f);
                item.DoReset();
            }


            for (int i = 0; i < _Postion.transform.childCount; i++)
            {
                _pos[i] = _Postion.transform.GetChild(i).gameObject.GetComponent<mILDroper>();
            }

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _times.Clear();
            _callBacks.Clear();
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () => { Debug.LogError("Max，如果让你指示机器人在这条路上往返两次，你会怎么做？ "); },
                () =>
                {
                    Robot(SoundManager.SoundType.VOICE, 1, max, () => { SoundManager.instance.ShowVoiceBtn(true); });
                }));
        }


        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
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

        IEnumerator SpeckerCoroutineMax(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(max, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(max, "daiji");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            if (talkIndex == 1)
            {
                _mask.Show();
                Debug.LogError("看我的！机器人循环执行四次以下指令：向前走四步，然后向后转。");

                Robot(SoundManager.SoundType.VOICE, 2, bell,
                    () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(false);

                        Robot(SoundManager.SoundType.VOICE, 6, bell,
                            () =>
                            {
                                mono.StartCoroutine(SpeckerCoroutineMax(SoundManager.SoundType.VOICE, 4,
                                    () => { Debug.LogError("哇！机器人一下子就完成了指令"); },
                                    () => { Sence(); }
                                ));
                            }, 0);
                    });
            }

            if (talkIndex == 2)
            {
                Sence2();
            }

            talkIndex++;
        }


        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        /// <summary>
        /// 拖拽
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        /// <param name="dragBool"></param>
        private void SetPos(Vector3 dragPos, int dragType, int dragIndex, bool dragBool) //type  index
        {
            Debug.LogError(dragType + ":" + dragIndex + ":" + dragBool);

            var curDrag = _robots[dragIndex];
            var curDragType = curDrag.dragType;
            if (dragBool)
            {
                mILDroper curPos = null;
                foreach (var item in _pos)
                {
                    if (item.dropType == curDragType)
                    {
                        curPos = item;
                        break;
                    }
                }


                curDrag.gameObject.transform.position = curPos.gameObject.transform.position;
                curDrag.gameObject.transform.GetRectTransform().DOScale(new Vector3(0.9f, 0.9f), 1f);
                if (curDragType == 1)
                {
                    _list[0].Hide();
                }

                bool isend = IsEnd();
                if (!isend)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                }

                if (isend)
                {
                    Debug.LogError("程序执行成功");
                    _end.transform.GetRectTransform().anchoredPosition = new Vector2(180, -100);
                    _end.Show();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    SpineManager.instance.DoAnimation(_end, "8", true, () => { });
                }
            }

            else
            {
                curDrag.gameObject.transform.GetRectTransform().DOScale(new Vector3(1f, 1f), 1f);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                _robots[dragIndex].DoReset();
            }
        }

        /// <summary>
        ///  robot走动循环
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="method_1"></param>
        void Robot(SoundManager.SoundType type, int index, GameObject obj, Action method_1 = null, float time = 5)
        {
          
            if (obj is null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (obj == bell)
            {
                mono.StartCoroutine(SpeckerCoroutine(type, index,
                    () =>
                    {
                        mono.StartCoroutine(Wait(time,
                            () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                                SpineManager.instance.DoAnimation(_robot, "animation", false,
                                    () =>
                                    {
                                        SpineManager.instance.DoAnimation(_robot, "animation3", false,
                                            () =>
                                            {
                                                SpineManager.instance.DoAnimation(_robot, "animation2", false,
                                                    () =>
                                                    {
                                                        SpineManager.instance.DoAnimation(_robot, "daiji", false,
                                                            () =>
                                                            {
                                                                if (method_1 != null)
                                                                {
                                                                    time = 0;
                                                                    method_1();
                                                                }
                                                            });
                                                        SoundManager.instance.ShowVoiceBtn(false);
                                                    });
                                            });
                                    });
                            }));
                    },
                    () => { }));
            }

            else
            {
                mono.StartCoroutine(SpeckerCoroutineMax(type, index,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        SpineManager.instance.DoAnimation(_robot, "animation", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_robot, "animation3", false,
                                    () =>
                                    {
                                        SpineManager.instance.DoAnimation(_robot, "animation2", false,
                                            () =>
                                            {
                                                SpineManager.instance.DoAnimation(_robot, "daiji", false,
                                                    () =>
                                                    {
                                                        if (method_1 != null)
                                                        {
                                                            method_1();
                                                        }
                                                    });
                                                SoundManager.instance.ShowVoiceBtn(false);
                                            });
                                    });
                            });
                    },
                    () => { }));
            }
        }

        void Sence()
        {
            _robot.Hide();
            _first.Hide();
            _p2.Hide();
            _p3.Hide();
            _p5.Hide();
            _p4.Hide();
            Debug.LogError("我使用了循环语句。");
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3,
                () =>
                {
                    _ani.Show();

                    for (int i = 1; i < 9; i++)
                    {
                        var name = i + "";
                        var time = SpineManager.instance.DoAnimation(_ani, name, false);
                        _times.Add(time);
                        _callBacks.Add(() => { SpineManager.instance.DoAnimation(_ani, name, false); });
                    }

                    mono.StartCoroutine(Delay(_times, 1, _callBacks));
                },
                () =>
                {
                    SpineManager.instance.DoAnimation(_ani, "8", true);
                    mono.StartCoroutine(SpeckerCoroutineMax(SoundManager.SoundType.VOICE, 5,
                        () => { Debug.LogError("同学们，接下来请尝试编写循环程序吧"); },
                        () =>
                        {
                            _mask.Hide();
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                }));
        }


        void Sence2()
        {
            _ani.Hide();
            bell.Hide();
            max.Hide();
            _image.Show();
            _Robots.Show();
            _Postion.Show();
            
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7,
                () => { _mask.Show(); Debug.LogError("机器人身体需要不断顺时针转圈。请大家拖曳正确的图片完成循环程序"); }, ()=> { _mask.Hide(); }));
        }


        /// <summary>
        /// 判断托拽是否完成
        /// </summary>
        /// <returns></returns>
        bool IsEnd()
        {
            if (_robots[0].transform.position == _pos[2].transform.position &&
                _robots[1].transform.position == _pos[0].transform.position &&
                _robots[2].transform.position == _pos[1].transform.position)
            {
                return true;
            }

            return false;
        }


        /// <summary>
        /// 点击播放动画
        /// </summary>
        /// <param name="index"></param>
        private void PlayAni(int index)
        {
            //按钮声音
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONBGM,1));

            //Debug.LogError(index);
            _end.Hide();
            Debug.LogError(index);
            if (index == 0)
                SpineManager.instance.DoAnimation(_robots[0].transform.GetChild(0).gameObject, "d2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_robots[0].transform.GetChild(0).gameObject, "d", false);
                    });
            if (index == 1)
                SpineManager.instance.DoAnimation(_robots[1].transform.GetChild(0).gameObject, "c2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_robots[1].transform.GetChild(0).gameObject, "c", false);
                    });
            if (index == 2)
                SpineManager.instance.DoAnimation(_robots[2].transform.GetChild(0).gameObject, "a2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(_robots[2].transform.GetChild(0).gameObject, "a", false);
                    });
            //if (index == 3) SpineManager.instance.DoAnimation(_Robots.transform.GetChild(3).gameObject, "a2", false, () => { SpineManager.instance.DoAnimation(_Robots.transform.GetChild(index).gameObject, "a", false); });
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        //动画过渡
        IEnumerator Delay(List<float> times, float delay, List<Action> callBacks)
        {
            int tempIndex = 0;

            while (tempIndex <= callBacks.Count - 1)
            {
                callBacks[tempIndex]?.Invoke();
                yield return new WaitForSeconds(times[tempIndex]);
                yield return new WaitForSeconds(delay);
                tempIndex++;
            }

            mono.StopCoroutine("Delay");
        }
    }
}