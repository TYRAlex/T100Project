
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

namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
        next,
    }
    public class TD5631Part5
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



        private Transform gamePanel;
        private Transform panel;

        private GameObject xiaObj;
        private ILDroper[] iLDropers;
        private Transform startPos;
        private Transform endPos;
        private GameObject jdObj;

        private Transform panel2;
        private GameObject jianYu;
        private Transform tongs;

        private Transform lajiPanel;
        private Transform lajiPos;
        private ILDrager[] iLDragers;

        private Transform panel3;
        private GameObject haiTun;
        private Transform yhljPanel;
        private Transform yhljPos;

        private ILDrager[] iLDrager2s;
        private GameObject chaoShenBo;
        private GameObject lajiTong;
        private GameObject XEM;

        private Transform effectPanel;
        private GameObject guang;

        private GameObject spineMask;
        //胜利动画名字
        private string tz;

        private int flag = 0;
        private bool isPlaying = false;
        private int curLevel = 0;
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
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
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

            gamePanel = curTrans.Find("gamePanel");
            panel = curTrans.Find("gamePanel/panel");


            xiaObj = panel.Find("xia").gameObject;
            iLDropers = xiaObj.GetComponentsInChildren<ILDroper>();
            for (int i = 0; i < iLDropers.Length; i++)
            {
                iLDropers[i].SetDropCallBack(OnAfter);
            }
            startPos = panel.Find("startPos");
            endPos = panel.Find("endPos");
            jdObj = panel.Find("jd").gameObject;
            jdObj.GetComponent<ILDrager>().SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);


            panel2 = curTrans.Find("gamePanel/panel2");

            jianYu = panel2.Find("jianyu").gameObject;
            tongs = panel2.Find("tongs");
            ILDroper[] iLDroper2s = tongs.GetComponentsInParent<ILDroper>(true);
            for (int i = 0; i < iLDroper2s.Length; i++)
            {
                iLDroper2s[i].SetDropCallBack(OnAfter2);
            }
            lajiPanel = panel2.Find("lajiPanel");
            iLDragers = lajiPanel.GetComponentsInChildren<ILDrager>(true);

            lajiPos = panel2.Find("lajiPos");
            panel3 = curTrans.Find("gamePanel/panel3");

            haiTun = panel3.Find("haitun").gameObject;
            yhljPanel = panel3.Find("yhljPanel");
            iLDrager2s = yhljPanel.GetComponentsInChildren<ILDrager>(true);

            yhljPos = panel3.Find("yhljPos");
            chaoShenBo = panel3.Find("chaoshengbo").gameObject;
            lajiTong = panel3.Find("lajitong").gameObject;
            lajiTong.GetComponent<ILDroper>().SetDropCallBack(OnAfter3);

            XEM = curTrans.Find("gamePanel/XEM").gameObject;

            effectPanel = curTrans.Find("effectPanel");
            guang = effectPanel.GetChild(0).gameObject;
            tz = "3-5-z";
            spineMask = curTrans.Find("spineMask").gameObject;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            curLevel = 0;
            GameInit();
            //GameStart();
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
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 3, true);
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "next")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        flag = 0;
                        curLevel++;
                        bd.SetActive(true);
                        Debug.Log("curLevel:"+ curLevel);
                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, curLevel + 2, () => { GameInit(false); }, () =>
                        {
                            bd.SetActive(false);
                            mask.SetActive(false);
                            if (curLevel == 2)
                            {
                                Empty4Raycast[] e4rc = yhljPanel.GetComponentsInChildren<Empty4Raycast>(true);
                                for (int i = 0; i < e4rc.Length; i++)
                                {
                                    e4rc[i].raycastTarget = false;
                                }
                                SpineManager.instance.DoAnimation(haiTun, haiTun.name + "2", false,
                                     () =>
                                     {
                                         for (int i = 0; i < yhljPanel.childCount; i++)
                                         {
                                             SpineManager.instance.DoAnimation(yhljPanel.GetChild(i).GetChild(0).gameObject, yhljPanel.GetChild(i).GetChild(0).name + 1, true);
                                         }
                                         SpineManager.instance.DoAnimation(haiTun, haiTun.name + "3", true);
                                         SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                                         SpineManager.instance.DoAnimation(lajiTong, lajiTong.name, false,
                                                 () =>
                                                 {
                                                     SpineManager.instance.DoAnimation(lajiTong, lajiTong.name + 2, false,
                                                         () =>
                                                         {
                                                             for (int i = 0; i < e4rc.Length; i++)
                                                             {
                                                                 e4rc[i].raycastTarget = true;
                                                             }
                                                         });
                                                 });
                                     });
                            }
                        }));

                    });
                }
                else if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        GameStart();

                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        curLevel = 0;
                        mask.SetActive(false);
                        GameInit(false);
                        InitXia();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        switchBGM(); anyBtns.gameObject.SetActive(false);
                        isPlaying = false;
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 5));
                    });
                }
            });
        }


        private void GameInit(bool isInit = true)
        {
            spineMask.SetActive(false);
            for (int i = 0; i < iLDropers.Length; i++)
            {
                iLDropers[i].isActived = true;
            }
            ILDrager[] temObjs = iLDragers.ToArray();
            for (int i = 0; i < temObjs.Length; i++)
            {

                int tem = int.Parse(temObjs[i].name);
                ILDrager temILDrager;
                temILDrager = iLDragers[tem];
                iLDragers[tem] = temObjs[i];
            }
            for (int i = 0; i < iLDragers.Length; i++)
            {
                iLDragers[i].SetDragCallback(OnBeginDrag2, OnDrag2, OnEndDrag2);
            }

            ILDrager[] temObj2s = iLDrager2s.ToArray();
            for (int i = 0; i < temObj2s.Length; i++)
            {

                int tem = int.Parse(temObj2s[i].name);
                ILDrager temILDrager;
                temILDrager = iLDrager2s[tem];
                iLDrager2s[tem] = temObj2s[i];
            }
            for (int i = 0; i < iLDrager2s.Length; i++)
            {
                iLDrager2s[i].SetDragCallback(OnBeginDrag3, OnDrag3, OnEndDrag3);
            }

            talkIndex = 1;
            flag = 0;
            isPlaying = false;
            panel.gameObject.SetActive(curLevel == 0);
            panel2.gameObject.SetActive(curLevel == 1);
            panel3.gameObject.SetActive(curLevel == 2);
            if (isInit)
            {
                SkeletonGraphic sg;
                for (int i = 0; i < gamePanel.childCount; i++)
                {
                    if (i != panel.GetSiblingIndex() || i != panel2.GetSiblingIndex() || i != panel3.GetSiblingIndex())
                    {
                        sg = gamePanel.GetChild(i).GetComponent<SkeletonGraphic>();
                        if (sg)
                        {
                            sg.Initialize(true);
                        }
                        SpineManager.instance.DoAnimation(gamePanel.GetChild(i).gameObject, gamePanel.GetChild(i).name, true);
                    }
                }
            }
          
            xiaObj.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(xiaObj, xiaObj.name, true);
            for (int i = 0, len = xiaObj.transform.childCount; i < len; i++)
            {
                SpineManager.instance.DoAnimation(xiaObj.transform.GetChild(i).gameObject, "kong", false);
                //xiaObj.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
            }
            jdObj.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(jdObj, jdObj.name, true);
            jdObj.transform.position = startPos.position;
            if (isInit)
            {
                InitXia();
            }


            XEM.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(XEM, XEM.name, true);


            for (int i = 0; i < iLDragers.Length; i++)
            {
                iLDragers[i].transform.position = lajiPos.GetChild(i).position;
                SpineManager.instance.DoAnimation(iLDragers[i].transform.GetChild(0).gameObject, iLDragers[i].transform.GetChild(0).name, true);
            }
            jianYu.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(jianYu, jianYu.name, true);

            for (int i = 0; i < tongs.childCount; i++)
            {
                SpineManager.instance.DoAnimation(tongs.GetChild(i).gameObject, tongs.GetChild(i).name, false);
            }
            haiTun.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(haiTun, haiTun.name, true);

            SpineManager.instance.DoAnimation(chaoShenBo, "kong", false);
            SpineManager.instance.DoAnimation(lajiTong, "kong", false);
            for (int i = 0; i < iLDrager2s.Length; i++)
            {
                iLDrager2s[i].transform.position = yhljPos.GetChild(i).position;
                SpineManager.instance.DoAnimation(iLDrager2s[i].transform.GetChild(0).gameObject, "kong", false);
            }
            SpineManager.instance.DoAnimation(guang, "kong", false);
        }

        void InitXia() 
        {
            bd.SetActive(true);
            mask.SetActive(true);
            anyBtns.gameObject.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () =>
            {
                bd.SetActive(false);
                mask.SetActive(false);
                anyBtns.gameObject.SetActive(true);
                PlayXiaSpine();

            }));
        }

        void GameStart()
        {
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));

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
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
            }

            if (talkIndex == 2)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () =>
                {

                    bd.SetActive(false);
                    mask.SetActive(false);
                    PlayXiaSpine();

                }));
            }

            talkIndex++;
        }

        void PlayXiaSpine()
        {

            for (int i = 0, len = xiaObj.transform.childCount; i < len; i++)
            {
                xiaObj.transform.GetChild(i).GetComponent<SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(xiaObj.transform.GetChild(i).gameObject, xiaObj.transform.GetChild(i).name, true);
            }
            SpineManager.instance.DoAnimation(xiaObj, xiaObj.name + "A0", true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
            jdObj.transform.DOMove(endPos.position, 1).SetEase(Ease.InCirc);
        }
        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            mask.SetActive(true);
            successSpine.SetActive(true);
            caidaiSpine.SetActive(true);
            anyBtns.gameObject.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            successSpine.GetComponent<SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(successSpine, tz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, tz + "2", false,
                () =>
                {
                    anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                    anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                    anyBtns.gameObject.SetActive(true);
                    anyBtns.GetChild(0).gameObject.SetActive(true);
                    anyBtns.GetChild(1).gameObject.SetActive(true);
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
        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }
        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                    iLDropers[index].isActived = false;
                }
                SpineManager.instance.DoAnimation(xiaObj.transform.GetChild(index).gameObject, iLDropers[index].gameObject.name, false);
            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            jdObj.transform.position = Input.mousePosition;
        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            jdObj.transform.position = Input.mousePosition;
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            jdObj.transform.DOMove(endPos.position, 1).SetEase(Ease.InFlash);
            //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                if (flag >= (Mathf.Pow(2, xiaObj.transform.childCount) - 1))
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONBGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SpineManager.instance.DoAnimation(xiaObj, xiaObj.name + "A0", false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                            SpineManager.instance.DoAnimation(guang, guang.name, true);
                            SpineManager.instance.DoAnimation(xiaObj, xiaObj.name + "B", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(xiaObj, xiaObj.name + "C", true);
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                                    SpineManager.instance.DoAnimation(XEM, XEM.name + 2, false,
                                              () =>
                                              {
                                                  SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                                  SpineManager.instance.DoAnimation(xiaObj, xiaObj.name + "D", false,
                                             () =>
                                             {
                                                 mask.SetActive(true);
                                                 anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                                 anyBtns.gameObject.SetActive(true);
                                                 anyBtns.GetChild(0).gameObject.SetActive(true);
                                             });
                                              });
                                });
                        });

                }

            }
        }



        private bool OnAfter2(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
            }
            return dragType == dropType;
        }

        private void OnBeginDrag2(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            iLDragers[index].transform.SetAsLastSibling();
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnDrag2(Vector3 pos, int type, int index)
        {
            iLDragers[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag2(Vector3 pos, int type, int index, bool isMatch)
        {
            spineMask.SetActive(true);

            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                spineMask.SetActive(false);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                SpineManager.instance.DoAnimation(iLDragers[index].transform.GetChild(0).gameObject, "kong", false);

                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                }

                if (flag >= (Mathf.Pow(2, lajiPanel.transform.childCount) - 1))
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONBGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(guang, guang.name, true);
                    SpineManager.instance.DoAnimation(jianYu, jianYu.name + 2, true);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                    SpineManager.instance.DoAnimation(XEM, XEM.name + 2, false,
                               () =>
                               {
                                   SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                   SpineManager.instance.DoAnimation(jianYu, jianYu.name + 3, false,
                               () =>
                               {
                                   mask.SetActive(true);
                                   anyBtns.GetChild(0).name = getBtnName(BtnEnum.next, 0);
                                   anyBtns.gameObject.SetActive(true);
                                   anyBtns.GetChild(0).gameObject.SetActive(true);
                               });
                               });
                }
                iLDragers[index].SetDragCallback(null, null, null);
            }
            iLDragers[index].transform.DOMove(lajiPos.GetChild(index).position, 1);
        }

        private bool OnAfter3(int dragType, int index, int dropType)
        {
            if (dragType == dropType)
            {
            }
            return dragType == dropType;
        }

        private void OnBeginDrag3(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
            SpineManager.instance.DoAnimation(iLDrager2s[index].transform.GetChild(0).gameObject, iLDrager2s[index].transform.GetChild(0).name + 2, false);
            iLDrager2s[index].transform.SetAsLastSibling();
            iLDrager2s[index].transform.position = Input.mousePosition;
        }

        private void OnDrag3(Vector3 pos, int type, int index)
        {
            iLDrager2s[index].transform.position = Input.mousePosition;
        }

        private void OnEndDrag3(Vector3 pos, int type, int index, bool isMatch)
        {
            spineMask.SetActive(true);
            if (!isMatch)
            {
                yhljPos.gameObject.SetActive(true);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                SpineManager.instance.DoAnimation(iLDrager2s[index].transform.GetChild(0).gameObject, iLDrager2s[index].transform.GetChild(0).name + 1, true);
                spineMask.SetActive(false);
            }
            else
            {

                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), null, () => { spineMask.SetActive(false); }));
                SpineManager.instance.DoAnimation(iLDrager2s[index].transform.GetChild(0).gameObject, "kong", false);
                if ((flag & (1 << index)) <= 0)
                {
                    flag += (1 << index);
                }
                if (flag >= Mathf.Pow(2, yhljPanel.transform.childCount) - 1)
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONBGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
                    SpineManager.instance.DoAnimation(lajiTong, "kong", false);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                    SpineManager.instance.DoAnimation(guang, guang.name, true);
                    SpineManager.instance.DoAnimation(haiTun, haiTun.name + 4, false,
                        () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 6);
                            SpineManager.instance.DoAnimation(chaoShenBo, chaoShenBo.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(chaoShenBo, "kong", false);
                                });

                            SpineManager.instance.DoAnimation(haiTun, haiTun.name + 5, true);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                            SpineManager.instance.DoAnimation(XEM, XEM.name + 2, false,
                                       () =>
                                       {
                                           SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                           SpineManager.instance.DoAnimation(haiTun, haiTun.name + 6, false,
                                        () =>
                                        {
                                            SpineManager.instance.DoAnimation(haiTun, "kong", false,
                                          () =>
                                          {
                                              flag = 0;
                                              playSuccessSpine();
                                          });
                                        });
                                       });
                        });
                }
                iLDrager2s[index].SetDragCallback(null, null, null);
            }
            iLDrager2s[index].transform.DOMove(yhljPos.GetChild(index).position, 1);
        }

    }
}
