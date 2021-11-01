using DG.Tweening;
using ILFramework;
using ILFramework.HotClass;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace CourseInsectsPart
{
    class AnimationPlayer
    {
        GameObject btn;
        GameObject target;
        Vector3 startPos;
        string name;
        GameObject mask;
        float colorA;
        Vector3 endPos;
        float scale;
        public AnimationPlayer()
        {
            btn = CourseInsectsPart1.curGo.transform.Find("UI3D/fangda/button").gameObject;
            mask = CourseInsectsPart1.curGo.transform.Find("UI3D/fangda/mask").gameObject;
            mask.GetComponent<ILObject3DAction>().OnMouseDownLua = OnMouseDown;
            name = "fangda";
            endPos = btn.transform.position;
            scale = 0.2f;
            colorA = 0.8f;
        }

        private void OnMouseDown(int obj)
        {
            mask.GetComponent<BoxCollider2D>().enabled = false;
            btn.transform.DOMove(startPos, 1.0f);
            btn.transform.DOScale(scale, 1.0f).OnComplete(() => OnReset());
        }

        public void SetPlayer(GameObject target, Vector2 pos)
        {
            this.target = target;
            startPos = new Vector3(pos.x,pos.y, btn.transform.position.z);
            OnInit();
            CourseInsectsPart1.curGo.GetComponent<MonoBehaviour>().StartCoroutine(WaitColor(0.5f, PlayAnimation));
        }
        public void OnInit()
        {
            mask.SetActive(true);
            mask.GetComponent<BoxCollider2D>().enabled = true;
            target.transform.localScale = Vector3.one;
            SpineManager.instance.PlayAnimationState(target.GetComponent<SkeletonAnimation>(), name);
        }
        public void OnReset()
        {
            mask.SetActive(false);
            btn.transform.localScale = Vector3.zero;
            target.transform.localScale = Vector3.zero;
            SpineManager.instance.PlayAnimationState(target.GetComponent<SkeletonAnimation>(), name);
        }
        public void PlayAnimation()
        {
            btn.transform.localScale = new Vector3(0.2f,0.2f,1);

            btn.transform.position = startPos;
            btn.transform.DOMove(endPos, 1.0f);
            btn.transform.DOScale(1, 1.0f).OnComplete(()=> 
            {
                SpineManager.instance.DoAnimation(target, name, true);
            });
        }
        IEnumerator WaitColor(float time,Action act)
        {
            int count = (int)(time / 0.02f);
            for(int i = 1;i <= count;i++)
            {
                mask.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, colorA * (float)i/(float)count );
                yield return new WaitForSeconds(0.02f);
            }
            mask.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, colorA);
            act();
        }
    }
}
