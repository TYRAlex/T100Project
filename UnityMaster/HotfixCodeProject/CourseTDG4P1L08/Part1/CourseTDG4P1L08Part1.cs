using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Spine.Unity;
using Part1;

namespace ILFramework.HotClass
{
    public class CourseTDG4P1L08Part1
    {
        static GameObject curGo;
        Transform Buttom;
        static Transform Btns;
        static GameObject NumAnimObject;
        Transform AllBones;
        static Transform Environment;
        public static GameObject CovoerMask;
        static GameObject Npc;
        // 跟随鼠标的所有工具
        static Transform Tools;
        public static GameObject curUseTool;
        Vector2 mousePos;
        RectTransform CanvasRect;
        Camera camera;

        Dictionary<string, int> SoundDict;

        //游戏对像
        public static Floor_1 floor_1;
        public static Floor_2 floor_2;
        public static Floor_3 floor_3;
        public static Floor_4 floor_4;
        public static Bones allBones;


        public static bool isCanMove = true;
        public static bool isCanTrigger = false;
        public static Vector3 toolPosition;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Buttom = curTrans.Find("Content/Buttom");
            Btns = Buttom.Find("ToolsBar/Btns");
            AllBones = Buttom.Find("AllBones");
            Environment = Buttom.Find("Environment");
            CovoerMask = curTrans.Find("Content/CoverMask").gameObject;
            Npc = curTrans.Find("Content/Npc").gameObject;
            NumAnimObject = Buttom.Find("ToolsBar/NumAnim").gameObject;

            SoundDict = new Dictionary<string, int>();
            SoundDict.Add("electrodrill", 3);
            SoundDict.Add("brush", 2);
            SoundDict.Add("pick", 5);
            SoundDict.Add("spade", 4);
            isCanMove = true;

            Tools = Buttom.Find("Tools");
            CanvasRect = Tools as RectTransform;
            camera = curTrans.Find("Camera").GetComponent<Camera>();
            toolPosition = Vector3.zero;
            InitGame();
        }


        void Update()
        {
            if (curUseTool != null && Input.GetMouseButton(0) && isCanMove && !CovoerMask.activeInHierarchy)
            {
                Debug.Log("执行了按下");
                mousePos = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, camera, out mousePos);
                curUseTool.GetComponent<RectTransform>().anchoredPosition = mousePos;

            }
            else if (curUseTool != null && Input.GetMouseButtonUp(0) && !isCanTrigger && !CovoerMask.activeInHierarchy)
            {
                Debug.Log("执行了松开");
                ResetTools();
            }
            else if (curUseTool != null &&
                Input.GetMouseButtonUp(0) && isCanMove && isCanTrigger)
            {
                isCanMove = false;

                if (floor_4 != null || floor_1 != null || floor_3 != null || floor_2 != null || allBones != null)
                {
                    if (curUseTool.name == "pick" || curUseTool.name == "spade")
                    {
                        if (curUseTool.transform.localPosition.x <= -950f)
                        {
                            curUseTool.transform.position = new Vector3(curUseTool.transform.position.x+1, toolPosition.y, 0f);
                        }
                        else if (curUseTool.transform.localPosition.x >= 950f)
                        {
                            curUseTool.transform.position = new Vector3(curUseTool.transform.position.x-1, toolPosition.y, 0f);
                        }
                        else
                        {
                            curUseTool.transform.position = new Vector3(curUseTool.transform.position.x, toolPosition.y, 0f);
                        }

                    }
                    else
                    {
                        curUseTool.transform.position = toolPosition;
                    }

                }

                var NormalObj = curUseTool.transform.Find("Normal").gameObject;
                var AnimObj = curUseTool.transform.Find("Anim").gameObject;
                NormalObj.SetActive(false);
                AnimObj.SetActive(true);

                //播放工具的音效
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, SoundDict[curUseTool.name], true);

                AnimObj.GetComponent<SkeletonGraphic>().AnimationState.SetEmptyAnimation(0, 0);
                SpineManager.instance.DoAnimation(AnimObj, curUseTool.name, false, () =>
                {
                    AnimObj.SetActive(false);
                    curUseTool.gameObject.SetActive(false);
                    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                });

