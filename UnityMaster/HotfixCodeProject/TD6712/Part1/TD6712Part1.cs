using DG.Tweening;
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
    }

    public class TD6712Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;

        private GameObject bfBtn;
        private GameObject okBtn;
        private GameObject fhBtn;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;


        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        //--------------------------------------------------
        private GameObject _parent; //父物体
        private GameObject _spineParent; //spine父物体
        private List<GameObject> _spineList; //spine List
        private GameObject _scoreParent;
        private BellSprites _numberSprites; //分数
        private GameObject _picturParent;
        private List<GameObject> _pictureList;

        private GameObject _bd; //胜利时布丁
        private GameObject _aniMask;

        private int number;
     //--------------------------------------------------


        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();

            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);


            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);

            bfBtn = curTrans.Find("mask/bf").gameObject;
            okBtn = curTrans.Find("mask/ok").gameObject;
            fhBtn = curTrans.Find("mask/fh").gameObject;
            Util.AddBtnClick(bfBtn, OnClickAnyBtn);
            Util.AddBtnClick(okBtn, OnClickAnyBtn);
            Util.AddBtnClick(fhBtn, OnClickAnyBtn);
            //-----------------------------------------

            _bd = curTrans.Find("mask/bd").gameObject;
            _parent = curTrans.GetGameObject("parent");
            var parentTrans = _parent.transform;
            _spineParent = parentTrans.GetGameObject("spines");
            _spineList = new List<GameObject>();
            for (int i = 0; i < _spineParent.transform.childCount; i++)
            {
                var child = _spineParent.transform.GetChild(i).gameObject;
                _spineList.Add(child);
                Util.AddBtnClick(child, SpineOnClick);
            }

            _scoreParent = parentTrans.GetGameObject("scores");
            _numberSprites = _scoreParent.GetComponentInChildren<BellSprites>();

            _picturParent = parentTrans.GetGameObject("pictures");
            _pictureList = new List<GameObject>();
            for (int i = 0; i < _picturParent.transform.childCount; i++)
            {
                var child = _picturParent.transform.GetChild(i).gameObject;
                _pictureList.Add(child);
            }

            _aniMask = parentTrans.GetGameObject("animask");
            //-----------------------------------------


            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            IsStart();
            //GameStart();
        }

        void IsStart()
        {
            mask.Show();
            bfBtn.Show();
            okBtn.Hide();
            fhBtn.Hide();
            _bd.Hide();
            bd.Hide();
        }


        /// <summary>
        /// 定义按钮mode
        /// </summary>
        /// <param name="btnEnum"></param>
        /// <returns></returns>
        public string getBtnName(BtnEnum btnEnum)
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

            SpineManager.instance.DoAnimation(bfBtn, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                    bfBtn.Hide();
                }
                else if (obj.name == "fh")
                {
                    GameInit();
                }
                else
                {
                    mask.Show();
                    _bd.Show();
                    okBtn.Hide();
                    fhBtn.Hide();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, _bd, null,
                        () => { }));
                }

                /*mask.gameObject.SetActive(false);
                bfBtn.name = "Btn";*/
            });
        }

        private void GameInit()
        {
            //------------
            mask.Hide();
            _aniMask.Hide();
            mono.StopAllCoroutines();
            number = 0;
            DOTween.KillAll();
            _scoreParent.transform.GetChild(0).gameObject.GetComponent<Image>().sprite = _numberSprites.sprites[0];
            SpineManager.instance.DoAnimation(bfBtn, "bf2", true);
            SpineManager.instance.DoAnimation(fhBtn, "fh2", true);
            SpineManager.instance.DoAnimation(okBtn, "ok2", true);
            SpineAndPicturesInit();
            _bd.Hide();
            //------------
            talkIndex = 1;

            isPressBtn = false;
        }

        void GameStart()
        {
            #region 弹出对话部分

            /*buDing.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
            {
                /*正义的一方对话结束 devil开始动画#1#

                devil.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
                {
                    /*对话#1#
                });
            });*/

            #endregion

            isPlaying = true;
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mask.Show();
            bd.Show();
            //小朋友们你们知道生活中有哪些图形是圆形的吗
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, bd, null, () =>
            {
                SoundManager.instance.ShowVoiceBtn(true);
                isPlaying = false;
            }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, GameObject obj, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(obj, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(obj, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(obj, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, bd, null,
                    () =>
                    {
                        bd.SetActive(false);
                        mask.SetActive(false);
                    }));
            }

            if (talkIndex == 2)
            {
            }

            talkIndex++;
        }

        /// <summary>
        /// 播放成功动画
        /// </summary>
        private void playSuccessSpine(Action ac = null)
        {
            Debug.Log("ending");
            mask.SetActive(true);
            successSpine.SetActive(true);
            okBtn.Hide();
            fhBtn.Hide();
            caidaiSpine.SetActive(true);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(caidaiSpine, "sp", false);
            SpineManager.instance.DoAnimation(successSpine, sz, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(successSpine, sz + "2", false,
                        () =>
                        {
                            /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                            //caidaiSpine.SetActive(false);
                            //successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        void SpineAndPicturesInit()
        {
            for (int i = 0; i < _spineList.Count; i++)
            {
                var temp = _spineList[i];
                temp.Show();
                temp.GetComponent<Empty4Raycast>().raycastTarget = true;

                var child1 = temp.transform.GetChild(0).gameObject;
                child1.Show();
                child1.transform.position = temp.transform.position;
                //SpineManager.instance.DoAnimation(child1, "kong", true);
                Wait(Random.Range(0.1f, 0.4f),
                    () => { SpineManager.instance.DoAnimation(child1, temp.name + "2", true); });
            }

            for (int i = 0; i < _pictureList.Count; i++)
            {
                var child = _pictureList[i];
                child.Hide();
            }
        }

        //游戏
        void SpineOnClick(GameObject obj)
        {
            _aniMask.Show();
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            switch (obj.name)
            {
                case "ty":
                case "yl":
                case "ttq":
                case "xsigu":
                case "pisa":
                    var child1 = obj.transform.GetChild(0).gameObject;

                    number++;
                    DoAnimation(child1, obj.name + "2", false, () => { });


                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), bd,
                        null, () => { }));
                    Wait(0.5f, () =>
                    {
                        DoAnimation(child1, child1.name, false, () =>
                        {
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                            DoAnimation(child1, child1.name + "3", false, () =>
                            {
                                _aniMask.Hide();
                                ShowTrueImage(obj);
                                _scoreParent.transform.GetChild(0).gameObject.GetComponent<Image>().sprite =
                                    _numberSprites.sprites[number];
                            });
                        });
                    });

                    break;
                default:
                    var child = obj.transform.GetChild(0).gameObject;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), bd,
                        null, () => { _aniMask.Hide(); }));
                    DoAnimation(child, obj.name, false,
                        () => { DoAnimation(child, obj.name + "2", true, () => { }); });
                    break;
            }
        }


        void DoAnimation(GameObject obj, string name, bool isloop = false, Action method = null)
        {
            SpineManager.instance.DoAnimation(obj, name, isloop, method);
        }

        //正确点击 显示
        void ShowTrueImage(GameObject obj)
        {
            for (int i = 0; i < _pictureList.Count; i++)
            {
                var child = _pictureList[i];
                if (obj.name == child.name)
                {
                    obj.GetComponent<Empty4Raycast>().raycastTarget = false;
                    var tw = obj.transform.GetChild(0).transform.DOMove(child.transform.position, 1.5f).OnComplete(() =>
                    {                    
                    });
                    break;
                }
            }
            if(number==5)
            {
               Wait(2f, ()=>{ SuccessSence(); });
            }
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSencondDoSomething(time, method));
        }

        IEnumerator WaitForSencondDoSomething(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }


        void SuccessSence()
        { 
            Debug.Log("NUmber      " + number);
            if (number == 5)
            {
                Debug.Log("  " + number);
                _aniMask.Show();
                Wait(2f, () =>
                {
                    playSuccessSpine(() =>
                    {
                        caidaiSpine.SetActive(false);
                        successSpine.SetActive(false);
                        okBtn.Show();
                        fhBtn.Show();
                    });
                });
            }
        }
    }
}