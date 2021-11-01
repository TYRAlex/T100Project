using System;
using System.Collections;
using UnityEngine;
using Spine.Unity;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course7311Part3
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
        private GameObject level3;
        private GameObject _mask;
        private GameObject _mask2;
        private bool _canClick;
        private bool[] _jugle;
        private bool[] _jugle2;
        private bool[] answer;
        private bool[] answer2;
        private int number;
        private GameObject left;
        private GameObject next;
        private GameObject sure;
        private GameObject[] abc;
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
            level3 = curTrans.Find("level3").gameObject;
            left = curTrans.Find("a").gameObject;
            next = curTrans.Find("b").gameObject;
            sure = curTrans.Find("c").gameObject;
            _mask = curTrans.Find("mask").gameObject;
            _mask2 = curTrans.Find("mask1").gameObject;
            _jugle = new bool[4];
            _jugle2 = new bool[4];
            answer = new bool[4] { true, false, false, false };
            answer2 = new bool[4] { false, false, true, false };
            abc = new GameObject[3] { level1, level2, level3 };

            Util.AddBtnClick(left.transform.GetChild(0).gameObject,Left);
            Util.AddBtnClick(next.transform.GetChild(0).gameObject, Next);
            Util.AddBtnClick(sure.transform.GetChild(0).gameObject, Sure);
            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(level1.transform.GetChild(i).gameObject,Click1);
                Util.AddBtnClick(level2.transform.GetChild(i).gameObject, Click2);
            }
            GameInit();
            GameStart();
        }







        private void GameInit()
        {
            talkIndex = 1;
            _mask.SetActive(true);
            number = 1;
            level1.SetActive(true);
            level2.SetActive(false);
            level3.SetActive(false);
            Clear(level1);
            Clear(level2);
            sure.SetActive(true);
            left.SetActive(false);
            next.SetActive(true);
            left.GetComponent<SkeletonGraphic>().Initialize(true);
            sure.GetComponent<SkeletonGraphic>().Initialize(true);
            next.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(next, "b", false);
            SpineManager.instance.DoAnimation(sure, "c", false);
            _mask2.SetActive(false);
        }



        void GameStart()
        {

            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _mask.SetActive(false); _canClick = true; }));

        }

        private void Click1(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            if (!_jugle[Convert.ToInt32(obj.name)])
            {
                Clear(level1);
                _jugle[Convert.ToInt32(obj.name)] = true;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation", false, () => { _canClick = true; });
            }
            else
            {
                _jugle[Convert.ToInt32(obj.name)] = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "0", false, () => { _canClick = true; });
            }
        }

        private void Click2(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            if (!_jugle2[Convert.ToInt32(obj.name)])
            {
                Clear(level2);
                _jugle2[Convert.ToInt32(obj.name)] = true;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "animation", false, () => { _canClick = true; });
            }
            else
            {
                _jugle2[Convert.ToInt32(obj.name)] = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "0", false, () => { _canClick = true; });
            }
        }

        private void Sure(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            _mask2.SetActive(true);
            SpineManager.instance.DoAnimation(sure,"c2",false,
                () => 
                {
                    if(number==1)
                    {
                        QD(_jugle,answer,level1);
                    }
                    else
                    {
                        QD(_jugle2, answer2, level2);
                    }
                }
                );
        }

        private void QD(bool[] a,bool[] b,GameObject obj)
        {
            int temp =0;
            for (int i = 0; i < 4; i++)
            {
                if(a[i])
                {
                    temp = 1;
                }
            }
            if (temp != 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    if (b[i])
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                        QDplus(obj.transform.GetChild(i).GetChild(0).gameObject,3);
                    }
                }
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                if(a[i]&&b[i])
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,4,false);
                    QDplus(obj.transform.GetChild(i).GetChild(0).gameObject,1);
                }
                if(a[i]==false&&b[i]==true)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                    QDplus(obj.transform.GetChild(i).GetChild(0).gameObject, 1);
                }
                if(a[i]&&!b[i])
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5, false);
                    QDplus(obj.transform.GetChild(i).GetChild(0).gameObject, 2);
                }
            }
        }

        private void QDplus(GameObject obj,int index)
        {
            SpineManager.instance.DoAnimation(obj, index.ToString()+"1", false,
                        () =>
                        {
                            _canClick = true;
                            SpineManager.instance.DoAnimation(obj, index.ToString(), true);
                            if(number==2&&index!=1)
                            {
                                _mask.SetActive(true);
                                mono.StartCoroutine(SpeckerCoroutine(Max,SoundManager.SoundType.VOICE,1,null,()=> { _mask.SetActive(false); }));
                            }
                        }
                        );
        }
        private void Clear(GameObject obj)
        {
            if(obj.name=="level3")
            {
                return;
            }
            for (int i = 0; i < 4; i++)
            {
                _jugle[i] = false;
                _jugle2[i] = false;
                obj.transform.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(i).GetChild(0).gameObject,"kong",false);
                obj.transform.GetChild(i).GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(i).GetChild(1).gameObject, "0", false);
            }
        }
        private void Left(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            SpineManager.instance.DoAnimation(left, "a2", false,
                () => 
                {
                    _mask2.SetActive(false);
                    _canClick = true;
                    number--;
                    Clear(abc[number - 1]);
                    abc[number].SetActive(false);
                    abc[number - 1].SetActive(true);
                    next.SetActive(true);
                    sure.SetActive(true);
                    SpineManager.instance.DoAnimation(next, "b", false);
                    SpineManager.instance.DoAnimation(sure, "c", false);
                    if (number == 1)
                    {
                        left.SetActive(false);
                    }
                }
                );
        }
        private void Next(GameObject obj)
        {
            if (!_canClick)
                return;
            BtnPlaySound();
            _canClick = false;
            SpineManager.instance.DoAnimation(next, "b2", false,
                () => 
                {
                    _mask2.SetActive(false);
                    _canClick = true; 
                    number++;
                    Clear(abc[number - 1]);
                    abc[number - 2].SetActive(false);
                    abc[number - 1].SetActive(true);
                    left.SetActive(true);
                    SpineManager.instance.DoAnimation(left,"a",false);
                    if (number == 3)
                    {
                        next.SetActive(false);
                        sure.SetActive(false);
                    }
                }
                );
            
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
