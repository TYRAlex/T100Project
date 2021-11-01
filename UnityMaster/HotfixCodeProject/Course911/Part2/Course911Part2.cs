using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Experimental.PlayerLoop;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using DG.Tweening;


namespace ILFramework.HotClass
{
    public class Course911Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;
        private GameObject bell;
        private GameObject Bg;
        private BellSprites bellTextures;
        private GameObject btnTest;
        GameObject _mask;
        GameObject _wc;
        GameObject _p1;
        GameObject _p2;
        GameObject _dcgj;

        GameObject _restart;
        GameObject _next;

        Vector3 endpos = Vector3.zero;
        GameObject _qp;

        //CheckRange checkRange;

        GameObject _ps;
        GameObject _bd;
        GameObject _time;
        GameObject _blood;


        float score = 0;
        float blood = 0;
        float time = 0;
        bool isstart = false;
        mILDrager _miLDrager;
        MonoScripts _monoScripts;


        List<Vector2> _startPos;

        List<Vector3> _endPos;

        Dictionary<GameObject, Vector3> _allVirusDic;

        Dictionary<GameObject, bool> _isDoLerp;
        List<GameObject> _bingDu;
        Vector3 _randomV3;

        private int level = 0;

        bool _isEnd;


        GameObject _shegnLi;
        void Start(object o)
        {

            curGo = (GameObject)o;
            curTrans = curGo.transform;
            //用于测试课程环节的切换，测试完成注意隐藏

            //Ani 和 Win 不太一样
            btnTest = curTrans.Find("btnTest").gameObject;
            _mask = curTrans.GetGameObject("mask");
            _wc = curTrans.GetGameObject("Ps/wc");
            _next = _wc.transform.GetGameObject("next");
            _p1 = curTrans.GetGameObject("Ps/p1");
            _p2 = curTrans.GetGameObject("Ps/p2");
            _dcgj = curTrans.GetGameObject("Ps/dcgj");

            _restart = curTrans.GetGameObject("restart");
            _ps = curTrans.GetGameObject("Ps");
            _bd = _ps.transform.GetGameObject("bd");
            _blood = _ps.transform.GetGameObject("blood");
            _time = _ps.transform.GetGameObject("time");
            endpos = Vector3.zero;
            _qp = curTrans.GetGameObject("Ps/qp");
            _monoScripts = _qp.transform.GetGameObject("ppq").GetComponent<MonoScripts>();
            _miLDrager = _qp.GetComponent<mILDrager>();

            _bingDu = new List<GameObject>();
            _isEnd = false;

            _endPos = new List<Vector3>(30);
            _allVirusDic = new Dictionary<GameObject, Vector3>();
            _isDoLerp = new Dictionary<GameObject, bool>();

            _shegnLi = _ps.transform.GetGameObject("shengli");


            for (int i = 0; i < 13; i++)
            {
                _bingDu.Add(_ps.transform.GetChild(i).gameObject);
                _allVirusDic.Add(_ps.transform.GetChild(i).gameObject, Vector3.zero);
                _isDoLerp.Add(_ps.transform.GetChild(i).gameObject, false);
            }

            //***************************************************************



            _monoScripts.UpdateCallBack = Update;
            score = 0;
            blood = 6;
            time = 30;
            isstart = false;
            level = 0;

            _startPos = new List<Vector2>();
            Util.AddBtnClick(_p1, ShowBig);
            Util.AddBtnClick(_p2, ShowBig);
            Util.AddBtnClick(_restart, Sence);
            Util.AddBtnClick(_next, Next);
            Util.AddBtnClick(btnTest, ReStart);

            _miLDrager.SetDragCallback(DragStart, Draging, DragEnd, null);
            btnTest.SetActive(false);
            ReStart(btnTest);



        }

