using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;

namespace ILFramework.HotClass
{
    public class Course736Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;
        private GameObject level1;
        private GameObject level2;
        private bool _canClick;
        private bool[] _jugle; 
        private GameObject left;
        private GameObject next;
        private GameObject sure;
        private GameObject mask;
        private GameObject mask1;
        private GameObject obj1;
        private int number;
        private int temp;
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
            level1 = curTrans.Find("level1").gameObject;
            level2 = curTrans.Find("level2").gameObject;
            left = curTrans.Find("a").gameObject;
            next = curTrans.Find("b").gameObject;
            sure = curTrans.Find("c").gameObject;
            mask = curTrans.Find("mask").gameObject;
            mask1 = curTrans.Find("mask1").gameObject;
            Util.AddBtnClick(left.transform.GetChild(0).gameObject, Left);
            Util.AddBtnClick(next.transform.GetChild(0).gameObject, Next);
            Util.AddBtnClick(sure.transform.GetChild(0).gameObject, Sure);
            _jugle = new bool[2];
            obj1 = curTrans.Find("obj").gameObject;
            GameInit();
            GameStart();
        }

        private void Click(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            Clear();
            if (Convert.ToInt32(obj.name)==temp)
            { _canClick = true; temp = 2;  return; }
            temp = Convert.ToInt32(obj.name);
            SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation", false, () => { _canClick = true;_jugle[Convert.ToInt32(obj.name)]= true; });
        }

        private void Clear()
        {
            for (int i = 0; i < 2; i++)
            {
                _jugle[i] = false;
                level1.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                level1.transform.GetChild(i).GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
            }
        }

        private void Left(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            mask.SetActive(true);
            _canClick = false;
            SpineManager.instance.DoAnimation(left, "a2", false,
                () =>
                {
                    temp = 2;
                    number = 1;
                    obj1.SetActive(false);
                    mask1.SetActive(false);
                    level2.SetActive(false);
                    level1.SetActive(true);
                    Clear();
                    mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; _canClick = true;mask.SetActive(false); }));
                    left.SetActive(false);
                    next.SetActive(true);
                    sure.SetActive(true);
                }
                );
        }

        private void Next(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            mask.SetActive(true);
            SpineManager.instance.DoAnimation(next, "b2", false,
                () =>
                {
                    number = 2;
                    mask1.SetActive(false);
                    _canClick = true;
                    level1.SetActive(false);
                    level2.SetActive(true);
                    left.SetActive(true);
                    SpineManager.instance.DoAnimation(left,"a",false);
                    next.SetActive(false);
                    sure.SetActive(false);
                    Speak(2, () => { SoundManager.instance.ShowVoiceBtn(true); mask.SetActive(true); });
                }
                );

        }
        private void Sure(GameObject obj)
        {
            if (!_canClick)
                return;
            _canClick = false;
            BtnPlaySound();
            SpineManager.instance.DoAnimation(sure, "c2", false,
                () =>
                {
                    mask.SetActive(true);
                    SoundManager.instance.ShowVoiceBtn(true);
                    if(_jugle[0]&&!_jugle[1])
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,5);
                        SpineManager.instance.DoAnimation(level1.transform.GetChild(0).GetChild(0).gameObject,"2",true);
                        SpineManager.instance.DoAnimation(level1.transform.GetChild(1).GetChild(0).gameObject, "1", true);
                    }
                    else if (!_jugle[0] && _jugle[1])
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                        SpineManager.instance.DoAnimation(level1.transform.GetChild(1).GetChild(0).gameObject, "1", true);
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                        SpineManager.instance.DoAnimation(level1.transform.GetChild(1).GetChild(0).gameObject, "3", true);
                    }
                }
                );
        }
        private void GameInit()
        {
            temp = 2;
            number = 1;
            obj1.SetActive(false);
            mask1.SetActive(false);
            Clear();
            talkIndex = 1;
            mask.SetActive(false);
            left.GetComponent<SkeletonGraphic>().Initialize(true);
            left.SetActive(false);
            next.SetActive(true);
            sure.SetActive(true);
            sure.GetComponent<SkeletonGraphic>().Initialize(true);
            next.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(next,"b",false);
            SpineManager.instance.DoAnimation(sure,"c",false);
            level1.SetActive(true);
            level2.SetActive(false);
            for (int i = 0; i < 2; i++)
            {
                Util.AddBtnClick(level1.transform.GetChild(i).gameObject,Click);
            }
        }



        void GameStart()
        {
            Max.SetActive(false);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false; _canClick = true; }));

        }

        private void Speak(int index, Action callback = null)
        {
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, index, null, callback));
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
            if (number==1)
            {
                Speak(1, () => { mask.SetActive(false);mask1.SetActive(true); _canClick = true; });
            }
            if(number==2)
            {
                Speak(3, () => { mask.SetActive(false); });
                level2.SetActive(false);
                obj1.SetActive(true);
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
