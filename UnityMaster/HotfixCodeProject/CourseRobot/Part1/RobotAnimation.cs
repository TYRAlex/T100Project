using ILFramework;
using ILFramework.HotClass;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


namespace CourseRobotPart
{
    class RobotAnimation
    {
        List<SkeletonAnimation> animations;
        
        GameObject parent;
        string tag = "animation";
        string tag1 = "idle2";
       // string findTag = "spine";
        Dictionary<int, List<string>> AnimationNames;
        int index;
        GameObject target;
        bool isEnd;
        int maxTime_index;
        public RobotAnimation(int index)
        {
            this.index = index;
            this.parent = CourseRobotPart1.curGo.transform.Find("UI3D/PlayAnimation").gameObject;
            Init();
        }
        public void InitState()
        {
            for (int i = 0; i < animations.Count; i++)
            {
                SpineManager.instance.ClearTrack(animations[i].gameObject);
            }
            if (index != 1 && index != 7)
            {   
               
                SpineManager.instance.PlayAnimationState(animations[animations.Count - 1], AnimationNames[animations.Count - 1][0]);
                
            }
            else if (index == 7)
            {
                SpineManager.instance.PlayAnimationState(animations[animations.Count - 1], "animation");
            }
            else
            {
                SpineManager.instance.PlayAnimationState(animations[0], AnimationNames[0][0]);
                SpineManager.instance.PlayAnimationState(animations[1], AnimationNames[1][0]);
               
            }
        }
        public void PlayAnimation()
        {
            Debug.Log("-----------------PlayAnimation-----------------");
            CourseRobotPart1.canPlayVoice = true;
            target.transform.localScale = Vector3.one;
            Debug.LogError(" 播放动画:" + index);
            PlayParent();
        }
        public void Init()
        {
            isEnd = true;
            target = parent.transform.GetChild(index).gameObject;
            SetAnimations();
            maxTime_index = CheckTime();
            maxTime_index = animations.Count - 1;
            SetIdleNames();
        }
        public void Hide()
        {
            InitState();
            target.transform.localScale = Vector3.zero;
            isEnd = true;
            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].AnimationState.Event -= PlayEvent;
            }
        }
        public void PlayParent(int index = 0)
        {
            Debug.LogError("AnimationCount:" + animations.Count);

            for (int i = 0; i < animations.Count; i++)
            {
                animations[i].AnimationState.Event += PlayEvent;
                Debug.LogError(111111);
                Debug.LogError("Add ----------" + i);
            }


            for (int i = 0; i < animations.Count; i++)
            {
                Debug.LogError("11111++:" + this.index);
                Play(index, i);
            }
        }
        public void PlayEvent(TrackEntry trackEntry, Event e)
        {
            Debug.LogError("执行了播放语音回调");
            Debug.LogError("audio: "+ e.Data.Name +",indexe: " + index);

            if (e.Data.Name == "audio")
            {
                if(index == 4)
                {
                    Debug.Log("audio --- " + index);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, true);
                }
                else if(index == 7)
                {
                    Debug.Log("audio --- " + index);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                }
                else if(index == 0)
                {
                    Debug.Log("audio --- " + index);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, true);
                }
                else if(index == 6)
                {
                    Debug.Log("audio --- " + index);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                    
                }
            }
            else if((e.Data.Name == "audio1"))
            {
                Debug.Log("audio1 --- " + index);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
            }

        }
        public void Play(int index,int i)
        {
            if (index < AnimationNames[i].Count)
            {
                bool ison = index == AnimationNames[i].Count - 1 ? true : false;
                SpineManager.instance.DoAnimation(animations[i].gameObject, AnimationNames[i][index], ison, () =>
                {
                    if (!ison)
                    {
                        Play(++index, i);
                    }
                    else
                    {
                        if (maxTime_index == i && isEnd)
                        {
                            isEnd = false;
                            SendScenceEnd();
                        }
                    }
                });
            }
        }
        public int CheckTime()
        {
            List<float> timeList = new List<float>();
            for(int i = 0; i < animations.Count;i++)
            {
                SkeletonData data = animations[i].Skeleton.Data;
                int lengh = data.Animations.Count;
                float curTime = 0;
                for (int j = 0; j < lengh; j++)
                {
                    float time = data.Animations.Items[j].Duration;
                    curTime += time;
                }
                timeList.Add(curTime);
            }
            return FindMaxNum(timeList);
        }
        public int FindMaxNum(List<float>curList)
        {
            int index = 0;
            float max = curList[index];
            for(int i = 1;i < curList.Count;i++)
            {
                if(max < curList[i])
                {
                    index = i;
                    max = curList[index];
                }
            }
            return index;
        }
        public void SetIdleNames()
        {
            AnimationNames = new Dictionary<int, List<string>>();
            for (int i = 0;i < animations.Count;i++)
            {
                SkeletonData data = animations[i].Skeleton.Data;
                int lengh = data.Animations.Count;
                List<string> slotsName = new List<string>();
                for(int j = 0;j < lengh;j++)
                {
                    string name = data.Animations.Items[j].Name;
                    if(name.IndexOf(tag) > -1)
                    {
                        slotsName.Add(name);
                    }
                    if(index != 6)
                    {
                        if (name.IndexOf(tag1) > -1)
                        {
                            slotsName.Insert(0, name);
                        }
                    }
                    if(index == 4 && animations[i].name == "bg_spine")
                    {
                        if (name.IndexOf("idle") > -1)
                        {
                            slotsName.Insert(0, name);
                        }
                    }
                }
                AnimationNames[i] = slotsName;
            }
            if(index == 5 || index == 6)
            {
                AnimationNames[animations.Count - 1].Add("idle2");
            }
        }
        public void SetAnimations()
        {
            animations = target.GetComponentsInChildren<SkeletonAnimation>().ToList();
            
           
        }
        public void SendScenceEnd()
        {
            Debug.Log("----------------SendScenceEnd--------------");
            animations[animations.Count - 1].AnimationState.Event -= PlayEvent;
            object[] obj;
            if (index == 0 || index == 7 || index == 2)
            {
                obj = new object[] { index, target.transform.Find("points"), animations[animations.Count - 1].transform };
            }
            else if(index == 1)
            {
                obj = new object[] { index, target.transform.Find("points1"),target.transform.Find("points2"), animations[0].transform, animations[1].transform };
            }
            else
            {
                obj = new object[] { index };
            }
            MesManager.instance.Dispatch("RobotAnimation", (int)Robot.ScenceEnd, obj);
        }
    }
    
}
