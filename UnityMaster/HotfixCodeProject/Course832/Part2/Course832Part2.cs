using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
namespace ILFramework.HotClass
{
    public class Course832Part2
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        private GameObject bell;
        private GameObject _bellYingZi;

        private RawImage _xuBg;
        private GameObject _mask;

        private GameObject _spine2;
        private RectTransform _spine2ParentRect;
        private GameObject _spine2Parent;
        private GameObject _dargLine;

       
        private mILDrager _mILDrager;
        private GameObject _goStraight;
        private GameObject _left;
        private GameObject _right;

        private GameObject _nextOnClick;
        private RectTransform _pointRect;


        private List<Vector3> _vector3LeftPos;
        private List<Vector3> _vector3RightPos;

       
   

        void Start(object o)
        {
            curGo = (GameObject)o;
            Transform curTrans = curGo.transform;
            mono = curGo.GetComponent<MonoBehaviour>();
        
            bell = curTrans.Find("bells/bell").gameObject;
       
            _xuBg = curTrans.Find("Bg/xuBg").GetComponent<RawImage>();
            _mask = curTrans.Find("mask").gameObject;
          
            _spine2ParentRect = curTrans.Find("Spines/Spine2Parent").GetComponent<RectTransform>();

          
              _spine2Parent = curTrans.Find("Spines/Spine2Parent").gameObject;
            _spine2 = _spine2Parent.transform.Find("Spine2").gameObject;
            _dargLine = curTrans.Find("OnClicks/DargLine").gameObject;
            _pointRect = curTrans.Find("OnClicks/Point").GetComponent<RectTransform>();
       
            _mILDrager = curTrans.Find("OnClicks/Point").GetComponent<mILDrager>();
            _mILDrager.SetDragCallback(null, PointDrag,PointDragEnd);
            _goStraight = curTrans.Find("Spines/Sign/GoStraight").gameObject;
            _left = curTrans.Find("Spines/Sign/Left").gameObject;
            _right = curTrans.Find("Spines/Sign/Right").gameObject;

            _nextOnClick = curTrans.Find("OnClicks/NextOnClick").gameObject;
            _bellYingZi = curTrans.GetGameObject("bells/bellYingZi");

            GameInit();
            GameStart();

                     
        }

      

        void GameInit()
        {

            tempDrage = false;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            talkIndex = 1;
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            bell.Show();
            _bellYingZi.Show();
            _xuBg.gameObject.Show();
            _xuBg.color = new Color(1, 1, 1, 1);

            _goStraight.Hide();
            _left.Hide();
            _right.Hide();

            _dargLine.Hide();
            _pointRect.gameObject.Hide();

         //   _nextOnClick.transform.SetAsFirstSibling();
            _nextOnClick.gameObject.Hide();

            _spine2Parent.Hide();
          //  _spine2ParentRect.parent.SetSiblingIndex(1);

            _spine2ParentRect.localRotation = Quaternion.Euler(0, 0, 0);
            SetBellPos(new Vector2(960, 150));

            _vector3LeftPos = new List<Vector3>();
            _vector3RightPos = new List<Vector3>();

            SetLeftMovePos();
            SetRightMovePos();

            DOTween.KillAll();
            Reset();
        }

