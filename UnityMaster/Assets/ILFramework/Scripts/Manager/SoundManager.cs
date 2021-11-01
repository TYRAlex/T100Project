using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.UI;
using UnityEngine;
using LuaInterface;
using Thirds.NewWayToPlayAudio.Encoder;

namespace ILFramework
{
    public class SoundManager : Manager<SoundManager>
    {
        public GameObject voiceBtn;
        public GameObject sheildGo;
        Action voiceEvent;
        Image voiceBtnImg;
        LuaFunction voiceCallBack; // 兼容1.0
        public GameObject sheild; // 兼容1.0语音屏蔽

        // 跳过语音
        public Action skipEvent;
        Coroutine clipCo = null;
        public GameObject skipBtn;
        public bool isShowSkipBtn;

        [HideInInspector]
        public string[] audiosPrefabName;
        public AudioSource bgmSource, voiceSource, soundSource;
        public enum SoundType { BGM, SOUND, VOICE, COMMONBGM, COMMONSOUND, COMMONVOICE };
        public Dictionary<SoundType, AudioClip[]> commonClips = new Dictionary<SoundType, AudioClip[]>();
        [HideInInspector]
        public AudioClip[] bebo1_commonClips; // 兼容1.0公共音频资源
        public Dictionary<string, Dictionary<SoundType, AudioClip[]>> audioClips = new Dictionary<string, Dictionary<SoundType, AudioClip[]>>();
        [HideInInspector]
        public SoundType[] soundTypes;

        private void Start()
        {
            audiosPrefabName = new string[] { "Bgm", "Sound", "Voice", "CommonRes_Bgm", "CommonRes_Sound", "CommonRes_Voice" };
            soundTypes = new SoundType[] { SoundType.BGM, SoundType.SOUND, SoundType.VOICE, SoundType.COMMONBGM, SoundType.COMMONSOUND, SoundType.COMMONVOICE };
            
            skipBtn.SetActive(false);
            ShowVoiceBtn(false);
            sheildGo.SetActive(false);
            var btn = voiceBtn.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                // 兼容1.0
                if (sheild != null && sheild.activeSelf)
                    return;

                ShowVoiceBtn(false);
                if (voiceCallBack != null)
                    voiceCallBack.Call();
                voiceEvent?.Invoke();
            });