        void SetRandomArrayList()
        {
            for (int i = 0; i < 30; i++)
            {
                Vector3 target = new Vector3(Random.Range(0, Screen.width), Random.Range(0, Screen.height), 0);
                //Debug.LogError(target);
                _endPos.Add(target);
                //_endPos[i] = target;
            }
            Debug.LogError(_endPos.Count);
        }

        Vector3 GetRandomArrayListVector()
        {
            Vector3 targetVector = Vector3.zero;
            int target = Random.Range(0, _endPos.Count - 1);

            targetVector = _endPos[target];
            // Debug.LogError("随机数为：" + target + "随机位置为：" + _endPos[target]);
            return targetVector;
        }


        float timer = 0;

        void Update()
        {
            timer += Time.deltaTime;
            if (isstart)
            {
                if (timer >= 1)
                {
                    time -= 1;
                    _time.GetComponentInChildren<Text>().text = time.ToString();
                    timer = 0;
                }
            }

            if (time <= 0)
            {
                isstart = false;
            }


            BingDuStart();
        }

        IEnumerator Wait(float time, Action method = null)
        {
            yield return new WaitForSeconds(time);
            if (method != null)
            {
                method();
            }
        }

        void ReStart(GameObject obj)
        {
            mono = curGo.GetComponent<MonoBehaviour>();
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            bell = curTrans.Find("bell").gameObject;
            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            level = 0;
            Bg.GetComponent<RawImage>().texture = bellTextures.texture[0];

            bell.transform.localScale = new Vector3(1.1f, 1.1f);
            bell.transform.GetRectTransform().anchoredPosition = new Vector3(450, -134);

            bell.Show();
            _time.GetComponentInChildren<Text>().text = time.ToString();
            _bd.GetComponentInChildren<Text>().text = score.ToString();
            _blood.transform.GetGameObject("number").GetComponent<Text>().text = blood.ToString();
            _next.Hide();
            _mask.Hide();
            _wc.Hide();
            _p1.Hide();
            _p2.Hide();
            _restart.Hide();
            _time.Hide();
            _bd.Hide();
            _blood.Hide();
            _qp.Hide();
            _isEnd = false;
            Hide(true);

            _shegnLi.Hide();

            _startPos.Add(new Vector2(-1200, 800));
            _startPos.Add(new Vector2(1200, 800));
            _startPos.Add(new Vector2(-1200, -800));
            _startPos.Add(new Vector2(1200, -800));
            _startPos.Add(new Vector2(-1200, 0));
            _startPos.Add(new Vector2(1200, 0));
            _startPos.Add(new Vector2(0, 800));
            _startPos.Add(new Vector2(0, -800));

            SetRandomArrayList();



            Debug.Log("StopDoTweenAndCoro______________________Start");
            //StopDoTweenAndCoro();
            Debug.Log("StopDoTweenAndCoro______________________End");
            SpineManager.instance.DoAnimation(_wc, "1", false);
            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
            GameStart();
            btnTest.SetActive(false);
        }

