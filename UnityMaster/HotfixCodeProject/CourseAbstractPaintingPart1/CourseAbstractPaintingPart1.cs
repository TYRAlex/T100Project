using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CourseAbstractPaintingPart1
    {
        enum CAT_SPINE
        {
            Mustache = 0, Ear = 1, Fur = 2, Head = 3, Body1 = 4, Tail = 5, Foot = 6, Square = 7, Body2 = 8
        }
        string[] catSpine;

        enum DD_SPINE
        {
            Breath = 0, Breath_Talk = 1, Talk = 2
        }
        string[] ddSpine;

        GameObject curGo;
        GameObject npc, cat, gameScene, tailPos, part1, part2, part3;
        GameObject[] catBtn;
        GameObject CatFront;

        int count, clickCount, part, soundIndex;
        bool isStart;
        ILObject3DAction obj;
        float startX, endX, lengthDrag;
        Dictionary<int, GameObject> organicDic;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            part1 = curTrans.transform.Find("part1").gameObject;
            part2 = curTrans.transform.Find("part2").gameObject;
            part3 = curTrans.transform.Find("part3").gameObject;

            npc = curTrans.transform.Find("part2/NPC").gameObject;
            cat = curTrans.transform.Find("part2/catSpine").gameObject;
            gameScene = curTrans.transform.Find("part2/gameScene").gameObject;
            tailPos = curTrans.transform.Find("part2/tailPos").gameObject;
            catBtn = GetChildren(gameScene);
            CatFront = part2.transform.Find("CatFront").gameObject;

            clickCount = 0;
            isStart = false;
            part = 0;
            //lengthDrag = Screen.width / 8f;
            //obj = curTrans.Find("Drag").GetComponent<ILObject3DAction>();
            organicDic = new Dictionary<int, GameObject>();
            mono = curGo.GetComponent<MonoBehaviour>();
            catSpine = new string[] { "mustache", "ear", "duanmao", "head", "body", "tail", "foot", "bu", "body1" };
            ddSpine = new string[] { "breath", "talk", "talk" };



            //obj.OnMouseDownLua = OnMouseDown;
            //obj.OnMouseUpLua = OnMouseUp;


            ChangePart();
        }

        //void OnMouseDown(int index)
        //{
        //    startX = Input.mousePosition.x - Screen.width / 2;
        //    Debug.Log(startX);
        //}

        //void OnMouseUp(int index)
        //{
        //    endX = Input.mousePosition.x - Screen.width / 2;
        //    Debug.Log(endX);
        //    ChangePart();
        //}

        void ChangePart()
        {

            if (part == 0)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
                part1.SetActive(false);
                part2.SetActive(true);
                part = -1;
                StartGame();

            }
            else if (part == 1)
            {
                SoundManager.instance.Stop();
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
                part2.SetActive(false);
                part3.SetActive(true);
                part = 2;
            }
            else if (part == 2)
            {
                Debug.Log("next level");
            }

            //if (endX - startX < 0)
            //{
            //    if (part == 0)
            //    {
            //        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
            //        part1.SetActive(false);
            //        part2.SetActive(true);
            //        part = -1;
            //        StartGame();

            //    }
            //    else if (part == 1)
            //    {
            //        SoundManager.instance.Stop();
            //        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);
            //        part2.SetActive(false);
            //        part3.SetActive(true);
            //        part = 2;
            //    }
            //    else if (part == 2)
            //    {
            //        Debug.Log("next level");
            //    }
            //}
            //else if(endX - startX > 0)
            //{
            //    if (part == 1)
            //    {
            //        part = 0;
            //        part1.SetActive(true);
            //        part2.SetActive(false);
            //    }
            //    else if (part == 2)
            //    {
            //        part = 1;
            //        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            //        part3.SetActive(false);
            //        part2.SetActive(true);
            //        StartGame();
            //    }
            //}
        }

        void StartGame()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.Speaking(npc, ddSpine[(int)DD_SPINE.Talk], SoundManager.SoundType.VOICE, 8, null, Init);
        }

        void Init()
        {
            CatFront.SetActive(false);
            cat.SetActive(true);
            npc.SetActive(true);
            SpineManager.instance.DoAnimation(npc, ddSpine[(int)DD_SPINE.Breath], true);
            for (int i = 0; i < catBtn.Length; i++)
            {
                catBtn[i].SetActive(false);
                string[] str = catBtn[i].name.Split('_');
                int index = Convert.ToInt32(str[1]);
                organicDic.Add(index, catBtn[i]);
            }
            isStart = true;
        }

        void Update()
        {
            Click();
        }

        void Click()
        {
            if (isStart)
            {
                isStart = false;
                for (int i = 0; i < organicDic.Count; i++)
                {
                    if (i == clickCount)
                    {
                        count = i;
                        //gameScene.SetActive(true);
                        organicDic[count].SetActive(true);

                        SoundManager.instance.Speaking(npc, ddSpine[UnityEngine.Random.Range(1, 3)], SoundManager.SoundType.VOICE, i, null, () =>
                        {
                            Debug.Log(organicDic[count].name);
                            Debug.Log(organicDic[count].transform.childCount);
                            for (int j = 0; j < organicDic[count].transform.childCount; j++)
                            {
                                Image image = organicDic[count].transform.GetChild(j).GetComponent<Image>();
                                if (image.sprite.texture.isReadable == true)
                                {
                                    image.alphaHitTestMinimumThreshold = 0.1f;
                                }
                                Util.AddBtnClick(organicDic[count].transform.GetChild(j).gameObject, PlayAnimation);
                            }
                        });
                    }
                }
            }
        }

        void PlayAnimation(GameObject btn)
        {
            mono.StartCoroutine(WaitTime(btn));
            //gameScene.SetActive(false);
        }

        IEnumerator WaitTime(GameObject btn)
        {
            GameObject father = btn.transform.parent.gameObject;
            int index = Convert.ToInt32((father.name.Split('_'))[1]);
            father.SetActive(false);
            soundIndex = index;
            if (index == 4)
            {
                if (btn.name == "body_2")
                {
                    index = 8;
                    organicDic[5].transform.GetChild(0).gameObject.SetActive(false);
                    //organicDic[clickCount + 1].transform.GetChild(0).position = tailPos.transform.position;
                    //organicDic[clickCount + 1].transform.GetChild(0).rotation = tailPos.transform.rotation;
                }
                else
                {
                    organicDic[5].transform.GetChild(1).gameObject.SetActive(false);
                }
            }

            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index);
            //SpineManager.instance.DoAnimation(cat, catSpine[index], true);
            yield return new WaitForSeconds(1f);

            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, soundIndex, () =>
            {
                if (index < 7)
                {
                    SpineManager.instance.DoAnimation(cat, catSpine[index], true);
                }
                else
                {
                    SpineManager.instance.DoAnimation(cat, catSpine[index], true);
                }

            }, () =>
            {
                Debug.Log("SoundIndex : " + soundIndex);
                isStart = true;
                clickCount++;
                if (clickCount == 8)
                {
                    part = 1;
                }

                if (index < 7)
                {
                    SpineManager.instance.DoAnimation(cat, catSpine[index], true);
                }
                else
                {
                    if (clickCount != 5)
                    {
                        Debug.Log(clickCount);
                        Debug.Log("11111 + " + "结束");
                        SpineManager.instance.DoAnimation(cat, catSpine[index], false);
                        cat.SetActive(false);
                        CatFront.SetActive(true);
                    }

                }

            });


            Debug.Log("clickCount:" + clickCount);
            mono.StopCoroutine(WaitTime(btn));
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
