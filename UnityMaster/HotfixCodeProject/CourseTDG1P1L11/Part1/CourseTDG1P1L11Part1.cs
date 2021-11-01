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
    public class CourseTDG1P1L11Part1
    {
        GameObject curGo;
        GameObject DiBan;
        GameObject CurSelectObj;
        int[] Level;
        int[] JianMaoLevel;
        string[] AnimArr1;
        string[] AnimArr2;
        string[] AnimArr3;
        int[] SheepNeedfoods;
        int[] EqureFoodNum;
        int CurPage;
        bool IsPressDown;
        Vector3 PreMousePos;
        MonoBehaviour mono;
        GameObject JianMaoSheep;
        GameObject WinObj;
        bool IsGameStart;
        
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Level = new int[] { 0, 0, 0, 0 };
            JianMaoLevel = new int[] { 0, 0, 0, 0 };
            AnimArr1 = new string[]{"1+2", "1_1", "1_2", "1_3", "1_idle", "2+3", "2_1", "2_2", "2_3", "2_idle", "", "3_1", "3_2", "3_3", "3_idle" };
            AnimArr2 = new string[] { "animationt_1", "animationt_2", "animationt_3", "animationt_4", "animationt_5", "animationt_t1", "animationt_t2",
                                      "animationt_t3", "animationt_t4", "animationt_t5", "animation3_2"};
            AnimArr3 = new string[] { "", "", "1", "2", "3", "4", "5", "", "6", "7", "8" };
            SheepNeedfoods = new int[] {8, 9, 5, 4, 10, 3, 2, 6};
            EqureFoodNum = new int[] { 3, 2, 5, 4, 6, 8, 9, 10 };
            DiBan = curTrans.Find("DB").gameObject;
            SwitchPage(0);
            CurSelectObj = null;
            JianMaoSheep = null;
            IsPressDown = false;
            IsGameStart = false;
            mono = curGo.GetComponent<MonoBehaviour>();
            WinObj = curTrans.Find("Over").gameObject;
            WinObj.SetActive(false);
            curTrans.Find("bg").gameObject.SetActive(true);
            curTrans.Find("bg1").gameObject.SetActive(false);
            WinObj.transform.Find("End").GetComponent<SkeletonGraphic>().AnimationState.Data.DefaultMix = 0;

            for (int i = 0; i < 4; i++)
            {
                GameObject go1 = curGo.transform.Find("Sheep" + i).GetChild(0).GetChild(0).gameObject;
                curGo.transform.Find("Sheep" + i).GetChild(0).GetComponent<SkeletonGraphic>().color = Color.white;
                go1.transform.parent.gameObject.SetActive(true);
                go1.SetActive(false);
                SpineManager.instance.DoAnimation(go1.transform.parent.gameObject, "1_idle", true);
            }

            for (int i = 0; i < 11; i++)
            {
                if (i == 10)
                {
                    ILObject3DAction i3d = DiBan.transform.Find("ItemRect11/JD").GetComponent<ILObject3DAction>();
                    i3d.OnPointDownLua = OnItemUIPressDown;
                    i3d.OnPointUpLua = OnItemUIPressUp;
                    
                }
                else
                {
                    ILObject3DAction i3d = DiBan.transform.Find("ItemRect" + (i + 1)).GetComponent<ILObject3DAction>();
                    i3d.OnPointDownLua = OnItemUIPressDown;
                    i3d.OnPointUpLua = OnItemUIPressUp;
                }
            }


            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            SoundManager.instance.Speaking(curTrans.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0, null, ()=>
            {
                SetbtnActive(true);
                IsGameStart = true;
                mono.StartCoroutine(TipPop());
            });
        }

        void EnterJianMaoArea(Collision2D col, int index)
        {
            Debug.Log("col.gameObject.name: " + col.gameObject.name);
            if(col.gameObject.name.Contains("Sheep"))
            {
                JianMaoSheep = col.gameObject;
            }
        }

        void ExitJianMaoArea(Collision2D col, int index)
        {
            if (col.gameObject.name.Contains("Sheep"))
            {
                JianMaoSheep = null;
            }
        }

        void SetbtnActive(bool active)
        {
            for(int i = 0; i < 11; i++)
            {
                if(i == 10)
                {
                    DiBan.transform.Find("ItemRect11/JD").GetComponent<Image>().raycastTarget = active;
                }
                else
                { 
                    DiBan.transform.Find("ItemRect" + (i + 1)).GetComponent<Image>().raycastTarget = active;
                }
            }
        }

        void FixedUpdate()
        {
            if(IsPressDown)
            {
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                CurSelectObj.transform.localPosition += (Input.mousePosition - PreMousePos) * scaleX;
                PreMousePos = Input.mousePosition;
            }

        }

        void OnItemUIPressDown(int index)
        {
            Debug.Log("OnItemUIPressDown index: " + index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
            IsPressDown = true;
            PreMousePos = Input.mousePosition;
            GameObject go = DiBan.transform.Find("ItemRect" + index).gameObject;
            SpineManager.instance.DoAnimation(go.transform.GetChild(0).gameObject, AnimArr2[index - 1], true);
            Vector3 startpos;
            if (index == 11)
            {
                go = DiBan.transform.Find("ItemRect11/JD").gameObject;
                go.GetComponent<Image>().color = Color.white;
                go.transform.GetChild(0).gameObject.SetActive(false);
                CurSelectObj = GameObject.Instantiate(go);
                startpos = new Vector3(-3, -345, 0);
                ILObject3DAction i3d = CurSelectObj.GetComponent<ILObject3DAction>();
                i3d.OnCollisionEnter2DLua = EnterJianMaoArea;
                i3d.OnCollisionExit2DLua = ExitJianMaoArea;
            }
            else
            {
                CurSelectObj = GameObject.Instantiate(go);
                startpos = go.transform.localPosition;
                GameObject.Destroy(CurSelectObj.GetComponent<ILObject3DAction>());
            }
            CurSelectObj.transform.SetParent(DiBan.transform, false);
            CurSelectObj.transform.localScale = Vector3.one;
            CurSelectObj.transform.localPosition = startpos;
            SpineManager.instance.DoAnimation(CurSelectObj.transform.GetChild(0).gameObject, AnimArr2[index - 1], true);
            if(CurPage < 2)
            {
                for (int i = CurPage * 5; i < CurPage * 5 + 5; i++)
                {
                    if(index != i + 1)
                    {
                        string name = string.Format("animation{0}_{1}", i / 5 + 1, i % 5 + 1);
                        DiBan.transform.Find("ItemRect" + (i + 1)).gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(DiBan.transform.Find("ItemRect" + (i + 1)).GetChild(0).gameObject, name, false);
                    }
                }
            }
        }

        void OnItemUIPressUp(int index)
        {
            IsPressDown = false;
            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);

            int idx = -1;
            for(int i = 0; i < results.Count; i++)
            {
                if(results[i].gameObject.name.Contains("Sheep"))
                {
                    idx = SwitchNameToIndex(results[i].gameObject.name);
                    break;
                }
            }

            SetbtnActive(false);
            if (CurPage < 2)
            {
                if (idx == -1)
                {
                    CurSelectObj.transform.DOLocalMove(DiBan.transform.Find("ItemRect" + index).localPosition, 0.25f).OnComplete(()=>
                    {
                        SetbtnActive(true);
                        GameObject.Destroy(CurSelectObj);
                    });
                }
                else
                {
                    GameObject go1 = curGo.transform.Find("Sheep" + idx).GetChild(0).GetChild(0).gameObject;
                    go1.SetActive(false);
                    
                    mono.StartCoroutine(Fade(CurSelectObj.transform.GetChild(0).gameObject, ()=>
                    {
                        GameObject.Destroy(CurSelectObj);
                    }));
                    if(Level[idx] >= 2)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 12, false);
                        CurSelectObj.transform.DOLocalMove(DiBan.transform.Find("ItemRect" + index).localPosition, 0.25f).OnComplete(() =>
                        {
                            GameObject.Destroy(CurSelectObj);
                        });
                        SetbtnActive(true);
                        return;
                    }
                    if(index == EqureFoodNum[Level[idx] * 4 + idx])
                    {
                        int id = Level[idx] * 5;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                        SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[id], false, () =>
                        {
                            Level[idx] += 1;
                            id = Level[idx] * 5;
                            SetbtnActive(true);
                            SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[id + 4], true);
                            CheckSwicthPage();
                        });
                    }
                    else if(index == 1)
                    {
                        int id = Level[idx] + 1;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, false);
                        mono.StartCoroutine(Wait(1.5f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 13, false);
                        }));
                        SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[id], false, () =>
                        {
                            SetbtnActive(true);
                            SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[Level[idx] * 5 + 4], true);
                            if (CurPage == Level[idx])
                            {
                                go1.SetActive(true);
                                int j = SheepNeedfoods[Level[idx] * 4 + idx];
                                SpineManager.instance.DoAnimation(go1, AnimArr3[j], true);
                            }
                        });
                    }
                    else if (index == 7)
                    {
                        int id = Level[idx] + 11;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, false);
                        mono.StartCoroutine(Wait(1, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 11, false);
                        }));
                        mono.StartCoroutine(Wait(3, () =>
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        }));
                        SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[id], false, () =>
                        {
                            SetbtnActive(true);
                            SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[Level[idx] * 5 + 4], true);
                            if (CurPage == Level[idx])
                            {
                                go1.SetActive(true);
                                int j = SheepNeedfoods[Level[idx] * 4 + idx];
                                SpineManager.instance.DoAnimation(go1, AnimArr3[j], true);
                            }
                        });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 12, false);
                        int id = Level[idx] + 6;
                        SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[id], false, () =>
                        {
                            SetbtnActive(true);
                            SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, AnimArr1[Level[idx] * 5 + 4], true);
                            if (CurPage == Level[idx])
                            {
                                go1.SetActive(true);
                                int j = SheepNeedfoods[Level[idx] * 4 + idx];
                                SpineManager.instance.DoAnimation(go1, AnimArr3[j], true);
                            }
                        });
                    }
                }
            }
            else
            {
                idx = -1;
                if (JianMaoSheep)
                {
                    idx = SwitchNameToIndex(JianMaoSheep.name);
                }
                if (JianMaoSheep && JianMaoLevel[idx] < 1 && Input.mousePosition.y < 580)
                {
                    Debug.Log("Input.mousePosition: " + Input.mousePosition);
                    CurSelectObj.transform.GetChild(0).gameObject.SetActive(true);
                    CurSelectObj.GetComponent<Image>().color = new Color(1, 1, 1, 0.01f);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                    SpineManager.instance.DoAnimation(CurSelectObj.transform.GetChild(0).gameObject, "jiandao", false, () =>
                    {
                        GameObject.Destroy(CurSelectObj);
                        if (CheckEnd())
                        {
                            DiBan.transform.Find("Kuang2").gameObject.SetActive(false);
                            DiBan.transform.Find("ItemRect11").gameObject.SetActive(false);
                            SoundManager.instance.Speaking(curGo.transform.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 3, null, ()=>
                            {
                                curGo.transform.Find("npc").gameObject.SetActive(true);
                                SpineManager.instance.DoAnimation(curGo.transform.Find("npc").gameObject, "breath");
                                mono.StartCoroutine(StartEndAnim());
                            });
                        }
                        else
                        { 
                            SetbtnActive(true);
                        }
                    });
                    SpineManager.instance.DoAnimation(curGo.transform.Find("Sheep" + idx).GetChild(0).gameObject, "animation_" + (JianMaoLevel[idx] + 1), false);
                    JianMaoLevel[idx] += 1;
                }
                else
                {
                    CurSelectObj.transform.DOLocalMove(new Vector3(-5, -345, 0), 0.25f).OnComplete(() =>
                    {
                        SetbtnActive(true);
                        GameObject.Destroy(CurSelectObj);
                    });
                }
            }
        }

        bool CheckEnd()
        {
            bool end = true;
            for(int i = 0; i < 4; i++)
            {
                if(JianMaoLevel[i] < 1)
                {
                    end = false;
                    break;
                }
            }
            return end;
        }

        void CheckSwicthPage()
        {
            bool stp = true;
            for(int i = 0; i < 4; i++)
            {
                if(Level[i] < CurPage + 1)
                {
                    stp = false;
                    break;
                }
            }
            if(stp)
            {
                if(CurPage == 0)
                {
                    SoundManager.instance.Speaking(curGo.transform.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        SwitchPage(CurPage + 1);
                    });
                }
                else if(CurPage == 1)
                {
                    SoundManager.instance.Speaking(curGo.transform.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 2, null, () =>
                    {
                        SwitchPage(CurPage + 1);
                    });
                }
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

        IEnumerator StartEndAnim()
        {
           
            yield return new WaitForSeconds(1);
            for (int i = 0; i < 4; i++)
            {
                yield return mono.StartCoroutine(Fade1(curGo.transform.Find("Sheep" + i).GetChild(0).gameObject, null));
            }
            yield return new WaitForSeconds(0.2f);
            WinObj.transform.Find("End").localPosition = new Vector3(7, -545, 0);
            WinObj.transform.Find("End").localScale = Vector3.one;
            WinObj.SetActive(true);
            WinObj.transform.GetChild(0).gameObject.SetActive(false);
            WinObj.transform.GetChild(1).gameObject.SetActive(true);
            curGo.transform.Find("bg").gameObject.SetActive(false);
            curGo.transform.Find("bg1").gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(WinObj.transform.Find("End").gameObject, "1", false, () =>
            {
                SpineManager.instance.DoAnimation(WinObj.transform.Find("End").gameObject, "1_idle", true);
            });
            SoundManager.instance.Speaking(curGo.transform.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 4, null, ()=>
            {
                curGo.transform.Find("npc").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(curGo.transform.Find("npc").gameObject, "breath");
            });
            yield return new WaitForSeconds(9);
            SoundManager.instance.Speaking(curGo.transform.Find("npc").gameObject, "talk", SoundManager.SoundType.VOICE, 5, null, null);
            yield return new WaitForSeconds(3.5f);
            WinObj.transform.Find("Mask").gameObject.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
            WinObj.transform.Find("End").localPosition = new Vector3(7, -356, 0);
            WinObj.transform.Find("End").localScale = Vector3.one * 0.7f;
            SpineManager.instance.DoAnimation(WinObj.transform.Find("End").gameObject, "animation", false, () =>
            {
                
                SpineManager.instance.DoAnimation(WinObj.transform.Find("End").gameObject, "animation_idle", true);
            });
            yield return new WaitForSeconds(1f);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
        }

        IEnumerator Fade(GameObject o, Action act)
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
            if(act != null)
            {
                act.Invoke();
            }
        }

        IEnumerator Fade1(GameObject o, Action act)
        {
            float a = o.GetComponent<SkeletonGraphic>().color.a;
            while (a > 0)
            {
                a -= Time.deltaTime * 5;
                o.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, a);
                yield return null;
            }
            yield return null;
            o.SetActive(false);
            o.GetComponent<SkeletonGraphic>().color = new Color(1, 1, 1, 1);
            if (act != null)
            {
                act.Invoke();
            }
        }

        IEnumerator TipPop()
        {
            for (int i = 0; i < 4; i++)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 14, false);
                int j = SheepNeedfoods[CurPage * 4 + i];
                GameObject go1 = curGo.transform.Find("Sheep" + i).GetChild(0).GetChild(0).gameObject;
                go1.SetActive(true);
                SpineManager.instance.DoAnimation(go1, AnimArr3[j], true);
                yield return new WaitForSeconds(0.25f);
            }
        }

        void SwitchPage(int page)
        {
            CurPage = page;
            DiBan.transform.Find("KuangAnim").gameObject.SetActive(true);
            DiBan.transform.Find("Kuang1").gameObject.SetActive(false);
            DiBan.transform.Find("Kuang2").gameObject.SetActive(false);
            for(int i = 0; i < 11; i++)
            {
                DiBan.transform.Find("ItemRect" + (i + 1)).gameObject.SetActive(false);
            }
            
            //DiBan.transform.Find("KuangAnim").GetComponent<SkeletonGraphic>().AnimationState.ClearTrack(0);
            DiBan.transform.Find("KuangAnim").GetComponent<SkeletonGraphic>().AnimationState.Data.DefaultMix = 0;
            if (page > 1)
            {
                SpineManager.instance.DoAnimation(DiBan.transform.Find("KuangAnim").gameObject, "animation" + (CurPage + 1), false, () =>
                {
                    DiBan.transform.Find("KuangAnim").gameObject.SetActive(false);
                    DiBan.transform.Find("Kuang2").gameObject.SetActive(true);
                    DiBan.transform.Find("ItemRect11").gameObject.SetActive(true);
                    DiBan.transform.Find("ItemRect11/JD").GetComponent<Image>().color = Color.white;
                    DiBan.transform.Find("ItemRect11/JD/JDSP").gameObject.SetActive(false);
                    SpineManager.instance.DoAnimation(DiBan.transform.Find("ItemRect11").GetChild(0).gameObject, "animation3_1", false);
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(DiBan.transform.Find("KuangAnim").gameObject, "animation" + (CurPage + 1), false, () =>
                {
                    DiBan.transform.Find("KuangAnim").gameObject.SetActive(false);
                    DiBan.transform.Find("Kuang1").gameObject.SetActive(true);
                    for(int i = CurPage * 5; i < CurPage * 5 + 5; i++)
                    {
                        string name = string.Format("animation{0}_{1}", i / 5 + 1, i % 5 + 1);
                        DiBan.transform.Find("ItemRect" + (i + 1)).gameObject.SetActive(true);
                        SpineManager.instance.DoAnimation(DiBan.transform.Find("ItemRect" + (i + 1)).GetChild(0).gameObject, name, false);
                    }
                });
                if (IsGameStart)
                {
                    mono.StartCoroutine(TipPop());
                }
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
    }
}
