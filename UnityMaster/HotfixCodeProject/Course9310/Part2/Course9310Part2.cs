using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course9310Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject bell;
        private Transform panelBox;
        private Transform panel;
        private Transform showPanel;
        private Transform panel2;

        private Transform cBtn;
        private Transform selectBg;
        private Transform selectPos;

        private GameObject btnStart;
        private GameObject btnBack;
        private GameObject successSpine;
        private bool isPlaying = false;

        ILDrager[] iLDragers;
        ILDroper[] iLDropers;
        SkeletonGraphic[] sGs;
        private int temIndex = 0;
        private int flag = 0;
        private bool isCorrect = false;
        private bool isCorrect2 = false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            bell = curTrans.Find("bell").gameObject;

            panelBox = curTrans.Find("panelBox");
            panelBox.gameObject.SetActive(false);
            panel = panelBox.Find("panel");
            for (int i = 0; i < panel.childCount; i++)
            {
                Util.AddBtnClick(panel.GetChild(i).gameObject, OnClickBtnPanel);
            }
            panel.gameObject.SetActive(true);
            showPanel = panelBox.Find("showPanel");
            iLDropers = showPanel.GetComponentsInChildren<ILDroper>(true);
            for (int i = 0; i < iLDropers.Length; i++)
            {
                iLDropers[i].SetDropCallBack(OnAfter);
            }
            showPanel.gameObject.SetActive(false);
            panel2 = panelBox.Find("panel2");
            for (int i = 0; i < panel2.childCount; i++)
            {
                Util.AddBtnClick(panel2.GetChild(i).gameObject, OnClickBtnSelect);
            }
            panel2.gameObject.SetActive(false);
            cBtn = panelBox.Find("c");
            Util.AddBtnClick(cBtn.GetChild(0).gameObject, OnClickCBtn);
            cBtn.gameObject.SetActive(false);
            selectBg = panelBox.Find("selectBg");
            iLDragers = selectBg.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < iLDragers.Length; i++)
            {
                iLDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }
            selectBg.gameObject.SetActive(false);
            selectPos = panelBox.Find("selectPos");
            selectPos.gameObject.SetActive(false);
            btnStart = curTrans.Find("btnStart").gameObject;
            Util.AddBtnClick(btnStart, OnClickBtnStart);
            btnStart.SetActive(true);
            btnBack = curTrans.Find("btnBack").gameObject;
            successSpine = curTrans.Find("btnBack/success").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);


            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }

        private void OnClickBtnPanel(GameObject obj)
        {
            btnBack.SetActive(true);
            BtnPlaySound();
            if (obj.name == "0")
            {
                showPanel.gameObject.SetActive(true);
                selectBg.gameObject.SetActive(true);
                selectPos.gameObject.SetActive(true);
                panel.GetChild(1).GetImage().raycastTarget = false;
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2, null, () => { btnBack.SetActive(false); }));
            }
            else
            {
                panel2.gameObject.SetActive(true);
                panel.GetChild(0).GetImage().raycastTarget = false;
                panel.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 4, null, () => { btnBack.SetActive(false); }));
            }
            obj.SetActive(false);
            cBtn.gameObject.SetActive(true);
        }

        private void OnClickBtnSelect(GameObject obj)
        {
            btnBack.SetActive(true);
            BtnPlaySound();

            temIndex = int.Parse(obj.name);

            for (int i = 0; i < panel2.childCount; i++)
            {
                if (i != int.Parse(obj.transform.name))
                {
                    SpineManager.instance.DoAnimation(panel2.GetChild(i).GetChild(1).gameObject, "0", false);
                }
            }
            SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation", false, () =>
            {
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation2", false, () =>
                {
                    btnBack.SetActive(false);
                });
            });

        }

        private void OnClickCBtn(GameObject obj)
        {
            btnBack.SetActive(true);
            BtnPlaySound();

            SpineManager.instance.DoAnimation(cBtn.gameObject, obj.name, false,
                () =>
                {
                    if (temIndex >= 0)
                    {
                        if (temIndex == 2)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                            SpineManager.instance.DoAnimation(panel2.GetChild(temIndex).GetChild(0).gameObject, "1", true);
                            SpineManager.instance.DoAnimation(panel2.GetChild(temIndex).GetChild(1).gameObject, "0", false,
                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                                    isCorrect = true;
                                    temIndex = -1;
                                    for (int i = 0; i < panel.childCount; i++)
                                    {
                                        panel.GetChild(i).GetImage().raycastTarget = true;
                                    }
                                    cBtn.gameObject.SetActive(false);
                                    if (isCorrect&& isCorrect2)
                                    {
                                        PlaySuccess();
                                    }
                                    else
                                    {
                                        isPlaying = false;
                                    }
                                });
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 5,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(panel2.GetChild(temIndex).GetChild(0).gameObject, "2", true);
                                    SpineManager.instance.DoAnimation(panel2.GetChild(temIndex).GetChild(1).gameObject, "0", false);
                                }, () => { isPlaying = false; temIndex = -1; }));
                        }

                    }
                    else
                    {
                        if (flag >= Mathf.Pow(2, showPanel.childCount) - 1)
                        {
                            isCorrect2 = true;
                            selectBg.gameObject.SetActive(false);
                            selectPos.gameObject.SetActive(false);
                            cBtn.gameObject.SetActive(false);
                            for (int i = 0; i < panel.childCount; i++)
                            {
                                panel.GetChild(i).GetImage().raycastTarget = true;
                            }
                            flag = 0;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            if (isCorrect && isCorrect2)
                            {
                                PlaySuccess();
                            }
                            else
                            {
                                isPlaying = false;
                            }
                        }
                        else
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, null, () => { isPlaying = false; }));
                        }

                    }
                });
        }

        public void PlaySuccess()
        {          
            SpineManager.instance.DoAnimation(successSpine, "animation", false,
              () =>
              {
                  SpineManager.instance.DoAnimation(successSpine, "animation2", true);
                  isPlaying = false;
              });
        }
        private void OnClickBtnStart(GameObject obj)
        {
            btnBack.SetActive(true);
            obj.SetActive(false);
            panelBox.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () =>
           {
               bell.SetActive(false);
               btnBack.SetActive(false);
           }));
        }

        private void OnClickBtnBack(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;

            obj.SetActive(false);
            if (selectBg.gameObject.activeSelf)
            {
                for (int i = 0; i < selectBg.childCount; i++)
                {
                    selectBg.GetChild(i).gameObject.SetActive(true);
                }
            }

            if (isCorrect)
            {
                panel.gameObject.SetActive(true);
                panel2.gameObject.SetActive(false);
            }
            else
            {
                if (panel2.gameObject.activeSelf)
                {
                    for (int i = 0; i < sGs.Length; i++)
                    {
                        sGs[i].Initialize(true);
                    }
                }

            }
        }

        private void GameInit()
        {
            ILDrager[] temObjs = iLDragers.ToArray();
            for (int i = 0; i < temObjs.Length; i++)
            {
                int tem = int.Parse(temObjs[i].name);
                ILDrager temILDrager;
                temILDrager = iLDragers[tem];
                iLDragers[tem] = temObjs[i];
            }
            temIndex = -1;
            talkIndex = 1;
            flag = 0;
            isCorrect = false;
            isCorrect2 = false;
            for (int i = 0; i < panel.childCount; i++)
            {
                panel.GetChild(i).gameObject.SetActive(true);
                panel.GetChild(i).GetImage().raycastTarget = true;
            }
            for (int i = 0; i < showPanel.childCount; i++)
            {
                showPanel.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            sGs = panel2.GetComponentsInChildren<SkeletonGraphic>(true);
            for (int i = 0; i < sGs.Length; i++)
            {
                sGs[i].Initialize(true);
            }

            for (int i = 0; i < iLDragers.Length; i++)
            {
                iLDragers[i].transform.position = selectPos.GetChild(i).position;
                iLDragers[i].gameObject.SetActive(true);
            }

            successSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(successSpine, "kong", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bell.SetActive(true);
            btnBack.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { btnBack.SetActive(false); }));
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

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType % 2 == dropType)
            {
                if (dragType > dropType)
                {
                    iLDropers[index].transform.GetChild(0).GetComponent<Image>().sprite = iLDropers[index].GetComponentInChildren<BellSprites>(true).sprites[1];
                    if ((flag & (1 << index)) == 0)
                    {
                        flag += (1 << index);
                    }
                }
                else
                {
                    if ((flag & (1 << index)) > 0)
                    {
                        flag -= (1 << index);
                    }
                    iLDropers[index].transform.GetChild(0).GetComponent<Image>().sprite = iLDropers[index].GetComponentInChildren<BellSprites>(true).sprites[0];
                }

                iLDropers[index].transform.GetChild(0).gameObject.SetActive(true);
            }
            return dragType % 2 == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            iLDragers[index].transform.SetAsLastSibling();
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            btnBack.SetActive(true);
            if (!isMatch)
            {

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                iLDragers[index].transform.DOMove(selectPos.GetChild(index).position, 1).OnComplete(() => { btnBack.SetActive(false); });
            }
            else
            {
                iLDragers[index].gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

                iLDragers[index].transform.DOMove(selectPos.GetChild(index).position, 1).OnComplete(() =>
                {
                    iLDragers[index].gameObject.SetActive(true);
                    btnBack.SetActive(false);
                });

            }
        }
    }
}
