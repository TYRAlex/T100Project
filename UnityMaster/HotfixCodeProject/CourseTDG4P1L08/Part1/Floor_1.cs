using System;
using System.Collections.Generic;
using UnityEngine;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;

namespace Part1
{
    public class Floor_1
    {
        Transform Eniverment;
        Transform self;
        GameObject Things;
        List<CommonObj> commonObjs;
        //当前已经挖掘的石头个数
        int CurStoneCount;
        public int CurIndex;
        public Floor_1(Transform eniverment)
        {
            this.Eniverment = eniverment;
            self = Eniverment.Find("Floor_1");
            Things = Eniverment.Find("Things").gameObject;
            commonObjs = new List<CommonObj>();

            SetFloor_1();
        }

        public void SetFloor_1()
        {
            CurStoneCount = 0;
            commonObjs.Clear();
            Things.SetActive(true);
            for (int i = 2; i <= self.childCount + 1; i++)
            {

                var obj = self.Find(i.ToString()).gameObject;
                var action = obj.GetComponent<ILObject3DAction>();
                action.index = i - 2;
                var comobj = new CommonObj(obj, i.ToString(), 0);
                //电钻的触发
                if (i < 4)
                {
                    action.OnTriggerEnter2DLua = OnElectrodrillEnter;
                    action.OnTriggerExit2DLua = OnElectrodrillExit;
                    obj.GetComponent<PolygonCollider2D>().enabled = true;

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



        //铲子的触发
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

                var comObj = commonObjs[index];
                Vector3 pos = new Vector3(0,comObj.obj.transform.position.y + 1f,0);
                CourseTDG4P1L08Part1.toolPosition = pos;
            }
        }

        private void OnSpadeExit(Collider2D other, int index)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }

        //电钻的触发
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

                var comObj = commonObjs[index];
                CourseTDG4P1L08Part1.toolPosition = comObj.obj.transform.position + new Vector3(0f, 1f, 0f);
            }
        }

        private void OnElectrodrillExit(Collider2D other, int index)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }


        //播放对应的动画
        public void PlayAnim(int index, bool isStone)
        {
            CourseTDG4P1L08Part1.CovoerMask.SetActive(true);
            var comObj = commonObjs[index];

            if (comObj.curCount < 1)
                comObj.curCount++;

            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(false);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(true);

            string AnimName = comObj.curCount + "_" + comObj.name;
            if (comObj.name == "4")
            {
                Things.SetActive(false);
            }
            SpineManager.instance.DoAnimation(AnimObj, AnimName, false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

                if (isStone)
                {
                    //已经成功挖掉一块石头
                    if (comObj.curCount >= 1)
                    {
                        comObj.obj.SetActive(false);
                        CurStoneCount++;
                        //开启下一环节可以挖沙子了
                        if (CurStoneCount >= 2)
                        {
                            commonObjs[2].obj.GetComponent<BoxCollider2D>().enabled = true;
                            commonObjs[2].obj.transform.Find("Normal").gameObject.SetActive(false);
                            PlayLightAnim(commonObjs[2].obj.transform.Find("Anim").gameObject, 4);

                        }
                    }
                }
                else
                {
                    //土挖完了开始挖下一层了
                    if (comObj.curCount >= 1)
                    {
                        Debug.Log("触发下一层");
                        comObj.obj.SetActive(false);
                        Floor_2 floor_2 = new Floor_2(this.Eniverment);

                        CourseTDG4P1L08Part1.floor_1 = null;
                        CourseTDG4P1L08Part1.floor_2 = floor_2;
                    }
                }

                CourseTDG4P1L08Part1.CovoerMask.SetActive(false);
                CourseTDG4P1L08Part1.isCanMove = true;
                CourseTDG4P1L08Part1.isCanTrigger = false;
            });
        }

        void PlayLightAnim(GameObject animObj, int index)
        {
            Debug.Log(animObj.name);
            animObj.SetActive(true);
            var skt = animObj.GetComponent<SkeletonGraphic>();
            if (index != 1)
            {
                var track = skt.AnimationState.SetAnimation(0, "animation" + index, true);
                track.TrackTime = 5f / 30f;
            }
            else
            {
                var track = skt.AnimationState.SetAnimation(0, "animation", true);
                track.TrackTime = 5f / 30f;
            }
        }
        void CloseLightAnim(int index)
        {
            var comObj = commonObjs[index];
            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(true);
            Things.SetActive(true);
            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(false);
        }
    }
}
