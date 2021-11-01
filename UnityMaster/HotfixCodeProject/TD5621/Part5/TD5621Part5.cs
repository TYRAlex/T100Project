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

    public class TD5621Part5
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject dbd;
        private GameObject _tt;
        private GameObject Bg;

        private Transform anyBtns;

        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        private int flag = 0;

        //创作指引完全结束
        bool isEnd = false;

        #region 田丁对话

        private float textSpeed;

        //用于情景对话，需要的自行复制在 Dialogues路径下找对应spine

        private GameObject devil;
        private Text devilText;
        private GameObject tt;
        private Text ttText;

        private Transform bdStartPos;
        private Transform bdEndPos;
        private Transform devilStartPos;
        private Transform devilEndPos;
        private Transform SpineShow;

        #endregion

        //--------------------------------------------------------------------
        private GameObject _game1;
        private GameObject _game2;
        private GameObject _game3;

        private List<mILDrager> _dragers;

        private List<mILDroper> _dropers;

        private GameObject _aniMask;

        private GameObject _xem;

        private GameObject _score;

        private GameObject _number;

        private GameObject _next;
        private int level;

        //--------------------------------------------------------------------
        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;


            mask = curTrans.Find("mask").gameObject;
            mask.SetActive(false);


            bd = curTrans.Find("mask/BD").gameObject;
            _tt = curTrans.Find("mask/TT").gameObject;
            _tt.Hide();
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


            tt = curTrans.Find("mask/tt").gameObject;
            ttText = tt.transform.GetChild(0).GetComponent<Text>();
            bdStartPos = curTrans.Find("mask/bdStartPos");
            bdEndPos = curTrans.Find("mask/bdEndPos");

            devil = curTrans.Find("mask/devil").gameObject;
            devilText = devil.transform.GetChild(0).GetComponent<Text>();
            devilStartPos = curTrans.Find("mask/devilStartPos");
            devilEndPos = curTrans.Find("mask/devilEndPos");

            //=========================================================================
            _game1 = curTrans.GetGameObject("game1");
            _game2 = curTrans.GetGameObject("game2");
            _game3 = curTrans.GetGameObject("game3");
            _dragers = new List<mILDrager>();
            _dropers = new List<mILDroper>();
            _aniMask = curTrans.GetGameObject("animask");
            _xem = curTrans.GetGameObject("xem");
            _score = curTrans.GetGameObject("score");
            _number = _score.transform.GetGameObject("number");
            _next = curTrans.GetGameObject("next");

            Util.AddBtnClick(_next, NextGame);
            //game1
            for (int i = 0; i < _game1.transform.childCount; i++)
            {
                if (2 < i)
                {
                    var child = _game1.transform.GetChild(i).GetComponent<mILDrager>();
                    _dragers.Add(child);
                    child.SetDragCallback(StartDrag, null, EndDrag);
                }

                if (0 < i && i < 3)
                {
                    var temp = _game1.transform.GetChild(i).GetChild(0).GetComponent<mILDroper>();
                    _dropers.Add(temp);
                }
            }

            //game2
            for (int i = 0; i < _game2.transform.childCount; i++)
            {
                if (3 < i)
                {
                    var child = _game2.transform.GetChild(i).GetComponent<mILDrager>();
                    _dragers.Add(child);
                    child.SetDragCallback(StartDrag, null, EndDrag);
                }

                if (0 < i && i < 4)
                {
                    var temp = _game2.transform.GetChild(i).GetChild(0).GetComponent<mILDroper>();
                    _dropers.Add(temp);
                }
            }

            //game3
            for (int i = 0; i < _game3.transform.childCount; i++)
            {
                if (4 < i)
                {
                    var child = _game3.transform.GetChild(i).GetComponent<mILDrager>();
                    _dragers.Add(child);
                    child.SetDragCallback(StartDrag, null, EndDrag);
                }

                if (0 < i && i < 5)
                {
                    var temp = _game3.transform.GetChild(i).GetChild(0).GetComponent<mILDroper>();
                    _dropers.Add(temp);
                }
            }
            //=========================================================================

            GameInit();
            IsStart();
        }

        private void StartDrag(Vector3 arg1, int arg2, int arg3)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4);
        }

        private void NextGame(GameObject obj)
        {
           
            _aniMask.Hide();
            for (int i = 0; i < _dragers.Count; i++)
            {
                _dragers[i].canMove = true;
            }

            Wait(0.5f, () =>
            {
                Debug.Log("aaa");
                SoundManager.instance.PlayClip(9);
                SpineManager.instance.DoAnimation(obj, obj.name, false, () =>
                {
                    SpineManager.instance.DoAnimation(obj, "kong", false);
                    obj.Hide();
                    if (level == 2)
                    {
                        _game2.Show();
                        InitSpine();
                        _game1.Hide();
                        _game3.Hide();
                        Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[1];
                    }

                    if (level == 5)
                    {
                        _game3.Show();
                        InitSpine();
                        _game1.Hide();
                        _game2.Hide();
                        Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[2];
                    }
                });
            });
        }

        private void IsStart()
        {
            mask.Show();
            anyBtns.gameObject.Show();
            anyBtns.GetChild(0).name = getBtnName(BtnEnum.bf, 0);
            anyBtns.GetChild(0).gameObject.Show();
        }


        private void EndDrag(Vector3 endPos, int type, int index, bool isend)
        {
            _aniMask.Show();
            int dropIndex = 0;
            switch (type)
            {
                case 0:
                    dropIndex = 0;
                    break;
                case 1:
                    dropIndex = 1;
                    break;
                case 4:
                    dropIndex = 2;
                    break;
                case 6:
                    dropIndex = 3;
                    break;
                case 7:
                    dropIndex = 4;
                    break;
                case 15:
                    dropIndex = 5;
                    break;
                case 13:
                    dropIndex = 6;
                    break;
                case 11:
                    dropIndex = 7;
                    break;
                case 12:
                    dropIndex = 8;
                    break;
            }

            if (isend)
            {
                level++;
                Debug.Log("setture");
                SpineManager.instance.DoAnimation(_dragers[index].transform.GetChild(0).gameObject,
                    _dragers[index].name + "boom", false, () =>
                    {
                        _dragers[index].DoReset();
                        _dragers[index].gameObject.Hide();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2);
                        var temp = _dropers[dropIndex].transform.parent.gameObject;
                        BtnPlaySoundSuccess(() => { });
                        SpineManager.instance.DoAnimation(temp, temp.name + "2", false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(temp, temp.name, false);
                                if (level == 2 || level == 5 || level == 9)
                                {
                                    SpineManager.instance.DoAnimation(_xem, "kong", false);
                                    _xem.transform.localPosition = new Vector3(-960, 1500);
                                    _xem.Show();

                                    SpineManager.instance.DoAnimation(_xem, "kong", false, () =>
                                    {
                                        Wait(2.4f,
                                            () => { SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0); });

                                        SpineManager.instance.DoAnimation(_xem, "xem", true,
                                            () =>
                                            {
                                                _xem.transform.DOLocalMove(new Vector3(-960, 480), 2f).OnComplete(() =>
                                                {
                                                    SpineManager.instance.DoAnimation(_xem, "xem2", false,
                                                        () =>
                                                        {
                                                            if (level == 2)
                                                            {
                                                                _number.GetComponent<Image>().sprite =
                                                                    _number.GetComponent<BellSprites>().sprites[1];
                                                                _score.transform.GetGameObject("0")
                                                                        .GetComponent<Image>()
                                                                        .sprite =
                                                                    _score.transform.GetGameObject("0")
                                                                        .GetComponent<BellSprites>()
                                                                        .sprites[1];
                                                                Wait(0.5f, () =>
                                                                {
                                                                    _next.Show();
                                                                    
                                                                    SpineManager.instance.DoAnimation(_next,
                                                                        _next.name + "2",
                                                                        true);
                                                                    for (int i = 0; i < _dragers.Count; i++)
                                                                    {
                                                                        _dragers[i].canMove = false;
                                                                    }
                                                                });
                                                            }

                                                            if (level == 5)
                                                            {
                                                                _number.GetComponent<Image>().sprite =
                                                                    _number.GetComponent<BellSprites>().sprites[2];
                                                                _score.transform.GetGameObject("1")
                                                                        .GetComponent<Image>()
                                                                        .sprite =
                                                                    _score.transform.GetGameObject("1")
                                                                        .GetComponent<BellSprites>()
                                                                        .sprites[1];
                                                                Wait(0.5f, () =>
                                                                {
                                                                    _next.Show();
                                                                 
                                                                    for (int i = 0; i < _dragers.Count; i++)
                                                                    {
                                                                        _dragers[i].canMove = false;
                                                                    }

                                                                    SpineManager.instance.DoAnimation(_next,
                                                                        _next.name + "2",
                                                                        true);
                                                                });
                                                            }

                                                            if (level == 9)
                                                            {
                                                                _number.GetComponent<Image>().sprite =
                                                                    _number.GetComponent<BellSprites>().sprites[3];
                                                                _score.transform.GetGameObject("2")
                                                                        .GetComponent<Image>()
                                                                        .sprite =
                                                                    _score.transform.GetGameObject("2")
                                                                        .GetComponent<BellSprites>()
                                                                        .sprites[1];
                                                                for (int i = 0; i < _dragers.Count; i++)
                                                                {
                                                                    _dragers[i].canMove = false;
                                                                }

                                                                _aniMask.Show();
                                                                Wait(2f, () =>
                                                                {
                                                                    playSuccessSpine();
                                                                    _aniMask.Hide();
                                                                });
                                                            }

                                                            SpineManager.instance.DoAnimation(_xem, "kong", false);
                                                            _xem.Hide();
                                                        });
                                                });
                                            });
                                    });
                                }
                                else
                                {
                                    _aniMask.Hide();
                                }
                            });
                    });
            }
            else
            {
                Debug.Log("set false");
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 5);
                BtnPlaySoundFail(() => { _aniMask.Hide(); });
                _dragers[index].DoReset();
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
                        anyBtns.gameObject.SetActive(false);
                        TDGameStart();
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
                        switchBGM();
                        anyBtns.gameObject.SetActive(false);
                        dbd.SetActive(true);
                        mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 4));
                    });
                }
            });
        }

        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            talkIndex = 1;
            level = 0;
            isPressBtn = false;
            flag = 0;

            tt.transform.position = bdStartPos.position;
            devil.transform.position = devilStartPos.position;

            for (int i = 0; i < _dragers.Count; i++)
            {
                _dragers[i].canMove = true;
            }

            SpineManager.instance.DoAnimation(_next, _next.name + "2", true);
            Bg.GetComponent<RawImage>().texture = Bg.GetComponent<BellSprites>().texture[0];
            _score.transform.GetGameObject("0").GetComponent<Image>().sprite =
                _score.transform.GetGameObject("0").GetComponent<BellSprites>().sprites[0];
            _score.transform.GetGameObject("1").GetComponent<Image>().sprite =
                _score.transform.GetGameObject("1").GetComponent<BellSprites>().sprites[0];
            _score.transform.GetGameObject("2").GetComponent<Image>().sprite =
                _score.transform.GetGameObject("2").GetComponent<BellSprites>().sprites[0];
            _number.GetComponent<Image>().sprite = _number.GetComponent<BellSprites>().sprites[0];
            _xem.transform.localPosition = new Vector3(-960, 1500);

            _tt.Hide();
            devil.Show();
            ttText.text = null;
            devilText.text = null;
            tt.Show();
            _xem.Hide();
            _game1.Show();
            _game2.Hide();
            _game3.Hide();
            _next.Hide();
            _score.Show();
            _aniMask.Hide();
            InitSpine();
        }


        void InitSpine()
        {
            //game1 spine初始化
            for (int i = 0; i < _game1.transform.childCount; i++)
            {
                var ta = _game1.transform.GetChild(i).gameObject;
                ta.Show();
                if (2 < i)
                {
                    var child = _game1.transform.GetChild(i).GetChild(0).gameObject;
                    SpineManager.instance.DoAnimation(child, "kong", false,
                        () => { SpineManager.instance.DoAnimation(child, child.name, true); }
                    );
                }

                if (i == 0)
                {
                    SpineManager.instance.DoAnimation(_game1.transform.GetChild(i).gameObject,
                        _game1.transform.GetChild(i).name, true);
                }

                if (0 < i && i < 3)
                {
                    var temp = _game1.transform.GetChild(i).gameObject;
                    SpineManager.instance.DoAnimation(temp, temp.name + "1", true);
                }
            }

            //game2 spine初始化
            for (int i = 0; i < _game2.transform.childCount; i++)
            {
                var ta = _game2.transform.GetChild(i).gameObject;
                ta.Show();
                if (3 < i)
                {
                    var child = _game2.transform.GetChild(i).GetChild(0).gameObject;

                    SpineManager.instance.DoAnimation(child, "kong", false,
                        () => { SpineManager.instance.DoAnimation(child, child.name, true); });
                }

                if (i == 0)
                {
                    SpineManager.instance.DoAnimation(_game2.transform.GetChild(i).gameObject,
                        _game2.transform.GetChild(i).name, true);
                }

                if (0 < i && i < 4)
                {
                    var temp = _game2.transform.GetChild(i).gameObject;
                    SpineManager.instance.DoAnimation(temp, temp.name + "1", true);
                }
            }

            //game3 spine初始化
            for (int i = 0; i < _game3.transform.childCount; i++)
            {
                var ta = _game3.transform.GetChild(i).gameObject;
                ta.Show();
                if (4 < i)
                {
                    var child = _game3.transform.GetChild(i).GetChild(0).gameObject;

                    SpineManager.instance.DoAnimation(child, "kong", false,
                        () => { SpineManager.instance.DoAnimation(child, child.name, true); });
                }

                if (i == 0)
                {
                    SpineManager.instance.DoAnimation(_game3.transform.GetChild(i).gameObject,
                        _game3.transform.GetChild(i).name, true);
                }

                if (0 < i && i < 5)
                {
                    var temp = _game3.transform.GetChild(i).gameObject;
                    SpineManager.instance.DoAnimation(temp, temp.name + "1", true);
                }
            }
        }


        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);

            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
            tt.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 0,
                    () =>
                    {
                        //SpineManager.instance.DoAnimation(tt, "dj", true);
                        ShowDialogue("今天真是太开心了", ttText);
                    },
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1);
                        devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(dbd, SoundManager.SoundType.VOICE, 1,
                                () =>
                                {
                                    //SpineManager.instance.DoAnimation(devil, "dj", true);
                                    ShowDialogue("哼！我最讨厌你们开心了，我要夺走你们的心情胸章！", devilText);
                                },
                                () =>
                                {
                                   
                                }));
                        });
                    }));
            });
        }
        
        void TDGameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            tt.transform.DOMove(devilEndPos.position, 1f).OnComplete(() =>
            {
                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 0, 
                    () =>
                    {
                        ShowDialogue("今天真是太开心了", ttText);
                    }, 
                    () =>
                    {
                        devil.transform.DOMove(bdEndPos.position, 1f).OnComplete(() =>
                        {
                            mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 1, () =>
                            {
                                ShowDialogue("哼！我最讨厌你们开心了，我要夺走你们的心情胸章！", devilText);
                            }, () =>
                            {
                                tt.Hide();
                                devil.Hide();
                                bd.Show();
                                mono.StartCoroutine(SpeckerCoroutine(bd, SoundManager.SoundType.VOICE, 2,
                                    () => { },
                                    () => { SoundManager.instance.ShowVoiceBtn(true); }));
                            }));
                        });
                    }));
            });
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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex,
            Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = dbd;
            }

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
                //点击标志位
                flag = 0;

                bd.Hide();
                _tt.Show();
                mono.StartCoroutine(SpeckerCoroutine(_tt, SoundManager.SoundType.VOICE, 3, null, () =>
                {
                    mask.SetActive(false);
                    _tt.SetActive(false);
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
                            SpineManager.instance.DoAnimation(anyBtns.GetChild(1).gameObject, "ok2");
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            ac?.Invoke();
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
        private void BtnPlaySoundFail(Action method = null)
        {
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 5);
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
            Wait(time, method);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess(Action method = null)
        {
            //SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4);
            var time = SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
            Wait(time, method);
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitFoSencondsDo(time, method));
        }

        IEnumerator WaitFoSencondsDo(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
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
    }
}