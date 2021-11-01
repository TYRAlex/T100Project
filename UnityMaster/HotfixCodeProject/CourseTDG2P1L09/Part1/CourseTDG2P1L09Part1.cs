using CourseTDG2P1L09Part1;
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
    public class CourseTDG2P1L09Part1
    {
        enum Butterfly_Spine
        {
            Drag = 0, Right = 1, Error = 2, Idle = 3, Fly1 = 4, Fly2 = 5, Stop = 6, UI_Idle = 7
        }        
        string[] butterflyAniStr, skinStr;

        int butterflyCount;
        ILDrager[] objActs;
        SkeletonGraphic[] objAnis;
        Butterfly[] butterflys;
        Vector3[] initPos;
        Quaternion[] initRot;
        Dictionary<char, GameObject[]> pointsDic;

        Camera sceneCamera;
        MonoBehaviour mono;
        GameObject curGo, npc, finishScene, butterflyDrager, butterflyDragerClone;
        GameObject[] butterflyObj;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            butterflyDrager = curTrans.Find("GameScene/Butterfly_Drager").gameObject;
            npc = curTrans.Find("npc").gameObject;
            finishScene = curTrans.Find("GameScene/finishScene").gameObject;
            sceneCamera = curTrans.Find("GameScene").GetComponent<Camera>();
            mono = curGo.GetComponent<MonoBehaviour>();
            butterflyAniStr = new string[] { "ui", "right", "error", "1", "animation", "animation2", "animation3", "ui_idle" };
            skinStr = new string[] { "c1", "c2", "c3", "c4", "c5", "c6", "c7", "c8", "c9", "w1", "w2", "w3", "w4", "w5", "w6", "w7", "w8", "w9" };
            
            pointsDic = new Dictionary<char, GameObject[]>() { { 'w', curTrans.GetChildren(curTrans.Find("GameScene/points/warm").gameObject) },
                                                               { 'c', curTrans.GetChildren(curTrans.Find("GameScene/points/cold").gameObject)} };
            initPos = new Vector3[] { new Vector3(-650.1f, -434.2f, 0), new Vector3(-388.7f, -414.3f, 0), new Vector3(-124.4f, -424.2f, 0),
                                      new Vector3(136.9f, -416f, 0), new Vector3(399.4f, -424.7f, 0), new Vector3(657f, -427.7f, 0)};
            initRot = new Quaternion[] { Quaternion.Euler(-90, -90, -4.54f), Quaternion.Euler(-90, -90, -0.114f), Quaternion.Euler(-90, -90, -0.114f),
                                         Quaternion.Euler(-90, -90, -0.114f), Quaternion.Euler(-90, -90, -0.114f), Quaternion.Euler(-90, -90, -0.114f)};            

            GameInit();
            GameStart();
        }

        void Update()
        {
            for (int i = 0; i < butterflys.Length; i++)
            {
                if (butterflys[i].isEnd && butterflys[i].isPlayEnd)
                {
                    butterflys[i].RandomFly();
                }
            }
        }

        void GameInit()
        {            
            butterflyDragerClone = null;
            butterflyDrager.SetActive(false);
            butterflyDragerClone = GameObject.Instantiate(butterflyDrager);
            butterflyDragerClone.transform.SetParent(sceneCamera.transform);
            butterflyDragerClone.transform.SetSiblingIndex(3);
            butterflyDragerClone.transform.localPosition = new Vector3(0, 0, 5);
            butterflyDragerClone.SetActive(true);
            butterflyObj = curGo.transform.GetChildren(butterflyDragerClone);

            objActs = null;
            objAnis = null;
            butterflys = null;

            objActs = new ILDrager[6];
            objAnis = new SkeletonGraphic[6];
            butterflys = new Butterfly[6];
            for (int i = 0; i < butterflyObj.Length; i++)
            {
                objAnis[i] = butterflyObj[i].transform.GetChild(0).transform.GetChild(0).GetComponent<SkeletonGraphic>();
                objActs[i] = butterflyObj[i].transform.GetChild(0).GetComponent<ILDrager>();
            }

            int[] randomSkin = RandomNumber();

            npc.SetActive(false);
            finishScene.SetActive(false);
            butterflyCount = 0;
            
            LogicManager.instance.ShowReplayBtn(false);

            for (int i = 0; i < butterflyObj.Length; i++)
            {
                butterflyObj[i].transform.localPosition = initPos[i];
                butterflyObj[i].transform.localRotation = initRot[i];
                butterflyObj[i].transform.localScale = Vector3.one;

                objAnis[i].initialSkinName = skinStr[randomSkin[i]];
                objAnis[i].Initialize(true);

                AnimationState ani = objAnis[i].AnimationState;
                if (ani != null)
                {
                    ani.SetEmptyAnimation(0, 0);
                } 

                char objName = objAnis[i].initialSkinName[0];

                if (butterflys[i] == null)
                {
                    if (objName == 'w')
                    {
                        butterflys[i] = new Butterfly(butterflyObj[i], pointsDic['w'][4].transform.position, mono);
                    }
                    else if (objName == 'c')
                    {
                        butterflys[i] = new Butterfly(butterflyObj[i], pointsDic['c'][4].transform.position, mono);
                    }
                }
                else
                {
                    butterflys[i].isEnd = false;
                    butterflys[i].isPlayEnd = false;
                }

                int index = i;
                mono.StartCoroutine(WaitTime(UnityEngine.Random.Range(0, 1f),() =>
                {
                    SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.UI_Idle], true);
                }));
                
                objActs[i].enabled = true;
                objActs[i].SetDragCallback(StartDrag, Drag, EndDrag);
            }

            AnimationState finishSpine = finishScene.transform.GetChild(0).GetComponent<SkeletonGraphic>().AnimationState;

            if (finishSpine != null)
            {
                finishSpine.SetEmptyAnimation(0, 0);
            }
        }

        void GameStart()
        {            
            butterflyObj[0].transform.localPosition = new Vector3(-650.1f, -434.2f, 0);
            butterflyObj[0].transform.rotation = Quaternion.Euler(-90, -90, -4.54f);

            SoundManager.instance.Speaking(npc, "talk",SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.BgSoundPart2(SoundManager.SoundType.COMMONBGM);
            });

            Util.AddBtnClick(finishScene, (o) =>
            {
                finishScene.SetActive(false);
            });

            LogicManager.instance.SetReplayEvent(Replay);
        }

        void Replay()
        {
            mono.StopAllCoroutines();
            GameObject.Destroy(butterflyDragerClone);
            GameInit();
            GameStart();
        }

        # region ILDrager

        void StartDrag(Vector3 pos, int type, int index)
        {
            butterflyObj[index].transform.DOScale(1.2f, 0.2f).SetEase(Ease.InExpo); 
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.Drag], false);
        }

        void Drag(Vector3 pos, int type, int index)
        {
            butterflyObj[index].transform.position = pos;
        }

        void EndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            butterflyObj[index].transform.DOScale(1f, 0.2f).SetEase(Ease.InExpo);
            
            Ray r = sceneCamera.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 100));
            Debug.DrawLine(butterflyObj[index].transform.position, butterflyObj[index].transform.position + new Vector3(0, 0, 10000), Color.red);
            RaycastHit hitInfo;
            if (Physics.Raycast(r, out hitInfo, 1000, 1 << LayerMask.NameToLayer("Water")))
            {
                char collisionName = hitInfo.transform.name[0];
                char objName = objAnis[index].initialSkinName[0];

                Debug.Log("collisionName:" + collisionName);
                Debug.Log("objName:" + objName);
                if (collisionName == objName)
                {
                    //GameObject child = objActs[index].transform.GetChild(0).gameObject;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.Right], false, () =>
                    {
                        objActs[index].enabled = false;
                        butterflys[index].RandomFly();
                        butterflys[index].isEnd = true;
                        butterflyCount++;

                        if (butterflyCount >= 6)
                        {
                            mono.StartCoroutine(WaitTime(3, () =>
                            {
                                finishScene.SetActive(true);
                                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 4, null, () =>
                                {
                                    SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 5, null, () =>
                                    {
                                        LogicManager.instance.ShowReplayBtn(true);
                                    });
                                });
                                SpineManager.instance.DoAnimation(finishScene.transform.GetChild(0).gameObject, "animation", false, () =>
                                {                                    
                                    SpineManager.instance.DoAnimation(finishScene.transform.GetChild(0).gameObject, "idle", true);                                   
                                });
                            }));                            
                        }
                    });
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                    SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.Error], false, () =>
                    {
                        butterflyObj[index].transform.DOLocalMove(initPos[index], 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                        {
                            SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.UI_Idle], true);
                        });
                        butterflyObj[index].transform.DOLocalRotateQuaternion(initRot[index], 0.2f).SetEase(Ease.InBack);
                    });
                }
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.Error], false, () =>
                {
                    butterflyObj[index].transform.DOLocalMove(initPos[index], 0.2f).SetEase(Ease.InBack).OnComplete(() =>
                    {
                        SpineManager.instance.DoAnimation(objAnis[index].gameObject, butterflyAniStr[(int)Butterfly_Spine.UI_Idle], true);
                    });
                    butterflyObj[index].transform.DOLocalRotateQuaternion(initRot[index], 0.2f).SetEase(Ease.InBack);
                });
            }
        }

        #endregion

        IEnumerator WaitTime(float time, Action act)
        {
            yield return new WaitForSeconds(time);

            act();
            mono.StopCoroutine(WaitTime(time, act));
        }

        void OnDisable()
        {
            mono.StopAllCoroutines();
            GameObject.Destroy(butterflyDragerClone);
            LogicManager.instance.ShowReplayBtn(false);
        }

        int[] RandomNumber()
        {
            List<int> skinList = new List<int>() { 0, 1, 2, 3, 4, 5, 6, 7, 8 };
            List<int> randomNum = new List<int>(skinList);          
            List<int> skinNum = new List<int>();
            int[] skinInt = new int[6];

            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, randomNum.Count);
                skinNum.Add(randomNum[index]);
                randomNum.RemoveAt(index);
            }

            randomNum.Clear();
            
            for (int i = 0; i < skinList.Count; i++)
            {
                randomNum.Add(skinList[i] + 9);
            }

            for (int i = 0; i < 3; i++)
            {
                int index = UnityEngine.Random.Range(0, randomNum.Count);
                skinNum.Add(randomNum[index]);
                randomNum.RemoveAt(index);
            }

            for (int i = 0; i < 6; i++)
            {
                int index = UnityEngine.Random.Range(0, skinNum.Count);
                skinInt[i] = skinNum[index];
                skinNum.RemoveAt(index);
            }

            return skinInt;
        }

    }
}
