using ClaseState;
using Spine;
using Spine.Unity;
using Spine.Unity.Modules.AttachmentTools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public enum Target
    {
        HuanTu,
        StartSpeak
    }
    public class CourseHieroglyphsPart1
    {
        GameObject curGo;
        List<int> sortTarget;
        public static GameObject mask;
        SpeakTask speakTask;
        TargetTask targetTask;
        List<SkeletonAnimation> btns;
        Dictionary<int, List<Sprite>> dataDit;
        SkeletonGraphic target;
        int count;
        public static bool isFirst;
        public static bool isOver;
        public static bool isNext;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            curGo.AddComponent<MesManager>();
            curGo.AddComponent<ClassCopyManager>();
            sortTarget = new List<int>() { 4, 1, 2, 3, 6,7,8,5 };
            MesManager.instance.Register("CourseHieroglyphs", (int)Target.HuanTu, NextTarget);
            MesManager.instance.Register("CourseHieroglyphs", (int)Target.StartSpeak, NextTarget);
            count = 0;
            isFirst = true;
            isOver = false;
            isNext = false;
            mask = curTrans.Find("UI3D/mask").gameObject;
            target = curTrans.Find("target").GetComponent<SkeletonGraphic>();
            SpineManager.instance.PlayAnimationState(target, sortTarget[count].ToString(), "0|0");
            OnInit();
            PushStateBtn();
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, count,false);
            AddSpeakTask(time, "Speak,Breath");
            mask.SetActive(true);
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM);
        }

        public void OnInit()
        {
            btns = new List<SkeletonAnimation>();
            GameObject go = curGo.transform.Find("UI3D/bg_kuang").gameObject;
            int count = go.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                btns.Add(go.transform.GetChild(i).GetComponent<SkeletonAnimation>());
            }
            dataDit = new Dictionary<int, List<Sprite>>();
            go = curGo.transform.Find("data").gameObject;
            count = go.transform.childCount;
            for (int i = 0; i < count; i++)
            {
                dataDit[i] = new List<Sprite>();
                GameObject child = go.transform.GetChild(i).gameObject;
                for (int j = 0; j < 2; j++)
                {
                    dataDit[i].Add(child.transform.GetChild(j).GetComponent<Image>().sprite);
                }
            }
        }
        public void NextTarget(params object[] obj)
        {
            isNext = true;
            if (count == 8)
            {
                isOver = true;
                float time1 = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 12, false);
                AddSpeakTask(time1, "Speak,Jump,Breath");
                return;
            }
            SpineManager.instance.PlayAnimationState(target, sortTarget[count].ToString(), "0|0");
            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, count + 1, false);
            AddSpeakTask(time,  "Speak,Breath" );
        }
        public void PushStateBtn()
        {
            for (int i = 0; i < btns.Count; i++)
            {
                SpineManager.instance.PlayAnimationState(btns[i], "ui_right_1", "0|0");
                btns[i].GetComponent<ILObject3DAction>().index = i + 1;
                btns[i].GetComponent<ILObject3DAction>().OnMouseDownLua = OnMouseDown;
                Shader shader = btns[i].gameObject.GetComponent<MeshRenderer>().material.shader;
                Debug.Log("dataDit[i][0]" + dataDit[i][0]);
                SpineManager.instance.CreateRegionAttachmentByTexture(btns[i], "tu1_ui", dataDit[i][0], shader);
                Debug.Log("dataDit1[i][0]" + dataDit[i][0]);
            }
        }
        public void OnMouseDown(int index)
        {
            if (sortTarget[count] == index)
            {
                Shader shader = btns[index - 1].gameObject.GetComponent<MeshRenderer>().material.shader;
                SpineManager.instance.CreateRegionAttachmentByTexture(btns[index - 1], "tu1_ui_L", dataDit[index - 1][1], shader);
                SpineManager.instance.CreateRegionAttachmentByTexture(btns[index - 1], "tu1_ui", dataDit[index - 1][0], shader);
                SpineManager.instance.DoAnimation(btns[index - 1].gameObject, "ui_right_1", false, delegate ()
                    {
                        SpineManager.instance.PlayAnimationState(btns[index - 1], "ui_right_1", "0|0");
                    });
                AddTargetTask(sortTarget[count].ToString());
                count++;
            }
            else
            {
                SpineManager.instance.HideSpineTexture(btns[index - 1], "tu1_ui_L");
                SpineManager.instance.DoAnimation(btns[index - 1].gameObject, "ui_erro_1", false,delegate()
                {
                    SpineManager.instance.PlayAnimationState(btns[index - 1], "ui_right_1", "0|0");
                    
                });
            }
        }
        public void AddSpeakTask(float time, string str)
        {
            
            speakTask = new SpeakTask(time, str);
            object[] obj = { time, str };
            speakTask.AddTask_Run(obj,speakTask);
        }
        public void AddTargetTask(string name)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0);
            targetTask = new TargetTask(name);
            object[] obj = {name};
            targetTask.AddTask_Run(obj,targetTask);
        }
    }
}
