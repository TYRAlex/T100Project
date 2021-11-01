using System;
using System.Collections.Generic;
using UnityEngine;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;

namespace Part1
{
    public class Floor_2
    {
        Transform Eniverment;
        Transform self;
        List<CommonObj> commonObjs;
        public int CurIndex;

        public Floor_2(Transform eniverment)
        {
            this.Eniverment = eniverment;
            self = Eniverment.Find("Floor_2");

            commonObjs = new List<CommonObj>();
            SetFloor_2();
        }

        void SetFloor_2()
        {
            commonObjs.Clear();
            for (int i = 5; i <= 6; i++)
            {
                var obj = self.Find(i.ToString()).gameObject;
                var action = obj.GetComponent<ILObject3DAction>();
                action.index = i - 1;
                var comobj = new CommonObj(obj, i.ToString(), 0);
                //锄头的触发
                if (i == 5)
                {
                    action.OnTriggerEnter2DLua = OnPickEnter;
                    action.OnTriggerExit2DLua = OnPickExit;
                    obj.GetComponent<BoxCollider2D>().enabled = true;

                    var normalObj = obj.transform.Find("Normal").gameObject;
                    var animObj = obj.transform.Find("Anim").gameObject;
                    normalObj.SetActive(false);
                    PlayLightAnim(animObj, i);
                }
                else
                {
                    //铲子的触发
                    action.OnTriggerEnter2DLua = OnSpadeEnter;
                    action.OnTriggerExit2DLua = OnSpadeExit;
                    obj.GetComponent<BoxCollider2D>().enabled = false;
                }
                commonObjs.Add(comobj);
            }
        }

        private void OnSpadeEnter(Collider2D other, int index)
        {
            if (!other.name.Equals("spade"))
            {
                Debug.LogError("需要铲子挖土");
                return;
            }
            else
            {
                Debug.Log("正确使用铲子");

                CourseTDG4P1L08Part1.isCanTrigger = true;
                CurIndex = index;
                CloseLightAnim(index);

                int toolIndex = index == 4 ? 0 : 1;
                var comObj = commonObjs[toolIndex];
                Vector3 pos = new Vector3(0,comObj.obj.transform.position.y + 1f,0);
                CourseTDG4P1L08Part1.toolPosition = pos;
            }
        }

        private void OnSpadeExit(Collider2D other, int index)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }


        //使用锄头挖石头
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
                CourseTDG4P1L08Part1.isCanTrigger = true;
                CurIndex = index;
                CloseLightAnim(index);
                int toolIndex = index == 4 ? 0 : 1;
                var comObj = commonObjs[toolIndex];
                Vector3 pos = new Vector3(0,comObj.obj.transform.position.y + 1f,0);
                CourseTDG4P1L08Part1.toolPosition = pos;
            }
        }

        private void OnPickExit(Collider2D other, int index)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }



        public void PlayAnim(int index)
        {
            CourseTDG4P1L08Part1.CovoerMask.SetActive(true);
            int curIndex = index == 4 ? 0 : 1;
            var comObj = commonObjs[curIndex];
            if (comObj.curCount < 1)
                comObj.curCount++;

            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(false);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(true);

            string AnimName = comObj.curCount + "_" + comObj.name;

            var skt = AnimObj.GetComponent<SkeletonGraphic>();
            //skt.AnimationState.SetEmptyAnimations(0);
            SpineManager.instance.DoAnimation(AnimObj, AnimName, false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

                if (comObj.curCount >= 1)
                {
                    comObj.obj.SetActive(false);
                    if (comObj.obj.name.Equals("5"))
                    {
                        //小石头挖完，开始挖沙子,激活沙子的触发器
                        commonObjs[1].obj.GetComponent<BoxCollider2D>().enabled = true;
                        commonObjs[1].obj.transform.Find("Normal").gameObject.SetActive(false);
                        PlayLightAnim(commonObjs[1].obj.transform.Find("Anim").gameObject, 6);
                    }
                    else
                    {
                        //进入新的环节
                        Debug.Log("开始挖下一层");
                        Floor_3 floor_3 = new Floor_3(this.Eniverment);

                        CourseTDG4P1L08Part1.floor_2 = null;
                        CourseTDG4P1L08Part1.floor_3 = floor_3;

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
            var comObj = commonObjs[index - 4];
            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(true);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(false);
        }
    }
}