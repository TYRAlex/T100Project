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

    public class TD3411Part4
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject di;
        private GameObject dd;
        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject anyBtn;
        private GameObject successSpine;
        private GameObject mask;

        bool isPlaying;

        //=================================================
        private GameObject _parent;
        private List<GameObject> _pictures;
        private GameObject _aniMask;
        List<string> _nameList;

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

            di = curTrans.Find("di").gameObject;
            di.SetActive(false);
            dd = curTrans.Find("mask/DD").gameObject;
            dd.SetActive(true);
            successSpine = curTrans.Find("mask/successSpine").gameObject;
            successSpine.SetActive(false);
 

            anyBtn = curTrans.Find("mask/Btn").gameObject;
            anyBtn.SetActive(false);
            //anyBtn.name = getBtnName(BtnEnum.bf);
            Util.AddBtnClick(anyBtn, OnClickAnyBtn);


            //==============================================
            _parent = curTrans.transform.GetGameObject("Parent");
            _pictures = new List<GameObject>();
            _nameList = new List<string>();
            _aniMask = curTrans.transform.GetGameObject("aniMask");
            PointerClickListener.Get(_aniMask).onClick = null;
            for (int i = 0; i < _parent.transform.childCount; i++)
            {
                var child = _parent.transform.GetChild(i).gameObject;
                _pictures.Add(child);
                _nameList.Add(child.name);
                Util.AddBtnClick(child, PlaySpine);
            }


            //=============================================================
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
        }
        void IsOver(GameObject obj)
        {
            for (int i = 0; i < _nameList.Count; i++)
            {
                if (obj.name == _nameList[i])
                {
                    _nameList.Remove(obj.name);
                }
            }
            if (_nameList.Count == 0)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
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

            SpineManager.instance.DoAnimation(anyBtn, result + "2", false);
            return result;
        }

        private void OnClickAnyBtn(GameObject obj)
        {
            BtnPlaySound();
            SpineManager.instance.DoAnimation(anyBtn, obj.name, false, () =>
            {
                if (obj.name == "bf")
                {
                    GameStart();
                }
                else
                {
                    GameInit();
                }

                mask.gameObject.SetActive(false);
            });
        }

        private void GameInit()
        {
            mask.Hide();
            dd.Hide();
            isPlaying = false;
            IninSpine(_pictures);
            _aniMask.Hide();
            dd.Hide();
            mask.Hide();
            talkIndex = 1;
        }

        void GameStart()
        {
            mask.Show();
            dd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2, () => { _aniMask.Show(); },
                () =>
                {
                    _aniMask.Hide();
                    mask.Hide();
                    dd.Hide();
                }));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 =
                null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            SpineManager.instance.DoAnimation(dd, "bd-daiji");
            yield return new WaitForSeconds(len);
            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(dd, "bd-speak");
            method_1?.Invoke();
            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(dd, "bd-daiji");
            SoundManager.instance.SetShield(true);
            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                mask.Show();
                dd.Show();

                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { _aniMask.Show(); },
                    () =>
                    {
                        _aniMask.Hide();
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
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 3, false);
            SpineManager.instance.DoAnimation(successSpine, "1", false, () =>
            {
                SpineManager.instance.DoAnimation(successSpine, "2", false, () =>
                {
                    /* anyBtn.name = getBtnName(BtnEnum.fh);*/
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

        private void PlaySpine(GameObject obj)
        {
            _aniMask.Show();

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);

            var child = obj.transform.GetChild(0).gameObject;
            // HideOtherSpine(obj, _pictures, false);
            obj.transform.SetAsLastSibling();
           // SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, int.Parse(obj.name), false);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, int.Parse(obj.name), null,()=> { isPlaying = true; }));
            SpineManager.instance.DoAnimation(child, child.name, false,
                () =>
                {
                    SpineManager.instance.DoAnimation(child, child.name + "4", false,
                        () => { BackSpineAni(child, _pictures); });
                  
                });
        }


        void HideOtherSpine(GameObject obj, List<GameObject> list, bool show)
        {
            if (show)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Show();
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    if (obj != list[i])
                    {
                        list[i].Hide();
                    }
                }
            }
        }


        void BackSpineAni(GameObject gameObject, List<GameObject> list)
        {
            PointerClickListener.Get(_aniMask).onClick = go =>
            {
                if(isPlaying)
                {
                    isPlaying = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 4, false);
                SpineManager.instance.DoAnimation(gameObject, gameObject.name + "2", false,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(gameObject, gameObject.name + "3", false,
                            () =>
                            {
                               
                                IsOver(gameObject.transform.parent.gameObject);
                                _aniMask.Hide();
                            });
                    });
                Debug.Log("返回");

                }
                else
                {
                    return;
                }
            };
        }

        int GetVoiceIndex(GameObject obj, List<GameObject> list)
        {
            var index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (obj == list[i])
                    index = i;
            }

            return index;
        }

        IEnumerator WaitForSencesDoAction(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSencesDoAction(time, method));
        }


        void IninSpine(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].Show();
                var child = list[i].transform.GetChild(0).gameObject;
                SpineManager.instance.DoAnimation(child, child.name + "3", true);
            }
        }
    }
}