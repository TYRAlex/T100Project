using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class FrogePart2
    {
        GameObject curGo;
        private GameObject[] problem;
        private GameObject bell;
        private GameObject bj0;
        private GameObject bj1;
        private GameObject bj2;
        private GameObject b;
        private GameObject img;

        private int talkIndex;
        private MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            GameObject problems = curTrans.Find("Problem").gameObject;
            bell = curTrans.Find("Bell").gameObject;
            bj0 = curTrans.Find("bj/bj0").gameObject;
            bj1 = curTrans.Find("bj/bj1").gameObject;
            bj2 = curTrans.Find("bj/bj2").gameObject;
            b = curTrans.Find("B").gameObject;
            img = bj2.transform.Find("Image").gameObject;

            problem = new GameObject[problems.transform.childCount];
            for(int i = 0; i < problems.transform.childCount; i++)
            {
                problem[i] = problems.transform.GetChild(i).gameObject;
            }
            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        private void GameInit()
        {
            talkIndex = 1;
            bj0.SetActive(true);
            bj1.SetActive(false);
            bj2.SetActive(false);
            img.SetActive(false);
            for(int i= 0; i < problem.Length; i++)
            {
                problem[i].SetActive(true);
                problem[i].transform.localScale = new Vector3(0, 0, 0);
            }
            SoundManager.instance.SetVoiceBtnEvent(DoTalk);
            Debug.Log("初始化成功");
            GameStart();
        }

        private void GameStart()
        {
            Debug.Log("游戏开始");
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            problem[0].transform.DOScale(new Vector3(1, 1, 1), 0.5f);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }

        private void DoTalk()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    bj0.SetActive(false);
                    bj1.SetActive(true);
                    problem[0].SetActive(false);
                    problem[1].transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                    float len = SpineManager.instance.DoAnimation(b, "b1", false);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () =>
                    {
                        SpineManager.instance.DoAnimation(b, "b2", true);
                    }, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }, len));
                }, 0, 0.5f));
            }
            else if (talkIndex == 2)
            {
                bj0.SetActive(false);
                bj1.SetActive(false);
                bj2.SetActive(true);
                problem[1].SetActive(false);
                SpineManager.instance.DoAnimation(b, "c1", false);
                problem[2].transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { }, () =>
                {
                    mono.StartCoroutine(DoTalk_2());
                }));

            }
            talkIndex++;
        }

        IEnumerator DoTalk_2()
        {
            float len = SpineManager.instance.DoAnimation(b, "c2", false); ;
            yield return new WaitForSeconds(len);
            SpineManager.instance.DoAnimation(b, "c4", false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { }, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => {
                    problem[2].SetActive(false);
                    problem[3].transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                    img.SetActive(true);
                    b.SetActive(false);
                }, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => {
                        problem[3].SetActive(false);
                        img.SetActive(false);
                        b.SetActive(true);
                        problem[4].transform.DOScale(new Vector3(1, 1, 1), 0.5f);
                        SpineManager.instance.DoAnimation(b, "d1", false);
                    }, () =>{
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => {
                            SpineManager.instance.DoAnimation(b, "e1", false);
                        }, () => { }));
                    }, 0, 0.5f));
                }));
            }, 0, 0.5f));
        }

        /// <summary>
        /// bell语音协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <param name="len2"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0, float len2 = 0)
        {
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            if (method_2 != null)
            {
                yield return new WaitForSeconds(len2);
                method_2();
            }
        }
    }
}