        void GameStart()
        {

            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 0,
                () => { Debug.Log("同学们知道，我们的手上有多少细菌吗？"); }, () => { SoundManager.instance.ShowVoiceBtn(true); }));
        }




        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null,
            Action method_2 = null, float len = 0)
        {
            SoundManager.instance.SetShield(false);
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }

            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "daijishuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "daiji");
            SoundManager.instance.SetShield(true);
            if (method_2 != null)
            {
                method_2();
            }
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {
                bell.Hide();
                _wc.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 1,
                    () =>
                    {
                        _mask.Show();

                        Debug.Log("一双未清洗的手上有多达80万个细菌！科学家还发现，平均每只手上的细菌种类有150种，它们大部分都是无害的。");
                        SpineManager.instance.DoAnimation(_wc, "2", true,
                            () => { SoundManager.instance.ShowVoiceBtn(true); });
                    },
                    () => { _mask.Hide(); }));
            }

            if (talkIndex == 2)
            {
                _p1.Show();
                _p2.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 2,
                    () =>
                    {
                        _mask.Show();
                        Debug.Log("手上比较常见的致病菌有金黄色葡萄球菌和大肠杆菌。");
                        SpineManager.instance.DoAnimation(_wc, "3", false,
                            () => { SpineManager.instance.DoAnimation(_wc, "4", true); });
                    }, () => { _mask.Hide(); }));
            }

            if (talkIndex == 3)
            {
                _p1.Hide();
                _p2.Hide();
                _dcgj.Hide();
                _restart.Hide();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 5,
                    () =>
                    {
                        Debug.Log("我的手太脏了，请帮我清洗干净吧！");
                        SpineManager.instance.DoAnimation(_wc, "4", false,
                            () => { SpineManager.instance.DoAnimation(_wc, "5", false); });
                        _next.Show();
                    }));
            }

            talkIndex++;
        }

        private void BtnPlaySound()
        {
            SoundManager.instance.PlayClip(9);
        }

        //失败激励语音
        private void BtnPlaySoundFail()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(0, 4), false);
        }

        //成功激励语音
        private void BtnPlaySoundSuccess()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, Random.Range(4, 13), false);
        }


        void Sence(GameObject obj)
        {
            _p1.Show();
            _p2.Show();
            _dcgj.Hide();
            _wc.Show();
            _restart.Hide();
            SoundManager.instance.ShowVoiceBtn(true);
        }


        private void ShowBig(GameObject obj)
        {
            SoundManager.instance.ShowVoiceBtn(false);
            if (obj == _p1)
            {
                _p1.Hide();
                _p2.Hide();

                _dcgj.Show();
                _mask.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 3, () => { },
                    () => { _mask.Hide(); }));
                SpineManager.instance.DoAnimation(_dcgj, "2", false,
                    () =>
                    {
                        _restart.Show();
                        SpineManager.instance.DoAnimation(_dcgj, "2", true, () => { });
                    });
            }

            if (obj == _p2)
            {
                _p1.Hide();
                _p2.Hide();

                _dcgj.Show();
                _mask.Show();
                mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.VOICE, 4, () => { },
                    () => { _mask.Hide(); }));
                SpineManager.instance.DoAnimation(_dcgj, "5", false,
                    () =>
                    {
                        _restart.Show();
                        SpineManager.instance.DoAnimation(_dcgj, "5", true);
                    });
            }
        }


        void Next(GameObject obj)
        {
            Hide(false);
            _wc.Hide();
            _dcgj.Hide();
            _p1.Hide();
            _p2.Hide();
            _restart.Hide();


            //BingDuStart();

            Bg.GetComponent<RawImage>().texture = bellTextures.texture[2];
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 0, true);
            _time.Show();
            _bd.Show();
            _blood.Show();
            _blood.transform.GetGameObject("number").GetComponent<Text>().text = blood.ToString();
            _qp.Show();
            _shegnLi.Hide();
            _bd.GetComponentInChildren<Text>().text = 0.ToString();
        }



        void StartDoTweenAndCoro()
        {
            //checkRange.SendGameMoveMessage();
            /*  for (int i = 0; i < checkRange.GameRectTransform.Count - 2; i++)
              {
                  var item = checkRange.GameRectTransform[i];
                  if (item.gameObject.activeSelf)
                  {
                      item.gameObject.GetComponent<RandomMove>().StartMyUpdate();
                  }
                  else
                  {
                      mono.StopAllCoroutines();
                  }
              }*/
        }
        void StopDoTweenAndCoro()
        {
            Debug.Log("StopDoTweenAndCoro______________________1");

            //checkRange.SendGameStopMoveMessage();
            /*     for (int i = checkRange.GameRectTransform.Count - 2; i >= 0; i--)
                 {
                     var item = checkRange.GameRectTransform[i];

                     Debug.Log("StopDoTweenAndCoro______________________2");
                     //item.GetComponent<RandomMove>().StopMyUpdate();

                     item.GetComponent<RandomMove>().start = false;
                     item.GetComponent<RandomMove>().KillTwAni();
                     Debug.Log("StopDoTweenAndCoro______________________3");

                 }*/
            Debug.Log("StopDoTweenAndCoro______________________4");
        }

        void Test()
        {
            /*for (int i = 0; i < checkRange.GameRectTransform.Count; i++)
            {
                var item = checkRange.GameRectTransform[i];
                var randomMove = item.GetComponent<RandomMove>();
                if (randomMove != null)
                {
                    randomMove.TwAni();
                }

            }*/
        }


        void Continue()
        {
            _blood.transform.GetGameObject("number").GetComponent<Text>().text = blood.ToString();
            time = 30;
            score = 0;
            blood -= 1;
            if (blood == 0)
            {
                _mask.Show();
            }
            _blood.transform.GetGameObject("number").GetComponent<Text>().text = blood.ToString();
            _bd.GetComponentInChildren<Text>().text = score.ToString();
            _time.GetComponentInChildren<Text>().text = time.ToString();
            isstart = true;
        }


        void Hide(bool isShow)
        {
            if (isShow == true)
            {
                for (int i = 0; i < _ps.transform.childCount; i++)
                {
                    _ps.transform.GetChild(i).gameObject.Hide();
                }
            }
            else if (isShow == false)
            {
                for (int i = 0; i < _ps.transform.childCount; i++)
                {
                    _ps.transform.GetChild(i).gameObject.Show();
                }
            }
        }


        private void DragStart(Vector3 arg1, int arg2, int arg3)
        {
            if (level == 0)
            {
                isstart = true;
                level++;
            }

            if (time <= 0)
                Continue();
            if (blood == 0)
            {
                Debug.Log("Ending        ");
                isstart = false; 
                _shegnLi.Show();
                SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONVOICE, 9, false);
                Debug.Log("这是结束动画animation3");
                SpineManager.instance.DoAnimation(_shegnLi, "animation3", false, () => { Debug.Log("这是结束动画"); SpineManager.instance.DoAnimation(_shegnLi, "animation4", false); });
                _mask.Show();
                //bell.Show();
            }
        }


        private void DragingTest(Vector3 arg1, int arg2, int arg3)
        {
            var rect = curTrans.GetRectTransform("Ps/qp");
            /* checkRange.GetNewestMyVertexLocalPosition(rect);

             for (int i = checkRange.GameRectTransform.Count - 2; i >= 0; i--)
             {

                 var item = checkRange.GameRectTransform[i];

                 var name = item.name;

                 var oPos = checkRange.VertexPosDics[name];

                 var mPos = checkRange.MyVertexPos;

                 bool isOverstep = checkRange.IsOverstep(name, oPos, mPos);

                 if (isOverstep)//||checkRange.IsDic checkRange.IsDic  isOverstep
                 {

                     Debug.Log("name:   " + name);

                     item.GetComponent<RandomMove>().StopMyUpdate();
                     item.localPosition = _startPos[UnityEngine.Random.Range(0, _startPos.Count)];

                     SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);

                     if (i < 3)
                     {

                         score += 5;
                     }
                     else
                     {

                         score++;
                     }


                     _bd.GetComponentInChildren<Text>().text = score.ToString();

                     SpineManager.instance.DoAnimation(_qp.transform.GetChild(0).gameObject, "qp2", false,
                         () =>
                         {

                             SpineManager.instance.DoAnimation(_qp.transform.GetChild(0).gameObject, "qp", false);
                         });
                     // mono.StartCoroutine(Wait(0.5f, () => { item.GetComponent<RandomMove>().TwAni(); }));
                     item.GetComponent<RandomMove>().StartMyUpdate();
                 }

             }*/
        }

        void DragEnd(Vector3 arg1, int arg2, int arg3s, bool isend)
        {
            if (!isstart) isstart = false;
        }

        void InitCheckRange()
        {
            /*checkRange.GameRectTransform.Clear();
            for (int i = 0; i < 13; i++)
            {
                checkRange.GameRectTransform.Add(_ps.transform.GetChild(i).GetRectTransform());
            }

            checkRange.DragGameObjec = _ps.transform.GetChild(13).GetRectTransform();*/
        }











        void Draging(Vector3 arg1, int arg2, int arg3)
        {
            if (isstart)
            {
                for (int i = _bingDu.Count - 1; i >= 0; i--)
                {
                    var item = _bingDu[i];
                    var clear = IsOverstep(_qp, item);
                    if (clear)
                    {

                        Debug.Log("Kill");
                        item.transform.localPosition = _startPos[UnityEngine.Random.Range(0, _startPos.Count)];
                        //mono.StartCoroutine(Wait(0.5f, () => { TwAni2(item); }));
                        SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 0, false);
                        if (i < 5)
                        {

                            score += 5;
                        }
                        else
                        {
                            score += 10;
                        }


                        _bd.GetComponentInChildren<Text>().text = score.ToString();
                        SpineManager.instance.DoAnimation(_qp.transform.GetChild(0).gameObject, "qp2", false,
                            () =>
                            {

                                SpineManager.instance.DoAnimation(_qp.transform.GetChild(0).gameObject, "qp", false);
                            });
                        item.transform.localPosition = _startPos[UnityEngine.Random.Range(0, _startPos.Count)];
                    }

                }
            }
        }



        //清除
        IEnumerator Check()
        {
            yield return new WaitForSeconds(0.01f);
            foreach (var item in _bingDu)
            {
                IsOverstep(_qp, item);
            }

        }

        //判断是否清除
        public bool IsOverstep(GameObject my, GameObject oth)
        {
            return (Vector3.Distance(my.transform.localPosition, oth.transform.localPosition) < 150);
        }



        //开始运动
        void BingDuStart()
        {
            if (Bg.GetComponent<RawImage>().texture == bellTextures.texture[2])
            {
                for (int i = 0; i < _bingDu.Count; i++)
                {
                    TwAni2(_bingDu[i]);
                }
            }
        }

        //运动
        void TwAni2(GameObject go)
        {
            if (_isDoLerp != null)
            {
                foreach (var item in _isDoLerp.Keys)
                //for (int i = 0; i < _isDoLerp.Keys.Count; i++)         
                {
                    //var item = _isDoLerp[];
                    if (go == item)
                    {
                        if (!_isDoLerp[item])
                        {
                            _allVirusDic[go] = GetRandomArrayListVector();
                            //Debug.Log(item+"    "+_allVirusDic[go]);
                            _isDoLerp[item] = true;
                        }
                        if (go.transform.position != _allVirusDic[go])
                        {
                            go.transform.position = Vector3.Lerp(go.transform.position, _allVirusDic[go], 0.005f);
                        }

                        if (Vector3.Distance(go.transform.position, _allVirusDic[go]) <= 5)
                        {
                            _isDoLerp[item] = false;
                        }

                    }
                }
            }
            /*  if (!_islerp) 
              {
                  _allVirusDic[go] = GetRandomArrayListVector();
                  _islerp = true;
              }
              if (go.transform.localPosition != _allVirusDic[go])
              {
                  go.transform.localPosition = Vector3.Lerp(go.transform.localPosition, _allVirusDic[go], 0.0004f);
              }

              if (Vector3.Distance(go.transform.localPosition, _allVirusDic[go]) <= 5)
              {
                  _islerp = false;
              }*/
        }




        //生成随机运动点
        private Vector3 RandomV3()
        {
            _randomV3.x = UnityEngine.Random.Range(-960, 920);
            _randomV3.y = UnityEngine.Random.Range(-540, 480);
            return _randomV3;
        }



        IEnumerator Delay(float delay, Action action)
        {
            yield return new WaitForSeconds(delay);
            action?.Invoke();
        }
    }
}