        void GameStart()
        {
            SoundManager.instance.PlayClip(SoundManager.SoundType.COMMONBGM, 1, true);
            mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND, 0,()=> { _mask.Show(); }, () => {
                _mask.Hide();
                BgAni();
                bell.Hide();
                _bellYingZi.Hide();
                _spine2Parent.Show();
               //
                _goStraight.Show();
                _dargLine.Show();
                _pointRect.gameObject.Show();
                _nextOnClick.Show();
               // 
            }));

            PointerClickListener.Get(_nextOnClick).onClick = null;
            PointerClickListener.Get(_nextOnClick).onClick = OnNextOnClick;
        }


        bool tempDrage;

        private void CtrlVoiceAndSpineAni(bool isDraging)
        {

            if (isDraging)
            {
                if (tempDrage)
                    return;

                SpineManager.instance.DoAnimation(_spine2, "animation2", true);
                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, true);
                tempDrage = true;
            }
            else
            {
                tempDrage = false;
                SpineManager.instance.DoAnimation(_spine2, "animation", false);
                SoundManager.instance.StopAudio(SoundManager.SoundType.VOICE);
               // SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, true);
            }
        }
       

        private void OnNextOnClick(GameObject go)
        {
            go.Hide();
            SoundManager.instance.ShowVoiceBtn(true);
            PointerClickListener.Get(go).onClick = null;
        }


        private void PointDragEnd(Vector3 arg1, int arg2, int arg3, bool arg4)
        {
            CtrlVoiceAndSpineAni(false);
        }

        private void PointDrag(Vector3 arg1, int arg2, int arg3)
        {
            CtrlVoiceAndSpineAni(true);

            var x =(int)Math.Round(arg1.x);

            bool isGtZero = x >= 0;
          
            if (isGtZero)
            {
                double percent = Math.Truncate(x * 1.00 / 535 * 100.0);                     
                var reslut = 180 * (percent / 100);              
                var angle =  Math.Round(-reslut, 0);         
                if (angle == -90)
                {
                    _right.Show();
                }
                else if (angle == 0)
                {
                    _goStraight.Show();
                }
                else
                {
                    _left.Hide();
                    _goStraight.Hide();
                    _right.Hide();
                }                
                _spine2ParentRect.localRotation = Quaternion.Euler(0, 0, (float)angle);
                            
            }
            else
            {
                
                double percent = Math.Truncate(x * 1.00 / -535 * 100.0);             
                var reslut =  180 * (percent / 100);
                var angle = Math.Round(reslut, 0);
                if ( angle == 90)
                {
                    _left.Show();
                }
                else if (angle == 0)
                {
                    _goStraight.Show();
                }
                else
                {
                    _left.Hide();
                    _goStraight.Hide();
                    _right.Hide();
                }
                _spine2ParentRect.localRotation = Quaternion.Euler(0, 0, (float)angle);

            }          
        }

        /// <summary>
        /// 背景模糊动画
        /// </summary>
        private void BgAni()
        {
           var  bgTw = _xuBg.DOColor(new Color(1, 1, 1, 0), 0.5f);
            DOTween.Sequence().Append(bgTw).
                              AppendCallback(() =>
                              {
                                  _xuBg.gameObject.Hide();
                              });        
        }


        private void SetBellPos(Vector2 pos)
        {
            bell.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);
            _bellYingZi.GetComponent<RectTransform>().anchoredPosition = new Vector2(pos.x, pos.y);
        }

        //bell说话协程
        IEnumerator SpeckerCoroutine(SoundManager.SoundType type, int clipIndex, Action method_1 = null, Action method_2 = null, float len = 0)
        {
            if (len > 0)
            {
                yield return new WaitForSeconds(len);
            }
            float ind = 0;
            ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(bell, "DAIJIshuohua");
            if (method_1 != null)
            {
                method_1();
            }

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(bell, "DAIJI");

            if (method_2 != null)
            {
                method_2();
            }
        }
        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            switch (talkIndex)
            {
                case 1:
                  //  _nextOnClick.transform.SetAsLastSibling();

                    _goStraight.Hide();
                    _left.Hide();
                    _right.Hide();

                    CtrlVoiceAndSpineAni(true);

                    _spine2ParentRect.localRotation = Quaternion.Euler(new Vector3(0,0,0));
                    _pointRect.localPosition = new Vector2(0,-334);
                 //   _spine2ParentRect.parent.SetSiblingIndex(2);

                    SetBellPos(new Vector2(214, -54));
                    bell.Show();
                    _bellYingZi.Show();
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,1,
                    ()=>{
                        _mask.Show();
                        mono.StartCoroutine(Delay(0.5f,()=> {
                            GoStraightAni(Left);

                            void Left()
                            {
                                LeftAni(Right);
                            }

                            void Right()
                            {
                                RightAni(()=> { CtrlVoiceAndSpineAni(false); _mask.Hide(); });
                            }
                        }));                
                    },
                    ()=>{                      
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
                    break;
                case 2:
                    mono.StartCoroutine(SpeckerCoroutine(SoundManager.SoundType.SOUND,2,()=> { _mask.Show(); },()=> { _mask.Hide(); }));
                    break;
                case 3:
                    break;

            }
            talkIndex++;
        }


        private IEnumerator Delay(float delay,Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }

        private void BtnPlaySound() {
            SoundManager.instance.PlayClip(9);
        }

        private void BtnPlaySoundF()
        {
            SoundManager.instance.PlayClip(6);
        }


        private void SetLeftMovePos()
        {
            List<Vector3> temp1 = new List<Vector3>();

            for (int i = 0; i >= -400; i--)
            {
                if (i == 0)
                {
                    Vector3 v3 = new Vector3(0, 0, 0);
                    temp1.Add(v3);
                }
                else if (i == -400)
                {
                    Vector3 v3 = new Vector3(-400, 400, 0);
                    temp1.Add(v3);
                }
                else
                {
                    var y = -i;
                    Vector3 v3 = new Vector3(i, (float)Math.Round(Math.Sqrt(Math.Pow(400, 2) - Math.Pow(400 - y, 2)), 0), 0);

                    bool isContain = temp1.Contains(v3);
                    if (!isContain)
                        temp1.Add(v3);
                }
            }

            List<Vector3> temp2 = new List<Vector3>();

            for (int i = temp1.Count - 1; i >= 0; i--)
            {

                var pos = temp1[i];
                var offset = 400 + pos.x;
                var x = -400 - offset;

                Vector3 v3 = new Vector3(x, pos.y, 0);
                temp2.Add(v3);

            }
            _vector3LeftPos = temp1.Concat(temp2).ToList();
        }

        private void SetRightMovePos()
        {
            for (int i = _vector3LeftPos.Count - 1; i >= 0; i--)
            {
                var pos = _vector3LeftPos[i];
                var x = pos.x + 800;

                Vector3 v3 = new Vector3(x, pos.y, 0);
                _vector3RightPos.Add(v3);
            }
        }


     
     
        private void LeftAni(Action callBack)
        {                 
          var  leftTw = _spine2ParentRect.DOLocalPath(_vector3LeftPos.ToArray(), 8.5f, PathType.CatmullRom);
          var  leftAngelTw = _spine2ParentRect.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0, 0, 180)), 8.5f);
          var  leftPointTw= _pointRect.DOLocalMoveX(-535, 8.5f);
            DOTween.Sequence().Append(leftTw).Join(leftAngelTw).Join(leftPointTw)        
                            .OnComplete(() => {
                                Reset();
                                callBack?.Invoke();
                            });         
        }
       
        private void RightAni(Action callBack)
        {
           var rightTw = _spine2ParentRect.DOLocalPath(_vector3RightPos.ToArray(), 8.5f, PathType.CatmullRom);
           var rightAngelTw = _spine2ParentRect.DOLocalRotateQuaternion(Quaternion.Euler(new Vector3(0, 0, -180)), 8.5f);
           var rightPointTw = _pointRect.DOLocalMoveX(535, 8.5f);
            DOTween.Sequence().Append(rightTw).Join(rightAngelTw).Join(rightPointTw)
                              .OnComplete(()=> {
                                  Reset();
                                  callBack?.Invoke();
                             });
        }

        private void GoStraightAni(Action callBack)
        {
            _spine2ParentRect.DOLocalMoveY(800f, 3.0f).OnComplete(()=> {
                Reset();
                callBack?.Invoke();
            }); 
        }

        private void Reset()
        {
            _spine2ParentRect.localPosition = new Vector3(0, 0, 0);
            _spine2ParentRect.localRotation = Quaternion.Euler(0, 0, 0);
            _pointRect.localPosition = new Vector3(0, -334, 0);
        }
    }
}
