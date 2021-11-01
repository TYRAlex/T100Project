using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;
using ILFramework;
using ILFramework.HotClass;
using UnityEngine.UI;
using Spine.Unity;
using Spine;

namespace Part1
{
    public class Bones
    {
        Transform AllBones;
        List<CommonObj> commonObjs;
        int CurBoneCount;
        GameObject Bag;

        GameObject NumAnimObj;
        //结束时的骨头组合界面
        GameObject EndPanel_1;
        GameObject TigerBone;
        GameObject Light;
        GameObject TouchMask;

        //结束后的游戏胜利界面
        GameObject EndPanel_2;
        GameObject OverAnim;
        // GameObject ReplayBtn;

        GameObject CoverMask;
        bool canEnd;
        public int CurIndex;

        public Bones(Transform allBones)
        {
            this.AllBones = allBones;
            canEnd = false;
            Bag = this.AllBones.parent.Find("ToolsBar/Btns/Bag").gameObject;
            NumAnimObj = this.AllBones.parent.Find("ToolsBar/NumAnim").gameObject;
            EndPanel_1 = this.AllBones.parent.parent.Find("EndPanel_1").gameObject;
            EndPanel_2 = this.AllBones.parent.parent.Find("EndPanel_2").gameObject;

            //thing of endpanel_1
            TigerBone = EndPanel_1.transform.Find("TigerBone").gameObject;
            Light = EndPanel_1.transform.Find("Light").gameObject;
            TouchMask = EndPanel_1.transform.Find("TouchMask").gameObject;
            //thing of endpanel_2 
            OverAnim = EndPanel_2.transform.Find("OverAnim").gameObject;
            //ReplayBtn = EndPanel_2.transform.Find("ReplayBtn").gameObject;

            CoverMask = this.AllBones.parent.parent.Find("CoverMask").gameObject;
            commonObjs = new List<CommonObj>();
            SetBones();
        }

        private void SetBones()
        {
            commonObjs.Clear();
            CurBoneCount = 0;
            for (int i = 1; i <= 5; i++)
            {

                var obj = this.AllBones.Find(i.ToString()).gameObject;
                var action = obj.GetComponent<ILObject3DAction>();
                action.index = i - 1;
                var comobj = new CommonObj(obj, i.ToString(), 0);
                obj.GetComponent<Image>().enabled = false;

                //刷子的触发
                action.OnTriggerEnter2DLua = OnBrushEnter;
                action.OnTriggerExit2DLua = OnBrushExit;
                action.OnPointDownLua = ClickBone;
                obj.GetComponent<BoxCollider2D>().enabled = true;


                var normalObj = obj.transform.Find("Normal").gameObject;
                var animObj = obj.transform.Find("Anim").gameObject;
                normalObj.SetActive(false);
                PlayLightAnim(animObj, i);

                //初始化结束界面和结束胜利界面
                ResetEndPanl();


                commonObjs.Add(comobj);
            }

            //注册背包的回调
            Bag.GetComponent<ILObject3DAction>().OnPointDownLua = ClickBagCallBack;
        }

        void ResetEndPanl()
        {
            CoverMask.SetActive(false);

            Light.SetActive(false);
            TigerBone.SetActive(false);
            TouchMask.GetComponent<ILObject3DAction>().OnPointDownLua = ClickEndPanel;
            EndPanel_1.SetActive(false);

            // ReplayBtn.GetComponent<ILObject3DAction>().OnPointDownLua = ClickReplayBtnCallBack;
            OverAnim.SetActive(false);
            // ReplayBtn.SetActive(false);

            EndPanel_2.SetActive(false);
        }

        private void ClickReplayBtnCallBack(int index)
        {
            Debug.Log("重新玩游戏");
            CourseTDG4P1L08Part1.InitGame();
        }

