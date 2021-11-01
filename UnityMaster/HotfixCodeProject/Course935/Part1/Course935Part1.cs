using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace ILFramework.HotClass
{
    public class Course935Part1
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;

        private GameObject Box;
        private GameObject max;
        private GameObject controlBox;
        private GameObject _fmask;
        private GameObject _lmask;
        private GameObject _rmask;


        private RectTransform _fre;
        private RectTransform _lre;
        private RectTransform _rre;

        private GameObject fh;
        private GameObject nx;
        private GameObject ban;
        private GameObject clickBox;
        private bool isplay;
        private bool _canmove;
        private bool _canroate;
        private bool _canvoice;
        private bool _canwin;
        private bool _canClick;
        private bool _canClick2;
        private int level;
        private GameObject shengli;

        private int number;
        private int tempnumber;
        private Vector3 tempvector3;
        private GameObject mymask;
        private float offx;
        private float x;
        private bool _canupdate;
        private RectTransform temprec;
        private bool _cando;
        private bool _canshow;
        private bool _canreplay;

        private bool _canClickThe3Dof=false;
        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            Input.multiTouchEnabled = false;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            max = curTrans.Find("max").gameObject;
            _fmask = max.transform.GetChild(0).GetChild(0).gameObject;
            _lmask = max.transform.GetChild(1).GetChild(0).gameObject;
            _rmask = max.transform.GetChild(2).GetChild(0).gameObject;
            _fre = _fmask.GetComponent<RectTransform>();
            _lre = _lmask.GetComponent<RectTransform>();
            _rre = _rmask.GetComponent<RectTransform>();
            clickBox = curTrans.Find("Click").gameObject;
            controlBox = curTrans.Find("controlBox").gameObject;
            temprec = max.GetComponent<RectTransform>();
            ban = curTrans.Find("ban").gameObject;
            fh = curTrans.Find("fh").gameObject;
            nx = curTrans.Find("next").gameObject;
            shengli = curTrans.Find("shengli").gameObject;
            mymask = curTrans.Find("mymask").gameObject;
            for (int i = 0; i < 4; i++)
            {
                Util.AddBtnClick(controlBox.transform.GetChild(i).GetChild(0).gameObject, Rotate);
            }
            Util.AddBtnClick(controlBox.transform.GetChild(4).GetChild(0).gameObject, Move);
            Util.AddBtnClick(fh, replay);
            Util.AddBtnClick(nx, next);
            for (int i = 0; i < clickBox.transform.childCount; i++)
            {
                Util.AddBtnClick(clickBox.transform.GetChild(i).gameObject, Click);
            }
            offx = Screen.width / 1920f;
            GameInit();
            GameStart();
        }

        void Update()
        {
            if (!_canupdate)
                return;
            RaycastHit2D hit1 = Physics2D.Raycast(max.transform.position,Vector2.up);
            RaycastHit2D hit4 = Physics2D.Raycast(max.transform.position, Vector2.down);
            RaycastHit2D hit2 = Physics2D.Raycast(max.transform.position, Vector2.left);
            RaycastHit2D hit3 = Physics2D.Raycast(max.transform.position, Vector2.right);
            if(number==0)
            {
                SetRectTransformSize(_fre,new Vector2(8,(hit1.distance/offx-50)));
                SetRectTransformSize(_lre, new Vector2(8, hit2.distance / offx-50));
                SetRectTransformSize(_rre,new Vector2(8,hit3.distance / offx-50));
                if(hit1.distance<210*offx)
                {
                    _canmove = false;
                }
            }
            if (number == 1)
            {
                SetRectTransformSize(_fre, new Vector2(8, hit2.distance / offx - 50));
                SetRectTransformSize(_lre, new Vector2(8, hit4.distance / offx - 50));
                SetRectTransformSize(_rre, new Vector2(8, hit1.distance / offx - 50));
                if (hit2.distance < 210 * offx)
                {
                    _canmove = false;
                }
            }
            if (number == 2)
            {
                SetRectTransformSize(_fre, new Vector2(8, hit4.distance / offx - 50));
                SetRectTransformSize(_lre, new Vector2(8, hit3.distance / offx - 50));
                SetRectTransformSize(_rre, new Vector2(8, hit2.distance / offx - 50));
                if (hit4.distance < 210 * offx)
                {
                    _canmove = false;
                }
            }
            if (number == 3)
            {
                SetRectTransformSize(_fre, new Vector2(8, hit3.distance / offx - 50));
                SetRectTransformSize(_lre, new Vector2(8, hit1.distance / offx - 50));
                SetRectTransformSize(_rre, new Vector2(8, hit4.distance / offx - 50));
                if (hit3.distance < 210 * offx)
                {
                    _canmove = false;
                }
            }
            if (_canshow)
            {
                _canshow = false;
                Show();
            }
               
        }

       

        private void TriggerCallBack()
        {
            if(!_canClickThe3Dof)
                return;
            if (FunctionOf3Dof.Instance.PadBtnCenter || FunctionOf3Dof.Instance.Trigger)
            {
                MoveFunc();
                return;
            }
            
            if (FunctionOf3Dof.Instance.PadBtnLeft || FunctionOf3Dof.Instance.PadBtnRight ||
                FunctionOf3Dof.Instance.PadBtnBotton || FunctionOf3Dof.Instance.PadBtnTop)
            {
                if (!_canroate || !_cando)
                    return;
                _canroate = false;
                _cando = false;
                RaycastHit2D hit1 = Physics2D.Raycast(max.transform.position, Vector2.up);
                RaycastHit2D hit4 = Physics2D.Raycast(max.transform.position, Vector2.down);
                RaycastHit2D hit2 = Physics2D.Raycast(max.transform.position, Vector2.left);
                RaycastHit2D hit3 = Physics2D.Raycast(max.transform.position, Vector2.right);
                if (FunctionOf3Dof.Instance.PadBtnTop)
                {
                    if (number != 0)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 0), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 0; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit1.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                }
                else if (FunctionOf3Dof.Instance.PadBtnRight)
                {
                    if (number != 3)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, -90), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 3; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit3.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                }
                else if (FunctionOf3Dof.Instance.PadBtnBotton)
                {
                    if (number != 2)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 180), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 2; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit4.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                }
                else if (FunctionOf3Dof.Instance.PadBtnLeft)
                {
                    if (number != 1)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 90), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 1; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit2.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                }
            }
        }


        private void Rotate(GameObject obj)
        {
            if (!_canroate||!_cando)
                return;
            _canroate = false;
            _cando = false;
            RaycastHit2D hit1 = Physics2D.Raycast(max.transform.position, Vector2.up);
            RaycastHit2D hit4 = Physics2D.Raycast(max.transform.position, Vector2.down);
            RaycastHit2D hit2 = Physics2D.Raycast(max.transform.position, Vector2.left);
            RaycastHit2D hit3 = Physics2D.Raycast(max.transform.position, Vector2.right);
            switch (obj.name)
            {
                case "up":
                    if (number != 0)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 0), 1f).OnComplete(()=> { _canmove = true; _cando = true;_canroate = true;number = 0; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit1.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                    break;
                case "down":
                    if (number != 2)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 180), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 2; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit4.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                    break;
                case "left":
                    if (number != 1)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, 90), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 1; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit2.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                    break;
                case "right":
                    if (number != 3)
                    {
                        Hide();
                        max.transform.DORotate(new Vector3(0, 0, -90), 1f).OnComplete(() => { _canmove = true; _cando = true; _canroate = true; number = 3; _canshow = true; });
                    }
                    else
                    {
                        _canmove = true; _canroate = true; _cando = true;
                        if (hit3.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                    break;
            }
            
        }

        private void Hide()
        {
            for (int i = 0; i < 3; i++)
            {
                max.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        private void Show()
        {
            for (int i = 0; i < 3; i++)
            {
                max.transform.GetChild(i).gameObject.SetActive(true);
            }
        }
        private void Move(GameObject obj)
        {
            MoveFunc();

           
        }

        void MoveFunc()
        {
            Debug.Log("Move~");
            if (!_cando||!_canClick)
                return;
            if (!_canmove)
            {
                Debug.Log("11111");
                if (max.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().rect.height > max.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().rect.height)
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                }
                else
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                }
                return;
            }
            _cando = false;
            _canmove = false;
            switch (number)
            {
                case 0:
                    temprec.DOAnchorPosY(temprec.anchoredPosition.y + 50, 0.35f).OnComplete(() => { _cando = true; _canmove = true; JugleVoice(); });
                    break;
                case 1:
                    temprec.DOAnchorPosX(temprec.anchoredPosition.x - 50, 0.35f).OnComplete(() => { _cando = true; _canmove = true; JugleVoice(); });
                    break;
                case 2:
                    temprec.DOAnchorPosY(temprec.anchoredPosition.y - 50, 0.35f).OnComplete(() => { _cando = true; _canmove = true; JugleVoice(); });
                    break;
                case 3:
                    temprec.DOAnchorPosX(temprec.anchoredPosition.x + 50, 0.35f).OnComplete(() => { _cando = true; _canmove = true; JugleVoice(); });
                    break;
            }
        }

        private void JugleVoice()
        {
            _canreplay = false;
            mono.StartCoroutine(WaitFrame(()=> 
            {
                _canreplay = true;
                if (_canmove)
                {
                    SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                    SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE,1,false);
                }
                if(!_canmove&&_cando)
                {
                    if(temprec.anchoredPosition.y>300f&&_canwin)
                    {
                        _canClick = false;
                        _canwin = false;
                        _canmove = false;
                        _cando = false;
                        shengli.SetActive(true);
                        mymask.SetActive(true);
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 2, false);
                        SpineManager.instance.DoAnimation(shengli, "animation3", false,
                            () => {
                                SpineManager.instance.DoAnimation(shengli, "animation4", false,
                    () =>
                                {
                                    shengli.SetActive(false);
                                    shengli.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                                    remake();
                                }
                    );
                            }
                            );
                        return;

                    }

                    if (max.transform.GetChild(1).GetChild(0).GetComponent<RectTransform>().rect.height > max.transform.GetChild(2).GetChild(0).GetComponent<RectTransform>().rect.height)
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                    }
                    else
                    {
                        SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 3, false);
                    }
                } }));
        }

        IEnumerator Wait(float time, Action callback = null)
        {
            yield return new WaitForSeconds(time);
            callback?.Invoke();
        }

        IEnumerator WaitFrame(Action callbcak =null)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            callbcak?.Invoke();
        }
        private void replay(GameObject obj)
        {
            if (_canClick && _cando&& _canreplay)
            {
                DOTween.Clear();
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "3", false);
                mono.StartCoroutine(Wait(0.2f,
                    () => { remake(); SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "4", false); _canClick = true; }
                    ));
            }
        }

        private void next(GameObject obj)
        {
            if (_canClick && _cando )
            {
                _canClick = false;
                SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "1", false);
                mono.StartCoroutine(Wait(0.2f,
                    () =>
                    {
                        SpineManager.instance.DoAnimation(obj.transform.GetChild(0).gameObject, "2", false);
                        controlBox.SetActive(false);
                        fh.SetActive(false);
                        nx.SetActive(false);
                        Bg.transform.GetChild(0).gameObject.SetActive(false);
                        Bg.transform.GetChild(1).gameObject.SetActive(false);
                        Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
                        ban.SetActive(true);
                        Max.SetActive(true);
                        max.SetActive(false);
                        mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, null,
                            () => { SoundManager.instance.ShowVoiceBtn(true); }
                            ));
                    }
                    ));
            }
        }
        private void remake()
        {
            max.transform.localPosition = curTrans.Find("maxpos").localPosition;
            for (int i = 0; i < max.transform.childCount; i++)
            {
                max.transform.GetChild(i).gameObject.SetActive(true);
            }
            number = 0;
            _canmove = true;
            _canroate = true;
            _cando = true;
            _canClick = true;
            _canreplay = true;
            _canwin = true;mymask.SetActive(false);
            max.transform.rotation = Quaternion.Euler(0, 0, 0);
        }

        private void Click(GameObject obj)
        {
            if (_canClick2)
            {
                obj.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
        private void GameInit()
        {
            _canClickThe3Dof = false;
            shengli.SetActive(false);
            shengli.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            Max.SetActive(false);
            max.SetActive(false);
            for (int i = 0; i < clickBox.transform.childCount; i++)
            {
                clickBox.transform.GetChild(i).GetChild(0).gameObject.SetActive(false);
            }
            _canClick2 = false;
            clickBox.SetActive(false);
            controlBox.SetActive(false);
            fh.SetActive(false);
            nx.SetActive(false);
            Bg.transform.GetChild(0).gameObject.SetActive(false);
            Bg.transform.GetChild(1).gameObject.SetActive(false);
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[1];
            ban.SetActive(true);
            level = 1;
            _canmove = true;
            _canroate = true;
            _canClick = true;
            _canwin = true;
            remake();
            for (int i = 0; i < max.transform.childCount; i++)
            {
                max.transform.GetChild(0).rotation = Quaternion.identity;
            }
            max.transform.rotation = Quaternion.Euler(0, 0, 0);
            tempvector3 = new Vector3(0, 0, 0);
            number = 0;
            talkIndex = 1;
            for (int i = 0; i < max.transform.childCount; i++)
            {
                max.transform.GetChild(i).gameObject.SetActive(true);
            }
            max.transform.GetChild(0).rotation = Quaternion.Euler(0,0,0);
        }



        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, true);
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => { Max.SetActive(false); isPlaying = false;
                mono.StartCoroutine(Wait(2f,()=>
                {
                    _canClickThe3Dof = true;
                    Bg.transform.GetChild(0).gameObject.SetActive(true);
                    Bg.transform.GetChild(1).gameObject.SetActive(true);
                    Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];
                    ban.SetActive(false);
                    controlBox.SetActive(true); fh.SetActive(true);
                    nx.SetActive(true);
                    _canupdate = true;
                    _cando = true;
                    max.SetActive(true);
                    RaycastHit2D hit1 = Physics2D.Raycast(max.transform.position, Vector2.up);
                    RaycastHit2D hit4 = Physics2D.Raycast(max.transform.position, Vector2.down);
                    RaycastHit2D hit2 = Physics2D.Raycast(max.transform.position, Vector2.left);
                    RaycastHit2D hit3 = Physics2D.Raycast(max.transform.position, Vector2.right);
                    if (number == 0)
                    {
                        SetRectTransformSize(_fre, new Vector2(8, (hit1.distance / offx - 50)));
                        SetRectTransformSize(_lre, new Vector2(8, hit2.distance / offx - 50));
                        SetRectTransformSize(_rre, new Vector2(8, hit3.distance / offx - 50));
                        if (hit1.distance < 210 * offx)
                        {
                            _canmove = false;
                        }
                    }
                }));
            }));
            ConnectAndroid.Instance.SendConnectData();


            FunctionOf3Dof.ClickAnyButtonDown = TriggerCallBack;


        }

        

        private void SetRectTransformSize(RectTransform trans, Vector2 newSize)
        {
            Vector2 oldSize = trans.rect.size;
            Vector2 deltaSize = newSize - oldSize;
            trans.offsetMin -= new Vector2(deltaSize.x * trans.pivot.x, deltaSize.y * trans.pivot.y);
            trans.offsetMax += new Vector2(deltaSize.x * (1f - trans.pivot.x), deltaSize.y * (1f - trans.pivot.y));

            trans.parent.GetChild(1).GetComponent<Text>().text = Mathf.Abs(Convert.ToInt32(newSize.y / 5)).ToString();
        }


        /// <summary>
        /// bell说话协程
        /// </summary>
        /// <param name="type"></param>
        /// <param name="clipIndex"></param>
        /// <param name="method_1"></param>
        /// <param name="method_2"></param>
        /// <param name="len"></param>
        /// <returns></returns>
        IEnumerator SpeckerCoroutine(GameObject speaker, SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (!speaker)
            {
                speaker = Max;
            }
            SoundManager.instance.SetShield(false);

            SpineManager.instance.DoAnimation(speaker, "daiji");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "daijishuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "daiji");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                ban.SetActive(false);
                Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
                clickBox.SetActive(true);
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 5,null,
                    () => { _canClick2 = true; }
                    ));
            }

            talkIndex++;
        }


        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        private bool OnAfter(int dragType, int index, int dropType)
        {

            if (dragType == dropType)
            {

            }
            return dragType == dropType;
        }

        private void OnBeginDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnDrag(Vector3 pos, int type, int index)
        {

        }

        private void OnEndDrag(Vector3 pos, int type, int index, bool isMatch)
        {
            if (!isMatch)
            {

            }
            else
            {
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONSOUND, 6);


            }
        }
    }
}
