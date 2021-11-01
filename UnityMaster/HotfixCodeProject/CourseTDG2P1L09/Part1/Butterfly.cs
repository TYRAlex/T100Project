using DG.Tweening;
using ILFramework;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CourseTDG2P1L09Part1
{
    class Butterfly
    {
        GameObject drag;
        SkeletonGraphic animation;
        MonoBehaviour mono;
        Vector3 centerPos;
        string[] butterflyAniStr;
        public bool isEnd, isPlayEnd;

        public Butterfly(GameObject drag, Vector3 centerPos, MonoBehaviour mono)
        {
            this.drag = drag;
            this.centerPos = centerPos;
            this.mono = mono;
            animation = drag.transform.GetChild(0).transform.GetChild(0).GetComponent<SkeletonGraphic>();
            butterflyAniStr = new string[] { "ui", "right", "error", "1", "animation", "animation2", "animation3" };
            isEnd = false;
            isPlayEnd = false;
        }

        public void RandomFly()
        {
            isPlayEnd = false;
            float time = UnityEngine.Random.Range(2, 4);
            SpineManager.instance.DoAnimation(animation.gameObject, butterflyAniStr[UnityEngine.Random.Range(3, 6)], true);
            mono.StartCoroutine(WaitTime(time, 260f));
            //mono.StartCoroutine(WaitTime(1f, 260f));
        }

        IEnumerator WaitTime(float time = 1f, float limit = 200f)
        {
            yield return new WaitForSeconds(time);
            SpineManager.instance.DoAnimation(animation.gameObject, butterflyAniStr[UnityEngine.Random.Range(3, 6)], true);
            Vector3 movePos = new Vector3(centerPos.x + UnityEngine.Random.Range(-limit + 30, limit + 30), centerPos.y + UnityEngine.Random.Range(-limit, limit), drag.transform.position.z);
            float flyTime = UnityEngine.Random.Range(3f, 6f);
            Vector3 dir = movePos - drag.transform.position;            
            Vector3 cross = Vector3.Cross(drag.transform.forward, dir.normalized);
            float angle = (Mathf.Acos(Vector3.Dot(drag.transform.up, dir.normalized)) * Mathf.Rad2Deg) * (cross.z / Mathf.Abs(cross.z));

            Debug.Log("movePos" + movePos);
            Debug.Log("cross:" + cross);
            Debug.Log("drag.transform.position" + drag.transform.position);
            Debug.Log("angle" + angle);
            drag.transform.DOMove(movePos, flyTime).OnComplete(() =>
                                                    {
                                                        SpineManager.instance.DoAnimation(animation.gameObject, butterflyAniStr[6], true);
                                                        drag.transform.DOMove(drag.transform.position, UnityEngine.Random.Range(4, 20)).OnComplete(() =>
                                                        {
                                                            isPlayEnd = true;
                                                        });
                                                    });
            drag.transform.DOLookAt(movePos, 2f);
            mono.StopCoroutine(WaitTime(time, limit));
        }
    }
}
