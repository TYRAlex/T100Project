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
    public class Course918Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;

        GameObject _mask;
        GameObject _open;
        GameObject _start;
        GameObject _tempDown;
        GameObject _tempUp;
        GameObject _speedDown;
        GameObject _speedUp;
        GameObject _timeDown;
        GameObject _timeUp;
        Text _tempText;
        Text _speedText;
        Text _timeText;
        GameObject _machines;
        GameObject _lid;
        GameObject _tube;

        BellSprites p1Textures;
        BellSprites p2Textures;


        BellSprites _overSpeed1;
        BellSprites _overSpeed2;

        BellSprites _tube1Sprites;
        BellSprites _tube2Sprites;
        bool _isOpen;

        bool _p1CanDrag;
        bool _p2CanDrag;

        mILDrager[] _tDrager;
        mILDroper[] _pos;

        int temp;
        int speed;
        int time;
        int startTime;
        int startSpeed;
        int startTemp;

        int level; //打开显示

        int tubesIndex; //匹配是否放置正确

        int touchIndex;//开启关闭

        Coroutine enumerator;

        private int _voice1Index;

        private int _voice2Index;
        GameObject _playAgain;//重玩
        void Start(object o)
        {
            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏
            btnTest = curTrans.Find("btnTest").gameObject;

            _p1CanDrag = true;
            _p2CanDrag = true;
            _isOpen = false;

            _mask = curTrans.GetGameObject("mask");


            _lid = curTrans.GetGameObject("Parent/lid");
            _machines = curTrans.GetGameObject("Parent/machines");
            _tube = curTrans.GetGameObject("Parent/tube");
            _open = _machines.transform.GetGameObject("open");
            _start = _machines.transform.GetGameObject("start");

            _tempDown = _machines.transform.GetGameObject("tempdown");
            _tempUp = _machines.transform.GetGameObject("tempup");
            _speedDown = _machines.transform.GetGameObject("speeddown");
            _speedUp = _machines.transform.GetGameObject("speedup");
            _timeDown = _machines.transform.GetGameObject("timedown");
            _timeUp = _machines.transform.GetGameObject("timeup");

            _speedText = _machines.transform.GetText("speed");
            _tempText = _machines.transform.GetText("temp");
            _timeText = _machines.transform.GetText("time");


            _playAgain = curTrans.GetGameObject("playagain");
            p1Textures = curTrans.GetGameObject("Parent/p1images").GetComponent<BellSprites>();
            p2Textures = curTrans.GetGameObject("Parent/p2images").GetComponent<BellSprites>();

            _tube1Sprites = curTrans.GetGameObject("Parent/shelves/shelve1/tubes/t1").GetComponent<BellSprites>();
            _tube2Sprites = curTrans.GetGameObject("Parent/shelves/shelve1/tubes/t2").GetComponent<BellSprites>();

            _overSpeed1 = curTrans.GetGameObject("Parent/shelves/shelve1/overspeed1").GetComponent<BellSprites>();
            _overSpeed2 = curTrans.GetGameObject("Parent/shelves/shelve1/overspeed2").GetComponent<BellSprites>();


            _tDrager = new mILDrager[curTrans.GetGameObject("Parent/shelves/shelve1/tubes").transform.childCount];
            _pos = new mILDroper[2];

            for (int i = 0; i < curTrans.GetGameObject("Parent/shelves/shelve1/tubes").transform.childCount; i++)
            {
                _tDrager[i] = curTrans.GetGameObject("Parent/shelves/shelve1/tubes").transform.GetChild(i).gameObject
                    .GetComponent<mILDrager>();
            }

            foreach (var item in _tDrager)
            {
                item.SetDragCallback(Grow, null, SetPos, null);
            }

            _pos[0] = curTrans.GetGameObject("Parent/p1").GetComponent<mILDroper>();
            _pos[1] = curTrans.GetGameObject("Parent/p2").GetComponent<mILDroper>();

            _voice1Index = 0;
            _voice2Index = 0;
            temp = 35;
            speed = 3000;
            time = 5;
            level = 0;
            tubesIndex = 0;
            touchIndex = 1;
            startTime = time;
            startSpeed = speed;
            startTemp = temp;




            Util.AddBtnClick(_open, Open);
            Util.AddBtnClick(_start, Open);

            Util.AddBtnClick(_speedDown, Adjustments);
            Util.AddBtnClick(_speedUp, Adjustments);

            Util.AddBtnClick(_tempDown, Adjustments);
            Util.AddBtnClick(_tempUp, Adjustments);

            Util.AddBtnClick(_timeDown, Adjustments);
            Util.AddBtnClick(_timeUp, Adjustments);
            Util.AddBtnClick(p1Textures.gameObject, MoveTubes);
            Util.AddBtnClick(p2Textures.gameObject, MoveTubes);

            Util.AddBtnClick(btnTest, ReStart);
            Util.AddBtnClick(_playAgain, ReStart);
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

            p1Textures.gameObject.GetComponent<Image>().sprite = null;
            p2Textures.gameObject.GetComponent<Image>().sprite = null;


            _p1CanDrag = true;
            _p2CanDrag = true;

            _voice1Index = 0;
            _voice2Index = 0;
            temp = 35;
            speed = 3000;
            time = 5;
            level = 0;
            tubesIndex = 0;
            touchIndex = 1;
            startTime = time;
            startSpeed = speed;
            startTemp = temp;

            _mask.Hide();
            _isOpen = false;
            _lid.Show();
            SpineManager.instance.DoAnimation(_lid, "dk3", true);
            _machines.Show();
            _tube.Hide();
            _tempText.text = temp.ToString();
            _timeText.text = time.ToString();
            _speedText.text = speed.ToString();

            _tube1Sprites.gameObject.GetComponent<Image>().sprite = _tube1Sprites.sprites[0];
            _tube2Sprites.gameObject.GetComponent<Image>().sprite = _tube2Sprites.sprites[0];

            _tube1Sprites.gameObject.Hide();
            _tube2Sprites.gameObject.Hide();

            SpineManager.instance.DoAnimation(_lid, "dk3");

            SpineManager.instance.DoAnimation(_machines, "animation");

            foreach (var item in _tDrager)
            {
                item.gameObject.Show();
                item.DoReset();
            }

            if (obj == btnTest)
            {
                GameStart();
                bell.Show();
            }

            _playAgain.Hide();

            _overSpeed1.GetComponent<Image>().sprite = null;
            _overSpeed2.GetComponent<Image>().sprite = null;
            _overSpeed1.gameObject.Hide();
            _overSpeed2.gameObject.Hide();

            p1Textures.gameObject.Hide();
            p2Textures.gameObject.Hide();
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            btnTest.SetActive(false);
        }


        void GameStart()
        {

            //离心机高速转动时，会产生强大的离心力，以此来分离血浆等。你知道怎么正确使用离心机吗？请试试看。
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { _mask.Show(); },
                ()
                    =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { },
                        () =>
                        {
                            bell.Hide();
                            _mask.Hide();
                        }));
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
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
            }

            talkIndex++;
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


        IEnumerator Wait(float time, Action callBack1 = null, Action callBack2 = null)
        {
            if (callBack2 != null) callBack2();
            yield return new WaitForSeconds(time);
            if (callBack1 != null) callBack1();
        }


        /// <summary>
        /// 打开关闭
        /// </summary>
        /// <param name="obj"></param>
        void Open(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            if (obj == _open)
            {

                if (!_isOpen && touchIndex == 1)
                {
                    _mask.Show();
                    Debug.Log("Open");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(_machines, "animation2", false);
                    SpineManager.instance.DoAnimation(_lid, "dk", false, () =>
                    {
                        SpineManager.instance.DoAnimation(_lid, "dk4", false);
                        _isOpen = true;
                        _mask.Hide();
                    });
                    mono.StartCoroutine(Wait(0.5f, () => { ShowImages(); }));
                    startSpeed = 3000;
                    startTime = 5;
                    startTemp = 35;
                    temp = startTemp;
                    time = startTime;
                    speed = startSpeed;
                    _speedText.text = startSpeed.ToString();
                    _timeText.text = startTime.ToString();
                    _tempText.text = startTemp.ToString();
                }

                if (_isOpen && touchIndex == 1)
                {
                    Debug.Log("Close");
                    Debug.Log(tubesIndex);
                    _mask.Show();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                    SpineManager.instance.DoAnimation(_machines, "animation2", false);
                    if (p1Textures.GetComponent<Image>().sprite != null && p2Textures.GetComponent<Image>().sprite != null)
                    {
                        mono.StartCoroutine(Wait(2.5f,
                        () =>
                        {
                            p1Textures.gameObject.Hide();
                            p2Textures.gameObject.Hide();
                        }));
                    }
                    else
                    {
                        p1Textures.gameObject.Hide();
                        p2Textures.gameObject.Hide();
                    }


                    SpineManager.instance.DoAnimation(_lid, "dk2", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(_lid, "dk3", false);
                            _isOpen = false;
                            _mask.Hide();
                        });
                    _timeText.text = startTime.ToString();
                    _speedText.text = startSpeed.ToString();
                }
            }

            if (obj == _start)
            {
                if (_isOpen)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                }
                if (!_isOpen) //关闭
                {
                    touchIndex++;
                    if (touchIndex == 2 && time > 0)
                    {
                        startSpeed = speed;
                        startTemp = temp;
                        startTime = time;
                        SpineManager.instance.DoAnimation(_machines, "animation3", false);
                        Debug.Log("start");
                        speed = 0;
                        _speedText.text = speed.ToString();
                        Runing();
                    }

                    if (touchIndex >= 3)
                    {
                        touchIndex = 1;
                        Debug.Log("Stop");
                        SpineManager.instance.DoAnimation(_machines, "animation3", false,
                            () => { SpineManager.instance.DoAnimation(_machines, "animation", false); });
                        mono.StopCoroutine(enumerator);
                    }
                }
            }

            mono.StartCoroutine(Wait(1, () => { SpineManager.instance.DoAnimation(_machines, "animation", false); }));
        }


        /// <summary>
        /// 调整数值
        /// </summary>
        /// <param name="obj"></param>
        private void Adjustments(GameObject obj)
        {

            if (touchIndex == 1)
            {
                if (!_isOpen)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    if (obj == _speedDown)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation6", false);
                        speed -= 100;
                    }

                    if (obj == _speedUp)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation7", false);
                        speed += 100;
                    }


                    if (obj == _tempDown)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation4", false);
                        temp -= 1;
                    }

                    if (obj == _tempUp)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation5", false);
                        temp += 1;
                    }


                    if (obj == _timeDown)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation8", false);
                        time -= 1;
                    }

                    if (obj == _timeUp)
                    {
                        SpineManager.instance.DoAnimation(_machines, "animation9", false);
                        time += 1;
                    }
                }
            }


            if (time < 0)
            {
                time = 0;
            }
            if (time > 20)
            {
                time = 20;
            }

            if (speed < 0)
            {
                speed = 0;
            }

            if (speed > 10000)
            {
                speed = 10000;
            }
            if (temp < 0)
            {
                temp = 0;
            }
            if (temp > 40)
            {
                temp = 40;
            }

            _tempText.text = temp.ToString();
            _timeText.text = time.ToString();
            _speedText.text = speed.ToString();
            startSpeed = speed;
            startTime = time;
            startTemp = temp;
            mono.StartCoroutine(Wait(1, () => { SpineManager.instance.DoAnimation(_machines, "animation", false); }));
        }

        /// <summary>
        /// 拖拽中
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        private void Grow(Vector3 dragPos, int dragType, int dragIndex)
        {
            if (dragIndex == 4) dragIndex = 3;
            if (dragIndex == 6) dragIndex = 4;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1.1f, 1);
        }


        /// <summary>
        /// 拖拽成功判断
        /// </summary>
        /// <param name="dragPos"></param>
        /// <param name="dragType"></param>
        /// <param name="dragIndex"></param>
        /// <param name="dragBool"></param>
        private void SetPos(Vector3 dragPos, int dragType, int dragIndex, bool dragBool)
        {

            if (_isOpen)
            {
                if (dragBool)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    tubesIndex += dragIndex;
                    Debug.Log("tubesIndex:            " + tubesIndex);
                    if (dragIndex == 4) dragIndex = 3;
                    if (dragIndex == 6) dragIndex = 4;
                    var curDrag = _tDrager[dragIndex - 1];
                    var curDragType = curDrag.dragType;
                    mILDroper curPos = null;

                    if (dragPos.x < -100)
                    {
                        if (_p1CanDrag)
                        {
                            curPos = _pos[0];
                            curDrag.gameObject.transform.position = curPos.gameObject.transform.position;
                            curDrag.gameObject.transform.GetRectTransform().DOScale(1, 0.1f).OnComplete(
                                () =>
                                {
                                    curDrag.gameObject.Hide();
                                    p1Textures.gameObject.GetComponent<Image>().sprite =
                                        p1Textures.sprites[dragIndex - 1];
                                    p1Textures.gameObject.Show();
                                });
                            _p1CanDrag = false;
                        }
                        else
                        {
                            _tDrager[dragIndex - 1].DoReset();
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 0.1f);
                        }

                        level += 1;
                    }

                    else if (dragPos.x > -100)
                    {
                        if (_p2CanDrag)
                        {
                            curPos = _pos[1];
                            curDrag.gameObject.transform.position = curPos.gameObject.transform.position;
                            curDrag.gameObject.transform.GetRectTransform().DOScale(1, 0.1f).OnComplete(
                                () =>
                                {
                                    curDrag.gameObject.Hide();
                                    p2Textures.gameObject.GetComponent<Image>().sprite =
                                        p2Textures.sprites[dragIndex - 1];
                                    p2Textures.gameObject.Show();
                                });
                            _p2CanDrag = false;
                        }
                        else
                        {
                            _tDrager[dragIndex - 1].DoReset();
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 0.1f);
                        }

                        level += 2;
                    }
                }

                if (!dragBool)
                {
                    if (dragIndex == 4)
                    {
                        dragIndex = 3;
                        _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                            () => { });
                        _tDrager[dragIndex - 1].DoReset();
                    }

                    if (dragIndex == 6)
                    {
                        dragIndex = 4;
                        _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                            () => { });
                        _tDrager[dragIndex - 1].DoReset();
                    }

                    if (dragIndex == 1 || dragIndex == 2)
                    {

                        if (dragIndex == 1 && _overSpeed1.GetComponent<Image>().sprite != null)
                        {
                            _playAgain.Show();
                            Debug.Log("设置位置1");
                            _tDrager[dragIndex - 1].gameObject.transform.localPosition = new Vector3(568, 180);
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                                () => { });
                        }
                        else if (dragIndex == 1 && _overSpeed1.GetComponent<Image>().sprite == null)
                        {
                            Debug.Log("返回位置1");
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                                () => { });
                            _tDrager[dragIndex - 1].DoReset();
                        }

                        if (dragIndex == 2 && _overSpeed2.GetComponent<Image>().sprite != null)
                        {
                            _playAgain.Show();
                            Debug.Log("设置位置2");
                            _tDrager[dragIndex - 1].gameObject.transform.localPosition = new Vector3(670, 180);
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                                () => { });
                        }
                        else if (dragIndex == 2 && _overSpeed2.GetComponent<Image>().sprite == null)
                        {
                            Debug.Log("返回位置2");
                            _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1).OnComplete(
                                () => { });
                            _tDrager[dragIndex - 1].DoReset();
                        }
                    }
                }
            }
            else
            {
                if (dragIndex == 4) dragIndex = 3;
                if (dragIndex == 6) dragIndex = 4;
                _tDrager[dragIndex - 1].DoReset();
                _tDrager[dragIndex - 1].gameObject.transform.DOScale(1f, 1);
            }
        }


        /// <summary>
        /// 打开显示
        /// </summary>
        void ShowImages()
        {
            if (level == 1)
            {
                p1Textures.gameObject.Show();
            }

            if (level == 2)
            {
                p2Textures.gameObject.Show();
            }

            if (level > 2)
            {
                p1Textures.gameObject.Show();
                p2Textures.gameObject.Show();
            }
        }


        /// <summary>
        /// 拖出试管
        /// </summary>
        /// <param name="obj"></param>
        private void MoveTubes(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            for (int i = 0; i < obj.GetComponent<BellSprites>().sprites.Length; i++)
            {
                if (obj.GetComponent<Image>().sprite == obj.GetComponent<BellSprites>().sprites[i])
                {
                    _tDrager[i].gameObject.Show();
                    obj.Hide();
                    if (i == 0)
                    {
                        if (_voice1Index > 0)
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _voice1Index,
                                () => { _mask.Show(); }, () => { _mask.Hide(); }));
                        _voice1Index = -1;
                    }

                    if (i == 1)
                    {
                        if (_voice2Index > 0)
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _voice2Index,
                                () => { _mask.Show(); }, () => { _mask.Hide(); }));
                        _voice2Index = -1;
                    }

                    if (obj == p1Textures.gameObject)
                    {
                        _p1CanDrag = true;
                        level -= 1;
                    }
                    else
                    {
                        _p2CanDrag = true;
                        level -= 2;
                    }

                    tubesIndex = tubesIndex - _tDrager[i].index;
                    Debug.Log(tubesIndex);
                }
            }
        }


        /// <summary>
        /// 倒计时
        /// </summary>
        void TimeDown(Action callBack = null)
        {
            if (touchIndex == 2)
            {
                enumerator = mono.StartCoroutine(Wait(1.5f,
                    () =>
                    {
                        if (time > 0)
                        {
                            TimeDown();
                        }

                        if (time == 0)
                        {
                            Ending();

                            mono.StartCoroutine(Wait(0.1f, () =>
                            {
                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { },
                                    () => { bell.Hide(); }));
                            }));

                        }

                        time--;


                        speed += startSpeed / startTime;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                        if (speed > startSpeed)
                        {
                            speed = startSpeed;
                        }
                        if (time == 1)
                        {
                            speed = startSpeed;
                        }

                        if (time <= 0)
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                            time = 0;
                            speed = 0;
                            touchIndex = 1;
                            if (callBack != null)
                                callBack();
                        }

                        _speedText.text = speed.ToString();
                        _timeText.text = time.ToString();
                    }));
            }
        }

        /// <summary>
        /// 是否平衡
        /// </summary>
        /// <returns></returns>
        bool IsTrueSet()
        {
            if (tubesIndex == 3 || tubesIndex == 7 || tubesIndex == 8)
                return true;
            return false;
        }


        /// <summary>
        /// 机器运转
        /// </summary>
        void Runing()
        {

            Debug.LogError(touchIndex);
            if (IsTrueSet())
            {
                Debug.Log("机器运行");
                TimeDown(() =>
                {
                    speed = 0;
                    _speedText.text = speed.ToString();
                });
                SpineManager.instance.DoAnimation(_machines, "hd2", true);
                mono.StartCoroutine(
                    Wait(2f, () => { SpineManager.instance.DoAnimation(_machines, "animation", true); }));
            }

            if (!IsTrueSet())
            {
                Debug.Log("放置错误");
                TimeDown();
                SpineManager.instance.DoAnimation(_machines, "my", true);
                //mono.StartCoroutine(Wait(1f, () => { Debug.Log("冒烟");SpineManager.instance.DoAnimation(_machines, "my", true); }));
                //请关闭离心机、请关闭离心机。如3秒内未点击stop/start按钮，则出现爆炸效果。
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6,
                    () =>
                    {
                        mono.StartCoroutine(Wait(4,
                            () =>
                            {
                                if (touchIndex == 2)
                                {
                                    _mask.Show();
                                    _tube.Show();
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                                    SpineManager.instance.DoAnimation(_tube, "animation", false, () => { _mask.Hide(); });
                                    SpineManager.instance.DoAnimation(_machines, "my", false, () => { });
                                }
                            }));


                        if (touchIndex == 1)
                        {

                        }

                    },
                    () => { }));
            }
        }


        /// <summary>
        ///机器结束
        /// </summary>
        void Ending()
        {
            _isOpen = false;
            if (IsTrueSet())
            {

                _overSpeed1.GetComponent<Image>().sprite = null;
                _overSpeed2.GetComponent<Image>().sprite = null;
                if (tubesIndex == 7 || tubesIndex == 3)
                {
                    Debug.Log("yellow");
                    if (startSpeed <= 3000 || startTime <= 8)
                    {
                        //操作不当
                        Debug.Log("yellow 操作不当");
                        _voice1Index = 3;
                    }

                    if (3000 < startSpeed && startSpeed <= 3500 && 8 < startTime && startTime <= 15 && 2 <= startTemp &&
                        startTemp <= 37)
                    {
                        //分层
                        Debug.Log("yellow 分离成功");
                        _voice1Index = 5;
                        _tube1Sprites.gameObject.GetComponent<Image>().sprite = _tube1Sprites.sprites[2];
                        _overSpeed1.GetComponent<Image>().sprite = _overSpeed1.sprites[2];
                    }

                    if (3000 < startSpeed && startSpeed <= 3500 && startTime > 15 && 2 <= startTemp && startTemp <= 37)
                    {
                        //分层,时间过长
                        Debug.Log("yellow分离成功但超时");
                        _voice1Index = 7;
                        _tube1Sprites.gameObject.GetComponent<Image>().sprite = _tube1Sprites.sprites[2];
                        _overSpeed1.GetComponent<Image>().sprite = _overSpeed1.sprites[2];
                    }

                    if (3501 < startSpeed && startSpeed <= 7000)
                    {
                        //溶血1
                        Debug.Log("yellow 溶血1");
                        _voice1Index = 4;
                        _tube1Sprites.gameObject.GetComponent<Image>().sprite = _tube1Sprites.sprites[1];
                        _overSpeed1.GetComponent<Image>().sprite = _overSpeed1.sprites[1];
                    }

                    if (7001 < startSpeed && startSpeed < 10000)
                    {
                        //溶血2
                        Debug.Log("yellow 溶血2");
                        _voice1Index = 4;
                        _tube1Sprites.gameObject.GetComponent<Image>().sprite = _tube1Sprites.sprites[3];
                        _overSpeed1.GetComponent<Image>().sprite = _overSpeed1.sprites[0];
                    }
                }

                if (tubesIndex == 8 || tubesIndex == 3)
                {
                    Debug.Log("Green");
                    if (startSpeed <= 3000 || startTime <= 3)
                    {
                        //操作不当
                        Debug.Log("操作不当");
                        _voice2Index = 3;
                    }

                    if (3000 < startSpeed && startSpeed <= 3500 && 3 < startTime && startTime <= 5 && 2 <= startTemp &&
                        startTemp <= 37)
                    {
                        //分离成功
                        Debug.Log("Green 分离成功");
                        _voice2Index = 5;
                        _tube2Sprites.gameObject.GetComponent<Image>().sprite = _tube2Sprites.sprites[2];
                        _overSpeed2.GetComponent<Image>().sprite = _overSpeed2.sprites[2];
                    }

                    if (3000 < startSpeed && startSpeed <= 3500 && 5 < startTime && 2 <= startTemp && startTemp <= 37)
                    {
                        //分离成功超时
                        Debug.Log("Green 分离成功但超时");
                        _voice2Index = 7;
                        _tube2Sprites.gameObject.GetComponent<Image>().sprite = _tube2Sprites.sprites[2];
                        _overSpeed2.GetComponent<Image>().sprite = _overSpeed2.sprites[2];
                    }

                    if (3501 < startSpeed && startSpeed <= 7000)
                    {
                        //溶血1
                        Debug.Log("Green 溶血1");
                        _voice2Index = 4;
                        _tube2Sprites.gameObject.GetComponent<Image>().sprite = _tube2Sprites.sprites[1];
                        _overSpeed2.GetComponent<Image>().sprite = _overSpeed2.sprites[1];
                    }

                    if (7001 < startSpeed && startSpeed < 10000)
                    {
                        //溶血2
                        Debug.Log("Green 溶血2");
                        _voice2Index = 4;
                        _tube2Sprites.gameObject.GetComponent<Image>().sprite = _tube2Sprites.sprites[3];
                        _overSpeed2.GetComponent<Image>().sprite = _overSpeed2.sprites[0];
                    }
                }

                _overSpeed1.GetComponent<Image>().SetNativeSize();
                _overSpeed2.GetComponent<Image>().SetNativeSize();
            }
        }
    }
}