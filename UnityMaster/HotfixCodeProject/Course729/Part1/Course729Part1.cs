using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ILFramework.HotClass
{
    public class Course729Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bg;
        private BellSprites bellSprite;
        private GameObject mask;

        private GameObject max;
        private Transform maxPos;
        private Transform maxEndPos;


        private GameObject panel;

        private GameObject panel2;
        private GameObject schoolDoor;
        private GameObject schoolDoorShow;
        private GameObject btnWheel;
        private GameObject btnHead;
        private Transform btnCrosses;
        private Transform btnPoles;

        private GameObject panel3;
        private GameObject crossStruct;

        private Transform TLMBg;
        private Transform gs;

        private Transform drager;
        private float temX = 0;


        private float angle;
        private float angleD;
        private float offsetX = 0;
        private Vector3 startPos;

        private Vector3[] tranPos;


        public float Angle
        {
            get
            {
                return angle;
            }
            set
            {
                angle = value;
                if (value > 15)
                {
                    angle = 15;
                }
                if (value < 0)
                {
                    angle = 0;
                }
            }
        }

        public float AngleD
        {
            get
            {
                return angleD;
            }
            set
            {
                angleD = value;
                if (value < -15)
                {
                    angleD = -15;
                }
                if (value > 0)
                {
                    angleD = 0;
                }
            }
        }


        private int transLen = 0;


        private SkeletonGraphic skeletonGraphic;
        private ILDrager ilDrager;

        private bool isPlaySpine = false;


        private Transform dragerPos;
        void Start(object o)
        {
            curGo = (GameObject)o;

            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            bg = curTrans.Find("Bg").gameObject;
            bellSprite = bg.GetComponent<BellSprites>();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);
            max = curTrans.Find("maxPanel/max").gameObject;
            maxEndPos = curTrans.Find("maxEndPos");
            maxPos = curTrans.Find("maxPos");
            max.transform.position = maxPos.position;
            max.SetActive(true);

            panel = curTrans.Find("Panel").gameObject;
            panel.SetActive(true);
            panel2 = curTrans.Find("Panel2").gameObject;
            panel2.SetActive(false);
            schoolDoor = curTrans.Find("Panel2/schoolDoor").gameObject;
            schoolDoorShow = curTrans.Find("Panel2/schoolDoorShow").gameObject;
            btnWheel = curTrans.Find("Panel2/2").gameObject;
            btnHead = curTrans.Find("Panel2/1").gameObject;
            btnCrosses = curTrans.Find("Panel2/4");
            btnPoles = curTrans.Find("Panel2/3");
            panel3 = curTrans.Find("Panel3").gameObject;
            panel3.SetActive(false);
            crossStruct = curTrans.Find("Panel3/crossStruct").gameObject;
            crossStruct.SetActive(false);
            TLMBg = curTrans.Find("Panel3/Bg");
            TLMBg.gameObject.SetActive(true);
            drager = TLMBg.Find("drager");
            dragerPos = TLMBg.Find("dragerPos");
            gs = TLMBg.Find("g");


            skeletonGraphic = crossStruct.GetComponent<SkeletonGraphic>();
            ilDrager = drager.GetComponent<ILDrager>();
            ilDrager.SetDragCallback(StartDarg, Darg, EndDarg);
            ilDrager.isActived = false;
            drager.position = dragerPos.position;
            startPos = dragerPos.position;
            transLen = gs.childCount/2;


            tranPos = new Vector3[transLen];

            for (int i = 0; i < transLen; i++)
            {
                gs.GetChild(i).localPosition = gs.GetChild(i + 3).localPosition;
                gs.GetChild(i).GetChild(0).eulerAngles= Vector3.zero;
                gs.GetChild(i).GetChild(1).eulerAngles =Vector3.zero;
                tranPos[i] = gs.GetChild(i+3).localPosition;
            }

            Util.AddBtnClick(btnWheel, onClickBtnSchoolDoor);
            Util.AddBtnClick(btnHead, onClickBtnSchoolDoor);

            for (int i = 0; i < btnCrosses.childCount; i++)
            {
                Util.AddBtnClick(btnCrosses.GetChild(i).gameObject, onClickBtnSchoolDoor);
            }
            for (int i = 0; i < btnPoles.childCount; i++)
            {
                Util.AddBtnClick(btnPoles.GetChild(i).gameObject, onClickBtnSchoolDoor);
            }
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

          
            GameStart();
        }
        private void onClickBtnSchoolDoor(GameObject obj)
        {       
            if (isPlaySpine)
                return;
            isPlaySpine = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,6,false);
            SoundManager.instance.ShowVoiceBtn(false);
            mono.StartCoroutine(SpeckerCoroutine(max,SoundManager.SoundType.VOICE, int.Parse(obj.name), () => { SpineManager.instance.DoAnimation(schoolDoorShow, obj.name, false); }, () => {SoundManager.instance.ShowVoiceBtn(true); isPlaySpine = false; }));
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            bg.GetComponent<Image>().sprite = bellSprite.sprites[0];
            mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                bg.GetComponent<Image>().sprite = bellSprite.sprites[1];
                panel.SetActive(false);
                panel2.SetActive(true);
                mask.SetActive(true);
                max.SetActive(false);
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
                speaker = max;
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
                bg.GetComponent<Image>().sprite = bellSprite.sprites[2];
                panel2.SetActive(false);
                mask.SetActive(false);
                max.transform.position = maxEndPos.position;
                max.SetActive(true);
                panel3.SetActive(true);
                //crossStruct.SetActive(true);
                ilDrager.isActived = true;
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 5, null, () => { max.SetActive(false); SoundManager.instance.ShowVoiceBtn(true); skeletonGraphic.freeze = false; }));
            }
            if (talkIndex == 2)
            {
                max.SetActive(true);
                TLMBg.gameObject.SetActive(false);
                crossStruct.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE,6, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

            }
            if (talkIndex == 3)
            {
                
                mono.StartCoroutine(SpeckerCoroutine(max, SoundManager.SoundType.VOICE, 7, () => { SpineManager.instance.DoAnimation(crossStruct, "animation", false, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); SpineManager.instance.DoAnimation(crossStruct, "animation2", false); }); }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }
            if (talkIndex == 4)
            {
                bg.GetComponent<Image>().sprite = bellSprite.sprites[0];
                panel3.SetActive(false);
                max.transform.position = maxPos.position;
                panel.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(max,SoundManager.SoundType.VOICE, 8, /*播放动画*/ null, null));
            }
            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        void StartDarg(Vector3 pos, int index, int type)
        {
            temX = Input.mousePosition.x;
        }

        void Darg(Vector3 pos, int index, int type)
        {

            if (startPos.x < Input.mousePosition.x)
            {
                drager.position = startPos;
            }

            if (temX > Input.mousePosition.x)
            {
                IsDrager(true, offsetX);
            }
            if (temX < Input.mousePosition.x)
            {
                IsDrager(false, offsetX);
            }
            offsetX = Mathf.Round(150 * (Mathf.Cos((45 - Angle) * Mathf.Deg2Rad) - Mathf.Cos(45 * Mathf.Deg2Rad)));
            temX = Input.mousePosition.x;

        }

        void EndDarg(Vector3 pos, int index, int type, bool match)
        {
            drager.position = gs.GetChild(2).position;
            temX = 0;
        }
        public void IsDrager(bool isLeft, float offset)
        {
            //drager.position = Input.mousePosition;
            for (int i = 0; i < transLen; i++)
            {
                gs.GetChild(i).GetChild(0).eulerAngles = new Vector3(0, 0, (isLeft ? AngleD-- : AngleD++));
                gs.GetChild(i).GetChild(1).eulerAngles = new Vector3(0, 0, (isLeft ? Angle++ : Angle--));
            }

            if (isLeft)
            {
                gs.GetChild(0).localPosition = new Vector3(tranPos[0].x - offset, tranPos[0].y, tranPos[0].z);
                gs.GetChild(1).localPosition = new Vector3(tranPos[1].x - (3 * offset), tranPos[1].y, tranPos[1].z);
                gs.GetChild(2).localPosition = new Vector3(tranPos[2].x - (5 * offset), tranPos[2].y, tranPos[2].z);

            }
            else
            {
                gs.GetChild(0).localPosition = new Vector3(gs.GetChild(0).localPosition.x + offset * 0.25f, tranPos[0].y, tranPos[0].z);
                gs.GetChild(1).localPosition = new Vector3(gs.GetChild(1).localPosition.x + offset * 0.5f, tranPos[1].y, tranPos[1].z);
                gs.GetChild(2).localPosition = new Vector3(gs.GetChild(2).localPosition.x + offset * 0.75f, tranPos[2].y, tranPos[2].z);

                if (offset <= 0)
                {
                    for (int i = 0; i < transLen; i++)
                    {
                        gs.GetChild(i).localPosition = tranPos[i];
                    }
                }

            }

        }
    }
}
