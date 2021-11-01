using Spine.Unity;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.EventSystems;
using ILFramework;

namespace ILFramework.HotClass
{
    public class CourseGentlyPeacockPart1
    {
        GameObject curGo;
        GameObject Buttom;
        MonoBehaviour mono;
        //part1
        GameObject Part1;
        GameObject KongQue_1;
        GameObject KongQue_2;
        GameObject KongQue_3;
        GameObject CurUseKongQueAnim;
        GameObject Buttons;

        //UIBar和按钮动画
        GameObject UIBar;
        GameObject UIAnimations;
        GameObject UIBarAnim;
        GameObject UIUpAnim;
        GameObject Npc;
        GameObject Finger;
        string lastAnimName;

        //Game Part
        GameObject AllBtns;
        Dictionary<string, Vector3> OldPosDict;
        List<ILObject3DAction> Actions;
        int gamePartIndex;
        bool canTrigger;

        GameObject EndPanel_1;
        GameObject EndPanel_2;
        GameObject Mask;

        GameObject EndBack;


        //拖拽部分
        Camera camera;
        Vector2 lastPos;
        Vector2 mousePos;
        RectTransform CanvasRect;
        List<GameObject> KongQueTirgers;

        bool canMove = false;
        bool isTeach = false;
        


        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();

            Buttom = curTrans.Find("Content/Buttom").gameObject;
            Part1 = Buttom.transform.Find("Part1").gameObject;
            KongQue_1 = Part1.transform.Find("KongQue_1").gameObject;
            KongQue_2 = Part1.transform.Find("KongQue_2").gameObject;
            Npc = Buttom.transform.Find("Npc").gameObject;
            EndPanel_1 = Buttom.transform.Find("EndPanel_1").gameObject;
            EndPanel_2 = Buttom.transform.Find("EndPanel_2").gameObject;
            Mask = Buttom.transform.Find("Mask").gameObject;
            EndBack = Buttom.transform.Find("EndBack").gameObject;

            Buttons = Buttom.transform.Find("Buttons").gameObject;
            UIBar = Buttons.transform.Find("Bar").gameObject;
            UIAnimations = Buttons.transform.Find("UIAnimations").gameObject;
            UIBarAnim = UIAnimations.transform.Find("UIBarAnim").gameObject;
            UIUpAnim = UIAnimations.transform.Find("UIUpAnim").gameObject;
            AllBtns = Buttons.transform.Find("AllBtns").gameObject;
            Finger = Buttons.transform.Find("FingerAnim").gameObject;

            OldPosDict = new Dictionary<string, Vector3>();
            Actions = new List<ILObject3DAction>();

            //拖拽部分
            camera = curTrans.Find("Content/Camera").GetComponent<Camera>();
            CanvasRect = curGo.transform as RectTransform;
            KongQueTirgers = new List<GameObject>();

            InitGame();
        }


