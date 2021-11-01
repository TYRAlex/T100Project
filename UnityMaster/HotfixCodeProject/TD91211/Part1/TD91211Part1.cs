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

    public class TD91211Part1
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private Transform anyBtns;
        private GameObject successSpine;
        private GameObject caidaiSpine;

        private GameObject mask;

        //胜利动画名字
        private string tz;
        private string sz;


        //------------------------------------------------------
        private GameObject _parent;
        private GameObject _shadesParent;
        private GameObject _blackSpine;
        private List<GameObject> _shadeList;
        private List<ILDrager> _characterList;
        private GameObject _aniMask;

        private int score;
        //------------------------------------------------------

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
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

            anyBtns.gameObject.SetActive(true);
            anyBtns.GetChild(0).gameObject.SetActive(true);
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            //替换胜利动画需要替换spine 
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //-----------------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            var _parentTrans = _parent.transform;
            _blackSpine = _parentTrans.GetGameObject("black");
            _shadesParent = _parentTrans.GetGameObject("shades");
            _characterList = new List<ILDrager>();
            _shadeList = new List<GameObject>();
            score = 0;

            for (int i = 0; i < _parentTrans.GetTransform("shades").childCount; i++)
            {
                var child = _parentTrans.GetTransform("shades").GetChild(i).gameObject;
                _shadeList.Add(child);
            }

            for (int i = 0; i < _parentTrans.GetTransform("characters").childCount; i++)
            {
                var child = _parentTrans.GetTransform("characters").GetChild(i).gameObject;
                _characterList.Add(child.GetComponent<ILDrager>());
            }

            for (int i = 0; i < _characterList.Count; i++)
            {
                var child = _characterList[i];
                child.SetDragCallback(null, null, EndDrage1);
            }

            _aniMask = _parentTrans.GetGameObject("animask");
            //-----------------------------------------------------------------------------
            GameInit();
            IsStart();
            //GameStart();
        }

        private void EndDrage(Vector3 endPos, int type, int index, bool isTrue)
        {
            _aniMask.Show();
            if (isTrue)
            {
                SoundManager.instance.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd,
                    Random.Range(4, 10), null, () => { _aniMask.Hide(); }));
                //_shadeList[index].transform.position;
                if (score < 3)
                {
                    for (int i = 0; i < _shadeList[0].transform.childCount; i++)
                    {
                        var child = _shadeList[0].transform.GetChild(i).gameObject;

                        if (child.name == _characterList[index].name)
                        {
                            Debug.Log("Set True");
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            _characterList[index].transform.position = child.transform.position;
                            _characterList[index].canMove = false;
                            var temp = _characterList[index].transform.GetChild(0).gameObject;                         
                            SpineManager.instance.DoAnimation(temp, temp.name + "4", false,
                                () => { SpineManager.instance.DoAnimation(temp, temp.name + "1", true); });

                            Debug.LogError("Score: " + score + "         childname:" + child.name);
                            if (score == 2)
                            {
                                _aniMask.Show();
                                Wait(2f, () =>
                                {
                                    CharacterListDoreset();
                                    SpineManager.instance.DoAnimation(_blackSpine, "c", false, () =>
                                    {
                                        _aniMask.Hide();
                                        ShowShade(1);
                                        SetDrop(1);
                                    });
                                });
                            }
                        }
                    }
                }

                if (3 <= score && score < 6)
                {
                    for (int i = 0; i < _shadeList[1].transform.childCount; i++)
                    {
                        var child = _shadeList[1].transform.GetChild(i).gameObject;
                        if (child.name == _characterList[index].name)
                        {
                            _characterList[index].transform.position = child.transform.position;
                            var temp = _characterList[index].transform.GetChild(0).gameObject;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            _characterList[index].canMove = false;
                            SpineManager.instance.DoAnimation(temp, temp.name + "4", false,
                                () => { SpineManager.instance.DoAnimation(temp, temp.name + "1", true); });

                            Debug.LogError("add Score");
                            if (score == 5)
                            {

                                Wait(2f, () =>
                                {
                                    CharacterListDoreset();
                                    SpineManager.instance.DoAnimation(_blackSpine, "b", false, () =>
                                    {
                                        ShowShade(2);
                                        SetDrop(2);
                                    });

                                });
                            }
                        }
                    }
                }

                if (6 <= score)
                {
                    for (int i = 0; i < _shadeList[2].transform.childCount; i++)
                    {
                        var child = _shadeList[2].transform.GetChild(i).gameObject;
                        if (child.name == _characterList[index].name)
                        {
                            _characterList[index].transform.position = child.transform.position;
                            _characterList[index].canMove = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                            var temp = _characterList[index].transform.GetChild(0).gameObject;
                            SpineManager.instance.DoAnimation(temp, temp.name + "4", false,
                                () => { SpineManager.instance.DoAnimation(temp, temp.name + "1", false); });

                            if (score == 8)
                            {
                                _aniMask.Show();
                                Wait(2f, () => { playSuccessSpine(); _aniMask.Hide(); });
                            }
                        }
                    }
                }

                score++;
                Debug.Log("score：    " + score);
            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                Debug.Log("Set False");
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd, Random.Range(0, 4), () => { }, () => { _aniMask.Hide(); }));
                _characterList[index].DoReset();
            }
        }

        void EndDrage1(Vector3 endPos, int type, int index, bool isTrue)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
            SoundManager.instance.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd,
                Random.Range(0, 4), () => { _aniMask.Show(); }, () => { _aniMask.Hide(); }));
            _characterList[index].DoReset();
            Debug.Log(_characterList[index].name);
        }

        void IsStart()
        {
            mask.Show();
            bd.Hide();
            dbd.Hide();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.SetActive(true);
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
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, dbd, 2));
                    });
                }
            });
        }

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            _aniMask.Hide();
            SpineManager.instance.DoAnimation(_blackSpine, "a", true);
            for (int i = 0; i < _characterList.Count; i++)
            {
                var temp = _characterList[i];
                var child = temp.transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(child, child.name + "1");
            }
            talkIndex = 1;
            score = 0;
            ShowShade(0);
            SetDrop(0);
            CharacterListDoreset();
        }

        //显示关卡
        void ShowShade(int index)
        {
            Debug.Log("index____________" + index);
            for (int i = 0; i < _shadeList.Count; i++)
            {
                var child = _shadeList[i];
                if (i == index)
                {
                    child.Show();
                    for (int j = 0; j < child.transform.childCount; j++)
                    {
                        var temp = child.transform.GetChild(j).gameObject;
                        temp.Show();
                    }
                }
                else
                {
                    child.Hide();
                    for (int j = 0; j < child.transform.childCount; j++)
                    {
                        var temp = child.transform.GetChild(j).gameObject;
                        temp.Hide();
                    }
                }
            }
        }


        //人物返回，动画初始化
        void CharacterListDoreset()
        {
            for (int i = 0; i < _characterList.Count; i++)
            {
                var child = _characterList[i];
                child.canMove = true;
                child.DoReset();
                var temp = child.transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(temp, temp.name + "1");
            }
        }

        //设置drop
        void SetDrop(int shadeIndex)
        {
            for (int j = 0; j < _characterList.Count; j++)
            {
                var child = _characterList[j];
                child.canMove = true;
                for (int i = 0; i < _shadeList[shadeIndex].transform.childCount; i++)
                {
                    var temp = _shadeList[shadeIndex].transform.GetChild(i).gameObject;
                    if (temp.name != child.name)
                    {            
                        child.SetDragCallback(null, null, EndDrage1);
                    }
                    else
                    {
                        child.SetDragCallback(null, null, EndDrage);
                       
                        child.drops[0] = temp.GetComponent<ILDroper>();
                        break;
                        
                    }
                }
            }
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            bd.Show();
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, () => { },
                () => { SoundManager.instance.ShowVoiceBtn(true); }));
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
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null, () =>
                {
                    mask.SetActive(false);
                    bd.SetActive(false);
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
                            SpineManager.instance.DoAnimation(anyBtns.GetChild(1).gameObject, "ok2", false);
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

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSencondsDoSomething(time, method));
        }

        IEnumerator WaitForSencondsDoSomething(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }
    }
}