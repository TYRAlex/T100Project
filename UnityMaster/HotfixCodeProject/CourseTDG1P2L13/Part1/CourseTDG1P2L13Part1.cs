using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseTDG1P2L13Part1
    {
        int scoreCount;
        string[] scoreAniStr, paintAniStr;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject npc, part1, part2, scoreSpine, heavySpine, finishSpine;
        GameObject[] paints;
        Dictionary<int, GameObject[]> pointsDic;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;


            npc = curTrans.Find("npc").gameObject;
            part1 = curTrans.Find("gameScene/part1").gameObject;
            part2 = curTrans.Find("gameScene/part2").gameObject;
            finishSpine = curTrans.Find("gameScene/finishSpine").gameObject;
            heavySpine = part1.transform.Find("heavySpine").gameObject;
            scoreSpine = part2.transform.Find("score/scoreSpine").gameObject;
            paints = part2.transform.GetChildren(part2.transform.Find("paints").gameObject);
            pointsDic = new Dictionary<int, GameObject[]>() { { 0, part2.transform.GetChildren(part2.transform.Find("points/p0").gameObject) },
                                                              { 1, part2.transform.GetChildren(part2.transform.Find("points/p1").gameObject) },
                                                              { 2, part2.transform.GetChildren(part2.transform.Find("points/p2").gameObject) }};
            mono = curGo.GetComponent<MonoBehaviour>();
            scoreAniStr = new string[] { "s1", "s2", "s3", "s4", "s5", "s6", "s7", "s8", };
            paintAniStr = new string[] { "R1", "R2", "R3", "R4", "R5", "R6", "R7", "R8", "W1", "W2", "W3", "W4" };

            GameInit();
            GameStart();
        }

        void GameInit()
        {
            part1.SetActive(true);
            part2.SetActive(false);
            finishSpine.SetActive(false);
            scoreSpine.SetActive(false);
            scoreCount = -1;
            for (int i = 0; i < paints.Length; i++)
            {
                paints[i].SetActive(true);
                //paints[i].GetComponent<Image>().enabled = true;
                paints[i].transform.GetChild(0).gameObject.SetActive(false);
            }
            SpineManager.instance.DoAnimation(heavySpine, "animation", false);
        }

        void GameStart()
        {
            Util.AddBtnClick(part1, ClickPart1);
            SoundManager.instance.BgSoundPart2();
            LogicManager.instance.SetReplayEvent(() =>
            {
                GameInit();
                GameStart();
            });
        }

        void ClickPart1(GameObject btn)
        {
            btn.SetActive(false);
            part2.SetActive(true);
            scoreSpine.transform.parent.gameObject.SetActive(true);
            scoreSpine.transform.parent.GetComponent<RawImage>().enabled = true;
            int index = UnityEngine.Random.Range(0, 3);
            Debug.Log("index:" + index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            for (int i = 0; i < paints.Length; i++)
            {
                paints[i].transform.position = pointsDic[index][i].transform.position;               
                Util.AddBtnClick(paints[i], ClickPaint);
            }
        }

        void ClickPaint(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            int index = Convert.ToInt32((btn.name.Split('_'))[1]);
            btn.GetComponent<Image>().enabled = false;
            btn.transform.GetChild(0).gameObject.SetActive(true);

            if (index < 8)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            }

            SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, paintAniStr[index], false, () =>
            {
                if (index < 8)
                {
                    scoreCount++;
                    scoreSpine.transform.parent.gameObject.SetActive(true);
                    scoreSpine.SetActive(true);
                    scoreSpine.transform.parent.GetComponent<RawImage>().enabled = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                    SpineManager.instance.DoAnimation(scoreSpine, scoreAniStr[scoreCount], false, () =>
                    {
                        
                        btn.GetComponent<Image>().enabled = true;
                        btn.SetActive(false);

                        if (scoreCount > 6)
                        {
                            finishSpine.SetActive(true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            SpineManager.instance.DoAnimation(finishSpine, "animation", false, () =>
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                                SpineManager.instance.DoAnimation(finishSpine, "idle", true);
                                LogicManager.instance.ShowReplayBtn(true);
                            });
                        }
                        SoundManager.instance.sheildGo.SetActive(false);
                    });
                }
                else
                {                    
                    btn.GetComponent<Image>().enabled = true;
                    btn.transform.GetChild(0).gameObject.SetActive(false);
                    SoundManager.instance.sheildGo.SetActive(false);
                }
            });
        }
    }
}
