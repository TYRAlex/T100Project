using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course733Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private mILDrager _mILDrager;
        private Transform dragBtn;
        private bool isOne;
        private bool isTwo;
        private bool isThree;
        private Transform roate1;
        private Transform roate2;
        private Transform roate3;
        private Transform all;
        private Transform onClick; private Transform onClick2;
        private Transform num;
        private int count;
        private Transform _mask;
        private Transform mask;
        private Transform frame;
        private Transform shengli;
        private Transform slide_yellow;
        private Transform slide_green;
        private int rightcount;

        bool isPlaying = false;


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
            dragBtn = curTrans.Find("dragbtn");
            all = curTrans.Find("all");
            roate1 = curTrans.Find("all/red/1");
            roate2 = curTrans.Find("all/red/2");
            roate3 = curTrans.Find("all/red/3");
            onClick = curTrans.Find("OnClick"); onClick2 = curTrans.Find("OnClick2");
            num = curTrans.Find("num");
            _mask = curTrans.Find("_mask");
            mask = curTrans.Find("mask");
            frame = curTrans.Find("frame");
            shengli = curTrans.Find("shengli");
            slide_yellow = curTrans.Find("y"); slide_green = curTrans.Find("g");
            count = 0;rightcount = 0;
            isOne = false;isTwo = false;isThree = false;
            dragBtn.GetComponent<mILDrager>().isActived = true;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            InitMILDrager();
            AddDragersEvent();
            AddDragBtnEvent();
        }







        private void GameInit()
        {
            talkIndex = 1;
            Reset();
            dragBtn.GetRectTransform().anchoredPosition = new Vector2(-335,420);
            slide_green.GetRectTransform().sizeDelta = new Vector2(41, 40); slide_yellow.GetRectTransform().sizeDelta = new Vector2(41, 40);
            onClick2.gameObject.SetActive(false);
            _mask.gameObject.SetActive(true);
            mask.gameObject.SetActive(false);
            shengli.gameObject.SetActive(false);
            frame.gameObject.SetActive(false);
       
            //SpineManager.instance.DoAnimation(frame.Find("a").gameObject,"xa1",false);
            //SpineManager.instance.DoAnimation(frame.Find("b").gameObject, "xb1", false);
            //SpineManager.instance.DoAnimation(frame.Find("c").gameObject, "xc1", false);
            frame.Find("a").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            frame.Find("b").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            frame.Find("c").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            frame.Find("a").gameObject.SetActive(false);
            frame.Find("b").gameObject.SetActive(false);
            frame.Find("c").gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(num.Find("1").gameObject,"a1",false);
            SpineManager.instance.DoAnimation(num.Find("2").gameObject, "b1", false);
            SpineManager.instance.DoAnimation(num.Find("3").gameObject, "c1", false);
            onClick.Find("1").GetComponent<Empty4Raycast>().raycastTarget=true;
            onClick.Find("2").GetComponent<Empty4Raycast>().raycastTarget = true;
            onClick.Find("3").GetComponent<Empty4Raycast>().raycastTarget = true;
            onClick2.Find("1").GetComponent<Empty4Raycast>().raycastTarget = true;
            onClick2.Find("2").GetComponent<Empty4Raycast>().raycastTarget = true;
            onClick2.Find("3").GetComponent<Empty4Raycast>().raycastTarget = true;
            count = 0; rightcount = 0;
            isOne = false; isTwo = false; isThree = false;
        }



        void GameStart()
        {
            Max.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

        }
        void InitMILDrager()
        {
            _mILDrager = new mILDrager();
            _mILDrager = dragBtn.GetComponent<mILDrager>();
        }
        private void AddDragersEvent()
        {
            _mILDrager.SetDragCallback(null, Draging, DragEnd);

        }
        private void AddDragBtnEvent() 
        {
            Util.AddBtnClick(onClick.Find("1").gameObject,OnClickEvent1);
            Util.AddBtnClick(onClick.Find("2").gameObject, OnClickEvent2);
            Util.AddBtnClick(onClick.Find("3").gameObject, OnClickEvent3);

            Util.AddBtnClick(onClick2.Find("1").gameObject, OnClickEvent11);
            Util.AddBtnClick(onClick2.Find("2").gameObject, OnClickEvent22);
            Util.AddBtnClick(onClick2.Find("3").gameObject, OnClickEvent33);

            Util.AddBtnClick(frame.Find("a/right1").gameObject, RightChoose);
            Util.AddBtnClick(frame.Find("a/fault").gameObject, FaultChoose);
            Util.AddBtnClick(frame.Find("b/right2").gameObject, RightChoose);
            Util.AddBtnClick(frame.Find("b/fault").gameObject, FaultChoose);
            Util.AddBtnClick(frame.Find("c/right3").gameObject, RightChoose);
            Util.AddBtnClick(frame.Find("c/fault").gameObject, FaultChoose);
        }
        private void OnClickEvent1(GameObject obj) 
        {
            BtnPlaySound(); _mask.gameObject.SetActive(true);
            onClick.Find("1").gameObject.SetActive(false);
            onClick.Find("2").gameObject.SetActive(false);
            onClick.Find("3").gameObject.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => { _mask.gameObject.SetActive(false); }));
            obj.GetComponent<Empty4Raycast>().raycastTarget = false;
            count++;
            isOne = true;
            isTwo = false;
            isThree = false;
        }
        private void OnClickEvent2(GameObject obj)
        {
            onClick.Find("1").gameObject.SetActive(false);
            onClick.Find("2").gameObject.SetActive(false);
            onClick.Find("3").gameObject.SetActive(false);
            _mask.gameObject.SetActive(true);
            BtnPlaySound(); obj.GetComponent<Empty4Raycast>().raycastTarget = false;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null, () => { _mask.gameObject.SetActive(false); }));
            count++;
            isOne = false;
            isTwo = true;
            isThree = false;
        }
        private void OnClickEvent3(GameObject obj)
        {
            onClick.Find("1").gameObject.SetActive(false);
            onClick.Find("2").gameObject.SetActive(false);
            onClick.Find("3").gameObject.SetActive(false);
            _mask.gameObject.SetActive(true);
            BtnPlaySound(); obj.GetComponent<Empty4Raycast>().raycastTarget = false;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 4, null, () => { _mask.gameObject.SetActive(false); }));
            count++;
            isOne = false;
            isTwo = false;
            isThree = true;
        }
        private void OnClickEvent11(GameObject obj)
        {
            BtnPlaySound();
            _mask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 5, null, () => { _mask.gameObject.SetActive(false); }));

           
        
            frame.gameObject.SetActive(true);
            frame.Find("a").gameObject.SetActive(true);
            frame.Find("c").gameObject.SetActive(false);
            frame.Find("b").gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(frame.Find("a").gameObject, "xa1", false);
        }
        private void OnClickEvent22(GameObject obj)
        {
            BtnPlaySound();
            _mask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 6, null, () => { _mask.gameObject.SetActive(false); }));

           
            frame.gameObject.SetActive(true);
            frame.Find("a").gameObject.SetActive(false);
            frame.Find("c").gameObject.SetActive(false);
            frame.Find("b").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(frame.Find("b").gameObject,"xb1",false);
        }
        private void OnClickEvent33(GameObject obj)
        {
            BtnPlaySound();
            _mask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 7, null, () => { _mask.gameObject.SetActive(false); }));

            

            frame.gameObject.SetActive(true);
            frame.Find("a").gameObject.SetActive(false);
            frame.Find("c").gameObject.SetActive(true);
            frame.Find("b").gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(frame.Find("c").gameObject, "xc1", false);
        }
        private void RightChoose(GameObject obj)
        {
            rightcount++;
            _mask.gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            if (obj.name == "right1") 
            {
                onClick2.Find("1").GetComponent<Empty4Raycast>().raycastTarget = false;
                roate1.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate1.rotation.y, 57));
                successVoice();
            }
            else if(obj.name == "right2")
            {
                onClick2.Find("2").GetComponent<Empty4Raycast>().raycastTarget = false;
                roate2.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate1.rotation.y, -45));
                successVoice();
            }
            else if (obj.name == "right3")
            {
                onClick2.Find("3").GetComponent<Empty4Raycast>().raycastTarget = false;
                roate3.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate1.rotation.y, 90));
                successVoice();
            }

            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject,"x"+obj.transform.parent.name+"2",false);
        }
        private void successVoice()
        {
           
            if (rightcount == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 9, null, () =>
                {
                    _mask.gameObject.SetActive(false);
                    frame.gameObject.SetActive(false);
                }));
            }
            else if (rightcount == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 10, null, () =>
                {
                    _mask.gameObject.SetActive(false);
                    frame.gameObject.SetActive(false);
                }));
            }
            else 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 11, null, () =>
                {
                    mask.gameObject.SetActive(true);
                    frame.gameObject.SetActive(false);
                    shengli.gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    SpineManager.instance.DoAnimation(shengli.gameObject, "animation", false, () =>
                    {
                        SpineManager.instance.DoAnimation(shengli.gameObject, "animation2", true);
                    });
                }));
            }
        }
        private void FaultChoose(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
            _mask.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "x" + obj.transform.parent.name + "3", false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 8, null, () => 
            { 
                _mask.gameObject.SetActive(false);
                SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, "x" + obj.transform.parent.name + "1", false);
            }));
        }
        private void Reset() 
        {
            dragBtn.GetRectTransform().anchoredPosition = new Vector2(-335, 420);
            slide_green.GetRectTransform().sizeDelta = new Vector2(41, 40); slide_yellow.GetRectTransform().sizeDelta = new Vector2(41, 40);
            roate1.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate1.rotation.y, 115));
            roate2.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate2.rotation.y, -15));
            roate3.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate3.rotation.y, -10));
        }
        private void Draging(Vector3 position, int type, int index)
        {
            //98.5

            //yellow max=150 min=41 fra=105.5

            float offset = dragBtn.GetRectTransform().anchoredPosition.x;
            float NewOffset = offset - (-335);
            float fra = NewOffset / 98.5f;
            float angle = fra * 75;
            float width = -(fra * 105.5f) + 35f;
            float width2 = (fra * 105.5f) + 34f;
            if (fra < 0)
            {
                slide_green.gameObject.SetActive(false); slide_yellow.gameObject.SetActive(true);
                slide_yellow.GetRectTransform().sizeDelta = new Vector2(width, 42);
            }
            else if (fra > 0)
            {
                slide_green.gameObject.SetActive(true); slide_yellow.gameObject.SetActive(false);
                slide_green.GetRectTransform().sizeDelta = new Vector2(width2, 42);
            }
          
           
            if (isOne == true)
            {
                roate1.rotation = Quaternion.Euler(new Vector3(roate1.rotation.x, roate1.rotation.y, 115+angle));
            }
            else if (isTwo == true)
            {
                roate2.rotation = Quaternion.Euler(new Vector3(roate2.rotation.x, roate1.rotation.y, -15+angle));
            } 
            else if (isThree == true) 
            {
                roate3.rotation = Quaternion.Euler(new Vector3(roate3.rotation.x, roate1.rotation.y, -10+angle));
            }

            Debug.Log(angle);
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch) 
        {
            onClick.Find("1").gameObject.SetActive(true);
            onClick.Find("2").gameObject.SetActive(true);
            onClick.Find("3").gameObject.SetActive(true);
            if (count >= 3) 
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
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

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { Max.SetActive(false); _mask.gameObject.SetActive(false); }));
                Delay(3f, () => 
                {
                    SpineManager.instance.DoAnimation(num.Find("1").gameObject, "a2", true);
                    SpineManager.instance.DoAnimation(num.Find("2").gameObject, "b2", true);
                    SpineManager.instance.DoAnimation(num.Find("3").gameObject, "c2", true);
                });
                Delay(4.5f,()=> 
                {
                    SpineManager.instance.DoAnimation(num.Find("1").gameObject, "a1", false);
                    SpineManager.instance.DoAnimation(num.Find("2").gameObject, "b1", false);
                    SpineManager.instance.DoAnimation(num.Find("3").gameObject, "c1", false);
                });

            }
            else if (talkIndex==2)
            {
                Reset();
                onClick2.gameObject.SetActive(true);
                dragBtn.GetComponent<mILDrager>().isActived = false;
            }

            talkIndex++;
        }

        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }



        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
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

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
