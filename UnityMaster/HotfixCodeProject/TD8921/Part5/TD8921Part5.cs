using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using RandomNew = System.Random;
namespace ILFramework.HotClass
{
    public enum BtnEnum
    {
        bf,
        fh,
        ok,
    }
    public class TD8921Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject Bg;

        private GameObject bossN;
        private GameObject bossD;

        private BellSprites bellTextures;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;


        private Transform scorePanel;

        private Transform homePos;

        private Transform spineShow;

        private Transform birdsPos;
        private Transform birds;
        private Transform yms;

        private Transform ymDragers;

        private GameObject spineMask;
        private ILDrager[] dragers;

        private Transform homeSpine;

        private Transform ymSpine;
        private GameObject boss;
        private Transform bossPos;


        private Transform zq;
        //胜利动画名字
        private string sz;
        bool isPlaying = false;

        private int totalBlood = 4;
        private int randomNum = 0;
        private List<int> randomList;
        private List<int> temShowList;

        ILDrager temIL = null;
        Queue temQueue = null;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

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
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);


            sz = "6-12-z";

            bossN = curTrans.Find("Bg/bossN").gameObject;
            bossN.SetActive(true);
            bossD = curTrans.Find("Bg/bossD").gameObject;
            bossD.SetActive(false);

            scorePanel = curTrans.Find("scorePanel");

            homePos = curTrans.Find("homePos");

            for (int i = 0; i < homePos.childCount; i++)
            {
                homePos.GetChild(i).GetChild(0).GetComponent<ILDroper>().SetDropCallBack(OnAfter);
            }
            spineShow = curTrans.Find("spineShow");
            birdsPos = curTrans.Find("birdsPos");
            birds = curTrans.Find("birds");
            yms = curTrans.Find("yms");
            ymDragers = curTrans.Find("ymDragers");

            dragers = ymDragers.GetComponentsInChildren<ILDrager>(true);
            for (int i = 0; i < dragers.Length; i++)
            {
                dragers[i].SetDragCallback(OnBeginDrag, OnDrag, OnEndDrag);
            }

            spineMask = curTrans.Find("spineMask").gameObject;
            spineMask.SetActive(false);
            randomList = new List<int>();
            temShowList = new List<int>();
            temQueue = new Queue();
            homeSpine = curTrans.Find("homeSpine");
            ymSpine = curTrans.Find("ymSpine");

            boss = curTrans.Find("boss").gameObject;
            bossPos = curTrans.Find("boss/bossPos");
            zq = curTrans.Find("zq");

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit(true,false);
            //GameStart();
        }

        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <param name="index"></param>
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
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(true,true); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { switchBGM(); anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 4)); });
                }

            });
        }

        private void GameInit(bool isInit, bool isTip)
        {
            talkIndex = 1;
            totalBlood = scorePanel.childCount;
            randomNum = 0;
            randomList.Clear();
            temQueue.Clear();
            temShowList.Clear();
            for (int i = 0; i < totalBlood; i++)
            {
                randomList.Add(i);
                scorePanel.GetChild(i).gameObject.SetActive(true);
            }

            for (int i = 0; i < homePos.childCount; i++)
            {
                for (int j = 1; j < homePos.GetChild(i).childCount; j++)
                {
                    SpineManager.instance.DoAnimation(homePos.GetChild(i).GetChild(j).gameObject, "kong", false);
                }
            }

            for (int i = 0; i < birdsPos.childCount; i++)
            {
                birds.GetChild(i).position = birdsPos.GetChild(i).position;
            }

            SpineManager.instance.DoAnimation(birds.GetChild(0).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(birds.GetChild(0).gameObject, birds.GetChild(0).name + "2", true); });
            SpineManager.instance.DoAnimation(birds.GetChild(1).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(birds.GetChild(1).gameObject, birds.GetChild(1).name + "2", true); });
            SpineManager.instance.DoAnimation(birds.GetChild(2).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(birds.GetChild(2).gameObject, birds.GetChild(2).name + "2", true); });
            SpineManager.instance.DoAnimation(birds.GetChild(3).gameObject, "kong", false, () => { SpineManager.instance.DoAnimation(birds.GetChild(3).gameObject, birds.GetChild(3).name + "2", true); });

            for (int i = 0; i < yms.childCount; i++)
            {
                for (int j = 0; j < yms.GetChild(i).childCount; j++)
                {
                    SpineManager.instance.DoAnimation(yms.GetChild(i).GetChild(j).gameObject, yms.GetChild(i).name + (j + 1), false);
                }
            }

            for (int i = 0; i < ymDragers.childCount; i++)
            {
                for (int j = 0; j < ymDragers.GetChild(i).childCount; j++)
                {
                    SpineManager.instance.DoAnimation(ymDragers.GetChild(i).GetChild(j).gameObject, ymDragers.GetChild(i).name + (j + 1), false);
                }
            }

            for (int i = 0; i < homeSpine.childCount; i++)
            {
                SpineManager.instance.DoAnimation(homeSpine.GetChild(i).gameObject, "kong", false);
                SpineManager.instance.DoAnimation(ymSpine.GetChild(i).gameObject, "kong", false);
            }
            SpineManager.instance.DoAnimation(boss, "1", true);

            SpineManager.instance.DoAnimation(zq.gameObject, "kong", false);

            RandomYm(true,isTip);
        }


        private void RandomYm(bool isInit,bool isTip)
        {
            if (!isInit)
            {
                randomList.RemoveAt(randomNum);
            }
            if (randomList.Count > 0)
            {
                randomNum = Random.Range(0, randomList.Count);
                for (int i = 0; i < yms.childCount; i++)
                {
                    if (i != randomList[randomNum])
                    {
                        yms.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        yms.GetChild(i).gameObject.SetActive(true);
                        for (int j = 0; j < yms.GetChild(i).childCount; j++)
                        {
                            yms.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
                for (int i = 0; i < ymDragers.childCount; i++)
                {
                    if (i != randomList[randomNum])
                    {
                        ymDragers.GetChild(i).gameObject.SetActive(false);
                    }
                    else
                    {
                        ymDragers.GetChild(i).gameObject.SetActive(true);
                        for (int j = 0; j < ymDragers.GetChild(i).childCount; j++)
                        {
                            ymDragers.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
                if (isTip)
                {
                    mono.StartCoroutine(waitTime());
                }
               
                temShowList.Clear();
                for (int i = 0; i < ymDragers.GetChild(randomList[randomNum]).childCount; i++)
                {
                    temShowList.Add(i);
                }
                int[] temaArr = temShowList.ToArray();
                RandomArr(temaArr);
                temShowList.Clear();
                for (int i = 0, len = temaArr.Length; i < len; i++)
                {
                    temShowList.Add(temaArr[i]);
                    temQueue.Enqueue(i);
                    ILDrager temIlDrager = ymDragers.GetChild(randomList[randomNum]).GetChild(i).GetComponent<ILDrager>();
                    SpineManager.instance.DoAnimation(ymDragers.GetChild(randomList[randomNum]).GetChild(i).gameObject, ymDragers.GetChild(randomList[randomNum]).name + (temaArr[i] + 1), false);
                    SpineManager.instance.DoAnimation(yms.GetChild(randomList[randomNum]).GetChild(i).gameObject, yms.GetChild(randomList[randomNum]).name + (temaArr[i] + 1), false);
                }
            }
            //spineMask.SetActive(false);
            isPlaying = false;
        }
        IEnumerator waitTime()
        {
            spineMask.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, true);
            SpineManager.instance.DoAnimation(homeSpine.GetChild(randomList[randomNum]).gameObject, homeSpine.GetChild(randomList[randomNum]).name, true);
            yield return new WaitForSeconds(2f);
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            SpineManager.instance.DoAnimation(homeSpine.GetChild(randomList[randomNum]).gameObject, "kong", false,()=> { spineMask.SetActive(false); });
           
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bd.SetActive(true);
            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }
        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = bd;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, null, () => { mask.SetActive(false); bd.SetActive(false); mono.StartCoroutine(waitTime()); }));
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
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
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

        //正脸环节专用bgm
        private void switchBGM()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
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
            }
            return dragType == dropType;
        }


        private void OnBeginDrag(Vector3 pos, int type, int index)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONSOUND);
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            temIL = ymDragers.GetChild(type).GetChild(index).GetComponent<ILDrager>();
            yms.GetChild(type).GetChild(index).gameObject.SetActive(false);
            temIL.canMove = false;
            if (temShowList[index] != (int)temQueue.Peek())
            {
                if (temShowList[index] < (int)temQueue.Peek())
                {
                    ymDragers.GetChild(type).GetChild(index).gameObject.SetActive(false);
                    return;
                }
                spineMask.SetActive(true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3);
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2, null, () => { spineMask.SetActive(false); }));
                SpineManager.instance.DoAnimation(ymDragers.GetChild(type).GetChild(index).gameObject, yms.GetChild(type).name + (temShowList[index] + ymDragers.GetChild(type).childCount + 1), false, () =>
                {
                    ymDragers.GetChild(type).GetChild(index).position = yms.GetChild(type).GetChild(index).position;
                });
            }
            else
            {
                SpineManager.instance.DoAnimation(ymDragers.GetChild(type).GetChild(index).gameObject, yms.GetChild(type).name + (temShowList[index] + 1), false, () =>
                {
                    temIL.canMove = true;
                    ymDragers.GetChild(type).GetChild(index).position = yms.GetChild(type).GetChild(index).position;
                });
            }

            //ymDragers.GetChild(type).GetChild(index).position = Input.mousePosition;

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {
            if (temIL.canMove)
            {
                ymDragers.GetChild(type).GetChild(index).position = Input.mousePosition;
            }
            else
            {
                ymDragers.GetChild(type).GetChild(index).position = yms.GetChild(type).GetChild(index).position;
            }
        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
          
            yms.GetChild(type).GetChild(index).gameObject.SetActive(temIL.canMove);
            //if (temShowList[index] < (int)temQueue.Peek())
            //{
            //    ymDragers.GetChild(type).GetChild(index).gameObject.SetActive(false);
            //}
            if (!temIL.canMove)
                return;
            SpineManager.instance.DoAnimation(ymDragers.GetChild(type).GetChild(index).gameObject, ymDragers.GetChild(type).name + (temShowList[index] + ymDragers.GetChild(type).childCount * 2 + 1), false, () =>
            {
                ymDragers.GetChild(type).GetChild(index).position = yms.GetChild(type).GetChild(index).position;
            });

            if (!isMatch)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
                SpineManager.instance.DoAnimation(yms.GetChild(type).GetChild(index).gameObject, yms.GetChild(type).name + (temShowList[index] + 1), false, () => { isPlaying = false;  });
            }
            else
            {
                spineMask.SetActive(true);
                yms.GetChild(type).GetChild(index).gameObject.SetActive(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                SpineManager.instance.DoAnimation(homePos.GetChild(type).GetChild(index+1).gameObject, homePos.GetChild(type).GetChild(temShowList[index]+1).name, false,
                    () =>
                    {
                        temQueue.Dequeue();
                        if (temQueue.Count <= 0)
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
                            SpineManager.instance.DoAnimation(ymSpine.GetChild(randomList[randomNum]).gameObject, ymSpine.GetChild(randomList[randomNum]).name, false,

                                () =>
                                {
                                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                                    SpineManager.instance.DoAnimation(birds.GetChild(randomList[randomNum]).gameObject, birds.GetChild(randomList[randomNum]).name + "1", true);
                                    birds.GetChild(randomList[randomNum]).DOMove(bossPos.position, 1f).SetEase(Ease.InQuart).OnComplete(() =>
                                    {
                                        SpineManager.instance.DoAnimation(birds.GetChild(randomList[randomNum]).gameObject, "kong", false);
                                        totalBlood--;
                                        //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                                        mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 3));
                                        if (totalBlood <= 0)
                                        {
                                            SpineManager.instance.DoAnimation(boss, "3", false, () =>
                                            {
                                                bossN.SetActive(false);
                                                bossD.SetActive(true);
                                                SpineManager.instance.DoAnimation(boss, "4", true);
                                                scorePanel.GetChild(totalBlood).gameObject.SetActive(false);
                                                bossPos.DOMove(bossPos.position, 1f).OnComplete(() => { spineMask.SetActive(false); isPlaying = false; playSuccessSpine(); });
                                            });
                                        }
                                        else
                                        {
                                            SpineManager.instance.DoAnimation(boss, "2", false, () => { SpineManager.instance.DoAnimation(boss, "1", true); scorePanel.GetChild(totalBlood).gameObject.SetActive(false); RandomYm(false,true); });
                                        }
                                    });
                                });
                        }
                        else
                        {
                            spineMask.SetActive(false);
                            isPlaying = false;
                        }
                    });
                //SpineManager.instance.DoAnimation(mirrorDrager.gameObject, "kong", false);
            }


        }


        private void RandomArr<T>(T[] arr)
        {
            RandomNew r = new RandomNew();//创建随机类对象，定义引用变量为r
            for (int i = 0; i < arr.Length; i++)
            {
                int index = r.Next(arr.Length);//随机获得0（包括0）到arr.Length（不包括arr.Length）的索引                                               
                T temp = arr[i];//当前元素和随机元素交换位置
                arr[i] = arr[index];
                arr[index] = temp;
            }
        }


    }
}
