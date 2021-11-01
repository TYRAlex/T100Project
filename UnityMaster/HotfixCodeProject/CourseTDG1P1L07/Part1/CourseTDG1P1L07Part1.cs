using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace ILFramework.HotClass
{
    public class CourseTDG1P1L07Part1
    {
        GameObject curGo;
        int FruitIndex;
        int TotalIndex;
        string[] FruitNameArr;
        string[] FruitHLSpineArr;
        string[] FruitAnimArr;
        float[] ScaleArr;
        int[] FruitArr;

        GameObject FruitBornPlane;
        GameObject ChooseIconObj;
        GameObject Panel_Len;
        GameObject Panel_Nuan;
        GameObject StarSpine;
        GameObject EndSpine;
        GameObject Dingding;
        GameObject SePan;
        MonoBehaviour mono;


        void Start(object o)
        {
            FruitNameArr = new string[]{ "ui_apple_1", "ui_blueberry_1", "ui_grape_1", "ui_watermelon_1",
                                         "ui_oranhe_1", "ui_peach_1", "ui_lemon_1", "ui_strawberry_1"};
            FruitHLSpineArr = new string[]{ "ui_apple", "ui_blueberry", "ui_grape", "ui_watermelon",
                                            "ui_orange", "ui_peach", "ui_lemon", "ui_strawberry"};
            FruitAnimArr = new string[]{ "apple", "blueberry", "grape", "watermelon",
                                         "orange", "peach", "lemon", "strawberry"};
            ScaleArr = new float[]{ 1.2f, 1.2f, 1.8f, 2.5f, 1.2f, 1.2f, 1.2f, 1.2f};
            FruitArr = new int[] {0, 1, 2, 3, 4, 5, 6, 7};
            for(int i = 0; i < 8; i++)
            {
                int k = UnityEngine.Random.Range(0, 8);
                int temp = FruitArr[i];
                FruitArr[i] = FruitArr[k];
                FruitArr[k] = temp;
            }

            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            FruitIndex = 0;
            TotalIndex = 0;
            FruitBornPlane = curTrans.Find("bg/FruitPlane").gameObject;
            ChooseIconObj = curTrans.Find("bg/FruitPlane/dot/FruitImg").gameObject;
            Panel_Len = curTrans.Find("bg/Panel_Len").gameObject;
            Panel_Nuan = curTrans.Find("bg/Panel_Nuan").gameObject;
            StarSpine = curTrans.Find("bg/Star").gameObject;
            EndSpine = curTrans.Find("bg/End").gameObject;
            Dingding = curTrans.Find("npc").gameObject;
            SePan = curTrans.Find("SP").gameObject;
            SePan.SetActive(true);
            SpineManager.instance.DoAnimation(SePan.transform.Find("spSpine").gameObject, "animation", false);
            mono = curGo.GetComponent<MonoBehaviour>();
            
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            FruitBornPlane.transform.Find("dot/FruitImg").localScale = Vector3.zero;

            SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                mono.StartCoroutine(SpeakStart(1));
            });


            EndSpine.SetActive(false);
            for (int i = 0; i < Panel_Len.transform.childCount; i++)
            {
                Panel_Len.transform.GetChild(i).gameObject.SetActive(false);
            }
            for (int i = 0; i < Panel_Nuan.transform.childCount; i++)
            {
                Panel_Nuan.transform.GetChild(i).gameObject.SetActive(false);
            }
            SoundManager.instance.ShowVoiceBtn(false);
        }

        IEnumerator SpeakStart(int index)
        {
            if(index > 2)
            {
                BornFruit();
            }
            else
            {
                if (index == 2)
                {
                    SePan.SetActive(false);
                    FruitBornPlane.transform.DOLocalMove(new Vector3(0, 350, 0), 0.25f).SetEase(Ease.InOutBack).OnComplete(() =>
                    {
                        ILObject3DAction UIeventSc = ChooseIconObj.GetComponent<ILObject3DAction>();
                        UIeventSc.OnPointDownLua = PointerDown;
                        UIeventSc.OnPointUpLua = PointerUp; ;
                    });
                }
                Dingding.SetActive(true);
                SpineManager.instance.DoAnimation(Dingding, "breath", true);
                yield return new WaitForSeconds(1);
                SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, index, null, () =>
                {
                    mono.StartCoroutine(SpeakStart(index + 1));
                });
            }
        }

        IEnumerator SpeakEnd(int index)
        {
            if (index > 10)
            {
                SpineManager.instance.DoAnimation(Panel_Len, "lengpan_1", true);
            }
            else
            {
                Dingding.SetActive(true);
                SpineManager.instance.DoAnimation(Dingding, "breath", true);
                yield return new WaitForSeconds(1);
                
                SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, index, null, () =>
                {
                    mono.StartCoroutine(SpeakEnd(index + 1));
                });
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

        void BornFruit()
        {
            if(TotalIndex >= 8)
            {
                EndSpine.SetActive(true);
                for (int i = 0; i < Panel_Len.transform.childCount; i++)
                {
                    Panel_Len.transform.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < Panel_Nuan.transform.childCount; i++)
                {
                    Panel_Nuan.transform.GetChild(i).gameObject.SetActive(false);
                }
                SpineManager.instance.DoAnimation(EndSpine, "end", false);
                SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, 8, null, () =>
                {
                    mono.StartCoroutine(SpeakEnd(9));
                    SpineManager.instance.DoAnimation(Panel_Nuan, "nuanpan_1", true);
                    SpineManager.instance.DoAnimation(Panel_Len, "lengpan_3", true);
                });
                SpineManager.instance.DoAnimation(Panel_Len, "lengpan_1", true);
            }
            else
            {
                FruitIndex = FruitArr[TotalIndex];
                GameObject img = FruitBornPlane.transform.Find("dot/FruitImg").gameObject;
                img.transform.localPosition = Vector3.zero;
                FruitBornPlane.transform.Find("dot/FruitImg").DOScale(1, 0.25f).SetEase(Ease.InOutBack);
                img.GetComponent<Image>().sprite = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseTDG1P1L07Part1"), FruitHLSpineArr[FruitIndex]);
                img.GetComponent<Image>().SetNativeSize();
                ChooseIconObj.GetComponent<Image>().raycastTarget = true;
            }
        }

        void RightEffect()
        {
            if (FruitIndex == 0)
            {
                Panel_Len.transform.Find("appleHL").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Panel_Len.transform.Find("appleHL").gameObject, "apple_1", false, () =>
                {
                    Panel_Len.transform.Find("appleHL").gameObject.SetActive(false);
                });
            }
            else if (FruitIndex == 1)
            {
                SpineManager.instance.DoAnimation(Panel_Len, "lengpan_2", false);
            }
            else if (FruitIndex == 2 || FruitIndex == 3 || FruitIndex == 6 || FruitIndex == 7)
            {
                SpineManager.instance.DoAnimation(StarSpine, "star", false);
            }
            else if (FruitIndex == 5)
            {
                SpineManager.instance.DoAnimation(Panel_Nuan, "nuanpan_2", false);
            }
            else if (FruitIndex == 4)
            {
                Panel_Nuan.transform.Find("orangeHL").gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(Panel_Nuan.transform.Find("orangeHL").gameObject, "orange_1", false, () =>
                {
                    Panel_Nuan.transform.Find("orangeHL").gameObject.SetActive(false);
                });
            }

            TotalIndex = TotalIndex + 1;
        }

        void PointerDown(int index)
        {

        }

        void PointerUp(int index)
        {
            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);
            ChooseIconObj.GetComponent<Image>().raycastTarget = false;

            bool match = false;
            for (int i = 0; i < results.Count; i++)
            {
                Debug.Log("results[i].gameObject.name: " + results[i].gameObject.name);
                if (results[i].gameObject.name == "LenColl")
                {
                    if (FruitIndex < 4)
                    {
                        match = true;
                    }
                }
                else if (results[i].gameObject.name == "NuanColl")
                {
                    if (FruitIndex >= 4)
                    {
                        match = true;
                    }
                }
            }

            if (match)
            {
                ChooseIconObj.transform.DOScale(ScaleArr[FruitIndex], 0.2f);
                ChooseIconObj.transform.DOLocalMove(FruitBornPlane.transform.Find("dot/Dot" + (FruitIndex + 1)).localPosition, 0.2f).OnComplete(() =>
                {
                    FruitBornPlane.transform.Find("dot/FruitImg").localScale = Vector3.zero;
                    GameObject obj = null;
                    if (FruitIndex < 4)
                    {
                        obj = Panel_Len.transform.Find((FruitIndex + 1).ToString()).gameObject;
                    }
                    else
                    {
                        obj = Panel_Nuan.transform.Find((FruitIndex + 1).ToString()).gameObject;
                    }
                    obj.SetActive(true);
                    SpineManager.instance.DoAnimation(obj, FruitAnimArr[FruitIndex], false, () =>
                     {
                         RightEffect();
                     });
                });
                if (FruitIndex == 0 || FruitIndex == 3 || FruitIndex == 4 || FruitIndex == 7)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                    mono.StartCoroutine(Wait(1.5f, ()=>
                    {
                        BornFruit();
                    }));
                }
                else if (FruitIndex == 1 || FruitIndex == 5)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                    mono.StartCoroutine(Wait(1.2f, () =>
                    {
                        BornFruit();
                    }));
                }
                else if(FruitIndex == 2 || FruitIndex == 6)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                    mono.StartCoroutine(Wait(1.0f, () =>
                    {
                        BornFruit();
                    }));
                }
            }
            else
            {
                ChooseIconObj.transform.DOLocalMove(Vector3.zero, 0.25f).OnComplete(() =>
                {
                    ChooseIconObj.GetComponent<Image>().raycastTarget = true;
                });
                if (FruitIndex == 0 || FruitIndex == 1 || FruitIndex == 4 || FruitIndex == 5)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                }
                else if (FruitIndex == 2 || FruitIndex == 3 || FruitIndex == 6 || FruitIndex == 7)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                }
            }
        }
    }
}
