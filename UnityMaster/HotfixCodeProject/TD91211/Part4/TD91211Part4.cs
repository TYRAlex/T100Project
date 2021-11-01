using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
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

    public class TD91211Part4
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

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine
        private GameObject buDing;
        private Text bdText;
        private GameObject devil;
        private Text devilText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        Dictionary<GameObject, Vector3> _allVirusDic;
        Dictionary<GameObject, bool> _isDoLerp;


        //private Dictionary<GameObject, Pao> _paos;

        //-------------------------------------------------------------------------
        private GameObject _parent;
        private GameObject _paopao;
        GameObject _aniMask;
        private GameObject _buding;
        private Empty4Raycast[] e4rs;
        GameObject _xem;
        private List<GameObject> _spineList;
        private int _level;
        //-------------------------------------------------------------------------


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


            buDing = curTrans.Find("mask/buDing").gameObject;
            bdText = buDing.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            buDing.transform.position = bdStartPos.position;
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devil.transform.position = devilStartPos.position;
            devilEndPos = curTrans.Find("mask/devilEndPos");

            bd = curTrans.Find("mask/BD").gameObject;
            bd.SetActive(false);
            dbd = curTrans.Find("mask/DBD").gameObject;

            _xem = curTrans.Find("mask/xem").gameObject;
            _xem.Hide();
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

            //--------------------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            _paopao = _parent.transform.GetGameObject("paopao");
            _aniMask = _parent.transform.GetGameObject("animask");
            e4rs = _paopao.gameObject.GetComponentsInChildren<Empty4Raycast>(true);
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                Util.AddBtnClick(e4rs[i].gameObject, OnClick);
            }

            _buding = _parent.transform.GetGameObject("buding");
            _spineList = new List<GameObject>();
            for (int i = 0; i < _parent.transform.GetGameObject("Image").transform.childCount; i++)
            {
                var child = _parent.transform.GetGameObject("Image").transform.GetChild(i).gameObject;
                _spineList.Add(child);
            }
            //--------------------------------------------------------------------------------

            GameInit();
            //GameStart();
            IsStart();
        }

        private void OnClick(GameObject obj)
        {
            _aniMask.Show();
            switch (_level)
            {
                case 0:
                    if (obj.name == "a")
                    {
                        _level++;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(4, 9),
                            null,
                            () =>
                            {
                                _spineList[1].Show();

                                for (int i = 0; i < _spineList[1].transform.childCount; i++)
                                {

                                    var temp = _spineList[1].transform.GetChild(i).gameObject;
                                    SpineManager.instance.DoAnimation(temp, temp.name, false);
                                    _spineList[0].transform.GetChild(0).gameObject.Hide();
                                }
                                Wait(1f, () =>
                                {
                                    Next(_level);
                                    _aniMask.Hide();
                                });
                            }));
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                            () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); obj.transform.parent.gameObject.Hide(); });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        if (obj.name == "h" || obj.name == "g")
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }

                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(0, 4),
                            null,
                            () => { _aniMask.Hide(); }));
                    }

                    break;
                case 1:
                    if (obj.name == "c")
                    {
                        _level++;

                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);

                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(4, 9),
                            null,
                            () =>
                            {
                                _spineList[2].Show();
                                for (int i = 0; i < _spineList[2].transform.childCount; i++)
                                {
                                    var temp = _spineList[2].transform.GetChild(i).gameObject;
                                    SpineManager.instance.DoAnimation(temp, temp.name, false);
                                    _spineList[0].transform.GetChild(1).gameObject.Hide();
                                }
                                Wait(1f, () =>
                                {
                                    Next(_level);
                                    _aniMask.Hide();
                                });
                            }));
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                            () =>
                            {
                                obj.transform.parent.gameObject.Hide();
                                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0);
                            });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        if (obj.name == "h" || obj.name == "g")
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }

                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(0, 4),
                            null,
                            () => { _aniMask.Hide(); }));
                    }

                    break;
                case 2:
                    if (obj.name == "f")
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 3, false);
                        _level++;
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(4, 9),
                            null,
                            () =>
                            {
                                _spineList[3].Show();
                                for (int i = 0; i < _spineList[3].transform.childCount; i++)
                                {


                                    var temp = _spineList[3].transform.GetChild(i).gameObject;
                                    SpineManager.instance.DoAnimation(temp, temp.name, false);
                                    _spineList[0].transform.GetChild(2).gameObject.Hide();
                                }
                                Wait(1f, () =>
                                {
                                    playSuccessSpine();
                                    _aniMask.Hide();
                                });
                            }));
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                            () => { obj.transform.parent.gameObject.Hide(); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        if (obj.name == "h" || obj.name == "g")
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }

                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(0, 4),
                            null,
                            () => { _aniMask.Hide(); }));
                    }

                    break;
                case 3:
                    if (obj.name == "e")
                    {

                        _level++;
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(4, 9),
                            null,
                            () =>
                            {

                                Wait(2f, () =>
                                {
                                    playSuccessSpine();
                                    _aniMask.Hide();
                                });
                            }));
                        SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                            () => { obj.transform.parent.gameObject.Hide(); SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        if (obj.name == "h" || obj.name == "g")
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name, false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }
                        else
                        {
                            SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "3", false,
                                () =>
                                {
                                    SpineManager.instance.DoAnimation(obj.transform.parent.gameObject, obj.name + "2",
                                        true);
                                });
                        }

                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, _buding,
                            Random.Range(0, 4),
                            null,
                            () => { _aniMask.Hide(); }));
                    }

                    break;
            }
        }


        void Next(int index)
        {
            _buding.transform.GetChild(1).gameObject.Hide();
            for (int i = 0; i < e4rs.Length; i++)
            {
                e4rs[i].transform.parent.gameObject.Show();
                SpineManager.instance.DoAnimation(e4rs[i].transform.parent.gameObject,
                    e4rs[i].transform.parent.gameObject.name + "2",
                    true);
            }

            switch (index)
            {
                case 1:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject,
                        _buding.transform.GetChild(0).name, false, () =>
                        {
                            _buding.transform.GetChild(1).gameObject.Show();
                            SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject,
                                _buding.transform.GetChild(1).name + "2", false);
                        });

                    break;
                case 2:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject,
                        _buding.transform.GetChild(0).name, false, () =>
                        {
                            _buding.transform.GetChild(1).gameObject.Show();
                            SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject,
                                _buding.transform.GetChild(1).name + "5", false);
                        });
                    break;
                case 3:
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject,
                        _buding.transform.GetChild(0).name, false, () =>
                        {
                            _buding.transform.GetChild(1).gameObject.Show();
                            SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject,
                                _buding.transform.GetChild(1).name + "3", false);
                        });
                    break;
            }
        }

        void IsStart()
        {
            mask.Show();
            anyBtns.GetChild(0).gameObject.Show();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
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
                        GameInit();
                        anyBtns.gameObject.SetActive(false);
                        mask.SetActive(false);
                        _buding.Show();
                        for (int i = 0; i < _buding.transform.childCount; i++)
                        {

                            _buding.transform.GetChild(i).gameObject.Show();

                        }
                        SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject,
            _buding.transform.GetChild(0).name, false, () =>
            {
                SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject,
                    _buding.transform.GetChild(1).name + "1", false);
            });
                    });
                }
                else
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false, () =>
                    {
                        _buding.Hide();
                        SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, dbd, 4));
                    });
                }
            });
        }

        private void GameInit()
        {
            mono.StopAllCoroutines();
            _aniMask.Hide();
            _xem.Hide();
            _buding.Hide();
            talkIndex = 1;
            textSpeed = 0.1f;
            devilText.text = "";
            bdText.text = "";
            SpineManager.instance.DoAnimation(_xem, "daiji", true);

            for (int i = 0; i < _spineList.Count; i++)
            {
                var child = _spineList[i];
                if (i == 0)
                {
                    child.Show();
                    for (int j = 0; j < child.transform.childCount; j++)
                    {
                        child.transform.GetChild(j).gameObject.Show();
                    }
                }
                else
                {
                    child.Hide();
                }

            }
            //----------------------------------------

            _level = 0;
            for (int i = 0, len = e4rs.Length; i < len; i++)
            {
                var parent = e4rs[i].transform.parent.gameObject;
                parent.Show();
                SpineManager.instance.DoAnimation(parent, parent.name + "2", true);
            }

            SpineManager.instance.DoAnimation(_buding, "bd-daiji", true);
            SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject, "kong", true);
            SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject, "kong", true);
            //----------------------------------------
        }


        void StartGame()
        {

            _buding.Show();
            bd.Hide();
            _buding.transform.GetChild(0).gameObject.Show();
            _buding.transform.GetChild(1).gameObject.Show();
            _aniMask.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
            SpineManager.instance.DoAnimation(_buding.transform.GetChild(0).gameObject,
                _buding.transform.GetChild(0).name, false, () =>
                {
                    SpineManager.instance.DoAnimation(_buding.transform.GetChild(1).gameObject,
                        _buding.transform.GetChild(1).name + "1", false);
                });
        }

        void GameStart()
        {


            _aniMask.Show();
            _xem.Show();
            bd.Hide();
            dbd.Hide();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, () => { SpineManager.instance.DoAnimation(_xem, "speak", true); },
                () =>
                {
                    SpineManager.instance.DoAnimation(_xem, "daiji", true);
                    bd.Show();
                    _xem.Hide();
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1,
                               () =>
                               {

                               },
                               () =>
                               {
                                   mask.Hide();
                                   StartGame();

                               }));
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
                buDing.SetActive(false);
                devil.SetActive(false);
                bd.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.COMMONVOICE, bd, 0, null, () =>
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
                            SpineManager.instance.DoAnimation(anyBtns.GetChild(1).gameObject, "ok2");
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
                        });
                });
        }


        //运动


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


        void ShowDialogue(string str, Text text, Action callBack = null)
        {
            mono.StartCoroutine(IEShowDialogue(str, text, callBack));
        }

        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0"); //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
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