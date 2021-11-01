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

    public class TD6712Part2
    {
        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bd;
        private GameObject Bg;
        private BellSprites bellTextures;


        private GameObject successSpine;
        private GameObject caidaiSpine;
        private GameObject mask;

        private Empty4Raycast[] e4rs;

        private GameObject rightBtn;
        private GameObject leftBtn;

        private GameObject btnBack;

        private int curPageIndex; //当前页签索引
        private Vector2 _prePressPos;

        private float textSpeed;


        //胜利动画名字
        private string tz;
        private string sz;
        bool isPlaying = false;
        bool isPressBtn = false;

        //---------------------------------
        private GameObject _parent;
        private GameObject _pages;
        private List<GameObject> _pageList;
        private GameObject _page1;
        private GameObject _page2;
        private GameObject _aniOfPage1;
        private GameObject _aniOfPage2;
        private List<GameObject> _aniOfPage1List;
        private List<GameObject> _aniOfPage2List;
        private GameObject _aniMask;

        List<string> _nameList;
        GameObject _btnMask;
        private string aniSpineName;
        //---------------------------------


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
            bd.SetActive(true);
            tz = "3-5-z";
            sz = "6-12-z";
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            //--------------------------------
            _parent = curTrans.GetGameObject("Parent");
            var parentTrans = _parent.transform;
            _pages = parentTrans.GetGameObject("pages");
            _btnMask = parentTrans.GetGameObject("btnMask");
            _pageList = new List<GameObject>();
            _page1 = _pages.transform.GetGameObject("page1");
            _page2 = _pages.transform.GetGameObject("page2");
            _pageList.Add(_page1);
            _pageList.Add(_page2);
            aniSpineName = "DTyt";
            _aniOfPage1List = new List<GameObject>();
            _aniOfPage2List = new List<GameObject>();
            _nameList = new List<string>();

            for (int i = 0; i < _page1.transform.childCount - 1; i++)
            {
                var child = _page1.transform.GetChild(i).gameObject;
                _aniOfPage1List.Add(child);
                _nameList.Add(child.name);
                Util.AddBtnClick(child, AniOnClick);
            }

            for (int i = 0; i < _page2.transform.childCount - 1; i++)
            {
                var child = _page2.transform.GetChild(i).gameObject;
                _aniOfPage2List.Add(child);
                _nameList.Add(child.name);
                Util.AddBtnClick(child, ImgOnClick);
            }

            _aniOfPage1 = _page1.transform.GetGameObject("ani");
            _aniOfPage2 = _page2.transform.GetGameObject("ani");
            _aniMask = _pages.transform.GetGameObject("aniMask");

            DOTween.KillAll();
            //--------------------------------

            GameInit();
            GameStart();
        }


        private void GameInit()
        {
            Input.multiTouchEnabled = false;
            isPlaying = false;
            _btnMask.Hide();
            mask.Hide();
            talkIndex = 1;
            curPageIndex = 0;
            _aniMask.Hide();
            isPressBtn = false;
            textSpeed = 0.5f;
            SpinInit();
            _page1.transform.localPosition = Vector3.zero;
            _page2.transform.localPosition = new Vector3(1920, 0);
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
            if (_nameList.Count == 0 || _nameList.Count == 3)
            {
                SoundManager.instance.ShowVoiceBtn(true);
            }
        }
        void GameStart()
        {

            mask.Show();
            bd.Show();
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 6, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0, null, () =>
            {
                mask.Hide();
                bd.Hide();
                //SoundManager.instance.ShowVoiceBtn(true);

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
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
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
                _aniMask.Show();
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 6, () => { },
                       () =>
                       {
                           mask.Hide();
                           bd.Hide();
                           _aniMask.Hide();
                           NextPage(_pageList);
                       }));
            }

            if (talkIndex == 2)
            {
                _aniMask.Show();
                mask.Show();
                bd.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 10, () => { },
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
                            /* anyBtn.name = getBtnName(BtnEnum.fh);*/
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


        IEnumerator IEShowDialogue(string str, Text text, Action callBack = null)
        {
            int i = 0;
            str = str.Replace(" ", "\u00A0"); //空格非换行
            while (i <= str.Length - 1)
            {
                yield return new WaitForSeconds(textSpeed);
                text.text += str[i];
                i++;
            }

            callBack?.Invoke();
            yield break;
        }

        //初始化动画
        void SpinInit()
        {
            SpineManager.instance.DoAnimation(_aniOfPage1, aniSpineName + "17", true);
            SpineManager.instance.DoAnimation(_aniOfPage2, "abc", true);
        }

        //点击播放对应spine
        private void AniOnClick(GameObject obj)
        {

            _aniMask.Show();
            SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, int.Parse(obj.name), null, () => { IsOver(obj); _aniMask.Hide(); }));
            //SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, int.Parse(obj.name));
            if (obj.name == "5")
                SpineManager.instance.DoAnimation(_aniOfPage1, aniSpineName + "6", false);
            else
                SpineManager.instance.DoAnimation(_aniOfPage1, aniSpineName + obj.name, false);
        }

        GameObject temp;
        private void ImgOnClick(GameObject obj)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.SOUND);
            _aniMask.Show();
            _btnMask.Show();
            BackSpineAni(obj);
            temp = obj;
            //mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, int.Parse(obj.name), null, () => { isPlaying = true; }));
            switch (obj.name)
            {
                case "a":
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 7, null, () => { isPlaying = true; _btnMask.Hide(); }));
                    break;
                case "b":
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 8, null, () => { isPlaying = true; _btnMask.Hide(); }));
                    break;
                case "c":
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 9, null, () => { isPlaying = true; _btnMask.Hide(); }));
                    break;
            }



            SpineManager.instance.DoAnimation(_aniOfPage2, obj.name + "1", false, () => { });
        }

        //点击animask返回
        void BackSpineAni(GameObject gameObject)
        {
            PointerClickListener.Get(_aniMask).onClick = go =>
            {
                if (isPlaying)
                {


                    _aniMask.Hide();
                    _btnMask.Show();
                    SpineManager.instance.DoAnimation(_aniOfPage2, gameObject.name + "2", false,
                        () =>
                        {
                            isPlaying = false;
                            _btnMask.Hide();
                            IsOver(temp);
                        });

                }
                else return;
            };
        }


        //翻页
        void NextPage(List<GameObject> list)
        {

            for (int i = 0; i < list.Count; i++)
            {
                var pos = new Vector3(list[i].transform.position.x - Screen.width, list[i].transform.position.y);
                list[i].transform.DOMove(pos, 1f).OnComplete(() =>
                {



                });
            }
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
    }
}