        private void ClickEndPanel(int index)
        {
            if (!canEnd)
                return;

            Debug.Log("打开胜利界面");
            EndPanel_2.SetActive(true);
            OverAnim.SetActive(true);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 0, null, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            });
            SpineManager.instance.DoAnimation(OverAnim, "animation", false, () =>
            {
                SpineManager.instance.DoAnimation(OverAnim, "idle");
                
                
                LogicManager.instance.SetReplayEvent(() =>
                {
                    CourseTDG4P1L08Part1.InitGame();
                });
                LogicManager.instance.ShowReplayBtn(true);

            });
        }

        private void ClickBagCallBack(int index)
        {
            Debug.Log("点击背包");
            if (CurBoneCount >= 5)
            {
                Debug.Log("骨头满了");
                CoverMask.SetActive(true);

                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

                var BagAnimObj = Bag.transform.Find("Anim").gameObject;
                SpineManager.instance.DoAnimation(BagAnimObj, "Bg1", false, () =>
                {
                    EndPanel_1.SetActive(true);
                    TigerBone.SetActive(true);
                    //骨头洒出来
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,8);
                    SpineManager.instance.DoAnimation(TigerBone, "1", false, () =>
                    {
                        //骨头组合
                        SpineManager.instance.DoAnimation(TigerBone, "1_1", false, () =>
                        {
                            Light.SetActive(true);
                            SpineManager.instance.DoAnimation(Light, "2");
                            canEnd = true;
                        });
                    });
                });

            }
            else
            {
                Debug.Log("骨头没满");

            }

        }

        private void OnBrushEnter(Collider2D other, int index)
        {
            if (!other.name.Equals("brush"))
            {
                Debug.LogError("需要刷子清理化石");
                return;
            }
            else
            {
               
                CourseTDG4P1L08Part1.isCanTrigger = true;
                CurIndex = index;
                CloseLightAnim(index);
                var comObj = commonObjs[index];
                CourseTDG4P1L08Part1.toolPosition = comObj.obj.transform.position + new Vector3(0,0.5f,0f);
            }
        }

        private void OnBrushExit(Collider2D arg1, int arg2)
        {
            CourseTDG4P1L08Part1.isCanTrigger = false;
        }

        private void ClickBone(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
            var comObj = commonObjs[index];
            if (comObj.curCount >= 1)
            {
                GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
                NormalObj.SetActive(false);

                GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
                AnimObj.SetActive(true);

                Vector3 oldPosition = comObj.obj.transform.localPosition;
                CoverMask.SetActive(true);
                SpineManager.instance.DoAnimation(AnimObj, "2_" + comObj.name, false, () =>
                {
                    SpineManager.instance.DoAnimation(AnimObj, "3_" + comObj.name);

                    comObj.obj.transform.DOLocalMove(new Vector3(0f, 425f, 0), 1.5f).OnComplete(() =>
                    {
                        comObj.obj.transform.DOLocalMove(new Vector3(0f, 818f, 0), 1.5f).OnComplete(() =>
                        {
                            SetAnimBag();
                            var BagAnimObj = Bag.transform.Find("Anim").gameObject;
                            SpineManager.instance.DoAnimation(BagAnimObj, "Bg1", false, () =>
                            {
                                comObj.obj.SetActive(false);

                                //骨骼计数更新
                                CurBoneCount++;
                                SpineManager.instance.DoAnimation(NumAnimObj, CurBoneCount + "_5", false);

                                //重置骨头的位置
                                comObj.obj.transform.localPosition = oldPosition;
                                //设置会默认的Bag
                                SetNoralBag();

                                if (CurBoneCount >= 5)
                                {
                                    //播放工具的音效
                                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, true);

                                    SetAnimBag();
                                    Debug.Log("最后的骨头展示效果");
                                    //显示背包高亮，显示背包动画
                                    SpineManager.instance.DoAnimation(BagAnimObj, "Bg2");
                                }

                                CoverMask.SetActive(false);
                            });
                        });
                    });
                });
            }
        }


        public void PlayAnim(int index)
        {

            var comObj = commonObjs[index];
            if (comObj.curCount < 1)
                comObj.curCount++;

            CourseTDG4P1L08Part1.CovoerMask.SetActive(true);
            comObj.obj.GetComponent<BoxCollider2D>().enabled = false;

            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(false);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(true);

            string AnimName = comObj.curCount + "_" + comObj.name;

            CoverMask.SetActive(true);
            SpineManager.instance.DoAnimation(AnimObj, AnimName, false, () =>
            {
                CourseTDG4P1L08Part1.isCanMove = true;
                CourseTDG4P1L08Part1.isCanTrigger = false;
                comObj.obj.GetComponent<Image>().enabled = true;
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                CoverMask.SetActive(false);
            });
        }

        void SetNoralBag()
        {
            var normal = Bag.transform.Find("Normal").gameObject;
            var light = Bag.transform.Find("Light").gameObject;
            var anim = Bag.transform.Find("Anim").gameObject;

            normal.SetActive(true);
            light.SetActive(false);
            anim.SetActive(false);
        }

        void SetAnimBag()
        {
            var normal = Bag.transform.Find("Normal").gameObject;
            var light = Bag.transform.Find("Light").gameObject;
            var anim = Bag.transform.Find("Anim").gameObject;

            normal.SetActive(false);
            light.SetActive(false);
            anim.SetActive(true);
        }



        void PlayLightAnim(GameObject animObj, int index)
        {
            animObj.SetActive(true);
            var skt = animObj.GetComponent<SkeletonGraphic>();

            if (index != 1)
            {
                // SpineManager.instance.DoAnimation(animObj, "animation" + index);
                var track = skt.AnimationState.SetAnimation(0,"animation" + index,true);
                track.TrackTime = 5f/30f;
            }
            else
            {
                SpineManager.instance.DoAnimation(animObj, "animation");
                var track = skt.AnimationState.SetAnimation(0,"animation",true);
                track.TrackTime = 5f/30f;
            }
        }

        void CloseLightAnim(int index)
        {
            var comObj = commonObjs[index];
            GameObject NormalObj = comObj.obj.transform.Find("Normal").gameObject;
            NormalObj.SetActive(true);

            GameObject AnimObj = comObj.obj.transform.Find("Anim").gameObject;
            AnimObj.SetActive(false);
        }
    }
}