using UnityEngine;
using System;
using System.Collections.Generic;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;

namespace Part1
{
    public class Floor_3
    {
        Transform Eniverment;
        Transform self;
        List<CommonObj> commonObjs;
        public int CurIndex;

        public Floor_3(Transform eniverment)
        {
            this.Eniverment = eniverment;
            self = Eniverment.Find("Floor_3");

            commonObjs = new List<CommonObj>();
            SetFloor_3();
        }

        private void SetFloor_3()
        {
            commonObjs.Clear();
            var obj = self.Find("7").gameObject;
            var action = obj.GetComponent<ILObject3DAction>();
            action.index = 7;
            var comobj = new CommonObj(obj, "7", 0);

            //锄头的触发
            action.OnTriggerEnter2DLua = OnPickEnter;
            action.OnTriggerExit2DLua = OnPickExit;
            obj.GetComponent<BoxCollider2D>().enabled = true;

            var normalObj = obj.transform.Find("Normal").gameObject;
            var animObj = obj.transform.Find("Anim").gameObject;
            normalObj.SetActive(false);
            PlayLightAnim(animObj, 7);

            commonObjs.Add(comobj);
        }

        private void OnPickEnter(Collider2D other, int index)
        {
            if (!other.name.Equals("pick"))
            {
                Debug.LogError("需要锄头挖小石头");
                return;
            }
            else
            {
                Debug.Log("正确使用锄头");
                // var NormalObj = other.transform.Find("Normal").gameObject;
                // var AnimObj = other.transform.Find("Anim").gameObject;
                // NormalObj.SetActive(false);
                // AnimObj.SetActive(true);

                // //播放工具的音效
                // SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, true);
                //other.GetComponent<BoxCollider2D>().enabled = false;
                // SpineManager.instance.DoAnimation(AnimObj, other.name, false, () =>
                // {
                //     AnimObj.SetActive(false);
                //     other.gameObject.SetActive(false);
                //     other.GetComponent<BoxCollider2D>().enabled = true;
                //     SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                // });
                CourseTDG4P1L08Part1.isCanTrigger = true;
                CurIndex = index;
                CloseLightAnim(index);
                var comObj = commonObjs[0];
                Vector3 pos = new Vector3(0,comObj.obj.transform.position.y + 1f,0);
                CourseTDG4P1L08Part1.toolPosition = pos;
            }
        }

        private void OnPickExit(Collider2D arg1, int arg2)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }

        public void PlayAnim(int index)
        {
            CourseTDG4P1L08Part1.CovoerMask.SetActive(true);
            var comObj = commonObjs[0];
            if (comObj.curCount < 1)
                comObj.curCount++;

            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(false);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(true);

            var skt = AnimObj.GetComponent<SkeletonGraphic>();
            // skt.AnimationState.ClearTracks();
            // skt.AnimationState.SetEmptyAnimations(0);

            string AnimName = comObj.curCount + "_" + comObj.name;
            SpineManager.instance.DoAnimation(AnimObj, AnimName, false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

                if (comObj.curCount >= 1)
                {
                    comObj.obj.SetActive(false);
                    Debug.Log("开始挖下一层");
                    Floor_4 floor_4 = new Floor_4(this.Eniverment);

                    CourseTDG4P1L08Part1.floor_3 = null;
                    CourseTDG4P1L08Part1.floor_4 = floor_4;

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
            track.TrackTime = 5f/30f;
        }

        void CloseLightAnim(int index)
        {
            var comObj = commonObjs[index-7];
            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(true);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(false);
        }
    }
}