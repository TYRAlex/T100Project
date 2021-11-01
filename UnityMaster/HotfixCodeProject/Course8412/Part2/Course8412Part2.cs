using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course8412Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private Transform spineShow;

        private Transform shadow;
        private GameObject bell;

        private Transform gears;
        private Transform gearsPos;
        private Transform endPanel;
        private GameObject endSpine;
        private GameObject endSpineShow;

        bool isPlaying = false;

        private ILDrager[] iLDragers;

        private GameObject spineMask;
        private string[] names;
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

            spineShow = curTrans.Find("spineShow");
            spineShow.GetComponent<ILDroper>().SetDropCallBack(OnAfter);
            spineShow.GetChild(0).gameObject.SetActive(false);
            spineShow.gameObject.SetActive(true);

            gearsPos = curTrans.Find("gearsPos");
            gears = curTrans.Find("gears");
            gears.gameObject.SetActive(true);
            iLDragers = gears.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < gears.childCount; i++)
            {
                iLDragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
                iLDragers[i].index = i;
            }

            spineMask = curTrans.Find("spineMask").gameObject;
            spineMask.SetActive(false);
            shadow = curTrans.Find("shadow");
            shadow.gameObject.SetActive(false);
            bell = curTrans.Find("shadow/bell").gameObject;

            endPanel = curTrans.Find("ensPanel");
            endSpine = curTrans.Find("ensPanel/endSpine").gameObject;
            endSpineShow = curTrans.Find("ensPanel/endSpineShow").gameObject;
            endSpineShow.SetActive(false);
            endPanel.gameObject.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            names = new string[] { "c", "b", "d", "a" };
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;

            SpineManager.instance.DoAnimation(spineShow.gameObject, "cl", false);
            for (int i = 0; i < gears.childCount; i++)
            {
                gears.GetChild(i).name = names[i];
                gears.GetChild(i).position = gearsPos.GetChild(i).position;
                SpineManager.instance.DoAnimation(gears.GetChild(i).gameObject, names[i], false);
            }
            SpineManager.instance.DoAnimation(endSpine, "animation2", true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 7, true);
            isPlaying = true;
            spineMask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 0, null, () => { isPlaying = false; spineMask.gameObject.SetActive(false); }));
        }


        private bool OnAfter(int dragType, int index, int dropType)
        {
            spineMask.SetActive(true);
            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }


        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.SetAsLastSibling();
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            //iLDragers[index].DoReset();
            iLDragers[index].transform.position = gearsPos.GetChild(index).position;
            if (isMatch)
            {
                gears.gameObject.SetActive(false);
                shadow.gameObject.SetActive(true);

                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SpineManager.instance.DoAnimation(spineShow.gameObject, "animation", false,
                    () =>
                    {

                    });

                mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 1, null, () =>
                {
                    spineShow.GetChild(0).gameObject.SetActive(true);
                    mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 2,
                        () => { SpineManager.instance.DoAnimation(spineShow.GetChild(0).gameObject, spineShow.GetChild(0).name, false); }, () =>
                        {
                            shadow.gameObject.SetActive(false);
                            spineShow.gameObject.SetActive(false);
                            endPanel.gameObject.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(bell, SoundManager.SoundType.VOICE, 3, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,3,false);
                                SpineManager.instance.DoAnimation(endSpine, "animation", false, () =>
                                {
                                    SpineManager.instance.DoAnimation(endSpine, "animation3", true);
                                    endSpineShow.SetActive(true);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                                    SpineManager.instance.DoAnimation(endSpineShow, "dhk", false, () => { SpineManager.instance.DoAnimation(endSpineShow, "dhk2", false, () => { }); });
                                });
                            }, () => { }));

                        }));
                }));


            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            }
            spineMask.SetActive(false);
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


    }
}
