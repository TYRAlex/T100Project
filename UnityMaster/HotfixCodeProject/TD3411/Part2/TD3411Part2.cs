using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DG.Tweening;
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

    public class TD3411Part2
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


        private GameObject _pages;
        private GameObject _page1;
        private List<GameObject> _materialsList;
        private GameObject _aniOfPage1;

        private List<string> _touchIndex;

        private GameObject _page2;
        private List<GameObject> _pictures;
        private GameObject _aniMask;

        private List<GameObject> _pageList;

        bool isPlaying;
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

            //---------------------------------------------------
            _pages = curTrans.GetGameObject("Parent/Page");
            _page1 = _pages.transform.GetGameObject("p1");
            _aniOfPage1 = _page1.transform.GetGameObject("ani");
            _page2 = _pages.transform.GetGameObject("p2");

            _materialsList = new List<GameObject>();
            _pageList = new List<GameObject>();
            _aniMask = curTrans.GetGameObject("Parent/aniMask");
            _touchIndex = new List<string>();

            for (int i = 0; i < _pages.transform.childCount; i++)
            {
                if (i < 4)
                    _pageList.Add(_pages.transform.GetChild(i).gameObject);
            }

            for (int i = 0; i < _page1.transform.childCount; i++)
            {
                var go = _page1.transform.GetChild(i).gameObject;
                _materialsList.Add(go);
                _touchIndex.Add(go.name);
                Util.AddBtnClick(go, OnClickMaterial);
            }

            _pictures = new List<GameObject>();
            for (int i = 0; i < _page2.transform.childCount; i++)
            {
                var go = _page2.transform.GetChild(i).gameObject;
                _pictures.Add(go);
                _touchIndex.Add(go.name);
                Util.AddBtnClick(go, OnclickPictures);
            }



            PointerClickListener.Get(_aniMask).onClick = null;

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            GameInit();
            GameStart();
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

        //初始化
        private void GameInit()
        {
            talkIndex = 1;
            isPlaying = false;
            InitPage(_pageList);
            SpineManager.instance.DoAnimation(_aniOfPage1, "0");
            _aniMask.Hide();
            dd.SetActive(false);
            mask.SetActive(false);
            for (int i = 0; i < _pictures.Count; i++)
            {
                _pictures[i].Show();
                PictureSpineInit(_pictures[i]);
            }

        }

        void GameStart()
        {
            _aniMask.Show();
            mask.Show();
            dd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0, () => { },
                () =>
                {
                    dd.SetActive(false);
                    mask.SetActive(false);
                    _aniMask.Hide();
                }));
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
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
                NextPage(_pageList);
            }

            if (talkIndex >= 2)
            {
                dd.SetActive(true);
                mask.SetActive(true);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 5, () => { }, () => { }));
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


        void OnClickMaterial(GameObject go)
        {
            _aniMask.Show();
            BtnPlaySound();
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0);
            //_aniMask.Show();
            // Debug.Log(ReturnVoiceIndex(go, _materialsList));
            //var time=SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, ReturnVoiceIndex(go, _materialsList),false);
            // Wait(time,()=>{_aniMask.Hide();});
            for (int i = 0; i < _materialsList.Count; i++)
            {
                if (go == _materialsList[i])
                {
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, i, () =>
                    {
                        SpineManager.instance.DoAnimation(_aniOfPage1, go.name, false,
                         () =>
                         {
                             SpineManager.instance.DoAnimation(_aniOfPage1, "0", false, () => { });

                             IsTouchOver(go);
                         });
                    }, () => { _aniMask.Hide(); }));


                }
            }


        }


        void InitPage(List<GameObject> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                var child = list[i];
                child.transform.localPosition = new Vector3(i * 1920, 0);
            }
        }
        int ReturnVoiceIndex(GameObject go, List<GameObject> list)
        {
            var index = -1;
            for (int i = 0; i < list.Count; i++)
            {
                if (go == list[i])
                    index = i;
            }

            return index;
        }

        void IsTouchOver(GameObject go)
        {
            for (int i = 0; i < _touchIndex.Count; i++)
            {
                if (go.name == _touchIndex[i])
                {
                    // Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                    _touchIndex.Remove(go.name);
                }
            }

            //Debug.Log("Count:   "+_touchIndex.Count);
            if (_touchIndex.Count == 4)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
            if (_touchIndex.Count == 1)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }


        GameObject GetChild(GameObject gameObject)
        {
            return gameObject.transform.GetChild(0).gameObject;
        }

        private void OnclickPictures(GameObject go)
        {
            _aniMask.Show();
            var child = GetChild(go);
            BtnPlaySound();
            if (int.Parse(go.name) < 14)
            {
                BackSpineAni(child, _pictures);
                go.transform.SetAsLastSibling();
                SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1);
                SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
                //SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, int.Parse(go.name) - 10, false);
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, int.Parse(go.name) - 10, () => { }, () => { isPlaying = true; }));
                SpineManager.instance.DoAnimation(child, child.name, false,
                    () => { SpineManager.instance.DoAnimation(child, child.name + "4", false, () => { }); });
                Wait(0.1f, () => { });
            }
            // Debug.Log(_touchIndex.Count + "Count:   ");
        }


        //picture初始化
        void PictureSpineInit(GameObject go)
        {
            var child = GetChild(go);
            SpineManager.instance.DoAnimation(child, child.name + "3", false);
        }


        //翻页
        void NextPage(List<GameObject> list)
        {
            _aniMask.Show();
            for (int i = 0; i < list.Count; i++)
            {
                var pos = new Vector3(list[i].transform.position.x - Screen.width, list[i].transform.position.y);
                list[i].transform.DOMove(pos, 1f).OnComplete(() =>
                {
                    mask.Show();
                    dd.Show();
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 4, () => { }, () =>
                    {
                        _aniMask.Hide();
                        dd.SetActive(false);
                        mask.SetActive(false);
                    }));
                });
            }
        }


        //点击animask返回
        void BackSpineAni(GameObject gameObject, List<GameObject> list)
        {
            PointerClickListener.Get(_aniMask).onClick = go =>
            {
                if (isPlaying)
                {
                    isPlaying = false;
                    SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2);
                    SpineManager.instance.DoAnimation(gameObject, gameObject.name + "2", false,
                        () =>
                        {
                            SpineManager.instance.DoAnimation(gameObject, gameObject.name + "3", false,
                                () =>
                                {
                                    //PicturesShowOrHide(list, gameObject, true);
                                    IsTouchOver(gameObject.transform.parent.gameObject);
                                    _aniMask.Hide();
                                   
                                });
                        });
                    //Debug.Log("返回");

                }
                else
                {
                    return;
                }
            };
        }

        //三张图片的显示和隐藏


        IEnumerator WaitForSencesDoAction(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            method?.Invoke();
        }

        void Wait(float time, Action method = null)
        {
            mono.StartCoroutine(WaitForSencesDoAction(time, method));
        }
    }
}