            btn = skipBtn.GetComponent<Button>();
            btn.onClick.AddListener(() =>
            {
                //isShowSkipBtn = true;
                if (isShowSkipBtn)
                {
                    isShowSkipBtn = false;
                    //skipBtn.SetActive(false);
                    this.StartCoroutine(SkipBtnDisappear());
                    StopAudio(SoundType.SOUND);
                    StopAudio(SoundType.VOICE);
                    if (clipCo != null)
                    {
                        StopCoroutine(clipCo);
                        clipCo = null;
                    }
                    skipEvent?.Invoke();
                }
            });
        }

        IEnumerator SkipBtnDisappear()
        {
            skipBtn.transform.GetComponent<Image>().CrossFadeAlpha(0, 0.5f, true);
            yield return new WaitForSeconds(0.5f);
            skipBtn.SetActive(false);
        }

        public float PlayClip(SoundType stype, int sidx, bool loop = false)
        {

            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                // switch (stype)
                // {
                //     case SoundType.COMMONBGM:
                //     case SoundType.BGM:
                //         AndroidNativeAudioMgr.PlayLongAudio(stype, sidx.ToString(), loop);
                //         break;
                //     case SoundType.SOUND:
                //     case SoundType.VOICE:
                //     case SoundType.COMMONSOUND:
                //     case SoundType.COMMONVOICE:
                //         AndroidNativeAudioMgr.PlayShortAudio(stype, sidx.ToString(), this, loop);
                //         break;
                // }
                
                AndroidNativeAudioMgr.PlayLongAudio(stype, sidx.ToString(), loop);
                return AndroidNativeAudioMgr.GetAudioLength(
                    AndroidNativeAudioMgr.GetAudioMusicIDByName(sidx.ToString(), stype));
            }
            else
            {
                AudioClip c = null;
                AudioSource source = null;
                string curCourse = HotfixManager.instance.curShowPackage.CourseName;
                //Debug.LogFormat(" PlayClip curCourse : {0} , stype: {1} , index: {2}", curCourse, stype, sidx);

                switch (stype)
                {
                    case SoundType.COMMONBGM:
                        c = commonClips[stype][sidx];
                        source = bgmSource;
                        break;
                    case SoundType.BGM:
                        c = audioClips[curCourse][stype][sidx];
                        source = bgmSource;

                        break;
                    case SoundType.COMMONSOUND:
                        c = commonClips[stype][sidx];
                        source = soundSource;
                        break;
                    case SoundType.SOUND:
                        c = audioClips[curCourse][stype][sidx];
                        source = soundSource;
                        break;
                    case SoundType.COMMONVOICE:
                        c = commonClips[stype][sidx];
                        source = voiceSource;
                        break;
                    case SoundType.VOICE:
                        c = audioClips[curCourse][stype][sidx];
                        source = voiceSource;
                        break;
                    default:
                        Debug.LogError("音频类型SoundType错误, 请检查传入类型是否正确！");
                        break;
                }

                //c = audioClips[stype][sidx];
                if (c != null)
                {
                    //if (!loop)
                    //    source.PlayOneShot(c);
                    //else
                    //{
                    //    source.clip = c;
                    //    source.loop = loop;
                    //    source.Play();
                    //}

                    BellLoom.QueueActions((o) =>
                    {
                        source.pitch = Time.timeScale;
                        //Debug.LogFormat(" BellLoom PlayClip ----> curCourse : {0} , stype: {1} , index: {2}", curCourse, stype, sidx);
                       
                        if (!loop)
                            source.PlayOneShot(c);
                        else
                        {
                            source.clip = c;
                            source.loop = loop;
                            source.Play();
                        }
                        
                    }, null);
                    
                    return c.length;
                }
                else
                {
                    Debug.LogFormat("播放音频错误：类型{0}，索引为{1}的语音为空！", stype.ToString(), sidx);
                    return 0;
                }
            }
        }
    

        public void StopAudio(SoundType st)
        {
            
            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                AndroidNativeAudioMgr.StopTargetTypeAudio(st);
                
            }
            else
            {
                if (bgmSource == null || soundSource == null || voiceSource == null)
                    return;
                switch (st)
                {
                    case SoundType.COMMONBGM:
                    case SoundType.BGM:
                        bgmSource.Stop();
                        break;
                    case SoundType.COMMONSOUND:
                    case SoundType.SOUND:
                        soundSource.Stop();
                        break;
                    case SoundType.COMMONVOICE:
                    case SoundType.VOICE:
                        voiceSource.Stop();
                        break;
                    default:
                        break;
                }
            }
        }

        public void StopAudio()
        {
            

            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                AndroidNativeAudioMgr.StopAllAudio();
                
            }
            else
            {
                if (bgmSource != null && soundSource != null && voiceSource != null)
                {
                    bgmSource.Stop();
                    soundSource.Stop();
                    voiceSource.Stop();
                }
            }
        }

        public void ShowVoiceBtn(bool isShow)
        {
            voiceBtn.SetActive(isShow);
            //if (isShow)
            //    sheildGo.SetActive(true);
        }

        public void SetShield(bool isShow)
        {
            if (voiceBtnImg == null)
                voiceBtnImg = voiceBtn.transform.GetComponent<Image>();
            voiceBtnImg.enabled = isShow;
        }

        /// <summary>
        /// 唤醒语音按钮
        /// </summary>
        public void ReSetVoiceBtnEnable()
        {
            if (voiceBtn)
            {
                Debug.Log("@-------------voiceBtn 唤醒 ");
                if (voiceBtnImg == null)
                    voiceBtnImg = voiceBtn.transform.GetComponent<Image>();
                voiceBtnImg.enabled = true;
            }
        }

        /// <summary>
        /// 置空1.0和2.0的语音委托，防止出现交叉打开课程报错的问题
        /// </summary>
        public void ResetAllVoiceBtnEvent()
        {

            if (voiceEvent != null)
                voiceEvent = null;
            if (voiceCallBack != null)
                voiceCallBack = null;
        }

        /// <summary>
        /// 2.0对应音频的委托调用
        /// </summary>
        /// <param name="action"></param>
        public void SetVoiceBtnEvent(Action action)
        {
            voiceEvent = action;
        }

        public void Speaking(GameObject speaker, string speakerAni, SoundType type, int soundIdx, Action startEvent = null, Action endEvent = null)
        {
            //skipEvent = endEvent;
            skipBtn.SetActive(true);
            isShowSkipBtn = true;
            skipBtn.transform.GetComponent<Image>().CrossFadeAlpha(0.82f, 0, true);
            clipCo = StartCoroutine(SpeakerTalk(speaker, speakerAni, type, soundIdx, startEvent, endEvent));
        }

        IEnumerator SpeakerTalk(GameObject speaker, string speakerAni, SoundType type, int soundIdx, Action startEvent, Action endEvent)
        {
            Action end = () =>
            {
                sheildGo.SetActive(false);
                speaker.SetActive(false);
                SetShield(true);
                skipEvent = null;
                endEvent?.Invoke();
            };
            skipEvent = end;

            speaker.SetActive(true);
            sheildGo.SetActive(true);
            SetShield(false);
            SpineManager.instance.DoAnimation(speaker, speakerAni);
            float len = PlayClip(type, soundIdx);
            startEvent?.Invoke();
            Debug.LogFormat("SoundType: {0},soundIdx: {1},length: {2}", type, soundIdx, len);
            yield return new WaitForSeconds(len);
            //sheildGo.SetActive(false);
            //speaker.SetActive(false);
            //endEvent?.Invoke();
            Debug.LogFormat("EndWait SoundType: {0},soundIdx: {1},length: {2}", type, soundIdx, len);
            skipBtn.SetActive(false);
            end();
        }

        public void PlayClipByEvent(SoundType type, int soundIdx, Action startEvent = null, Action endEvent = null)
        {
            skipEvent = endEvent;
            skipBtn.SetActive(true);
            isShowSkipBtn = true;
            skipBtn.transform.GetComponent<Image>().CrossFadeAlpha(0.82f, 0, true);
            clipCo = StartCoroutine(PlayClipCoroutine(type, soundIdx, startEvent, endEvent));
        }

        IEnumerator PlayClipCoroutine(SoundType type, int soundIdx, Action startEvent, Action endEvent)
        {
            float len = PlayClip(type, soundIdx);
            SetShield(false);
            startEvent?.Invoke();
            yield return new WaitForSeconds(len);
            skipBtn.SetActive(false);
            SetShield(true);
            endEvent?.Invoke();
        }

        public void ResetAudio()
        {
            //BellLoom.QueueActions((p) =>
            //{
            //    Debug.Log("停止音效协程");
            //    StopAudio();
            //    StopAllSoundCoroutine();
            //}, null);

            BellLoom.QueueActions((p) =>
            {
                StopAudio(); Debug.Log("停止音效协程");
                StopAllSoundCoroutine();
            }, null);

        }

        public void StopAllSoundCoroutine()
        {
            StopAllCoroutines();
        }

        public float GetLength(SoundType stype, int sidx)
        {
            try
            {
                if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
                {
                    return AndroidNativeAudioMgr.GetAudioLength(
                        AndroidNativeAudioMgr.GetAudioMusicIDByName(sidx.ToString(), stype));
                }
                else
                {
                    AudioClip c = null;
                    string curCourse = HotfixManager.instance.curShowPackage.CourseName;
                    switch (stype)
                    {
                        case SoundType.COMMONBGM:
                        case SoundType.COMMONSOUND:
                        case SoundType.COMMONVOICE:
                            c = commonClips[stype][sidx];
                            break;
                        case SoundType.BGM:
                        case SoundType.SOUND:
                        case SoundType.VOICE:
                            c = audioClips[curCourse][stype][sidx];
                            break;
                        default:
                            Debug.LogError("音频类型SoundType错误, 请检查传入类型是否正确！");
                            break;
                    }
                    return c.length;
                }

                
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return 0;
            }
        }

        public void ResetStatus()
        {
            ResetAudio();
            voiceBtn.SetActive(false);
            sheildGo.SetActive(false);
            skipBtn.SetActive(false);
            skipEvent = null;
            clipCo = null;
        }

        //************ 兼容 1.0 Lua *************//
        public float PlayClip(int index, string coursName = "common", string type = "sound")
        {
            float clipLen = 0;
            if (coursName == "common")
                clipLen = PlayClip(index, type);
            else
            {
                switch (type)
                {
                    case "bgm":
                        clipLen = PlayClip(SoundType.BGM, index, true);
                        break;
                    case "voice":
                        clipLen = PlayClip(SoundType.VOICE, index);
                        break;
                    case "sound":
                        clipLen = PlayClip(SoundType.SOUND, index);
                        break;
                    default:
                        break;
                }
            }
            return clipLen;
        }

        float PlayClip(int index, string type)
        {
            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                string path=Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                    "bell.ai.bebo.launcher");
                path = path + "/static/resource/Common/Audio/BeBO1Audio/" + index + ".mp3";
                SoundType soundType = SoundType.BGM;
                switch (type)
                {
                    case "bgm":
                        soundType = SoundType.COMMONBGM;
                        break;
                    case "sound":
                        soundType = SoundType.COMMONSOUND;
                        break;
                    case "voice":
                        soundType = SoundType.COMMONVOICE;
                        break;
                }

                AndroidNativeAudioMgr.PlayBeBO1Audio(index.ToString(), soundType, false);
                return AndroidNativeAudioMgr.GetAudioLength(
                    AndroidNativeAudioMgr.GetBeBO1AudioMusicIDByName(index.ToString()));
            }
            else
            {
                AudioClip c = bebo1_commonClips[index];
                bool loop = false;
                AudioSource s = null;
                switch (type)
                {
                    case "bgm":
                        s = bgmSource;
                        loop = true;
                        break;
                    case "sound":
                        s = soundSource;
                        break;
                    case "voice":
                        s = voiceSource;
                        break;
                }
                if (c != null)
                {
                    //if (!loop)
                    //    s.PlayOneShot(c);
                    //else
                    //{
                    //    s.clip = c;
                    //    s.loop = loop;
                    //    s.Play();
                    //}
                    BellLoom.QueueActions((o) =>
                    {
                        if (!loop)
                            s.PlayOneShot(c);
                        else
                        {
                            s.clip = c;
                            s.loop = loop;
                            s.Play();
                        }
                    }, null);

                    return c.length;
                }
            }

            
            return 0;
        }

        public void ResetClip(string type = "all")
        {
            Stop(type);
        }

        public void StopAll()
        {
            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                AndroidNativeAudioMgr.StopAllAudio();
            }
            else
            {
                bgmSource.Stop();
                soundSource.Stop();
                voiceSource.Stop();
            }

            
        }

        public void Stop(string type = "all")
        {
            if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
            {
                switch (type)
                {
                    case "bgm":
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.BGM);
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.COMMONBGM);
                        break;
                    case "sound":
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.SOUND);
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.COMMONSOUND);
                        break;
                    case "voice":
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.VOICE);
                        AndroidNativeAudioMgr.StopTargetTypeAudio(SoundType.COMMONVOICE);
                        break;
                }
            }
            else
            {
                if (bgmSource == null || soundSource == null || voiceSource == null)
                    return;
                switch (type)
                {
                    case "bgm":
                        //bgmSource.Stop();
                        BellLoom.QueueActions((o) => { bgmSource.Stop(); }, null);
                        break;
                    case "sound":
                        //soundSource.Stop();
                        BellLoom.QueueActions((o) => { soundSource.Stop(); }, null);
                        break;
                    case "voice":
                        //voiceSource.Stop();
                        BellLoom.QueueActions((o) => { voiceSource.Stop(); }, null);
                        break;
                    default:
                        //bgmSource.Stop();
                        //soundSource.Stop();
                        //voiceSource.Stop();
                        //BellLoom.QueueActions((o) => {
                        //    Debug.Log(" -------------- Stop Audio ------------ ");
                        //    bgmSource.Stop();
                        //    soundSource.Stop();
                        //    voiceSource.Stop();
                        //}, null);
                        break;
                }
            }
            
        }

        public float GetLength(int index, string courseName = "common", string type = "sound")
        {
            try
            {
                if (GameManager.instance.IsUsingNewAudioFunc&&Application.platform == RuntimePlatform.Android && GetCurrenAndroidVersion.GetVersionInt() >= 29)
                {
                    if(courseName=="common")
                        return AndroidNativeAudioMgr.GetAudioLength(
                            AndroidNativeAudioMgr.GetBeBO1AudioMusicIDByName(index.ToString()));
                    else
                    {
                        ILFramework.SoundManager.SoundType targetSountype = SoundType.BGM;
                        return AndroidNativeAudioMgr.GetAudioLength(
                            AndroidNativeAudioMgr.GetAudioMusicIDByName(index.ToString(), targetSountype));
                    }

                    
                }
                else
                {
                    float clipLen = 0;
                    if (courseName == "common")
                    {
                        switch (type)
                        {
                            case "bgm":
                                clipLen = GetLength(SoundType.COMMONBGM, index);
                                break;
                            case "voice":
                                clipLen = GetLength(SoundType.COMMONVOICE, index);
                                break;
                            case "sound":
                                clipLen = GetLength(SoundType.COMMONSOUND, index);
                                break;
                            default:
                                break;
                        }
                    }
                    else
                    {
                        switch (type)
                        {
                            case "bgm":
                                clipLen = GetLength(SoundType.BGM, index);
                                break;
                            case "voice":
                                clipLen = GetLength(SoundType.VOICE, index);
                                break;
                            case "sound":
                                clipLen = GetLength(SoundType.SOUND, index);
                                break;
                            default:
                                break;
                        }
                    }
                    return clipLen;
                }

                
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return 0;
            }
        }

        /// <summary>
        /// 1.0对用的委托调用
        /// </summary>
        /// <param name="callBack"></param>
        public void SetLuaVoiceCallBack(LuaFunction callBack)
        {
            voiceCallBack = callBack;
        }
        public void BgSoundPart1(SoundType type = SoundType.BGM, float size = 0.1f)
        {
            PlayClip(type, 0, true);
            bgmSource.volume = size;
        }
        public void BgSoundPart2(SoundType type = SoundType.BGM, float size = 0.3f)
        {
            PlayClip(type, 0, true);
            bgmSource.volume = size;
        }
    }
}