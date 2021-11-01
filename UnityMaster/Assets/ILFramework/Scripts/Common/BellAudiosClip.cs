using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BellAudiosClip : MonoBehaviour {

    public AudioClip[] clips;
    public enum AudioType
    {
        Bgm,
        Sound,
        Voice,
        CommonBgm,
        CommonSound,
        CommonVoice,
    }
    public AudioType audioType;
}
