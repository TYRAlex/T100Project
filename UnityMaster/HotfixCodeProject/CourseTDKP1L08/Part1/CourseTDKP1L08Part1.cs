using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ILFramework.HotClass
{
    enum TianTianAnimations
    {
        idle,
        animation_1,
        animation_2
    }


    public class CourseTDKP1L08Part1
    {
        public static int Speed;

        GameObject curGo;
        GameObject Buttom;
        GameObject Top;
        GameObject Shield;
        GameObject TianTianNpc;
        GameObject StartBtn;
        GameObject StartBtnAnim;
        Transform TimeNumber;
        MonoBehaviour mono;
        GameObject TianTianGameNpc;
        GameObject TianTainGameAnim;

        GameObject EndAnimationGameOject;
        GameObject EndAnim;
        GameObject ReplayBtn;

        RectTransform ttRect;
        string[] gameAnimations;
        //EventTrigger TianTianGameEvent;

        ILDrager TianTianGameEvent;
        ILObject3DAction object3DAction;


        bool canMove = false;

        bool isEating = false;

        Camera camera;
        Vector2 lastPos;
        Vector2 mousePos;

        RectTransform CanvasRect;

        //蘑菇产生点
        List<Transform> productPoints;
        List<MushRoom> MushRoomPool;
        List<MushRoom> UsedMushRoomPool;
        public static Action action = null;

        float timer = 0f;
        float totaltime = 0f;
        float offsetTimer = 0.4f;

        public int maxCoount = 4;
        public static int curCount;
        bool GameStart = false;
        Transform NumberSprites, NumberKuan, Numbers;
        int eatMushRoomCount = 0;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curTrans.GetComponent<MonoBehaviour>();

            camera = curTrans.Find("Camera").GetComponent<Camera>();

            Top = curTrans.Find("Content/Top").gameObject;
            Buttom = curTrans.Find("Content/Buttom").gameObject;
            Shield = Buttom.transform.Find("Shield").gameObject;
            TianTianNpc = Buttom.transform.Find("TianTainNpc").gameObject;
            StartBtn = Buttom.transform.Find("StartBtn").gameObject;
            StartBtnAnim = StartBtn.transform.Find("StartBtnAnim").gameObject;
            TimeNumber = Buttom.transform.Find("TimeNumber");
            TianTianGameNpc = Buttom.transform.Find("TianGameNpc").gameObject;
            TianTainGameAnim = TianTianGameNpc.transform.Find("TianGameNpcAnim").gameObject;
            ttRect = TianTianGameNpc.GetComponent<RectTransform>();
            CanvasRect = curGo.transform as RectTransform;

            EndAnimationGameOject = Buttom.transform.Find("EndAnimation").gameObject;
            EndAnim = EndAnimationGameOject.transform.Find("EndAnim").gameObject;
            ReplayBtn = EndAnimationGameOject.transform.Find("ReplayBtn").gameObject;

            gameAnimations = new string[] { "idle", "animation_1", "animation_2" };

            maxCoount = 4;
            Speed = 2;
            curCount = 0;
            offsetTimer = 0.4f;
            //计数
            NumberSprites = Top.transform.Find("NumberSprites");
            NumberKuan = Buttom.transform.Find("NumberKuan");
            Numbers = Buttom.transform.Find("Numbers");

            object3DAction = TianTianGameNpc.GetComponent<ILObject3DAction>();

            //注册碰撞监听事件
            object3DAction.OnTriggerEnter2DLua = OnTriggerEnter2D;

            var pruductPointsTran = Buttom.transform.Find("ProductPoints");
            productPoints = new List<Transform>();
            for (int i = 0; i < pruductPointsTran.childCount; i++)
            {
                productPoints.Add(pruductPointsTran.GetChild(i));
            }

            var mushRoomPoolTran = Top.transform.Find("MushroomPool");
            MushRoomPool = new List<MushRoom>();
            UsedMushRoomPool = new List<MushRoom>();
            for (int i = 0; i < mushRoomPoolTran.childCount; i++)
            {
                MushRoom mushRoom = new MushRoom(mushRoomPoolTran.GetChild(i).gameObject, mushRoomPoolTran, MushRoomPool, UsedMushRoomPool);
                MushRoomPool.Add(mushRoom);
            }

            //注册人物的点击事件和松开事件
            TianTianGameEvent = TianTianGameNpc.GetComponent<ILDrager>();
            TianTianGameEvent.index = 1;
            TianTianGameEvent.SetDragCallback(OnBeginDragGameNpc, OnDragGameNpc, OnEndDragGameNpc);

            //播放背景音乐
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            //播放语音介绍
            SoundManager.instance.Speaking(TianTianNpc, "talk", SoundManager.SoundType.VOICE, 0, () =>
            {
                ReplayGame();
            }, () =>
            {
                InitGame();
            });
        }

        private void OnEndDragGameNpc(Vector3 arg1, int arg2, int arg3, bool arg4)
        {
            canMove = false;
            lastPos = ttRect.anchoredPosition;
        }

        private void OnDragGameNpc(Vector3 arg1, int arg2, int arg3)
        {
            if (canMove)
            {
                Vector2 offset = Vector2.zero;
                RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, camera, out offset);
                offset -= mousePos;
                offset = lastPos + offset;
                if (offset.x >= 760f)
                {
                    ttRect.anchoredPosition = new Vector2(760f, -350f);
                }
                else if (offset.x <= -760f)
                {
                    ttRect.anchoredPosition = new Vector2(-760f, -350f);
                }
                else
                {
                    ttRect.anchoredPosition = new Vector2(offset.x, -350f);
                }

            }
        }

        private void OnBeginDragGameNpc(Vector3 arg1, int arg2, int arg3)
        {
            lastPos = ttRect.anchoredPosition;
            mousePos = Vector2.zero;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(CanvasRect, Input.mousePosition, camera, out mousePos);
            canMove = true;
        }

        void FixedUpdate()
        {
            if (!GameStart)
                return;

            //超过10s蘑菇下落数量增加，速度加快
            totaltime += Time.deltaTime;
            if (totaltime > 10f)
            {
                maxCoount = 4;
                Speed = 3;
                offsetTimer = 0.3f;
            }

            if (action != null)
            {

                action.Invoke();
            }


            if (curCount < maxCoount)
            {
                // Debug.Log("CurCount:" + curCount);

                timer += Time.deltaTime;
                if (timer > offsetTimer)
                {
                    //随机一种蘑菇在随机位置下落
                    int mushRoomIndex = UnityEngine.Random.Range(0, MushRoomPool.Count);
                    int productPointIndex = UnityEngine.Random.Range(0, productPoints.Count);
                    MushRoomPool[mushRoomIndex].OutPool(productPoints[productPointIndex]);
                    timer = 0f;
                }
            }

        }

        void OnTriggerEnter2D(Collider2D other, int index)
        {
            if (other.name.Contains("MushRoom") || other.name.Contains("PMush"))
            {
                EatingMushRoom(other.gameObject);
            }
        }

        void EatingMushRoom(GameObject mushRoom)
        {
            if (!isEating)
            {
                //吃到的蘑菇消失
                SetUsedMushRoomState(mushRoom);

                if (mushRoom.name.Contains("MushRoom"))
                {
                    //无毒蘑菇
                    int voiceIndex = UnityEngine.Random.Range(1, 3);
                    isEating = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, voiceIndex);

                    PlayTianTianGameAnimation(TianTianAnimations.animation_1, false, () =>
                    {
                        isEating = false;
                        MushRoomCount();
                        PlayTianTianGameAnimation(TianTianAnimations.idle, true);
                    });

                }
                else
                {
                    //有毒蘑菇
                    isEating = true;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                    PlayTianTianGameAnimation(TianTianAnimations.animation_2, false, () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        //游戏结束逻辑
                        GameOver();
                    });

                }

            }
        }

        void SetUsedMushRoomState(GameObject mushRoom)
        {

            for (int i = 0; i < UsedMushRoomPool.Count; i++)
            {
                if (UsedMushRoomPool[i].CheckIsSelf(mushRoom))
                {
                    UsedMushRoomPool[i].EnterPool();
                    break;
                }
            }
        }


        void InitGame()
        {
            TianTianNpc.SetActive(false);
            eatMushRoomCount = 0;

            StartBtn.SetActive(true);
            StartBtn.GetComponent<Button>().onClick.AddListener(ReadToStartGame);

            ReplayBtn.SetActive(false);
            ReplayBtn.transform.Find("Anim").gameObject.SetActive(false);

            NumberSprites.gameObject.SetActive(false);
            NumberKuan.gameObject.SetActive(false);
            Numbers.gameObject.SetActive(false);

            Numbers.GetChild(0).GetComponent<Image>().sprite = NumberSprites.GetChild(0).GetComponent<Image>().sprite;
            Numbers.GetChild(1).GetComponent<Image>().sprite = NumberSprites.GetChild(0).GetComponent<Image>().sprite;

            ReplayBtn.GetComponent<Button>().onClick.AddListener(ReplayGame);

            SpineManager.instance.DoAnimation(StartBtnAnim, "animation_1", false, () =>
            {
                SpineManager.instance.DoAnimation(StartBtnAnim, "idle", true);
            });


        }

        void ReadToStartGame()
        {
            Shield.SetActive(false);
            StartBtn.SetActive(false);
            TimeNumber.gameObject.SetActive(true);
            mono.StartCoroutine(Timging());

        }

        IEnumerator Timging()
        {
            int time = 0;
            while (time < 4)
            {
                for (int i = 0; i < TimeNumber.childCount; i++)
                {
                    TimeNumber.GetChild(i).gameObject.SetActive(false);
                }
                TimeNumber.GetChild(time).gameObject.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                Debug.Log(111);
                yield return new WaitForSeconds(1f);
                time++;
            }

            StartGame();
        }

        void StartGame()
        {
            TimeNumber.gameObject.SetActive(false);
            //Debug.LogError("游戏开始");
            TianTianGameNpc.SetActive(true);
            PlayTianTianGameAnimation(TianTianAnimations.idle, true);
            GameStart = true;

            NumberKuan.gameObject.SetActive(true);
            SpineManager.instance.DoAnimation(NumberKuan.gameObject, "animation", false, () =>
            {
                Numbers.gameObject.SetActive(true);
            });
        }

        void GameOver()
        {
            //Debug.LogError("游戏结束");
            //游戏结束
            //游戏结束逻辑
            isEating = false;
            GameStart = false;
            action = null;

            //下落的蘑菇全部重置
            for (int i = 0; i < UsedMushRoomPool.Count; i++)
            {
                UsedMushRoomPool[i].ResetMushRoom();
            }
            UsedMushRoomPool.Clear();

            //田田归位
            ttRect.anchoredPosition = new Vector2(-50f, -350f);
            ttRect.gameObject.SetActive(false);

            //相关变量初始化
            maxCoount = 4;
            Speed = 2;
            offsetTimer = 0.4f;
            timer = 0f;
            totaltime = 0f;
            curCount = 0;

            //启动结算动画
            Shield.SetActive(true);
            EndAnimationGameOject.SetActive(true);

            SoundManager.instance.PlayClipByEvent(SoundManager.SoundType.SOUND, 3, null, () =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
            });

            int StarCount = GetStar();
            EndAnim.SetActive(true);
            SpineManager.instance.DoAnimation(EndAnim, StarCount.ToString(), false, () =>
            {
                ReplayBtn.SetActive(true);
                var Anim = ReplayBtn.transform.Find("Anim").gameObject;
                Anim.SetActive(true);
                SpineManager.instance.DoAnimation(Anim, "idle", true);
                //mono.StartCoroutine(ReadyToReplayGame());
            });
        }

        private int GetStar()
        {
            if (eatMushRoomCount >= 3 && eatMushRoomCount < 6)
            {
                return 2;

            }
            else if (eatMushRoomCount >= 6 && eatMushRoomCount < 8)
            {
                return 3;
            }
            else if (eatMushRoomCount >= 8 && eatMushRoomCount < 10)
            {
                return 4;
            }
            else if (eatMushRoomCount >= 10)
            {
                return 5;
            }

            return 1;
        }

        IEnumerator ReadyToReplayGame()
        {
            yield return new WaitForSeconds(2f);
            ReplayGame();
        }

        void ReplayGame()
        {
            //Debug.LogError("重新开始游戏");
            var Anim = ReplayBtn.transform.Find("Anim").gameObject;
            Anim.SetActive(true);
            SpineManager.instance.DoAnimation(Anim, "animation_1", false, () =>
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                StartBtn.SetActive(true);
                ReplayBtn.SetActive(false);
                Anim.SetActive(false);
                EndAnimationGameOject.SetActive(false);
                EndAnim.SetActive(false);
                NumberSprites.gameObject.SetActive(false);
                NumberKuan.gameObject.SetActive(false);
                Numbers.gameObject.SetActive(false);
                eatMushRoomCount = 0;

                Numbers.GetChild(0).GetComponent<Image>().sprite = NumberSprites.GetChild(0).GetComponent<Image>().sprite;
                Numbers.GetChild(1).GetComponent<Image>().sprite = NumberSprites.GetChild(0).GetComponent<Image>().sprite;

                ReadToStartGame();
            });
        }


        void PlayTianTianGameAnimation(TianTianAnimations animation, bool isLoop = false, Action endCallBack = null)
        {
            SpineManager.instance.DoAnimation(TianTainGameAnim, gameAnimations[(int)animation], isLoop, endCallBack);
        }

        void OnClickGameNpc(BaseEventData data)
        {

            // Debug.LogError("点击了田田");

        }

        void OnClickUpGameNpc(BaseEventData data)
        {
            // Debug.LogError("松开了田田");

        }

        void MushRoomCount()
        {
            eatMushRoomCount++;
            Numbers.GetChild(0).GetComponent<Image>().sprite = GetNumberSprite(eatMushRoomCount / 10);
            Numbers.GetChild(1).GetComponent<Image>().sprite = GetNumberSprite(eatMushRoomCount % 10);
        }

        Sprite GetNumberSprite(int num)
        {
            return NumberSprites.GetChild(num).GetComponent<Image>().sprite;
        }

    }
}
