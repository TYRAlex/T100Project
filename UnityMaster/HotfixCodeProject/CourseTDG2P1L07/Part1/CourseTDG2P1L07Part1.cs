using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseTDG2P1L07Part1
    {
        GameObject curGo;
        GameObject CurShowAreaObj;
        GameObject WinObj;
        GameObject ShowArea;
        int CurPage;
        string[] BuWei;
        string[] AnimBuWei;
        string[] AnimBuWei1;
        string[] ChuFaNameArr;
        GameObject CurSelectUIObj;
        Vector3 PreMousePos;
        bool IsPressUIDown;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            BuWei = new string[] { "UI_Body0", "UI_Body1", "UI_Body2", "UI_Body3", "Eye0", "Eye1", "Eye2", "Eye3", "Leg0",
                                    "Leg1", "Leg2", "Leg3", "Tail0", "Tail1", "Tail2", "Tail3" };
            AnimBuWei = new string[] { "ui_b1", "ui_b2", "ui_b3", "ui_b4", "ui_e1", "ui_e2", "ui_e3", "ui_e4", "ui_f1",
                                        "ui_f2", "ui_f3", "ui_f4", "ui_o1", "ui_o2", "ui_o3", "ui_o4" };
            AnimBuWei1 = new string[] { "ui_b", "ui_e", "ui_f", "ui_o" };
            ChuFaNameArr = new string[] { "BodyRect", "ChuFaArea_Eye", "ChuFaArea_SiZhi", "ChuFaArea_Wing"};

            ShowArea = curTrans.Find("ShowArea").gameObject;
            WinObj = curTrans.Find("Win1").gameObject;
            IsPressUIDown = false;
            SwitchPage(0);

            if (WinObj.transform.GetChild(0).name != "Win2")
            {
                GameObject.DestroyImmediate(WinObj.transform.GetChild(0).gameObject);
            }

            Transform t1 = curGo.transform.Find("DiBan/ItemPlane");
            
            for (int i = 0; i < 16; i++)
            {
                int j = i;
                ILObject3DAction i3d = t1.GetChild(i).GetComponent<ILObject3DAction>();
                i3d.OnPointDownLua += OnPressUiDown;
                i3d.OnPointUpLua += OnPressUiUp;
                /*
                t1.GetChild(i).GetComponent<Button>().onClick.RemoveAllListeners();
                t1.GetChild(i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                    if (j < 4)
                    {
                        for (int m = 0; m < 4; m++)
                        {
                            if (j == m)
                            {
                                ShowBody(j);
                                t1.GetChild(m).GetChild(0).gameObject.SetActive(true);
                                t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                            }
                            else
                            {
                                ShowArea.transform.GetChild(m).gameObject.SetActive(false);
                                t1.GetChild(m).GetChild(0).gameObject.SetActive(false);
                                t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            }
                        }
                        SpineManager.instance.DoAnimation(t1.GetChild(j).GetChild(0).gameObject, AnimBuWei[j], false);
                    }
                    else
                    {
                        for (int m = CurPage * 4; m < CurPage * 4 + 4; m++)
                        {
                            if (m == j)
                            {
                                CurShowAreaObj.transform.Find(BuWei[m]).gameObject.SetActive(true);
                                t1.GetChild(m).GetChild(0).gameObject.SetActive(true);
                                t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                            }
                            else
                            {
                                CurShowAreaObj.transform.Find(BuWei[m]).gameObject.SetActive(false);
                                t1.GetChild(m).GetChild(0).gameObject.SetActive(false);
                                t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                            }
                        }
                        SpineManager.instance.DoAnimation(t1.GetChild(j).GetChild(0).gameObject, AnimBuWei[j], false);
                    }
                    curTrans.Find("DiBan/Ok").gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(curTrans.Find("DiBan/Ok").gameObject, "ok", false);
                });
                */
            }

            curTrans.Find("DiBan/Ok/Button").GetComponent<Button>().onClick.RemoveAllListeners();
            curTrans.Find("DiBan/Ok/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                if (CurPage >= 3)
                {
                    WinObj.SetActive(true);
                    WinObj.transform.Find("Win2").gameObject.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    SpineManager.instance.DoAnimation(WinObj, "idle", true);
                    GameObject obj = GameObject.Instantiate(CurShowAreaObj);
                    obj.transform.SetParent(WinObj.transform, false);
                    obj.transform.localPosition = new Vector3(0, 540, 0);
                    obj.transform.localScale = Vector3.zero;
                    obj.transform.SetAsFirstSibling();
                    obj.transform.DOScale(1, 0.2f).SetEase(Ease.InOutBack).OnComplete(() =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                        WinObj.transform.Find("Win2").gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(WinObj.transform.Find("Win2").gameObject, "animation_wow", false);
                    });
                }
                Debug.Log("CurPage: " + CurPage);
                SwitchPage((CurPage + 1) % 4);
                curTrans.Find("DiBan/Ok").gameObject.SetActive(false);
                
            });
            curTrans.Find("DiBan/Ok").gameObject.SetActive(false);
            WinObj.SetActive(false);

            WinObj.transform.Find("Win2/Button").GetComponent<Button>().onClick.RemoveAllListeners();
            WinObj.transform.Find("Win2/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                if(WinObj.transform.GetChild(0).name != "Win2")
                {
                    GameObject.DestroyImmediate(WinObj.transform.GetChild(0).gameObject);
                }
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                WinObj.gameObject.SetActive(false);
            });

            SoundManager.instance.Speaking(curTrans.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0);
            SoundManager.instance.BgSoundPart1();
        }

        void SetBtnActive(bool active)
        {
            for(int i = 0; i < 16; i++)
            {
                curGo.transform.Find("DiBan/ItemPlane/Item" + i).GetComponent<Image>().raycastTarget = active;
            }
        }

        void OnPressUiDown(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            GameObject go = curGo.transform.Find("DiBan/ItemPlane/Item" + index).gameObject;
            CurSelectUIObj = GameObject.Instantiate(go);
            CurSelectUIObj.transform.SetParent(curGo.transform.Find("DiBan/ItemPlane"), true);
            CurSelectUIObj.transform.GetChild(0).gameObject.SetActive(false);
            CurSelectUIObj.GetComponent<Image>().color = Color.white;
            CurSelectUIObj.transform.localScale = Vector3.one;
            CurSelectUIObj.transform.localPosition = go.transform.localPosition;
            IsPressUIDown = true;
            PreMousePos = Input.mousePosition;
            if(index < 4)
            {
                for(int i = 0; i < 4; i++)
                {
                    ShowArea.transform.Find(BuWei[i]).gameObject.SetActive(false);
                }
            }
        }

        void OnPressUiUp(int index)
        {
            IsPressUIDown = false;
            CurSelectUIObj.GetComponent<Image>().raycastTarget = false;
            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);

            bool IsHit = false;
            for(int i = 0; i < results.Count; i++)
            {
                if(results[i].gameObject.name == ChuFaNameArr[index / 4])
                {
                    IsHit = true;
                    break;
                }
            }

            Transform t1 = curGo.transform.Find("DiBan/ItemPlane");
            if (IsHit)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                if (index < 4)
                {
                    for (int m = 0; m < 4; m++)
                    {
                        if (index == m)
                        {
                            ShowBody(index);
                            t1.GetChild(m).GetChild(0).gameObject.SetActive(true);
                            t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                        }
                        else
                        {
                            ShowArea.transform.GetChild(m).gameObject.SetActive(false);
                            t1.GetChild(m).GetChild(0).gameObject.SetActive(false);
                            t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        }
                    }
                    SpineManager.instance.DoAnimation(t1.GetChild(index).GetChild(0).gameObject, AnimBuWei[index], false);
                }
                else
                {
                    int num = index / 4;
                    for (int m = num * 4; m < num * 4 + 4; m++)
                    {
                        if (m == index)
                        {
                            CurShowAreaObj.transform.Find(BuWei[m]).gameObject.SetActive(true);
                            t1.GetChild(m).GetChild(0).gameObject.SetActive(true);
                            t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                        }
                        else
                        {
                            CurShowAreaObj.transform.Find(BuWei[m]).gameObject.SetActive(false);
                            t1.GetChild(m).GetChild(0).gameObject.SetActive(false);
                            t1.GetChild(m).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                        }
                    }
                    SpineManager.instance.DoAnimation(t1.GetChild(index).GetChild(0).gameObject, AnimBuWei[index], false);
                }
                GameObject.Destroy(CurSelectUIObj);
                CurSelectUIObj = null;
                curGo.transform.Find("DiBan/Ok").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(curGo.transform.Find("DiBan/Ok").gameObject, "ok", false);
            }
            else
            {
                SetBtnActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                CurSelectUIObj.transform.DOLocalMove(t1.transform.Find("Item" + index).localPosition, 0.25f).OnComplete(()=>
                {
                    SetBtnActive(true);
                    GameObject.Destroy(CurSelectUIObj);
                    CurSelectUIObj = null;
                });
            }
        }

        void FixedUpdate()
        {
            if (IsPressUIDown)
            {
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                CurSelectUIObj.transform.localPosition += (Input.mousePosition - PreMousePos) * scaleX;
                PreMousePos = Input.mousePosition;
            }
        }

        void ShowBody(int m)
        {
            Transform trans = ShowArea.transform.Find(BuWei[m]);
            trans.gameObject.SetActive(true);
            //if(m < 4)
            //{
                //trans.transform.SetParent(curGo.transform.Find("DiBan/ItemPlane"), false);
                //trans.transform.SetParent(ShowArea.transform, true);
                //trans.transform.localPosition = new Vector3(-100, 0, 0);
                //trans.SetSiblingIndex(m);
            //}
            for(int n = 0; n < ShowArea.transform.Find(BuWei[m]).childCount; n++)
            {
                if(ShowArea.transform.Find(BuWei[m]).GetChild(n).name == "b0" || ShowArea.transform.Find(BuWei[m]).GetChild(n).name.Contains("ChuFaArea"))
                {
                    ShowArea.transform.Find(BuWei[m]).GetChild(n).gameObject.SetActive(true);
                }
                else if(ShowArea.transform.Find(BuWei[m]).GetChild(n).name == "shadow")
                {
                    ShowArea.transform.Find(BuWei[m]).GetChild(n).gameObject.SetActive(true);
                }
                else
                {
                    ShowArea.transform.Find(BuWei[m]).GetChild(n).gameObject.SetActive(false);
                }
            }
            CurShowAreaObj = ShowArea.transform.Find(BuWei[m]).gameObject;
        }

        void SwitchPage(int Page)
        {
            Transform t1 = curGo.transform.Find("DiBan/ItemPlane");
            Transform t2 = curGo.transform.Find("DiBan/ItemTypePlane/Item");
            CurPage = Page;
            for (int i = 0; i < 16; i++)
            {
                if(i >= Page * 4 && i < Page * 4 + 4)
                {
                    t1.GetChild(i).gameObject.SetActive(true);
                    t1.GetChild(i).GetChild(0).gameObject.SetActive(false);
                    t1.GetChild(i).GetComponent<Image>().color = new Color(1, 1, 1, 1);
                }
                else
                {
                    t1.GetChild(i).gameObject.SetActive(false);
                }
            }
            SpineManager.instance.DoAnimation(t2.gameObject, AnimBuWei1[CurPage], false);

            if(Page == 0)
            {
                for (int i = 0; i < ShowArea.transform.childCount; i++)
                {
                    ShowArea.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
        }
    }
}
