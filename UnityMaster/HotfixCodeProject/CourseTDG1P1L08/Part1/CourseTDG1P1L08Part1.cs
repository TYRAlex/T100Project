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
    enum RollState
    {
        Idle = 0,
        Roll,
        Slow,
        result
    }
    public class CourseTDG1P1L08Part1
    {
        MonoBehaviour mono;
        GameObject curGo;
        GameObject ZhuanZi;
        bool IsPressDown;
        Vector3 Prepos;
        Vector3 PreDir;
        Vector3 CenterPos;
        RollState rollState;
        float RollSpeed;
        float Slow_ASpeed;
        float CurNum = 0;
        float AllMoveNum = 0;
        float CurSpeed = 0;
        float dir = 0;
        float DragSpeed;
        int SlowCirle;
        float RollTime = 0;
        int RotateArrIndex = 0;
        int[] RotateArr;
        string[] UpDescImgNameArr;
        string[] DownDescImgNameArr;
        float JieGeRollMusic = 2.0f;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
            GameObject SePan = curTrans.Find("bg/StartBg/DescripSpine").gameObject;
            GameObject Dingding = curTrans.Find("bg/npc").gameObject;
            ZhuanZi = curTrans.Find("bg/zp/ZhuanZi").gameObject;
            curTrans.Find("bg/StartBg").gameObject.SetActive(true);
            IsPressDown = false;
            rollState = RollState.Idle;
            dir = 0;
            JieGeRollMusic = 2.0f;
            RotateArr = new int[] { 5, 2, 7, 3, 8, 4, 1, 6, 8, 5, 1, 7, 5, 3, 6, 4 };
            UpDescImgNameArr = new string[]{"c_1_p_l", "c_2_p_l", "c_3_p_l", "c_4_p_l", "c_5_p_l", "c_6_p_l", "c_7_p_l", "c_8_p_l",
                                            "c_9_p_l", "c_10_p_l", "c_11_p_l", "c_12_p_l", "c_13_p_l", "c_14_p_l", "c_15_p_l", "c_16_p_l" };
            DownDescImgNameArr = new string[]{"c_1_p_r", "c_2_p_r", "c_3_p_r", "c_4_p_r", "c_5_p_r", "c_6_p_r", "c_7_p_r", "c_8_p_r",
                                            "c_9_p_r", "c_10_p_r", "c_11_p_r", "c_12_p_r", "c_13_p_r", "c_14_p_r", "c_15_p_r", "c_16_p_r" };
            RollSpeed = 30;
            SlowCirle = 3;
            DragSpeed = 0;
            Coroutine ct = null;
            SpineManager.instance.DoAnimation(SePan, "zhuanpan", false, ()=>
            {
                ct = mono.StartCoroutine(Wait(6, () =>
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                    SpineManager.instance.DoAnimation(SePan, "jieshuo_red", false, () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, false);
                        SpineManager.instance.DoAnimation(SePan, "jieshuo_green", false);
                    });
                }));
            });
            SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                Dingding.SetActive(true);
                SpineManager.instance.DoAnimation(Dingding, "breath", true);
                if(ct != null)
                {
                    mono.StopCoroutine(ct);
                }
                mono.StartCoroutine(Wait(1, ()=> 
                {
                    curTrans.Find("bg/StartBg").gameObject.SetActive(false);
                    SoundManager.instance.Speaking(Dingding, "talk", SoundManager.SoundType.VOICE, 1, null, () =>
                    {
                        ILObject3DAction i3d = curTrans.Find("bg/zp/UIJH").GetComponent<ILObject3DAction>();
                        i3d.OnPointDownLua = PointerDown;
                        i3d.OnPointUpLua = PointerUp; ;
                    });
                }));
            });
            
            curTrans.Find("bg/zp/ZhuanZi/Mask").GetComponent<Button>().onClick.RemoveAllListeners();
            curTrans.Find("bg/zp/ZhuanZi/Mask").GetComponent<Button>().onClick.AddListener(() =>
            {
                HideResultUI();
                curTrans.Find("bg/zp/ZhuanZi/Mask").GetComponent<Image>().raycastTarget = false;
            });
            RotateArrIndex = 0;
            HideResultUI();
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM, 0.4f);
            CenterPos = new Vector3(960, 565, 0);
        }

        void HideResultUI()
        {
            for (int i = 0; i < 16; i++)
            {
                curGo.transform.Find("bg/zp/ZhuanZi/HL" + (i + 1)).gameObject.SetActive(false);
                curGo.transform.Find("bg/zp/ZhuanZi/HLF" + (i + 1)).gameObject.SetActive(false);
            }
            curGo.transform.Find("bg/zp/ZhuanZi/Mask").gameObject.SetActive(false);
            curGo.transform.Find("bg/zp/DownDescroipImg").gameObject.SetActive(false);
            curGo.transform.Find("bg/zp/UpDescroipImg").gameObject.SetActive(false);
            curGo.transform.Find("bg/zp/UIJH").GetComponent<Image>().raycastTarget = true;
            rollState = RollState.Idle;
        }

        void PointerDown(int index)
        {
            Debug.Log("1111");
            Prepos = Input.mousePosition;
            PreDir = Prepos;
            IsPressDown = true;
            dir = 0;
        }

        void PointerUp(int index)
        {
            curGo.transform.Find("bg/zp/UIJH").GetComponent<Image>().raycastTarget = false;
            StartRoll();
            dir = 0;
            IsPressDown = false;
        }

        IEnumerator Wait(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act.Invoke();
        }

        void StartRoll()
        {
            if(rollState == RollState.Idle)
            {
                rollState = RollState.Roll;
                if(DragSpeed < 0)
                {
                    CurSpeed = -RollSpeed;
                }
                else
                {
                    CurSpeed = RollSpeed;
                }
                RollTime = 0;
            }
        }

        void ShanShuo(int count, Action act)
        {
            int downIndex = (RotateArr[RotateArrIndex] + 8 - 1) % 16 + 1;
            if (count > 4)
            {
                curGo.transform.Find("bg/zp/ZhuanZi/HL" + RotateArr[RotateArrIndex]).GetComponent<Image>().CrossFadeAlpha(1, 0.2f, true);
                curGo.transform.Find("bg/zp/ZhuanZi/HL" + downIndex).GetComponent<Image>().CrossFadeAlpha(1, 0.2f, true);
                if (act != null)
                { 
                    act.Invoke();
                }
            }
            else
            {
                if(count % 2 == 0)
                {
                    curGo.transform.Find("bg/zp/ZhuanZi/HL" + RotateArr[RotateArrIndex]).GetComponent<Image>().CrossFadeAlpha(0, 0.2f, true);
                    curGo.transform.Find("bg/zp/ZhuanZi/HL" + downIndex).GetComponent<Image>().CrossFadeAlpha(0, 0.2f, true);
                }
                else
                {
                    curGo.transform.Find("bg/zp/ZhuanZi/HL" + RotateArr[RotateArrIndex]).GetComponent<Image>().CrossFadeAlpha(1, 0.2f, true);
                    curGo.transform.Find("bg/zp/ZhuanZi/HL" + downIndex).GetComponent<Image>().CrossFadeAlpha(1, 0.2f, true);
                }
                mono.StartCoroutine(Wait(0.2f, () =>
                {
                    ShanShuo(count + 1, act);
                }));
            }
        }

        void FixedUpdate()
        {
            if(IsPressDown)
            {
                dir = Vector2.Distance(Input.mousePosition, Prepos);
                Vector3 CurDir = Input.mousePosition - CenterPos;
                Vector3 PreDir = Prepos - CenterPos;
                Vector3 fn = Vector3.Cross(CurDir, PreDir);

                if(fn.z > 0)
                {
                    AddAllMoveNum(dir * 0.03f);
                }
                else
                {
                    AddAllMoveNum(-dir * 0.03f);
                    dir = -dir * 0.03f;
                }
                DragSpeed = dir / Time.deltaTime;
                ZhuanZi.transform.localRotation = Quaternion.Euler(Vector3.back * (CurNum + AllMoveNum) * 22.5f);
                Prepos = Input.mousePosition;
            }
            
            if(rollState == RollState.Roll)
            {
                AddAllMoveNum(CurSpeed * Time.deltaTime);
                RollTime += Time.deltaTime;
                if(RollTime > 1.5)
                {
                    rollState = RollState.Slow;
                    float dis = 0;
                    float CurIndex = (CurNum + AllMoveNum) % 16;
                    float TargetIndex = (16 - (RotateArr[RotateArrIndex] + UnityEngine.Random.Range(0.2f, 0.8f)) + 1) % 16;
                    TargetIndex = Math.Abs(TargetIndex);
                    if(CurSpeed > 0)
                    {
                        if (CurIndex > TargetIndex)
                        {
                            dis = TargetIndex + 16 - CurIndex;
                        }
                        else
                        {
                            dis = TargetIndex - CurIndex;
                        }
                    }
                    else
                    {
                        if (CurIndex > TargetIndex)
                        {
                            dis = CurIndex - TargetIndex;
                        }
                        else
                        {
                            dis = CurIndex + 16 - TargetIndex;
                        }
                        dis = -dis;
                    }
                    for (int i = SlowCirle; i >= 0; i--)
                    {
                        float AllDis = dis + SlowCirle * 16;
                        if(CurSpeed < 0)
                        {
                            AllDis = dis - SlowCirle * 16;
                        }
                        Slow_ASpeed = (RollSpeed * RollSpeed) / (2 * AllDis);
                        if (RollSpeed / Slow_ASpeed < 3f)
                        {
                            break;
                        }
                    }
                    rollState = RollState.Slow;
                }
            }
            else if(rollState == RollState.Slow)
            {
                if((CurSpeed - Slow_ASpeed * Time.deltaTime < 0 && CurSpeed > 0) || (CurSpeed - Slow_ASpeed * Time.deltaTime > 0 && CurSpeed < 0))
                {
                    AddAllMoveNum((CurSpeed / 2) * (CurSpeed / Slow_ASpeed));
                    CurSpeed = 0;
                    float idx = (CurNum + AllMoveNum) % 16;
                    ZhuanZi.transform.localRotation = Quaternion.Euler(Vector3.back * idx * 22.5f);
                    rollState = RollState.result;
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                    mono.StartCoroutine(Wait(0.5f, () =>
                    {
                        int downIndex = (RotateArr[RotateArrIndex] + 8 - 1) % 16 + 1;
                        curGo.transform.Find("bg/zp/ZhuanZi/HL" + RotateArr[RotateArrIndex]).gameObject.SetActive(true);
                        curGo.transform.Find("bg/zp/ZhuanZi/HL" + downIndex).gameObject.SetActive(true);
                        //ShanShuo(0, () =>
                        //{
                            curGo.transform.Find("bg/zp/ZhuanZi/HL" + RotateArr[RotateArrIndex]).gameObject.SetActive(false);
                            curGo.transform.Find("bg/zp/ZhuanZi/HL" + downIndex).gameObject.SetActive(false);
                            curGo.transform.Find("bg/zp/ZhuanZi/HLF" + RotateArr[RotateArrIndex]).gameObject.SetActive(true);
                            curGo.transform.Find("bg/zp/ZhuanZi/HLF" + downIndex).gameObject.SetActive(true);
                            Transform trans2 = curGo.transform.Find("bg/zp/UpDescroipImg");
                            trans2.gameObject.SetActive(true);
                            trans2.localScale = Vector3.zero;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                            trans2.DOScale(1, 0.25f).SetEase(Ease.InOutBack).OnComplete(() =>
                            {
                                Transform trans1 = curGo.transform.Find("bg/zp/DownDescroipImg");
                                trans1.gameObject.SetActive(true);
                                trans1.localScale = Vector3.zero;
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 4, false);
                                trans1.DOScale(1, 0.25f).SetEase(Ease.InOutBack).OnComplete(()=>
                                {
                                    curGo.transform.Find("bg/zp/ZhuanZi/Mask").GetComponent<Image>().raycastTarget = true;
                                });
                                trans1.GetComponent<Image>().sprite = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseTDG1P1L08Part1"), DownDescImgNameArr[downIndex - 1]);
                                RotateArrIndex++;
                                RotateArrIndex = RotateArrIndex % 16;
                            });
                            trans2.GetComponent<Image>().sprite = ResourceManager.instance.LoadResourceAB<Sprite>(Util.GetHotfixPackage("CourseTDG1P1L08Part1"), UpDescImgNameArr[RotateArr[RotateArrIndex] - 1]);
                            curGo.transform.Find("bg/zp/ZhuanZi/Mask").gameObject.SetActive(true);
                            
                        //});
                    }));
                }
                else
                {
                    AddAllMoveNum(((CurSpeed + CurSpeed - Slow_ASpeed * Time.deltaTime) / 2) * Time.deltaTime);
                    CurSpeed -= Slow_ASpeed * Time.deltaTime;
                }
            }
            else if(rollState == RollState.result)
            {
                
            }

            if(rollState == RollState.Roll || rollState == RollState.Slow)
            {
                ZhuanZi.transform.localRotation = Quaternion.Euler(Vector3.back * ((CurNum + AllMoveNum) % 16) * 22.5f);
            }
        }

        void AddAllMoveNum(float num)
        {
            AllMoveNum += num;
            if (num > 0)
            {
                JieGeRollMusic -= num;
            }
            else
            {
                JieGeRollMusic += num;
            }
            if (JieGeRollMusic <= 0)
            {
                JieGeRollMusic = 2;
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
            }
        }
    }
}
