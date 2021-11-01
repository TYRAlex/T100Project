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

    public class TD91211Part2
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

        private GameObject pageBar;
        private Transform SpinePage;

        private Empty4Raycast[] e4rs;

        GameObject _aniMask;

        private GameObject btnBack;
        List<string> _nameList;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

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
            mask.SetActive(false);


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

            anyBtns.gameObject.SetActive(false);
            anyBtns.GetChild(0).gameObject.SetActive(false);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);

            pageBar = curTrans.Find("PageBar").gameObject;
            _nameList = new List<string>();

            SpinePage = curTrans.Find("PageBar/MaskImg/SpinePage");
            e4rs = SpinePage.gameObject.GetComponentsInChildren<Empty4Raycast>(true);

            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClickMar);
                _nameList.Add(e4rs[i].name);
            }

            _aniMask = pageBar.transform.GetGameObject("animask");
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";

            btnBack = curTrans.Find("PageBar/MaskImg/btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();

        }


        void RemoveName(GameObject obj)
        {
            for (int i = 0; i < _nameList.Count; i++)
            {
                if (obj.name == _nameList[i])
                    _nameList.RemoveAt(i);
            }
        }

        private void OnClickMar(GameObject obj)
        {
            _aniMask.Show();
            SoundManager.instance.StopAudio(SoundManager.SoundType.COMMONVOICE);
            var parent = obj.transform.parent.gameObject;
            Debug.Log(parent.name);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
            if (obj.name == "a" || obj.name == "b")
            {
                RemoveName(obj);
                tem = obj.transform.parent.gameObject;
                tem.transform.SetAsLastSibling();
                if (obj.name == "a")
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 5, null, () =>
                    {
                        _aniMask.Hide();
                    
                        btnBack.SetActive(true);
                    }));
                }
                if (obj.name == "b")
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 6, null, () =>
                    {
                        _aniMask.Hide();
                     
                        btnBack.SetActive(true);
                    }));
                }

                SpineManager.instance.DoAnimation(tem, tem.name, false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(tem, tem.name + "2", false,
                            () => { });
                    });
            }
            else
            {
                RemoveName(obj);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, int.Parse(obj.name), null, () =>
                {
                    _aniMask.Hide();
                    if (_nameList.Count == 2) SoundManager.instance.ShowVoiceBtn(true);
                }));
                SpineManager.instance.DoAnimation(parent, obj.name, false,
                    () => { SpineManager.instance.DoAnimation(parent, parent.name, false, () => { }); });
            }

        }


        private GameObject tem;

        private void OnClickBtnBack(GameObject obj)
        {

            _aniMask.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            SpineManager.instance.DoAnimation(tem, tem.name + "3", false, () =>
            {
                SpineManager.instance.DoAnimation(tem, tem.name + "4", false, () =>
                {
                    if (_nameList.Count == 0) SoundManager.instance.ShowVoiceBtn(true);
                    obj.SetActive(false);
                    _aniMask.Hide();
                });
            });
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
            BtnPlaySound();
            SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        GameStart();
                    });
                }
                else if (obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        GameInit();
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd, 0));
                    });
                }
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;
            isPressBtn = false;
            _aniMask.Hide();
            SpinePage.GetRectTransform().anchoredPosition = new Vector2(0, 0);
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                if (i == 0)
                {

                    SpinePage.GetChild(0).transform.GetRectTransform().anchoredPosition = Vector3.zero;
                    SpineManager.instance.DoAnimation(SpinePage.GetChild(i).gameObject, SpinePage.GetChild(i).name,
                        false);
                }
                if (i == 1)
                {
                    SpinePage.GetChild(1).transform.GetRectTransform().anchoredPosition = new Vector3(1920, 0);
                    var a = SpinePage.GetChild(1).GetChild(0).gameObject;
                    var b = SpinePage.GetChild(1).GetChild(1).gameObject;
                    SpineManager.instance.DoAnimation(a, a.name + "4", false, () => { });
                    SpineManager.instance.DoAnimation(b, b.name + "4", false, () => { });
                }
            }

        }

        void Next()
        {
            _aniMask.Show();
            for (int i = 0; i < SpinePage.childCount; i++)
            {
                SpinePage.GetChild(0).transform.DOLocalMove(new Vector3(-2880, 540), 1f).OnComplete(()=> { _aniMask.Hide(); });
                //DOMove(new Vector3(-Screen.width,0),1f);
                SpinePage.GetChild(1).transform.DOLocalMove(new Vector3(0, 0), 1f);
            }

        }
        void GameStart()
        {
            mask.Show();

            bd.Show();
            Debug.Log("Gamestart");
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, () => { },
                () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                }));
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, GameObject bd, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bd, "bd-speak");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bd, "bd-daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 4, () => { },
                    () =>
                    {
                        mask.Hide();
                        bd.Hide();
                        Next();
                    }));
            }

            if (talkIndex == 2)
            {
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 7, () => { },
                    () =>
                    {
                    }));
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
                            anyBtns.GetChild(0).name = getBtnName(BtnEnum.fh, 0);
                            anyBtns.GetChild(1).name = getBtnName(BtnEnum.ok, 1);
                            anyBtns.gameObject.SetActive(true);
                            anyBtns.GetChild(0).gameObject.SetActive(true);
                            anyBtns.GetChild(1).gameObject.SetActive(true);
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
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
    }
}