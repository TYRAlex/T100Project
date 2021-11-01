using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework
{
    public class PowerpointManager: Manager<PowerpointManager>
    {
        GameObject selfGo;
        int max;
        string[] dataStr;
        public ILObject3DAction drag;
        List<GameObject> tampList;
        GameObject tampParent;
        float lengDrag;
        int direction;
        public int curIndex;
        float startX;
        float startEnd;
        Action act;
        public bool isFirst;
        public bool IsMove { get; set; }

        public void CreatePower(GameObject curGo,string[] dataStr, Action _act = null)
        {
            this.selfGo = curGo;
            this.max = dataStr.Length;
            this.dataStr = dataStr;
            this.tampParent = curGo.transform.Find("tamp_Parent").gameObject;
            this.drag = curGo.transform.Find("tamp_Parent/Drag").GetComponent<ILObject3DAction>();
            act = _act;
            isFirst = true;
            OnInit();
        }

        public void PlayAnimationVoice(GameObject curGo)
        {
            Transform curTrans = curGo.transform;

            GameObject npc = curTrans.Find("npc").gameObject;
            SoundManager.instance.StopAudio();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            SoundManager.instance.bgmSource.volume = 0.3f;
            SoundManager.instance.Speaking(npc, "animation2", SoundManager.SoundType.VOICE, 0, null, () =>
            {
                npc.SetActive(true);
                SpineManager.instance.DoAnimation(npc, "animation", true);
            });
        }

        public void MaskShow()
        {
            drag.GetComponent<BoxCollider>().enabled = true;
        }
        public void MaskHide()
        {
            drag.GetComponent<BoxCollider>().enabled = false;
        }
        public void OnInit()
        {
            lengDrag = Screen.width / 70;
            direction = -1;
            curIndex = 0;
            tampList = new List<GameObject>();
            for (int i = 0;i < max;i++)
            {
                GameObject go = tampParent.transform.Find(dataStr[i]).gameObject;
                go.SetActive(false);
                tampList.Add(go);
            }
            tampList[0].SetActive(true);
            drag.OnMouseDownLua = OnMouseDown;
            drag.OnMouseDragLua = OnMouseDrag;
            drag.OnMouseUpLua = OnMouseUp;
            if (act != null) act();
        }
        public void OnMouseDown(int index)
        {
            IsMove = false;
            //Debug.Log("--------------------OnMouseDown");
            startX = Input.mousePosition.x - Screen.width / 2;
        }
        public void OnMouseDrag(int index)
        {
            if (drag.GetComponent<BoxCollider>().enabled == false) return;
            if(isFirst)
            {
                startEnd = Input.mousePosition.x - Screen.width / 2;
                IsCheck();
            }
        }
        public void OnMouseUp(int index)
        {
            isFirst = true;
        }
        public void SwitchPower(int index)
        {
            for (int i = 0; i < max; i++)
            {
                tampList[i].SetActive(false);
            }
            tampList[index].SetActive(true);
        }
        public void IsCheck()
        {
            float result = startEnd - startX;
            if (result > 0)
            {
                direction = 1;
            }
            else
            {
                direction = 0;
            }
            if (Math.Abs(result) < lengDrag) return;
            if (direction == 0)
            {
                if (curIndex == max - 1)
                {
                    return;
                }
                curIndex++;
                IsMove = true;
                isFirst = false;
            }
            else if (direction == 1)
            {
                if (curIndex == 0)
                {
                    return;
                }
                curIndex--;
                IsMove = true;
                isFirst = false;
            }
            else
            {
                return;
            }
            SwitchPower(curIndex);
            if (act != null) act();
        }
    }
}
