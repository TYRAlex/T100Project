using CoursePortraitPart1;
using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Bindings;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class CoursePortraitPart1
    {     
        GameObject curGo;

        int seconds;
        int scores;
        int decadeText, digitText;
        int startCutTime;
        bool isStart;
        float speed;
        float timer;
        float perTime;
        float allTime;
        int fiveCount;
        Image decade, digit, scoreDecade, scoreDigit;
        GameObject addScore, paintPool,startPainting, initPainting, scorePool, paintingCopy, startCutDown, testCube, cube;
        GameObject[] number, points, paintCopy, cutNumber;
        List<Painting> paintList;
        List<Painting> paintStartList;
        List<int> fiveList;
        Action Act;
        MonoBehaviour mono;

        enum POP_SPINE
        {
            Star0 = 0, Star1 = 1, Star2 = 2, Star3 = 3, Star4 = 4, Star5 = 5, PopUp = 6, Light = 7, Flower = 8, Again = 9, Close = 10
        }
        string[] popSpine;

        enum DD_SPINE
        {
            Sad1 = 0, Idle = 1, Fun_Shake = 2, Fun = 3, Win = 4, Fun_Jump = 5, Action = 6, Idle_Talk = 7, Sad = 8, Talk = 9 
        }
        string[] ddSpine;

        enum PAINTING_SPINE
        {
            Error = 0, Error2 = 1, Error3 = 2, Error4 = 3, Right1 = 4
        }
        string[] paintingSpine;

        GameObject overScene, over, overFront, overFrontFather, overBack, overImage, starOver, btnOver, mask, dingding, NPC, overFrontMask, overBackFather;
        GameObject btnCancel, btnAgain;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            mask = curTrans.Find("Mask").gameObject;
            NPC = curTrans.Find("NPC").gameObject;
            testCube = curTrans.Find("1_1").gameObject;
            cube = curTrans.Find("cube").gameObject;
            overFrontMask = curTrans.Find("overFrontMask").gameObject; ;
            number = GetChildren(curTrans.Find("gameScene/number").gameObject);
            points = GetChildren(curTrans.Find("gameScene/downPoints").gameObject);
            decade = curTrans.Find("gameScene/cutDown/seconds/decade").GetComponent<Image>();
            digit = curTrans.Find("gameScene/cutDown/seconds/digit").GetComponent<Image>();
            scoreDigit = curTrans.Find("gameScene/scoreIcon/score/scoreDigit").GetComponent<Image>();
            scoreDecade = curTrans.Find("gameScene/scoreIcon/score/scoreDecade").GetComponent<Image>();
            addScore = curTrans.Find("gameScene/addScore").gameObject;
            paintPool = curTrans.Find("gameScene/paintPool").gameObject;
            startPainting = paintPool.transform.Find("painting").gameObject;
            initPainting = paintPool.transform.Find("initPainting").gameObject;
            scorePool = curTrans.Find("gameScene/scorePool").gameObject;
            startCutDown = curTrans.Find("gameScene/startCutDown").gameObject;
            cutNumber = GetChildren(startCutDown.transform.Find("cutNumber").gameObject);
            popSpine = new string[] { "star_0", "star_1", "star_2", "star_3", "star_4", "star_5", "popup","light", "lizi", "button_again", "button_close"};
            ddSpine = new string[] { "sad", "breath", "fun", "fun_1", "win", "win", "unhappy", "talk", "sad", "talk"};
            paintingSpine = new string[] { "erro", "erro2", "erro3", "erro4", "right1" };

            scores = 6;
            
            timer = 0;
            allTime = 0;
            paintList = new List<Painting>();
            paintStartList = new List<Painting>();
            fiveList = new List<int>();
            mono = curTrans.GetComponent<MonoBehaviour>();
            addScore.SetActive(false);
            SoundManager.instance.Speaking(NPC, ddSpine[(int)DD_SPINE.Idle_Talk], SoundManager.SoundType.VOICE, 3);
            Util.AddBtnClick(mask, CutScene);

            //game over
            overScene = curTrans.Find("overScene").gameObject;
            over = overScene.transform.Find("over").gameObject;
            overFrontFather = overScene.transform.Find("overFront").gameObject;
            overFront = overScene.transform.Find("overFront/overFront1").gameObject;
            overBackFather = overScene.transform.Find("overBack").gameObject;
            overBack = overScene.transform.Find("overBack/overBack1").gameObject;
            starOver = overScene.transform.Find("starOver").gameObject;
            dingding = overScene.transform.Find("dingding").gameObject;
            overImage = overScene.transform.Find("overImage").gameObject;
            btnOver = overScene.transform.Find("btnOver").gameObject;
            //btnCancel = btnOver.transform.Find("btn_cancel").gameObject;
            btnAgain = btnOver.transform.Find("btn_again").gameObject;
            Util.AddBtnClick(btnAgain, ClickAgain);
            //Util.AddBtnClick(btnCancel, ClickCancel);
            //CutScene(btnAgain);  
            pt = new Painting(testCube, 1, 20);
            pt.painting.SetActive(true);
        }
        int speedTime;
        Painting pt;
        void Update()
        {

            //testCube.transform.Translate(Vector3.down * 3f);
            //if (testCube.transform.position.y < 0)
            //{
            //    testCube.transform.position = new Vector3(900f, 1200f, 0);
            //}
            //cube.transform.Translate(Vector3.down * 20f);
            //if (cube.transform.position.y < 0)
            //{
            //    cube.transform.position = new Vector3(900f, 1200f, 0);
            //}
            //pt.Move();
            //if (isStart)
            //{
            //    if (Act != null)
            //    {
            //        Act();
            //    }

            //    timer += Time.deltaTime;
            //    allTime += Time.deltaTime;

            //    if (timer > perTime)
            //    {
            //        Painting pt = CreatPainting();
            //        paintList.Add(pt);
            //        timer = 0;
            //    }
            //    if (allTime > 1)
            //    {
            //        GameCutDown();
            //        allTime = 0;

            //        speedTime++;

            //        if (speedTime % 5 == 0)
            //        {
            //            speed += 3f;
            //            perTime = Mathf.Lerp(perTime, 0, 0.2f);
            //        }
            //    }
            //    //if (isTrue)
            //    //{
            //    //    Painting pt = CreatPainting();
            //    //    paintList.Add(pt);
            //    //    timer = 0;
            //    //    isTrue = false;
            //    //}
            //    //Debug.Log("update:"+ paintStartList[4].painting.name + "---"+ paintStartList[4].isPortrait);
            //    CheckClick();             
            //}            
        }

        void FixedUpdate()
        {
            if (isStart)
            {
                if (Act != null)
                {
                    Act();
                }

                timer += Time.deltaTime;
                allTime += Time.deltaTime;

                if (timer > perTime)
                {
                    Painting pt = CreatPainting();
                    paintList.Add(pt);
                    timer = 0;
                }
                if (allTime > 1)
                {
                    GameCutDown();
                    allTime = 0;

                    speedTime++;

                    if (speedTime % 5 == 0)
                    {
                        speed += 3f;
                        perTime = Mathf.Lerp(perTime, 0, 0.2f);
                    }
                }
                //if (isTrue)
                //{
                //    Painting pt = CreatPainting();
                //    paintList.Add(pt);
                //    timer = 0;
                //    isTrue = false;
                //}
                //Debug.Log("update:"+ paintStartList[4].painting.name + "---"+ paintStartList[4].isPortrait);
                CheckClick();
            }
        }

        void CutScene(GameObject btn)
        {
            SoundManager.instance.Stop();
            mask.SetActive(false);
            overScene.SetActive(false);
            btnOver.SetActive(false);
            starOver.SetActive(false);
            //overFront.SetActive(false);
            //overBack.SetActive(false);
            dingding.SetActive(false);
            overImage.SetActive(false);
            scoreDecade.gameObject.SetActive(false);
            //overFront.transform.localScale = Vector3.zero;
            //overBack.transform.localScale = Vector3.zero;
            scores = 0;
            //scores--;
            fiveCount = 0;
            seconds = 15;
            speed = 14f;
            perTime = 1f;
            //speed += 2;
            //perTime = 0.9f;
            // perTime = Mathf.Lerp(perTime, 0, 0.1f);
            //Debug.LogError("per:" + perTime);
            scoreDigit.sprite = number[scores].GetComponent<Image>().sprite;
            decade.sprite = number[seconds / 10].GetComponent<Image>().sprite;
            digit.sprite = number[seconds % 10].GetComponent<Image>().sprite;

            AddStartList();
            startCutTime = 4;
            startCutDown.SetActive(true);
            startCutDown.transform.GetChild(0).gameObject.SetActive(true);
            startCutDown.transform.GetChild(2).gameObject.SetActive(true);
            mono.StartCoroutine(StartCutDown());

            //overFront.transform.localScale = Vector3.one / 2;
            SpineManager.instance.DoAnimation(overFront, popSpine[0], false);
            overFront.transform.SetParent(overFrontMask.transform);
            //overBack.transform.localScale = Vector3.one / 2;
            SpineManager.instance.DoAnimation(overBack, popSpine[0], false);
            overBack.transform.SetParent(overFrontMask.transform);

            SpineManager.instance.PlayAnimationDuring(overFront, popSpine[8], "0|0");
            SpineManager.instance.PlayAnimationDuring(overBack, popSpine[0], "0|0");
        }

        IEnumerator StartCutDown()
        {
            while (startCutTime > 0)
            {                
                startCutTime--;
                if (startCutTime == 0)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 9, false);
                    startCutDown.transform.GetChild(0).gameObject.SetActive(false);
                    startCutDown.transform.GetChild(2).gameObject.SetActive(false);
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8, false);                    
                }
                for (int i = cutNumber.Length - 1; i >= 0; i--)
                {
                    if (startCutTime == i)
                    {
                        cutNumber[i].SetActive(true);
                    }
                    else
                    {
                        cutNumber[i].SetActive(false);
                    }
                }
                yield return new WaitForSeconds(1f);
            }

            startCutDown.SetActive(false);
            StartGame();
            mono.StopCoroutine(StartCutDown());
        }

        void StartGame()
        {
            isStart = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = 0.3f;
        }

        void AddStartList()
        {
            paintingCopy = GameObject.Instantiate(startPainting);
            paintingCopy.transform.SetParent(paintPool.transform);
            paintCopy = GetChildren(paintingCopy);

            for (int i = 0; i < paintCopy.Length; i++)
            {
                //if ((i + 1) % 5 == 0)
                //{
                //    Painting pt = new Painting(paint[i], 1, speed);
                //    //Util.AddBtnClick(pt.image, Click);
                //    paintStartList.Add(pt);
                //}
                Painting pt = new Painting(paintCopy[i], 1, speed);
                //Util.AddBtnClick(pt.image, Click);
                paintStartList.Add(pt);

            }
        }

        void CheckClick()
        {
            int inx = paintList.Count;
            if (inx > 0)
            {
                for (int i = 0; i < paintList.Count; i++)
                {
                    //Debug.Log("isPortrait:" + i + ":"+ paintList[i].isPortrait);
                    if (paintList[i].isPortrait == 2)
                    {
                        //arr.Add(i);
                        //Debug.Log("click1:"+ paintList[i].painting.name);
                        AddScore(paintList[i]);
                        i--;

                        //paintList[i].painting.transform.SetParent(startPainting.transform);
                        //paintStartList.Add(paintList[i]);
                        //paintList.RemoveAt(i);                       
                        //Act -= paintList[i].Move;
                    }
                    else if (paintList[i].isEnd == true)
                    {
                        //arr.Add(i);
                        //Debug.Log("click2:" + paintList[i].painting.name);
                        //paintList[i].painting.transform.SetParent(startPainting.transform);
                        //paintList[i].painting.transform.localPosition = Vector3.zero; 
                        paintList[i].isEnd = false;
                        paintStartList.Add(paintList[i]);
                        Act -= paintList[i].Move;
                        paintList.RemoveAt(i);
                        i--;

                        //Debug.LogError("checklist:" + paintList.Count);
                        //Debug.LogError("initpainting:" + initPainting.transform.childCount);
                    }

                    //if (paintList[i].isPortrait == 1)
                    //{
                    //    MinusScore(paintList[i]);
                    //}
                }                
            }
            
        }               
        
        Painting CreatPainting()
        {
            int index = UnityEngine.Random.Range(0, paintStartList.Count);
            fiveCount++;
            if (fiveCount % 4 == 0)
            {
                if (!fiveList.Contains(5))
                {
                    while (Convert.ToInt32((paintStartList[index].painting.name.Split('_'))[1]) != 5)
                    {
                        index = UnityEngine.Random.Range(0, paintStartList.Count);
                        Debug.Log("index:" + paintStartList[index].painting.name);
                    }                    
                }
                Debug.Log("paintStartList[index].painting.name:" + paintStartList[index].painting.name);
                fiveList.Clear();
                //fiveCount = 0;
            }
            Debug.Log("fivecount:" + fiveCount);           
            Painting pt = paintStartList[index];
            pt.painting.transform.position = points[UnityEngine.Random.Range(0, points.Length)].transform.position +
                                                                        new Vector3(UnityEngine.Random.Range(-100f, 100f), 0, 0);
            pt.painting.transform.SetParent(initPainting.transform);
            pt.painting.SetActive(true);
            pt.speed = speed;
            paintStartList.RemoveAt(index);

            int inx =Convert.ToInt32((pt.painting.name.Split('_'))[1]);
            fiveList.Add(inx);

            Act += pt.Move;
            return pt;
        }

        void MinusScore(Painting add)
        {
            scores -= add.score;

            if (scores < 0)
            {
                scores = 0;
            }
            int decade = scores / 10;
            int digit = scores % 10;
            scoreDigit.sprite = number[digit].GetComponent<Image>().sprite;

            if (decade == 0)
            {
                scoreDecade.gameObject.SetActive(false);
                //scoreDecade.sprite = number[decade].GetComponent<Image>().sprite;
            }
        }

        void AddScore(Painting add)
        {
            GameObject go = null;

            if (scorePool.transform.childCount > 0 && scorePool.transform.GetChild(0).gameObject.activeSelf == false)
            {
                go = scorePool.transform.GetChild(0).gameObject;
            }
            else
            {
                go = GameObject.Instantiate(addScore);
                go.transform.SetParent(scorePool.transform);
            }
            
            AddScore ad = new AddScore(add.painting.transform.position, go);            

            scores += add.score;
            int decade = scores / 10;
            int digit = scores % 10;
            scoreDigit.sprite = number[digit].GetComponent<Image>().sprite;

            if (decade > 0)
            {
                scoreDecade.gameObject.SetActive(true);
                scoreDecade.sprite = number[decade].GetComponent<Image>().sprite;
            }

            //add.speed = speed;
            paintStartList.Add(add);
            Act -= add.Move;
            paintList.Remove(add);
            mono.StartCoroutine(WaitTime(go));
            //go.SetActive(false);
        }

        IEnumerator WaitTime(GameObject go)
        {            
            yield return new WaitForSeconds(1f);
            go.SetActive(false);
            mono.StopCoroutine(WaitTime(go));
        }

        void GameCutDown()
        {
            if (seconds > 0)
            {
                seconds--;
                decadeText = seconds / 10;
                digitText = seconds % 10;

                decade.sprite = number[decadeText].GetComponent<Image>().sprite;
                digit.sprite = number[digitText].GetComponent<Image>().sprite;
            }
            else
            {
                Debug.Log("gamecutdowmover");
                GameOver();
            }            
        }

        void Click(GameObject btn)
        {
            Debug.Log("clickkkkkk");
            //string[] str = btn.transform.parent.name.Split('_');
            Painting paint = null;
            if (paintList.Count > 0)
            {
                for (int i = 0; i < paintList.Count; i++)
                {
                    if (paintList[i].painting.name == btn.transform.parent.name)
                    {
                        paint = paintList[i];
                        break;
                    }                    
                }
            }
            Debug.Log("Clik0");
            paint.image.SetActive(false);
            //paint.ani.SetActive(true);
            paint.ani.transform.localScale = paint.aniScale;
            Debug.Log("Clik0.5111");
            SpineManager.instance.DoAnimation(paint.ani, paintingSpine[paint.index], false, () =>
            {
                Debug.Log("Clik0.5");
                if (paint.index == 4)
                {
                    Debug.Log("Clik1");
                    paint.isPortrait = 2;
                    AddScore(paint);
                }

                paint.ani.transform.localScale = Vector3.zero;
                paint.image.SetActive(true);
                //else
                //{
                //    Debug.Log("Clik2");
                //    //paint.ani.SetActive(false);
                //    //paint.image.SetActive(true);

                //    Debug.Log(paint.image.name);
                //}
                //Debug.Log("Clik2.5");
            });
            Debug.Log("Clik3");
        }

        void GameOver()
        {
            isStart = false;
            overScene.SetActive(true);
            
            int scoreInx = Mathf.Clamp(scores, 0, 5);
            Debug.Log("scoreInx:" + scoreInx);
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SpineManager.instance.DoAnimation(over, popSpine[(int)POP_SPINE.PopUp], false, () =>
            {
                dingding.SetActive(true);
                
                if (scoreInx > 0)
                {
                    SpineManager.instance.DoAnimation(dingding, ddSpine[scoreInx], true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(over, popSpine[scoreInx], false, () =>
                    {
                        
                        //overFront.SetActive(false);
                        //overFront.SetActive(true);
                        //overBack.SetActive(true);
                        if (scoreInx > 1)
                        {
                            overBack.transform.SetParent(overBackFather.transform);                            
                            //overBack.transform.localScale = Vector3.one / 2;
                            //SpineManager.instance.DoAnimation(overBack, popSpine[(int)POP_SPINE.Light], false);
                            SpineManager.instance.PlayAnimationDuring(overBack, popSpine[(int)POP_SPINE.Light], "0|0.5");
                            //overFront.SetActive(true);
                            Debug.Log("over21");
                            
                            //overBack.transform.localScale = Vector3.zero;
                        }
                        if (scoreInx > 4)
                        {
                            //overFront.SetActive(true);
                            //overFront.transform.localScale = Vector3.one / 2;
                            overFront.transform.SetParent(overFrontFather.transform);
                            Debug.Log("over3");
                            //SpineManager.instance.DoAnimation(overFront, popSpine[0], false);
                            //SpineManager.instance.PlayAnimationDuring(overFront, popSpine[(int)POP_SPINE.Flower], "0|0");
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                            SpineManager.instance.DoAnimation(overFront, popSpine[(int)POP_SPINE.Flower], false, () =>
                            {
                                Debug.Log("over31");
                                btnOver.SetActive(true);
                            });
                        }
                        
                        btnOver.SetActive(true);
                       // btnCancel.SetActive(true);
                    });                    
                }
                else
                {
                    overFront.transform.SetParent(overFrontFather.transform);
                    //overFront.transform.localScale = Vector3.one / 2;
                    //overFront.SetActive(true);
                    
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    SpineManager.instance.DoAnimation(overFront, popSpine[scoreInx],false);
                    SpineManager.instance.DoAnimation(dingding, ddSpine[scoreInx], false);
                    SpineManager.instance.DoAnimation(over, popSpine[scoreInx], false, () =>
                    {
                        //overFront.transform.SetParent(overScene.transform);
                        btnOver.SetActive(true);
                        //btnCancel.SetActive(true);
                    });
                    
                }

                //else
                //{
                //    SpineManager.instance.DoAnimation(dingding, ddSpine[scoreInx], false);

                //    over.SetActive(false);
                //    overImage.SetActive(true);
                //    btnOver.SetActive(true);
                //    btnCancel.SetActive(true);
                //    overFront.SetActive(false);
                //}
            });

            Debug.Log(paintList.Count);
            int inx = paintList.Count;
            for (int i = 0; i < inx; i++)
            {
                //Debug.LogError("paintList[i].painting.name:" + paintList[i].painting.name);
                paintList[i].painting.transform.SetParent(paintingCopy.transform);
                //Debug.LogError("paintingCopy.name:" + paintingCopy.name);
                Act -= paintList[i].Move;
            }

            Debug.Log("GameOver");
        }

        void ClickAgain(GameObject btn)
        {
            overFront.transform.SetParent(overFrontFather.transform);
            //overFront.transform.localScale = Vector3.zero;
            Act = null;
            paintStartList.Clear();
            paintList.Clear();
            fiveList.Clear();
            GameObject.Destroy(paintingCopy);
            paintCopy = null;

            btn.SetActive(false);
            //overFront.SetActive(true);
           // overFront.transform.localScale = Vector3.one / 2;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            SpineManager.instance.DoAnimation(overFront, popSpine[(int)POP_SPINE.Again], false, () =>
            {
                CutScene(btn);
                btn.SetActive(true);
                //SpineManager.instance.PlayAnimationDuring(overFront, popSpine[(int)POP_SPINE.Again], "0|0");
                //over.SetActive(true);
            });
        }

        void ClickCancel(GameObject btn)
        {
            overFront.transform.SetParent(overFrontFather.transform);
            btn.SetActive(false);
            //overFront.transform.localScale = Vector3.one / 2;
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            SpineManager.instance.DoAnimation(overFront, popSpine[(int)POP_SPINE.Close], false, () =>
            {
                //SpineManager.instance.PlayAnimationDuring(overFront, popSpine[(int)POP_SPINE.Close], "0|0");
                btn.SetActive(true);
            });
            Debug.Log("--------ClickCancel-------------");
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
