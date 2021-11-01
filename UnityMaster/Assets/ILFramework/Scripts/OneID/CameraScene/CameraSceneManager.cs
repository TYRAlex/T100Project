using System;
using System.Collections;
using System.Collections.Generic;
using ILFramework;
using UnityEngine;
using UnityEngine.UI;

namespace OneID
{
    public class CameraSceneManager : MonoBehaviour
    {
        
        private Button _photographButton;
        private Button _confirmButton;
        private Button _cancelButton;
        private OpenCamera _openCamera;

        private Transform _showImages;

        private Transform _itemParent;
        private List<Image> _itemList;

        private Transform _finishedImagePanel;

        private OneIDStudent _currentStudent;

        private GameObject _ifSignLogoGameObject;

        private GameObject _takePhotoSpine;
        private GameObject _comfirmSpine;
        private GameObject _cancelSpine;
        
        private void Awake()
        {
            _photographButton = this.transform.GetTargetComponent<Button>("Photograph");
            _confirmButton = this.transform.GetTargetComponent<Button>("Comfirm");
            _cancelButton = this.transform.GetTargetComponent<Button>("Cancel");
            _openCamera = this.transform.GetTargetComponent<OpenCamera>("Camera");
            _showImages = this.transform.GetTransform("ShowItems");
            //_itemParent = this.transform.GetTransform("Scroll View/Viewport/Content");
            _itemParent = this.transform.GetTransform("ItemPanel/Image");
            _finishedImagePanel = this.transform.GetTransform("FinishedImagePanel");
            _ifSignLogoGameObject = this.transform.GetGameObject("IfSignLogo");
            _takePhotoSpine = this.transform.GetGameObject("Photograph/Spine");
            _comfirmSpine = this.transform.GetGameObject("Comfirm/Spine");
            _cancelSpine = this.transform.GetGameObject("Cancel/Spine");
            
            //InializedAllStudent();
            
            InializedItemList();
            
            //Debug.LogError("摄像机初始化完毕");
        }

        

        void ResetCurrentStudentPanel()
        {
            bool isAllSign = true;
            foreach (var temp in OneIDSceneManager.Instance.GetAllStudentDic())
            {
                if (temp.Value.IsSignIn == false)
                    isAllSign = false;
            }

            if (isAllSign)
            {
                OneIDSceneManager.Instance.SetSwitchButtonVisible(true,OneID_ButtonShow.Next);
            }
            else
            {
                OneIDSceneManager.Instance.SetSwitchButtonVisible(false);
            }
        }

        private void OnEnable()
        {
            _photographButton.onClick.AddListener(_openCamera.ScreenShot);
            _confirmButton.onClick.AddListener(_openCamera.ConfirmPictrue);
            _cancelButton.onClick.AddListener(_openCamera.CancelPictrue);
            _ifSignLogoGameObject.Hide();
            _currentStudent = OneIDSceneManager.Instance.GetStudentByName("1");
            ShowCurrentStudentHeadSelectImage(_currentStudent);
            OneIDSceneManager.Instance.GetStudentInfoManager.SetTargetLightVisible(_currentStudent.Name, true);
            
            ResetCurrentStudentPanel();
            
        }

        private void OnDisable()
        {
            _photographButton.onClick.RemoveListener(_openCamera.ScreenShot);
            _confirmButton.onClick.RemoveListener(_openCamera.ConfirmPictrue);
            _cancelButton.onClick.RemoveListener(_openCamera.CancelPictrue);
        }

        

        private void InializedItemList()
        {
            _itemList=new List<Image>();
            for (int i = 0; i < _itemParent.childCount; i++)
            {
                Transform target = _itemParent.GetChild(i);
                //print(target.name);
                _itemList.Add(target.GetComponent<Image>());
                PointerClickListener.Get(target.GetGameObject("Click")).clickDown = ClickAndSelectItem;
            }
        }

        private void ClickAndSelectItem(GameObject go)
        {
            Image target = go.transform.parent.GetComponent<Image>();
            //print(target.name);
            OneIDSceneManager.Instance.PlayCommonSound(5);
            for (int i = 0; i < _showImages.childCount; i++)
            {
                Transform targetImage = _showImages.GetChild(i);
                //print("1:"+go.name+"2:"+targetImage.name);
                if (target.name == targetImage.name)
                {
                    targetImage.GetGameObject().Show();
                    targetImage.GetComponent<Image>().sprite = target.sprite;
                }
                else
                {
                    targetImage.gameObject.Hide();
                }
            }

        }

        

