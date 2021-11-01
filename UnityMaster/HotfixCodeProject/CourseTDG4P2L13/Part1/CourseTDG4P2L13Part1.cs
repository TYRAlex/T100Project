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
    public class CourseTDG4P2L13Part1
    {
        GameObject curGo;
        int CurPage;
        GameObject CurSelectObj;
        bool IsPressDown;
        Vector3 PreMousePos;
        GameObject SKA;
        GameObject SKG;
        Transform RotateCKTans;
        //Transform RotateTans;
        Transform SmallZhang;
        Transform BigZhang;
        Transform Dot1Trans;
        Transform Dot2Trans;
        Transform GuaDian1;
        Transform GuaDian2;
        MonoBehaviour Mono;

        string[] AnimNameArr1;
        string[] AnimNameArr2;
        string[] AnimNameArr3;
        int[] AnimIndex;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            AnimNameArr1 = new string[] { "ui_ronghua1", "ui_rongniao1", "ui_congcao1", "ui_rongchong1"};
            AnimNameArr2 = new string[] { "ui_ronghua", "ui_rongniao", "ui_rongcao", "ui_rongchong" };
            AnimNameArr3 = new string[] { "ronghua", "rongniao", "rongcao", "rongchong" };
            AnimIndex = new int[] {2, 4, 1, 3, 2, 3, 1, 4, 4, 1, 2, 3, 2, 4, 3, 1};
            CurSelectObj = null;
            SKA = curTrans.Find("bg/ShowArea/WaWaAnim1").gameObject;
            SKG = curTrans.Find("bg/ShowArea/WaWaAnim2").gameObject;
            BigZhang = curTrans.Find("bg/BigZhangSp"); 
            SmallZhang = curTrans.Find("bg/SmallZhangSP");

            //RotateCKTans = curTrans.Find("bg/ShowArea/WaWaAnim1/SkeletonUtility-Root/root/tiantian/tiantian3/tiantian2/head");
            GuaDian1 = curTrans.Find("bg/ShowArea/WaWaAnim1/SkeletonUtility-Root/root/tiantian/tiantian3/tiantian2/head/tiantian1/tiantian5");
            GuaDian2 = curTrans.Find("bg/ShowArea/WaWaAnim1/SkeletonUtility-Root/root/tiantian/tiantian3/tiantian2/head/tiantian1/tiantian6");
            //RotateTans = SKG.transform.Find("DPlane");
            Dot1Trans = SKG.transform.Find("dot1");
            Dot2Trans = SKG.transform.Find("dot2");
            SpineManager.instance.DoAnimation(SKA, "idle", true);
            SpineManager.instance.DoAnimation(SKG, "idle", true);
            SpineManager.instance.DoAnimation(SmallZhang.gameObject, "anniu_1", true);
            BigZhang.gameObject.SetActive(false);
            LogicManager.instance.ShowReplayBtn(false);
            CurPage = -1;
            SwitchPage(0);

            for(int i = 0; i < 4; i++)
            {
                int j = i;
                curTrans.Find("bg/DiBan/AnNiu" + i).GetComponent<Button>().onClick.RemoveAllListeners();
                curTrans.Find("bg/DiBan/AnNiu" + i).GetComponent<Button>().onClick.AddListener(() =>
                {
                    SpineManager.instance.DoAnimation(curTrans.Find("bg/DiBan/AnNiu" + j).GetChild(0).gameObject, AnimNameArr2[j], false);
                    SwitchPage(j);
                });
            }

            for (int i = 0; i < 16; i++)
            {
                Dot1Trans.Find("Item" + i).gameObject.SetActive(false);
                Dot2Trans.Find("Item" + i).gameObject.SetActive(false);
            }

            for (int i = 0; i < 16; i++)
            {
                curGo.transform.Find("bg/DiBan/ItemAN" + i).gameObject.SetActive(true);
                curGo.transform.Find("bg/DiBan/ItemAN" + i).localPosition = Vector3.right * 10000;
                string s = string.Format("ui{0}_{1}", i / 4 + 1, i % 4 + 1);
                curGo.transform.Find("bg/DiBan/ItemAN" + i).GetComponent<SkeletonGraphic>().AnimationState.Data.DefaultMix = 0;
                SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ItemAN" + i).gameObject, s, false);

                ILObject3DAction i3d = curGo.transform.Find("bg/DiBan/ItemAN" + i).GetChild(0).GetComponent<ILObject3DAction>();
                i3d.OnPointDownLua = OnUIItemDown;
                i3d.OnPointUpLua = OnUIItemUp;
            }

            curTrans.Find("bg/Bg_AN1").localPosition = new Vector3(-511, 11.43f, 0);
            curTrans.Find("bg/DiBan").localPosition = new Vector3(0, 0, 0);
            SmallZhang.localPosition = new Vector3(-780, -621, 0);
            curTrans.Find("bg/Bg_AN2").localPosition = new Vector3(409.2f, 11.43f, 0);
            curTrans.Find("bg/ShowArea").localPosition = new Vector3(0, 0, 0);
            SmallZhang.GetChild(0).GetComponent<Image>().raycastTarget = false;
            SmallZhang.GetChild(0).GetComponent<Button>().onClick.RemoveAllListeners();
            SmallZhang.GetChild(0).GetComponent<Button>().onClick.AddListener(() =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
                SpineManager.instance.DoAnimation(SmallZhang.gameObject, "anniu", false);
                curTrans.Find("bg/Bg_AN1").DOLocalMove(new Vector3(-2000, 11.43f, 0), 0.5f);
                curTrans.Find("bg/DiBan").DOLocalMove(new Vector3(-1489, 0, 0), 0.5f);
                SmallZhang.DOLocalMove(new Vector3(-2000, -621, 0), 0.5f);
                curTrans.Find("bg/Bg_AN2").DOLocalMove(new Vector3(2000, 11.43f, 0), 0.5f);
                curTrans.Find("bg/ShowArea").DOLocalMove(new Vector3(1591, 0, 0), 0.5f).OnComplete(()=>
                {
                    BigZhang.gameObject.SetActive(true);
                    SpineManager.instance.DoAnimation(BigZhang.gameObject, "animation", false, ()=>
                    {
                        LogicManager.instance.ShowReplayBtn(true);
                        LogicManager.instance.SetReplayEvent(() =>
                        {
                            Start(o);
                        });
                    });
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                    Mono.StartCoroutine(Wait(1.0f, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                    }));
                });
            });

            SoundManager.instance.BgSoundPart2();
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            if(act != null)
            {
                act.Invoke();
            }
        }

        void OnDisable()
        {
            LogicManager.instance.ShowReplayBtn(false);
        }

        void OnUIItemDown(int index)
        {
            Debug.Log("xx1");
            GameObject go = curGo.transform.Find("bg/DiBan/ItemAN" + index).gameObject;
            CurSelectObj = GameObject.Instantiate(go);
            CurSelectObj.name = "temp" + index;
            CurSelectObj.transform.SetParent(curGo.transform.Find("bg/DiBan"), false);
            CurSelectObj.transform.localScale = Vector3.one;
            CurSelectObj.transform.localPosition = go.transform.localPosition;
            CurSelectObj.transform.GetChild(0).GetComponent<Image>().raycastTarget = false;
            string s = string.Format("ui{0}_{1}", index / 4 + 1, index % 4 + 1);
            SpineManager.instance.DoAnimation(CurSelectObj, s, false);
            PreMousePos = Input.mousePosition;
            IsPressDown = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false);
        }

        void OnUIItemUp(int index)
        {
            IsPressDown = false;

            EventSystem _mEventSystem = GameObject.Find("EventSystem").GetComponent<EventSystem>();
            var mPointerEventData = new PointerEventData(_mEventSystem);
            mPointerEventData.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(mPointerEventData, results);

            GameObject dot = null;
            for(int i = 0; i < results.Count; i++)
            {
                if(results[i].gameObject.name.Contains("dot"))
                {
                    if(results[i].gameObject.name == "dot1")
                    {
                        dot = Dot1Trans.gameObject;
                    }
                    if (results[i].gameObject.name == "dot2")
                    {
                        dot = Dot2Trans.gameObject;
                    }
                    break;
                }
            }

            if (dot != null)
            {
                for(int i = 0; i < 16; i++)
                {
                    dot.transform.Find("Item" + i).gameObject.SetActive(false);
                }
                dot.transform.Find("Item" + index).gameObject.SetActive(true);

                SpineManager.instance.DoAnimation(dot.transform.Find("Item" + index).gameObject, AnimNameArr3[index / 4] + AnimIndex[index], false);
                GameObject.Destroy(CurSelectObj);
                CurSelectObj = null;

                string s = GetWaWaAnimName();
                SpineManager.instance.DoAnimation(SKA, s, false, ()=>
                {
                    SpineManager.instance.DoAnimation(SKA, "idle", true);
                });
                SpineManager.instance.DoAnimation(SKG, s, false, ()=>
                {
                    SpineManager.instance.DoAnimation(SKG, "idle", true);
                });

                if(CheckCanCompelate())
                {
                    SmallZhang.GetChild(0).GetComponent<Image>().raycastTarget = true;
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                SetBtnActive(false);
                int num = SwitchNameToIndex(CurSelectObj.name);
                CurSelectObj.transform.DOLocalMove(curGo.transform.Find("bg/DiBan/ItemAN" + num).localPosition, 0.25f).OnComplete(()=>
                {
                    SetBtnActive(true);
                    GameObject.Destroy(CurSelectObj);
                    CurSelectObj = null;
                });
            }
        }

        void SetBtnActive(bool active)
        {
            for (int i = 0; i < 16; i++)
            {
                curGo.transform.Find("bg/DiBan/ItemAN" + i).GetChild(0).GetComponent<Image>().raycastTarget = active;
            }

            for (int i = 0; i < 4; i++)
            {
                curGo.transform.Find("bg/DiBan/AnNiu" + i).GetComponent<Image>().raycastTarget = active;
            }
        }

        void SwitchPage(int Page)
        {
            if(CurPage == Page)
            {
                return;
            }
            CurPage = Page;
            curGo.transform.Find("bg/DiBan/ANConect").transform.localPosition = new Vector3(-38.52f, -551 + (3 - CurPage) * 155, 0);

            for (int i = 0; i < 16; i ++)
            {
                curGo.transform.Find("bg/DiBan/ItemAN" + i).localPosition = Vector3.right * 10000;
            }
            SpineManager.instance.DoAnimation(curGo.transform.Find("bg/DiBan/ItemAllAnim").gameObject, AnimNameArr1[CurPage], false, () =>
            {
                for (int i = CurPage * 4; i < CurPage * 4 + 4; i++)
                {
                    curGo.transform.Find("bg/DiBan/ItemAN" + i).localPosition = new Vector3(-50, -551, 0);
                }
            });
        }

        bool CheckCanCompelate()
        {
            bool b1 = false;
            bool b2 = false;
            for (int i = 0; i < 16; i++)
            {
                if(Dot1Trans.Find("Item" + i).gameObject.activeSelf == true)
                {
                    b1 = true;
                    break;
                }
            }
            if(b1)
            {
                for (int i = 0; i < 16; i++)
                {
                    if (Dot2Trans.Find("Item" + i).gameObject.activeSelf == true)
                    {
                        b2 = true;
                        break;
                    }
                }
            }

            return b1 && b2;
        }

        void FixedUpdate()
        {
           if(IsPressDown && CurSelectObj != null)
            {
                Debug.Log("hit");
                float scaleX = (curGo.GetComponent<RectTransform>().sizeDelta.x / Screen.width);
                CurSelectObj.transform.localPosition += (Input.mousePosition - PreMousePos) * scaleX;
                PreMousePos = Input.mousePosition;
            }

            /*
             Vector3 v = RotateCKTans.localRotation.eulerAngles;
             if(v.z > 180)
             {
                 v.z -= 360;
             }
             RotateTans.localRotation = Quaternion.Euler(new Vector3(v.x, v.y, v.z * 3.6f));
             RotateTans.Find("DPLane1").localRotation = Quaternion.Euler(new Vector3(v.x, v.y, v.z * 2.5f + 11));
             */
            Dot1Trans.SetParent(GuaDian1, false);
            Dot1Trans.localScale = Vector3.one * 0.01f;
            Dot1Trans.rotation = Quaternion.Euler(Vector3.zero);
            Dot1Trans.localPosition = Vector3.zero;


            Dot2Trans.SetParent(GuaDian2, false);
            Dot2Trans.localScale = Vector3.one * 0.01f;
            Dot2Trans.rotation = Quaternion.Euler(Vector3.zero);
            Dot2Trans.localPosition = Vector3.zero;

            Dot1Trans.SetParent(SKG.transform, true);
            Dot2Trans.SetParent(SKG.transform, true);
            Dot1Trans.localScale = Vector3.one;
            Dot2Trans.localScale = Vector3.one;
        }

        string GetWaWaAnimName()
        {
            int val = UnityEngine.Random.Range(1, 10);
            if(val > 3)
            {
                return "animation4";
            }
            else if(val == 3)
            {
                return "animation3";
            }
            else if (val == 2)
            {
                return "animation2";
            }
            else
            {
                return "animation";
            }
        }

        int SwitchNameToIndex(string name)
        {
            if (name.Contains("15"))
            {
                return 12;
            }
            else if (name.Contains("14"))
            {
                return 11;
            }
            else if (name.Contains("13"))
            {
                return 10;
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
