using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

namespace ILFramework.HotClass
{
    public class TD5653Part5
    {
        #region 常用变量
        int talkIndex;

        GameObject curCanvas;
        GameObject tt;
        GameObject mask;
        GameObject unDragableMask;
        GameObject successSpine;
        GameObject caidaiSpine;
        GameObject btn01;
        GameObject btn02;
        GameObject btn03;

        Transform curCanvasTra;
        Transform maskTra;

        MonoBehaviour mono;

        //bool isPlaying = false;
        #endregion

        #region 游戏变量
        bool isGameStart = false;
        bool isDemonShow = false;
        bool isShooted = false;

        int targetIndex = 0;
        int flag = 0;
        int targetnums = 0;

        GameObject arrowAnimation;
        GameObject arrow;
        GameObject jpg;
        GameObject[] life;
        GameObject demon;
        GameObject demonTarget;
        GameObject net;
        GameObject smoke;
        GameObject star;

        RawImage totemImage;
        RawImage[] targetImage;

        BellSprites totemImages;
        BellSprites targetImages;

        Transform backGroundTra;
        Transform archTra;
        Transform netsTra;
        #endregion

        #region 情景对话 用于情景对话，需要的自行复制在 Dialogues路径下找对应spine

        private GameObject buDing;
        private GameObject devil;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        private Text buDingText;
        private Text devilText;
        #endregion         

        void Start(object o)
        {
            curCanvas = (GameObject)o;
            curCanvasTra = curCanvas.transform;

            mono = curCanvasTra.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            LoadMask();

            LoadTalk();

            LoadGame();//加载

            GameInit();

            MaskStart();
        }

        void LoadMask()
        {
            unDragableMask = curCanvasTra.Find("UnDragableMask").gameObject;
            unDragableMask.SetActive(false);

            maskTra = curCanvasTra.Find("mask");
            mask = maskTra.gameObject;
            mask.SetActive(true);

            tt = maskTra.Find("TT").gameObject;
            tt.transform.GetRectTransform().anchoredPosition = new Vector2(255, -84);
            tt.transform.localScale = new Vector2(0.45f, 0.45f);
            tt.SetActive(false);

            successSpine = maskTra.Find("successSpine").gameObject;
            successSpine.SetActive(false);

            caidaiSpine = maskTra.Find("caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            btn01 = maskTra.Find("Btns/0").gameObject;
            btn02 = maskTra.Find("Btns/1").gameObject;
            btn03 = maskTra.Find("Btns/2").gameObject;

            btn01.GetComponent<SkeletonGraphic>().Initialize(true);
            btn02.GetComponent<SkeletonGraphic>().Initialize(true);
            btn03.GetComponent<SkeletonGraphic>().Initialize(true);

            btn01.SetActive(false);
            btn02.SetActive(false);
            btn03.SetActive(false);

            Util.AddBtnClick(btn01, Replay);
            Util.AddBtnClick(btn02, Win);
            Util.AddBtnClick(btn03, GameStart);

            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
        }

        void LoadTalk()
        {
            //对话部分
            bdStartPos = curCanvasTra.Find("mask/bdStartPos");
            bdEndPos = curCanvasTra.Find("mask/bdEndPos");
            devilStartPos = curCanvasTra.Find("mask/devilStartPos");
            devilEndPos = curCanvasTra.Find("mask/devilEndPos");

            buDing = curCanvasTra.Find("mask/Tian").gameObject;

            devil = curCanvasTra.Find("mask/devil").gameObject;

            buDingText = buDing.transform.Find("Text").GetComponent<Text>();
            devilText = devil.transform.Find("Text").GetComponent<Text>();
            buDingText.text = string.Empty;
            devilText.text = string.Empty;

            buDing.transform.position = bdStartPos.position;
            devil.transform.position = devilStartPos.position;
        }

        void LoadGame()
        {
            talkIndex = 1;

            archTra = curCanvasTra.Find("Arch");
            backGroundTra = curCanvasTra.Find("BackGround");

            targetImage = new RawImage[5];
            for (int i = 0; i < 5; ++i)
            {
                targetImage[i] = backGroundTra.Find("Target" + (i + 1)).GetRawImage();
            }

            targetImages = backGroundTra.Find("Bg").GetComponent<BellSprites>();

            totemImage = backGroundTra.Find("Totem").GetRawImage();
            totemImages = backGroundTra.Find("Totem").GetComponent<BellSprites>();

            star = totemImage.transform.GetChild(0).gameObject;
            star.SetActive(false);

            arrow = backGroundTra.Find("Arrow").gameObject;
            arrow.SetActive(false);

            arrowAnimation = backGroundTra.Find("ArrowAnimation").gameObject;
            arrowAnimation.SetActive(false);

            //生命值
            jpg = curCanvasTra.Find("Hp/JPG").gameObject;

            life = new GameObject[3];
            for (int i = 0; i < 3; ++i)
            {
                life[i] = curCanvasTra.Find("Hp/Life" + (i + 1)).gameObject;
                life[i].SetActive(true);
            }

            smoke = curCanvasTra.Find("Hp/Smoke").gameObject;
            smoke.SetActive(false);

            jpg = curCanvasTra.Find("Hp/JPG").gameObject;

            demon = curCanvasTra.Find("Demon").gameObject;
            demon.SetActive(false);

            demonTarget = demon.transform.Find("DemonTarget").gameObject;
            demonTarget.SetActive(true);

            //清除生成的网
            netsTra = backGroundTra.Find("Nets");
            ClearNets();

            net = backGroundTra.Find("Net").gameObject;
            net.SetActive(false);

            Util.AddBtnClick(archTra.GetChild(0).gameObject, Shoot);
        }

        //初始化
        void GameInit()
        {
            isGameStart = false;
            isDemonShow = false;
            isShooted = false;

            flag = 0;

            archTra.transform.GetRectTransform().anchoredPosition = new Vector2(800, 0);

            arrowAnimation.GetComponent<SkeletonGraphic>().Initialize(true);
            archTra.GetComponent<SkeletonGraphic>().Initialize(true);
            demon.GetComponent<SkeletonGraphic>().Initialize(true);
            jpg.GetComponent<SkeletonGraphic>().Initialize(true);
            star.GetComponent<SkeletonGraphic>().Initialize(true);

            totemImage.texture = totemImages.texture[flag];

            SpineManager.instance.DoAnimation(jpg, "xem1", false);
            SpineManager.instance.DoAnimation(archTra.gameObject, "GJ", false);
        }

        void MaskStart()
        {
            SoundManager.instance.ShowVoiceBtn(false);

            btn03.SetActive(true);
            SpineManager.instance.DoAnimation(btn03, "bf2");
        }

        #region 点击按钮

        void GameStart(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn03, "bf", false, () =>
            {
                DevilSlide();
                btn03.Hide();
            });
        }

