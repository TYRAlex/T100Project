using DG.Tweening;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Video;

namespace ILFramework.HotClass
{
    public class CourseTDG3P1L10Part1
    {
        int paintIndex, toolIndex;
        bool isClickPaint, isClickTool, isPause;
        string[] uiSpineStr, paintSkinStr, toolSkinStr;
        Vector3 videoPos;
        MonoBehaviour mono;
        VideoPlayer videoPlayer;
        GameObject curGo, videoPlayerObj, videoFrameImg, prePaintBtn, preToolBtn, finishBtn, pauseBtn, npc;
        GameObject[] uiPaint, uiTool;       

        enum UISPINE
        {
            Idle0 = 0, Idle1 = 1, Right = 2, Error = 3
        }

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;

            uiPaint = curTrans.GetChildren(curTrans.Find("GameScene/uiPaint").gameObject);
            uiTool = curTrans.GetChildren(curTrans.Find("GameScene/uiTool").gameObject);
            videoPlayerObj = curTrans.Find("GameScene/videoPlayer").gameObject;
            videoFrameImg = videoPlayerObj.transform.Find("frameImg").gameObject;
            npc = curTrans.Find("npc").gameObject;
            pauseBtn = curTrans.Find("GameScene/videoPlayer/pause").gameObject;
            uiSpineStr = new string[] {"idle", "idle2", "click_right", "click_error"};
            paintSkinStr = new string[] { "bs1", "dh_1", "gd_1", "ps_1"};
            toolSkinStr = new string[] { "bs_2", "dh_2",  "gd_2", "ps_2"};
            videoPlayer = videoPlayerObj.GetComponent<VideoPlayer>();
            mono = curTrans.GetComponent<MonoBehaviour>();

            GameInit();
            GameStart();
        }

