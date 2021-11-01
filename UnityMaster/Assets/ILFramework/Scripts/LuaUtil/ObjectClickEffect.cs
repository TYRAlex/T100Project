using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using Spine.Unity;
using System;
using UnityEngine.UI;
using ILFramework;

[RequireComponent(typeof(DOTweenAnimation))]
public class ObjectClickEffect : MonoBehaviour
{
    DOTweenAnimation _dotweenAni;
    SoundManager _soundManager;
    //CourseManager _courseManager;
    //Migu migu;
    public bool is3DClick = false;
    public string courseName;
    public int soundNum = 0;
    public float clickInterval = 1f;//点击间隔
    public bool isActive = true;
    public GameObject Actor;
    public string idle = "daijiqj";
    public string action = "shuohuaqj";
    public GameObject effect;
    bool canClick = true;
    Vector3 _prevScale;
    public float scaleEnd = 1.05f;
    public float during = 0.5f;

    void Awake()
    {
        _dotweenAni = GetComponent<DOTweenAnimation>();
        _dotweenAni.autoKill = false;
        _dotweenAni.autoPlay = false;
        //_dotweenAni.animationType = DG.Tweening.Core.DOTweenAnimationType.PunchScale;
        _soundManager = FindObjectOfType<SoundManager>();
        //_courseManager = FindObjectOfType<CourseManager>();
        if (effect)
        {
            _prevScale = effect.transform.localScale;  
            effect.SetActive(false);
        }
        if (!is3DClick)
        {
            var btn = GetComponent<Button>();
            if (!btn)
            {
                gameObject.AddComponent<Button>();
                btn = GetComponent<Button>();
                btn.transition = Selectable.Transition.None;
            }
            btn.onClick.AddListener(OnClick);
        }
    }

    void Update()
    {
        if (courseName.Trim() == string.Empty) courseName = "course"; //_courseManager.currentCourseName; 避免为空 进入"common"判断分支 因为不用此参数所以随意填个字符串
    }

    void OnMouseUp()
    {
        if (!is3DClick) return;
        OnClick();
    }

    public void OnClick()
    {
        Debug.Log("测试"+isActive.ToString() + canClick.ToString() + gameObject.name);
        if (!isActive) return;
        if (!canClick) return;    
        StartCoroutine(clickRoutine());
        if (_dotweenAni==null)
        {
            _dotweenAni = GetComponent<DOTweenAnimation>();
            _dotweenAni.autoKill = false;
            _dotweenAni.autoPlay = false;
        }
        _dotweenAni.DORestart();
        clickInterval=_soundManager.PlayClip(soundNum, courseName);
        Debug.Log(clickInterval + "当前音频长度");
        if (Actor)
        {
            var spineAnimationState = Actor.GetComponent<SkeletonGraphic>().AnimationState;
            doShotAnimation(spineAnimationState, clickInterval, action, idle);
        }
        if (effect)
        {
            StartCoroutine(showEffect());
        }
    }

    IEnumerator clickRoutine()
    {
        canClick = false;
        yield return new WaitForSeconds(clickInterval);
        canClick = true;
    }

    void OnDisable()
    {
        canClick = true;
    }

    IEnumerator showEffect()
    {
        effect.SetActive(true);
        effect.transform.DOScale(scaleEnd, during);
        effect.GetComponent<Graphic>().DOFade(0, during);
        yield return new WaitForSeconds(0.5f);
        //effect.SetActive(false);
        DoReset();
    }

    //做短动作
    public void doShotAnimation(Spine.AnimationState spineAnimationState, float second, string action = "shuohuaqj", string idle = "daijiqj")
    {
        StartCoroutine(shotAniRoutine(spineAnimationState, second, action, idle));
    }

    IEnumerator shotAniRoutine(Spine.AnimationState spineAnimationState, float second, string action, string idle)
    {
        spineAnimationState.SetAnimation(0, action, true);
        yield return new WaitForSeconds(second);
        spineAnimationState.SetAnimation(0, idle, true);
    }

    public void DoReset()
    {
        effect.SetActive(false);
        effect.transform.DOScale(_prevScale, 0);
        effect.GetComponent<Graphic>().DOFade(1, 0);
    }
}
