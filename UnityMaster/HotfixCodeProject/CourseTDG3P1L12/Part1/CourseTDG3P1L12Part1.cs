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
    public class CourseTDG3P1L12Part1
    {
        GameObject curGo;
        GameObject PreTSObj;
        GameObject NextTSObj;
        Vector3[] OgrinPos;
        Vector3 OrginPos1;
        Vector3[] OgrinPos2;
        Vector3[] OgrinParPos3;
        Transform ContentTrans;
        Transform CurTrans;
        string[] IdleAnimNameArr;
        string[] IdleAnimNameArr1;  //显示进场动画
        string[] AnimNameArr;  //显示进场后Idle动画
        string[] BgImgName;
        string[] BtnAnimName;
        float[] EnterOffset;
        float TiShiTime;
        int[] ReSortNum;
        float PreContentPos;
        bool IsScroll = false;
        bool IsCheck = false;
        MonoBehaviour Mono;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Mono = curGo.GetComponent<MonoBehaviour>();
            OgrinPos = new Vector3[] {new Vector3(195.3f, -473.5f, 0), new Vector3(-251, -473.5f, 0), new Vector3(-757.4f, -473.5f, 0), Vector3.zero,
                                      new Vector3(195.3f, -473.5f, 0), new Vector3(-181.9f, -473.5f, 0), new Vector3(-615.5f, -473.5f, 0), Vector3.zero,
                                      new Vector3(-16.9f, -124.8f, 0), new Vector3(-7.7f, -120.2f, 0), new Vector3(-7.2f, -54.8f, 0), Vector3.zero};

            OrginPos1 = new Vector3(2879.6f, -165, 0);
            OgrinPos2 = new Vector3[] { new Vector3(594.58f, -464, 0), new Vector3(625, -474, 0),  new Vector3(-2, -62.8f, 0)};
            OgrinParPos3 = new Vector3[] {new Vector3(-31.5f, -472, 0), new Vector3(-59, -472, 0), new Vector3(-54, -472, 0), new Vector3(-54, -472, 0),
                                          new Vector3(20.7f, -465, 0), new Vector3(11.3f, -465, 0), new Vector3(19.8f, -465, 0), new Vector3(-28.4f, -465, 0),
                                          new Vector3(-233.8f, -174.8f, 0), new Vector3(239.2f, -113.72f, 0), new Vector3(687, -84.8f, 0), new Vector3(-706.3f, -114.1f, 0)};

            ReSortNum = new int[] { 0, 3, 2, 1, 0, 1, 2, 3, 1, 2, 3, 0};

            ContentTrans = curTrans.Find("bg/ScrollRect_Parent/Viewport/Content");
            PreTSObj = curTrans.Find("bg/PreTiShi").gameObject;
            NextTSObj = curTrans.Find("bg/NextTiShi").gameObject;
            IdleAnimNameArr = new string[] { "giraffe_idle", "bottle_idle", "animation" };
            AnimNameArr = new string[] { "giraffe_3", "giraffe_2", "giraffe_1", "bottle_g", "bottle_t", "bottle_s", "animation", "animation", "animation" };
            IdleAnimNameArr1 = new string[] { "giraffe_idle3", "giraffe_idle2", "giraffe_idle1", "bottle_idle1", "bottle_idle2", "bottle_idle3", "idle", "idle", "idle" };
            BgImgName = new string[] {"giraffe_bg", "pingzi_bg", "bg_3"};
            BtnAnimName = new string[] { "stretching", "tortuosity", "geometry", "contrast" };

            for (int i = 0; i < 3; i++)
            {
                Transform Par2 = ContentTrans.GetChild(i);
                for (int j = 0; j < 4; j++)
                {
                    Par2.GetChild(j).localPosition = OgrinPos[i * 4 + j];
                    Par2.GetChild(j).gameObject.SetActive(false);
                }
            }

            ContentTrans.localPosition = OrginPos1;
            PreContentPos = ContentTrans.localPosition.x;
            IsScroll = false;
            IsCheck = false;
            NextTSObj.SetActive(false);
            PreTSObj.SetActive(false);
            if (curGo.transform.Find("bg/ScrollRect_Parent").GetComponent<ScrollRectMoveManager>() != null)
            {
                GameObject.DestroyImmediate(curGo.transform.Find("bg/ScrollRect_Parent").GetComponent<ScrollRectMoveManager>());
            }
            curGo.transform.Find("bg/ScrollRect_Parent").gameObject.AddComponent<ScrollRectMoveManager>();
            ScrollRectMoveManager.instance.CreateManager(curTrans.transform.Find("bg/ScrollRect_Parent"), OnDrag);
            Debug.Log("ScrollRectMoveManager.instance.index: " + ScrollRectMoveManager.instance.index);
            CurTrans = null;
            Transform Par3 = curTrans.Find("bg/BtnPlane");
            for (int i = 0; i < Par3.childCount; i++)
            {
                int j = i;
                Par3.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                Par3.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    for(int m = 0; m < 4; m++)
                    {
                        Par3.GetChild(j).gameObject.SetActive(true);
                        if (m != j)
                        {
                            Par3.GetChild(m).GetChild(0).gameObject.SetActive(false);
                            Par3.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        }
                        else
                        {
                            Par3.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                            Par3.GetChild(m).GetChild(0).gameObject.SetActive(true);
                            int n = m;
                            SpineManager.instance.DoAnimation(Par3.GetChild(m).GetChild(0).gameObject, BtnAnimName[m], false);
                        }
                    }
                   
                    if (CurTrans != null)
                    {
                        if (NameSwitchToNumber(CurTrans.name) != j)
                        {
                            GameObject NewCurObj = ContentTrans.transform.GetChild(ScrollRectMoveManager.instance.index).GetChild(j).gameObject;

                            //Vector3 OldNewPos = OgrinPos[ScrollRectMoveManager.instance.index * 4 + NameSwitchToNumber(CurTrans.name)];
                            //OldNewPos = new Vector3(OldNewPos.x - EnterOffset[j], OldNewPos.y, OldNewPos.z);
                            //CurTrans.transform.DOLocalMove(OldNewPos, 0.25f).OnComplete(() =>
                            //{
                            //SpineManager.instance.DoAnimation(CurTrans.gameObject, "1", false);
                                Debug.Log("CurTrans: " + CurTrans.name);
                                if(j != 3)
                                {

                                    PreTSObj.gameObject.SetActive(false);
                                    NextTSObj.gameObject.SetActive(false);
                                }
                                if(CurTrans.name != "Par")
                                {
                                    CurTrans.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimation(0, 0);
                                }
                                //CurTrans.gameObject.SetActive(false);
                                CurTrans.localPosition = new Vector3(10000, 120, 0);
                                CurTrans = NewCurObj.transform;
                                CurTrans.localPosition = OgrinPos[ScrollRectMoveManager.instance.index * 4 + j];
                                
                            //});
                            NewCurObj.SetActive(true);

                            //Vector3 NewNewPos = OgrinPos[ScrollRectMoveManager.instance.index * 4 + j];
                            //NewNewPos = new Vector3(NewNewPos.x + EnterOffset[j], NewNewPos.y, NewNewPos.z);
                            //NewCurObj.transform.localPosition = NewNewPos;
                            //NewCurObj.transform.DOLocalMove(OgrinPos[ScrollRectMoveManager.instance.index * 4 + j], 0.25f).OnComplete(() =>
                            //{

                            //});
                            if (j != 3)
                            {
                                int n = ScrollRectMoveManager.instance.index * 3 + j;

                                SpineManager.instance.DoAnimation(NewCurObj, AnimNameArr[n], false, () =>
                                {
                                    SpineManager.instance.DoAnimation(NewCurObj, IdleAnimNameArr1[n], true);
                                });
                            }
                            else
                            {
                                SetBtnActive(false);
                                for (int m = 0; m < 4; m++)
                                {
                                    if (ScrollRectMoveManager.instance.index == 0)
                                    {
                                        int n = m;
                                        Transform ts = NewCurObj.transform.GetChild(n);
                                        Vector3 pos1 = OgrinParPos3[n];
                                        ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                        if (n == 0)
                                        {
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "giraffe_idle", true);
                                        }
                                        else
                                        { 
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "giraffe_idle" + m, true);
                                        }

                                        Mono.StartCoroutine(Wait(ReSortNum[n] * 0.6f, ()=>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                            ts.DOLocalMove(OgrinParPos3[n], 0.4f);
                                        }));
                                    }
                                    else if (ScrollRectMoveManager.instance.index == 1)
                                    {
                                        int n = m;
                                        Transform ts = NewCurObj.transform.GetChild(n);
                                        Vector3 pos1 = OgrinParPos3[n + 4];
                                        ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                        if (n == 0)
                                        {
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "bottle_idle", true);
                                        }
                                        else
                                        {
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "bottle_idle" + m, true);
                                        }

                                        Mono.StartCoroutine(Wait(ReSortNum[n + 4] * 0.6f, () =>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                            ts.DOLocalMove(OgrinParPos3[n + 4], 0.4f);
                                        }));
                                    }
                                    else if (ScrollRectMoveManager.instance.index == 2)
                                    {
                                        int n = m;
                                        Transform ts = NewCurObj.transform.GetChild(n);
                                        Vector3 pos1 = OgrinParPos3[n + 8];
                                        ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                        if (n == 3)
                                        {
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "animation", true);
                                        }
                                        else
                                        {
                                            SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "idle", true);
                                        }
                                        Mono.StartCoroutine(Wait(ReSortNum[n + 8] * 0.6f, () =>
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                            ts.DOLocalMove(OgrinParPos3[n + 8], 0.4f);
                                        }));
                                    }
                                }
                                Mono.StartCoroutine(Wait(3.0f, () =>
                                {
                                    if(ScrollRectMoveManager.instance.index != 2)
                                    {
                                        TiShiTime = 3;
                                        NextTSObj.SetActive(true);
                                        SpineManager.instance.DoAnimation(NextTSObj, "4", true);
                                    }
                                    if(ScrollRectMoveManager.instance.index != 0)
                                    {
                                        TiShiTime = 3;
                                        PreTSObj.SetActive(true);
                                        SpineManager.instance.DoAnimation(PreTSObj, "4", true);
                                    }
                                    SetBtnActive(true);
                                }));
                            }
                        }
                    }
                    else
                    {
                        //SetBtnActive(false);
                        //Vector3 NewNewPos = OgrinPos[ScrollRectMoveManager.instance.index * 4 + j];
                        //NewNewPos = new Vector3(NewNewPos.x + EnterOffset[j], NewNewPos.y, NewNewPos.z);
                        GameObject NewCurObj = ContentTrans.transform.GetChild(ScrollRectMoveManager.instance.index).GetChild(j).gameObject;
                        //NewCurObj.transform.localPosition = NewNewPos;
                        NewCurObj.SetActive(true);
                        //NewCurObj.transform.DOLocalMove(OgrinPos[ScrollRectMoveManager.instance.index * 4 + j], 0.25f).OnComplete(() =>
                        //{
                            CurTrans = NewCurObj.transform;
                        //});

                        //Vector3 IdleNewPos = OgrinPos2[ScrollRectMoveManager.instance.index];
                        //IdleNewPos = new Vector3(IdleNewPos.x - EnterOffset[j], IdleNewPos.y, 0);
                        Transform tans = ContentTrans.transform.GetChild(ScrollRectMoveManager.instance.index).Find("Idle");
                        //tans.DOLocalMove(IdleNewPos, 0.25f).OnComplete(() =>
                        //{
                            tans.gameObject.SetActive(false);
                        //});
                        
                        if (j != 3)
                        {
                            Debug.Log("ScrollRectMoveManager.instance.index1: " + ScrollRectMoveManager.instance.index + "  j: " + j);
                            int n = ScrollRectMoveManager.instance.index * 3 + j;
                            

                            SpineManager.instance.DoAnimation(NewCurObj, AnimNameArr[n], false, () =>
                            {
                               
                                SpineManager.instance.DoAnimation(NewCurObj, IdleAnimNameArr1[n], true);
                            });
                        }
                        else
                        {
                            SetBtnActive(false);
                            for (int m = 0; m < 4; m++)
                            {
                                if (ScrollRectMoveManager.instance.index == 0)
                                {
                                    int n = m;
                                    Transform ts = NewCurObj.transform.GetChild(n);
                                    Vector3 pos1 = OgrinParPos3[n];
                                    ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                    if (n == 0)
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "giraffe_idle", true);
                                    }
                                    else
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "giraffe_idle" + m, true);
                                    }
                                    Mono.StartCoroutine(Wait(ReSortNum[n] * 0.6f, () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                        ts.DOLocalMove(OgrinParPos3[n], 0.4f);
                                    }));
                                }
                                else if (ScrollRectMoveManager.instance.index == 1)
                                {
                                    int n = m;
                                    Transform ts = NewCurObj.transform.GetChild(n);
                                    Vector3 pos1 = OgrinParPos3[n + 4];
                                    ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                    if (n == 0)
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "bottle_idle", true);
                                    }
                                    else
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "bottle_idle" + m, true);
                                    }
                                    Mono.StartCoroutine(Wait(ReSortNum[n + 4] * 0.6f, () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                        ts.DOLocalMove(OgrinParPos3[n + 4], 0.4f);
                                    }));
                                }
                                else if (ScrollRectMoveManager.instance.index == 2)
                                {
                                    int n = m;
                                    Transform ts = NewCurObj.transform.GetChild(n);
                                    Vector3 pos1 = OgrinParPos3[n + 8];
                                    ts.localPosition = new Vector3(pos1.x + 1920, pos1.y, pos1.z);
                                    Debug.Log("NewCurObj.transform.GetChild(m).name: " + NewCurObj.transform.GetChild(m).name);
                                    if(n == 3)
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "animation", true);
                                    }
                                    else
                                    {
                                        SpineManager.instance.DoAnimation(NewCurObj.transform.GetChild(m).gameObject, "idle", true);
                                    }
                                    
                                    Mono.StartCoroutine(Wait(ReSortNum[n + 8] * 0.6f, () =>
                                    {
                                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                        ts.DOLocalMove(OgrinParPos3[n + 8], 0.4f);
                                    }));
                                }
                            }
                            Mono.StartCoroutine(Wait(3.0f, () =>
                            {
                                if (ScrollRectMoveManager.instance.index != 2)
                                {
                                    TiShiTime = 3;
                                    NextTSObj.SetActive(true);
                                    SpineManager.instance.DoAnimation(NextTSObj, "4", true);
                                }
                                if (ScrollRectMoveManager.instance.index != 0)
                                {
                                    TiShiTime = 3;
                                    PreTSObj.SetActive(true);
                                    SpineManager.instance.DoAnimation(PreTSObj, "4", true);
                                }
                                SetBtnActive(true);
                            }));
                        }
                    }
                });
            }

            SetBtnActive(false);
            GameObject go = curTrans.Find("bg/npc").gameObject;
            Mono.StartCoroutine(ShowBtn());
            SoundManager.instance.Speaking(go, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                PreContentPos = ContentTrans.localPosition.x;
                SetBtnActive(true);
                IsCheck = true;
            });

            Vector3 IdleNewPos1 = OgrinPos2[ScrollRectMoveManager.instance.index];
            IdleNewPos1 = new Vector3(IdleNewPos1.x - 1920, IdleNewPos1.y, 0);
            Transform tans1 = ContentTrans.transform.GetChild(ScrollRectMoveManager.instance.index).Find("Idle");
            tans1.gameObject.SetActive(true);
            tans1.localPosition = IdleNewPos1;
            tans1.DOLocalMove(OgrinPos2[ScrollRectMoveManager.instance.index], 0.25f).OnComplete(() =>
            {
                SpineManager.instance.DoAnimation(tans1.gameObject, IdleAnimNameArr[ScrollRectMoveManager.instance.index], true);
            });

            for(int i = 0; i < Par3.childCount; i++)
            {
                Par3.GetChild(i).gameObject.SetActive(false);
            }

            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.2f);
        }

        IEnumerator ShowBtn()
        {
            yield return new WaitForSeconds(0.1f);
            Transform Par3 = curGo.transform.Find("bg/BtnPlane");
            for(int i = 0; i < 4; i++)
            {
                int j = i;
                Par3.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                Par3.GetChild(i).gameObject.SetActive(true);
                Par3.GetChild(i).GetChild(0).gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Par3.GetChild(i).GetChild(0).gameObject, BtnAnimName[i], false, ()=>
                {
                    Par3.GetChild(j).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                    Par3.GetChild(j).GetChild(0).gameObject.SetActive(false);
                });
                yield return new WaitForSeconds(0.25f);
            }
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            if(act != null)
            {
                act.Invoke();
            }
        }

        void SetBtnActive(bool Active)
        {
            Transform Par3 = curGo.transform.Find("bg/BtnPlane");
            for (int i = 0; i < Par3.childCount; i++)
            {
                Par3.GetChild(i).GetComponent<Image>().raycastTarget = Active;
            }
            curGo.transform.Find("bg/ScrollRect_Parent/Viewport").GetComponent<Image>().raycastTarget = Active;
        }

        void ShowIdle(int index)
        {
            Vector3 IdleNewPos = OgrinPos2[index];
            Transform tans = ContentTrans.transform.GetChild(index).Find("Idle");
            tans.gameObject.SetActive(true);
            tans.localPosition = IdleNewPos;
            SpineManager.instance.DoAnimation(tans.gameObject, IdleAnimNameArr[index], true);
            Debug.Log("over");
        }

        void OnDrag()
        {
            ContentTrans.parent.GetComponent<Image>().raycastTarget = true;
            if (CurTrans != null)
            {
                if(CurTrans.transform.parent.name != ContentTrans.GetChild(ScrollRectMoveManager.instance.index).name)
                { 
                    CurTrans.gameObject.SetActive(false);
                    CurTrans = null;
                }
            }
            Debug.Log("ScrollRectMoveManager.instance.index11: " + ScrollRectMoveManager.instance.index);
            PreContentPos = ContentTrans.localPosition.x;
            IsScroll = false;
            SetBtnActive(true);
            curGo.transform.Find("bg").GetComponent<Image>().sprite = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseTDG3P1L12Part1"),
                                                                                                           BgImgName[ScrollRectMoveManager.instance.index]);
            Transform Par3 = curGo.transform.Find("bg/BtnPlane");
            for (int i = 0; i < 4; i++)
            {
                Par3.GetChild(i).gameObject.SetActive(true);
                Par3.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                Par3.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
        }

        void FixedUpdate()
        {
            if(IsCheck && Math.Abs(PreContentPos - ContentTrans.localPosition.x) > 2.0f)
            {
                if (IsScroll == false)
                {
                    PreTSObj.SetActive(false);
                    NextTSObj.SetActive(false);
                    ContentTrans.parent.GetComponent<Image>().raycastTarget = false;
                    IsScroll = true;
                    if(PreContentPos - ContentTrans.localPosition.x > 0)
                    { 
                        SwitchPage(1);
                    }
                    else
                    {
                        SwitchPage(-1);
                    }
                    PreContentPos = ContentTrans.localPosition.x;
                }
            }

            if(PreTSObj.activeSelf || NextTSObj.activeSelf)
            {
                TiShiTime -= Time.deltaTime;
                if(TiShiTime < 0)
                {
                    PreTSObj.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0.01f);
                    NextTSObj.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0.01f);
                    if (TiShiTime < -5)
                    {
                        TiShiTime = 3;
                    }
                }
                else
                {
                    PreTSObj.GetComponent<SkeletonGraphic>().color = Color.white;
                    NextTSObj.GetComponent<SkeletonGraphic>().color = Color.white;
                }
            }
        }

        void SwitchPage(int offset)
        {
            int i = ScrollRectMoveManager.instance.index + offset;
            if(i < 0 || i > 2)
            {
                return;
            }
            SetBtnActive(false);
            Transform Par2 = ContentTrans.GetChild(i);

            curGo.transform.Find("bg").GetComponent<Image>().sprite = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseTDG3P1L12Part1"),
                                                                                               BgImgName[i]);

            for (int j = 0; j < 4; j++)
            {
                Par2.GetChild(j).localPosition = OgrinPos[i * 4 + j];
                Par2.GetChild(j).gameObject.SetActive(false);
            }
            ShowIdle(i);
        }

        int NameSwitchToNumber(string name)
        {
            if (name.Contains("_0"))
            {
                return 0;
            }
            else if (name.Contains("_1"))
            {
                return 1;
            }
            else if (name.Contains("_2"))
            {
                return 2;
            }
            else if (name == "Par")
            {
                return 3;
            }
            return -1;
        }
    }
}
