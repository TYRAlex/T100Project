using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course933Part1
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
        private GameObject xlx;
        private GameObject xlx2;
        private bool _x1;
        private bool _x2;
        private bool _canClick;
        private bool[] _jugle;
        private bool _canvoice;
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
            show = curTrans.Find("show").gameObject;
            xlx = curTrans.Find("xlx").gameObject;
            xlx2 = curTrans.Find("xlx2").gameObject;
            _jugle = new bool[2];
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            Util.AddBtnClick(xlx.transform.GetChild(0).gameObject, Click1);
            Util.AddBtnClick(xlx2.transform.GetChild(0).gameObject, Click2);
            GameInit();
            GameStart();
        }

        private void Click1(GameObject obj)
        {
            if(!_x1&&_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                _x1 = true;
                SpineManager.instance.DoAnimation(xlx,"ad2",false,
                    () => { SpineManager.instance.DoAnimation(xlx, "ad3", false);_canClick = true;_jugle[0] = true;Jugle(); }
                    );
            }
            if (_x1 && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                _x1 = false;
                SpineManager.instance.DoAnimation(xlx, "ad4", false,
                    () => { SpineManager.instance.DoAnimation(xlx, "ad1", false); _canClick = true;Jugle(); }
                    );
            }
        }

        private void Click2(GameObject obj)
        {
            if (!_x2 && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                _x2 = true;
                SpineManager.instance.DoAnimation(xlx2, "bd2", false,
                    () => { SpineManager.instance.DoAnimation(xlx2, "bd3", false); _canClick = true;_jugle[1] = true;Jugle(); }
                    );
            }
            if (_x2 && _canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SoundManager.instance.ShowVoiceBtn(false);
                _canClick = false;
                _x2 = false;
                SpineManager.instance.DoAnimation(xlx2, "bd4", false,
                    () => { SpineManager.instance.DoAnimation(xlx2, "bd1", false); _canClick = true; Jugle(); }
                    );
            }
        }

        private void Jugle()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_jugle[i])
                    return;
            }
            if (!_canvoice)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }

        private void GameInit()
        {
            talkIndex = 1;
            xlx.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            xlx2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true); 
            for (int i = 0; i < show.transform.childCount; i++)
            {
                show.transform.GetChild(i).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            }
            show.SetActive(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            SpineManager.instance.DoAnimation(xlx,"zhong",false);
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => {  isPlaying = false; SoundManager.instance.ShowVoiceBtn(true); }));

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
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,
                    () => { SoundManager.instance.ShowVoiceBtn(true); }
                    ));
                mono.StartCoroutine(Wait(2.7f,
                    () => 
                    {
                        SpineManager.instance.DoAnimation(show.transform.GetChild(0).gameObject, show.transform.GetChild(0).gameObject.name,false);
                        SpineManager.instance.DoAnimation(show.transform.GetChild(3).gameObject, show.transform.GetChild(3).gameObject.name, false);
                    }
                    ));
                mono.StartCoroutine(Wait(3.5f,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(show.transform.GetChild(1).gameObject, show.transform.GetChild(1).gameObject.name, false);
                        SpineManager.instance.DoAnimation(show.transform.GetChild(4).gameObject, show.transform.GetChild(4).gameObject.name, false);
                    }
                    ));
                mono.StartCoroutine(Wait(4f,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(show.transform.GetChild(2).gameObject, show.transform.GetChild(2).gameObject.name, false);
                        SpineManager.instance.DoAnimation(show.transform.GetChild(5).gameObject, show.transform.GetChild(5).gameObject.name, false);
                    }
                    ));
            }
            if (talkIndex == 2)
            {
                show.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                SpineManager.instance.DoAnimation(xlx,"zhong2",false,
                    () => {

                        SpineManager.instance.DoAnimation(xlx,"ad1",false);
                        xlx2.SetActive(true);SpineManager.instance.DoAnimation(xlx2,"bd1",false);
                        mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,2,null,
                            () =>{ Max.SetActive(false);_canClick = true; }
                            ));
                    }
                    );
            }
            if(talkIndex==3)
            {
                _canvoice = true;
                Max.SetActive(true);
                _canClick = false;
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,null));
            }
                talkIndex++;
        }

        IEnumerator Wait (float time ,Action callback = null)
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
