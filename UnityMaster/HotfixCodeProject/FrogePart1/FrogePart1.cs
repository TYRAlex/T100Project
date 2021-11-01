using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class FrogePart1
    {
        GameObject curGo;
        private GameObject bell;
        private GameObject pan_1_a12;
        private GameObject pan_2_a12;
        private GameObject pan_3_a34;
        private GameObject pan_4_a34;
        private GameObject pan_4_bj1;
        private GameObject pan_4_bj2;
        private GameObject pan_4_bj3;
        private GameObject pan_4_frogeBtn;
        private GameObject pan_4_a5;
        private GameObject pan_4_a2zhezhao;
        private GameObject pan_4_imgBtn;
        private GameObject pan_4_rightBtn;
        private GameObject pan_4_img;
        private GameObject[] panels;
        private GameObject[] pan_4_bjs;
        private string[] pan_4_spines;

        private ScrollPage scroll;

        private int talkIndex;
        private int currentPage;
        private int currentFroge;

        private MonoBehaviour mono;

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            GameObject con = curTrans.Find("ScrollView/Viewport/Content").gameObject;
            panels = new GameObject[con.transform.childCount];
            for (int i = 0; i < panels.Length; i++)
            {
                panels[i] = con.transform.GetChild(i).gameObject;
            }
            Debug.Log(".......................SS.." + panels.Length);
            scroll = curTrans.Find("ScrollView").GetComponent<ScrollPage>();
            bell = curTrans.Find("Bell").gameObject;
            pan_1_a12 = panels[0].transform.Find("A12").gameObject;
            pan_2_a12 = panels[1].transform.Find("A12").gameObject;
            pan_3_a34 = panels[2].transform.Find("A34").gameObject;
            pan_4_a34 = panels[3].transform.Find("A34").gameObject;
            pan_4_bj1 = panels[3].transform.Find("bj1").gameObject;
            pan_4_bj2 = panels[3].transform.Find("bj2").gameObject;
            pan_4_bj3 = panels[3].transform.Find("bj3").gameObject;
            pan_4_rightBtn = panels[3].transform.Find("RightBtn").gameObject;
            pan_4_frogeBtn = panels[3].transform.Find("FrogeBtn").gameObject;
            pan_4_a5 = panels[3].transform.Find("A5").gameObject;
            pan_4_a2zhezhao = panels[3].transform.Find("A2zhezhao").gameObject;
            pan_4_imgBtn = panels[3].transform.Find("ImgBtn").gameObject;
            pan_4_img = panels[3].transform.Find("Image").gameObject;
            GameObject[] bjArray = { pan_4_bj1, pan_4_bj2, pan_4_bj3 };
            pan_4_bjs = bjArray;
            string[] spinesArray = { "d31", "d11", "d21", "d1", "d2", "d3" };
            pan_4_spines = spinesArray;

            mono = curGo.GetComponent<MonoBehaviour>();
            GameInit();
        }

        /// <summary>
        /// 初始化
        /// </summary>
        private void GameInit()
        {
            talkIndex = 1;
            currentPage = 1;
            currentFroge = 0;
            scroll.OnDragSlide = () => {
                DoDragSlide();
            };
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            Util.AddBtnClick(pan_4_frogeBtn, DoFrogeBtn);
            Util.AddBtnClick(pan_4_imgBtn, DoImgBtn);
            Util.AddBtnClick(pan_4_rightBtn, DoRightBtn);
            pan_1_a12.SetActive(true);
            for (int i = 0; i < 4; i++)
            {
                InitPage(i);
            }
            Debug.Log("初始化成功");
            GameStart();
        }

        /// <summary>
        /// 游戏开始
        /// </summary>
        private void GameStart()
        {
            Debug.Log("游戏开始");
            bell.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => {
                SpineManager.instance.DoAnimation(pan_1_a12, "a1", false);
            }, () => {
            }, 0));
        }

        /// <summary>
        /// 语音键点击方法
        /// </summary>
        private void TalkClick()
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bell.SetActive(true);
                pan_4_imgBtn.SetActive(false);
                pan_4_a2zhezhao.SetActive(true);
                SpineManager.instance.DoAnimation(pan_4_a5, "e1d", true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 15, () =>
                {
                    SpineManager.instance.DoAnimation(pan_4_a2zhezhao, "1", false);
                }, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 16, () =>
                    {
                        SpineManager.instance.DoAnimation(pan_4_a2zhezhao, "2", false);
                    }, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                }, 0, 0));
            } else if (talkIndex == 2)
            {
                float len = SpineManager.instance.DoAnimation(pan_4_a2zhezhao, "3", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 17, () => { }, () => { }, len));
            }
            talkIndex++;
        }

        /// <summary>
        /// 各页面初始化
        /// </summary>
        /// <param name="pageIndex"></param>
        private void InitPage(int pageIndex)
        {
            Debug.Log("初始化页面(页面从0开始)：" + pageIndex);
            if (pageIndex == 0)
            {
                bell.SetActive(false);
            } else if (pageIndex == 1)
            {
                mono.StartCoroutine(Page_2_Coroutine());
            } else if (pageIndex == 2)
            {
                SpineManager.instance.DoAnimation(pan_3_a34, "c1", false);
            } else if (pageIndex == 3)
            {
                talkIndex = 1;
                currentFroge = 0;
                pan_4_bj1.SetActive(true);
                pan_4_bj2.SetActive(false);
                pan_4_bj3.SetActive(false);
                pan_4_frogeBtn.SetActive(false);
                pan_4_a34.SetActive(true);
                pan_4_img.SetActive(false);
                pan_4_imgBtn.SetActive(false);
                pan_4_a2zhezhao.SetActive(false);
                pan_4_rightBtn.SetActive(false);
                SpineManager.instance.DoAnimation(pan_4_a5, "e1d", true);
                SpineManager.instance.DoAnimation(pan_4_a34, "d1", true);
            }
        }

        IEnumerator Page_2_Coroutine()
        {
            SpineManager.instance.DoAnimation(pan_2_a12, "b21d", false);
            yield return new WaitForSeconds(1);
            pan_2_a12.SetActive(false);
        }

        /// <summary>
        /// 各页面的开始执行操作
        /// </summary>
        /// <param name="pageIndex"></param>
        private void PageStart(int pageIndex)
        {
            Debug.Log("正在执行的页面操作：" + pageIndex);
            if (pageIndex == 0)
            {
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, () => { }, () => { }, 0));
                SpineManager.instance.DoAnimation(pan_1_a12, "a1d", false);
            }
            else if (pageIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, () => { }, () => {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => {
                        pan_2_a12.SetActive(true);
                        SpineManager.instance.DoAnimation(pan_2_a12, "b21", false);
                    }, () => {
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => {
                            SpineManager.instance.DoAnimation(pan_2_a12, "b22", false);
                        }, () => {
                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => {
                                SpineManager.instance.DoAnimation(pan_2_a12, "b23", false);
                            }, () => {
                                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5, () => {
                                    SpineManager.instance.DoAnimation(pan_2_a12, "b24", false);
                                }, () => {
                                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => {
                                        SpineManager.instance.DoAnimation(pan_2_a12, "b25", false);
                                    }, () => {
                                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, () => {
                                            SpineManager.instance.DoAnimation(pan_2_a12, "b26", false);
                                        }, () => {
                                            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, () => { }, () => { }));
                                        }));
                                    }));
                                }));
                            }));
                        }));
                    }));
                }));
            } else if (pageIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, () => { }, () => {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () => {
                        SpineManager.instance.DoAnimation(pan_3_a34, "c2", false);
                    }, () => { }));
                }));
            }
            else if (pageIndex == 3)
            {
                bell.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 11, () => { }, () =>
                {
                    bell.SetActive(false);
                    pan_4_rightBtn.SetActive(true);
                    pan_4_frogeBtn.SetActive(true);
                }));
            }
            else
            {
                Debug.Log("错误下标：" + pageIndex);
            }
        }

        /// <summary>
        /// 拉动成功后的回调方法
        /// </summary>
        private void DoDragSlide()
        {
            Debug.Log(".......................>>>>>>>>>" + currentPage);
            SpineManager.instance.DoAnimation(this.bell, "DAIJI");
            bell.SetActive(false);
            mono.StopAllCoroutines();//停止该类中的所有协程
            SoundManager.instance.StopAudio();//中止正在播放的所有语音
            InitPage(currentPage);
            currentPage = scroll.GetCurrentPage();
            PageStart(currentPage);
        }
    
        /// <summary>
        /// 青蛙按钮点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void DoFrogeBtn(GameObject obj)
        {
            Debug.Log("青蛙按钮点击");
            pan_4_bj1.SetActive(false);
            pan_4_bj2.SetActive(false);
            pan_4_bj3.SetActive(false);
            pan_4_rightBtn.SetActive(false);
            pan_4_a34.SetActive(false);
            pan_4_frogeBtn.SetActive(false);
            //SpineManager.instance.DoAnimation(pan_4_a5, "e2d");
            pan_4_a5.SetActive(false);
            pan_4_img.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 12, () => { }, () =>
            {
                pan_4_a5.SetActive(true);
                SpineManager.instance.DoAnimation(pan_4_a5, "e1", false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, () => {
                    pan_4_img.SetActive(false);
                }, () =>
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 14, () => {
                        SpineManager.instance.DoAnimation(pan_4_a5, "e2", false);
                    }, () =>
                    {
                        pan_4_imgBtn.SetActive(true);
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                }));
            }));
        }

        /// <summary>
        /// 细胞图片按钮点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void DoImgBtn(GameObject obj)
        {
            Debug.Log("细胞图片按钮点击");
            SpineManager.instance.DoAnimation(pan_4_a5, "e1d", true);
            SoundManager.instance.ShowVoiceBtn(false);
            pan_4_imgBtn.SetActive(false);
            pan_4_bj1.SetActive(true);
            pan_4_a34.SetActive(true);
            pan_4_frogeBtn.SetActive(true);
        }

        /// <summary>
        /// 青蛙切换按钮点击事件
        /// </summary>
        /// <param name="obj"></param>
        private void DoRightBtn(GameObject obj)
        {
            Debug.Log("青蛙切换按钮点击");
            pan_4_rightBtn.SetActive(false);
            pan_4_frogeBtn.SetActive(false);
            currentFroge++;
            if (currentFroge == 3) currentFroge = 0;
            for (int i = 0; i < pan_4_bjs.Length; i++)
            {
                if (i != currentFroge) pan_4_bjs[i].SetActive(false);
                else pan_4_bjs[i].SetActive(true);
            }
            mono.StartCoroutine(DoRightBtnCoroutine());
        }

        IEnumerator DoRightBtnCoroutine()
        {
            float len = SpineManager.instance.DoAnimation(pan_4_a34, pan_4_spines[currentFroge], false);
            yield return new WaitForSeconds(len);
            SpineManager.instance.DoAnimation(pan_4_a34, pan_4_spines[currentFroge + 3], true);
            yield return new WaitForSeconds(0.5f);
            pan_4_rightBtn.SetActive(true);
            pan_4_frogeBtn.SetActive(true);
        }

        /// <summary>
        /// bell语音协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <param name="len2"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0, float len2 = 0)
        {
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            float clipLength = SoundManager.instance.PlayClip(type, clipIndex, false);
            if (method_1 != null)
            {
                yield return new WaitForSeconds(len);
                method_1();
            }
            yield return new WaitForSeconds(clipLength - len);
            SpineManager.instance.DoAnimation(bell, "DAIJI");
            if (method_2 != null)
            {
                yield return new WaitForSeconds(len2);
                method_2();
            }
        }
    }
}
