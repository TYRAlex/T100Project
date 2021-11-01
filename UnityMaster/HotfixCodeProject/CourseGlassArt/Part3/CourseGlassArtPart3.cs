using DG.Tweening;
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
    public class CourseGlassArtPart3
    {
        enum BOARD_SPINE
        {
            Idle = 0, MoveRight = 1, MoveLeft = 2
        }
        string[] boardSpine;

        enum GLASS_SPINE
        {
            Poly0 = 0, Poly1 = 1, Poly2 = 2, Poly3 = 3, Poly4 = 4, Poly5 = 5, Poly6 = 6, Poly7 = 7, Light = 8
        }
        string[] glassSpine;

        enum FINISH_SPINE
        {
            State1 = 0, State2 = 1, State3 = 2, State4 = 3, State2_1 = 4, State2_2 = 5, State3_1 = 6,
        }
        string[] finishSpine;

        GameObject curGo;
        GameObject boardStart_btn, boardEnd_btn, board, light, poly, NPC, finishScene;
        GameObject[] polyDrag, imgSpine, winSpine, winPos;

        float alpha;
        int enterLight;
        int glassNum;
        Vector3[] aroundPos;
        List<Vector3> posList;
        List<int> glassList;
        MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            finishScene = curTrans.Find("finshScene").gameObject;
            boardStart_btn = curTrans.Find("gameScene/board/boardStart_btn").gameObject;
            board = curTrans.Find("gameScene/board/board").gameObject;
            boardEnd_btn = curTrans.Find("gameScene/board/boardEnd_btn").gameObject;
            light = curTrans.Find("gameScene/light").gameObject;
            poly = curTrans.Find("gameScene/poly").gameObject;
            NPC = curTrans.Find("NPC").gameObject;
            polyDrag = GetChildren(poly);
            winSpine = GetChildren(finishScene);
            //imgSpine = GetChildren(curTrans.Find("gameScene/glassImage").gameObject);
            winPos = GetChildren(curTrans.Find("gameScene/glassPos").gameObject);
            boardSpine = new string[] { "idle", "chouti", "chouti2" };
            glassSpine = new string[] { "s4", "s1", "z2", "s3", "s2", "z1", "z3", "s5", "guangyun" };
            finishSpine = new string[] { "1", "2", "3", "4", "2_1", "2_2", "3_1" };

            aroundPos = new Vector3[] { curTrans.Find("gameScene/aroundPos/pos1").transform.position,
                                        curTrans.Find("gameScene/aroundPos/pos2").transform.position
                                        };
            Init();
            Util.AddBtnClick(boardStart_btn, StarScene);
        }

        void Init()
        {
            alpha = 0;
            enterLight = 0;
            glassNum = 0;
            boardStart_btn.SetActive(true);
            boardEnd_btn.SetActive(false);
            poly.SetActive(false);
            NPC.SetActive(false);
            posList = new List<Vector3>();
            glassList = new List<int>();

            mono = curGo.GetComponent<MonoBehaviour>();
            SoundManager.instance.Speaking(NPC, "animation2", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                SoundManager.instance.bgmSource.volume = 0.5f;
            });

            //SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.VOICE, 0, null, () =>
            //{
            //    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            //    SoundManager.instance.bgmSource.volume = 0.5f;
            //});
            PlayAnimationState(board.GetComponent<SkeletonGraphic>(), boardSpine[(int)BOARD_SPINE.MoveRight]);

            finishScene.SetActive(false);
            for (int i = 0; i < winSpine.Length; i++)
            {
                PlayAnimationState(winSpine[i].GetComponent<SkeletonGraphic>(), finishSpine[(int)FINISH_SPINE.State2_2]);
            }

            SpineManager.instance.DoAnimation(light, glassSpine[(int)GLASS_SPINE.Light], true);
        }

        void StarScene(GameObject btn)
        {
            //btn.transform.DOMove(boardEnd_btn.transform.position, 1f).SetEase(Ease.InOutBack);

            btn.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(board, boardSpine[(int)BOARD_SPINE.MoveRight], false, () =>
            {
                boardEnd_btn.SetActive(true);
                SpineManager.instance.DoAnimation(board, boardSpine[(int)BOARD_SPINE.Idle], false);
                //board.SetActive(false);
                poly.SetActive(true);
                //boardEnd_btn.SetActive(true);
            });

            for (int i = 0; i < polyDrag.Length; i++)
            {
                ILDrager drager = polyDrag[i].GetComponent<ILDrager>();
                drager.SetDragCallback(StarDrag, Drag, EndDrag);
                polyDrag[i].GetComponent<Image>().alphaHitTestMinimumThreshold = 0.3f;
                posList.Add(polyDrag[i].transform.position);
            }
        }

        void StarDrag(Vector3 pos, int type, int index)


        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            //polyDrag[index].transform.DOScale(0.6f, 0.3f).SetEase(Ease.InQuad);

        }

        void Drag(Vector3 pos, int type, int index)
        {
            //float x = pos.x - Screen.width / 2;
            //float y = pos.y - Screen.height / 2;
            float x = pos.x;
            float y = pos.y;
            if (x > aroundPos[0].x &&
                x < aroundPos[1].x &&
                y > aroundPos[0].y &&
                y < aroundPos[1].y)
            {
                enterLight = 2;
                //imgSpine[index].SetActive(true);
                //imgSpine[index].GetComponent<Image>().color = new Color(1, 1, 1, 0.4f);
                //polyDrag[index].GetComponent<Image>().color = new Color(1, 1, 1, 0f);
                Debug.Log(111);
                GameObject go = polyDrag[index].transform.GetChild(0).gameObject;
                go.SetActive(true);
                SpineManager.instance.DoAnimation(go, glassSpine[index], false);
                //polyDrag[index].GetComponent<Image>().enabled = false;
                //mono.StartCoroutine(FadeImage(polyDrag[index], enterLight));
            }
            else
            {
                enterLight = 1;
                //imgSpine[index].SetActive(false);
                //imgSpine[index].GetComponent<Image>().color = new Color(1, 1, 1, 0);

                polyDrag[index].transform.GetChild(0).gameObject.SetActive(false);
                //polyDrag[index].GetComponent<Image>().color = new Color(1, 1, 1, 1f);
                //polyDrag[index].GetComponent<Image>().enabled = true;
                //mono.StartCoroutine(FadeImage(polyDrag[index], enterLight));
            }
            //Debug.Log(x + "," + y);
            //Debug.Log(aroundPos[0]);
            //Debug.Log(aroundPos[1]);

        }

        IEnumerator FadeImage(GameObject go, int isHave)
        {
            if (isHave == 1)
            {
                while (alpha < 255)
                {
                    alpha++;
                    go.GetComponent<Image>().color = new Color(1, 1, 1, alpha / 255f);
                    yield return null;
                }
                go.GetComponent<Image>().color = new Color(1, 1, 1, 1f);
            }
            else if (isHave == 2)
            {
                while (alpha > 0)
                {
                    alpha--;
                    go.GetComponent<Image>().color = new Color(1, 1, 1, alpha / 255f);
                    yield return null;
                }
                go.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }

            mono.StopCoroutine(FadeImage(go, isHave));
        }

        void EndDrag(Vector3 pos, int type, int index, bool isEnd)
        {
            //float x = pos.x - Screen.width / 2;
            //float y = pos.y - Screen.height / 2;
            float x = pos.x;
            float y = pos.y;
            if (x > aroundPos[0].x &&
                x < aroundPos[1].x &&
                y > aroundPos[0].y &&
                y < aroundPos[1].y)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);

                if (isEnd)
                {
                    if (!glassList.Contains(index))
                    {
                        glassList.Add(index);
                    }

                    if (glassList.Count > 7)
                    {
                        for (int i = 0; i < polyDrag.Length; i++)
                        {
                            polyDrag[i].transform.DOMove(winPos[i].transform.position, 0.3f);
                            polyDrag[i].GetComponent<Image>().enabled = false;
                            GameObject go = polyDrag[i].transform.GetChild(0).gameObject;
                            go.SetActive(false);
                        }
                        FinishScene();
                        //mono.StartCoroutine(WaitTime());
                        Util.AddBtnClick(boardEnd_btn, EndScene);

                        Debug.Log("finish");
                    }
                }
                else
                {
                    if (glassList.Contains(index))
                    {
                        glassList.Remove(index);
                    }
                }
            }
            else
            {
                if (glassList.Contains(index))
                {
                    glassList.Remove(index);
                }
            }
            Debug.Log("glassList.Count:" + glassList.Count + "polyDrag[index]:" + polyDrag[index]);

        }

        IEnumerator WaitTime()
        {

            yield return new WaitForSeconds(0.02f);
            FinishScene();
            mono.StopCoroutine(WaitTime());
        }

        void FinishScene()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5, false);
            for (int i = 0; i < polyDrag.Length; i++)
            {
                GameObject go = polyDrag[i].transform.GetChild(0).gameObject;
                go.SetActive(true);
                SpineManager.instance.DoAnimation(go, glassSpine[i], false);
            }
            finishScene.SetActive(true);
            SpineManager.instance.DoAnimation(winSpine[1], finishSpine[(int)FINISH_SPINE.State1], false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                SpineManager.instance.DoAnimation(winSpine[1], finishSpine[(int)FINISH_SPINE.State2], false, () =>
                {
                    SpineManager.instance.DoAnimation(winSpine[1], finishSpine[(int)FINISH_SPINE.State3], true);
                });
                SpineManager.instance.DoAnimation(winSpine[0], finishSpine[(int)FINISH_SPINE.State2_1], false, () =>
                {
                    SpineManager.instance.DoAnimation(winSpine[0], finishSpine[(int)FINISH_SPINE.State4], true);
                });
                SpineManager.instance.DoAnimation(winSpine[2], finishSpine[(int)FINISH_SPINE.State2_2], false);
            });
        }

        void EndScene(GameObject btn)
        {
            btn.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
            SpineManager.instance.DoAnimation(board, boardSpine[(int)BOARD_SPINE.MoveLeft], false);
        }

        void PlayAnimationState(SkeletonGraphic ske, string name, string time = "0|0")
        {
            SpineManager.instance.DoAnimation(ske.gameObject, name);
            SpineManager.instance.PlayAnimationDuring(ske.gameObject, name, time);
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
