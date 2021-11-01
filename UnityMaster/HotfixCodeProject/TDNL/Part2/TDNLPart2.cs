using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TDNLPart2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private Transform dtImg;
        private Transform element;
        private GameObject successPanel;
        private int totalNum = 0;
        bool isPlaying = false;

        Vector3 elementPos;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            bell = curTrans.Find("bell").gameObject;
            bell.SetActive(false);
            dtImg = curTrans.Find("dPanel/dt");

            totalNum = dtImg.childCount;
            for (int i = 0; i < totalNum; i++)
            {
                dtImg.GetChild(i).gameObject.SetActive(false);
            }

            element = curTrans.Find("element");
            for (int i = 0; i < element.childCount; i++)
            {
                element.GetChild(i).gameObject.SetActive(true);
                Util.AddBtnClick(element.GetChild(i).gameObject, onClickImg);
            }
            successPanel = curTrans.Find("successPanel").gameObject;
            successPanel.SetActive(false);

            elementPos = Vector3.zero;
            isPlaying = false;

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
        }
      
        private void onClickImg(GameObject obj)
        {
            if (isPlaying)
            {
                return;
            }
            isPlaying = true;
            elementPos = obj.transform.position;
            bool isImg = false;
            int temI = 0;
            for (int i = 0; i < dtImg.childCount; i++)
            {
                if (dtImg.GetChild(i).name == obj.name)
                {
                    isImg = true;
                    temI = i;
                    totalNum--;
                    break;
                }
            }

            if (isImg)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4,13), () =>
                {
                    SpineManager.instance.DoAnimation(obj, obj.name + "2", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            obj.transform.DOMove(dtImg.GetChild(temI).position, 0.5f).OnComplete(
                        () =>
                        {
                            SpineManager.instance.DoAnimation(obj, obj.name + "4", false,
                        () =>
                        {
                            obj.SetActive(false); obj.transform.position = elementPos; dtImg.GetChild(temI).gameObject.SetActive(true);  isPlaying = false;
                            if (totalNum <= 0)
                            {
                                isPlaying = true;
                                successPanel.SetActive(true);
                            }
                        });
                        });
                        });
                }
                       ));
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), () => { SpineManager.instance.DoAnimation(obj, obj.name + "3", false, () => { SpineManager.instance.DoAnimation(obj, obj.name, true); }); }, () => { isPlaying = false; }));

            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            for (int i = 0; i < element.childCount; i++)
            {
                mono.StartCoroutine(PlayElements(i));
                //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONSOUND, 0, () => { SpineManager.instance.DoAnimation(element.GetChild(i).gameObject, element.GetChild(i).name, true); }, () => { isPlaying = false; },i));
            }

        }

        IEnumerator PlayElements(int index) {
            yield return new WaitForSeconds(index*0.1f);
            SpineManager.instance.DoAnimation(element.GetChild(index).gameObject, element.GetChild(index).name, true);
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

    }
}
