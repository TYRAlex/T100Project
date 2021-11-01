using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseTDG1P1L12Part1
    {
        GameObject curGo;
        Vector3[] WW_OrginPos;
        Vector3[] WW_OrginChildPos;
        Vector3[] ZhanShiPos;
        Vector3[] AwardPos;
        Vector3 Pos;
        int[] SpeedDir;
        float Speed;
        bool IsStart;
        GameObject ZhuaZi;
        int grab;   //0未开始抓取, 1开始抓取, 2结束抓取
        GameObject grabLine;
        GameObject TempGo;
        float Length = 0;
        MonoBehaviour mono;
        int grapNum = 0;
        bool Drop = false;
        float DropLength = 0;
        bool AreadyGrapWawa = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            WW_OrginPos = new Vector3[] {new Vector3(-400, 133.21f, 0), new Vector3(-180, 133.21f, 0), new Vector3(180, 133.21f, 0), new Vector3(400, 133.21f, 0),
                                         new Vector3(-400, -68.83f, 0), new Vector3(-180, -68.83f, 0), new Vector3(180, -68.83f, 0), new Vector3(400, -68.83f, 0)};
            WW_OrginChildPos = new Vector3[] {new Vector3(378.7f, -553.5f, 0), new Vector3(149.7f, -552.4f, 0), new Vector3(-205.2f, -553.8f, 0), new Vector3(-431.1f, -552.2f, 0),
                                              new Vector3(377.6f, -306, 0), new Vector3(159.2f, -306.4f, 0), new Vector3(-192.7f, -304.4f, 0), new Vector3(-422.4f, -303.3f, 0)};
            ZhanShiPos = new Vector3[] {new Vector3(-420.5f,-309.4f, 0), new Vector3(-287.3f, -355.3f, 0), new Vector3(-552.1f, -356.7f, 0), new Vector3(-414.6f, -398.5f, 0),
                                        new Vector3(428.6f, -312.2f, 0), new Vector3(278f, -356.9f, 0), new Vector3(551.5f, -355f, 0), new Vector3(415.9f, -398.2f, 0)};
            AwardPos = new Vector3[] {new Vector3(751.4f, -1022.7f, 0), new Vector3(286.8f, -1027.4f, 0), new Vector3(-410.3f, -1040.3f, 0), new Vector3(-860.7f, -1021.3f, 0),
                                        new Vector3(756f, -539.9f, 0), new Vector3(309.9f, -550.4f, 0), new Vector3(-389.9f, -540.1f, 0), new Vector3(-856f, -547.7f, 0)};
            SpeedDir = new int[]{1, 1, 1, 1, -1, -1, -1, -1};
            Speed = 40;
            IsStart = false;
            AreadyGrapWawa = false;
            if (curTrans.Find("bg/Mask").childCount > 0)
            {
                int num1 = SwitchToNum(curTrans.Find("bg/Mask").GetChild(0).name);
                curTrans.Find("bg/Mask").GetChild(0).SetParent(curTrans.Find("dotPlane/dot" + num1), false);
            }

            

            Transform tans = curTrans.Find("bg");
            if (tans.GetChild(0).name != "dotPlane")
            {
                tans.Find("dotPlane").SetAsFirstSibling();
            }
            for (int i = 0; i < 8; i++)
            {
                tans.Find("dotPlane/dot" + (i + 1)).localPosition = WW_OrginPos[i];
                tans.Find("dotPlane/dot" + (i + 1)).GetChild(0).GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                string s = string.Format("ww{0}_idle", (i + 1));
                if(i == 1)
                {
                    s = "ww3_idle";
                }
                else if(i == 2)
                {
                    s = "ww2_idle";
                }
                SpineManager.instance.DoAnimation(tans.Find("dotPlane/dot" + (i + 1)).GetChild(0).gameObject, s, true);
            }
            SoundManager.instance.Speaking(curTrans.Find("bg/npc").gameObject, "talk", SoundManager.SoundType.VOICE, 0, null, ()=>
            {
                curTrans.Find("bg/ZZ/Line").GetComponent<RectTransform>().sizeDelta = new Vector2(18, 100);
                curGo.transform.Find("bg/StartSpine/Btn").GetComponent<Button>().enabled = true;
                curTrans.Find("bg/StartSpine/Btn").GetComponent<Button>().onClick.RemoveAllListeners();
                IsStart = true;
                curTrans.Find("bg/StartSpine/Btn").GetComponent<Button>().onClick.AddListener(() =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                    SpineManager.instance.DoAnimation(curTrans.Find("bg/StartSpine").gameObject, "anniu", false);
                    StartGrab();
                });
            });

            
            ZhuaZi = curGo.transform.Find("bg/ZZ/Line/ZhuaZi/Collider").gameObject;
            ILObject3DAction i3d = ZhuaZi.GetComponent<ILObject3DAction>();
            i3d.OnCollisionEnter2DLua = OnCollier;
            grabLine = curGo.transform.Find("bg/ZZ/Line").gameObject;
            grab = 0;

            curGo.transform.Find("bg/Win0").gameObject.SetActive(false);
            curGo.transform.Find("bg/Win1").gameObject.SetActive(false);
            curGo.transform.Find("bg/Bg").gameObject.SetActive(false);
            for (int i = 0; i < curGo.transform.Find("bg/Win1").childCount; i++)
            {
                curGo.transform.Find("bg/Win1").GetChild(i).gameObject.SetActive(false);
            }
            if(ZhuaZi.transform.childCount > 0)
            { 
                ZhuaZi.GetComponent<BoxCollider2D>().enabled = true;
                int num = SwitchToNum(ZhuaZi.transform.GetChild(0).name);
                ZhuaZi.transform.GetChild(0).SetParent(curGo.transform.Find("bg/dotPlane/dot" + num), false);
                curGo.transform.Find("bg/dotPlane/dot" + num).GetChild(0).localPosition = WW_OrginChildPos[num - 1];
                
                ZhuaZi.transform.localScale = Vector3.one;
            }


            SpineManager.instance.DoAnimation(ZhuaZi.transform.parent.gameObject, "gz_1", false);
            SpineManager.instance.DoAnimation(curTrans.transform.Find("wing").gameObject, "animation", true);
            SpineManager.instance.DoAnimation(curTrans.transform.Find("bg/bg1/star").gameObject, "animation2", true);
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            curTrans.Find("bg/Mask").gameObject.SetActive(false);
        }

        void StartGrab()
        {
            if (grab == 0)
            {
                curGo.transform.Find("bg/StartSpine/Btn").GetComponent<Button>().enabled = false;
                grab = 1;
            }
        }

        void GrapOver()
        {
            if(ZhuaZi.transform.childCount > 0)
            {
                int BoFangIndex = UnityEngine.Random.Range(1, 4);
                if(BoFangIndex == 3)
                {
                    BoFangIndex = 12;
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, BoFangIndex, false);
                //if (UnityEngine.Random.Range(0, 2) == 0)
                //{
                //curGo.transform.Find("bg/Win0").gameObject.SetActive(true);
                //SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Win0").gameObject, "animation", false, ()=>
                //{
                //NewTurn();
                //});
                //}
                //else
                //{
                //curGo.transform.Find("bg/Win1").gameObject.SetActive(true);
                //curGo.transform.Find("bg/Win1/1").gameObject.SetActive(true);

                //SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Win1/1").gameObject, "animation", false, ()=>
                //{
                //SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Win1/1").gameObject, "idle", false, () =>
                //{
                //NewTurn();
                //});
                //});
                //}

                TempGo = ZhuaZi.transform.GetChild(0).gameObject;
                curGo.transform.Find("bg/Mask").gameObject.SetActive(true);
                Pos = TempGo.transform.localPosition;
                int num = SwitchToNum(ZhuaZi.transform.GetChild(0).name);
                TempGo.transform.SetParent(curGo.transform.Find("bg/Mask"), true);
                TempGo.transform.DOScale(1.0f, 0.4f).SetEase(Ease.OutBack);
                Vector3 Temppos = TempGo.transform.localPosition;
                TempGo.transform.DOLocalMove(AwardPos[num - 1], 0.4f);

                mono.StartCoroutine(Wait(1.5f, () =>
                {
                    NewTurn();
                }));
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, UnityEngine.Random.Range(10, 12), false);
                SpineManager.instance.DoAnimation(ZhuaZi.transform.parent.gameObject, "gz_4", false, ()=>
                {
                    NewTurn();
                });
            }
        }

        void NewTurn()
        {
            curGo.transform.Find("bg/Win0").gameObject.SetActive(false);
            curGo.transform.Find("bg/Win1").gameObject.SetActive(false);
            if (TempGo != null)
            {
                TempGo.transform.SetParent(ZhuaZi.transform, true);
                TempGo = null;
            }
            for (int i = 0; i < curGo.transform.Find("bg/Win1").childCount; i++)
            {
                curGo.transform.Find("bg/Win1").GetChild(i).gameObject.SetActive(false);
            }
            if(ZhuaZi.transform.childCount > 0)
            { 
                int num = SwitchToNum(ZhuaZi.transform.GetChild(0).name);
                curGo.transform.Find("bg/dotPlane/dot" + num).localScale = Vector3.one;
                ZhuaZi.transform.GetChild(0).SetParent(curGo.transform.Find("bg/dotPlane/dot" + num), true);
                curGo.transform.Find("bg/dotPlane/dot" + num).GetChild(0).DOScale(0.5f, 0.4f);
                curGo.transform.Find("bg/dotPlane/dot" + num).GetChild(0).DOLocalMove(WW_OrginChildPos[num - 1], 0.4f);
                curGo.transform.Find("bg/dotPlane/dot" + num).DOLocalMove(ZhanShiPos[num - 1], 0.4f).OnComplete(()=>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                });
                curGo.transform.Find("bg/Mask").gameObject.SetActive(false);
                grapNum += 1;
                if(grapNum >= 8)
                {
                    SoundManager.instance.Speaking(curGo.transform.Find("bg/npc").gameObject, "talk", SoundManager.SoundType.VOICE, 3, null, ()=>
                    {
                        /*
                        curGo.transform.Find("bg/Bg").gameObject.SetActive(true);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                        mono.StartCoroutine(Wait(1.0f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                        }));
                        SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Bg/Win2").gameObject, "animation", false, () =>
                        {
                            SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Bg/Win2").gameObject, "idle", true);
                        });
                        */
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
                        IsStart = false;
                        for (int i = 0; i < 8; i++)
                        {
                            curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).DOLocalMove(new Vector3(-525 + 150 * i, 0, 0), 0.5f);
                        }
                        mono.StartCoroutine(Wait(1.0f, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);
                        }));

                        curGo.transform.Find("bg/Bg/Award").gameObject.SetActive(false);
                        curGo.transform.Find("bg/dotPlane").SetSiblingIndex(8);
                        curGo.transform.Find("bg/Bg").gameObject.SetActive(true);
                        curGo.transform.Find("bg/Bg").GetComponent<Image>().material.SetFloat("_Size", 0);
                        mono.StartCoroutine(Fade());
                        mono.StartCoroutine(Wait(0.51f, () =>
                        {
                            curGo.transform.Find("bg/Bg/Award").gameObject.SetActive(true);
                            SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Bg/Award").gameObject, "animation", false, () =>
                            {
                                SpineManager.instance.DoAnimation(curGo.transform.Find("bg/Bg/Award").gameObject, "idle", true);
                            });
                        }));
                    });
                }
            }
           
            ZhuaZi.GetComponent<BoxCollider2D>().enabled = true;
            SpineManager.instance.DoAnimation(ZhuaZi.transform.parent.gameObject, "gz_1", false);
            ZhuaZi.transform.localScale = Vector3.one;
            if (grapNum < 8)
            {
                curGo.transform.Find("bg/StartSpine/Btn").GetComponent<Button>().enabled = true;
            }
            AreadyGrapWawa = false;
        }

        IEnumerator Fade()
        {
            float val = 0;
            while(val < 3)
            {
                val = curGo.transform.Find("bg/Bg").GetComponent<Image>().material.GetFloat("_Size");
                curGo.transform.Find("bg/Bg").GetComponent<Image>().material.SetFloat("_Size", val + Time.deltaTime * 2);
                yield return null;
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

        void OnCollier(Collision2D colison, int index)
        {
            if(AreadyGrapWawa)
            {
                return;
            }
            AreadyGrapWawa = true;
            grab = 3;
            int idex = SwitchToNum(colison.gameObject.transform.parent.parent.name);
            SpeedDir[idex - 1] = 0;
            string s = string.Format("ww{0}_idle2", idex);
            string s1 = string.Format("ww{0}_animation", idex);
            if(idex == 2)
            {
                s = "ww3_idle2";
                s1 = "ww3_animation";
            }
            else if(idex == 3)
            {
                s = "ww2_idle2";
                s1 = "ww2_animation";
            }

            float x; 
            Transform trans = colison.gameObject.transform.parent.parent;
            trans.SetParent(ZhuaZi.transform, true);
            trans.DOLocalMove(Vector3.zero, 0.25f);
            if (Math.Abs(trans.localPosition.x) > 70)
            {
                Drop = true;
                DropLength = Length * (UnityEngine.Random.Range(2, 6) / 10.0f);
            }
            else
            {
                Drop = false;
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
            SpineManager.instance.DoAnimation(colison.gameObject.transform.parent.gameObject, s, true);
            SpineManager.instance.DoAnimation(ZhuaZi.transform.parent.gameObject, "gz_3", false, ()=>
            {
                //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                mono.StartCoroutine(Wait(0.25f, () =>
                {
                    ZhuaZi.GetComponent<BoxCollider2D>().enabled = false;
                    colison.gameObject.GetComponent<BoxCollider2D>().enabled = false;
                    grab = 2;
                   
                    Debug.Log("trans.localPosition.x: " + trans.localPosition.x);
                    trans.localPosition = new Vector3(trans.localPosition.x, 0, 0);
                    trans.GetChild(0).SetParent(ZhuaZi.transform, true);
                    trans.SetParent(curGo.transform.Find("bg/dotPlane"), true);
                    trans.SetSiblingIndex(SwitchToNum(trans.name) - 1);
                    x = trans.localPosition.x;
                    trans.localPosition = new Vector3(x, WW_OrginPos[SwitchToNum(trans.name) - 1].y, 0);
                    SpineManager.instance.DoAnimation(colison.gameObject.transform.parent.gameObject, s1, true);
                }));
            });
        }

        void Update()
        {
            if(IsStart)
            {
                for (int i = 0; i < 8; i++)
                {
                    if(curGo.transform.Find("bg/dotPlane/dot" + (i + 1)) != null)
                    { 
                        Vector3 prepos = curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).localPosition;
                        Vector3 tarpos = curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).localPosition + Vector3.right * Speed * Time.deltaTime * SpeedDir[i] * 0.8f;
                        if(tarpos.x > 540 || tarpos.x < -540)
                        {
                            curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).localPosition = new Vector3(540 * (tarpos.x / Math.Abs(tarpos.x)), tarpos.y, 0);
                            SpeedDir[i] = -SpeedDir[i];
                        }
                        else if(tarpos.x * prepos.x <= 0 && prepos.x != 0)
                        {
                            curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).localPosition = new Vector3(0, tarpos.y, 0);
                            SpeedDir[i] = -SpeedDir[i];
                        }
                        else
                        {
                            curGo.transform.Find("bg/dotPlane/dot" + (i + 1)).localPosition = tarpos;
                        }
                    }
                }
            }

            if(grab == 1)
            {
                Length = Length + Time.deltaTime * 200;
                if (Length >= 430)
                {
                    Length = 430;
                    grab = 2;
                    ZhuaZi.GetComponent<BoxCollider2D>().enabled = false;
                }
                grabLine.GetComponent<RectTransform>().sizeDelta = new Vector2(18, 100 + Length);
                
            }
            else if(grab == 2)
            {
                Length = Length - Time.deltaTime * 400;
                if (Length <= 0)
                {
                    Length = 0;
                    grab = 0;
                    GrapOver();
                }
                if(Drop && DropLength >= Length)
                {
                    Drop = false;
                    int num = SwitchToNum(ZhuaZi.transform.GetChild(0).name);
                    curGo.transform.Find("bg/dotPlane/dot" + num).localScale = Vector3.one;
                    Transform transf = ZhuaZi.transform.GetChild(0);
                    transf.SetParent(curGo.transform.Find("bg/dotPlane/dot" + num), true);
                    //curGo.transform.Find("bg/dot" + num).GetChild(0).localPosition = WW_OrginChildPos[num - 1];
                    float x = transf.localPosition.x;

                    SpineManager.instance.DoAnimation(ZhuaZi.transform.parent.gameObject, "gz_2", false);
                    transf.localScale = Vector3.one * 0.5f;

                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                    transf.DOLocalMove(new Vector3(x, WW_OrginChildPos[num - 1].y, 0), 0.4f).OnComplete(()=>
                    {
                        string s = string.Format("ww{0}_idle", num);
                        if (num == 2)
                        {
                            s = "ww3_idle";
                        }
                        else if (num == 3)
                        {
                            s = "ww2_idle";
                        }
                        SpineManager.instance.DoAnimation(transf.gameObject, s, true);
                        if(UnityEngine.Random.Range(0, 2) == 0)
                        {
                            SpeedDir[num - 1] = 1;
                        }
                        else
                        {
                            SpeedDir[num - 1] = -1;
                        }
                        transf.GetChild(0).GetComponent<BoxCollider2D>().enabled = true;
                    }).SetEase(Ease.InCubic);
                }
                grabLine.GetComponent<RectTransform>().sizeDelta = new Vector2(18, 100 + Length);
            }
        }

        public int SwitchToNum(string s)
        {
            if (s.Contains("1"))
            {
                return 1;
            }
            else if (s.Contains("2"))
            {
                return 2;
            }
            else if (s.Contains("3"))
            {
                return 3;
            }
            else if (s.Contains("4"))
            {
                return 4;
            }
            else if (s.Contains("5"))
            {
                return 5;
            }
            else if (s.Contains("6"))
            {
                return 6;
            }
            else if (s.Contains("7"))
            {
                return 7;
            }
            else
            {
                return 8;
            }
        }
    }
}