                if (floor_1 != null)
                {
                    bool isStone = (curUseTool.name == "electrodrill") ? true : false;
                    floor_1.PlayAnim(floor_1.CurIndex, isStone);
                }
                else if (floor_2 != null)
                {
                    floor_2.PlayAnim(floor_2.CurIndex);
                }
                else if (floor_3 != null)
                {
                    floor_3.PlayAnim(floor_3.CurIndex);
                }
                else if (floor_4 != null)
                {
                    floor_4.PlayAnim(floor_4.CurIndex);

                }
                else if (allBones != null)
                {
                    allBones.PlayAnim(allBones.CurIndex);
                }

            }
        }

        public static void InitGame()
        {
            floor_1 = null;
            floor_2 = null;
            floor_3 = null;
            floor_4 = null;
            allBones = null;
            LogicManager.instance.ShowReplayBtn(false);
            SoundManager.instance.BgSoundPart2();
            CovoerMask.SetActive(true);
            RegisterToolBtnsCallBack();
            ResetTools();
            ResetGame();
            SpineManager.instance.DoAnimation(NumAnimObject, "0_5", false);
            floor_1 = new Floor_1(Environment);
            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                CovoerMask.SetActive(false);
            });
        }

        static void RegisterToolBtnsCallBack()
        {
            for (int i = 0; i < Btns.childCount; i++)
            {
                var action = Btns.GetChild(i).GetComponent<ILObject3DAction>();
                action.index = i + 1;
                action.OnPointDownLua = ClickToolBtnCallBack;
            }
        }

        private static void ClickToolBtnCallBack(int index)
        {
            ResetToolBtns();
            ResetTools();
            var btn = Btns.GetChild(index - 1);
            var normal = btn.Find("Normal").gameObject;
            var light = btn.Find("Light").gameObject;
            var anim = btn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            SpineManager.instance.DoAnimation(anim, GetUIAnimName(index), false, () =>
            {
                anim.SetActive(false);
                light.SetActive(true);
            });

            //设置当前使用工具
            if (index != 3)
            {
                curUseTool = Tools.Find(btn.name).gameObject;
                curUseTool.SetActive(true);
            }
        }
        static string GetUIAnimName(int index)
        {
            return index == 1 ? "UI" : "UI" + index;
        }

        //重置工具按钮状态
        static void ResetToolBtns()
        {
            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i);
                var normal = btn.Find("Normal").gameObject;
                var light = btn.Find("Light").gameObject;
                var anim = btn.Find("Anim").gameObject;

                normal.SetActive(true);
                light.SetActive(false);
                anim.SetActive(false);
            }
        }

        static void ResetTools()
        {
            curUseTool = null;
            for (int i = 0; i < Tools.childCount; i++)
            {
                var tool = Tools.GetChild(i).gameObject;
                tool.transform.Find("Normal").gameObject.SetActive(true);
                tool.transform.Find("Anim").gameObject.SetActive(false);
                tool.GetComponent<BoxCollider2D>().enabled = true;
                tool.SetActive(false);
                tool.transform.localPosition = Vector2.zero;

            }
        }

        public static void ResetGame()
        {
            //endPanel_1
            var EndPanel_1 = curGo.transform.Find("Content/EndPanel_1").gameObject;
            EndPanel_1.transform.Find("Light").gameObject.SetActive(false);
            EndPanel_1.transform.Find("TigerBone").gameObject.SetActive(false);
            EndPanel_1.SetActive(false);

            //endPanel_2
            var EndPanel_2 = curGo.transform.Find("Content/EndPanel_2").gameObject;
            EndPanel_2.transform.Find("OverAnim").gameObject.SetActive(false);
            EndPanel_2.transform.Find("ReplayBtn").gameObject.SetActive(false);
            EndPanel_2.SetActive(false);

            // 重置floors
            var Environment = curGo.transform.Find("Content/Buttom/Environment");
            var floor_1 = Environment.Find("Floor_1");
            var floor_2 = Environment.Find("Floor_2");
            var floor_3 = Environment.Find("Floor_3");
            var floor_4 = Environment.Find("Floor_4");

            ResetObjNormal(floor_1);
            ResetObjNormal(floor_2);
            ResetObjNormal(floor_3);
            ResetObjNormal(floor_4);

            //重置骨头
            var AllBone = curGo.transform.Find("Content/Buttom/AllBones");
            ResetObjNormal(AllBone);

            var covermask = curGo.transform.Find("Content/CoverMask").gameObject;
            covermask.SetActive(false);
        }

        static void ResetObjNormal(Transform tran)
        {
            tran.gameObject.SetActive(true);
            for (int i = 0; i < tran.childCount; i++)
            {
                var obj = tran.GetChild(i);
                obj.gameObject.SetActive(true);
                var normal = obj.Find("Normal").gameObject;
                var anim = obj.Find("Anim").gameObject;
                normal.SetActive(true);
                anim.SetActive(false);

                if (obj.GetComponent<PolygonCollider2D>() == null)
                {
                    obj.GetComponent<BoxCollider2D>().enabled = false;
                }
                else
                {
                    obj.GetComponent<PolygonCollider2D>().enabled = false;
                }

            }
        }

    }
}
