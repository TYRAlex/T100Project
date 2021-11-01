using System;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;
using Object = UnityEngine.Object;


namespace Thirds.NewWayToPlayAudio.Encoder
{
    [System.Serializable]
    public struct Audio
    {
        public string name;
        public int fileID;
        public Object obj;
        public int streamID;
        public AndroidNative_AudioType audioType;
        public SoundManager.SoundType generalAudioType;
    
        public Audio(AndroidNative_AudioType audioType,string name, int musicID,SoundManager.SoundType soundType=SoundManager.SoundType.COMMONSOUND ,Object obj=null,int streamID=0)
        {
            this.audioType = audioType;
            this.name = name;
            this.fileID = musicID;
            this.obj = obj;
            this.streamID = streamID;
            generalAudioType = soundType;
        }

        public void SetSoundTye(SoundManager.SoundType type)
        {
            generalAudioType = type;
        }
    }

    public enum AndroidNative_AudioType
    {
        Long,
        Short
    }
    public class AndroidNativeAudioMgr : MonoBehaviour
    {
        public List<Audio> audios = new List<Audio>();
        public static AndroidNativeAudioMgr ins;
    
        private static int currentStreamID = 0;
        void Awake()
        {
            if(ins==null)
            {
                ins = this;
                //DontDestroyOnLoad(this);
                AndroidNativeAudio.makePool();
            
            }
            else
            {
                Destroy(this);
            }
        }
     
