using CourseRobotPart;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;

namespace ILFramework.HotClass
{
    public enum Robot
    {
        IdleFinaly,
        ScenceEnd
    }
    public class CourseRobotPart1
    {
        static public GameObject curGo;
        int RobotNum;
        int RobotMaxNum;
        static public GameObject mask;
        GameObject back;
        List<RobotTarget> RobotLists;
        List<RobotAnimation> RobotScences;
        ControlMove move;
        ControlMove move1;
        int curIndex;
        GameObject dingding;

        GameObject robot_5;
        SkeletonAnimation robot_5_Animation;

        public static bool canPlayVoice = false;

        void Start(object o)
        {
            curGo = (GameObject)o;

            Transform curTrans = curGo.transform;
            RobotMaxNum = 8;
            RobotNum = 0;
            curIndex = -1;

            robot_5 = curTrans.Find("UI3D/PlayAnimation/6/spine_animation").gameObject;
            robot_5_Animation = robot_5.GetComponent<SkeletonAnimation>();

            mask = curTrans.Find("UI3D/mask").gameObject;
            back = curTrans.Find("UI3D/back").gameObject;
            dingding = curTrans.Find("dingding").gameObject;
            mask.SetActive(true);
            back.SetActive(false);
            back.GetComponent<ILObject3DAction>().OnMouseDownLua = OnMouseBackIdle;
            RobotLists = new List<RobotTarget>();
            RobotScences = new List<RobotAnimation>();
            //Thread.Sleep(5000);
            curGo.AddComponent<MesManager>();
            for(int i = 0;i < RobotMaxNum;i++)
            {
                RobotLists.Add(new RobotTarget(i));
                RobotScences.Add(RobotLists[i].playRobot);
            }
            //PlayRobotIdle();
            Addlistener();
            dingding.SetActive(true);
            Speak();
            SoundManager.instance.BgSoundPart1();
            for(int i = 0; i < RobotScences.Count; i++)
            { 
                RobotScences[i].Hide();
            }
        }

        private void OnMouseBackIdle(int obj)
        {
            Debug.Log("------------OnMouseBackIdle--------------");
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            if(curIndex != -1)
            {
                if(move != null)
                {
                    move.Resat();
                }
                if (move1 != null)
                {
                    move1.Resat();
                }
                move = null;
                move1 = null;
                RobotScences[curIndex].Hide();
                back.SetActive(false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                
            }
            curIndex = -1;
        }

        public void Speak()
        {
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND,0, null, PlayRobotIdle);
        }
        public void PlayRobotIdle()
        {
            SoundManager.instance.skipBtn.SetActive(false);
            RobotLists[RobotNum].PlayIdle();
        }
        public void Addlistener()
        {
            MesManager.instance.Register("RobotTarget", (int)Robot.IdleFinaly, RobotIdleFinily);
            MesManager.instance.Register("RobotAnimation", (int)Robot.ScenceEnd, RobotScenceEnd);
        }

        private void RobotScenceEnd(object[] param)
        {
            Debug.LogError("RobotScenceEnd ----------- " + curIndex);
            curIndex = (int)param[0];
            back.SetActive(true);
            mask.SetActive(false);
     
            if (curIndex == 0 || curIndex == 7)
            {
                Debug.Log(((Transform)param[1]).name);
                Debug.Log(((Transform)param[2]).name);
                move = new ControlMove((Transform)param[1], (Transform)param[2]);
            }
            else if(curIndex == 1)
            {
                move = new ControlMove((Transform)param[1], (Transform)param[3],false);
                move1 = new ControlMove((Transform)param[2], (Transform)param[4],false);
            }
            else if(curIndex == 2)
            {
                move = new ControlMove((Transform)param[1], (Transform)param[2], false);
            }
        }
        public void Update()
        {
            if(move != null)
            {
                move.Update();
            }
            if(move1 != null)
            {
                move1.Update();
            }

            if(robot_5_Animation.AnimationName == "animation3" && canPlayVoice)
            {
                Debug.LogError("��������");
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                canPlayVoice = false;
            }


        }
        private void RobotIdleFinily(object[] param)
        {
            RobotNum++;
            if(RobotNum > RobotMaxNum)
            {
                RobotNum = RobotNum % 9 + 1;
            }
            Debug.Log("----------RobotIdleFinily-----------");
            //��ɻ�����ȫ����������
            if (RobotNum == RobotMaxNum)
            {
                mask.SetActive(false);
            }
            else
            {
                PlayRobotIdle();
            }
            
        }
    }
}
