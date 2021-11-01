
using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
        next,
    }

    public class TD5661Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private GameObject btnBack;


        // 游戏逻辑变量
        private Transform panel;
        private Transform levelPanel;
        private GameObject jianZhiSpine;
        private Transform nPanel;

        private Transform anniuPanel;

        private struct AnNiu
        {
            public GameObject anNiuEffectSpine;
            public GameObject anniu;
            public Image mask;
        }

        List<AnNiu> listBtn;

        private Transform level2Panel;

        private RectTransform leftPos;
        private RectTransform rightPos;

        private Transform di;
        private Scrollbar scrollbar;
        private Transform niaoPanel;

        private struct Niao
        {
            public GameObject niao;
            public Rigidbody2D rigidbody2D;
            public EventDispatcher eventDispatcher;
            public Image img;
            public BellSprites bellSprites;
            public void Dispose()
            {
                niao = null;
                rigidbody2D = null;
                eventDispatcher = null;
                img = null;
                bellSprites = null;
            }
        }
        List<Niao> listNiaos;
        private Transform di2;
        private Transform niao2Panel;
        private Transform bkSpine;

        private Transform xemTran;
        private Transform gongjiSpine;

        private GameObject teiqiangSpine;
        private GameObject endSpine;
        //胜利动画名字
        private string tz;

        private bool isPlaying = false;
        private int level = 0;

        private int flag = 0;

        private int[] randomArray;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(true);


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;
            dbd.SetActive(false);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = successSpine.transform.GetChild(0).gameObject;
            caidaiSpine.SetActive(false);
            anyBtns = curTrans.Find("mask/Btns");
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                Util.AddBtnClick(anyBtns.GetChild(i).gameObject, OnClickAnyBtn);
                anyBtns.GetChild(i).gameObject.SetActive(false);
            }
            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            ChangeClickArea();

            btnBack = curTrans.Find("btnBack").gameObject;
            //Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            tz = "3-5-z";

            panel = curTrans.Find("panel");
            levelPanel = panel.Find("levelPanel");
            jianZhiSpine = levelPanel.Find("jianzhi2").gameObject;
            nPanel = levelPanel.Find("nPanel");
            anniuPanel = levelPanel.Find("anniu");

            listBtn = new List<AnNiu>();
            AnNiu tem = new AnNiu();
            for (int i = 0; i < anniuPanel.childCount - 1; i++)
            {
                tem.anNiuEffectSpine = anniuPanel.GetChild(i).gameObject;
                tem.anniu = anniuPanel.GetChild(i).GetChild(0).gameObject;
                Util.AddBtnClick(tem.anniu, OnClickAnNiu);
                tem.mask = anniuPanel.GetChild(i).GetChild(1).GetComponent<Image>();
                listBtn.Add(tem);
            }
            level2Panel = panel.Find("level2Panel");

            leftPos = level2Panel.Find("leftPos").GetRectTransform();
            rightPos = level2Panel.Find("rightPos").GetRectTransform();

            di = level2Panel.Find("di");
            scrollbar = di.Find("bian/Scrollbar").GetComponent<Scrollbar>();
            niaoPanel = di.Find("niao");
            di2 = level2Panel.Find("di2");
            niao2Panel = di2.Find("niao");

            bkSpine = di2.Find("bk");
            listNiaos = new List<Niao>(new Niao[niaoPanel.childCount]);

            xemTran = level2Panel.Find("xem");

            gongjiSpine = level2Panel.Find("gongji");
            teiqiangSpine = level2Panel.Find("tieqiang").gameObject;

            endSpine = panel.Find("animation").gameObject;
            endSpine.SetActive(false);
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            randomArray = new int[] { 0, 1, 2, 0, 1, 2 };
            GameInit();
            //GameStart();
        }

        private void OnClickAnNiu(GameObject obj)
        {

            int temIndex = obj.transform.parent.GetSiblingIndex();
            if ((flag & (1 << temIndex)) > 0)
                return;
            flag += (1 << temIndex);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(listBtn[temIndex].anNiuEffectSpine, "kong", false);
            if (temIndex == 0)
            {
                btnBack.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
                SpineManager.instance.DoAnimation(jianZhiSpine, obj.name, false,
                    () =>
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                        mono.StartCoroutine(StepPlaySpine(temIndex));
                    });
                //美术需要这样的效果，只能写死动画时长，每个静止动画有0.5f的时长 13.5f =  3*0.5+12;
                listBtn[temIndex + 1].mask.transform.GetComponent<Image>().DOFillAmount(0, 13.5f).OnComplete(() => 
                { 
                    btnBack.SetActive(false);
                    listBtn[temIndex + 1].mask.transform.GetComponent<Image>().raycastTarget = false;
                });
            }
            else
            {
                mono.StartCoroutine(StepPlaySpine(temIndex, obj));
            }
        }


        IEnumerator StepPlaySpine(int temIndex)
        {

            float time = 0;
            for (int i = 0; i < nPanel.childCount; i++)
            {
                nPanel.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
                time = SpineManager.instance.DoAnimation(nPanel.GetChild(i).gameObject, nPanel.GetChild(i).name + 3, false);
                yield return new WaitForSeconds(time + 0.5f);
            }
            StepBtnEvent(temIndex + 1);
        }
        IEnumerator StepPlaySpine(int temIndex, GameObject obj)
        {
            btnBack.SetActive(true);
            float duration = 0;
            for (int i = 0; i < nPanel.childCount; i++)
            {
                int tem = i;
                if (temIndex == 2)
                {
                    SpineManager.instance.GetAnimationLength(nPanel.GetChild(tem).gameObject, nPanel.GetChild(tem).name + 2);
                }
                duration += SpineManager.instance.GetAnimationLength(nPanel.GetChild(tem).GetChild(temIndex - 1).gameObject, obj.name);
                if (temIndex == 1)
                {
                    duration += SpineManager.instance.GetAnimationLength(nPanel.GetChild(tem).gameObject, nPanel.GetChild(tem).name);
                }
                duration += SpineManager.instance.GetAnimationLength(nPanel.GetChild(tem).GetChild(temIndex - 1).gameObject, "kong");
                duration += 0.5f;

            }
            //以上为了获取mask动画的过度时长
            if (temIndex < listBtn.Count - 1)
            {
                listBtn[temIndex + 1].mask.transform.GetComponent<Image>().DOFillAmount(0, duration).OnComplete(() => 
                {
                    listBtn[temIndex + 1].mask.transform.GetComponent<Image>().raycastTarget = false;
                    btnBack.SetActive(false); 
                });
            }

            for (int i = 0; i < nPanel.childCount; i++)
            {
                float time = 0;
                int tem = i;
                if (temIndex == 2)
                {
                    SpineManager.instance.DoAnimation(nPanel.GetChild(tem).gameObject, nPanel.GetChild(tem).name + 2, false);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, temIndex + 1, false);
                time += SpineManager.instance.DoAnimation(nPanel.GetChild(tem).GetChild(temIndex - 1).gameObject, obj.name, false,
                    () =>
                    {
                        if (temIndex == 1)
                        {
                            SpineManager.instance.DoAnimation(nPanel.GetChild(tem).gameObject, nPanel.GetChild(tem).name, false);
                        }
                        time += SpineManager.instance.DoAnimation(nPanel.GetChild(tem).GetChild(temIndex - 1).gameObject, "kong", false);

                    });
                yield return new WaitForSeconds(time + 0.5f);

            }
            if (temIndex < listBtn.Count - 1)
            {
                StepBtnEvent(temIndex + 1);
            }
            else
            {
                mono.StartCoroutine(DelayFunc(2f, () =>
                {
                    mask.SetActive(true);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                    ChangeClickArea();
                }));

            }
        }
        //private void OnClickBtnBack(GameObject obj)
        //{
        //    if (isPlaying)
        //        return;
        //    BtnPlaySound();
        //    obj.SetActive(false);
        //    SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);

        //    GameInit();
        //}

        private int numTime = 0;
        private bool isSuccess = false;
        private void OnCollisionEnter2D(Collision2D c, int time)
        {
            if (c.gameObject.name == "bk")
            {
                numTime++;
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6, false);
                SpineManager.instance.DoAnimation(bkSpine.gameObject, bkSpine.name + (numTime + 1), false, () =>
                {
                    if (numTime >= 2)
                    {
                        bkSpine.gameObject.SetActive(false);
                        numTime = 0;
                    }

                });
            }

            if (c.gameObject.name != c.otherCollider.name && c.transform.childCount > 0 && c.transform.GetChild(0).name == c.otherCollider.transform.GetChild(0).name)
            {
                btnBack.SetActive(true);
                Component.Destroy(c.transform.GetComponent<UIEventListener>());
                Component.Destroy(c.otherCollider.transform.GetComponent<UIEventListener>());
                if ((flag & (1 << int.Parse(c.transform.GetChild(0).name))) == 0)
                {
                    flag += (1 << int.Parse(c.transform.GetChild(0).name));
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                }
                c.rigidbody.velocity = Vector2.zero;
                mono.StartCoroutine(PlayBoomHide(c.gameObject));
            }
        }

        IEnumerator PlayBoomHide(GameObject obj)
        {
            float time = 0;
            obj.transform.GetChild(0).gameObject.SetActive(false);
            //time = SpineManager.instance.DoAnimation(obj.transform.GetChild(1).gameObject, obj.transform.GetChild(1).name, false);
            //yield return new WaitForSeconds(time);
           
            time = SpineManager.instance.DoAnimation(obj, "boom", false);
            yield return new WaitForSeconds(time);
            obj.gameObject.SetActive(false);
            btnBack.SetActive(false);
            if ((flag == Mathf.Pow(2, niaoPanel.childCount / 2) - 1) && !isSuccess)
            {
                isSuccess = true;
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4,10), false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 7, false);
                SpineManager.instance.DoAnimation(gongjiSpine.GetChild(0).gameObject, gongjiSpine.GetChild(0).name, false);
                float delayTime = SpineManager.instance.DoAnimation(gongjiSpine.gameObject, gongjiSpine.name + (level == 1 ? 3 : 2), false);
                mono.StartCoroutine(DelayFunc(delayTime / 5 * 3,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        SpineManager.instance.DoAnimation(xemTran.GetChild(xemTran.childCount - level).GetChild(0).gameObject, xemTran.name + "-y", true);

                        xemTran.GetChild(xemTran.childCount - level).GetChild(0).DOScale(Vector3.one * 2, 0.8f);
                        xemTran.GetChild(xemTran.childCount - level).GetChild(0).DOMove(teiqiangSpine.transform.position, 0.8f).OnComplete(
                              () =>
                              {
                                  SpineManager.instance.DoAnimation(xemTran.GetChild(xemTran.childCount - level).GetChild(0).gameObject, "kong", false);
                                  SpineManager.instance.DoAnimation(teiqiangSpine, teiqiangSpine.name, false,
                                      () =>
                                      {
                                          SpineManager.instance.DoAnimation(teiqiangSpine, "kong", false,
                                              () =>
                                              {
                                                  if (level >= 2)
                                                  {
                                                      SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, true);
                                                      mono.StartCoroutine(DelayFunc(1.5f, () =>
                                                      {
                                                          endSpine.SetActive(true);
                                                          SpineManager.instance.DoAnimation(endSpine, endSpine.name, true);
                                                          mono.StartCoroutine(DelayFunc(3f, () => { playSuccessSpine(); }));
                                                      }));
                                                  }
                                                  else
                                                  {
                                                      mask.SetActive(true);
                                                      anyBtns.gameObject.SetActive(true);
                                                      anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                                      anyBtns.GetChild(1).gameObject.Hide();
                                                      ChangeClickArea();
                                                  }

                                              });
                                      });
                              });
                        SpineManager.instance.DoAnimation(xemTran.GetChild(xemTran.childCount - level).GetChild(1).gameObject, "sc-boom", false);
                    }));




            }
        }

        IEnumerator DelayFunc(float delayTime, Action callback)
        {
            yield return new WaitForSeconds(delayTime);
            callback?.Invoke();
        }
        private void GetRandomArray<T>(T[] arr)
        {
            System.Random r = new System.Random();
            for (int i = 0; i < arr.Length; i++)
            {
                int tem = r.Next(arr.Length);
                T temItem = arr[i];
                arr[i] = arr[tem];
                arr[tem] = temItem;
            }

        }

        List<int> temInts;
        private int GetRandomNum(int len)
        {
            int tem = -1;
            while (tem < 0)
            {
                tem = Random.Range(0, len);
                if (temInts != null && temInts.Contains(tem))
                {
                    tem = -1;
                }
                else
                {
                    temInts.Add(tem);
                }
            }
            return tem;
        }
        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum, int index)
        {
            string result = string.Empty;
            switch (btnEnum)
            {
                case BtnEnum.bf:
                    result = "bf";
                    break;
                case BtnEnum.fh:
                    result = "fh";
                    break;
                case BtnEnum.ok:
                    result = "ok";
                    break;
                case BtnEnum.next:
                    result = "next";
                    break;
                default:
                    break;
            }
            SpineManager.instance.DoAnimation(anyBtns.GetChild(index).gameObject, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            if (isPlaying)
                return;
            isPlaying = true;
            BtnPlaySound();
            //SoundManager.instance.StopAudio(SoundManager.SoundType.BGM); 
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "next")
                {
                    if(level == 0)
                    {
                        AnyBtnNextStep(obj, () =>
                        {
                            bd.SetActive(true);
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () =>
                            {
                                mask.SetActive(false);
                                bd.SetActive(false);
                                level++;
                                isSuccess = false;
                                GameInitLevel(level);
                            }));

                        });
                    }
                    else
                    {
                        mask.SetActive(false);
                        bd.SetActive(false);
                        level++;
                        isSuccess = false;
                        GameInitLevel(level);
                    }
                }
                else if (obj.name == "bf")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                    AnyBtnNextStep(obj, GameStart);
                }
                else if (obj.name == "fh")
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                    AnyBtnNextStep(obj, () =>
                    {
                        mask.SetActive(false);
                        level = 1;
                        GameInitLevel(level);
                    });
                }
                else
                {
                    AnyBtnNextStep(obj, () =>
                    {
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 3));
                    });
                }
            });
        }

        void AnyBtnNextStep(GameObject obj, Action callback)
        {
            SpineManager.instance.DoAnimation(obj, "kong", false, () =>
            {
                anyBtns.gameObject.SetActive(false);
                isPlaying = false;
                callback?.Invoke();
            });
        }

        private void GameInit()
        {
            DOTween.KillAll();
            talkIndex = 1;
            level = 0;
            flag = 0;
            isPlaying = false;
            isSuccess = false;
            btnBack.SetActive(true);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
            levelPanel.gameObject.SetActive(true);
            jianZhiSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(jianZhiSpine, jianZhiSpine.name, false);


            for (int i = 0; i < listBtn.Count; i++)
            {
                SpineManager.instance.DoAnimation(nPanel.GetChild(i).gameObject, "kong", false);
                for (int j = 0; j < nPanel.GetChild(i).childCount; j++)
                {
                    SpineManager.instance.DoAnimation(nPanel.GetChild(i).GetChild(j).gameObject, "kong", false);
                }
                SpineManager.instance.DoAnimation(listBtn[i].anNiuEffectSpine, "kong", false);
                listBtn[i].mask.raycastTarget = true;
                listBtn[i].mask.transform.GetComponent<Image>().fillAmount = 1;
            }
            level2Panel.gameObject.SetActive(false);
        }

        float changeNum = 0;
        void GameInitLevel(int level)
        {
            flag = 0;
            isSuccess = false;
            endSpine.SetActive(false);
            btnBack.SetActive(false);
            levelPanel.gameObject.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            level2Panel.gameObject.SetActive(true);
            di.gameObject.SetActive(level == 1);
            scrollbar.value = 0;
            di2.gameObject.SetActive(level > 1);
            for (int i = 0; i < niaoPanel.childCount; i++)
            {
                niaoPanel.GetChild(i).name = i.ToString();
            }
            InitNiaoList(level > 1 ? niao2Panel : niaoPanel);

            bkSpine.gameObject.SetActive(true);
            bkSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(bkSpine.gameObject, bkSpine.name + 1, false);

            for (int i = 0; i < xemTran.childCount; i++)
            {
                xemTran.GetChild(i).GetChild(0).position = xemTran.GetChild(i).position;
                xemTran.GetChild(i).GetChild(0).localScale = Vector3.one;
                if (i <= xemTran.childCount - level)
                {
                    xemTran.GetChild(i).GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
                    SpineManager.instance.DoAnimation(xemTran.GetChild(i).GetChild(0).gameObject, xemTran.name + "2", true);
                }
                else
                {
                    SpineManager.instance.DoAnimation(xemTran.GetChild(i).GetChild(0).gameObject, "kong", false);
                }
            }
            gongjiSpine.GetChild(0).GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(gongjiSpine.GetChild(0).gameObject, "kong", false);
            gongjiSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(gongjiSpine.gameObject, gongjiSpine.name, false);
            teiqiangSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(teiqiangSpine, "kong", false);

        }


        void InitNiaoList(Transform niaoPanel)
        {
            GetRandomArray(randomArray);
            listNiaos.Clear();

            Niao temNiao = new Niao();
            for (int i = 0; i < niaoPanel.childCount; i++)
            {
                if (!niaoPanel.GetChild(i).GetComponent<UIEventListener>())
                    niaoPanel.GetChild(i).gameObject.AddComponent<UIEventListener>();
                niaoPanel.GetChild(i).name = i + "";
                niaoPanel.GetChild(i).gameObject.SetActive(true);
                temNiao.niao = niaoPanel.GetChild(i).gameObject;
                temNiao.niao.transform.GetRectTransform().anchoredPosition = i < niaoPanel.childCount / 2 ? new Vector2(Random.Range(WorldToUgui(leftPos.position).x - 250, WorldToUgui(leftPos.position).x - 100), WorldToUgui(leftPos.position).y - 250 * (i + 1)) : new Vector2(Random.Range(WorldToUgui(rightPos.position).x - 250, WorldToUgui(rightPos.position).x - 100), WorldToUgui(rightPos.position).y - 250 * (i - niaoPanel.childCount / 2 + 1));
                temNiao.niao.transform.rotation = Quaternion.Euler(0, 0, Random.Range(0, 360));
                temNiao.rigidbody2D = niaoPanel.GetChild(i).GetComponent<Rigidbody2D>();
                temNiao.eventDispatcher = niaoPanel.GetChild(i).GetComponent<EventDispatcher>();
                temNiao.img = niaoPanel.GetChild(i).GetChild(0).GetComponent<Image>();
                temNiao.bellSprites = niaoPanel.GetChild(i).GetChild(0).GetComponent<BellSprites>();
                listNiaos.Add(temNiao);
            }

            for (int i = 0; i < listNiaos.Count; i++)
            {
                Move(listNiaos[i].niao);

                listNiaos[i].img.gameObject.SetActive(true);
                listNiaos[i].img.sprite = listNiaos[i].bellSprites.sprites[randomArray[i]];
                listNiaos[i].img.name = randomArray[i].ToString();
                listNiaos[i].eventDispatcher.CollisionEnter2D -= OnCollisionEnter2D;
                listNiaos[i].eventDispatcher.CollisionEnter2D += OnCollisionEnter2D;
                listNiaos[i].niao.GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(listNiaos[i].niao, niaoPanel.name, false);
                listNiaos[i].niao.transform.GetChild(1).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(listNiaos[i].niao.transform.GetChild(1).gameObject, "kong", false);
            }
        }
        void FixedUpdate()
        {
            if (level2Panel.gameObject.activeSelf && di.gameObject.activeSelf)
            {
                scrollbar.value = Mathf.PingPong(changeNum++, 100) / 100;
            }

            if (isDrag)
            {
                Vector3 direction = Vector3.ClampMagnitude(mousePos - niaoPos, 1);
                direction.z = 0f;
                direction = direction.normalized;
                temTran.right = -direction;
            }

        }

        void GameStart()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }));
        }


        void StartPlayGame()
        {
            StepBtnEvent(0);
            btnBack.SetActive(true);
            listBtn[0].mask.transform.GetComponent<Image>().DOFillAmount(0, 1.5f).OnComplete(() => 
            {
                btnBack.SetActive(false);
                listBtn[0].mask.transform.GetComponent<Image>().raycastTarget = false;
            });
        }

        void StepBtnEvent(int index)
        {
            //btnBack.SetActive(true);
            listBtn[index].anNiuEffectSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(listBtn[index].anNiuEffectSpine, anniuPanel.name, true);
            //listBtn[index].mask.transform.DOScaleY(0, 1.5f).OnComplete(() => { btnBack.SetActive(false); });
        }
        //bell说话协程
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "animation");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "animation2");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "animation");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bd.SetActive(false);
                mask.SetActive(false);
                StartPlayGame();
            }

            talkIndex++;
        }


        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                () =>
                {
                    isPlaying = false;
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
                    ChangeClickArea();
                    caidaiSpine.SetActive(false); successSpine.SetActive(false); ac?.Invoke();
                });
                });
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        bool isDrag = false;
        private Vector2 mousePos;
        private Vector2 niaoPos;
        private Transform temTran;
        void Move(GameObject obj)
        {
            UIEventListener.Get(obj).onBeginDrag = beginDragData =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                temTran = obj.transform;
                obj.transform.SetAsLastSibling();
                listNiaos[int.Parse(obj.name)].rigidbody2D.velocity = Vector2.zero;
                SpineManager.instance.DoAnimation(obj, "niao2", false);
            };

            UIEventListener.Get(obj).onDrag = dragData =>
            {
                isDrag = true;
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)curTrans, Input.mousePosition, null, out mousePos);
                RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)curTrans, obj.transform.position, null, out niaoPos);
            };
            UIEventListener.Get(obj).onEndDrag = endDragData =>
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false);
                isDrag = false;
                SpineManager.instance.DoAnimation(obj, "niao", false);
                obj.GetComponent<Rigidbody2D>().AddForce(obj.transform.right * 10f);
            };

        }

        public bool IsRectTransformOverlap(RectTransform rect1, RectTransform rect2)
        {
            float rect1MinX = rect1.position.x - rect1.rect.width / 2;
            float rect1MaxX = rect1.position.x + rect1.rect.width / 2;
            float rect1MinY = rect1.position.y - rect1.rect.height / 2;
            float rect1MaxY = rect1.position.y + rect1.rect.height / 2;

            float rect2MinX = rect2.position.x - rect2.rect.width / 2;
            float rect2MaxX = rect2.position.x + rect2.rect.width / 2;
            float rect2MinY = rect2.position.y - rect2.rect.height / 2;
            float rect2MaxY = rect2.position.y + rect2.rect.height / 2;

            bool xNotOverlap = rect1MaxX <= rect2MinX || rect2MaxX <= rect1MinX;
            bool yNotOverlap = rect1MaxY <= rect2MinY || rect2MaxY <= rect1MinY;

            bool notOverlap = xNotOverlap || yNotOverlap;

            return !notOverlap;
        }


        public bool IsIntersect(Transform tran1, Transform tran2)
        {
            bool isIntersect;
            //另一个矩形的位置大小信息;
            Vector3 moveOrthogonPos = tran2.position;
            Vector3 moveOrthogonScale = tran2.localScale;
            //自己矩形的位置信息
            Vector3 smallOrthogonPos = tran1.position;
            Vector3 smallOrthogonScale = tran1.localScale;
            //分别求出两个矩形X或Z轴的一半之和
            float halfSum_X = (moveOrthogonScale.x * 0.5f) + (smallOrthogonScale.x * 0.5f);
            float halfSum_Z = (moveOrthogonScale.z * 0.5f) + (smallOrthogonScale.z * 0.5f);
            //分别求出两个矩形X或Z轴的距离
            float distance_X = Mathf.Abs(moveOrthogonPos.x - smallOrthogonPos.x);
            float distance_Z = Mathf.Abs(moveOrthogonPos.z - smallOrthogonPos.z);
            //判断X和Z轴的是否小于他们各自的一半之和
            if (distance_X <= halfSum_X && distance_Z <= halfSum_Z)
            {
                isIntersect = true;
                Debug.Log("相交");
            }
            else
            {
                isIntersect = false;
                Debug.Log("不相交");
            }
            return isIntersect;
        }
        public Vector2 WorldToUgui(Vector3 position)
        {
            Vector2 pixlPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(curTrans.GetRectTransform(), position, null, out pixlPoint);
            return pixlPoint;
        }

        //橙汁新增（修复点击按钮区域过大bug）
        void ChangeClickArea()
        {
            int index = 0;
            for (int i = 0; i < anyBtns.childCount; i++)
            {
                if (anyBtns.GetChild(i).gameObject.activeSelf)
                    index++;
            }

            if (index == 1)
                anyBtns.GetComponent<RectTransform>().sizeDelta = new Vector2(240, 240);
            else
                anyBtns.GetComponent<RectTransform>().sizeDelta = new Vector2(680, 240);
        }
    }
}
