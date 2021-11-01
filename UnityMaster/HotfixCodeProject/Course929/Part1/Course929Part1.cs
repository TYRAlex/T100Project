using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum QuestionType
    {
        Single,         //单选
        Multiple,       //多选
        Judge,          //判断
    }

    public class Course929Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;
        private GameObject _mask;

        private GameObject _bigAni;

        private Transform _choose;
        private GameObject _ani;
        private Transform _question;
        private GameObject[] _questionBg;     //题目背景
        private QuestionType[] _questionType;     //题目类型
        private Transform _click;
        private GameObject[] _clickArray;   //每节课点击的父物体
        private Empty4Raycast[] _clickEmpty;    //所有点击物体

        Dictionary<int, int[]> _answerDic;  //答案字典,键为_level, 值为答案数组
        private GameObject _a;  //上一题
        private GameObject _b;  //下一题
        private GameObject _c;  //确认
        private GameObject _maskC;  //遮挡确认键的遮罩

        private int _level;
        private bool _canClick;
        //private GameObject _click;
        //private Transform _level1;
        //private Transform _level2;
        //private Transform _level3;
        //private GameObject[] _answer1;
        //private GameObject[] _answer2;
        //private GameObject[] _answer3;
        //private GameObject _lastClickObj;

        //private int _level;
        //private bool _canClick;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;
            _bigAni = curTrans.Find("bigAni").gameObject;

            _choose = curTrans.Find("Choose");
            _maskC = _choose.GetGameObject("mask");
            _ani = _choose.Find("ani").gameObject;
            //问题相关
            _question = _choose.Find("Question");
            _questionBg = new GameObject[_question.childCount];
            for (int i = 0; i < _question.childCount; i++)
            {
                _questionBg[i] = _question.GetChild(i).gameObject;
            }

            //点击物体相关
            _click = _choose.Find("Click");
            _clickArray = new GameObject[_click.childCount];
            for (int i = 0; i < _click.childCount; i++)
            {
                _clickArray[i] = _click.GetChild(i).gameObject;
            }
            _clickEmpty = _click.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0; i < _clickEmpty.Length; i++)
            {
                Util.AddBtnClick(_clickEmpty[i].gameObject, ClickEvent);
            }

            //上下题与确认
            _a = _choose.GetGameObject("a");
            _b = _choose.GetGameObject("b");
            _c = _choose.GetGameObject("c");
            Util.AddBtnClick(_a.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_b.transform.GetChild(0).gameObject, ChangeQuestion);
            Util.AddBtnClick(_c.transform.GetChild(0).gameObject, SureClick);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            #region 需要进行修改的部分
            //添加选择题类型
            _questionType = new QuestionType[4] { QuestionType.Single, QuestionType.Single, QuestionType.Single, QuestionType.Single };

            //添加正确选项
            _answerDic = new Dictionary<int, int[]>();
            _answerDic.Add(0, new int[1] { 0 });
            _answerDic.Add(1, new int[1] { 3 });
            _answerDic.Add(2, new int[1] { 2 });
            _answerDic.Add(3, new int[1] { 1 });
            #endregion

            ChooseInit();
            GameStart();
        }

        private void ChooseInit()
        {
            talkIndex = 1;
            _level = 0;
            _canClick = false;
            _maskC.Hide();

            SpineManager.instance.DoAnimation(_ani, "kong", false);
            SpineManager.instance.DoAnimation(_a, _a.name, false);
            SpineManager.instance.DoAnimation(_b, _b.name, false);
            SpineManager.instance.DoAnimation(_c, _c.name, false);
            _mask.Hide();
            bell.Hide();
            _choose.gameObject.Hide();
        }


        void GameStart()
        {
            _bigAni.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_bigAni, "animation", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, 
            () => 
            {
                mono.StartCoroutine(WaitCoroutine(
                () => 
                {
                    bell.Hide();
                    _choose.gameObject.Show();

                    ShowCurQuestion();
                    CheckBtnShow();
                }, 1.5f));
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
                speaker = bell;
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
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

        //根据等级show题目与点击区域
        void ShowCurQuestion()
        {
            _canClick = false;
            //先隐藏所有题目与点击区域
            for (int i = 0; i < _questionBg.Length; i++)
            {
                _questionBg[i].Hide();
            }
            for (int i = 0; i < _clickArray.Length; i++)
            {
                _clickArray[i].Hide();
            }

            //展示对应的背景与点击区域，隐藏掉确认反馈的头像和选中效果
            _questionBg[_level].Show();
            for (int i = 0; i < _questionBg[_level].transform.childCount; i++)
            {
                _questionBg[_level].transform.GetChild(i).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(_questionBg[_level].transform.GetChild(i).gameObject, "kong", false);
            }
            _clickArray[_level].Show();

            string a = _level == 0 ? null : (_level + 1).ToString();
            _ani.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            if (a == "2")
                SpineManager.instance.DoAnimation(_ani, "animation3", false);
            else if (a == "3")
                SpineManager.instance.DoAnimation(_ani, "animation2", false);
            else
                SpineManager.instance.DoAnimation(_ani, "animation" + a, false);
            for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
            {
                Transform tra = _clickArray[_level].transform.GetChild(i).GetChild(0);
                tra.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(tra.gameObject, "0", false);
            }

            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, _level + 1, null, () => { _canClick = true; }));
        }

        //是否隐藏上下题
        void CheckBtnShow()
        {
            if (_level == 0)
            {
                _a.Hide();
                _b.Show();
            }
            else if (_level == _questionBg.Length - 1)
            {
                _a.Show();
                _b.Hide();
            }
            else
            {
                _a.Show();
                _b.Show();
            }
        }

        //上下题
        private void ChangeQuestion(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                _maskC.Hide();
                GameObject o = obj.transform.parent.gameObject;

                SoundManager.instance.PlayClip(9);
                SpineManager.instance.DoAnimation(o, o.name + "2", false,
                () =>
                {
                    if (obj.name == "a")
                        _level--;
                    else
                        _level++;

                    _canClick = true;
                    ShowCurQuestion();
                    CheckBtnShow();
                });
            }
        }

        //点击选项
        private void ClickEvent(GameObject obj)
        {
            if (_questionType[_level] != QuestionType.Multiple)
            {
                if (_canClick)
                {
                    _canClick = false;
                    if (SpineManager.instance.GetCurrentAnimationName(obj.transform.GetChild(0).gameObject) == "0")
                    {
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            SpineManager.instance.DoAnimation(_clickArray[_level].transform.GetChild(i).GetChild(0).gameObject, "0", false);
                        }

                        SoundManager.instance.PlayClip(9);
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "0", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                }
            }
            else
            {
                if (_canClick)
                {
                    _canClick = false;
                    SoundManager.instance.PlayClip(9);
                    if (SpineManager.instance.GetCurrentAnimationName(obj.transform.GetChild(0).gameObject) == "0")
                    {
                        obj.transform.GetChild(0).gameObject.Show();
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                    else
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "0", false,
                        () =>
                        {
                            _canClick = true;
                        });
                    }
                }
            }
        }

        private void SureClick(GameObject obj)
        {
            if (_canClick)
            {
                _canClick = false;
                SoundManager.instance.PlayClip(9);
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.transform.parent.gameObject.name + "2", false,
                () =>
                {
                    if (_questionType[_level] != QuestionType.Multiple)
                    {
                        int answerIndex = 10;   //初始值为10，模拟不选的错误情况
                        //获得选中的是哪个选项
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                answerIndex = i;
                            }
                        }

                        _clickArray[_level].Hide();

                        if (answerIndex == 10)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            GameObject head = _questionBg[_level].transform.GetChild(_answerDic[_level][0]).gameObject;
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "3", true, () => { _canClick = true; });
                        }
                        else if (answerIndex == _answerDic[_level][0])
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                            GameObject head = _questionBg[_level].transform.GetChild(answerIndex).gameObject;
                            head.Show();
                            SpineManager.instance.DoAnimation(head, "1", true, () => { _canClick = true; });
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            GameObject head1 = _questionBg[_level].transform.GetChild(answerIndex).gameObject;
                            GameObject head2 = _questionBg[_level].transform.GetChild(_answerDic[_level][0]).gameObject;
                            head1.Show();
                            head2.Show();
                            SpineManager.instance.DoAnimation(head1, "2", true);
                            SpineManager.instance.DoAnimation(head2, "1", true, () => { _canClick = true; });
                        }
                    }
                    else
                    {
                        int index = 0;
                        //获得选中的选项数量
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                index++;
                            }
                        }

                        int[] answerIndex = new int[index];
                        index = 0;
                        //储存选中的选项
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            GameObject o = _clickArray[_level].transform.GetChild(i).GetChild(0).gameObject;
                            if (SpineManager.instance.GetCurrentAnimationName(o) != "0")
                            {
                                answerIndex[index] = i;
                            }
                        }

                        bool _isFalse = false;  //判断是否应播错误音效
                        //遍历的所有选项判断选项对错与漏选状况
                        for (int i = 0; i < _clickArray[_level].transform.childCount; i++)
                        {
                            bool _trueAns = false;  //遍历的所有选项里是否有正确的选项
                            bool _clickAns = false;  //遍历的所有选项里是否有选中的选项

                            for (int j = 0; j < _answerDic[_level].Length; j++)
                            {
                                if (_answerDic[_level][j] == i)
                                {
                                    _trueAns = true;
                                    j = _answerDic[_level].Length;
                                }
                            }
                            if (SpineManager.instance.GetCurrentAnimationName(_clickArray[_level].transform.GetChild(i).GetChild(0).gameObject) != "0")
                                _clickAns = true;

                            //总共四种情况
                            if (_trueAns && _clickAns)
                            {
                                //两者都有，则选对了
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "1", true);
                            }
                            else if (_trueAns && !_clickAns)
                            {
                                //正确选项有但没选中，则为漏选
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "3", true);
                                _isFalse = true;
                            }
                            else if (!_trueAns && _clickAns)
                            {
                                //正确选项没有但选中，则为错选
                                GameObject head = _questionBg[_level].transform.GetChild(i).gameObject;
                                head.Show();
                                SpineManager.instance.DoAnimation(head, "2", true);
                                _isFalse = true;
                            }
                            else
                            {
                                //正确选项没有也没选中，不做操作
                            }
                        }

                        if (_isFalse)
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                        else
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                        _clickArray[_level].Hide();
                        _canClick = true;
                    }

                    _maskC.Show();
                });
            }
        }
    }
}