        void GameInit()
        {
            paintIndex = 5;
            toolIndex = 5;
            isClickPaint = false;
            isClickTool = false;
            videoFrameImg.SetActive(false);
            videoPlayerObj.SetActive(false);
            videoPlayerObj.transform.localScale = Vector3.zero;
            prePaintBtn = null;
            preToolBtn = null;
            finishBtn = null;
            videoPos = Vector3.zero;
            for (int i = 0; i < uiPaint.Length; i++)
            {
                int index = i;
                SkeletonGraphic spine = uiPaint[i].transform.GetChild(0).GetComponent<SkeletonGraphic>();
                spine.initialSkinName = paintSkinStr[i];
                spine.Initialize(true);
                SpineManager.instance.DoAnimation(spine.gameObject, uiSpineStr[(int)UISPINE.Idle0],false, () =>
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, uiSpineStr[(int)UISPINE.Idle1], true);                   
                });                
            }
            for (int i = 0; i < uiTool.Length; i++)
            {
                int index = i;
                SkeletonGraphic spine = uiTool[i].transform.GetChild(0).GetComponent<SkeletonGraphic>();
                spine.initialSkinName = toolSkinStr[i];
                spine.Initialize(true);
                SpineManager.instance.DoAnimation(spine.gameObject, uiSpineStr[(int)UISPINE.Idle0], false, () =>
                {
                    SpineManager.instance.DoAnimation(spine.gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                });
            }
        }

        void GameStart()
        {
            for (int i = 0; i < uiPaint.Length; i++)
            {
                Util.AddBtnClick(uiPaint[i], ClickPaint);
                Util.AddBtnClick(uiTool[i], ClickTool);
            }
            Util.AddBtnClick(videoPlayerObj, ClickBackground);
            Util.AddBtnClick(pauseBtn, PauseVideo);
            SoundManager.instance.Stop();
            SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM);

            SoundManager.instance.Speaking(npc, "talk", SoundManager.SoundType.VOICE, 0);

        }

        void ClickPaint(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            int index = Convert.ToInt32(btn.name);
            finishBtn = btn;
            if (isClickTool)
            {
                float time;
                if (index == toolIndex)
                {                   
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    time = SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Right], false, () =>
                    {
                        ClickRight(index, btn, preToolBtn);                        
                        //SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                        //SpineManager.instance.DoAnimation(preToolBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    });                    
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    time =SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Error], false, () =>
                    {
                        SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                        //SoundManager.instance.sheildGo.SetActive(false);
                    });
                    SpineManager.instance.DoAnimation(preToolBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Error], false, () =>
                    {
                        SpineManager.instance.DoAnimation(preToolBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    });
                }
                //if (prePaintBtn != null)
                //{
                //    SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], false);
                //}
                mono.StartCoroutine(WaitTime(time * 2f, () => {
                    paintIndex = 5;
                    toolIndex = 5;
                    prePaintBtn = null;
                    preToolBtn = null;
                    isClickPaint = false;
                    isClickTool = false;
                    SoundManager.instance.sheildGo.SetActive(false);
                }));
            }
            else
            {
                if (index != paintIndex)
                {
                    isClickPaint = true;
                    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Right], false, () =>
                    {
                        SoundManager.instance.sheildGo.SetActive(false);
                    });
                    if (prePaintBtn != null)
                    {
                        SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    }
                }
                else
                {
                    isClickPaint = false;
                    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    SoundManager.instance.sheildGo.SetActive(false);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Debug.Log("PaintIndex:" + index);
                prePaintBtn = btn;
                paintIndex = index;
            }                                      
        }

        void ClickTool(GameObject btn)
        {
            SoundManager.instance.sheildGo.SetActive(true);
            int index = Convert.ToInt32(btn.name);
            finishBtn = btn;
            if (isClickPaint)
            {
                float time;
                if (index == paintIndex)
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 1, false);
                    time = SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Right], false, () =>
                    {
                        ClickRight(index, btn, prePaintBtn);
                        //SoundManager.instance.sheildGo.SetActive(false);
                        //SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                        //SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    });                   
                }
                else
                {
                    SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                    time = SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Error], false, () =>
                    {
                        SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                        //SoundManager.instance.sheildGo.SetActive(false);
                    });
                    SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Error], false, () =>
                    {
                        SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    });
                }
                //if (prePaintBtn != null)
                //{
                //    SpineManager.instance.DoAnimation(prePaintBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], false);
                //}
                mono.StartCoroutine(WaitTime(time * 2f, () => {
                    paintIndex = 5;
                    toolIndex = 5;
                    prePaintBtn = null;
                    preToolBtn = null;
                    isClickPaint = false;
                    isClickTool = false;
                    SoundManager.instance.sheildGo.SetActive(false);
                }));
            }
            else
            {
                if (index != toolIndex)
                {
                    isClickTool = true;
                    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Right], false, () =>
                    {
                        SoundManager.instance.sheildGo.SetActive(false);
                    });
                    if (preToolBtn != null)
                    {
                        SpineManager.instance.DoAnimation(preToolBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    }                   
                }
                else
                {
                    isClickTool = false;
                    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    SoundManager.instance.sheildGo.SetActive(false);
                }
                SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                Debug.Log("ToolIndex:" + index);
                preToolBtn = btn;
                toolIndex = index;
            }            
        }

        IEnumerator WaitTime(float time, Action act)
        {
            yield return new WaitForSeconds(time);
           
            act();
            mono.StopCoroutine(WaitTime(time, act));
        }

        void ClickRight(int index, GameObject btn, GameObject preBtn)
        {
            SoundManager.instance.StopAudio(SoundManager.SoundType.BGM);
            //videoPlayerObj.SetActive(true);
            //videoPlayer.url = " ";
            videoPlayerObj.transform.position = finishBtn.transform.position;
            videoPlayer.url = GetVideoPath(index);
            Debug.LogWarning("GetVideoPath(index):" + GetVideoPath(index));
            videoPlayerObj.SetActive(true);
            videoFrameImg.SetActive(true);
            mono.StartCoroutine(WaitTime(0.5f, () =>
            {
                videoPlayerObj.transform.DOLocalMove(videoPos, 0.5f);
                videoPlayerObj.transform.DOScale(1f, 0.5f).SetEase(Ease.InOutBack).OnComplete(() =>
                {
                    isPause = false;
                    videoFrameImg.SetActive(false);
                    videoPlayer.Play();
                    //SoundManager.instance.sheildGo.SetActive(false);
                    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                    SpineManager.instance.DoAnimation(preBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
                });
            }));

            //LogicManager.instance.ShowVideo(true);
            //LogicManager.instance.PlayVideo(index.ToString() + ".mp4", true);

            //mono.StartCoroutine(WaitTime(13f, () =>
            //{
            //    LogicManager.instance.ShowVideo(false);
            //    SpineManager.instance.DoAnimation(btn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
            //    SpineManager.instance.DoAnimation(preBtn.transform.GetChild(0).gameObject, uiSpineStr[(int)UISPINE.Idle1], true);
            //}));
        }

        void PauseVideo(GameObject btn)
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
            if (isPause)
            {
                videoPlayer.Play();
                isPause = false;
            }
            else
            {
                videoPlayer.Pause();
                isPause = true;
            }            
        }

        string GetVideoPath(int videoIndex)
        {            
            string url = LogicManager.instance.GetVideoPath(videoIndex.ToString() + ".mp4");
            return url;
        }

        void ClickBackground(GameObject btn)
        {
            //videoPlayer.url = " ";
            videoPlayer.url = GetVideoPath(4);
            //videoPlayer.Play();
            videoPlayerObj.transform.DOMove(finishBtn.transform.position, 0.5f);
            videoPlayerObj.transform.DOScale(0f, 0.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                videoPlayerObj.SetActive(false);
                //videoPlayer.url = " ";
                SoundManager.instance.BgSoundPart2(SoundManager.SoundType.BGM);                
                //videos[index].GetComponent<VideoPlayer>().Play();
                //LogicManager.instance.ShowVideo(true);
                //LogicManager.instance.PlayVideo(index.ToString() + ".mp4", true);
            });
            //LogicManager.instance.ShowVideo(false);
        }

    }
}
