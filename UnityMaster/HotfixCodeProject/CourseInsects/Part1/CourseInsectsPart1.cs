using CourseInsectsPart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public struct TargetPlay
    {
        public GameObject anim;
        public Vector2 pos;
    }
    public class CourseInsectsPart1
    {
        static public GameObject curGo;
        public List<TargetPlay> dataPlays;
        AnimationPlayer player;
        ILObject3DAction bihu;
        ILObject3DAction qingwa_idle;
        GameObject qingwa_error;
        GameObject mask;
        GameObject dingding;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            Transform ui3d = curTrans.transform.Find("UI3D");
            Transform parent = ui3d.Find("animation");
            dataPlays = new List<TargetPlay>();
            player = new AnimationPlayer();
            for (int i = 0;i < 3;i++)
            {
                ILObject3DAction il = parent.GetChild(i).GetComponent<ILObject3DAction>();
                il.index = i + 1;
                il.OnMouseDownLua = OnMouseDown;
                AddTargerPlay(il.name);
            }
            bihu = parent.transform.Find("bihu").GetComponent<ILObject3DAction>();
            bihu.OnMouseDownLua = OnMouseDown;
            SpineManager.instance.DoAnimation(bihu.gameObject, "idle");
            qingwa_idle = parent.transform.Find("qingwa").GetComponent<ILObject3DAction>();
            qingwa_idle.OnMouseDownLua = OnMouseDown;
            SpineManager.instance.DoAnimation(qingwa_idle.gameObject, "idle");
            qingwa_error = parent.transform.Find("qingwa/error").gameObject;
            mask = ui3d.transform.Find("mask").gameObject;
            dingding = curTrans.Find("dingding").gameObject;
            Speaking(0);
            SoundManager.instance.BgSoundPart1();
        }
        public void Speaking(int index)
        {
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index, () => mask.SetActive(true), () => mask.SetActive(false));
        }
        public void AddTargerPlay(string name)
        {
            TargetPlay curTarget = new TargetPlay();
            GameObject go = curGo.transform.Find("UI3D/animation/"+name).gameObject;
            SpineManager.instance.DoAnimation(go, "idle");
            curTarget.anim = curGo.transform.Find("UI3D/fangda/button/" + name).gameObject;
            curTarget.pos = go.transform.GetChild(0).transform.position;
            dataPlays.Add(curTarget);
        }
        private void OnMouseDown(int index)
        {            
            if (index == 4)
            {
                SpineManager.instance.DoAnimation(bihu.gameObject, "error",false,() => SpineManager.instance.DoAnimation(bihu.gameObject, "idle"));
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index, () => {
                    mask.SetActive(true);
                    dingding.SetActive(false);
                }, () => mask.SetActive(false));
            }
            else if(index == 5)
            {
                qingwa_error.transform.localScale = Vector3.one;
                SpineManager.instance.DoAnimation(qingwa_error, "error", false,() => qingwa_error.transform.localScale = Vector3.zero);
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index, () => {
                    mask.SetActive(true);
                    dingding.SetActive(false);
                }, () => mask.SetActive(false));
            }
            else
            {
                player.SetPlayer(dataPlays[index - 1].anim, dataPlays[index - 1].pos);
                SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, index, () => mask.SetActive(true), () => mask.SetActive(false));
            }
        }
    }
}
