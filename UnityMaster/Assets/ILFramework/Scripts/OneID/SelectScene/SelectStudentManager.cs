using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using ILFramework;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace OneID
{
    public class SelectStudentManager : MonoBehaviour
    {
        private Transform _studentInfoPanel;

        private List<Image> _studentInfoList;

        private Image _lastImage;

        private BellSprites _bellSprites;

        private Transform _newStudentInfoPanel;
        private RawImage _rawImage1;
        private RawImage _rawImage2;

        private float a = 12f;

        private Image _student1;
        private Image _Student2;

        private int _studentNumber = -1;

        private float _cellHeight = 485f;
        private Transform _selectSutdentPanel;
        private Transform _selectSudentImageGrid1;
        private Transform _selectSudentImageGrid2;

        private float _accelarate = 500f;

        private float _totalHeight = 0f;

        private BellSprites _allStudentSprites;
        private List<Sprite> _allStudentSpriteList;

        private GameObject _frameSpine;

        private bool _isSelecting=false;
        void Awake()
        {
            _studentInfoList = new List<Image>();
            _studentInfoPanel = this.transform.GetTransform("StudentInfo");
            _bellSprites = _studentInfoPanel.GetComponent<BellSprites>();
            for (int i = 0; i < _studentInfoPanel.childCount; i++)
            {
                Transform target = _studentInfoPanel.GetChild(i);
                _studentInfoList.Add(target.GetComponent<Image>());
            }

            _newStudentInfoPanel = this.transform.GetTransform("NewStudentInfo");
            _rawImage1 = _newStudentInfoPanel.GetTargetComponent<RawImage>("1");
            _rawImage2 = _newStudentInfoPanel.GetTargetComponent<RawImage>("2");
            _student1 = this.transform.GetTargetComponent<Image>("Student1");
            _Student2 = this.transform.GetTargetComponent<Image>("Student2");

            _selectSutdentPanel = this.transform.GetTransform("NewStudentInfo2");
            
            _selectSudentImageGrid1 = _selectSutdentPanel.GetTransform("Grid1");
            _selectSudentImageGrid2 = _selectSutdentPanel.GetTransform("Grid2");
            _frameSpine = this.transform.GetGameObject("FrameSpine");
            InializedAllStudentImage();
        }

        void Start()
        {
            //StartCoroutine(StartSelectIE());

            _totalHeight = _cellHeight * GetCurrenShowStudentNumber();
            _studentNumber = -1;
            
            //StartCoroutine(MoveNewImageIE2());
        }

        private void OnEnable()
        {
            SetAllStudentItemName();
            ResetAllStudentItem();
            _isSelecting = false;
            _student1.sprite = null;
            _Student2.sprite = null;
        }

        void InializedAllStudentImage()
        {
            _allStudentSprites = _selectSutdentPanel.GetTargetComponent<BellSprites>();
            _allStudentSpriteList=new List<Sprite>();
            Sprite[] allSprites = _allStudentSprites.sprites;
            for (int i = 0; i < allSprites.Length; i++)
            {
                _allStudentSpriteList.Add(allSprites[i]);
            }
        }

        void ResetAllStudentItem()
        {
            if (_studentNumber != -1)
            {
                _selectSudentImageGrid1.transform.GetChild(_studentNumber).gameObject.Hide();
                _selectSudentImageGrid2.transform.GetChild(_studentNumber).gameObject.Hide();
            }

            int number = GetCurrenShowStudentNumber();
            float height = number * _cellHeight;
            Vector3 pos=new Vector3(_selectSudentImageGrid2.localPosition.x,-height,_selectSudentImageGrid2.localPosition.z);
            _selectSudentImageGrid1.transform.localPosition = new Vector3(_selectSudentImageGrid1.localPosition.x, 0,
                _selectSudentImageGrid1.localPosition.z);
            _selectSudentImageGrid2.transform.localPosition = pos;
        }

        void SetAllStudentItemName()
        {
            for (int i = 0; i < _selectSudentImageGrid1.childCount; i++)
            {
                Image target = _selectSudentImageGrid1.GetChild(i).GetComponent<Image>();
                target.name = target.sprite.name;
            }
            for (int i = 0; i < _selectSudentImageGrid2.childCount; i++)
            {
                Image target = _selectSudentImageGrid2.GetChild(i).GetComponent<Image>();
                target.name = target.sprite.name;
            }
        }

        public void SelectStudent()
        {
            if(_isSelecting)
                return;
            _isSelecting = true;
            OneIDSceneManager.Instance.PlayCommonSound(7);
            SpineManager.instance.DoAnimation(_frameSpine, "lhj2", false, () =>
            {
                SpineManager.instance.DoAnimation(_frameSpine, "lhj1", false, () =>
                {
                    ResetAllStudentItem();
                    StartCoroutine(MoveNewImageIE2());
                });
            });
            
        }

        IEnumerator MoveNewImageIE2()
        {
            _totalHeight = _cellHeight * GetCurrenShowStudentNumber();
            Debug.LogError(_totalHeight);
            float startTime = 0f;
            bool isMaxSpeed = false;
            bool isMinSpeed = true;
            bool isStartSelectStudent = false;
            float finalValue = 0;
            //OneIDSceneManager.Instance.PlayCommonSound(8,true);
            while (true)
            {
                if (!isStartSelectStudent)
                {
                    if (startTime <= 2f && isMaxSpeed == false && isMinSpeed)
                    {
                        startTime += Time.fixedDeltaTime;
                    }
                    else if (startTime >= 2f&&isMaxSpeed==false&& isMinSpeed)
                    {
                        //Debug.LogError("222");
                        _accelarate *= 1.01f;
                    }
                    if (_accelarate >= 2500f&&isMaxSpeed==false)
                    {
                        //Debug.LogError("333");
                        isMaxSpeed = true;
                        isMinSpeed = false;
                        startTime =0f;
                    
                    }
                    if (isMaxSpeed&&startTime<=2f)
                    {
                        //Debug.LogError("4444");
                        startTime += Time.fixedDeltaTime;
                    
                    }
                    else if(isMaxSpeed&&startTime>1f)
                    {
                        //Debug.LogError("5555");
                        _accelarate /= 1.01f;
                    }

                    if (_accelarate <= 600f && isMaxSpeed==true && isMinSpeed==false)
                    {
                        //Debug.LogError("进入最后选择环节");
                        isMaxSpeed = false;
                        isMinSpeed = true;
                        finalValue= RandomStudentAndSelect2();
                        isStartSelectStudent = true;
                        //todo...
                    }
                    MoveImage2(_accelarate*Time.fixedDeltaTime);
                }
                else
                {
                    //Debug.LogError(finalValue - _selectSudentImageGrid1.transform.localPosition.y);
                    float distance = finalValue - _selectSudentImageGrid1.transform.localPosition.y;
                    if (Mathf.Abs(distance)  < 10f)
                    {
                        
                        //SelectStudentAndExcuteNext();
                        Transform transform1 = _selectSudentImageGrid1.transform;
                        Vector3 position = new Vector3(transform1.localPosition.x, finalValue,
                            transform1.localPosition.z);
                        transform1.localPosition = position;
                        OneIDSceneManager.Instance.StopAudioSource("sound");
                        SelectStudentAndExcuteNext();
                        
                        break;
                    }

                    if (distance < 1000f&& distance>0 &&
                        _accelarate > 100f)
                    {
                        //Debug.LogError(distance);
                        _accelarate /= 1.01f;
                    }

                    MoveImage2(_accelarate*Time.fixedDeltaTime);
                }
               
                
                yield return new WaitForFixedUpdate();
                
                
            }
        }


        void MoveImage2(float l)
        {
            Transform transform1 = _selectSudentImageGrid1;
            Transform transform2 = _selectSudentImageGrid2;
            var localPosition = transform1.localPosition;
            float y = localPosition.y;
            y += l;
            if (y >= _totalHeight)
            {
                y = 0f;
                Transform temp = _selectSudentImageGrid1;
                _selectSudentImageGrid1 = _selectSudentImageGrid2;
                _selectSudentImageGrid2 = temp;
                transform1=_selectSudentImageGrid1;
                transform2=_selectSudentImageGrid2;
                //Debug.LogError("1:"+_selectSudentImageGrid1.name+"2:"+_selectSudentImageGrid2.name);
            }

            //Debug.Log("之前：" + y);
            localPosition = new Vector3(
                localPosition.x, y,
                localPosition.z);
            transform1.localPosition = localPosition;
            y = y - _totalHeight;
            //Debug.Log("之后：" + y);
            localPosition = transform2.localPosition;
            localPosition = new Vector3(localPosition.x, y, localPosition.z);
          
            transform2.localPosition = localPosition;
        }

   

        void SelectStudentAndExcuteNext()
        {
            OneIDSceneManager.Instance.PlayCommonSound(9);
            OneIDSceneManager.Instance.ChangeThePKStudentStatu(GetTheTargetStudentName(), true);
            OneIDSceneManager.Instance.AddOneStudentToVSDic(GetTheTargetStudentName());
            if (this.gameObject.name == "SelectPanel2")
            {
                OneIDSceneManager.Instance.SetSwitchButtonVisible(true);
                ShowTheTargetStudentImage();
            }
            else
            {
                int replaceIndex = ShowTheTargetStudentImage();

                if (replaceIndex == 1)
                {
                    OneIDSceneManager.Instance.SetSwitchButtonVisible(true);
                }
            }

            

            _isSelecting = false;
            //ResetAllStudentItem();
        }

        int ShowTheTargetStudentImage()
        {
            int replaceIndex = 0;
            Sprite target = GetTheStudentImage();
            if (target != null)
            {
                if (_student1.sprite == null)
                {
                    _student1.sprite = target;
                    return 0;
                }
                else if (_Student2.sprite == null)
                {
                    _Student2.sprite = target;
                    return 1;
                }
            }
            else
            {
                Debug.LogError("图片不存在，请检查！");
                
            }
            return -1;
        }

        float RandomStudentAndSelect2()
        {
            int number = Random.Range(0, GetCurrenShowStudentNumber());
            float targetValue = number * _cellHeight;
            _studentNumber = number;
            return targetValue;
        }

        string GetTheTargetStudentName()
        {
            string studentName = (_studentNumber + 1).ToString();
            return studentName;
        }

        Sprite GetTheStudentImage()
        {
            string imageName = JudgeNumberAndGetSpriteName(_studentNumber);
            //Debug.LogError("图片名称："+imageName+"学员下标为:"+_studentNumber);
            Sprite sprite= _allStudentSpriteList.Find(p => p.name.Equals(imageName));
            if(sprite==null)
                Debug.LogError("名称不对，请检查！"+imageName);
            
            return sprite;
        }

        int GetCurrenShowStudentNumber()
        {
            int targetNumber = 0;
            for (int i = 0; i < _selectSudentImageGrid1.childCount; i++)
            {
                GameObject target = _selectSudentImageGrid1.GetChild(i).gameObject;
                if (target.activeSelf)
                    targetNumber++;
            }
            return targetNumber;
        }

        string JudgeNumberAndGetSpriteName(int number)
        {
            //string name = _selectSudentImageGrid1.GetTransform((number).ToString()).name;
            int index = number;
            for (int i = 0; i <= number; i++)
            {
                if (_selectSudentImageGrid1.GetChild(i).GetGameObject().activeSelf == false)
                    index++;
            }
            string name = _selectSudentImageGrid1.GetChild(index).name;
            return name;
        }







        // IEnumerator MoveNewImageIE()
        // {
        //     float startTime = 0f;
        //     bool isMaxSpeed = false;
        //     bool isMinSpeed = true;
        //     bool isStartSelectStudent = false;
        //     float finalValue = 0;
        //     Rect r = _rawImage1.uvRect;
        //     r.y = 0;
        //     _rawImage1.uvRect = r;
        //     r.y = -1;
        //     _rawImage2.uvRect = r;
        //     while (true)
        //     {
        //
        //         if (!isStartSelectStudent)
        //         {
        //             if (startTime <= 6f&& isMaxSpeed==false && isMinSpeed)
        //             {
        //                 Debug.LogError("11");
        //                 startTime += Time.fixedDeltaTime;
        //             }
        //             else if (startTime >= 6f&&isMaxSpeed==false&& isMinSpeed)
        //             {
        //                 Debug.LogError("222");
        //                 a *= 1.008f;
        //             }
        //
        //             if (a >= 70f&&isMaxSpeed==false)
        //             {
        //                 Debug.LogError("333");
        //                 isMaxSpeed = true;
        //                 isMinSpeed = false;
        //                 startTime =0f;
        //             
        //             }
        //
        //             if (isMaxSpeed&&startTime<=2f)
        //             {
        //                 Debug.LogError("4444");
        //                 startTime += Time.fixedDeltaTime;
        //             
        //             }
        //             else if(isMaxSpeed&&startTime>2f)
        //             {
        //                 Debug.LogError("5555");
        //                 a /= 1.01f;
        //             }
        //
        //             if (a <= 15f && isMaxSpeed==true && isMinSpeed==false)
        //             {
        //                 Debug.LogError("进入最后选择环节");
        //                 isMaxSpeed = false;
        //                 isMinSpeed = true;
        //                 finalValue= RandomStudentAndSelect();
        //                 isStartSelectStudent = true;
        //                 //todo...
        //             }
        //             MoveImage( 0.5f*a*Time.fixedDeltaTime*Time.fixedDeltaTime);
        //         }
        //         else
        //         {
        //             // if (finalValue - _rawImage1.uvRect.y >= 0)
        //             // {
        //             //     //_rawImage1.DOColor().SetEase()
        //             //     // if (finalValue - _rawImage1.uvRect.y >= 0.01f)
        //             //     // {
        //             //     //     MoveImage( 0.5f*a*Time.fixedDeltaTime*Time.fixedDeltaTime);
        //             //     // }
        //             //     // else
        //             //     // {
        //             //         // float currentValue = Mathf.LerpUnclamped(_rawImage1.uvRect.y, finalValue, a*Time.fixedDeltaTime*Time.fixedDeltaTime*1.5f);
        //             //         // if (finalValue - currentValue < 0.002f)
        //             //         //     currentValue = finalValue;
        //             //         // Debug.Log("CurrentValue:" + currentValue+"FinalValue"+finalValue);
        //             //         // r.y = currentValue;
        //             //         // _rawImage1.uvRect = r;
        //             //         // if(Math.Abs(currentValue - finalValue) <= 0)
        //             //         //     break;
        //             //     // }
        //             //     if (finalValue - _rawImage1.uvRect.y < 0.002f)
        //             //     {
        //             //         r.y = finalValue;
        //             //         _rawImage1.uvRect = r;
        //             //         r.y = finalValue - 1f;
        //             //         _rawImage2.uvRect = r;
        //             //         break;
        //             //     }
        //             //
        //             //     
        //             //     
        //             // }
        //             // else
        //             // {
        //             //     Debug.Log("11111111111CurrentValue:" + _rawImage1.uvRect.y+"FinalValue"+finalValue);
        //             //     
        //             //     if (finalValue == 0&&_rawImage1.uvRect.y>=0.998f)
        //             //     {
        //             //         r.y = 0;
        //             //         _rawImage1.uvRect = r;
        //             //         r.y = -1;
        //             //         _rawImage2.uvRect = r;
        //             //         break;
        //             //         
        //             //     }
        //             // }
        //             if (Mathf.Abs(finalValue - _rawImage1.uvRect.y)  < 0.002f)
        //             {
        //                 r.y = finalValue;
        //                 _rawImage1.uvRect = r;
        //                 r.y = finalValue - 1f;
        //                 _rawImage2.uvRect = r;
        //                 SelectStudentAndExcuteNext();
        //                 
        //                 break;
        //             }
        //             MoveImage( 0.5f*a*Time.fixedDeltaTime*Time.fixedDeltaTime);
        //
        //             
        //         }
        //
        //         yield return new WaitForFixedUpdate();
        //     }
        // }
        //
        // void MoveImage(float l)
        // {
        //     Rect r = _rawImage1.uvRect;
        //     r.y += l;
        //     _rawImage1.uvRect = r;
        //     //_rawImage1.uvRect.Set(_rawImage1.uvRect.x, r.y, _rawImage1.uvRect.width, _rawImage1.uvRect.height);
        //     r.y = r.y - 1f;
        //     _rawImage2.uvRect = r;
        //     if (_rawImage1.uvRect.y >= 1f)
        //     {
        //         r.y = -1f;
        //         _rawImage1.uvRect = r;
        //         RawImage raw = _rawImage1;
        //         _rawImage1 = _rawImage2;
        //         _rawImage2 = raw;
        //     }
        // }

        // float RandomStudentAndSelect()
        // {
        //     int number = Random.Range(0, 8);
        //     float targetValue = number * 0.125f;
        //     _studentNumber = number;
        //     OneIDSceneManager.Instance.ChangeThePKStudentStatu(GetTheTargetStudentName(),true);
        //     return targetValue;
        // }
        //
        // IEnumerator StartSelectIE()
        // {
        //     int i = 0;
        //     float t = 0.5f;
        //     float minSpeed = 0.5f;
        //     float maxSpeed = 0.03f;
        //     bool isSpeedUp = true;
        //     bool isSlowDown = false;
        //     int randomStudentNumber = Random.Range(0, _studentInfoList.Count);
        //     while (true)
        //     {
        //         if (i > _studentInfoList.Count - 1)
        //             i = 0;
        //         SelectOneItem(_studentInfoList[i]);
        //         yield return new WaitForSeconds(t);
        //         if (isSpeedUp)
        //             t /= 1.1f;
        //         else if (isSlowDown)
        //             t *= 1.1f;
        //
        //         if (t <= maxSpeed&& isSlowDown==false)
        //         {
        //             isSpeedUp = false;
        //             isSlowDown = true;
        //         }
        //         else if (t >= minSpeed&& isSlowDown==true)
        //         {
        //             // isSlowDown = false;
        //             
        //             if (i == randomStudentNumber)
        //             {
        //                 Debug.LogError("幸运玩家是："+(i+1));
        //                 break;
        //             }
        //
        //            
        //         }
        //         Debug.Log(t);
        //         i++;
        //         //Debug.LogError(t);
        //         
        //     }
        // }
        //
        // void SelectOneItem(Image target)
        // {
        //     if (_lastImage != null)
        //         _lastImage.sprite = _bellSprites.sprites[0];
        //     target.sprite = _bellSprites.sprites[1];
        //     _lastImage = target;
        // }
    }
}

