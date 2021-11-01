using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course923Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;

        private GameObject spineShow;

        private Transform panel;
        private Transform showPanel;
        private Transform panelPos;
        private GameObject mask;
        private GameObject success;


        ILDrager[] iLDragers;

        ILDroper[] iLDroper;

        int flag = 0;
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

            spineShow = curTrans.Find("spineShow").gameObject;
            spineShow.SetActive(true);
            panel = curTrans.Find("panel");
            panel.gameObject.SetActive(false);
            iLDragers = panel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < panel.childCount; i++)
            {
                iLDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
            panelPos = curTrans.Find("panelPos");
        
            showPanel = curTrans.Find("showPanel");
            showPanel.gameObject.SetActive(true);
            iLDroper = showPanel.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < showPanel.childCount; i++)
            {
                iLDroper[i].SetDropCallBack(OnAfter);
            }

            mask = curTrans.Find("mask").gameObject;
            success = curTrans.Find("mask/success").gameObject;
            mask.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            panelPos.gameObject.SetActive(false);
            talkIndex = 1;
            flag = 0;
            iLDragers = panel.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < showPanel.childCount; i++)
            {
                panel.GetChild(i).gameObject.SetActive(true);
                panel.GetChild(i).position = panelPos.GetChild(i).position;
                showPanel.GetChild(i).GetChild(0).gameObject.SetActive(false);
                iLDragers[i].index = i;
            }
            panelPos.gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(spineShow, "kong", false,()=> { SpineManager.instance.DoAnimation(spineShow, "ly11",true); });
            SpineManager.instance.DoAnimation(success,"kong",false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

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



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                SpineManager.instance.DoAnimation(spineShow, "kong", false);
                bell.SetActive(false);
                panel.gameObject.SetActive(true);
                panelPos.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () => { panelPos.gameObject.SetActive(false); }));
            }
            if (talkIndex == 2)
            {
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                mask.SetActive(false);
                bell.SetActive(true);
                panel.gameObject.SetActive(false);
                for (int i = 0; i < showPanel.childCount; i++)
                {             
                    showPanel.GetChild(i).GetChild(0).gameObject.SetActive(false);
                }         
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType%4 == dropType)
            {
                if (dragType==5)
                {
                    iLDroper[index].transform.GetChild(0).GetComponent<Image>().sprite = iLDroper[index].GetComponentInChildren<BellSprites>(true).sprites[0];
                }
                else if (dragType == 9)
                {
                    iLDroper[index].transform.GetChild(0).GetComponent<Image>().sprite = iLDroper[index].GetComponentInChildren<BellSprites>(true).sprites[1];
                }
                iLDroper[index].transform.GetChild(0).gameObject.SetActive(true);
            }
            return dragType % 4 == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            //SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            iLDragers[index].transform.SetAsLastSibling();
           iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            panelPos.gameObject.SetActive(true);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            if (!isMatch)
            {
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () => { panelPos.gameObject.SetActive(false); }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                iLDragers[index].transform.DOMove(panelPos.GetChild(index).position,1);
            }
            else
            {
                iLDragers[index].gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                if ((flag&(1<<index))<=0)
                {
                    flag += (1 << index);
                }

                iLDragers[index].transform.DOMove(panelPos.GetChild(index).position, 1).OnComplete(() => {

                    if (flag >= (Mathf.Pow(2, panel.childCount) - 1))
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
                        mask.SetActive(true);
                        SpineManager.instance.DoAnimation(success, "animation", false, () => { SpineManager.instance.DoAnimation(success, "animation2", true);
                            SoundManager.instance.ShowVoiceBtn(true);
                        });
                    }
                    panelPos.gameObject.SetActive(false);
                   
                });
               
               
            }
        }
    }
}
