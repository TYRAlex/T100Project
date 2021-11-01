using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course7310Part4
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject[] level;
        private GameObject level1;
        private GameObject level2;
        private GameObject level3;
        private GameObject Drag;
        private GameObject Drop;
        private Transform DragPos;
        private GameObject mask;
        private GameObject left;
        private GameObject right;
        private int number;
        private bool _canclick;
        private bool[] _jugle;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            DOTween.Clear();
            Input.multiTouchEnabled = false;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            level1 = curTrans.Find("level1").gameObject;
            level2 = curTrans.Find("level2").gameObject;
            level3 = curTrans.Find("level3").gameObject;
            Drag = level1.transform.Find("Drag").gameObject;
            Drop = level1.transform.Find("Drop").gameObject;
            DragPos = level1.transform.Find("DragPos");
            mask = curTrans.Find("mask").gameObject;
            level = new GameObject[3] { level1, level2, level3 };
            left = curTrans.Find("a").gameObject;
            right = curTrans.Find("b").gameObject;
            _jugle = new bool[3];
            for (int i = 0; i < 3; i++)
            {
                Drag.transform.GetChild(i).GetComponent<mILDrager>().SetDragCallback(OnBeginDrag, null, OnEndDrag, null);
            }
            Util.AddBtnClick(left.transform.GetChild(0).gameObject, Click);
            Util.AddBtnClick(right.transform.GetChild(0).gameObject, Click);
            GameInit();
            GameStart();

          
                
            
        }

        private void Click(GameObject obj)
        {
            
            if (!_canclick)
                return;
            BtnPlaySound();
            _canclick = false;
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + 2, false,
                () =>
                {
                    if (obj.name == "a")
                    {
                        if (number == 1)
                            return;
                        level[number - 1].SetActive(false);
                        level[number - 2].SetActive(true);
                        if(number==2)
                        {
                            for (int i = 0; i < 3; i++)
                            {
                                Drag.transform.GetChild(i).position = DragPos.Find(Drag.transform.GetChild(i).name).position;
                                Drag.transform.GetChild(i).GetComponent<mILDrager>().isActived = false;
                            }
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canclick = true; InitLevel1(); }));
                            left.SetActive(false);
                        }
                        if(number==3)
                        {
                            right.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () => { _canclick = true; }));
                        }
                        number--;
                    }
                    else
                    {

                        if (number == 3)
                        {

                            return;
                        }
                            

                        level[number - 1].SetActive(false);
                        level[number].SetActive(true);
                        if (number == 2)
                        {
                            right.SetActive(false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null, () => { _canclick = true; }));
                        }
                        if (number == 1)
                        {
                            for (int i = 0; i < Drag.transform.childCount; i++)
                                Drag.transform.GetChild(i).gameObject.SetActive(true);

                            _jugle[0] = false;
                            _jugle[1] = false;
                            _jugle[2] = false;
                            left.SetActive(true);
                            SpineManager.instance.DoAnimation(left, "a", false);
                            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () => { _canclick = true; }));
                        }
                        number++;
                    }
                }
                );
        }

        private void Jugle()
        {
            for (int i = 0; i < 3; i++)
            {
                if (!_jugle[i])
                    return;
            }
            _canclick = false;
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,()=> { _canclick = true; }));
        }



        private void GameInit()
        {
            right.SetActive(true);
            left.SetActive(false);
            right.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            left.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(right,"b",false);
            number = 1;
            level1.SetActive(true);
            level2.SetActive(false);
            level3.SetActive(false);
            mask.SetActive(false);
            talkIndex = 1;
            _canclick = false;
            for (int i = 0; i < 3; i++)
            {
                Drag.transform.GetChild(i).gameObject.SetActive(true);
                Drag.transform.GetChild(i).position = DragPos.Find(Drag.transform.GetChild(i).name).position;
                Drag.transform.GetChild(i).GetComponent<mILDrager>().isActived = false;
            }
        }
        private void InitLevel1()
        {
            for (int i = 0; i < 3; i++)
            {
                Drag.transform.GetChild(i).gameObject.SetActive(true);
                Drag.transform.GetChild(i).position = DragPos.Find(Drag.transform.GetChild(i).name).position;
                Drag.transform.GetChild(i).GetComponent<mILDrager>().isActived = true;
            }
        }

        void GameStart()
        {
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,0,null,()=> { _canclick = true;InitLevel1(); }));
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

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            Drag.transform.Find(index.ToString()).SetAsLastSibling();
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                mask.SetActive(true);
                Drag.transform.Find(index.ToString()).localPosition = DragPos.Find(index.ToString()).localPosition;
                Drag.transform.Find(index.ToString()).DOShakePosition(1f,10f).OnComplete(()=> { mask.SetActive(false); });
            }
            else
            {
                mask.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,index+4,null,()=> 
                {
                    mask.SetActive(false); 
                    _jugle[index] = true;
                    Jugle();
                }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                Drag.transform.Find(index.ToString()).gameObject.SetActive(false);
                Drag.transform.Find(index.ToString()).GetComponent<mILDrager>().isActived = false;
            }
        }

        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }
    }
}
