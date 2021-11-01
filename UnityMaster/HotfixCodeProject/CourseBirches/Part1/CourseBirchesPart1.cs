using CourseBirchesPart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ILFramework.HotClass
{
    public class CourseBirchesPart1
    {
        GameObject curGo;
        ILObject3DAction mask;
        TargetChild curTarget;
        List<TargetChild> targets;
        string[] namesAnim;
        GameObject dingding;
        bool isShowSprite;
        float timer;
        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            namesAnim = new string[] { "hf", "jnb", "nc", "ptn", "smy" };
            mask = curGo.transform.Find("UI3D/mask").GetComponent<ILObject3DAction>();
            mask.OnMouseDownLua = MaskOnClickBtn;
            curTarget = null;
            isShowSprite = false;
            dingding = curGo.transform.Find("dingding").gameObject;
            InitTargetChild();
            timer = 2.0f;
            SoundManager.instance.BgSoundPart1();
        }
        public void InitTargetChild()
        {
            targets = new List<TargetChild>();
            GameObject go = curGo.transform.Find("UI3D").gameObject;
            for(int i = 0;i < 5;i++)
            {
                TargetChild target = new TargetChild(go, i, namesAnim[i]);
                target.Btn.OnMouseDownLua = BtnsOnClick;
                targets.Add(target);
            }
            mask.gameObject.SetActive(true);
            SoundManager.instance.Speaking(dingding, "talk", SoundManager.SoundType.SOUND, 0, null, () =>
                {
                    curGo.GetComponent<MonoBehaviour>().StartCoroutine(WaitShowBtns());
                });
        }
        IEnumerator WaitShowBtns()
        {
            int index = 0;
            while(index < 5)
            {
                float time = targets[index].playDoAnimation();
                yield return new WaitForSeconds(timer/5);
                index++;
            }
            mask.gameObject.SetActive(false);
        }
        public void BtnsOnClick(int index)
        {
            //mask.gameObject.SetActive(true);
            curTarget = targets[index - 1];
            curTarget.BtnClick(()=>isShowSprite = true);
        }
        public void MaskOnClickBtn(int index)
        {
            if(isShowSprite)
            {
                isShowSprite = false;
                curTarget.HideSprite(() =>mask.gameObject.SetActive(false));
            }
        }
    }
}
