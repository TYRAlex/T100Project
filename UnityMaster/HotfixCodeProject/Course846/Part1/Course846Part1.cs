using System;
using System.Collections;
using UnityEngine;
using DG.Tweening;
using Spine.Unity;
using UnityEngine.UI;
using UnityEngine.Video;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Runtime.CompilerServices;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course846Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject hxr;
        private GameObject qbyg;
        private GameObject mask;
        private GameObject clickBig;
        private GameObject clickReturn;
        private GameObject Dbell;
        private GameObject zi;
        private List<GameObject> Box;
        private int BoxIndex;
        private GameObject djs;
        private int timeIndex;
        private int alltime;
        private GameObject cg;
        private bool _canReply;
        private bool _canBegin;
        private int _number;
        private Text text;

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
            Dbell = curTrans.Find("Dbell").gameObject;
            timeIndex = 1;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            cg = curTrans.Find("cg").gameObject;
            djs = curTrans.Find("djs").gameObject;
            hxr = curTrans.Find("hxr").gameObject;
            qbyg = curTrans.Find("qbyg").gameObject;
            clickReturn = qbyg.transform.GetChild(0).gameObject;
            clickBig = qbyg.transform.GetChild(1).gameObject;
            mask = curTrans.Find("mask").gameObject;
            zi = curTrans.Find("zi").gameObject;
            text = djs.transform.Find("Text").GetComponent<Text>();
            Box = new List<GameObject>();

            for (int i = 0; i < 7; i++)
            {
                Box.Add(curTrans.Find("Box").GetChild(i).gameObject);
            }

            Util.AddBtnClick(clickReturn, ClickReturnEvent);
            Util.AddBtnClick(clickBig, ClickBigEvent);
            Util.AddBtnClick(Dbell.transform.GetChild(0).gameObject, ClickBeginEvent);

            for (int i = 0; i < Box.Count; i++)
            {
                for (int t = 0; t < 4; t++)
                {
                    Util.AddBtnClick(Box[i].transform.GetChild(0).GetChild(t).gameObject, ReplyEvent);
                }

            }

            GameInit();
            GameStart();
        }

        private void ClickBigEvent(GameObject obj)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND,1,false);
            clickBig.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SpineManager.instance.DoAnimation(qbyg, "dian", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(qbyg, "animation", true);
                    clickReturn.SetActive(true);
                }
                );

        }

        private void ClickReturnEvent(GameObject obj)
        {
            clickReturn.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2, false);
            SpineManager.instance.DoAnimation(qbyg, "suoxiao", false,
                () =>
                {
                    SpineManager.instance.DoAnimation(qbyg, "yd", true);
                    clickBig.SetActive(true);
                    SoundManager.instance.ShowVoiceBtn(true);
                }
                );
        }

        private void ClickBeginEvent(GameObject obj)
        {
            if (!_canBegin)
                return;
            Dbell.transform.GetChild(0).gameObject.SetActive(false);
            zi.SetActive(false);
            Dbell.SetActive(false);
            curTrans.Find("di").gameObject.SetActive(false);
            Bg.transform.GetComponent<RawImage>().texture = bellTextures.texture[0];
            Bg.transform.GetChild(0).gameObject.SetActive(false);
            djs.SetActive(true);
            Next();
        }

        private void Next()
        {
            if (Box.Count > 0)
            {
                _number = 20;
                text.text = _number.ToString();
                mono.StartCoroutine(TimeDJS(1f));
                BoxIndex = Random.Range(0, Box.Count);
                Box[BoxIndex].SetActive(true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,BoxIndex,true);
                curTrans.Find("tp").Find(Box[BoxIndex].name).gameObject.SetActive(true);
                Box[BoxIndex].GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(Box[BoxIndex], Box[BoxIndex].name, true);
                RandomButton(Box[BoxIndex]);
            }
            else
            {
                Win();
            }
        }

        private void RandomButton(GameObject obj)
        {
            List<int> number = new List<int> { 0, 1, 2, 3 };
            for (int i = 0; i < 4; i++)
            {
                int a = Random.Range(0, number.Count);
                number.RemoveAt(a);
                obj.transform.GetChild(0).GetChild(i).SetSiblingIndex(a);
            }
            number.Clear();
        }
        private void ReplyEvent(GameObject obj)
        {
            if (_canReply)
            {
                _canReply = false;
                obj.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.transform.GetChild(0).gameObject.name + "2", false);
                if (obj.name == "zq")
                {
                    mono.StopAllCoroutines();
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,7,false);
                    obj.transform.GetChild(1).gameObject.SetActive(true);
                    obj.transform.GetChild(1).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "x2", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, "x3", false);
                            mono.StartCoroutine(WaitTime(1,
                                () =>
                                {
                                    Box[BoxIndex].SetActive(false);
                                    curTrans.Find("tp").Find(Box[BoxIndex].name).gameObject.SetActive(false);
                                    Box.RemoveAt(BoxIndex);
                                    Next();
                                    _canReply = true;
                                }
                                ));
                        }
                        );
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,8);
                    obj.transform.parent.Find("zq").GetChild(1).gameObject.SetActive(true);
                    Lose();
                }
            }
        }

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator TimeDJS(float time, Action callback = null)
        {
            while(true)
            {
                yield return new WaitForSeconds(1f);
                alltime += 1;
                _number -= 1;
                text.text = _number.ToString();
                if(_number ==0)
                {   Lose();
                    yield break;
                }
            }


            //if (timeIndex != 0)
            //{
            //    djs.transform.GetChild(timeIndex - 1).gameObject.SetActive(false);
            //}
            //timeIndex = 21;
            //djs.transform.GetChild(timeIndex - 1).gameObject.SetActive(true);

            //while (true)
            //{
            //    yield return new WaitForSeconds(time);
            //    if (timeIndex == 1)
            //    {
            //        Lose();
            //        yield break;
            //    }
            //    timeIndex--;
            //    djs.transform.GetChild(timeIndex).gameObject.SetActive(false);
            //    djs.transform.GetChild(timeIndex - 1).gameObject.SetActive(true);

            //    callback?.Invoke();
            //}
        }

        private void Win()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,9,false);
            Bg.transform.GetComponent<RawImage>().texture = bellTextures.texture[1];
            djs.SetActive(false);
            Dbell.SetActive(true);
            curTrans.Find("di").gameObject.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(Dbell, SoundManager.SoundType.VOICE, 3, null,
                () =>
                { SoundManager.instance.ShowVoiceBtn(true); }
                ));
            cg.SetActive(true);
            cg.transform.GetChild(0).GetComponent<Text>().text = alltime.ToString();

        }
        private void Lose()
        {
            mono.StopAllCoroutines();
            mask.SetActive(true);
            Max.SetActive(true);
            Max.transform.SetAsLastSibling();
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                () =>
                { SoundManager.instance.ShowVoiceBtn(true); }
                ));
        }
        private void GameInit()
        {
            //SpineManager.instance.DoAnimation(qbyg,"jing",false);
            curTrans.Find("zi2").gameObject.SetActive(false);
            curTrans.Find("di").gameObject.SetActive(false);
            timeIndex = 21;
            _canBegin = false;
            mask.SetActive(false);
            Dbell.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            zi.SetActive(false);
            clickReturn.SetActive(false);
            clickBig.SetActive(false);
            qbyg.SetActive(false);
            hxr.SetActive(true);
            talkIndex = 1;
            _canReply = true;
            cg.SetActive(false);
            Bg.transform.GetComponent<RawImage>().texture = bellTextures.texture[1];
            Bg.transform.GetChild(0).gameObject.SetActive(true);
            for (int i = 0; i < 7; i++)
            {
                curTrans.Find("Box").GetChild(i).gameObject.SetActive(false);
                curTrans.Find("tp").GetChild(i).gameObject.SetActive(false);
                djs.SetActive(false);
            }
            alltime = 0;
            if (Box.Count > 0)
            {
                curTrans.Find("tp").Find(Box[BoxIndex].name).gameObject.SetActive(false);
                Box[BoxIndex].SetActive(false);
                Box.Clear();
            }

            for (int i = 0; i < 7; i++)
            {
                Box.Add(curTrans.Find("Box").GetChild(i).gameObject);
            }
            for (int i = 0; i < Box.Count; i++)
            {
                for (int t = 0; t < 4; t++)
                {
                    Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject, Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject.name + "1", false);
                    if (Box[i].transform.GetChild(0).GetChild(t).gameObject.name == "zq")
                    {
                        Box[i].transform.GetChild(0).GetChild(t).GetChild(1).gameObject.SetActive(false);
                    }
                }

            }
            _canReply = true;
        }

        private void GameReplay()           
        {
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            djs.SetActive(true);
            cg.SetActive(false);
            alltime = 0;
            if (Box.Count > 0)
            {
                curTrans.Find("tp").Find(Box[BoxIndex].name).gameObject.SetActive(false);
                Box[BoxIndex].SetActive(false);
                Box.Clear();
            }
            
            for (int i = 0; i < 7; i++)
            {
                Box.Add(curTrans.Find("Box").GetChild(i).gameObject);
            }
            for (int i = 0; i < Box.Count; i++)
            {
                for (int t = 0; t < 4; t++)
                {
                    Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject, Box[i].transform.GetChild(0).GetChild(t).GetChild(0).gameObject.name+"1",false);
                    if(Box[i].transform.GetChild(0).GetChild(t).gameObject.name == "zq")
                    {
                        Box[i].transform.GetChild(0).GetChild(t).GetChild(1).gameObject.SetActive(false);
                    }
                }

            }
            _canReply = true;
            Next();
        }


        void GameStart()
        {
            hxr.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(hxr,"animation",true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 11, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0,true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); isPlaying = false; }));

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
                hxr.SetActive(false);
                qbyg.SetActive(true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,10,true);
                curTrans.Find("zi2").gameObject.SetActive(true);
                qbyg.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(qbyg, "yd", true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 1, null,
                    () =>
                    { clickBig.SetActive(true); }
                    ));
            }
            if (talkIndex == 2)
            {
                qbyg.SetActive(false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                curTrans.Find("zi2").gameObject.SetActive(false);
                Max.SetActive(false);
                zi.SetActive(true);
                Dbell.SetActive(true);
                curTrans.Find("di").gameObject.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Dbell, SoundManager.SoundType.VOICE, 2, null,
                    () =>
                    { Dbell.transform.GetChild(0).gameObject.SetActive(true);
                        _canBegin = true;
                    }
                    ));
            }
            if (talkIndex == 3)
            {
                mask.SetActive(false);
                Dbell.SetActive(false);
                Max.SetActive(false);
                curTrans.Find("di").gameObject.SetActive(false);
                GameReplay();
            }
            if (talkIndex != 3)
            {
                talkIndex++;
            }
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
