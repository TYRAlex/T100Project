using UnityEngine;
using System;
using System.Collections.Generic;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;

namespace Part1
{
    public class Floor_4
    {

        Transform Eniverment;
        Transform self;
        List<CommonObj> commonObjs;
        Transform AllBones;

        //当前已经挖掘的石头个数
        int CurStoneCount;
        public int CurIndex;
        public Floor_4(Transform eniverment)
        {
            this.Eniverment = eniverment;
            self = Eniverment.Find("Floor_4");

            commonObjs = new List<CommonObj>();

            SetFloor_4();
        }

        private void SetFloor_4()
        {

            CurStoneCount = 0;
            commonObjs.Clear();
            AllBones = Eniverment.parent.Find("AllBones");
            for (int i = 8; i <= 10; i++)
            {

                var obj = self.Find(i.ToString()).gameObject;
                var action = obj.GetComponent<ILObject3DAction>();
                action.index = i - 1;
                var comobj = new CommonObj(obj, i.ToString(), 0);
                //电钻的触发

                action.OnTriggerEnter2DLua = OnElectrodrillEnter;
                action.OnTriggerExit2DLua = OnElectrodrillExit;
                obj.GetComponent<BoxCollider2D>().enabled = true;

                var normalObj = obj.transform.Find("Normal").gameObject;
                var animObj = obj.transform.Find("Anim").gameObject;
                normalObj.SetActive(false);
                PlayLightAnim(animObj, i);

                commonObjs.Add(comobj);
            }
        }


        private void OnElectrodrillEnter(Collider2D other, int index)
        {
            if (!other.name.Equals("electrodrill"))
            {
                Debug.LogError("需要电钻挖石头");
                return;
            }
            else
            {
                CourseTDG4P1L08Part1.isCanTrigger = true;
                CurIndex = index;
                CloseLightAnim(index);
                // PlayAnim(index);
                int toolIndex = 0;
                if (index == 7)
                    toolIndex = 0;
                if (index == 8)
                    toolIndex = 1;
                if (index == 9)
                    toolIndex = 2;
                var comObj = commonObjs[toolIndex];
                CourseTDG4P1L08Part1.toolPosition = comObj.obj.transform.position + new Vector3(0f,1.5f,0f);
            }
        }

        private void OnElectrodrillExit(Collider2D arg1, int arg2)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }

        //播放对应的动画
        public void PlayAnim(int index)
        {
            int curIndex = 0;
            if (index == 7)
                curIndex = 0;
            if (index == 8)
                curIndex = 1;
            if (index == 9)
                curIndex = 2;

            CourseTDG4P1L08Part1.CovoerMask.SetActive(true);
            var comObj = commonObjs[curIndex];
            if (comObj.curCount < 1)
                comObj.curCount++;

            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(false);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(true);

            var skt = AnimObj.GetComponent<SkeletonGraphic>();
            //skt.AnimationState.ClearTracks();
            // skt.AnimationState.SetEmptyAnimations(0);

            string AnimName = comObj.curCount + "_" + comObj.name;
            SpineManager.instance.DoAnimation(AnimObj, AnimName, false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

                //已经成功挖掉一块石头
                if (comObj.curCount >= 1)
                {
                    comObj.obj.SetActive(false);
                    CurStoneCount++;
                    //开启下一环节可以挖骨头
                    if (CurStoneCount >= 3)
                    {
                        Debug.Log("进入下一环节挖骨头");
                        Bones bones = new Bones(AllBones);
                        CourseTDG4P1L08Part1.floor_4 = null;
                        CourseTDG4P1L08Part1.allBones = bones;
                    }
                }
                CourseTDG4P1L08Part1.isCanMove = true;
                CourseTDG4P1L08Part1.isCanTrigger = false;
                CourseTDG4P1L08Part1.CovoerMask.SetActive(false);
            });
        }

        void PlayLightAnim(GameObject animObj, int index)
        {
            animObj.SetActive(true);
            var skt = animObj.GetComponent<SkeletonGraphic>();
            var track = skt.AnimationState.SetAnimation(0, "animation" + index, true);
            track.TrackTime = 5f / 30f;
        }

        void CloseLightAnim(int index)
        {
            var comObj = commonObjs[index - 7];
            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(true);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(false);
        }
    }
}