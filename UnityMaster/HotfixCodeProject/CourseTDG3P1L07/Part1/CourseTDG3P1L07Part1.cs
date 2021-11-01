using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseTDG3P1L07Part1
    {
        GameObject curGo;
        MonoBehaviour Mono;
        Transform ContentTrans;
        Transform NextTiShi;
        Transform PreTiShi;
        string[] ItemAnimArr;

        Dictionary<int, string[]> ItemAnimArrDic;
        int CompelatePage;
        int CurPage;
        bool FristEnter;

        float PreContentPos;
        bool IsScroll;
        bool IsCheck;
        float timeCount = 3;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Mono = curGo.GetComponent<MonoBehaviour>();

            ItemAnimArr = new string[] {"gear_", "propeller_", "spring_", "telescope_", "lamp_", "clock_"};

            ContentTrans = curGo.transform.Find("bg/ScrollRect_Parent/Viewport/Content");
            NextTiShi = curGo.transform.Find("bg/NextBtn");
            PreTiShi = curGo.transform.Find("bg/PretBtn");
            NextTiShi.gameObject.SetActive(false);
            PreTiShi.gameObject.SetActive(false);
            if (curGo.transform.Find("bg/ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("bg/ScrollRect_Parent").GetComponent<ScrollRectMoveManager>());
            }
            curGo.transform.Find("bg/ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curGo.transform.transform.Find("bg/ScrollRect_Parent"), OnDrag);
            ContentTrans.transform.localPosition = new Vector3(2880, 0, 0);
            SetScrollActive(false);
            SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/SmallItem1").gameObject, "anniu_idle", true);
            SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ClearBtn").gameObject, "3_idle", true);
            SoundManager.instance.Speaking(curTrans.Find("bg/npc").gameObject, "talk", SoundManager.SoundType.VOICE, 4, null, () =>
            {
                IsCheck = true;
            });

            for (int i = 0; i < 4; i++)
            {
                
                for (int j = 0; j < 6; j++)
                {
                    ContentTrans.Find("Page" + i).Find("Item" + j).GetComponent<SkeletonGraphic>().AnimationState.Data.DefaultMix = 0;
                    ContentTrans.Find("Page" + i).Find("Item" + j).gameObject.SetActive(false);
                }
                ContentTrans.Find("Page" + i).Find("Compelate").gameObject.SetActive(false);
                ContentTrans.Find("Page" + i).Find("MainItem").gameObject.SetActive(true);
            }
            FristEnter = true;
            CompelatePage = 0;
            timeCount = 3;
            IsScroll = false;
            IsCheck = false;
            SwitchPage(0);

            for(int i = 0; i < 6; i++)
            {
                int j = i;
                
                curGo.transform.Find("bg/DiBan/ChildIcon/" + i).Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                curGo.transform.Find("bg/DiBan/ChildIcon/" + i).Find("Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    ContentTrans.Find("Page" + CurPage).Find("Item" + j).gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(ContentTrans.Find("Page" + CurPage).Find("Item" + j).gameObject, ItemAnimArr[j] + (ScrollRectMoveManager.instance.index + 1), false);
                    SetbtnActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                    SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ChildIcon/" + j).gameObject, (6 - j).ToString(), false, ()=>
                    {
                        SetbtnActive(true);
                        if (CheckIsOver())
                        {
                            for (int m = 0; m < 6; m++)
                            {
                                ContentTrans.Find("Page" + CurPage).Find("Item" + m).gameObject.SetActive(false);
                            }
                            ContentTrans.Find("Page" + CurPage).Find("MainItem").gameObject.SetActive(false);
                            ContentTrans.Find("Page" + CurPage).Find("Compelate").gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(ContentTrans.Find("Page" + CurPage).Find("Compelate").gameObject, "wancheng", true);
                            curGo.transform.Find("bg/DiBan").gameObject.SetActive(false);
                            SetScrollActive(true);
                            if (CompelatePage < 3)
                            {
                                timeCount = 3;
                                NextTiShi.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(NextTiShi.gameObject, "4", true);
                            }
                            if (CompelatePage > 0)
                            {
                                timeCount = 3;
                                PreTiShi.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(PreTiShi.gameObject, "4", true);
                            }
                            CompelatePage += 1;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                            Mono.StartCoroutine(Wait(1, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                            }));
                        }
                    });
                });
            }

            curGo.transform.Find("bg/DiBan/ClearBtn/Button").GetComponent<Button>().onClick.RemoveAllListeners();
            curGo.transform.Find("bg/DiBan/ClearBtn/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                Transform trans = ContentTrans.Find("Page" + ScrollRectMoveManager.instance.index);
                SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ClearBtn").gameObject, "anniu_1", false, () =>
                {
                    SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ClearBtn").gameObject, "3_idle", true);
                });
                for(int i = 0; i < 6; i++)
                {
                    trans.Find("Item" + i).gameObject.SetActive(false);
                }
            });

            /*
            for(int i = 0; i < 4; i++)
            {
                int j = i;
                curGo.transform.Find("bg/DiBan/Kuang" + i).Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
                curGo.transform.Find("bg/DiBan/Kuang" + i).Find("Button").GetComponent<Button>().onClick.AddListener(() =>
                {
                    Debug.Log("kki: " + j);
                    SwitchPage(j);
                });
            }
            */

            SoundManager.instance.BgSoundPart2();
        }

        void FixedUpdate()
        {
            if (PreTiShi.gameObject.activeSelf || NextTiShi.gameObject.activeSelf)
            {
                if (timeCount < -5)
                {
                    timeCount = 3;
                }

                if (timeCount >= 0)
                {
                    NextTiShi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    PreTiShi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    NextTiShi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                    PreTiShi.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                }
                timeCount -= Time.deltaTime;
            }
            if (Math.Abs(PreContentPos - ContentTrans.transform.localPosition.x) > 2.0f)
            {
                Debug.Log("IsScroll: " + IsScroll);
                if (IsScroll == false && IsCheck)
                {
                    Debug.Log("Enter Panel");
                    PreTiShi.gameObject.SetActive(false);
                    NextTiShi.gameObject.SetActive(false);
                    IsScroll = true;
                    SetbtnActive(false);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                }
            }
            PreContentPos = ContentTrans.transform.localPosition.x;
        }

        void OnDrag()
        {
            Debug.Log("OnDrag");
            SetbtnActive(true);
            if (ScrollRectMoveManager.instance.index < CompelatePage)
            {
                SetScrollActive(true);
                if (IsCheck)
                {
                    timeCount = 3;
                    if (ScrollRectMoveManager.instance.index == 0)
                    {
                        NextTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTiShi.gameObject, "4", true);
                    }
                    else if (ScrollRectMoveManager.instance.index == 1)
                    {
                        NextTiShi.gameObject.SetActive(true);
                        PreTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTiShi.gameObject, "4", true);
                        SpineManager.instance.DoAnimation(PreTiShi.gameObject, "4", true);
                    }
                    else if (ScrollRectMoveManager.instance.index == 2)
                    {
                        NextTiShi.gameObject.SetActive(true);
                        PreTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTiShi.gameObject, "4", true);
                        SpineManager.instance.DoAnimation(PreTiShi.gameObject, "4", true);
                    }
                    else
                    {
                        PreTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(PreTiShi.gameObject, "4", true);
                    }
                }
            }
            else
            {
                curGo.transform.Find("bg/DiBan").gameObject.SetActive(true);
                SetScrollActive(false);
                SwitchPage(ScrollRectMoveManager.instance.index);
            }
            IsScroll = false;
        }

        bool CheckIsOver()
        {
            bool isover = true;
            for(int i = 0; i < 6; i++)
            {
                if(ContentTrans.Find("Page" + CurPage).Find("Item" + i).gameObject.activeSelf == false)
                {
                    isover = false;
                    break;
                }
            }
            return isover;
        }

        void SwitchPage(int Page)
        {
            CurPage = Page;
            for(int i = 0; i < 4; i++)
            {
                if(i != CurPage)
                {
                    curGo.transform.Find("bg/DiBan/Kuang" + i).GetComponent<SkeletonGraphic>().enabled = false;
                }
                else
                {
                    curGo.transform.Find("bg/DiBan/Kuang" + i).GetComponent<SkeletonGraphic>().enabled = true;
                }
            }

            if(FristEnter)
            {
                for (int i = 0; i < 6; i++)
                {
                    curGo.transform.Find("bg/DiBan/ChildIcon/" + i).gameObject.SetActive(false);
                }
                Mono.StartCoroutine(Wait(0.8f, () =>
                {
                    for (int i = 0; i < 6; i++)
                    {
                        curGo.transform.Find("bg/DiBan/ChildIcon/" + i).gameObject.SetActive(true);
                    }
                }));
                FristEnter = false;
            }
        }

        void SetbtnActive(bool active)
        {
            for (int i = 0; i < 6; i++)
            {
                curGo.transform.Find("bg/DiBan/ChildIcon/" + i).Find("Button").GetComponent<Image>().raycastTarget = active;
            }

            for (int i = 0; i < 4; i++)
            {
                curGo.transform.Find("bg/DiBan/Kuang" + i).Find("Button").GetComponent<Image>().raycastTarget = active;
            }
        }

        void SetScrollActive(bool active)
        {
            for(int i = 0; i < 4; i++)
            {
                ContentTrans.Find("Page" + i).GetComponent<Image>().raycastTarget = active;

                for (int j = 0; j < 6; j++)
                {
                    ContentTrans.Find("Page" + i).Find("Item" + j).GetComponent<SkeletonGraphic>().raycastTarget = active;
                }
                ContentTrans.Find("Page" + i).Find("MainItem").GetComponent<SkeletonGraphic>().raycastTarget = active;
                ContentTrans.Find("Page" + i).Find("Compelate").GetComponent<SkeletonGraphic>().raycastTarget = active;
            }
            ContentTrans.parent.GetComponent<Image>().raycastTarget = active;
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            if(act != null)
            {
                act.Invoke();
            }
        }
    }
}
