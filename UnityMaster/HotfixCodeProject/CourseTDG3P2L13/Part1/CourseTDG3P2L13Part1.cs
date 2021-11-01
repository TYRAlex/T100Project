using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;

namespace ILFramework.HotClass
{
    public class CourseTDG3P2L13Part1
    {
        GameObject curGo;
        Transform Buttom, Btns, StartBtn;
        GameObject CoverMask, TimeNumber, Npc;
        MonoBehaviour mono;
        //paint产生部分
        Transform PaintPool, PaintProductPoint, CurPaint;
        Transform PictureFrames, FrameDownPoints, FrameDownIdle, FrameDown;
        Action MoveAction;
        float Speed = 5f;
        List<GameObject> AllPaints;
        bool GameStart;
        int WrongCount;
        List<string> ShortFrams;
        List<string> PaintFrameOnHead;
        List<string> OldPaintIdleAnims;

        //画落下可以触发按钮点击的部分
        Transform CanClickTrigger_1;
        Transform CanClickTrigger_2;
        bool CanClick = false;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            Buttom = curTrans.Find("Content/Buttom");
            Btns = Buttom.Find("Btns");
            StartBtn = Buttom.Find("StartBtn");
            TimeNumber = Buttom.Find("TimeNumber").gameObject;
            CoverMask = Buttom.Find("CoverMask").gameObject;
            Npc = Buttom.Find("Npc").gameObject;
            PaintPool = Buttom.Find("PaintPool");
            PaintProductPoint = Buttom.Find("PaintProductPoint");
            mono = curGo.GetComponent<MonoBehaviour>();

            AllPaints = new List<GameObject>();
            PictureFrames = Buttom.Find("PictureFrames");
            FrameDownPoints = PictureFrames.Find("FrameDownPoints");
            FrameDownIdle = PictureFrames.Find("FrameDownIdle");
            FrameDown = PictureFrames.Find("FrameDown");
            ShortFrams = new List<string>() { "K_A1", "K_S1", "K_S4", };
            PaintFrameOnHead = new List<string>();
            OldPaintIdleAnims = new List<string>();
            CanClickTrigger_1 = Buttom.Find("CanClickTrigger_1");
            CanClickTrigger_2 = Buttom.Find("CanClickTrigger_2");
            CanClick = false;
            InitGame();
        }

        private void InitGame()
        {
            //open replayBtn
            LogicManager.instance.ShowReplayBtn(false);


            //register btns
            PaintFrameOnHead.Clear();
            OldPaintIdleAnims.Clear();
            CoverMask.SetActive(false);
            StartBtn.gameObject.SetActive(true);
            StartBtn.GetComponent<ILObject3DAction>().OnPointDownLua = ClickStartBtn;
            for (int i = 0; i < Btns.childCount; i++)
            {
                var btn = Btns.GetChild(i);
                var action = btn.GetComponent<ILObject3DAction>();
                action.OnPointDownLua = ClicktBtn;
                action.index = i;
            }
            //注册npc触发器事件
            Npc.GetComponent<ILObject3DAction>().OnTriggerExit2DLua = OnPaintEnter;
            SpineManager.instance.DoAnimation(Npc, "idle");

            WrongCount = 0;
            Speed = 300f;
            MoveAction = null;
            GameStart = false;
            CanClick = false;
            AllPaints.Clear();
            RegisterPaintBtns();
            RegisterCanClickTriggerCallback();
            ResetDownPaintState();
        }

        void RegisterPaintBtns()
        {
            if (PaintProductPoint.childCount > 0)
            {
                var paints = PaintProductPoint.GetChildren(PaintProductPoint.gameObject);
                for (int i = 0; i < paints.Length; i++)
                {
                    paints[i].SetActive(false);
                    paints[i].transform.SetParent(PaintPool);
                    paints[i].transform.localPosition = Vector3.zero;
                }
            }

            for (int i = 0; i < PaintPool.childCount; i++)
            {
                var paint = PaintPool.GetChild(i);
                var anim = paint.Find("Anim").gameObject;
                anim.SetActive(false);
                paint.gameObject.SetActive(false);
                AllPaints.Add(paint.gameObject);
            }

        }
        void RegisterCanClickTriggerCallback()
        {
            var action_1 = CanClickTrigger_1.GetComponent<ILObject3DAction>();
            action_1.index = 0;
            action_1.OnTriggerExit2DLua = OnTrigger_1;

            var action_2 = CanClickTrigger_2.GetComponent<ILObject3DAction>();
            action_2.index = 1;
            action_2.OnTriggerEnter2DLua = OnTrigger_2;
        }

