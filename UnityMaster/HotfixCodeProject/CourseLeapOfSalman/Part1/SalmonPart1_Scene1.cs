using DG.Tweening;
using ILFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CourseLeapOfSalmanPart1
{
    class SalmonPart1_Scene1
    {
        //level 1
        GameObject[] points1;
        GameObject salmon1Spine0, salmon1Spine1, shovel1, sand1Spine, bubbleSpine1,  npc;
        Vector3 shovel1Pos;
        string[] salmon1Str, sand1Str;
        bool isEnter, isEnterFirst;
        public bool isEnd;
        int iEnterTime;
        MonoBehaviour mono;

        public SalmonPart1_Scene1(GameObject curGo)
        {
            Transform curTrans = curGo.transform;
            //level 1
            salmon1Spine0 = curTrans.Find("gameScene/Level_1/salmonSpine").gameObject;
            salmon1Spine1 = curTrans.Find("gameScene/Level_1/salmon1").gameObject;
            bubbleSpine1 = curTrans.Find("gameScene/Level_1/background_front/bubble").gameObject;
            sand1Spine = curTrans.Find("gameScene/Level_1/sandSpine").gameObject;
            points1 = GetChildren(curTrans.Find("gameScene/Level_1/points").gameObject);
            salmon1Str = new string[] { "1st", "2nd", "idle", "idle_2", "swim" };
            sand1Str = new string[] { "1", "2", "3", "idle"};
            shovel1 = curTrans.Find("gameScene/Level_1/shovel").gameObject;
            npc = curTrans.Find("npc").gameObject;            
            
            mono = curGo.GetComponent<MonoBehaviour>();
            
            Init();
        }

        void Init()
        {
            salmon1Spine1.SetActive(false);
            SoundManager.instance.voiceBtn.SetActive(false);
            salmon1Spine0.transform.localPosition = new Vector3(-71f, -516f, 0);
            salmon1Spine1.transform.localPosition = new Vector3(-413.9f, 89f, 0);
            sand1Spine.transform.localPosition = new Vector3(310f, 9f, 0);
            shovel1.transform.localPosition = new Vector3(8f, -164f, 0);
            shovel1Pos = shovel1.transform.position;
            shovel1.SetActive(true);
            shovel1.transform.GetChild(0).gameObject.SetActive(false);
            sand1Spine.SetActive(true);
            salmon1Spine0.SetActive(true );
            salmon1Spine1.SetActive(false);
            isEnter = false;
            isEnterFirst = false;
            isEnd = false;
            iEnterTime = 0;

            SceneInit();
        }

        void SceneInit()
        {
            //ShowLevel(1);
            SpineManager.instance.DoAnimation(bubbleSpine1, "animation", true);
            //SoundManager.instance.soundSource.volume = 1f;
            mono.StartCoroutine(WaitTime(3.2f, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 16, false);
            }));
            SpineManager.instance.SetTimeScale(salmon1Spine0, 2);
            SpineManager.instance.DoAnimation(salmon1Spine0, salmon1Str[0], false, () =>
            {
                SpineManager.instance.DoAnimation(sand1Spine, sand1Str[3], true);
                salmon1Spine0.SetActive(false);
                salmon1Spine1.SetActive(true);
                SpineManager.instance.DoAnimation(salmon1Spine1.transform.GetChild(0).gameObject, salmon1Str[3], true);
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 2);

                ILDrager drager = shovel1.GetComponent<ILDrager>();
                drager.SetDragCallback(Level1DragStart, Level1Drag, Level1DragEnd);
            });

        }

        void Level1DragStart(Vector3 pos, int type, int index)
        {
            Debug.Log(111);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
            shovel1.transform.GetChild(0).gameObject.SetActive(true);
        }

        void Level1Drag(Vector3 pos, int type, int index)
        {
            if (pos.x > points1[2].transform.position.x && pos.x < points1[3].transform.position.x && 
                pos.y > points1[2].transform.position.y && pos.y < points1[3].transform.position.y)
            {
                if (isEnter && !isEnterFirst)
                {
                    mono.StartCoroutine(WaitTime(0.5f, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 18, false);
                    }));
                    isEnterFirst = true;
                    //shovel1.SetActive(false);
                    SpineManager.instance.DoAnimation(sand1Spine, sand1Str[iEnterTime], false, () =>
                    {
                        iEnterTime++;
                        //shovel1.SetActive(true);
                        if (iEnterTime == 3)
                        {
                            shovel1.SetActive(false);
                            sand1Spine.SetActive(false);
                            Util.AddBtnClick(salmon1Spine1, ClickSalmon);
                        }                        
                        isEnter = false;
                        
                    });
                }                
            }
            else if(pos.x > points1[3].transform.position.x)
            {
                isEnter = true;
                isEnterFirst = false;
            }
            else
            {
                isEnter = false;
                isEnterFirst = false;
            }
        }

        void Level1DragEnd(Vector3 pos, int type, int index, bool isEnd)
        {
            shovel1.transform.GetChild(0).gameObject.SetActive(false);
            if (!isEnd)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                shovel1.transform.position = shovel1Pos;
            }
        }

        void ClickSalmon(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13, false);
            SpineManager.instance.DoAnimation(salmon1Spine1.transform.GetChild(0).gameObject, salmon1Str[4], true);
            salmon1Spine1.transform.DOMove(points1[1].transform.position, 4f).OnComplete( () =>
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
