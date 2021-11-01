using DG.Tweening;
using ILFramework;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CourseLeapOfSalmanPart1
{
    class SalmonPart1_Scene3
    {
        enum SALMON3_SPINE
        {
            Idle0 = 0, Idle1 = 1, Jump0 = 2, Jump1 = 3, Swim = 4
        }
        string[] salmon3Str, trampoline3Str;
        

        GameObject water3Spine, salmon3Spine, trampoline3Spine, bubbleSpine3, npc;
        GameObject[] combineBtn, combineUI, points3;
        int iCount, iTime;
        public bool isEnd;
        MonoBehaviour mono;

        public SalmonPart1_Scene3(GameObject curGo)
        {
            Transform curTrans = curGo.transform;
            //level 0

            water3Spine = curTrans.Find("gameScene/Level_3/background/water").gameObject;
            salmon3Spine = curTrans.Find("gameScene/Level_3/salmonSpine").gameObject;
            trampoline3Spine = curTrans.Find("gameScene/Level_3/trampoline").gameObject;
            bubbleSpine3 = curTrans.Find("gameScene/Level_3/background/bubble").gameObject;
            npc = curTrans.Find("npc").gameObject;
            combineBtn = GetChildren(curTrans.Find("gameScene/Level_3/combineBtn").gameObject);
            combineUI = GetChildren(curTrans.Find("gameScene/Level_3/combineUI").gameObject);
            points3 = GetChildren(curTrans.Find("gameScene/Level_3/points").gameObject);
            salmon3Str = new string[] { "idle", "idle_2", "jump", "jump_2", "swim"};
            trampoline3Str = new string[] { "1", "2", "3"};
            mono = curGo.GetComponent<MonoBehaviour>();

            Init();            
        }

        void Init()
        {
            SoundManager.instance.voiceBtn.SetActive(false);
            salmon3Spine.transform.localPosition = new Vector3(0, -540f, 0);
            trampoline3Spine.transform.localPosition = new Vector3(201f, -656f, 0);
            combineUI[0].transform.localPosition = new Vector3(-193f, 8f, 0);
            combineUI[1].transform.localPosition = new Vector3(14f, 3f, 0);
            combineUI[2].transform.localPosition = new Vector3(246f, -1f, 0);
            combineUI[3].transform.localPosition = new Vector3(32f, -14f, 0);
            isEnd = false;
            iCount = 0;
            iTime = 0;
            for (int i = 0; i < combineBtn.Length; i++)
            {
                combineBtn[i].SetActive(true);
            }
            for (int i = 0; i < combineUI.Length; i++)
            {
                combineUI[i].SetActive(false);
            }
            trampoline3Spine.SetActive(true);
            SpineManager.instance.PlayAnimationState(trampoline3Spine.GetComponent<SkeletonGraphic>(), trampoline3Str[1]);

            SceneInit();
        }

        void SceneInit()
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, true);            
            SpineManager.instance.DoAnimation(water3Spine, "animation", true);
            SpineManager.instance.DoAnimation(bubbleSpine3, "animation", true);
            mono.StartCoroutine(WaitTime(5f, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16, false);
            }));
            SpineManager.instance.DoAnimation(salmon3Spine, salmon3Str[(int)SALMON3_SPINE.Jump0], false, () =>
            {
                SoundManager.instance.soundSource.volume = 0.2f;
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 4, null, () =>
                {
                    SoundManager.instance.soundSource.volume = 0.4f;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, true);
                });
                SpineManager.instance.DoAnimation(salmon3Spine, salmon3Str[(int)SALMON3_SPINE.Idle1], true);

                for (int i = 0; i < combineBtn.Length; i++)
                {
                    Util.AddBtnClick(combineBtn[i], ClickButton);
                }
            });
        }

        void ClickButton(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13, false);
            string[] arr = btn.name.Split('_');
            int index = Convert.ToInt32(arr[1]);

            btn.SetActive(false);

            Debug.Log("iCount:" + iCount);

            combineUI[index].SetActive(true);
            combineUI[index].transform.position = points3[iCount].transform.position;

            iCount++;

            if (iCount == 3)
            {
                combineUI[iCount].SetActive(true);
                Util.AddBtnClick(combineUI[iCount], ClickCombine);
            }

        }

        void ClickCombine(GameObject btn)
        {
            btn.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13, false);
            for (int i = 0; i < combineUI.Length; i++)
            {
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 17, null, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 23, false);
                });
                combineUI[i].transform.DOMove(points3[3].transform.position, 0.9f).OnComplete( () =>
                {
                    iTime++;
                    if (iTime == 1)
                    {                        
                        TrampolineAnimation();
                    }                    
                });
            }
        }

        void TrampolineAnimation()
        {
            for ( int i = 0; i < combineUI.Length; i++)
            {
                combineUI[i].SetActive(false);
            }
            
            SpineManager.instance.DoAnimation(trampoline3Spine, trampoline3Str[0], false, () =>
            {
                SpineManager.instance.DoAnimation(trampoline3Spine, trampoline3Str[1], false, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 15, false);
                    SpineManager.instance.DoAnimation(trampoline3Spine, trampoline3Str[2], false, () =>
                    {
                        trampoline3Spine.SetActive(false);
                        mono.StartCoroutine(WaitTime(2.2f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 17, false);
                        }));
                        SpineManager.instance.DoAnimation(salmon3Spine, salmon3Str[(int)SALMON3_SPINE.Jump1], false, () =>
                        {
                            isEnd = true;
                        });
                    });
                });
            });
        }

        IEnumerator WaitTime(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act();
            mono.StopCoroutine(WaitTime(time, act));
        }

        public GameObject[] GetChildren(GameObject father)
        {
            GameObject[] children = new GameObject[father.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = father.transform.GetChild(i).gameObject;
            }
            return children;
        }
    }
}
