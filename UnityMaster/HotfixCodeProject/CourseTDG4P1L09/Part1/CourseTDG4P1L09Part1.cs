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
    public class CourseTDG4P1L09Part1
    {
        private int uiIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        GameObject npc, uiSpine, btnSpine, hammerObj, homeGraphic, shineSpine, gearSpine;
        GameObject[] hammers, points;

        string[] houseAniStr, uiAniStr, btnAniStr ,hammerAniStr;
        List<string[]> houseAniList;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            npc = curTrans.Find("npc").gameObject;
            uiSpine = curTrans.Find("GameScene/uiSpine").gameObject;
            btnSpine = curTrans.Find("GameScene/button").gameObject;
            shineSpine = curTrans.Find("GameScene/shineSpine").gameObject;
            gearSpine = curTrans.Find("GameScene/gearSpine").gameObject;
            hammerObj = curTrans.Find("GameScene/hammers").gameObject;
            curTrans.Find("Bg").gameObject.SetActive(true);
            homeGraphic = curTrans.Find("GameScene/homeGraphic").gameObject;
            hammers = curTrans.GetChildren(hammerObj);
            points = curTrans.GetChildren(curTrans.Find("GameScene/points").gameObject);

            houseAniStr = new string[] { "animation_1", "animation_2", "animation_3", "animation_4", "animation_5",
                                        "animation_6", "animation_7", "animation_8", "animation_9", "animation_10",
                                        "animation_11", "animation_12", "animation_13", "animation_14", "animation_15", "animation_16"};
            houseAniList = new List<string[]>() { new string[] { "animation_1", "animation_2" }, new string[] { "animation_3"}, new string[] { "animation_4", "animation_5", "animation_6"}, new string[] { "animation_7", "animation_8" },
                                                  new string[] { "animation_9","animation_10", "animation_11", "animation_12"}, new string[] { "animation_13", "animation_14", "animation_15", "animation_16"} };
            uiAniStr = new string[] { "animation",  "animation1", "animation2",  "animation3",  "animation4", "animation5",  "idle"};
            hammerAniStr = new string[] {  "chuizi", "chuizi2", "chuizi3"};
            btnAniStr = new string[] { "1_idle", "1", "chilun_idle"};
            mono = curGo.GetComponent<MonoBehaviour>();

            GameInit();
            GameStart();
        }

        void GameInit()
        {
            //aniIndex = 0;
            uiIndex = -1;
            btnSpine.SetActive(true);
            uiSpine.SetActive(true);
            gearSpine.SetActive(true);
            hammerObj.SetActive(false);
            shineSpine.SetActive(false);
            npc.SetActive(false);
            gearSpine.transform.GetChild(0).gameObject.SetActive(true);
            btnSpine.GetComponent<Button>().enabled = true;
            uiSpine.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimation(0, 0);
            //houseAni.GetComponent<SkeletonAnimation>().AnimationState.SetEmptyAnimation(0, 0);
            SkeletonGraphic skeletonGraphic = homeGraphic.GetComponent<SkeletonGraphic>();
            skeletonGraphic.Skeleton.SetToSetupPose();
            skeletonGraphic.AnimationState.ClearTracks();
            skeletonGraphic.AnimationState.SetEmptyAnimation(0, 0);
        }

        void GameStart()
        {
            SpineManager.instance.DoAnimation(btnSpine.transform.GetChild(0).gameObject, btnAniStr[0], true);
            SpineManager.instance.DoAnimation(uiSpine, uiAniStr[6], true);
            //SpineManager.instance.DoAnimation(gearSpine, btnAniStr[2], false);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {                
                SoundManager.instance.BgSoundPart2();
                Util.AddBtnClick(btnSpine, HouseAnimation);
            });

            LogicManager.instance.ShowReplayBtn(false);
            LogicManager.instance.SetReplayEvent(() =>
            {
                GameInit();
                GameStart();
            });
        }

        void HammerClick(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(btn, btnAniStr[1], false, () =>
            {
                uiIndex++;
                SoundManager.instance.sheildGo.SetActive(true);
                SpineManager.instance.DoAnimation(btn, btnAniStr[0], true); 
                
                if (uiIndex == 15)
                {
                    btn.SetActive(false);
                }
                HammerMove();
            });                   
        }

        void HammerMove()
        {
            Debug.Log("uiIndex:" + uiIndex);
            hammerObj.SetActive(true);
            switch (uiIndex)
            {                
                case 2:
                case 3:
                case 14:
                case 15:
                    {
                        hammers[0].SetActive(true);
                        hammers[1].SetActive(true);
                        hammers[2].SetActive(false);
                        hammers[0].transform.position = points[0].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                        hammers[1].transform.position = points[1].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                    }
                    break;
                case 4:
                case 5:
                case 6:
                case 7:
                    {
                        hammers[0].SetActive(true);
                        hammers[1].SetActive(true);
                        hammers[2].SetActive(true);
                        hammers[0].transform.position = points[0].transform.position + new Vector3(0, 400, 0) + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                        hammers[1].transform.position = points[1].transform.position + new Vector3(0, 400, 0) + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                        hammers[2].transform.position = points[2].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                    }
                    break;
                case 9:
                case 10:                
                case 12:
                case 13:
                    {
                        hammers[0].SetActive(true);
                        hammers[1].SetActive(true);
                        hammers[2].SetActive(true);
                        hammers[0].transform.position = points[0].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                        hammers[1].transform.position = points[1].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                        hammers[2].transform.position = points[2].transform.position + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                    }
                    break;
                case 11:
                    hammers[0].SetActive(false);
                    hammers[1].SetActive(true);
                    hammers[2].SetActive(false);                    
                    hammers[1].transform.position = points[1].transform.position + new Vector3(0, 200, 0) + new Vector3(Util.Random(-50, 50), Util.Random(-50, 50), 0);
                    break;
            }
           
            if (uiIndex > 1 && uiIndex != 8)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, true);
                for (int i = 0; i < hammers.Length; i++)
                {
                    SkeletonGraphic skeletonGraphic = hammers[i].transform.GetChild(0).GetComponent<SkeletonGraphic>();
                    SpineManager.instance.DoAnimation(skeletonGraphic.gameObject, hammerAniStr[Util.Random(0, 3)], true);                    
                }
                mono.StartCoroutine(WaitTime(1.5f));
            }
            else
            {
                //Animation();
                HouseAnimation();
            }            
        }

        void HouseAnimation()
        {
            //SkeletonAnimation skeletonAnimation = houseAni.GetComponent<SkeletonAnimation>();
            SkeletonGraphic skeletonAnimation = homeGraphic.GetComponent<SkeletonGraphic>();
            //skeletonAnimation.Skeleton.SetToSetupPose();
            skeletonAnimation.AnimationState.ClearTracks();
            //skeletonAnimation.AnimationName = null;
            //houseAni.GetComponent<SkeletonAnimation>().AnimationState.SetEmptyAnimation(0, 0);
            SpineManager.instance.DoAnimation(homeGraphic, houseAniStr[uiIndex], false, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(uiSpine, uiAniStr[uiIndex + 17], false, () =>
                {
                    SoundManager.instance.sheildGo.SetActive(false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
                    SpineManager.instance.DoAnimation(uiSpine, uiAniStr[uiIndex + 1], false, () =>
                    {
                        if (uiIndex == 15)
                        {
                            shineSpine.SetActive(true);
                            SpineManager.instance.DoAnimation(shineSpine, "animation", true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                        }
                    });
                });
            });
        }

        void HouseAnimation(GameObject btn)
        {
            uiIndex++;
            SoundManager.instance.sheildGo.SetActive(true);
            if (uiIndex == 0)
            {
                gearSpine.transform.GetChild(0).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(gearSpine, btnAniStr[2], false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(btnSpine.transform.GetChild(0).gameObject, btnAniStr[1], false, () =>
            {                
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                SpineManager.instance.DoAnimation(uiSpine, uiAniStr[uiIndex], false, () =>
                {                   
                    SkeletonGraphic skeletonAnimation = homeGraphic.GetComponent<SkeletonGraphic>();
                    //skeletonAnimation.Skeleton.SetToSetupPose();
                    skeletonAnimation.AnimationState.ClearTracks();
                    //skeletonAnimation.AnimationName = null;
                    //houseAni.GetComponent<SkeletonAnimation>().AnimationState.SetEmptyAnimation(0, 0);           

                    switch (uiIndex)
                    {
                        case 1:
                            SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][0], false, () =>
                            {
                                SoundManager.instance.sheildGo.SetActive(false);
                            });
                            break;
                        case 0:
                        case 3:
                            SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][0], false, () =>
                            {
                                skeletonAnimation.AnimationState.ClearTracks();
                                SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][1], false, () =>
                                {
                                    SoundManager.instance.sheildGo.SetActive(false);
                                });
                            });
                            break;
                        case 2:
                            SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][0], false, () =>
                            {
                                skeletonAnimation.AnimationState.ClearTracks();
                                SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][1], false, () =>
                                {
                                    skeletonAnimation.AnimationState.ClearTracks();
                                    SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][2], false, () =>
                                    {
                                        SoundManager.instance.sheildGo.SetActive(false);
                                    });
                                });
                            });
                            break;
                        case 4:
                        case 5:
                            SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][0], false, () =>
                            {
                                skeletonAnimation.AnimationState.ClearTracks();
                                SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][1], false, () =>
                                {
                                    skeletonAnimation.AnimationState.ClearTracks();
                                    SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][2], false, () =>
                                    {
                                        skeletonAnimation.AnimationState.ClearTracks();
                                        SpineManager.instance.DoAnimation(homeGraphic, houseAniList[uiIndex][3], false, () =>
                                        {
                                            SoundManager.instance.sheildGo.SetActive(false);
                                            if (uiIndex == 5)
                                            {
                                                shineSpine.SetActive(true);
                                                SpineManager.instance.DoAnimation(shineSpine, "animation", true);
                                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                                                LogicManager.instance.ShowReplayBtn(true);
                                                btnSpine.GetComponent<Button>().enabled = false;
                                            }
                                        });
                                    });
                                });
                            });
                            break;
                    }
                });
            });                       
        }

        IEnumerator WaitTime(float time)
        {
            yield return new WaitForSeconds(time);

            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

            for (int i = 0; i < hammers.Length; i++)
            {
                hammers[i].transform.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimation(0, 0);
            }
            //Animation();
            HouseAnimation();
            mono.StopCoroutine(WaitTime(time));
        }

        void OnDisable()
        {
            LogicManager.instance.ShowReplayBtn(false);
        }

    }
}
