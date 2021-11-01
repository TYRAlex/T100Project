using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class TD91263Part1
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject mask;
        GameObject unDragableMask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;
        GameObject nextButton;

        Transform ddTra;
        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        bool isPlaying = false;

        #endregion

        #region 游戏变量
        bool isKeepCreate;
        bool isCatched;

        float fixedY;
        float percentage;

        int[] nums;
        int[] maxs;
        int[] counts;
        float[][] startPositions;

        List<GameObject> fishList;

        Transform hammerTra;
        Transform fishTra;
        Transform poolsTra;
        Transform fishDropTra;

        RectTransform frameRect;

        SkeletonGraphic waterSke;

        GameObject net;
        GameObject stone;
        GameObject netFrame;
        GameObject bottom;
        GameObject bround;
        GameObject boom;

        GameObject[] blisters;

        Rigidbody2D hammerRigid1;
        Rigidbody2D hammerRigid2;

        mILDrager drag;

        BoxCollider2D collider2D;

        Coroutine[] coroutines;
        #endregion

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;
            Input.multiTouchEnabled = false;
            DOTween.KillAll();
            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            if (!ddTra)
            {
                LoadMask();

                LoadGame();//加载
            }

            MaskInit();

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            //unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;

            ddTra = maskTra.Find("DD");

            nextButton = maskTra.Find("NextButton").gameObject;

            successSpine = maskTra.Find("successSpine").gameObject;

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GamePlay);
            //Util.AddBtnClick(nextButton, NextGame);
        }

        void MaskInit()
        {
            //unDragableMask.SetActive(false);
            mask.SetActive(true);

            ddTra.GetChild(0).gameObject.SetActive(false);
            ddTra.GetChild(1).gameObject.SetActive(false);

            //nextButton.SetActive(false);

            successSpine.SetActive(false);

            caidaiSpine.SetActive(false);

            btn01.GetComponent<SkeletonGraphic>().Initialize(true);
            btn02.GetComponent<SkeletonGraphic>().Initialize(true);
            btn03.GetComponent<SkeletonGraphic>().Initialize(true);
            nextButton.GetComponent<SkeletonGraphic>().Initialize(true);

            SpineManager.instance.DoAnimation(nextButton, "next2", false);
            SpineManager.instance.DoAnimation(btn01, "next2", false);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);
        }

        void LoadGame()
        {
            fishTra = curCanvasTra.Find("fish");
            hammerTra = curCanvasTra.Find("water/hammer");
            poolsTra = curCanvasTra.Find("pools");
            fishDropTra = curCanvasTra.Find("net/drag/fishDrop");

            frameRect = curCanvasTra.Find("frame").GetRectTransform();

            stone = curCanvasTra.Find("water/stone").gameObject;
            netFrame = frameRect.Find("net").gameObject;
            net = curCanvasTra.Find("net").gameObject;
            bottom = curCanvasTra.Find("bottom").gameObject;
            bround = curCanvasTra.Find("Bround").gameObject;
            boom = curCanvasTra.Find("boom").gameObject;

            hammerRigid1 = hammerTra.GetChild(0).GetComponent<Rigidbody2D>();
            hammerRigid2 = hammerTra.GetChild(1).GetComponent<Rigidbody2D>();
            drag = net.GetComponent<mILDrager>();
            collider2D = net.GetComponent<BoxCollider2D>();

            waterSke = curCanvasTra.Find("water/1").GetComponent<SkeletonGraphic>();
            waterSke.timeScale = 0.5f;

            blisters = new GameObject[poolsTra.childCount];
            for (int i = 0; i < blisters.Length; i++)
            {
                blisters[i] = poolsTra.GetChild(i).Find("blister").gameObject;
            }
        }

        void GameInit()
        {
            isKeepCreate = false;
            isCatched = false;

            percentage = 1f;

            //🐟
            for (int i = 0; i < fishTra.childCount; i++)
            {
                GameObject obj = fishTra.GetChild(i).gameObject;
                obj.SetActive(false);

                Material material = obj.GetComponent<SkeletonGraphic>().material;
                material.SetColor("_Color", Color.white);
            }

            //忽略碰撞
            Physics2D.IgnoreLayerCollision(4, 8);

            //锤子
            hammerTra.GetChild(0).GetComponent<EventDispatcher>().CollisionEnter2D += HammerColliderEnter;
            hammerTra.GetChild(1).GetComponent<EventDispatcher>().CollisionEnter2D += HammerColliderEnter;

            hammerRigid1.velocity = Vector2.zero;
            hammerRigid2.velocity = Vector2.zero;

            hammerTra.GetChild(0).localRotation = Quaternion.Euler(0, 0, -90);
            hammerTra.GetChild(1).localRotation = Quaternion.Euler(0, 0, 90);

            //石头
            stone.GetComponent<EventDispatcher>().CollisionEnter2D += HammerColliderEnter;

            //底部
            bottom.GetComponent<EventDispatcher>().CollisionEnter2D += BottomColliiderEnter;

            //边界
            bround.GetComponent<EventDispatcher>().TriggerEnter2D += BroundTriggerEnter;

            //网
            net.transform.localScale = Vector2.zero;
            net.transform.GetRectTransform().anchoredPosition = new Vector2(-270, 280);
            fixedY = net.transform.position.y;

            net.SetActive(false);
            net.transform.GetChild(0).gameObject.SetActive(true);
            net.transform.GetChild(1).gameObject.SetActive(false);
            fishDropTra.parent.GetChild(0).gameObject.SetActive(false);

            drag.dragType = poolsTra.childCount;
            for (int i = 0; i < fishDropTra.childCount; i++)
            {
                fishDropTra.GetChild(i).gameObject.SetActive(false);
            }

            drag.SetDragCallback(DragStart, Drag, DragEnd);
            drag.isActived = true;

            //点击框
            netFrame.transform.localScale = Vector2.one;
            frameRect.anchoredPosition = Vector2.right * 300;

            //水池
            for (int i = 0; i < poolsTra.childCount; i++)
            {
                poolsTra.GetChild(i).Find("drop").GetComponent<mILDroper>().SetDropCallBack(DoAfter);
            }

            //水花
            for (int i = 0; i < blisters.Length; i++)
            {
                InitSpine(blisters[i], "", false);
            }

            //水池计数
            nums = new int[poolsTra.childCount];
            for (int i = 0; i < nums.Length; i++)
            {
                nums[i] = 0;

                //计数
                Transform tra = poolsTra.GetChild(i).GetChild(0);
                tra.localScale = Vector2.one;
                tra.GetText().text = "" + nums[i];
                tra.GetText().color = Color.white;

                //对勾
                RawImage raw = poolsTra.GetChild(i).Find("correct").GetRawImage();
                raw.gameObject.SetActive(false);
            }

            //最大计数
            maxs = new int[] { 3, 2, 3 };

            //鱼列表
            fishList = new List<GameObject>();
            for (int i = 0; i < fishTra.childCount; i++)
            {
                fishList.Add(fishTra.GetChild(i).gameObject);
            }
            Shuffle<GameObject>(ref fishList);

            //重置协程
            coroutines = new Coroutine[fishTra.childCount];

            counts = new int[fishTra.childCount];
            for (int i = 0; i < counts.Length; i++)
            {
                counts[i] = 0;
            }

            //把随机生成点分为3块
            startPositions = new float[3][]
            {
                new float[2]{ -580, -200},
                new float[2] { -200, 200},
                new float[2] {200, 540}
            };

            //boom
            InitSpine(boom, "", false);

            Util.AddBtnClick(netFrame, ClickFrame);
        }

        //开始游戏
        void StartGame()
        {
            mask.SetActive(false);

            mono.StartCoroutine(WaitFor(0.5f, () =>
            {
                //框滑出
                frameRect.DOAnchorPosX(5, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    mono.StartCoroutine(Create());

                    isPlaying = false;
                });
            }));
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2", false);
        }

        #region 游戏方法
        //点击
        void ClickFrame(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            EnlargeShrink(netFrame.transform, false, 1f, method: () =>
            {
                //网出现
                net.SetActive(true);
                net.GetComponent<EventDispatcher>().CollisionEnter2D += CatchFish;
                collider2D.enabled = true;

                EnlargeShrink(net.transform, true, 1f, method: () =>
                {
                    //框
                    frameRect.DOAnchorPosX(300, 0.8f).SetEase(Ease.OutSine).OnComplete(() =>
                    {
                        isPlaying = false;
                    });
                });
            });
        }

        //抓到鱼
        void CatchFish(Collision2D c = null, int _time = 0)
        {
            Debug.Log("CatchFish");

            //"回收"
            Transform tra = c.transform;
            FishReSet(tra.gameObject);

            //”抓鱼“
            int num = tra.GetSiblingIndex();
            GameObject obj = fishDropTra.GetChild(num).gameObject;
            obj.SetActive(true);

            if (obj.name == "demon") InitSpine(obj, "xem3");
            else InitSpine(obj, "yu" + obj.name + "2");

            int types = poolsTra.childCount;

            //设置放置类型
            if (num <= 1) types = 2;
            if (num > 1 && num < 5) types = 0;
            if (num == 5) types = 1;

            //切换状态
            drag.dragType = types;
            collider2D.enabled = false;
            isCatched = true;
        }

        //撞到锤子或石头
        void HammerColliderEnter(Collision2D c = null, int _time = 0)
        {
            GameObject obj = c.gameObject;

            if (obj.name == "demon")
            {
                SpineManager.instance.DoAnimation(obj, "xem2", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, "xem1");
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(obj, "yu" + obj.name + "3", false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, "yu" + obj.name + "1");
                });
            }
        }

        //撞到bottom
        void BottomColliiderEnter(Collision2D c = null, int _time = 0)
        {
            FishReSet(c.gameObject);
        }

        //穿过Borund
        void BroundTriggerEnter(Collider2D other = null, int _time = 0)
        {
            GameObject obj = other.gameObject;
            Material material = obj.GetComponent<SkeletonGraphic>().material;

            mono.StartCoroutine(ChangeSpineAlpha(material, 0, 0.5f, ()=>
            {
                FishReSet(obj);
            }));
        }

        //出现鱼
        void CreateFish(int startType)
        {
            if (fishList.Count == 0) return;

            //随机
            float startPositon = Random.Range(startPositions[startType][0], startPositions[startType][1]);
            int fishNumber = Random.Range(0, fishList.Count);

            //从列表中挑出一个
            GameObject obj = fishList[fishNumber];
            fishList.RemoveAt(fishNumber);
            RectTransform rect = obj.transform.GetRectTransform();
            Rigidbody2D rigidbody2D = obj.GetComponent<Rigidbody2D>();
            Material material = obj.GetComponent<SkeletonGraphic>().material;

            //重置存在时间
            int num = obj.transform.GetSiblingIndex();
            if (coroutines[num] != null) mono.StopCoroutine(coroutines[num]);
            ++counts[num];

            //初始化
            obj.SetActive(true);
            rect.anchoredPosition = new Vector2(startPositon, 25);
            rigidbody2D.velocity = Vector2.zero;
            rigidbody2D.gravityScale = 0;
            material.SetColor("_Color", new Color(1, 1, 1, 0f));
            obj.transform.localScale = Vector2.zero;

            if (obj.name == "demon") InitSpine(obj, "xem1");
            else InitSpine(obj, "yu" + obj.name + 3);

            //出现动画
            float _time = 0.8f / percentage;
            obj.transform.DOScale(Vector2.one, _time).SetEase(Ease.InSine);
            mono.StartCoroutine(ChangeSpineAlpha(material, 1, _time));
            obj.transform.GetRectTransform().DOAnchorPosY(-100, _time).SetEase(Ease.InSine);

            mono.StartCoroutine(WaitFor(_time, () =>
            {
                coroutines[num] = mono.StartCoroutine(SetExistTime(obj, num, 10f));
                rigidbody2D.gravityScale = percentage * 50;
                rigidbody2D.velocity = Vector2.down * 350 * percentage;
            }));
        }

        //随机生成🐟
        IEnumerator Create()
        {
            isKeepCreate = true;

            while (isKeepCreate)
            {
                //生成多少条
                int createTimes = Random.Range(0, startPositions.Length) + 1;

                //得到一个随机数组
                int[] nums = new int[createTimes];
                
                for (int i = 0; i < nums.Length; i++)
                {
                    nums[i] = i;
                }

                Shuffle<int>(ref nums);

                for (int i = 0; i < createTimes; i++)
                {
                    CreateFish(nums[i]);

                    yield return new WaitForSeconds(1f / percentage);
                }

                yield return new WaitForSeconds(1f / percentage);
            }
        }

        //设定存在时间(暂时存在关闭残留的bug)
        IEnumerator SetExistTime(GameObject obj, int num, float _time = 15f)
        {
            int temp = counts[num];
            float _disolveTime = 0.5f;
            Material material = obj.GetComponent<SkeletonGraphic>().material;

            yield return new WaitForSeconds(_time - _disolveTime);

            mono.StartCoroutine(ChangeSpineAlpha(material, 0, _disolveTime));

            yield return new WaitForSeconds(_disolveTime);

            //这个判断避免了bug但是没能从根本上解决
            if (material.GetColor("_Color").a == 0)
            {
                FishReSet(obj);
            }
        }

        //回收重置状态
        void FishReSet(GameObject obj)
        {
            obj.GetComponent<Rigidbody2D>().velocity = Vector2.zero;
            obj.SetActive(false);

            //如果收集满了就不回收
            int num = obj.transform.GetSiblingIndex();

            /*
            //装满后不出现对应的🐟
            if (nums[0] == maxs[0] && num < 5 && num > 1) return;
            if (nums[1] == maxs[1] && num == 5) return;
            if (nums[2] == maxs[2] && num <= 1) return;*/
            fishList.Add(obj);
        }
        #endregion

        #region 拖拽
        void DragStart(Vector3 position, int type, int index)
        {
            net.transform.position = new Vector2(Input.mousePosition.x, net.transform.position.y);
            fishDropTra.parent.GetChild(0).gameObject.SetActive(true);
            drag.index = 0;
        }

        void Drag(Vector3 position, int type, int index)
        {
            //还没抓到🐟时限制Y轴
            if (!isCatched)
            {
                net.transform.position = new Vector2(Input.mousePosition.x, fixedY);

                //限制左右范围
                RectTransform rect = net.transform.GetRectTransform();
                float _x = rect.anchoredPosition.x;
                _x = Mathf.Clamp(_x, -637, 608);
                rect.anchoredPosition = new Vector2(_x, rect.anchoredPosition.y);
            }
        }

        void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
            fishDropTra.parent.GetChild(0).gameObject.SetActive(false);

            //如果没抓到🐟跳过
            if (!isCatched) return;

            Debug.Log("DragEnd:" + isMatch);
            drag.isActived = false;

            //倒网
            net.transform.GetChild(0).gameObject.SetActive(false);
            net.transform.GetChild(1).gameObject.SetActive(true);

            //如果没有在水池范围内
            if(drag.index == 0)
            {
                boom.transform.position = net.transform.position + new Vector3(-35, -40);
                InitSpine(boom, "sc-boom", false);
            }

            mono.StartCoroutine(WaitFor(0.5f, () =>
            {
                #region 松手重置
                drag.DoReset();
                drag.dragType = poolsTra.childCount;
                collider2D.enabled = true;
                drag.isActived = true;
                isCatched = false;

                for (int i = 0; i < fishDropTra.childCount; i++)
                {
                    fishDropTra.GetChild(i).gameObject.SetActive(false);
                }
                #endregion

                //收网
                net.transform.GetChild(0).gameObject.SetActive(true);
                net.transform.GetChild(1).gameObject.SetActive(false);
            }));

            if (isMatch)
            {
                //计数增加
                if (++nums[type] > maxs[type]) nums[type] = maxs[type];
                else
                {
                    //重力恢复
                    percentage = 1f;
                    waterSke.timeScale = 0.5f;

                    //胜利
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                    //文字变化
                    Transform tra = poolsTra.GetChild(type).GetChild(0);
                    tra.GetText().text = "" + nums[type];
                    TextEnlarge(tra);
                    TextColorDisPlay(tra.GetText());

                    
                    mono.StartCoroutine(WaitFor(0.8f, () =>
                    {
                        //显示对勾
                        if(nums[type] == maxs[type])
                        {
                            RawImage raw = poolsTra.GetChild(type).Find("correct").GetRawImage();
                            ColorDisPlay(raw, _time: 0.8f);
                        }

                        //判断结束
                        if (IsSameArray(nums, maxs))
                        {
                            GameEnd();
                        }
                    }));
                }
            }
            else
            {
                //错误抖动
                net.transform.DOShakePosition(0.5f, 8);

                mono.StartCoroutine(WaitFor(0.8f, () =>
                {
                    //重力加快
                    percentage = 1.5f;
                    waterSke.timeScale = 0.75f;
                }));
            }
        }

        bool DoAfter(int dragType, int index, int dropType)
        {
            if (!isCatched) return dragType == dropType;

            drag.index = 1;

            //水花动画
            string blisterAnimation = dropType == 1 ? "sw2" : "sw1";
            SpineManager.instance.DoAnimation(blisters[dropType], blisterAnimation, false);

            return dragType == dropType;
        }
        #endregion

        #region 游戏通用环节
        void GamePlay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            //SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                ddTra.GetChild(0).gameObject.SetActive(true);

                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(0).gameObject, SoundManager.SoundType.COMMONVOICE, 0, null, () =>
                {
                    btn03.SetActive(false);
                    ddTra.GetChild(0).gameObject.SetActive(false);

                    StartGame();
                }));
            });

        }

        void GameEnd()
        {
            isPlaying = true;
            isKeepCreate = false;
            drag.isActived = false;

            mono.StartCoroutine(WaitFor(2f, () =>
            {
                mask.SetActive(true);
                btn03.SetActive(false);
                successSpine.SetActive(true);
                caidaiSpine.SetActive(true);

                SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

                SpineManager.instance.DoAnimation(successSpine, "6-12-z", false, () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, "6-12-z2", false, () =>
                    {
                        successSpine.SetActive(false);
                        caidaiSpine.SetActive(false);

                        SpineManager.instance.DoAnimation(btn01, "fh2", false);
                        SpineManager.instance.DoAnimation(btn02, "ok2", false);

                        btn01.SetActive(true);
                        btn02.SetActive(true);

                        isPlaying = false;
                    });
                });
            }));
        }

        //重玩
        void Replay(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;
            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                MaskInit();
                GameInit();


                StartGame();
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            if (isPlaying) return;
            isPlaying = true;

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(ddTra.GetChild(1).gameObject, SoundManager.SoundType.COMMONVOICE, 0));

                ddTra.GetChild(1).gameObject.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);

                isPlaying = false;
            });
        }

        #endregion

        #region 通用方法
        //物体渐变显示或者消失
        void ColorDisPlay(RawImage raw, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                raw.color = new Color(255, 255, 255, 0);
                raw.gameObject.SetActive(true);
                raw.DOColor(Color.white, _time).SetEase(Ease.OutSine).OnComplete(() => method?.Invoke());
            }
            else
            {
                raw.color = Color.white;
                raw.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.OutSine).OnComplete(() =>
                {
                    raw.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //协程:改变Spine透明度
        IEnumerator ChangeSpineAlpha(Material material, float aimAlpha, float time, Action method = null)
        {
            float i = 0;
            float curAlpha = material.GetColor("_Color").a;
            float deltaAlpha = aimAlpha - curAlpha;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= time)
            {
                material.SetColor("_Color", new Color(1, 1, 1, curAlpha + deltaAlpha * i / time));

                yield return wait;
                i += Time.fixedDeltaTime;
            }

            method?.Invoke();
        }

        //List洗牌
        void Shuffle<T>(ref List<T> t)
        {
            for (int i = 0, n = t.Count; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }

        void Shuffle<T>(ref T[] t)
        {
            for (int i = 0, n = t.Length; i < n; ++i)
            {
                int j = (Random.Range(0, int.MaxValue)) % (i + 1);
                T temp = t[i];
                t[i] = t[j];
                t[j] = temp;
            }
        }

        //比较数组是否相同
        bool IsSameArray(int[] _a, int[] _b)
        {
            if (_a.Length != _b.Length) return false;

            int n = _a.Length;
            for (int i = 0; i < n; ++i)
            {
                if (_a[i] != _b[i]) return false;
            }

            return true;
        }

        //字体放大
        void TextEnlarge(Transform tra, bool isEnlarge = true, float time = 0.5f, Action method = null)
        {
            Vector2 curScale = Vector2.one;

            tra.localScale = Vector3.zero;

            if (isEnlarge)
            {
                tra.DOScale(curScale, time).SetEase(Ease.OutExpo).OnComplete(() =>
                {
                    method?.Invoke();
                });
            }
        }

        //字体渐变
        void TextColorDisPlay(Text text, bool isShow = true, Action method = null, float _time = 0.5f)
        {
            if (isShow)
            {
                text.color = new Color(255, 255, 255, 0);
                text.gameObject.SetActive(true);
                text.DOColor(Color.white, _time).SetEase(Ease.OutExpo).OnComplete(() => method?.Invoke());
            }
            else
            {
                text.color = Color.white;
                text.DOColor(new Color(255, 255, 255, 0), _time).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    text.gameObject.SetActive(false);
                    method?.Invoke();
                });
            }
        }

        //缩小和放大效果
        void EnlargeShrink(Transform _tra, bool _isEnlarge = true, float _time = 0.5f, Action method = null)
        {
            if (_isEnlarge)
            {
                _tra.DOScale(Vector2.one * 1.1f, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _tra.DOScale(Vector2.one, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        method?.Invoke();
                    });
                });
            }
            else
            {
                _tra.DOScale(Vector2.one * 1.1f, 0.1f).SetEase(Ease.InOutSine).OnComplete(() =>
                {
                    _tra.DOScale(Vector2.zero, 0.4f).SetEase(Ease.InOutSine).OnComplete(() =>
                    {
                        method?.Invoke();
                    });
                });
            }
        }


        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker) speaker = ddTra.GetChild(0).gameObject;

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");


            method_2?.Invoke();
        }

        //协程:等待
        IEnumerator WaitFor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //Spine初始化
        void InitSpine(GameObject _obj, string animation, bool isLoop = true)
        {
            SkeletonGraphic _ske = _obj.GetComponent<SkeletonGraphic>();

            _ske.startingAnimation = animation;
            _ske.startingLoop = isLoop;
            _ske.Initialize(true);
        }
        #endregion
    }
}
