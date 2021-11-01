using System;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace ILFramework.HotClass
{
    public class CourseTDG2P2L14Part1
    {
        GameObject curGo;
        GameObject CurtainAnim;
        Transform Buttom;
        string[] CurtainAnimations;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Buttom = curTrans.Find("Content/Buttom");
            CurtainAnim = Buttom.Find("CurtainAnim").gameObject;
            CurtainAnimations = new string[]{"close","open"};
            InitGame();
            
        }

        private void InitGame()
        {
           PlayAnimation(CurtainAnim,CurtainAnimations[0],false,81f);
        }

        void PlayAnimation(GameObject animObj, string animName, bool isLoop, float startFrame = 0f, Action endCallBack = null)
        {
            var skt = animObj.GetComponent<SkeletonGraphic>();
            skt.AnimationState.SetEmptyAnimation(0, 0f);
            animObj.SetActive(true);
            var track = skt.AnimationState.SetAnimation(0, animName, isLoop);
            track.TrackTime = startFrame / 30f;
            track.Complete += (TrackEntry trackEntry) =>
            {
                endCallBack?.Invoke();
            };
        }
    }
}
