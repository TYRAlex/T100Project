using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ILFramework.HotClass
{
    public class Course7310Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        private GameObject Box;
        private bool _canclick;
        private bool[] _jugle;
        private bool[] _jugle2;
        private GameObject Box2;
        private GameObject _btn;
        private bool _canpress;
        private bool _cando;
        private float timesum;
        private GameObject show;
        private GameObject mymask;

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

            mymask = curTrans.Find("mymask").gameObject;
            Box = curTrans.Find("box").gameObject;
            _jugle = new bool[3];
            _jugle2 = new bool[2];
            Box2 = curTrans.Find("box2").gameObject;
            show = Box2.transform.GetChild(1).gameObject;
            _btn = Box2.transform.GetChild(0).gameObject;

            Box2.Show(); show.Show();
            Box2.GetComponent<SkeletonGraphic>().Initialize(true);
            show.GetComponent<SkeletonGraphic>().Initialize(true);

            for (int i = 0; i < 3; i++)
            {
                Util.AddBtnClick(Box.transform.GetChild(i).gameObject, Click);
            }
            UIEventListener.Get(_btn).onDown = btndown;
            UIEventListener.Get(_btn).onUp = btnup;
            UIEventListener.Get(_btn).onExit = btnexit;

            curTrans.Find("monos").GetComponent<MonoScripts>().UpdateCallBack = myUpdate;
            GameInit();
            GameStart();
        }

        private void myUpdate()
        {
            if (_canpress && _cando)
            {
                timesum += Time.deltaTime;
            }
            if (timesum < 1f && _cando && !_canpress)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,5,null,
                    () => { _btn.SetActive(true); show.SetActive(false); show.GetComponent<SkeletonGraphic>().Initialize(true); timesum = 0f; _jugle2[0] = true; Jugle2(); }
                    ));
                _btn.SetActive(false);
                _cando = false;
                SpineManager.instance.DoAnimation(Box2, "duan", false);
                show.SetActive(true);
                SpineManager.instance.DoAnimation(show, "dianti", false);

            }
            else if (timesum >= 1f && _cando)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SoundManager.instance.ShowVoiceBtn(false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 6, null,
                      () => { _btn.SetActive(true); show.SetActive(false); show.GetComponent<SkeletonGraphic>().Initialize(true); timesum = 0f; _jugle2[1] = true; Jugle2(); }
                      ));
                _btn.SetActive(false);
                _cando = false;
                SpineManager.instance.DoAnimation(Box2, "chang", false);
                show.SetActive(true);
                SpineManager.instance.DoAnimation(show, "yinshuiji", false);

            }
        }

        private void btndown(PointerEventData eventData)
        {
            _canpress = true;
            _cando = true;
        }
        private void btnup(PointerEventData eventData)
        {
            _canpress = false;
        }
        private void btnexit(GameObject obj)
        {
            _canpress = false;
        }
        private void Click(GameObject obj)
        {
            if (!_canclick)
                return;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0);
            SoundManager.instance.ShowVoiceBtn(false);
            _canclick = false;
            mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,Convert.ToInt32(obj.name),null,
                () => {
                    _canclick = true;
                    _jugle[Convert.ToInt32(obj.name) - 1] = true; Jugle();
                }
                ));
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "animation" + (Convert.ToInt32(obj.name) + 1).ToString(), false);
            SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "zi" + obj.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "zijing" + obj.name, false); 
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
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void Jugle2()
        {
            for (int i = 0; i < 2; i++)
            {
                if (!_jugle2[i])
                    return;
            }
            SoundManager.instance.ShowVoiceBtn(true);
        }

        private void GameInit()
        {
            timesum = 0f;
            talkIndex = 1;
            for (int i = 0; i < 3; i++)
            {
                Box.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                Box.transform.GetChild(i).GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
            }
            show.SetActive(false);
            Box.SetActive(true);
            Box2.SetActive(false);
            mymask.SetActive(false);
            _btn.SetActive(true);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
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
                _canclick = false;
                Box.SetActive(false);
                Box2.SetActive(true);
                mymask.SetActive(true);
                SpineManager.instance.DoAnimation(Box2, "anniu", true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                    () => { _canclick = true;mymask.SetActive(false); SpineManager.instance.DoAnimation(Box2, "jing", false); }
                    ));
            }
            if(talkIndex==2)
            {
                _btn.SetActive(false);
                SpineManager.instance.DoAnimation(Box2,"biaoxiandou",true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 7, null, null));
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