        //情景对话
        void DevilSlide()
        {
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 0, true);
            btn03.SetActive(false);
            devil.SetActive(true);
            buDing.SetActive(true);


            devil.transform.DOMove(devilEndPos.position, 1).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 1, () => mono.StartCoroutine(IEShowDialogue("哈哈哈哈，本大王来啦", devilText)), () =>
                {
                    buDing.transform.DOMove(bdEndPos.position, 1).OnComplete(() =>
                    {
                        mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 0, () => mono.StartCoroutine(IEShowDialogue("加油，我们一定会战胜小恶魔布鲁鲁的", buDingText)), () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        }));
                    });
                }));
            });
        }

        void TalkClick()
        {
            SoundManager.instance.PlayClip(9);//点击声音
            SoundManager.instance.ShowVoiceBtn(false);

            switch (talkIndex)
            {
                case 1:
                    tt.Show();
                    buDing.SetActive(false);
                    devil.SetActive(false);

                    mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 3, null, () =>
                    {
                        GamePlay();
                    }));

                    break;
            }

            ++talkIndex;
        }

        //射箭
        void Shoot(GameObject obj)
        {
            if (isShooted) return;
            isShooted = true;

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);

            mono.StartCoroutine(WaitCor(1f, () =>
            {
                RaycastHit2D hit = Physics2D.Raycast(archTra.position, Vector2.up * 1000, int.MaxValue, LayerMask.GetMask("PostProcessing"));

                if (hit)
                {

                    //打中小恶魔
                    if (hit.collider.gameObject.name == "DemonTarget")
                    {

                        GameObject go = Object.Instantiate(net, netsTra);
                        go.transform.GetRectTransform().anchoredPosition = new Vector2(Random.Range(-100, 700), Random.Range(-100, 200));
                        go.SetActive(true);
                        go.GetComponent<SkeletonGraphic>().raycastTarget = false;
                        SpineManager.instance.DoAnimation(go, "w" + (flag + 1), false, () =>
                        {
                            isShooted = false;
                        });

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);

                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);

                        int temp = int.Parse(hit.collider.gameObject.transform.GetRawImage().texture.name);

                        arrowAnimation.transform.GetRectTransform().anchoredPosition = hit.transform.GetRectTransform().anchoredPosition + Vector2.down * 100;
                        arrowAnimation.SetActive(true);

                        SpineManager.instance.DoAnimation(arrowAnimation, "bz-L" + temp, false, () =>
                        {
                            mono.StartCoroutine(WaitCor(1f, () =>
                            {
                                arrowAnimation.SetActive(false);
                                arrowAnimation.GetComponent<SkeletonGraphic>().Initialize(true);
                                isShooted = false;
                            }));

                            if (temp == targetIndex)
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);

                                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 9), false);

                                int j = 0;
                                while (j + 1 == targetIndex) j = Random.Range(0, 6);

                                hit.collider.gameObject.transform.GetRawImage().texture = targetImages.texture[j];

                                //判断是否还有剩余的正确选项
                                if (--targetnums == 0)
                                {
                                    mono.StartCoroutine(WaitCor(1f, () =>
                                    {
                                        ClearNets();

                                        smoke.transform.GetRectTransform().anchoredPosition = life[flag].transform.GetRectTransform().anchoredPosition;
                                        smoke.SetActive(true);
                                        SpineManager.instance.DoAnimation(smoke, "boom", false, () =>
                                        {
                                            smoke.SetActive(false);
                                        });

                                        life[flag].SetActive(false);

                                        if (++flag >= 3)
                                        {
                                            unDragableMask.SetActive(true);
                                            demon.SetActive(false);
                                            isGameStart = false;
                                            isShooted = true;
                                            mono.StopAllCoroutines();

                                            mono.StartCoroutine(WaitCor(2f, () =>
                                            {
                                                GameEnd();
                                            }));
                                        }
                                        else
                                        {
                                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);

                                            star.SetActive(true);
                                            SpineManager.instance.DoAnimation(star, "star", false, () =>
                                            {
                                                star.SetActive(false);
                                            });

                                            SpineManager.instance.DoAnimation(jpg, "xem" + (flag + 1));
                                            totemImage.texture = totemImages.texture[flag];
                                            RandomTarget(flag + 1);
                                        }
                                    }));
                                }
                            }
                            else
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                            }
                        });
                    }
                }
                else
                {
                    isShooted = false;
                }
            }));

            SpineManager.instance.DoAnimation(archTra.gameObject, "GJ2", false, () =>
            {
                SpineManager.instance.DoAnimation(archTra.gameObject, "GJ", false);
            });
        }

        //重玩
        void Replay(GameObject obj)
        {
            unDragableMask.SetActive(true);

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn01, "fh", false, () =>
            {
                LoadMask();

                LoadTalk();

                LoadGame();//加载

                GameInit();

                GamePlay();
            });
        }

        //胜利
        void Win(GameObject obj)
        {
            unDragableMask.SetActive(true);

            tt.transform.GetRectTransform().anchoredPosition = new Vector2(973, -101);
            tt.transform.localScale = new Vector2(0.6f, 0.6f);

            SoundManager.instance.PlayClip(9);

            SpineManager.instance.DoAnimation(btn02, "ok", false, () =>
            {
                mono.StartCoroutine(SpeckerCoroutine(tt, SoundManager.SoundType.VOICE, 2));
                tt.SetActive(true);
                btn01.SetActive(false);
                btn02.SetActive(false);
            });
        }
        #endregion

        #region 游戏方法
        //游戏开始
        void GamePlay()
        {
            tt.Hide();
            mask.SetActive(false);
            mono.StartCoroutine(ArchMove());
            mono.StartCoroutine(DemonShow());
            unDragableMask.Hide();
            RandomTarget(0);
            totemImage.texture = totemImages.texture[flag];
        }

        //清除网
        void ClearNets()
        {
            if (netsTra.childCount > 0)
            {
                for (int i = 0, n = netsTra.childCount; i < n; ++i)
                {
                    Object.Destroy(netsTra.GetChild(i).gameObject);
                }
            }
        }

        //随机化箭靶
        void RandomTarget(int num)
        {
            targetIndex = num + 1;
            Texture[] textures = new Texture[5];

            int n = textures.Length;
            for (int i = 0; i < n; ++i)
            {
                int j = Random.Range(0, n);
                while (j == num) j = Random.Range(0, n);

                textures[i] = targetImages.texture[j];
            }

            //随机一到两个是正确的
            targetnums = Random.Range(1, 3);

            for (int i = 0; i < targetnums; ++i)
            {
                int j = Random.Range(0, n);

                while (textures[j] == targetImages.texture[num])
                {
                    j = Random.Range(0, n);
                }

                textures[j] = targetImages.texture[num];
            }

            textures = shuffle<Texture>(textures);

            for (int i = 0; i < n; ++i)
            {
                targetImage[i].texture = textures[i];
            }
        }

        //小恶魔随机出现
        IEnumerator DemonShow()
        {
            isGameStart = true;

            while (isGameStart)
            {
                yield return new WaitForSeconds(Random.Range(2f, 4f));

                mono.StartCoroutine(DemonTargetRotate());

                yield return new WaitForSeconds(Random.Range(3f, 5f));

                demon.SetActive(false);
                isDemonShow = false;
                demon.GetComponent<SkeletonGraphic>().Initialize(true);
            }
        }

        //弓箭左右移动
        IEnumerator ArchMove()
        {
            isGameStart = true;

            while (isGameStart)
            {
                yield return mono.StartCoroutine(SetMoveAncPosX(archTra, -400, 4f));
                yield return mono.StartCoroutine(SetMoveAncPosX(archTra, 800, 4f));
            }
        }

        //小恶魔摇摆
        IEnumerator DemonTargetRotate()
        {
            demon.transform.GetRectTransform().anchoredPosition = new Vector2(Random.Range(-350, -1200), 0);
            SpineManager.instance.DoAnimation(demon, "xem-d1", true);
            demon.SetActive(true);

            isDemonShow = true;
            demonTarget.transform.rotation = Quaternion.Euler(0, 0, 0);
            yield return 0;

            while (isDemonShow)
            {
                yield return mono.StartCoroutine(Rotate(demonTarget.transform, 70, 1.85f));
                yield return new WaitForSeconds(0.1f);
                //yield return mono.StartCoroutine(Rotate(demonTarget.transform, -70, 1f));
            }
        }

        //协程:旋转(先变快后变慢)
        IEnumerator Rotate(Transform transform, float deltaAngle, float duration = 1f, Action callBack = null)
        {
            float i = 0;
            float curAngle = transform.rotation.eulerAngles.z;
            float time = duration;
            float BesselVar = 0;

            WaitForFixedUpdate wait = new WaitForFixedUpdate();


            while (i <= time)
            {
                transform.rotation = Quaternion.Euler(0, 0, curAngle + deltaAngle * i * BesselVar / time);
                yield return wait;
                i += Time.fixedDeltaTime;

                if (i <= time / 2)
                {
                    BesselVar = 4 * (i / time);
                }
                else
                {
                    BesselVar = 4 * (1 - i / time);
                }
            }

            callBack?.Invoke();
        }
        #endregion


        #region 通用方法
        IEnumerator SetMoveAncPosX(Transform temp, float aim, float duration = 1f, Action callBack = null)
        {
            float i = 0;

            float curPosx = temp.GetRectTransform().anchoredPosition.x;
            float curPosy = temp.GetRectTransform().anchoredPosition.y;
            float distance = aim - curPosx;
            duration *= Mathf.Abs(distance / 1200);

            WaitForFixedUpdate wait = new WaitForFixedUpdate();

            while (i <= duration)
            {
                if (isShooted) break;
                temp.GetRectTransform().anchoredPosition = new Vector2(curPosx + distance * i / duration, curPosy);
                yield return wait;
                i += Time.fixedDeltaTime;
            }
            
            callBack?.Invoke();
        }

        //对话框
        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0");  //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(0.1f);
                text.text += str[i];
                if (i == 25)
                {
                    text.text = "";
                }
                i++;
            }
            callBack?.Invoke();
            yield break;
        }

        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {

            if (!speaker) speaker = tt;

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");


            method_2?.Invoke();
        }

        IEnumerator WaitCor(float _time, Action method = null)
        {
            yield return new WaitForSeconds(_time);

            method?.Invoke();
        }

        //洗牌算法
        T[] shuffle<T>(T[] t)
        {
            T[] ret = t;
            for (int i = 0, n = ret.Length; i < n; ++i)
            {
                int j = ((int)Random.value) % (i + 1);
                T temp = ret[i];
                ret[i] = ret[j];
                ret[j] = temp;
            }
            return ret;
        }

        void GameEnd()
        {
            mask.SetActive(true);
            btn03.SetActive(false);
            tt.SetActive(false);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);

            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3);

            SpineManager.instance.DoAnimation(successSpine, "3-5-z", false, () =>
            {
                SpineManager.instance.DoAnimation(successSpine, "3-5-z2", false, () =>
                {
                    successSpine.SetActive(false);
                    caidaiSpine.SetActive(false);

                    SpineManager.instance.DoAnimation(btn01, "fh2", false);
                    SpineManager.instance.DoAnimation(btn02, "ok2", false);

                    btn01.SetActive(true);
                    btn02.SetActive(true);

                    unDragableMask.SetActive(false);
                });
            });
        }
        #endregion
    }
}
