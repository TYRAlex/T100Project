using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course7310Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        private GameObject show;
        private bool _canclick;
        private GameObject Box;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            DOTween.Clear();
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            show = curTrans.Find("show").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Box = curTrans.Find("Box").gameObject;
            Util.AddBtnClick(show.transform.GetChild(0).gameObject,Click);

            for (int i = 0; i < Box.transform.childCount; i++)           
                Box.transform.GetChild(i).gameObject.SetActive(false);

            Box.transform.GetChild(2).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(51, 301);
            Box.transform.GetChild(2).gameObject.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            //Box.transform.GetChild(2).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(51, 301), 0f);
            //Box.transform.GetChild(2).DOScale(new Vector3(1, 1, 1), 0f);
            GameInit();
            GameStart();
        }

        private void Click(GameObject obj)
        {
            if(_canclick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,()=> { mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, () => { SoundManager.instance.ShowVoiceBtn(true); })); }));
                _canclick = false;
                mono.StartCoroutine(Speak());
                //SpineManager.instance.DoAnimation(show,"animation2",false,
                //    () => { SpineManager.instance.DoAnimation(show, "animation3", false,()=> { SpineManager.instance.DoAnimation(show, "animation4", false); }); }
                //    );
            }

        }

        IEnumerator Speak()
        {
            yield return new WaitForSeconds(2.2f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            SpineManager.instance.DoAnimation(show, "dh1", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh2", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh3", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh4", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh5", false);
            yield return new WaitForSeconds(1.06f);
            SpineManager.instance.DoAnimation(show, "dh6", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh7", false);
            yield return new WaitForSeconds(1.1f);
            SpineManager.instance.DoAnimation(show, "dh8", false);
            yield return new WaitForSeconds(1.8f);
            SpineManager.instance.DoAnimation(show, "dh9", false,()=> { SpineManager.instance.DoAnimation(show, "animation3", false, () => { SpineManager.instance.DoAnimation(show, "animation4", false); }); });
        }



        private void GameInit()
        {
            talkIndex = 1;
            _canclick = false;
            Box.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            show.SetActive(true);
            Box.transform.GetChild(2).localScale = new Vector3(1, 1, 1);
            SpineManager.instance.DoAnimation(show,"animation",false);
            for (int i = 0; i < Box.transform.childCount; i++)
            {
                Box.transform.GetChild(i).gameObject.SetActive(false);
            }
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { isPlaying = false; _canclick = true; }));

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
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,3,null,()=> { SoundManager.instance.ShowVoiceBtn(true); }));
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                show.SetActive(false);
                Box.SetActive(true);
                //Open(Box, 0);
                //Open(Box, 2);
                //Delay(3f, () => {
                //    Open(Box, 3); 
                //    Delay(3f, () => { 
                //        Shut(Box, 3); Open(Box, 1);
                //        Delay(3f, () => { 
                //            Move(); 
                //            Delay(3f, () => {  });
                //        }); 
                //    });
                //});
                mono.StartCoroutine(ShowBox());
            }
            if(talkIndex==2)
            {
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,4,null,null));
            }
            talkIndex++;
        }

        IEnumerator ShowBox()
        {
            Box.transform.GetChild(0).gameObject.SetActive(true);
            Box.transform.GetChild(2).gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            Box.transform.GetChild(3).gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            Box.transform.GetChild(3).gameObject.SetActive(false);
            Box.transform.GetChild(1).gameObject.SetActive(true);
            yield return new WaitForSeconds(3);
            Box.transform.GetChild(2).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(23, 113), 1f);
            Box.transform.GetChild(2).DOScale(new Vector3(0, 0, 0), 1f);
            yield return new WaitForSeconds(1);
            Box.transform.GetChild(1).gameObject.SetActive(false);
            Box.transform.GetChild(2).gameObject.SetActive(false);
            Box.transform.GetChild(4).gameObject.SetActive(true);
        }

        private void Move()
        {
            Box.transform.GetChild(2).gameObject.GetComponent<RectTransform>().DOAnchorPos(new Vector2(23,113),1f);
            Box.transform.GetChild(2).DOScale(new Vector3(0, 0,0), 1f).OnComplete(()=> { Shut(Box, 1);Open(Box, 4);Shut(Box, 2); }); ;
        }

        private void Open(GameObject obj,int index)
        {
            obj.transform.GetChild(index).gameObject.SetActive(true);
        }
        private void Shut(GameObject obj, int index)
        {
            obj.transform.GetChild(index).gameObject.SetActive(false);
        }

        private void Delay(float time,Action callback =null)
        {
            mono.StartCoroutine(Wait(time,callback));
        }

        IEnumerator Wait(float time ,Action callback =null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
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