        /// <summary>
        /// 播放音效
        /// </summary>
        /// <param name="clipName"></param>
        /// <param name="obj"></param>
        /// <param name="loop"></param>
        /// <returns></returns>
        public static void PlayShortAudio(SoundManager.SoundType soundType,string clipName, Object obj, bool loop = false)
        {
        
            //clipName =Application.persistentDataPath+ "/static/resource/Audio/"+GameManager.instance.CurrentPlayCourseName+"/"+soundType.ToString()+"/" + clipName+".mp3";
            clipName = HotfixManager.instance.AndroidCurrentPath + "/" + GameManager.instance.CurrentPlayCourseName +
                       "/Audio/" +
                       soundType.ToString() + "/" + clipName + ".mp3";
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name != clipName)
                        continue;
                    AndroidNativeAudio.play(ins.audios[i].fileID, 1, -1, 1, loop ? -1 : 0);
                    return;
                }
            }
            AndroidNativeAudio.load(clipName, true, (fileID)=> {
                
                int streamID= AndroidNativeAudio.play(fileID, 1, -1, 1, loop ? -1 : 0);
                ins.audios.Add(new Audio(AndroidNative_AudioType.Short,clipName, fileID,soundType, obj,streamID));
                return; 
            });
        }

        public static int GetAudioMusicIDByName(string clipName,SoundManager.SoundType soundType)
        {
            switch (soundType)
            {
                case SoundManager.SoundType.BGM:
                case SoundManager.SoundType.SOUND:
                case SoundManager.SoundType.VOICE:
                    clipName = HotfixManager.instance.AndroidCurrentPath+"/" + GameManager.instance.CurrentPlayCourseName + "/Audio/" +
                               soundType.ToString() + "/" + clipName + ".mp3";
                    break;
                case SoundManager.SoundType.COMMONBGM:
                case SoundManager.SoundType.COMMONSOUND:
                case SoundManager.SoundType.COMMONVOICE:
                    string currentPath = Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                        "bell.ai.bebo.launcher");
                    
                    clipName = currentPath+ "/static/resource/Common/Audio/" + soundType.ToString() + "/" + clipName + ".mp3";
                    break;
            }
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name != clipName)
                        continue;
                    Debug.LogError("找到相应的音频:" + clipName);
                    return ins.audios[i].fileID;
                }
            }
            Debug.LogError("没找到找到相应的音频！，请检查！:" + clipName);
            return -1;
        }
        
        public static int GetBeBO1AudioMusicIDByName(string clipName)
        {
            clipName = HotfixManager.instance.AndroidCurrentPath+"/" + GameManager.instance.CurrentPlayCourseName + "/Audio/BeBO1Audio/"+ clipName + ".mp3";
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name != clipName)
                        continue;
                    return ins.audios[i].fileID;
                }
            }

            return -1;
        }


        public static void PlayLongAudio(SoundManager.SoundType soundType,string clipName,bool loop =true,Action<int> callback=null)
        {
            // clipName = Application.persistentDataPath+ "/static/resource/Audio/" + GameManager.instance.CurrentPlayCourseName+"/"+
            //            soundType.ToString() + "/" + clipName + ".mp3";
            switch (soundType)
            {
                case SoundManager.SoundType.BGM:
                case SoundManager.SoundType.SOUND:
                case SoundManager.SoundType.VOICE:
                    clipName = HotfixManager.instance.AndroidCurrentPath+"/" + GameManager.instance.CurrentPlayCourseName + "/Audio/" +
                               soundType.ToString() + "/" + clipName + ".mp3";
                    break;
                case SoundManager.SoundType.COMMONBGM:
                case SoundManager.SoundType.COMMONSOUND:
                case SoundManager.SoundType.COMMONVOICE:
                    string currentPath = Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                        "bell.ai.bebo.launcher");
                    
                    clipName = currentPath+ "/static/resource/Common/Audio/" + soundType.ToString() + "/" + clipName + ".mp3";
                    break;
            }
            
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name != clipName)
                        continue;
                    ANAMusic.play(ins.audios[i].fileID);
                    return;
                }
            }

            ANAMusic.load(clipName, true, false, (fileID)=>
            {
                ANAMusic.play(fileID, callback);
                ins.audios.Add(new Audio(AndroidNative_AudioType.Long,clipName, fileID,soundType));
            });
        }
        
        public static void PlayBeBO1Audio(string clipName,SoundManager.SoundType soundType,bool loop =true,Action<int> callback=null)
        {
            string currentPath = Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                "bell.ai.bebo.launcher");
                    
            clipName = currentPath+ "/static/resource/Common/Audio/BeBO1Audio/"+ clipName + ".mp3";
            
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name != clipName)
                        continue;
                    ins.audios[i].SetSoundTye(soundType);
                    ANAMusic.play(ins.audios[i].fileID);
                    return;
                }
            }

            ANAMusic.load(clipName, true, false, (fileID)=>
            {
                ANAMusic.play(fileID, callback);
                ins.audios.Add(new Audio(AndroidNative_AudioType.Long, clipName, fileID, soundType));
            });
        }
        
        

        public static void LoadLongAudio(string clipName,SoundManager.SoundType soundType)
        {
            switch (soundType)
            {
                case SoundManager.SoundType.BGM:
                case SoundManager.SoundType.SOUND:
                case SoundManager.SoundType.VOICE:
                    clipName = HotfixManager.instance.AndroidCurrentPath+"/" + HotfixManager.instance.CourseName + "/Audio/" +
                               soundType.ToString() + "/" + clipName + ".mp3";
                    break;
                case SoundManager.SoundType.COMMONBGM:
                case SoundManager.SoundType.COMMONSOUND:
                case SoundManager.SoundType.COMMONVOICE:
                    string currentPath = Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                        "bell.ai.bebo.launcher");
                    
                    clipName = currentPath+ "/static/resource/Common/Audio/" + soundType.ToString() + "/" + clipName + ".mp3";
                    break;
            }
            Debug.LogError("路径为："+clipName);
            ANAMusic.load(clipName, true, true, (fileID)=>
            {
                ins.audios.Add(new Audio(AndroidNative_AudioType.Long,clipName, fileID,soundType));
            });
        }

        public static void LoadBeBO1CommonClip(string clipName)
        {
            Debug.LogError("要加载的BeBO1的音频名称为："+clipName);
            clipName=Application.persistentDataPath.Replace("ai.bell.BeBOCourseDefaultPlayer",
                "bell.ai.bebo.launcher")+ "/static/resource/Common/Audio/BeBO1Audio/" + clipName + ".mp3";
            Debug.LogError("BeBO1的路径为："+clipName);
            ANAMusic.load(clipName, true, false, (fileID)=>
            {
                ins.audios.Add(new Audio(AndroidNative_AudioType.Long,clipName, fileID));
            });
        }

        public static void StopSound(string clipName)
        {
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name == clipName)
                    {
                        AndroidNativeAudio.stop(ins.audios[i].streamID);
                    }
                }
            }

            
        }

        public static void StopBGM(string clipName)
        {
            if (ins.audios.Count > 0)
            {
                for (int i = 0; i < ins.audios.Count; i++)
                {
                    if (ins.audios[i].name == clipName)
                    {
                        ANAMusic.pause(ins.audios[i].fileID);
                        return;
                    
                    }
                }
            }
        }

        public static void StopTargetTypeAudio(SoundManager.SoundType soundType)
        {
            for (int i = 0; i < ins.audios.Count; i++)
            {
                if (ins.audios[i].generalAudioType == soundType)
                {
                    //AndroidNativeAudio.stop(ins.audios[i].streamID);
                    Debug.Log("暂停"+soundType.ToString()+"音频");
                    ANAMusic.pause(ins.audios[i].fileID);
                    
                }
            }
            // if (ins.audios.Count > 0)
            // {
            //     if(soundType==SoundManager.SoundType.BGM||soundType==SoundManager.SoundType.COMMONBGM)
            //         ANAMusic.pauseAll();
            //     else
            //     {
            //         for (int i = 0; i < ins.audios.Count; i++)
            //         {
            //             if (ins.audios[i].generalAudioType == soundType)
            //             {
            //                 AndroidNativeAudio.stop(ins.audios[i].streamID);
            //
            //
            //             }
            //         }
            //     }
            //
            //
            // }
        }

        

        public static void StopAllAudio()
        {
            Debug.Log("关掉所有的音频");
            ANAMusic.pauseAll();
            AndroidNativeAudio.pauseAll();
            // if (ins.audios.Count > 0)
            // {
            //     for (int i = 0; i < ins.audios.Count; i++)
            //     {
            //         if (ins.audios[i].generalAudioType == SoundManager.SoundType.BGM||ins.audios[i].generalAudioType == SoundManager.SoundType.COMMONBGM)
            //         {
            //             ANAMusic.pause(ins.audios[i].fileID);
            //             
            //         }
            //         else
            //         {
            //             AndroidNativeAudio.stop(ins.audios[i].streamID);
            //         }
            //     }
            // }
        }

        public static float GetAudioLength(int musicID)
        {
            int length = ANAMusic.getDuration(musicID);
            Debug.Log("返回的音频长度为:"+length);
            float targetLength = length / 1000f;
            return targetLength;
        }




        void OnApplicationQuit()
        {
            // Clean up when done
            for (int i = 0; i < ins.audios.Count; i++)
            {
                if (ins.audios[i].audioType == AndroidNative_AudioType.Short)
                    AndroidNativeAudio.unload(ins.audios[i].fileID);
                else if (ins.audios[i].audioType == AndroidNative_AudioType.Long)
                {
                    ANAMusic.pause(ins.audios[i].fileID);
                    ANAMusic.release(ins.audios[i].fileID);
                
                }
            }
            AndroidNativeAudio.releasePool();
        
        }
    }
}