using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class Course843Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject yyBox;
        private GameObject wyBox;
        private bool _canClick;
        private mILDrager[] allMil;
        private Vector3 startPos;
        private float t;
        private float y;
        private mILDrager[] yymil;
        private mILDrager[] wumil;
        private GameObject qd;
        private GameObject cx;
        private bool _canqd;
        private bool _cancx;
        private GameObject mask;
        private float x;
        private VideoPlayer _videoPlayer;
        private Transform _videos;
        private RawImage _rtImg;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;

            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            mask = curTrans.Find("mask").gameObject;
            qd = curTrans.Find("qd").gameObject;
            cx = curTrans.Find("cx").gameObject;
            yyBox = curTrans.Find("yyBox").gameObject;
            wyBox = curTrans.Find("wyBox").gameObject;
            allMil = new mILDrager[8];
            _videos = curTrans.Find("Videos");
            _videoPlayer = curTrans.Find("VideoPlayer").gameObject.GetComponent<VideoPlayer>();
            _videoPlayer.url = GetVideoPath("1.mp4");
            _rtImg = curTrans.GetRawImage("Videos/RTImg");

            yyBox.SetActive(true);
            wyBox.SetActive(true);

            for (int i = 1; i < 5; i++)
            {
                GameObject.Find(i.ToString()).transform.parent = yyBox.transform;
                GameObject.Find(i.ToString()).transform.SetSiblingIndex(i - 1);
                GameObject.Find(i.ToString()).transform.GetChild(0).localScale = new Vector3(1, 1, 1);
                GameObject.Find(i.ToString()).GetComponent<RectTransform>().sizeDelta = new Vector2(270f, 340f);
            }
            for (int i = 5; i < 9; i++)
            {
                GameObject.Find(i.ToString()).transform.parent = wyBox.transform;
                GameObject.Find(i.ToString()).transform.SetSiblingIndex(i - 1);
                GameObject.Find(i.ToString()).transform.GetChild(0).localScale = new Vector3(1, 1, 1);
                GameObject.Find(i.ToString()).GetComponent<RectTransform>().sizeDelta = new Vector2(270f, 340f);
            }
            for (int i = 0; i < yyBox.transform.childCount; i++)
            {
                allMil[i] = yyBox.transform.GetChild(i).GetComponent<mILDrager>();
                Util.AddBtnClick(yyBox.transform.GetChild(i).gameObject, ClickEvent);
            }
            for (int i = 0; i < wyBox.transform.childCount; i++)
            {
                allMil[i + 4] = wyBox.transform.GetChild(i).GetComponent<mILDrager>();
                Util.AddBtnClick(wyBox.transform.GetChild(i).gameObject, ClickEvent);
            }
            for (int i = 0; i < allMil.Length; i++)
            {
                allMil[i].SetDragCallback(BeginDragMoveEvent, null, DragEnd, null);
            }

            yymil = new mILDrager[4] { allMil[0], allMil[3], allMil[4], allMil[7] };
            wumil = new mILDrager[4] { allMil[1], allMil[2], allMil[5], allMil[6] };

            Util.AddBtnClick(qd, Check);
            Util.AddBtnClick(cx, ChaXun);
            GameInit();
            GameStart();
        }

        IEnumerator WaitTime(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        private void ChaXun(GameObject obj)
        {
            if (_cancx)
            {
                SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
                mono.StartCoroutine(PlayMp4());
                BtnPlaySound();
                _cancx = false;
                obj.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "ui1d", false);
                yyBox.SetActive(false);
                wyBox.SetActive(false);
                curTrans.Find("yy").gameObject.SetActive(false);
                curTrans.Find("wy").gameObject.SetActive(false);
                qd.SetActive(false);
                cx.SetActive(false);
                mono.StartCoroutine(WaitTime(66,
                    () =>
                    {
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
                        _rtImg.gameObject.SetActive(false);
                        _cancx = true;
                        yyBox.SetActive(true);
                        wyBox.SetActive(true);
                        curTrans.Find("yy").gameObject.SetActive(true);
                        curTrans.Find("wy").gameObject.SetActive(true);
                        qd.SetActive(true);
                        cx.SetActive(true);

                    }
                    ));
            }
        }



        private string GetVideoPath(string videoPath)
        {
            var path = LogicManager.instance.GetVideoPath(videoPath);
            return path;
        }

        IEnumerator PlayMp4()
        {
            _videoPlayer.Prepare();

            while (true)
            {
                if (!_videoPlayer.isPrepared)   //监听是否准备完毕。没有完成一直等待，完成后跳出循环，进行img赋值，让后播放                             
                    yield return null;
                else
                    break;
            }

            _rtImg.gameObject.SetActive(true);
            _rtImg.texture = _videoPlayer.texture;
            _videoPlayer.Play();

            StopCoroutines("PlayMp4");
        }

        private void StopCoroutines(string methodName)
        {
            mono.StopCoroutine(methodName);
        }

        private void Check(GameObject obj)
        {
            if (_canqd)
            {
                BtnPlaySound();
                _canqd = false;
                obj.transform.GetChild(0).gameObject.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "ui2d", false);
                for (int i = 0; i < 8; i++)
                {
                    if (allMil[i].gameObject.transform.localPosition.y > 45)
                    {
                        allMil[i].index = 0;
                    }
                    else { allMil[i].index = 1; }
                }

                for (int i = 0; i < 4; i++)
                {
                    if (yymil[i].index == 1)
                    {
                        SpineManager.instance.DoAnimation(yymil[i].gameObject.transform.GetChild(0).gameObject, "t" + yymil[i].gameObject.name + "c", false,
                          () =>
                          { _canqd = true; }
                          );
                    }
                    if (wumil[i].index == 0)
                    {
                        SpineManager.instance.DoAnimation(wumil[i].gameObject.transform.GetChild(0).gameObject, "t" + wumil[i].gameObject.name + "c", false,
                          () =>
                          { _canqd = true; }
                          );
                    }
                }
                ALLCheck();
            }
        }

        private void ALLCheck()
        {
            for (int i = 0; i < 4; i++)
            {
                if (yymil[i].index == 1)
                { return; }
                if (wumil[i].index == 0)
                { return; }
            }
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,1,false);
            curTrans.Find("cd").gameObject.SetActive(true);
            curTrans.Find("cd").gameObject.transform.SetAsLastSibling();
            mask.transform.SetAsLastSibling();
            mask.SetActive(true);
            _canClick = false;
            _canqd = false;
            SpineManager.instance.DoAnimation(curTrans.Find("cd").gameObject, "sp", false,
                () =>
                {
                    curTrans.Find("cd").gameObject.SetActive(false); 
                }
                );

        }
        private void ClickEvent(GameObject obj)
        {
            if (_canClick)
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND,0,false);
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "d" + obj.name, false,
                    () =>
                    { _canClick = true; }
                    );
            }
        }

        private void BeginDragMoveEvent(Vector3 dragPosition, int dragType, int dragIndex)
        {
            startPos = dragPosition;
            y = Input.mousePosition.y;
            yyBox.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
            wyBox.transform.GetComponent<HorizontalLayoutGroup>().enabled = false;
            allMil[dragType].gameObject.transform.SetAsLastSibling();
            allMil[dragType].gameObject.transform.parent.SetAsLastSibling();
        }
        private void DragEnd(Vector3 dragPosition, int dragType, int dragIndex, bool dragBool)
        {
            if (dragBool)
            {
                if (dragPosition.y > 45)
                {

                    if (y > 0.5 * Screen.height)
                    {
                        if (wyBox.transform.childCount > 0)
                        {
                            x = -1;
                            for (int i = 0; i < wyBox.transform.childCount; i++)
                            {
                                if (dragPosition.x > wyBox.transform.GetChild(i).localPosition.x)
                                {
                                    x = i;
                                }
                                
                            }
                        }
                        allMil[dragType].gameObject.transform.localPosition = startPos;
                    }
                    else
                    {
                        if (yyBox.transform.childCount > 0)
                        {
                            x = -1;
                            for (int i = 0; i < yyBox.transform.childCount; i++)
                            {
                                if (dragPosition.x > yyBox.transform.GetChild(i).localPosition.x)
                                {
                                    x = i;
                                }
                           
                            }
                        }
                        allMil[dragType].gameObject.transform.parent = yyBox.transform;
                        JugleScale(yyBox.transform.childCount);
                        for (int i = 0; i < yyBox.transform.childCount; i++)
                        {
                            yyBox.transform.GetChild(i).GetChild(0).localScale = new Vector3(1f * t, 1f * t, 1);
                            yyBox.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(270f * t, 340f * t);
                        }
                        JugleScale(wyBox.transform.childCount);
                        for (int i = 0; i < wyBox.transform.childCount; i++)
                        {
                            wyBox.transform.GetChild(i).GetChild(0).localScale = new Vector3(1f * t, 1f * t, 1);
                            wyBox.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(270f * t, 340f * t);
                        }
                    }

                }
                if (dragPosition.y < 45)
                {
                    if (y < 0.5 * Screen.height)
                    {
                        if (yyBox.transform.childCount > 0)
                        {
                            x = -1;
                            for (int i = 0; i < yyBox.transform.childCount; i++)
                            {
                                if (dragPosition.x > yyBox.transform.GetChild(i).localPosition.x)
                                {
                                    x = i;
                                }
                            }
                        }
                        allMil[dragType].gameObject.transform.localPosition = startPos;
                    }
                    else
                    {
                        if (wyBox.transform.childCount > 0)
                        {
                            x = -1;
                            for (int i = 0; i < wyBox.transform.childCount; i++)
                            {
                                if (dragPosition.x > wyBox.transform.GetChild(i).localPosition.x)
                                {
                                    x = i;
                                }

                            }
                        }
                        allMil[dragType].gameObject.transform.parent = wyBox.transform;
                        JugleScale(wyBox.transform.childCount);
                        for (int i = 0; i < wyBox.transform.childCount; i++)
                        {
                            wyBox.transform.GetChild(i).GetChild(0).localScale = new Vector3(1f * t, 1f * t, 1);
                            wyBox.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(270f * t, 340f * t);
                        }
                        JugleScale(yyBox.transform.childCount);
                        for (int i = 0; i < yyBox.transform.childCount; i++)
                        {
                            yyBox.transform.GetChild(i).GetChild(0).localScale = new Vector3(1f * t, 1f * t, 1);
                            yyBox.transform.GetChild(i).GetComponent<RectTransform>().sizeDelta = new Vector2(270f * t, 340f * t);
                        }
                    }
                }
            }
            else
            {
                allMil[dragType].gameObject.transform.localPosition = startPos;
            }


            allMil[dragType].gameObject.transform.SetSiblingIndex(Convert.ToInt32(x + 1));
            yyBox.transform.GetComponent<HorizontalLayoutGroup>().enabled = true;
            wyBox.transform.GetComponent<HorizontalLayoutGroup>().enabled = true;
        }

        private float JugleScale(int number)
        {
            switch (number)
            {
                case 0:
                    t = 0;
                    break;
                case 1:
                    t = 1.20f;
                    break;
                case 2:
                    t = 1.15f;
                    break;
                case 3:
                    t = 1.05f;
                    break;
                case 4:
                    t = 1f;
                    break;
                case 5:
                    t = 0.8f;
                    break;
                case 6:
                    t = 0.6f;
                    break;
                case 7:
                    t = 0.5f;
                    break;
                case 8:
                    t = 0.4f;
                    break;
            }
            return t;
        }

        private void GameInit()
        {
            cx.transform.GetChild(0).GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Max.transform.SetAsLastSibling();
            mask.transform.SetAsLastSibling();
            _rtImg.gameObject.SetActive(false);
            mask.SetActive(true);
            _canqd = false;
            _canClick = false;
            talkIndex = 1;
            _cancx = false;
            yyBox.SetActive(true);
            wyBox.SetActive(true);
            curTrans.Find("yy").gameObject.SetActive(true);
            curTrans.Find("wy").gameObject.SetActive(true);
            qd.SetActive(true);
            cx.SetActive(true);
            SpineManager.instance.DoAnimation(cx.transform.GetChild(0).gameObject,"ui1",false);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;mask.SetActive(false);_canqd = true; _canClick = true; _cancx = true; }));

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
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {

            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }


    }
}
