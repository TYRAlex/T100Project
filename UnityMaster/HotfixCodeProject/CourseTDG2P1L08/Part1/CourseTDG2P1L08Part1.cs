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
    public class CourseTDG2P1L08Part1
    {
        GameObject curGo;
        GameObject DiBan;
        GameObject CurSelectObj;
        GameObject ContentTrans;
        GameObject NextTiShi;
        GameObject PreTiShi;
        MonoBehaviour Mono;
        float PreContentPos;
        bool IsScroll;
        bool IsCheck;

        string[] UIAnimNameArr;
        string[] UIAnimNameArr1;
        int[] NumArr;
        int[] NumArr1;
        int[] ItemNumArr1;
        int[] ItemNumArr2;
        int[] ItemNumArr3;
        Dictionary<int, int[]> ItemNumArrDic;
        //string[] ItemAnimArr1;
        //string[] ItemAnimArr2;
        //string[] ItemAnimArr3;

        string[] ItemAnimArr1;
        string[] ItemAnimArr2;


        //Dictionary<int, string[]> ItemAnimArrDic;
        //Dictionary<int, int[]> MatchIndexDic;
        int CurPage = 0;
        int GrounpPage = 0;
        float timeCount = 3;

        Vector3 PreMousePos;
        bool IsPressDown;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Mono = curGo.GetComponent<MonoBehaviour>();
            UIAnimNameArr = new string[] { "ui_vase1", "ui_vase2", "ui_vase3", "uibar_dinnerware1", "uibar_dinnerware2", "uibar_dinnerware3", "uibar_dinnerware4", "uibar_dinnerware5",
                                          "ui_fruit1", "ui_fruit2", "ui_fruit3", "ui_fruit4", "ui_fruit5"};
            UIAnimNameArr1 = new string[] { "uibar_vase", "uibar_dinnerware", "uibar_fruit"};

            //ItemAnimArr1 = new string[] { "vase1", "p1_dinnerware3", "p1_dinnerware4", "p1_dinnerware2", "p1_fruit4", "p1_fruit3", "p1_fruit1", "p1_fruit5" };
            //ItemAnimArr2 = new string[] { "vase2", "p2_dinnerware4", "p2_fruit3", "p2_dinnerware2", "p2_fruit4", "p2_dinnerware5", "p2_fruit1" };
            //ItemAnimArr3 = new string[] { "p3_fruit3", "vase_3", "p3_fruit1", "p3_dinnerware1", "p3_dinnerware2", "p3_fruit2", "p3_fruit5" };
            ItemAnimArr1 = new string[] { "p{0}_vase_s", "p{0}_dinnerware_s", "p{0}_fruit_s" };
            ItemAnimArr2 = new string[] { "vase1", "vase2", "vase3", "dinnerware1", "dinnerware2", "dinnerware3", "dinnerware4",
                                          "dinnerware5", "fruit1", "fruit2", "fruit3", "fruit4", "fruit5" };

            //int[] MatchPageIndex1 = new int[] { 10, 2, 3, 1, 8, 7, 5, 9 };
            //int[] MatchPageIndex2 = new int[] { 11, 3, 7, 1, 8, 4, 5};
            //int[] MatchPageIndex3 = new int[] { 7, 12, 5, 0, 1, 6, 9};
            NumArr = new int[] { 3, 5, 5 };
            NumArr1 = new int[] { 0, 3, 8 };

            ItemNumArr1 = new int[] {1, 2, 3};
            ItemNumArr2 = new int[] {1, 2, 4};
            ItemNumArr3 = new int[] { 1, 2, 3};
            ItemNumArrDic = new Dictionary<int, int[]>();
            ItemNumArrDic.Add(0, ItemNumArr1);
            ItemNumArrDic.Add(1, ItemNumArr2);
            ItemNumArrDic.Add(2, ItemNumArr3);
            // MatchIndexDic = new Dictionary<int, int[]>();
            //MatchIndexDic.Add(0, MatchPageIndex1);
            //MatchIndexDic.Add(1, MatchPageIndex2);
            //MatchIndexDic.Add(2, MatchPageIndex3);
            //ItemAnimArrDic = new Dictionary<int, string[]>();
            //ItemAnimArrDic.Add(0, ItemAnimArr1);
            //ItemAnimArrDic.Add(1, ItemAnimArr2);
            //ItemAnimArrDic.Add(2, ItemAnimArr3);

            DiBan = curTrans.Find("DB/DiBan").gameObject;
            NextTiShi = curTrans.Find("NextPageTile").gameObject;
            PreTiShi = curTrans.Find("PrePageTile").gameObject;
            timeCount = 3;


            for (int i = 0; i < 13; i++)
            {
                ILObject3DAction i3d = DiBan.transform.Find("ItemUI" + i).gameObject.GetComponent<ILObject3DAction>();
                i3d.OnPointDownLua = OnPointerDown;
                i3d.OnPointUpLua = OnPointerUp;
            }
            ContentTrans = curTrans.Find("ScrollRect_Parent/Viewport/Content").gameObject;
            ContentTrans.transform.localPosition = new Vector3(1920, 0, 0);
            

            if (curGo.transform.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("ScrollRect_Parent").GetComponent<ScrollRectMoveManager>());
            }
            curGo.transform.Find("ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curGo.transform.transform.Find("ScrollRect_Parent"), OnDrag);
            ContentTrans.transform.parent.parent.GetComponent<ScrollRect>().enabled = false;
            NextTiShi.SetActive(false);
            PreTiShi.SetActive(false);
            SetBtnActive(false);
            //DiBan.SetActive(false);

            for (int i = 0; i < 3; i++)
            {
                ContentTrans.transform.Find("Page" + i).Find("GouTuSP").gameObject.SetActive(false);
                ContentTrans.transform.Find("Page" + i).Find("HuaKuang_Mask").gameObject.SetActive(false);
                ContentTrans.transform.Find("Page" + i).Find("Type").gameObject.SetActive(false);
                for (int j = 0; j < 13; j++)
                {
                    Transform go = ContentTrans.transform.Find("Page" + i).Find("Rect" + j);
                    if (go != null)
                    {
                        go.gameObject.SetActive(false);
                    }
                    if(j < 6)
                    {
                        Transform tt = ContentTrans.transform.Find("Page" + i).Find("dot" + j);
                        if(tt != null)
                        {
                            if (tt.childCount > 0)
                            {
                                Transform tt1 = tt.GetChild(0);
                                tt1.SetParent(tt.parent, false);
                                tt1.transform.localScale = Vector3.one;
                                tt1.gameObject.SetActive(false);
                            }
                        }
                        if(j < 3)
                        {

                            ContentTrans.transform.Find("Page" + i).Find("ItemSDSP" + j).gameObject.SetActive(false);
                        }
                    }
                }
            }

            IsScroll = false;
            IsCheck = false;
            GrounpPage = 0;
            CurPage = 0;

            SwitchPage(0);
            SoundManager.instance.Speaking(curTrans.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0, null, ()=>
            {
                StartNewPage();
                IsCheck = true;
            });
            SoundManager.instance.BgSoundPart2();
        }

        void StartNewPage()
        {
            GameObject go = ContentTrans.transform.Find("Page" + GrounpPage).Find("GouTuSP").gameObject;
            GameObject go1 = ContentTrans.transform.Find("Page" + GrounpPage).Find("Type").gameObject;
            go.SetActive(true);
            go1.SetActive(true);
            
            string s1 = string.Format("p{0}_illustrate", (GrounpPage + 1));
            string s2 = string.Format("p{0}_illustrate2", (GrounpPage + 1));
            string s3 = string.Format("p{0}_name", (GrounpPage + 1));

            SpineManager.instance.DoAnimation(go1, s3, false);

            SpineManager.instance.DoAnimation(go, s1, false, () =>
            {
                SpineManager.instance.DoAnimation(go, s2, false, ()=>
                {
                    //DiBan.SetActive(true);
                    go.SetActive(false);
                    GameObject go2 = ContentTrans.transform.Find("Page" + GrounpPage).Find("ItemSDSP0").gameObject;
                    go2.SetActive(true);
                    SetBtnActive(true);
                    string s4 = string.Format(ItemAnimArr1[0], (GrounpPage + 1));
                    SpineManager.instance.DoAnimation(go2, s4, true);
                });
            });
        }

        void SwitchPage(int Page)
        {
            SetBtnActive(false);
            CurPage = Page;
            for (int i = 0; i < 13; i++)
            {
                DiBan.transform.Find("ItemUI" + i).gameObject.SetActive(false);
            }
            for(int i = 0; i < 3; i++)
            {
                if(CurPage > 0)
                { 
                    if(i != CurPage)
                    { 
                        ContentTrans.transform.Find("Page" + GrounpPage).Find("ItemSDSP" + i).gameObject.SetActive(false);
                    }
                    else
                    {
                        SetBtnActive(true);
                        GameObject go = ContentTrans.transform.Find("Page" + GrounpPage).Find("ItemSDSP" + i).gameObject;
                        go.SetActive(true);
                        
                        string s1 = string.Format(ItemAnimArr1[i], (GrounpPage + 1));
                        SpineManager.instance.DoAnimation(go, s1, true);
                    }
                }
            }
            GameObject SG = DiBan.transform.Find("ItemPopAnim").gameObject;
            SpineManager.instance.DoAnimation(SG, UIAnimNameArr1[Page], false);
            Mono.StartCoroutine(Wait(0.5f, () =>
            {
                for(int i = NumArr1[CurPage]; i < NumArr1[CurPage] + NumArr[CurPage]; i++)
                {
                    Transform go = DiBan.transform.Find("ItemUI" + i);
                    if(go != null)
                    {
                        go.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(go.gameObject, UIAnimNameArr[i], false);
                    }
                }
            }));
        }

        void OnPointerDown(int index)
        {
            GameObject go = DiBan.transform.Find("ItemUI" + index).gameObject;
            GameObject NewObj = GameObject.Instantiate(go);
            NewObj.transform.SetParent(DiBan.transform, false);
            NewObj.transform.localScale = Vector3.one;
            NewObj.transform.localPosition = go.transform.localPosition;
            NewObj.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            CurSelectObj = NewObj;
            PreMousePos = Input.mousePosition;
            IsPressDown = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
        }

        void OnPointerUp(int index)
        {
            IsPressDown = false;

            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);

            //int[] CurPageArr = MatchIndexDic[GrounpPage];
            Transform trans = null;
            int tempIndex = -1;
            for (int i = 0; i < results.Count; i++)
            {
                
                if(results[i].gameObject.name.Contains("Check") == false)
                {
                    continue;
                }
                trans = results[i].gameObject.transform;
                break;
                
                //Debug.Log("CurPageArr[index1]: " + CurPageArr[index1] + "   index: " + index);
                /*
               if (CurPageArr[index1] == index)
               {
                   trans = results[i].gameObject.transform;
                   tempIndex = index1;
                   break;
               }
               */
            }
            if (trans == null)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                int index1 = SwitchNameToIndex(CurSelectObj.name);
                SetBtnActive(false);
                CurSelectObj.transform.DOLocalMove(DiBan.transform.Find("ItemUI" + index1).localPosition, 0.25f).OnComplete(()=>
                {
                    SetBtnActive(true);
                    GameObject.Destroy(CurSelectObj.gameObject);
                    CurSelectObj = null;
                });
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                GameObject.Destroy(CurSelectObj);
                if (trans.childCount > 0)
                {
                    Transform t2 = trans.GetChild(0);
                    t2.SetParent(ContentTrans.transform.Find("Page" + GrounpPage), false);
                    t2.gameObject.SetActive(false);
                }
                Transform t1 = ContentTrans.transform.Find("Page" + GrounpPage + "/Rect" + index);
                if (t1 == null)
                {
                    Transform t4 = ContentTrans.transform.Find("Page" + GrounpPage);
                    for(int i = 0; i < 7; i++)
                    {
                        if(t4.Find("dot" + i) != null && t4.Find("dot" + i).childCount > 0)
                        {
                            if (t4.Find("dot" + i).GetChild(0).name == "Rect" + index)
                            {
                                t1 = t4.Find("dot" + i).GetChild(0);
                                break;
                            }
                        }
                    }
                }
                t1.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(t1.GetChild(0).gameObject, ItemAnimArr2[index], false);
                t1.SetParent(trans, false);
                t1.localScale = Vector3.one;
                t1.localPosition = Vector3.zero;
                int num1 = SwitchNameToIndex(trans.name);
                if(ContentTrans.transform.Find("Page" + GrounpPage).Find("dot" + num1).childCount > 0)
                {
                    Transform t5 = ContentTrans.transform.Find("Page" + GrounpPage).Find("dot" + num1).GetChild(0);
                    t5.SetParent(ContentTrans.transform.Find("Page" + GrounpPage));
                    t5.gameObject.SetActive(false);
                }
                t1.SetParent(ContentTrans.transform.Find("Page" + GrounpPage).Find("dot" + num1), true);
                CheckPageOver();
            }
        }

        void FixedUpdate()
        {
            if (IsPressDown)
            {
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                CurSelectObj.transform.localPosition += (Input.mousePosition - PreMousePos) * scaleX;
                PreMousePos = Input.mousePosition;
            }
            if(PreTiShi.activeSelf || NextTiShi.activeSelf)
            {
                if(timeCount < -5)
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
                if (IsScroll == false && IsCheck)
                {
                    PreTiShi.gameObject.SetActive(false);
                    NextTiShi.gameObject.SetActive(false);
                    IsScroll = true;
                    SetBtnActive(false);
                }
            }
            PreContentPos = ContentTrans.transform.localPosition.x;
        }
        void OnDrag()
        {
            if (ScrollRectMoveManager.instance.index < GrounpPage)
            {
                SetBtnActive(true);
                ContentTrans.transform.parent.parent.GetComponent<ScrollRect>().enabled = true;
                if(IsCheck)
                {
                    timeCount = 3;
                    if (ScrollRectMoveManager.instance.index == 0)
                    {
                        NextTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTiShi, "4", true);
                    }
                    else if (ScrollRectMoveManager.instance.index == 1)
                    {
                        NextTiShi.gameObject.SetActive(true);
                        PreTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(NextTiShi, "4", true);
                        SpineManager.instance.DoAnimation(PreTiShi, "4", true);
                    }
                    else
                    {
                        PreTiShi.gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(PreTiShi, "4", true);
                    }
                }
            }
            else
            {
                DiBan.gameObject.SetActive(true);
                ContentTrans.transform.parent.parent.GetComponent<ScrollRect>().enabled = false;
                SwitchPage(0);
                StartNewPage();
            }
            IsScroll = false;
        }

        void CheckPageOver()
        {
            //int[] CurPageArr = MatchIndexDic[GrounpPage];
            string s = string.Format("Page{0}", GrounpPage);
            Transform trans = ContentTrans.transform.Find(s);

            bool NeedChangePage = true;
            int startIndex = 0;
            int p1 = 0;
            while(p1 < CurPage)
            {
                startIndex += ItemNumArrDic[GrounpPage][p1];
                p1 += 1;
            }

            for (int i = startIndex; i < startIndex + ItemNumArrDic[GrounpPage][CurPage]; i++)
            {
                if(trans.Find("dot" + i).childCount == 0)
                {
                    NeedChangePage = false;
                    break;
                }
            }
            if(NeedChangePage)
            {
                if(CurPage == 2)
                {
                    DiBan.gameObject.SetActive(false);
                    string s3 = string.Format("ScrollRect_Parent/Viewport/Content/Page{0}/HuaKuang_Mask", GrounpPage);
                    GameObject go1 = curGo.transform.Find(s3).gameObject;
                    GameObject go2 = ContentTrans.transform.Find("Page" + GrounpPage).Find("ItemSDSP2").gameObject;
                    go2.SetActive(false);

                    go1.SetActive(true);
                    SpineManager.instance.DoAnimation(go1, "over_frame", false, ()=>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                        string s2 = string.Format("ScrollRect_Parent/Viewport/Content/Page{0}/GouTuSP", GrounpPage);
                        GameObject go3 = curGo.transform.Find(s2).gameObject;
                        go3.SetActive(true);

                        SpineManager.instance.DoAnimation(go3, string.Format( "p{0}_illustrate2", (GrounpPage + 1)), false);

                        Mono.StartCoroutine(Wait(1.5f, () =>
                        {
                            ContentTrans.transform.parent.parent.GetComponent<ScrollRect>().enabled = true;
                            if (GrounpPage < 2)
                            {
                                timeCount = 3;
                                NextTiShi.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(NextTiShi, "4", true);
                            }
                            if (GrounpPage > 0)
                            {
                                timeCount = 3;
                                PreTiShi.gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(PreTiShi, "4", true);
                            }
                            GrounpPage += 1;
                        }));
                    });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                }
                else
                {
                    SwitchPage(CurPage + 1);
                }
            }
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            if(act != null)
            act.Invoke();
        }

        IEnumerator Fade(GameObject o)
        {
            float a = o.GetComponent<SkeletonGraphic>().color.a;
            while (a > 0)
            {
                a -= Time.deltaTime * 5;
                o.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, a);
                yield return null;
            }
            yield return null;
            o.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
        }

        void SetBtnActive(bool active)
        {
            ContentTrans.transform.parent.GetComponent<Image>().raycastTarget = active;

            for (int i = 0; i < 3; i++)
            {
                ContentTrans.transform.Find("Page" + i).GetComponent<Image>().raycastTarget = active;
                ContentTrans.transform.Find("Page" + i).Find("bg").GetComponent<Image>().raycastTarget = active;
                for (int j = 0; j < 8; j++)
                {
                    Transform go = ContentTrans.transform.Find("Page" + i).Find("Bg_Item" + j);
                    if (go != null)
                    {
                        go.GetComponent<Image>().raycastTarget = active;
                    }
                }
            }

            for(int i = 0; i < 13; i++)
            {
                DiBan.transform.Find("ItemUI" + i).GetComponent<Image>().raycastTarget = active;
            }
        }

        int SwitchNameToIndex(string name)
        {
            if (name.Contains("12"))
            {
                return 12;
            }
            else if (name.Contains("11"))
            {
                return 11;
            }
            else if (name.Contains("10"))
            {
                return 10;
            }
            else if (name.Contains("9"))
            {
                return 9;
            }
            else if (name.Contains("8"))
            {
                return 8;
            }
            else if (name.Contains("7"))
            {
                return 7;
            }
            else if (name.Contains("6"))
            {
                return 6;
            }
            else if (name.Contains("5"))
            {
                return 5;
            }
            else if (name.Contains("4"))
            {
                return 4;
            }
            else if (name.Contains("3"))
            {
                return 3;
            }
            else if (name.Contains("2"))
            {
                return 2;
            }
            else if (name.Contains("1"))
            {
                return 1;
            }
            else if (name.Contains("0"))
            {
                return 0;
            }
            else
            {
                return -1;
            }
        }

        /*
        void ReSortSlibItem()
        {
            for(int i = 0; i < Data1.Count; i++)
            {
                Data1[i].go.transform.SetParent(ShowArea.transform, true);
                Data1[i].Height = Data1[i].go.transform.localPosition.y - Data1[i].go.GetComponent<RectTransform>().sizeDelta.y / 2;
            }

            Sort();
            for (int i = 0; i < Data1.Count; i++)
            {
                string name = string.Format("Item{0}_Rect", Data1[i].index);
                GameObject go = ShowArea.transform.Find(name).gameObject;
                Data1[i].go.transform.SetParent(go.transform, true);
                Data1[i].go.transform.parent.SetSiblingIndex(i);
            } 
        }

        void Sort()
        {
            for (int i = 0; i < Data1.Count; i++)
            {
                for (int j = i + 1; j < Data1.Count; j++)
                {
                    if(Data1[i].Height < Data1[j].Height)
                    {
                        var temp = Data1[i];
                        Data1[i] = Data1[j];
                        Data1[j] = temp;
                    }
                }
            }
        }
        */
    }
}
