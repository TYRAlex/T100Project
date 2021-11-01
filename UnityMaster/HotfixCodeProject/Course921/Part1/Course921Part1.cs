using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course921Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform Onbtn;
        private Transform _mask;
        private Transform Mask1;
        private GameObject spine_Go;
        private GameObject spine_Go2;
        private bool isplay;
        private Transform mask;
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

            Onbtn = curTrans.Find("OnBtn");
            _mask = curTrans.Find("_mask");
            Mask1 = curTrans.Find("Mask1");
            spine_Go = curTrans.Find("spineGo").gameObject;
            spine_Go2 = curTrans.Find("spineGo2").gameObject;
        
            isplay = false;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            isplay = false;
            _mask.gameObject.SetActive(true);
            Mask1.gameObject.SetActive(true);
            spine_Go.SetActive(true);
            spine_Go2.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            spine_Go.transform.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            spine_Go2.transform.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            // spine_Go2.transform.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            PlaySpine(spine_Go,"jing",null,true);
            PlaySpine(spine_Go2, "animation", null,false);
            // PlaySpine(spine_Go2, "jing", null, true);
            Onbtn.gameObject.SetActive(true);
            Util.AddBtnClick(Onbtn.GetChild(0).gameObject,OnClickEvent1);
            Util.AddBtnClick(Onbtn.GetChild(1).gameObject,OnClickEvent2);
            
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            //画面中有两种病床
            Mask1.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { Mask1.gameObject.SetActive(false); _mask.gameObject.SetActive(false); }));

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
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                spine_Go.SetActive(false);
                spine_Go2.SetActive(true);
                Onbtn.gameObject.SetActive(false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.SOUND,3,null,()=> { Max.gameObject.SetActive(false); }));
                Delay(4.493f, () =>
                 {
                     Delay(2f, ()=>{ SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1); });
                     Delay(6.5f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1); });
                     Delay(11f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1); });
                     PlaySpine(spine_Go2,"animation2",()=> { PlaySpine(spine_Go2, "animation",null,false); },false);
                 });
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private void OnClickEvent1(GameObject obj) 
        {
            if (isplay==false) 
            {
                isplay = true;
                BtnPlaySound();
                Max.gameObject.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(false);
                _mask.gameObject.SetActive(true);
                PlaySpine(spine_Go, "a1", () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    PlaySpine(spine_Go, "a2", null, false);
                    Delay(5.33f, () =>
                    {
                        ShowMask(obj);
                    });
                }, false);

            }
          
        }
        private void OnClickEvent2(GameObject obj)
        {
            if (isplay==false) 
            {
                isplay = true;
                BtnPlaySound();
                Max.gameObject.SetActive(false);
                SoundManager.instance.ShowVoiceBtn(false);
                _mask.gameObject.SetActive(true);
                PlaySpine(spine_Go, "b1", () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    PlaySpine(spine_Go, "b2", null, false);
                    Delay(2.5f, () =>
                    {
                        ShowMask(obj);
                    });
                }, false);
            }
           
        }
        void ShowMask(GameObject obj) 
        {
            AddEvent(_mask.gameObject,g=>
            {
                Debug.Log(obj.name);
                BtnPlaySound();
                switch (obj.name)
                {
                    case "1":
                        PlaySpine(spine_Go,"a3", () => { isplay = false; }, false);
                        break;
                    case "2":
                        PlaySpine(spine_Go, "b3", () => { isplay = false; }, false);
                        break;
                }
                RemoveEvent(g);
                _mask.gameObject.SetActive(false);
                Max.gameObject.SetActive(true);
                SoundManager.instance.ShowVoiceBtn(true);
            });
        }
       
        private float PlaySpine(GameObject go, string name, Action callBack = null, bool isLoop = false)
        {
            var time = SpineManager.instance.DoAnimation(go, name, isLoop, callBack);
            return time;
        }
        private void AddEvent(GameObject go, PointerClickListener.VoidDelegate callBack)
        {
            PointerClickListener.Get(go).onClick = g => { callBack?.Invoke(g); };
        }
        private void RemoveEvent(GameObject go)
        {
            PointerClickListener.Get(go).onClick = null;
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
    }
}
