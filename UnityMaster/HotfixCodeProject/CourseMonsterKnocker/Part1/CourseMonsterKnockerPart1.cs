using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseMonsterKnockerPart1
    {
        GameObject curGo;
        GameObject uiPos, uiSpine, iconPos, iconSpine, bgSpine, npc, frame;
        GameObject[] uiToggle, iconToggle, showImage;
        Transform gameScene, bgBack;
        string[] uiSpineStr;
        List<string[]> iconSpineList;
        int ClickBtnCount = 3;


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            gameScene = curTrans.Find("gameScene").transform;
            bgBack = curTrans.Find("bgBack").transform;
            uiPos = curTrans.Find("bgBack/uiPos").gameObject;
            uiSpine = uiPos.transform.Find("uiSpine").gameObject;
            iconPos = curTrans.Find("bgBack/iconPos").gameObject;
            iconSpine = iconPos.transform.Find("iconSpine").gameObject;
            bgSpine = curTrans.Find("bg/bgSpine").gameObject;
            npc = curTrans.Find("npc").gameObject;
            frame = curTrans.Find("bg/frame").gameObject;
            uiToggle = curTrans.GetChildren(curTrans.Find("gameScene/uiToggle").gameObject);
            iconToggle = curTrans.GetChildren(curTrans.Find("gameScene/iconToggle").gameObject);
            showImage = curTrans.GetChildren(curTrans.Find("gameScene/showImage").gameObject);

            uiSpineStr = new string[] { "button_dz", "button_fs", "button_xh", "button_ms" };
            iconSpineList = new List<string[]>() {
                new string[] { "ms_1_ui", "ms_2_ui", "ms_3_ui", "ms_4_ui"},
                new string[] { "dz_1_ui", "dz_2_ui", "dz_3_ui", "dz_4_ui"},
                new string[] { "fs_1_ui", "fs_2_ui", "fs_3_ui", "fs_4_ui"},
                new string[] { "xh_1_ui", "xh_2_ui", "xh_3_ui", "xh_4_ui"},
            };

            SceneInit();
        }

        void SceneInit()
        {
            ClickBtnCount = 0;
            //SpineManager.instance.DoAnimation(iconSpine, iconSpineList[0][0], false);
            SpineManager.instance.DoAnimation(bgSpine, "animation", true);
            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0,
                () =>
                {
                    SoundManager.instance.sheildGo.SetActive(true);
                },
                () =>
                {
                    for (int i = 0; i < uiToggle.Length; i++)
                    {
                        int index = i;
                        // uiToggle[i].GetComponent<Button>().onClick.AddListener((bool value) => ChooseToggle(value, index));
                        uiToggle[i].GetComponent<Button>().onClick.AddListener(() => ChooseBtn(index));

                        uiToggle[i].GetComponent<Button>().interactable = false;
                        if (i == 0)
                        {
                            uiToggle[i].GetComponent<Button>().interactable = true;
                            uiToggle[i].transform.Find("Background").Find("Checkmark").gameObject.SetActive(false);
                        }
                    }
                    SoundManager.instance.sheildGo.SetActive(false);
                });

            for (int i = 0; i < iconToggle.Length; i++)
            {
                iconToggle[i].SetActive(false);
                for (int j = 0; j < iconToggle[i].transform.childCount; j++)
                {
                    Util.AddBtnClick(iconToggle[i].transform.GetChild(j).gameObject, ChangeIcon);
                }
            }

            //Close all Child of showImage
            frame.transform.localPosition = new Vector3(-1045, -62, 0);
            for (int i = 0; i < showImage.Length; i++)
            {
                for (int j = 0; j < showImage[i].transform.childCount; j++)
                {
                    showImage[i].transform.GetChild(j).gameObject.SetActive(false);
                }
            }


        }

        void ChooseBtn(int index)
        {
            for (int i = 0; i < uiToggle.Length; i++)
            {
                uiToggle[i].transform.Find("Background").Find("Checkmark").gameObject.SetActive(true);
            }

            if (ClickBtnCount < 3)
            {
                ClickBtnCount++;
            }
            uiToggle[ClickBtnCount].GetComponent<Button>().interactable = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SoundManager.instance.sheildGo.SetActive(true);
            uiPos.transform.position = uiToggle[index].transform.position;
            uiPos.SetActive(true);
            uiPos.transform.SetParent(gameScene);
            uiToggle[index].SetActive(false);
            SpineManager.instance.DoAnimation(uiSpine, uiSpineStr[index], false, () =>
            {
                uiPos.SetActive(false);
                uiToggle[index].SetActive(true);
                uiPos.transform.SetParent(bgBack);
                SoundManager.instance.sheildGo.SetActive(false);
            });


            frame.transform.DOMoveX(86f, 0.4f).SetEase(Ease.InExpo).OnComplete(() =>
            {
                ShowImage(iconToggle[index]);
            });
        }

        void ChangeIcon(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            Debug.Log("iconName:---" + btn.name);
            string[] iconArr = btn.name.Split('_');
            int index = Convert.ToInt32(iconArr[1]);
            int childIndex = Convert.ToInt32(iconArr[2]);

            btn.SetActive(false);
            iconPos.SetActive(true);
            iconPos.transform.SetParent(gameScene);
            iconPos.transform.position = btn.transform.position;
            ShowImage(showImage[index].transform.GetChild(childIndex).gameObject);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

            Debug.Log("Icon的动画:---" + iconSpineList[index][childIndex]);

            // int pIndex = 0;
            // if(index == 3)
            // {
            //     pIndex = 0;
            // }else
            // {
            //     pIndex = index;
            // }


            SpineManager.instance.DoAnimation(iconSpine, iconSpineList[index][childIndex], false, () =>
            {
                btn.SetActive(true);
                iconPos.transform.SetParent(bgBack);
                //iconPos.SetActive(false);
                SoundManager.instance.sheildGo.SetActive(false);
            });
        }

        void ShowImage(GameObject image)
        {
            GameObject father = image.transform.parent.gameObject;

            for (int i = 0; i < father.transform.childCount; i++)
            {
                GameObject go = father.transform.GetChild(i).gameObject;
                if (go != image)
                {
                    go.SetActive(false);
                }
                else
                {
                    go.SetActive(true);
                }
            }
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
