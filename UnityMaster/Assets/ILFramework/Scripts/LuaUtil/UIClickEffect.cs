using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class UIClickEffect : MonoBehaviour {

    public GameObject effect;
    public float scaleEnd = 2f;
    public float during = 0.5f;
    Vector3 _prevScale;
    bool canClick = true;

    private void Awake()
    {
        if (!effect) return;
        _prevScale = effect.transform.localScale;
        effect.SetActive(false);
    }

    private void OnDisable()
    {
        if (effect != null)
            DoReset();
    }
    private void OnEnable()
    {
        if(effect!=null)
            effect.GetComponent<Image>().DOFade(1, 0);
    }

    public void DoEffect()
    {
        if (!canClick) return;
        if (!effect) return;
        effect.SetActive(true);
        //判断负方向
        var v3 = new Vector3(scaleEnd, scaleEnd, scaleEnd);
        if (transform.localScale.x < 0) v3.x = -scaleEnd;
        if (transform.localScale.y < 0) v3.y = -scaleEnd;
        if (transform.localScale.z < 0) v3.z = -scaleEnd;

        effect.transform.DOScale(v3, during);
        effect.GetComponent<Image>().DOFade(0, during);
        if(effect.transform.childCount>0)
        {
            Debug.Log("testChild");
            effect.GetComponentInChildren<Image>().DOFade(0, during);
        }
        StartCoroutine(DelayReset(during));
    }

    IEnumerator DelayReset(float time)
    {
        canClick = false;
        yield return new WaitForSeconds(time);
        DoReset();
    }

    public void DoReset()
    {
        canClick = true;
        effect.SetActive(false);
        effect.transform.DOScale(_prevScale, 0);
        effect.GetComponent<Image>().DOFade(1, 0);
        if (effect.transform.childCount > 0)
        {
            effect.GetComponentInChildren<Image>().DOFade(1, 0);
        }
    }
}
