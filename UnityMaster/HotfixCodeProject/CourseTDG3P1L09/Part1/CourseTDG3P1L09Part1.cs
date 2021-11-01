using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseTDG3P1L09Part1
    {
        GameObject curGo;
        Transform ContentTrans;
        GameObject NextTile;
        GameObject PreTile;
        GameObject OkBtn;
        MonoBehaviour mono;
        Dictionary<int, Vector3> FlowerPos;
        int[] UITypeUICountArr;
        string[] UIPopAnimArr;
        string[] UIAnimArr;
        string[] ItemAnimArr;
        int CurUIPage;
        int GrounpPage;
        int VaseChoose;

        float PreContentPos;
        float timeCount;
        bool IsScroll;
        bool IsCheck;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            UITypeUICountArr = new int[] { 4, 3, 4, 3, 3};
            UIPopAnimArr = new string[] { "ui_vase", "ui_tablecloth", "ui_flower", "ui_fruit", "ui_tableware" };
            UIAnimArr = new string[] {"vase_4", "vase_3", "vase_2", "vase_1", "tablecloth_3", "tablecloth_2", "tablecloth_1", "flower_4", "flower_3",
                                      "flower_2", "flower_1", "fruit_3", "fruit_2", "fruit_1", "tableware_3", "tableware_2", "tableware_1"};
            ItemAnimArr = new string[] { "vase1", "vase2", "vase3", "vase4", "zhuobu1", "zhuobu2", "zhuobu3", "flower_1", "flower_2", "flower_3", "flower_4", "jingwu5",
                                         "jingwu3", "jingwu4", "jingwu2", "jingwu1", "jingwu13", "jingwu10", "jingwu11", "jingwu7", "jingwu9", "jingwu8", "jingwu12", "jingwu6" };
            FlowerPos = new Dictionary<int, Vector3>();
            FlowerPos.Add(6, new Vector3(0, -600, 0));
            FlowerPos.Add(10, new Vector3(0, -560, 0));
            FlowerPos.Add(12, new Vector3(0, -640, 0));
            FlowerPos.Add(13, new Vector3(0, -610, 0));
            FlowerPos.Add(14, new Vector3(0, -660, 0));
            FlowerPos.Add(15, new Vector3(-10, -620, 0));
            mono = curGo.GetComponent<MonoBehaviour>();
            ContentTrans = curTrans.Find("ScrollRect_Parent/Viewport/Content");
            NextTile = curTrans.Find("NextTiShi").gameObject;
            PreTile = curTrans.Find("PreTiShi").gameObject;
            curTrans.Find("WinEffect").gameObject.SetActive(false);
            OkBtn = curTrans.Find("OkBtn").gameObject;
            OkBtn.SetActive(false);
            PreTile.SetActive(false);
            NextTile.SetActive(false);
            for (int i = ContentTrans.childCount - 1; i >= 0; i--)
            {
                if(ContentTrans.GetChild(i).name != "Page0")
                {
                    GameObject.DestroyImmediate(ContentTrans.GetChild(i).gameObject);
                }
            }
            if (curGo.transform.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>());
            }
            
            curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curGo.transform.transform.Find("ScrollRect_Parent"), OnDrag);
            ContentTrans.transform.localPosition = new Vector3(0, 0, 0);
            SetScrollActive(false);

            SwitchUIPage(0);
            GrounpPage = 0;
            timeCount = 3;
            IsScroll = false;
            IsCheck = false;

            for (int i = 0; i < 17; i++)
            {
                int m = i;
                curGo.transform.Find("DiBan/Item" + i).GetComponent<Image>().enabled = true;
                curGo.transform.Find("DiBan/Item" + i).gameObject.GetComponent<Button>().onClick.RemoveAllListeners();
                curGo.transform.Find("DiBan/Item" + i).gameObject.GetComponent<Button>().onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    if(m < 14)
                    {
                        int startindex = CountUIPageStartIndex();
                        for (int j = startindex; j < startindex + UITypeUICountArr[CurUIPage]; j++)
                        {
                            curGo.transform.Find("DiBan/Item" + j).Find("ItemSp").gameObject.SetActive(false);
                            curGo.transform.Find("DiBan/Item" + j).GetComponent<Image>().enabled = true;
                            ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + j).gameObject.SetActive(false);
                            Transform ts1 = ContentTrans.Find("Page" + GrounpPage).Find(string.Format("ItemSP{0}_1", j));
                            if(ts1 != null)
                            {
                                ts1.gameObject.SetActive(false);
                            }

                            
                            if(j != m)
                            {
                                //Debug.Log("j: " + j + "    GrounpPage: " + GrounpPage);
                                //ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + j).localPosition = new Vector3(10000, 120, 0);
                                //ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + j).GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimation(0, 0);
                            }
                            
                        }

                        curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject.SetActive(true);
                        curGo.transform.Find("DiBan/Item" + m).GetComponent<Image>().enabled = false;
                        SpineManager.instance.DoAnimation(curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject, UIAnimArr[m], false);
                        ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).gameObject.SetActive(true);
                        if(m < 4)
                        {
                            VaseChoose = m;
                        }
                        if(m > 6 && m < 11)
                        {
                            int index = VaseChoose * 4 + m - 7;
                            if(FlowerPos.ContainsKey(index))
                            {
                                ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).localPosition = FlowerPos[index];
                            }
                            else
                            {
                                ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).localPosition = new Vector3(0, -575, 0);
                            }
                        }
                        
                        ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
                        SpineManager.instance.DoAnimation(ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).gameObject, ItemAnimArr[m], false);
                        //ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).GetComponent<SkeletonGraphic>().AnimationState.AddAnimation(0, ItemAnimArr[m], false, 0.5f);
                        Transform ts = ContentTrans.Find("Page" + GrounpPage).Find(string.Format("ItemSP{0}_1", m, false));
                        if (ts != null)
                        {
                            ts.gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(ts.gameObject, ItemAnimArr[m + 10]);
                        }
                    }
                    else
                    {
                        if (curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject.activeSelf)
                        {
                            curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject.SetActive(false);
                            ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).gameObject.SetActive(false);
                            Transform ts1 = ContentTrans.Find("Page" + GrounpPage).Find(string.Format("ItemSP{0}_1", m));
                            if (ts1 != null)
                            {
                                ts1.gameObject.SetActive(false);
                            }
                        }
                        else
                        {
                            curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject.SetActive(true);
                            curGo.transform.Find("DiBan/Item" + m).GetComponent<Image>().enabled = false;
                            SpineManager.instance.DoAnimation(curGo.transform.Find("DiBan/Item" + m).Find("ItemSp").gameObject, UIAnimArr[m], false, ()=>
                            {
                                curGo.transform.Find("DiBan/Item" + m).GetComponent<Image>().enabled = true;
                            });
                            ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + m).gameObject, ItemAnimArr[m], false);
                            Transform ts = ContentTrans.Find("Page" + GrounpPage).Find(string.Format("ItemSP{0}_1", m, false));
                            if (ts != null)
                            {
                                ts.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(ts.gameObject, ItemAnimArr[m + 10]);
                            }
                        }
                    }
                    OkBtn.SetActive(true);
                    SpineManager.instance.DoAnimation(OkBtn, "Ok", false);
                });
                ContentTrans.Find("Page" + GrounpPage).Find("ItemSP" + i).gameObject.SetActive(false);
                Transform tan = ContentTrans.Find("Page" + GrounpPage).Find(string.Format("ItemSP{0}_1", i));
                if(tan != null)
                {
                    tan.gameObject.SetActive(false);
                }
            }
            ContentTrans.Find("Page" + GrounpPage).Find("HuaKuang_Mask").gameObject.SetActive(false);

            OkBtn.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            OkBtn.transform.Find("Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                IsCheck = false;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                OkBtn.SetActive(false);
                if (CurUIPage == 4)
                {
                    ContentTrans.Find("Page" + GrounpPage).Find("HuaKuang_Mask").gameObject.SetActive(true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                    SpineManager.instance.DoAnimation(ContentTrans.Find("Page" + GrounpPage).Find("HuaKuang_Mask").gameObject, "Ok_frame", false, ()=>
                    {
                        //curTrans.Find("WinEffect").gameObject.SetActive(true);
                        GameObject go = GameObject.Instantiate(curTrans.Find("WinEffect").gameObject);
                        go.transform.SetParent(curTrans, false);
                        go.transform.localScale = Vector3.one;
                        go.transform.localPosition = curTrans.Find("WinEffect").transform.localPosition;
                        go.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(go, "animation", false, ()=>
                        {
                            GameObject.Destroy(go);

                            if (ScrollRectMoveManager.instance.index < ContentTrans.childCount - 1)
                            {
                                Debug.Log("xx1");
                                timeCount = 3;
                                NextTile.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(NextTile, "4", true);
                            }
                            if (ScrollRectMoveManager.instance.index > 0)
                            {
                                Debug.Log("xx2");
                                timeCount = 3;
                                PreTile.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(PreTile, "4", true);
                            }

                            ContentTrans.parent.parent.GetComponent<ScrollRect>().enabled = true;
                            IsCheck = true;
                            SetScrollActive(true);
                        });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                    });
                    curGo.transform.Find("DiBan").gameObject.SetActive(false);

                    GameObject obj = GameObject.Instantiate(ContentTrans.Find("Page0").gameObject);
                    obj.name = "Page" + (GrounpPage + 1);
                    obj.transform.SetParent(ContentTrans);
                    List<float> p = ContentTrans.parent.parent.GetComponent<ScrollRectMoveManager>().points;
                    p.Add(1);
                    for (int i = 0; i < p.Count; i++)
                    {
                        //Debug.Log("p.Count: " + p.Count);
                        if(p.Count - 1 == 0)
                        {
                            ContentTrans.parent.parent.GetComponent<ScrollRectMoveManager>().points[i] = 0;
                        }
                        else
                        {
                            ContentTrans.parent.parent.GetComponent<ScrollRectMoveManager>().points[i] = i / (p.Count - 1.0f);
                        }
                    }
                    ContentTrans.parent.parent.GetComponent<ScrollRectMoveManager>().grids.Add(obj);
                    float x = (GrounpPage + 1) * 960 - ScrollRectMoveManager.instance.index * 1920;
                    mono.StartCoroutine(Stick(x, null));
                    ContentTrans.parent.parent.GetComponent<ScrollRect>().enabled = false;
                    ContentTrans.transform.localPosition = new Vector3(x, 0, 0);

                    for (int i = 0; i < 17; i++)
                    {
                        curGo.transform.Find("DiBan/Item" + i).GetComponent<Image>().enabled = true;
                        ContentTrans.Find("Page" + (GrounpPage + 1)).Find("ItemSP" + i).gameObject.SetActive(false);
                        Transform tan = ContentTrans.Find("Page" + (GrounpPage + 1)).Find(string.Format("ItemSP{0}_1", i));
                        if (tan != null)
                        {
                            tan.gameObject.SetActive(false);
                        }
                    }
                    ContentTrans.Find("Page" + (GrounpPage + 1)).Find("HuaKuang_Mask").gameObject.SetActive(false);

                   
                }
                else
                {
                    SwitchUIPage(CurUIPage + 1);
                }
            });

            curGo.transform.Find("DiBan").gameObject.SetActive(true);

            SoundManager.instance.Speaking(curTrans.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                IsCheck = true;
            });
            SoundManager.instance.BgSoundPart2();
        }

        void FixedUpdate()
        {
            if (PreTile.activeSelf || NextTile.activeSelf)
            {
                if (timeCount < -5)
                {
                    timeCount = 3;
                }

                if (timeCount >= 0)
                {
                    NextTile.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                    PreTile.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    NextTile.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                    PreTile.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0);
                }
                timeCount -= Time.deltaTime;
            }
            if (Math.Abs(PreContentPos - ContentTrans.transform.localPosition.x) > 2.0f)
            {
                if (IsScroll == false && IsCheck)
                {
                    PreTile.gameObject.SetActive(false);
                    NextTile.gameObject.SetActive(false);
                    IsScroll = true;
                    SetScrollActive(false);
                }
            }
            PreContentPos = ContentTrans.transform.localPosition.x;
        }

        void OnDrag()
        {
            Debug.Log("Enter OnDrag" + "IsCheck: " + IsCheck);
            if (IsCheck)
            {
                SetScrollActive(true);
                if (ScrollRectMoveManager.instance.index < ContentTrans.childCount - 1)
                {
                
                    if (ScrollRectMoveManager.instance.index == 0)
                    {
                        NextTile.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTile, "4", true);
                    }
                    else if(ScrollRectMoveManager.instance.index == ContentTrans.childCount - 1)
                    {
                        timeCount = 3;
                        PreTile.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(PreTile, "4", true);
                    }
                    else
                    {
                        timeCount = 3;
                        NextTile.gameObject.SetActive(true);
                        PreTile.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTile, "4", true);
                        SpineManager.instance.DoAnimation(PreTile, "4", true);
                    }
               
                }
                else
                {
                    curGo.transform.Find("DiBan").gameObject.SetActive(true);
                    SetScrollActive(false);
                    SwitchUIPage(0);
                }
                IsScroll = false;
                GrounpPage = ScrollRectMoveManager.instance.index;
            }
        }

        void SetBtnActive(bool active)
        {
            for(int i = 0; i < 17; i++)
            {
                curGo.transform.Find("DiBan/Item" + i).GetComponent<Image>().raycastTarget = active;
            }
        }

        void SetScrollActive(bool active)
        {
            Debug.Log("SetScrollActive: " + active);
            for(int i = 0; i < ContentTrans.childCount; i++)
            {
                ContentTrans.transform.Find("Page" + i).Find("bg").GetComponent<Image>().raycastTarget = active;
            }
            ContentTrans.parent.GetComponent<Image>().raycastTarget = active;
        }

        IEnumerator Stick(float x, Action act)
        {
            int num = 0;
            while(num < 100)
            {
                ContentTrans.transform.localPosition = new Vector3(x, 0, 0);
                num += 1;
                yield return null;
            }

            Debug.Log("xxxNum");
            if (act != null)
            {
                act.Invoke();
            }
        }

        void SwitchUIPage(int page)
        {
            CurUIPage = page;
            for(int i = 0; i < 17; i++)
            {
                curGo.transform.Find("DiBan/Item" + i).gameObject.SetActive(false);
            }
            curGo.transform.Find("DiBan/UiItemBg").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(curGo.transform.Find("DiBan/UiItemBg").gameObject, UIPopAnimArr[CurUIPage], false, ()=>
            {
                curGo.transform.Find("DiBan/UiItemBg").gameObject.SetActive(false);
                int startindex = CountUIPageStartIndex();
                for (int i = startindex; i < startindex + UITypeUICountArr[CurUIPage]; i++)
                {
                    curGo.transform.Find("DiBan/Item" + i).gameObject.SetActive(true);
                    curGo.transform.Find("DiBan/Item" + i).Find("ItemSp").gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(curGo.transform.Find("DiBan/Item" + i).GetChild(0).gameObject, UIAnimArr[i], false);
                }
            });
        }

        int CountUIPageStartIndex()
        {
            int startIndex = 0;
            for (int i = 0; i < UITypeUICountArr.Length; i++)
            {
                if(i == CurUIPage)
                {
                    break;
                }
                startIndex += UITypeUICountArr[i];
            }
            return startIndex;
        }
    }
}
