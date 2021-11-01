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
    public class CourseTDG2P1L12Part1
    {
        GameObject curGo;
        int CurPage;
        List<int[]>PageArr;
        string[] ItemTypeAnimArr1;
        string[] ItemTypeAnimArr2;
        string[] ItemTypeAnimArr3;
        string[] TileAnimArr;
        GameObject Diban;
        GameObject ShowArea;
        GameObject CurSelectObj;
        GameObject RollPlane;
        GameObject WinObj;
        GameObject ComplateBtn;
        bool IsPressDiBanUI;
        Vector3 PreMousePostion;
        Vector3 PreItemPostion;
        float TileTime;
        MonoBehaviour Mono;
        int[] ColVal;
        int CurPressItemDownIndex;
        //Coroutine cr;

        Vector3[] OrginUIPos;
        

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Transform tran5 = curTrans.Find("Win").GetChild(0);
            if (tran5.name.Contains("Win1") == false)
            {
                tran5.SetParent(curTrans);
                tran5.localPosition = Vector3.zero;
                tran5.localScale = Vector3.one;
                tran5.SetSiblingIndex(4);
            }

            PageArr = new List<int[]>();
            PageArr.Add(new int[] { 1, 2, 3, 4, 5 });
            PageArr.Add(new int[] { 6, 7, 8, 9, 10, 11, 12 });
            PageArr.Add(new int[] { 13, 14, 15, 16 });
            ItemTypeAnimArr1 = new string[] { "tx", "st", "xj" };
            ItemTypeAnimArr2 = new string[] { "fl_1", "fl_2", "fl_3", "fl_1idle", "fl_2idle", "fl_3idle" };
            ItemTypeAnimArr3 = new string[] { "tx_1", "tx_2", "tx_3", "tx_4", "tx_5", "st_1", "st_2", "st_3", "st_4", "st_5", "st_6", "st_7", "xj_1", "xj_2", "xj_3", "xj_4"};
            TileAnimArr = new string[] {"animation1", "animation3", "animation4", "animation5", "animation2", "animation7", "animation8", "animation11", "animation15",
                                        "animation12", "animation10", "animation16", "animation6", "animation9", "animation13", "animation14"};

            OrginUIPos = new Vector3[] {new Vector3(-311.7f, -99.8f, 0), new Vector3(-173.5f, -4.3f, 0), new Vector3(8.8f, -62.5f, 0), new Vector3(215.7f, -9.9f, 0),
                                        new Vector3(329.2f, -96.3f, 0), new Vector3(-353.4f, -23.3f, 0), new Vector3(-235.7f, -68.7f, 0), new Vector3(-148.4f, -24.97f, 0),
                                        new Vector3(7.9f, -49.8f, 0), new Vector3(175, -43, 0), new Vector3(324.3f, -85.1f, 0), new Vector3(328.17f, 10.17f, 0),
                                        new Vector3(-348.9f, -37.97f, 0), new Vector3(-180.7f, -43, 0), new Vector3(159, -43, 0), new Vector3(317.9f, -46.8f, 0)};

            Diban = curTrans.Find("DiBan").gameObject;
            ShowArea = curTrans.Find("TilePlane").gameObject;
            RollPlane = curTrans.Find("RollPlane").gameObject;
            WinObj = curTrans.Find("WinOver").gameObject;
            ComplateBtn = curTrans.Find("CompelateSpine").gameObject;
            Mono = curTrans.GetComponent<MonoBehaviour>();

            for (int i = 0; i < 16; i++)
            {
                ILObject3DAction i3d = Diban.transform.Find("Item" + (i + 1)).GetComponent<ILObject3DAction>();
                i3d.OnPointDownLua = OnUIItemPointDown;
                i3d.OnPointUpLua = OnUIItemPointUp;
                i3d.gameObject.transform.localPosition = OrginUIPos[i];

                ShowArea.transform.GetChild(i).gameObject.SetActive(true);
                ShowArea.transform.GetChild(i).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0.01f);
                for (int j = 0; j < ShowArea.transform.GetChild(i).childCount; j++)
                {
                    ShowArea.transform.GetChild(i).GetChild(j).gameObject.SetActive(false);
                }
            }
            IsPressDiBanUI = false;

            RollPlane.transform.Find("RollRight/Button").GetComponent<Button>().onClick.RemoveAllListeners();
            RollPlane.transform.Find("RollRight/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                SpineManager.instance.DoAnimation(RollPlane.transform.Find("RollRight").gameObject, "xz", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                Rotate(true);
            });

            RollPlane.transform.Find("RollLeft/Button").GetComponent<Button>().onClick.RemoveAllListeners();
            RollPlane.transform.Find("RollLeft/Button").GetComponent<Button>().onClick.AddListener(() =>
            {
                SpineManager.instance.DoAnimation(RollPlane.transform.Find("RollLeft").gameObject, "xz1", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                Rotate(false);
            });

            RollPlane.GetComponent<Button>().onClick.RemoveAllListeners();
            RollPlane.GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                int index = NameSwitchNum(CurSelectObj.name);
                ShowArea.transform.Find("Tile" + index).GetChild(1).gameObject.SetActive(false);
                ShowArea.transform.Find("Tile" + index).GetChild(0).gameObject.SetActive(true);
                RollPlane.gameObject.SetActive(false);
            });

            SetBtnActive(true);
            for (int i = 13; i < 17; i++)
            {
                ILObject3DAction i3d = ShowArea.transform.Find("Tile" + i).GetChild(0).GetComponent<ILObject3DAction>();
                //i3d.OnPointDownLua = OnPressShowItemDown;
                //i3d.OnPointUpLua = OnPressShowItemUp;
                i3d.OnCollisionEnter2DLua = YuMaoColliderEnter;
                i3d.OnCollisionExit2DLua = YuMaoColliderExit;

                ILObject3DAction i3d1 = Diban.transform.Find("Item" + i).GetComponent<ILObject3DAction>();
                i3d1.OnCollisionEnter2DLua = YuMaoColliderEnter;
                i3d1.OnCollisionExit2DLua = YuMaoColliderExit;
            }
            SwitchPage(0);
            RollPlane.SetActive(false);
            WinObj.SetActive(false);
            curTrans.Find("TileJY1").gameObject.SetActive(false);
            curTrans.Find("TileJY2").gameObject.SetActive(false);
            SoundManager.instance.Speaking(curTrans.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0, null, ()=>
            {
                SetBtnActive(true);
                //curTrans.Find("TileJY1").gameObject.SetActive(true);
                //SpineManager.instance.DoAnimation(curTrans.Find("TileJY1").gameObject, "animation2", true);
                //cr = Mono.StartCoroutine(MA(curTrans.Find("TileJY1").GetComponent<SkeletonGraphic>()));
            });
            ColVal = new int[]{0, 0, 0, 0};
            SoundManager.instance.BgSoundPart1();
            CurPressItemDownIndex = -1;

            ComplateBtn.transform.localPosition = new Vector3(1300, -800, 0);
            ComplateBtn.transform.Find("Button").GetComponent<Button>().onClick.RemoveAllListeners();
            ComplateBtn.transform.Find("Button").GetComponent<Button>().onClick.AddListener(()=>
            {
                ComplateBtn.gameObject.transform.localPosition = new Vector3(1300, -800, 0);
                curTrans.Find("Win").gameObject.SetActive(true);
                curTrans.Find("Win/Win1").gameObject.SetActive(false);
                ShowArea.transform.SetParent(curTrans.Find("Win"), true);
                SetBtnActive(false);
                ShowArea.transform.SetAsFirstSibling();
                ShowArea.transform.localScale = Vector3.zero;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                ShowArea.transform.DOScale(1, 0.25f).OnComplete(() =>
                {
                    curTrans.Find("Win/Win1").gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(curTrans.Find("Win/Win1").gameObject, "animation_wow", false);
                }).SetEase(Ease.OutBack);
                Mono.StartCoroutine(Wait(1.0f, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                }));
                SpineManager.instance.DoAnimation(curTrans.Find("Win").gameObject, "animation", false, () =>
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("Win").gameObject, "idle", true);
                });
            });
            curTrans.Find("Win").gameObject.SetActive(false);
        }

        void YuMaoColliderEnter(Collision2D col, int index)
        {
            if (col.gameObject.name == "CollHead")
            {
              
               ColVal[index - 13] += 1;
            }
            else if(col.gameObject.name == "CollYM" + (index - 13))
            {
                ColVal[index - 13] += 2;
            }
        }

        void YuMaoColliderExit(Collision2D col, int index)
        {
            if (col.gameObject.name == "CollYM" + (index - 13))
            {
                ColVal[index - 13] -= 2;
            }
            else if(col.gameObject.name == "CollHead")
            {
                ColVal[index - 13] -= 1;
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

        void FixedUpdate()
        {
            if(IsPressDiBanUI)
            {
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                CurSelectObj.transform.localPosition += (Input.mousePosition - PreMousePostion) * scaleX;
                PreMousePostion = Input.mousePosition;
                TileTime = TileTime - Time.deltaTime;
                if(TileTime <= 0)
                {
                    ShowTile();
                    TileTime = 3;
                }
            }
            else if(CurPressItemDownIndex > 0)
            {
                Transform trans4 = ShowArea.transform.Find("Tile" + CurPressItemDownIndex).GetChild(0);
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                trans4.localPosition += (Input.mousePosition - PreMousePostion) * scaleX;
                PreMousePostion = Input.mousePosition;
                TileTime = TileTime - Time.deltaTime;
            }
        }

        void ShowTile()
        {
            int index = NameSwitchNum(CurSelectObj.name);
            if(index > 12)
            {
                return;
            }
            ShowArea.transform.Find("Tile" + index).GetComponent<SkeletonGraphic>().color = Color.white;
            SpineManager.instance.DoAnimation(ShowArea.transform.Find("Tile" + index).gameObject, TileAnimArr[index - 1], false, ()=>
            {
                ShowArea.transform.Find("Tile" + index).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0.01f);
            });
            for(int i = 0; i < ShowArea.transform.Find("Tile" + index).childCount; i++)
            {
                ShowArea.transform.Find("Tile" + index).GetChild(i).gameObject.SetActive(false);
            }
        }

        /*
        IEnumerator MA(SkeletonGraphic sg)
        {
            float add = 1;
            while(true)
            {
                if(sg.color.a >= 1.3)
                {
                    add = -1;
                    
                }
                else if(sg.color.a <= 0.4f)
                {
                    add = 1;
                }
                sg.color = new Color(1, 1, 1, sg.color.a + add * Time.deltaTime);
                yield return null;
            }
        }
        */

        void OnPressShowItemDown(int index)
        {
            if(index > 12)
            {
                PreItemPostion = ShowArea.transform.Find("Tile" + index).GetChild(0).localPosition;
                PreMousePostion = Input.mousePosition;
                CurPressItemDownIndex = index;
            }
        }

        void OnPressShowItemUp(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            //ShowArea.transform.Find("Tile" + index).GetChild(0).gameObject.SetActive(false);
            //ShowArea.transform.Find("Tile" + index).GetChild(1).gameObject.SetActive(true);
            //CurSelectObj = ShowArea.transform.Find("Tile" + index).gameObject;
            //RollPlane.SetActive(true);
            if (ColVal[index - 13] != 2)
            {
                SetBtnActive(false);
                Debug.Log("xx1");
                ShowArea.transform.Find("Tile" + index).GetChild(0).DOLocalMove(PreItemPostion, 0.25f, false).OnComplete(() =>
                {
                    Debug.Log("xx2");
                    SetBtnActive(true);
                }); 
            }
            CurPressItemDownIndex = -1;
        }

        void OnUIItemPointDown(int Index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            IsPressDiBanUI = true;
            Diban.transform.Find("Item" + Index).GetChild(0).gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Diban.transform.Find("Item" + Index).GetChild(0).gameObject, ItemTypeAnimArr3[Index - 1], false);
            PreMousePostion = Input.mousePosition;
            CurSelectObj = Diban.transform.Find("Item" + Index).gameObject;
            TileTime = 2;
        }

        void OnUIItemPointUp(int Index)
        {
            IsPressDiBanUI = false;
            TileTime = 2;
            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);
            bool hit = false;
            if (Index < 13)
            {
                for(int i = 0; i < results.Count; i++)
                {
                    Debug.Log("results[i]: " + results[i].gameObject.name);
                    if(results[i].gameObject.name.Contains("TileCheck"))
                    {
                        int idx = NameSwitchNum(results[i].gameObject.name);
                        if (idx == NameSwitchNum(CurSelectObj.name))
                        {
                       
                            ShowArea.transform.Find("Tile" + idx).GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 0.01f);
                            //if(idx > 12)
                            //{
                                //ShowArea.transform.Find("Tile" + idx).GetChild(0).gameObject.SetActive(false);
                                //ShowArea.transform.Find("Tile" + idx).GetChild(1).gameObject.SetActive(true);
                                //RollPlane.gameObject.SetActive(true);
                                //SpineManager.instance.DoAnimation(RollPlane.transform.Find("RollRight").gameObject, "xz", false);
                                //SpineManager.instance.DoAnimation(RollPlane.transform.Find("RollLeft").gameObject, "xz1", false);
                            //}
                            //else
                            //{
                                ShowArea.transform.Find("Tile" + idx).GetChild(0).gameObject.SetActive(true);
                            //}
                            CurSelectObj.gameObject.SetActive(false);
                            if(CheckPageOver())
                            {
                                if(CurPage < 2)
                                {
                                    SwitchPage(CurPage + 1);
                                    //Mono.StopCoroutine(cr);
                                    //if(CurPage == 1)
                                    //{
                                        //curGo.transform.Find("TileJY1").gameObject.SetActive(false);
                                        //curGo.transform.Find("TileJY2").gameObject.SetActive(true);
                                        //SpineManager.instance.DoAnimation(curGo.transform.Find("TileJY2").gameObject, "animation1", true);
                                        //cr = Mono.StartCoroutine(MA(curGo.transform.Find("TileJY2").GetComponent<SkeletonGraphic>()));
                                    //}
                                    //else
                                    //{
                                        //curGo.transform.Find("TileJY1").gameObject.SetActive(false);
                                        //curGo.transform.Find("TileJY2").gameObject.SetActive(false);
                                    //}
                                }
                            }
                            hit = true;
                            break;
                        }
                    }
                }
            }
            else
            {
                if(ColVal[Index - 13] == 2)
                {
                    ColVal[Index - 13] = 0;
                    hit = true;
                    CurSelectObj.gameObject.SetActive(false);
                    Transform trans3 = ShowArea.transform.Find("Tile" + Index).GetChild(0);
                    ShowArea.transform.Find("Tile" + Index).GetChild(1).gameObject.SetActive(false);
                    trans3.gameObject.SetActive(true);
                    //trans3.SetParent(Diban.transform);
                    //trans3.localPosition = CurSelectObj.transform.localPosition;
                    //trans3.transform.SetParent(ShowArea.transform.Find("Tile" + Index), true);
                    //trans3.transform.SetAsFirstSibling();

                    if (CheckPageOver())
                    {
                        ComplateBtn.transform.DOLocalMove(new Vector3(700, -800, 0), 0.25f).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(ComplateBtn, "animation", true);
                        });
                        /*
                        WinObj.SetActive(true);
                        SpineManager.instance.DoAnimation(WinObj.transform.GetChild(0).gameObject, "animation", false, () =>
                        {
                            SpineManager.instance.DoAnimation(WinObj.transform.GetChild(0).gameObject, "idle", true);
                        });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                        Mono.StartCoroutine(Wait(1.0f, () => { SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false); }));
                        */
                    }
                }
            }

            if (hit)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                int idx = NameSwitchNum(CurSelectObj.name);
                SetBtnActive(false);
                CurSelectObj.transform.DOLocalMove(OrginUIPos[idx - 1], 0.25f).OnComplete(() =>
                {
                    SetBtnActive(true);
                    Diban.transform.Find("Item" + idx).GetChild(0).gameObject.SetActive(false);
                });
            }
        }

        bool CheckPageOver()
        {
            bool over = true;
            for (int i = 0; i < 16; i++)
            {
                if(Diban.transform.Find("Item" + (i + 1)).gameObject.activeSelf)
                {
                    over = false;
                    break;
                }
            }
            return over;
        }

        void SetBtnActive(bool active)
        {
            for(int i = 0; i < 16; i++)
            {
                Diban.transform.Find("Item" + (i + 1)).GetComponent<Image>().raycastTarget = active;
            }
            for(int i = 12; i < ShowArea.transform.childCount; i++)
            {
                ShowArea.transform.Find("Tile" + (i + 1)).GetChild(0).GetComponent<Image>().raycastTarget = active;
            }
        }

        void Rotate(bool Left)
        {
            int index = NameSwitchNum(CurSelectObj.name);
            Vector3 ag = ShowArea.transform.Find("Tile" + index).GetChild(0).localRotation.eulerAngles;
            if (ag.z > 180 && ag.z < 360)
            {
                ag.z = ag.z - 360;
            }

            if (Left)
            {
                ag.z += 15;
                if(index > 12)
                { 
                    ShowArea.transform.Find("Tile" + index).GetChild(1).DOLocalRotate(new Vector3(0, 0, ag.z), 0.2f);
                }
                ShowArea.transform.Find("Tile" + index).GetChild(0).DOLocalRotate(new Vector3(0, 0, ag.z), 0.2f);
            }
            else
            {
                ag.z -= 15;
                if (index > 12)
                {
                    ShowArea.transform.Find("Tile" + index).GetChild(1).DOLocalRotate(new Vector3(0, 0, ag.z), 0.2f);
                }
                ShowArea.transform.Find("Tile" + index).GetChild(0).DOLocalRotate(new Vector3(0, 0, ag.z), 0.2f);
            }
        }

        void SwitchPage(int Page)
        {
            CurPage = Page;
            Debug.Log("CurPage: " + CurPage);
            int[] bl = PageArr[CurPage];
            for (int i = 1; i < 17; i++)
            {
                Diban.transform.Find("Item" + i).gameObject.SetActive(false);
            }

            Diban.transform.Find("ItemSpine").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(Diban.transform.Find("ItemSpine").gameObject, ItemTypeAnimArr1[CurPage], false, () =>
            {
                Diban.transform.Find("ItemSpine").gameObject.SetActive(false);
                for (int i = 0; i < bl.Length; i++)
                {
                    Diban.transform.Find("Item" + bl[i]).gameObject.SetActive(true);
                    Diban.transform.Find("Item" + bl[i]).GetChild(0).gameObject.SetActive(false);
                }
            });
            SpineManager.instance.DoAnimation(Diban.transform.Find("AnNiu").gameObject, ItemTypeAnimArr2[CurPage], false);
        }

        int NameSwitchNum(string name)
        {
            if(name.Contains("16"))
            {
                return 16;
            }
            else if(name.Contains("15"))
            {
                return 15;
            }
            else if (name.Contains("14"))
            {
                return 14;
            }
            else if (name.Contains("13"))
            {
                return 13;
            }
            else if (name.Contains("12"))
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
            return -1;
        }
    }
}
