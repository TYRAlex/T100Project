using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course733Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        private Transform qiao;
        private Transform onclick;
        private Transform guang;
        private Transform zi;
        private int clickcount;
        private Transform _mask;

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
            onclick = curTrans.Find("OnClick");
            guang = curTrans.Find("guang");
            zi = curTrans.Find("zi");
            _mask = curTrans.Find("_mask");

            clickcount = 0;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
            AddBtnClick();
        }







        private void GameInit()
        {
            talkIndex = 1;
          SpineManager.instance.DoAnimation(qiao.gameObject,"jing",false);
         //   qiao.gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            _mask.gameObject.SetActive(true);
            onclick.gameObject.SetActive(true);
            for (int i = 0; i < onclick.childCount; i++) 
            {
                onclick.GetChild(i).gameObject.GetComponent<Empty4Raycast>().raycastTarget = true;
            }
            for (int i = 1; i <= guang.childCount; i++) 
            {
                guang.Find(i.ToString()).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            for (int i = 1; i <= zi.childCount; i++)
            {
                zi.Find(i.ToString()).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
        }



        void GameStart()
        {
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            //17s
            Delay(17f,()=> 
            {
                SpineManager.instance.DoAnimation(qiao.gameObject,"animation",false);
            });
        }
        private void AddBtnClick() 
        {
            Util.AddBtnClick(onclick.Find("1").gameObject, OnClickEvent1);
            Util.AddBtnClick(onclick.Find("2").gameObject, OnClickEvent1);
            Util.AddBtnClick(onclick.Find("3").gameObject, OnClickEvent2);
            Util.AddBtnClick(onclick.Find("4").gameObject, OnClickEvent1);
        }
        private void OnClickEvent1(GameObject obj) 
        {
            clickcount++;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,0);
            _mask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, int.Parse(obj.name)+1, null, () => 
            {
                _mask.gameObject.SetActive(false);
                if (clickcount == 4)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));
            obj.GetComponent<Empty4Raycast>().raycastTarget = false;
            SpineManager.instance.DoAnimation(guang.Find(obj.name.ToString()).gameObject,"g"+obj.name,false);
            SpineManager.instance.DoAnimation(zi.Find(obj.name.ToString()).gameObject, "z" + obj.name, false);
           
        }
        private void OnClickEvent2(GameObject obj) 
        {
            clickcount++; SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            _mask.gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, int.Parse(obj.name) + 1, null, () =>
            {
                _mask.gameObject.SetActive(false);
                if (clickcount == 4)
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }
            }));
            obj.GetComponent<Empty4Raycast>().raycastTarget = false;
            SpineManager.instance.DoAnimation(guang.Find(obj.name.ToString()).gameObject, "g" + obj.name, false);
            SpineManager.instance.DoAnimation(guang.Find("5").gameObject, "g5", false);
            SpineManager.instance.DoAnimation(guang.Find("6").gameObject, "g6", false);
            SpineManager.instance.DoAnimation(zi.Find(obj.name.ToString()).gameObject, "z" + obj.name, false);
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
                //游戏玩法介绍
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 1, null, () =>
                 {
                     _mask.gameObject.SetActive(false);
                     onclick.gameObject.SetActive(true);
                 }));
            }
            else if (talkIndex == 2) 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.SOUND, 6, null, () =>
                {
                    
                }));
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
