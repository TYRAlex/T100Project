using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseNorthlandPart
{
    class AnimationClass : TaskBase
    {
        GameObject tiantian;
        GameObject target;
        GameObject light;
        GameObject endAni;
        bool isOn;
        string lightName;
        string nextLightName;
        bool isSuccess;
        bool isStart;
        public AnimationClass(string lightName,string nextLightName,string functions) : base()
        {
            tiantian = curGo.transform.Find("tiantian").gameObject;
            target = curGo.transform.Find("Camera/tagetAnimation").gameObject;
            light = curGo.transform.Find("Camera/lightAnimation").gameObject;
            endAni = curGo.transform.Find("Camera/endAnimation").gameObject;
            this.lightName = lightName;
            this.nextLightName = nextLightName;            
            FuncName = functions.Split(',').ToList();
            isStart = false;
            isSuccess = false;
            isOn = false;

            target.SetActive(true);
            light.SetActive(true);
            endAni.SetActive(false);

            SpineManager.instance.PlayAnimationState(target.GetComponent<SkeletonAnimation>(), "st");
            //if (CourseNorthlandPart1.speakIndex == 6)
            //{
            //    SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), "st_l");
            //}
            //else
            //{
            //    SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), "st");
            //}
            if (CourseNorthlandPart1.speakIndex == 1)
            {
                SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), "t_s");
            }
            else
            {
                SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), "st");
            }
        }
        public override void OnInit()
        {   

        }
        public void StateSpeak()
        {
            CourseNorthlandPart1.mask.SetActive(true);
            isStart = true;
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, CourseNorthlandPart1.speakIndex);
            SpineManager.instance.DoAnimation(tiantian, "talk");
            mono.StartCoroutine(wait(time));
        }
        public void Speak()
        {
            CourseNorthlandPart1.mask.SetActive(true);
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, CourseNorthlandPart1.speakIndex);
            SpineManager.instance.DoAnimation(tiantian, "talk");
            if(CourseNorthlandPart1.speakIndex<9 && CourseNorthlandPart1.speakIndex>0)
            {
                mono.StartCoroutine(_wait(Light));
                isSuccess = false;
                isOn = false;
            }
            else if(CourseNorthlandPart1.speakIndex < 13 && CourseNorthlandPart1.speakIndex > 9)
            {
                mono.StartCoroutine(_wait(Light));
                isSuccess = true;
                isOn = true;
            }
            else
            {
                isSuccess = false;
                isOn = false;
            }
            mono.StartCoroutine(wait(time));
        }
        IEnumerator _wait(Action act)
        {
            yield return new WaitForSeconds(1.0f);
            act();
        }
        private void Light()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            //SpineManager.instance.DoAnimation(light, lightName + "_l", false, () =>
            //    {
            //        if (isSuccess)
            //        {
            //            SpineManager.instance.DoAnimation(light, lightName + "_s", false, () =>
            //            {
            //                if (nextLightName.Length > 0)
            //                {
            //                    Debug.Log("--------nextLightName-------------" + nextLightName);
            //                    SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), nextLightName + "_l");
            //                }
            //                light.transform.localScale = Vector3.zero;
            //                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
            //                SpineManager.instance.DoAnimation(target, lightName, false, StopAnimation);
            //            });                        
            //        }
            //        else
            //        {
            //            light.transform.localScale = Vector3.zero;
            //        }
            //    });
           
            if (isSuccess)
            {                
                SpineManager.instance.DoAnimation(light, lightName + "_s", false, () =>
                {
                    if (nextLightName.Length > 0)
                    {
                        Debug.Log("--------nextLightName-------------" + nextLightName);
                        SpineManager.instance.PlayAnimationState(light.GetComponent<SkeletonAnimation>(), nextLightName);
                    }
                    light.transform.localScale = Vector3.zero;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1);
                    SpineManager.instance.DoAnimation(target, lightName, false, StopAnimation);
                });
            }
            else
            {
                //SpineManager.instance.DoAnimation(light, lightName + "_l", false, () =>
                //{
                //    light.transform.localScale = Vector3.zero;
                //});
                if (lightName == "st")
                {
                    SpineManager.instance.DoAnimation(target, lightName + "_l", true);
                }
                else
                {
                    SpineManager.instance.DoAnimation(light, lightName + "_l", true);
                }                
            }
            light.transform.localScale = new Vector3(0.5f, 0.5f, 1);
        }
        IEnumerator wait(float time)
        {
            yield return new WaitForSeconds(time);
            SpineManager.instance.DoAnimation(tiantian, "breath");
            if (!isSuccess)
            {
                ClassCopyManager.instance.taskQueue.End();
                CourseNorthlandPart1.mask.SetActive(isSuccess);
                if (isStart)
                {
                    MesManager.instance.Dispatch("CourseNorthlandPart1", (int)AnimationState.STATE);
                }
            }
        }
        public void Breath()
        {
            ClassCopyManager.instance.taskQueue.End();
            if(CourseNorthlandPart1.index == 7)
            {
                MesManager.instance.Dispatch("CourseNorthlandPart1", (int)AnimationState.GAMEOVER);
            }
            else if (isOn)
            {
                MesManager.instance.Dispatch("CourseNorthlandPart1", (int)AnimationState.PLAY);
            }
        }
        private void StopAnimation()
        {
            if(isSuccess)
            {
                //isSuccess = false;
                ClassCopyManager.instance.taskQueue.End();
                CourseNorthlandPart1.mask.SetActive(isSuccess);
            }
           
        }
        public void GameOver()
        {

            light.SetActive(false);
            target.SetActive(false);
            endAni.SetActive(true);

            CourseNorthlandPart1.mask.SetActive(true);
            SpineManager.instance.DoAnimation(endAni, "1", false, () =>
            {
                //SpineManager.instance.SetTimeScale(endAni, 0f);
                SpineManager.instance.DoAnimation(endAni, "2");
            });
            SoundManager.instance.Speaking(tiantian, "talk", SoundManager.SoundType.SOUND, CourseNorthlandPart1.speakIndex, null, () =>
            {
                SpineManager.instance.PlayAnimationDuring(endAni, "2", "0|0.9");
            });
            
            //float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, CourseNorthlandPart1.speakIndex);
            //SpineManager.instance.DoAnimation(tiantian, "talk");
            //mono.StartCoroutine(wait(time));
        }
    }
}
