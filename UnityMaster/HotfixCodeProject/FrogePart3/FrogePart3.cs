using System;
using System.Collections;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class FrogePart3
    {
        GameObject curGo;
        private GameObject window;
        private GameObject problem;
        private GameObject c;
        private GameObject carPanel;
        private Drager car_1;
        private Drager car_2;
        private GameObject bell;

        private MonoBehaviour mono;

        private int talkIndex;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            window = curTrans.Find("bg/Window").gameObject;
            problem = curTrans.Find("bg/Window/Problem").gameObject;
            carPanel = curTrans.Find("CarPanel").gameObject;
            car_1 = carPanel.transform.Find("Car_1").GetComponent<Drager>();
            car_2 = carPanel.transform.Find("Car_2").GetComponent<Drager>();
            bell = curTrans.Find("Bell").gameObject;
            c = curTrans.Find("C").gameObject;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        private void GameInit()
        {
            talkIndex = 1;
            carPanel.SetActive(false);
            
            problem.transform.DOScale(new Vector3(0,1,1),0);
            SoundManager.instance.SetVoiceBtnEvent(DoTalk);
            c.transform.localPosition = new Vector3(-960, -540, 0);
            car_1.SetDragCallback(DragStart, Drag, DragEnd);
            car_2.SetDragCallback(DragStart, Drag, DragEnd);
            car_1.isActived = false;
            car_2.isActived = false;
            c.SetActive(false);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () =>
            {
                problem.transform.DOScale(new Vector3(1, 1, 1), 2);
            }, () => {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
            
        }

        private void DragStart(Vector3 pos, int index, int type)
        {

        }

        private void Drag(Vector3 pos, int index, int type)
        {

        }

        private void DragEnd(Vector3 pos, int index, int type, bool isMatch)
        {
            Drager obj;
            string aniName;
            int soundIndex = 0;
            if(index == 1)
            {
                obj = car_1;
                aniName = "2";
                soundIndex = 3;
            }
            else
            {
                obj = car_2;
                aniName = "3";
                soundIndex = 4;
            }

            if (isMatch)
            {
                obj.DoReset();
                obj.gameObject.SetActive(false);
                Debug.Log("........................" + obj.gameObject.name);
                float len = SpineManager.instance.DoAnimation(c, aniName, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, soundIndex, () => {
                    car_1.isActived = false;
                    car_2.isActived = false;
                }, () =>
                {
                    car_1.isActived = true;
                    car_2.isActived = true;
                    SoundManager.instance.ShowVoiceBtn(true);
                }, len));
            }
            else
            {
                obj.DoReset();
            }
        }

        private void DoTalk()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () =>
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
            }else if(talkIndex == 2)
            {
                carPanel.SetActive(true);
                c.SetActive(true);
                window.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => {
                    SpineManager.instance.DoAnimation(c, "1", false);
                }, () =>
                {
                    car_1.isActived = true;
                    car_2.isActived = true;
                    bell.SetActive(false);
                })); 
            }else if(talkIndex == 3)
            {
                carPanel.SetActive(false);
                c.transform.DOLocalMove(new Vector3(-960, -684.4f, 0), 1);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () =>
                {
                    SpineManager.instance.DoAnimation(c, "4", false);
                }, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { }, () => { }));
                }, 0, 0, 1));
            }
            talkIndex++;
        }


        /// <summary>
        /// bell”Ô“Ù–≠≥Ã
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <param name="len2"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0, float len2 = 0, float len0 = 0)
        {
            yield return new WaitForSeconds(len0);
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