        void ResetDownPaintState()
        {
            for(int i = 0;i < FrameDownPoints.childCount;i++)
            {
                FrameDownPoints.GetChild(i).gameObject.SetActive(false);
            }
        }
        private void OnTrigger_1(Collider2D other, int index)
        {
            if (other.name.Contains("K"))
            {
                Debug.Log("可以点");
                CanClick = true;
            }
        }

        private void OnTrigger_2(Collider2D other, int index)
        {
            if (other.name.Contains("K"))
            {
                Debug.Log("不可以点");

                CanClick = false;
            }
        }


        private void ClicktBtn(int index)
        {
            if (CurPaint == null)
                return;

            var btn = Btns.GetChild(index);
            var normal = btn.Find("Normal").gameObject;
            var anim = btn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);

            if(!CanClick)
            {
                Debug.Log("不可点击状态");
                SpineManager.instance.DoAnimation(anim, btn.name + "1", false);
                return;
            }

            //点击正确
            if (CurPaint.name.Contains(btn.name))
            {
                SpineManager.instance.DoAnimation(anim, btn.name + "1", false, () =>
                {
                    normal.SetActive(true);
                    anim.SetActive(false);

                    var paintAnim = CurPaint.Find("Anim").gameObject;
                    paintAnim.SetActive(true);
                    SpineManager.instance.DoAnimation(paintAnim, "animation", false);
                    Debug.Log("点击正确");
                    Speed = Speed * 2;
                });

            }
            else
            {
                //点击错误
                SpineManager.instance.DoAnimation(anim, btn.name + "2", false, () =>
                {
                    Debug.Log("点击错误");
                    normal.SetActive(true);
                    anim.SetActive(false);

                    //执行点击错误后paint和丁丁的动画，产生新paint
                    // SetNewPaint();
                    CurPaint.Find("Normal").gameObject.SetActive(false);
                    Speed = Speed * 2;
                    Debug.Log("------" + CurPaint.name);

                    SetWrongState();
                });
            }

        }

        private void ClickStartBtn(int index)
        {
            var normal = StartBtn.Find("Normal").gameObject;
            var anim = StartBtn.Find("Anim").gameObject;
            normal.SetActive(false);
            anim.SetActive(true);
            CoverMask.SetActive(true);
            SpineManager.instance.DoAnimation(anim, "star", false, () =>
            {
                Debug.Log("开始计时");
                normal.SetActive(true);
                anim.SetActive(false);
                ReadyToStartGame();
            });
        }

        void ReadyToStartGame()
        {

            StartBtn.gameObject.SetActive(false);
            TimeNumber.gameObject.SetActive(true);
            mono.StartCoroutine(Timging());
        }

        IEnumerator Timging()
        {
            int time = 0;
            while (time < 4)
            {
                for (int i = 0; i < TimeNumber.transform.childCount; i++)
                {
                    TimeNumber.transform.GetChild(i).gameObject.SetActive(false);
                }
                TimeNumber.transform.GetChild(time).gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                yield return new WaitForSeconds(1f);
                time++;
            }

            StartGame();
        }

        private void StartGame()
        {
            TimeNumber.SetActive(false);
            CoverMask.SetActive(false);
            GameStart = true;
            Debug.Log("游戏开始");
            //新画下落
            SetNewPaint();
        }

        private void FixedUpdate()
        {
            if (!GameStart)
                return;
            if (MoveAction != null)
            {
                MoveAction.Invoke();
            }
            //MovePaint();
        }


        //设置新的下落的画
        public void SetNewPaint()
        {
            //回归原位
            if (CurPaint != null)
            {
                CurPaint.Find("Normal").gameObject.SetActive(true);
                CurPaint.Find("Anim").gameObject.SetActive(false);
                CurPaint.gameObject.SetActive(false);
                CurPaint.SetParent(PaintPool);
                CurPaint.localPosition = Vector3.zero;
                CurPaint = null;
            }

            Speed = 300f;
            int randIndex = UnityEngine.Random.Range(0, PaintPool.childCount);
            CurPaint = PaintPool.GetChild(randIndex);
            CurPaint.SetParent(PaintProductPoint);
            CurPaint.localPosition = Vector3.zero;
            CurPaint.gameObject.SetActive(true);

            //当前画开始下落
            MoveAction += MovePaint;
        }

        void MovePaint()
        {
            if (CurPaint != null)
            {
                CurPaint.Translate(Vector3.down * Speed * Time.deltaTime);
            }
        }

        //paint碰到人
        private void OnPaintEnter(Collider2D other, int index)
        {
            if (other.name.Contains("K"))
            {
                //触发事件
                MoveAction -= MovePaint;

                //设置新的图
                SetNewPaint();
            }
        }

        void SetWrongState()
        {
            WrongCount++;
            var downPaint = FrameDownPoints.GetChild(WrongCount - 1).gameObject;
            var downPaintAnim = downPaint.transform.Find("Anim").gameObject;
            string paintName = CurPaint.name;
            PaintFrameOnHead.Add(paintName);

            downPaint.SetActive(true);
           // SpineManager.instance.DoAnimation(downPaintAnim, paintName, false);
           var skt = downPaintAnim.GetComponent<SkeletonGraphic>();
           skt.AnimationState.ClearTrack(0);
           TrackEntry track = skt.AnimationState.SetAnimation(0,paintName,false);
           track.TrackTime = 0f;

            //判断是短画框还是长画框
            string anim_1 = ShortFrams.Contains(paintName) ? "K_s" : "K_l";
            string anim_2 = ShortFrams.Contains(paintName) ? "k_s_idle" : "k_l_idle";
            OldPaintIdleAnims.Add(anim_2);

            var downIdlePaint = FrameDownPoints.GetChild(WrongCount - 1).gameObject;
            var downIdlePaintAnim = downIdlePaint.transform.Find("Anim").gameObject;
            downIdlePaint.SetActive(true);

            if (WrongCount < 4)
            {
                SpineManager.instance.DoAnimation(Npc, "dingding" + WrongCount, false, () =>
                {
                    if (WrongCount < 4)
                    {
                        // SpineManager.instance.DoAnimation(Npc, "dingding" + WrongCount + "_idle");
                        // SpineManager.instance.DoAnimation(downIdlePaintAnim, anim_2,true);
                        Npc.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dingding" + WrongCount + "_idle", true).TrackTime = 0f;
                        for (int i = 0; i < FrameDownPoints.childCount; i++)
                        {
                            var D = FrameDownPoints.GetChild(i).gameObject;
                            if (D.activeInHierarchy)
                            {
                                var anim = D.transform.GetChild(0).gameObject;
                                anim.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, OldPaintIdleAnims[i], true).TrackTime = 0f;
                            }

                        }

                    }

                });
            }
            else if (WrongCount >= 4)
            {
                mono.StartCoroutine(GameOver());
            }
        }

        IEnumerator GameOver()
        {
            yield return new WaitForSeconds(0.7f);
            Debug.Log("游戏结束");
            GameStart = false;
            CoverMask.SetActive(true);
            Npc.GetComponent<SkeletonGraphic>().AnimationState.SetAnimation(0, "dingding" + WrongCount, false).TrackTime = 20f / 30f;

            //书向下落
            Debug.LogError("开始下落");
            for (int i = 0; i < 4; i++)
            {
                var paint = FrameDownPoints.transform.GetChild(i).gameObject;
                var anim = paint.transform.Find("Anim").gameObject;
                //paint.SetActive(true);
                string animName = ShortFrams.Contains(PaintFrameOnHead[i]) ? "K_s_d" : "K_l_d";
                animName = animName + (i + 1).ToString();
                var skt = anim.GetComponent<SkeletonGraphic>();
                TrackEntry trackEntry = skt.AnimationState.SetAnimation(0, animName, false);
                trackEntry.TrackTime = 20f / 30f;
            }

            LogicManager.instance.ShowReplayBtn(true);
            LogicManager.instance.SetReplayEvent(()=>{
                MoveAction = null;
                InitGame();
            });
        }


    }
}
