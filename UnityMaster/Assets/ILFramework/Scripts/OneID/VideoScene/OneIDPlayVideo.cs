using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

namespace OneID
{
    public class OneIDPlayVideo : MonoBehaviour
    {
        public VideoPlayer OneIDVideoPlayer;
        
        // private void Update()
        // {
        //     if (Input.GetMouseButtonDown(0))
        //     {
        //         OneIDVideoPlayer.clip = Resources.Load<VideoClip>("Video/5.6.2.3《蚕豆小老虎》_创作指引视频01_onekeybatch");
        //         OneIDVideoPlayer.Play();
        //     }
        // }

        private void OnEnable()
        {
            //this.GetComponent<VideoPlayer>().Play();
        }

        private void OnDisable()
        {
            this.GetComponent<VideoPlayer>().Stop();
        }
    }
}