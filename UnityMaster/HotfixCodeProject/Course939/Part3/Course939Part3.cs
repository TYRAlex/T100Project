using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course939Part3
    {

        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform questions;
        private Transform btns;
        private GameObject btnBack;

        private int curQuestion = 0;

        public int CurQuestion
        {
            get => curQuestion;

            set
            {
                if (value < 0)
                {
                    curQuestion = 0;
                }
                else if (value > 2)
                {
                    curQuestion = 2;
                }
                else
                {
                    curQuestion = value;

                }
            }
        }

        List<Transform> temSelectTranList;
        string[] answers;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            questions = curTrans.Find("questions");
            for (int i = 0; i < questions.childCount; i++)
            {
                for (int j = 0; j < questions.GetChild(i).childCount; j++)
                {
                    Util.AddBtnClick(questions.GetChild(i).GetChild(j).gameObject, OnClickBtnSelect);
                }

            }

            btns = curTrans.Find("btns");

            for (int i = 0; i < btns.childCount; i++)
            {
                Util.AddBtnClick(btns.GetChild(i).GetChild(0).gameObject, OnClickBtn);
            }
            btnBack = curTrans.Find("btnBack").gameObject;
            btnBack.SetActive(false);
            CurQuestion = 0;

            temSelectTranList = new List<Transform>();
            answers = new string[] { "3", "0", "03" };
            SoundManager.instance.ShowVoiceBtn(false);
            GameInit();
            GameStart();
        }



        private void OnClickBtnSelect(GameObject obj)
        {
            if (answers[CurQuestion] == "")
            {
                Debug.LogError("~no mark right answer!");
                return;
            }
            BtnPlaySound();
            btnBack.SetActive(true);
            if (temSelectTranList.Contains(obj.transform))
            {
                temSelectTranList.Remove(obj.transform);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "0", false, () => { btnBack.SetActive(false); });
            }
            else
            {
                if (answers[CurQuestion].Length <= 1)
                {
                    UpdateCurQuestionPanel();
                    temSelectTranList.Clear();
                }
                temSelectTranList.Add(obj.transform);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation2", false, () => { btnBack.SetActive(false); });
                });
            }
        }

        private void OnClickBtn(GameObject obj)
        {
            obj.SetActive(false);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                () =>
                {
                    UpdateCurQuestionPanel();
                    if (obj.transform.parent.name == "a")
                    {
                        if (CurQuestion <= 0)
                            return;
                        btnBack.SetActive(false);
                        CurQuestion--;
                        UpdateQuestions();
                    }
                    else if (obj.transform.parent.name == "b")
                    {
                        if (CurQuestion >= (questions.childCount - 1))
                            return;
                        btnBack.SetActive(false);
                        CurQuestion++;
                        UpdateQuestions();
                    }
                    else
                    {
                        SelectStatus(temSelectTranList);
                    }
                });
        }

        /// <summary>
        /// 一级问题面板刷新
        /// </summary>
        private void UpdateQuestions()
        {
            temSelectTranList.Clear();
            btns.GetChild(0).gameObject.SetActive(CurQuestion > 0);
            btns.GetChild(1).gameObject.SetActive(CurQuestion < questions.childCount - 1);
            for (int i = 0; i < btns.childCount; i++)
            {
                btns.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            for (int i = 0; i < questions.childCount; i++)
            {
                if (CurQuestion == i)
                {
                    questions.GetChild(i).gameObject.SetActive(true);
                }
                else
                {
                    questions.GetChild(i).gameObject.SetActive(false);
                }
            }
            UpdateCurQuestionPanel();
        }

        void GameInit()
        {

            for (int i = 0; i < btns.childCount; i++)
            {
                SpineManager.instance.DoAnimation(btns.GetChild(i).gameObject, btns.GetChild(i).name, false);
            }
            UpdateQuestions();
        }
        void UpdateCurQuestionPanel()
        {
            for (int j = 0; j < questions.GetChild(CurQuestion).childCount; j++)
            {
                questions.GetChild(CurQuestion).GetChild(j).GetChild(0).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
                questions.GetChild(CurQuestion).GetChild(j).GetChild(1).gameObject.GetComponent<SkeletonGraphic>().Initialize(true);
            }
        }

        void GameStart()
        {

        }

        /// <summary>
        /// 确定提交刷新面板
        /// </summary>
        /// <param name="tran"></param>
        private void SelectStatus(List<Transform> trans)
        {
            btnBack.SetActive(true);
            if (trans.Count <= 0)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                for (int i = 0; i < questions.GetChild(CurQuestion).childCount; i++)
                {
                    for (int j = 0; j < answers[CurQuestion].Length; j++)
                    {
                        if (questions.GetChild(CurQuestion).GetChild(i).name == answers[CurQuestion][j].ToString())
                        {
                            int tem = i;
                            SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(0).gameObject, "31", false, () =>
                            {
                                SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(tem).GetChild(0).gameObject, "3", true);
                            });
                            SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(1).gameObject, "0", false);
                        }
                    }
                }
            }
            else
            {

                if (answers[CurQuestion].Length <= 1)
                {
                    if (trans[0].name == answers[CurQuestion])
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                        SpineManager.instance.DoAnimation(trans[0].GetChild(0).gameObject, "11", false, () =>
                        {
                            SpineManager.instance.DoAnimation(trans[0].GetChild(0).gameObject, "1", true);
                        });
                        SpineManager.instance.DoAnimation(trans[0].GetChild(1).gameObject, "0", false);
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                        SpineManager.instance.DoAnimation(trans[0].GetChild(0).gameObject, "21", false, () =>
                        {
                            SpineManager.instance.DoAnimation(trans[0].GetChild(0).gameObject, "2", true);
                        });
                        SpineManager.instance.DoAnimation(trans[0].GetChild(1).gameObject, "0", false);
                        for (int i = 0; i < questions.GetChild(CurQuestion).childCount; i++)
                        {
                            if (questions.GetChild(CurQuestion).GetChild(i).name == answers[CurQuestion])
                            {
                                int tem = i;
                                SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(0).gameObject, "11", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(tem).GetChild(0).gameObject, "1", true);
                                });
                                SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(1).gameObject, "0", false);
                            }

                        }
                    }
                }
                else
                {

                    for (int i = 0; i < questions.GetChild(CurQuestion).childCount; i++)
                    {
                        for (int j = 0; j < answers[CurQuestion].Length; j++)
                        {
                            if (questions.GetChild(CurQuestion).GetChild(i).name == answers[CurQuestion][j].ToString())
                            {
                                int tem = i;
                                SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(0).gameObject, "31", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(tem).GetChild(0).gameObject, "3", true);
                                });
                                SpineManager.instance.DoAnimation(questions.GetChild(CurQuestion).GetChild(i).GetChild(1).gameObject, "0", false);
                            }
                        }
                    }

                    int flag = 0;
                    for (int i = 0; i < trans.Count; i++)
                    {
                        for (int j = 0; j < answers[CurQuestion].Length; j++)
                        {
                            if (trans[i].name.Equals(answers[CurQuestion][j].ToString()))
                            {
                                if ((flag & (1 << i)) == 0)
                                {
                                    flag += (1 << i);
                                }
                                int tem = i;
                                SpineManager.instance.DoAnimation(trans[i].GetChild(0).gameObject, "11", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(trans[tem].GetChild(0).gameObject, "1", true);
                                });
                                SpineManager.instance.DoAnimation(trans[i].GetChild(1).gameObject, "0", false);
                            }
                        }
                    }
                    for (int i = 0; i < trans.Count; i++)
                    {
                        if ((flag & (1 << i)) <= 0)
                        {
                            int tem = i;
                            SpineManager.instance.DoAnimation(trans[i].GetChild(0).gameObject, "21", false, () =>
                            {
                                SpineManager.instance.DoAnimation(trans[tem].GetChild(0).gameObject, "2", true);
                            });
                            SpineManager.instance.DoAnimation(trans[i].GetChild(1).gameObject, "0", false);
                        }
                    }

                    if (flag == Mathf.Pow(2, answers[CurQuestion].Length) - 1 && trans.Count == answers[CurQuestion].Length)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                    }

                }
                //trans.Clear();
            }



        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
