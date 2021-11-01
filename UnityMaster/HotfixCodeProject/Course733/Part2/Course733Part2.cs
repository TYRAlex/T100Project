using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course733Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform qiao;
        private Transform guang;
        private Transform chuan; private Transform chuan2;
        private Transform onclick;
        private Transform _mask;
        private Transform qiu;
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
            qiao = curTrans.Find("qiao");
            guang = curTrans.Find("guang");
            chuan = curTrans.Find("chuan");
            chuan2 = curTrans.Find("chuan2");
            onclick = curTrans.Find("OnClick");
            _mask = curTrans.Find("mask");
            qiu = curTrans.Find("mask/qiu");

            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            AddBtnClick();
        }







        private void GameInit()
        {
            talkIndex = 1;
            chuan.gameObject.SetActive(false); chuan2.gameObject.SetActive(false);
          
            onclick.Find("1").gameObject.SetActive(false);
            onclick.Find("2").gameObject.SetActive(false);
    
            
          
            qiao.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            guang.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(qiao.gameObject,"qiao5",false);
            chuan.GetRectTransform().anchoredPosition = new Vector2(-358,149);
            chuan2.GetRectTransform().anchoredPosition = new Vector2(263, 149);
            //SpineManager.instance.DoAnimation(_mask.Find("qiu/1").gameObject,"a3",false);
            //SpineManager.instance.DoAnimation(_mask.Find("qiu/2").gameObject, "b3", false);
            //SpineManager.instance.DoAnimation(_mask.Find("qiu/3").gameObject, "jt3", false);
            _mask.Find("qiu/1").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.Find("qiu/2").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.Find("qiu/3").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.Find("1").gameObject.SetActive(false);
            _mask.Find("2").gameObject.SetActive(false);
            _mask.Find("3").gameObject.SetActive(false);
            _mask.Find("qiu/4").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.Find("qiu/5").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.Find("qiu/6").gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            _mask.gameObject.SetActive(false);
            _mask.Find("qiu").gameObject.SetActive(false);
        }


        private void AddBtnClick() 
        {
            Util.AddBtnClick(onclick.Find("1").gameObject,OnClickEvent1);
            Util.AddBtnClick(onclick.Find("2").gameObject, OnClickEvent2);
        }
        private void OnClickEvent1(GameObject obj) 
        {
            onclick.Find("1").gameObject.SetActive(false);
            guang.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1,false);
            SpineManager.instance.DoAnimation(qiao.gameObject,"qiao1",false,()=> 
            {
                SpineManager.instance.DoAnimation(qiao.gameObject,"qiao2",false);
                chuan.gameObject.SetActive(true); chuan2.gameObject.SetActive(true);
                chuan.GetRectTransform().DOAnchorPosY(1400,4f); chuan2.GetRectTransform().DOAnchorPosY(1400, 4f).OnComplete(()=> 
                {
                    Delay(2f,()=> 
                    {
                        SpineManager.instance.DoAnimation(qiao.gameObject, "qiao7", false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 2, null, () =>
                        {
                             onclick.Find("2").gameObject.SetActive(true);
                        }));
                    });
                   
                });
            });
        }
        private void OnClickEvent2(GameObject obj)
        {
            onclick.Find("2").gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            SpineManager.instance.DoAnimation(qiao.gameObject, "qiao8", false, () => 
            {
                SpineManager.instance.DoAnimation(qiao.gameObject, "qiao9", false, () =>
                   {
                       SpineManager.instance.DoAnimation(qiao.gameObject,"qiao5",false);
                       Delay(2f,()=> 
                       {
                           SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                           SpineManager.instance.DoAnimation(guang.gameObject,"qiaoguang",true);
                           mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 3, null,()=> 
                           { 
                               SoundManager.instance.ShowVoiceBtn(true); 
                           }));
                       });
                });
            });

        }
        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            SpineManager.instance.DoAnimation(guang.gameObject, "g1", true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () =>
            {
                chuan.gameObject.SetActive(true); chuan2.gameObject.SetActive(true);
                Delay(1.5f,()=>
                {
                    
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null,()=> 
                    {
                        onclick.Find("1").gameObject.SetActive(true);
                    }));
                });
                
            }));

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
                guang.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 4, null));
                Delay(2.5f, () =>
                {
                    SpineManager.instance.DoAnimation(qiao.gameObject, "qiao1", false, () => { SoundManager.instance.ShowVoiceBtn(true); });
                });
            }
            else if (talkIndex == 2)
            {
                _mask.gameObject.SetActive(true);
                _mask.Find("1").gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 5, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                Delay(7f, () =>
                {
                    _mask.Find("2").gameObject.SetActive(true);
                });
                Delay(9f, () =>
                {
                    _mask.Find("2").gameObject.SetActive(false);
                });
                Delay(10f, () =>
                {
                    _mask.Find("3").gameObject.SetActive(true);
                });
            }
            else if (talkIndex == 3)
            {
                qiu.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(qiu.Find("1").gameObject, "a3", false);
                SpineManager.instance.DoAnimation(qiu.Find("2").gameObject, "b3", false);
                SpineManager.instance.DoAnimation(qiu.Find("3").gameObject, "jt3", false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 6, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
                Delay(6f, () =>
                 {
               
                     SpineManager.instance.DoAnimation(qiu.Find("1").gameObject, "a1", false);
                     SpineManager.instance.DoAnimation(qiu.Find("2").gameObject, "b1", false);
                     SpineManager.instance.DoAnimation(qiu.Find("3").gameObject, "jt1", false);
                 });
                Delay(9f, () =>
                 {
                    
                     SpineManager.instance.DoAnimation(qiu.Find("4").gameObject, "shanguang1", false);
                     SpineManager.instance.DoAnimation(qiu.Find("5").gameObject, "shanguang2", false);
                 });
                Delay(20f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SpineManager.instance.DoAnimation(qiu.Find("3").gameObject, "jt4", false, () =>
                     {
                         SpineManager.instance.DoAnimation(qiu.Find("3").gameObject, "jt5", false);
                     });
                 });
                Delay(32f, () =>
                {
                
                    //红色箭头
                    SpineManager.instance.DoAnimation(qiu.Find("6").gameObject, "150jt", false);
                });
            }
            else if (talkIndex == 4) 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 7, null));
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
