using DG.Tweening;
using Spine;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseElephantFriendPart1
    {
        GameObject curGo;
        GameObject npc, waterImg, waterSpine, littleElephant, adultElephant;
        GameObject litEleSpine, adlEleSpine, uiSpine, bubbleSpine;
        GameObject[] uiBtn, points;
        UnityEngine.Animation littleText, adultText;

        int count;
        int inVoice;
        int[] waterCount;
        string[] littlrElephantStr, adultElephantStr, uiStr, waterStr;
        Vector3 addVector;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            npc = curTrans.Find("npc").gameObject;
            waterImg = curTrans.Find("gameScene/waterImg").gameObject;
            waterSpine = curTrans.Find("gameScene/waterSpine").gameObject;
            bubbleSpine = curTrans.Find("gameScene/bubbleSpine").gameObject;
            littleElephant = curTrans.Find("gameScene/littleElephant").gameObject;
            adultElephant = curTrans.Find("gameScene/adultElephant").gameObject;
            uiSpine = curTrans.Find("gameScene/uiSpine").gameObject;
            litEleSpine = littleElephant.transform.Find("littleSpine").gameObject;
            adlEleSpine = adultElephant.transform.Find("adultSpine").gameObject;
            littleText = littleElephant.transform.Find("littleText").gameObject.GetComponent<UnityEngine.Animation>();
            adultText = adultElephant.transform.Find("adultText").gameObject.GetComponent<UnityEngine.Animation>();
            uiBtn = GetChildren(curTrans.Find("gameScene/uiBtn").gameObject);
            points = GetChildren(curTrans.Find("gameScene/points").gameObject);

            littlrElephantStr = new string[] { "idle", "walk", "pa", "3", "4", "5", "6", "7", "8", "8_1", "8_2"};
            adultElephantStr = new string[] { "idle", "shuiqiang", "shuiguan", "shuitong", "yun", "walk", "idle_hanshui", "help", "1", "walk2", "idle2", "idle3" };
            uiStr = new string[] { "help_idle", "shuiqinag_click", "shuiguan_click", "shuitong_click", "yun_click", "help_click", "tool"};
            waterStr = new string[] { "water1", "water2", "water3", "water4", "water5", "water6", "water7", "water8"};
            mono = curGo.GetComponent<MonoBehaviour>();
            waterCount = new int[] { 2, 1, 3, 4, 0 };
            addVector = new Vector3(0, 90, 0);
           
            Init();
        }

        void Init()
        {
            npc.SetActive(false);
            littleElephant.transform.position = points[0].transform.position;
            adultElephant.transform.position = points[5].transform.position;
            adultText.gameObject.SetActive(false);
            adultText.transform.GetChild(0).gameObject.SetActive(true);
            adultText.transform.GetChild(1).gameObject.SetActive(false);
            littleText.gameObject.SetActive(true);
            littleText.transform.GetChild(0).gameObject.SetActive(true);
            littleText.transform.GetChild(1).gameObject.SetActive(false);
            bubbleSpine.SetActive(false);
            uiSpine.SetActive(false);
            waterImg.SetActive(false);

            count = 0;
            inVoice = 15;
            SpineManager.instance.PlayAnimationState(waterSpine.GetComponent<SkeletonGraphic>(), waterStr[0]);
            SpineManager.instance.PlayAnimationState(uiSpine.GetComponent<SkeletonGraphic>(), uiStr[0]);

            //SkeletonGraphic sa = waterSpine.GetComponent<SkeletonGraphic>();
            //sa.skeleton.SetSlotsToSetupPose();
            //sa.state.SetAnimation(0, waterStr[0], false);

            SceneInit();
        }

        void SceneInit()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = 0.3f;
            SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[0], true);
            littleText.Play("talk0");
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, true);
            mono.StartCoroutine(WaitTime(1f, () =>
            {
                SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[5], true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, true);
                adultElephant.transform.DOMove(points[2].transform.position, 2f).OnComplete(() =>
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    littleText.Stop("talk0");
                    littleText.transform.GetChild(0).gameObject.SetActive(false);
                    littleText.transform.GetChild(1).gameObject.SetActive(true);
                    littleText.Play("talk1");
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 5, true);
                    SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[6], false, () =>
                    {
                        littleText.Stop("talk1");
                        littleText.gameObject.SetActive(false);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        //SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[1], true);
                        SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[5], true);
                        //littleElephant.transform.DOMove(points[1].transform.position, 2f).OnComplete(() =>
                        // {
                        //     SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[2], true);
                        // });
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, true);
                        adultElephant.transform.DOMove(points[3].transform.position, 1.6f).OnComplete(() =>
                        {
                            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);

                            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 11, true);
                            mono.StartCoroutine(WaitTime(1.2f, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                            }));
                            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                            SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[2], false, () =>
                            {
                                SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[0], true);
                            });
                            SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[7], false, () =>
                            {
                                //mono.StartCoroutine(WaitTime(0.7f, () =>
                                //{
                                //    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);

                                //}));
                                ////SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9, false);
                                //SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[8], false, () =>
                                //{
                                //    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                                //    BackWalk();
                                //});
                                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                                BackWalk();
                            });
                        });
                    });
                });
            }));            
        }

        void BackWalk()
        {
            //SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[1], true);
            //SpineManager.instance.SetTimeScale(litEleSpine, -1);
            //SkeletonGraphic skeletonAnimation = adlEleSpine.GetComponent<SkeletonGraphic>();
            //skeletonAnimation.AnimationState.GetCurrent(0).TrackTime = SpineManager.instance.GetAnimationLength(adlEleSpine, adultElephantStr[5]);
            //SpineManager.instance.SetTimeScale(adlEleSpine, -1);
            SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[9], true);

            littleElephant.transform.DOMove(points[0].transform.position, 2f).OnComplete(() =>
            {
                SpineManager.instance.SetTimeScale(litEleSpine, 1);
                SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[0], true);
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10, true);
            adultElephant.transform.DOMove(points[2].transform.position, 2f).OnComplete(() =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                SpineManager.instance.SetTimeScale(adlEleSpine, 1);
                SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[0], true);
                SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 13, null, () =>
                {
                    uiBtn[0].SetActive(true);
                    uiSpine.SetActive(true);
                    SpineManager.instance.DoAnimation(uiSpine, uiStr[0]);                   
                    Util.AddBtnClick(uiBtn[0], HelpClick);
                    adultText.gameObject.SetActive(true);
                    adultText.transform.GetChild(0).gameObject.SetActive(true);
                    adultText.Play("talk3");
                });               
            });
        }

        void HelpClick(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 7, false);
            SpineManager.instance.DoAnimation(uiSpine, uiStr[5], false, () =>
            {
                SpineManager.instance.DoAnimation(uiSpine, uiStr[6], false, () =>
                {
                    for (int i = 0; i < uiBtn.Length; i++)
                    {
                        if (i != 0)
                        {
                            uiBtn[i].SetActive(true);
                            Util.AddBtnClick(uiBtn[i], ToolClick);
                        }
                        else
                        {
                            uiBtn[i].SetActive(false);
                        }
                    }
                });
            });
        }

        void ToolClick(GameObject btn)
        {
            adultText.Stop("talk3");
            adultText.gameObject.SetActive(false);
            adultText.transform.GetChild(0).gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 8, false);

            string[] str = btn.name.Split('_');
            int index = Convert.ToInt32(str[1]);

            uiSpine.SetActive(false);
            //for (int i = 0; i < uiBtn.Length; i++)
            //{
            //    uiBtn[i].SetActive(false);
            //}
            if (count > 1)
            {
                if (count < 7)
                {
                    littleElephant.transform.DOMove(littleElephant.transform.position + new Vector3(0, 50, 0), 2f);
                }
                else
                {
                    littleElephant.transform.DOMove(littleElephant.transform.position + new Vector3(0, 100, 0), 3.5f);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 6, false);
                SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[count + 1], true);
                //littleElephant.transform.DOMove(littleElephant.transform.position + addVector, 2f);
                Debug.Log("littlrElephantStr[count + 1]" + littlrElephantStr[count + 1]);
            }
            else if(count == 1)
            {
                SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[count + 2], true);
                littleElephant.transform.DOMove(littleElephant.transform.position + new Vector3(0, 40, 0), 2f);
            }

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, true);
            SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[index], false, () =>
            {
                if (count <= 1)
                {
                    SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[0], true);
                }
                else if (count > 1 && count < 5)
                {
                    SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[11], true);
                }
                else 
                {
                    SpineManager.instance.DoAnimation(adlEleSpine, adultElephantStr[10], true);
                }                               
            });
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, waterCount[index], false);
            SpineManager.instance.DoAnimation(waterSpine, waterStr[count], false, () =>
            {                
                if (count < 8)
                {
                    inVoice++;
                    if (inVoice > 17)
                    {
                        inVoice = 15;
                    }
                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, inVoice, null, () =>
                    {
                        waterImg.SetActive(true);
                        uiSpine.SetActive(true);
                        //uiBtn[0].SetActive(true);
                        //SpineManager.instance.DoAnimation(uiSpine, uiStr[0]);
                        adultText.gameObject.SetActive(true);
                        adultText.transform.GetChild(0).gameObject.SetActive(true);
                        adultText.Play("talk3");
                    });
                }
                else
                {
                    for (int i = 0; i < uiBtn.Length; i++)
                    {
                        uiBtn[i].SetActive(false);
                    }
                    waterImg.SetActive(false);
                    GameOver();
                }
            });            
            count++;
        }

        void GameOver()
        {
            Debug.Log("gameOver");
            bubbleSpine.SetActive(true);
            SpineManager.instance.DoAnimation(bubbleSpine,"paopao", true);
            SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[9], true);
            littleElephant.transform.DOMove(points[4].transform.position, 3f).OnComplete(() =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 12, null, () =>
                {
                    SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 14);
                });                
                SpineManager.instance.DoAnimation(litEleSpine, littlrElephantStr[10], true);
                adultText.gameObject.SetActive(true);
                adultText.transform.GetChild(0).gameObject.SetActive(false);
                adultText.transform.GetChild(1).gameObject.SetActive(true);
                adultText.Play("talk4");
            });
        }

        IEnumerator WaitTime(float time, Action act)
        {
            yield return new WaitForSeconds(time);
            act();

            mono.StopCoroutine(WaitTime(time, act));
        }

        GameObject[] GetChildren(GameObject father)
        {
            GameObject[] children = new GameObject[father.transform.childCount];
            for (int i = 0; i < children.Length; i++)
            {
                children[i] = father.transform.GetChild(i).gameObject;
            }

            return children;
        }
    }
}
