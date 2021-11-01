using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace ILFramework.HotClass
{
    public class Course732Part5
    {

        private int talkIndex;
        private MonoBehaviour mono;
        GameObject curGo;
        Transform curTrans;

        private GameObject Bg;
        private BellSprites bellTextures;

        private GameObject Max;

        bool isPlaying = false;


        private Image _drawLine;
      
        private Transform _blueLine;
        private GameObject[] _blueLines;
        private Transform _redLine;
        private GameObject[] _redLines;

        private Transform _clickBtn;
        private PolygonCollider2D[] _polyClickBtn;       

        private GameObject _spine0;
        private GameObject _spine1;
        private GameObject _spine2;
        private GameObject _car;

        private bool isDrawLine;

        private bool isCompleteDrawLine;
        private bool isCarStartDrawLine;

        private bool drawRedLine;
        private bool drawBlueLine;

        private int blueIndex;
        private int redIndex;
       

        private GameObject _mask;

        void Start(object o)
        {
            curGo = (GameObject)o;
            mono = curGo.GetComponent<MonoBehaviour>();
            curTrans = curGo.transform;
            mono.StopAllCoroutines();
            SoundManager.instance.StopAudio();
            Input.multiTouchEnabled = false;

            Bg = curTrans.Find("Bg").gameObject;
            bellTextures = Bg.GetComponent<BellSprites>();

            Max = curTrans.Find("bell").gameObject;
            
            SoundManager.instance.ShowVoiceBtn(false);
            SoundManager.instance.SetVoiceBtnEvent(TalkClick);

            SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 2, true);

            GameInit();
            GameStart();
        }

        private void GameInit()
        {
            talkIndex = 1;           
            isDrawLine = false;
            isCompleteDrawLine = false;            
            isCarStartDrawLine = false;
            drawRedLine = false;
            drawRedLine = false;
            blueIndex = 0;
            redIndex = 0;
            drawBlueIndex = 0;
            drawRedIndex = 0;
           

            _spine0 = curTrans.Find("spinePanel/panel/0").gameObject;
            _spine1 = curTrans.Find("spinePanel/panel/1").gameObject;
            _spine2 = curTrans.Find("spinePanel/panel/2").gameObject;
            _car = curTrans.Find("spinePanel/panel/car").gameObject;

            _spine0.Show();
            SpineManager.instance.DoAnimation(_spine0, "2", false);

            _car.Show();
            _car.GetComponent<Spine.Unity.SkeletonGraphic>().startingAnimation = null;
            _car.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
            SpineManager.instance.DoAnimation(_car, "che", false);
            SpineManager.instance.DoAnimation(_spine1, "kong", false);
            SpineManager.instance.DoAnimation(_spine2, "kong", false);

            _spine1.Hide();
            _spine2.Hide();

            _blueLine = curTrans.Find("lineManager/blueLine");
            _blueLines = new GameObject[_blueLine.childCount];
            for (int i = 0; i < _blueLines.Length; i++)
            {
                _blueLines[i] = _blueLine.GetChild(i).gameObject;
                _blueLines[i].Hide();
            }

            _redLine = curTrans.Find("lineManager/redLine");
            _redLines = new GameObject[_redLine.childCount];
            for (int i = 0; i < _blueLines.Length; i++)
            {
                _redLines[i] = _redLine.GetChild(i).gameObject;
                _redLines[i].Hide();
            }

            _drawLine = curTrans.Find("lineManager/lineBox/drawLine").GetComponent<Image>();
            _drawLine.gameObject.Hide();


            _clickBtn = curTrans.Find("clickBtn/panel");
            _polyClickBtn = _clickBtn.GetComponentsInChildren<PolygonCollider2D>(true);
                      

            _mask = curTrans.Find("spinePanel/panel/mask").gameObject;
            _mask.Hide();
        }
        void GameStart()
        {
            Vector2 _pos = Vector2.one;
            Max.SetActive(true);
            isPlaying = true;
            mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 0, null, () => 
            {
                Max.Hide();
                isPlaying = false;
                isDrawLine = true;
                isCompleteDrawLine = true;
                isCarStartDrawLine = true;                               
            }));

        }  
        private void IsShowOrHide(GameObject[] gos,bool isShow)
        {
            for (int i = 0; i < gos.Length; i++)
            {
                gos[i].SetActive(isShow);
            }
        }
        void Update()
        {

          
            if (isDrawLine)
            {
                if (Vector3.Distance(Input.mousePosition, _drawLine.transform.position) > 0)
                {

                    if (Input.mousePosition.x - _drawLine.transform.position.x > 0)
                    {
                        SetLine(_drawLine.transform.position, Input.mousePosition, -1);
                    }
                    else
                    {
                        SetLine(_drawLine.transform.position, Input.mousePosition, 1);
                    }
                }
                if (isCompleteDrawLine && isCarStartDrawLine)
                {
                    if(Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[0].transform.position)) <20)
                    {
                        SetDrawLineInit(_polyClickBtn,0,0);
                        isCarStartDrawLine = false;
                        
                    }                   
                }
                DrawBulueOrReadLine();


            }
        }
        private void SetDrawLineInit(PolygonCollider2D [] _polys,int index,int lineIndex)
        {
            _drawLine.gameObject.Show();
            _drawLine.rectTransform.sizeDelta = new Vector2(54, 10);
            _drawLine.sprite = _drawLine.GetComponent<BellSprites>().sprites[lineIndex];
            _drawLine.transform.position = _polys[index].transform.position;
        }
        private int drawBlueIndex;
        private int drawRedIndex;
        private void DrawBulueOrReadLine()
        {
            if (isCompleteDrawLine && isCarStartDrawLine == false)
            {               
                if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[1].transform.position)) <20)//画蓝线
                {
                    drawBlueIndex = 1;

                    isCompleteDrawLine = false;
                    drawBlueLine = true;

                    SetDrawLineInit(_polyClickBtn, 1,0);
                    _blueLines[0].Show();                    
                }
                if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[3].transform.position)) <20)//画红线
                {

                    drawRedIndex = 1;

                    SetDrawLineInit(_polyClickBtn, 3,1);
                    _redLines[2].Show();                                      
                    isCompleteDrawLine = false;
                    drawRedLine = true;
                }
            }
            else if (drawBlueLine)
            {                
                if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[2].transform.position)) < 20)//画蓝线
                {                   
                    if (drawBlueIndex == 1)
                    {
                        drawBlueIndex = 2;
                        _blueLines[1].Show();
                        SetDrawLineInit(_polyClickBtn, 2,0);
                    }
                }
                else if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[3].transform.position)) < 20)//画蓝线
                {
                    if (drawBlueIndex == 2)
                    {
                        _blueLines[2].Show();
                        drawBlueLine = false;
                        blueIndex = 1;
                        drawBlueIndex = 0;
                        _drawLine.gameObject.Hide();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 0, false);
                        SpineManager.instance.DoAnimation(_car, "che2", false, () =>
                        {
                            isCompleteDrawLine = true;
                            isCarStartDrawLine = true;
                            ShowOrHideLine(_blueLines, false);

                            if (blueIndex == 1 && redIndex == 1)
                            {
                                isDrawLine = false;
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                        });
                    }                   
                }
            }
            else if (drawRedLine)
            {               
                if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[2].transform.position)) < 20)//画红线
                {
                    if (drawRedIndex == 1)
                    {
                        drawRedIndex = 2;
                        _redLines[1].Show();
                        SetDrawLineInit(_polyClickBtn, 2,1);
                    }                    
                }
                else if (Mathf.Abs(Vector3.Distance(Input.mousePosition, _polyClickBtn[1].transform.position)) < 20)//画红线
                {
                    if (drawRedIndex == 2)
                    {
                        _redLines[0].Show();
                        drawRedLine = false;
                        redIndex = 1;
                        drawRedIndex = 0;
                        _drawLine.gameObject.Hide();
                        SoundManager.instance.PlayClip(SoundManager.SoundType.BGM, 1, false);
                        SpineManager.instance.DoAnimation(_car, "che3", false, () =>
                        {
                            isCompleteDrawLine = true;
                            isCarStartDrawLine = true;
                            ShowOrHideLine(_redLines, false);

                            if (blueIndex == 1 && redIndex == 1)
                            {
                                isDrawLine = false;
                                SoundManager.instance.ShowVoiceBtn(true);
                            }
                        });
                    }
                }
            }            
        }

        /// <summary>
        /// 最后一个 Vector3.forward 控制方向正负，加负号可逆转方向
        /// </summary>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="dirIndex"></param>
        private void SetLine(Vector3 v1, Vector3 v2,int dirIndex)
        {
            Vector3 mid = (v1 - v2) / 2;
           
            Vector2 temv2 = Vector2.one;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(curTrans as RectTransform,v2,null,out temv2);
            Vector2 temObjv2 = Vector2.one;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(curTrans as RectTransform, v1, null, out temObjv2);
            _drawLine. GetComponent<RectTransform>().sizeDelta = new Vector2(_drawLine.rectTransform.sizeDelta.x, /*Vector3.Distance(v2, v1)*/Vector2.Distance(temv2, temObjv2));           
            _drawLine.GetComponent<RectTransform>().rotation = Quaternion.AngleAxis(Vector3.Angle(mid, Vector3.down), dirIndex *Vector3.forward);
            
        } 
        private void ShowOrHideLine(GameObject[]_lines,bool isShow)
        {
            for (int i = 0; i < _lines.Length; i++)
            {
                _lines[i].SetActive(isShow);
            }
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
        IEnumerator WaiteCoroutine(Action method_2 = null, float len = 0)
        {
            
            yield return new WaitForSeconds(len);
            method_2?.Invoke();
        }

        void TalkClick()
        {
            BtnPlaySound();
            SoundManager.instance.ShowVoiceBtn(false);
            if (talkIndex == 1)
            {                

                HideLine(_redLines, false);
                HideLine(_blueLines, false);
                Max.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 2, null, ()=> 
                { 
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
                _spine1.Show();
                _spine1.GetComponent<Spine.Unity.SkeletonGraphic>().Initialize(true);
                SpineManager.instance.SetTimeScale(_spine1, 0.8f);
                SpineManager.instance.DoAnimation(_spine1, "ljc1", false,()=> 
                {
                    mono.StartCoroutine(WaiteCoroutine(() =>
                    {
                        SpineManager.instance.SetTimeScale(_spine1, 0.4f);
                        //重叠
                        SpineManager.instance.DoAnimation(_spine1, "lja1", false, () =>
                        {
                            SpineManager.instance.SetTimeScale(_spine1, 1f);
                            mono.StartCoroutine(WaiteCoroutine(() =>
                            {
                                //蓝
                                SpineManager.instance.DoAnimation(_spine1, "ljb1", false, () =>
                                {
                                    mono.StartCoroutine(WaiteCoroutine(() =>
                                    {
                                        //红
                                        SpineManager.instance.DoAnimation(_spine1, "ljd1", false,()=>
                                        {
                                            //蓝，红                                                                                      
                                            mono.StartCoroutine(WaiteCoroutine(() =>
                                            {
                                                _spine2.Show();
                                                SpineManager.instance.SetTimeScale(_spine2, 0.6f);
                                                SpineManager.instance.SetTimeScale(_spine1, 0.6f);

                                                SpineManager.instance.DoAnimation(_spine1, "ljb1", false);
                                                SpineManager.instance.DoAnimation(_spine2, "ljd1", false, () =>
                                                {
                                                    mono.StartCoroutine(WaiteCoroutine(() =>
                                                    {                                                       
                                                        SpineManager.instance.DoAnimation(_spine1, "lja1", false);
                                                        SpineManager.instance.DoAnimation(_spine2, "ljb1", false, () =>
                                                        {
                                                            _spine2.Hide();
                                                        });
                                                    }, 4.0f));
                                                });                                               
                                            }, 4.0f));                                          
                                        });
                                    }, 1.0f));
                                });
                            }, 5.0f));
                        });

                    }, 1.0f));                                    
                });
                _spine2.Show();
                SpineManager.instance.SetTimeScale(_spine2, 0.8f);
                SpineManager.instance.DoAnimation(_spine2, "lje1", false, () =>
                {
                    SpineManager.instance.DoAnimation(_spine2, "lje2", false, () => { _spine2.Hide(); });
                });
            }
            else if (talkIndex == 2)
            {
                _mask.Show();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 3, null,()=> 
                {
                    SoundManager.instance.ShowVoiceBtn(true);
                }));
                SpineManager.instance.DoAnimation(_spine1, "animation4", false);
            }
            else if (talkIndex == 3)
            {
                _mask.Hide();
                mono.StartCoroutine(SpeckerCoroutine(Max, SoundManager.SoundType.VOICE, 4, () =>
                {
                    //SoundManager.instance.ShowVoiceBtn(true);
                }));
                SpineManager.instance.DoAnimation(_spine1, "animation5", false,()=> 
                {
                    _spine1.Hide();
                });
            }
            talkIndex++;
        }

        private void HideLine(GameObject[] lines,bool isShow)
        {
            for (int i = 0; i < lines.Length; i++)
            {
                lines[i].SetActive(isShow);
            }
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
