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

    public class TD6712Part5
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

        //--------------------------------------------------------------------
        private GameObject _parent;

        private List<GameObject> _spinesList;
        private GameObject _score;
        private BellSprites _numberSprites;
        private GameObject _dg;
        private ILDrager _dgDrager;
        private bool _canClick;
        private int level;
        private int truePosIndex;
        private GameObject _sx;
        private int score;
        GameObject _dbd;
        string name;
        private GameObject _aniMask;

        //--------------------------------------------------------------------
        void Start(object o)
        {
            curGo = (GameObject) o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();

            mask = curTrans.Find("mask").gameObject;
            //mask.SetActive(false);


            bd = curTrans.Find("mask/BD").gameObject;
            //bd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
            caidaiSpine = curTrans.Find("mask/caidaiSpine").gameObject;
            caidaiSpine.SetActive(false);


            bfBtn = curTrans.Find("mask/bf").gameObject;
            okBtn = curTrans.Find("mask/ok").gameObject;
            fhBtn = curTrans.Find("mask/fh").gameObject;
            bfBtn.SetActive(false);
            Util.AddBtnClick(bfBtn, OnClickAnyBtn);
            Util.AddBtnClick(okBtn, OnClickAnyBtn);
            Util.AddBtnClick(fhBtn, OnClickAnyBtn);

            tz = "3-5-z";
            sz = "6-12-z";

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //-----------------------------------------------------------------------
            _parent = curTrans.GetGameObject("parent");
            var _parentTrans = _parent.transform;
            _aniMask = _parentTrans.GetGameObject("animask");

            _spinesList = new List<GameObject>();
            for (int i = 0; i < 5; i++)
            {
                var child = _parentTrans.GetChild(i).gameObject;
                _spinesList.Add(child);
            }

            _dbd = curTrans.Find("mask/dbd").gameObject;
            _dbd.Hide();
            name = "em";
            _score = _parentTrans.GetGameObject("score");
            _numberSprites = _score.transform.GetGameObject("number").GetComponent<BellSprites>();
            _dg = _parentTrans.GetGameObject("dg");
            _sx = _dg.transform.GetGameObject("sx");
            _dgDrager = _dg.GetComponent<ILDrager>();
            _dgDrager.SetDragCallback(null, Draging, EndDrage, Shoot);
            _canClick = false;
            level = -1;
            score = 0;
            truePosIndex = 0;
            //-----------------------------------------------------------------------
            GameInit();
            IsStart();
        }

        void IsStart()
        {
            mask.Show();
            bfBtn.Show();
            okBtn.Hide();
            fhBtn.Hide();
            bd.Hide();
        }

        private void Draging(Vector3 arg1, int arg2, int arg3)
        {
            _canClick = false;
            level = -1;
        }

        private void Shoot(int obj)
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                IsSuccess();
                _canClick = false;
            }
        }

        private void EndDrage(Vector3 endPos, int type, int index, bool arg4)
        {
            var x = _dg.transform.localPosition.x;
            if (x <= -440)
            {
                level = 3;
            }

            if (-440 < x && x <= -100)
            {
                level = 2;
            }

            if (-100 < x && x <= 300)
            {
                level = 1;
            }

            if (x > 300)
            {
                level = 0;
            }


            _canClick = true;
        }


        void IsSuccess()
        {
            _aniMask.Show();
            if (score == 0)
            {
                PlaySpine("dg1", "dg6", () => { Sencechange("dg2", 3); });
            }

            if (score == 1)
            {
                PlaySpine("dg2", "dg7", () => { Sencechange("dg3", 2); });
            }

            if (score == 2)
            {
                PlaySpine("dg3", "dg8", () => { Sencechange("dg4", 0); });
            }

            if (score == 3)
            {
                PlaySpine("dg4", "dg9", () => { Sencechange("dg5", 3); });
            }

            if (score == 4)
            {
                PlaySpine("dg5", "dg10", () => { playSuccessSpine(); });
            }
        }


        void PlaySpine(string dgStart, string dgShoot, Action next = null)
        {
            SpineManager.instance.DoAnimation(_dg.transform.GetGameObject("dg"), dgShoot, false);
            _sx.Show();
            Wait(0.9f, () =>
            {
                SpineManager.instance.DoAnimation(_sx, "jgsx", false, () =>
                {
                    var initname = ReturnSpine(score, level).name;
                    var spinename = int.Parse(ReturnSpine(score, this.level).name) + 5;
                    _sx.Hide();

                    if (level == truePosIndex)
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 10), false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);

                        SpineManager.instance.DoAnimation(ReturnSpine(score, this.level),
                            name + "sj", false);
                        score++;
                        _score.transform.GetGameObject("number").GetComponent<Image>().sprite =
                            _numberSprites.sprites[score];
                        _score.transform.GetGameObject("number").GetComponent<Image>().SetNativeSize();
                        Wait(2f, () =>
                        {
                            next?.Invoke();
                            _aniMask.Hide();
                        });
                    }
                    else
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
                        SpineManager.instance.DoAnimation(ReturnSpine(score, this.level),
                            name + spinename.ToString(), false,
                            () =>
                            {
                                SpineManager.instance.DoAnimation(_dg.transform.GetChild(0).gameObject, dgStart,
                                    false, () => { _aniMask.Hide();});

                                Debug.Log("InitName" + initname);
                                SpineManager.instance.DoAnimation(ReturnSpine(score, this.level),
                                    "em" + initname.ToString(), true,
                                    () => {  });
                            });
                    }
                });
            });
        }

        void Sencechange(string dgStart, int trueindex)
        {
            SpineManager.instance.DoAnimation(_dg.transform.GetGameObject("dg"), dgStart, true);
            InitSpine(score, false);
            truePosIndex = trueindex;
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
                    okBtn.Hide();
                    fhBtn.Hide();
                    bd.Hide();
                    _dbd.Show();
                    SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 4, true);
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, _dbd, 2, null,
                        () => { }));
                }
            });
        }

        private void GameInit()
        {
            talkIndex = 1;
            
            Input.multiTouchEnabled = false;
            truePosIndex = 0;
            level = -1;
            score = 0;
            mask.Hide();
            _aniMask.Hide();
            SpineManager.instance.DoAnimation(bfBtn, "bf2", true);
            SpineManager.instance.DoAnimation(fhBtn, "fh2", true);
            SpineManager.instance.DoAnimation(okBtn, "ok2", true);
            InitSpine(score, true);
            _score.transform.GetGameObject("number").GetComponent<Image>().sprite = _numberSprites.sprites[0];
            _score.transform.GetGameObject("number").GetComponent<Image>().SetNativeSize();
            _sx.Hide();
        }

        void GameStart()
        {
            mask.Show();
            bd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 1, null,
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
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, bd, 0, null,
                    () =>
                    {
                        bd.SetActive(false);
                        mask.SetActive(false);
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
            okBtn.Hide();
            fhBtn.Hide();
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
                            /* anyBtn.name = getBtnName(BtnEnum.fh);*/
                            caidaiSpine.SetActive(false);
                            successSpine.SetActive(false);
                            okBtn.Show();
                            fhBtn.Show();
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

        void InitSpine(int score, bool isStart)
        {
            for (int i = 0; i < _spinesList.Count; i++)
            {
                var child = _spinesList[i];
                if (i == score)
                {
                    child.Show();
                }
                else
                {
                    child.Hide();
                }

                for (int j = 0; j < child.transform.childCount; j++)
                {
                    var temp = child.transform.GetChild(j).gameObject;
                    Wait(Random.Range(0.1f,0.4f), () => {  SpineManager.instance.DoAnimation(temp, name + temp.name, true); });
                  
                }
            }

            if (isStart)
            {
                _dg.transform.localPosition = new Vector3(0, -540);
                SpineManager.instance.DoAnimation(_dg.transform.GetChild(0).gameObject, "dg1", true);
            }
        }


        IEnumerator WaitForSecondDoSomething(float time, Action methon = null)
        {
            yield return new WaitForSeconds(time);
            methon?.Invoke();
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSecondDoSomething(time, method));
        }


        GameObject ReturnSpine(int parentIndex, int childIndex)
        {
            GameObject result = null;
            for (int i = 0; i < _spinesList.Count; i++)
            {
                if (i == parentIndex)
                {
                    for (int j = 0; j < _spinesList[i].transform.childCount; j++)
                    {
                        if (j == childIndex)
                            result = _spinesList[i].transform.GetChild(j).gameObject;
                    }
                }
            }

            return result;
        }
    }
}