using System.Collections;
using LuaInterface;
using UnityEngine;
using System;
using UnityEngine.Video;
using UnityEngine.UI;

namespace ILFramework
{

    public class LogicManager : Manager<LogicManager>
    {
        // replay
        public GameObject replayBtn;
        Action replayAction;
        LuaFunction replayLuaFunc;
        [HideInInspector]
        public GameObject shiled;

        // video
        public VideoPlayer videoPlayer;
        public RawImage videoImage;
        bool isPlaying = true;
        /*视频是否播放完成*/
        public bool isPlayEnd = false;
        private void Start()
        {
            replayBtn.SetActive(false);
            Util.AddBtnClick(replayBtn, (g) =>
            {
                if (shiled != null && shiled.activeSelf)
                    return;
                //replayBtn.SetActive(false);
                replayAction?.Invoke();
                if (replayLuaFunc != null)
                    replayLuaFunc.Call();
            });
            //videoPlayer.gameObject.SetActive(false);
        }

        public void SetReplayEvent(Action ac)
        {
            replayAction = ac;
        }

        public void ShowReplayBtn(bool show)
        {
            replayBtn.SetActive(show);
        }

        /// <summary>
        /// 兼容1.0 设置重玩按钮事件
        /// </summary>
        public void SetReplayEvent(LuaFunction luaCall)
        {
            replayLuaFunc = luaCall;
        }

        /// <summary>
        /// 获取视频地址
        /// </summary>
        /// <param name="videoName"> 视频名称 带后缀名 </param>
        /// <returns></returns>
        public string GetVideoPath(string videoName)
        {
            string result = "";
#if UNITY_EDITOR
            result = "Assets/HotFixPackage/" + HotfixManager.instance.curShowPackage.Name + "/Videos/" + videoName;
#elif UNITY_ANDROID
            if (HotfixManager.instance.CurrentDynamicPath != string.Empty)
            {
                result="file://" + HotfixManager.instance.CurrentDynamicPath+ "/"+ HotfixManager.instance.curShowPackage.CourseName + "/" + videoName;
            }
            else
            {
                Debug.LogError("路径为空，请检查！");
            }

#else
            result = AppConst.hotfix_dir + "/" + HotfixManager.instance.curShowPackage.CourseName + "/" + videoName;
#endif
            return result;
        }

        public void InstiateVideoPlayer(bool isClass = false)
        {
            if (videoPlayer == null)
            {
                string packageName = "";

                GameObject v = ResourceManager.instance.LoadCommonResAB<GameObject>("VideoPlayer");
                if (isClass)
                {
                    packageName = HotfixManager.instance.curShowPackage.Name.Replace("/", "");
                }
                else
                {
                    packageName = "MainCanvas";
                }
                GameObject video = Instantiate(v, GameObject.Find(packageName).transform);

                videoPlayer = video.GetComponent<VideoPlayer>();
                videoImage = video.GetComponentInChildren<RawImage>();
            }
        }

        public void PlayVideo(string videoName)
        {
            InstiateVideoPlayer();
            Debug.LogFormat(" video url : {0} ", videoName);
            videoPlayer.url = GetVideoPath(videoName);
            videoPlayer.playOnAwake = false;
            videoPlayer.prepareCompleted += (videoSource) => { videoPlayer.Play(); };
            videoPlayer.Prepare();
        }

        public void PlayVideo(string videoName, bool isClass = false, bool isloop = false, Action ac = null)
        {
            InstiateVideoPlayer(isClass);
            Debug.LogFormat(" video url : {0} ", videoName);
            videoPlayer.url = GetVideoPath(videoName);
            videoPlayer.playOnAwake = false;
            videoPlayer.isLooping = isloop;
            videoPlayer.prepareCompleted += (videoSource) => { videoPlayer.Play(); };
            videoPlayer.Prepare();
            videoPlayer.loopPointReached += (videoSource) => { ac?.Invoke(); };
        }


        private void Update()
        {
            if (videoPlayer)
            {
                if ((ulong)videoPlayer.frame >= videoPlayer.frameCount)
                {
                    isPlayEnd = true;
                    return;
                }
            }
        }

        public void StopVideo()
        {
            videoPlayer.Stop();
        }

        public void ShowVideo(bool isShow)
        {
            if (videoPlayer != null)
            {
                BellLoom.QueueActions((o) =>
                {
                    videoPlayer.gameObject.SetActive(isShow);
                    //videoImage.gameObject.SetActive(isShow);
                }, null);
            }
        }

        public void SetPlay(bool isPlay)
        {
            if (isPlay)
            {
                if (!isPlaying)
                {
                    if (!videoImage.gameObject.activeSelf)
                        videoImage.gameObject.SetActive(true);
                    videoPlayer.Play();
                }
            }
            else
                videoPlayer.Pause();
            isPlaying = isPlay;
        }

        /// <summary>
        /// 兼容1.0 videoManager Play
        /// </summary>
        /// <param name="course"></param>
        /// <param name="index"></param>
        public void Play(string course, int index)
        {
            string video = index + 1 + ".mp4";
            PlayVideo(video);
        }

        /// <summary>
        /// 兼容1.0 videoManager Stop
        /// </summary>
        public void Stop()
        {
            StopVideo();
        }

        /// <summary>
        /// 兼容1.0 videoManager Show
        /// </summary>
        /// <param name="show"></param>
        public void Show(bool show)
        {
            ShowVideo(show);
            videoImage.gameObject.SetActive(false);
        }

        /// <summary>
        /// 兼容1.0 videoManager setPlay
        /// </summary>
        /// <param name="play"></param>
        public void setPlay(bool play)
        {
            SetPlay(play);
        }
    }
}