        public void ShowCurrentStudent(string stuName)
        {
            Debug.Log("学生名称："+stuName);
            OneIDStudent student= OneIDSceneManager.Instance.GetStudentByName(stuName);
            _currentStudent = student;
            // Debug.Log("学生名称："+_currentStudent.Name);
            if (student.IsSignIn)
            {
                //Debug.LogError("11111111111");
                SetCurrentFinishedImageVisible(stuName,true);
                SetConfirmAndCancelButtonVisible(false);
                _openCamera.WebCam.Stop();
                for (int i = 0; i < _showImages.childCount; i++)
                {
                    _showImages.GetChild(i).GetGameObject().Hide();
                }
                _ifSignLogoGameObject.Show();
                _photographButton.gameObject.Hide();
            }
            else
            {
                //Debug.LogError("22222222222");
                SetCurrentFinishedImageVisible(stuName,false);
                SetConfirmAndCancelButtonVisible(false);
                for (int i = 0; i < _showImages.childCount; i++)
                {
                    _showImages.GetChild(i).GetGameObject().Hide();
                }
                _openCamera.WebCam.Play();
                _ifSignLogoGameObject.Hide();
                _photographButton.gameObject.Show();
            }
        }

        void ShowCurrentStudentHeadSelectImage(OneIDStudent student)
        {
            if (student.IsSignIn)
            {
                //Debug.LogError("11111111111");
                SetCurrentFinishedImageVisible(student.Name, true);
                SetConfirmAndCancelButtonVisible(false);
                _openCamera.WebCam.Stop();
                for (int i = 0; i < _showImages.childCount; i++)
                {
                    _showImages.GetChild(i).GetGameObject().Hide();
                }
                _ifSignLogoGameObject.Show();
            }
            else
            {
                //Debug.LogError("22222222222");
                SetCurrentFinishedImageVisible(student.Name, false);
                SetConfirmAndCancelButtonVisible(false);
                for (int i = 0; i < _showImages.childCount; i++)
                {
                    _showImages.GetChild(i).GetGameObject().Hide();
                }
                _ifSignLogoGameObject.Hide();
            }
        }

        void SetCurrentFinishedImageVisible(string stuName,bool isShow)
        {
            for (int i = 0; i < _finishedImagePanel.childCount; i++)
            {
                GameObject target = _finishedImagePanel.GetChild(i).GetGameObject();
                if (target.name.Equals(stuName))
                {
                    if(target.activeSelf!=isShow)
                        target.SetActive(isShow);
                }
                else
                {
                    target.Hide();
                }
            }
        }

        public void TakePhotoAni(Action callback)
        {
            OneIDSceneManager.Instance.PlayCommonSound(6);
            SpineManager.instance.DoAnimation(_takePhotoSpine, "an5", false,
                () => SpineManager.instance.DoAnimation(_takePhotoSpine, "an2", false, () =>
                {
                    callback?.Invoke();
                }));
        }

        public void ComfirmPicture()
        {
            OneIDSceneManager.Instance.PlayCommonSound(3);
            SpineManager.instance.DoAnimation(_comfirmSpine, "an4", false,
                () => SpineManager.instance.DoAnimation(_comfirmSpine, "an1", false, () =>
                {
                    _photographButton.gameObject.Hide();
                    _ifSignLogoGameObject.Show();
                    OneIDSceneManager.Instance.PlayCommonSound(4);
                    SpineManager.instance.DoAnimation(_ifSignLogoGameObject.transform.GetGameObject("Spine"), "q",
                        false); 
                    _currentStudent.IsSignIn = true;
                    //Debug.LogError("xueshegn:" +_currentStudent.Name+"  :"+_currentStudent.IsSignIn);
                    SetConfirmAndCancelButtonVisible(false);
                    OneIDSceneManager.Instance.GetStudentInfoManager.UpdateStudentStatu();
                    OneIDSceneManager.Instance.SetSwitchButtonVisible(true,OneID_ButtonShow.Next);
                }));
           
        }

        public void CancelPicture()
        {
            OneIDSceneManager.Instance.PlayCommonSound(3);
            SpineManager.instance.DoAnimation(_cancelSpine, "an6", false,
                () => SpineManager.instance.DoAnimation(_cancelSpine, "an3", false, () =>
                {
                    
                    SetConfirmAndCancelButtonVisible(false);
                    if(_openCamera.WebCam!=null)
                        _openCamera.WebCam.Play();
                }));
            
        }

        public void SetConfirmAndCancelButtonVisible(bool isShow)
        {
            _confirmButton.gameObject.SetActive(isShow);
            _cancelButton.gameObject.SetActive(isShow);
        }

        public Transform GetCurrentShowImage()
        {
            for (int i = 0; i < _showImages.childCount; i++)
            {
                Transform target = _showImages.GetChild(i);
                if (target.name.Equals(_currentStudent.Name))
                {
                    Debug.LogError("图片的名字是:"+target.name);
                    return target;
                }
            }
            Debug.LogError("找不到对应名称的图片，请检查！"+_currentStudent.Name);
            return null;
        }

        public string GetCurrentStudentName()
        {
            return _currentStudent.Name;
        }
    }
}

