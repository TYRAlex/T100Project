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
    class RobotTarget
    {
        SkeletonAnimation playSkeleton;
        ILObject3DAction targetOnclick;
        List<string> idleNames;
        public Transform parent;
        int index;
        string tag = "idle";
        public RobotAnimation playRobot;
        public MonoBehaviour mono;
        public RobotTarget(int index)
        {
            this.parent = CourseRobotPart1.curGo.transform.Find("UI3D/btn");
            this.index = index;
            this.playRobot = new RobotAnimation(index);
            this.mono = CourseRobotPart1.curGo.GetComponent<MonoBehaviour>();
            Init();
        }
        public void Init()
        {
            targetOnclick = parent.GetChild(index).GetComponent<ILObject3DAction>();
            targetOnclick.OnMouseDownLua = OnMouseDown;

            playSkeleton = targetOnclick.transform.GetChild(0).GetComponent<SkeletonAnimation>();
            SetIdleNames();
            TargetInit();
        }
        public void SetIdleNames()
        {
            SkeletonData data = playSkeleton.Skeleton.Data;
            int lengh = data.Animations.Count;
            idleNames = new List<string>();
            for (int i = 0; i < lengh; i++)
            {
                string name = data.Animations.Items[i].Name;
                //name是否包含tag字符
                if (name.IndexOf(tag) > -1)
                {
                    idleNames.Add(name);
                }
            }
        }
        public void TargetInit()
        {
            SpineManager.instance.PlayAnimationState(playSkeleton, idleNames[0]);
        }
        /// <summary>
        /// 递归播放多个序列动画，播放完整个序列动画后发出消息
        /// </summary>
        /// <param name="index"></param>
        public void PlayIdle(int index = 0)
        {
            Debug.Log("index -----------------" + index + "idleNames.Count : " + idleNames.Count);
            if (index < idleNames.Count)
            {
                bool ison = index == idleNames.Count - 1 ? true : false;
                SpineManager.instance.DoAnimation(playSkeleton.gameObject, idleNames[index], ison);
                mono.StartCoroutine(wait(ison, index));
            }
        }
        IEnumerator wait(bool ison, int index, float time = 1)
        {
            yield return new WaitForSeconds(time);
            if (!ison)
            {
                index = index + 1;
                PlayIdle(index);
                SendIdleEnd();
            }
        }
        public void SendIdleEnd()
        {
            //发送当前机器人进场完毕
            MesManager.instance.Dispatch("RobotTarget", (int)Robot.IdleFinaly);
        }
        public void OnMouseDown(int _index)
        {
            Debug.LogError("Index : " + index);


            float time = SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2);
            CourseRobotPart1.mask.SetActive(true);
            mono.StartCoroutine(waitOnclick(time));

        }
        IEnumerator waitOnclick(float time)
        {
            yield return new WaitForSeconds(time / 2);
            //播放动画场景
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, this.index + 1);
            playRobot.PlayAnimation();
        }
    }
}
