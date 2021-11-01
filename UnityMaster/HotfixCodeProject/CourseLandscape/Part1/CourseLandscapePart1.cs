using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseLandscapePart1
    {
        GameObject curGo;
        int nextIndex;
        GameObject anim_Landsscape1;
        GameObject anim_Landsscape2;
        List<ILObject3DAction> btns;
        List<ILObject3DAction> fingers;
        GameObject mask;
        GameObject dingding;
        string name;
        string name1;
        int curIndex;
        int maxIndex;
        Transform btnParent;
        Transform fingerParent;
        void Start(object o)
        {
            curGo = (GameObject)o;
            name = "animation_";
            name1 = "error_";
            Transform curTrans = curGo.transform;
            curIndex = 0;
            maxIndex = 9;
            nextIndex = 4;
            Transform ui3d = curTrans.Find("UI3D");
            anim_Landsscape1 = ui3d.Find("anim_Landscape1").gameObject;
            anim_Landsscape2 = ui3d.Find("anim_Landscape2").gameObject;
            mask = ui3d.Find("mask").gameObject;
            dingding = curTrans.Find("dingding").gameObject;
            btnParent = ui3d.Find("btn");
            fingerParent = ui3d.Find("fingers");

            btns = new List<ILObject3DAction>();
            for (int i = 0; i < btnParent.childCount; i++)
            {
                btns.Add(btnParent.GetChild(i).GetComponent<ILObject3DAction>());
                btns[i].index = i + 1;
                btns[i].OnMouseDownLua = BtnOnClick;
            }
            btnParent.gameObject.SetActive(false);

            //������ָ
            fingers = new List<ILObject3DAction>();
            for (int i = 0; i < fingerParent.childCount; i++)
            {
                fingers.Add(fingerParent.GetChild(i).GetComponent<ILObject3DAction>());
                fingers[i].gameObject.SetActive(false);
            }

            fingerParent.gameObject.SetActive(false);

            Speaking(0, () =>
            {
                btnParent.gameObject.SetActive(true);
                fingerParent.gameObject.SetActive(true);
            });
            InitBtn();


            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
        }
        public void InitBtn()
        {
            ClearBtn();
        }
        public void Speaking(int index, Action act = null)
        {
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index, () =>
            {

                mask.SetActive(true);

                //��ʼ˵��ʱ��������ָ����
                CloseAllFingers();


            }, () =>
            {
                Debug.Log("11111");
                curIndex++;
                if (index == maxIndex - 1)
                {
                    btnParent.gameObject.SetActive(false);
                    ClearBtn();
                    Speaking(maxIndex);
                }
                else if (index < maxIndex - 1)
                {
                    mask.SetActive(false);
                    act?.Invoke();

                    PlaySingleFingerAnimation();
                }
            });
        }
        public void BtnOnClick(int i)
        {
            if (curIndex < maxIndex)
            {
                ClearBtn();
                mask.SetActive(true);
                SpineManager.instance.DoAnimation(btns[i - 1].gameObject, "animation_" + i, false);
                if (i == curIndex)
                {
                    PlayAnimation(i);
                }
                else
                {
                    PlayError();
                }
            }
        }
        public void ClearBtn()
        {
            for (int i = 0; i < btns.Count; i++)
            {
                SpineManager.instance.PlayAnimationState(btns[i].GetComponent<SkeletonAnimation>(), "animation_" + (i + 1));

            }



        }
        public void PlayError()
        {
            if (curIndex <= nextIndex)
            {
                anim_Landsscape2.transform.localScale = Vector3.zero;
                anim_Landsscape1.transform.localScale = Vector3.one;
                if (curIndex == 1)
                {
                    SpineManager.instance.DoAnimation(anim_Landsscape1, name1 + 0, false, () => mask.SetActive(false));
                }
                else
                {
                    SpineManager.instance.DoAnimation(anim_Landsscape1, name1 + (curIndex - 1), false, () => mask.SetActive(false));
                }
            }
            else
            {
                anim_Landsscape2.transform.localScale = Vector3.one;
                anim_Landsscape1.transform.localScale = Vector3.zero;
                SpineManager.instance.DoAnimation(anim_Landsscape2, name1 + (curIndex - 1), false, () => mask.SetActive(false));
            }
        }
        public void PlayAnimation(int index)
        {
            if (index < nextIndex)
            {
                anim_Landsscape2.transform.localScale = Vector3.zero;
                anim_Landsscape1.transform.localScale = Vector3.one;
                Speaking(curIndex);
                SpineManager.instance.DoAnimation(anim_Landsscape1, name + index, false/*,()=>curIndex++*/);
            }
            else
            {
                anim_Landsscape2.transform.localScale = Vector3.one;
                anim_Landsscape1.transform.localScale = Vector3.zero;
                Speaking(curIndex);

                SpineManager.instance.DoAnimation(anim_Landsscape2, name + index, false, () =>
                {
                    if (index == 8)
                    {
                        SpineManager.instance.DoAnimation(anim_Landsscape2, name + index + "_idle");
                    }
                });
            }
        }

        public void CloseAllFingers()
        {
            //��ʼ˵��ʱ��������ָ����
            for (int i = 0; i < fingers.Count; i++)
            {
                fingers[i].gameObject.SetActive(false);
            }
        }

        public void PlaySingleFingerAnimation()
        {
            Debug.Log("CurIndex:" + curIndex);
            for (int i = 0; i < fingers.Count; i++)
            {
                // //��ǰ��ָָ�ĵط�
                // if (i + 1 == curIndex)
                // {
                //     fingers[i].gameObject.SetActive(true);
                //     //SpineManager.instance.PlayAnimationState(fingers[i].GetComponent<SkeletonAnimation>(), "ts");
                //     SpineManager.instance.DoAnimation(fingers[i].gameObject, "ts", true);
                // }
                // else
                // {
                //     fingers[i].gameObject.SetActive(false);
                // }
                fingers[i].gameObject.SetActive(false);
            }




            fingers[curIndex - 1].gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(fingers[curIndex - 1].gameObject, "ts", true);

        }
    }
}
