using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
namespace ILFramework.HotClass
{
    public class Course847Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        float a;
        float b;
        private GameObject dragBox;
        private GameObject[] allDrag;
        private mILDrager[] allMil;
        private GameObject _mask;
        private bool[] jugleroate;
        private bool[] juglemove;
        private GameObject clickBox;
        private GameObject insideBox;
        private GameObject moveBox;
        private Vector3 statrPos;
        private bool _canClick;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            statrPos = new Vector3();
            moveBox = curTrans.Find("moveBox").gameObject;
            insideBox = curTrans.Find("Inside").gameObject;
            clickBox = curTrans.Find("Click").gameObject;
            jugleroate = new bool[4];
            juglemove = new bool[3];
            _mask = curTrans.Find("mymask").gameObject;
            dragBox = curTrans.Find("DragBox").gameObject;
            allDrag = new GameObject[dragBox.transform.childCount];
            allMil = new mILDrager[allDrag.Length];

            for (int i = 0; i < allDrag.Length; i++)
            {
                allDrag[i] = dragBox.transform.GetChild(i).gameObject;
                allMil[i] = allDrag[i].GetComponent<mILDrager>();
                allMil[i].SetDragCallback(BeginDragEvent, DragingRoate, null, null);
            }

            Util.AddBtnClick(clickBox, ClickEvent);
            for (int i = 0; i < 3; i++)
            {
                moveBox.transform.GetChild(i).GetComponent<mILDrager>().SetDragCallback(BeginDragMoveEvent, null, EndMove, null);
                insideBox.transform.GetChild(i).GetComponent<mILDroper>().SetDropCallBack(null, null, DropFalse);
            }
            GameInit();
            GameStart();
        }

        private void BeginDragEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            a = dragPosition.x;
            b = dragPosition.x;
        }
        private void DragingRoate(Vector3 dragPosition, int dragType, int dragIndex)
        {
            a = b;
            b = Input.mousePosition.x;
            if (b > a)
            {
                allDrag[dragType].transform.GetChild(0).Rotate(0, -8, 0);
                jugleroate[dragType] = true;
                JugleRoate();
            }
            if (a > b)
            {
                allDrag[dragType].transform.GetChild(0).Rotate(0, 8, 0);
                jugleroate[dragType] = true;
                JugleRoate();
            }
        }
        private void JugleRoate()
        {
            for (int i = 0; i < jugleroate.Length; i++)
            {
                if (!jugleroate[i])
                { return; }
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void BeginDragMoveEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            statrPos = dragPosition;
        }

        private void EndMove(Vector3 dragPosition, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
                moveBox.transform.GetChild(dragType).gameObject.SetActive(false);
                insideBox.transform.GetChild(dragType).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(insideBox.transform.GetChild(dragType).GetChild(0).gameObject, insideBox.transform.GetChild(dragType).GetChild(0).gameObject.name + "2", true);
                juglemove[dragType] = true;
                JugleMove();
            }
            else
            {
                moveBox.transform.GetChild(dragType).gameObject.transform.localPosition = statrPos;
            }
        }

        private void DropFalse(int dropType)
        {
            //错误音效
            SoundManager.instance.PlayClip(1);
        }

        private void JugleMove()
        {
            Debug.Log(juglemove[0]);
            Debug.Log(juglemove[1]);
            Debug.Log(juglemove[2]);
            for (int i = 0; i < 3; i++)
            {
                if (!juglemove[i])
                { return; }
            }
            SoundManager.instance.ShowVoiceBtn(true);
            insideBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(insideBox, "zhuan", true);
            for (int t = 0; t < 3; t++)
            {
                insideBox.transform.GetChild(t).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(insideBox.transform.GetChild(t).GetChild(0).gameObject, insideBox.transform.GetChild(t).GetChild(0).gameObject.name + "3", true);
            }
        }
        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                BtnPlaySound();
                _canClick = false;
                obj.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation3", false,
                    () =>
                    {
                        clickBox.SetActive(false);
                        insideBox.SetActive(true);
                        insideBox.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                        SpineManager.instance.DoAnimation(insideBox, "kong3", false,
                            () =>
                            {
                                for (int i = 0; i < 3; i++)
                                {
                                    insideBox.transform.GetChild(i).gameObject.SetActive(true);
                                }
                                SpineManager.instance.DoAnimation(insideBox, "kong", true);
                                Bg.transform.GetChild(0).gameObject.SetActive(true);
                                moveBox.SetActive(true);
                                mono.StartCoroutine(WaitTime(2f,
                        () =>
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                moveBox.transform.GetChild(i).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                SpineManager.instance.DoAnimation(moveBox.transform.GetChild(i).GetChild(0).gameObject, moveBox.transform.GetChild(i).GetChild(0).gameObject.name + "1", false);
                            }
                        }
                        ));
                                Max.SetActive(false);
                                _mask.SetActive(true);
                                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null,
                                    () => { _mask.SetActive(false); }
                                    ));
                            }
                            );

                    }
                    );

            }
        }

        private void GameInit()
        {
            moveBox.SetActive(false);
            dragBox.SetActive(true);
            insideBox.SetActive(false);
            curTrans.Find("show1").gameObject.SetActive(false);
            curTrans.Find("show2").gameObject.SetActive(false);
            _mask.SetActive(false);
            talkIndex = 1;
            Bg.transform.GetChild(0).gameObject.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            clickBox.SetActive(false);
            allDrag[0].transform.GetChild(0).eulerAngles = new Vector3(0, -180, 0);
            allDrag[1].transform.GetChild(0).eulerAngles = new Vector3(0, -180, 0);
            allDrag[2].transform.GetChild(0).eulerAngles = new Vector3(0, 0, 0);
            allDrag[3].transform.GetChild(0).eulerAngles = new Vector3(0, -180, 0);

            for (int i = 0; i < 3; i++)
            {
                SpineManager.instance.DoAnimation(insideBox.transform.GetChild(i).GetChild(0).gameObject, insideBox.transform.GetChild(i).GetChild(0).gameObject.name, false);
                moveBox.transform.GetChild(i).gameObject.SetActive(true);
                moveBox.transform.GetChild(i).localPosition = curTrans.Find("moveBoxPos").GetChild(i).localPosition;
                insideBox.transform.GetChild(i).gameObject.SetActive(false);

            }
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            _mask.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { isPlaying = false; _mask.SetActive(false); }));

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
                speaker = Max;
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

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }


        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                _canClick = true;
                _mask.SetActive(true);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                dragBox.SetActive(false);
                clickBox.SetActive(true);
                clickBox.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(clickBox.transform.GetChild(0).gameObject, "animation", true);
                Max.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    { _mask.SetActive(false); }
                    ));
            }
            if (talkIndex == 2)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true) ;
                Max.SetActive(true);
                Bg.transform.GetChild(0).gameObject.SetActive(false);
                insideBox.SetActive(false);
                curTrans.Find("show1").gameObject.SetActive(true);
                curTrans.Find("show2").gameObject.SetActive(true);
                curTrans.Find("show1").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(curTrans.Find("show1").gameObject, "xl0", true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3));
                mono.StartCoroutine(WaitTime(2,
                    () =>
                    {
                        //mono.StartCoroutine(WaitTime(0.8f,
                        //    ()=>
                        //    {
                        //        curTrans.Find("show2").gameObject.SetActive(true);
                        //    }
                        //    ));
                        SpineManager.instance.DoAnimation(curTrans.Find("show1").gameObject, "xl1", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(curTrans.Find("show1").gameObject, "xl2", true);
                            }
                            );
                    }
                    ));
                mono.StartCoroutine(WaitTime(10,
                    () =>
                    {
                        //curTrans.Find("show2").gameObject.SetActive(false);
                        SpineManager.instance.DoAnimation(curTrans.Find("show1").gameObject, "xl3", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(curTrans.Find("show1").gameObject, "xl0", true);
                            }
                            );
                    }
                    ));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
