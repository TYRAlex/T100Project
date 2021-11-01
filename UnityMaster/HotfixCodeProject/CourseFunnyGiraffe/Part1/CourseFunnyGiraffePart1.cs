using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseFunnyGiraffePart1
    {        
        int dirIndex, organIndex;
        string[] clickStr;
        List<GameObject[]> lOrganList, rOrganList;
        Dictionary<int, GameObject[]> giraffeDic;
        Dictionary<int, List<GameObject[]>> organDic;
        Dictionary<int, int[]> indexDic;
        GameObject curGo, npcTT, npcDD, uiBtn, leftBtn, rightBtn, backgroundBtn;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            uiBtn = curTrans.Find("GameScene/uiBtn").gameObject;
            npcTT = curTrans.Find("npc/ttSpine").gameObject;
            npcDD = curTrans.Find("npc/ddSpine").gameObject;
            leftBtn = uiBtn.transform.Find("leftBtn_0/leftImg_0").gameObject;
            rightBtn = uiBtn.transform.Find("rightBtn_1/rightImg_1").gameObject;
            backgroundBtn = curTrans.Find("background/background0").gameObject;

            giraffeDic = new Dictionary<int, GameObject[]>() { { 0, curTrans.GetChildren(curTrans.Find("GameScene/rGiraffe_0").gameObject)},
                                                               { 1, curTrans.GetChildren(curTrans.Find("GameScene/lGiraffe_1").gameObject)} };
            organDic = new Dictionary<int, List<GameObject[]>>();
            lOrganList = new List<GameObject[]>();
            rOrganList = new List<GameObject[]>();
            clickStr = new string[] { "ui_l", "ui_r"};
            dirIndex = 0;
            organIndex = 0;
            indexDic = new Dictionary<int, int[]>() { { 0, new int[] { 0, 0, 0, 0} },
                                                      { 1, new int[] { 0, 0, 0, 0} } };

            SceneInit();
        }

        void SceneInit()
        {
            //SoundManager.instance.Speaking(npcTT, "talk", SoundManager.SoundType.VOICE, 0);
            SoundManager.instance.Speaking(npcDD, "animation2", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                SoundManager.instance.bgmSource.volume = 0.3f;
            });

            //SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 0, null, () =>
            //{
            //    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //    SoundManager.instance.bgmSource.volume = 0.3f;
            //});

            for (int i = 0; i < giraffeDic[0].Length; i++)
            {
                rOrganList.Add(giraffeDic[0][i].transform.GetChildren(giraffeDic[0][i]));
                lOrganList.Add(giraffeDic[1][i].transform.GetChildren(giraffeDic[1][i]));
            }
            organDic.Add(0, rOrganList);
            organDic.Add(1, lOrganList);            

            for (int i = 0; i < giraffeDic[0].Length; i++)
            {
                Util.AddBtnClick(giraffeDic[0][i], ShowBotton);
                Util.AddBtnClick(giraffeDic[1][i], ShowBotton);
            }

            Util.AddBtnClick(leftBtn, ChangeEmoji);
            Util.AddBtnClick(rightBtn, ChangeEmoji);
            Util.AddBtnClick(backgroundBtn, (o) =>
            {
                uiBtn.SetActive(false);
            });            
        }

        void ShowBotton(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            string[] arr = btn.name.Split('_');
            dirIndex = Convert.ToInt32(arr[1]);
            organIndex = Convert.ToInt32(arr[2]);
            
            uiBtn.SetActive(true);
            uiBtn.transform.position = btn.transform.position;
            Debug.LogFormat("btn.name:{0},dirIndex:{1},organIndex:{2}", btn.name, dirIndex, organIndex);
        }

        void ChangeEmoji(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            btn.SetActive(false);
            string[] arr = btn.name.Split('_');
            int index = Convert.ToInt32(arr[1]);
            GameObject tempObj = btn.transform.parent.GetChild(0).gameObject;
            tempObj.SetActive(true);
            SoundManager.instance.sheildGo.SetActive(true);
            SpineManager.instance.DoAnimation(tempObj, clickStr[index], false, () =>
            {
                tempObj.SetActive(false);
                btn.SetActive(true);
                SoundManager.instance.sheildGo.SetActive(false);
            });

            if (index == 0)
            {
                indexDic[dirIndex][organIndex]--;
                if (indexDic[dirIndex][organIndex] < 0)
                {
                    indexDic[dirIndex][organIndex] = giraffeDic[dirIndex][organIndex].transform.childCount - 1;
                }
            }
            else if (index == 1)
            {
                indexDic[dirIndex][organIndex]++;
                if (indexDic[dirIndex][organIndex] > giraffeDic[dirIndex][organIndex].transform.childCount - 1)
                {
                    indexDic[dirIndex][organIndex] = 0;
                }
            }
            Debug.LogFormat("btn2.name:{0},dirIndex:{1},organIndex:{2},indexDic[dirIndex][organIndex]:{3}", btn.name, dirIndex, organIndex, indexDic[dirIndex][organIndex]);
            curGo.ShowGameObject(organDic[dirIndex][organIndex][indexDic[dirIndex][organIndex]]);
            Debug.Log("organDic[dirIndex][organIndex][indexDic[dirIndex][organIndex]]:" + organDic[dirIndex][organIndex][indexDic[dirIndex][organIndex]]);
        }
    }
}