        void InitGame()
        {
            UIBar.SetActive(false);
            UIAnimations.SetActive(false);
            UIBarAnim.SetActive(false);
            UIUpAnim.SetActive(false);

            KongQue_1.SetActive(true);
            KongQue_2.SetActive(false);
            CurUseKongQueAnim = KongQue_1;
            
            if(Part1.transform.childCount > 2)
            {
                GameObject.Destroy(Part1.transform.GetChild(2).gameObject);
            }


            //新建孔雀3
            KongQue_3 = GameObject.Instantiate(KongQue_2);
            KongQue_3.SetActive(false);
            KongQue_3.transform.SetParent(Part1.transform);
            KongQue_3.transform.localPosition = KongQue_2.transform.localPosition;
            KongQue_3.transform.localRotation = KongQue_2.transform.localRotation;
            KongQue_3.transform.localScale = Vector3.one;


            Finger.SetActive(false);

            EndPanel_1.SetActive(false);
            EndPanel_2.SetActive(false);
            Mask.SetActive(false);
            EndBack.SetActive(false);
            Buttons.SetActive(true);
            Part1.SetActive(true);


            OldPosDict.Clear();
            Actions.Clear();
            KongQueTirgers.Clear();

            lastPos = Vector2.zero;
            mousePos = Vector2.zero;

            gamePartIndex = 1;
            canTrigger = false;
            canMove = false;



            for (int i = 0; i < AllBtns.transform.childCount; i++)
            {
                var obj = AllBtns.transform.GetChild(i);
                OldPosDict.Add(obj.name, obj.localPosition);
                var eventTriger = obj.GetComponent<EventTrigger>();

                ILDrager drager = obj.GetComponent<ILDrager>();
                drager.index = i;
                drager.SetDragCallback(StartDrag, Drag, EndDrag);


                //注册每个按钮的物理检测事件
                ILObject3DAction act = obj.GetComponent<ILObject3DAction>();
                act.index = i;
                act.OnTriggerEnter2DLua = OnTriggerEnter2DOfBtn;
                act.OnPointDownLua = ClickDown;
                act.OnPointUpLua = ClickUp;

                Actions.Add(act);
            }

            //获取孔雀身体所有部分的触发区域
            for (int i = 0; i < KongQue_1.transform.childCount; i++)
            {
                KongQueTirgers.Add(KongQue_1.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < KongQue_3.transform.childCount; i++)
            {
                KongQueTirgers.Add(KongQue_3.transform.GetChild(i).gameObject);
            }

            StartGame();
        }



        //测试拖拽
        private void ClickUp(int index)
        {
            canTrigger = false;
            GameObject obj = AllBtns.transform.GetChild(index).gameObject;
            Debug.Log("松开了" + obj.name);
            obj.transform.GetChild(0).gameObject.SetActive(false);
            obj.transform.GetChild(1).gameObject.SetActive(true);
        }

        private void ClickDown(int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
            canTrigger = true;
            GameObject obj = AllBtns.transform.GetChild(index).gameObject;
            Debug.Log("点击了" + obj.name);
            if (obj.name == "feather1")
            {
                Finger.SetActive(false);
            }

            obj.transform.GetChild(0).gameObject.SetActive(true);
            obj.transform.GetChild(1).gameObject.SetActive(false);
            SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, obj.name + "_s", false, () =>
            {
                obj.transform.GetChild(0).gameObject.SetActive(false);
                obj.transform.GetChild(1).gameObject.SetActive(true);
            });
        }

