using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace ILFramework.HotClass
{
    public class Course936Part1
    {

         private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;
        private Transform PAN; private Transform PAN2;
        private Transform Pan;
        private Transform Pan2;
        private GameObject Max;
        private Transform left;
        private Transform right;

         private float z1;
 
        private bool a;
        private Vector2 b;
        private Vector2 c;
        private int EndIndex;
        private float x;
        private mILDrager drag;
        private mILDrager drag2;
        private Vector2 pos;
        private Transform new1;
        private Transform textList;
        private Transform biao;
        private Text biaoText;
        private string yushuzhengString;
        private bool isfirstString;
        private bool isleft;
        private bool isTwo;
        private Transform canDrag;
        private string numString;

        private Transform pointer;
        private Transform CWBtn;
        private Transform _Mask;
        private Transform Biao;
        private bool _canCorrect;
        private GameObject pan1;
        private GameObject pan2;

        private float ScreenProportion;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();
            new1 = curTrans.Find("new1");
            Max = curTrans.Find("bell").gameObject;
            Pan = curTrans.Find("1/PointerRoatePoint");
            Pan2 = curTrans.Find("2/PointerRoatePoint");
            pointer = curTrans.Find("2/PointerRoatePoint/1/1");
            PAN = curTrans.Find("1");
            PAN2 = curTrans.Find("2");
            pan1 = curTrans.Find("11").gameObject;
            pan2 = curTrans.Find("22").gameObject;
            drag = curTrans.Find("1").GetComponent<mILDrager>();
            drag2 = curTrans.Find("2").GetComponent<mILDrager>();
            textList = curTrans.Find("biao/textList");
            biao = curTrans.Find("biao/biao");
            Biao = curTrans.Find("biao");

          CWBtn = curTrans.Find("cw");
            numString = "";
            biaoText = biao.Find("Text").GetComponent<Text>();
            biaoText.text = "";
            yushuzhengString = "";
            _Mask = curTrans.Find("Mask");
            a = true;
            b =  new Vector2(0, 0);
            c = new Vector2(0, 0);
            z1 = Pan.rotation.z;
            EndIndex = 0;
            x = 0;
            isfirstString = true;
            isleft = false;
            isTwo = true;
            canDrag = curTrans.Find("canDrag");
            left = curTrans.Find("left");
            right = curTrans.Find("right");
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);
             // pos = new Vector2(200, 189);
            pos = new1.position;
            ScreenProportion = 1920 / Screen.width;
            _canCorrect = true;
            PAN.GetComponent<mILDrager>().isActived = true;
            PAN2.GetComponent<mILDrager>().isActived = true;
            GameInit();
            AddBtnOnClick();
            GameStart();
        }







        private void GameInit()
        {
            EndIndex = 0;
            Input.multiTouchEnabled = false;
            GuiWei();
            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM,0, true);
            pan1.gameObject.SetActive(true);pan2.gameObject.SetActive(true);
        
            pan1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            pan2.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(pan1, "b1d", false);
            SpineManager.instance.DoAnimation(pan2, "b2d", false);
            PAN.gameObject.SetActive(false); PAN2.gameObject.SetActive(false);
            PAN2.GetRectTransform().anchoredPosition = new Vector2(1420,550);
            Biao.GetRectTransform().anchoredPosition = new Vector2(1503,541);
 

            textList.GetChild(0).GetComponent<Text>().text = "";
            textList.GetChild(1).GetComponent<Text>().text = "";
            textList.GetChild(2).GetComponent<Text>().text = "";
            textList.GetChild(3).GetComponent<Text>().text = "";
            OnClickResetting();

            _Mask.gameObject.SetActive(true);
            canDrag.gameObject.SetActive(false);
            mono.StartCoroutine(SpeckerCoroutine(Max.gameObject,SoundManager.SoundType.SOUND,0,null,()=> 
            {
              
                SoundManager.instance.ShowVoiceBtn(true);
            }));

            left.gameObject.SetActive(false);
            right.gameObject.SetActive(false);

            drag.SetDragCallback(DragStart, Draging, DragEnd);
            drag2.SetDragCallback(DragStart, Draging2, DragEnd2);
         
        }
        private void DragStart(Vector3 position, int type, int index) 
        {
            b = Input.mousePosition;
        }
        private void Draging(Vector3 position,int type, int index) 
        {
            if (a)
            {
                c = Input.mousePosition;
                a = false;
             //   Pan.rotation = Quaternion.Euler(new Vector3(0,0, -VectorAngle((b - pos), (c - pos))));
                Pan.Rotate(0, 0, -VectorAngle((b - pos), (c - pos)));
                x+= VectorAngle((b - pos), (c - pos));
              
            }
            else 
            {
                a = true;
                b = Input.mousePosition;
               // Pan.rotation = Quaternion.Euler(new Vector3(0, 0, -VectorAngle((c - pos), (b - pos))));
                Pan.Rotate(0, 0, -VectorAngle((c - pos), (b - pos)));
                x += VectorAngle((c - pos), (b - pos));
            }

            //   Debug.LogError("c:"+c);
            //    Debug.LogError("b:" + b);
            //   Debug.LogError("b-pos"+(b-pos));
            // Debug.LogError("c-pos" + (c - pos));
           Debug.Log(x);
        }
        private void Draging3(Vector3 position, int type, int index)
        {
            if (a)
            {
                c = Input.mousePosition;
              
                //   Pan.rotation = Quaternion.Euler(new Vector3(0,0, -VectorAngle((b - pos), (c - pos))));
               // Pan.Rotate(0, 0, -VectorAngle((b - pos), (c - pos)));
                Pan.transform.rotation = Quaternion.Euler(Pan.rotation.x,Pan.rotation.y,VectorAngle(b,c));
              //  x += VectorAngle((b - pos), (c - pos));
            }
          

            //   Debug.LogError("c:"+c);
            //    Debug.LogError("b:" + b);
            //   Debug.LogError("b-pos"+(b-pos));
            // Debug.LogError("c-pos" + (c - pos));
            Debug.Log(x);
        }
        private void Draging2(Vector3 position, int type, int index)
        {
            if (a)
            {
                c = Input.mousePosition;
                a = false;
                Pan2.Rotate(0, 0, -VectorAngle((b - pos), (c - pos)));
                x += VectorAngle((b - pos), (c - pos));
            }
            else
            {
                a = true;
                b = Input.mousePosition;
                Pan2.Rotate(0, 0, -VectorAngle((c - pos), (b - pos)));
                x += VectorAngle((c - pos), (b - pos));
            }

            //   Debug.LogError("c:"+c);
            //    Debug.LogError("b:" + b);
            //   Debug.LogError("b-pos"+(b-pos));
            
            Debug.Log(-VectorAngle((b - pos), (c - pos)));
        }
        private void DragEnd(Vector3 position, int type, int index, bool isMatch)
        {
        
            PAN.GetComponent<mILDrager>().isActived = false;
            if (EndIndex < 4)
            {
                if (x >= 0)
                {
                    isleft = false;
                    Debug.Log("右边");

                }
                else
                {
                    isleft = true;
                    Debug.Log("左边");

                }
                float yushu = x % 360;
                x = 0;
                int yushuzheng = Convert.ToInt32(Convert.ToDouble(yushu));
                yushuzhengString = yushuzheng.ToString();
               
                    textList.GetChild(EndIndex).GetComponent<Text>().text = yushuzhengString;
                
                
                EndIndex++;
                _canCorrect = true;
                canDrag.gameObject.SetActive(true);
            }
        }
            private void DragEnd2(Vector3 position, int type, int index, bool isMatch)
            {
            PAN2.GetComponent<mILDrager>().isActived = false;
            if (EndIndex < 4)
                {
                    if (x >= 0)
                    {
                        isleft = false;
                        Debug.Log("右边");

                    }
                    else
                    {
                        isleft = true;
                        Debug.Log("左边");

                    }
                    float yushu = x % 360;
                    x = 0;
                    int yushuzheng = Convert.ToInt32(Convert.ToDouble(yushu));
                    yushuzhengString = yushuzheng.ToString();
                if (EndIndex == 0)
                {
                    textList.GetChild(EndIndex).GetComponent<Text>().text = yushuzhengString;
                }
                else 
                {
                    textList.GetChild(EndIndex).GetComponent<Text>().text = (yushuzheng + int.Parse(textList.GetChild(EndIndex - 1).GetComponent<Text>().text)).ToString();
                    Debug.Log((yushuzheng + int.Parse(textList.GetChild(EndIndex - 1).GetComponent<Text>().text)).ToString());
                }
             
                EndIndex++;
                _canCorrect = true;
                canDrag.gameObject.SetActive(true);
            }

            }
            private void ReturnPointer(float RoateAngle)
        {
            mono.StartCoroutine(returnPointer(RoateAngle));
        }
        private void ReturnPointer2(float RoateAngle)
        {
            mono.StartCoroutine(returnPointer2(RoateAngle));
        }

        IEnumerator returnPointer(float RoateAngle)
        {

            //while (true)
            //{

            //    if (isleft)
            //    {
            //        z1 = z1 - 0.5f;
            //    }
            //    else
            //    {
            //        z1 = z1 + 0.5f;
            //    }
            //    //else
            //    //{
            //    //    yield break;
            //    //}
            //    //else { yield break; }
            float a = Pan.eulerAngles.z;
            int b = Convert.ToInt32(Convert.ToDouble(Pan.eulerAngles.z));
            Debug.Log(RoateAngle);
           // float angle = Pan.rotation.z*100;
            Debug.Log(a);
            float temp = 1;
            while (true) 
            {
                temp += 1;
                yield return new WaitForSeconds(0.01f);
                if (isleft)
                {
                    
                     Pan.rotation = Quaternion.Euler(new Vector3(Pan.rotation.x, Pan.rotation.y, a + RoateAngle*temp/100));
                   // Pan.Rotate(0, 0, RoateAngle);
                   
                }
                else
                {
                    Pan.rotation = Quaternion.Euler(new Vector3(Pan.rotation.x, Pan.rotation.y, a - RoateAngle * temp / 100));
                }
                if (temp == 100)
                {
                    
                    _Mask.gameObject.SetActive(true);
                    isfirstString = true;
                    numString = "";
                    yushuzhengString = "";
                    biaoText.text = "";
                    Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false), () =>
                    {
                        _Mask.gameObject.SetActive(false);
                        PAN.GetComponent<mILDrager>().isActived = true;
                    });
                    if (EndIndex == 4) 
                    {
                        Delay(1f, () =>
                     {
                         SoundManager.instance.ShowVoiceBtn(true);
                     });
                     
                    }
                    break;
                }
                    
            }
              

            //   // Pan.Rotate(0, 0, z1);
            //}

        }
        IEnumerator returnPointer2(float RoateAngle)
        {
            float a = pointer.eulerAngles.z;
            int b = Convert.ToInt32(Convert.ToDouble(pointer.eulerAngles.z));
            Debug.Log(RoateAngle);
            // float angle = Pan.rotation.z*100;
            Debug.Log(a);
            float temp = 1;
            while (true)
            {
                temp += 1;
                yield return new WaitForSeconds(0.01f);
                if (isleft)
                {

                    pointer.rotation = Quaternion.Euler(new Vector3(pointer.rotation.x, pointer.rotation.y, a + RoateAngle * temp / 100));
                    // Pan.Rotate(0, 0, RoateAngle);

                }
                else
                {
                    pointer.rotation = Quaternion.Euler(new Vector3(pointer.rotation.x, pointer.rotation.y, a - RoateAngle * temp / 100));
                }
                if (temp == 100)
                {
                    _Mask.gameObject.SetActive(true);
                    isfirstString = true;
                    numString = "";
                    yushuzhengString = "";
                    biaoText.text = "";
                    Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 0, false),()=> 
                    {
                        _Mask.gameObject.SetActive(false);
                        PAN2.GetComponent<mILDrager>().isActived = true;
                    });
                    _Mask.gameObject.SetActive(false);
                    if (EndIndex == 4)
                    {
                        Delay(1f, () =>
                        {
                            SoundManager.instance.ShowVoiceBtn(true);
                        });
                    }
                    break;
                }
               
            }
        }


        void GameStart()
        {
            Max.SetActive(true);
       
         //   mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.COMMONVOICE, 0, null, () => { Max.SetActive(false);  }));

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

            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            yield return new WaitForSeconds(len);

            float ind = SoundManager.instance.PlayClip(type, clipIndex);
            SpineManager.instance.DoAnimation(speaker, "DAIJIshuohua");

            method_1?.Invoke();

            yield return new WaitForSeconds(ind);
            SpineManager.instance.DoAnimation(speaker, "DAIJI");
            SoundManager.instance.SetShield(true);

            method_2?.Invoke();
        }



        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 0)
            {
                SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(0).gameObject, "g1", true);
                SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(1).gameObject, "g2", true);
                SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(2).gameObject, "g1", true);
                SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(3).gameObject, "g4", true);
                Delay(3f, () =>
                {
                    SpineManager.instance.DoAnimation(pan1, "b1", false);
                    SpineManager.instance.DoAnimation(pan2, "b2", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(0).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(1).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(2).gameObject, "kong", false);
                    SpineManager.instance.DoAnimation(curTrans.Find("effect").GetChild(3).gameObject, "kong", false);
                });
                
                mono.StartCoroutine(SpeckerCoroutine(Max.gameObject, SoundManager.SoundType.SOUND, 1, null, () =>
                {
                    
                    isTwo = false;
                    Max.SetActive(false);
                    pan1.gameObject.SetActive(false);pan2.gameObject.SetActive(false);
                    curTrans.Find("1").gameObject.SetActive(true);

                    //   curTrans.Find("2").gameObject.SetActive(false);
                    curTrans.Find("biao").gameObject.SetActive(true);
                    biao.gameObject.SetActive(true);
                       curTrans.Find("biao").transform.GetRectTransform().DOAnchorPosX(444,2f).SetEase(Ease.Linear).OnComplete(() =>
                     {
                         _Mask.gameObject.SetActive(false);
                     });

                }));

               
               

            }
            else if (talkIndex == 1)
            {
                //第一关结束
                isTwo = true;
                canDrag.gameObject.SetActive(false);
                curTrans.Find("1").gameObject.SetActive(false);
                curTrans.Find("2").gameObject.SetActive(true);
                curTrans.Find("2").transform.GetRectTransform().anchoredPosition = new Vector2(600, 550);
                textList.GetChild(0).GetComponent<Text>().text = "";
                textList.GetChild(1).GetComponent<Text>().text = "";
                textList.GetChild(2).GetComponent<Text>().text = "";
                textList.GetChild(3).GetComponent<Text>().text = "";
                OnClickResetting();
            }
            else if (talkIndex == 2)
            {
                curTrans.Find("1").gameObject.SetActive(false); curTrans.Find("2").gameObject.SetActive(false);biao.gameObject.SetActive(false);
                curTrans.Find("biao").gameObject.SetActive(false);
                left.gameObject.SetActive(true);
                right.gameObject.SetActive(true);
                SpineManager.instance.DoAnimation(left.gameObject, "b1", false);
                SpineManager.instance.DoAnimation(right.gameObject, "b2", false);
                mono.StartCoroutine(SpeckerCoroutine(Max.gameObject, SoundManager.SoundType.SOUND, 2, null, () =>
                    {
                        SoundManager.instance.ShowVoiceBtn(true);
                    }));
            }
            else if (talkIndex==3) 
            {
                mono.StartCoroutine(SpeckerCoroutine(Max.gameObject, SoundManager.SoundType.SOUND, 3, null, () =>
                {
                   
                }));
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
        IEnumerator myupdate(float timing,Action _method=null) 
        {
            while (true) 
            {
                yield return new WaitForSeconds(timing);
                _method?.Invoke();
            }
        }
        IEnumerator wait(float timing, Action _method = null) 
        {
           
                yield return new WaitForSeconds(timing);
                _method?.Invoke();
            
        }
        private void MyUpdate(float timing, Action _method = null) 
        {
            mono.StartCoroutine(myupdate(timing,_method));
        }
        private void MyWait(float timing, Action _method = null)
        {
            mono.StartCoroutine(wait(timing, _method));
        }
        private void ControlleRoate()
        {
           
                    Vector3 mouse = Input.mousePosition;
                    //  Vector3 obj = Camera.main.WorldToScreenPoint(Pan.position);
                    Vector3 obj = Pan.position;
                    Vector3 direction = mouse - obj;
                    direction.z = 0f;
                    direction = direction.normalized;
                    Pan.up = direction;
         //   Debug.Log("dir"+direction);
 
        }
        public float CheckAngle(float value)
        {
            float angle = value - 180;

            if (angle > 0)
                return angle - 180;

            return angle + 180;
        }
        private float VectorAngle(Vector2 from, Vector2 to)
        {
         float angle;

            Vector3 cross = Vector3.Cross(from, to);
         angle = Vector2.Angle(from, to);
         return cross.z > 0 ? -angle : angle;
         }
        private void AddBtnOnClick() 
        {
          //  Util.AddBtnClick(biao.Find("click").Find("0").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("1").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("2").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("3").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("4").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("5").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("6").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("7").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("8").gameObject, OnClick);
           // Util.AddBtnClick(biao.Find("click").Find("9").gameObject, OnClick);

            for (int i = 0; i < biao.Find("click").childCount-3; i++) 
            {
                Util.AddBtnClick(biao.Find("click").GetChild(i).gameObject, OnClick);   
            }
            Util.AddBtnClick(biao.Find("click").Find("dui").gameObject, OnClickCorrect);
            Util.AddBtnClick(biao.Find("click").Find("fu").gameObject, OnClickSubtract);
            Util.AddBtnClick(biao.Find("click").Find("c").gameObject, OnClickResetting);
            Util.AddBtnClick(curTrans.Find("cw").gameObject, OnClickCW);
          
        }
        private void OnClick(GameObject obj) { InputNum(obj); }
     
        private void OnClickCW(GameObject obj) 
        {
            EndIndex = 0;
            biaoText.text = ""; numString = ""; isfirstString = true;
            if (isTwo == false)
            {
                Pan.rotation = Quaternion.Euler(new Vector3(0,0,0));
            }
            else 
            {
                pointer.parent.rotation= Quaternion.Euler(new Vector3(0, 0, 0));
                pointer.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            }
            if (EndIndex != 4)
            {
                canDrag.gameObject.SetActive(true);
            }
        }
        private void OnClickCorrect(GameObject obj) 
        {
            //第一种指针
            if (_canCorrect == true) 
            {
                 if (isTwo == false)
            {
                if (isleft)
                    {
                        if (biaoText.text=="0") 
                        {
                            if (yushuzhengString == biaoText.text) 
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                Debug.Log("正确");
                                _Mask.gameObject.SetActive(true);
                                _canCorrect = false;
                                //指针回正
                                //  Pan.Rotate(new Vector3(0,0, float.Parse("-" + biaoText.text)));
                                ReturnPointer(float.Parse("-" + biaoText.text));
                                canDrag.gameObject.SetActive(false);
                            }
                        }
                    else if ("-" + biaoText.text == (yushuzhengString))
                    {
                        //bingo
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        Debug.Log("正确");
                        _Mask.gameObject.SetActive(true);
                        _canCorrect = false;
                        //指针回正
                        //  Pan.Rotate(new Vector3(0,0, float.Parse("-" + biaoText.text)));
                        ReturnPointer(float.Parse("-" + biaoText.text));
                        canDrag.gameObject.SetActive(false);
                    }
                    else
                    {
                            //
                            _canCorrect = false;
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                        Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false),()=> 
                        {
                            _canCorrect = true;
                        });
                        Debug.Log("错误");
                    }

                }
                else
                    {
                        if (biaoText.text == "0")
                        {
                            if (yushuzhengString == biaoText.text)
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                Debug.Log("正确");
                                _Mask.gameObject.SetActive(true);
                                _canCorrect = false;
                                //指针回正
                                //  Pan.Rotate(new Vector3(0,0, float.Parse("-" + biaoText.text)));
                                ReturnPointer(float.Parse("-" + biaoText.text));
                                canDrag.gameObject.SetActive(false);
                            }
                        }
                       else if (biaoText.text == ("-" + yushuzhengString))
                    {
                        //bingo
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        Debug.Log("正确"); _Mask.gameObject.SetActive(true);
                        _canCorrect = false;
                        //指针回正
                        // Pan.Rotate(new Vector3(0, 0, 60));
                        ReturnPointer(float.Parse(biaoText.text)); 
                        canDrag.gameObject.SetActive(false);
                    }
                    else
                    {
                            //
                            _canCorrect = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false), () =>
                            {
                                _canCorrect = true;
                            });
                        }
                }
            }
            //第二种指针
            else
            {
                if (isleft)
                    {
                        if (biaoText.text == "0")
                        {
                            if (yushuzhengString == biaoText.text)
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                Debug.Log("正确");
                                _Mask.gameObject.SetActive(true);
                                _canCorrect = false;
                                //指针回正
                                //  Pan.Rotate(new Vector3(0,0, float.Parse("-" + biaoText.text)));
                                ReturnPointer(float.Parse("-" + biaoText.text));
                                canDrag.gameObject.SetActive(false);
                            }
                        }
                      else  if ("-" + biaoText.text == (yushuzhengString))
                    {
                        //bingo
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        Debug.Log("正确"); _Mask.gameObject.SetActive(true);
                        _canCorrect = false;
                        //指针回正
                        ReturnPointer2(float.Parse("-" + biaoText.text));
                        canDrag.gameObject.SetActive(false);
                    }
                    else
                    {
                            //  
                            _canCorrect = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false), () =>
                            {
                                _canCorrect = true;
                            });
                        }

                }
                else
                    {
                        if (biaoText.text == "0")
                        {
                            if (yushuzhengString == biaoText.text)
                            {
                                SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                                Debug.Log("正确");
                                _Mask.gameObject.SetActive(true);
                                _canCorrect = false;
                                //指针回正
                                //  Pan.Rotate(new Vector3(0,0, float.Parse("-" + biaoText.text)));
                                ReturnPointer(float.Parse("-" + biaoText.text));
                                canDrag.gameObject.SetActive(false);
                            }
                        }
                      else  if (biaoText.text == ("-" + yushuzhengString))
                    {
                        //bingo
                        SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 2, false);
                        Debug.Log("正确"); _Mask.gameObject.SetActive(true);
                        _canCorrect = false;
                        //指针回正
                        ReturnPointer2(float.Parse(biaoText.text));
                        canDrag.gameObject.SetActive(false);
                    }
                    else
                    {
                            //
                            _canCorrect = false;
                            SoundManager.instance.PlayClip(SoundManager.SoundType.VOICE, 1, false);
                            Delay(SoundManager.instance.PlayClip(SoundManager.SoundType.SOUND, 4, false), () =>
                            {
                                _canCorrect = true;
                            });
                        }
                }
            }
            if (EndIndex == 4) 
            {
                canDrag.gameObject.SetActive(true);
            }
            }
           
        
        }

        private void OnClickSubtract(GameObject obj)
        {
            if (isfirstString)
            {
                BtnPlaySound();
                numString += "-"; 
                biaoText.text = numString; 
            } }
        private void OnClickResetting(GameObject obj)
        {
            BtnPlaySound();
            biaoText.text = ""; numString = ""; isfirstString = true;
        }
        private void OnClickResetting()
        {
            BtnPlaySound();
            biaoText.text = ""; numString = ""; isfirstString = true;EndIndex = 0;
        }
        private void InputNum(GameObject obj) 
        {
            BtnPlaySound();
            isfirstString = false;
            if (numString.Length > 5) { return; }
            numString += obj.name;
            biaoText.text = numString;
        }
        private void Delay(float delay, Action callBack)
        {
            mono.StartCoroutine(IEDelay(delay, callBack));
        }
        private void GuiWei() 
        {
            Pan.rotation = Quaternion.Euler(new Vector3(0,0,0));
            Pan2.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
            pointer.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
        }
        IEnumerator IEDelay(float delay, Action callBack)
        {
            yield return new WaitForSeconds(delay);
            callBack?.Invoke();
        }
    }
}
