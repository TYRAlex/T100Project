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
    class SalmonPart1_Scene2
    {
        enum BEAR2_SPINE
        {
            Attack = 0, Hurt = 1, Idle = 2
        }
        string[] bear2Str;

        enum SAW2_SPINE
        {
            Idle = 0, Click = 1, Move = 2
        }
        string[] saw2Str;

        enum TREE2_SPINE
        {
            Fall0 = 0, Fall1 = 1, Fall2 = 2, Idle = 3
        }
        string[] tree2Str;

        enum SALMON2_SPINE
        {
            Attack0 = 0, Attack1 = 1, Attack2 = 2, Idle0 = 3, Idle1 = 4, Swim = 5
        }
        string[] salmon2Str;

        GameObject[] points2;
        GameObject salmon2, saw2, tree2Spine, bear2Spine, bubbleSpine2, saw2Spine, npc;
        Vector3 saw2Pos;        
        bool isEnter, isEnterFirst, isEnterLeft;
        public bool isEnd;
        int iEnterTime;
        MonoBehaviour mono;

        public SalmonPart1_Scene2(GameObject curGo)
        {
            Transform curTrans = curGo.transform;
            //level 1
            salmon2 = curTrans.Find("gameScene/Level_2/salmon").gameObject;
            saw2 = curTrans.Find("gameScene/Level_2/saw").gameObject;
            saw2Spine = curTrans.Find("gameScene/Level_2/sawSpine").gameObject;
            tree2Spine = curTrans.Find("gameScene/Level_2/tree").gameObject;
            bear2Spine = curTrans.Find("gameScene/Level_2/bearSpine").gameObject;
            bubbleSpine2 = curTrans.Find("gameScene/Level_2/background_front/bubble").gameObject;
            npc = curTrans.Find("npc").gameObject;
            points2 = GetChildren(curTrans.Find("gameScene/Level_2/points").gameObject);
            salmon2Str = new string[] { "attack", "attack2", "attack3", "idle", "idle_2", "swim" };
            saw2Str = new string[] { "1", "2", "sawwood" };
            tree2Str = new string[] { "fallen_1", "fallen_2", "fallen_3", "idle" };
            bear2Str = new string[] { "attack", "hurt1", "idle" };
            mono = curTrans.GetComponent<MonoBehaviour>();            
            
            Init();          
        }

        void Init()
        {           
            SoundManager.instance.voiceBtn.SetActive(false);            
            salmon2.transform.localPosition = new Vector3(-1076f, -132.7f, 0);
            saw2.transform.localPosition = new Vector3(-267.4f, 21.8f, 0);
            saw2.SetActive(true);
            isEnter = false;
            isEnterFirst = false;
            isEnterLeft = false;
            isEnd = false;
            iEnterTime = 0;
            saw2Pos = saw2.transform.position;
            SpineManager.instance.PlayAnimationState(tree2Spine.GetComponent<SkeletonGraphic>(), tree2Str[(int)TREE2_SPINE.Idle]);
            SpineManager.instance.PlayAnimationState(saw2Spine.GetComponent<SkeletonGraphic>(), saw2Str[(int)SAW2_SPINE.Idle]);

            SceneInit();
        }

        void SceneInit()
        {
            SpineManager.instance.DoAnimation(bubbleSpine2, "animation", true);
            SpineManager.instance.DoAnimation(bear2Spine, bear2Str[(int)BEAR2_SPINE.Idle], true);
            SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Swim], true);
            salmon2.transform.DOMove(points2[0].transform.position, 3f).OnComplete(() =>
            {
                AttackSalmon();
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14, true);
                SpineManager.instance.DoAnimation(bear2Spine, bear2Str[(int)BEAR2_SPINE.Attack], true);
            });
        }

        void AttackSalmon()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16, false);
            SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Attack1], false, () =>
            {
                SpineManager.instance.SetTimeScale(salmon2.transform.GetChild(0).gameObject, 5f);
                SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Attack2], true);
                salmon2.transform.DOMove(points2[1].transform.position, 0.5f).OnComplete(() =>
                {
                    //SpineManager.instance.SetTimeScale(salmon2.transform.GetChild(0).gameObject, 2f);
                    SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Idle0], true);
                    SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 3, null, () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, true);
                        SpineManager.instance.SetTimeScale(salmon2.transform.GetChild(0).gameObject, 1f);
                        SpineManager.instance.DoAnimation(bear2Spine, bear2Str[(int)BEAR2_SPINE.Idle], true);
                        SpineManager.instance.DoAnimation(tree2Spine, tree2Str[(int)TREE2_SPINE.Idle], true);
                        SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Idle1], true);
                        ILDrager drager = saw2.GetComponent<ILDrager>();
                        drager.SetDragCallback(Level2DragStart, Level2Drag, Level2DragEnd);
                    });
                });
            });
        }

        void Level2DragStart(Vector3 pos, int type, int index)
        {
            Debug.Log(111);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
            SpineManager.instance.DoAnimation(saw2.transform.GetChild(0).gameObject, saw2Str[(int)SAW2_SPINE.Click], true);
        }

        void Level2Drag(Vector3 pos, int type, int index)
        {
            //saw2[1].transform.position = saw2[0].transform.position;
            //SpineManager.instance.DoAnimation(saw2.transform.GetChild(0).gameObject, saw2Str[(int)SAW2_SPINE.Click], true);
            if (pos.x > points2[3].transform.position.x && pos.x < points2[4].transform.position.x &&
                pos.y > points2[3].transform.position.y && pos.y < points2[4].transform.position.y)
            {
                if (isEnter & !isEnterFirst)
                {
                    if (pos.x > points2[5].transform.position.x)
                    {
                        isEnterLeft = true;
                    }
                    else if (pos.x < points2[5].transform.position.x && isEnterLeft)
                    {
                        isEnterLeft = false;
                        isEnterFirst = true;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 19, false);
                        saw2.SetActive(false);
                        saw2Spine.SetActive(true);
                        SpineManager.instance.DoAnimation(saw2Spine, saw2Str[(int)SAW2_SPINE.Move], false);
                        SpineManager.instance.DoAnimation(tree2Spine, tree2Str[iEnterTime], false, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 12, false);
                            saw2.SetActive(true);
                            saw2Spine.SetActive(false);
                            iEnterTime++;
                            if (iEnterTime == 3)
                            {
                                saw2.SetActive(false);
                                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 10, null, () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                                });
                                SpineManager.instance.DoAnimation(bear2Spine, bear2Str[(int)BEAR2_SPINE.Hurt], false);
                                Util.AddBtnClick(salmon2, ClickSalmon);
                            }
                            isEnter = false;
                        });
                    }
                }
            }
            else if (pos.x > points2[4].transform.position.x)
            {
                isEnter = true;
                isEnterFirst = false;
            }
            //else if (pos.x < points2[3].transform.position.x)
            //{
            //    isEnter = true;
            //    isEnterFirst = false;
            //}
            else
            {
                isEnter = false;
                isEnterFirst = false;
            }
        }

        void Level2DragEnd(Vector3 pos, int type, int index, bool isEnd)
        {
            if (!isEnd)
            {
                //if (isEnterFirst)
                //{
                //    saw2.transform.position = points2[5].transform.position;
                //}
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                SpineManager.instance.DoAnimation(saw2.transform.GetChild(0).gameObject, saw2Str[(int)SAW2_SPINE.Idle], true);
                saw2.transform.position = saw2Pos;
                //saw2[1].transform.position = saw2[0].transform.position;
            }
        }

        void ClickSalmon(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13, false);
            SpineManager.instance.DoAnimation(salmon2.transform.GetChild(0).gameObject, salmon2Str[(int)SALMON2_SPINE.Swim], true);
            salmon2.transform.DOMove(points2[2].transform.position, 3f).OnComplete( () =>
            {
                isEnd = true;
            });
        }

        IEnumerator WaitTime(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act();
            mono.StopCoroutine(WaitTime(time, act));
        }

        GameObject[] GetChildren(GameObject father)
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
