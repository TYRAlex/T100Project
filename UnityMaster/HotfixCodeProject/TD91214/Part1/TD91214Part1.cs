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
    public class TD91214Part1
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
        private bool isPlaying;
        private GameObject btnBack;

        private int curPage;

        private Transform _pos1;
        private Transform _pos2;
        private Transform _pos3;
        private Transform _pos4;

        private Transform _level1;
        private GameObject[] _clickLevel1;
        private bool[] _allClickLevel1;
        private GameObject _ani;
        private Transform _level2;
        private GameObject[] _clickLevel2;
        private bool[] _allClickLevel2;
        private Transform _level3;
        private GameObject[] _clickLevel3;
        private bool[] _allClickLevel3;

        private int _curIndex;
        private GameObject _curObj;
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
            bd.SetActive(true);
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

            btnBack = curTrans.Find("btnBack").gameObject;
            Util.AddBtnClick(btnBack, OnClickBtnBack);
            btnBack.SetActive(false);

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            _pos1 = curTrans.Find("Pos1");
            _pos2 = curTrans.Find("Pos2");
            _pos3 = curTrans.Find("Pos3");
            _pos4 = curTrans.Find("Pos4");
            _level1 = curTrans.Find("Level1");
            _level2 = curTrans.Find("Level2");
            _level3 = curTrans.Find("Level3");
            _ani = _level1.GetGameObject("Ani");

            _clickLevel1 = new GameObject[_level1.childCount - 1];
            for (int i = 0; i < _level1.childCount - 1; i++)
            {
                Util.AddBtnClick(_level1.GetChild(i).gameObject, ClickLevel1);
            }
            _clickLevel2 = new GameObject[_level2.childCount];
            for (int i = 0; i < _level2.childCount; i++)
            {
                Util.AddBtnClick(_level2.GetChild(i).gameObject, OnClickShow);
            }
            _clickLevel3 = new GameObject[_level3.childCount];
            for (int i = 0; i < _level3.childCount; i++)
            {
                Util.AddBtnClick(_level3.GetChild(i).gameObject, OnClickShow);
            }

            _allClickLevel1 = new bool[_level1.childCount - 1];
            for (int i = 0; i < _allClickLevel1.Length; i++)
            {
                _allClickLevel1[i] = false;
            }
            _allClickLevel2 = new bool[_level2.childCount];
            for (int i = 0; i < _allClickLevel2.Length; i++)
            {
                _allClickLevel2[i] = false;
            }
            _allClickLevel3 = new bool[_level3.childCount];
            for (int i = 0; i < _allClickLevel3.Length; i++)
            {
                _allClickLevel3[i] = false;
            }

            curPage = 1;
            GameInit();
            GameStart();
        }

        private void ClickLevel1(GameObject obj)
        {
            if(!isPlaying)
            {
                isPlaying = true;
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
                int index = Convert.ToInt32(obj.name);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, index + 2,
                () =>
                {
                    _allClickLevel1[index] = true;
                    SpineManager.instance.DoAnimation(_ani, (index + 1).ToString(), false);
                },
                () =>
                {
                    JudgeClickAll(curPage);
                    isPlaying = false;
                }, 0));
            }
        }

        private void OnClickBtnBack(GameObject obj)
        {
            obj.SetActive(false);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 2);
            GameObject o = _curObj.transform.parent.gameObject;
            SpineManager.instance.DoAnimation(_curObj, _curObj.name + "2", false, 
            () => 
            { 
                SpineManager.instance.DoAnimation(_curObj, _curObj.name + "3", false, 
                () => 
                { 
                    o.transform.SetSiblingIndex(_curIndex);
                    isPlaying = false;
                    JudgeClickAll(curPage);
                }); 
            });
        }

        private void OnClickShow(GameObject obj)
        {
            if (!isPlaying)
            {
                isPlaying = true;
                SoundManager.instance.ShowVoiceBtn(false);
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 1);
                _curObj = obj.transform.GetGameObject(obj.name);
                _curIndex = obj.transform.GetSiblingIndex();
                if (curPage == 2)
                    _allClickLevel2[_curIndex] = true;
                if (curPage == 3)
                    _allClickLevel3[_curIndex] = true;
                obj.transform.SetAsLastSibling();
                SpineManager.instance.DoAnimation(_curObj, _curObj.name, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, curPage == 2 ? _curIndex + 10 : _curIndex + 14, null,
                () =>
                {
                    btnBack.SetActive(true);
                }));
            }
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
                        anyBtns.gameObject.SetActive(false); GameStart();
                    });
                }
                else if(obj.name == "fh")
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); mask.SetActive(false); GameInit(); });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () => { anyBtns.gameObject.SetActive(false); dbd.SetActive(true); mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, 0)); });
                }
               
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            _level1.position = _pos2.position;
            _level2.position = _pos3.position;
            _level3.position = _pos3.position;
            isPlaying = false;

            //动画初始化
            SpineManager.instance.DoAnimation(_ani, "0", false);
            for (int i = 0; i < _level2.childCount; i++)
            {
                GameObject o = _level2.GetChild(i).gameObject;
                SpineManager.instance.DoAnimation(o, o.name + "3", false);
            }
            for (int i = 0; i < _level3.childCount; i++)
            {
                GameObject o = _level3.GetChild(i).gameObject;
                SpineManager.instance.DoAnimation(o, o.name + "3", false);
            }
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
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

        IEnumerator WaitCoroutine(Action method_1 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            yield return new WaitForSeconds(len);
            method_1?.Invoke();
            SoundManager.instance.SetShield(true);
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1, null, 
                () => 
                { 
                    mask.SetActive(false); 
                    bd.SetActive(false); 
                }));
            }
            if (talkIndex == 2)
            {
                isPlaying = true;
                curPage = 2;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, 
                ()=> 
                {
                    mask.SetActive(true);
                    bd.SetActive(true);
                },
                () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                    _level1.DOLocalMoveX(_pos1.localPosition.x, 1.0f);
                    _level2.DOLocalMoveX(_pos2.localPosition.x, 1.0f);
                    mono.StartCoroutine(WaitCoroutine(() => { isPlaying = false; }, 1.0f));
                }, 0));
            }
            if (talkIndex == 3)
            {
                isPlaying = true;
                curPage = 3;
                _level2.DOLocalMoveX(_pos1.localPosition.x, 1.0f);
                _level3.DOLocalMoveX(_pos2.localPosition.x, 1.0f);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 13, 
                () =>
                {
                    mask.SetActive(true);
                    bd.SetActive(true);
                },
                () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
                    isPlaying = false;
                }, 1.0f));
            }
            if (talkIndex == 4)
            {
                isPlaying = true;
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 17,
                () =>
                {
                    mask.SetActive(true);
                    bd.SetActive(true);
                },null, 0));
            }
            talkIndex++;
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

        void JudgeClickAll(int page)
        {
            bool[] array;
            bool all = true;
            if (page == 1)
                array = _allClickLevel1;
            else if (page == 2)
                array = _allClickLevel2;
            else
                array = _allClickLevel3;
            for (int i = 0; i < array.Length; i++)
            {
                if(array[i] == false)
                {
                    all = false;
                    return;
                }
            }

            if (all)
                SoundManager.instance.ShowVoiceBtn(true);
            else
                SoundManager.instance.ShowVoiceBtn(false);
        }
    }
}
