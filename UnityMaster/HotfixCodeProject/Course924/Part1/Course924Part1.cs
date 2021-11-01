using System;
using System.Collections;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class Course924Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject jxb;
        private GameObject clickBox;
        private bool _canClick;
        private bool a;
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

            jxb = curTrans.Find("jxb").gameObject;
            clickBox = curTrans.Find("clickBox").gameObject;

            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject,ClickEvent);
            }

            GameInit();
            GameStart();
        }

        private void ClickEvent(GameObject obj)
        {
            if(_canClick)
            {
                _canClick = false;
                if (obj.name=="0")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1,false);
                    if (!a)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
                        obj.transform.GetChild(0).gameObject.SetActive(false);
                        clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                        SpineManager.instance.DoAnimation(curTrans.Find("clickBox").GetChild(1).GetChild(0).gameObject, "jing2", false);
                       
                        SpineManager.instance.DoAnimation(jxb, "animation", false,
                            ()=>
                            {
                                SpineManager.instance.DoAnimation(jxb,"jings",true);
                                obj.transform.GetChild(0).gameObject.SetActive(true);
                                clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                                _canClick = true; a = true;
                            }
                            );
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        obj.transform.GetChild(0).gameObject.SetActive(false);
                        SpineManager.instance.DoAnimation(jxb,"animation5",false,
                            ()=>
                            {
                                SpineManager.instance.DoAnimation(jxb, "jings", true);
                                obj.transform.GetChild(0).gameObject.SetActive(true);
                                _canClick = true;
                            }
                            );
                    }
                }
                else if(obj.name =="2")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    if (!a)
                    {
                        clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                        SpineManager.instance.DoAnimation(jxb, "animation6", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(jxb, "jings", true);
                                clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                                _canClick = true;
                            }
                            );
                    }
                    else
                    {
                        clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                        SpineManager.instance.DoAnimation(jxb, "animation2", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(jxb, "jings", true);
                                clickBox.transform.GetChild(1).GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                                _canClick = true;
                            }
                            );

                    }
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    if (obj.name=="3")
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,2,false); }
                    if (obj.name == "4")
                    { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false); }
                    obj.transform.GetChild(0).gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(jxb, "animation" + Convert.ToInt32(obj.name), false,
                        ()=>
                        {
                            SpineManager.instance.DoAnimation(jxb, "jings", true);
                            obj.transform.GetChild(0).gameObject.SetActive(true);
                            _canClick = true;
                        }
                        );
                }
                
            }
        }

        IEnumerator Wait(float time ,Action callbcak = null)
        {
            yield return new WaitForSeconds(time);
            callbcak?.Invoke();
        }



        private void GameInit()
        {
            //SpineManager.instance.GetCurrentAnimationName(jxb);
            //Debug.LogError(SpineManager.instance.GetCurrentAnimationName(jxb));
            talkIndex = 1;
            a = false;
            for (int i = 0; i < 4; i++)
            {
                clickBox.transform.GetChild(i).GetChild(0).gameObject.SetActive(true);
            }
            jxb.SetActive(false);
            jxb.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            jxb.SetActive(true);
            SpineManager.instance.DoAnimation(jxb, "jings", true);
            //SpineManager.instance.DoAnimation(jxb, "jings", true);
            SpineManager.instance.DoAnimation(clickBox.transform.GetChild(0).GetChild(0).gameObject,"jing",false);
            SpineManager.instance.DoAnimation(clickBox.transform.GetChild(1).GetChild(0).gameObject, "jing5", false);
            SpineManager.instance.DoAnimation(clickBox.transform.GetChild(2).GetChild(0).gameObject, "jing3", false);
            SpineManager.instance.DoAnimation(clickBox.transform.GetChild(3).GetChild(0).gameObject, "jing4", false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { _canClick = true; Max.SetActive(false); isPlaying = false; }));

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

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
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


    }
}
