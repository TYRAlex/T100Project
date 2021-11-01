using ILFramework;
using ILFramework.HotClass;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace CourseButterflyPart
{
    class otherCommon
    {
        Image xian;
        Transform bi;
        GameObject biBtn;
        SpriteRenderer right;
        SpriteRenderer left;
        Vector3 biStrate;
        float distance;
        float timeMax = 1.0f;
        int max;
        int index;
        public otherCommon(GameObject go)
        {
            max = (int)(timeMax / 0.1f);
            index = 0;
            xian = go.transform.Find("xian").GetComponent<Image>();
            distance = xian.rectTransform.rect.height * xian.transform.localScale.y;
            bi = go.transform.Find("biImg").transform;
            biStrate = bi.localPosition;
            biBtn = go.transform.Find("biBtn").gameObject;
            biBtn.GetComponent<Object3DAction>().OnMouseUpLua = OnMouseUp;
            right = go.transform.Find("right").GetComponent<SpriteRenderer>();
            left = go.transform.Find("left").GetComponent<SpriteRenderer>();
        }
        public void OnInit(Sprite sprite)
        {
            right.sprite = sprite;
            left.sprite = sprite;
            bi.localPosition = biStrate;
            xian.fillAmount = 0.0f;
            biBtn.SetActive(true);
            bi.gameObject.SetActive(false);
        }
        public void OnMouseUp(int index)
        {
            if(PowerpointManager.instance.IsMove == false)
            {
                Debug.Log("---------BtnOnclick------------");
                biBtn.SetActive(false);
                DragXian();
            }
        }
        public void DragXian()
        {
            bi.gameObject.SetActive(true);
            CourseButterflyPart1.mono.StartCoroutine(draging());
        }
        IEnumerator draging()
        {
            while (index != max)
            {
                index++;
                yield return new WaitForSeconds(0.1f);
                xian.fillAmount = (float)index / (float)max;
                bi.localPosition = new Vector3(biStrate.x, biStrate.y - distance * xian.fillAmount, biStrate.z);
            }
            index = 0;
            yield return new WaitForSeconds(0.2f);
            bi.gameObject.SetActive(false);
            xian.fillAmount = 0.0f;
            MesManager.instance.Dispatch("CourseButterflyPart1", (int)State.HuaXian);
        }
    }
}