        private void StartDrag(Vector3 pos, int type, int index)
        {
            canMove = true;
            isTeach = false;
            //throw new NotImplementedException();
            Finger.SetActive(false);
            // Debug.Log("开始拖拽:" + data.selectedObject.name);
            GameObject obj = AllBtns.transform.GetChild(index).gameObject;
            RectTransform objRect = obj.GetComponent<RectTransform>();
            lastPos = objRect.anchoredPosition;
            mousePos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, camera, out mousePos);
        }

        private void Drag(Vector3 pos, int type, int index)
        {

            GameObject obj = AllBtns.transform.GetChild(index).gameObject;
            RectTransform objRect = obj.GetComponent<RectTransform>();

            if (canMove)
            {
                Vector2 offset = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, camera, out offset);
                offset -= mousePos;
                offset = lastPos + offset;

                objRect.anchoredPosition = offset;
            }
            else
            {
                objRect.anchoredPosition = OldPosDict[obj.name];
            }

        }


        private void EndDrag(Vector3 pos, int type, int index, bool isEnd)
        {
            canMove = false;
            GameObject obj = AllBtns.transform.GetChild(index).gameObject;
            RectTransform objRect = obj.GetComponent<RectTransform>();
            objRect.anchoredPosition = OldPosDict[obj.name];
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9);
        }


        void StartGame()
        {
            AllBtns.SetActive(false);
            SoundManager.instance.BgSoundPart1(SoundManager.SoundType.BGM, 0.3f);
            SpineManager.instance.DoAnimation(KongQue_1, "idle", true);

            NpcTalk(0, null, () =>
            {
                UIAnimations.SetActive(true);
                UIBarAnim.SetActive(true);
                //bar弹入
                SpineManager.instance.DoAnimation(UIBarAnim, "uibar_up", false, () =>
                {
                    UIUpAnim.SetActive(true);
                    //所有按钮弹出
                    SpineManager.instance.DoAnimation(UIUpAnim, "ui_up", false, () =>
                    {
                        UIAnimations.SetActive(false);
                        UIBarAnim.SetActive(false);
                        UIUpAnim.SetActive(false);
                        //真实UI出现
                        UIBar.SetActive(true);
                        AllBtns.SetActive(true);

                        ChooseGamePart(gamePartIndex);

                    });
                });
            });

            // KongQue_1.GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();
            // KongQue_1.GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();

            // KongQue_2.GetComponent<SkeletonGraphic>().Skeleton.SetToSetupPose();
            // KongQue_2.GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();
        }

        public void NpcTalk(int voiceIndex = 0, Action startAction = null, Action endAction = null)
        {

            SoundManager.instance.Speaking(Npc, "talk", SoundManager.SoundType.VOICE, voiceIndex, startAction, () =>
             {
                 if (endAction != null)
                     endAction.Invoke();
                 Mask.SetActive(false);
             });
        }


        private void OnTriggerEnter2DOfBtn(Collider2D other, int index)
        {
            GameObject BtnObject = Actions[index].gameObject;
            string nowName = BtnObject.name;

            if (other.name.Contains(nowName))
            {

                Mask.SetActive(true);
                //拖动的物体的名字与进入触发区域的名字对应
                Debug.Log("匹配成功:" + nowName);

                // BtnObject.SetActive(false);
                BtnObject.GetComponent<RectTransform>().anchoredPosition = OldPosDict[nowName];

                if (canTrigger)
                {
                    gamePartIndex++;
                    Debug.Log("当前游戏部分:" + gamePartIndex);
                    canTrigger = false;
                    canMove = false;
                    // PlayRightVoice();
                }

               

                CurUseKongQueAnim.GetComponent<SkeletonGraphic>().AnimationState.ClearTracks();
                //播放正确语音
                SpineManager.instance.DoAnimation(CurUseKongQueAnim, nowName, false, () =>
                {
                    Debug.Log("进入游戏部分:---" + gamePartIndex);
                    PlayRightVoice(()=>{    
                        ChooseGamePart(gamePartIndex);
                    });
                    // ChooseGamePart(gamePartIndex);
                    // SpineManager.instance.DoAnimation(CurUseKongQueAnim, nowName + "+_1", false, () =>
                    // {
                    //     lastAnimName = nowName + "_1";
                    //     ChooseGamePart(gamePartIndex);
                    // });
                });

            }
            else
            {
                Debug.Log("匹配不成功:" + nowName);

                if (canTrigger)
                {
                    //播放错误语音
                    PlayErrorVoice();
                    canTrigger = false;
                    canMove = false;
                }
            }
        }

        void PlayRightVoice(Action action = null)
        {
            int index = UnityEngine.Random.Range(0, 3);
            // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 10);
            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND,index,null,()=>{
                if(action != null)
                {
                    action.Invoke();
                }
            });
        }

        void PlayErrorVoice()
        {
            int index = UnityEngine.Random.Range(3, 6);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, index);
            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 9);
        }


        void ActiveTriggerAt(int index)
        {
            for (int i = 0; i < KongQueTirgers.Count; i++)
            {
                KongQueTirgers[i].SetActive(false);
            }

            if (gamePartIndex < 9)
            {
                KongQueTirgers[gamePartIndex - 1].SetActive(true);
            }

        }


        void ChooseGamePart(int index)
        {
            var skt = CurUseKongQueAnim.GetComponent<SkeletonGraphic>();
           // skt.Skeleton.SetToSetupPose();
            skt.AnimationState.ClearTracks();
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            switch (index)
            {
                case 1:
                    GamePart1();
                    break;
                case 2:
                    GamePart2();
                    break;
                case 3:
                    GamePart3();
                    break;
                case 4:
                    GamePart4();
                    break;
                case 5:
                    GamePart5();
                    break;
                case 6:
                    GamePart6();
                    break;
                case 7:
                    GamePart7();
                    break;
                case 8:
                    GamePart8();
                    break;
                case 9:
                    GamePart9();
                    break;

            }
            //根据gamepartindex激活对应部分的触发器
            ActiveTriggerAt(gamePartIndex);
        }

        void GamePart1()
        {
            //羽片
            Debug.Log("进入游戏第1部分");
            NpcTalk(1, null, () =>
            {
                Finger.SetActive(true);
                isTeach = true;
                SpineManager.instance.DoAnimation(Finger, "animation", true, () =>
                {
                    if (isTeach)
                    {
                        isTeach = false;
                        Finger.transform.DOLocalMove(new Vector3(532f, -185f, 0f), 1.5f).OnComplete(() =>
                        {

                            Finger.transform.DOLocalMove(new Vector3(478f, -516f, 0f), 1.5f).OnComplete(() =>
                            {
                                isTeach = true;
                            });
                        });
                    }
                });
            });
        }

        void GamePart2()
        {
            //尾羽
            Debug.Log("进入游戏第2部分");
            NpcTalk(2, () =>
            {
                SpineManager.instance.DoAnimation(KongQue_1, "feather1_1");
            });
        }

        void GamePart3()
        {
            //脖子
            Debug.Log("进入游戏第3部分");
            KongQue_1.SetActive(false);
            KongQue_3.SetActive(true);
            CurUseKongQueAnim = KongQue_3;
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "animation", false, () =>
            {
                NpcTalk(3);
            });

        }

        void GamePart4()
        {
            //头
            Debug.Log("进入游戏第4部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "neek_1");
            NpcTalk(4, () =>
            {
                // SpineManager.instance.DoAnimation(KongQue_2,"neek_1");
            });
        }

        void GamePart5()
        {
            //身体
            Debug.Log("进入游戏第5部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "head_1");
            NpcTalk(5, () =>
            {
                // SpineManager.instance.DoAnimation(KongQue_2,"head_1");    
            });
        }

        void GamePart6()
        {
            //花冠
            Debug.Log("进入游戏第6部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "body_1");
            NpcTalk(6, () =>
            {
                // SpineManager.instance.DoAnimation(KongQue_2,"body_1");
            });
        }

        void GamePart7()
        {
            //嘴巴
            Debug.Log("进入游戏第7部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "pileum_1");
            NpcTalk(7, () =>
            {
                // SpineManager.instance.DoAnimation(KongQue_2,"pileum_1");
            });
        }

        void GamePart8()
        {
            //腿
            Debug.Log("进入游戏第8部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "mouth_1");
            NpcTalk(8, () =>
            {
                // SpineManager.instance.DoAnimation(KongQue_2,"mouth_1");
            });
        }

        void GamePart9()
        {
            //游戏结束
            Debug.Log("进入游戏结束部分");
            SpineManager.instance.DoAnimation(CurUseKongQueAnim, "animation_1", false, () =>
            {
                EndPanel_1.SetActive(true);
                Mask.SetActive(true);
                EndBack.SetActive(true);
                Buttons.SetActive(false);
                Part1.SetActive(false);
                GameObject.Destroy(KongQue_3);
                Npc.SetActive(false);

               // SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 8);
               
                SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND,8,null,()=>{
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7);
                });
                SpineManager.instance.DoAnimation(EndPanel_1, "animation", false, () =>
                {
                    EndPanel_1.SetActive(false);
                    EndPanel_2.SetActive(true);
                    SpineManager.instance.DoAnimation(EndPanel_2, "idle", false, () =>
                    {
                        SpineManager.instance.DoAnimation(EndPanel_2, "idle");
                    });
                });
            });
        }

        

    }
}
