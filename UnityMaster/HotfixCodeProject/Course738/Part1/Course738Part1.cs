using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course738Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform onClick;
        private Transform onClick2;
        private Transform onClick3;
        private Transform ball;
        private Transform mask_spine;
        private Transform mask;
        private Transform car;
        private bool _canClick;

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
            onClick = curTrans.Find("onclick");
            onClick2 = curTrans.Find("onclick2");
            onClick3 = curTrans.Find("onclick3");
            ball = curTrans.Find("ball");
            mask = curTrans.Find("mask");
            mask_spine = curTrans.Find("mask/1");
            car = curTrans.Find("car");
            _canClick = true;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            onClick.gameObject.SetActive(false); onClick2.gameObject.SetActive(false); onClick3.gameObject.SetActive(false);
            Util.AddBtnClick(onClick.gameObject,OnClickEvent);
            Util.AddBtnClick(onClick2.gameObject, OnClickEvent2);
            Util.AddBtnClick(onClick3.gameObject, OnClickEvent2);
            onClick.gameObject.SetActive(false);
            mask.gameObject.SetActive(false);
            car.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(ball.gameObject,"kong",false);
            SpineManager.instance.DoAnimation(car.gameObject, "a4", false);
            SpineManager.instance.DoAnimation(mask_spine.gameObject, "kong", false);
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            //红外传感器可以帮助我们测算距离，把它作为倒车雷达的话，应该安装在哪个位置呢？
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SpineManager.instance.DoAnimation(ball.gameObject, "a1", false,()=>
            { 
                onClick.gameObject.SetActive(true);onClick.GetComponent<RawImage>().raycastTarget = true;
                onClick2.gameObject.SetActive(true); onClick2.GetComponent<RawImage>().raycastTarget = true;
                onClick3.gameObject.SetActive(true); onClick3.GetComponent<RawImage>().raycastTarget = true;
            }); 
            }));

        }
        //开头点击三个红圈的正确点击事件
        private void OnClickEvent(GameObject obj)
        {
            if (_canClick) 
            {
                onClick2.gameObject.SetActive(false); onClick3.gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                obj.GetComponent<RawImage>().raycastTarget = false;
                SpineManager.instance.DoAnimation(ball.gameObject, "a2", false, () =>
                {
                    //没错，接下来我们就一起来认识一下红外传感器吧！
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                });
            }

        }       
        //开头点击三个红圈的错误点击事件
        private void OnClickEvent2(GameObject obj) 
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3);
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () =>
                {
                    _canClick = true;
                }));
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
                mask.gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4);
                SpineManager.instance.DoAnimation(mask_spine.gameObject, "b1", false, () =>
                   {
                       SpineManager.instance.DoAnimation(mask_spine.gameObject, "b2", false);
                   });
            }
            else if (talkIndex==2)
            {
                mask.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null, () => { }));
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5);
                SpineManager.instance.DoAnimation(car.gameObject,"daoche",false,()=> 
                {
     
                });
                SpineManager.instance.DoAnimation(ball.gameObject, "kong", false);
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
